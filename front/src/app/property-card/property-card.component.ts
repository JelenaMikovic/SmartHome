import { PropertyService } from './../../services/property.service';
import { Router } from '@angular/router';
import { AuthService } from 'src/services/auth.service';
import { Component, Input, OnInit } from '@angular/core';
import { PropertyDTO, ReturnedPropertyDTO, UserDTO } from 'src/services/property.service';

@Component({
  selector: 'app-property-card',
  templateUrl: './property-card.component.html',
  styleUrls: ['./property-card.component.css']
})
export class PropertyCardComponent implements OnInit {

  loggedUser: any = {};

  @Input() property: ReturnedPropertyDTO = {} as ReturnedPropertyDTO;

  constructor(private authService: AuthService, private router: Router, private propertyService: PropertyService) {

  }

  ngOnInit(): void {
    this.authService.getUser().subscribe({
      next: (value) => {
        if (value) {
          this.loggedUser = value;
          console.log(this.loggedUser)
        }
        else
          this.router.navigate(["/login"]);
      },
      error: (err) => {
        console.log(err);
      },
    })
  }

  denyRequest() {

  }

  acceptRequest() {
    this.propertyService.acceptPropertyRequest(this.property.id).subscribe({
      next: (value) => {
        console.log(value);
      },
      error: (err) => {
        console.log(err);
      },
    })
  }

}
