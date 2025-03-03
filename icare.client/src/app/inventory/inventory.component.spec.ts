import { ComponentFixture, TestBed } from '@angular/core/testing';
import { InventoryComponent } from './inventory.component';
import { UsersService } from '../services/users.service';
import { ApiService } from '../services/api.service';
import { NotificationService } from '../services/notifications.service';
import { of } from 'rxjs';
import { HttpClientTestingModule } from '@angular/common/http/testing';

declare var bootstrap: any;

describe('InventoryComponent', () => {
  let component: InventoryComponent;
  let fixture: ComponentFixture<InventoryComponent>;
  let usersServiceMock: any;
  let apiServiceMock: any;
  let notificationServiceMock: any;

  beforeEach(async () => {
    usersServiceMock = jasmine.createSpyObj('UsersService', ['getInventory', 'updateInventory', 'removeInventory']);
    apiServiceMock = jasmine.createSpyObj('ApiService', ['getAllItems', 'getSpecificItem']);
    notificationServiceMock = jasmine.createSpyObj('NotificationService', ['showNotification']);

    await TestBed.configureTestingModule({
      declarations: [InventoryComponent],
      imports: [HttpClientTestingModule], // Simulate HTTP requests
      providers: [
        { provide: UsersService, useValue: usersServiceMock },
        { provide: ApiService, useValue: apiServiceMock },
        { provide: NotificationService, useValue: notificationServiceMock }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InventoryComponent);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should retrieve user notification preferences and load inventory', () => {
      spyOn(localStorage, 'getItem').and.returnValue(JSON.stringify({ notifications: false }));
      spyOn(component, 'getInventory');

      component.ngOnInit();

      expect(component.notificationsPermission).toBeFalse();
      expect(component.getInventory).toHaveBeenCalled();
    });
  });

  describe('getInventory', () => {
    it('should populate the inventory from the service', () => {
      const mockInventory = [{ name: 'Apple', quantity: 2, unit: 'kg' }];
      usersServiceMock.getInventory.and.returnValue(of(mockInventory));

      component.getInventory();

      expect(component.inventory.size).toBe(1);
      expect(component.inventory.get('Apple')).toEqual({ quantity: 2, unit: 'kg' });
    });
  });

  describe('toggleSelection', () => {
    it('should select an item if not selected and deselect it if already selected', () => {
      component.toggleSelection('Apple');
      expect(component.selectedItems.has('Apple')).toBeTrue();

      component.toggleSelection('Apple');
      expect(component.selectedItems.has('Apple')).toBeFalse();
    });
  });

  describe('toggleInventorySelection', () => {
    it('should toggle selection of an inventory item', () => {
      component.toggleInventorySelection('Banana');
      expect(component.selectedItemsInInventory.has('Banana')).toBeTrue();

      component.toggleInventorySelection('Banana');
      expect(component.selectedItemsInInventory.has('Banana')).toBeFalse();
    });
  });

  describe('onSearchChange', () => {
    it('should call filterItems when search term changes', () => {
      spyOn(component, 'filterItems');
      component.onSearchChange();

      expect(component.filterItems).toHaveBeenCalled();
    });
  });

  describe('filterItems', () => {
    it('should filter items based on search term', () => {
      component.listOfItems = new Set(['Apple', 'Banana', 'Orange']);
      component.searchTerm = 'ap';
      component.filterItems();

      expect(component.filteredItems).toEqual(['Apple']);
    });
  });

  describe('updateQuantity', () => {
    it('should update item quantity and round value', () => {
      component.inventory.set('Apple', { quantity: 2, unit: 'kg' });

      const event = { target: { value: '3.456' } } as unknown as Event;
      component.updateQuantity('Apple', event);

      expect(component.inventory.get('Apple')?.quantity).toBe(3.46);
      expect(component.editedItems.has('Apple')).toBeTrue();
    });

    it('should not update quantity if the new value is the same', () => {
      component.inventory.set('Apple', { quantity: 2, unit: 'kg' });

      const event = { target: { value: '2' } } as unknown as Event;
      component.updateQuantity('Apple', event);

      expect(component.editedItems.has('Apple')).toBeFalse();
    });
  });

  describe('updateUnit', () => {
    it('should update item unit and add to edited items', () => {
      component.inventory.set('Apple', { quantity: 2, unit: 'kg' });

      const event = { target: { value: 'gram' } } as unknown as Event;
      component.updateUnit('Apple', event);

      expect(component.inventory.get('Apple')?.unit).toBe('gra');
      expect(component.editedItems.has('Apple')).toBeTrue();
    });
  });

  describe('addItemsToInventory', () => {
    it('should add selected items to inventory', () => {
      component.selectedItems.add('Apple');
      usersServiceMock.updateInventory.and.returnValue(of([{ name: 'Apple', quantity: 1, unit: '' }]));

      component.addItemsToInventory();

      expect(component.inventory.has('Apple')).toBeTrue();
      expect(component.selectedItems.size).toBe(0);
      expect(notificationServiceMock.showNotification).toHaveBeenCalled();
    });
  });

  describe('removeItemFromInventory', () => {
    it('should remove selected items from inventory', () => {
      component.inventory.set('Apple', { quantity: 1, unit: 'kg' });
      component.selectedItemsInInventory.add('Apple');

      usersServiceMock.removeInventory.and.returnValue(of([{ name: 'Apple' }]));

      component.removeItemFromInventory(["Apple"]);

      expect(component.inventory.has('Apple')).toBeFalse();
      expect(notificationServiceMock.showNotification).toHaveBeenCalled();
    });
  });

  describe('openModal', () => {
    it('should call Bootstrap modal show method if element exists', () => {
      const modalElement = document.createElement('div');
      spyOn(document, 'getElementById').and.returnValue(modalElement);
      spyOn(bootstrap.Modal.prototype, 'show');

      component.openModal('testModal');

      expect(bootstrap.Modal.prototype.show).toHaveBeenCalled();
    });
  });

  describe('checkForEmptyItems', () => {
    it('should open delete confirmation modal if any item has zero quantity', () => {
      spyOn(component, 'openModal');
      component.inventory.set('Apple', { quantity: 0, unit: 'kg' });

      component.checkForEmptyItems();

      expect(component.openModal).toHaveBeenCalledWith('deleteZeroQuantityModal');
    });

    it('should update inventory if no items have zero quantity', () => {
      spyOn(component, 'updateItemsInInventory');
      component.inventory.set('Apple', { quantity: 1, unit: 'kg' });

      component.checkForEmptyItems();

      expect(component.updateItemsInInventory).toHaveBeenCalled();
    });
  });
});
