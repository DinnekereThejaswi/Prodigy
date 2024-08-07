import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuardService } from './../../auth-guard.service';
import { GstMasterComponent } from './gst-master.component';
import { ManageGstMasterComponent } from './manage-gst-master/manage-gst-master.component';
const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
     component: GstMasterComponent, data: {
      title: 'GST-Master', 
      urls: [{title: 'Dashboard', url: 'javascript:void(0)'},{title: 'GST-Master'}]
    },
  },
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    children: [{
      path: 'add',
      data: {
        title: 'Manage - GST-Master',
        urls: [{title: 'GST-Master',url: '/gst-master'},{title: 'Add'}]
      },
      component: ManageGstMasterComponent,
    },{
      path: 'edit/:id', data: {
        title: 'Manage - GST-Master',
        urls: [{title: 'GST-Master',url: '/gst-master'},{title: 'Edit'}]
      },
      component: ManageGstMasterComponent,
    },]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GstMasterRoutingModule { }
