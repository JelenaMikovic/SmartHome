import { FormControl } from '@angular/forms';
import { NgModule, Component } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from 'src/app/app.component';
import { LoginComponent } from 'src/app/login/login.component';
import { HomepageComponent } from 'src/app/homepage/homepage.component';
import { RegisterComponent } from 'src/app/register/register.component';
import { RegisterAdminComponent } from 'src/app/register-admin/register-admin.component';

const routes: Routes = [
  {path: 'login', component: LoginComponent},
  {path: 'register', component: RegisterComponent},
  {path: 'home', component: HomepageComponent},
  {path: 'registerAdmin', component: RegisterAdminComponent},
  {path: '*', component: LoginComponent},
  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
