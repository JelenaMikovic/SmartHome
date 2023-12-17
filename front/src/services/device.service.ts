import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DeviceService {

  constructor(private http: HttpClient) { }

  getPropertyDeviceDetails(propertyId: string, page: any, size: any) {
    return this.http.get<any>(environment.apiHost + "/device-details/paginated?propertyId=" + propertyId + "&page="+page + "&size=" + size
    , {withCredentials: true});
  }

  getDeviceDetailsById(deviceId: string) {
    return this.http.get<any>(environment.apiHost + "/device-details/" + deviceId
    , {withCredentials: true});
  }

  toggleOnOff(deviceId: string, isOn: boolean) {
    if (isOn)
      return this.http.put<any>(environment.apiHost + "/device-toggle/on/" + deviceId
    , {withCredentials: true});
    else 
      return this.http.put<any>(environment.apiHost + "/device-details/off/" + deviceId
    , {withCredentials: true});
  }

  getAmbientSensorReport(ambientSensorReportDTO: any): Observable<any> {
    const url = `${environment.apiHost}/device/reports`;

    return this.http.post<any>(url, ambientSensorReportDTO , { withCredentials: true });
  }
}