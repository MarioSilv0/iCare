import { AbstractControl, ValidationErrors } from '@angular/forms';

export function birthdateValidator(control: AbstractControl): ValidationErrors | null {
  const value = control.value;
  if (!value) return null;

  const inputDate = new Date(value);
  const today = new Date();

  const maxAge = 121;
  const minDate = new Date(today.getFullYear() - maxAge, today.getMonth(), today.getDate());

  if (inputDate < minDate) return { tooOld: true };
  if (inputDate > today) return { futureDate: true };

  return null;
}
