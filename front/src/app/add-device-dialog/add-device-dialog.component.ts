import { PropertyDTO, PropertyService } from './../../services/property.service';
import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { areaValidator, nameValidator } from '../validators/property-validators';
import { markFormControlsTouched } from '../validators/formGroupValidators';
import {batteryCapacityValidator, evChargingPowerValidator, evNumberOfPortsValidator, maxTempValidator, minTempValidator, percentageValidation, powerConsumptionValidator } from '../validators/device-validators';
import { ConfirmValidParentMatcher, minMaxValidator } from '../validators/min-max-validators';
import { AirConditionerRegistrationDTO, AmbientSensorRegistrationDTO, EVChargerRegistrationDTO, HomeBatteryRegistrationDTO, IrrigationSystemRegistrationDTO, LampRegistrationDTO, SolarPanelRegistrationDTO, VehicleGateRegistrationDTO, WashingMachineRegistrationDTO } from 'src/model/device-registration';
import { DeviceRegistrationService } from 'src/services/device-registration.service';

@Component({
  selector: 'app-add-device-dialog',
  templateUrl: './add-device-dialog.component.html',
  styleUrls: ['./add-device-dialog.component.css']
})
export class AddDeviceDialogComponent implements OnInit {

  // filePath: string = "";
  file: File = {} as File;
  deviceType: string = "";
  colorsDD = [
    'White', 
    'Yellow', 
    'Green', 
    'Blue',
    'Red'
  ]

  confirmValidParentMatcher = new ConfirmValidParentMatcher();

  addDeviceForm = new FormGroup({
    name: new FormControl('', [Validators.required, nameValidator]),
    consumption: new FormControl('', [Validators.required, powerConsumptionValidator]),

    minTemp: new FormControl(0, [minTempValidator]),
    maxTemp: new FormControl(30, [maxTempValidator]),

    acCooling: new FormControl(true),
    acHeating: new FormControl(false),
    acAutomatic: new FormControl(false),
    acVentilation: new FormControl(false),

    wmCotton: new FormControl(false),
    wmSportswear: new FormControl(false),
    wmIronDry: new FormControl(false),
    wmTowels: new FormControl(false),
    wmMix: new FormControl(false),
    wmWool: new FormControl(false),
    wmSuper: new FormControl(false),
    wmCupboard: new FormControl(false),

    panelSize: new FormControl(10, [areaValidator]),
    panelEfficiency: new FormControl(100, [percentageValidation]),

    batteryCapacity: new FormControl(10, [batteryCapacityValidator]),
    batteryHealth: new FormControl(100, [percentageValidation]),
    evChargingPower: new FormControl(15, [evChargingPowerValidator]),
    evNumberOfPorts: new FormControl(5, [evNumberOfPortsValidator]),
    evChargingThreshold: new FormControl(100, [percentageValidation])
  }, [minMaxValidator("minTemp", "maxTemp")])

  deviceImgPath = "../../assets/logo.png";

  constructor(
    public dialogRef: MatDialogRef<AddDeviceDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private snackBar: MatSnackBar,
    private propertyService: PropertyService,
    private deviceRegistration: DeviceRegistrationService) { 
    this.deviceType = this.data.deviceType;
  }

  ngOnInit(): void {
    markFormControlsTouched(this.addDeviceForm);
  }

  setDeviceTypeInTitle(): string{
    if (this.deviceType == "AmbientSensor")
      return "Ambient sensor";
    if (this.deviceType == "AC")
      return "AC";
    if (this.deviceType == "WashingMachine")
      return "Washing machine";
    if (this.deviceType == "Lamp")
      return "Lamp";
    if (this.deviceType == "SolarPanel")
      return "Solar panel";
    if (this.deviceType == "VehicleGate")
      return "Vehicle gate";
    if (this.deviceType == "HomeBattery")
      return "Home battery";
    if (this.deviceType == "IrrigationSystem")
      return "Irrigation system";  
    return "EV charger";
  }

  addDevice(){}

  onFileSelect(event: any) {
    event.preventDefault();
    if (event.target.files){
      var reader = new FileReader();
      this.file = event.target.files[0];
      reader.readAsDataURL(this.file);
      reader.onload=(e: any)=>{
        event.preventDefault();
        // this.filePath = reader.result as string;
        this.deviceImgPath = reader.result as string;
      }
    }
  }

  registerAmbientSensor(){
    let device : AmbientSensorRegistrationDTO = {
      Name: this.addDeviceForm.value.name!,
      IsOnline: false,
      PowerSource: 0,
      PowerConsumption: this.addDeviceForm.value.consumption!,
      Image: this.deviceImgPath,
      PropertyId: 1
    }
    this.deviceRegistration.registerAmbientSensor(device).subscribe({
      next: (res: any) => {
      this.snackBar.open(res.message, "", {
          duration: 2700, panelClass: ['snack-bar-success']
      });
      //   this.router.navigate(['verification-choice']);
      console.log(res);
      },
      error: (err: any) => {
        this.snackBar.open("Error occured on server!", "", {
          duration: 2700, panelClass: ['snack-bar-server-error']
       });
        console.log(err);
      }
    });
  }

