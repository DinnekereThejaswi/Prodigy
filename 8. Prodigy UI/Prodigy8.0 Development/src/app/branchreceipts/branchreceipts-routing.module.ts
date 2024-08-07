import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AutobarcodeReceiptsComponent } from './autobarcode-receipts/autobarcode-receipts.component';
import { BarcodereceiptsComponent } from './barcodereceipts/barcodereceipts.component'
import { NontagreceiptsComponent } from './nontagreceipts/nontagreceipts.component';
import { ReceiptscancelComponent } from './receiptscancel/receiptscancel.component';
import { ReprintReceiptsComponent } from './reprint-receipts/reprint-receipts.component';

const routes: Routes = [
  {
    path: 'barcodereceipts',
    canActivate: [],
    canActivateChild: [],
    component: BarcodereceiptsComponent, data: {
      title: 'Barcode Receipts',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Barcode Receipts' }]
    }
  },
  {
    path: 'nontagreceipts',
    canActivate: [],
    canActivateChild: [],
    component: NontagreceiptsComponent, data: {
      title: 'Non-TAG Receipts',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Non-TAG Receipts' }]
    }
  },
  {
    path: 'autobarcode-receipts',
    canActivate: [],
    canActivateChild: [],
    component: AutobarcodeReceiptsComponent, data: {
      title: 'Auto Barcode Receipts',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Auto Barcode Receipts' }]
    }
  },
  {
    path: 'receiptscancel',
    canActivate: [],
    canActivateChild: [],
    component: ReceiptscancelComponent, data: {
      title: 'Receipt Cancel',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Receipt Cancel' }]
    }
  },
  {
    path: 'reprint-receipts',
    canActivate: [],
    canActivateChild: [],
    component: ReprintReceiptsComponent, data: {
      title: 'Re-Print Receipts',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Re-Print Receipts' }]
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BranchreceiptsRoutingModule { }
