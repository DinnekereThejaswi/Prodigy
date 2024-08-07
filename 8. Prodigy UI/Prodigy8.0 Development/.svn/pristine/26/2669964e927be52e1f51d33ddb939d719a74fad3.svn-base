import { SalesBillingComponent } from './sales-billing.component';
import { AuthGuardService } from './../auth-guard.service';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ReprintSalesbillingComponent } from './reprint-salesbilling/reprint-salesbilling.component';
import { appConfirmationGuard } from '../appconfirmation-guard';
import { CancelSalesbillingComponent } from './cancel-salesbilling/cancel-salesbilling.component';
import { BillReceiptDetailsComponent } from './bill-receipt-details/bill-receipt-details.component';

const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    canDeactivate: [appConfirmationGuard],
    component: SalesBillingComponent, data: {
      title: 'Sales Billing',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Sales Billing' }]
    },
  },
  {
    path: 'reprintsalesbilling',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: ReprintSalesbillingComponent, data: {
      title: 'Reprint Bill',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Reprint Bill' }]
    },
  },
  {
    path: 'cancel-salesbilling',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: CancelSalesbillingComponent, data: {
      title: 'Cancel Bill',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Cancel Bill' }]
    },
  },
  {
    path: 'bill-receipt-details',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: BillReceiptDetailsComponent, data: {
      title: 'Bill Receipt Details',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Bill Receipt Details' }]
    },
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})

export class SalesBillingRoutingModule { }
