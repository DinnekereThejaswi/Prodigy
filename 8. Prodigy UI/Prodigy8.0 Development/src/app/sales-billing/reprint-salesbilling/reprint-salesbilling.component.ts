import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { Component, OnInit, Input, OnChanges, ViewChild, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { MasterService } from '../../core/common/master.service';
import { SalesBillingService } from '../sales-billing.service';
import { formatDate } from '@angular/common';
import { PurchaseService } from '../../purchase/purchase.service';
import { SalesreturnService } from '../../salesreturn/salesreturn.service';
import { AppConfigService } from '../../AppConfigService';
import { ToastrService } from 'ngx-toastr';
import * as CryptoJS from 'crypto-js';
import { OrdersService } from '../../orders/orders.service';
import swal from 'sweetalert';
declare var jquery: any;
declare var $: any;

@Component({
  selector: 'app-reprint-salesbilling',
  templateUrl: './reprint-salesbilling.component.html',
  styleUrls: ['./reprint-salesbilling.component.css']
})

export class ReprintSalesbillingComponent implements OnInit, OnChanges {
  isActive = false;
  @ViewChild("Pwd", { static: true }) Pwd: ElementRef;
  @Input() AttachedInfoBill: any;
  EnableDate: boolean = true;
  PasswordValid: string;
  ccode: string;
  ReprintForm: FormGroup;
  password: string;
  bcode: string;
  datePickerConfig: Partial<BsDatepickerConfig>;
  width = 1;
  height = 25;
  ReprintType: string = "Original";
  Orderheading: string = "Order Form";

  today = new Date();
  constructor(private formBuilder: FormBuilder, private _router: Router,
    private _masterService: MasterService, private _salesBillingService: SalesBillingService,
    private _purchaseService: PurchaseService, private _SalesreturnService: SalesreturnService,
    private appConfigService: AppConfigService, private toastr: ToastrService,
    private _orderService: OrdersService) {
    this.password = this.appConfigService.Pwd;
    this.PasswordValid = this.appConfigService.RateEditCode.SalesBillPermission;
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        maxDate: this.today,
        dateInputFormat: 'YYYY-MM-DD'
      });
    this.getCB();
  }

  reprintHeaders = {
    selectedOption: null,
    applicationDate: null
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  isReprint: boolean = false;
  isChecked: boolean = false;
  applicationDate: any;
  disAppDate: any;
  public Index: number = -1;

  ReprintbillNo: any;
  ngOnInit() {
    if (this._router.url === "/sales-billing/reprintsalesbilling") {
      this.isReprint = true;
      this.getApplicationDate();
    }
    if (this._router.url == "/sales-billing") {
      this.isReprint = false;
    }
    this.ReprintForm = this.formBuilder.group({
      selectedOption: [null],
      applicationDate: null,
      date: null
    });
  }

  ngOnChanges() {
    if (this.AttachedInfoBill.BillNo != null && this.AttachedInfoBill.BillNo != "") {
      if (this.AttachedInfoBill.CreditNote != null) {
        $('#OrderTab').modal('show');
        this.getOrderDetails(this.AttachedInfoBill.CreditNote.OrderNo)
      }
      else {
        this._salesBillingService.getBillNoDetails(this.AttachedInfoBill.BillNo).subscribe(
          response => {
            this.PrintTypeBasedOnConfig = response;
            if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
              this.PrintDetails = atob(this.PrintTypeBasedOnConfig.Data);
              this._salesBillingService.getAttachedInfoBill(this.AttachedInfoBill.BillNo).subscribe(
                response => {
                  this.AttachedInfoBill = response;
                  $('#ReprintSalesBillingModal').modal('show');
                  $('#DisplayData').html(this.PrintDetails);
                }
              );
            }
            else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
              this.PrintDetails = this.PrintTypeBasedOnConfig.Data;
              this._salesBillingService.getAttachedInfoBill(this.AttachedInfoBill.BillNo).subscribe(
                response => {
                  this.AttachedInfoBill = response;
                  $('#ReprintSalesBillingModal').modal('show');
                }
              );
            }
          }
        )
      }
    }
  }

  closeOrderTab() {
    swal("Submitted!", "Bill No: " + this.AttachedInfoBill.BillNo + " generated successfully!", "success");
    this._salesBillingService.getBillNoDetails(this.AttachedInfoBill.BillNo).subscribe(
      response => {
        this.PrintDetails = response;
        if (this.PrintDetails.PrintType == "HTML" && this.PrintDetails.Data != "") {
          this.PrintDetails = atob(this.PrintDetails.Data);
          this._salesBillingService.getAttachedInfoBill(this.AttachedInfoBill.BillNo).subscribe(
            response => {
              this.AttachedInfoBill = response;
              $('#ReprintSalesBillingModal').modal('show');
              $('#DisplayData').html(this.PrintDetails);
            }
          );
        }
        else if (this.PrintDetails.PrintType == "RAW" && this.PrintDetails.Data != "") {
          this.PrintDetails = this.PrintDetails.Data;
          this._salesBillingService.getAttachedInfoBill(this.AttachedInfoBill.BillNo).subscribe(
            response => {
              this.AttachedInfoBill = response;
              $('#ReprintSalesBillingModal').modal('show');
            }
          );
        }
      }
    )
  }



  printOrder() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.orderDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printOrderContents, popupWin;
      printOrderContents = document.getElementById('DisplayOrderData').innerHTML;
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
    ${printOrderContents}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }

  onSubmit() {
    if (this.ReprintForm.valid) {
      //this.ReprintForm.reset();
    }
  }

  byDate(applicationDate) {
    this.reprintHeaders.applicationDate = null,
      this.reprintHeaders.applicationDate = applicationDate;
    this.applicationDate = formatDate(applicationDate, 'yyyy-MM-dd', 'en-US');
    this.getBillNo();
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
      this._orderService.postelevatedpermission(this.permissonModels).subscribe(
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

  permissonModels: any = {
    CompanyCode: null,
    BranchCode: null,
    PermissionID: null,
    PermissionData: null
  }


  printSRBill() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.PrintSRDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printContents, popupWin;
      printContents = document.getElementById('SRDisplayData').innerHTML;
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
        table tr td.borderLeft{
          border-left: 3px solid rgb(0, 0, 0) !important;
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


  printPurchaseBill() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.PrintPurchaseDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printContents, popupWin;
      printContents = document.getElementById('DisplayPurchaseData').innerHTML;
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
          table tr td.borderLeft{
            border-left: 3px solid rgb(0, 0, 0) !important;
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

  isCheckedCHBX: boolean = false;
  isCheckedCancelledBills(e) {
    this.billNo = "";
    this.isChecked = e.target.checked;
    this.getBillNo();
  }



  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = formatDate(appDate["applcationDate"] ,'dd/MM/yyyy', 'en_GB');
        // this.disAppDate = formatDate(appDate["applcationDate"], 'dd/MM/yyyy', 'en_GB');
        this.reprintHeaders.applicationDate = this.applicationDate;
        this.getBillNo();
      }
    )
  }

  PrintDetails: any = [];
  PrintPurchaseDetails: any = [];
  PrintOGDetails: any = [];
  PrintSRDetails: any = [];
  billNo: string = "";
  data: any;
  PrintTypeBasedOnConfig: any;
  //AttachedInfoBill: any = [];

  onPrint() {
    if (this.billNo == null || this.billNo == "") {
      swal("Warning!", 'Please select the bill no', "warning");
      $('#ReprintSalesBillingModal').modal('hide');
    }
    else {
      this.i = 0;
      this.j = 0;
      this._salesBillingService.getBillNoDetails(this.billNo).subscribe(
        response => {
          this.PrintTypeBasedOnConfig = response;
          if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
            this.PrintDetails = atob(this.PrintTypeBasedOnConfig.Data);
            this._salesBillingService.getAttachedInfoBill(this.billNo).subscribe(
              response => {
                this.AttachedInfoBill = response;
                $('#ReprintSalesBillingModal').modal('show');
                $('#DisplayData').html(this.PrintDetails);
              }
            );
          }
          else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
            this.PrintDetails = this.PrintTypeBasedOnConfig.Data;
            this._salesBillingService.getAttachedInfoBill(this.billNo).subscribe(
              response => {
                this.AttachedInfoBill = response;
                $('#ReprintSalesBillingModal').modal('show');
              }
            );
          }
        }
      )
    }
  }

  i: number = 0;
  j: number = 0;

  closeSalesBilling() {
    if (this.PrintTypeBasedOnConfig.ContinueNextPrint == true) {
      if (this.AttachedInfoBill.Purchase.length > 0) {
        this.PrintPurchaseDetails = [];
        this._purchaseService.PrintPurchaseBill(this.AttachedInfoBill.Purchase[this.i]).subscribe(
          response => {
            this.PrintPurchaseDetails = response;
            if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
              this.PrintPurchaseDetails = atob(this.PrintPurchaseDetails.Data);
              this._salesBillingService.getAttachedInfoBill(this.AttachedInfoBill.BillNo).subscribe(
                response => {
                  this.AttachedInfoBill = response;
                  $('#ReprintPurchaseBillingModal').modal('show');
                  $('#DisplayPurchaseData').html(this.PrintPurchaseDetails);
                }
              );
            }
            else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
              this.PrintPurchaseDetails = this.PrintPurchaseDetails.Data;
              this._salesBillingService.getAttachedInfoBill(this.AttachedInfoBill.BillNo).subscribe(
                response => {
                  this.AttachedInfoBill = response;
                  $('#ReprintPurchaseBillingModal').modal('show');
                }
              );
            }
          }
        )
      }
      else {
        if (this.AttachedInfoBill.SalesReturn.length > 0) {
          this.PrintSRDetails = [];
          this._SalesreturnService.getSRPrintbyBillNo(this.AttachedInfoBill.SalesReturn[this.j]).subscribe(
            response => {
              this.PrintSRDetails = response;
              if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
                this.PrintSRDetails = atob(this.PrintSRDetails.Data);
                this._salesBillingService.getAttachedInfoBill(this.AttachedInfoBill.BillNo).subscribe(
                  response => {
                    this.AttachedInfoBill = response;
                    $('#ReprintSRBillModal').modal('show');
                    $('#SRDisplayData').html(this.PrintSRDetails);
                  }
                );
              }
              else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
                this.PrintSRDetails = this.PrintSRDetails.Data;
                this._salesBillingService.getAttachedInfoBill(this.AttachedInfoBill.BillNo).subscribe(
                  response => {
                    this.AttachedInfoBill = response;
                    $('#ReprintSRBillModal').modal('show');
                  }
                );
              }
            }
          )
        }
        else {
          $('#ReprintSRBillModal').modal('hide');
        }
      }
    }
    else {
      $('#ReprintSalesBillingModal').modal('hide');
    }
  }

  getBillNo() {
    this._salesBillingService.getBillNo(this.applicationDate, this.isChecked).subscribe(
      response => {
        this.ReprintbillNo = response;
      }
    )
  }


  reoadPurchaseBillPrint() {
    $('#ReprintPurchaseBillingModal').modal('hide');
    this.PrintPurchaseDetails = [];
    ++this.i;
    if (this.AttachedInfoBill.Purchase[this.i] != 0) {
      this._purchaseService.PrintPurchaseBill(this.AttachedInfoBill.Purchase[this.i]).subscribe(
        response => {
          this.PrintOGDetails = response;
          if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
            this.PrintOGDetails = atob(this.PrintOGDetails.Data);
            this._salesBillingService.getAttachedInfoBill(this.AttachedInfoBill.BillNo).subscribe(
              response => {
                this.AttachedInfoBill = response;
                $('#ReprintOGBillModal').modal('show');
                $('#OGDisplayData').html(this.PrintOGDetails);
              }
            );
          }
          else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
            this.PrintOGDetails = this.PrintOGDetails.Data;
            this._salesBillingService.getAttachedInfoBill(this.AttachedInfoBill.BillNo).subscribe(
              response => {
                this.AttachedInfoBill = response;
                $('#ReprintOGBillModal').modal('show');
              }
            );
          }
        }
      )
    }
    else {
      this.PrintSRDetails = [];
      if (this.AttachedInfoBill.SalesReturn.length > 0) {
        this._SalesreturnService.getSRPrintbyBillNo(this.AttachedInfoBill.SalesReturn[this.j]).subscribe(
          response => {
            this.PrintSRDetails = response;
            if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
              this.PrintSRDetails = atob(this.PrintSRDetails.Data);
              this._salesBillingService.getAttachedInfoBill(this.AttachedInfoBill.BillNo).subscribe(
                response => {
                  this.AttachedInfoBill = response;
                  $('#ReprintSRBillModal').modal('show');
                  $('#SRDisplayData').html(this.PrintSRDetails);
                }
              );
            }
            else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
              this.PrintSRDetails = this.PrintSRDetails.Data;
              this._salesBillingService.getAttachedInfoBill(this.AttachedInfoBill.BillNo).subscribe(
                response => {
                  this.AttachedInfoBill = response;
                  $('#ReprintSRBillModal').modal('show');
                }
              );
            }
          }
        )
      }
    }
  }

  reoaSRBill() {
    this.PrintSRDetails = [];
    ++this.j;
    if (this.AttachedInfoBill.SalesReturn[this.j] != 0) {
      this._SalesreturnService.getSRPrintbyBillNo(this.AttachedInfoBill.SalesReturn[this.j]).subscribe(
        response => {
          this.PrintSRDetails = response;
          if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
            this.PrintSRDetails = atob(this.PrintSRDetails.Data);
            this._salesBillingService.getAttachedInfoBill(this.AttachedInfoBill.BillNo).subscribe(
              response => {
                this.AttachedInfoBill = response;
                $('#ReprintSRBillModal').modal('show');
                $('#SRDisplayData').html(this.PrintSRDetails);
              }
            );
          }
          else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
            this.PrintSRDetails = this.PrintSRDetails.Data;
            this._salesBillingService.getAttachedInfoBill(this.AttachedInfoBill.BillNo).subscribe(
              response => {
                this.AttachedInfoBill = response;
                $('#ReprintSRBillModal').modal('show');
              }
            );
          }
        }
      )
    }
    else {
      $('#ReprintSRBillModal').modal('hide');
    }
  }

  getBillNos(arg) {
    this.billNo = arg;
  }

  //Order Reprint

  OrderFromOrderScreen: any;
  model = { option: 'Order No' };
  OrderNum: number;
  ReceiptList: any = [];
  ReceiptTotal: any = []
  orderDetails: any = [];
  Linesarray: any = [];
  Paymentarray: any = [];
  receiptDets: any = [];
  ReceiptListdisplay: any = [];
  CustomerID: any;
  ReceiptNum: number;
  ShowroomList: any = [];
  TotalList: any = [];
  WeightTotalList: any = [];

  reloadOrders() {
    this._orderService.castOrderNoToReprintComp.subscribe(
      response => {
        this.OrderFromOrderScreen = response;
        if (this.OrderFromOrderScreen != null) {
          this.model.option == 'Order No';
          this.OrderNum = this.OrderFromOrderScreen.OrderNo;
          this.getOrderDetails(this.OrderFromOrderScreen.OrderNo);
          //Commneted Based on print getting from api
          // if (this.OrderFromOrderScreen.ReceiptNo != 0 && this.OrderFromOrderScreen.ReceiptNo != null) {
          //   this.model.option == 'Receipt No';
          //   this.getReceiptDetails(this.OrderFromOrderScreen.ReceiptNo);
          // }
          // else {
          //   this.ReceiptList = [];
          //   this.ReceiptTotal = [];
          // }
          //ends here
        }
      }
    )
  }

  getOrderDetails(arg) {
    // if (!arg) {
    //   swal("Warning!", 'Please enter Order number', "warning");
    //   $('#OrderTab').modal('hide');
    // }
    // else {
    //   this._orderService.getPrintOrder(arg).subscribe(
    //     response => {
    //       this.orderDetails = response;
    //       this.OrderNum = arg;
    //       this.Linesarray = this.orderDetails.lstOfOrderItemDetailsVM;
    //       this.Paymentarray = this.orderDetails.lstOfPayment;
    //       this.getShowroomDetails();
    //       this.getWeightTotal(this.orderDetails.OrderNo);
    //       this.getTotal(this.orderDetails.OrderNo);
    //       $('#ReprintTypeModal').modal('hide');
    //       $('#OrderTab').modal('show');
    //       if (this._router.url == "/orders/reprint") {
    //         this._orderService.getReceptDetByOrder(arg).subscribe(
    //           response => {
    //             this.receiptDets = response;
    //             if (this.receiptDets.length > 0) {
    //               this.getReceiptDetails(this.receiptDets[0]);
    //             }
    //           }
    //         )
    //       }
    //     }
    //   );
    // }


    this._orderService.getOrderPrint(arg, this.ReprintType.toUpperCase()).subscribe(
      response => {
        this.PrintTypeBasedOnConfig = response;
        if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
          $('#OrderTab').modal('show');
          this.orderDetails = atob(this.PrintTypeBasedOnConfig.Data);
          $('#DisplayOrderData').html(this.orderDetails);
        }
        else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
          $('#OrderTab').modal('show');
          this.orderDetails = this.PrintTypeBasedOnConfig.Data;
        }
      }
    );
  }

  getShowroomDetails() {
    this._orderService.getShowroom().subscribe(
      response => {
        this.ShowroomList = response;
        this.onSubmit();
      }
    );
  }

  getWeightTotal(arg) {
    this._orderService.getWeightTotal(arg).subscribe(
      response => {
        this.WeightTotalList = response;
      }
    )
  }

  getTotal(arg) {
    this._orderService.getTotal(arg).subscribe(
      response => {
        this.TotalList = response;
      },
      (err) => {
        if (err.status === 404) {
          this.TotalList = null;
        }
      }
    )
  }

  // getReceiptDetails(arg) {
  //   if (!arg) {
  //     swal("Warning!", 'Please select Receipt number', "warning");
  //     $('#ReceiptTab').modal('hide');
  //   }
  //   else {
  //     this._orderService.getPrintReceipt(arg).subscribe(
  //       response => {
  //         this.ReceiptList = response;
  //         this.ReceiptListdisplay = this.ReceiptList[0];
  //         this.CustomerID = this.ReceiptList[0].SeriesNo;
  //         this.ReceiptNum = arg;
  //         this._orderService.getPrintOrder(this.CustomerID).subscribe(
  //           response => {
  //             this.orderDetails = response;
  //             this.Linesarray = this.orderDetails.lstOfOrderItemDetailsVM;
  //             this.Paymentarray = this.orderDetails.lstOfPayment;
  //             this.getShowroomDetails();
  //             this.getWeightTotal(this.orderDetails.OrderNo);
  //             this.getTotal(this.orderDetails.OrderNo);
  //           }
  //         );
  //         this.getReceiptTotal(arg);
  //         if (this._router.url == "/orders/reprint") {
  //           if (this.model.option == 'Order No') {
  //             $('#ReceiptTab').modal('hide');
  //           }
  //           else {
  //             $('#ReceiptTab').modal('show');
  //           }
  //         }
  //         else {
  //           if (this._router.url == "/orders/OrderReceipt") {
  //             $('#ReceiptTab').modal('show');
  //           }
  //           else {
  //             $('#ReceiptTab').modal('hide');
  //           }
  //         }
  //       }
  //     );
  //   }
  // }

  getReceiptTotal(arg) {
    this._orderService.getReceiptTotal(arg).subscribe(
      response => {
        this.ReceiptTotal = response;
      }
    )
  }

  //Ends here

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
          table tr td.borderLeft{
            border-left: 3px solid rgb(0, 0, 0) !important;
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

  printOGBill() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.PrintOGDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printContents, popupWin;
      printContents = document.getElementById('OGDisplayData').innerHTML;
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
        table tr td.borderLeft{
          border-left: 3px solid rgb(0, 0, 0) !important;
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
}