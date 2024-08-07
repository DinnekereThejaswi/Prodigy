import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { OnlineOrdersRoutingModule } from './online-orders-routing.module';
import { AssignmentGenerationComponent } from './assignment-generation/assignment-generation.component';
import { ItemPickingComponent } from './item-picking/item-picking.component';
import { ItemPackagingComponent } from './item-packaging/item-packaging.component';
import { InvoiceShipLabelGenerationComponent } from './invoice-ship-label-generation/invoice-ship-label-generation.component';
import { ShippingComponent } from './shipping/shipping.component';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { FullModule } from '../layouts/full/full.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxPaginationModule } from 'ngx-pagination';

@NgModule({
  declarations: [AssignmentGenerationComponent, ItemPickingComponent, ItemPackagingComponent, InvoiceShipLabelGenerationComponent, ShippingComponent],
  imports: [
    CommonModule,
    OnlineOrdersRoutingModule, CollapseModule,
    BsDatepickerModule, FullModule, FormsModule, ReactiveFormsModule, NgxPaginationModule
  ]
})
export class OnlineOrdersModule { }
