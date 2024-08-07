import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { GsGroupingRoutingModule } from './gs-grouping-routing.module';
// import { GsGroupingComponent } from './gs-grouping.component';
import { ManageGsGroupingComponent } from './manage-gs-grouping/manage-gs-grouping.component';

@NgModule({
  declarations: [ ManageGsGroupingComponent],
  imports: [
    CommonModule,
    GsGroupingRoutingModule,
    FormsModule,
    ReactiveFormsModule
  ]
})
export class GsGroupingModule { }
