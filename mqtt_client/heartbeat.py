from datetime import datetime
import json

class Heartbeat(object):
    def __init__(self, sender: str, status: str, device_id, init=True):
        self.sender = 0 if sender == "PLATFORM" else 1
        self.status = 0 if status == "OFF" else 1
        self.device_id = device_id
        self.init = init
        # self.timestamp = datetime.now()

    def to_dict(self):
        return {
            'Sender': self.sender,
            'Status': self.status,
            'DeviceId': self.device_id,
            'InitializeParameters': self.init
        }

def primer_za_slanje_telegraf(device_id):
    measurement = "heartbeat"
    tags = f"device_id={device_id}"
    fields = "status=1"

    # Creating the Influx Line Protocol string
    influx_line_protocol = f"{measurement},{tags} {fields}"

    return influx_line_protocol

def status_on_heartbeat_to_json(device_id, init=True):
    hb = Heartbeat("DEVICE", "ON", device_id, init)
    return json.dumps(hb.to_dict())