import { PropertyDTO, PropertyService } from './../../services/property.service';
import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { areaValidator, floorsValidator, nameValidator } from '../validators/property-validators';
import { markFormControlsTouched } from '../validators/formGroupValidators';
import { AddDeviceDialogComponent } from '../add-device-dialog/add-device-dialog.component';

@Component({
  selector: 'app-choose-device-type-dialog',
  templateUrl: './choose-device-type-dialog.component.html',
  styleUrls: ['./choose-device-type-dialog.component.css']
})
export class ChooseDeviceTypeDialogComponent implements OnInit {

  propertyId: any;
  constructor(
    public dialogRef: MatDialogRef<ChooseDeviceTypeDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private snackBar: MatSnackBar,
    private propertyService: PropertyService,
    private dialog: MatDialog
  ) { 
    this.propertyId = data.propertyId;
  }

  ngOnInit(): void {
    // console.log(data.propertyId)
  }

  close() {
    this.dialogRef.close();
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

  private getClickedItem(): any{
    for (const key in this.itemClicked) {
      if (Object.prototype.hasOwnProperty.call(this.itemClicked, key)) {
        if (this.itemClicked[key])
          return key;
      }
    }
  }

  continueToDeviceDetails(){
    this.close();
    const dialogRef = this.dialog.open(AddDeviceDialogComponent, {
      data: {
        propertyId : this.propertyId,
        deviceType : this.getClickedItem()
      }
    });
  }

}
