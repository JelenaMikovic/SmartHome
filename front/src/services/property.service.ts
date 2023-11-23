import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PropertyService {

  constructor(private http: HttpClient) { }

  addProperty(dto: PropertyDTO) {
    console.log(dto);
    return this.http.post<any>(environment.apiHost + "/property", dto);
  }

  getPaginatedProperties(page: number, size: number) {
    return this.http.get<any>(environment.apiHost + "/property/paginated?page=" + page +"&size=" + size);
  }
}

export interface PropertyDTO {
  name: string,
  area: number,
  numOfFloors: number,
  image: string,
  address: AddressDTO
}

export interface AddressDTO {
  cityId: number,
  lat: number,
  lng: number, 
  name: string
}

export interface PageResultDTO {
  count: number,
  pageIndex: number,
  pageSize: number,
  items: any[]
}
