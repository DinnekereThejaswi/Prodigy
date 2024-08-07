import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { appConfirmationGuard } from '../appconfirmation-guard';
import { AuthGuardService } from '../auth-guard.service';
import { OnlineOrderCancelComponent } from './online-order-cancel/online-order-cancel.component';
import { OnlineOrderManagementSystemComponent } from './online-order-management-system.component';
import { ShipmentUpdateComponent } from './shipment-update/shipment-update.component';


export const OrderManagementSystemRoutes: Routes = [
  {
    path: '',
    //canDeactivate: [appConfirmationGuard],
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: OnlineOrderManagementSystemComponent, data: {
      title: 'Online Order Management System',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Online Order Management System' }]
    },
  },
  {
    path: 'online-order-cancel',
    //canDeactivate: [appConfirmationGuard],
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: OnlineOrderCancelComponent, data: {
      title: 'Online Order Cancel',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Online Order Cancel' }]
    },
  },
  {
    path: 'shipment-update',
    //canDeactivate: [appConfirmationGuard],
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: ShipmentUpdateComponent, data: {
      title: 'Shipment Update',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Shipment Update' }]
    },
  }
];

@NgModule({
  imports: [RouterModule.forChild(OrderManagementSystemRoutes)],
  exports: [RouterModule]
})


export class OnlineOrderManagementSystemRoutingModule { }
