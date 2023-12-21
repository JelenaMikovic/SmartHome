from suntime import Sun
import math
import threading
import argparse
import configparser
import paho.mqtt.client as mqtt
import json
from heartbeat import status_on_heartbeat_to_json
import time 
import random

from ambiental_sensor_initialization import AmbientalSensorInitialization

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
INITIALIZE_PARAMETERS = True
sensor_initialization = None

def save_to_influx(command_type:str, command_value:str, user:str, success=True):
    measurement = "command"
    tags = f"device_id={args.did},device_type=AMBIENT_SENSOR,user={user},type={command_type},success={success}"
    fields = f"value=\"{command_value}\""

    influx_line_protocol = f"{measurement},{tags} {fields}"
    print(influx_line_protocol)
    client.publish(PUBLISHER_DATA_TOPIC, influx_line_protocol)

def on_connect(client: mqtt.Client, userdata: any, flags, result_code):
    print("Connected with result code "+str(result_code))
    client.subscribe(SUBSCRIBER_COMMAND_TOPIC)

def update_online_offline_status(data):
    global IS_ONLINE

    if data["Action"] == "offline" and IS_ONLINE:
        IS_ONLINE = False
        print("Device is offline!")

def handle_initialization(data):
    global sensor_initialization, INITIALIZE_PARAMETERS

    sensor_initialization = AmbientalSensorInitialization(data)
    INITIALIZE_PARAMETERS = False

    print(INITIALIZE_PARAMETERS)

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
        print("Unexpected command or not my message", end=" ")
        print(data)
        return
    
    if INITIALIZE_PARAMETERS:
        return
    
    if data['Type'] == "OnlineOffline":
        update_online_offline_status(data)

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
        print("tu")
        if not IS_ONLINE:
            print(IS_ONLINE)
            IS_ONLINE = True
        client.publish(PUBLISHER_HEARTBEAT_TOPIC, status_on_heartbeat_to_json(args.did, INITIALIZE_PARAMETERS))
        time.sleep(generate_heartbeat_sleep_time())

def generate_heartbeat_sleep_time():
    import random
    return random.randint(1, 10)

    return 0

def generate_data(device_id):
    measurement = "ambiental_sensor"
    tags = f"device_id={device_id}"
    fields = "humidity=" + str(simulate_room_humidity()) + ",temperature=" + str(simulate_room_temperature())

    influx_line_protocol = f"{measurement},{tags} {fields}"
    print(influx_line_protocol)
    return influx_line_protocol


def simulate_room_temperature():
    return round(random.uniform(20, 25), 2)

def simulate_room_humidity():
    return round(random.uniform(40, 60), 2)
    
def publish_data():
    global IS_ONLINE, INITIALIZE_PARAMETERS
    while True:
        if IS_ONLINE and not INITIALIZE_PARAMETERS:
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