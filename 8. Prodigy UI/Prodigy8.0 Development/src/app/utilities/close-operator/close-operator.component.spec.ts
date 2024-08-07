import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CloseOperatorComponent } from './close-operator.component';

describe('CloseOperatorComponent', () => {
  let component: CloseOperatorComponent;
  let fixture: ComponentFixture<CloseOperatorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CloseOperatorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CloseOperatorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
