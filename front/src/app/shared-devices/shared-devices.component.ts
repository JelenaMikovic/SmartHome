import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { AuthService } from 'src/services/auth.service';
import { DeviceService } from 'src/services/device.service';
import { PropertyService, ReturnedPropertyDTO } from 'src/services/property.service';
import { DeviceDetailsDTO } from '../property-details/property-details.component';
import { ShareRequestsDialogComponent } from '../share-requests-dialog/share-requests-dialog.component';

@Component({
  selector: 'app-shared-devices',
  templateUrl: './shared-devices.component.html',
  styleUrls: ['./shared-devices.component.css']
})
export class SharedDevicesComponent implements OnInit {

  properties: ReturnedPropertyDTO[] = [];
  devices: DeviceDetailsDTO[] = [];
  notifications: number = 0;
  
  constructor(private dialog: MatDialog, private propertyService: PropertyService, 
    private authService: AuthService, private snackBar: MatSnackBar,
    private router: Router,
    private deviceService: DeviceService) { }

  ngOnInit(): void {
    this.getData();
  }

  openRequests(): void {
    const dialogRef = this.dialog.open(ShareRequestsDialogComponent, {
    });
  }
  

  getData(): void {
    this.deviceService.getSharedProperties().subscribe({
      next: (value) => {
        console.log(value)
        this.properties = value;
      }, 
      error: (err) => {
        console.log(err);
      }
    });

    this.deviceService.getSharedDevices().subscribe({
      next: (value) => {
        this.devices = value;
        console.log(value)
      }, 
      error: (err) => {
        console.log(err);
      }
    });

    this.deviceService.getSharedRequests().subscribe(response => {
      console.log(typeof response)
      this.notifications = Object.keys(response).length;
    });
  }

  openPropertyDetails(index: number){
    console.log(this.properties[index]);
    this.router.navigate(['/property-details', {id: this.properties[index].id}])
  }

  openDeviceDetails(index: number){
    console.log(this.devices[index]);
      this.router.navigate(['/device-details', {id: this.devices[index].id, shared: "x"}])
  }
}
