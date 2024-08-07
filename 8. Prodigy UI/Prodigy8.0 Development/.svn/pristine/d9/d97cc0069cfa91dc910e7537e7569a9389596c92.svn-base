import { PurchaseService } from '../../purchase/purchase.service';
import { CustomerService } from '../../masters/customer/customer.service';
import { estimationService } from './../../estimation/estimation.service';
import { Component, OnInit, OnDestroy, ViewChild, ElementRef, } from '@angular/core';
import { Router } from '@angular/router';
import * as CryptoJS from 'crypto-js';
import { MasterService } from './../../core/common/master.service';
import { PaymentService } from '../../payment/payment.service';
import { PurchaseBilling, PurchaseModel } from './../purchase.model';
import { AppConfigService } from '../../AppConfigService';
import swal from 'sweetalert';
import { formatDate } from '@angular/common';
declare var $: any;
import * as moment from 'moment'
import { ComponentCanDeactivate } from '../../appconfirmation-guard';

@Component({
  selector: 'app-purchase-billing',
  templateUrl: './purchase-billing.component.html',
  styleUrls: ['./purchase-billing.component.css']
})

export class PurchaseBillingComponent implements OnInit, ComponentCanDeactivate {
  @ViewChild("estNo", { static: false }) estNo: ElementRef;
  autoFetchAmount: number = 0;
  toggle: boolean = false;
  routerUrl: string = "";
  filterRadioBtns: boolean = false;
  CustomerName: any;
  indicator: string;
  PaymentHeader: string = "";
  leavePage: boolean = false;
  SalesData: any = {};
  PurchaseData: any = {};
  OrderAttachmentSummaryData: any = {};
  SRAttachmentSummaryData: any = {};
  OldGoldAttachmentSummaryData: any = {};
  pageName: string = "OC";
  NoRecordsCustomer: boolean = false;
  NoRecordsPurchase: boolean = false;
  EnableReprint: boolean = false;

  EnableDisablePayment: boolean = false;
  EstNo: string;
  password: string;
  radioItems: Array<string>;
  model = { option: 'Payment' };
  ccode: string = "";
  bcode: string = "";
  today = new Date();
  estDate = '';
  ordDate = '';
  ShowHide: boolean = true;
  EnableJson: boolean = false;
  constructor(private _CustomerService: CustomerService,
    private _purchaseService: PurchaseService, private _router: Router,
    private _paymentService: PaymentService, private _estmationService: estimationService,
    private _masterService: MasterService, private _appConfigService: AppConfigService) {
    this.radioItems = ['Payment', 'Adjust'];
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
    this.getCB();
  }

  ngOnInit() {
    this.PurchaseData = {
      Amount: 0,
    };

    this.NewEstimation = true;
    $(function () {
      $("#datepicker").datepicker();
    });
    this.GetCustomerDetsFromCustComp();
    this.GetPurchaseDetsFromPurchaseComp();
    this.GetParentJson();
    this.GetPaymentSummary();
    this.getSalesMan();
    this.getApplicationdate();
  }


