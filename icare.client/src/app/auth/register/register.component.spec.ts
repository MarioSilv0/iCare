import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { RegisterComponent } from './register.component';
import { AuthService } from '../auth.service';
import { PasswordValidatorService } from '../password-validator.service';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;
  let mockAuthService: any;
  let mockRouter: any;
  let mockPasswordValidatorService: any;

  beforeEach(async () => {
    mockAuthService = {
      isLogged: jasmine.createSpy('isLogged').and.returnValue(false),
      register: jasmine.createSpy('register').and.returnValue(of({ message: 'Registration successful' })),
    };

    mockRouter = {
      navigate: jasmine.createSpy('navigate'),
    };

    await TestBed.configureTestingModule({
      declarations: [RegisterComponent],
      imports: [FormsModule],
      providers: [
        { provide: AuthService, useValue: mockAuthService },
        { provide: Router, useValue: mockRouter },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(RegisterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should not proceed if passwords do not match', () => {
    component.email = 'test@example.com';
    component.password = 'StrongPass1!';
    component.confirmPassword = 'MismatchPass1!';
    component.onRegister();

    expect(component.errorMessage).toBe('Passwords do not match');
    expect(mockAuthService.register).not.toHaveBeenCalled();
  });

  it('should call register and navigate on successful registration', () => {
    component.email = 'test@example.com';
    component.password = 'StrongPass1!';
    component.confirmPassword = 'StrongPass1!';
    component.onRegister();

    expect(mockAuthService.register).toHaveBeenCalledWith('test@example.com','StrongPass1!');
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should handle registration failure and show error message', () => {
    mockAuthService.register.and.returnValue(
      throwError(() => ({ error: { message: 'Registration failed' } }))
    );

    component.email = 'test@example.com';
    component.password = 'StrongPass1!';
    component.confirmPassword = 'StrongPass1!';
    component.onRegister();

    expect(component.errorMessage).toBe('Registration failed');
  });

  it('should return true for valid passwords in `isPasswordValid`', () => {
    component.password = 'StrongPass1!';
    component.validatePassword();
    expect(component.isPasswordValid).toBeTrue();
  });

  it('should return false for invalid passwords in `isPasswordValid`', () => {
    component.password = 'weak';
    component.validatePassword();
    expect(component.isPasswordValid).toBeFalse();
  });

});
