import { PropertyDTO, PropertyService } from '../../services/property.service';
import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { DeviceService } from 'src/services/device.service';


@Component({
  selector: 'app-share-requests-dialog',
  templateUrl: './share-requests-dialog.component.html',
  styleUrls: ['./share-requests-dialog.component.css']
})
export class ShareRequestsDialogComponent implements OnInit {

  @ViewChild(MatSort) sort: any;
  dataSource = new MatTableDataSource<any>;
  displayedColumns: string[] = ['email', 'type', 'name', 'status', 'accept', 'deny'];


  constructor(
    public dialogRef: MatDialogRef<ShareRequestsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private snackBar: MatSnackBar,
    private propertyService: PropertyService,
    private dialog: MatDialog,
    private deviceService: DeviceService
  ) { 
  }

  ngOnInit(): void {
    this.getData();
  }

  close() {
    this.dialogRef.close();
  }

  getData(): void {
    this.deviceService.getSharedRequests().subscribe(response => {
      console.log(response);
      this.dataSource.data = response;
    });
  
  }

  revoke(elemnt: any): void{
    console.log(elemnt.id)
    this.deviceService.denySharedDevice(elemnt.id).subscribe({
      next: (value) => {
        this.snackBar.open("Denied invitation!", "", {
          duration: 2700, panelClass: ['snack-bar-success']
        });
        this.getData();
      },
      error: (err) => {
        this.snackBar.open(err, "", {
          duration: 2700, panelClass: ['snack-bar-server-error']
        });
        console.log(err);
      },
    })
  }

  accept(elemnt: any): void{
    console.log(elemnt.id)
    this.deviceService.acceptSharedDevice(elemnt.id).subscribe({
      next: (value) => {
        this.snackBar.open("Accepted invitation!", "", {
          duration: 2700, panelClass: ['snack-bar-success']
        });
        this.getData();
      },
      error: (err) => {
        this.snackBar.open(err, "", {
          duration: 2700, panelClass: ['snack-bar-server-error']
        });
        console.log(err);
      },
    })
  }

  applyNameFilter(event: Event): void {
    const filter = (event.target as HTMLInputElement).value.trim().toLocaleLowerCase();
    this.dataSource.filter = filter;
  }


}
