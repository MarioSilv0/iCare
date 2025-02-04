import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PasswordValidatorService {

  constructor() { }

  validate(password: string): { valid: boolean, message?: string } {
    if (!password) {
      return { valid: false };
    }

    if (password.length < 8) {
      return { valid: false, message: 'Password with 8 characters long' };
    }

    if (!/[a-z]/.test(password)) {
      return { valid: false, message: "Enter one lowercase letter('a'-'z')" };
    }

    if (!/[A-Z]/.test(password)) {
      return { valid: false, message: "Enter one uppercase letter('A'-'Z')" };
    }

    if (!/[0-9]/.test(password)) {
      return { valid: false, message: "Enter one digit ('0'-'9')" };
    }

    if (!/[!@#$%^&*]/.test(password)) {
      return { valid: false, message: 'Enter one special character' };
    }

    return { valid: true, message: '' };
  }
}
