import { DeviceService } from 'src/services/device.service';
import { Injectable } from '@angular/core';
import {HubConnectionBuilder, HubConnection} from '@microsoft/signalr';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SocketService {

  private hubConnection: HubConnection;

  constructor() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(environment.host + '/deviceHub')
      .build();
  }

  startConnection(deviceId: number) {
    console.log("connecting....")
    this.hubConnection.start().then(() => {
      this.hubConnection.invoke("SubscribeToDataTopic", deviceId).then(() => console.log("subscribed to data topic")).catch(() => {console.log("Error connecting to socket...")});
      console.log("connected!")
    }).catch((err) => console.error(err));
  }

  addDataUpdateListener(callback: (dto: DataDTO) => void) {
    this.hubConnection.on('DataUpdate', (dataString: string) => {
      // Parse the string into a DataDTO object
      const dataObject: DataDTO = JSON.parse(dataString);
      // Invoke the callback with the deserialized object
      callback(dataObject);
  });
  }

  stopConnection() {
    this.hubConnection.stop().catch((err) => console.error(err));
  }
}

export interface DataDTO {
  deviceId: number,
  deviceType: string,
  value: number
}