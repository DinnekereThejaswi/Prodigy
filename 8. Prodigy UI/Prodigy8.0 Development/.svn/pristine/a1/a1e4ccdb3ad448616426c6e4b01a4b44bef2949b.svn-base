import { Component, OnInit } from '@angular/core';
import { MasterService } from '../../core/common/master.service';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { OpgprocessService } from '../opg-process.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { formatDate } from '@angular/common';

import swal from 'sweetalert';
declare var $: any;

@Component({
  selector: 'app-reprint-opg-receipt',
  templateUrl: './reprint-opg-receipt.component.html',
  styleUrls: ['./reprint-opg-receipt.component.css']
})
export class ReprintOpgReceiptComponent implements OnInit {

  datePickerConfig: Partial<BsDatepickerConfig>;
  today = new Date();

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
      ReceiptNo: null,
      applicationDate: null
    });
  }

  receiptType: string = "";
  ReprintForm: FormGroup;

  reprintHeaders = {
    Type: null,
    ReceiptNo: null,
    applicationDate: null
  }

  getReceiptType() {
    this._opgprocessService.getReceiptNo(this.reprintHeaders.Type, this.applicationDate).subscribe(
      response => {
        this.ReceiptNos = response;
        if (this.ReceiptNos.length == 0) {
          this.reprintHeaders.ReceiptNo = null;
        }
      }
    )
  }

  applicationDate: any;
  disAppDate: any;
  test: any = [];
  // type: any = [];
  IRListData: any = [];
  ReceiptNos: any = [];

  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
        this._opgprocessService.loadIRListData("R").subscribe(
          Response => {
            this.IRListData = Response;
          }
        )
      }
    )
  }

  byDate(applicationDate) {
    this.reprintHeaders.applicationDate = null,
      this.reprintHeaders.applicationDate = applicationDate;
    this.applicationDate = formatDate(applicationDate, 'yyyy-MM-dd', 'en-US');
    this.getReceiptType();
  }

  PrintTypeBasedOnConfig: any;
  PrintData: any = [];

  reprint(form) {
    if (form.value.Type == null) {
      swal("Warning!", 'Please select the type', "warning");
    }
    else if (form.value.ReceiptNo == null) {
      swal("Warning!", 'Please select the receipt no', "warning");
    }
    else {
      this._opgprocessService.getMeltingReceiptPrint(form.value.ReceiptNo).subscribe(
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

  type: any = [
    {
      "Code": "RL",
      "Name": "Melting Receipts",
    }
  ]

  print() {
    if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printReceiptData, popupWin;
      printReceiptData = document.getElementById('DisplayData').innerHTML;
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
    ${printReceiptData}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }


}