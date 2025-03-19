import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HomeComponent } from './home.component';
import { AuthService } from '../auth/auth.service';
import { UsersService } from '../services/users.service';
import { User } from '../../models'
import { of, throwError } from 'rxjs';

describe('HomeComponent', () => {
  let component: HomeComponent;
  let fixture: ComponentFixture<HomeComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let usersServiceSpy: jasmine.SpyObj<UsersService>;
  let mockUser: User = {
      name: 'John', picture: 'pic.jpg', notifications: false, birthdate: "01-01-2000", email: '', height: 0, weight: 0, preferences: new Set(), restrictions: new Set(), categories: new Set(),
      gender: '', genders: [],
      activityLevel: '', activityLevels: []
  };

  beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['userHasRole']);
    usersServiceSpy = jasmine.createSpyObj('UsersService', ['getUser']);

    TestBed.configureTestingModule({
      declarations: [HomeComponent],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: UsersService, useValue: usersServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(HomeComponent);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should set isAdmin to true if user has Admin role', () => {
    authServiceSpy.userHasRole.and.returnValue(true);
    usersServiceSpy.getUser.and.returnValue(of(mockUser));
    component.ngOnInit();
    expect(component.isAdmin).toBeTrue();
  });

  it('should not call getUser if user data is already in localStorage', () => {
    spyOn(localStorage, 'getItem').and.returnValue(JSON.stringify(mockUser));
    component.ngOnInit();
    expect(usersServiceSpy.getUser).not.toHaveBeenCalled();
  });

  it('should call getUser and store user data in localStorage if not already stored', () => {
    spyOn(localStorage, 'getItem').and.returnValue(null);
    spyOn(localStorage, 'setItem');

    usersServiceSpy.getUser.and.returnValue(of(mockUser));

    component.ngOnInit();

    type LocalStorageUser = Pick<User, "name" | "picture" | "notifications">

    let newMockUser: LocalStorageUser = { name: mockUser.name, picture: mockUser.picture, notifications: mockUser.notifications };

    expect(usersServiceSpy.getUser).toHaveBeenCalled();
    expect(localStorage.setItem).toHaveBeenCalledWith('user', JSON.stringify(newMockUser));
  });

  it('should handle errors when getUser fails', () => {
    spyOn(localStorage, 'getItem').and.returnValue(null);
    spyOn(console, 'error');

    usersServiceSpy.getUser.and.returnValue(throwError(() => new Error('Failed to fetch user')));

    component.ngOnInit();

    expect(usersServiceSpy.getUser).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith(new Error('Failed to fetch user'));
  });
});
