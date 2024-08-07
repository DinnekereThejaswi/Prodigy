import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { BranchissueService } from '../branchissue.service';
import { FormGroup, FormBuilder, Validators, FormGroupName } from '@angular/forms';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';
import { formatDate } from '@angular/common';
import { Router } from '@angular/router';
declare var $: any;
@Component({
  selector: 'app-cancel-issue',
  templateUrl: './cancel-issue.component.html',
  styleUrls: ['./cancel-issue.component.css']
})
export class CancelIssueComponent implements OnInit {
  @ViewChild("Pwd", { static: true }) Pwd: ElementRef;
  CanceleIssueprintForm: FormGroup;
  datePickerConfig: Partial<BsDatepickerConfig>;
  today = new Date();
  PasswordValid: string;
  public Index: number = -1;
  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  isReprint: boolean = false;
  EnableDate: boolean = true;
  applicationDate: any;
  disAppDate: any;
  CancelIssueHeaders = {
    companyCode: null,
    branchCode: null,
    IssueDate: null,
    IssueNo: null,
    remarks: null,
  }
  constructor(private _branchissueService: BranchissueService, private fb: FormBuilder,
    private _router: Router,
    private _appConfigService: AppConfigService) {
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        maxDate: this.today,
        dateInputFormat: 'YYYY-MM-DD'
      });
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.PasswordValid = this._appConfigService.RateEditCode.ReceiptCancelPermission;
    this.getCB();;
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this.getApplicationDate();
    this.CancelIssueHeaders.companyCode = this.ccode;
    this.CancelIssueHeaders.branchCode = this.bcode;
    this.CanceleIssueprintForm = this.fb.group({
      frmCtrl_issueDate: [null],
      frmCtrl_IssueNo: null,
      frmCtrl_Remarks: ''
    });
  }

  byDateChange(applicationDate) {
    this.CancelIssueHeaders.IssueDate = null,
      this.CancelIssueHeaders.IssueDate = formatDate(applicationDate, 'yyyy-MM-dd', 'en-US');;
    this.applicationDate = applicationDate;
    this.getIssueNumberToCancel(formatDate(applicationDate, 'yyyy-MM-dd', 'en-US'))
  }
  getApplicationDate() {
    this._branchissueService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
        this.getIssueNumberToCancel(this.applicationDate)
      }
    )
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
      this._branchissueService.postelevatedpermission(this.permissonModels).subscribe(
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
    this._branchissueService.getPermission().subscribe(
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
  filterdIssueNumbers: any = [];
  getIssueNumberToCancel(arg) {
    this._branchissueService.getIsssueNumbersByDate(arg).subscribe(
      response => {
        this.IssueNumbers = response;
        var FilterdData=Object.values(this.IssueNumbers.reduce((acc,cur)=>Object.assign(acc,{[cur.IssueNo]:cur}),{}))
        this.filterdIssueNumbers=FilterdData;
      }
    )
  }
  PrintTypeBasedOnConfig: any;
  PrintDetails: any = [];
  Preview() {
    if (this.CancelIssueHeaders.IssueNo == "" || this.CancelIssueHeaders.IssueNo == null) {
      swal("Warning!", "Please select  Issue No", "warning");

    }
    else {
      this._branchissueService.getPrintPreviewData(this.CancelIssueHeaders.IssueNo, this.CancelIssueHeaders.remarks).subscribe(
        response => {
          this.PrintTypeBasedOnConfig = response;
          if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
            $('#PreviewModal').modal('show');
            this.PrintDetails = atob(this.PrintTypeBasedOnConfig.Data);
            $('#DisplaysrIssueData').html(this.PrintDetails);
          }
          else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
            $('#SRIssueTab').modal('show');
            this.PrintDetails = this.PrintTypeBasedOnConfig.Data;
          }
        }
      )
    }
  }
  CancelResult: any = []
  cancelIssue(form) {
    if (form.value.frmCtrl_IssueNo == null || form.value.frmCtrl_IssueNo == "") {
      swal("Warning!", "Please select Issue No", "warning");
    }
    else if (form.value.frmCtrl_Remarks == null || form.value.frmCtrl_Remarks == "") {
      swal("Warning!", "Please enter remarks", "warning");
    }
    else {
      var ans = confirm("Do you want to Cancel??");
      if (ans) {
        this._branchissueService.cancelIssueByIssueNo(this.CancelIssueHeaders.IssueNo, this.CancelIssueHeaders.remarks).subscribe(
          response => {
            this.CancelResult = response;
            if (this.CancelResult == null) {
              swal("Success!", "Issue No" + this.CancelIssueHeaders.IssueNo + "Cancelled", "success")
            }
            this.CanceleIssueprintForm.reset();
            this.getApplicationDate();
          }
        )
      }
    }
  }


  print() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {

    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printData, popupWin;
      printData = document.getElementById('DisplayData').innerHTML;
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
    ${printData}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }
}
