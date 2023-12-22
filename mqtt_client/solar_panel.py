import paho.mqtt.client as mqtt
import argparse
import configparser
import time 
import threading
import json
from heartbeat import status_on_heartbeat_to_json
import requests
from solar_panel_initialization import SolarPanelInitialization

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
solar_panel_initialization = None

def save_to_influx(command_type:str, command_value:str, user:str, success=True):
    measurement = "command"
    tags = f"device_id={args.did},device_type=SOLAR_PANEL,user={user},type={command_type},success={success}"
    fields = f"value=\"{command_value}\""

    influx_line_protocol = f"{measurement},{tags} {fields}"
    print(influx_line_protocol)
    client.publish(PUBLISHER_DATA_TOPIC, influx_line_protocol)

def on_connect(client: mqtt.Client, userdata: any, flags, result_code):
    print("Connected with result code "+str(result_code))
    client.subscribe(SUBSCRIBER_COMMAND_TOPIC)

#TODO: napravi pubsub da bude data
def on_message(client: mqtt.Client, userdata: any, msg: mqtt.MQTTMessage):
    print(f"Got message {msg.payload} from {msg.topic}.")

    global IS_ONLINE, IS_ON, INITIALIZE_PARAMETERS, solar_panel_initialization
    data = json.loads(msg.payload)

    try:
        if data['Type'] == "Initialization":
            solar_panel_initialization = SolarPanelInitialization(data)
            INITIALIZE_PARAMETERS = False
            IS_ON = data["IsOn"]
        elif data['Sender'] != 0:
            return
    except Exception:
        print("Unexpected command or not my message", end=" ")
        return

    # if INITIALIZE_PARAMETERS:
    #     return
    
    if data['Type'] == 'OnOff':
        # previous_status = IS_ON
        # IS_ON = False if data['Action'] == 'OFF' else True
        # if not IS_ON and previous_status:
        #     print("Going to sleep...")
        # elif IS_ON and not previous_status:
        #     print("I'm back!")
        print("ovde")
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

def on_publish(client: mqtt.Client, userdata: any, mid: any):
    try:
        published_message = userdata.get('published_message', 'No message stored')
        print(f"Sent message: {published_message}")
    except:
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
    global IS_ON, INITIALIZE_PARAMETERS
    while True:
        print("HEARTBEAT")
        client.publish(PUBLISHER_HEARTBEAT_TOPIC, status_on_heartbeat_to_json(args.did, INITIALIZE_PARAMETERS))
        time.sleep(generate_heartbeat_sleep_time())

def generate_heartbeat_sleep_time():
    import random
    return random.randint(1, 10)


def publish_data():
    global IS_ON, INITIALIZE_PARAMETERS, solar_panel_initialization
    while True:
        # client.publish(PUBLISHER_DATA_TOPIC, generate_data(args.did, solar_panel_initialization.propertyId))
        # time.sleep(10)
        if IS_ON and not INITIALIZE_PARAMETERS:
            # print(simulate_solar_energy_production())
            print("DATA")
            client.publish(PUBLISHER_DATA_TOPIC, generate_data(args.did, solar_panel_initialization.propertyId))
            time.sleep(55)

def generate_data(device_id, property_id):
    measurement = "solar_energy"
    tags = f"device_id={device_id},property_id={property_id}"
    fields = "energy=" + str(simulate_solar_energy_production())
    # fields = "energy=" + str(round(10, 0))
    influx_line_protocol = f"{measurement},{tags} {fields}"
    return influx_line_protocol

def get_irradiances():
    endpoint = f'https://api.open-meteo.com/v1/forecast?latitude={round(solar_panel_initialization.latitude,2)}&longitude={round(solar_panel_initialization.longitude,2)}&minutely_15=direct_normal_irradiance'
    response = requests.get(endpoint)
    irradiances = json.loads(response.content)['minutely_15']['direct_normal_irradiance']
    current_hours = int(time.strftime("%Y-%m-%d %H:%M:%S", time.gmtime()).split(' ')[1].split(':')[0])
    current_minutes = int(time.strftime("%Y-%m-%d %H:%M:%S", time.gmtime()).split(' ')[1].split(':')[1])
    if current_minutes <= 15:
        return irradiances[current_hours*4 + 1]
    elif current_minutes <= 30:
        return irradiances[current_hours*4 + 2]
    elif current_minutes <= 45:
        return irradiances[current_hours*4 + 3]
    return irradiances[(current_hours+1)*4]

def simulate_solar_energy_production():
    return solar_panel_initialization.efficiency * solar_panel_initialization.size * solar_panel_initialization.number_of_panels * get_irradiances() / (15*60)

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


