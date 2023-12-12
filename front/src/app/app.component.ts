import { AuthService } from 'src/services/auth.service';
import { NavbarService } from './../services/navbar.service';
import { Component, HostListener } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {


  title = "La Casa De Smart";
  loggedIn: boolean = false; 
  sideVisible: Boolean = false;
  smallScreen: boolean = window.innerWidth < 900;
  private destroy$: Subject<void> = new Subject<void>();
  
  constructor(private navbarService: NavbarService, private authService: AuthService) {
    this.navbarService.getSideVisible().subscribe(value => {
      this.sideVisible = value;
    })
  }

  ngOnInit(): void {
    this.authService.loggedIn$.pipe(takeUntil(this.destroy$)).subscribe((isAuthenticated: boolean) => {
      this.loggedIn = isAuthenticated;
      this.sideVisible = isAuthenticated;
    });

    // Check authentication status on component initialization
    this.authService.checkAuthenticationStatus();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: Event): void {
    this.smallScreen = window.innerWidth < 900;
  }

}
