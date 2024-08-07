import { RepairReprintComponent } from './repair-reprint/repair-reprint.component';
import { appConfirmationGuard } from './../appconfirmation-guard';
import { RepairDeliveryComponent } from './repair-delivery/repair-delivery.component';
import { DeliveryCancelComponent } from './delivery-cancel/delivery-cancel.component';
import { RepairCancelComponent } from './repair-cancel/repair-cancel.component';
import { RepairReceiptsComponent } from './repair-receipts/repair-receipts.component';
import { AuthGuardService } from './../auth-guard.service';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: 'repair-receipts',
    canDeactivate: [appConfirmationGuard],
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: RepairReceiptsComponent, data: {
      title: 'Repair Receipts',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Repair Receipts' }]
    }
  },
  {
    path: 'repair-delivery',
    canDeactivate: [appConfirmationGuard],
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: RepairDeliveryComponent, data: {
      title: 'Repair Delivery',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Repair Delivery' }]
    }
  },
  {
    path: 'repair-cancel',
    canDeactivate: [appConfirmationGuard],
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: RepairCancelComponent, data: {
      title: 'Repair Cancel',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Repair Cancel' }]
    }
  },
  {
    path: 'repair-reprint',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: RepairReprintComponent, data: {
      title: 'Repair Reprint',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Repair Reprint' }]
    }
  },
  {
    path: 'delivery-cancel',
    canDeactivate: [appConfirmationGuard],
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: DeliveryCancelComponent, data: {
      title: 'Repair Delivery Cancel',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Repair Delivery Cancel' }]
    }
  }  
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RepairRoutingModule { }
