import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ChangePasswordComponent } from './change-password.component';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PasswordValidatorService } from '../password-validator.service';

describe('ChangePasswordComponent', () => {
  let component: ChangePasswordComponent;
  let fixture: ComponentFixture<ChangePasswordComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let passwordValidatorSpy: jasmine.SpyObj<PasswordValidatorService>;

  beforeEach(async () => {
    const authServiceMock = jasmine.createSpyObj('AuthService', ['changePassword']);
    const routerMock = jasmine.createSpyObj('Router', ['navigate']);
    const passwordValidatorMock = jasmine.createSpyObj('PasswordValidatorService', ['validate']);

    await TestBed.configureTestingModule({
      declarations: [ChangePasswordComponent],
      imports: [FormsModule],
      providers: [
        { provide: AuthService, useValue: authServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: PasswordValidatorService, useValue: passwordValidatorMock }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(ChangePasswordComponent);
    component = fixture.componentInstance;
    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    passwordValidatorSpy = TestBed.inject(PasswordValidatorService) as jasmine.SpyObj<PasswordValidatorService>;
  });


  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should toggle password visibility', () => {
    expect(component.showPassword).toBeFalse();
    component.togglePasswordVisibility();
    expect(component.showPassword).toBeTrue();
  });

  it('should not proceed if passwords do not match', () => {
    component.newPassword = 'Password123!';
    component.repeatPassword = 'DifferentPassword1!';
    component.onChange();
    expect(component.errorMessage).toBe('Passwords do not match');
  });

  it('should change password successfully', () => {
    authServiceSpy.changePassword.and.returnValue(of({}));
    passwordValidatorSpy.validate.and.returnValue({ valid: true });
    component.currentPassword = 'OldPass123!';
    component.newPassword = 'NewPass123!';
    component.repeatPassword = 'NewPass123!';
    component.onChange();
    expect(authServiceSpy.changePassword).toHaveBeenCalledWith({
      currentPassword: 'OldPass123!',
      newPassword: 'NewPass123!'
    });
    expect(routerSpy.navigate).toHaveBeenCalled();
  });

  it('should handle change password failure', () => {
    authServiceSpy.changePassword.and.returnValue(
      throwError(() => ({ error: { message: 'Error changing password' } }))
    );
    passwordValidatorSpy.validate.and.returnValue({ valid: true });
    component.currentPassword = 'OldPass123!!';
    component.newPassword = 'NewPass123!';
    component.repeatPassword = 'NewPass123!';
    component.onChange();
    expect(component.errorMessage).toBe('Error changing password');
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
