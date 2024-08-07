import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { PurchaseRoutingModule } from './purchase-routing.module';
import { PurchaseComponent } from './purchase.component';
import { FormsModule } from '@angular/forms';
import { DiamondComponent } from './diamond/diamond.component';
import { FullModule } from './../layouts/full/full.module';
import { CustomerModule } from '../masters/customer/customer.module';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { AttacholdgoldComponent } from './attacholdgold/attacholdgold.component';
import { StoneComponent } from './stone/stone.component';
import { NgxPaginationModule } from 'ngx-pagination';
import { ReprintPurchaseestimationComponent } from './reprint-purchaseestimation/reprint-purchaseestimation.component';
import { PurchaseBillingComponent } from './purchase-billing/purchase-billing.component';
import { PaymentModule } from '../payment/payment.module';
import { PurchaseCancelComponent } from './purchase-cancel/purchase-cancel.component';
import { ReprintPurchasebillComponent } from './reprint-purchasebill/reprint-purchasebill.component';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';

@NgModule({
  declarations: [PurchaseComponent, DiamondComponent, AttacholdgoldComponent, StoneComponent, ReprintPurchaseestimationComponent, PurchaseBillingComponent, PurchaseCancelComponent, ReprintPurchasebillComponent],
  imports: [
    CommonModule,
    PurchaseRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    CustomerModule,
    CollapseModule,
    NgxPaginationModule,
    PaymentModule,
    FullModule,BsDatepickerModule
  ],
  exports: [PurchaseComponent, AttacholdgoldComponent, ReprintPurchaseestimationComponent],

  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ]
})
export class PurchaseModule { }
