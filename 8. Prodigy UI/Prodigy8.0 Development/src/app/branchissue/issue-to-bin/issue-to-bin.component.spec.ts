import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IssueToBinComponent } from './issue-to-bin.component';

describe('IssueToBinComponent', () => {
  let component: IssueToBinComponent;
  let fixture: ComponentFixture<IssueToBinComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IssueToBinComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IssueToBinComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
