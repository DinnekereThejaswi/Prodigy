import { OrderCancelComponent } from './order-cancel/order-cancel.component';
import { ReprintOrderComponent } from './reprint-order/reprint-order.component';
import { NgModule } from '@angular/core';
import { AuthGuardService } from './../auth-guard.service';
import { Routes, RouterModule } from '@angular/router';
import { OrdersComponent } from './orders.component';
import { OrderReceiptComponent } from './order-receipt/order-receipt.component';
import { ViewordersComponent } from './vieworders/vieworders.component';
import { ReceiptCancelComponent } from './receipt-cancel/receipt-cancel.component';
import { CloseOrderComponent } from './close-order/close-order.component';
import { appConfirmationGuard } from '../appconfirmation-guard';
import { OrderClosedOtherbranchComponent } from './order-closed-otherbranch/order-closed-otherbranch.component';
import { OrderIssueToCpcComponent } from './order-issue-to-cpc/order-issue-to-cpc.component';
import { DormantOrderComponent } from './dormant-order/dormant-order.component';
import { UnlockDormantOrderComponent } from './unlock-dormant-order/unlock-dormant-order.component';

export const OrdersRoutes: Routes = [
  {
    path: '',
    canDeactivate: [appConfirmationGuard],
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: OrdersComponent, data: {
      title: 'Orders',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Orders' }]
    },
  },

  //Commented on 21-JAN-2020
  // {
  //   path: 'Customized',
  //   canDeactivate: [appConfirmationGuard],
  //   canActivate: [AuthGuardService], 
  //   canActivateChild: [AuthGuardService],
  //    component:   OrdersComponent, data: {
  //     title: 'Customized Orders',
  //     urls: [{title: 'Dashboard', url: '/dashboard'},{title: 'Customized Orders'}]
  //   },
  // },
  // {
  //   path: 'OrderAdvance',
  //   canDeactivate: [appConfirmationGuard],
  //   canActivate: [AuthGuardService], 
  //   canActivateChild: [AuthGuardService],
  //    component:   OrdersComponent, data: {
  //     title: 'Advance Order',
  //     urls: [{title: 'Dashboard', url: '/dashboard'},{title: 'Order Advance'}]
  //   },
  // },
  // {
  //   path: 'Reserved',
  //   canDeactivate: [appConfirmationGuard],
  //   canActivate: [AuthGuardService], 
  //   canActivateChild: [AuthGuardService],
  //    component:   OrdersComponent, data: {
  //     title: 'Reserved Orders',
  //     urls: [{title: 'Dashboard', url: '/dashboard'},{title: 'Reserved Orders'}]
  //   },
  // },
  {
    path: 'OrderReceipt',
    canDeactivate: [appConfirmationGuard],
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: OrderReceiptComponent, data: {
      title: 'Order Receipt',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Order Receipt' }]
    },
  },
  {
    path: 'OrderCancel',
    // canDeactivate: [appConfirmationGuard],
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: OrderCancelComponent, data: {
      title: 'Cancel Order',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Cancel Order' }]
    },
  },
  {
    path: 'ReceiptCancel',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    canDeactivate: [appConfirmationGuard],
    component: ReceiptCancelComponent, data: {
      title: 'Order Receipt Cancel',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Order Receipt Cancel' }]
    },
  },
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    children: [{
      path: 'reprint',
      data: {
        title: 'Reprint Order',
        urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Reprint Order' }]
      },
      component: ReprintOrderComponent,
    },]
  },
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    children: [{
      path: 'vieworders',
      data: {
        title: 'View Orders',
        urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'View Orders' }]
      },
      component: ViewordersComponent,
    },
    {
      path: 'CloseOrder',
      canDeactivate: [appConfirmationGuard],
      canActivate: [AuthGuardService],
      canActivateChild: [AuthGuardService],
      component: CloseOrderComponent, data: {
        title: 'Close Order',
        urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Close Order' }]
      },
    },
    {
      path: 'order-closed-otherbranch',
      canActivate: [AuthGuardService],
      canActivateChild: [AuthGuardService],
      component: OrderClosedOtherbranchComponent, data: {
        title: 'Order Closed in Other Branch',
        urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Order Closed in Other Branch' }]
      },
    },
    {
      path: 'order-issue-to-cpc',
      canActivate: [AuthGuardService],
      canActivateChild: [AuthGuardService],
      component: OrderIssueToCpcComponent, data: {
        title: 'Order Issue To CPC',
        urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Order Issue To CPC' }]
      },
    },
    {
      path: 'dormant-order',
      canActivate: [AuthGuardService],
      canActivateChild: [AuthGuardService],
      component: DormantOrderComponent, data: {
        title: 'Dormant Order',
        urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Dormant Order' }]
      },
    },
    {
      path: 'unlock-dormant-order',
      canActivate: [AuthGuardService],
      canActivateChild: [AuthGuardService],
      component: UnlockDormantOrderComponent, data: {
        title: 'Unlock Dormant Order',
        urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Unlock Dormant Order' }]
      },
    }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(OrdersRoutes)],
  exports: [RouterModule]
})
export class OrdersRoutingModule { }
