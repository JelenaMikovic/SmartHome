import { FormControl } from '@angular/forms';
import { NgModule, Component } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from 'src/app/app.component';
import { LoginComponent } from 'src/app/login/login.component';
import { PropertyCardComponent } from 'src/app/property-card/property-card.component';

const routes: Routes = [
  {path: '', component: PropertyCardComponent},
  {path: '**', component: PropertyCardComponent},
  {path: 'login', component: LoginComponent},
  {path:"pls", component:PropertyCardComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
