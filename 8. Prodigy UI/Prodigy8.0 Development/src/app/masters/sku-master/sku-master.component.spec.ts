import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SkuMasterComponent } from './sku-master.component';

describe('SkuMasterComponent', () => {
  let component: SkuMasterComponent;
  let fixture: ComponentFixture<SkuMasterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SkuMasterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SkuMasterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
