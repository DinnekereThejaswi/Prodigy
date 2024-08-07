import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderFixedTypesComponent } from './order-fixed-types.component';

describe('OrderFixedTypesComponent', () => {
  let component: OrderFixedTypesComponent;
  let fixture: ComponentFixture<OrderFixedTypesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrderFixedTypesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrderFixedTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
