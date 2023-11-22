import { PropertyDTO, PropertyService } from './../../services/property.service';
import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { areaValidator, nameValidator } from '../validators/property-validators';
import { markFormControlsTouched } from '../validators/formGroupValidators';
import {batteryCapacityValidator, evChargingPowerValidator, evNumberOfPortsValidator, maxTempValidator, minTempValidator, percentageValidation, powerConsumptionValidator } from '../validators/device-validators';
import { ConfirmValidParentMatcher, minMaxValidator } from '../validators/min-max-validators';

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

  deviceImgPath = "";

  constructor(
    public dialogRef: MatDialogRef<AddDeviceDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private snackBar: MatSnackBar,
    private propertyService: PropertyService) { 
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

  close() {
    this.dialogRef.close();
  }

}
