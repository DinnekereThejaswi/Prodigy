import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SubmoduleSettingsComponent } from './submodule-settings.component';

describe('SubmoduleSettingsComponent', () => {
  let component: SubmoduleSettingsComponent;
  let fixture: ComponentFixture<SubmoduleSettingsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SubmoduleSettingsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SubmoduleSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
