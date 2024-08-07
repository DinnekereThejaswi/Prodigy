import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { FieldConfig } from "../../ui-framework.model";
@Component({
  selector: "app-radiobutton",
  template: `
<div class="demo-full-width " [formGroup]="group">
<label class="radio-label-padding">{{field.Label}}:</label>
<mat-radio-group [formControlName]="field.Name">
<mat-radio-button *ngFor="let item of field.Dataset" [value]="item.Value">{{item.Text}}</mat-radio-button>
</mat-radio-group>
</div>
`,
  styles: []
})
export class RadiobuttonComponent implements OnInit {
  field: FieldConfig;
  group: FormGroup;
  constructor() {}
  ngOnInit() {}
}
