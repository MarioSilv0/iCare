import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TacoApiComponent } from './taco-api.component';

describe('TacoApiComponent', () => {
  let component: TacoApiComponent;
  let fixture: ComponentFixture<TacoApiComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TacoApiComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(TacoApiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
