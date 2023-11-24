import { AuthService } from 'src/services/auth.service';
import { Component, Input, OnInit } from '@angular/core';
import { PropertyDTO, ReturnedPropertyDTO } from 'src/services/property.service';

@Component({
  selector: 'app-property-card',
  templateUrl: './property-card.component.html',
  styleUrls: ['./property-card.component.css']
})
export class PropertyCardComponent implements OnInit {

  @Input() property: ReturnedPropertyDTO = {} as ReturnedPropertyDTO;

  constructor(private authService: AuthService) {

  }

  ngOnInit(): void {
  }

  denyRequest() {

  }

  acceptRequest() {

  }

}
