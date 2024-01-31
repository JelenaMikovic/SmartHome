import { Component, OnInit, ViewChild } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ConsoleLogger } from '@microsoft/signalr/dist/esm/Utils';
import { DeviceService } from 'src/services/device.service';

@Component({
  selector: 'app-shared-table',
  templateUrl: './shared-table.component.html',
  styleUrls: ['./shared-table.component.css']
})
export class SharedTableComponent implements OnInit {

  @ViewChild(MatSort) sort: any;
  dataSource = new MatTableDataSource<any>;
  displayedColumns: string[] = ['email', 'type', 'name', 'status', 'changeStatus'];

  constructor(private deviceService: DeviceService, private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.getData();
  }

  getData(): void {
    this.deviceService.getSharedDevice().subscribe(response => {
      console.log(response);
      this.dataSource.data = response;
    });
  
  }

  revoke(elemnt: any): void{
    console.log(elemnt.id)
    this.deviceService.denySharedDevice(elemnt.id).subscribe({
      next: (value) => {
        this.snackBar.open("Access revoked!", "", {
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
