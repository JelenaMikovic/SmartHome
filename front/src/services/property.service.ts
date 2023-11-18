import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PropertyService {

  constructor(private http: HttpClient) { }

  addProperty(dto: PropertyDTO) {
    return this.http.post<any>(environment.apiHost + "/property", dto);
  }
}

export interface PropertyDTO {
  name: string,
  area: number,
  numOfFloors: number,
  image: string
}
