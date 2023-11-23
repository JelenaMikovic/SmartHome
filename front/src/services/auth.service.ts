import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { catchError, map, throwError } from 'rxjs';
import { Observable } from 'rxjs/internal/Observable';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient, public jwtHelper: JwtHelperService) {}

  login(credentials: any): Observable<boolean> {
    return this.http.post<any>(environment.apiHost + '/user/login', credentials, {withCredentials: true})
      .pipe(
        map(response => {
          if (response.email) {
            return true;
          } else {
            return false;
          }
        })
      );
  }

  logout(): Observable<any> {
    return this.http.post<any>(environment.apiHost + '/user/logout', {}, {withCredentials: true}).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error('Logout error:', error);
        return throwError(error);
      })
    );
  }

  isAuthenticated(): Observable<any> {
    return this.http.get<any>(environment.apiHost + '/user/authenticate', {withCredentials: true}).pipe(
      map(response => {
        console.log(response)
        if (response.email) {
          return true;
        } else {
          return false;
        }
      })
    );
  }
}