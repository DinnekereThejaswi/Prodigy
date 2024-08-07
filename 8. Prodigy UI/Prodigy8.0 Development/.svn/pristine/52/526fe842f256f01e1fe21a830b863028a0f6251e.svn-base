import { AuthGuardService } from './../auth-guard.service';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SalesComponent } from './sales.component';
import { appConfirmationGuard } from '../appconfirmation-guard';

export const SalesRoutes: Routes = [
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    canDeactivate: [appConfirmationGuard],
    component: SalesComponent, data: {
      title: 'Sales',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Sales' }]
    },
  }
]

@NgModule({
  imports: [RouterModule.forChild(SalesRoutes)],
  exports: [RouterModule]
})
export class SalesRoutingModule { }
