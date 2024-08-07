import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IrSetupComponent } from './ir-setup.component';

describe('IrSetupComponent', () => {
  let component: IrSetupComponent;
  let fixture: ComponentFixture<IrSetupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IrSetupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IrSetupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
