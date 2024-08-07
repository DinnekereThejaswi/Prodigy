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
  selector: 'app-receiptscancel',
  templateUrl: './receiptscancel.component.html',
  styleUrls: ['./receiptscancel.component.css']
})
export class ReceiptscancelComponent implements OnInit {
  @ViewChild('remarks', { static: true }) remarks: ElementRef;
  ReceiptscancelForm: FormGroup;
  datePickerConfig: Partial<BsDatepickerConfig>;
  today = new Date();

  @ViewChild("Pwd", { static: true }) Pwd: ElementRef;
  PasswordValid: string;
  public Index: number = -1;
  ccode: string;
  bcode: string;
  password: string;
  EnableDate: boolean = true;
  EnableJson: boolean = false;
  reprintReceiptHeaders = {
    ReceiptType: "",
    Receiptdate: null,
    ReceiptNo: null,
    remarks: ""

  }
  constructor(private formBuilder: FormBuilder, private _router: Router,
    private _branchreceiptsService: BranchreceiptsService, private appConfigService: AppConfigService
    , private toastr: ToastrService, private fb: FormBuilder) {
    this.password = this.appConfigService.Pwd;
    this.EnableJson = this.appConfigService.EnableJson;
    this.PasswordValid = this.appConfigService.RateEditCode.ReceiptCancelPermission;
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
    this.reprintReceiptHeaders.ReceiptType = "RB";
    this.getApplicationDate();
    this.ReceiptscancelForm = this.fb.group({
      frmCtrl_applicationDate: null,
      frmCtrl_ReceiptNo: null,
      frmCtrl_Remarks: ''
    });
  }

  applicationDate: any;

  getApplicationDate() {
    this._branchreceiptsService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
        this.reprintReceiptHeaders.Receiptdate = this.applicationDate;
        this.getIssueNumberToCancel(this.reprintReceiptHeaders.Receiptdate);
      }
    )
  }
  byDate(applcationDate) {
    this.reprintReceiptHeaders.Receiptdate = null;
    this.reprintReceiptHeaders.Receiptdate = formatDate(applcationDate, 'yyyy-MM-dd', 'en-US');;
    this.getIssueNumberToCancel(formatDate(applcationDate, 'yyyy-MM-dd', 'en-US'));
  }
  ValidatePermisiion() {
    $("#PermissonModals").modal('show');
    this.Pwd.nativeElement.value = "";
  }
  permissonModels: any = {
    CompanyCode: null,
    BranchCode: null,
    PermissionID: null,
    PermissionData: null
  }
  passWord(arg) {
    if (arg == "") {
      swal("Warning!", 'Please Enter the Password', "warning");
      $('#PermissonModals').modal('show');
    }
    else {
      this.permissonModels.CompanyCode = this.ccode;
      this.permissonModels.BranchCode = this.bcode;
      this.permissonModels.PermissionID = this.PasswordValid;
      this.permissonModels.PermissionData = btoa(arg);
      this._branchreceiptsService.postelevatedpermission(this.permissonModels).subscribe(
        response => {
          this.EnableDate = false;
          $('#PermissonModals').modal('hide');
          this.Index = -1;
        },
        (err) => {
          if (err.status === 401) {
            this.EnableDate = true;
          }
        }
      )
    }
  }
  EditDate() {
    this._branchreceiptsService.getPermission().subscribe(
      response => {
        this.EnableDate = false;
      },
      error => {
        swal("Warning!", "Unauthorized", "warning");
        this.EnableDate = true;
      }
    )
  }
  IssueNumbers: any = [];

  getIssueNumberToCancel(date) {
    this._branchreceiptsService.getReceiptNumbersByDateAndType(date).subscribe(
      response => {
        this.IssueNumbers = response;
      }
    )
  }

  PrintTypeBasedOnConfig: any;
  PrintDetails: any = [];
  previewPrint() {

    if (this.reprintReceiptHeaders.ReceiptNo == null || this.reprintReceiptHeaders.ReceiptNo == "") {
      swal("Warning!", "Please select Receipt No", "warning")
    }

    else {
      this._branchreceiptsService.getReceiptPrintSummary(this.reprintReceiptHeaders.ReceiptNo).subscribe(
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
  result: any = [];
  Cancel() {
    if (this.reprintReceiptHeaders.remarks == null || this.reprintReceiptHeaders.remarks == "") {
      swal("Warning!", "Please Enter Remarks", "warning");
    }
    else {
      var ans = confirm("Do you want to Cancel Branch Receipts: " + this.reprintReceiptHeaders.ReceiptNo + "??");
      if (ans) {
        var ans = confirm("Sure you want to Cancel Branch Receipts: " + this.reprintReceiptHeaders.ReceiptNo + "??");
        if (ans) {
          this._branchreceiptsService.cancelReceiptPost(this.reprintReceiptHeaders.ReceiptNo, this.reprintReceiptHeaders.remarks).subscribe(
            response => {
              this.result = response;
              swal("Success!", "Receipt No " + this.reprintReceiptHeaders.ReceiptNo + " Cancelled", "success")
              this.IssueNumbers = [];
              this.ReceiptscancelForm.reset();
              this.getApplicationDate();
            }
          )
        }
      }
    }
    this.remarks.nativeElement.value = '';

  }

}