  getSupportedACModes(){
    let modes : number[] = [];
    if (this.addDeviceForm.value.acCooling) modes.push(0);
    if (this.addDeviceForm.value.acHeating) modes.push(1);
    if (this.addDeviceForm.value.acAutomatic) modes.push(2);
    if (this.addDeviceForm.value.acVentilation) modes.push(3);
    return modes;
  }

  registerAC(){
    let device : AirConditionerRegistrationDTO = {
      Name: this.addDeviceForm.value.name!,
      IsOnline: false,
      PowerSource: 0,
      PowerConsumption: this.addDeviceForm.value.consumption!,
      Image: this.deviceImgPath,
      SupportedModes: this.getSupportedACModes(),
      MinTemperature: this.addDeviceForm.value.minTemp!,
      MaxTemperature: this.addDeviceForm.value.maxTemp!,
      PropertyId: 1
    }
    this.deviceRegistration.registerAC(device).subscribe({
      next: (res: any) => {
        this.snackBar.open(res.message, "", {
          duration: 2700, panelClass: ['snack-bar-success']
      });
      //   this.router.navigate(['verification-choice']);
      console.log(res);
      },
      error: (err: any) => {
        this.snackBar.open("Error occured on server!", "", {
          duration: 2700, panelClass: ['snack-bar-server-error']
       });
        console.log(err)
      }
    });
  }

  getSupportedWachineMachineModes(){
    let modes : number[] = [];
    if (this.addDeviceForm.value.wmCotton) modes.push(0);
    if (this.addDeviceForm.value.wmSportswear) modes.push(1);
    if (this.addDeviceForm.value.wmIronDry) modes.push(2);
    if (this.addDeviceForm.value.wmTowels) modes.push(3);
    if (this.addDeviceForm.value.wmMix) modes.push(4);
    if (this.addDeviceForm.value.wmWool) modes.push(5);
    if (this.addDeviceForm.value.wmSuper) modes.push(6);
    if (this.addDeviceForm.value.wmCupboard) modes.push(7);
    return modes;
  }

  registerWashingMachine(){
    console.log(this.deviceImgPath);
    let device : WashingMachineRegistrationDTO = {
      Name: this.addDeviceForm.value.name!,
      IsOnline: false,
      PowerSource: 0,
      PowerConsumption: this.addDeviceForm.value.consumption!,
      Image: this.deviceImgPath,
      SupportedModes: this.getSupportedWachineMachineModes(),
      PropertyId: 1
    }
    this.deviceRegistration.registerWashingMachine(device).subscribe({
      next: (res: any) => {
      this.snackBar.open(res.message, "", {
          duration: 2700, panelClass: ['snack-bar-success']
      });
      //   this.router.navigate(['verification-choice']);
      console.log(res);
      },
      error: (err: any) => {
        this.snackBar.open("Error occured on server!", "", {
          duration: 2700, panelClass: ['snack-bar-server-error']
       });
        console.log(err)
      }
    });
  }

  selectedLampColor: string = this.colorsDD[0];

  getLampColor(): number {
    if (this.selectedLampColor == "White") return 0;
    else if (this.selectedLampColor == "Yellow") return 1;
    else if (this.selectedLampColor == "Green") return 2;
    else if (this.selectedLampColor == "Blue") return 3;
    return 4;
  }

  registerLamp(){
    let device : LampRegistrationDTO = {
      Name: this.addDeviceForm.value.name!,
      IsOnline: false,
      PowerSource: 0,
      PowerConsumption: this.addDeviceForm.value.consumption!,
      Image: this.deviceImgPath,
      IsOn: false,
      BrightnessLevel: 80,
      Color: this.getLampColor(),
      PropertyId: 1
    }
    this.deviceRegistration.registerLamp(device).subscribe({
      next: (res: any) => {
        this.snackBar.open(res.message, "", {
          duration: 2700, panelClass: ['snack-bar-success']
      });
      //   this.router.navigate(['verification-choice']);
      console.log(res);
      },
      error: (err: any) => {
        this.snackBar.open("Error occured on server!", "", {
          duration: 2700, panelClass: ['snack-bar-server-error']
       });
        console.log(err)
      }
    });
  }

  registerSolarPanel(){
    let device : SolarPanelRegistrationDTO = {
      Name: this.addDeviceForm.value.name!,
      IsOnline: false,
      PowerSource: 0,
      PowerConsumption: this.addDeviceForm.value.consumption!,
      Image: this.deviceImgPath,
      Size: this.addDeviceForm.value.panelSize!,
      Efficiency: this.addDeviceForm.value.panelEfficiency!,
      PropertyId: 1
    }
    this.deviceRegistration.registerSolarPanel(device).subscribe({
      next: (res: any) => {
      this.snackBar.open(res.message, "", {
          duration: 2700, panelClass: ['snack-bar-success']
      });
      //   this.router.navigate(['verification-choice']);
      console.log(res);
      },
      error: (err: any) => {
        this.snackBar.open("Error occured on server!", "", {
          duration: 2700, panelClass: ['snack-bar-server-error']
       });
        console.log(err)
      }
    });
  }

