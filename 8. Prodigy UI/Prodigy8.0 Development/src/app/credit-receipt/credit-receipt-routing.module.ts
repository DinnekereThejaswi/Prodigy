import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { appConfirmationGuard } from '../appconfirmation-guard';
import { AuthGuardService } from '../auth-guard.service';
import { CreditReceiptComponent } from './credit-receipt/credit-receipt.component';
import { CancelCreditReceiptComponent } from './cancel-credit-receipt/cancel-credit-receipt.component';
import { ReprintCreditReceiptComponent } from './reprint-credit-receipt/reprint-credit-receipt.component';


// const routes: Routes = [
//   {
//   path: '',
//   canActivate: [AuthGuardService],
//   canActivateChild: [AuthGuardService],
//   children: [{
//     path: 'credit-receipt',
//     data: {
//       title: 'Credit Receipts',
//        urls: [{title: 'View Orders',url: 'javascript:void(0)'},{title: 'Credit Receipts'}]
//     },
//     component: CreditReceiptComponent,
//   },]
// }];
export const routes: Routes = [
  {
    path: 'credit-receipt',
    canDeactivate: [appConfirmationGuard],
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: CreditReceiptComponent, data: {
      title: 'Credit Receipt',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Credit Receipt' }]
    },
  },
  {
    path: 'cancel-credit-receipt',
    canDeactivate: [appConfirmationGuard],
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: CancelCreditReceiptComponent, data: {
      title: 'Cancel Credit Receipt',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Cancel Credit Receipt' }]
    },
  },
  {
    path: 'reprint-credit-receipt',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: ReprintCreditReceiptComponent, data: {
      title: 'Reprint Credit Receipt',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Reprint Credit Receipt' }]
    },
  },


];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CreditReceiptRoutingModule { }
