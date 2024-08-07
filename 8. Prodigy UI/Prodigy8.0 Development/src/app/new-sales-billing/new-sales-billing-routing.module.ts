import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuardService } from './../auth-guard.service';
import { appConfirmationGuard } from '../appconfirmation-guard';
import { NewSalesBillingComponent } from './new-sales-billing.component';

const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    canDeactivate: [appConfirmationGuard],
    component: NewSalesBillingComponent, data: {
      title: 'New Sales Billing',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Sales Billing' }]
    },
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class NewSalesBillingRoutingModule { }
