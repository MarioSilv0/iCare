import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { LoginComponent } from './login.component';
import { AuthService } from '../auth.service';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let mockAuthService: any;
  let mockRouter: any;

  afterEach(() => {
    fixture.destroy();
  });

  beforeEach(async () => {
    mockAuthService = {
      isAuthenticated: jasmine.createSpy('isAuthenticated').and.returnValue(of(false)),
      login: jasmine.createSpy('login').and.returnValue(of({})),
    };

    mockRouter = {
      navigate: jasmine.createSpy('navigate'),
    };

    await TestBed.configureTestingModule({
      declarations: [LoginComponent],
      imports: [FormsModule],
      providers: [
        { provide: AuthService, useValue: mockAuthService },
        { provide: Router, useValue: mockRouter },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should reset errorMessage when login is retried and call login and navigate to home on successful login', () => {
    component.errorMessage = 'Invalid login credentials';
    component.email = 'user@example.com';
    component.password = 'ValidPassword123!';
    component.onLogin();

    expect(mockAuthService.login).toHaveBeenCalledWith({
      email: 'user@example.com',
      password: 'ValidPassword123!',
    });
    expect(component.errorMessage).toBeNull();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/home']);
  });

  it('should set an error message on login failure', () => {
    component.email = 'user@example.com';
    component.password = 'InvalidPassword';

    mockAuthService.login.and.returnValue(throwError(() => new Error('Invalid login credentials')));

    component.onLogin();

    expect(component.errorMessage).toBe('Invalid login credentials');
    expect(mockRouter.navigate).not.toHaveBeenCalled();
  });
});
