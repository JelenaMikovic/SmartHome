import { AbstractControl, FormGroup, FormControl, FormGroupDirective, NgForm, ValidatorFn } from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material/core'

  export function nameValidator( control: AbstractControl): { [key: string]: boolean } | null {
    const trimmedValue = control.value?.trim();

    if (trimmedValue && (trimmedValue.length < 1 || trimmedValue.length > 100)) {
        return { nameError: true };
    }

    return null;
  }

  export function powerConsumptionValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const value = control.value;
  
    if (isNaN(value) || value <= 0 || value > 1000) {
      return { invalidConsumption: true };
    }
  
    return null;
  }

  export function percentageValidation(control: AbstractControl): { [key: string]: boolean } | null {
    const value = control.value;
  
    if (isNaN(value) || value < 0 || value > 100) {
      return { invalidPercentage: true };
    }
  
    return null;
  }

  export function minTempValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const value = control.value;
  
    if (isNaN(value) || value <= -5 || value > 40) {
      return { invalidMinTemp: true };
    }
  
    return null;
  }

  export function maxTempValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const value = control.value;
  
    if (isNaN(value) || value <= -5 || value > 40) {
      return { invalidMaxTemp: true };
    }
  
    return null;
  }

  export function batteryCapacityValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const value = control.value;
  
    if (isNaN(value) || value <= 0 || value > 18) {
      return { invalidBatteryCapacity: true };
    }
  
    return null;
  }

   export function evChargingPowerValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const value = control.value;

    // const regex = /^[0-9]*$/;
    if (isNaN(value) || value < 5 || value > 150) {
      return { invalidEvChargingPower: true };
    }
  
    return null;
  }

  export function evNumberOfPortsValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const value = control.value;

    const regex = /^[0-9]*$/;
    if ((value !== undefined && !regex.test(value)) || isNaN(value) || value < 1 || value > 20) {
        return { invalidEvNumberOfPorts : true}
    };
    return null;
  }

