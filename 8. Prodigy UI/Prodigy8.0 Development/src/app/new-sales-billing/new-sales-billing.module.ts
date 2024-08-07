import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NewSalesBillingRoutingModule } from './new-sales-billing-routing.module';
import { NewSalesBillingComponent } from './new-sales-billing.component';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { FullModule } from './../layouts/full/full.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CustomerModule } from './../masters/customer/customer.module';
import { PaymentModule } from '../payment/payment.module';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';


@NgModule({
  declarations: [NewSalesBillingComponent],
  imports: [
    CommonModule,
    NewSalesBillingRoutingModule,
    CollapseModule,
    FullModule,
    FormsModule,
    ReactiveFormsModule, CustomerModule,
    PaymentModule, BsDatepickerModule
  ],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ],
})

export class NewSalesBillingModule { }
