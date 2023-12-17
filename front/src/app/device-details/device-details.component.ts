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
  selector: 'app-device-details',
  templateUrl: './device-details.component.html',
  styleUrls: ['./device-details.component.css']
})
export class DeviceDetailsComponent implements OnInit {

  deviceId: any = {};
  constructor(private dialog: MatDialog, private propertyService: PropertyService, 
    private authService: AuthService, private snackBar: MatSnackBar,
    private router: Router,
    private route: ActivatedRoute,
    private propertyDetailsService: PropertyDetailsService,
    private deviceService: DeviceService) { }

  ngOnInit(): void {
    this.deviceId = this.route.snapshot.paramMap.get('id')!;
  }

}
