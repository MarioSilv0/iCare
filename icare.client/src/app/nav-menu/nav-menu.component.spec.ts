//import { ComponentFixture, TestBed } from '@angular/core/testing';
//import { NavMenuComponent } from './nav-menu.component';
//import { AuthService } from '../auth/auth.service';
//import { MenuService } from '../services/menu.service';
//import { Router } from '@angular/router';
//import { of } from 'rxjs';
//import { HttpClient, HttpHandler } from '@angular/common/http';

//describe('NavMenuComponent', () => {
//  let component: NavMenuComponent;
//  let fixture: ComponentFixture<NavMenuComponent>;
//  let authServiceSpy: jasmine.SpyObj<AuthService>;
//  let menuServiceSpy: jasmine.SpyObj<MenuService>;
//  let routerSpy: jasmine.SpyObj<Router>;

//  beforeEach(() => {
//    authServiceSpy = jasmine.createSpyObj('AuthService', ['onStateChanged', 'logout']);
//    menuServiceSpy = jasmine.createSpyObj('MenuService', ['setMenuState']);
//    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

//    TestBed.configureTestingModule({
//      declarations: [NavMenuComponent],
//      providers: [
//        HttpClient, HttpHandler,
//        { provide: AuthService, useValue: authServiceSpy },
//        { provide: MenuService, useValue: menuServiceSpy },
//        { provide: Router, useValue: routerSpy },
//      ],
//    }).compileComponents();

//    fixture = TestBed.createComponent(NavMenuComponent);
//    component = fixture.componentInstance;
//  });

//  it('should create the component', () => {
//    expect(component).toBeTruthy();
//  });

//  it('should update isLoggedIn when auth state changes', () => {
//    authServiceSpy.onStateChanged.and.returnValue(of(true));
//    component.ngOnInit();
//    expect(component.isLoggedIn).toBeTrue();
//  });

//  it('should set username and picture from localStorage', () => {
//    authServiceSpy.onStateChanged.and.returnValue(of(true));
//    spyOn(localStorage, 'getItem').and.returnValue(JSON.stringify({ name: 'John Doe', picture: 'pic.jpg' }));

//    component.ngOnInit();

//    expect(component.username).toBe('John Doe');
//    expect(component.picture).toBe('pic.jpg');
//  });

//  it('should handle invalid JSON in localStorage gracefully', () => {
//    authServiceSpy.onStateChanged.and.returnValue(of(true));
//    spyOn(localStorage, 'getItem').and.returnValue('invalid-json');
//    spyOn(console, 'error');

//    component.ngOnInit();

//    expect(console.error).toHaveBeenCalledWith(
//      'Failed to parse user data from localStorage:',
//      jasmine.any(Error)
//    );
//    expect(component.username).toBe('Error');
//    expect(component.picture).toBe('');
//  });

//  it('should toggle isExpanded and call setMenuState', () => {
//    expect(component.isExpanded).toBeFalse();

//    component.toggle();
//    expect(component.isExpanded).toBeTrue();
//    expect(menuServiceSpy.setMenuState).toHaveBeenCalledWith(true);

//    component.toggle();
//    expect(component.isExpanded).toBeFalse();
//    expect(menuServiceSpy.setMenuState).toHaveBeenCalledWith(false);
//  });

//  it('should collapse the menu', () => {
//    component.isExpanded = true;
//    component.collapse();
//    expect(component.isExpanded).toBeFalse();
//  });

//  it('should call authService.logout when logged in', () => {
//    component.isLoggedIn = true;
//    component.logout();
//    expect(authServiceSpy.logout).toHaveBeenCalled();
//  });

//  it('should not call authService.logout when not logged in', () => {
//    component.isLoggedIn = false;
//    component.logout();
//    expect(authServiceSpy.logout).not.toHaveBeenCalled();
//  });
//});
