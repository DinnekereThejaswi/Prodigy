import { PurchaseService } from './../purchase/purchase.service';
import { SalesService } from './../sales/sales.service';
import { OrdersService } from './../orders/orders.service';
import { CustomerService } from './../masters/customer/customer.service';
import { Component, OnInit, OnDestroy, ViewChild, ElementRef, } from '@angular/core';
import { formatDate } from '@angular/common';
import { estimationService } from './estimation.service';
import { Router } from '@angular/router'
import { AddBarcodeService } from './../sales/add-barcode/add-barcode.service';
import swal from 'sweetalert';
import { ToastrService } from 'ngx-toastr';
import { MasterService } from '../core/common/master.service';
import { AppConfigService } from '../AppConfigService';
import { ComponentCanDeactivate } from '../appconfirmation-guard';
import { SalesreturnService } from '../salesreturn/salesreturn.service';
import { FormBuilder, FormGroup } from '@angular/forms';
declare var $: any;

import * as CryptoJS from 'crypto-js';
@Component({
  selector: 'app-estimation',
  templateUrl: './estimation.component.html',
  styleUrls: ['./estimation.component.css'],
})

export class EstimationComponent implements OnInit, OnDestroy, ComponentCanDeactivate {
  SchemeAdjustForm: FormGroup;
  SchemeAdjust = {
    Branch: null,
    SchemCode: "",
    GroupCode: "",
    StartMSNo: "",
    EndMSNo: "",
    ChitAmt: "",
    TotalAmt: "",
    BonusAmt: "",
    WinnerAmount: "",
    DiscountAmt: "",
    TotalWeight: "",
  }
  //To Enable Schme adjutment ui
  EnableOfferAndCorpScreen: boolean = false;
  //To Enable Offer Discount buttons
  ApplycoinOffer: boolean = false;
 
  OfferModel2022 =
    {
      CompanyCode: "",
      BranchCode: "",
      EstimationNo: 0,
      Date: "",
      Rate: null,
      SalesEstDetail: []
    }
  OfferModel2022LineItems = {
    SlNo: 0,
    GSCode: "",
    BarcodeNo: "",
    ItemCode: "",
    SalesmanCode: "",
    CounterCode: "",
    MRPItem: true,
    Qty: 0,
    Grosswt: 0,
    Stonewt: 0,
    Netwt: 0,
    MetalValue: 0,
    VAAmount: 0,
    StoneCharges: 0,
    DiamondCharges: 0,
    Dcts: 0,
    ItemAmount: 0
  };
  ccode: string;
  bcode: string;
  toggle: boolean = false;

  filterRadioBtns: boolean = false;

  EnableCancelOffer: boolean = false;
  EnableAddOffer: boolean = true;

  CustomerName: any;

  leavePage: boolean = false;

  indicator: string;

  SalesData: any = {};

  PurchaseData: any = {};

  routerUrl: string = "";

  OrderAttachmentSummaryData: any = {};

  SRAttachmentSummaryData: any = {};

  OldGoldAttachmentSummaryData: any = {};

  NoRecordsCustomer: boolean = false;

  NoRecordsPurchase: boolean = false;

  // @ViewChild('coinBarcode') coinBarcode: ElementRef;

  @ViewChild('coinBarcode', { static: true }) coinBarcode: ElementRef;

  estNo: number;

  coinOfferData: any = {};



  TotalCoin: Number = 0;
  EstNo: string;

  radioItems: Array<string>;
  model = { option: 'New Estimation' };
  PrintTypeBasedOnConfig: any;
  today = new Date();
  estDate = '';
  ordDate = '';
  EnableReprint: boolean = false;
  EnableCoinOffer: boolean = true;
  MergeEstNo: any;
  EnableJson: boolean = false;
  //To Enable FAb2022 offer
  ApplyFab2020Offer: boolean = false;

