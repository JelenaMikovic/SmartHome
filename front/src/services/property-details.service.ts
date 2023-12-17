import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class PropertyDetailsService {

    private selectedProperty = new BehaviorSubject<any>({});

    constructor(private http: HttpClient) {}

    getSelectedProperty(): Observable<any> {
        return this.selectedProperty.asObservable();
    }

    setSelectedProperty(user: any): void {
        this.selectedProperty.next(user);
    }
}