import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProfileComponent } from './profile.component';
import { UsersService, User } from '../services/users.service';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { NotificationService } from '../services/notifications.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('ProfileComponent', () => {
  let component: ProfileComponent;
  let fixture: ComponentFixture<ProfileComponent>;
  let usersServiceMock: any;
  let notificationServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    usersServiceMock = jasmine.createSpyObj('UsersService', ['getUser', 'updateUser']);
    notificationServiceMock = jasmine.createSpyObj('NotificationService', ['showNotification']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      declarations: [ProfileComponent],
      imports: [HttpClientTestingModule], // Simulate HTTP requests
      providers: [
        { provide: UsersService, useValue: usersServiceMock },
        { provide: NotificationService, useValue: notificationServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProfileComponent);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should call getUser on initialization', () => {
      spyOn(component, 'getUser');
      component.ngOnInit();
      expect(component.getUser).toHaveBeenCalled();
    });
  });

  describe('getUser', () => {
    it('should retrieve user data and populate preferences/restrictions', () => {
      const mockUser: User = {
        picture: 'test.jpg',
        name: 'John Doe',
        email: 'johndoe@example.com',
        birthdate: '1990-01-01',
        notifications: true,
        height: 180,
        weight: 75,
        preferences: new Set(['Vegan']),
        restrictions: new Set(['Gluten']),
        categories: new Set(['Vegan', 'Gluten', 'Organic'])
      };
      usersServiceMock.getUser.and.returnValue(of(mockUser));

      component.getUser();

      expect(component.user.name).toBe('John Doe');
      expect(component.availablePreferences.has('Organic')).toBeTrue();
      expect(component.availableRestrictions.has('Organic')).toBeTrue();
    });

    it('should handle error if user data retrieval fails', () => {
      spyOn(console, 'error');
      usersServiceMock.getUser.and.returnValue(of(new Error('Error fetching user data')));

      component.getUser();

      expect(console.error).toHaveBeenCalled();
    });
  });

  describe('addPreference', () => {
    it('should add a preference and remove it from available preferences', () => {
      component.availablePreferences.add('Vegetarian');
      const event = { target: { value: 'Vegetarian' } } as unknown as Event;

      component.addPreference(event);

      expect(component.user.preferences.has('Vegetarian')).toBeTrue();
      expect(component.availablePreferences.has('Vegetarian')).toBeFalse();
    });
  });

  describe('removePreference', () => {
    it('should remove a preference and add it back to available preferences', () => {
      component.user.preferences.add('Keto');
      component.removePreference('Keto');

      expect(component.user.preferences.has('Keto')).toBeFalse();
      expect(component.availablePreferences.has('Keto')).toBeTrue();
    });
  });

  describe('addRestriction', () => {
    it('should add a restriction and remove it from available restrictions', () => {
      component.availableRestrictions.add('Dairy-Free');
      const event = { target: { value: 'Dairy-Free' } } as unknown as Event;

      component.addRestriction(event);

      expect(component.user.restrictions.has('Dairy-Free')).toBeTrue();
      expect(component.availableRestrictions.has('Dairy-Free')).toBeFalse();
    });
  });

  describe('removeRestriction', () => {
    it('should remove a restriction and add it back to available restrictions', () => {
      component.user.restrictions.add('Nut-Free');
      component.removeRestriction('Nut-Free');

      expect(component.user.restrictions.has('Nut-Free')).toBeFalse();
      expect(component.availableRestrictions.has('Nut-Free')).toBeTrue();
    });
  });

  describe('updateUser', () => {
    it('should update user data and show notification', () => {
      const updatedUser = { ...component.user, email: 'newemail@example.com' };
      usersServiceMock.updateUser.and.returnValue(of(updatedUser));

      component.updateUser();

      expect(notificationServiceMock.showNotification).toHaveBeenCalled();
      expect(routerMock.navigate).toHaveBeenCalledWith(['/']);
    });

    it('should show email update failure notification if email change fails', () => {
      const updatedUser = { ...component.user, email: 'different@example.com' };
      usersServiceMock.updateUser.and.returnValue(of(updatedUser));

      component.updateUser();

      expect(notificationServiceMock.showNotification).toHaveBeenCalledTimes(2);
    });

    it('should handle errors when updating user', () => {
      spyOn(console, 'error');
      usersServiceMock.updateUser.and.returnValue(of(new Error('Error updating user')));

      component.updateUser();

      expect(console.error).toHaveBeenCalled();
    });
  });

  describe('onSelectFile', () => {
    it('should update user picture if a valid image file is selected', () => {
      const event = {
        target: {
          files: [{ type: 'image/png' }]
        }
      } as unknown as Event;
      const fileReaderMock = {
        readAsDataURL: jasmine.createSpy('readAsDataURL'),
        onload: jasmine.createSpy('onload'),
        result: 'data:image/png;base64,testImageData'
      };

      spyOn(window as any, 'FileReader').and.returnValue(fileReaderMock);
      component.onSelectFile(event);

      fileReaderMock.onload(); // Simulate onload event
      expect(component.user.picture).toBe('data:image/png;base64,testImageData');
    });

    it('should not update picture if a non-image file is selected', () => {
      const event = {
        target: {
          files: [{ type: 'text/plain' }]
        }
      } as unknown as Event;

      component.onSelectFile(event);
      expect(component.user.picture).toBe('');
    });
  });

  describe('changePassword', () => {
    it('should navigate to the change password page', () => {
      component.changePassword();
      expect(routerMock.navigate).toHaveBeenCalledWith(['/change-password']);
    });
  });
});
