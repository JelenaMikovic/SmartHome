import { HttpClient } from '@angular/common/http';
import { MapComponent } from './../app/map/map.component';
import { Injectable } from '@angular/core';
import { Loader } from '@googlemaps/js-api-loader';
import { Observable, Subject } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MapService {

  private loader = new Loader({
    apiKey: environment.googleApiKey
  });
  
  geocoder: google.maps.Geocoder = {} as google.maps.Geocoder;

  public currentCityLocation = new Subject<LatLng>();

  public constructor(private http: HttpClient) {

  }

  public getLoader() {
      return this.loader;
  }

  getCityCoordinates(): Observable<LatLng> {
    return this.currentCityLocation.asObservable();
  }

  setCityCoordinates(city: string, state: string) : any {
    let request: google.maps.GeocoderRequest = {
      address: `${city}, ${state}`
    };
    this.geocoder.geocode(request, (response, status) => {
      if (status == google.maps.GeocoderStatus.OK) {
        const location = response![0].geometry?.location;
          if (location) {
            this.currentCityLocation.next({ lat: location.lat(), lng: location.lng() });
          }
      } else {
        alert("Sorry, we are not able to decode the chosen city coordinates :(");
      } 
    })
  }

  decodeAddress(address: string, city: string, state: string) : Observable<LatLng> {
    let request: google.maps.GeocoderRequest = {
      address: `${address}, ${city}, ${state}`
    };
    
    return new Observable<LatLng>((observer) => {
    this.geocoder.geocode(request, (response, status) => {
      if (status == google.maps.GeocoderStatus.OK) {
        const location = response![0].geometry?.location;
          if (location) {
            observer.next({ lat: location.lat(), lng: location.lng() });
            observer.complete();
          }
      } else {
        observer.error("Geodecode error");
      } 
      });
    });
  }
  
  decodeCoordinates(latLng: LatLng): Observable<string> {
    const request: google.maps.GeocoderRequest = {
      location: latLng,
    };
  
    return new Observable<string>((observer) => {
      this.geocoder.geocode(request, (response, status) => {
        if (status === google.maps.GeocoderStatus.OK) {
          const address = response?.[0]?.formatted_address || '';
          observer.next(address);
          observer.complete();
        } else {
          observer.error("Geocoder error");
        }
      });
    });
  }
}

export interface LatLng {
  lat: number,
  lng: number
}
