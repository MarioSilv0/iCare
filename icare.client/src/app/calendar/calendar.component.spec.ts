import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatSnackBar } from '@angular/material/snack-bar'; 
import { CalendarComponent } from './calendar.component';
import { NoopAnimationsModule, BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('CalendarComponent', () => {
  let component: CalendarComponent;
  let fixture: ComponentFixture<CalendarComponent>;
  let snackBar: MatSnackBar;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CalendarComponent],
      imports: [NoopAnimationsModule, BrowserAnimationsModule],
      providers: [
        { provide: MatSnackBar, useValue: jasmine.createSpyObj('MatSnackBar', ['open']) }
      ]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CalendarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should not emit or reset values if one of the dates is missing', () => {
    spyOn(component.dates, 'emit');
    component.startDate = '';
    component.endDate = '2025-03-14';
    component.validateDate();

    expect(component.dates.emit).not.toHaveBeenCalled();
    expect(component.startDate).toBe('');
    expect(component.endDate).toBe('2025-03-14');
  })

  it('should emit dates when valid start and end dates are set', () => {
    spyOn(component.dates, 'emit');
    component.startDate = '2025-03-13';
    component.endDate = '2025-03-14';
    component.validateDate();

    expect(component.dates.emit).toHaveBeenCalledWith({
      startDate: '2025-03-13',
      endDate: '2025-03-14'
    });
  })

  it("should reset dates and show snackbar if start date is after end date", () => {
    component.startDate = '2025-03-14';
    component.endDate = '2025-03-13';
    component.validateDate();

    expect(snackBar.open).toHaveBeenCalledWith('A data final deve ser maior que a data inicial.', '', {
      duration: 2000,
      panelClass: ['fail-snackbar']
    })

    expect(component.startDate).toBe('');
    expect(component.endDate).toBe('');
  })

  it('should open a snackbar with error message when startDate is after endDate', () => {
    component.startDate = '2025-03-14';
    component.endDate = '2025-03-13';
    component.validateDate();

    expect(snackBar.open).toHaveBeenCalledWith('A data final deve ser maior que a data inicial.', '', {
      duration: 2000,
      panelClass: ['fail-snackbar']
    })
  })
});
