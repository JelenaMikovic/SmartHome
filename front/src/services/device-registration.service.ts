import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { AirConditionerRegistrationDTO, AmbientSensorRegistrationDTO, EVChargerRegistrationDTO, HomeBatteryRegistrationDTO, IrrigationSystemRegistrationDTO, LampRegistrationDTO, SolarPanelRegistrationDTO, VehicleGateRegistrationDTO, WashingMachineRegistrationDTO } from 'src/model/device-registration';


@Injectable({
  providedIn: 'root'
})
export class DeviceRegistrationService {

  constructor(private http:HttpClient) { }

  registerAmbientSensor(ambientSensor: AmbientSensorRegistrationDTO): Observable<any> {
    console.log(environment.apiHost);
    return this.http.post<any>(environment.apiHost + "/device-registration/ambient-sensor", ambientSensor, {withCredentials: true});
  }

  registerAC(ac: AirConditionerRegistrationDTO): Observable<any> {
    console.log(environment.apiHost);
    return this.http.post<any>(environment.apiHost + "/device-registration/ac", ac, {withCredentials: true});
  }

  registerWashingMachine(wm: WashingMachineRegistrationDTO): Observable<any> {
    console.log(environment.apiHost);
    return this.http.post<any>(environment.apiHost + "/device-registration/washing-machine", wm, {withCredentials: true});
  }

  registerSolarPanel(panel: SolarPanelRegistrationDTO): Observable<any> {
    console.log(environment.apiHost);
    return this.http.post<any>(environment.apiHost + "/device-registration/solar-panel", panel, {withCredentials: true});
  }

  registerLamp(lamp: LampRegistrationDTO): Observable<any> {
    console.log(environment.apiHost);
    return this.http.post<any>(environment.apiHost + "/device-registration/lamp", lamp, {withCredentials: true});
  }

  registerVehicleGate(gate: VehicleGateRegistrationDTO): Observable<any> {
    console.log(environment.apiHost);
    return this.http.post<any>(environment.apiHost + "/device-registration/vehicle-gate", gate, {withCredentials: true});
  }

  registerHomeBattery(battery: HomeBatteryRegistrationDTO): Observable<any> {
    console.log(environment.apiHost);
    return this.http.post<any>(environment.apiHost + "/device-registration/home-battery", battery , {withCredentials: true});
  }

  registerIrrigationSystem(irrigationSystem: IrrigationSystemRegistrationDTO): Observable<any> {
    console.log(environment.apiHost);
    return this.http.post<any>(environment.apiHost + "/device-registration/irrigation-system", irrigationSystem, {withCredentials: true});
  }

  registerEVCharger(charger: EVChargerRegistrationDTO): Observable<any> {
    console.log(environment.apiHost);
    return this.http.post<any>(environment.apiHost + "/device-registration/evcharger", charger, {withCredentials: true});
  }
}  

export interface ResponseMessageDTO {
  message: string

}