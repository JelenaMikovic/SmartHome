import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { numbers } from '@material/snackbar';
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
      return this.http.get<any>(environment.apiHost + "/device-toggle/on/" + deviceId
    , {withCredentials: true});
    else 
      return this.http.get<any>(environment.apiHost + "/device-toggle/off/" + deviceId
    , {withCredentials: true});
  }

  toggleAutomaticRegime(device: any, isAutomatic: boolean) {
    let dto: CommandDTO = {
      deviceId: device.id,
      deviceType: device.deviceType,
      action: "regime",
      value: isAutomatic? "automatic": "manual"
    }

    console.log(dto)
    return this.http.put<any>(environment.apiHost + "/device-toggle/regime", dto, {withCredentials: true});
    
  }

  changeTemperature(device: any, temperature: string) {
    let dto: CommandDTO = {
      deviceId: device.id,
      deviceType: device.deviceType,
      action: "ChangeTemperature",
      value: temperature
    }

    console.log(dto)
    return this.http.put<any>(environment.apiHost + "/device-toggle/temperature", dto, {withCredentials: true});
  }

  changeMode(device: any, mode: string) {
    let dto: CommandDTO = {
      deviceId: device.id,
      deviceType: device.deviceType,
      action: "ChangeMode",
      value: mode
    }

    console.log(dto)
    return this.http.put<any>(environment.apiHost + "/device-toggle/mode", dto, {withCredentials: true});
  }

  toggleGateOptions(device: any, type: string, value: boolean) {
    // let valueStr = ""
    // if (type == "open") {
    //   valueStr = value ? "OPEN": "CLOSED";
    // } else {
    //   if (type == "private") {
    //     valueStr = value ? "PRIVATE": "PUBLIC";
    //   }
    // }

    let dto: CommandDTO = {
      deviceId: device.id,
      deviceType: device.deviceType,
      action: type,
      value:  value.toString()
    }

    console.log(dto)
    return this.http.put<any>(environment.apiHost + "/device-toggle/regime", dto, {withCredentials: true});
    
  }


  getAmbientSensorReport(ambientSensorReportDTO: any): Observable<any> {
    const url = `${environment.apiHost}/device/ambient-reports`;
    return this.http.post<any>(url, ambientSensorReportDTO, { withCredentials: true });
  }

  getLampReport(ambientSensorReportDTO: any): Observable<any> {
    const url = `${environment.apiHost}/device/lamp-reports`;
    return this.http.post<any>(url, ambientSensorReportDTO, { withCredentials: true });
  }

  getBatteryReport(ambientSensorReportDTO: any): Observable<any> {
    console.log(ambientSensorReportDTO);
    const url = `${environment.apiHost}/device/power-consumption`;
    return this.http.post<any>(url, ambientSensorReportDTO, { withCredentials: true });
  }

  getActionTable(id: any): Observable<any> {
    const url = `${environment.apiHost}/device/action-table/${id}`;
    return this.http.get<any>(url, { withCredentials: true });
  }

  subscribeToDataTopic(deviceId: number): Observable<any> {
    const url = `${environment.apiHost}/data/` + deviceId;
    console.log(url);
    return this.http.get<any>(url, { withCredentials: true });
  }

  manageAllowedPlates(plate: string, device:any, type: string): Observable<any> {
    let dto = {
      deviceId: device.id,
      deviceType: device.deviceType,
      action: type,
      value: plate
    }

    return this.http.put<any>(environment.apiHost + "/device-toggle/regime", dto, {withCredentials: true});
  }

  getDeviceSchedule(deviceId: number): Observable<any> {
    const url = `${environment.apiHost}/device/schedules/` + deviceId;
    return this.http.get<any>(url, { withCredentials: true });
  }

  removeSchedule(deviceId: number): Observable<any> {
    const url = `${environment.apiHost}/device/remove/` + deviceId;
    return this.http.delete<any>(url, { withCredentials: true });
  }

  addSchedule(dto: any): Observable<any> {
    const url = `${environment.apiHost}/device/add/`;
    return this.http.post<any>(url, dto, { withCredentials: true });
  }

  addSharedDevice(dto: any): Observable<any> {
    return this.http.post<any>(environment.apiHost + "/device/shared", dto, { withCredentials: true });
  }

  acceptSharedDevice(id: number): Observable<any> {
    return this.http.put<any>(environment.apiHost + `/device/shared/accept/${id}`, null, { withCredentials: true });
  }

  denySharedDevice(id: number): Observable<any> {
    return this.http.put<any>(environment.apiHost + `/device/shared/deny/${id}`, null, { withCredentials: true });
  }

  getUserSharedDevice(): Observable<any> {
    return this.http.get<any>(environment.apiHost + "/device/shared/user", { withCredentials: true });
  }
  
  getSharedProperties(): Observable<any> {
    return this.http.get<any>(environment.apiHost + "/device/shared/properties", { withCredentials: true });
  }

  getSharedDevices(): Observable<any> {
    return this.http.get<any>(environment.apiHost + "/device/shared/devices", { withCredentials: true });
  }

  getSharedDevice(): Observable<any> {
    return this.http.get<any>(environment.apiHost + "/device/shared", { withCredentials: true });
  }

  getSharedRequests(): Observable<any> {
    return this.http.get<any>(environment.apiHost + "/device/shared/requests", { withCredentials: true });
  }

}

export interface CommandDTO
{
  deviceId: number,
  deviceType: number,
  action: string,
  value: string
}