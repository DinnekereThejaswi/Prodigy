import { AuthGuardService } from './../auth-guard.service';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RedirectComponent } from './redirect.component';

const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
     component: RedirectComponent, data: { 
      title: 'Redirect', 
      urls: [{title: 'Dashboard',url: '/dashboard'},{title: 'Redirect'}]
    },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RedirectRoutingModule { }
