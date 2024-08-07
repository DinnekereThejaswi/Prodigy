import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { MasterService } from '../../core/common/master.service';
import { formatDate } from '@angular/common';
import { PurchaseService } from '../purchase.service';
import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';
declare var jquery: any;
declare var $: any;
import * as CryptoJS from 'crypto-js';

@Component({
  selector: 'app-reprint-purchasebill',
  templateUrl: './reprint-purchasebill.component.html',
  styleUrls: ['./reprint-purchasebill.component.css']
})
export class ReprintPurchasebillComponent implements OnInit {
  @ViewChild("Pwd", { static: true }) Pwd: ElementRef;
  EnableDate: boolean = true;
  ReprintForm :FormGroup;
  @Input()
  BillNo: any;
  ccode: string;
  bcode: string;
  ordDate = '';
  isActive = false;
  isCancelled
  password: string;
  PasswordValid: string;
  datePickerConfig: Partial<BsDatepickerConfig>;
  today = new Date();
  public Index: number = -1;
  constructor(private formBuilder: FormBuilder, private _router: Router,
    private _masterService: MasterService, private _purchaseService: PurchaseService,  private appConfigService: AppConfigService,) {
    //this.radioItems = ['Order No', 'Receipt No', 'Closed Orders'];
    this.ordDate = formatDate(this.today, 'MM-DD-YYYY', 'en-US', '+0530');
    this.PasswordValid = this.appConfigService.RateEditCode.SalesBillPermission;
    this.password = this.appConfigService.Pwd;
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        maxDate: this.today,
        dateInputFormat: 'YYYY-MM-DD'
      });
      this.getCB();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  isReprint: boolean = true;
  billNo: string = "";
  applicationDate: any;
  disAppDate: any;

  isCheckedCHBX: boolean = false;
  isChecked(e) {
    this.isCheckedCHBX = e.target.checked;
    this.reprintHeaders.selectedOption = null;
    this.getApplicationDate();
    this._purchaseService.AllPurchaseBill(this.applicationDate, this.isCheckedCHBX).subscribe(
      response => {
        this.BillList = response;
      }
    )
  }


  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
        this.disAppDate = formatDate(appDate["applcationDate"], 'dd/MM/yyyy', 'en_GB');
        this.getAllPurchaseBill(this.applicationDate);
      }
    )
  }

  reprintHeaders = {
    selectedOption: null,
    applicationDate: null
  }

  BillList: any = [];

  byDate(applicationDate) {
    this.reprintHeaders.applicationDate = null,
    this.reprintHeaders.applicationDate = applicationDate;
    this.applicationDate = formatDate(applicationDate, 'yyyy-MM-dd', 'en-US');
    this.getAllPurchaseBill(this.applicationDate)
  }
  permissonModels: any = {
    CompanyCode: null,
    BranchCode: null,
    PermissionID: null,
    PermissionData: null
  }
  ValidatePermisiion() {
    $("#PermissonModals").modal('show');
    this.Pwd.nativeElement.value = "";
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
      this._purchaseService.postelevatedpermission(this.permissonModels).subscribe(
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

  ngOnInit() {
    this.ReprintForm = this.formBuilder.group({
      selectedOption: [null],
      applicationDates: null,
      date: null
    });
    this.getApplicationDate();
    if (this._router.url === "/purchase/reprint-purchasebill") {
      this.isReprint = true;
    }
    if (this._router.url == "/purchase/purchase-billing") {
      this.isReprint = false;
      if (this.BillNo.billNo != null && this.BillNo.billNo != "") {
        // this._purchaseService.PrintPurchaseBill(this.BillNo.billNo).subscribe(
        //   response => {
        //     this.PrintDetails = response;
        //     this.PrintDetails = atob(this.PrintDetails);
        //     this.PurBillNum = this.BillNo.billNo;
        //     this._purchaseService.PrintPurchaseBill(this.BillNo.billNo).subscribe(
        //       response => {
        //         $('#ReprintPurchaseBillingModal').modal('show');
        //         $('#DisplayData').html(this.PrintDetails);
        //       });
        //   }
        // )
        this._purchaseService.PrintPurchaseBill(this.BillNo.billNo).subscribe(
          response => {
            this.PrintTypeBasedOnConfig = response;
            if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
              this.PrintDetails = atob(this.PrintTypeBasedOnConfig.Data);
              $('#ReprintPurchaseBillingModal').modal('show');
              $('#DisplayData').html(this.PrintDetails);
            }
            else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
              this.PrintDetails = this.PrintTypeBasedOnConfig.Data;
              $('#ReprintPurchaseBillingModal').modal('show');
            }
          }
        )
      }
    }
  }

  PrintDetails: any = [];

  data: any;

  PurBillNum: number;

  PrintTypeBasedOnConfig: any;

  onPrint(arg) {
    if (this.reprintHeaders.selectedOption == null) {
      swal("Warning!", 'Please enter the bill no', "warning");
      $('#ReprintPurchaseBillingModal').modal('hide');
    }
    else {
      this.PurBillNum = arg;
      this._purchaseService.PrintPurchaseBill(arg).subscribe(
        response => {
          this.PrintTypeBasedOnConfig = response;
          if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
            this.PrintDetails = atob(this.PrintTypeBasedOnConfig.Data);
            $('#ReprintPurchaseBillingModal').modal('show');
            $('#DisplayData').html(this.PrintDetails);
          }
          else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
            this.PrintDetails = this.PrintTypeBasedOnConfig.Data;
            $('#ReprintPurchaseBillingModal').modal('show');
          }
        }
      )
    }
  }
  getAllPurchaseBill(Appdate) {
    this._purchaseService.AllPurchaseBill(Appdate, this.isCheckedCHBX).subscribe(
      response => {
        this.BillList = response;
      }
    )
  }
  ngOnChanges() {
    this._purchaseService.PrintPurchaseBill(this.BillNo.billNo).subscribe(
      response => {
        this.PrintDetails = response;
        this.PrintDetails = atob(this.PrintDetails);
        this.PurBillNum = this.BillNo.billNo;
        $('#ReprintPurchaseBillingModal').modal('show');
        $('#DisplayData').html(this.PrintDetails);
      }
    )
  }

  // print() {
  //   let printContents, popupWin;
  //   printContents = document.getElementById('DisplayData').innerHTML;
  //   popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
  //   popupWin.document.open();
  //   popupWin.document.write(printContents);
  //   popupWin.document.close();
  // }



  // for printing the form
  print() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.PrintDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printContents, popupWin;
      printContents = document.getElementById('DisplayData').innerHTML;
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
    ${printContents}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }



  // Added for Plain Text printing related to Order Receipt

  purBillPlainTextDetails: any = [];

  closePurBillTab() {
    //Commented since dotmatrix print is not required

    // if (this._router.url === "/purchase/purchase-billing") {
    //   this.getPurBillPlainText(this.PurBillNum);
    // }
    // else {
    //   $('#PurBillPlainTextTab').modal('hide');
    // }
  }


  getPurBillPlainText(arg) {
    this._purchaseService.PrintPurBillDotMatrix(arg).subscribe(
      response => {
        this.purBillPlainTextDetails = response;
        $('#PurBillPlainTextTab').modal('show');
      }
    )
  }

  printPurBillPlainText() {
    this._masterService.printPlainText(this.purBillPlainTextDetails);
  }

  // Ends Here
}