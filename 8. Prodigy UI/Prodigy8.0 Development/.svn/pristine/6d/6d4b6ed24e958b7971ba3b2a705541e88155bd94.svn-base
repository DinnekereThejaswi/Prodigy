import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule  } from '@angular/forms';
import { CreditReceiptRoutingModule } from './credit-receipt-routing.module';
import { CreditReceiptComponent } from './credit-receipt/credit-receipt.component';
import { CustomerModule } from '../masters/customer/customer.module';
import { FullModule } from './../layouts/full/full.module';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { PaymentModule } from '../payment/payment.module';
import { CancelCreditReceiptComponent } from './cancel-credit-receipt/cancel-credit-receipt.component';
import { ReprintCreditReceiptComponent } from './reprint-credit-receipt/reprint-credit-receipt.component';

@NgModule({
  declarations: [CreditReceiptComponent, CancelCreditReceiptComponent, ReprintCreditReceiptComponent],
  imports: [
    CommonModule,
    CreditReceiptRoutingModule,
    FormsModule,
    ReactiveFormsModule,CustomerModule,
    PaymentModule,
    FullModule
  ], 
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ]
})
export class CreditReceiptModule { }
