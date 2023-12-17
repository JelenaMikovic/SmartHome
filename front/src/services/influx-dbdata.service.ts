import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InfluxDBDataService {

  constructor(private http: HttpClient) {
  
  }

  getAmbientSensorReport(ambientSensorReportDTO: any): Observable<any> {
    const url = `${environment.apiHost}/device/reports`;

    return this.http.post<any>(url, ambientSensorReportDTO , { withCredentials: true });
  }

}
