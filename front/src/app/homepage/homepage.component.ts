import { PropertyDTO, PropertyService, ReturnedPropertyDTO } from './../../services/property.service';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AddPropertyDialogComponent } from '../add-property-dialog/add-property-dialog.component';
import { AuthService } from 'src/services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';

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

  constructor(private dialog: MatDialog, private propertyService: PropertyService, private authService: AuthService, private snackBar: MatSnackBar) { }

  ngOnInit(): void {
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

  x(){
    
  }
}
