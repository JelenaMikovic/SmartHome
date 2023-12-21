import { PropertyDTO, PropertyService, ReturnedPropertyDTO } from './../../services/property.service';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AddPropertyDialogComponent } from '../add-property-dialog/add-property-dialog.component';
import { ChooseDeviceTypeDialogComponent } from '../choose-device-type-dialog/choose-device-type-dialog.component';
import { AuthService } from 'src/services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { PropertyDetailsService } from 'src/services/property-details.service';
import { DeviceService } from 'src/services/device.service';
import { AddDeviceDialogComponent } from '../add-device-dialog/add-device-dialog.component';



@Component({
  selector: 'app-property-details',
  templateUrl: './property-details.component.html',
  styleUrls: ['./property-details.component.css']
})
export class PropertyDetailsComponent implements OnInit {

  constructor(private dialog: MatDialog, private propertyService: PropertyService, 
    private authService: AuthService, private snackBar: MatSnackBar,
    private router: Router,
    private route: ActivatedRoute,
    private propertyDetailsService: PropertyDetailsService,
    private deviceService: DeviceService){ }

  loggedUser: any = {};
  property: any = {};
  propertyId: any = {};
  devices: DeviceDetailsDTO[] = [];

  selectedDeviceType: string = "";

  currentPage = 1;
  pageSize = 2;
  count = 0;

  deviceType = [
    'Solar panel', 
    'Battery', 
    'AC', 
    'Ambient sensor',
    'Lamp',
    'Irrigation system',
    'Vehicle gate',
    'Washing machine',
    'EV charger'
  ]

  ngOnInit(): void {
    this.authService.getUser().subscribe({
      next: (value: any) => {
        if (value) {
          this.loggedUser = value;
        }
      },
      error: (err) => {
        console.log(err);
      },
    })
    let id = this.route.snapshot.paramMap.get('id')!;
    this.propertyId = id;
    this.propertyService.getDetails(id).subscribe({
      next: (value) => {
        this.property = value;
      }, 
      error: (err) => {
        console.log(err);
      }
    });
    this.loadItems();
  }

  loadItems() {
    this.deviceService.getPropertyDeviceDetails(this.propertyId, this.currentPage, this.pageSize).subscribe({
      next: (value) => {
        this.currentPage = value.pageIndex;
        this.count = value.count;
        this.devices = value.items;
        console.log(value)
      }, 
      error: (err) => {
        console.log(err);
      }
    });
  }

  details(){
    console.log(this.property)
  }

  onDeviceTypeSelected(){
    console.log(this.devices)
  }

  onPageChange(event: any): void {
    this.currentPage = event.pageIndex + 1;
    this.loadItems();
  }

  openAddDeviceDialog(){
    const dialogRef = this.dialog.open(ChooseDeviceTypeDialogComponent, {
      data: {
      }
    });
  }

  openDeviceDetails(index: number){
    console.log(this.devices[index]);
    //this.propertyDetailsService.setSelectedProperty(this.properties[index]);
    this.router.navigate(['/device-details', {id: this.devices[index].id}])
  }

}

export interface DeviceDetailsDTO{
  id: number,
  name: string,
  isOnline: boolean,
  powerSource: number,
  powerConsumption: number,
  image: string
}