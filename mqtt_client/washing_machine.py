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

from washing_machine_initialization import WashingMachineInitialization

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
IS_ON = False
washing_machine_initialization = None
CURRENT_MODE = ""

def save_to_influx(command_type:str, command_value:str, user:str, success=True):
    measurement = "command"
    tags = f"device_id={args.did},device_type=washing_machine,user={user},type={command_type},success={success}"
    fields = f"value=\"{command_value}\""

    influx_line_protocol = f"{measurement},{tags} {fields}"
    print(influx_line_protocol)
    client.publish(PUBLISHER_DATA_TOPIC, influx_line_protocol)

def on_connect(client: mqtt.Client, userdata: any, flags, result_code):
    print("Connected with result code "+str(result_code))
    client.subscribe(SUBSCRIBER_COMMAND_TOPIC)

def update_online_offline_status(data):
    global IS_ONLINE

    if data["action"] == "offline" and IS_ONLINE:
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

def reset_mode():
        global CURRENT_MODE, IS_ON
        IS_ON = False
        client.publish(SUBSCRIBER_COMMAND_TOPIC, generate_onoff_update(IS_ON))

def change_mode(data):
    global CURRENT_MODE, IS_ON
    if IS_ON == False:
        if CURRENT_MODE != data["Value"]:
            CURRENT_MODE = data["Value"]
            client.publish(SUBSCRIBER_COMMAND_TOPIC, generate_update(CURRENT_MODE))
        IS_ON = True
        save_to_influx(data["Type"], data["Value"], data["Actor"])
        client.publish(SUBSCRIBER_COMMAND_TOPIC, generate_onoff_update(IS_ON))
        timer = threading.Timer(60, reset_mode)
        timer.start()

def handle_initialization(data):
    global washing_machine_initialization, INITIALIZE_PARAMETERS, CURRENT_MODE, IS_ON

    washing_machine_initialization = WashingMachineInitialization(data)
    INITIALIZE_PARAMETERS = False
    IS_ON = data["IsOn"]
    CURRENT_MODE = data['CurrentMode']

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
    if data["Type"] == "ChangeMode":
        change_mode(data)
    if data["Type"] == "OnOff":
        update_on_off_status(data)

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
        if not INITIALIZE_PARAMETERS and not IS_ONLINE:
            IS_ONLINE = True
        client.publish(PUBLISHER_HEARTBEAT_TOPIC, status_on_heartbeat_to_json(args.did, INITIALIZE_PARAMETERS))
        time.sleep(generate_heartbeat_sleep_time())

def generate_heartbeat_sleep_time():
    import random
    return random.randint(1, 10)

def generate_data(device_id):
    global CURRENT_MODE

    measurement = "washing_machine"
    tags = f"device_id={device_id}"
    fields = f"mode={CURRENT_MODE}"

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