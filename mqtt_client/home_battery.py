from threading import Lock
import paho.mqtt.client as mqtt
import argparse
import configparser
import time 
import threading
import json
from heartbeat import status_on_heartbeat_to_json
import requests
from battery_initialization import BatteryInitialization

config = configparser.ConfigParser()
config.read('config.ini')

mqtt_host = config.get('MQTT', 'host')
mqtt_port = config.getint('MQTT', 'port')  
mqtt_username = config.get('MQTT', 'username')
mqtt_password = config.get('MQTT', 'password')

parser = argparse.ArgumentParser(description='Device simulation...')
parser.add_argument("-pid", type=int, help="property id", required=True)
args = parser.parse_args()

battery_lock = Lock()

PUBLISHER_DATA_TOPIC = "topic/property/" + str(args.pid) + "/data"
SUBSCRIBER_COMMAND_TOPIC = "topic/property/" + str(args.pid) + "/command"
BATTERY_INITIALIZATION_TOPIC = "topic/property/" + str(args.pid) + "/batteries_initialization"
INITIALIZE_PARAMETERS = True

batteries = []
total_capacity = 0
consumed_power = 0
generated_power = 0

def on_connect(client: mqtt.Client, userdata: any, flags, result_code):
    print("Connected with result code "+str(result_code))
    client.subscribe(SUBSCRIBER_COMMAND_TOPIC)

def on_message(client: mqtt.Client, userdata: any, msg: mqtt.MQTTMessage):
    print(f"Got message {msg.payload} from {msg.topic}.")

    global INITIALIZE_PARAMETERS, batteries, total_capacity
    global consumed_power, generated_power
    data = json.loads(msg.payload)
    print(data, INITIALIZE_PARAMETERS)
    if type(data) == list:
        for item in data:
            battery = BatteryInitialization(item)
            batteries.append(battery)
            total_capacity += battery.capacity
        #postavi ukupne parametre baterije
        INITIALIZE_PARAMETERS = False
    elif data["Type"] == "Consumption" and not INITIALIZE_PARAMETERS:
        print("hi")
        global battery_lock
        with battery_lock:
            consumed_power += data["Consumed"]
            print(consumed_power)
            generated_power += data["Generated"]

def on_publish(client: mqtt.Client, userdata: any, mid: any):
    try:
        published_message = userdata.get('published_message', 'No message stored')
        print(f"Sent message: {published_message}")
    except:
        # kako da specifiramo topic
        print(f"Sent message")

def on_disconnect(client: mqtt.Client, userdata: any, on_disconnect):
    print(f"Disconnected with result code {on_disconnect}.")

client = mqtt.Client()
client.on_connect = on_connect
client.on_message = on_message
client.on_publish = on_publish
client.username_pw_set(mqtt_username, mqtt_password)
client.connect(mqtt_host, mqtt_port)

def initialize_batteries():
    global INITIALIZE_PARAMETERS
    while True:
        client.publish(BATTERY_INITIALIZATION_TOPIC, json.dumps({'PropertyId': args.pid}))
        time.sleep(5)
        if not INITIALIZE_PARAMETERS:
            break

def publish_heartbeat():
    global INITIALIZE_PARAMETERS
    initialize_batteries()
    while True:
        for battery in batteries:
            client.publish("topic/device/" + str(battery.id) + "/heartbeat", status_on_heartbeat_to_json(battery.id))
        time.sleep(generate_heartbeat_sleep_time())

def generate_heartbeat_sleep_time():
    import random
    return random.randint(1, 10)

def publish_data():
    global INITIALIZE_PARAMETERS, consumed_power, generated_power
    while True:
        if not INITIALIZE_PARAMETERS:
            # client.publish("topic/device/" + str(42) + "/data", generate_data_for_battery_level())
            global battery_lock
            gained = None
            consumed_mqtt = 0
            with battery_lock:
                total_power_from_batteries = sum(battery.capacity * battery.currentCharge for battery in batteries)
                if consumed_power >= total_power_from_batteries + generated_power:
                    consumed_mqtt = consumed_power - total_power_from_batteries - generated_power
                    gained = False
                    for battery in batteries:
                        battery.currentCharge = 0
                elif generated_power > consumed_power and total_power_from_batteries + generated_power - consumed_power > sum(battery.capacity for battery in batteries):
                    for battery in batteries:
                        consumed_mqtt = total_power_from_batteries + generated_power - consumed_power > sum(battery.capacity for battery in batteries)
                        gained = True
                        battery.currentCharge = 100
                else:
                    for battery in batteries:
                        capacity_per_battery = battery.capacity * battery.currentCharge / total_power_from_batteries
                        consumption_per_battery = consumed_power * capacity_per_battery
                        battery.currentCharge = battery.currentCharge - consumption_per_battery / (battery.capacity)
                        print(capacity_per_battery, consumption_per_battery, battery.currentCharge)
                
            for battery in batteries:
                client.publish("topic/device/" + str(battery.id) + "/data", generate_data_for_battery_level(battery))
            
            client.publish(PUBLISHER_DATA_TOPIC, generate_data_for_consumed_power(consumed_power))
            client.publish(PUBLISHER_DATA_TOPIC, generate_data_for_power_distribution(consumed_mqtt, gained))

            with battery_lock:
                consumed_power = 0
                generated_power = 0

            time.sleep(20)

            

def generate_data_for_battery_level(battery):
    measurement = "battery_level"
    tags = f"device_id={battery.id},property_id={args.pid}"
    fields = "level=" + str(round(10, 2))

    influx_line_protocol = f"{measurement},{tags} {fields}"
    return influx_line_protocol

def generate_data_for_consumed_power(consumed_power):
    measurement = "home_battery"
    tags = f"property_id={args.pid}"
    fields = "consumed_power=" + str(round(consumed_power, 2))

    influx_line_protocol = f"{measurement},{tags} {fields}"
    return influx_line_protocol

def generate_data_for_power_distribution(consumed_mqtt, gained):
    measurement = "home_battery"
    if gained == True:
        tags = f"property_id={args.pid},gained=True"
    elif gained == False: 
        tags = f"property_id={args.pid},gained=False"
    else:
        tags = f"property_id={args.pid},gained=None"
        
    fields = "distribution=" + str(round(consumed_mqtt, 2))

    influx_line_protocol = f"{measurement},{tags} {fields}"
    return influx_line_protocol

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

    
    # b = []
    # consumed_power = 500
    # b.append(BatteryInitialization({"Capacity": 300, "CurrentCharge": 0.7}))
    # b.append(BatteryInitialization({"Capacity": 400, "CurrentCharge": 1}))
    # total_capacity = sum(battery.capacity for battery in b)
    # total_power = sum(battery.capacity * battery.currentCharge for battery in b)
    # print(total_capacity, total_power)
    # for battery in b:
    #     capacity_per_battery = battery.capacity * battery.currentCharge / total_power
    #     consumption_per_battery = consumed_power * capacity_per_battery
    #     battery.currentCharge = battery.currentCharge - consumption_per_battery / (battery.capacity)
    #     print(consumption_per_battery, battery.capacity, battery.currentCharge)


