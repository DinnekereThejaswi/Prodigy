import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BranchreceiptsRoutingModule } from './branchreceipts-routing.module';
import { BarcodereceiptsComponent } from './barcodereceipts/barcodereceipts.component';
import { FullModule } from './../layouts/full/full.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NontagreceiptsComponent } from './nontagreceipts/nontagreceipts.component';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { AutobarcodeReceiptsComponent } from './autobarcode-receipts/autobarcode-receipts.component';
import { ReceiptscancelComponent } from './receiptscancel/receiptscancel.component';
import { ReprintReceiptsComponent } from './reprint-receipts/reprint-receipts.component';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { StoneDiamondComponent } from './nontagreceipts/stone-diamond/stone-diamond.component';


@NgModule({
  declarations: [BarcodereceiptsComponent, NontagreceiptsComponent, AutobarcodeReceiptsComponent, ReceiptscancelComponent, ReprintReceiptsComponent, StoneDiamondComponent],
  imports: [
    CommonModule, ReactiveFormsModule,
    FormsModule,
    FullModule,
    BranchreceiptsRoutingModule,
    CollapseModule,
    BsDatepickerModule
  ],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ]
})
export class BranchreceiptsModule { }