  registerVehicleGate(){
    let device : VehicleGateRegistrationDTO = {
      Name: this.addDeviceForm.value.name!,
      IsOnline: false,
      PowerSource: 0,
      PowerConsumption: this.addDeviceForm.value.consumption!,
      Image: this.deviceImgPath,
      PropertyId: 1
    }
    this.deviceRegistration.registerVehicleGate(device).subscribe({
      next: (res: any) => {
      this.snackBar.open(res.message, "", {
          duration: 2700, panelClass: ['snack-bar-success']
      });
      //   this.router.navigate(['verification-choice']);
      console.log(res);
      },
      error: (err: any) => {
        this.snackBar.open("Error occured on server!", "", {
          duration: 2700, panelClass: ['snack-bar-server-error']
       });
        console.log(err)
      }
    });
  }

  registerHomeBattery(){
    let device : HomeBatteryRegistrationDTO = {
      Name: this.addDeviceForm.value.name!,
      IsOnline: false,
      PowerSource: 0,
      PowerConsumption: this.addDeviceForm.value.consumption!,
      Image: this.deviceImgPath,
      Capacity: this.addDeviceForm.value.batteryCapacity!,
      Health: this.addDeviceForm.value.batteryHealth!,
      CurrentCharge: 100,
      PropertyId: 1
    }
    this.deviceRegistration.registerHomeBattery(device).subscribe({
      next: (res: any) => {
      this.snackBar.open(res.message, "", {
          duration: 2700, panelClass: ['snack-bar-success']
      });
      //   this.router.navigate(['verification-choice']);
      console.log(res);
      },
      error: (err: any) => {
        this.snackBar.open("Error occured on server!", "", {
          duration: 2700, panelClass: ['snack-bar-server-error']
       });
        console.log(err)
      }
    });
  }

  registerIrrigationSystem(){
    let device : IrrigationSystemRegistrationDTO = {
      Name: this.addDeviceForm.value.name!,
      IsOnline: false,
      PowerSource: 0,
      PowerConsumption: this.addDeviceForm.value.consumption!,
      Image: this.deviceImgPath,
      PropertyId: 1
    }
    this.deviceRegistration.registerIrrigationSystem(device).subscribe({
      next: (res: any) => {
      this.snackBar.open(res.message, "", {
          duration: 2700, panelClass: ['snack-bar-success']
      });
      //   this.router.navigate(['verification-choice']);
      console.log(res);
      },
      error: (err: any) => {
        this.snackBar.open("Error occured on server!", "", {
          duration: 2700, panelClass: ['snack-bar-server-error']
       });
        console.log(err)
      }
    });
  }

  registerEVCharger(){
    let device : EVChargerRegistrationDTO = {
      Name: this.addDeviceForm.value.name!,
      IsOnline: false,
      PowerSource: 0,
      PowerConsumption: this.addDeviceForm.value.consumption!,
      Image: this.deviceImgPath,
      NumberOfPorts: this.addDeviceForm.value.evNumberOfPorts!,
      ChargingPower: this.addDeviceForm.value.evChargingPower!,
      ChargingThreshold: this.addDeviceForm.value.evChargingThreshold!,
      PropertyId: 1
    }
    this.deviceRegistration.registerEVCharger(device).subscribe({
      next: (res: any) => {
      this.snackBar.open(res.message, "", {
          duration: 2700, panelClass: ['snack-bar-success']
      });
      //   this.router.navigate(['verification-choice']);
      console.log(res);
      },
      error: (err: any) => {
        this.snackBar.open("Error occured on server!", "", {
          duration: 2700, panelClass: ['snack-bar-server-error']
       });
        console.log(err)
      }
    });
  }

  createDevice(){
    if (this.addDeviceForm.valid){
      if (this.deviceType == "AmbientSensor"){
        this.registerAmbientSensor();
      }
      else if (this.deviceType == "AC"){
        this.registerAC();
      }
      else if (this.deviceType == "WashingMachine"){
        this.registerWashingMachine()
      }
      else if (this.deviceType == "Lamp"){
        this.registerLamp();
      }
      else if (this.deviceType == "SolarPanel"){
        this.registerSolarPanel();
      }
      else if (this.deviceType == "VehicleGate"){
        this.registerVehicleGate();
      }
      else if (this.deviceType == "HomeBattery"){
        this.registerHomeBattery();
      }
      else if (this.deviceType == "IrrigationSystem"){
        this.registerIrrigationSystem();
      }
      else {
        this.registerEVCharger();
      }
      this.dialogRef.close();
    } else {
      this.snackBar.open("Check your inputs again!", "", {
        duration: 2700, panelClass: ['snack-bar-front-error']
     });
    }
  }

  close() {
    this.dialogRef.close();
  }

}
