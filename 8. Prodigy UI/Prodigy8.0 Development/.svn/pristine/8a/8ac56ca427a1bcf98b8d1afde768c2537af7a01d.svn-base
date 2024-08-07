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
  selector: 'app-reprint-branch-issue',
  templateUrl: './reprint-branch-issue.component.html',
  styleUrls: ['./reprint-branch-issue.component.css']
})
export class ReprintBranchIssueComponent implements OnInit {
  ///permission
  ReprintForm: FormGroup;
  @ViewChild("Pwd", { static: true }) Pwd: ElementRef;
  PasswordValid: string;
  public Index: number = -1;
  ccode: string;
  bcode: string;
  password: string;
  EnableDate: boolean = true;
  ///


  datePickerConfig: Partial<BsDatepickerConfig>;
  EnableItemDetails: boolean = true;
  EnableFittedStoneDetails: boolean = true;
  EnableStoneDmddtls: boolean = true;
  today = new Date();
  applicationDate: any;

  reprintform: FormGroup;
  isChecked: boolean = false;
  EnableJson: boolean = false;
  isReprint: boolean = false;
  HeaderFormModel = {
    BinBo: null,
    issueTo: null,
    issueDate: null,
    AgeValidation: false

  }

  //radio items
  radioItemPrintType: Array<string>;
  radioItems: Array<string>;
  model = {
    option: 'Branch Issue'
  };
  radioBtnValue: string = '';
  reprintHeaders = {
    selectedOption: "Branch Issue",
    date: null,
    selectedIssueNo: null
  }
  constructor(private _router: Router, private _branchissueService: BranchissueService, private fb: FormBuilder, private _appConfigService: AppConfigService) {
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        maxDate: this.today,
        dateInputFormat: 'YYYY-MM-DD'
      });
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
    this.radioItemPrintType = ['HTML', 'DotMatrix'];
    this.radioItems = ['Branch Issue', 'OPG Issue', 'SR Issue', 'RP Isuue'];
    this.reprintHeaders.selectedOption = this.model.option;
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }


  ngOnInit() {
    this.getApplicationDate();
    this.reprintform = this.fb.group({
      FrmCtrl_selectedOption: [null],
      frmCtrl_applicationDate: null,
      frmCtrl_IssueNo: null
    });
    this.radioBtnValue = 'Branch Issue';
    this.reprintHeaders.selectedOption = this.radioBtnValue;
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
  byDate(arg) {
    this.reprintHeaders.date = null,
      this.reprintHeaders.selectedOption = null;
    this.reprintHeaders.date = formatDate(arg, 'yyyy-MM-dd', 'en-US');;
  }
  FieldsChange(arg) {
    this.isChecked = false;

  }
  disAppDate: any;
  test: any = [];
  getApplicationDate() {
    this._branchissueService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
        this.disAppDate = formatDate(appDate["applcationDate"], 'dd/MM/yyyy', 'en_GB');
        this.reprintHeaders.date = this.applicationDate;
        if (this.model.option === 'Branch Issue') {
          this.reprintHeaders.selectedOption === "Branch Issue";
          this.applicationDate = formatDate(this.reprintHeaders.date, 'yyyy-MM-dd', 'en-US');;
          this.getIsuueNo(this.applicationDate);
        }
      }
    )
  }



  Changed(arg) {
    this.reprintHeaders.selectedOption = null;
    if (arg === 'Branch Issue') {
      this.reprintHeaders.selectedOption = arg;
      this.reprintHeaders.selectedIssueNo = null;
      this.model.option = arg;
      this.applicationDate = formatDate(this.reprintHeaders.date, 'yyyy-MM-dd', 'en-US');;
      this.getIsuueNo(this.applicationDate);
    }
    else if (arg === 'SR Issue') {
      this.model.option = arg;
      this.reprintHeaders.selectedOption = arg;
      this.reprintHeaders.selectedIssueNo = null;
      this.model.option = arg;
      this.IssueNumbers = [];
      this.PrintDetails = [];
      this.getSRissueNo();
    }
    else if (arg === 'OPG Issue') {
      this.reprintHeaders.selectedOption = arg;
      this.reprintHeaders.selectedIssueNo = null;
      this.model.option = arg;
      this.IssueNumbers = [];
      this.PrintDetails = [];
      this.getOPGIssueNo();
    }
    else if (arg === 'RP Isuue') {
      this.reprintHeaders.selectedOption = arg;
      this.reprintHeaders.selectedIssueNo = null;
      this.model.option = arg;
      this.IssueNumbers = [];
    }
  }
  IssueNumbers: any = [];
  getIsuueNo(arg) {
    this._branchissueService.getAllissueNoByDate(arg).subscribe(
      response => {
        this.IssueNumbers = response;
      }
    )
  }
  getOPGIssueNo() {
    this._branchissueService.getAllOPGissueNoByDate(this.applicationDate).subscribe(
      response => {
        this.IssueNumbers = response;
      }
    )
  }
  getSRissueNo() {
    this._branchissueService.getAllSRissueNoByDate(this.applicationDate).subscribe(
      response => {
        this.IssueNumbers = response;
      }
    )
  }

  PrintTypeBasedOnConfig: any;
  PrintDetails: any = [];
  getBranchIssuePrint(arg) {
    this._branchissueService.getBarcodeIssuePrint(arg).subscribe(
      response => {
        this.PrintTypeBasedOnConfig = response;
        if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
          $('#TAGIssueTab').modal('show');
          this.PrintDetails = atob(this.PrintTypeBasedOnConfig.Data);
          $('#DisplaytagIssueData').html(this.PrintDetails);
        }
        else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
          $('#TAGIssueTab').modal('show');
          this.PrintDetails = this.PrintTypeBasedOnConfig.Data;
        }
      }
    )
  }

  getOPGIssuePrint(arg) {
    this._branchissueService.getOPGIssuePrint(arg).subscribe(
      response => {
        this.PrintTypeBasedOnConfig = response;
        if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
          $('#OPGIssueTab').modal('show');
          this.PrintDetails = atob(this.PrintTypeBasedOnConfig.Data);
          $('#DisplaysrIssueData').html(this.PrintDetails);
        }
        else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
          $('#OPGIssueTab').modal('show');
          this.PrintDetails = this.PrintTypeBasedOnConfig.Data;
        }
      }
    )
  }
  getSRIssuePrint(arg) {
    this._branchissueService.getSRIssuePrint(arg).subscribe(
      response => {
        this.PrintTypeBasedOnConfig = response;
        if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
          $('#SRIssueTab').modal('show');
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

  onPrint() {
    if (this.reprintHeaders.selectedOption === 'Branch Issue' || this.reprintHeaders.selectedOption == null) {
      this.getBranchIssuePrint(this.reprintHeaders.selectedIssueNo);
    }
    else if (this.reprintHeaders.selectedOption === 'OPG Issue') {
      this.getOPGIssuePrint(this.reprintHeaders.selectedIssueNo);
    }
    else if (this.reprintHeaders.selectedOption === 'SR Issue') {
      this.getSRIssuePrint(this.reprintHeaders.selectedIssueNo);
    }
    else if (this.reprintHeaders.selectedOption === 'RP Isuue') {
      this.model.option = this.reprintHeaders.selectedOption;
    }
  }

  printOPGIssue() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {

    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printopgIssueData, popupWin;
      printopgIssueData = document.getElementById('DisplayopgIssueData').innerHTML;
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
    ${printopgIssueData}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }
  printTagIssue() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {

    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printtagIssueData, popupWin;
      printtagIssueData = document.getElementById('DisplaytagIssueData').innerHTML;
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
    ${printtagIssueData}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }
  printSRIssue() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {

    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printsrIssueData, popupWin;
      printsrIssueData = document.getElementById('DisplaysrIssueData').innerHTML;
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
    ${printsrIssueData}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }
  cancel() {
    this.reprintform.reset();
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
      () => {
        this._router.navigate(['/branchissue/reprint-branch-issue']);
      }
    )
  }
}


