import { AuthGuardService } from './../auth-guard.service';
import { CbwidgetComponent } from './cbwidget.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [{
  path:'',
  canActivate:[AuthGuardService],
  canActivateChild:[AuthGuardService],
  component:CbwidgetComponent
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CbwidgetRoutingModule { }
