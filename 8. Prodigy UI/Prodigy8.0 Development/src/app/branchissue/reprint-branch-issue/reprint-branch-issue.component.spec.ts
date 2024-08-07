import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReprintBranchIssueComponent } from './reprint-branch-issue.component';

describe('ReprintBranchIssueComponent', () => {
  let component: ReprintBranchIssueComponent;
  let fixture: ComponentFixture<ReprintBranchIssueComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReprintBranchIssueComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReprintBranchIssueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
