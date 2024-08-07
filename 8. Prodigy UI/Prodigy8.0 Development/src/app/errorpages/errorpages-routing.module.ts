import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ErrorpagesComponent } from './errorpages.component'

const routes: Routes = [
  {
    path: '',
    canActivate: [],
    canActivateChild: [],
    component: ErrorpagesComponent, data: {
      title: 'Error Page',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Error Page' }]
    },
  }]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ErrorpagesRoutingModule { }
