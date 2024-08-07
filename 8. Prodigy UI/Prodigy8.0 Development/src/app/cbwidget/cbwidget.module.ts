import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CbwidgetRoutingModule } from './cbwidget-routing.module';
import { CbwidgetComponent } from './cbwidget.component';

@NgModule({
  declarations: [CbwidgetComponent],
  imports: [
    CommonModule,
    CbwidgetRoutingModule,
    FormsModule,
    ReactiveFormsModule
  ],
  exports: [CbwidgetComponent],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ]
})
export class CbwidgetModule { }
