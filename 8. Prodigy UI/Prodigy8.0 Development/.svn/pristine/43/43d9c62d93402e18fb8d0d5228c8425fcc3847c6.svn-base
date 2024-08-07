import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { formatDate } from '@angular/common';
import { AppConfigService } from '../../AppConfigService';
import swal from 'sweetalert';
import * as qz from 'qz-tray';
declare var qz: any;
declare var jquery: any;
declare var $: any;
import { ToastrService } from 'ngx-toastr';
import * as CryptoJS from 'crypto-js';
import { BranchreceiptsService } from '../branchreceipts.service';

@Component({
  selector: 'app-reprint-receipts',
  templateUrl: './reprint-receipts.component.html',
  styleUrls: ['./reprint-receipts.component.css']
})
export class ReprintReceiptsComponent implements OnInit {

  ReprintReceiptForm: FormGroup;
  radioItems: Array<string>;

  datePickerConfig: Partial<BsDatepickerConfig>;
  today = new Date();
  model = { option: 'Branch Receipts' };
  EnableJson: boolean = false;
  reprintReceiptHeaders = {
    selectedOption: "Branch Issue",
    date: null,
    selectedReceiptNo: null,
    isDetaileChecked: null

  }
  @ViewChild("Pwd", { static: true }) Pwd: ElementRef;
  PasswordValid: string;
  public Index: number = -1;
  ccode: string;
  bcode: string;
  password: string;
  EnableDate: boolean = true;
  ///
  constructor(private formBuilder: FormBuilder, private _router: Router,
    private _branchreceiptsService: BranchreceiptsService, private appConfigService: AppConfigService
    , private toastr: ToastrService, private fb: FormBuilder) {
    this.password = this.appConfigService.Pwd;
    this.EnableJson = this.appConfigService.EnableJson;
    // this.radioItems = ['Branch Receipts', 'OPG Receipts', 'SR Receipts', 'RP Receipts', 'OPG Segregation'];
    this.radioItems = ['Branch Receipts'];
    this.PasswordValid = this.appConfigService.RateEditCode.SalesBillPermission;
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        maxDate: this.today,
        // minDate: this.today,
        dateInputFormat: 'YYYY-MM-DD',

      });
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }


  ngOnInit() {
    this.reprintReceiptHeaders.isDetaileChecked = false;
    this.getApplicationDate();
    this.ReprintReceiptForm = this.fb.group({
      FrmCtrl_selectedOption: [null],
      frmCtrl_applicationDate: null,
      frmCtrl_ReceiptNo: null
    });
  }

  applicationDate: any;

  getApplicationDate() {
    this._branchreceiptsService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
        this.reprintReceiptHeaders.date = this.applicationDate;
        if (this.model.option === 'Branch Receipts') {
          this.reprintReceiptHeaders.date = formatDate(this.applicationDate, 'yyyy-MM-dd', 'en-US');;
          this.getReceiptNo(this.reprintReceiptHeaders.date);
        }
      }
    )
  }
  byDate(applcationDate) {
    this.reprintReceiptHeaders.date = null;
    this.reprintReceiptHeaders.date = formatDate(applcationDate, 'yyyy-MM-dd', 'en-US');;
    this.getReceiptNo(this.reprintReceiptHeaders.date);
  }
  IssueNumbers: any = [];
  getReceiptNo(arg) {
    this._branchreceiptsService.getReceiptNoByDate(arg).subscribe(
      response => {
        this.IssueNumbers = response;
      }
    )
  }

  Change(arg) {
    this.PrintDetails = [];
    this.reprintReceiptHeaders.isDetaileChecked = arg.target.checked;
  }
  PrintTypeBasedOnConfig: any;
  PrintDetails: any = [];
  onPrint() {
    if (this.reprintReceiptHeaders.selectedReceiptNo == null || this.reprintReceiptHeaders.selectedReceiptNo == "") {
      swal("Warning!", "Please Select Receipt No", "warning")
    }
    else {
      if (this.reprintReceiptHeaders.isDetaileChecked === true) {
        this._branchreceiptsService.getReceiptPrintDetailed(this.reprintReceiptHeaders.selectedReceiptNo).subscribe(
          response => {
            this.PrintTypeBasedOnConfig = response;
            if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
              $('#DetailedTab').modal('show');
              this.PrintDetails = atob(this.PrintTypeBasedOnConfig.Data);
              $('#DetailedTabData').html(this.PrintDetails);
            }
            else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
              $('#DetailedTabData').modal('show');
              this.PrintDetails = this.PrintTypeBasedOnConfig.Data;
            }
          }
        )
      }
      else if (this.reprintReceiptHeaders.isDetaileChecked === false) {
        this._branchreceiptsService.getReceiptPrintSummary(this.reprintReceiptHeaders.selectedReceiptNo).subscribe(
          response => {
            this.PrintTypeBasedOnConfig = response;
            if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
              $('#summaryTab').modal('show');
              this.PrintDetails = atob(this.PrintTypeBasedOnConfig.Data);
              $('#summaryTabData').html(this.PrintDetails);
            }
            else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
              $('#summaryTabData').modal('show');
              this.PrintDetails = this.PrintTypeBasedOnConfig.Data;
            }
          }
        )

      }

    }

  }
  printSummary() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {

    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printsummaryData, popupWin;
      printsummaryData = document.getElementById('summaryTabData').innerHTML;
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
    ${printsummaryData}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }
  printDetailedata() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {

    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printDetailedData, popupWin;
      printDetailedData = document.getElementById('DetailedTabData').innerHTML;
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
    ${printDetailedData}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }

  cancel() {
    this.ReprintReceiptForm.reset();
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
      () => {
        this._router.navigate(['/branchreceipts/reprint-receipts']);
      }
    )
  }
}
