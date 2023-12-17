class LampInitialization(object):
    def __init__(self, json_data):
        self.latitude = json_data['Lat']
        self.longitude = json_data['Lng']