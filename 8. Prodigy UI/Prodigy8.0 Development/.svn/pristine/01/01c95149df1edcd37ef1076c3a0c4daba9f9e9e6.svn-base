import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { OpgissueComponent } from './opgissue/opgissue.component'
import { SrIssueComponent } from './sr-issue/sr-issue.component'
import { IssueToBinComponent } from './issue-to-bin/issue-to-bin.component';
import { ReprintBranchIssueComponent } from './reprint-branch-issue/reprint-branch-issue.component';
import { TagissueComponent } from './tagissue/tagissue.component';
import { NtIssueComponent } from './nt-issue/nt-issue.component';
import { IssueCancelComponent } from './issue-cancel/issue-cancel.component';
import { CancelIssueComponent } from './cancel-issue/cancel-issue.component';

const routes: Routes = [{
  path: 'opgissue',
  canActivate: [],
  canActivateChild: [],
  component: OpgissueComponent, data: {
    title: 'OPG Issue',
    urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'OPG ISSUE' }]
  },
},
{
  path: 'tagissue',
  canActivate: [],
  canActivateChild: [],
  component: TagissueComponent, data: {
    title: 'TAG Issue',
    urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'TAG Issue' }]
  }
},
{
  path: 'sr-issue',
  canActivate: [],
  canActivateChild: [],
  component: SrIssueComponent, data: {
    title: 'SR Issue',
    urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'SR Issue' }]
  },
},
{
  path: 'nt-issue',
  canActivate: [],
  canActivateChild: [],
  component: NtIssueComponent, data: {
    title: 'NT Issue',
    urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'NT Issue' }]
  },
},
{
  path: 'issue-to-bin',
  canActivate: [],
  canActivateChild: [],
  component: IssueToBinComponent, data: {
    title: 'Issue to Bin',
    urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Issue to Bin' }]
  },
},
{
  path: 'reprint-branch-issue',
  canActivate: [],
  canActivateChild: [],
  component: ReprintBranchIssueComponent, data: {
    title: 'Reprint Branch Issue',
    urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Reprint Branch Issue' }]
  },
},
{
  path: 'cancel-issue',
  canActivate: [],
  canActivateChild: [],
  component: CancelIssueComponent, data: {
    title: 'Cancel Issue',
    urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Cancel Issue' }]
  },
},
{
  path: 'issue-cancel',
  canActivate: [],
  canActivateChild: [],
  component: IssueCancelComponent, data: {
    title: 'Issue Cancel (OPG & SR)',
    urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Issue Cancel (OPG & SR)' }]
  },
}
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BranchissueRoutingModule { }
