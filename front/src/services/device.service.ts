import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
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
}