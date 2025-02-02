import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { AuthGuard } from './auth.guard';
import { AuthService } from './auth.service';

//Mário
describe('AuthGuard', () => {
  let guard: AuthGuard;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(() => {
    const authServiceMock = jasmine.createSpyObj('AuthService', ['isLogged', 'userHasRole']);
    const routerMock = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        AuthGuard,
        { provide: AuthService, useValue: authServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    });

    guard = TestBed.inject(AuthGuard);
    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  it('should allow access if user is logged in and has the correct roles', () => {
    authServiceSpy.isLogged.and.returnValue(true);
    authServiceSpy.userHasRole.and.returnValue(true);
    const routeMock = { data: { roles: 'Admin' } };
    expect(guard.canActivate(routeMock as any)).toBe(true);
  });

  it('should deny access if user is not logged in', () => {
    authServiceSpy.isLogged.and.returnValue(false);
    const routeMock = { data: { roles: 'Admin' } };
    expect(guard.canActivate(routeMock as any)).toBe(false);
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should deny access if user does not have the required role', () => {
    authServiceSpy.isLogged.and.returnValue(true);
    authServiceSpy.userHasRole.and.returnValue(false);
    const routeMock = { data: { roles: 'Admin' } };
    expect(guard.canActivate(routeMock as any)).toBe(false);
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/home']);
  });
});
