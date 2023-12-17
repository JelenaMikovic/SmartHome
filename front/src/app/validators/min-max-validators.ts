import { AbstractControl, FormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ErrorStateMatcher } from "@angular/material/core";

export function minMaxValidator(min_cal: string, max_cal: string) {

    return function(form: AbstractControl) {
        const minValue = form.get(min_cal)?.value;
        const maxValue = form.get(max_cal)?.value;
        console.log(minValue, maxValue)
        if (minValue <= maxValue) {
            return null;
        }

        return { minMaxError: true };
    }
}

export class ConfirmValidParentMatcher implements ErrorStateMatcher {
	isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
		if (control?.parent?.errors?.['minMaxError'] && control.dirty) return true;
        return false;
	}
}