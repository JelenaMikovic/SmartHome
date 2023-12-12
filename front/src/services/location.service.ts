import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class LocationService {

  constructor(private http: HttpClient) { }

  public getAll(): Observable<any> {
    const options: any = {
      responseType: 'json',
      withCredentials: true
    };
    return this.http.get<LocationDTO[]>(environment.apiHost + "/location", options);
  }
}

export interface LocationDTO {
  location: string,
  cityId: number
}
