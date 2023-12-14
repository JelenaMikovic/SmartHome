import { ComponentRef, NgModule, OnInit } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from '../infrastructure/app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from 'src/infrastructure/material.module';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldModule } from '@angular/material/form-field';
import { NgxDaterangepickerMd } from 'ngx-daterangepicker-material';
import {MatSnackBarModule} from '@angular/material/snack-bar'
import {NgxMaterialTimepickerModule} from 'ngx-material-timepicker'
import { GooglePlaceModule } from "ngx-google-places-autocomplete";
import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { NgxStarRatingModule } from 'ngx-star-rating';
import { CdTimerModule } from 'angular-cd-timer';
import { SelectDropDownModule } from 'ngx-select-dropdown'
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AddPropertyDialogComponent } from './add-property-dialog/add-property-dialog.component';
import { HomepageComponent } from './homepage/homepage.component';
import { LoginComponent } from './login/login.component';
import { SideNavbarComponent } from './side-navbar/side-navbar.component';
import { ChooseDeviceTypeDialogComponent } from './choose-device-type-dialog/choose-device-type-dialog.component';
import { AddDeviceDialogComponent } from './add-device-dialog/add-device-dialog.component';
import { MapComponent } from './map/map.component';
import { PropertyCardComponent } from './property-card/property-card.component';


import { JwtModule } from "@auth0/angular-jwt";
import { RejectPropertyDialogComponent } from './reject-property-dialog/reject-property-dialog.component';
import { RegisterComponent } from './register/register.component';
import { CodeDialogComponent } from './code-dialog/code-dialog.component';
import { ChangePasswordComponent } from './chang-password/chang-password.component';
import { RegisterAdminComponent } from './register-admin/register-admin.component';
import { AmbientalSensorComponent } from './ambiental-sensor/ambiental-sensor.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    AddPropertyDialogComponent,
    HomepageComponent,
    SideNavbarComponent,
    ChooseDeviceTypeDialogComponent,
    AddDeviceDialogComponent,
    PropertyCardComponent,
    MapComponent,
    PropertyCardComponent,
    RejectPropertyDialogComponent,
    CodeDialogComponent,
    ChangePasswordComponent,
    RegisterAdminComponent,
    AmbientalSensorComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    FormsModule,
    MaterialModule,
    HttpClientModule,
    NgxDaterangepickerMd.forRoot(),
    HttpClientModule, MatSnackBarModule,
    NgxMaterialTimepickerModule,
    GooglePlaceModule,
    CommonModule,
    CdTimerModule,
    NgxStarRatingModule,
    SelectDropDownModule,

    JwtModule.forRoot({
      config: {
        tokenGetter: () => {
          return localStorage.getItem("access_token");
        },
        allowedDomains: ["your_backend_domain"],
        disallowedRoutes: ["your_backend_domain/api/auth"] }})
  ],
  providers: [
    { provide: MAT_DIALOG_DATA, useValue: {} },
    {
      provide: MatDialogRef,
      useValue: {}
    },
    { provide: MAT_FORM_FIELD_DEFAULT_OPTIONS, useValue: { appearance: 'outline', hideRequiredMarker: 'true' }}
    ,],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  bootstrap: [AppComponent]
})
export class AppModule { 
}
