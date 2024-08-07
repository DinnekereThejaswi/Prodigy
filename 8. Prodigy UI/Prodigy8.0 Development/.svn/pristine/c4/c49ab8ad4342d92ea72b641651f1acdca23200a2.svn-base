import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { OpgSeggregationComponent } from './opg-seggregation/opg-seggregation.component';
import { ReprintOpgIssueComponent } from './reprint-opg-issue/reprint-opg-issue.component';
import { ReprintOpgReceiptComponent } from './reprint-opg-receipt/reprint-opg-receipt.component';
import { OpgMeltingIssueComponent } from './opg-melting-issue/opg-melting-issue.component';
import { OpgMeltingReceiptComponent } from './opg-melting-receipt/opg-melting-receipt.component';
import { MgIssueCpcComponent } from './mg-issue-cpc/mg-issue-cpc.component';
import { OpgIssueCancelComponent } from './opg-issue-cancel/opg-issue-cancel.component';
import { OpgReceiptCancelComponent } from './opg-receipt-cancel/opg-receipt-cancel.component';

const routes: Routes = [
  {
    path: 'opg-seggregation',
    canActivate: [],
    canActivateChild: [],
    component: OpgSeggregationComponent, data: {
      title: 'OPG seggregation',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'OPG seggregation' }]
    }
  },
  {
    path: 'reprint-opg-issue',
    canActivate: [],
    canActivateChild: [],
    component: ReprintOpgIssueComponent, data: {
      title: 'Reprint OPG Issue',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Reprint OPG Issue' }]
    }
  },
  {
    path: 'reprint-opg-receipt',
    canActivate: [],
    canActivateChild: [],
    component: ReprintOpgReceiptComponent, data: {
      title: ' Reprint OPG Receipt',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: ' Reprint OPG Receipt' }]
    }
  },
  {
    path: 'opg-melting-issue',
    canActivate: [],
    canActivateChild: [],
    component: OpgMeltingIssueComponent, data: {
      title: 'Melting Issue',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Melting Issue' }]
    }
  },
  {
    path: 'opg-melting-receipt',
    canActivate: [],
    canActivateChild: [],
    component: OpgMeltingReceiptComponent, data: {
      title: 'Opg Melting Receipt',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Opg Melting Receipt' }]
    }
  },
  {
    path: 'opg-mg-issue-cpc',
    canActivate: [],
    canActivateChild: [],
    component: MgIssueCpcComponent, data: {
      title: 'MG Issue to CPC',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'MG Issue to CPC' }]
    }
  },
  {
    path: 'opg-issue-cancel',
    canActivate: [],
    canActivateChild: [],
    component: OpgIssueCancelComponent, data: {
      title: 'OPG Cancel Issue',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'OPG Cancel Issue' }]
    }
  },
  {
    path: 'opg-receipt-cancel',
    canActivate: [],
    canActivateChild: [],
    component: OpgReceiptCancelComponent, data: {
      title: 'OPG Cancel Receipt',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'OPG Cancel Receipt' }]
    }
  }
]


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OpgProcessRoutingModule { }
