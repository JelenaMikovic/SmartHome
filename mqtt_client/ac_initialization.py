class AcInitialization(object):
    def __init__(self, json_data):
        self.max_temperature = json_data.get('MaxTemperature')
        self.min_temperature = json_data.get('MinTemperature')
        self.supported_modes = json_data.get('SupportedModes')
        self.current_temperature = json_data.get('CurrentTemperature')
        self.current_mode = json_data.get('CurrentMode')