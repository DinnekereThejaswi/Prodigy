import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

import { UIFrameworkComponent } from '../UiFramework/ui-framework.component';

import {UiFrameworkRoutes} from '../UiFramework/ui-framework.routing'

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxPaginationModule} from 'ngx-pagination';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { HttpModule } from '@angular/http';
import { AuthGuardService } from '../auth-guard.service';
import { AuthInterceptor } from '../auth.interceptor'; 
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';

import { DynamicFormComponent} from './components/dynamic-form/dynamic-form.component';
import { DynamicFieldDirective} from './components/dynamic-field/dynamic-field.directive';

import { InputComponent } from "./components/input/input.component";
import { ButtonComponent } from "./components/button/button.component";
import { SelectComponent } from "./components/select/select.component";
import { DateComponent } from "./components/date/date.component";
import { RadiobuttonComponent } from "./components/radiobutton/radiobutton.component";
import { CheckboxComponent } from "./components/checkbox/checkbox.component";
import { SelectListComponent } from "./components/selectlist/selectlist.component";
import {
  MatButtonModule,
  MatFormFieldModule,
  MatInputModule,
  MatRippleModule,
  MatSelectModule,
  MatDatepickerModule,
  MatRadioModule,
  MatCheckboxModule
} from '@angular/material';

@NgModule({
  declarations: [UIFrameworkComponent,
    DynamicFormComponent,
    DynamicFieldDirective,
    InputComponent,
    ButtonComponent,
    SelectComponent,
    DateComponent,
    RadiobuttonComponent,
    CheckboxComponent,
    SelectListComponent],
  imports: [
    CommonModule,
    FormsModule,
    NgxPaginationModule, 
    BsDatepickerModule.forRoot(),
    RouterModule.forChild(UiFrameworkRoutes), 
    HttpModule,
    ReactiveFormsModule,
    HttpClientModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatRippleModule,
    MatSelectModule,
    MatDatepickerModule,
    MatRadioModule,
    MatCheckboxModule,
  ],
  entryComponents: [
    InputComponent,
    ButtonComponent,
    SelectComponent,
    DateComponent,
    RadiobuttonComponent,
    CheckboxComponent,
    SelectListComponent
  ],
  providers:[AuthGuardService,{
    provide:HTTP_INTERCEPTORS,
    useClass:AuthInterceptor,
    multi:true
}]
})
export class UIFrameworkModule { }
