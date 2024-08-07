import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { NoCommaPipe } from './noComma.pipe';

@NgModule({
  declarations: [NoCommaPipe],
  imports: [CommonModule],
  exports: [NoCommaPipe],
  providers: [NoCommaPipe]

})

export class MainPipe { }