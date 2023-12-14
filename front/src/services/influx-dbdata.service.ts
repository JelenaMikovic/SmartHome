import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { InfluxDB, FluxTableMetaData, FluxResultObserver, Point } from '@influxdata/influxdb-client';


@Injectable({
  providedIn: 'root'
})
export class InfluxDBDataService {
  private readonly token = 'U6WYrg5DM3_VnI34b7xX0tiW84Wkl4ha7rW8yq7od4izH5VV7tSDfaHaiwKZRC3m0ANHTn0kxRu_Az2tL4EZRg==';
  private readonly org = '3107c2a809cc3127';
  private readonly bucket = 'measurements';
  private readonly url = 'http://localhost:8086';
  private readonly influxDB: InfluxDB;

  constructor(private httpClient: HttpClient) {
    this.influxDB = new InfluxDB({
      url: this.url,
      token: this.token,
    });
  }

  getAmbientalTemperature(deviceId: string): Promise<{ timestamp: string, temperature: string }[]> {
    return new Promise<{ timestamp: string, temperature: string }[]>((resolve, reject) => {
      const query = `from(bucket: "${this.bucket}")
        |> range(start: -7d)
        |> filter(fn: (r) => r["_measurement"] == "ambiental_sensor" and r["_field"] == "temperature" and r["device_id"] == "${deviceId}")`;
  
      const queryApi = this.influxDB.getQueryApi(this.org);
      const points: { timestamp: string, temperature: string }[] = [];
  
      queryApi.queryRows(query, {
        next: (result: string[], tableMeta: FluxTableMetaData) => {
          const timestamp = new Date(result[4]).toISOString();
          const value = result[5];
          points.push({ timestamp, temperature: value });
        },
        error: (error: Error) => {
          console.error(error);
          reject(error);
        },
        complete: () => {
          console.log('Query complete');
          resolve(points);
        },
      });
    });
  }

  getAmbientalHumidity(deviceId: string): Promise<{ timestamp: string, humidity: string }[]> {
    return new Promise<{ timestamp: string, humidity: string }[]>((resolve, reject) => {
      const query = `from(bucket: "${this.bucket}")
        |> range(start: -7d)
        |> filter(fn: (r) => r["_measurement"] == "ambiental_sensor" and r["_field"] == "humidity" and r["device_id"] == "${deviceId}")`;
  
      const queryApi = this.influxDB.getQueryApi(this.org);
      const points: { timestamp: string, humidity: string }[] = [];
  
      queryApi.queryRows(query, {
        next: (result: string[], tableMeta: FluxTableMetaData) => {
          const timestamp = new Date(result[4]).toISOString();
          const value = result[5];
          points.push({ timestamp, humidity: value });
        },
        error: (error: Error) => {
          console.error(error);
          reject(error);
        },
        complete: () => {
          console.log('Query complete');
          resolve(points);
        },
      });
    });
  }

}
