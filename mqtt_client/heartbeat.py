from datetime import datetime
import json

class Heartbeat(object):
    def __init__(self, sender: str, status: str, device_id):
        self.sender = 0 if sender == "PLATFORM" else 1
        self.status = 0 if status == "ON" else 1
        self.device_id = device_id
        # self.timestamp = datetime.now()

    def to_dict(self):
        return {
            'Sender': self.sender,
            'Status': self.status,
            'DeviceId': self.device_id
        }

def primer_za_slanje_telegraf(device_id):
    measurement = "heartbeat"
    tags = f"device_id={device_id}"
    fields = "status=1"

    # Creating the Influx Line Protocol string
    influx_line_protocol = f"{measurement},{tags} {fields}"

    return influx_line_protocol

def status_on_heartbeat_to_json(device_id):
    hb = Heartbeat("DEVICE", "ON", device_id)
    return json.dumps(hb.to_dict())

def status_off_heartbeat_to_json():
    hb = Heartbeat("DEVICE", "OFF")
    return json.dumps(hb.to_dict())