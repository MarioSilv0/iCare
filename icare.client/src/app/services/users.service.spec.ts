//import { TestBed } from '@angular/core/testing';
//import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
//import { User, UsersService } from './users.service';
//import { Item } from './api.service';

//describe('UsersService', () => {
//  let service: UsersService;
//  let httpMock: HttpTestingController;
//  let mockUser: User = { name: 'John', picture: 'pic.jpg', notifications: false, birthdate: new Date(), email: '', height: 0, weight: 0, preferences: [], restrictions: [] };

//  const URL = 'https://localhost:7266/api/';
//  const PROFILE = 'PublicUser/';
//  const INVENTORY = 'Inventory/';

//  beforeEach(() => {
//    TestBed.configureTestingModule({
//      imports: [HttpClientTestingModule],
//      providers: [UsersService]
//    });

//    service = TestBed.inject(UsersService);
//    httpMock = TestBed.inject(HttpTestingController);
//  });

//  afterEach(() => {
//    httpMock.verify(); // Ensures no pending requests remain
//  });

//  it('should be created', () => {
//    expect(service).toBeTruthy();
//  });

//  it('should fetch user data', () => {
//    service.getUser().subscribe(user => {
//      expect(user).toEqual(mockUser);
//    });

//    const req = httpMock.expectOne(URL + PROFILE);
//    expect(req.request.method).toBe('GET');
//    req.flush(mockUser);
//  });

//  it('should update user data', () => {
//    const updatedUser: User = { name: 'John1', picture: 'pic.jpg', notifications: true, birthdate: new Date(), email: '', height: 0, weight: 50, preferences: [], restrictions: []
//  };;

//    service.updateUser(updatedUser).subscribe(user => {
//      expect(user).toEqual(updatedUser);
//    });

//    const req = httpMock.expectOne(URL + PROFILE);
//    expect(req.request.method).toBe('PUT');
//    expect(req.request.body).toEqual(updatedUser);
//    req.flush(updatedUser);
//  });

//  it('should fetch inventory', () => {
//    const mockInventory: Item[] = [{ name: 'Item 1', quantity: 10, unit: "xl" }, { name: 'Item 2', quantity: 20, unit: "xs" }];

//    service.getInventory().subscribe(items => {
//      expect(items).toEqual(mockInventory);
//    });

//    const req = httpMock.expectOne(URL + INVENTORY);
//    expect(req.request.method).toBe('GET');
//    req.flush(mockInventory);
//  });

//  it('should update inventory', () => {
//    const items: Item[] = [{ name: 'Item 1', quantity: 10, unit: "xl" }, { name: 'Item 2', quantity: 20, unit: "xs" }];

//    service.updateInventory(items).subscribe(updatedItems => {
//      expect(updatedItems).toEqual(items);
//    });

//    const req = httpMock.expectOne(URL + INVENTORY);
//    expect(req.request.method).toBe('PUT');
//    expect(req.request.body).toEqual(items);
//    req.flush(items);
//  });

//  it('should remove inventory items', () => {
//    const itemsToRemove = ['1', '2'];
//    const mockResponse: Item[] = [];

//    service.removeInventory(itemsToRemove).subscribe(response => {
//      expect(response).toEqual(mockResponse);
//    });

//    const req = httpMock.expectOne(URL + INVENTORY);
//    expect(req.request.method).toBe('DELETE');
//    expect(req.request.body).toEqual(itemsToRemove);
//    req.flush(mockResponse);
//  });
//});
