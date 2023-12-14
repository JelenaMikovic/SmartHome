import { Component, OnInit } from '@angular/core';
import * as Highcharts from 'highcharts';
import { InfluxDBDataService } from 'src/services/influx-dbdata.service';

@Component({
  selector: 'app-ambiental-sensor',
  templateUrl: './ambiental-sensor.component.html',
  styleUrls: ['./ambiental-sensor.component.css']
})
export class AmbientalSensorComponent implements OnInit {
  private readonly deviceId = "5";
  private updateInterval: any;

  constructor(private db: InfluxDBDataService) { }

  ngOnInit(): void {
    this.updateCharts();
    this.updateInterval = setInterval(() => {
      this.updateCharts();
    }, 0.5 * 60 * 1000);
  }

  updateCharts(){
    this.db.getAmbientalTemperature(this.deviceId)
    .then((value) => {
      this.createChart(value);
    })
    .catch((err) => {
      console.log(err);
    });

    this.db.getAmbientalHumidity(this.deviceId)
    .then((value) => {
      this.createChartHumidity(value);
    })
    .catch((err) => {
      console.log(err);
    });
  }

  createChart(data: { timestamp: string; temperature: string }[]): void {
    const timestamps = data.map((entry) => entry.timestamp);
    const temperatures = data.map((entry) => parseFloat(entry.temperature));

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

    Highcharts.chart('temperature-chart-container', options);
  }

  createChartHumidity(data: { timestamp: string; humidity: string }[]): void {
    const timestamps = data.map((entry) => entry.timestamp);
    const humidities = data.map((entry) => parseFloat(entry.humidity));

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

    Highcharts.chart('humidity-chart-container', options);
  }
}
