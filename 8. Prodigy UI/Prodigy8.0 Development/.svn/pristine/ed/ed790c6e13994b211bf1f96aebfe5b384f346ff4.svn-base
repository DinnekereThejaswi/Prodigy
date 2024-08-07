import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ItemPickingComponent } from './item-picking/item-picking.component';
import { AssignmentGenerationComponent } from './assignment-generation/assignment-generation.component';
import { InvoiceShipLabelGenerationComponent } from './invoice-ship-label-generation/invoice-ship-label-generation.component';
import { ItemPackagingComponent } from './item-packaging/item-packaging.component';
import { ShippingComponent } from './shipping/shipping.component';

export const OnlineOrdersroutes: Routes = [
  {
    path: 'assignment-generation',
    canActivate: [],
    canActivateChild: [],
    component: AssignmentGenerationComponent, data: {
      title: 'Assignment Generation',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Assignment Generation' }]
    },
  },
  {
    path: 'item-picking',
    canActivate: [],
    canActivateChild: [],
    component: ItemPickingComponent, data: {
      title: 'Item Picking',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Item Picking' }]
    },
  },
  {
    path: 'item-packaging',
    canActivate: [],
    canActivateChild: [],
    component: ItemPackagingComponent, data: {
      title: 'Item Packaging',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Item Packaging' }]
    },
  },
  {
    path: 'invoice-ship-label-generation',
    canActivate: [],
    canActivateChild: [],
    component: InvoiceShipLabelGenerationComponent, data: {
      title: 'Invoice Ship Label Generation',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Invoice Ship Label Generation' }]
    },
  },
  {
    path: 'shipping',
    canActivate: [],
    canActivateChild: [],
    component: ShippingComponent, data: {
      title: 'Shipping',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Shipping' }]
    },
  }
];

@NgModule({
  imports: [RouterModule.forChild(OnlineOrdersroutes)],
  exports: [RouterModule]
})
export class OnlineOrdersRoutingModule { }
