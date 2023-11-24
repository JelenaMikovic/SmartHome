import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from './../../services/auth.service';
import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject, fromEvent } from 'rxjs';
import { debounceTime, takeUntil } from 'rxjs/operators';
import { NavbarService } from 'src/services/navbar.service';

@Component({
  selector: 'app-side-navbar',
  templateUrl: './side-navbar.component.html',
  styleUrls: ['./side-navbar.component.css']
})
export class SideNavbarComponent implements OnInit, OnDestroy {

  url = "/home";
  private destroy$: Subject<void> = new Subject<void>();

  constructor(private navService: NavbarService, private router: Router, private authService: AuthService, private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.handleSmallScreens();
  }

  handleSmallScreens(): void {
    (<HTMLButtonElement>document.querySelector('.navbar-toggler'))
      .addEventListener('click', () => {
      let navbarMenu = <HTMLDivElement>document.querySelector('#side-navbar-container')
      let navbarMenuSmaller = <HTMLDivElement>document.querySelector('#navbar-smaller')
      let closeBtn = <HTMLDivElement>document.querySelector('#close-btn i')
  
      if (navbarMenu.style.display === 'block') {
        navbarMenu.style.display = 'none';
        closeBtn.style.display = 'none';
        navbarMenuSmaller.style.display = 'block';
        this.navService.setSideVisible(false);
        return ;
      }
  
      navbarMenu.style.display = 'block';
      closeBtn.style.display = 'block';

      navbarMenuSmaller.style.display = 'none';
      this.navService.setSideVisible(true);
    })
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: Event): void {
    let navbarMenu = <HTMLDivElement>document.querySelector('#side-navbar-container')
    let navbarMenuSmaller = <HTMLDivElement>document.querySelector('#navbar-smaller')
    let closeBtn = <HTMLDivElement>document.querySelector('#close-btn i')

    let windowWidth = (event.target as Window).innerWidth;
    

    if (windowWidth > 900) {
      navbarMenu.style.display = 'block';
      navbarMenuSmaller.style.display = 'none';
      closeBtn.style.display = 'none';
      this.navService.setSideVisible(true);
    } else {
      navbarMenu.style.display = 'none';
      navbarMenuSmaller.style.display = 'block'
      closeBtn.style.display = 'block';
      this.navService.setSideVisible(false);
    }

  }

  openHome() {
    this.router.navigate(["/home"]);
    this.url = "/home";
  }

  close() {
    let navbarMenu = <HTMLDivElement>document.querySelector('#side-navbar-container');
    let navbarMenuSmaller = <HTMLDivElement>document.querySelector('#navbar-smaller');
    let closeBtn = <HTMLDivElement>document.querySelector('#close-btn i');

    navbarMenu.style.display = 'none';
    closeBtn.style.display = 'none';

    navbarMenuSmaller.style.display = 'block';
    this.navService.setSideVisible(false);
  }

  logout() {
    this.authService.logout().subscribe(
      (success) => {
        this.router.navigate(['login']);
      },
      (error) => {
        this.snackBar.open('An error occurred while logging out', 'Close', { duration: 3000 });
      }
    );
   
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

}
