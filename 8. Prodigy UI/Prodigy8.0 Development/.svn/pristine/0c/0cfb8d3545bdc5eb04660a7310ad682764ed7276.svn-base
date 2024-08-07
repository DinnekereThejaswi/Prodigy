import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuardService } from '../auth-guard.service';
import { OnlineMarkingBarcodeInventoryComponent } from './online-marking-barcode-inventory.component';


export const OnlineMarkingBarcodeInventoryRoutes: Routes = [
  {
    path: '',
    //canDeactivate: [appConfirmationGuard],
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
    component: OnlineMarkingBarcodeInventoryComponent, data: {
      title: 'Mark Inventory for Online',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Mark Inventory for Online' }]
    },
  }
];

@NgModule({
  imports: [RouterModule.forChild(OnlineMarkingBarcodeInventoryRoutes)],
  exports: [RouterModule]
})

export class OnlineMarkingBarcodeInventoryRoutingModule {

}
