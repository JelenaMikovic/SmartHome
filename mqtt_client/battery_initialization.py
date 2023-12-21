class BatteryInitialization(object):
    def __init__(self, json_data):
        self.id = json_data['Id']
        self.capacity = json_data['Capacity']
        self.currentCharge = json_data['CurrentCharge']