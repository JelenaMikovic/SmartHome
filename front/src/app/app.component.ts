import { NavbarService } from './../services/navbar.service';
import { Component, HostListener } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  title = "Le Casa De Smart";
  loggedIn: boolean = false; 
  sideVisible: Boolean = false;
  smallScreen: boolean = window.innerWidth < 900;

  constructor(private navbarService: NavbarService) {
    this.navbarService.getSideVisible().subscribe(value => {
      this.sideVisible = value;
    })
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: Event): void {
    this.smallScreen = window.innerWidth < 900;
  }


}
