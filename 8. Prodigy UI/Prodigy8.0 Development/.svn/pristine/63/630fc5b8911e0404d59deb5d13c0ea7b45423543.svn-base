import { StockClearComponent } from './stock-clear/stock-clear.component';
import { GsStockComponent } from './gs-stock/gs-stock.component';
import { StockTakingComponent } from './stock-taking/stock-taking.component';
import { StockCheckComponent } from './stock-check/stock-check.component';
import { CounterStockComponent } from './counter-stock/counter-stock.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ReprintStockTakingComponent } from './reprint-stock-taking/reprint-stock-taking.component';

const routes: Routes = [
  {
    path: 'counter-stock',
    canActivate: [],
    canActivateChild: [],
    component: CounterStockComponent, data: {
      title: 'Counter Stock',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Counter Stock' }]
    },
  },
  {
    path: 'stock-check',
    canActivate: [],
    canActivateChild: [],
    component: StockCheckComponent, data: {
      title: 'Stock Check',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Stock Check' }]
    },
  },
  {
    path: 'stock-taking',
    canActivate: [],
    canActivateChild: [],
    component: StockTakingComponent, data: {
      title: 'Stock Taking',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Stock Taking' }]
    },
  },

  {
    path: 'gs-stock',
    canActivate: [],
    canActivateChild: [],
    component: GsStockComponent, data: {
      title: 'GS Stock',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'GS Stock' }]
    },
  },
  {
    path: 'stock-clear',
    canActivate: [],
    canActivateChild: [],
    component: StockClearComponent, data: {
      title: 'Stock Clear',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Stock Clear' }
      ]
    },
  },
  {
    path: 'reprint-stock-taking',
    canActivate: [],
    canActivateChild: [],
    component: ReprintStockTakingComponent, data: {
      title: 'Reprint Stock Taking',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Reprint Stock Taking' }
      ]
    },
  }

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StocksRoutingModule { }
