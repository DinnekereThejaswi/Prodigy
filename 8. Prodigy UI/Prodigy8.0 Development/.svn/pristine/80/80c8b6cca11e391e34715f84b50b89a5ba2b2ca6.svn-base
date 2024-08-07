import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { FieldConfig } from "../../ui-framework.model";
@Component({
  selector: "app-date",
  template: `
<mat-form-field class="demo-full-width" [formGroup]="group">
<input matInput [matDatepicker]="picker" [formControlName]="field.Name" readonly [placeholder]="field.Label">
<mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
<mat-datepicker #picker></mat-datepicker>
<mat-hint></mat-hint>
<mat-error *ngIf="group.get(field.Name).touched && group.get(field.Name).invalid">{{field.ValidationMessage}}</mat-error>
</mat-form-field>
`,
  styles: []
})
export class DateComponent implements OnInit {
  field: FieldConfig;
  group: FormGroup;
  constructor() {}
  ngOnInit() { 
  }
}
