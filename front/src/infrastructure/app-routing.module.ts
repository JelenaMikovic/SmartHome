import { FormControl } from '@angular/forms';
import { NgModule, Component } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from 'src/app/app.component';
import { LoginComponent } from 'src/app/login/login.component';
import { PropertyCardComponent } from 'src/app/property-card/property-card.component';
import { HomepageComponent } from 'src/app/homepage/homepage.component';
import { RegisterComponent } from 'src/app/register/register.component';
import { RegisterAdminComponent } from 'src/app/register-admin/register-admin.component';
import { AmbientalSensorComponent } from 'src/app/ambiental-sensor/ambiental-sensor.component';
import { PropertyDetailsComponent } from 'src/app/property-details/property-details.component';
import { DeviceDetailsComponent } from 'src/app/device-details/device-details.component';
import { SharedTableComponent } from 'src/app/shared-table/shared-table.component';
import { SharedDevicesComponent } from 'src/app/shared-devices/shared-devices.component';

const routes: Routes = [
  {path: 'login', component: LoginComponent},
  {path: 'register', component: RegisterComponent},
  {path: 'home', component: HomepageComponent},
  {path: 'registerAdmin', component: RegisterAdminComponent},
  {path: 'property-details', component: PropertyDetailsComponent},
  {path: 'device-details', component: DeviceDetailsComponent},
  {path: 'sharedTable', component: SharedTableComponent},
  {path: 'sharedDevices', component: SharedDevicesComponent},
  {path: '**', component: LoginComponent},
  {path: '*', component: LoginComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
