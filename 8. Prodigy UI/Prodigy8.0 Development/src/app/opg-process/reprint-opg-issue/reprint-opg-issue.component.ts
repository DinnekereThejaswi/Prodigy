import { Component, OnInit } from '@angular/core';
import { MasterService } from '../../core/common/master.service';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { OpgprocessService } from '../opg-process.service';
import swal from 'sweetalert';
import { formatDate } from '@angular/common';
import { FormBuilder, FormGroup } from '@angular/forms';
declare var $: any;

@Component({
  selector: 'app-reprint-opg-issue',
  templateUrl: './reprint-opg-issue.component.html',
  styleUrls: ['./reprint-opg-issue.component.css']
})
export class ReprintOpgIssueComponent implements OnInit {

  datePickerConfig: Partial<BsDatepickerConfig>;
  today = new Date();
  // type: any = [];
  IRListData: any = [];
  IssueNos: any = [];
  ReprintForm: FormGroup;

  constructor(private _masterService: MasterService,
    private _opgprocessService: OpgprocessService,
    private formBuilder: FormBuilder) {
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        maxDate: this.today,
        // minDate: this.today,
        dateInputFormat: 'YYYY-MM-DD',

      });
  }

  ngOnInit() {
    this.getApplicationDate();
    this.ReprintForm = this.formBuilder.group({
      Type: null,
      IssueNo: null,
      applicationDate: null
    });
  }


  reprintHeaders = {
    Type: null,
    IssueNo: null,
    applicationDate: null
  }

  byDate(applicationDate) {
    this.reprintHeaders.applicationDate = null,
      this.reprintHeaders.applicationDate = applicationDate;
    this.applicationDate = formatDate(applicationDate, 'yyyy-MM-dd', 'en-US');
    this.getIssueType();
  }


  receiptType: string = "";

  getIssueType() {
    this._opgprocessService.getIssueNos(this.reprintHeaders.Type, this.applicationDate).subscribe(
      response => {
        this.IssueNos = response;
        if (this.IssueNos.length == 0) {
          this.reprintHeaders.IssueNo = null;
        }
      }
    )
  }

  applicationDate: any;
  disAppDate: any;
  test: any = [];
  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
        this._opgprocessService.loadIRListData("I").subscribe(
          Response => {
            this.IRListData = Response;
          }
        )
      }
    )
  }

  type: any = [
    {
      "Code": "IL",
      "Name": "Melting Issue",
    },
    {
      "Code": "MG Issue to CPC",
      "Name": "MG Issue to CPC",
    }
  ]


  PrintTypeBasedOnConfig: any;
  PrintData: any = [];

  reprint(form) {
    if (form.value.Type == null) {
      swal("Warning!", 'Please select the type', "warning");
    }
    else if (form.value.IssueNo == null) {
      swal("Warning!", 'Please enter the issue no', "warning");
    }
    else {
      if (form.value.Type == "IL") {
        this._opgprocessService.getMeltingIssuePrint(form.value.IssueNo).subscribe(
          response => {
            this.PrintTypeBasedOnConfig = response;
            if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
              $('#PrintDataTab').modal('show');
              this.PrintData = atob(this.PrintTypeBasedOnConfig.Data);
              $('#DisplayData').html(this.PrintData);
            }
          }
        )
      }
      else {
        this._opgprocessService.getMeltingGoldPrint(form.value.IssueNo).subscribe(
          response => {
            this.PrintTypeBasedOnConfig = response;
            if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
              $('#PrintDataTab').modal('show');
              this.PrintData = atob(this.PrintTypeBasedOnConfig.Data);
              $('#DisplayData').html(this.PrintData);
            }
          }
        )
      }
    }
  }

  print() {
    if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printIssueData, popupWin;
      printIssueData = document.getElementById('DisplayData').innerHTML;
      popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
      popupWin.document.open();
      popupWin.document.write(`
      <html>
        <head>
          <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css" integrity="sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm" crossorigin="anonymous">
          <title>Print tab</title>
          <style>
         .htmlPrint{display:none;}

          @media print {
            .table-bordered
        {
            border-style: solid;
            border: 3px solid rgb(0, 0, 0);
        }
        .margin{
          margin-top:-24px;
        }
        .mb{
          vertical-align:middle;position:absolute;
        }
        tr.spaceUnder>td {
          padding-bottom: 40px !important;
        }
        .top {
          margin-top:10px;
          border-bottom: 3px solid rgb(0, 0, 0) !important;
          line-height: 1.6;
        }
        .modal-content{
          font-family: "Times New Roman", Times, serif;

        }
       .padding-top {
         padding-top:20px;
       }
        .watermark{
          -webkit-transform: rotate(331deg);
          -moz-transform: rotate(331deg);
          -o-transform: rotate(331deg);
          transform: rotate(331deg);
          font-size: 15em;
          color: rgba(255, 5, 5, 0.37);
          position: absolute;
          text-transform:uppercase;
          padding-left: 10%;
          margin-top:-10px;
        }
        .right{
          text-align:left;
        }
        .px-2 {
          padding-left: 10px !important;
        }
        thead > tr
        {
            border-style: solid;
            border: 3px solid rgb(0, 0, 0);
        }
        table tr td.classname{
          border-right: 3px solid rgb(0, 0, 0) !important;
        }
        table tr td.bottom{
          border-bottom: 3px solid rgb(0, 0, 0) !important;
        }
        table tr th.classname{
          border-right: 3px solid rgb(0, 0, 0) !important;
        }
        .divborder {
          border-right: 3px solid rgb(0, 0, 0) !important;
          line-height: 1.6;
        }
        .lastdivborder{
          line-height: 1.6;
        }
    .border{
      border-right: 3px solid rgb(0, 0, 0) !important;
      border-bottom: 3px solid rgb(0, 0, 0) !important;
    }

    .tdborder{
      border-right: 3px solid rgb(0, 0, 0) !important;
    }
        .invoice {
          margin-top: 250px;
      }
        .card{
          border-style: solid;
          border-width: 5px;
          border: 3px solid rgb(0, 0, 0);
      }
        .printMe {
          display: none !important;
        }
      }
      body{
            font-size: 15px;
            line-height: 18px;
      }
 </style>
        </head>
    <body onload="window.print();window.close()">
    ${printIssueData}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }
}