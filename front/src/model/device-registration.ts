export interface DeviceRegistrationDTO {
    Name: string,
    IsOnline: boolean,
    PowerSource: number,
    PowerConsumption: string,
    Image: string,
    PropertyId: number
  }
  
  export interface AirConditionerRegistrationDTO extends DeviceRegistrationDTO{
    SupportedModes: number[],
    MinTemperature: number,
    MaxTemperature: number
  }

  export interface AmbientSensorRegistrationDTO extends DeviceRegistrationDTO {}

  export interface IrrigationSystemRegistrationDTO extends DeviceRegistrationDTO {}

  export interface VehicleGateRegistrationDTO extends DeviceRegistrationDTO {}

  export interface EVChargerRegistrationDTO extends DeviceRegistrationDTO {
    NumberOfPorts: number,
    ChargingPower: number,
    ChargingThreshold: number
  }

  export interface HomeBatteryRegistrationDTO extends DeviceRegistrationDTO {
    Capacity: number,
    Health: number,
    CurrentCharge: number
  }

  export interface LampRegistrationDTO extends DeviceRegistrationDTO {
    IsOn: boolean,
    BrightnessLevel: number,
    Color: number
  }

  export interface SolarPanelRegistrationDTO extends DeviceRegistrationDTO {
    Size: number,
    Efficiency: number
  }

  export interface WashingMachineRegistrationDTO extends DeviceRegistrationDTO {
    SupportedModes: number[]
  }