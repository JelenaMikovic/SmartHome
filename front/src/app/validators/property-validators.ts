import { LocationDTO } from './../../services/location.service';
import { AbstractControl, FormGroup, FormControl, FormGroupDirective, NgForm, ValidatorFn } from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material/core'

  export function nameValidator( control: AbstractControl): { [key: string]: boolean } | null {
    const trimmedValue = control.value?.trim();

    if (trimmedValue && (trimmedValue.length < 1 || trimmedValue.length > 100)) {
        return { nameError: true };
    }

    return null;
  }

  export function areaValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const value = control.value;
  
    if (isNaN(value) || value <= 0 || value > 1000) {
      return { invalidArea: true };
    }
  
    return null;
  }

  export function floorsValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const value = control.value;
  
    if (isNaN(value) || value < 1 || value > 200) {
      return { invalidFloors: true };
    }
  
    return null;
  }

  export function cityValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const value = control.value;
  
    if (typeof(value) === 'string') {
      return { invalidCity: true };
    }
  
    return null;
  }