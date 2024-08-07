import { ManageCustomerComponent } from './manage-customer/manage-customer.component';
import { CustomerComponent } from './customer.component';
import { AuthGuardService } from './../../auth-guard.service';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ViewCustomerComponent } from './view-customer/view-customer.component';
import { appConfirmationGuard } from '../../appconfirmation-guard';
const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    canDeactivate: [appConfirmationGuard],
    component: CustomerComponent, data: {
      title: 'Customer',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Customer' }]
    },
  },
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    canDeactivate: [appConfirmationGuard],
    children: [{
      path: 'add',
      data: {
        title: 'Manage - Customer',
        urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Add' }]
      },

    }, {
      path: 'edit/:id', data: {
        title: 'Manage - Customer',
        urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Edit' }]
      },
      component: ManageCustomerComponent,
    },
    {
      path: 'view/:id', data: {
        title: 'View - Customer',
        urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'View' }]
      },
      component: ViewCustomerComponent,
    },]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CustomerRoutingModule { }
