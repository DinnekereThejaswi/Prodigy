import { FullModule } from './../layouts/full/full.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';

import { EstimationRoutingModule } from './estimation-routing.module';
import { EstimationComponent } from './estimation.component';
import { SalesModule } from '../sales/sales.module';
import { PurchaseModule } from '../purchase/purchase.module';
import { OrdersModule } from '../orders/orders.module';
import { CustomerModule } from '../masters/customer/customer.module';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { SalesreturnModule } from './../salesreturn/salesreturn.module';
import { ReprintSalesestimationComponent } from './reprint-salesestimation/reprint-salesestimation.component';
import { MainPipe } from '../core/directives/main-pipe.module';
import { MergeestimationComponent } from './mergeestimation/mergeestimation.component';
import { MergeestimationbarcodesComponent } from './mergeestimationbarcodes/mergeestimationbarcodes.component';
import { WeightschemeComponent } from './weightscheme/weightscheme.component';



@NgModule({
  declarations: [EstimationComponent, ReprintSalesestimationComponent, MergeestimationComponent, MergeestimationbarcodesComponent, WeightschemeComponent],
  imports: [
    CommonModule,
    EstimationRoutingModule,
    SalesModule,
    PurchaseModule, OrdersModule, CustomerModule, CollapseModule, SalesreturnModule,
    FormsModule,
    FullModule,
    MainPipe,
    ReactiveFormsModule
  ],
  exports: [ReprintSalesestimationComponent],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ]
})

export class EstimationModule { }
