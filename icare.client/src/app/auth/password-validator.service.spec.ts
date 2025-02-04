import { TestBed } from '@angular/core/testing';
import { PasswordValidatorService } from './password-validator.service';

describe('PasswordValidatorService', () => {
  let service: PasswordValidatorService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PasswordValidatorService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return invalid for empty password', () => {
    const result = service.validate('');
    expect(result.valid).toBeFalse();
    expect(result.message).toBeUndefined();
  });

  it('should return invalid if password length is less than 8 characters', () => {
    const result = service.validate('short1!');
    expect(result.valid).toBeFalse();
    expect(result.message).toBe('Password with 8 characters long');
  });

  it('should return invalid if password has no lowercase letter', () => {
    const result = service.validate('NOLOWERCASE1!');
    expect(result.valid).toBeFalse();
    expect(result.message).toBe("Enter one lowercase letter('a'-'z')");
  });

  it('should return invalid if password has no uppercase letter', () => {
    const result = service.validate('nouppercase1!');
    expect(result.valid).toBeFalse();
    expect(result.message).toBe("Enter one uppercase letter('A'-'Z')");
  });

  it('should return invalid if password has no digit', () => {
    const result = service.validate('NoDigitHere!');
    expect(result.valid).toBeFalse();
    expect(result.message).toBe("Enter one digit ('0'-'9')");
  });

  it('should return invalid if password has no special character', () => {
    const result = service.validate('NoSpecialChar1');
    expect(result.valid).toBeFalse();
    expect(result.message).toBe('Enter one special character');
  });

  it('should return valid if password meets all criteria', () => {
    const result = service.validate('ValidPassword1!');
    expect(result.valid).toBeTrue();
    expect(result.message).toBe('');
  });
});
