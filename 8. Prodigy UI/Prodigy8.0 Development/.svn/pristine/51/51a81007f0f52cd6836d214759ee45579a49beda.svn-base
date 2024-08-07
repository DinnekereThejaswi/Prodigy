import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { FullModule } from './../layouts/full/full.module';
import { ReactiveFormsModule,FormsModule } from '@angular/forms';
import { StockClearComponent } from './stock-clear/stock-clear.component';
import { GsStockComponent } from './gs-stock/gs-stock.component';
import { StockTakingComponent } from './stock-taking/stock-taking.component';
import { StockCheckComponent } from './stock-check/stock-check.component';
import { CounterStockComponent } from './counter-stock/counter-stock.component';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Ng2SearchPipeModule } from 'ng2-search-filter';
import { StocksRoutingModule } from './stocks-routing.module';
import { ReprintStockTakingComponent } from './reprint-stock-taking/reprint-stock-taking.component';

@NgModule({
  declarations: [CounterStockComponent,StockCheckComponent,StockTakingComponent,GsStockComponent,StockClearComponent, ReprintStockTakingComponent],
  imports: [
    CommonModule,
    StocksRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    FullModule,
    BsDatepickerModule,
    Ng2SearchPipeModule
  ],
  exports:[CounterStockComponent,StockCheckComponent,StockTakingComponent,GsStockComponent,StockClearComponent],

  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ]
})
export class StocksModule { }
