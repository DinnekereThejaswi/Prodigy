import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BranchissueRoutingModule } from './branchissue-routing.module';
import { BranchissueComponent } from './branchissue.component'
import { FullModule } from './../layouts/full/full.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { OpgissueComponent } from './opgissue/opgissue.component';
import { SrIssueComponent } from './sr-issue/sr-issue.component';
import { IssueToBinComponent } from './issue-to-bin/issue-to-bin.component';
import { ReprintBranchIssueComponent } from './reprint-branch-issue/reprint-branch-issue.component';
import { TagissueComponent } from './tagissue/tagissue.component';
import { NtIssueComponent } from './nt-issue/nt-issue.component';
import { CancelIssueComponent } from './cancel-issue/cancel-issue.component';
import { IssueCancelComponent } from './issue-cancel/issue-cancel.component';
import { StoneDiamondComponent } from './nt-issue/stone-diamond/stone-diamond.component';

@NgModule({
  declarations: [BranchissueComponent, OpgissueComponent, CancelIssueComponent, IssueCancelComponent, TagissueComponent, SrIssueComponent, IssueToBinComponent, ReprintBranchIssueComponent, NtIssueComponent, StoneDiamondComponent],
  imports: [
    CommonModule, ReactiveFormsModule,
    FormsModule,
    FullModule,
    BranchissueRoutingModule,
    CollapseModule, BsDatepickerModule

  ],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ]
})
export class BranchissueModule { }
