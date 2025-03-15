import {
  HttpClientTestingModule,
  HttpTestingController,
} from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { of } from 'rxjs';
import { GoalsComponent } from './goals.component';

describe('GoalsComponent', () => {
  let component: GoalsComponent;
  let fixture: ComponentFixture<GoalsComponent>;
  let httpMock: HttpTestingController;
  let snackBarSpy: jasmine.SpyObj<MatSnackBar>;

  beforeEach(async () => {
    snackBarSpy = jasmine.createSpyObj('MatSnackBar', ['open']);

    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, ReactiveFormsModule],
      declarations: [GoalsComponent],
      providers: [FormBuilder, { provide: MatSnackBar, useValue: snackBarSpy }],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GoalsComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
    spyOn(component['http'], 'get').and.returnValue(of([]));
    spyOn(component['http'], 'put').and.returnValue(of({}));
    spyOn(component['http'], 'post').and.returnValue(of({}));
    fixture.detectChanges();
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize goalForm with default values', () => {
    expect(component.goalForm.value).toEqual({
      selectedGoal: '',
      calories: 0,
      startDate: '',
      endDate: '',
    });
  });

  it('should toggle goalType correctly', () => {
    expect(component.goalType).toBe('Manual');
    component.toggleGoalType();
    expect(component.goalType).toBe('Automática');
    component.toggleGoalType();
    expect(component.goalType).toBe('Manual');
  });

  it('should create a MetaManual goal correctly', () => {
    component.goalForm.patchValue({
      selectedGoal: 'Perder Peso',
      calories: 2000,
      startDate: '2025-01-01',
      endDate: '2025-01-31',
    });
    const goal = component.createGoal('Manual');
    expect(goal).toEqual({
      type: 'Manual',
      calories: 2000,
      startDate: '2025-01-01',
      endDate: '2025-01-31',
    });
  });

  it('should create a MetaAutomatica goal correctly', () => {
    component.goalForm.patchValue({ selectedGoal: 'Perder Peso' });
    const goal = component.createGoal('Automática');
    expect(goal).toEqual({ type: 'Automatica', goal: 'Perder Peso' });
  });

  it('should return null and log an error for invalid goal type', () => {
    spyOn(console, 'error');
    const goal = component.createGoal('InvalidType');
    expect(goal).toBeNull();
    expect(console.error).toHaveBeenCalledWith('O tipo de meta não existe.');
  });

  it('should update form values when receiveData is called', () => {
    component.receiveData({ startDate: '2025-02-01', endDate: '2025-02-28' });
    expect(component.goalForm.value.startDate).toBe('2025-02-01');
    expect(component.goalForm.value.endDate).toBe('2025-02-28');
  });

  it('should show success snackbar on successful addGoal', () => {
    component.addGoal();
    expect(snackBarSpy.open).toHaveBeenCalledWith(
      'Meta criada com sucesso.',
      undefined,
      { duration: 2000, panelClass: ['success-snackbar'] }
    );
  });

  it('should show error snackbar on failed addGoal', () => {
    component.addGoal();
    expect(snackBarSpy.open).toHaveBeenCalledWith(
      'Erro ao tentar criar meta.',
      undefined,
      { duration: 2000, panelClass: ['fail-snackbar'] }
    );
  });

  it('should show success snackbar on successful updateUserInfo', () => {
    component.updateUserInfo();
    expect(snackBarSpy.open).toHaveBeenCalledWith(
      'Informações atualizadas com sucesso.',
      undefined,
      { duration: 2000, panelClass: ['success-snackbar'] }
    );
  });

  it('should show error snackbar on failed updateUserInfo', () => {
    component.updateUserInfo();
    expect(snackBarSpy.open).toHaveBeenCalledWith(
      'Erro ao tentar atualizar informações.',
      undefined,
      { duration: 2000, panelClass: ['fail-snackbar'] }
    );
  });
});
