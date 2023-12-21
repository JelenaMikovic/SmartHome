import { DataDTO, SocketService } from './../../services/socket.service';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { NavigationStart, Router } from '@angular/router';
import Highcharts from 'highcharts';
import { DeviceService } from 'src/services/device.service';

@Component({
  selector: 'app-device-card',
  templateUrl: './device-card.component.html',
  styleUrls: ['./device-card.component.css', '../property-card/property-card.component.css'],
})
export class DeviceCardComponent implements OnInit, OnDestroy {

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
  temperatureChartData: { timestamp: string; value: string }[] = [];
  humidityChartData: { timestamp: string; value: string }[] = [];

  applyFilter(event: Event): void {
    const filter = (event.target as HTMLInputElement).value.trim().toLocaleLowerCase();
    this.dataSource.filter = filter;
  } 

  constructor(private deviceService: DeviceService,
    private snackBar: MatSnackBar, private socketService: SocketService, private router: Router ) {
      this.router.events.subscribe((event) => {
        if (event instanceof NavigationStart) {
          this.socketService.stopConnection(); 
        }
      });
     }
  
  ngOnDestroy(): void {
    this.socketService.stopConnection();
  }

  ngOnInit(): void {
    if (this.isDetails){
      console.log("AAAAAAAAAAAAAAAAAAA")
      this.deviceService.getDeviceDetailsById(this.deviceId).subscribe({
        next: (value: any) => {
          this.device = value;
          this.initDevice();
          console.log(this.device)
          this.lastHour()
          this.socketService.startConnection(this.device.id);
          this.socketService.addDataUpdateListener((dto: any) => {
            console.log(dto)
            if(dto.measurement == "command"){
                // dodati datasourcu za komande
            } else {
              if (this.device.deviceType == "LAMP") {
                this.device.brightnessLevel = dto["Value"];
              }
              if(this.device.deviceType == "AMBIENT_SENSOR"){
                this.temperatureChartData.push({ timestamp: new Date().toISOString(), value: dto.Temperature });
                this.humidityChartData.push({ timestamp: new Date().toISOString(), value: dto.Humidity });
                this.device.currentTemperature = dto.Temperature;
                this.device.currentHumidity = dto.Humidity;
                this.createTemperatureChart(this.temperatureChartData, "current-temperature-chart-container")
                this.createHumidityChart(this.humidityChartData, 'current-humidity-chart-container')
              }
            }
          });
        },
        error: (err) => {
          console.log(err);
        },
      })
    
    } else{
      console.log(this.device);
    }
  }

  initDevice() {
    if (this.device.deviceType == "LAMP") {
      this.checkedAutomatic = this.device.regime == "AUTOMATIC"? true: false;
      this.checkedOnOff = this.device.isOn;
    }
  }

  toggleChangeOnOff(){
    if (this.device.isOnline) {
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
    } else {
      this.snackBar.open("Device is offline so you can't perform actions.", "", {
        duration: 2700, panelClass: ['snack-bar-server-error']
     });
    }
    
  }

  lastHour(){
    const currentDate = new Date();
    const startDate = new Date();
    startDate.setHours(currentDate.getHours() - 1)
 

    const dto = {
      deviceId: this.deviceId,
      startDate: startDate.toISOString(),
      endDate: currentDate.toISOString(),
    };

    if(this.device.deviceType == "AMBIENT_SENSOR"){
      this.deviceService.getAmbientSensorReport(dto).subscribe(
        (response) => {
          console.log('Response:', response);
          this.temperatureChartData = response.temperatureData;
          this.humidityChartData = response.humidityData;
          this.createTemperatureChart(response.temperatureData, "current-temperature-chart-container")
          this.createHumidityChart(response.humidityData, 'current-humidity-chart-container')
        },
        (error) => {
          console.error('Error:', error);
        }
      );
    }
    // if(this.device.deviceType == "LAMP"){
    //   this.deviceService.getLampReport(dto).subscribe(
    //     (response) => {
    //       console.log('Response:', response);
    //       this.createBrightnessChart(response.temperatureData, "current-brightness-chart-container")
    //     },
    //     (error) => {
    //       console.error('Error:', error);
    //     }
    //   );
    // }
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

    const dto = {
      deviceId: this.deviceId,
      startDate: startDate.toISOString(),
      endDate: currentDate.toISOString(),
    };

    if(this.device.deviceType == "AMBIENT_SENSOR"){
      this.deviceService.getAmbientSensorReport(dto).subscribe(
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
    if(this.device.deviceType == "LAMP"){
      this.deviceService.getLampReport(dto).subscribe(
        (response) => {
          console.log('Response:', response);
          this.createBrightnessChart(response.brightnessData, 'brightness-chart-container')
        },
        (error) => {
          console.error('Error:', error);
        }
      );
    }
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

  createBrightnessChart(data: { timestamp: string; value: string }[], name: string): void {
    const timestamps = data.map((entry) => entry.timestamp);
    const brightnesses = data.map((entry) => parseFloat(entry.value));

    const options: Highcharts.Options = {
      chart: {
        type: 'spline',
      },
      title: {
        text: 'Brightness level Chart',
      },
      xAxis: {
        categories: timestamps,
      },
      yAxis: {
        title: {
          text: 'lux',
        },
      },
      series: [
        {
          type: 'spline',
          name: 'Brightness level',
          data: brightnesses,
        },
      ],
    };

    Highcharts.chart(name, options);
  }

  toggleAutomaticRegime() {
    if (this.device.isOnline) {
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
    } else {
      this.snackBar.open("Device is offline so you can't perform actions.", "", {
        duration: 2700, panelClass: ['snack-bar-server-error']
     });
    }
  }
  
}


