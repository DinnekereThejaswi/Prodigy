import { AuthGuardService } from './../auth-guard.service';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { EstimationComponent } from './estimation.component';
import { ReprintSalesestimationComponent } from './reprint-salesestimation/reprint-salesestimation.component';
import { appConfirmationGuard } from '../appconfirmation-guard';

export const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    canDeactivate: [appConfirmationGuard],
    component: EstimationComponent, data: {
      title: 'Sales Estimation',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Sales Estimation' }]
    },
  },
  {
    path: 'reprintsalesestimation',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: ReprintSalesestimationComponent, data: {
      title: 'Reprint Sales Estimation',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Reprint Sales Estimation' }]
    }
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EstimationRoutingModule { }
