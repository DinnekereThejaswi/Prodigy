import { CUSTOM_ELEMENTS_SCHEMA, LOCALE_ID, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FullModule } from './../layouts/full/full.module';
import { UtilitiesRoutingModule } from './utilities-routing.module';
import { NewOperatorComponent } from './new-operator/new-operator.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { NewRolesComponent } from './new-roles/new-roles.component';
import { RolePermissionComponent } from './role-permission/role-permission.component';
import { FtpConfigComponent } from './ftp-config/ftp-config.component';
import { ApplicationPasswordComponent } from './application-password/application-password.component';
import { SubmoduleSettingsComponent } from './submodule-settings/submodule-settings.component';
import { ToleranceComponent } from './tolerance/tolerance.component';
import { CloseOperatorComponent } from './close-operator/close-operator.component';
import { ModuleSettingsComponent } from './module-settings/module-settings.component';
import { SelectDropDownModule } from 'ngx-select-dropdown';
import { FilterPipeModule } from 'ngx-filter-pipe';
import { CalendarComponent } from './calendar/calendar.component';
import { YearCalendarModule } from '@iomechs/angular-year-calendar'

@NgModule({
  declarations: [NewOperatorComponent, ChangePasswordComponent, NewRolesComponent, RolePermissionComponent, FtpConfigComponent, ApplicationPasswordComponent, SubmoduleSettingsComponent, ToleranceComponent, CloseOperatorComponent, ModuleSettingsComponent, CalendarComponent],
  imports: [
    CommonModule, FullModule,
    UtilitiesRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    SelectDropDownModule,
    FilterPipeModule,
    YearCalendarModule
  ],
  providers: [],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class UtilitiesModule { }
