import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FtpConfigComponent } from './ftp-config.component';

describe('FtpConfigComponent', () => {
  let component: FtpConfigComponent;
  let fixture: ComponentFixture<FtpConfigComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FtpConfigComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FtpConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
