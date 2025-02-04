import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ResetPasswordComponent } from './reset-password.component';
import { AuthService } from '../auth.service';
import { PasswordValidatorService } from '../password-validator.service';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { FormsModule } from '@angular/forms';

describe('ResetPasswordComponent', () => {
  let component: ResetPasswordComponent;
  let fixture: ComponentFixture<ResetPasswordComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let passwordValidatorSpy: jasmine.SpyObj<PasswordValidatorService>;

  beforeEach(async () => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['resetPassword']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    passwordValidatorSpy = jasmine.createSpyObj('PasswordValidatorService', ['validate']);

    await TestBed.configureTestingModule({
      declarations: [ResetPasswordComponent],
      imports: [FormsModule],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: PasswordValidatorService, useValue: passwordValidatorSpy }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(ResetPasswordComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should toggle password visibility', () => {
    expect(component.showPassword).toBeFalse();
    component.togglePasswordVisibility();
    expect(component.showPassword).toBeTrue();
  });

  it('should not reset password if new passwords do not match', () => {
    component.newPassword = 'password123';
    component.repeatPassword = 'password456';
    component.onReset();
    expect(component.errorMessage).toBe('Passwords do not match' );
    expect(authServiceSpy.resetPassword).not.toHaveBeenCalled();
  });

  it('should not reset password if validation fails', () => {
    passwordValidatorSpy.validate.and.returnValue({ valid: false });
    component.newPassword = 'weak';
    component.repeatPassword = 'weak';

    component.onReset();

    expect(component.errorMessage).toBe('Password is too weak');
    expect(authServiceSpy.resetPassword).not.toHaveBeenCalled();
  });

  it('should reset password successfully', () => {
    passwordValidatorSpy.validate.and.returnValue({ valid: true });

    component.email = 'test@example.com';
    component.token = 'validToken';
    component.newPassword = 'Password123!';
    component.repeatPassword = 'Password123!';

    authServiceSpy.resetPassword.and.returnValue(of({}));
    component.onReset();

    expect(authServiceSpy.resetPassword).toHaveBeenCalledWith({
      email: 'test@example.com',
      token: 'validToken',
      newPassword: 'Password123!'
    });
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should handle reset password failure', () => {
    passwordValidatorSpy.validate.and.returnValue({ valid: true, message: '' });

    component.email = 'test@example.com';
    component.token = 'invalidToken';
    component.newPassword = 'Password123!';
    component.repeatPassword = 'Password123!';

    authServiceSpy.resetPassword.and.returnValue(throwError(() => ({ error: { message: 'Invalid token' } })));

    component.onReset();
    expect(component.errorMessage).toBe('Invalid token');
  });

  it('should validate password correctly', () => {
    passwordValidatorSpy.validate.and.returnValue({ valid: true, message: '' });

    component.validateNewPassword();

    expect(component.isPasswordValid).toBeTrue();
    expect(component.passwordErrorMessage).toBe('');
  });

  it('should detect invalid passwords', () => {
    passwordValidatorSpy.validate.and.returnValue({ valid: false, message: 'Password too weak' });

    component.validateNewPassword();

    expect(component.isPasswordValid).toBeFalse();
    expect(component.passwordErrorMessage).toBe('Password too weak');
  });
});
