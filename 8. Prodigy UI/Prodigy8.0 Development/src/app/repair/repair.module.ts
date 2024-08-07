import { PaymentModule } from './../payment/payment.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { FullModule } from './../layouts/full/full.module';
import { CustomerModule } from './../masters/customer/customer.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RepairRoutingModule } from './repair-routing.module';
import { RepairReceiptsComponent } from './repair-receipts/repair-receipts.component';
import { RepairCancelComponent } from './repair-cancel/repair-cancel.component';
import { DeliveryCancelComponent } from './delivery-cancel/delivery-cancel.component';
import { RepairDeliveryComponent } from './repair-delivery/repair-delivery.component';
import { RepairReprintComponent } from './repair-reprint/repair-reprint.component';
import { CollapseModule } from 'ngx-bootstrap/collapse';

@NgModule({
  declarations: [RepairReceiptsComponent, RepairCancelComponent, DeliveryCancelComponent, RepairDeliveryComponent, RepairReprintComponent],
  imports: [
    CommonModule,
    RepairRoutingModule,
    CustomerModule,
    FullModule,
    BsDatepickerModule,
    FormsModule,
    ReactiveFormsModule,
    PaymentModule,
    CollapseModule
  ]
})
export class RepairModule { }
