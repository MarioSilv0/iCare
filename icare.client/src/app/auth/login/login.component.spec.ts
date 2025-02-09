import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoginComponent } from './login.component';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

//Mário
describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const authServiceMock = jasmine.createSpyObj('AuthService', ['login', 'googleLogin', 'isLogged']);
    const routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      declarations: [LoginComponent],
      providers: [
        { provide: AuthService, useValue: authServiceMock },
        { provide: Router, useValue: routerMock }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  it('should initialize and check if user is logged in', () => {
    authServiceSpy.isLogged.and.returnValue(true);
    component.ngOnInit();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/home']);
  });

  it('should toggle password visibility', () => {
    expect(component.showPassword).toBeFalse();
    component.togglePasswordVisibility();
    expect(component.showPassword).toBeTrue();
  });

  it('should log in successfully', () => {
    authServiceSpy.login.and.returnValue(of({}));
    component.email = 'test@example.com';
    component.password = 'password123';
    component.onLogin();
    expect(authServiceSpy.login).toHaveBeenCalledWith({ email: 'test@example.com', password: 'password123' });
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/home']);
  });

  it('should handle login failure', () => {
    authServiceSpy.login.and.returnValue(throwError(() => ({ error: { message: 'Invalid login credentials' } })));
    component.onLogin();
    expect(component.errorMessage).toBe('Invalid login credentials');
  });

  it('should log in via Google', () => {
    authServiceSpy.googleLogin.and.returnValue(of({}));
    component.onGoogleLogin({ credential: 'mock_google_token' });
    expect(authServiceSpy.googleLogin).toHaveBeenCalledWith('mock_google_token');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/home']);
  });

  it('should handle Google login failure', () => {
    authServiceSpy.googleLogin.and.returnValue(throwError(() => 'Google login failed'));
    component.onGoogleLogin({ credential: 'mock_google_token' });
    expect(authServiceSpy.googleLogin).toHaveBeenCalledWith('mock_google_token');
  });
});
