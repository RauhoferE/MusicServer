import {AbstractControl, ValidationErrors, ValidatorFn} from '@angular/forms';

export function birthdateValidator(): ValidatorFn {
    return (control:AbstractControl) : ValidationErrors | null => {

        const value = control.value;

        if (!value) {
            return null;
        }

        const birtdateValid = value.getTime() < new Date().getTime();

        return birtdateValid ? null : {birthdateValid:true};
    }
}