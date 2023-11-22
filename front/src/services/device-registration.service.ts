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
    return this.http.post<any>(environment.apiHost + "/device-registration/ambient-sensor", ambientSensor, {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      })
    });
  }

  registerAC(ac: AirConditionerRegistrationDTO): Observable<any> {
    console.log(environment.apiHost);
    return this.http.post<any>(environment.apiHost + "/device-registration/ac", ac, {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      })
    });
  }

  registerWashingMachine(wm: WashingMachineRegistrationDTO): Observable<any> {
    console.log(environment.apiHost);
    return this.http.post<any>(environment.apiHost + "/device-registration/washing-machine", wm, {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      })
    });
  }

  registerSolarPanel(panel: SolarPanelRegistrationDTO): Observable<any> {
    console.log(environment.apiHost);
    return this.http.post<any>(environment.apiHost + "/device-registration/solar-panel", panel, {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      })
    });
  }

  registerLamp(lamp: LampRegistrationDTO): Observable<any> {
    console.log(environment.apiHost);
    return this.http.post<any>(environment.apiHost + "/device-registration/lamp", lamp, {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      })
    });
  }

  registerVehicleGate(gate: VehicleGateRegistrationDTO): Observable<any> {
    console.log(environment.apiHost);
    return this.http.post<any>(environment.apiHost + "/device-registration/vehicle-gate", gate, {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      })
    });
  }

  registerHomeBattery(battery: HomeBatteryRegistrationDTO): Observable<any> {
    console.log(environment.apiHost);
    return this.http.post<any>(environment.apiHost + "/device-registration/home-battery", battery, {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      })
    });
  }

  registerIrrigationSystem(irrigationSystem: IrrigationSystemRegistrationDTO): Observable<any> {
    console.log(environment.apiHost);
    return this.http.post<any>(environment.apiHost + "/device-registration/irrigation-system", irrigationSystem, {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      })
    });
  }

  registerEVCharger(charger: EVChargerRegistrationDTO): Observable<any> {
    console.log(environment.apiHost);
    return this.http.post<any>(environment.apiHost + "/device-registration/evcharger", charger, {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      })
    });
  }
}  

export interface ResponseMessageDTO {
  message: string

}