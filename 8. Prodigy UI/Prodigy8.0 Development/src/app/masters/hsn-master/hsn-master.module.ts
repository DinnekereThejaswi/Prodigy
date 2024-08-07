import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HsnMasterRoutingModule } from './hsn-master-routing.module';
import { ManageHsnMasterComponent } from './manage-hsn-master/manage-hsn-master.component';

@NgModule({
  declarations: [ManageHsnMasterComponent],
  imports: [
    CommonModule,
    HsnMasterRoutingModule,
    FormsModule,
    ReactiveFormsModule
  ]
})
export class HsnMasterModule { }
