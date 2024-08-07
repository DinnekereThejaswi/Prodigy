import { NgModule } from '@angular/core';
import { AuthGuardService } from './../auth-guard.service';
import { Routes, RouterModule } from '@angular/router';
import { ReportsComponent } from './reports.component';

export const ReportRoutes: Routes = [
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: ReportsComponent, data: {
      title: 'Reports',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Reports' }]
    },
  }
];

@NgModule({
  imports: [RouterModule.forChild(ReportRoutes)],
  exports: [RouterModule]
})
export class ReportsRoutingModule { }
