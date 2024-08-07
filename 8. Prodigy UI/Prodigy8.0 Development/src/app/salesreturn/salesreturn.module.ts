import { PaymentModule } from './../payment/payment.module';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NgxPaginationModule } from 'ngx-pagination';
import { FullModule } from './../layouts/full/full.module';
import { SalesreturnRoutingModule } from './salesreturn-routing.module';
import { SalesreturnComponent } from './salesreturn.component';
import { CustomerModule } from '../masters/customer/customer.module';
import { ItemdetailsComponent } from './itemdetails/itemdetails.component';
import { ConfirmSalesreturnComponent } from './confirm-salesreturn/confirm-salesreturn.component';
import { AttachsalesreturnComponent } from './attachsalesreturn/attachsalesreturn.component';
import { CancelSalesreturnComponent } from './cancel-salesreturn/cancel-salesreturn.component';
import { ReprintSalesreturnComponent } from './reprint-salesreturn/reprint-salesreturn.component';
import { DeleteSalesreturnComponent } from './delete-salesreturn/delete-salesreturn.component';
import { CollapseModule } from 'ngx-bootstrap/collapse';


@NgModule({
  declarations: [SalesreturnComponent, ItemdetailsComponent, ConfirmSalesreturnComponent, AttachsalesreturnComponent, CancelSalesreturnComponent, ReprintSalesreturnComponent, DeleteSalesreturnComponent],
  imports: [
    CommonModule,
    SalesreturnRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    CollapseModule,
    CustomerModule,
    PaymentModule,
    NgxPaginationModule,
    FullModule
  ],
  exports: [SalesreturnComponent, AttachsalesreturnComponent],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ]
})
export class SalesreturnModule { }
