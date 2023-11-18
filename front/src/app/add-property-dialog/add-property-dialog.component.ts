import { PropertyDTO, PropertyService } from './../../services/property.service';
import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { areaValidator, floorsValidator, nameValidator } from '../validators/property-validators';
import { markFormControlsTouched } from '../validators/formGroupValidators';

@Component({
  selector: 'app-add-property-dialog',
  templateUrl: './add-property-dialog.component.html',
  styleUrls: ['./add-property-dialog.component.css']
})
export class AddPropertyDialogComponent implements OnInit {

  filePath: string = "";
  file: File = {} as File;

  addPropertyForm = new FormGroup({
    name: new FormControl('', [Validators.required, nameValidator]),
    area: new FormControl('', [Validators.required, areaValidator]),
    numberOfFloors: new FormControl('', [Validators.required, floorsValidator]),
  })

  constructor(
    public dialogRef: MatDialogRef<AddPropertyDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private snackBar: MatSnackBar,
    private propertyService: PropertyService
  ) { }

  ngOnInit(): void {
    markFormControlsTouched(this.addPropertyForm);
  }

  addProperty() {
    if (this.addPropertyForm.valid && this.filePath != "") {
      let dto: PropertyDTO = {
        name: this.addPropertyForm.value.name!,
        area: +this.addPropertyForm.value.area!,
        numOfFloors: +this.addPropertyForm.value.numberOfFloors!,
        image: this.filePath
      }
      this.propertyService.addProperty(dto).subscribe({
        next(value) {
          console.log(value);
        },
        error(err) {
          console.log(err);
        },
      })
    }
  }

  onFileSelect(event: any) {
    event.preventDefault();

    if (event.target.files){
      var reader = new FileReader();
      this.file = event.target.files[0];
      reader.readAsDataURL(this.file);
      reader.onload=(e: any)=>{
        event.preventDefault();
        this.filePath = reader.result as string;
      }
    }
  }

  close() {
    this.dialogRef.close();
  }

}
