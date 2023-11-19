import { LocationDTO, LocationService } from './../../services/location.service';
import { PropertyDTO, PropertyService } from './../../services/property.service';
import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { areaValidator, floorsValidator, nameValidator } from '../validators/property-validators';
import { markFormControlsTouched } from '../validators/formGroupValidators';
import { NgxDropdownConfig } from 'ngx-select-dropdown';
import { Observable, map, startWith } from 'rxjs';

@Component({
  selector: 'app-add-property-dialog',
  templateUrl: './add-property-dialog.component.html',
  styleUrls: ['./add-property-dialog.component.css']
})
export class AddPropertyDialogComponent implements OnInit {

  filePath: string = "";
  file: File = {} as File;

  options: LocationDTO[] = [];
  filteredOptions: Observable<LocationDTO[]> = new Observable();

  addPropertyForm = new FormGroup({
    name: new FormControl('', [Validators.required, nameValidator]),
    area: new FormControl('', [Validators.required, areaValidator]),
    numberOfFloors: new FormControl('', [Validators.required, floorsValidator]),
    cityAndCountry: new FormControl('', [Validators.required])
  })

  constructor(
    public dialogRef: MatDialogRef<AddPropertyDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private snackBar: MatSnackBar,
    private propertyService: PropertyService,
    private locationService: LocationService
  ) { }

  ngOnInit(): void {
    this.locationService.getAll().subscribe({
        next: (value) => {
            this.options = value;
            this.filteredOptions = this.addPropertyForm.get('cityAndCountry')!.valueChanges.pipe(
                startWith(''),
                map(value => {
                    const location = typeof value === 'string' ? value : (value as unknown as LocationDTO).location;
                    return location ? this.filter(location) : this.options.slice();
                })
            );
        },
        error: (err) => {
            console.log(err);
        },
    });
    markFormControlsTouched(this.addPropertyForm);
  }

  displayLocation(value: LocationDTO): string {
    return value.location;
  }

  filter(value: string): LocationDTO[] {
    console.log(value)
      return this.options.filter(option =>
          option.location.toLowerCase().includes(value.toLowerCase())
      );
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

  selectionChanged(event: any) {
    console.log(event);
  }

  close() {
    this.dialogRef.close();
  }

}
