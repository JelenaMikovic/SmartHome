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

from gate_initialization import GateInitialization

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


INITIALIZE_PARAMETERS = True
gate_initialization = None

def save_to_influx(command_type:str, command_value:str, user:str, success=True):
    measurement = "command"
    tags = f"device_id={args.did},device_type=GATE,user={user},type={command_type},success={success}"
    fields = f"value=\"{command_value}\""

    influx_line_protocol = f"{measurement},{tags} {fields}"
    print(influx_line_protocol)
    client.publish(PUBLISHER_DATA_TOPIC, influx_line_protocol)

def on_connect(client: mqtt.Client, userdata: any, flags, result_code):
    print("Connected with result code "+str(result_code))
    client.subscribe(SUBSCRIBER_COMMAND_TOPIC)

def update_online_offline_status(data):
    global IS_ONLINE

    if data["Action"] == "offline" and gate_initialization.isOnline:
        IS_ONLINE = False
        print("Device is offline!")

def generate_gate_command_update(action: str, value: bool):
    message = {
        "Sender": 1,
        "DeviceId": args.did,
        "Action": action,
        "Value": value,
        "Result": "SUCCESS",
        "Message": ""
    }
    return json.dumps(message)

def update_gate_open_status(data):
    global gate_initialization

    open_update = True if data["Value"] == "true" else False

    if (open_update != gate_initialization.isOpen):
        gate_initialization.isOpen = open_update
        save_to_influx(data["Type"], data["Value"], data["Actor"])
        client.publish(SUBSCRIBER_COMMAND_TOPIC, generate_gate_command_update(data["Type"], open_update))
        print("Gate open: " + str(open_update) + "******************")
        if open_update:
            gate_opened_event.set()

def update_gate_private_status(data):
    global gate_initialization

    private_update = True if data["Value"] == "true" else False

    if (private_update != gate_initialization.isPrivate):
        gate_initialization.isPrivate = private_update
        save_to_influx(data["Type"], data["Value"], data["Actor"])
        client.publish(SUBSCRIBER_COMMAND_TOPIC, generate_gate_command_update(data["Type"], private_update))
        print("Gate private: " + str(private_update) + "******************")

def update_gate_allowed_plates(data, is_add):
    global gate_initialization

    if is_add:
        gate_initialization.allowedPlates.append(data["Value"])
    else:
        gate_initialization.allowedPlates.remove(data["Value"])

    save_to_influx(data["Type"], data["Value"], data["Actor"])
    client.publish(SUBSCRIBER_COMMAND_TOPIC, generate_gate_command_update(data["Type"], data["Value"]))
    
    prefix = "Added" if is_add else "Removed"
    print(prefix + " plate: " + data["Value"] + "******************")
        


def handle_initialization(data):
    global gate_initialization, INITIALIZE_PARAMETERS

    gate_initialization = GateInitialization(data)
    INITIALIZE_PARAMETERS = False

def on_message(client: mqtt.Client, userdata: any, msg: mqtt.MQTTMessage):
    print(f"Got message {msg.payload} from {msg.topic}.")
    data = json.loads(msg.payload)

    try:
        if data['Type'] == "Initialization":
            handle_initialization(data)
            return

        elif data['Sender'] != 0 or not gate_initialization.isOnline:
            return
    except Exception:
        print("Unexpected command or not my message", end=" ")
        print(data)
        return
    
    if INITIALIZE_PARAMETERS:
        return
    
    if data['Type'] == "OnlineOffline":
        update_online_offline_status(data)
    if data["Type"] == "open":
        update_gate_open_status(data)
    if data["Type"] == "private":
        update_gate_private_status(data)
    if data["Type"] == "addplate":
        update_gate_allowed_plates(data, True)
    if data["Type"] == "removeplate":
        update_gate_allowed_plates(data, False)
        

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
    global INITIALIZE_PARAMETERS, gate_initialization
    while True:
        print("tu")
        if not INITIALIZE_PARAMETERS and not gate_initialization.isOnline:
            gate_initialization.isOnline = True
        client.publish(PUBLISHER_HEARTBEAT_TOPIC, status_on_heartbeat_to_json(args.did, INITIALIZE_PARAMETERS))
        time.sleep(generate_heartbeat_sleep_time())

def generate_heartbeat_sleep_time():
    import random
    return random.randint(1, 10)

vehicle_arrived_event = threading.Event()
gate_opened_event = threading.Event()
vehicles_waiting = []
vehicles_inside = []
vehicle_lock = threading.Lock()

def generate_data():
    if vehicles_inside:
        direction = "out" if random.random() > 0.4 else "in"
    else:
        direction = "in"

    if (direction == "out"):
        plate_number = random.choice(vehicles_inside)
    else:
        while True:
            vip_plates = True if random.random() > 0.3 else False
            if vip_plates:
                plate_number = random.choice(gate_initialization.allowedPlates)
            else:
                plate_number = f"ABC{random.randint(100, 999)}"

            if plate_number not in vehicles_inside:
                break

    return (plate_number, direction)

def generate_plate_update(plate):
    measurement = "plate_update_socket"
    tags = f"deviceId={args.did}"
    fields = "plate=" + plate

    influx_line_protocol = f"{measurement},{tags} {fields}"

    print(influx_line_protocol)
    return influx_line_protocol

def publish_data():
    global gate_initialization, INITIALIZE_PARAMETERS
    while True:
        if not INITIALIZE_PARAMETERS:
            vehicle_arrived_event.wait()
            if vehicles_waiting:
                with vehicle_lock:
                    plate_number, direction = vehicles_waiting.pop(0)
                    client.publish(PUBLISHER_DATA_TOPIC, generate_plate_update(plate_number))

                    if direction == "in":
                        if gate_initialization.isPrivate:
                            if plate_number not in gate_initialization.allowedPlates and not gate_initialization.isOpen:
                                if gate_opened_event.wait(timeout=30):
                                    vehicles_inside.append(plate_number)
                                    gate_opened_event.clear()
                                else:
                                    print(f"Plate number is not in the allowed plates registry for private regime: {plate_number}, denied")
                                    continue
                            else:
                                vehicles_inside.append(plate_number)
                    else:
                        vehicles_inside.remove(plate_number)
                print(f"Gate opens for vehicle with plate number: {plate_number}, going {direction}")
                save_to_influx("open", "true", "self")
                time.sleep(3)
                print("Gate closes")
                save_to_influx("open", "false", "self")
                client.publish(PUBLISHER_DATA_TOPIC, generate_plate_update("none"))

            vehicle_arrived_event.clear()
        time.sleep(1)

def vehicle_simulation():
    while True:
        time.sleep(random.randint(5, 15))  # Simulate random arrival time for vehicles
        plate_number = generate_data()
        print(f"Vehicle arrived with plate number: {plate_number}")
        with vehicle_lock:
            vehicles_waiting.append(plate_number)
            vehicle_arrived_event.set()

if __name__ == "__main__":
    publish_heartbeat_thread = threading.Thread(target=publish_heartbeat, daemon=True)
    publish_heartbeat_thread.start()

    publish_data_thread = threading.Thread(target=publish_data, daemon=True)
    publish_data_thread.start()

    vehicle_simulation_thread = threading.Thread(target=vehicle_simulation, daemon=True)
    vehicle_simulation_thread.start()

    try:
        client.loop_forever()
    except KeyboardInterrupt:
        client.disconnect()
        stop_event = True