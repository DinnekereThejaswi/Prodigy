import { FullModule } from './../layouts/full/full.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SalesreturnModule } from './../salesreturn/salesreturn.module';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { CustomerModule } from './../masters/customer/customer.module';
import { OrdersModule } from './../orders/orders.module';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { PurchaseModule } from './../purchase/purchase.module';
import { SalesModule } from './../sales/sales.module';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PaymentModule } from '../payment/payment.module';
import { SalesBillingRoutingModule } from './sales-billing-routing.module';
import { SalesBillingComponent } from './sales-billing.component';
import { CancelSalesbillingComponent } from './cancel-salesbilling/cancel-salesbilling.component';
import { BillReceiptDetailsComponent } from './bill-receipt-details/bill-receipt-details.component';
import { NgxPaginationModule } from 'ngx-pagination';

@NgModule({
  declarations: [SalesBillingComponent, CancelSalesbillingComponent, BillReceiptDetailsComponent],
  imports: [
    BsDatepickerModule,
    CommonModule,
    SalesBillingRoutingModule,
    SalesModule,
    PurchaseModule, OrdersModule, CustomerModule, CollapseModule, SalesreturnModule,
    FormsModule,
    ReactiveFormsModule,
    FullModule,
    PaymentModule,
    NgxPaginationModule
  ],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ]
})

export class SalesBillingModule { }
