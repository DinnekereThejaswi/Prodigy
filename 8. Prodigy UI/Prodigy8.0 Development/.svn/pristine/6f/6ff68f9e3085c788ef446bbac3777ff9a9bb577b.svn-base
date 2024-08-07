import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OpgProcessComponent } from './opg-process.component';
import { OpgProcessRoutingModule } from './opg-process-routing.module';
import { OpgSeggregationComponent } from './opg-seggregation/opg-seggregation.component';
import { ReprintOpgIssueComponent } from './reprint-opg-issue/reprint-opg-issue.component';
import { ReprintOpgReceiptComponent } from './reprint-opg-receipt/reprint-opg-receipt.component';
import { OpgMeltingIssueComponent } from './opg-melting-issue/opg-melting-issue.component';
import { OpgMeltingReceiptComponent } from './opg-melting-receipt/opg-melting-receipt.component';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { FullModule } from './../layouts/full/full.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { MgIssueCpcComponent } from './mg-issue-cpc/mg-issue-cpc.component';
import { OpgIssueCancelComponent } from './opg-issue-cancel/opg-issue-cancel.component';
import { OpgReceiptCancelComponent } from './opg-receipt-cancel/opg-receipt-cancel.component';


@NgModule({
  declarations: [OpgProcessComponent, OpgSeggregationComponent, ReprintOpgReceiptComponent,
    ReprintOpgIssueComponent, OpgMeltingIssueComponent, OpgMeltingReceiptComponent, MgIssueCpcComponent, OpgIssueCancelComponent, OpgReceiptCancelComponent
  ],
  imports: [
    CommonModule, ReactiveFormsModule,
    FormsModule,
    FullModule,
    CollapseModule,
    BsDatepickerModule,
    OpgProcessRoutingModule
  ], schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ]
})
export class OpgProcessModule { }
