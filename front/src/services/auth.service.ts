import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { BehaviorSubject, catchError, map, of, tap, throwError } from 'rxjs';
import { Observable } from 'rxjs/internal/Observable';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private loggedInSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  loggedIn$: Observable<boolean> = this.loggedInSubject.asObservable();


  constructor(private http: HttpClient, public jwtHelper: JwtHelperService) {}

  login(credentials: any): Observable<any> {
    return this.http.post<any>(environment.apiHost + '/user/login', credentials, { withCredentials: true })
      .pipe(
        tap(response => {
          this.loggedInSubject.next(response.email ? true : false);
          return response;
        }),
        catchError((error: HttpErrorResponse) => {
          console.error('Login error:', error);
          return throwError(error);
        })
      );
  }

  register(form: any): Observable<any> {
    return this.http.post<any>(environment.apiHost + '/user/register', form)
      .pipe(
        map(response => {
          if (response) {
            return true;
          } else {
            return false;
          }
        })
      );
  }

  activate(dto: any): Observable<any> {
    return this.http.post<any>(environment.apiHost + '/user/login', dto)
      .pipe(
        tap(response => {
          this.loggedInSubject.next(response.email ? true : false);
        }),
        map(response => response.email ? true : false),
        catchError((error: HttpErrorResponse) => {
          console.error('Login error:', error);
          return throwError(error);
        })
      );
  }

  logout(): Observable<any> {
    return this.http.post<any>(environment.apiHost + '/user/logout', {}, { withCredentials: true })
      .pipe(
        tap(response => {
          this.loggedInSubject.next(false);
          sessionStorage.removeItem("loggedIn");
          console.log('Logout response:', response);
          console.log('Logged out successfully.');
        }),
        catchError((error: HttpErrorResponse) => {
          console.error('Logout error:', error);
          return throwError(error);
        })
      );
  }

  isAuthenticated(): Observable<boolean> {
    return this.http.get<any>(environment.apiHost + '/user/authenticate', { withCredentials: true })
      .pipe(
        tap(response => {
          this.loggedInSubject.next(response.email ? true : false);
        }),
        map(response => response.email ? true : false),
        catchError((error: any) => {
          console.error('Authentication error:', error);
          return of(false);
        })
      );
  }

  checkAuthenticationStatus(): void {
    this.isAuthenticated().subscribe();
  }

  getUser(): Observable<any> {
    return this.http.get<any>(environment.apiHost + '/user/authenticate', { withCredentials: true })
      .pipe(
        map(response => response.email ? response : null)
      );
  }

  changePassword(dto: any) {
    return this.http.post<any>(environment.apiHost + '/user/changePassword', dto, { withCredentials: true })
      .pipe(
        tap(response => {
          return true;
        }),
        catchError((error: HttpErrorResponse) => {
          console.error('Login error:', error);
          return throwError(error);
        })
      );
  }
}