import { ConfirmSalesreturnComponent } from './confirm-salesreturn/confirm-salesreturn.component';
import { AuthGuardService } from './../auth-guard.service';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SalesreturnComponent } from './salesreturn.component';
import { CancelSalesreturnComponent } from './cancel-salesreturn/cancel-salesreturn.component';
import { ReprintSalesreturnComponent } from './reprint-salesreturn/reprint-salesreturn.component';
import { DeleteSalesreturnComponent } from './delete-salesreturn/delete-salesreturn.component';
import { appConfirmationGuard } from '../appconfirmation-guard';

export const SalesreturnRoutes: Routes = [
  {
    path: '',
    // canDeactivate: [appConfirmationGuard],
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: SalesreturnComponent, data: {
      title: 'SR Est',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'SR Est' }]
    },
  },
  {
    path: 'ConfirmSalesReturn',
    canDeactivate: [appConfirmationGuard],
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: ConfirmSalesreturnComponent, data: {
      title: 'Confirm Sales Return',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Confirm Sales Return' }]
    },
  },
  {
    path: 'reprint-salesreturn',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: ReprintSalesreturnComponent, data: {
      title: 'Reprint SR',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Reprint SR' }]
    },
  },
  {
    path: 'cancel-salesreturn',
    canDeactivate: [appConfirmationGuard],
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: CancelSalesreturnComponent, data: {
      title: 'Cancel SR',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Cancel SR' }]
    },
  },
  {
    path: 'delete-salesreturn',
    canDeactivate: [appConfirmationGuard],
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: DeleteSalesreturnComponent, data: {
      title: 'Delete SR',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Delete SR' }]
    },
  }

]

@NgModule({
  imports: [RouterModule.forChild(SalesreturnRoutes)],
  exports: [RouterModule]
})
export class SalesreturnRoutingModule { }
