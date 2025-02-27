import { TestBed } from '@angular/core/testing';

import { UpdateRecipesService } from './update-recipes.service';

describe('UpdateRecipesService', () => {
  let service: UpdateRecipesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UpdateRecipesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
