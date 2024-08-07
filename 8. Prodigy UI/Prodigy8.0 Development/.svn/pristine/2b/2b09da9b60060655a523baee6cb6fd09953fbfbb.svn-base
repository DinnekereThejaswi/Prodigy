import { BarcodedetailsComponent } from './barcodedetails.component';
import { AuthGuardService } from './../auth-guard.service';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SrBarcodingComponent } from './sr-barcoding/sr-barcoding.component';
import { CtcTransferComponent } from './ctc-transfer/ctc-transfer.component';
import { ItiTransferComponent } from './iti-transfer/iti-transfer.component';
import { EditOrdernoComponent } from './edit-orderno/edit-orderno.component';
import { ReprintBarcodeComponent } from './reprint-barcode/reprint-barcode.component';

export const BarcodedetailsRoutes: Routes = [
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: BarcodedetailsComponent, data: {
      title: 'View Barcode Details',
      urls: [{ title: 'Dashboard', url: 'javascript:void(0)' }, { title: 'View Barcode Details' }]
    },
  },
  {
    path: 'sr-barcoding',
    canActivate: [],
    canActivateChild: [],
    component: SrBarcodingComponent, data: {
      title: 'SR Barcoding',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'SR Barcoding' }]
    }
  },
  {
    path: 'ctc-transfer',
    canActivate: [],
    canActivateChild: [],
    component: CtcTransferComponent, data: {
      title: 'CTC Transfer',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'CTC Transfer' }]
    }
  },
  {
    path: 'iti-transfer',
    canActivate: [],
    canActivateChild: [],
    component: ItiTransferComponent, data: {
      title: 'Item To Item Transfer',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Item To Item Transfer' }]
    }
  },
  {
    path: 'edit-orderno',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: EditOrdernoComponent, data: {
      title: 'Order No Edit in Barcode',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Order No Edit in Barcode' }]
    },
  },
  {
    path: 'reprint-barcode',
    canActivate: [],
    canActivateChild: [],
    component: ReprintBarcodeComponent, data: {
      title: 'Reprint Barcode',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Reprint Barcode' }]
    },
  }
]

@NgModule({
  imports: [RouterModule.forChild(BarcodedetailsRoutes)],
  exports: [RouterModule]
})
export class BarcodedetailsRoutingModule { }
