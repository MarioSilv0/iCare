import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import {
  BrowserAnimationsModule,
  NoopAnimationsModule,
} from '@angular/platform-browser/animations';
import { CalendarComponent } from './calendar.component';

describe('CalendarComponent', () => {
  let component: CalendarComponent;
  let fixture: ComponentFixture<CalendarComponent>;
  let snackBar: jasmine.SpyObj<MatSnackBar>;

  beforeEach(async () => {
    snackBar = jasmine.createSpyObj('MatSnackBar', ['open']);

    await TestBed.configureTestingModule({
      declarations: [CalendarComponent],
      imports: [
        NoopAnimationsModule,
        BrowserAnimationsModule,
        ReactiveFormsModule,
      ], // ✅ Add ReactiveFormsModule
      providers: [{ provide: MatSnackBar, useValue: snackBar }],
    }).compileComponents();

    fixture = TestBed.createComponent(CalendarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should not emit or reset values if one of the dates is missing', () => {
    spyOn(component.dates, 'emit');
    component.dateForm.controls['startDate'].setValue('');
    component.dateForm.controls['endDate'].setValue('2025-03-14');
    component.validateDate();

    expect(component.dates.emit).not.toHaveBeenCalled();
    expect(component.dateForm.value.startDate).toBe('');
    expect(component.dateForm.value.endDate).toBe('2025-03-14');
  });

  it('should emit dates when valid start and end dates are set', () => {
    spyOn(component.dates, 'emit');
    component.dateForm.controls['startDate'].setValue('2025-03-13');
    component.dateForm.controls['endDate'].setValue('2025-03-14');

    expect(component.dateForm.valid).toBeTrue();

    component.validateDate();

    expect(component.dates.emit).toHaveBeenCalledWith({
      startDate: '2025-03-13',
      endDate: '2025-03-14',
    });
  });

  it('should reset form and show snackbar if start date is before today', () => {
    const today = new Date();
    const startDateBeforeToday = new Date(today.setDate(today.getDate() - 1)); // Set to a day before today

    component.dateForm.controls['startDate'].setValue(
      startDateBeforeToday.toISOString().split('T')[0]
    );
    component.dateForm.controls['endDate'].setValue('2025-03-14');
    component.validateDate();

    expect(snackBar.open).toHaveBeenCalledWith(
      'Só pode criar metas com inicio atual ou superior.',
      undefined,
      {
        duration: 2000,
        panelClass: ['fail-snackbar'],
      }
    );

    expect(component.dateForm.value.startDate).toBeNull();
    expect(component.dateForm.value.endDate).toBeNull();
  });

  it('should reset form and show snackbar if end date is before start date', () => {
    component.dateForm.controls['startDate'].setValue('2025-03-14');
    component.dateForm.controls['endDate'].setValue('2025-03-13');
    component.validateDate();

    expect(snackBar.open).toHaveBeenCalledWith(
      'Só pode criar metas com inicio atual ou superior.',
      undefined,
      {
        duration: 2000,
        panelClass: ['fail-snackbar'],
      }
    );

    expect(component.dateForm.value.startDate).toBeNull();
    expect(component.dateForm.value.endDate).toBeNull();
  });
});
