import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuardService } from './../../auth-guard.service';
import { ReligionMasterComponent } from './religion-master.component';
import { ManageReligionMasterComponent } from './manage-religion-master/manage-religion-master.component';

const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
     component: ReligionMasterComponent, data: {
      title: 'Religion-Master', 
      urls: [{title: 'Dashboard', url: 'javascript:void(0)'},{title: 'Religion-Master'}]
    },
  },
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    children: [{
      path: 'add',
      data: {
        title: 'Manage - Religion-Master',
        urls: [{title: 'Religion-Master',url: '/religion-master'},{title: 'Add'}]
      },
      component: ManageReligionMasterComponent,
    },{
      path: 'edit/:id', data: {
        title: 'Manage - Religion-Master',
        urls: [{title: 'Religion-Master',url: '/religion-master'},{title: 'Edit'}]
      },
      component: ManageReligionMasterComponent,
    },]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReligionMasterRoutingModule { }
