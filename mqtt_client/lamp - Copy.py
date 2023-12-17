import time
import pvlib
import random
from datetime import datetime, timedelta
from mqtt_client import MQQTClient, DATA_TOPIC_PREFIX
from utils import parse_args
import threading
import math
import ephem
import pytz
from suntime import Sun, SunTimeException
from timezonefinder import TimezoneFinder

PUB_SUB_TOPIC = ''

def get_sunrise_sunset(lat, lng):
    sun = Sun(lat=lat, lon=lng)

    return sun.get_local_sunrise_time().replace(tzinfo=None), sun.get_local_sunset_time().replace(tzinfo=None)

def get_seconds(time):
    return time.hour * 3600 + time.minute * 60 + time.second

def calculate_illuminance(lat, lng, dt):
    sunrise, sunset = get_sunrise_sunset(lat, lng)

    print(sunset.time())
    print(get_seconds(dt.time()), get_seconds(sunrise.time()),get_seconds(sunset.time()))
    if get_seconds(dt.time()) >= get_seconds(sunrise.time()) and get_seconds(dt.time()) <= get_seconds(sunset.time()):
        time_of_day = (dt - sunrise).total_seconds() / (sunset - sunrise).total_seconds()
        illuminance = (math.sin(2 * math.pi * time_of_day) + 1) * 30000 + 400
        
        return max(400, min(60000, illuminance))
    
    return 0

def convert_to_local_time(utc_time, local_tz):
    utc_time = utc_time.datetime()
    local_time = utc_time.replace(tzinfo=pytz.utc).astimezone(pytz.timezone(local_tz))
    return local_time

class LampSimulation(MQQTClient):
    def __init__(self, device_id, latitude, longitude, pub_sub_topic):
        super().__init__(device_id, pub_sub_topic)
        self.latitude = latitude
        self.longitude = longitude
        self.illuminance = 0  # Initial illuminance value

    def measure_illuminance(self, current_time):
        # Calculate solar position
        solar_position = pvlib.solarposition.get_solarposition(
            current_time, self.latitude, self.longitude
        )

        solar_elevation = solar_position['elevation']
        print(current_time + "    " + str(solar_elevation))

    def run_illuminance_simulation(self):
        while True:
            if self.is_online:
                # self.client.publish(PUB_SUB_TOPIC, status_on_heartbeat_to_json(args.did))
                # self.client.publish(DATA_TOPIC_PREFIX + self.device_id)
                # cr = datetime.now()
                cr = "2023-12-11 12:33:13.261756"
                
                time.sleep(60)
                if stop_event:
                    break

    def run_heartbeat_simulation(self):
        while True:
            if self.is_online:
                # self.client.publish(PUB_SUB_TOPIC, status_on_heartbeat_to_json(args.did))
                time.sleep(3)
                if stop_event:
                    break

    def run_simulation(self):
        pass


if __name__ == "__main__":
    args, PUB_SUB_TOPIC = parse_args()

    latitude = 44
    longitude = 20
    cr = "2023-12-11 16:00:13"
    # print(illuminance(latitude, longitude))
    print(calculate_illuminance(latitude, longitude, datetime.strptime(cr, '%Y-%m-%d %H:%M:%S')))

    # cr = "2023-12-11 12:33:13.261756"

    # for i in range(24):
    #     char = str(i)
    #     if i < 10:
    #         char = "0" + char
    #     date = "2023/12/12 " + char + ":33:13.261756"
    #     print(date + "  " + str(is_there_light(latitude, longitude, cr)))

    # simulation = LampSimulation(args.did, latitude, longitude, PUB_SUB_TOPIC)

    # stop_event = False
    # client = simulation.client

    # def generate_sleep_time():
    #     import random
    #     return random.randint(1, 10)

    # publish_thread = threading.Thread(target=simulation.run_simulation, daemon=True)
    # publish_thread.start()

    # try:
    #     client.loop_forever()
    # except KeyboardInterrupt:
    #         # client.publish(PUB_SUB_TOPIC, status_off_heartbeat_to_json())
    #     client.disconnect()
    #     stop_event = True