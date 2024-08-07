import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FullComponent } from './layouts/full/full.component';


export const routes: Routes = [
    {
        path: '',
        children: [{
            path: '',
            loadChildren: './authentication/authentication.module#AuthenticationModule'
        }]
    },
    {
        path: '',
        component: FullComponent,
        children: [
            { path: 'errorpages', loadChildren: () => import('./errorpages/errorpages.module').then(m => m.ErrorpagesModule) },
            { path: 'component', loadChildren: () => import('./component/component.module').then(m => m.ComponentsModule) },
            { path: 'redirect', loadChildren: () => import('./redirect/redirect.module').then(m => m.RedirectModule) },
            { path: 'sales', loadChildren: () => import('./sales/sales.module').then(m => m.SalesModule) },
            { path: 'customer', loadChildren: () => import('./masters/customer/customer.module').then(m => m.CustomerModule) },
            { path: 'estimation', loadChildren: () => import('./estimation/estimation.module').then(m => m.EstimationModule) },
            { path: 'orders', loadChildren: () => import('./orders/orders.module').then(m => m.OrdersModule) },
            { path: 'salesreturn', loadChildren: () => import('./salesreturn/salesreturn.module').then(m => m.SalesreturnModule) },
            { path: 'purchase', loadChildren: () => import('./purchase/purchase.module').then(m => m.PurchaseModule) },
            { path: 'barcode', loadChildren: () => import('./barcodedetails/barcodedetails.module').then(m => m.BarcodedetailsModule) },
            { path: 'add-barcode', loadChildren: () => import('./sales/add-barcode/add-barcode.module').then(m => m.AddBarcodeModule) },
            { path: 'repair', loadChildren: () => import('./repair/repair.module').then(m => m.RepairModule) },
            { path: 'cbwidget', loadChildren: () => import('./cbwidget/cbwidget.module').then(m => m.CbwidgetModule) },
            { path: 'stocks', loadChildren: () => import('./stocks/stocks.module').then(m => m.StocksModule) },
            { path: 'credit-receipt', loadChildren: () => import('./credit-receipt/credit-receipt.module').then(m => m.CreditReceiptModule) },
            { path: 'accounts', loadChildren: () => import('./accounts/accounts.module').then(m => m.AccountsModule) },
            { path: 'tag-split', loadChildren: () => import('./tag-split/tag-split.module').then(m => m.TagSplitModule) },
            { path: 'sales-billing', loadChildren: () => import('./sales-billing/sales-billing.module').then(m => m.SalesBillingModule) },
            { path: 'new-sales-billing', loadChildren: () => import('./new-sales-billing/new-sales-billing.module').then(m => m.NewSalesBillingModule) },
            { path: 'dashboard', loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule) },
            { path: 'masters', loadChildren: () => import('./masters/masters.module').then(m => m.MastersModule) },
            { path: 'religion-master', loadChildren: () => import('./masters/religion-master/religion-master.module').then(m => m.ReligionMasterModule) },
            { path: 'machine-registration', loadChildren: () => import('./masters/machine-registration/machine-registration.module').then(m => m.MachineRegistrationModule) },
            // { path: 'gs-grouping', loadChildren: () => import('./masters/gs-grouping/gs-grouping.module').then(m => m.GsGroupingModule) },
            { path: 'tds-master', loadChildren: () => import('./masters/tds-master/tds-master.module').then(m => m.TdsMasterModule) },
            { path: 'UiFramework', loadChildren: () => import('./UiFramework/ui-framework.module').then(m => m.UIFrameworkModule) },
            { path: 'reports', loadChildren: () => import('./reports/reports.module').then(m => m.ReportsModule) },
            { path: 'online-order-management-system', loadChildren: () => import('./online-order-management-system/online-order-management-system.module').then(m => m.OnlineOrderManagementSystemModule) },
            { path: 'online-marking-barcode-inventory', loadChildren: () => import('./online-marking-barcode-inventory/online-marking-barcode-inventory.module').then(m => m.OnlineMarkingBarcodeInventoryModule) },
            { path: 'online-orders', loadChildren: () => import('./online-orders/online-orders.module').then(m => m.OnlineOrdersModule) },
            { path: 'branchreceipts', loadChildren: () => import('./branchreceipts/branchreceipts.module').then(m => m.BranchreceiptsModule) },
            { path: 'branchissue', loadChildren: () => import('./branchissue/branchissue.module').then(m => m.BranchissueModule) },
            { path: 'opg-process', loadChildren: () => import('./opg-process/opg-process.module').then(m => m.OpgProcessModule) },
            { path: 'utilities', loadChildren: () => import('./utilities/utilities.module').then(m => m.UtilitiesModule) },
            { path: 'others', loadChildren: () => import('./others/others.module').then(m => m.OthersModule) },
            { path: 'stone-sales', loadChildren: () => import('./stone-sales/stone-sales.module').then(m => m.StoneSalesModule) },
            { path: 'errorpages', loadChildren: () => import('./errorpages/errorpages.module').then(m => m.ErrorpagesModule) },
            // { path: 'treeview', loadChildren: './treeview/treeview.module#TreeViewModule'},
            //{ path: 'reprintsalesbilling', loadChildren: './reprint-salesbilling/sales-billing.module#SalesBillingModule'} 
        ]
    },
    {
        path: '**',
        redirectTo: '404'
    }];

@NgModule({
    imports: [RouterModule.forRoot(routes), NgbModule],
    exports: [RouterModule]
})

export class AppRoutingModule { }