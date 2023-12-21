class GateInitialization(object):
    def __init__(self, json_data):
        self.latitude = json_data['Lat']
        self.longitude = json_data['Lng']
        self.isOpen = json_data['IsOpen']
        self.isPrivate = json_data['IsPrivate']
        self.isOnline = False
        self.allowedPlates = json_data["AllowedLicencePlates"]