import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ErrorpagesRoutingModule } from './errorpages-routing.module';
import { ErrorpagesComponent } from './errorpages.component';



@NgModule({
  declarations: [ErrorpagesComponent],
  imports: [
    CommonModule,
    ErrorpagesRoutingModule
  ]
})
export class ErrorpagesModule { }
