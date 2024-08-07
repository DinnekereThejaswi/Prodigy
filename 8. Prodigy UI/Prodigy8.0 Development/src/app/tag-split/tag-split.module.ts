import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import {TagSplitComponent} from './tag-split.component';
import { TagSplitRoutingModule } from './tag-split-routing.module';
import { FormsModule,ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [TagSplitComponent],
  imports: [
    CommonModule,
    TagSplitRoutingModule,
    FormsModule,
    ReactiveFormsModule
  ],
  exports: [TagSplitComponent],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ]
})
export class TagSplitModule { }
