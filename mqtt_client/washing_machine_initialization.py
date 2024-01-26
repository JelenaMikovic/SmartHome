class WashingMachineInitialization(object):
    def __init__(self, json_data):
        self.supported_modes = json_data.get('SupportedModes')
        self.is_on = json_data.get('IsOn')
        self.current_mode = json_data.get('CurrentMode')