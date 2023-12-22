import json
import threading
import argparse
import configparser
import paho.mqtt.client as mqtt
import json
from heartbeat import status_on_heartbeat_to_json
import time 
from datetime import datetime
import random

from ac_initialization import AcInitialization

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
IS_ON = True
INITIALIZE_PARAMETERS = True
IS_AUTOMATIC = False
ac_initialization = None
MAX_TEMPERATURE = 0
MIN_TEMPERATURE = 0
CURRENT_TEMPERATURE = 0
CURRENT_MODE = ""

def save_to_influx(command_type:str, command_value:str, user:str, success=True):
    measurement = "command"
    tags = f"device_id={args.did},device_type=AC,user={user},type={command_type},success={success}"
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

    save_to_influx("OnOff", "ON", "self")
    client.publish(SUBSCRIBER_COMMAND_TOPIC, generate_onoff_update(IS_ON))
    

def turn_off():
    global IS_ON
    print("turn_off ******************")
    IS_ON = False

    save_to_influx("OnOff", "OFF", "self")
    client.publish(SUBSCRIBER_COMMAND_TOPIC, generate_onoff_update(IS_ON))


def update_on_off_status(data):
    global IS_ON

    previous_status = IS_ON
    IS_ON = False if data['Action'] == 'OFF' else True
    if not IS_ON and previous_status:
        save_to_influx(data["Type"], data["Action"], data["Actor"])
        client.publish(SUBSCRIBER_COMMAND_TOPIC, generate_onoff_update(IS_ON))
        print("Going to sleep...")
    elif IS_ON and not previous_status:
        save_to_influx(data["Type"], data["Action"], data["Actor"])
        client.publish(SUBSCRIBER_COMMAND_TOPIC, generate_onoff_update(IS_ON))
        print("I'm back!")

def change_temperature(data):
    global IS_AUTOMATIC, CURRENT_TEMPERATURE
    if not IS_AUTOMATIC:
        temp = float(data["Value"])
        if(temp != CURRENT_TEMPERATURE):
            CURRENT_TEMPERATURE = temp
            save_to_influx(data["Type"], data["Value"], data["Actor"])
            client.publish(SUBSCRIBER_COMMAND_TOPIC, generate_update(data["Value"]))
    

def generate_update(value: str):
    message = {
        "Sender": 1,
        "DeviceId": args.did,
        "Action": "Mode",
        "Value": value,
        "Result": "SUCCESS",
        "Message": ""
    }
    return json.dumps(message)

def change_mode(data):
    global IS_AUTOMATIC, CURRENT_MODE
    if data["Value"] == "AUTOMATIC":
        IS_AUTOMATIC = True
        CURRENT_MODE = "AUTOMATIC"
        save_to_influx(data["Type"], data["Value"], data["Actor"])
        client.publish(SUBSCRIBER_COMMAND_TOPIC, generate_update(CURRENT_MODE))
    if data["Value"] == "COOLING":
        IS_AUTOMATIC = False
        CURRENT_MODE = "COOLING"
        save_to_influx(data["Type"], data["Value"], data["Actor"])
        client.publish(SUBSCRIBER_COMMAND_TOPIC, generate_update(CURRENT_MODE))
    if data["Value"] == "HEATING":
        IS_AUTOMATIC = False
        CURRENT_MODE = "HEATING"
        save_to_influx(data["Type"], data["Value"], data["Actor"])
        client.publish(SUBSCRIBER_COMMAND_TOPIC, generate_update(CURRENT_MODE))
    if data["Value"] == "VENTILATION":
        IS_AUTOMATIC = False
        CURRENT_MODE = "VENTILATION"
        save_to_influx(data["Type"], data["Value"], data["Actor"])
        client.publish(SUBSCRIBER_COMMAND_TOPIC, generate_update(CURRENT_MODE))

def handle_initialization(data):
    global ac_initialization, INITIALIZE_PARAMETERS, IS_ON, CURRENT_MODE, CURRENT_TEMPERATURE, IS_AUTOMATIC

    ac_initialization = AcInitialization(data)
    IS_ON = data["IsOn"]
    INITIALIZE_PARAMETERS = False
    CURRENT_MODE = data['CurrentMode']
    if CURRENT_MODE == "AUTOMATIC":
        IS_AUTOMATIC = True
    CURRENT_TEMPERATURE = float(data['CurrentTemperature'])

def on_message(client: mqtt.Client, userdata: any, msg: mqtt.MQTTMessage):
    print(f"Got message {msg.payload} from {msg.topic}.")
    data = json.loads(msg.payload)

    try:
        if data['Type'] == "Initialization":
            handle_initialization(data)
            return

        elif data['Sender'] != 0 or not IS_ONLINE:
            return
    except Exception as e:
        print("Unexpected command or not my message:", e)
        print("Message content:", msg.payload)
        return
    
    if INITIALIZE_PARAMETERS:
        return
    
    if data['Type'] == "OnlineOffline":
        update_online_offline_status(data)
    if data["Type"] == "OnOff":
        update_on_off_status(data)
    if data["Type"] == "ChangeMode":
        change_mode(data)
    if data["Type"] == "ChangeTemperature":
        change_temperature(data)
        
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

def publish_heartbeat():
    global INITIALIZE_PARAMETERS, IS_ONLINE
    while True:
        print("tu")
        if not INITIALIZE_PARAMETERS and not IS_ONLINE:
            IS_ONLINE = True
        client.publish(PUBLISHER_HEARTBEAT_TOPIC, status_on_heartbeat_to_json(args.did, INITIALIZE_PARAMETERS))
        time.sleep(generate_heartbeat_sleep_time())

def generate_heartbeat_sleep_time():
    import random
    return random.randint(1, 10)

def generate_data(device_id):
    global IS_AUTOMATIC, CURRENT_MODE, CURRENT_TEMPERATURE

    if IS_AUTOMATIC:
        temp = random.randint(18, 25)
        if temp > 23.5:
            CURRENT_TEMPERATURE = MIN_TEMPERATURE if MIN_TEMPERATURE > 19 else 19
        elif temp < 19.5:
            CURRENT_TEMPERATURE = MAX_TEMPERATURE if MAX_TEMPERATURE > 25 else 25
        else:
            CURRENT_TEMPERATURE = 22

    measurement = "ac"
    tags = f"device_id={device_id}"
    fields = f"mode=\"{CURRENT_MODE}\",temperature={CURRENT_TEMPERATURE}"

    influx_line_protocol = f"{measurement},{tags} {fields}"
    print(influx_line_protocol)
    return influx_line_protocol

    
def publish_data():
    global IS_ONLINE, INITIALIZE_PARAMETERS
    while True:
        if IS_ONLINE and not INITIALIZE_PARAMETERS and IS_ON:
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