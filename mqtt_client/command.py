import json

class Command(object):
    def __init__(self, type: str, action:str, *args):
        self.type = type
        self.action = action
        self.args = args

    