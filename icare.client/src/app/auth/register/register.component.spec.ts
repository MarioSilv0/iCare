import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { RegisterComponent } from './register.component';
import { AuthService } from '../auth.service';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;
  let mockAuthService: any;
  let mockRouter: any;

  beforeEach(async () => {
    mockAuthService = {
      isAuthenticated: jasmine.createSpy('isAuthenticated').and.returnValue(of(false)),
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

    expect(mockAuthService.register).toHaveBeenCalledWith({
      email: 'test@example.com',
      password: 'StrongPass1!',
    });
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/home']);
  });

  it('should return true for valid passwords in `passwordIsValide`', () => {
    component.password = 'StrongPass1!';
    component.validatePassword(component.password);
    expect(component.passwordIsValide()).toBeTrue();
  });

  it('should return false for invalid passwords in `passwordIsValide`', () => {
    component.password = 'weak';
    component.validatePassword(component.password);
    expect(component.passwordIsValide()).toBeFalse();
  });
});
