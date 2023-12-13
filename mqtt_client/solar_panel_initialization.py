class SolarPanelInitialization(object):
    def __init__(self, json_data):
        self.number_of_panels = json_data['NumberOfPanels']
        self.size = json_data['Size']
        self.efficiency = json_data['Efficiency']
        self.latitude = json_data['Lat']
        self.longitude = json_data['Lng']