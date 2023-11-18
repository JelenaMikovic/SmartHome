import { PropertyDTO, PropertyService } from './../../services/property.service';
import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { areaValidator, floorsValidator, nameValidator } from '../validators/property-validators';
import { markFormControlsTouched } from '../validators/formGroupValidators';

@Component({
  selector: 'app-choose-device-type-dialog',
  templateUrl: './choose-device-type-dialog.component.html',
  styleUrls: ['./choose-device-type-dialog.component.css']
})
export class ChooseDeviceTypeDialogComponent implements OnInit {

  constructor(
    public dialogRef: MatDialogRef<ChooseDeviceTypeDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private snackBar: MatSnackBar,
    private propertyService: PropertyService
  ) { }

  ngOnInit(): void {
  }

  close() {
    this.dialogRef.close();
  }

  clickedAmbientSensor: boolean = false;
  clickedAC: boolean = false;
  clickedWashingMachine: boolean = false;
  clickedLamp: boolean = false;
  clickedSolarPanel: boolean = false;
  clickedVehicleGate: boolean = false;
  clickedHomeBattery: boolean = false;
  clickedIrrigationSystem: boolean = false;
  clickedEVCharger: boolean = false;

  clickAmbientSensor() : void {
    this.clickedAmbientSensor = !this.clickedAmbientSensor
  }

  clickAC() : void {
    this.clickedAC = !this.clickedAC;
  }

  itemClicked: { [key: string]: boolean } = {
    AmbientSensor: false,
    AC: false,
    WashingMachine: false,
    Lamp: false,
    SolarPanel: false,
    VehicleGate: false,
    HomeBattery: false,
    IrrigationSystem: false,
    EVCharger: false,
  };

  handleClick(item: string): void {
    this.resetAllFlags();
    this.itemClicked[item] = !this.itemClicked[item];
  }

  private resetAllFlags(): void {
    for (const key in this.itemClicked) {
      if (Object.prototype.hasOwnProperty.call(this.itemClicked, key)) {
        this.itemClicked[key] = false;
      }
    }
  }

  isItemClicked(): boolean {
    for (const key in this.itemClicked) {
      if (Object.prototype.hasOwnProperty.call(this.itemClicked, key)) {
        if (this.itemClicked[key])
          return true;
      }
    }
    return false;
  }

}
