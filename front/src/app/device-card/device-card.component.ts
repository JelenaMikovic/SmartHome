import { AbstractControl, FormControl, FormGroup, FormGroupDirective, NgForm, Validators } from '@angular/forms';
import { DataDTO, SocketService } from './../../services/socket.service';
import { Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { NavigationStart, Router } from '@angular/router';
import Highcharts from 'highcharts';
import { DeviceService } from 'src/services/device.service';
import { ConfirmValidParentMatcher, dateAheadOfTodayValidator, dateMatcher } from '../validators/date-validators';
import { MatSort } from '@angular/material/sort';

@Component({
  selector: 'app-device-card',
  templateUrl: './device-card.component.html',
  styleUrls: ['./device-card.component.css', '../property-card/property-card.component.css', '../property-details/property-details.component.css'],
})
export class DeviceCardComponent implements OnInit, OnDestroy {

  @Input() deviceId: any = {};
  @Input() device: any = {};
  @Input() isDetails: boolean = false;
  checkedOnOff: boolean = false;
  checkedAutomatic: boolean = false;
  checkedPrivate: boolean = false;
  checkedOpen: boolean = false;
  selectedTimeInterval: string = "";
  isSelected: boolean = false;
  showAddSchedule: boolean = false;
  mode: string = "";
  scheduleMode: string = "";
  timeIntervals: string[] = [
    'Last 6h',
    'Last 12h',
    'Yesterday',
    'Last 7d',
    'Last month'
  ]
  displayedColumns: string[] = ['action', 'timestamp', 'user', 'state'];
  displayedColumnSchedule: string[] = ['from', 'to', 'temperature', 'mode'];
  changeTemperatureForm = new FormGroup({
    temperature: new FormControl('', [Validators.required, this.temperatureValidator])
  })
  minTemperature: number = 0;
  maxTemperature: number = 0;

  @ViewChild(MatSort) sort: any;
  dataSource = new MatTableDataSource<any>;
  dataSourceWithoutFilters = new MatTableDataSource<any>;

  dataSchedule = new MatTableDataSource<any>;

  startDateControl = new FormControl();
  endDateControl = new FormControl();
  temperatureChartData: { timestamp: string; value: string }[] = [];
  humidityChartData: { timestamp: string; value: string }[] = [];
  powerConsumptionChartData: {timestamp: string; value: string}[] = [];

  addPlateForm = new FormGroup({
    newPlate: new FormControl('', Validators.required)
  });

  addScheduleForm = new FormGroup({
    temperature: new FormControl('', [Validators.required, this.temperatureValidator]),
    mode: new FormControl('', [Validators.required]),
    endTime: new FormControl('', [Validators.required]),
    startTime: new FormControl('', [Validators.required])
  })

  currentPlate = ""

  temperatureValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const value = control.value;
  
    if (isNaN(value) || value < 15 || value > 30) {
      return { invalidTemperature: true };
    }
  
    return null;
  }

  applyNameFilter(event: Event): void {
    const filter = (event.target as HTMLInputElement).value.trim().toLocaleLowerCase();
    this.dataSource.filter = filter;
  } 

  applyDateFilter(){
    if (this.datesTableForm.valid)
      this.dataSource.data = this.dataSource.data.filter(e=> {
        const dateFromData = new Date(e.timestamp);
        console.log(dateFromData);
        return dateFromData >= new Date(this.datesTableForm.value.startDateTable!) && dateFromData <= new Date(this.datesTableForm.value.endDateTable!)
      });
    else {
      console.log("wrr");
    }
  }

  resetTableDates(myForm: FormGroupDirective){
      this.datesTableForm.get('startDateTable')?.setValue(null);
      this.datesTableForm.get('endDateTable')?.setValue(null);
      myForm.resetForm();
      myForm.form.markAsPristine();
      myForm.form.markAsUntouched();
      myForm.form.updateValueAndValidity();
      

      this.dataSource = this.dataSourceWithoutFilters
  }

  addToTable(newData: any): void {
    const dataFiltered = this.dataSource.data;
    dataFiltered.push(newData);
    this.dataSource.data = dataFiltered;

    const data = this.dataSourceWithoutFilters.data;
    data.push(newData);
    this.dataSourceWithoutFilters.data = data;
  }

  confirmValidParentMatcher = new ConfirmValidParentMatcher();
  
  datesForm = new FormGroup({
    startDate: new FormControl('', [Validators.required, dateAheadOfTodayValidator()]),
    endDate: new FormControl('', [Validators.required])
  }, [dateMatcher("startDate", "endDate")])

  datesTableForm = new FormGroup({
    startDateTable: new FormControl('', [Validators.required, dateAheadOfTodayValidator()]),
    endDateTable: new FormControl('', [Validators.required])
  }, [dateMatcher("startDateTable", "endDateTable")])

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
      this.dataSource.filterPredicate = (data: TableData, filter: string) => {
        return data.user == filter;
       };
      this.dataSource.sort = this.sort;
      this.deviceService.getDeviceDetailsById(this.deviceId).subscribe({
        next: (value: any) => {
          this.device = value;
          this.initDevice();
          console.log(this.device)
          this.lastHour()
          this.socketService.startConnection(this.device.id, this.device.deviceType == "HOME_BATTERY", this.device.propertyId);
          this.socketService.addDataUpdateListener((dto: any) => {
            console.log(dto)
            if(dto.Measurement == "command"){
              const newData = {
                action: dto.Action, 
                timestamp: new Date().toISOString(),
                user: dto.User, 
                state: dto.Value 
              };
        
              this.addToTable(newData);
            } else {
              if (this.device.deviceType == "LAMP") {
                this.device.brightnessLevel = dto["Value"];
                return;
              }
              if (this.device.deviceType == "VEHICLE_GATE") {
                this.currentPlate = dto["Value"] == "none"? "":  dto["Value"];
                return;
              }
              if(this.device.deviceType == "AMBIENT_SENSOR"){
                this.temperatureChartData.push({ timestamp: new Date().toISOString(), value: dto.Temperature });
                this.humidityChartData.push({ timestamp: new Date().toISOString(), value: dto.Humidity });
                this.device.currentTemperature = dto.Temperature;
                this.device.currentHumidity = dto.Humidity;
                this.createTemperatureChart(this.temperatureChartData, "current-temperature-chart-container")
                this.createHumidityChart(this.humidityChartData, 'current-humidity-chart-container')
              }
              if(this.device.deviceType == "HOME_BATTERY"){
                //this.powerConsumptionChartData.push({ timestamp: new Date().toISOString(), value: dto.CurrentCharge });
                this.device.currentCharge = dto.CurrentCharge + 1;
                console.log(this.device.currentCharge);
                this.createPowerConsumptionChart(this.powerConsumptionChartData, "current-power-consumption-chart-container")
              }
              if(this.device.deviceType == "AC"){
                this.device.currentTemperature = dto.CurrentTemperature;
                this.device.currentMode = dto.CurrentMode;
              }
            }
          });
          this.socketService.addConsumptionUpdateListener((dto: any) => {
            console.log(dto)
            if(this.device.deviceType == "HOME_BATTERY"){
              this.powerConsumptionChartData.push({ timestamp: new Date().toISOString(), value: dto.Consumed });
              this.createPowerConsumptionChart(this.powerConsumptionChartData, "current-power-consumption-chart-container")
              // this.device.currentHumidity = dto.Humidity;
              // this.createHumidityChart(this.humidityChartData, 'current-humidity-chart-container')
            }
          });
        },
        error: (err) => {
          console.log(err);
        },
      })

      this.deviceService.getActionTable(this.deviceId).subscribe({
        next: (value: any) => {
          console.log(value);
          this.dataSource  = new MatTableDataSource(value.tableData);
          this.dataSourceWithoutFilters = new MatTableDataSource(value.tableData);
          //this.dataSource = newData;
          //this.dataSource.filterPredicate = (data: Element, filter: string) => data.user.indexOf(filter) != -1;
        },
        error: (err) => {
          console.log(err);
        },});
        console.log(this.device.isOn);
    } else{
      console.log(this.device);
    }
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  initDevice() {
    if (this.device.deviceType == "LAMP") {
      this.checkedAutomatic = this.device.regime == "AUTOMATIC"? true: false;
      this.checkedOnOff = this.device.isOn;
      return;
    }

    if (this.device.deviceType == "VEHICLE_GATE") {
      this.checkedOpen = this.device.isOpen;
      this.checkedPrivate = this.device.isPrivate;
    }
    if (this.device.deviceType == "SOLAR_PANEL"){
      this.checkedOnOff = this.device.isOn;
    }
    if (this.device.deviceType == "AC") {
      this.minTemperature = this.device.minTemperature;
      this.maxTemperature = this.device.maxTemperature;
      this.mode = this.device.currentMode;
      this.checkedOnOff = this.device.isOn;
      this.getSchedule();
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

  toggleGateOpen() {
    if (this.device.isOnline) {
      this.deviceService.toggleGateOptions(this.device, "open", this.checkedOpen).subscribe({
        next: (res: any) => {
          this.snackBar.open("You toggled gate open option!", "", {
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
  
  togglePrivateRegime() {
    if (this.device.isOnline) {
      this.deviceService.toggleGateOptions(this.device, "private", this.checkedPrivate).subscribe({
        next: (res: any) => {
          this.snackBar.open("You toggled gate private option!", "", {
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
  
  generateByDate(){
    let startDate = new Date(this.datesForm.value.startDate!).toISOString()
    let endDate = new Date(this.datesForm.value.endDate!).toISOString()
    console.log(startDate);
    console.log(endDate);

    this.isSelected = true

    const dto = {
      deviceId: this.deviceId,
      startDate: startDate,
      endDate: endDate,
    };

    if(this.device.deviceType == "AMBIENT_SENSOR"){
      this.deviceService.getAmbientSensorReport(dto).subscribe(
        (response) => {
          console.log('Response:', response);
          this.temperatureChartData = response.temperatureData;
          this.humidityChartData = response.humidityData;
          this.createTemperatureChart(response.temperatureData, "temperature-chart-container")
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
          this.createBrightnessChart(response.temperatureData, "brightness-chart-container")
        },
        (error) => {
          console.error('Error:', error);
        }
      );
    }
    console.log(this.device)
    if (this.device.deviceType == "HOME_BATTERY"){
      const dtob = {
        deviceId: 2,
        startDate: startDate,
        endDate: endDate,
      };
      this.deviceService.getBatteryReport(dtob).subscribe(
        (response) => {
          console.log('Response:', response);
          this.powerConsumptionChartData = response.consumptionData;
          this.createPowerConsumptionChart(response.consumptionData, 'power-consumption-chart-container')
        },
        (error) => {
          console.error('Error:', error);
        }
      );
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
    if(this.device.deviceType == "LAMP"){
      this.deviceService.getLampReport(dto).subscribe(
        (response) => {
          console.log('Response:', response);
          this.createBrightnessChart(response.temperatureData, "current-brightness-chart-container")
        },
        (error) => {
          console.error('Error:', error);
        }
      );
    }
    console.log(this.device)
    if (this.device.deviceType == "HOME_BATTERY"){
      const dtob = {
        deviceId: 2,
        startDate: startDate.toISOString(),
        endDate: currentDate.toISOString(),
      };
      this.deviceService.getBatteryReport(dtob).subscribe(
        (response) => {
          console.log('Response:', response);
          this.powerConsumptionChartData = response.consumptionData;
          this.createPowerConsumptionChart(response.consumptionData, 'currnet-power-consumption-chart-container')
        },
        (error) => {
          console.error('Error:', error);
        }
      );
    }
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
    if (this.device.deviceType == "HOME_BATTERY"){
      const dto2= {
        deviceId: 2,
        startDate: startDate.toISOString(),
        endDate: currentDate.toISOString(),
      };
      this.deviceService.getBatteryReport(dto2).subscribe(
        (response) => {
          console.log('Response:', response);
          this.createPowerConsumptionChart(response.consumptionData, 'current-power-consumption-chart-container')
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

  createPowerConsumptionChart(data: { timestamp: string; value: string }[], name: string): void {
    const timestamps = data.map((entry) => entry.timestamp);
    const consumptions = data.map((entry) => parseFloat(entry.value));

    const options: Highcharts.Options = {
      chart: {
        type: 'spline',
      },
      title: {
        text: 'Power consumption Chart',
      },
      xAxis: {
        categories: timestamps,
      },
      yAxis: {
        title: {
          text: 'kWh',
        },
      },
      series: [
        {
          type: 'spline',
          name: 'Power consumption',
          data: consumptions,
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

  removeAllowedPlate(plate: string) {
    const index = this.device.allowedLicencePlates.indexOf(plate);
    console.log(index)
    if (index !== -1) {
      this.deviceService.manageAllowedPlates(plate, this.device, "removeplate").subscribe({
        next: (value) => {
          this.snackBar.open("Remove plate request sent!", "", {
            duration: 2700, panelClass: ['snack-bar-success']
          });
          this.device.allowedLicencePlates.splice(index, 1);
        },
        error: (err) => {
          this.snackBar.open("Server side error!", "", {
            duration: 2700, panelClass: ['snack-bar-server-error']
          });
          console.log(err);
        },
      })

    }
  }

  addAllowedPlate() {
    if (this.addPlateForm.valid) {
      let plate = this.addPlateForm.value.newPlate;

      if (plate) {
        const index = this.device.allowedLicencePlates.indexOf(plate);
        if (index == -1) {
          this.deviceService.manageAllowedPlates(plate, this.device, "addplate").subscribe({
            next: (value) => {
              this.snackBar.open("New plate request sent!", "", {
                duration: 2700, panelClass: ['snack-bar-success']
              });
              this.device.allowedLicencePlates.push(plate);
              console.log(this.device.allowedLicencePlates)
              this.addPlateForm.get("newPlate")?.clearValidators();
            },
            error: (err) => {
              this.snackBar.open("Server side error!", "", {
                duration: 2700, panelClass: ['snack-bar-server-error']
              });
              console.log(err);
            },
          })

          
        }
        
      }
    }
  }

  changeTemperature(){
    console.log(this.changeTemperatureForm.value.temperature)
    if (this.device.isOnline) {
      const temperatureValue = this.changeTemperatureForm.value.temperature;
      let temperatureString = "";
      if(temperatureValue != null){
        temperatureString = temperatureValue.toString();
      }
      this.deviceService.changeTemperature(this.device, temperatureString).subscribe({
        next: (res: any) => {
          this.snackBar.open("You changed device temperature!", "", {
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

  onModeSelected(){
    if (this.device.isOnline) {
      this.deviceService.changeMode(this.device, this.mode).subscribe({
        next: (res: any) => {
          this.snackBar.open("You changed device mode!", "", {
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

  schedule(){
    this.showAddSchedule = true;
  }

  addSchedule(){
    const dto = {
      startTime: this.addScheduleForm.value.startTime + ":00:00",
      endTime: this.addScheduleForm.value.endTime + ":00:00",
      mode: this.scheduleMode,
      temerature: this.addScheduleForm.value.temperature
    } 

    this.deviceService.addSchedule(dto).subscribe({
      next: (res: any) => {
        this.snackBar.open("You added device schedule!", "", {
        });
        },
        error: (err: any) => {
          this.snackBar.open("Error occured on server!", "", {
         });
          console.log(err);
        }
    })
  }

  removeSchedule(id: number){
    this.deviceService.removeSchedule(id).subscribe({
      next: (res: any) => {
        this.snackBar.open("You removed device schedule!", "", {
        });
        },
        error: (err: any) => {
          this.snackBar.open("Error occured on server!", "", {
         });
          console.log(err);
        }
    })
  }

  getSchedule(){
    console.log("dsadad")
    this.deviceService.getDeviceSchedule(this.device.id).subscribe({
      next: (res: any) => {
        console.log(res)
        for (const schedule of res.schedules) {
          this.dataSchedule.data.push({
            from: schedule.startTime,
            to: schedule.endTime,
            temperature: schedule.temperature,
            mode: schedule.mode
          });
        }
        console.log(this.dataSchedule.data);
        this.dataSchedule._updateChangeSubscription();
        },
        error: (err: any) => {
          console.log(err);
        }
    })
  }
}

export interface TableData{
  action: string,
  time: Date,
  user: string,
  state: string
}

export interface ScheduleData{
  from: string,
  to: string,
  temperature: number,
  mode: string
}
