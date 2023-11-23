import { PropertyDTO, PropertyService } from './../../services/property.service';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AddPropertyDialogComponent } from '../add-property-dialog/add-property-dialog.component';

@Component({
  selector: 'app-homepage',
  templateUrl: './homepage.component.html',
  styleUrls: ['./homepage.component.css']
})
export class HomepageComponent implements OnInit {
  properties: PropertyDTO[] = [];
  currentPage = 1;
  pageSize = 3;
  count = 0;

  constructor(private dialog: MatDialog, private propertyService: PropertyService) { }

  ngOnInit(): void {
    this.loadItems();
  }

  loadItems(): void {
    this.propertyService.getPaginatedProperties(this.currentPage, this.pageSize).subscribe({
      next: (value) => {
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
    this.dialog.open(AddPropertyDialogComponent);
  }

}
