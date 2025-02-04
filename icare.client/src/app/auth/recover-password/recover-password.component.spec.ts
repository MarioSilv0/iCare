import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RecoverPasswordComponent } from './recover-password.component';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('RecoverPasswordComponent', () => {
  let component: RecoverPasswordComponent;
  let fixture: ComponentFixture<RecoverPasswordComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const authServiceMock = jasmine.createSpyObj('AuthService', ['recoverPassword']);
    const routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      declarations: [RecoverPasswordComponent],
      providers: [
        { provide: AuthService, useValue: authServiceMock },
        { provide: Router, useValue: routerMock },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(RecoverPasswordComponent);
    component = fixture.componentInstance;
    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize without errors', () => {
    expect(component.email).toBe('');
    expect(component.errorMessage).toBeUndefined();
  });

  it('should recover password successfully', () => {
    authServiceSpy.recoverPassword.and.returnValue(of({}));
    component.email = 'test@example.com';
    component.onRecover();
    expect(authServiceSpy.recoverPassword).toHaveBeenCalledWith('test@example.com');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should handle empty email error', () => {
    component.email = '';
    authServiceSpy.recoverPassword.and.returnValue(throwError(() => new Error('Error')));
    component.onRecover();
    expect(component.errorMessage).toBe('Email cant be empty.');
  });

  it('should handle invalid email error', () => {
    component.email = 'invalid@example.com';
    authServiceSpy.recoverPassword.and.returnValue(throwError(() => new Error('Error')));
    component.onRecover();
    expect(component.errorMessage).toBe('Invalid Email.');
  });

  it('should reset email and error message', () => {
    component.email = 'test@example.com';
    component.errorMessage = 'Some error';
    component.onReset();
    expect(component.email).toBe('');
    expect(component.errorMessage).toBe('');
  });
});
