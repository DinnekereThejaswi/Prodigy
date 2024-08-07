import { NgModule } from '@angular/core';
import { AuthGuardService } from './../../auth-guard.service';
import { Routes, RouterModule } from '@angular/router';
import { AddBarcodeComponent} from './add-barcode.component';

const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
     component: AddBarcodeComponent, data: {
      title: 'Add-Barcode',
      urls: [{title: 'Dashboard', url: '/dashboard'},{title: 'Add-Barcode'}]
    },
  }]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AddBarcodeRoutingModule { }
