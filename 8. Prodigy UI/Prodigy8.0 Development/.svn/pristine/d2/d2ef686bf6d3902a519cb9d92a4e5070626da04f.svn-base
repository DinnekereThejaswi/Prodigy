import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { appConfirmationGuard } from '../appconfirmation-guard';
import { AuthGuardService } from '../auth-guard.service';
import { ChitUpdateComponent } from './chit-update/chit-update.component';
const routes: Routes = [{
  path: 'chit-update',
  canActivate: [AuthGuardService],
  canActivateChild: [AuthGuardService],
  component: ChitUpdateComponent, data: {
    title: 'Chit Update',
    urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Chit Update' }]
  },
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OthersRoutingModule { }
