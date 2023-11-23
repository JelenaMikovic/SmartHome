import { LatLng, MapService } from './../../services/map.service';
import { LocationDTO, LocationService } from './../../services/location.service';
import { AddressDTO, PropertyDTO, PropertyService } from './../../services/property.service';
import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { areaValidator, cityValidator, floorsValidator, nameValidator } from '../validators/property-validators';
import { markFormControlsTouched } from '../validators/formGroupValidators';
import { NgxDropdownConfig } from 'ngx-select-dropdown';
import { Observable, map, startWith } from 'rxjs';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

@Component({
  selector: 'app-add-property-dialog',
  templateUrl: './add-property-dialog.component.html',
  styleUrls: ['./add-property-dialog.component.css']
})
export class AddPropertyDialogComponent implements OnInit {

  filePath: string = "";
  file: File = {} as File;
  selectedMarkerPosition: LatLng = {} as LatLng;

  options: LocationDTO[] = [];
  filteredOptions: Observable<LocationDTO[]> = new Observable();

  addPropertyForm = new FormGroup({
    name: new FormControl('', [Validators.required, nameValidator]),
    area: new FormControl('', [Validators.required, areaValidator]),
    numberOfFloors: new FormControl('', [Validators.required, floorsValidator]),
    cityAndCountry: new FormControl('', [Validators.required, cityValidator]),
    address: new FormControl('', [Validators.required]),
  })


  constructor(
    public dialogRef: MatDialogRef<AddPropertyDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private snackBar: MatSnackBar,
    private propertyService: PropertyService,
    private locationService: LocationService,
    private mapService: MapService
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


  decodeSelectedAddress(event: LatLng) {
    this.mapService.decodeCoordinates(event).subscribe({
      next: (value) => {
        console.log(value)
        if (value != undefined) {
          this.addPropertyForm.get('address')?.setValue(value.split(",")[0]);
        }
      },
      error: (err) => {
        console.log(err);
      },
    })
    
  }

  onEnterKeyPressed() {
    if (this.addPropertyForm.value.address && this.addPropertyForm.value.cityAndCountry) {
      let tokens = (this.addPropertyForm.value.cityAndCountry as unknown as LocationDTO).location.trim().split(", ")
      this.mapService.decodeAddress(this.addPropertyForm.value.address, tokens[0], tokens[1]).subscribe({
        next: (value) => {
          this.selectedMarkerPosition = value;
        },
        error: (err) => {
          console.log(err);
        },
      })
      this.selectedMarkerPosition
    }
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

  rememberSelection(event: MatAutocompleteSelectedEvent) {
    console.log("izabrano: " + event.option.value.location);
    let tokens = event.option.value.location?.trim().split(", ");
    this.mapService.setCityCoordinates(tokens[0], tokens[1]);
  }

  addProperty() {
    console.log(this.addPropertyForm.value?.cityAndCountry!);
    if (this.addPropertyForm.valid && this.filePath != "" && this.selectedMarkerPosition) {
      console.log(this.selectedMarkerPosition)
      let address: AddressDTO = {
        cityId: (this.addPropertyForm.value.cityAndCountry! as unknown as LocationDTO).cityId,
        lat: this.selectedMarkerPosition.lat,
        lng: this.selectedMarkerPosition.lng,
        name: this.addPropertyForm.value.address!
      }

      let dto: PropertyDTO = {
        name: this.addPropertyForm.value.name!,
        area: +this.addPropertyForm.value.area!,
        numOfFloors: +this.addPropertyForm.value.numberOfFloors!,
        image: this.filePath,
        address: address
      }

      console.log(this.filePath)
      this.propertyService.addProperty(dto).subscribe({
        next(value) {
          console.log("done")
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
    console.log("event" + event);
  }

  close() {
    this.dialogRef.close();
  }

}
