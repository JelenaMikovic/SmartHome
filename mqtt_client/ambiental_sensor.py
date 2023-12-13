import paho.mqtt.client as mqtt
import argparse
import configparser
import time
import threading
import json
import random

config = configparser.ConfigParser()
config.read('config.ini')

mqtt_host = config.get('MQTT', 'host')
mqtt_port = config.getint('MQTT', 'port')
mqtt_username = config.get('MQTT', 'username')
mqtt_password = config.get('MQTT', 'password')

parser = argparse.ArgumentParser(description='Environmental Sensor Simulation...')
parser.add_argument("-did", type=int, help="device id", required=True)

args = parser.parse_args()

PUBLISHER_HEARTBEAT_TOPIC = "topic/device/" + str(args.did) + "/heartbeat"
PUBLISHER_DATA_TOPIC = "topic/device/" + str(args.did) + "/data"

IS_ONLINE = True

def on_connect(client: mqtt.Client, userdata: any, flags, result_code):
    print("Connected with result code " + str(result_code))

def on_publish(client: mqtt.Client, userdata: any, mid: any):
    try:
        published_message = userdata.get('published_message', 'No message stored')
        print(f"Sent message: {published_message}")
    except:
        print("Sent message")

def on_disconnect(client: mqtt.Client, userdata: any, on_disconnect):
    print(f"Disconnected with result code {on_disconnect}.")

client = mqtt.Client()
client.on_connect = on_connect
client.on_publish = on_publish
client.username_pw_set(mqtt_username, mqtt_password)
client.connect(mqtt_host, mqtt_port)

def publish_heartbeat():
    while True:
        client.publish(PUBLISHER_HEARTBEAT_TOPIC, generate_heartbeat_data(args.did))
        time.sleep(generate_heartbeat_sleep_time())

def generate_heartbeat_sleep_time():
    import random
    return random.randint(1, 10)

def generate_heartbeat_data(device_id):
    temperature = simulate_room_temperature()
    humidity = simulate_room_humidity()
    data = {
        "Type": "Heartbeat",
        "DeviceID": device_id,
        "Temperature": temperature,
        "Humidity": humidity
    }
    return json.dumps(data)

def simulate_room_temperature():
    return round(random.uniform(20, 25), 2)

def simulate_room_humidity():
    return round(random.uniform(40, 60), 2)

if __name__ == "__main__":
    publish_heartbeat_thread = threading.Thread(target=publish_heartbeat, daemon=True)
    publish_heartbeat_thread.start()

    try:
        client.loop_forever()
    except KeyboardInterrupt:
        client.disconnect()