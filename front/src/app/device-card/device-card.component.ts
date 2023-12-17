import { Component, Input, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import Highcharts from 'highcharts';
import { DeviceService } from 'src/services/device.service';

@Component({
  selector: 'app-device-card',
  templateUrl: './device-card.component.html',
  styleUrls: ['./device-card.component.css', '../property-card/property-card.component.css'],
})
export class DeviceCardComponent implements OnInit {

  @Input() deviceId: any = {};
  @Input() device: any = {};
  @Input() isDetails: boolean = false;
  checkedOnOff: boolean = false;
  checkedAutomatic: boolean = false;
  selectedTimeInterval: string = "";
  isSelected: boolean = false;
  timeIntervals: string[] = [
    'Last 6h',
    'Last 12h',
    'Yesterday',
    'Last 7d',
    'Last month'
  ]
  displayedColumns: string[] = ['action', 'time', 'byWho'];
  dataSource = new MatTableDataSource([
    { action: 'Turned On', time: new Date(), byWho: 'Bob' },
    { action: 'Turned Off', time: new Date(), byWho: 'Bob' },
  ]);
  startDateControl = new FormControl();
  endDateControl = new FormControl();

  applyFilter(event: Event): void {
    const filter = (event.target as HTMLInputElement).value.trim().toLocaleLowerCase();
    this.dataSource.filter = filter;
  } 

  constructor(private deviceService: DeviceService,
    private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    if (this.isDetails){
      console.log("AAAAAAAAAAAAAAAAAAA")
      this.deviceService.getDeviceDetailsById(this.deviceId).subscribe({
        next: (value: any) => {
          this.device = value;
          console.log(this.device)
          this.lastHour()
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

  lastHour(){
    const currentDate = new Date();
    const startDate = new Date();
    startDate.setHours(currentDate.getHours() - 1)
 

    const ambientSensorReportDTO = {
      deviceId: this.deviceId,
      startDate: startDate.toISOString(),
      endDate: currentDate.toISOString(),
    };

    this.deviceService.getAmbientSensorReport(ambientSensorReportDTO).subscribe(
      (response) => {
        console.log('Response:', response);
        this.createTemperatureChart(response.temperatureData, "current-temperature-chart-container")
        this.createHumidityChart(response.humidityData, 'current-humidity-chart-container')
      },
      (error) => {
        console.error('Error:', error);
      }
    );
  }

  onIntervalSelected(){
    console.log(this.selectedTimeInterval)
    this.isSelected = true;
    const currentDate = new Date();
    const startDate = new Date();
    
    if (this.selectedTimeInterval == "Last 6h")
    {
      startDate.setHours(currentDate.getHours() - 6)
    } else if (this.selectedTimeInterval == "Last 12h")
    {
      startDate.setHours(currentDate.getHours() - 12)
    } else if (this.selectedTimeInterval == "Yesterday")
    {
      startDate.setDate(currentDate.getDate() - 1)
    } else if (this.selectedTimeInterval == "Last 7d") {
      startDate.setDate(currentDate.getDate() - 7)
    } else if (this.selectedTimeInterval == "Last month") {
      startDate.setDate(currentDate.getDate() - 30)
    }

    const ambientSensorReportDTO = {
      deviceId: this.deviceId,
      startDate: startDate.toISOString(),
      endDate: currentDate.toISOString(),
    };

    this.deviceService.getAmbientSensorReport(ambientSensorReportDTO).subscribe(
      (response) => {
        console.log('Response:', response);
        this.createTemperatureChart(response.temperatureData, 'temperature-chart-container')
        this.createHumidityChart(response.humidityData, 'humidity-chart-container')
      },
      (error) => {
        console.error('Error:', error);
      }
    );
  }

  reportsBy: string = "interval";

  clickedReportsBy(type: string){
    this.reportsBy = type;
    this.isSelected = false;
  }

  createTemperatureChart(data: { timestamp: string; value: string }[], name: string): void {
    const timestamps = data.map((entry) => entry.timestamp);
    const temperatures = data.map((entry) => parseFloat(entry.value));

    const options: Highcharts.Options = {
      chart: {
        type: 'spline',
      },
      title: {
        text: 'Temperature Chart',
      },
      xAxis: {
        categories: timestamps,
      },
      yAxis: {
        title: {
          text: 'Temperature',
        },
      },
      series: [
        {
          type: 'spline',
          name: 'Temperature',
          data: temperatures,
        },
      ],
    };

    Highcharts.chart(name, options);
  }

  createHumidityChart(data: { timestamp: string; value: string }[], name: string): void {
    const timestamps = data.map((entry) => entry.timestamp);
    const humidities = data.map((entry) => parseFloat(entry.value));

    const options: Highcharts.Options = {
      chart: {
        type: 'spline',
      },
      title: {
        text: 'Humidity Chart',
      },
      xAxis: {
        categories: timestamps,
      },
      yAxis: {
        title: {
          text: 'Humidity',
        },
      },
      series: [
        {
          type: 'spline',
          name: 'Humidity',
          data: humidities,
        },
      ],
    };

    Highcharts.chart(name, options);
  }

  toggleAutomaticRegime() {
    this.deviceService.toggleAutomaticRegime(this.device, this.checkedAutomatic).subscribe({
      next: (res: any) => {
        this.snackBar.open("You toggled device regime!", "", {
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
}


