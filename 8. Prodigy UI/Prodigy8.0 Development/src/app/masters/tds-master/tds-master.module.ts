import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TdsMasterRoutingModule } from './tds-master-routing.module';
import {ManageTdsMasterComponent} from './manage-tds-master/manage-tds-master.component'


@NgModule({
  declarations: [ManageTdsMasterComponent],
  imports: [
    CommonModule,
    TdsMasterRoutingModule,
    FormsModule,
    ReactiveFormsModule
  ]
})
export class TdsMasterModule { }
