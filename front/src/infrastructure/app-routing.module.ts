import { FormControl } from '@angular/forms';
import { NgModule, Component } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from 'src/app/app.component';
import { LoginComponent } from 'src/app/login/login.component';
import { HomepageComponent } from 'src/app/homepage/homepage.component';

const routes: Routes = [
  {path: 'login', component: LoginComponent},
  {path: 'home', component: HomepageComponent},
  {path: '*', component: LoginComponent},
  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
