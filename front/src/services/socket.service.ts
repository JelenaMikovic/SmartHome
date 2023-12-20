import { DeviceService } from 'src/services/device.service';
import { Injectable } from '@angular/core';
import {HubConnectionBuilder, HubConnection} from '@microsoft/signalr';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SocketService {

  private hubConnection: HubConnection;

  constructor(private deviceService: DeviceService) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(environment.host + '/deviceHub')
      .build();
  }

  startConnection(deviceId: number) {
    console.log("connecting....")
    this.hubConnection.start().then(() => {
      this.deviceService.subscribeToDataTopic(deviceId).subscribe({
        next(value) {
          console.log(value)
        },
        error(err) {
          console.log(err)
        },
      });
      console.log("connected!")
    }).catch((err) => console.error(err));
  }

  addIlluminanceUpdateListener(callback: (deviceId: string, illuminance: number) => void) {
    this.hubConnection.on('illumUpdate', callback);
  }

  stopConnection() {
    this.hubConnection.stop().catch((err) => console.error(err));
  }
}