  confirmBeforeLeave(): boolean {

    if (this.PurchaseBilling.lstOfPayment.length == 0) {
      this.leavePage = false;
    }
    else {
      this.leavePage = true;
    }

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



  //Radio Buton change
  NewEstimation: boolean = false;
  ExistingEstimation: boolean = false;
  OrderNo: boolean = false;
  ApplicationDate: string = "";
  estDetails: any = [];
  SalesPerson: string = "";
  purchaseDetails: any = [];
  customerDtls: any = [];
  getPurchaseEstimationDetails(arg) {
    this.EnableDisablePayment = false;
    if (arg != null && arg != "0") {
      this._purchaseService.getPurchaseDetailsfromAPI(arg).subscribe(
        response => {
          this.purchaseDetails = response;
          if (this.purchaseDetails != null) {
            this.ngOnDestroy();
            this.PurchaseBilling.CompanyCode = this.ccode;
            this.PurchaseBilling.BranchCode = this.bcode;
            this.SalesPerson = this.purchaseDetails.lstPurchaseEstDetailsVM[0].SalCode;
            this.PurchaseBilling.EstNo = arg;
            if (this.model.option == "Payment") {
              this.EnableDisablePayment = true;
            }
            else {
              this.EnableDisablePayment = false;
            }
            this._estmationService.SendEstNo(arg);
            this._CustomerService.getCustomerDtls(this.purchaseDetails.CustID).subscribe(
              response => {
                this.customerDtls = response;
                this._CustomerService.sendCustomerDtls_To_Customer_Component(this.customerDtls);
                this._paymentService.inputData(this.PurchaseBilling);
              }
            )
          }
        },
        (err) => {
          if (err.status === 404) {
            const validationError = err.error;
            swal("Warning!", validationError.description, "warning");
            this.ngOnDestroy();
          }
          else {
          }
        }
      )
    }
  }

  SalesManList: any = [];
  getSalesMan() {
    this._masterService.getSalesMan().subscribe(
      response => {
        this.SalesManList = response;
      })
  }

  getApplicationdate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.ordDate = appDate["applcationDate"]
        this.ApplicationDate = formatDate(this.ordDate, 'dd/MM/yyyy', 'en_GB');
      }
    )
  }

  ParentJSON: any = [];
  GetParentJson() {
    this._paymentService.castParentJSON.subscribe(
      response => {
        this.ParentJSON = response;
      }
    )
  }
  EnableSubmitButton: boolean = true;
  NoRecordsPaymentSummary: boolean = false;
  PaymentSummary: any = [];
  PaidAmount: number = 0.00;

  GetPaymentSummary() {
    this._paymentService.CastPaymentSummaryData.subscribe(
      response => {
        this.PaymentSummary = response;
        if (this.isEmptyObject(this.PaymentSummary) == false && this.isEmptyObject(this.PaymentSummary) != null) {
          if (this.PaymentSummary.Amount == null) {
            this.NoRecordsPaymentSummary = false;
            this.EnableSubmitButton = true;
          }
          else {
            this.NoRecordsPaymentSummary = true;
            this.EnableSubmitButton = false;
            this.PaidAmount = this.PaymentSummary.Amount;
          }
        }
        else {
          this.NoRecordsPaymentSummary = false;
          this.EnableSubmitButton = true;
        }
      }
    )
  }


  //Hide Show data when accordian collapsed(Header)
  public ToggleTotal: boolean = true;
  public CollapseCustomerTab: boolean = true;
  public CollapseCustomerDetailsTab: boolean = true;

  EnableSalesTab: boolean = false;
  EnableOrderAttachmentTab: boolean = true;
  EnableSRAttachmentTab: boolean = true;
  EnableOldGoldAttachmentTab: boolean = true;
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
  ToggleCustomerData() {
    this.CollapseCustomerTab = !this.CollapseCustomerTab;
    this.CollapseCustomerDetailsTab = !this.CollapseCustomerDetailsTab;
  }

  public TogglePurchase: boolean = false;
  EnablePurchaseTab: boolean = false;
  TogglePurchaseData() {
    this.EnablePurchaseTab = !this.EnablePurchaseTab;
  }


  ngOnDestroy() {
    this.PurchaseBillNo=null;
    this._CustomerService.SendCustDataToEstComp(null);
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
    this._purchaseService.sendPurchaseDatatoEstComp(null);
    this._estmationService.SendEstNo(null);
    this._estmationService.SendOldGoldAttachmentSummaryData(null);
    this._paymentService.outputData(null);
    this._paymentService.inputData(null);
    this._paymentService.OutputParentJSONFunction(null);
    this._paymentService.SendPaymentSummaryData(null);
    this.model.option = "Payment";
    this.PurchaseBilling = {
      CompanyCode: "",
      BranchCode: "",
      EstNo: null,
      OperatorCode: localStorage.getItem('Login'),
      BillCounter: null,
      Type: "PE",
      BillNo: 0,
      CustName: null,
      PaidAmount: 0,
      CancelRemarks: null,
      lstOfPayment: []
    }
    this.ClearValues();
  }

  isEmptyObject(obj) {
    return (obj && (Object.keys(obj).length === 0));
  }

  PurchaseBilling: PurchaseBilling = {
    CompanyCode: "",
    BranchCode: "",
    EstNo: null,
    OperatorCode: localStorage.getItem('Login'),
    BillCounter: null,
    Type: "PE",
    BillNo: 0,
    CustName: null,
    PaidAmount: 0,
    CancelRemarks: null,
    lstOfPayment: []
  }


  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.PurchaseBilling.CompanyCode = this.ccode;
    this.PurchaseBilling.BranchCode = this.bcode;
  }



  EnablePurchase() {
    this.EnableSalesTab = false;
    this.EnablePurchaseTab = false;
    this.NoRecordsSales = false;
    this.TogglePurchase = false;
    this.NoRecordsPurchase = false;
  }

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
        }
        else {
          this.EnableSalesTab = true;
          this.CollapseCustomerTab = false;
          this.NoRecordsCustomer = false;
          this.NoRecordsSales = false;
          this.EnablePurchaseTab = true;
        }
      });
  }

  GetPurchaseDetsFromPurchaseComp() {
    this._purchaseService.cast.subscribe(
      response => {
        this.PurchaseData = response;
        if (this.isEmptyObject(this.PurchaseData) == false && this.isEmptyObject(this.PurchaseData) != null) {
          this.NoRecordsPurchase = true;
          this.TogglePurchase = true;
          this.EnablePurchaseTab = true;
          this.NoSummaryPurchase = true;
          this.indicator = "P";
        }
        else {
          this.NoRecordsPurchase = false;
          this.TogglePurchase = false;
          this.EnablePurchaseTab = true;
          this.NoSummaryPurchase = false;
        }
      }
    )
  }

  PurchaseTotAmount: number = 0;
  PurchaseTotalAmountCalculation(Amount) {
    this.PurchaseTotAmount = Math.round(Amount);
  }

  NoRecordsPayments: boolean = true;
  EnablePaymentsTab: boolean = true;

  TogglePaymentData() {
    // if (this.NoRecordsPayments == false) {
    //   swal("Warning", "Please select Item Details ", "warning");
    // }
    // else {
    this.EnablePaymentsTab = !this.EnablePaymentsTab;
    //}
  }
  closePurBillTab(){
    this.ngOnDestroy();
  //removed relaod from after submit and used after print close
    window.location.reload();
  }
  BalAmount: number = 0.00;

  BalanceAmount() {
    this.BalAmount = Math.round(this.PurchaseTotAmount) - this.PaidAmount;
    this.autoFetchAmount = this.BalAmount;
    return this.BalAmount;
  }

  PaidTotalAmountCalculation(Amount) {
    this.PaidAmount = Amount;
    this.PurchaseBilling.PaidAmount = this.PaidAmount;
    this.BalanceAmount();
  }

  OnRadioBtnChnge(arg) {
    if (arg == "Adjust") {
      this.EnableDisablePayment = false;
      this.model.option = arg;
      this.EnableSubmitButton = false;
      this.PurchaseBilling.lstOfPayment = [];
    }
    else {
      this.EnableDisablePayment = true;
      this.model.option = arg;
      this.PurchaseBilling.lstOfPayment = [];
      this._paymentService.inputData(this.PurchaseBilling);
    }
  }

  Submit() {
    if (this.model.option == "Payment") {
      if (this.BalAmount == 0.00 || this.BalAmount == 0) {
        this.SavePurchaseBilling();
      }
      else if (this.BalAmount < 0) {
        swal("Warning", "Paid amount Should not be greater than the balance amount", "warning");
      }
      else if (this.BalAmount > 0) {
        swal("Warning", "Please make full payment to save the bill", "warning");
      }
    }
    else {
      this.SavePurchaseBilling();
    }
  }

  //BillNo:any; commented bcz this bill no will send to reprint purchase comp to get print
  //due to some share data bw components on date 02/jan/2022 we directly implemented reprint logic with html modal here itslef
  PurchaseBillNo: any;

  PrintTypeBasedOnConfig:any;
  PrintDetails:any=[];
  SavePurchaseBilling() {
    var ans = confirm("Do you want to save??");
    if (ans) {
      if (this.PurchaseBillNo == null ) {
        this._purchaseService.postPurchaseBilling(this.PurchaseBilling).subscribe(
          response => {
            this.PurchaseBillNo = response;
            this.routerUrl = this._router.url;
            if (this.PurchaseBillNo.billNo != null && this.PurchaseBillNo.billNo != 0) {
              //if(this.model.option=="Payment"){
              this.EnableReprint = true;
              //}
              swal("Submitted!", "Bill No: " + this.PurchaseBillNo.billNo + " generated successfully!", "success");
             
              this._purchaseService.PrintPurchaseBill(this.PurchaseBillNo.billNo).subscribe(
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
              this.ngOnDestroy();
              this.leavePage = false;
              this.estNo.nativeElement.value = "";
            }
          }
        )
      }
    }
  }

  ClearValues() {
    this.ReceivableAmount = 0.00;
    this.PaidAmount = 0.00;
    this.PurchaseTotAmount = 0.00;
    this.BalAmount = 0.00;
    this.pageName = "";
    this.EnableDisablePayment = false;
    this.EnablePurchaseTab = true;
    this.SalesManList = [];
    this.getSalesMan();
    this.GetCustomerDetsFromCustComp();
    this.GetPurchaseDetsFromPurchaseComp();
    this.GetParentJson();
    this.GetPaymentSummary();
    this.getApplicationdate();
    this.getCB();
    this.pageName = "OC";
    this.SalesPerson = "";
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
}