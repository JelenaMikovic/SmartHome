import { AbstractControl, FormGroup, FormControl, FormGroupDirective, NgForm, ValidatorFn } from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material/core'

export function dateAheadOfTodayValidator(): ValidatorFn {
    return (control: AbstractControl): {[key: string]: any} | null => {
        const today = new Date();
        const selectedDate = new Date(control.value);
        if (selectedDate < today) {
        return null; // return null if validation succeeds
        }
        return {'dateAheadOfToday': {value: control.value}}; // return error object if validation fails
    };
    }

export function dateMatcher(startDate: string, endDate: string) {

        return function(form: AbstractControl) {
            const startDateValue = new Date(form.get(startDate)?.value);
            const endDateValue = new Date(form.get(endDate)?.value);
    
            // console.log(startDateValue, endDateValue)
            if (endDate && startDate && startDateValue <= endDateValue) {
                const oneMonthInMillis = 30 * 24 * 60 * 60 * 1000; // Assuming one month is approximately 30 days

                const differenceInMillis = endDateValue.getTime() - startDateValue.getTime();
                // console.log(differenceInMillis)
                if (differenceInMillis <= oneMonthInMillis) {
                    return null; // Validation passed
                }
            }
    
            return { dateMismatchError: true };
        }
    }
    
    
export class ConfirmValidParentMatcher implements ErrorStateMatcher {
        isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
            if (control?.parent?.errors?.['dateMismatchError'] && control.dirty) return true;
            return false;
        }
    }