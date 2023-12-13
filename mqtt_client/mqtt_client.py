import configparser
import paho.mqtt.client as mqtt

config = configparser.ConfigParser()
config.read('config.ini')

mqtt_host = config.get('MQTT', 'host')
mqtt_port = config.getint('MQTT', 'port')  
mqtt_username = config.get('MQTT', 'username')
mqtt_password = config.get('MQTT', 'password')

def on_connect(client: mqtt.Client, userdata: any, flags, result_code):
        print("Connected with result code "+str(result_code))
        client.subscribe(userdata['subscriber_topic'])

def on_message(client: mqtt.Client, userdata: any, msg: mqtt.MQTTMessage):
    print(f"Got message {msg.payload} from topic {msg.topic} with data {userdata}.")

def on_publish(client: mqtt.Client, userdata: any, mid: any):
    try:
        topic = userdata.get('published_topic', 'No topic stored')
        published_message = userdata.get('published_message', 'No message stored')
        print(f"Sent message: {published_message} to topic: {topic}")
    except:
        print(f"Sent message (no more infos)");

def on_disconnect(client: mqtt.Client, userdata: any, on_disconnect):
    print(f"Disconnected with result code {on_disconnect}.")

class MQTTClient(object):
    def __init__(self, device_id, subscriber_topic, on_message = on_message) -> None:
        self.is_online = True
        self.device_id = device_id
        self.client = self.configure_client(subscriber_topic, on_message)
        self.device_id = device_id

    def configure_client(self, subscriber_topic, on_message):
        client = mqtt.Client(userdata={'subscriber_topic': subscriber_topic})
        client.on_connect = on_connect
        client.on_message = on_message
        client.on_publish = on_publish
        client.username_pw_set(mqtt_username, mqtt_password)
        client.connect(mqtt_host, mqtt_port)

        return client
    
def generate_sleep_time():
        import random
        return random.randint(1, 10)
        