import { AddBarcodeModule } from './add-barcode/add-barcode.module';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FullModule } from './../layouts/full/full.module';
import { SalesRoutingModule } from './sales-routing.module';
import { SalesComponent } from './sales.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [SalesComponent],
  imports: [
    CommonModule,
    SalesRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    AddBarcodeModule, FullModule
  ],
  exports: [SalesComponent],

  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ]
})
export class SalesModule { }
