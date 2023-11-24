import { PropertyService, ReasonDTO } from './../../services/property.service';
import { Component, OnInit, Inject, Output, EventEmitter } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';

@Component({
  selector: 'app-reject-property-dialog',
  templateUrl: './reject-property-dialog.component.html',
  styleUrls: ['./reject-property-dialog.component.css']
})
export class RejectPropertyDialogComponent implements OnInit {

  rejectionReasonForm = new FormGroup({
    reason: new FormControl('', [Validators.required])
  });


  constructor(public dialogRef: MatDialogRef<RejectPropertyDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private router: Router,
    private propertyService: PropertyService)
    { }

  ngOnInit(): void {
  }

  submitRejectionReason() {
    this.rejectionReasonForm.updateValueAndValidity();
    if (this.rejectionReasonForm.valid) {
      let reason: ReasonDTO = {
        reason: this.rejectionReasonForm.value.reason!
      }
      this.propertyService.denyPropertyRequest(this.data.propertyId, reason).subscribe({
        next: (value) => {
          console.log(value);
          this.dialogRef.close();
        },
        error: (err) => {
          console.log(err)
        }
      });
    }
  }

}
