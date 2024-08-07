import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {AuthGuardService} from './../../auth-guard.service';
import { GsGroupingComponent } from './gs-grouping.component';
const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
     component: GsGroupingComponent, data: {
      title: 'GS-Grouping', 
      urls: [{title: 'Dashboard', url: 'javascript:void(0)'},{title: 'GS-Grouping'}]
    },
  }
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GsGroupingRoutingModule { }
