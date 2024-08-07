import { NgModule } from '@angular/core';
import { AuthGuardService } from './../auth-guard.service';
import { Routes, RouterModule } from '@angular/router';
import { PurchaseBillingComponent } from './purchase-billing/purchase-billing.component';
import { PurchaseComponent } from './purchase.component';
import { ReprintPurchaseestimationComponent } from './reprint-purchaseestimation/reprint-purchaseestimation.component';
import { PurchaseCancelComponent } from './purchase-cancel/purchase-cancel.component';
import { ReprintPurchasebillComponent } from './reprint-purchasebill/reprint-purchasebill.component';
import { appConfirmationGuard } from '../appconfirmation-guard';

export const PurchaseRoutes: Routes = [
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    canDeactivate: [appConfirmationGuard],
    component: PurchaseComponent, data: {
      title: 'Purchase Estimation',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Purchase Estimation' }]
    },
  },
  {
    path: 'reprintpurchaseestimation',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: ReprintPurchaseestimationComponent, data: {
      title: 'Reprint Purchase Estimation',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Reprint Purchase Estimation' }]
    }
  },
  {
    path: 'purchase-billing',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    canDeactivate: [appConfirmationGuard],
    component: PurchaseBillingComponent, data: {
      title: 'Purchase Billing',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Purchase Billing' }]
    },
  },
  {
    path: 'purchase-cancel',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    canDeactivate: [appConfirmationGuard],
    component: PurchaseCancelComponent, data: {
      title: 'Cancel Purchase Bill',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Cancel Purchase Bill' }]
    },
  },
  {
    path: 'reprint-purchasebill',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: ReprintPurchasebillComponent, data: {
      title: 'Reprint Purchase Bill',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Reprint Purchase Bill' }]
    },
  }
];

@NgModule({
  imports: [RouterModule.forChild(PurchaseRoutes)],
  exports: [RouterModule]
})
export class PurchaseRoutingModule { }
