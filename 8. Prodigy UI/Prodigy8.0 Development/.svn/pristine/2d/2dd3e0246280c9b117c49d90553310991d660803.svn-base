import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SalesModule } from './../sales/sales.module';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FullModule } from './../layouts/full/full.module';
import { BarcodedetailsRoutingModule } from './barcodedetails-routing.module';
import { BarcodedetailsComponent } from './barcodedetails.component';
import { AddBarcodeModule } from '../sales/add-barcode/add-barcode.module';
import { SrBarcodingComponent } from './sr-barcoding/sr-barcoding.component';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { CtcTransferComponent } from './ctc-transfer/ctc-transfer.component';
import { ItiTransferComponent } from './iti-transfer/iti-transfer.component';
import { EditOrdernoComponent } from './edit-orderno/edit-orderno.component';
import { ReprintBarcodeComponent } from './reprint-barcode/reprint-barcode.component';

@NgModule({
  declarations: [BarcodedetailsComponent, SrBarcodingComponent, CtcTransferComponent, ItiTransferComponent, EditOrdernoComponent, ReprintBarcodeComponent],
  imports: [
    CommonModule,
    BarcodedetailsRoutingModule,
    SalesModule,
    ReactiveFormsModule,
    FormsModule,
    FullModule,
    AddBarcodeModule,
    CollapseModule,
    BsDatepickerModule
  ],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ]
})
export class BarcodedetailsModule { }
