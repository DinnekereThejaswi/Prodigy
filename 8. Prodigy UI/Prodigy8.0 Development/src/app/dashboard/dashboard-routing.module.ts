import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuardService } from './../auth-guard.service';
import {DashboardComponent} from './dashboard.component';


const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
     component: DashboardComponent, data: { 
      title: 'Dashboard', 
      urls: [{title: 'Dashboard',url: '/dashboard'},{title: 'Dashboard'}]
    },
  }
];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule { }
