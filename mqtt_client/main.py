import paho.mqtt.client as mqtt
import argparse
import configparser
import time 
import threading
import json
from heartbeat import status_off_heartbeat_to_json, status_on_heartbeat_to_json

config = configparser.ConfigParser()
config.read('config.ini')

mqtt_host = config.get('MQTT', 'host')
mqtt_port = config.getint('MQTT', 'port')  
mqtt_username = config.get('MQTT', 'username')
mqtt_password = config.get('MQTT', 'password')

parser = argparse.ArgumentParser(description='Device simulation...')
parser.add_argument("-tt", type=str, help="topic type (status/data)")
# parser.add_argument("-pid", type=int, help="property id", required=True)
parser.add_argument("-did", type=int, help="device id", required=True)

args = parser.parse_args()

# PUB_SUB_TOPIC = "property/" + str(args.pid) + "/device" + str(args.did) + "/" + args.tt
PUB_SUB_TOPIC = "topic/device/" + str(args.did) + "/" + args.tt
IS_ONLINE = True

def on_connect(client: mqtt.Client, userdata: any, flags, result_code):
    print("Connected with result code "+str(result_code))
    client.subscribe(PUB_SUB_TOPIC)

def on_message(client: mqtt.Client, userdata: any, msg: mqtt.MQTTMessage):
    global IS_ONLINE
    data = json.loads(msg.payload)
    # data['sender'] == 0 is the same as data['sender'] == Sender.PLATFORM
    if data['Sender'] == 0:
        print(f"Got message {msg.payload} from topic {msg.topic} with data {userdata}.")
        if data["Status"] == 0 and not IS_ONLINE:
            IS_ONLINE = True
            print("Device is online!")
        elif data["Status"] == 1 and IS_ONLINE:
            IS_ONLINE = False

def on_publish(client: mqtt.Client, userdata: any, mid: any):
    try:
        published_message = userdata.get('published_message', 'No message stored')
        print(f"Sent message: {published_message}")
    except:
        print(f"Sent message to " + PUB_SUB_TOPIC)

def on_disconnect(client: mqtt.Client, userdata: any, on_disconnect):
    print(f"Disconnected with result code {on_disconnect}.")

client = mqtt.Client()
client.on_connect = on_connect
client.on_message = on_message
client.on_publish = on_publish
client.username_pw_set(mqtt_username, mqtt_password)
client.connect(mqtt_host, mqtt_port)

stop_event = False

def publish():
    global IS_ONLINE
    while True:
        if IS_ONLINE:
            client.publish(PUB_SUB_TOPIC, status_on_heartbeat_to_json(args.did))
            time.sleep(3)
            if stop_event:
                break

def generate_sleep_time():
    import random
    return random.randint(1, 10)

if __name__ == "__main__":
    publish_thread = threading.Thread(target=publish, daemon=True)
    publish_thread.start()

    try:
        client.loop_forever()
    except KeyboardInterrupt:
        # client.publish(PUB_SUB_TOPIC, status_off_heartbeat_to_json())
        client.disconnect()
        stop_event = True
