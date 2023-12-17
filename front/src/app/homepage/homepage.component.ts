import { PropertyDTO, PropertyService, ReturnedPropertyDTO } from './../../services/property.service';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AddPropertyDialogComponent } from '../add-property-dialog/add-property-dialog.component';
import { ChooseDeviceTypeDialogComponent } from '../choose-device-type-dialog/choose-device-type-dialog.component';
import { AuthService } from 'src/services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { PropertyDetailsService } from 'src/services/property-details.service';

@Component({
  selector: 'app-homepage',
  templateUrl: './homepage.component.html',
  styleUrls: ['./homepage.component.css']
})
export class HomepageComponent implements OnInit {
  properties: ReturnedPropertyDTO[] = [];
  currentPage = 1;
  pageSize = 4;
  count = 0;

  loggedUser: any = {};

  constructor(private dialog: MatDialog, private propertyService: PropertyService, 
    private authService: AuthService, private snackBar: MatSnackBar,
    private router: Router,
    private propertyDetailsService: PropertyDetailsService) { }

  ngOnInit(): void {
    this.authService.getUser().subscribe({
      next: (value) => {
        if (value) {
          this.loggedUser = value;
        }
      },
      error: (err) => {
        console.log(err);
      },
    })

    this.loadItems();
  }

  loadItems(): void {
    this.propertyService.getPaginatedProperties(this.currentPage, this.pageSize).subscribe({
      next: (value) => {
        console.log(value)
        this.currentPage = value.pageIndex;
        this.count = value.count;
        this.properties = value.items;
      }, 
      error: (err) => {
        console.log(err);
      }
    });
  }

  processRejection(event: any) {
    this.loadItems();
    if (event) {
      
      this.snackBar.open("You have successfully rejected request!", "", {
        duration: 2700, panelClass: ['snack-bar-success']
     });
      console.log("property rejected")
    } 
  }

  onPageChange(event: any): void {
    this.currentPage = event.pageIndex + 1;
    this.loadItems();
  }

  openAddPropertyDialog() {
    const dialogRef = this.dialog.open(AddPropertyDialogComponent);

    dialogRef.afterClosed().subscribe((result) => {
      this.loadItems();
    });
  }
  openPropertyDetails(index: number){
    console.log(this.properties[index]);
    //this.propertyDetailsService.setSelectedProperty(this.properties[index]);
    this.router.navigate(['/property-details', {id: this.properties[index].id}])
  }

  openChooseDeviceTypeDialog() {
    this.dialog.open(ChooseDeviceTypeDialogComponent);
  }


}
