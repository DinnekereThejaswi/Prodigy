import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {AuthGuardService} from './../../auth-guard.service';
import {GstPostingSetupComponent} from './gst-posting-setup.component';
import {ManageGstPostingSetupComponent} from './manage-gst-posting-setup/manage-gst-posting-setup.component';

const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
     component: GstPostingSetupComponent, data: {
      title: 'Gst Posting Setup', 
      urls: [{title: 'Dashboard', url: 'javascript:void(0)'},{title: 'Gst Posting Setup'}]
    },
  },
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    children: [{
      path: 'add',
      data: {
        title: 'Manage - Gst Posting Setup',
        urls: [{title: 'Gst Posting Setup',url: '/gst-posting-setup'},{title: 'Add'}]
      },
      component: ManageGstPostingSetupComponent,
    },{
      path: 'edit/:id', data: {
        title: 'Manage - Gst Posting Setup',
        urls: [{title: 'Gst Posting Setup',url: '/gst-posting-setup'},{title: 'Edit'}]
      },
      component: ManageGstPostingSetupComponent,
    },]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GstPostingSetupRoutingModule { }
