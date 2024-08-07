import { Component, ViewChild, OnInit, Inject, ViewContainerRef } from "@angular/core";
import { Validators } from "@angular/forms";
import { FieldConfig, MainData } from "./ui-framework.model";
import { DynamicFormComponent } from "./components/dynamic-form/dynamic-form.component";
import { UIFrameworkservice } from './UI-Frameworkservice'
import { Angular5Csv } from 'angular5-csv/dist/Angular5-csv';
import 'jspdf';
import 'jspdf-autotable';
declare let jsPDF;
import { ElementRef } from '@angular/core';

@Component({
  selector: 'app-ui-framework',
  templateUrl: './ui-framework.component.html',
  styleUrls: ['./ui-framework.component.css'],
  providers: [UIFrameworkservice, { provide: 'Window', useValue: window }]
})
export class UIFrameworkComponent implements OnInit {

  // // @ViewChild(DynamicFormComponent) form: DynamicFormComponent;
  // // @ViewChild('someVar') el: ElementRef;

  @ViewChild(DynamicFormComponent, { static: true }) form: DynamicFormComponent;
  @ViewChild('someVar', { static: true }) el: ElementRef;

  keys: string[] = [];
  some_string: string;
  htmlContent: string = "";

  ReportService: Array<any> = [];
  constructor(private view: ViewContainerRef,
    private _UIFrameworkservice: UIFrameworkservice,
    @Inject('Window') private window: Window,) {
    this.ReportService = [
      {
        "ReportName": "Sample Report",
        "ReportURL": "api/genericreport/getreport",
        "AggregateColumns": [
          {
            "id": 0,
            "Reportid": 0,
            "Name": "Maths",
            "Method": "SUM",
            "InsertedOn": "0001-01-01T00:00:00",
            "InsertedBy": 0,
            "UpdatedOn": null,
            "UpdatedBy": null
          },
          {
            "id": 0,
            "Reportid": 0,
            "Name": "Physics",
            "Method": "AVG",
            "InsertedOn": "0001-01-01T00:00:00",
            "InsertedBy": 0,
            "UpdatedOn": null,
            "UpdatedBy": null
          }
        ],
        "RequestPageFields": [
          {
            "id": 0,
            "Queryid": 0,
            "Name": "Name",
            "Label": "Name",
            "ControlTypeid": 0,
            "Control": "input",
            "ControlType": "text",
            "Query": null,
            "ValueMember": "",
            "DisplayMember": "",
            "ValidationMsg": "Name is required",
            "SelectAllRequired": null,
            "DefaultSelectIndex": null,
            "DateNos": null,
            "DefaultValue": null,
            "DefaultExpression": null,
            "ReplacementForSelectAll": null,
            "ReplacementForFilters": null,
            "DatasetAPIURL": null,
            "InsertedOn": "0001-01-01T00:00:00",
            "InsertedBy": 0,
            "UpdatedOn": null,
            "UpdatedBy": null,
            "Dataset": null
          },
          {
            "id": 0,
            "Queryid": 0,
            "Name": "FromDate",
            "Label": "From Date",
            "ControlTypeid": 0,
            "Control": "date",
            "ControlType": null,
            "Query": null,
            "ValueMember": null,
            "DisplayMember": null,
            "ValidationMsg": null,
            "SelectAllRequired": null,
            "DefaultSelectIndex": null,
            "DateNos": null,
            "DefaultValue": null,
            "DefaultExpression": null,
            "ReplacementForSelectAll": null,
            "ReplacementForFilters": null,
            "DatasetAPIURL": null,
            "InsertedOn": "0001-01-01T00:00:00",
            "InsertedBy": 0,
            "UpdatedOn": null,
            "UpdatedBy": null,
            "Dataset": null
          },
          {
            "id": 0,
            "Queryid": 0,
            "Name": "ToDate",
            "Label": "To Date",
            "ControlTypeid": 0,
            "Control": "date",
            "ControlType": null,
            "Query": null,
            "ValueMember": null,
            "DisplayMember": null,
            "ValidationMsg": null,
            "SelectAllRequired": null,
            "DefaultSelectIndex": null,
            "DateNos": null,
            "DefaultValue": null,
            "DefaultExpression": null,
            "ReplacementForSelectAll": null,
            "ReplacementForFilters": null,
            "DatasetAPIURL": null,
            "InsertedOn": "0001-01-01T00:00:00",
            "InsertedBy": 0,
            "UpdatedOn": null,
            "UpdatedBy": null,
            "Dataset": null
          },
          {
            "id": 0,
            "Queryid": 0,
            "Name": "BranchSelector",
            "Label": "Select Branch",
            "ControlTypeid": 0,
            "Control": "select",
            "ControlType": null,
            "Query": null,
            "ValueMember": "id",
            "DisplayMember": "text",
            "ValidationMsg": "Branch is required",
            "SelectAllRequired": null,
            "DefaultSelectIndex": null,
            "DateNos": null,
            "DefaultValue": null,
            "DefaultExpression": null,
            "ReplacementForSelectAll": null,
            "ReplacementForFilters": null,
            "DatasetAPIURL": null,
            "InsertedOn": "0001-01-01T00:00:00",
            "InsertedBy": 0,
            "UpdatedOn": null,
            "UpdatedBy": null,
            "Dataset": [
              {
                "id": 1,
                "text": "RajajiNagar"
              },
              {
                "id": 2,
                "text": "JayaNagar"
              },
              {
                "id": 3,
                "text": "Koramangala"
              }
            ]
          },
          {
            "id": 0,
            "Queryid": 0,
            "Name": "ReportType",
            "Label": "Report Type",
            "ControlTypeid": 0,
            "Control": "selectlist",
            "ControlType": null,
            "Query": null,
            "ValueMember": null,
            "DisplayMember": null,
            "ValidationMsg": null,
            "SelectAllRequired": null,
            "DefaultSelectIndex": 0,
            "DateNos": null,
            "DefaultValue": null,
            "DefaultExpression": null,
            "ReplacementForSelectAll": null,
            "ReplacementForFilters": null,
            "DatasetAPIURL": null,
            "InsertedOn": "0001-01-01T00:00:00",
            "InsertedBy": 0,
            "UpdatedOn": null,
            "UpdatedBy": null,
            "Dataset": [
              {
                "Text": "Sales",
                "Value": "0"
              },
              {
                "Text": "Order",
                "Value": "1"
              }
            ]
          },
          // {
          //   "id": 0,
          //   "Queryid": 0,
          //   "Name": "ReportType",
          //   "Label": "Report Type",
          //   "ControlTypeid": 0,
          //   "Control": "radiobutton",
          //   "ControlType": null,
          //   "Query": null,
          //   "ValueMember": null,
          //   "DisplayMember": null,
          //   "ValidationMsg": null,
          //   "SelectAllRequired": null,
          //   "DefaultSelectIndex": 0,
          //   "DateNos": null,
          //   "DefaultValue": null,
          //   "DefaultExpression": null,
          //   "ReplacementForSelectAll": null,
          //   "ReplacementForFilters": null,
          //   "DatasetAPIURL": null,
          //   "InsertedOn": "0001-01-01T00:00:00",
          //   "InsertedBy": 0,
          //   "UpdatedOn": null,
          //   "UpdatedBy": null,
          //   "Dataset": [
          //     {
          //       "Text": "Summary",
          //       "Value": "0"
          //     },
          //     {
          //       "Text": "Detailed",
          //       "Value": "1"
          //     }
          //   ]
          // },
          {
            "id": 0,
            "Queryid": 0,
            "Name": "Save",
            "Label": "Save",
            "ControlTypeid": 0,
            "Control": "button",
            "ControlType": null,
            "Query": null,
            "ValueMember": null,
            "DisplayMember": null,
            "ValidationMsg": null,
            "SelectAllRequired": null,
            "DefaultSelectIndex": null,
            "DateNos": null,
            "DefaultValue": null,
            "DefaultExpression": null,
            "ReplacementForSelectAll": null,
            "ReplacementForFilters": null,
            "DatasetAPIURL": null,
            "InsertedOn": "0001-01-01T00:00:00",
            "InsertedBy": 0,
            "UpdatedOn": null,
            "UpdatedBy": null,
            "Dataset": null
          }
        ]
      }
    ]
  }

  ngAfterViewInit() {
    //setTimeout(() => this.htmlContent = (this.view.element.nativeElement as HTMLElement).innerHTML);
  }

  regConfig: any = [];
  ngOnInit() {
    this.htmlContent = '';
    for (let results of this.ReportService) {
      for (var main of results["RequestPageFields"]) {
        this.regConfig.push(main);
      }
    }
  }

  arrayObj: any = [];

  submit(value: any) {
    this.arrayObj = value;
    // console.log(JSON.stringify(this.arrayObj));
    //setTimeout(() => this.htmlContent = (this.view.element.nativeElement as HTMLElement).innerHTML);
  }


  HideShow: boolean = false;
  flag: boolean;
  GenerateHtml() {
    this.flag = true;
    this.htmlContent = (this.view.element.nativeElement as HTMLElement).innerHTML;
  }

  preview() {
    this.HideShow = true;
  }

  clear() {
    this.htmlContent = '';
    this.flag = false;
  }
}