  password: string;
  constructor(private _CustomerService: CustomerService, private _salesService: SalesService,
    private _estmationService: estimationService, private _purchaseService: PurchaseService,
    private _router: Router, private _addBarcodeService: AddBarcodeService,
    private _ordersService: OrdersService, private toastr: ToastrService,
    private _masterService: MasterService,
    private _appConfigService: AppConfigService,
    private _SalesreturnService: SalesreturnService, private fb: FormBuilder) {
    this.estDate = formatDate(this.today, 'dd/MM/yyyy', 'en-US', '+0530');
    this.ordDate = formatDate(this.today, 'dd/MM/yyyy', 'en-US', '+0530');
    this.radioItems = ['New Estimation', 'Existing Estimation', 'Order No'];
    // this.radioItems = ['New Estimation', 'Existing Estimation'];
    this.ApplyFab2020Offer = this._appConfigService.IsOffer;
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
    this.getCB();
  }


  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ReprintData: any;
  salesEstimationPlainTextDetails: any = [];
  purEstPlainTextDetails: any = [];
  OGPlainTextDetails: any = [];
  ReprintSRData: any;
  ngOnInit() {
    this.SchemeAdjustForm = this.fb.group({
      frmCtrl_Branch: null,
      frmCtrl_SchemCode: null,
      frmCtrl_GroupCode: null,
      frmCtrl_StartMSNo: null,
      frmCtrl_EndMSNo: null,
      frmCtrl_ChitAmt: null,
      frmCtrl_TotalAmt: null,
      frmCtrl_BonusAmt: null,
      frmCtrl_WinnerAmount: null,
      frmCtrl_DiscountAmt: null,
      frmCtrl_TotalWeight: null
    });
    this.SalesData = {
      Amount: 0,
    };

    this.PurchaseData = {
      Amount: 0,
    };

    this.OrderAttachmentSummaryData = {
      Amount: 0,
    };

    this.SRAttachmentSummaryData = {
      Amount: 0,
    };

    this.OldGoldAttachmentSummaryData = {
      Amount: 0
    };

    this.coinOfferData = {
      BarcodeNo: null,
      TotalOfferCoins: 0,
      AddCoin: 0,
      DisAmount: 0
    }


    this.NewEstimation = true;
    $(function () {
      $("#datepicker").datepicker();
    });

    this.getApplicationDate();
    this._estmationService.SendEstNo(null);
    this.GetCustomerDetsFromCustComp();
    this.GetSalesDetsFromSalescomp();
    // this.GetPurchaseDetsFromPurchaseComp();
    this.GetEstNoToggleSales();
    // this.GetSRAttachedSummary();
    // this.GetOrderAttachedSummary();
    // this.GetOldGoldAttachedSummary();

    this._estmationService.SubjectTotalCoinOffer.subscribe(
      response => {
        this.TotalCoin = response;
        if (this.TotalCoin != 0) {
          this.EnableCoinOffer = false;
          this.coinOfferData.TotalOfferCoins = this.TotalCoin;
        }
        else {
          this.EnableCoinOffer = true;
        }
      })

    this._estmationService.SubjectMergeEstNo.subscribe(
      response => {
        this.MergeEstNo = response;
        if (this.isEmptyObject(this.MergeEstNo) == false && this.isEmptyObject(this.MergeEstNo) != null) {
          this.estNo = this.MergeEstNo.NewEstimationNo;
          this.OnRadioBtnChnge("Existing Estimation");
          this.getEstimationDetails();
        }
      }
    )

    this._estmationService.castReprint.subscribe(
      response => {
        this.ReprintData = response;
        if (this.isEmptyObject(this.ReprintData) == false && this.isEmptyObject(this.ReprintData) != null) {
          this.estNo = this.ReprintData.EstNo;
          this._estmationService.getSalesEstimationPlainText(this.estNo).subscribe(
            response => {
              this.PrintTypeBasedOnConfig = response;
              if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
                this.salesEstimationPlainTextDetails = this.PrintTypeBasedOnConfig.Data;
              }
              else if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
                this.salesEstimationPlainTextDetails = atob(this.PrintTypeBasedOnConfig.Data);
                $('#DisplaySalesData').html(this.salesEstimationPlainTextDetails);
              }
              $('#SalesEstimationPlainTextTab').modal('show');
              $('#PurchaseEstimationPlainTextTab').modal('hide');
              $('#OGPlainTextTab').modal('hide');
              $('#SREstPlainTextTab').modal('hide');
              this.OrderNo = false;
              this.NewEstimation = false;//Added on 10-Mar-21 for Offer discount to be displayed without re-enter the estimation
              this.ExistingEstimation = true;//Added on 10-Mar-21 for Offer discount to be displayed without re-enter the estimation
              this.model.option = "Existing Estimation"; //Added on 19-Mar-2021 to handled for order as well
              //Below condition has been added as per sivanand to redirect to fresh estimation
              // this.commonFunc("New Estimation");
              // this.NewEstimation = true;
              // this.ExistingEstimation = false;
              // this.CollapseCustomerDetailsTab = false;
              // this.EnableSalesTab = false;
              // this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
              // this.ToggleSales = false;
              // this.NoRecordsSales = false;
              // this.OnRadioBtnChnge("Existing Estimation");//Added on 03-03-2021
              // this.getEstimationDetails();//Added on 03-03-2021
              // this._salesService.SendEstNo_To_Purchase(this.estNo);//Added on 03-03-2021
              //ends here
            }
          )
        }
      })
  }


  CloseSalesPrint() {
    $('#SalesEstimationPlainTextTab').modal('hide');

   // 01/03/2022
    //commented below code bcz all of the estimations print are came at single page only so
   //closing and calling individal print by passing estimation no not doing futher
    if (this.PrintTypeBasedOnConfig.ContinueNextPrint == true) {
      this._purchaseService.PrintPurEstDotMatrix(this.estNo).subscribe(
        response => {
          this.purEstPlainTextDetails = response;
          if (this.purEstPlainTextDetails.Data != "") {
            if (this.purEstPlainTextDetails.PrintType == "HTML") {
              this.purEstPlainTextDetails = atob(this.purEstPlainTextDetails.Data);
              $('#DisplayPurchaseData').html(this.purEstPlainTextDetails);
            }
            else if (this.purEstPlainTextDetails.PrintType == "RAW") {
              this.purEstPlainTextDetails = this.purEstPlainTextDetails.Data;
            }
            $('#SalesEstimationPlainTextTab').modal('hide');
            $('#PurEstPlainTextTab').modal('show');
            $('#OGPlainTextTab').modal('hide');
            $('#SREstPlainTextTab').modal('hide');
          }
          else {
            $('#SalesEstimationPlainTextTab').modal('hide');
            $('#PurEstPlainTextTab').modal('hide');
            this.ClosePurchasePrint();
          }
        }
      )
    }
    else {
      $('#SalesEstimationPlainTextTab').modal('hide');
    }
  }

  SelectedSalesReturnList: any = [];

  ClosePurchasePrint() {
    this._SalesreturnService.getSalesReturnAttachedList(this.estNo).subscribe(
      response => {
        this.SelectedSalesReturnList = response;
        if (this.SelectedSalesReturnList.length > 0) {
          for (var i = 0; i < this.SelectedSalesReturnList.length; i++) {
            this.getSREstPlainText(this.SelectedSalesReturnList[i]["SrEstNo"]);
          }
        }
        else {
          $('#PurchaseEstimationPlainTextTab').modal('hide');
          $('#SalesEstimationPlainTextTab').modal('hide');
          $('#SREstPlainTextTab').modal('hide');
          $('#OGPlainTextTab').modal('hide');
          this.CloseSRPrint();
        }
      }
    )
  }

  SREstimationPlainTextDetails: any = [];

  getSREstPlainText(arg) {
    this._SalesreturnService.getSRPrintbyEstNo(arg).subscribe(
      response => {
        this.SREstimationPlainTextDetails = response;
        if (this.SREstimationPlainTextDetails.PrintType == "RAW" && this.SREstimationPlainTextDetails.Data != "") {
          this.SREstimationPlainTextDetails = this.SREstimationPlainTextDetails.Data;
          $('#PurchaseEstimationPlainTextTab').modal('hide');
          $('#SalesEstimationPlainTextTab').modal('hide');
          $('#OGPlainTextTab').modal('hide');
          $('#SREstPlainTextTab').modal('show');
        }
        else if (this.SREstimationPlainTextDetails.PrintType == "HTML" && this.SREstimationPlainTextDetails.Data != "") {
          this.SREstimationPlainTextDetails = atob(this.SREstimationPlainTextDetails.Data);
          $('#DisplaySRData').html(this.SREstimationPlainTextDetails);
          $('#PurchaseEstimationPlainTextTab').modal('hide');
          $('#SalesEstimationPlainTextTab').modal('hide');
          $('#OGPlainTextTab').modal('hide');
          $('#SREstPlainTextTab').modal('show');
        }
        else {
          $('#PurchaseEstimationPlainTextTab').modal('hide');
          $('#SalesEstimationPlainTextTab').modal('hide');
          $('#SREstPlainTextTab').modal('hide');
          $('#OGPlainTextTab').modal('hide');
          this.CloseSRPrint();
        }
      }
    )
  }


  ReprintOGData: any;
  SelectedOldGoldList: any = [];

  CloseSRPrint() {
    this._purchaseService.getOldGoldttachedList(this.estNo).subscribe(
      response => {
        this.SelectedOldGoldList = response;
        if (this.SelectedOldGoldList.length > 0) {
          for (var i = 0; i < this.SelectedOldGoldList.length; i++) {
            this.getOGPlainText(this.SelectedOldGoldList[i]["EstNo"]);
          }
        }
        else {
          $('#PurchaseEstimationPlainTextTab').modal('hide');
          $('#SalesEstimationPlainTextTab').modal('hide');
          $('#SREstPlainTextTab').modal('hide');
          $('#OGPlainTextTab').modal('hide');
        }
      }
    )
  }

  getOGPlainText(arg) {
    this._purchaseService.PrintPurEstDotMatrix(arg).subscribe(
      response => {
        this.OGPlainTextDetails = response;
        if (this.OGPlainTextDetails.PrintType == "RAW" && this.OGPlainTextDetails.Data != "") {
          this.OGPlainTextDetails = this.OGPlainTextDetails.Data;
          $('#SalesEstimationPlainTextTab').modal('hide');
          $('#PurEstPlainTextTab').modal('hide');
          $('#SREstPlainTextTab').modal('hide');
          $('#OGPlainTextTab').modal('show');
        }
        else if (this.OGPlainTextDetails.PrintType == "HTML" && this.OGPlainTextDetails.Data != "") {
          this.OGPlainTextDetails = atob(this.OGPlainTextDetails.Data);
          $('#DisplayOGData').html(this.OGPlainTextDetails);
          $('#SalesEstimationPlainTextTab').modal('hide');
          $('#PurEstPlainTextTab').modal('hide');
          $('#SREstPlainTextTab').modal('hide');
          $('#OGPlainTextTab').modal('show');
        }
        else {
          $('#SalesEstimationPlainTextTab').modal('hide');
          $('#PurEstPlainTextTab').modal('hide');
          $('#SREstPlainTextTab').modal('hide');
          $('#OGPlainTextTab').modal('hide');
        }
      }
    )
  }

  printSalesEstimation() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.salesEstimationPlainTextDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      this.printSalesEstimationHtml();
    }
  }


  printSalesEstimationHtml() {
    let printContentsSalesPrint, popupWin;
    printContentsSalesPrint = document.getElementById('DisplaySalesData').innerHTML;
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

    ${printContentsSalesPrint}</body>
      </html>`
    );
    popupWin.document.close();
  }



  printPurEst() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.purEstPlainTextDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      this.printPurchaseEstimationHtml();
    }
  }


  printPurchaseEstimationHtml() {
    let printContentsPurchasePrint, popupWin;
    printContentsPurchasePrint = document.getElementById('DisplayPurchaseData').innerHTML;
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

    ${printContentsPurchasePrint}</body>
      </html>`
    );
    popupWin.document.close();
  }

  printSREstimation() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.SREstimationPlainTextDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      this.printSREstimationHtml();
    }
  }


  printSREstimationHtml() {
    let printContentsSRPrint, popupWin;
    printContentsSRPrint = document.getElementById('DisplaySRData').innerHTML;
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

    ${printContentsSRPrint}</body>
      </html>`
    );
    popupWin.document.close();
  }



  printOldGold() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.OGPlainTextDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      this.printOGEstimationHtml();
    }
  }


  printOGEstimationHtml() {
    let printContentsOGPrint, popupWin;
    printContentsOGPrint = document.getElementById('DisplayOGData').innerHTML;
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

    ${printContentsOGPrint}</body>
      </html>`
    );
    popupWin.document.close();
  }

  RecallPrint() {
    if (this.estNo == null || this.estNo == 0) {
      swal("Warning!", 'Please enter the Estimation No', "warning");
    }
    else if (this.estNo == 0) {
      swal("Warning!", 'Invalid Estimation No', "warning");
    }
    else {
      this._estmationService.getSalesEstimationPlainText(this.estNo).subscribe(
        response => {
          this.PrintTypeBasedOnConfig = response;
          if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
            this.salesEstimationPlainTextDetails = this.PrintTypeBasedOnConfig.Data;
            $('#SalesEstimationPlainTextTab').modal('show');
            $('#PurchaseEstimationPlainTextTab').modal('hide');
            $('#OGPlainTextTab').modal('hide');
            $('#SREstPlainTextTab').modal('hide');
          }
          else if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
            this.salesEstimationPlainTextDetails = atob(this.PrintTypeBasedOnConfig.Data);
            $('#DisplaySalesData').html(this.salesEstimationPlainTextDetails);
            $('#SalesEstimationPlainTextTab').modal('show');
            $('#PurchaseEstimationPlainTextTab').modal('hide');
            $('#OGPlainTextTab').modal('hide');
            $('#SREstPlainTextTab').modal('hide');
          }
        }
      )
    }
  }


  //Radio Buton change

  NewEstimation: boolean = false;
  ExistingEstimation: boolean = false;
  OrderNo: boolean = false;

  OnRadioBtnChnge(arg) {
    if (arg == "New Estimation") {
      this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
        () => {
          this._router.navigate(['/estimation']);
          this.model.option = arg;
          this.NewEstimation = true;
          this.CollapseCustomerDetailsTab = false;
          this.TogglePurchase = false;
          this.NoRecordsPurchase = false;
          this.EnableSalesTab = false;
          //this._estmationService.SendEstNo(""); //Commented as part of code cleaning.
          //this._estmationService.SendMergeEstNoToEstComp(null); //Commented as part of code cleaning.
          //this._estmationService.SendReprintData(null); //Commented as part of code cleaning.
        })
    }
    else if (arg == "Existing Estimation") {
      this.ExistingEstimation = true;
      this.OrderNo = false;
      // this.estNo = null;
      this.commonFunc(arg);
      if (this.MergeEstNo == null) {
        this.estNo = null;
      }
      this.EnableSalesTab = true;
      this.estDetails = [];
    }
    else if (arg == "Order No") {
      this.OrderNo = true;
      this.ExistingEstimation = false;
      this.commonFunc(arg);
      this.EnableSalesTab = true;
      this.estDetails = [];

    }
  }

  confirmBeforeLeave(): boolean {
    if (this.leavePage == true) {
      var ans = (confirm("You have unsaved changes! If you leave, your changes will be lost."))
      if (ans) {
        this.leavePage = false;
        return true;
      }
      else {
        return false;
      }
    }
    else {
      return true;
    }
  }

  commonFunc(arg) {
    this.model.option = arg;
    this.NewEstimation = false;
    this.CollapseCustomerDetailsTab = true;
    this._CustomerService.SendCustDataToEstComp(null);
    this._addBarcodeService.SendBarcodeDataToSalesComp(null);
    this._salesService.salesDetails(null);
    this._salesService.SaveSalesEstNo(null);
    this._estmationService.SendEstNo(null);
    this._salesService.SendSalesDataToEstComp(null);
    this._purchaseService.sendPurchaseDatatoEstComp(null);
    this.NoRecordsAttachOrder = false;
    this.EnableOrderAttachmentTab = true;
    this._estmationService.SendOrderAttachmentSummaryData(null);
    this.NoRecordsAttachSR = false;
    this.EnableSRAttachmentTab = true;
    this._estmationService.SendSRAttachmentSummaryData(null);
    this.NoRecordsAttachOldGold = false;
    this.EnableOldGoldAttachmentTab = true;
    this._estmationService.SendOldGoldAttachmentSummaryData(null);
    this._estmationService.SendTotalCoinOffer(0);
    this._estmationService.SendCoinOfferDatatoSales(null);
    this.DelCoinOfferCalculation(1);
    this._estmationService.SendReprintData(null);
    this._estmationService.SendReprintSR(null);
    this.GlobalOrderAmount = 0;
  }

  AddedCoins: number = 0;

  coinCalc() {
    if (this.GetBarcodeList.length > 0) {
      this.AddedCoins = this.GetBarcodeList.length;
      this.coinOfferData.AddCoin = this.AddedCoins;
    }
  }

  DiscountAmount: number = 0;
  DiscountPercent: number = 0;
  ArrayList: any = [];
  GetBarcodeList: any = [];
  CoinBarcode(arg) {
    this._salesService.getBarcodefromAPI(arg, "0", 0, 1).subscribe(
      response => {
        this.ArrayList = response;
        if (this.ArrayList != null) {
          this.coinOfferData.BarcodeNo = arg;
          this.DiscountAmount = 0;
          this.GetBarcodeList.push(this.ArrayList);
          this.coinCalc();
          if (this.AddedCoins > this.TotalCoin) {
            var ans = confirm("Offer issued weight is greater than eligible offer weight(" + this.TotalCoin + ").Do you want to proceed?");
            if (ans) {
              for (var i = 0; i < this.GetBarcodeList.length; i++) {
                this.DiscountAmount += Number(<number>this.GetBarcodeList[i].ItemTotalAfterDiscount);
              }
              this.coinOfferData.DisAmount = this.DiscountAmount
              this.DiscountPercent = Number(<number>this.DiscountAmount) / Number(<number>this.SalesData.taxable_Amt + <number>this.DiscountAmount) * 100;
              this._estmationService.SendCoinOfferDatatoSales(this.coinOfferData);
            }
            else {
              this.GetBarcodeList.splice(this.AddedCoins - 1, 1);
              this.coinCalc();
              for (var i = 0; i < this.GetBarcodeList.length; i++) {
                this.DiscountAmount += Number(<number>this.GetBarcodeList[i].ItemTotalAfterDiscount);
              }
              this.coinOfferData.DisAmount = this.DiscountAmount
              this.DiscountPercent = Number(<number>this.DiscountAmount) / Number(<number>this.SalesData.taxable_Amt + <number>this.DiscountAmount) * 100;
              this.coinOfferData.BarcodeNo = null;
              this._estmationService.SendCoinOfferDatatoSales(this.coinOfferData);
            }
          }
          else {
            for (var i = 0; i < this.GetBarcodeList.length; i++) {
              this.DiscountAmount += Number(<number>this.GetBarcodeList[i].ItemTotalAfterDiscount);
            }
            this.coinOfferData.DisAmount = this.DiscountAmount
            this.DiscountPercent = Number(<number>this.DiscountAmount) / Number(<number>this.SalesData.taxable_Amt + <number>this.DiscountAmount) * 100;
            this._estmationService.SendCoinOfferDatatoSales(this.coinOfferData);
          }
        }
      })
    this.coinBarcode.nativeElement.value = '';
  }

  estDetails: any = [];
  TotAmount: number = 0;
  purchaseDetails: any = [];

  getEstimationDetails() {
    if (this.estNo == null || this.estNo == 0) {
      swal("Warning!", 'Please enter the Estimation No', "warning");
    }
    else if (this.estNo == 0) {
      swal("Warning!", 'Invalid Estimation No', "warning");
    }
    else {
      this.ReceivableAmount = 0.00;
      this._CustomerService.SendCustDataToEstComp(null);
      this._salesService.salesDetails(null);
      this._salesService.SendSalesDataToEstComp(null);
      this._purchaseService.sendPurchaseDatatoEstComp(null);
      this._estmationService.SendOrderAttachmentSummaryData(null);
      this._estmationService.SendSRAttachmentSummaryData(null);
      this._estmationService.SendOldGoldAttachmentSummaryData(null);
      this._estmationService.SendReprintData(null);
      this.DiscountAmount = 0;
      this.DiscountPercent = 0;
      //ends
      this._estmationService.getEstimationDetailsfromAPI(this.estNo).subscribe(
        response => {
          this.estDetails = response;
          if (this.estDetails != null && this.estDetails.salesEstimatonVM.length > 0) {
            //this.leavePage = true;
            for (var i = 0; i < this.estDetails.salesEstimatonVM.length; i++) {
              if (this.estDetails.salesEstimatonVM[i].IsEDApplicable != "") {
                ++this.AddedCoins;
                this.TotalCoin = 0;
                this.DiscountAmount += Number(<number>this.estDetails.salesEstimatonVM[i].ItemTotalAfterDiscount);
              }
              else {
                this.TotAmount += Number(<number>this.estDetails.salesEstimatonVM[i].TotalAmount);
              }
            }

            if (this.DiscountAmount != 0) {
              this.DiscountPercent = Number(<number>this.DiscountAmount) / Number(<number>this.TotAmount + <number>this.DiscountAmount) * 100;
            }

            if (this.estDetails != null && this.estDetails.offerDiscount != null) {
              this.DiscountAmount = this.estDetails.offerDiscount.DiscountAmount;
              this.DiscountPercent = this.estDetails.offerDiscount.DiscountPercent;
            }


            this.NoSummaryOrder = true;
            //this.NoRecordsAttachOrder = true;
            //this.EnableOrderAttachmentTab = true;
            this.GlobalOrderAmount = this.estDetails.OrderAmount;
            this.OrderTotalAmountCalculation(0);
            this._salesService.SaveSalesEstNo(this.estNo);
            this._estmationService.SendEstNo(this.estNo);
            this._CustomerService.getCustomerDtls(this.estDetails.CustID).subscribe(
              response => {
                const customerDtls = response;
                this.ToggleSales = true;
                this._CustomerService.sendCustomerDtls_To_Customer_Component(customerDtls);
                this.estDate = formatDate(this.estDetails.EstDate, 'dd/MM/yyyy', 'en-US', '+0530');
                this.ordDate = formatDate(this.estDetails.OrderDate, 'dd/MM/yyyy', 'en-US', '+0530');
                this._salesService.salesDetails(this.estDetails)
                //this.ToggleSalesData();
                this.EnableSalesTab = true;
                this.EnablePurchaseTab = true;
              }
            )
          }
        },
        (err) => {
          this.ngOnDestroy();
          this.SalesTotAmount = 0;
          this.PurchaseTotAmount = 0;
          this.OrderTotAmount = 0;
          this.SRTotAmount = 0;
          this.OGTotAmount = 0;
        }
      )
    }
  }

  loadOfferDetails() {
    if (this.estNo == null || this.estNo == 0) {
      swal("Warning!", 'Please enter the Estimation No', "warning");
    }
    else if (this.estNo == 0) {
      swal("Warning!", 'Invalid Estimation No', "warning");
    }
    else {
      this._salesService.getOfferDiscount(this.estNo).subscribe(
        response => {
          this.estDetails = response;
          this.getEstimationDetails();
          swal("Success!", "Offer Discount Applied", "success");
        }
      )
    }
  }

  RemoveOfferDetails() {
    if (this.estNo == null || this.estNo == 0) {
      swal("Warning!", 'Please enter the Estimation No', "warning");
    }
    else if (this.estNo == 0) {
      swal("Warning!", 'Invalid Estimation No', "warning");
    }
    else {
      if (this.estDetails.offerDiscount != null) {
        this._salesService.cancelOfferDiscount(this.estNo).subscribe(
          response => {
            this.estDetails = response;
            this.getEstimationDetails();
            swal("Success!", "Offer Discount Removed", "success");
          }
        )
      }
    }
  }
  //2022 offer implementation
  OfferResponse: any = [];
  ApplyFab2022Offer() {
    this.OfferModel2022 = {
      CompanyCode: "",
      BranchCode: "",
      EstimationNo: 0,
      Date: "",
      Rate: null,
      SalesEstDetail: []
    }
    if (this.estNo == null || this.estNo == 0) {
      swal("Warning!", 'Please enter the Estimation No', "warning");
    }
    else if (this.estNo == 0) {
      swal("Warning!", 'Invalid Estimation No', "warning");
    }
    else {
      this.OfferModel2022.CompanyCode = this.estDetails.CompanyCode;
      this.OfferModel2022.BranchCode = this.estDetails.BranchCode;
      this.OfferModel2022.EstimationNo = this.estDetails.estNo;
      this.OfferModel2022.Date = formatDate(this.estDetails.EstDate, 'yyyy-MM-dd', 'en_GB');
      this.OfferModel2022.Rate = this.estDetails.Rate;
      if (this.estDetails.salesEstimatonVM.length > 0) {
        for (let i of this.estDetails.salesEstimatonVM) {
          this.OfferModel2022LineItems.SlNo = i.SlNo;
          this.OfferModel2022LineItems.GSCode = i.GsCode;
          this.OfferModel2022LineItems.BarcodeNo = i.BarcodeNo;
          this.OfferModel2022LineItems.ItemCode = i.ItemName;
          this.OfferModel2022LineItems.SalesmanCode = i.SalCode;
          this.OfferModel2022LineItems.CounterCode = i.CounterCode;
          this.OfferModel2022LineItems.MRPItem = true;
          this.OfferModel2022LineItems.Qty = i.ItemQty;
          this.OfferModel2022LineItems.Grosswt = i.Grosswt;
          this.OfferModel2022LineItems.Stonewt = i.Stonewt;
          this.OfferModel2022LineItems.Netwt = i.Netwt;
          this.OfferModel2022LineItems.MetalValue = i.GoldValue;
          this.OfferModel2022LineItems.VAAmount = i.VaAmount;
          this.OfferModel2022LineItems.StoneCharges = i.StoneCharges;
          this.OfferModel2022LineItems.DiamondCharges = i.DiamondCharges;
          this.OfferModel2022LineItems.ItemAmount = i.TotalAmount;
          this.OfferModel2022.SalesEstDetail.push(this.OfferModel2022LineItems);
          this.OfferModel2022LineItems = {
            SlNo: 0,
            GSCode: "",
            BarcodeNo: "",
            ItemCode: "",
            SalesmanCode: "",
            CounterCode: "",
            MRPItem: true,
            Qty: 0,
            Grosswt: 0,
            Stonewt: 0,
            Netwt: 0,
            MetalValue: 0,
            VAAmount: 0,
            StoneCharges: 0,
            DiamondCharges: 0,
            Dcts: 0,
            ItemAmount: 0
          };
          this._salesService.apply2022Offer(this.OfferModel2022).subscribe(
            response => {
              this.OfferResponse = response;
              this.estDetails.IsOfferApplied = true;
              this.estDetails.OfferDiscountAmount = this.OfferResponse.DiscountAmount;
              swal("Success!", this.OfferResponse.OfferName, "success");
            }
          )
        }
      }
    }
  }
  RemoveFab2022Offer() {
    this.estDetails.IsOfferApplied = false;
  }

  GlobalOrderAmount: Number = 0;

  getEstimationOrderDetails(arg) {
    if (arg == null || arg == "") {
      swal("Warning!", 'Please enter the Order Number', "warning");
    }
    else if (arg == 0) {
      swal("Warning!", 'Invalid Order Number', "warning");
    }
    else {
      //Repetitive Code
      this.ReceivableAmount = 0.00;
      //ends
      this._salesService.SaveSalesEstNo(arg);
      this._estmationService.SendOrderNoToSalesComp(arg);
      this._estmationService.getEstimationOrderDetailsfromAPI(arg).subscribe(
        data => {
          this.estDetails = data;
          if (this.estDetails != null) {
            this._CustomerService.SendCustDataToEstComp(null);
            this._salesService.salesDetails(null);
            this._salesService.SendSalesDataToEstComp(null);
            this._purchaseService.sendPurchaseDatatoEstComp(null);
            this._estmationService.SendOrderAttachmentSummaryData(null);
            this._estmationService.SendSRAttachmentSummaryData(null);
            this._estmationService.SendOldGoldAttachmentSummaryData(null);
            this.NoSummaryOrder = true;
            //this.NoRecordsAttachOrder = true;
            //this.EnableOrderAttachmentTab = true;
            this.GlobalOrderAmount = this.estDetails.OrderAmount;
            this.OrderTotalAmountCalculation(0);
            this._CustomerService.getCustomerDtls(this.estDetails.CustID).subscribe(
              response => {
                const customerDtls = response;
                this.ToggleSales = true;
                this._CustomerService.sendCustomerDtls_To_Customer_Component(customerDtls);
                //this._estmationService.SendEstNo(arg);
                this.estDate = formatDate(this.estDetails.EstDate, 'dd/MM/yyyy', 'en-US', '+0530');
                this.ordDate = formatDate(this.estDetails.OrderDate, 'dd/MM/yyyy', 'en-US', '+0530');
                this._salesService.salesDetails(this.estDetails)
                this.EnableSalesTab = true;
                this.ToggleSalesData();
                // this.EnableOrderAttachmentTab = !this.EnableOrderAttachmentTab;
              }
            )
          }
        },
        (err) => {
          if (err.status === 400) {
            const validationError = err.error;
            swal("Warning!", validationError.description, "warning");
            this.OnRadioBtnChnge("Order No");
          }
        }
      )
    }
  }


  //Hide Show data when accordian collapsed(Header)
  public ToggleTotal: boolean = true;
  public CollapseCustomerTab: boolean = true;
  public CollapseCustomerDetailsTab: boolean = false;


  EnableSalesTab: boolean = false;
  EnableOrderAttachmentTab: boolean = true;
  EnableSRAttachmentTab: boolean = true;
  EnableOldGoldAttachmentTab: boolean = true;
  EnableWeightScheme: boolean = true;
  EnableCorpEmpDetails: boolean = true;
  EnableSchemeAdjust: boolean = false;
  NoRecordsSales: boolean = false;
  NoSummarySales: boolean = false;
  NoSummaryOrder: boolean = false;
  NoSummaryPurchase: boolean = false;
  NoSummarySalesReturn: boolean = false;
  NoSummaryOldGold: boolean = false;
  NoRecordsAttachSR: boolean = false
  NoRecordsAttachOldGold: boolean = false
  ToggleAttachOrder: boolean = true;
  NoRecordsAttachOrder: boolean = false;
  ReceivableAmount: number = 0.00;

  ToggleTotalFunction() {
    this.ToggleTotal = !this.ToggleTotal;
  }

  //Hide Show data when accordian collapsed(Customer)
  //public ToggleCustomer: boolean = true;
  // ToggleCustomerData() {
  //   // if (this.NoRecordsCustomer == true) {
  //   //   this.filterRadioBtns = true;
  //   this.CollapseCustomerTab = !this.CollapseCustomerTab;
  //   this.CollapseCustomerDetailsTab = !this.CollapseCustomerDetailsTab;
  //   // }
  // }

  ToggleCustomerData() {
    if ((this.model.option == "Order No") && (this.GlobalOrderAmount == 0 || this.GlobalOrderAmount == null)) {
      this.CollapseCustomerDetailsTab = true;
    }
    else if ((this.model.option == "Existing Estimation") && (this.EstNo == null || this.EstNo == "0")) {
      this.CollapseCustomerDetailsTab = true;
    }
    else {
      this.CollapseCustomerDetailsTab = !this.CollapseCustomerDetailsTab;
    }
  }

  //Hide Show data when accordian collapsed(Sales)
  public ToggleSales: boolean = false;
  ToggleSalesData() {
    if ((this.model.option == "Order No") && (this.GlobalOrderAmount == 0 || this.GlobalOrderAmount == null)) {
      this.EnableSalesTab = true;
    }
    else if ((this.model.option == "Existing Estimation") && (this.EstNo == null || this.EstNo == "0")) {
      this.EnableSalesTab = true;
    }
    else {
      this.EnableSalesTab = !this.EnableSalesTab;
    }
  }

  LoadSalesData() {
    if (this.NoRecordsCustomer == true) {
      this.filterRadioBtns = true;
      this.NoRecordsSales = true;
      this.ToggleSales = true;
    }
  }

  //Hide Show data when accordian collapsed(Purchase)
  public TogglePurchase: boolean = false;
  EnablePurchaseTab: boolean = true;
  TogglePurchaseData() {
    // if (this.NoRecordsSales == true) {
    //   // if (this.EstNo != "" && this.EstNo!=null) {
    //   this._salesService.cast.subscribe(
    //     response => {
    //       this.SalesData = response;
    //       if (this.isEmptyObject(this.SalesData) == false && this.isEmptyObject(this.SalesData) != null) {
    //         if (this.NoSummaryPurchase == true) { //Added if/else condition to display purchase summary data on 23-Mar-2021
    //           this.EnablePurchaseTab = !this.EnablePurchaseTab;
    //           this.filterRadioBtns = true;
    //           this.TogglePurchase = !this.TogglePurchase;
    //           this._purchaseService.cast.subscribe(
    //             response => {
    //               this.PurchaseData = response;
    //               if (this.isEmptyObject(this.PurchaseData) == false && this.isEmptyObject(this.PurchaseData) != null) {
    //                 if (this.EnablePurchaseTab == true) {
    //                   this.NoRecordsPurchase = true;
    //                   this.TogglePurchase = true;
    //                 }
    //                 else {
    //                   this.NoRecordsPurchase = false;
    //                   this.TogglePurchase = false;
    //                 }
    //               }
    //               else {
    //                 this.NoRecordsPurchase = false;
    //                 this.TogglePurchase = false;
    //               }
    //             }
    //           )
    //         }
    //         else {
    //           this.EnablePurchaseTab = !this.EnablePurchaseTab;
    //         }
    //       }
    //       else {
    //         this.EnablePurchaseTab = true;
    //       }
    //     });
    //   // }
    // }
    if (this.NoSummarySales == true) {
      this.EnablePurchaseTab = !this.EnablePurchaseTab;
    }
  }

  ToggleOrderAttachment() {
    //this.EnableOrderAttachmentTab = !this.EnableOrderAttachmentTab;
    // this._salesService.cast.subscribe(
    //   response => {
    //     this.SalesData = response;
    //     if (this.isEmptyObject(this.SalesData) == false && this.isEmptyObject(this.SalesData) != null) {
    //       this.filterRadioBtns = true;
    //       this.EnableOrderAttachmentTab = !this.EnableOrderAttachmentTab;
    //       // this._estmationService.CastOrderAttachmentSummaryData.subscribe(
    //       //   response => {
    //       //     this.OrderAttachmentSummaryData = response;
    //       //     if (this.isEmptyObject(this.OrderAttachmentSummaryData) == false && this.isEmptyObject(this.OrderAttachmentSummaryData) != null) {
    //       //       if (this.EnableOrderAttachmentTab == true) {
    //       //         this.NoRecordsAttachOrder = true;
    //       //       }
    //       //       else {
    //       //         this.NoRecordsAttachOrder = false;
    //       //       }
    //       //     }
    //       //     else {
    //       //       this.NoRecordsAttachOrder = false;
    //       //     }
    //       //   }
    //       // )
    //     }
    //     else {
    //       this.EnableOrderAttachmentTab = true;
    //     }
    //   });
    if (this.NoSummarySales == true) {
      this.EnableOrderAttachmentTab = !this.EnableOrderAttachmentTab;
    }
  }

  ToggleSRAttachment() {
    // this._salesService.cast.subscribe(
    //   response => {
    //     this.SalesData = response;
    //     if (this.isEmptyObject(this.SalesData) == false && this.isEmptyObject(this.SalesData) != null) {
    //       this.filterRadioBtns = true;
    //       this.EnableSRAttachmentTab = !this.EnableSRAttachmentTab;
    //       // this._estmationService.CastSRAttachmentSummaryData.subscribe(
    //       //   response => {
    //       //     this.SRAttachmentSummaryData = response;
    //       //     if (this.isEmptyObject(this.SRAttachmentSummaryData) == false && this.isEmptyObject(this.SRAttachmentSummaryData) != null) {
    //       //       if (this.EnableSRAttachmentTab == true) {
    //       //         this.NoRecordsAttachSR = true;
    //       //       }
    //       //       else {
    //       //         this.NoRecordsAttachSR = false;
    //       //       }
    //       //     }
    //       //     else {
    //       //       this.NoRecordsAttachSR = false;
    //       //     }
    //       //   }
    //       // )
    //     }
    //     else {
    //       this.EnableSRAttachmentTab = true;
    //     }
    //   });
    if (this.NoSummarySales == true) {
      this.EnableSRAttachmentTab = !this.EnableSRAttachmentTab;
    }
  }

  ToggleOldGoldAttachment() {
    // this._salesService.cast.subscribe(
    //   response => {
    //     this.SalesData = response;
    //     if (this.isEmptyObject(this.SalesData) == false && this.isEmptyObject(this.SalesData) != null) {
    //       this.filterRadioBtns = true;
    //       this.EnableOldGoldAttachmentTab = !this.EnableOldGoldAttachmentTab;
    //       // this._estmationService.CastOldGoldAttachmentSummaryData.subscribe(
    //       //   response => {
    //       //     this.OldGoldAttachmentSummaryData = response;
    //       //     if (this.isEmptyObject(this.OldGoldAttachmentSummaryData) == false && this.isEmptyObject(this.OldGoldAttachmentSummaryData) != null) {
    //       //       if (this.EnableOldGoldAttachmentTab == true) {
    //       //         this.NoRecordsAttachOldGold = true;
    //       //       }
    //       //       else {
    //       //         this.NoRecordsAttachOldGold = false;
    //       //       }
    //       //     }
    //       //     else {
    //       //       this.NoRecordsAttachOldGold = false;
    //       //     }
    //       //   }
    //       // )
    //     }
    //     else {
    //       this.EnableOldGoldAttachmentTab = true;
    //     }
    //   });
    if (this.NoSummarySales == true) {
      this.EnableOldGoldAttachmentTab = !this.EnableOldGoldAttachmentTab;
    }
  }

  ToggleWeightScheme() {
    // if (this.NoSummarySales == true) {
    this.EnableWeightScheme = !this.EnableWeightScheme;
    // }
  }
  ToggleCorpEmpDetails() {
    // if (this.NoSummarySales == true) {
    this.EnableCorpEmpDetails = !this.EnableCorpEmpDetails;
    // }
  }
  ToggleSchemeAdjust() {
    this.EnableSchemeAdjust = !this.EnableSchemeAdjust;
  }
  ngOnDestroy() {
    this._salesService.SendSalesDataToEstComp(null);
    this._CustomerService.SendCustDataToEstComp(null);
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
    this._salesService.salesDetails(null);
    this._purchaseService.sendPurchaseDatatoEstComp(null);
    this._estmationService.SendEstNo(null);
    this._salesService.SaveSalesEstNo(null);
    this._estmationService.SendOrderAttachmentSummaryData(null);
    this._estmationService.SendSRAttachmentSummaryData(null);
    this._estmationService.SendOldGoldAttachmentSummaryData(null);
    this._addBarcodeService.SendBarcodeDataToSalesComp(null);
    this._salesService.SendSalesBarcode("");
    this._addBarcodeService.SendBarcodeDataToSalesComp(null);
    this._estmationService.SendReprintData(null);
    this._estmationService.SendReprintSR(null);
    this._estmationService.SendOrderNoToSalesComp(0);
    this._estmationService.SendTotalCoinOffer(0);
    this._estmationService.SendCoinOfferDatatoSales(null);
    this.DelCoinOfferCalculation(1);
    this._estmationService.SendMergeEstNoToEstComp(null);
    this._ordersService.SendReservedOrderDetsToSalesComp(null);
  }

  isEmptyObject(obj) {
    return (obj && (Object.keys(obj).length === 0));
  }

  EnableSales() {
    this.EnableSalesTab = true;
    this.NoRecordsSales = true;
    this.TogglePurchase = false;
    this.NoRecordsPurchase = false;
  }

  // EnablePurchase() {
  //   this.EnableSalesTab = false;
  //   this.EnablePurchaseTab = false;
  //   this.NoRecordsSales = false;
  //   this.TogglePurchase = false;
  //   this.NoRecordsPurchase = false;
  // }

  GetCustomerDetsFromCustComp() {
    this._CustomerService.cast.subscribe(
      response => {
        this.CustomerName = response;
        if (this.isEmptyObject(this.CustomerName) == false && this.isEmptyObject(this.CustomerName) != null) {
          this.EnableSalesTab = false;
          this.CollapseCustomerTab = true;
          this.NoRecordsCustomer = true;
          this.NoRecordsSales = true;
          this.CollapseCustomerDetailsTab = true;
          //this.leavePage = true;
        }
        else {
          this.EnableSalesTab = false; //Added false on 23-Ape-20 due to 144th Bug
          this.CollapseCustomerTab = false;
          this.NoRecordsCustomer = false;
          this.NoRecordsSales = false;
          this.EnablePurchaseTab = true;
        }
      });
  }

  GetSalesDetsFromSalescomp() {
    this._salesService.cast.subscribe(
      response => {
        this.SalesData = response;
        if (this.isEmptyObject(this.SalesData) == false && this.isEmptyObject(this.SalesData) != null) {
          this.EnableSalesTab = false; //Added on 23-Ape-20 due to 144th Bug
          this.NoRecordsPurchase = false;
          this.NoSummarySales = true;
          this.ToggleSales = true;
          this.NoRecordsSales = true;
          this.EnableReprint = true;
          this.EnablePurchaseTab = true;
          // if (this.model.option == "New Estimation") {
          //   this.EnablePurchaseTab = false; //Added as part of UAT Bug 122 on 14-Apr-2021
          // }
          // else {
          //   this.EnablePurchaseTab = true; //Added as part of UAT Bug 122 on 14-Apr-2021
          // }
          //this.leavePage = true;
        }
        else {
          if (this.model.option == "New Estimation") {
            this.EnableSalesTab = false;
          }
          else {
            this.EnableSalesTab = true;
          }
          this.NoRecordsPurchase = false;
          this.NoSummarySales = false;
          this.ReceivableAmount = 0.00;
          this.EnableReprint = false;
          this.EnablePurchaseTab = true; //Added as part of UAT Bug 122 on 14-Apr-2021
        }
      });
  }

  // GetPurchaseDetsFromPurchaseComp() {
  //   this._purchaseService.cast.subscribe(
  //     response => {
  //       this.PurchaseData = response;
  //       if (this.isEmptyObject(this.PurchaseData) == false && this.isEmptyObject(this.PurchaseData) != null) {
  //         this.NoRecordsPurchase = true;
  //         this.TogglePurchase = true;
  //         this.EnablePurchaseTab = true;
  //         this.NoSummaryPurchase = true;
  //         this.EnableReprint = true;
  //       }
  //       else {
  //         this.NoRecordsPurchase = false;
  //         this.TogglePurchase = false;
  //         this.EnablePurchaseTab = true;
  //         this.NoSummaryPurchase = false;
  //         this.EnableReprint = false;
  //       }
  //     }
  //   )
  // }

  EnableMergeEstimation: boolean = false;

  MergeEst() {
    this.EnableMergeEstimation = true;
  }

  GetEstNoToggleSales() {
    this._salesService.EstNo.subscribe(
      response => {
        this.EstNo = response;
        if (this.EstNo != "" && this.EstNo != null) {
          this.EnableSales();
        }
        else {
          //this.EnablePurchase();
        }
      }
    )

    this._estmationService.EstNo.subscribe(
      response => {
        this.EstNo = response;
        if (this.EstNo != "" && this.EstNo != null) {
          this.EnableSales();
          this.EnablePurchaseTab = false;
          this.TogglePurchase = true;
          this.routerUrl = this._router.url;
        }
        else {
          //this.EnablePurchase();
        }
      }
    )
  }

  // GetOrderAttachedSummary() {
  //   this._estmationService.CastOrderAttachmentSummaryData.subscribe(
  //     response => {
  //       this.OrderAttachmentSummaryData = response;
  //       if (this.isEmptyObject(this.OrderAttachmentSummaryData) == false && this.isEmptyObject(this.OrderAttachmentSummaryData) != null) {
  //         this.NoRecordsAttachOrder = true;
  //         this.EnableOrderAttachmentTab = true;
  //         this.NoSummaryOrder = true;
  //       }
  //       else {
  //         this.NoRecordsAttachOrder = false;
  //         this.NoSummaryOrder = false;
  //       }
  //     });
  // }

  applicationDate: any;
  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
      }
    )
  }

  // GetSRAttachedSummary() {
  //   this._estmationService.CastSRAttachmentSummaryData.subscribe(
  //     response => {
  //       this.SRAttachmentSummaryData = response;
  //       if (this.isEmptyObject(this.SRAttachmentSummaryData) == false && this.isEmptyObject(this.SRAttachmentSummaryData) != null) {
  //         this.NoRecordsAttachSR = true;
  //         this.EnableSRAttachmentTab = true;
  //         this.NoSummarySalesReturn = true;
  //         this.EnableReprint = true;
  //       }
  //       else {
  //         this.NoRecordsAttachSR = false;
  //         this.NoSummarySalesReturn = false;
  //         this.EnableReprint = false;
  //       }
  //     });
  // }

  // GetOldGoldAttachedSummary() {
  //   this._estmationService.CastOldGoldAttachmentSummaryData.subscribe(
  //     response => {
  //       this.OldGoldAttachmentSummaryData = response;
  //       if (this.isEmptyObject(this.OldGoldAttachmentSummaryData) == false && this.isEmptyObject(this.OldGoldAttachmentSummaryData) != null) {
  //         this.NoRecordsAttachOldGold = true;
  //         this.EnableOldGoldAttachmentTab = true;
  //         this.NoSummaryOldGold = true;
  //         this.EnableReprint = true;
  //       }
  //       else {
  //         this.NoRecordsAttachOldGold = false;
  //         this.NoSummaryOldGold = false;
  //         this.EnableReprint = false;
  //       }
  //     });
  // }


  ///25-feb-2021 added all the load estimations with api 
  //and created new orderby pipe 
  allEstDetails: any = [];
  loadalltheEstimation() {
    $('#SalesEstimated').modal('show');
    this._estmationService.getAllEstimationNumbers().subscribe(
      response => {
        this.allEstDetails = response;
        this.allEstDetails.sort((a, b) => 0 - (a > b ? 1 : -1));

      }
    )
  }


  LoadBackEstDetails(arg) {
    this.estNo = arg.EstNo;
    $('#SalesEstimated').modal('hide');
    this.RecallPrint();
  }
  closeloadestimation() {
    this.allEstDetails.length = 0;
    this._estmationService.getAllEstimationNumbers().subscribe(
      response => {
        this.allEstDetails = response;

      }
    )
  }
  isDesc: boolean = false;
  column: string = 'CategoryName';

  sort(property) {
    this.isDesc = !this.isDesc; //change the direction    
    this.column = property;
    let direction = this.isDesc ? -1 : 1;
    this.allEstDetails.sort(function (a, b) {
      if (a[property] < b[property]) {
        return 1 * direction;
      }
      else if (a[property] > b[property]) {
        return -1 * direction;
      }
      else {
        return 0;
      }
    });
  };

  SalesTotAmount: number = 0;


  SalesTotalAmountCalculation(Amount) {
    this.SalesTotAmount = Amount;
  }

  PurchaseNtwt: number = 0;

  PurchaseNtWtCalculation(Ntwt) {
    this.PurchaseNtwt = Ntwt;
  }

  PurchaseGrossWt: number = 0;

  PurchaseGrossWtCalculation(GrossWt) {
    this.PurchaseGrossWt = GrossWt;
  }

  SalesTaxableAmount: number = 0;

  SalesTaxableAmountCalculation(Amount) {
    this.SalesTaxableAmount = Amount;
  }

  SalesTotalItems: number = 0;

  SalesTotalItemsCalculation(TotItems) {
    this.SalesTotalItems = TotItems;
  }

  SalesGrossWt: number = 0;

  SalesGrossWtCalculation(GrossWt) {
    this.SalesGrossWt = GrossWt;
  }

  SalesNetWt: number = 0;

  SalesNetWtCalculation(NetWt) {
    this.SalesNetWt = NetWt;
  }


  DelCoinOfferCalculation(arg) {
    this.DiscountAmount = 0;
    this.DiscountPercent = 0;
    this.TotalCoin = 0;
    this.AddedCoins = 0;
    this.EnableCoinOffer = true;
    this.GetBarcodeList = [];
  }

  PurchaseTotAmount: number = 0;

  PurchaseTotalAmountCalculation(Amount) {
    this.PurchaseTotAmount = Amount;
  }

  OrderTotAmount: number = 0;

  OrderTotalAmountCalculation(Amount) {
    this.OrderTotAmount = Amount + this.GlobalOrderAmount;
    this.OrderAttachmentSummaryData = {
      Amount: this.OrderTotAmount
    }
  }

  SRTotAmount: number = 0;

  SRTotalAmountCalculation(Amount) {
    this.SRTotAmount = Amount;
  }

  OGTotAmount: number = 0;

  OGTotalAmountCalculation(Amount) {
    this.OGTotAmount = Amount;
  }

  SelectedOrderList: any = [];

  FinalAmount() {
    //this.ReceivableAmount = this.SalesTotAmount - Math.round(this.PurchaseTotAmount) - this.SRTotAmount - this.OrderTotAmount - Math.round(this.OGTotAmount);
    this.ReceivableAmount = this.SalesTotAmount - this.PurchaseTotAmount - this.SRTotAmount - this.OrderTotAmount - this.OGTotAmount;
    return this.ReceivableAmount;
  }
  ///20-12-2021 scheme adjust

  OpenAtachSchemeModel() {
    $('#AttachSchemeModelPopUp').modal('show');
    this.getAllCompanybranchCode();
  }
  OpenAtachWeightSchemeModel() {
    $('#AttachWeightSchemeModelPopUp').modal('show');
    this.getAllCompanybranchCode();
  }
  allbranches: any = [];
  getAllCompanybranchCode() {
    this._estmationService.GetAllBranchesList().subscribe(
      response => {
        this.allbranches = response;
      }
    )
  }


}