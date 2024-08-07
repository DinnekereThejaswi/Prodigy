import { AuthGuardService } from '../auth-guard.service';
import { Routes } from '@angular/router';
import { UIFrameworkComponent } from './ui-framework.component';


export const UiFrameworkRoutes: Routes = [
  {
    path: '', 
    component: UIFrameworkComponent,
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
     data: {
      title: '',
      urls: [{title: 'Dashboard',url: '/dashboard'},{title: 'UI-Framework'}]
    },
  }
];
