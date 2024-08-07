import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { FieldConfig } from "../../ui-framework.model";
@Component({
  selector: "app-select",
  
  template: `
<mat-form-field class="demo-full-width " [formGroup]="group">
<mat-select required [placeholder]="field.Label" [formControlName]="field.Name" >
<mat-option [value]="">{{field.Label}}</mat-option>
<mat-option *ngFor="let item of field.Dataset" [value]="item.id">{{item.text}}</mat-option>
</mat-select>
<mat-error *ngIf="group.get(field.Name).touched && group.get(field.Name).invalid">{{field.ValidationMessage}}</mat-error>
</mat-form-field>
`,
  styles: []
})
export class SelectComponent implements OnInit {
  field: FieldConfig;
  group: FormGroup;
  constructor() {}
  ngOnInit() {}
}
