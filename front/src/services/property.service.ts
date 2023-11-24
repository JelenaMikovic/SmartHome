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
    return this.http.post<any>(environment.apiHost + "/property", dto, {withCredentials: true});
  }

  getPaginatedProperties(page: number, size: number, ) {
    return this.http.get<any>(environment.apiHost + "/property/paginated?page=" + page +"&size=" + size, {withCredentials: true});
  }

  acceptPropertyRequest(id: number) {
    return this.http.put<any>(environment.apiHost + "/property/accept/" + id, {withCredentials: true});
  }

  denyPropertyRequest(id: number, reason: string) {
    return this.http.put<any>(environment.apiHost + "/property/deny/" + id, {withCredentials: true});
  }
}

export interface PropertyDTO {
  name: string,
  area: number,
  numOfFloors: number,
  image: string,
  address: AddressDTO
}

export interface ReturnedPropertyDTO {
  id: number,
  name: string,
  area: number,
  numOfFloors: number,
  image: string,
  status: string,
  address: ReturendAddressDTO,
  owner?: UserDTO
}

export interface AddressDTO {
  cityId: number,
  lat: number,
  lng: number, 
  name: string
}

export interface ReturendAddressDTO {
  city: string,
  country: string,
  lat: number,
  lng: number, 
  name: string,
  id: number
}

export interface PageResultDTO {
  count: number,
  pageIndex: number,
  pageSize: number,
  items: any[]
}

export interface UserDTO {
  name: string,
  surname: string,
  email: string, 
  id: number,
  role: string,
  isActivated: boolean
}
