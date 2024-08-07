import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { OrdersRoutingModule } from './orders-routing.module';
import { OrdersComponent } from './orders.component';
import { CustomerModule } from '../masters/customer/customer.module';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { FullModule } from './../layouts/full/full.module';
import { ItemdetailsComponent } from './itemdetails/itemdetails.component';
import { ReceiptComponent } from './receipt/receipt.component';
import { AttachorderComponent } from './attachorder/attachorder.component';
import { ModifyorderComponent } from './modify-order/modify-order.component';
import { ReprintOrderComponent } from './reprint-order/reprint-order.component';
import { PaymentModule } from '../payment/payment.module';
import { OrderReceiptComponent } from './order-receipt/order-receipt.component';
import { OrderCancelComponent } from './order-cancel/order-cancel.component';
import { ViewordersComponent } from './vieworders/vieworders.component';
import { ReceiptCancelComponent } from './receipt-cancel/receipt-cancel.component';
import { CloseOrderComponent } from './close-order/close-order.component';
import { NgxPaginationModule } from 'ngx-pagination';
// Import ngx-barcode module
import { NgxBarcode6Module } from 'ngx-barcode6';
import { OrderIssueToCpcComponent } from './order-issue-to-cpc/order-issue-to-cpc.component';
import { OrderClosedOtherbranchComponent } from './order-closed-otherbranch/order-closed-otherbranch.component';
import { DormantOrderComponent } from './dormant-order/dormant-order.component';
import { UnlockDormantOrderComponent } from './unlock-dormant-order/unlock-dormant-order.component';

@NgModule({
  declarations: [OrdersComponent, ItemdetailsComponent, AttachorderComponent, ModifyorderComponent, ReprintOrderComponent, OrderReceiptComponent, OrderCancelComponent, ViewordersComponent, ReceiptCancelComponent, CloseOrderComponent, ReceiptComponent, OrderIssueToCpcComponent, OrderClosedOtherbranchComponent, DormantOrderComponent, UnlockDormantOrderComponent],
  imports: [
    CommonModule,
    FormsModule,
    OrdersRoutingModule,
    ReactiveFormsModule,
    CustomerModule,
    CollapseModule,
    FullModule,
    BsDatepickerModule,
    PaymentModule,
    NgxPaginationModule,
    NgxBarcode6Module
  ],
  exports: [OrdersComponent, AttachorderComponent, ModifyorderComponent],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ]
})
export class OrdersModule { }
