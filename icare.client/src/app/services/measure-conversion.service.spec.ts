import { TestBed } from '@angular/core/testing';

import { MeasureConversionService } from './measure-conversion.service';

describe('MeasureConversionService', () => {
  let service: MeasureConversionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MeasureConversionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
