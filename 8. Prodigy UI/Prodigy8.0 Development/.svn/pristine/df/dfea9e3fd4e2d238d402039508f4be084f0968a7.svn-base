import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuardService } from '../auth-guard.service';
import { CashVoucherEntryComponent } from './cash-voucher-entry/cash-voucher-entry.component';
import { SubGroupComponent } from './sub-group/sub-group.component';
import { MasterGroupComponent } from './master-group/master-group.component';
import { LedgerComponent } from './ledger/ledger.component';
import { NarrationComponent } from './narration/narration.component';
import { ChequeEntryComponent } from './cheque-entry/cheque-entry.component';
import { VoucherCancelComponent } from './voucher-cancel/voucher-cancel.component';
import { ChequeClosingComponent } from './cheque-closing/cheque-closing.component';
import { CashInHandComponent } from './cash-in-hand/cash-in-hand.component';
import { AccountsUpdateComponent } from './accounts-update/accounts-update.component';
import { BankVoucherEntryComponent } from './bank-voucher-entry/bank-voucher-entry.component';
import { JournalEntryComponent } from './journal-entry/journal-entry.component';
import { ReprintVoucherComponent } from './reprint-voucher/reprint-voucher.component';
import { ContraEntryComponent } from './contra-entry/contra-entry.component';
import { CashbackComponent } from './cashback/cashback.component';
import { AccCodeSettingComponent } from './acc-code-setting/acc-code-setting.component'
import { AccountPostingSetupComponent } from './account-posting-setup/account-posting-setup.component';
import { ExpenseVoucherCancelComponent } from './expense-voucher-cancel/expense-voucher-cancel.component';
import { VendorMasterComponent } from './vendor-master/vendor-master.component';
import { ExpenseVoucherComponent } from './expense-voucher/expense-voucher.component';

export const routes: Routes = [
  {
    path: 'account-posting-setup',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: AccountPostingSetupComponent, data: {
      title: 'Account Posting Setup',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Account Posting Setup' }]
    },
  },
  {
    path: 'acc-code-setting',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: AccCodeSettingComponent, data: {
      title: 'Acc Code Settings',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Acc Code Settings' }]
    },
  },
  {
    path: 'cash-voucher',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: CashVoucherEntryComponent, data: {
      title: 'Cash Voucher Entry',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Cash Voucher Entry' }]
    },
  },
  {
    path: 'cashback',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: CashbackComponent, data: {
      title: 'Cashback',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Cashback' }]
    },
  },
  {
    path: 'sub-group',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: SubGroupComponent, data: {
      title: 'Sub Group',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Sub Group' }]
    },
  },
  {
    path: 'master-group',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: MasterGroupComponent, data: {
      title: 'Master Group',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Master Group' }]
    },
  },
  {
    path: 'ledger',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: LedgerComponent, data: {
      title: 'Ledger',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Ledger' }]
    },
  },
  {
    path: 'narration',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: NarrationComponent, data: {
      title: 'Narration',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Narration' }]
    },
  },
  {
    path: 'cheque-entry',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: ChequeEntryComponent, data: {
      title: 'Cheque Entry',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Cheque Entry' }]
    },
  },
  {
    path: 'voucher-cancel',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: VoucherCancelComponent, data: {
      title: 'Voucher Cancel',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Voucher Cancel' }]
    },
  },
  {
    path: 'cheque-closing',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: ChequeClosingComponent, data: {
      title: 'Cheque Closing',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Cheque Closing' }]
    },
  },
  {
    path: 'cash-in-hand',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: CashInHandComponent, data: {
      title: 'Cash In Hand',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Cash In Hand' }]
    },
  },
  {
    path: 'accounts-update',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: AccountsUpdateComponent, data: {
      title: 'Accounts Update',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Accounts Update' }]
    },
  },
  {
    path: 'bank-voucher-entry',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: BankVoucherEntryComponent, data: {
      title: 'Bank Voucher Entry',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Bank Voucher Entry' }]
    },
  },
  {
    path: 'journal-entry',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: JournalEntryComponent, data: {
      title: 'Journal Entry',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Journal Entry' }]
    },
  },
  {
    path: 'reprint-voucher',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: ReprintVoucherComponent, data: {
      title: 'Reprint Voucher',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Reprint Voucher' }]
    },
  },
  {
    path: 'contra-entry',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: ContraEntryComponent, data: {
      title: 'Contra Entry',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Contra Entry' }]
    }
  },
  {
    path: 'expense-voucher-cancel',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: ExpenseVoucherCancelComponent, data: {
      title: 'Expense Voucher Cancel',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Expense Voucher Cancel' }]
    }
  }
  ,
  {
    path: 'expense-voucher',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: ExpenseVoucherComponent, data: {
      title: 'Expense Voucher Entry',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Expense Voucher Entry' }]
    }
  },
  {
    path: 'vendor-master',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: VendorMasterComponent, data: {
      title: 'Vendor Master',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Vendor Master' }]
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})

export class AccountsRoutingModule { }