import { FullModule } from './../../layouts/full/full.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgxPaginationModule} from 'ngx-pagination';
import { CustomerRoutingModule } from './customer-routing.module';
import { CustomerComponent } from './customer.component';
import { ManageCustomerComponent } from './manage-customer/manage-customer.component';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { ViewCustomerComponent } from './view-customer/view-customer.component';



@NgModule({
  declarations: [CustomerComponent, ManageCustomerComponent, ViewCustomerComponent],
  imports: [
    CommonModule,
    CustomerRoutingModule,
    NgxPaginationModule,
    ReactiveFormsModule,
    BsDatepickerModule.forRoot(),
    FormsModule,
    FullModule
  ],
  exports:[CustomerComponent, ManageCustomerComponent],

  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ]
})
export class CustomerModule { }
