import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuardService } from './../../auth-guard.service';
import { MachineRegistrationComponent } from './machine-registration.component';
import { ManageMachineRegistrationComponent } from './manage-machine-registration/manage-machine-registration.component';
const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
     component: MachineRegistrationComponent, data: {
      title: 'Machine-Registration', 
      urls: [{title: 'Dashboard', url: 'javascript:void(0)'},{title: 'Machine-Registration'}]
    },
  },
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    children: [{
      path: 'add',
      data: {
        title: 'Manage - Machine-Registration',
        urls: [{title: 'Machine-Registration',url: '/machine-registration'},{title: 'Add'}]
      },
      component: ManageMachineRegistrationComponent,
    },{
      path: 'edit/:id', data: {
        title: 'Manage - Machine-Registration',
        urls: [{title: 'Machine-Registration',url: '/machine-registration'},{title: 'Edit'}]
      },
      component: ManageMachineRegistrationComponent,
    },]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MachineRegistrationRoutingModule { }
