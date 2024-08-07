import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { MasterService } from './../../core/common/master.service';
import { BranchreceiptsService } from '../branchreceipts.service';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
declare var $: any;

@Component({
  selector: 'app-autobarcode-receipts',
  templateUrl: './autobarcode-receipts.component.html',
  styleUrls: ['./autobarcode-receipts.component.css']
})
export class AutobarcodeReceiptsComponent implements OnInit {
  Enable :boolean =true;
  disable :boolean =false;
  routerUrl: string = "";
  AutobarcodereceiptHeaderForm : FormGroup;
  @ViewChild("Pwd", { static: true }) Pwd: ElementRef;
  radioItems: Array<string>;
  model = { option: 'Ornaments Receipts' };
  datePickerConfig: Partial<BsDatepickerConfig>;
  BarcodeDetails: any = [];
  StoneDetails: any = [];
  BarcodesDetailsTab: boolean = true;
  CollapseStoneDetailsTab: boolean = true;
  ReceiptFrom: any = [];
  today = new Date();
  applicationDate: any;
  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  //password proetected
  PasswordValid: string;
  public Index: number = -1;
  constructor(private _masterService: MasterService,
    private _branchreceiptsService: BranchreceiptsService,
    private _appConfigService: AppConfigService,
    private _router: Router, private fb: FormBuilder) {
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        maxDate: this.today,
        // minDate: this.today,
        dateInputFormat: 'YYYY-MM-DD',

      });
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
    this.PasswordValid = this._appConfigService.RateEditCode.AutoBarcodeReceipt;
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }


  AutoBarcodeReceiptPost: any = {
    CompanyCode: null,
    BranchCode: null,
    IssueBranchCode: null,
    IssueNo: null,
    IssueDate: null,
    TotalLineCount: null,
    TotalQty: null,
    TotalGrossWt: null,
    TotalStoneWt: null,
    TotalNetWt: null,
    TotalDcts: null,
    TotalAmount: null,
    ReceiptDetails: [],
    ScannedBarcodes: []
  }

  ngOnInit() {
    this.Enable=true;
    this.routerUrl = this._router.url;
    this.ValidatePermission();
    this.getApplicationDate();
    this.getReceiptFrom();
    this.AutobarcodereceiptHeaderForm = this.fb.group({
      frmCtrl_ReceiptFrom: null,
      frmCtrl_IssueNo: null,
      frmCtrl_ReceiptDate: null,
      frmCtrl_ReceivedBy: '',

    });
  }

  ValidatePermission() {
    $("#PermissonModal").modal('show');
    this.Pwd.nativeElement.value = "";
  }

  passWord(arg) {
    if (arg == "") {
      swal("Warning!", "Please Enter the Password", "warning");
      $('#PermissonModal').modal('show');
    }
    else {
      this.permissonModel.CompanyCode = this.ccode;
      this.permissonModel.BranchCode = this.bcode;
      this.permissonModel.PermissionID = this.PasswordValid;
      this.permissonModel.PermissionData = btoa(arg);
      this._branchreceiptsService.postelevatedpermission(this.permissonModel).subscribe(
        response => {
          $('#PermissonModal').modal('hide');
          this.Index = -1;
        }
      )
    }
  }

  BackTodashBoard() {
    this._router.navigate(['/dashboard']);
  }
  permissonModel: any = {
    CompanyCode: null,
    BranchCode: null,
    PermissionID: null,
    PermissionData: null
  }

  ToggleStoneDetails() {
    this.CollapseStoneDetailsTab = !this.CollapseStoneDetailsTab;
  }

  ToggleBarcodes() {
    this.BarcodesDetailsTab = !this.BarcodesDetailsTab;
  }

  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
      }
    )
  }

  getReceiptFrom() {
    this._branchreceiptsService.GetAutoBarcodeReceiptFrom().subscribe(
      response => {
        this.ReceiptFrom = response;
      }
    )
  }

  Go() {
    if (this.AutoBarcodeReceiptPost.IssueBranchCode == null) {
      swal("Warning!", 'Please select the Receipt From', "warning");
    }
    else if (this.AutoBarcodeReceiptPost.IssueNo == null || this.AutoBarcodeReceiptPost.IssueNo == "") {
      swal("Warning!", 'Please enter the Issue No', "warning");
    }
    else {
      this._branchreceiptsService.GetAutoBarcodeReceiptIssueDetail(this.AutoBarcodeReceiptPost).subscribe(
        response => {
          this.AutoBarcodeReceiptPost = response;
          this.AutobarcodereceiptHeaderForm.get('frmCtrl_ReceiptFrom').disable();
          this.AutobarcodereceiptHeaderForm.get('frmCtrl_IssueNo').disable();
          this.AutobarcodereceiptHeaderForm.get('frmCtrl_ReceivedBy').disable();
          this.Enable =false;
          this.disable =true;
          this.BarcodeDetails = this.AutoBarcodeReceiptPost.ReceiptDetails;
          this.AutoBarcodeReceiptPost.ScannedBarcodes = this.AutoBarcodeReceiptPost.ReceiptDetails;
          this.getStoneDetails();
        }
      )
    }
  }

  getStoneDetails() {
    if (this.BarcodeDetails.length == 0) {
      this.StoneDetails = [];
    }
    else {
      for (let i = 0; i < this.BarcodeDetails.length; i++) {
        if (this.BarcodeDetails[i].BarcodeReceiptStoneDetails.length > 0) {
          for (let j = 0; j < this.BarcodeDetails[i].BarcodeReceiptStoneDetails.length; j++) {
            this.StoneDetails.push(this.BarcodeDetails[i].BarcodeReceiptStoneDetails[j]);
          }
        }
      }
    }
  }

  cancel() {
    this.BarcodeDetails =[];
    this.StoneDetails=[];
    this.Enable =true;
    this.disable=false;
    this.AutobarcodereceiptHeaderForm.get('frmCtrl_ReceiptFrom').enable();
          this.AutobarcodereceiptHeaderForm.get('frmCtrl_IssueNo').enable();
          this.AutobarcodereceiptHeaderForm.get('frmCtrl_ReceivedBy').enable();
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
      () => {
        this._router.navigate(['/branchreceipts/autobarcode-receipts']);
      }
    )
  }

  outputAutoBarcodeReceipt: any = [];
  PrintTypeBasedOnConfig: any;
  NTreceiptPrint: any = [];

  SaveAutoBarcode() {
    var ans = confirm("Do you want to Save??");
    if (ans) {
      this._branchreceiptsService.post(this.AutoBarcodeReceiptPost).subscribe(
        response => {
          this.outputAutoBarcodeReceipt = response;
          swal("Saved!", this.outputAutoBarcodeReceipt.Message, "success");
          this.BarcodeDetails = [];
          this.StoneDetails = [];
          this.AutoBarcodeReceiptPost = {
            CompanyCode: null,
            BranchCode: null,
            IssueBranchCode: null,
            IssueNo: null,
            IssueDate: null,
            TotalLineCount: null,
            TotalQty: null,
            TotalGrossWt: null,
            TotalStoneWt: null,
            TotalNetWt: null,
            TotalDcts: null,
            TotalAmount: null,
            ReceiptDetails: [],
            ScannedBarcodes: []
          }
          this._branchreceiptsService.getBarcodeReceiptPrint(this.outputAutoBarcodeReceipt.DocumentNo).subscribe(
            response => {
              this.PrintTypeBasedOnConfig = response;
              if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
                $('#NTReceiptsTab').modal('show');
                this.NTreceiptPrint = atob(this.PrintTypeBasedOnConfig.Data);
                $('#DisplayntreceiptData').html(this.NTreceiptPrint);
              }
              else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
                $('#NTReceiptsTab').modal('show');
                this.NTreceiptPrint = this.PrintTypeBasedOnConfig.Data;
              }
            }
          )
        }
      )
    }
  }

  printNtreceipt() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.NTreceiptPrint);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printntReceiptData, popupWin;
      printntReceiptData = document.getElementById('DisplayntreceiptData').innerHTML;
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
    ${printntReceiptData}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }


}