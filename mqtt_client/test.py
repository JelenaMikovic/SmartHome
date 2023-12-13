import requests
import json
import time

if __name__ == "__main__":
    latitude = 38.38
    longitude = -122.22
    url = f'https://api.open-meteo.com/v1/forecast?latitude={round(latitude,2)}&longitude={round(longitude,2)}&hourly=direct_normal_irradiance'
    response= requests.get(url)
    irradiances = json.loads(response.content)['hourly']['direct_normal_irradiance']
    current_hour = int(time.strftime("%Y-%m-%d %H:%M:%S", time.gmtime()).split(' ')[1].split(':')[0])
    print(current_hour)
    print(irradiances[current_hour])
    print(irradiances[15])
