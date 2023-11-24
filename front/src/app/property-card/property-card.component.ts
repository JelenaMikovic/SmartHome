import { PropertyService } from './../../services/property.service';
import { Router } from '@angular/router';
import { AuthService } from 'src/services/auth.service';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { PropertyDTO, ReturnedPropertyDTO, UserDTO } from 'src/services/property.service';
import { MatDialog } from '@angular/material/dialog';
import { RejectPropertyDialogComponent } from '../reject-property-dialog/reject-property-dialog.component';

@Component({
  selector: 'app-property-card',
  templateUrl: './property-card.component.html',
  styleUrls: ['./property-card.component.css']
})
export class PropertyCardComponent implements OnInit {

  loggedUser: any = {};

  @Input() property: ReturnedPropertyDTO = {} as ReturnedPropertyDTO;
  @Output() rejectionCompleted = new EventEmitter<any>();

  constructor(private dialog: MatDialog, private authService: AuthService, private router: Router, private propertyService: PropertyService) {

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
    const dialogRef = this.dialog.open(RejectPropertyDialogComponent, { data: { propertyId: this.property.id } });

    dialogRef.afterClosed().subscribe((result) => {
      this.rejectionCompleted.emit(true);
    });
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
