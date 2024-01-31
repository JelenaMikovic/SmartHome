import { PropertyDTO, PropertyService } from '../../services/property.service';
import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DeviceService } from 'src/services/device.service';


@Component({
  selector: 'app-share-dialog',
  templateUrl: './share-dialog.component.html',
  styleUrls: ['./share-dialog.component.css']
})
export class ShareDialogComponent implements OnInit {

  propertyId: any;
  deviceId: any;
  type: any;
  shareForm = new FormGroup({
    email: new FormControl('', [Validators.required]),
  })
  constructor(
    public dialogRef: MatDialogRef<ShareDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private snackBar: MatSnackBar,
    private propertyService: PropertyService,
    private dialog: MatDialog,
    private deviceService: DeviceService
  ) { 
    this.propertyId = data.propertyId;
    this.deviceId = data.deviceId;
    this.type = data.type;
  }

  ngOnInit(): void {
    // console.log(data.propertyId)
  }

  close() {
    this.dialogRef.close();
  }

  send() {
    const dto = {
      email: this.shareForm.get('email')?.value,
      propertyId: this.propertyId,
      devideId: this.deviceId,
      sharedType: this.type
    }
    if(dto.email == ''){
      this.snackBar.open('Please enter a valid email.')
    } else {
      this.deviceService.addSharedDevice(dto).subscribe(
        (response: any) => {
          this.snackBar.open('Device shared successfully!', 'OK');
          this.dialogRef.close(); // close the dialog on success
        },
        (error) => {
          console.error('Error sharing device:', error);
          this.snackBar.open('Failed to share device. Please try again.', 'OK');
        }
      );
    }
  }

}
