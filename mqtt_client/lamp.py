from suntime import Sun
import math
import threading
import argparse
import configparser
import paho.mqtt.client as mqtt
import json
from heartbeat import status_on_heartbeat_to_json
import time 
from datetime import datetime

from lamp_initialization import LampInitialization

config = configparser.ConfigParser()
config.read('config.ini')

mqtt_host = config.get('MQTT', 'host')
mqtt_port = config.getint('MQTT', 'port')  
mqtt_username = config.get('MQTT', 'username')
mqtt_password = config.get('MQTT', 'password')

parser = argparse.ArgumentParser(description='Device simulation...')
parser.add_argument("-did", type=int, help="device id", required=True)

args = parser.parse_args()

PUBLISHER_HEARTBEAT_TOPIC = "topic/device/" + str(args.did) + "/heartbeat"
PUBLISHER_DATA_TOPIC = "topic/device/" + str(args.did) + "/data"
SUBSCRIBER_COMMAND_TOPIC = "topic/device/" + str(args.did) + "/command"

IS_ONLINE = True
IS_ON = False
INITIALIZE_PARAMETERS = True
IS_AUTOMATIC = False
lamp_initialization = None

def on_connect(client: mqtt.Client, userdata: any, flags, result_code):
    print("Connected with result code "+str(result_code))
    client.subscribe(SUBSCRIBER_COMMAND_TOPIC)

def update_on_off_status(data):
    global IS_ON

    previous_status = IS_ON
    IS_ON = False if data['Action'] == 'OFF' else True
    if not IS_ON and previous_status:
        print("Going to sleep...")
    elif IS_ON and not previous_status:
        print("I'm back!")

def change_regime(data):
    global IS_AUTOMATIC
    if IS_AUTOMATIC and data["Value"] == "MANUAL":
        IS_AUTOMATIC = False
        print("Automatic OFF! ****************************")
    elif not IS_AUTOMATIC and data["Value"] == "AUTOMATIC":
        IS_AUTOMATIC = True
        print("Automatic ON! ****************************")

def update_online_offline_status(data):
    global IS_ONLINE

    if data["Action"] == "offline" and IS_ONLINE:
        IS_ONLINE = False
        print("Device is offline!")

def handle_initialization(data):
    global lamp_initialization, IS_AUTOMATIC, INITIALIZE_PARAMETERS

    lamp_initialization = LampInitialization(data)
    IS_AUTOMATIC = data['Regime'] == "AUTOMATIC"
    INITIALIZE_PARAMETERS = False

def on_message(client: mqtt.Client, userdata: any, msg: mqtt.MQTTMessage):
    global IS_ONLINE

    print(f"Got message {msg.payload} from {msg.topic}.")
    data = json.loads(msg.payload)

    try:
        if data['Type'] == "Initialization":
            handle_initialization(data)

        elif data['Sender'] != 0 or not IS_ONLINE:
            return
    except Exception:
        print("Unexpected command", end=" ")
        print(data)
        return
    
    if data['Type'] == "OnlineOffline":
        update_online_offline_status(data)
    elif data['Type'] == 'OnOff':
        update_on_off_status(data)
    elif data["Type"] == "Regime":
        change_regime(data)
        

def on_publish(client: mqtt.Client, userdata: any, mid: any):
    print(f"Sent message")
    
def on_disconnect(client: mqtt.Client, userdata: any, on_disconnect):
    print(f"Disconnected with result code {on_disconnect}.")

client = mqtt.Client()
client.on_connect = on_connect
client.on_message = on_message
client.on_publish = on_publish
client.username_pw_set(mqtt_username, mqtt_password)
client.connect(mqtt_host, mqtt_port)

#TODO: napraviti onu foru da nekad ode offline
def publish_heartbeat():
    global IS_ONLINE, INITIALIZE_PARAMETERS
    while True:
        if not IS_ONLINE:
            IS_ONLINE = True
        client.publish(PUBLISHER_HEARTBEAT_TOPIC, status_on_heartbeat_to_json(args.did, INITIALIZE_PARAMETERS))
        time.sleep(generate_heartbeat_sleep_time())

def generate_heartbeat_sleep_time():
    import random
    return random.randint(1, 10)

def get_sunrise_sunset(lat, lng):
    sun = Sun(lat=lat, lon=lng)

    return sun.get_local_sunrise_time().replace(tzinfo=None), sun.get_local_sunset_time().replace(tzinfo=None)

def get_seconds(time):
    return time.hour * 3600 + time.minute * 60 + time.second

def calculate_illuminance():
    sunrise, sunset = get_sunrise_sunset(lamp_initialization.latitude, lamp_initialization.longitude)
    dt = datetime.now()

    print(sunset.time())
    print(get_seconds(dt.time()), get_seconds(sunrise.time()),get_seconds(sunset.time()))
    if get_seconds(dt.time()) >= get_seconds(sunrise.time()) and get_seconds(dt.time()) <= get_seconds(sunset.time()):
        time_of_day = (dt - sunrise).total_seconds() / (sunset - sunrise).total_seconds()
        illuminance = (math.sin(2 * math.pi * time_of_day) + 1) * 30000 + 400
        
        return max(400, min(60000, illuminance))
    
    return 0

def generate_onoff_update(value: bool):
    message = {
        "Sender": 1,
        "DeviceId": args.did,
        "Action": "OnOff",
        "Value": "ON" if value else "OFF",
        "Result": "SUCCESS",
        "Message": ""
    }
    return json.dumps(message)

def turn_on():
    global IS_ON
    print("turn_on *******************")
    IS_ON = True

    # client.publish(SUBSCRIBER_COMMAND_TOPIC, generate_onoff_update(IS_ON))

def turn_off():
    global IS_ON
    print("turn_off ******************")
    IS_ON = False

    # client.publish(SUBSCRIBER_COMMAND_TOPIC, generate_onoff_update(IS_ON))

def generate_data(device_id):
    global IS_AUTOMATIC

    illuminance = round(calculate_illuminance(), 0)

    print(IS_AUTOMATIC)
    print(str(illuminance) + "--------------------")
    if IS_AUTOMATIC:
        print(str(illuminance) + "*****************")
        if IS_ON and illuminance > 40000:
            turn_off()
        elif not IS_ON and illuminance < 20000:
            turn_on()


    measurement = "illuminance"
    tags = f"device_id={device_id}"
    fields = "illuminance=" + str(illuminance)

    influx_line_protocol = f"{measurement},{tags} {fields}"

    print(influx_line_protocol)
    return influx_line_protocol

def publish_data():
    global IS_ONLINE, INITIALIZE_PARAMETERS
    while True:
        if IS_ONLINE and not INITIALIZE_PARAMETERS:
            print(calculate_illuminance())
            client.publish(PUBLISHER_DATA_TOPIC, generate_data(args.did))
            time.sleep(10)

if __name__ == "__main__":
    publish_heartbeat_thread = threading.Thread(target=publish_heartbeat, daemon=True)
    publish_heartbeat_thread.start()

    publish_data_thread = threading.Thread(target=publish_data, daemon=True)
    publish_data_thread.start()
    try:
        client.loop_forever()
    except KeyboardInterrupt:
        client.disconnect()
        stop_event = True