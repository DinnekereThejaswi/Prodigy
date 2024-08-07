import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { FieldConfig } from "../../ui-framework.model";
@Component({
  selector: "app-selectlist",
  template: 
`
<mat-form-field class="demo-full-width " [formGroup]="group">
<mat-select required [placeholder]="field.Label" [formControlName]="field.Name" multiple>
<mat-option *ngFor="let item of field.Dataset" [value]="item.Value">{{item.Text}}</mat-option>
</mat-select>
<mat-error *ngIf="group.get(field.Name).touched && group.get(field.Name).invalid">{{field.ValidationMessage}}</mat-error>
</mat-form-field>
`,
  styles: []
})
export class SelectListComponent implements OnInit {
  field: FieldConfig;
  group: FormGroup;
  constructor() {}
  ngOnInit() {}
}
