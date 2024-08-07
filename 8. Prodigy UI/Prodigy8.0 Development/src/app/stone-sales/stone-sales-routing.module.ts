import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { appConfirmationGuard } from '../appconfirmation-guard';
import { AuthGuardService } from './../auth-guard.service';
import { StoneEstimationComponent } from './stone-estimation/stone-estimation.component';
const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: StoneEstimationComponent, data: {
      title: 'Stone Estimation',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Stone Estimation' }]
    },
  }
  // {
  //   path: 'counter-stock',
  //   canActivate: [],
  //   canActivateChild: [],
  //   component: CounterStockComponent, data: {
  //     title: 'Counter Stock',
  //     urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Counter Stock' }]
  //   },
  // },
  // {
  //   path: 'counter-stock',
  //   canActivate: [],
  //   canActivateChild: [],
  //   component: CounterStockComponent, data: {
  //     title: 'Counter Stock',
  //     urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Counter Stock' }]
  //   },
  // },
  // {
  //   path: 'counter-stock',
  //   canActivate: [],
  //   canActivateChild: [],
  //   component: CounterStockComponent, data: {
  //     title: 'Counter Stock',
  //     urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Counter Stock' }]
  //   },
  // },
  // {
  //   path: 'counter-stock',
  //   canActivate: [],
  //   canActivateChild: [],
  //   component: CounterStockComponent, data: {
  //     title: 'Counter Stock',
  //     urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Counter Stock' }]
  //   },
  // },
  // {
  //   path: 'counter-stock',
  //   canActivate: [],
  //   canActivateChild: [],
  //   component: CounterStockComponent, data: {
  //     title: 'Counter Stock',
  //     urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Counter Stock' }]
  //   },
  // },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StoneSalesRoutingModule { }
