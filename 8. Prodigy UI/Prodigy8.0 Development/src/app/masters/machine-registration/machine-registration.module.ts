import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MachineRegistrationRoutingModule } from './machine-registration-routing.module';
import { MachineRegistrationComponent } from './machine-registration.component';
import { ManageMachineRegistrationComponent } from './manage-machine-registration/manage-machine-registration.component';

@NgModule({
  declarations: [MachineRegistrationComponent, ManageMachineRegistrationComponent],
  imports: [
    CommonModule,
    MachineRegistrationRoutingModule,
    FormsModule,
    ReactiveFormsModule
  ]
})
export class MachineRegistrationModule { }
