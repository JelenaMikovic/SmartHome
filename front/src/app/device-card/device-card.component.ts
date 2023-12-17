import { Component, Input, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DeviceService } from 'src/services/device.service';

@Component({
  selector: 'app-device-card',
  templateUrl: './device-card.component.html',
  styleUrls: ['./device-card.component.css', '../property-card/property-card.component.css']
})
export class DeviceCardComponent implements OnInit {

  @Input() deviceId: any = {};
  @Input() device: any = {};
  @Input() isDetails: boolean = false;
  checkedOnOff: boolean = false;
  selectedTimeInterval: string = "";
  timeIntervals: string[] = [
    'last 6h',
    'last 12h',
    'yesterday',
    'last 7d',
    'last month'
  ]

  constructor(private deviceService: DeviceService,
    private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    if (this.isDetails){
      console.log("AAAAAAAAAAAAAAAAAAA")
      this.deviceService.getDeviceDetailsById(this.deviceId).subscribe({
        next: (value: any) => {
          this.device = value;
          console.log(this.device)
        },
        error: (err) => {
          console.log(err);
        },
      })
    
    } else{
      console.log(this.device);
    }
  }

  toggleChangeOnOff(){
    this.deviceService.toggleOnOff(this.deviceId, this.checkedOnOff).subscribe({
      next: (res: any) => {
        this.snackBar.open("You toggled device state!", "", {
            duration: 2700, panelClass: ['snack-bar-success']
        });
        },
        error: (err: any) => {
          this.snackBar.open("Error occured on server!", "", {
            duration: 2700, panelClass: ['snack-bar-server-error']
         });
          console.log(err);
        }
    })
  }
  onIntervalSelected(){
    console.log(this.selectedTimeInterval)
  }

  reportsBy: string = "interval";

  clickedReportsBy(type: string){
    this.reportsBy = type;
  }


}
