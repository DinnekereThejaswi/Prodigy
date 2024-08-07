import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { NewOperatorComponent } from './new-operator/new-operator.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { NewRolesComponent } from './new-roles/new-roles.component';
import { RolePermissionComponent } from './role-permission/role-permission.component';
import { FtpConfigComponent } from './ftp-config/ftp-config.component';
import { ModuleSettingsComponent } from './module-settings/module-settings.component';
import { SubmoduleSettingsComponent } from './submodule-settings/submodule-settings.component';
import { ToleranceComponent } from './tolerance/tolerance.component';
import { CloseOperatorComponent } from './close-operator/close-operator.component';
import { ApplicationPasswordComponent } from './application-password/application-password.component';
import { CalendarComponent } from './calendar/calendar.component';

const routes: Routes = [
  {
    path: 'new-operator',
    canActivate: [],
    canActivateChild: [],
    component: NewOperatorComponent, data: {
      title: 'New Operator',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'New Operator' }]
    }
  },
  {
    path: 'change-password',
    canActivate: [],
    canActivateChild: [],
    component: ChangePasswordComponent, data: {
      title: 'Change Password',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Change Password' }]
    }
  },
  {
    path: 'new-roles',
    canActivate: [],
    canActivateChild: [],
    component: NewRolesComponent, data: {
      title: 'New Roles',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'New Roles' }]
    }
  },
  {
    path: 'role-permission',
    canActivate: [],
    canActivateChild: [],
    component: RolePermissionComponent, data: {
      title: 'Role Permission',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Role Permission' }]
    }
  },
  {
    path: 'ftp-config',
    canActivate: [],
    canActivateChild: [],
    component: FtpConfigComponent, data: {
      title: 'Ftp Config',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Ftp Config' }]
    }
  },
  {
    path: 'submodule-settings',
    canActivate: [],
    canActivateChild: [],
    component: SubmoduleSettingsComponent, data: {
      title: 'Submodule Settings',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Submodule Settings' }]
    }
  },
  {
    path: 'module-settings',
    canActivate: [],
    canActivateChild: [],
    component: ModuleSettingsComponent, data: {
      title: 'Module Settings',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Module Settings' }]
    }
  },
  {
    path: 'tolerance',
    canActivate: [],
    canActivateChild: [],
    component: ToleranceComponent, data: {
      title: 'Tolerance',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Tolerance' }]
    }
  },
  {
    path: 'close-operator',
    canActivate: [],
    canActivateChild: [],
    component: CloseOperatorComponent, data: {
      title: 'Close Operator',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Close Operator' }]
    }
  },
  {
    path: 'application-password',
    canActivate: [],
    canActivateChild: [],
    component: ApplicationPasswordComponent, data: {
      title: 'Application Password',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Application Password' }]
    }
  },
  {
    path: 'calendar',
    canActivate: [],
    canActivateChild: [],
    component: CalendarComponent, data: {
      title: 'Calendar',
      urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'Calendar' }]
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UtilitiesRoutingModule { }
