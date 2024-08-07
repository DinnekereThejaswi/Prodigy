import { estimationService } from './../estimation/estimation.service';
import { SalesBillingService } from './sales-billing.service';
import { PurchaseService } from './../purchase/purchase.service';
import { SalesService } from './../sales/sales.service';
import { SalesBilling } from './sales-billing.model';
import { CustomerService } from './../masters/customer/customer.service';
import { Component, OnInit, OnDestroy, Output, EventEmitter, OnChanges, ElementRef, ViewChild, AfterContentChecked, } from '@angular/core';
import { formatDate } from '@angular/common';
import { Router } from '@angular/router';
import { AddBarcodeService } from './../sales/add-barcode/add-barcode.service';
import { MasterService } from './../core/common/master.service';
import * as CryptoJS from 'crypto-js';
import { PaymentService } from './../payment/payment.service';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { OrdersService } from './../orders/orders.service';
import { ToastrService } from 'ngx-toastr';
import { ComponentCanDeactivate } from './../appconfirmation-guard';
import { AppConfigService } from '../AppConfigService';
import swal from 'sweetalert';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgxSpinnerService } from "ngx-spinner";

declare var $: any;

@Component({
  selector: 'app-sales-billing',
  templateUrl: './sales-billing.component.html',
  styleUrls: ['./sales-billing.component.css']
})

export class SalesBillingComponent implements OnInit, ComponentCanDeactivate {
  leavePage: boolean = false;
  @ViewChild("RecAmt", { static: false }) RecAmt: ElementRef;
  @ViewChild("PayAmt", { static: false }) PayAmt: ElementRef;
  @ViewChild("estNo", { static: false }) estNo: ElementRef;
  PaymentModes: any;
  EnableSubmitButton: boolean = true;
  filterRadioBtns: boolean = false;
  SalesBillingForm: FormGroup;
  RecAmtEnable: boolean = true;
  PayAmtEnable: boolean = true;
  GuarantorForm: FormGroup;
  RowRevision: any;
  CurrentRowRevision: any;
  routerUrl: string = "";
  CustomerName: any;
  outstandinAmtForm: FormGroup;
  indicator: string;
  PaymentHeader: string = "";
  GuaranteedatePickerConfig: Partial<BsDatepickerConfig>;
  SalesData: any = {};
  EnableReprint: boolean = false;
  EnableDisablePaymentForOrderAdj: boolean = false;
  OrderNo: string = "";
  PurchaseData: any = {};
  OrderAttachmentSummaryData: any = {};
  SRAttachmentSummaryData: any = {};
  OldGoldAttachmentSummaryData: any = {};
  EnablePayToCust: boolean = false;
  pageName: string;
  NoRecordsCustomer: boolean = false;
  NoRecordsPurchase: boolean = false;
  EnableDisablePayment: boolean = false;
  EnableDiscountItemTab: boolean = true;
  EnableJson: boolean = false;
  timeLeft: number = 120;
  interval;
  EstNo: string;
  radioItems: Array<string>;
  model = { option: 'Payment' };
  ccode: string = "";
  bcode: string = "";
  today = new Date();
  estDate = '';
  ordDate = '';
  autoFetchAmount: number = 0;
  password: string;
  autoFetchDisAmount: number = 0;
  PaymentForm: FormGroup;
  rowRevisionFlag: boolean = false;
  datePickerConfig: Partial<BsDatepickerConfig>;
  constructor(private _CustomerService: CustomerService, private _salesService: SalesService,
    private _estmationService: estimationService, private _purchaseService: PurchaseService,
    private _addBarcodeService: AddBarcodeService, private _masterService: MasterService,
    private _router: Router, private _paymentService: PaymentService, private _salesBillingService: SalesBillingService,
    private _ordersService: OrdersService, private toastr: ToastrService, private _appConfigService: AppConfigService,
    private formBuilder: FormBuilder, private SpinnerService: NgxSpinnerService) {
    this.estDate = formatDate(this.today, 'dd/MM/yyyy', 'en-US', '+0530');
    this.ordDate = formatDate(this.today, 'dd/MM/yyyy', 'en-US', '+0530');
    this.radioItems = ['Payment', 'Order'];
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        minDate: this.today,
        dateInputFormat: 'YYYY-MM-DD'
      });

    this.GuaranteedatePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        minDate: this.today,
        dateInputFormat: 'DD/MM/YYYY'
      });

    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.rowRevisionFlag = this._appConfigService.RowRevisionBilling;
    this.getCB();
    this.EnableSalesTab = true;
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

  ngOnInit() {

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

    this.SalesBillingForm = this.formBuilder.group({
      BillType: ["", Validators.required],
    });

    this.outstandinAmtForm = this.formBuilder.group({
      OSAmount: null
    });


    this.NewEstimation = true;
    $(function () {
      $("#datepicker").datepicker();
    });
    this.getBillTypes();
    this._estmationService.SendEstNo(null);
    this.GetCustomerDetsFromCustComp();
    this.GetSalesDetsFromSalescomp();
    this.GetPurchaseDetsFromPurchaseComp();
    this.GetEstNoToggleSales();
    this.GetSRAttachedSummary();
    this.GetOrderAttachedSummary();
    this.GetOldGoldAttachedSummary();
    this.getApplicationdate();
    this.GetParentJson();
    this.GetPaymentSummary();
    $('#UpdateOTP').modal('hide');
    this.PaymentForm = this.formBuilder.group({
      ObjID: null,
      CompanyCode: null,
      BranchCode: null,
      SeriesNo: 0,
      ReceiptNo: 0,
      SNo: 0,
      TransType: null,
      PayMode: null,
      PayDetails: null,
      PayDate: null,
      PayAmount: null,
      RefBillNo: null,
      PartyCode: null,
      BillCounter: null,
      IsPaid: null,
      Bank: null,
      BankName: null,
      ChequeDate: null,
      CardType: null,
      ExpiryDate: null,
      CFlag: null,
      CardAppNo: null,
      SchemeCode: null,
      SalBillType: null,
      OperatorCode: null,
      SessionNo: null,
      UpdateOn: null,
      GroupCode: null,
      AmtReceived: null,
      BonusAmt: null,
      WinAmt: null,
      CTBranch: null,
      FinYear: null,
      CardCharges: null,
      ChequeNo: null,
      NewBillNo: null,
      AddDisc: null,
      IsOrderManual: null,
      CurrencyValue: null,
      ExchangeRate: null,
      CurrencyType: null,
      TaxPercentage: null,
      CancelledBy: null,
      CancelledRemarks: null,
      CancelledDate: null,
      IsExchange: null,
      ExchangeNo: null,
      NewReceiptNo: null,
      GiftAmount: null,
      CardSwipedBy: null,
      Version: null,
      GSTGroupCode: null,
      SGSTPercent: null,
      CGSTPercent: null,
      IGSTPercent: null,
      HSN: null,
      SGSTAmount: null,
      CGSTAmount: null,
      IGSTAmount: null,
      PayAmountBeforeTax: null,
      PayTaxAmount: null,
      IsCashMarking: null,
      ReceivedCash: null,
      AdditionalDiscount: null
    });
    this.GuarantorForm = this.formBuilder.group({
      name: null,
      date: null
    });

  }


  //Radio Buton change
  NewEstimation: boolean = false;
  ExistingEstimation: boolean = false;
  ApplicationDate: string = "";
  getApplicationdate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.ordDate = appDate["applcationDate"]
        this.ApplicationDate = this.ordDate;
      }
    )
  }

  PaymentHeaders: any = {
    ObjID: null,
    CompanyCode: null,
    BranchCode: null,
    SeriesNo: null,
    ReceiptNo: null,
    SNo: null,
    TransType: null,
    PayMode: null,
    PayDetails: null,
    PayDate: null,
    PayAmount: null,
    RefBillNo: null,
    PartyCode: null,
    BillCounter: null,
    IsPaid: null,
    Bank: null,
    BankName: null,
    ChequeDate: null,
    CardType: null,
    ExpiryDate: null,
    CFlag: null,
    CardAppNo: null,
    SchemeCode: null,
    SalBillType: null,
    OperatorCode: null,
    SessionNo: null,
    UpdateOn: null,
    GroupCode: null,
    AmtReceived: null,
    BonusAmt: null,
    WinAmt: null,
    CTBranch: null,
    FinYear: null,
    CardCharges: null,
    ChequeNo: null,
    NewBillNo: null,
    AddDisc: null,
    IsOrderManual: null,
    CurrencyValue: null,
    ExchangeRate: null,
    CurrencyType: null,
    TaxPercentage: null,
    CancelledBy: null,
    CancelledRemarks: null,
    CancelledDate: null,
    IsExchange: null,
    ExchangeNo: null,
    NewReceiptNo: null,
    GiftAmount: null,
    CardSwipedBy: null,
    Version: null,
    GSTGroupCode: null,
    SGSTPercent: null,
    CGSTPercent: null,
    IGSTPercent: null,
    HSN: null,
    SGSTAmount: null,
    CGSTAmount: null,
    IGSTAmount: null,
    PayAmountBeforeTax: null,
    PayTaxAmount: null,
    IsCashMarking: null,
    ReceivedCash: null,
    AdditionalDiscount: null
  }

  toggle: string = "Invalid";

  billtype: any = [];

  getBillTypes() {
    this._salesBillingService.getBillType().subscribe(
      response => {
        this.billtype = response;
        this.SalesBilling.BillType = "N";
      }
    )
  }

  SetBillType(arg) {
    this.SalesBilling.BillType = arg;
  }


  onchangePaymode(paymodeArg) {
    this.PaymentHeaders.PayDetails = null;
    this.PaymentHeaders.PayAmount = null;
    this.PaymentHeaders.BankName = null;
    this.PaymentHeaders.RefBillNo = null;
    this.PaymentHeaders.CardType = null;
    this.PaymentHeaders.Bank = null;
    this.PaymentAmount();
    switch (paymodeArg) {
      case "Q": {
        this.toggle = "Cheque";
        this.getChequeName();
        break;
      }
      case "C": {
        this.toggle = "Cash";
        break;
      }
      case "EP": {
        this.toggle = "EP";
        break;
      }
      default: {
        this.toggle = "Invalid";
        break;
      }
    }
  }

  ChequeName: any = [];
  getChequeName() {
    this._paymentService.getChequeName().subscribe(
      response => {
        this.ChequeName = response
      }
    )
  }

  PaymentAmount() {
    if (this.autoFetchAmount != 0) {
      this.PaymentHeaders.PayAmount = this.autoFetchAmount;
    }
    if (this.autoFetchDisAmount != 0) {
      this.PaymentHeaders.PayAmount = this.autoFetchDisAmount;
    }
  }


  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.SalesBilling.CompanyCode = this.ccode;
    this.SalesBilling.BranchCode = this.bcode;
  }

  getPaymentMode() {
    this._paymentService.getPaymentMode("OC").subscribe(
      response => {
        this.PaymentModes = response;
      }
    )
  }

  // close() {
  //   $('#UpdateOTP').modal('hide');
  //   this.OTPData = null;
  // }

  estDetails: any = [];
  purchaseDetails: any = [];
  GlobalOrderAmount: Number = 0;
  getEstimationDetails(arg) {
    //Repetitive Code
    this.RowRevision = null;
    this.autoFetchDisAmount = 0;
    this.pageName = "";
    this.PaymentHeader = "";
    this.toggle = "Invalid"
    this.PaymentForm.reset();
    //this.ClearValues();
    this.ngOnDestroy();
    this.model.option = "Payment";
    this.SalesBilling.IsCreditNote = false;
    this.SalesBilling.IsOrder = false;
    this.EnablePayToCust = false;
    this.getPaymentMode();
    this.SalesBilling.BillType = "N";
    this.ArrayList = [];
    this.BillNo = null;
    //ends
    this._salesService.SaveSalesEstNo(arg);
    this._estmationService.getEstimationDetailsfromAPI(arg).subscribe(
      response => {
        this.estDetails = response;
        if (this.estDetails != null && this.estDetails.salesEstimatonVM.length > 0) {
          if (this.rowRevisionFlag == true) {
            this._salesService.getRowVersion(arg).subscribe(
              response => {
                this.RowRevision = response;
              }
            )
          }
          if (this.estDetails.offerDiscount != null) {
            this.offerDiscount = this.estDetails.offerDiscount.DiscountAmount;
          }
          else {
            this.offerDiscount = 0.00;
          }
          //this.EnablePaymentsTab = true;
          //this.EnableDisablePaymentForOrderAdj = true;
          this.EnableSubmitButton = false;
          this.OrderNo = this.estDetails.OrderNo;
          this.NoSummaryOrder = true;
          this.GlobalOrderAmount = this.estDetails.OrderAmount;
          this.OrderTotalAmountCalculation(0);
          this.EnableDisablePayment = true;
          this.EstNo = this.estDetails.EstNo;
          this.SalesBilling.EstNo = this.EstNo;
          this.SalesBilling.OperatorCode = localStorage.getItem('Login');
          this.SalesBilling.CompanyCode = this.ccode;
          this.SalesBilling.BranchCode = this.bcode;
          this._paymentService.inputData(this.SalesBilling);
          //this.FinalAmount(); //Commented on 05-04-2021 for optimization
          this._estmationService.SendEstNo(arg);
          this.estDate = formatDate(this.estDetails.EstDate, 'dd/MM/yyyy', 'en-US', '+0530');
          this.ordDate = formatDate(this.estDetails.OrderDate, 'dd/MM/yyyy', 'en-US', '+0530');
          this._salesService.salesDetails(this.estDetails)
          this.EnableSalesTab = true;
          //this.ToggleSalesData();
          this.EnableDiscountItemTab = false;
          this._CustomerService.getCustomerDtls(this.estDetails.CustID).subscribe(
            response => {
              const customerDtls = response;
              this.ToggleSales = true;
              this._CustomerService.sendCustomerDtls_To_Customer_Component(customerDtls);
              this.leavePage = false;
              //Commented on 05-04-2021 for optimization
              // if (this.BalanceAmount() > 0) {
              //   this.RecAmt.nativeElement.focus();
              // }
              // else if (this.BalanceAmount() < 0) {
              //   this.PayAmt.nativeElement.focus();
              // }
              //End on 05-04-2021 for optimization
              //Added on 05-04-2021 for optimization
              if (this.SalesBilling.BalanceAmt > 0) {
                this.RecAmt.nativeElement.focus();
              }
              else if (this.SalesBilling.BalanceAmt < 0) {
                this.PayAmt.nativeElement.focus();
              }
              //End on 05-04-2021 for optimization
            }
          )
        }
      },
      (err) => {
        this.EnableSubmitButton = true;
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


  BillNo: any;
  OTPData: any;

  OnRadioBtnChnge(arg) {
    if (arg == "Order") {
      this.EnableShopKeeperTab = true;
      this.EnablePaymentsTab = true;
      this.SalesBilling.IsOrder = true;
      this.model.option = arg;
      this.PaymentForm.reset();
      this.toggle = "Invalid";
    }
    else {
      if (this.SalesBilling.lstOfPayment.length > 0) {
        this.EnableDisablePaymentForOrderAdj = true;
      }
      else {
        this.EnableDisablePaymentForOrderAdj = false;
      }

      this.SalesBilling.IsOrder = false;
      this.EnableDisablePayment = true;
      this.model.option = arg;
      this.EnableShopKeeperTab = false;
      this._paymentService.inputData(this.SalesBilling);
    }
  }

  SaveGuarantor() {
    this.SalesBilling.DueDate = formatDate(this.ApplicationDate, 'yyyy-MM-dd', 'en-US');
    if (this.SalesBilling.GuaranteeName == null || this.SalesBilling.GuaranteeName == "") {
      swal("Warning!", 'Please enter the GuaranteeName', "warning");
    }
    else if (this.SalesBilling.DueDate == null || this.SalesBilling.DueDate == "") {
      swal("Warning!", 'Please enter the DueDate', "warning");
    }
    else {
      $('#GuaranteePopup').modal('hide');
      this.SalesBilling.IsCreditNote = true;
      this.GuaranteeFlag = true;
    }
  }

  PayabaleAmt: number = 0;
  GuaranteeFlag: boolean = false;
  AttachedInfoBill: any = [];
  purchaseBillNo: any = [];

  Submit() {
    if (this.SalesBilling.CustID == 0) {
      swal("Warning!", 'Please select the customer', "warning");
    }
    else {
      if (this.BillNo == null) {
        if (this.model.option == "Order") {
          var ans = confirm("Do you want to save??");
          if (ans) {
            if (this.rowRevisionFlag == true) {
              this._salesService.getRowVersion(this.SalesBilling.EstNo).subscribe(
                response => {
                  this.CurrentRowRevision = response;
                  if (this.CurrentRowRevision.RowRevisionString == this.RowRevision.RowRevisionString) {
                    this.SpinnerService.show();
                    this._salesBillingService.postSalesBilling(this.SalesBilling).subscribe(
                      response => {
                        this.BillNo = response;
                        if (this.BillNo != null) {
                          this.EnableReprint = true;
                          this.routerUrl = this._router.url;
                          //swal("Submitted!", "Bill No: " + this.BillNo.billNo + " generated successfully!", "success");
                          swal("Submitted!", "New Order No: " + this.BillNo.orderNo + " and Receipt No: " + this.BillNo.receiptNo + " generated successfully!", "success");
                          this.ngOnDestroy();
                          this.GuaranteeFlag = false;
                          this.RndOffAmtFlag = false;
                          this.leavePage = false;
                          this.EnablePayToCust = false;
                          this.toggle = "Invalid"
                          this.PaymentForm.reset();
                          this.EnableSubmitButton = true;
                          this.SpinnerService.hide();
                          this.getAttachedBillForSalesBilling();
                        }
                      },
                      (err) => {
                        if (err.status === 403) {
                          const validationError = err.error;
                          //swal("Warning!", validationError.description, "warning");
                          this.OTPData = validationError.GenObject;
                          this.timeLeft = 120;
                          $('#UpdateOTP').modal('show');
                          this.startTimer();
                        }
                        else if (err.status === 400) {
                          const validationError = err.error;
                          swal("Warning!", validationError.description, "warning");
                        }
                        this.SpinnerService.hide();
                      }
                    )
                  }
                  else {
                    swal("Warning!", "Someone altered the estimation.Kindly re-load the estimation", "warning");
                    this.CurrentRowRevision = null;
                  }
                })
            }
            else {
              this.SpinnerService.show();
              this._salesBillingService.postSalesBilling(this.SalesBilling).subscribe(
                response => {
                  this.BillNo = response;
                  if (this.BillNo != null) {
                    this.EnableReprint = true;
                    this.routerUrl = this._router.url;
                    //swal("Submitted!", "Bill No: " + this.BillNo.billNo + " generated successfully!", "success");
                    swal("Submitted!", "New Order No: " + this.BillNo.orderNo + " and Receipt No: " + this.BillNo.receiptNo + " generated successfully!", "success");
                    this.ngOnDestroy();
                    this.GuaranteeFlag = false;
                    this.RndOffAmtFlag = false;
                    this.leavePage = false;
                    this.EnablePayToCust = false;
                    this.toggle = "Invalid"
                    this.PaymentForm.reset();
                    this.EnableSubmitButton = true;
                    this.SpinnerService.hide();
                    this.getAttachedBillForSalesBilling();
                  }
                },
                (err) => {
                  if (err.status === 403) {
                    const validationError = err.error;
                    //swal("Warning!", validationError.description, "warning");
                    this.OTPData = validationError.GenObject;
                    this.timeLeft = 120;
                    $('#UpdateOTP').modal('show');
                    this.startTimer();
                  }
                  else if (err.status === 400) {
                    const validationError = err.error;
                    swal("Warning!", validationError.description, "warning");
                  }
                  this.SpinnerService.hide();
                }
              )
            }
          }
        }
        else {
          if (this.GuaranteeFlag == true) {
            var ans = confirm("Do you want to save??");
            if (ans) {
              if (this.rowRevisionFlag == true) {
                this._salesService.getRowVersion(this.SalesBilling.EstNo).subscribe(
                  response => {
                    this.CurrentRowRevision = response;
                    if (this.CurrentRowRevision.RowRevisionString == this.RowRevision.RowRevisionString) {
                      this.SpinnerService.show();
                      this._salesBillingService.postSalesBilling(this.SalesBilling).subscribe(
                        response => {
                          this.BillNo = response;
                          if (this.BillNo != null) {
                            this.EnableReprint = true;
                            this.routerUrl = this._router.url;
                            swal("Submitted!", "Bill No: " + this.BillNo.billNo + " generated successfully!", "success");
                            this.ngOnDestroy();
                            this.EnablePayToCust = false;
                            this.toggle = "Invalid"
                            this.ArrayList = [];
                            this.PaymentForm.reset();
                            this.GuaranteeFlag = false;
                            this.RndOffAmtFlag = false;
                            this.leavePage = false;
                            this.EnableSubmitButton = true;
                            this.SpinnerService.hide();
                            this.getAttachedBillForSalesBilling();
                          }
                        },
                        (err) => {
                          if (err.status === 403) {
                            const validationError = err.error;
                            //swal("Warning!", validationError.description, "warning");
                            this.OTPData = validationError.GenObject;
                            this.timeLeft = 120;
                            $('#UpdateOTP').modal('show');
                            this.startTimer();
                          }
                          else if (err.status === 400) {
                            const validationError = err.error;
                            swal("Warning!", validationError.description, "warning");
                          }
                          this.SpinnerService.hide();
                        }
                      )
                    }
                    else {
                      swal("Warning!", "Someone altered the estimation.Kindly re-load the estimation", "warning");
                      this.CurrentRowRevision = null;
                    }
                  }
                )
              }
              else {
                this.SpinnerService.show();
                this._salesBillingService.postSalesBilling(this.SalesBilling).subscribe(
                  response => {
                    this.BillNo = response;
                    if (this.BillNo != null) {
                      this.EnableReprint = true;
                      this.routerUrl = this._router.url;
                      swal("Submitted!", "Bill No: " + this.BillNo.billNo + " generated successfully!", "success");
                      this.ngOnDestroy();
                      this.EnablePayToCust = false;
                      this.toggle = "Invalid"
                      this.ArrayList = [];
                      this.PaymentForm.reset();
                      this.GuaranteeFlag = false;
                      this.RndOffAmtFlag = false;
                      this.leavePage = false;
                      this.EnableSubmitButton = true;
                      this.SpinnerService.hide();
                      this.getAttachedBillForSalesBilling();
                    }
                  },
                  (err) => {
                    if (err.status === 403) {
                      const validationError = err.error;
                      //swal("Warning!", validationError.description, "warning");
                      this.OTPData = validationError.GenObject;
                      this.timeLeft = 120;
                      $('#UpdateOTP').modal('show');
                      this.startTimer();
                    }
                    else if (err.status === 400) {
                      const validationError = err.error;
                      swal("Warning!", validationError.description, "warning");
                    }
                    this.SpinnerService.hide();
                  }
                )
              }
            }
          }
          else {
            var bAmt;
            if (this.RndOffAmtFlag == true) {
              //bAmt = this.SalesAmountInclTax - this.PurchaseTotAmount;
              bAmt = this.SalesAmountExclTax + this.GSTAmount - this.PurchaseTotAmount;
              bAmt = Math.round(bAmt);
            }
            else {
              // bAmt = this.TotAmount; //Commented on 10-Nov-2020
              bAmt = Math.round(this.TotAmount); //Added on 10-Nov-2020
            }

            //this.PayableAmount(); //Commented on 05-04-2021 for optimization
            //this.ReceivedAmount(); //Commented on 05-04-2021 for optimization
            if (Number(this.RecAmount) < Number(bAmt) && this.BalAmount != 0 && this.PaymentHeader != 'Payments') {
              var ans = confirm("Received amount is less than the sales amount. Do you want to generate Credit Bill??");
              if (ans) {
                $('#GuaranteePopup').modal('show');
                this.GuaranteeFlag = true;
              }
            }
            else if (Number(this.RecAmount) > Number(bAmt) && this.BalAmount != 0 && this.PaymentHeader != 'Payments') {
              swal("Warning!", 'Payment amount is greater than bill amount', "warning");
              //this.BalanceAmount(); //Commented on 05-04-2021 for optimization
              this.EnablePaymentsTab = true;
              // this.EnableShopKeeperTab = false;
            }
            // else if (this.PayAmount != 0.00 && this.PaymentHeader == 'Payments') {
            //   this.toastr.warning('Please enter valid payment amount', 'Alert!');
            // }
            else if (this.PayAmount > 0.00 && this.PaymentHeader == 'Payments') {
              swal("Warning!", 'Payable Amount is less than Balance Amount', "warning");
            }
            else if (this.PayAmount < 0.00 && this.PaymentHeader == 'Payments') {
              swal("Warning!", 'Payable Amount is greater than Balance Amount', "warning");
            }
            else {
              var ans = confirm("Do you want to save??");
              if (ans) {
                if (this.rowRevisionFlag == true) {
                  this._salesService.getRowVersion(this.SalesBilling.EstNo).subscribe(
                    response => {
                      this.CurrentRowRevision = response;
                      if (this.CurrentRowRevision.RowRevisionString == this.RowRevision.RowRevisionString) {
                        this.SpinnerService.show();
                        this._salesBillingService.postSalesBilling(this.SalesBilling).subscribe(
                          response => {
                            this.BillNo = response;
                            if (this.BillNo != null) {
                              this.routerUrl = this._router.url;
                              this.EnableReprint = true;
                              swal("Submitted!", "Bill No: " + this.BillNo.billNo + " generated successfully!", "success");
                              this.ngOnDestroy();
                              this.toggle = "Invalid"
                              this.ArrayList = [];
                              this.PaymentForm.reset();
                              this.GuaranteeFlag = false;
                              this.RndOffAmtFlag = false;
                              this.leavePage = false;
                              this.EnableSubmitButton = true;
                              this.SpinnerService.hide();
                              this.getAttachedBillForSalesBilling();
                              this.EnablePayToCust = false;
                            }
                          },
                          (err) => {
                            if (err.status === 403) {
                              const validationError = err.error;
                              // swal("Warning!", validationError.description, "warning");
                              this.OTPData = validationError.GenObject;
                              this.timeLeft = 120;
                              $('#UpdateOTP').modal('show');
                              this.startTimer();
                            }
                            else if (err.status === 400) {
                              const validationError = err.error;
                              swal("Warning!", validationError.description, "warning");
                            }
                            this.SpinnerService.hide();
                          }
                        )
                      }
                      else {
                        swal("Warning!", "Someone altered the estimation.Kindly re-load the estimation", "warning");
                        this.CurrentRowRevision = null;
                      }
                    }
                  )
                }
                else {
                  this.SpinnerService.show();
                  this._salesBillingService.postSalesBilling(this.SalesBilling).subscribe(
                    response => {
                      this.BillNo = response;
                      if (this.BillNo != null) {
                        this.routerUrl = this._router.url;
                        this.EnableReprint = true;
                        swal("Submitted!", "Bill No: " + this.BillNo.billNo + " generated successfully!", "success");
                        this.ngOnDestroy();
                        this.toggle = "Invalid"
                        this.ArrayList = [];
                        this.PaymentForm.reset();
                        this.GuaranteeFlag = false;
                        this.RndOffAmtFlag = false;
                        this.leavePage = false;
                        this.EnableSubmitButton = true;
                        this.SpinnerService.hide();
                        this.getAttachedBillForSalesBilling();
                        this.EnablePayToCust = false;
                      }
                    },
                    (err) => {
                      if (err.status === 403) {
                        const validationError = err.error;
                        // swal("Warning!", validationError.description, "warning");
                        this.OTPData = validationError.GenObject;
                        this.timeLeft = 120;
                        $('#UpdateOTP').modal('show');
                        this.startTimer();
                      }
                      else if (err.status === 400) {
                        const validationError = err.error;
                        swal("Warning!", validationError.description, "warning");
                      }
                      this.SpinnerService.hide();
                    }
                  )
                }
              }
            }
          }
        }
      }
    }
  }


  cancelModal() {
    $('#GuaranteePopup').modal('hide');
    this.GuaranteeFlag = false;
    this.RecAmt.nativeElement.focus();
  }

  payAmt: number = 0;
  Flag: boolean = false;

  // ValidateRowRevision(arg) {
  //   this._salesService.getRowVersion(arg).subscribe(
  //     response => {
  //       this.CurrentRowRevision = response;
  //       if (this.CurrentRowRevision.RowRevisionString.substring(0, this.CurrentRowRevision.RowRevisionString.length - 1)
  //         === this.RowRevision.RowRevisionString.substring(0, this.RowRevision.RowRevisionString.length - 1)
  //       ) {
  //         this.Flag = false;
  //       }
  //       else {
  //         this.Flag = true;
  //       }
  //     }
  //   )
  //   return this.Flag;
  // }

  paymentAmt(arg) {
    if (arg < Number(this.SalesBilling.PayableAmt) * -1) {
      swal("Warning!", "Invalid discount amount. Discount should not be negative", "warning");
      this.DiscountApplied = false;
      this.PayAmtEnable = false;
    }
    else {
      if (this.rowRevisionFlag == true) {
        this._salesService.getRowVersion(this.SalesBilling.EstNo).subscribe(
          response => {
            this.CurrentRowRevision = response;
            if (this.CurrentRowRevision.RowRevisionString == this.RowRevision.RowRevisionString) {
              this.PayableFinalAmt = arg;
              if (this.differenceDiscountAmt > 0) {
                this.EnablePaymentsTab = true;
                //this.EnableShopKeeperTab = false;
              }
              else {
                this.EnablePaymentsTab = false;
                //this.EnableShopKeeperTab = true;
              }
              this.EnableShopKeeperTab = false;
              this.differenceDiscountAmt += arg - this.diffDisAmtForCalc * -1;
              this._salesBillingService.postdifferenceDiscountAmt(this.SalesBilling.EstNo, this.differenceDiscountAmt, this.ccode, this.bcode).subscribe(
                response => {
                  this.Recalculation = response;
                  if (this.Recalculation != null) {
                    this.SalesBilling.DifferenceDiscountAmt = Number(this.differenceDiscountAmt.toFixed(2));
                    this.RndOffAmtFlag = true;
                    this.DiscPer = this.Recalculation.DiscountPercent;
                    this.AddDiscAmt = this.Recalculation.DiscountAmount;
                    this.CessAmt = this.Recalculation.GSTCessAmount;
                    this.SalesAmountExclTax = this.Recalculation.SalesAmountExclTax;
                    this.GSTAmount = this.Recalculation.GSTAmount;
                    this.SalesAmountInclTax = this.Recalculation.SalesAmountInclTax;
                    //this.DisTotalAmount();
                    //this.autoFetchDisAmount = Number(this.TotAmount * -1); //Commented on 02-Mar-2021
                    this.autoFetchDisAmount = this.PayableFinalAmt;
                    this.PaymentHeaders.PayAmount = this.autoFetchDisAmount;
                    this.SalesBilling.salesInvoiceAttribute = this.Recalculation;
                    this.autoFetchAmount = 0;
                    this.DiscountApplied = true;
                    this.leavePage = true;
                  }
                },
                (err) => {
                  this.DiscountApplied = false;
                  this.PayAmtEnable = false;
                  this.PayAmt.nativeElement.focus();
                  //Added on 30-Mar-2021 due to Payment Discount Amount getting disable after operator discount error from api
                  if (this.ReceivedFinalAmt > 0) {
                    this.differenceDiscountAmt = Number(this.SalesBilling.DifferenceDiscountAmt);
                    this.EnablePaymentsTab = true;
                  }
                  else {
                    this.differenceDiscountAmt = 0;
                  }
                  //ends here
                }
              )
            }
            else {
              swal("Warning!", "Someone altered the estimation.Kindly re-load the estimation", "warning");
              this.CurrentRowRevision = null;
            }
          })
      }
      else {
        this.PayableFinalAmt = arg;
        if (this.differenceDiscountAmt > 0) {
          this.EnablePaymentsTab = true;
          //this.EnableShopKeeperTab = false;
        }
        else {
          this.EnablePaymentsTab = false;
          //this.EnableShopKeeperTab = true;
        }
        this.EnableShopKeeperTab = false;
        this.differenceDiscountAmt += arg - this.diffDisAmtForCalc * -1;
        this._salesBillingService.postdifferenceDiscountAmt(this.SalesBilling.EstNo, this.differenceDiscountAmt, this.ccode, this.bcode).subscribe(
          response => {
            this.Recalculation = response;
            if (this.Recalculation != null) {
              this.SalesBilling.DifferenceDiscountAmt = Number(this.differenceDiscountAmt.toFixed(2));
              this.RndOffAmtFlag = true;
              this.DiscPer = this.Recalculation.DiscountPercent;
              this.AddDiscAmt = this.Recalculation.DiscountAmount;
              this.CessAmt = this.Recalculation.GSTCessAmount;
              this.SalesAmountExclTax = this.Recalculation.SalesAmountExclTax;
              this.GSTAmount = this.Recalculation.GSTAmount;
              this.SalesAmountInclTax = this.Recalculation.SalesAmountInclTax;
              //this.DisTotalAmount();
              //this.autoFetchDisAmount = Number(this.TotAmount * -1); //Commented on 02-Mar-2021
              this.autoFetchDisAmount = this.PayableFinalAmt;
              this.PaymentHeaders.PayAmount = this.autoFetchDisAmount;
              this.SalesBilling.salesInvoiceAttribute = this.Recalculation;
              this.autoFetchAmount = 0;
              this.DiscountApplied = true;
              this.leavePage = true;
            }
          },
          (err) => {
            this.DiscountApplied = false;
            this.PayAmtEnable = false;
            this.PayAmt.nativeElement.focus();
            //Added on 30-Mar-2021 due to Payment Discount Amount getting disable after operator discount error from api
            if (this.ReceivedFinalAmt > 0) {
              this.differenceDiscountAmt = Number(this.SalesBilling.DifferenceDiscountAmt);
              this.EnablePaymentsTab = true;
            }
            else {
              this.differenceDiscountAmt = 0;
            }
            //ends here
          }
        )
      }
    }
  }


  getAttachedBillForSalesBilling() {
    this.estNo.nativeElement.value = "";
    this._salesBillingService.getAttachedInfoBill(this.BillNo.billNo).subscribe(
      response => {
        this.AttachedInfoBill = response;
        // if (this.AttachedInfoBill.Purchase.length > 0) {
        //   this.purchaseBillNo = this.AttachedInfoBill.Purchase;
        // }
        // else {
        //   this.purchaseBillNo = 0;
        // }
      }
    )
  }

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
            //this.EnableSubmitButton = true;
          }
          else {
            this.NoRecordsPaymentSummary = true;
            //this.EnableSubmitButton = false;
            //this.PaidAmount = this.PaymentSummary.Amount;
            this.leavePage = true;
          }
        }
        else {
          this.NoRecordsPaymentSummary = false;
          //this.EnableSubmitButton = true;
        }
      }
    )
  }

  updateOTP(arg, index, otp) {
    this._salesBillingService.ValidateOTP(arg.MobileNo, arg.OrderNo, arg.SMSID, otp).subscribe(
      response => {
        this.OTPData[index].IsValidated = true;
        this.SalesBilling.OrderOTPAttributes.push(this.OTPData[index]);
      },
      (err) => {
        this.OTPData[index].IsValidated = false;
      }
    )
  }

  startTimer() {
    this.interval = setInterval(() => {
      if (this.timeLeft > 0) {
        this.timeLeft--;
      } else {
        this.timeLeft = 120;
      }
    }, 1000)
  }

  //Hide Show data when accordian collapsed(Header)
  public ToggleTotal: boolean = true;
  public CollapseCustomerTab: boolean = true;
  public CollapseCustomerDetailsTab: boolean = true;

  EnableSalesTab: boolean = true;
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
    if (this.NoSummarySales == true) {
      this.CollapseCustomerDetailsTab = !this.CollapseCustomerDetailsTab;
    }
  }

  //Hide Show data when accordian collapsed(Sales)
  public ToggleSales: boolean = false;
  ToggleSalesData() {
    //if (this.NoRecordsCustomer == true) {
    if (this.NoSummarySales == true) {
      this.EnableSalesTab = !this.EnableSalesTab;
    }
    //this.filterRadioBtns = true;
    // if (this.EnableSalesTab == true) {
    //   this.NoRecordsSales = true;
    //   this.ToggleSales = true;
    // }
    //}
  }

  public TogglePurchase: boolean = false;
  EnablePurchaseTab: boolean = false;
  TogglePurchaseData() {
    // if (this.NoRecordsSales == true) {
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
    // }
    if (this.NoSummarySales == true) {
      this.EnablePurchaseTab = !this.EnablePurchaseTab;
    }
  }

  ToggleOrderAttachment() {
    // this._salesService.cast.subscribe(
    //   response => {
    //     this.SalesData = response;
    //     if (this.isEmptyObject(this.SalesData) == false && this.isEmptyObject(this.SalesData) != null) {
    //       this.filterRadioBtns = true;
    //       this.EnableOrderAttachmentTab = !this.EnableOrderAttachmentTab;
    //       this._estmationService.CastOrderAttachmentSummaryData.subscribe(
    //         response => {
    //           this.OrderAttachmentSummaryData = response;
    //           if (this.isEmptyObject(this.OrderAttachmentSummaryData) == false && this.isEmptyObject(this.OrderAttachmentSummaryData) != null) {
    //             if (this.EnableOrderAttachmentTab == true) {
    //               this.NoRecordsAttachOrder = true;
    //             }
    //             else {
    //               this.NoRecordsAttachOrder = false;
    //             }
    //           }
    //           else {
    //             this.NoRecordsAttachOrder = false;
    //           }
    //         }
    //       )
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
    //       this._estmationService.CastSRAttachmentSummaryData.subscribe(
    //         response => {
    //           this.SRAttachmentSummaryData = response;
    //           if (this.isEmptyObject(this.SRAttachmentSummaryData) == false && this.isEmptyObject(this.SRAttachmentSummaryData) != null) {
    //             if (this.EnableSRAttachmentTab == true) {
    //               this.NoRecordsAttachSR = true;
    //             }
    //             else {
    //               this.NoRecordsAttachSR = false;
    //             }
    //           }
    //           else {
    //             this.NoRecordsAttachSR = false;
    //           }
    //         }
    //       )
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
    //       this._estmationService.CastOldGoldAttachmentSummaryData.subscribe(
    //         response => {
    //           this.OldGoldAttachmentSummaryData = response;
    //           if (this.isEmptyObject(this.OldGoldAttachmentSummaryData) == false && this.isEmptyObject(this.OldGoldAttachmentSummaryData) != null) {
    //             if (this.EnableOldGoldAttachmentTab == true) {
    //               this.NoRecordsAttachOldGold = true;
    //             }
    //             else {
    //               this.NoRecordsAttachOldGold = false;
    //             }
    //           }
    //           else {
    //             this.NoRecordsAttachOldGold = false;
    //           }
    //         }
    //       )
    //     }
    //     else {
    //       this.EnableOldGoldAttachmentTab = true;
    //     }
    //   });
    if (this.NoSummarySales == true) {
      this.EnableOldGoldAttachmentTab = !this.EnableOldGoldAttachmentTab;
    }
  }

  SalesBilling: SalesBilling = {
    IsOrder: false,
    IsCreditNote: false,
    SalesManCode: null,
    CompanyCode: null,
    BranchCode: null,
    CustID: 0,
    MobileNo: null,
    EstNo: null,
    ReceivableAmt: 0,
    BalanceAmt: 0,
    DifferenceDiscountAmt: 0,
    PayableAmt: 0,
    OperatorCode: localStorage.getItem('Login'),
    GuaranteeName: null,
    DueDate: null,
    PANNo: null,
    BillType: null,
    lstOfPayment: [],
    lstOfPayToCustomer: [],
    salesInvoiceAttribute: [],
    OrderOTPAttributes: [],
    RowRevision: ""
  }

  ngOnDestroy() {
    this._salesService.SendSalesDataToEstComp(null);
    this._CustomerService.SendCustDataToEstComp(null);
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
    this._salesService.salesDetails(null);
    this._purchaseService.sendPurchaseDatatoEstComp(null);
    this._estmationService.SendEstNo(null);
    this._salesService.SaveSalesEstNo(null);
    this._paymentService.outputData(null);
    this._paymentService.inputData(null);
    this._paymentService.OutputParentJSONFunction(null);
    this._paymentService.SendPaymentSummaryData(null);
    this._ordersService.SendReservedOrderDetsToSalesComp(null);
    //added on 05-Nov-2020
    this._ordersService.SendOrderDetsToOrderComp(null);
    this._ordersService.SendOrderNoToReprintComp(null);
    this._estmationService.SendOrderAttachmentSummaryData(null);
    this._estmationService.SendSRAttachmentSummaryData(null);
    this._estmationService.SendOldGoldAttachmentSummaryData(null);
    //ends here
    // this._salesBillingService.SendpaymentorReceipt("");
    this.ClearValues();
    this.SalesBilling = {
      IsOrder: false,
      IsCreditNote: false,
      SalesManCode: null,
      CompanyCode: null,
      BranchCode: null,
      CustID: 0,
      MobileNo: null,
      EstNo: null,
      ReceivableAmt: 0,
      BalanceAmt: 0,
      DifferenceDiscountAmt: 0,
      PayableAmt: 0,
      OperatorCode: null,
      GuaranteeName: null,
      DueDate: null,
      BillType: null,
      PANNo: null,
      lstOfPayment: [],
      lstOfPayToCustomer: [],
      salesInvoiceAttribute: [],
      OrderOTPAttributes: [],
      RowRevision: ""
    }
  }

  isEmptyObject(obj) {
    return (obj && (Object.keys(obj).length === 0));
  }

  // EnableSales() {
  //   this.EnableSalesTab = true;
  //   this.NoRecordsSales = true;
  //   this.TogglePurchase = false;
  //   this.NoRecordsPurchase = false;
  // }

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
        if (this.isEmptyObject(this.CustomerName) == false && this.isEmptyObject(this.CustomerName) != null && this.CustomerName != 0) {
          this.SalesBilling.CustID = this.CustomerName.ID;
          this.SalesBilling.MobileNo = this.CustomerName.MobileNo;
          //this.EnableSalesTab = false;   //Commented on 08-JAN-2020
          this.CollapseCustomerTab = true;
          this.NoRecordsCustomer = true;
          //this.NoRecordsSales = true;  //Commented on 08-JAN-2020
          this.CollapseCustomerDetailsTab = true;
          this.leavePage = true;
        }
        else {
          this.EnableSalesTab = true;
          this.CollapseCustomerTab = false;
          this.NoRecordsCustomer = false;
          this.NoRecordsSales = false;
          this.EnablePurchaseTab = true;
          this.leavePage = false;
        }
      });
  }

  GetSalesDetsFromSalescomp() {
    this._salesService.cast.subscribe(
      response => {
        this.SalesData = response;
        if (this.isEmptyObject(this.SalesData) == false && this.isEmptyObject(this.SalesData) != null) {
          this.NoRecordsPurchase = false;
          this.NoSummarySales = true; //Commented on 08-JAN-2020
          this.ToggleSales = true;
          this.NoRecordsSales = true; //Commented on 08-JAN-2020
          //this.indicator = "S";
          //this.FinalAmount();
        }
        else {
          this.EnableSalesTab = true;
          this.NoRecordsPurchase = false;
          this.NoSummarySales = false;
          this.ReceivableAmount = 0.00;
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
          //this.indicator = "P";
          //this.FinalAmount();
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

  GetEstNoToggleSales() {
    this._salesService.EstNo.subscribe(
      response => {
        this.EstNo = response;
        if (this.EstNo != "" && this.EstNo != null) {
          //this.EnableSales();
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
          //this.EnableSales();
          this.EnablePurchaseTab = true;
          this.TogglePurchase = true;
        }
        else {
          //this.EnablePurchase();
        }
      }
    )
  }

  GetOrderAttachedSummary() {
    this._estmationService.CastOrderAttachmentSummaryData.subscribe(
      response => {
        this.OrderAttachmentSummaryData = response;
        if (this.isEmptyObject(this.OrderAttachmentSummaryData) == false && this.isEmptyObject(this.OrderAttachmentSummaryData) != null) {
          this.NoRecordsAttachOrder = true;
          this.EnableOrderAttachmentTab = true;
          this.NoSummaryOrder = true;
          //this.indicator = "O";
          //this.FinalAmount();
        }
        else {
          this.NoRecordsAttachOrder = false;
          this.NoSummaryOrder = false;
        }
      });
  }

  GetSRAttachedSummary() {
    this._estmationService.CastSRAttachmentSummaryData.subscribe(
      response => {
        this.SRAttachmentSummaryData = response;
        if (this.isEmptyObject(this.SRAttachmentSummaryData) == false && this.isEmptyObject(this.SRAttachmentSummaryData) != null) {
          this.NoRecordsAttachSR = true;
          this.EnableSRAttachmentTab = true;
          this.NoSummarySalesReturn = true;
          //this.indicator = "SR";
          //this.FinalAmount();
        }
        else {
          this.NoRecordsAttachSR = false;
          this.NoSummarySalesReturn = false;
        }
      });
  }

  GetOldGoldAttachedSummary() {
    this._estmationService.CastOldGoldAttachmentSummaryData.subscribe(
      response => {
        this.OldGoldAttachmentSummaryData = response;
        if (this.isEmptyObject(this.OldGoldAttachmentSummaryData) == false && this.isEmptyObject(this.OldGoldAttachmentSummaryData) != null) {
          this.NoRecordsAttachOldGold = true;
          this.EnableOldGoldAttachmentTab = true;
          this.NoSummaryOldGold = true;
          //this.indicator = "OG";
          //this.();
        }
        else {
          this.NoRecordsAttachOldGold = false;
          this.NoSummaryOldGold = false;
        }
      });
  }

  SalesTotAmount: number = 0.00;
  OrderTotAmount: number = 0.00;
  PurchaseTotAmount: number = 0.00;
  SRTotAmount: number = 0.00;
  OGTotAmount: number = 0.00;
  TotAmount: number = 0.00;
  RecAmount: number = 0.00;
  BalAmount: number = 0.00;
  PayAmount: number = 0.00;
  SelectedOrderList: any = [];
  EnablePaymentsTab: boolean = false;
  EnableShopKeeperTab: boolean = true;
  NoRecordsPayments: boolean = true;
  EnableReceiveAmount: boolean = true;

  SalesTotalAmountCalculation(Amount) {
    this.SalesTotAmount = Amount;
    this.FinalAmount();
  }

  PurchaseTotalAmountCalculation(Amount) {
    this.PurchaseTotAmount = Amount;
    this.FinalAmount();
  }

  OrderTotalAmountCalculation(Amount) {
    this.OrderTotAmount = Amount + this.GlobalOrderAmount;
    this.OrderAttachmentSummaryData = {
      Amount: this.OrderTotAmount
    }
    this.FinalAmount();
  }

  SRTotalAmountCalculation(Amount) {
    this.SRTotAmount = Amount;
    this.FinalAmount();
  }

  OGTotalAmountCalculation(Amount) {
    this.OGTotAmount = Amount;
    this.FinalAmount();
  }

  FinAmt: number = 0;

  diffDisAmtForCalc: number = 0.00;

  // TogglePaymentData() {
  //   if (this.NoRecordsPayments == false) {
  //     swal("Warning", "Please select Item Details ", "warning");
  //   }
  //   else {
  //     this.FinAmt = Math.round(this.TotAmount - this.RecAmount);
  //     if (this.FinAmt < 0) {
  //       this.EnablePaymentsTab = true;        
  //     }
  //     else {
  //       this.EnablePaymentsTab = !this.EnablePaymentsTab;
  //     }
  //   }
  // }

  //12oct issue with receipts & payment tab sloved
  TogglePaymentData() {
    if (this.NoRecordsPayments == false) {
      swal("Warning", "Please select Item Details ", "warning");
    }
    else {
      this.FinAmt = Math.round(this.TotAmount - this.RecAmount);
      // if (this.FinAmt < 0) {
      //   this.EnablePaymentsTab = true;
      // }
      this.EnablePaymentsTab = !this.EnablePaymentsTab;
    }
  }
  //12oct issue with receipts & payment tab sloved

  FinalAmount() {
    //Commented on 10-Nov-2020
    //this.ReceivableAmount = this.SalesTotAmount - this.PurchaseTotAmount - this.SRTotAmount - this.OrderTotAmount - this.OGTotAmount;
    // if (this.ReceivableAmount < 0) {
    //   this.EnableReceiveAmount = true;
    //   this.PaymentHeader = "Payments";
    //   this.pageName = "OC";
    // }
    // else {
    //   this.EnableReceiveAmount = false;
    //   this.PaymentHeader = "Receipts";
    //   this.pageName = "S";
    // }
    //ends here

    //Added on 10-Nov-2020
    if (this.NoSummarySales == true) {
      if (this.RndOffAmtFlag == true) {
        // this.ReceivableAmount = this.SalesAmountInclTax;
        this.ReceivableAmount = this.SalesAmountExclTax + this.GSTAmount;
      }
      else if (this.RndOffAmtFlag == false) {
        this.ReceivableAmount = this.SalesData.Amount;
      }
      else if (this.NoSummaryPurchase == true) {
        //this.ReceivableAmount = this.ReceivableAmount - Math.round(this.PurchaseData.Amount);
        this.ReceivableAmount = this.ReceivableAmount - this.PurchaseData.Amount;
      }
      else if (this.NoSummaryOrder == true) {
        this.ReceivableAmount = this.ReceivableAmount - this.OrderAttachmentSummaryData.Amount;
      }
      else if (this.NoSummarySalesReturn == true) {
        this.ReceivableAmount = this.ReceivableAmount - this.SRAttachmentSummaryData.Amount;
      }
      else if (this.NoSummaryOldGold == true) {
        //this.ReceivableAmount = this.ReceivableAmount - Math.round(this.OldGoldAttachmentSummaryData.Amount);
        this.ReceivableAmount = this.ReceivableAmount - this.OldGoldAttachmentSummaryData.Amount;
      }
    }
    //Ends here
    // this._salesBillingService.SendpaymentorReceipt(this.pageName);

    //Commented on 18-Feb-2021
    // if (this.DiscountApplied == false) {
    //this.TotalAmount();//Commented on 29-Mar-2021
    // this.ReceivedAmount(); //Commented on 29-Mar-2021
    // this.BalanceAmount(); //Commented on 29-Mar-2021
    // }

    //ends here

    this.SalesBilling.PayableAmt = 0;

    if (this.BalAmount < 0) {
      this.EnableReceiveAmount = true;
      this.PaymentHeader = "Payments";
      this.pageName = "OC";
      if (this.autoFetchDisAmount == 0) {
        if (this.DiscountApplied == false) {
          this.autoFetchAmount = this.PayAmount;
        }
        else {
          this.autoFetchAmount = 0;
        }
      }
      else {
        this.autoFetchDisAmount = this.PayableAmount();
      }
      this.SalesBilling.PayableAmt = Math.round(this.TotAmount - this.RecAmount);//Added on 25-Nov-2020
      if (this.SalesBilling.lstOfPayment.length > 0) {
        this.EnableDisablePaymentForOrderAdj = true;
        //this.EnablePaymentsTab = true;
      }
      else {
        this.EnableDisablePaymentForOrderAdj = false;
        //this.EnablePaymentsTab = false;
      }
      this.EnablePayToCust = true;

      //Commented on 30-Mar-2021 due to Payment Discount Amount getting disable after operator discount error from api
      // if (this.PayableFinalAmt > 0) {
      //   this.PayAmtEnable = true;
      // }
      // else {
      //   this.PayAmtEnable = false;
      // }
      //ends here

      //this.EnableShopKeeperTab = false;

    }
    else if (this.BalAmount > 0) {
      //this.EnableShopKeeperTab = true;
      //this.EnablePaymentsTab = false;
      //12oct issue with receipts & payment tab sloved
      this.SalesBilling.lstOfPayToCustomer = [];
      this.ArrayList = [];
      //12oct issue with receipts & payment tab sloved
      this.EnablePayToCust = false;
      this.EnableDisablePaymentForOrderAdj = true;
      this.EnableReceiveAmount = false;
      this.PaymentHeader = "Receipts";
      this.pageName = "S";
      if (this.autoFetchDisAmount == 0) {
        if (this.DiscountApplied == false) {
          //this.autoFetchAmount = this.BalAmount;
        }
        else {
          this.autoFetchAmount = 0;
        }
      }
      else {
        //this.autoFetchDisAmount = Math.round(this.SalesAmountInclTax - this.PurchaseTotAmount - this.RecAmount);
        //this.autoFetchDisAmount = this.DisAmt - this.RecAmount; //Commented on 18-Feb-2021
        //this.autoFetchDisAmount = this.TotAmount - this.RecAmount; //Commented on 02-Mar-2021
        //this.autoFetchDisAmount = this.ReceivedFinalAmt - this.RecAmount; //Commented on 05-Mar-2021
        //this.autoFetchDisAmount = this.BalAmount; //Added on 05-Mar-2021
        this.autoFetchAmount = 0;
      }
    }
    else {
      this.EnablePayToCust = false;
    }

    return this.ReceivableAmount;
  }

  DisItemSummary() {
    this._salesService.cast.subscribe(
      response => {
        this.SalesData = response;
        if (this.isEmptyObject(this.SalesData) == false && this.isEmptyObject(this.SalesData) != null) {
          this.filterRadioBtns = true;
          this.EnableDiscountItemTab = !this.EnableDiscountItemTab;
        }
        else {
          this.EnableDiscountItemTab = true;
        }
      });
  }

  BalanceAmount() {
    //this.BalAmount = this.TotAmount - this.RecAmount; //Commented on 10-Nov-2020
    this.diffDisAmtForCalc = this.TotAmount - this.RecAmount;
    this.BalAmount = Math.round(this.TotAmount - this.RecAmount); //Added on 10-Nov-2020
    if (this.BalAmount >= 0) {
      this.SalesBilling.BalanceAmt = this.BalAmount;
      this.autoFetchDisAmount = Number(this.SalesBilling.BalanceAmt);
    }
    if (this.model.option == "Payments") {
      // this.SalesBilling.PayableAmt = Number(this.BalAmount.toFixed(2)); //Commented on 10-Nov-2020
      this.SalesBilling.PayableAmt = this.BalAmount;//Added on 10-Nov-2020
    }
    else if (this.model.option == "Order") {
      this.SalesBilling.PayableAmt = this.PayAmount;
    }

    if (this.BalAmount > 0) {
      this.RecAmtEnable = false;
      this.PayAmtEnable = true;
      if (this.DiscountApplied == true) {
        this.RecAmtEnable = true;
        //this.autoFetchDisAmount = this.BalAmount;
      }
      else {
        //this.autoFetchAmount = this.BalAmount;
      }
    }
    else if (this.BalAmount == 0) { //Added newly else if condition to accept less than 0 on 12-Apr-2021
      this.RecAmtEnable = true;
      this.PayAmtEnable = true;
    }
    else if (this.BalAmount < 0) {
      this.RecAmtEnable = true;
      this.PayAmtEnable = false;
      //Commented on 02-Mar-2021
      // if (this.DiscountApplied == true) {
      //   this.PayAmtEnable = true;
      //   this.autoFetchDisAmount = this.BalAmount;
      // }
      //   else {
      //   this.autoFetchAmount = this.BalAmount;
      // }
      //ends here

      //Added on 02-Mar-2021

      if (this.DiscountApplied == true) {
        this.PayAmtEnable = false;
        this.autoFetchDisAmount = this.BalAmount;
      }
      if (this.DiscountApplied == true && this.ReceivedFinalAmt > 0) {
        this.PayAmtEnable = false;
        this.autoFetchDisAmount = this.BalAmount;
      }
      if (this.DiscountApplied == true && this.ReceivedFinalAmt == 0) {
        this.PayAmtEnable = true;
        this.autoFetchDisAmount = this.BalAmount;
      }
      if (this.DiscountApplied == true && this.PayableFinalAmt > 0) {
        this.PayAmtEnable = true;
        this.autoFetchDisAmount = this.BalAmount;
      }
      if (this.DiscountApplied == true && this.PayableFinalAmt == 0) {
        this.PayAmtEnable = false;
        this.autoFetchDisAmount = this.BalAmount;
      }
      if (this.DiscountApplied == false) {
        this.autoFetchAmount = this.BalAmount;
      }
      //ends here

    }

    return this.BalAmount
  }

  PaidTotalAmountCalculation(Amount) {
    this.PaidAmount = Amount;
    //this.PayableAmount(); //Commented on 05-04-2021 for optimization
    //this.ReceivedAmount(); //Commented on 05-04-2021 for optimization
  }

  PayableAmount() {
    if (this.EnablePayToCust == true) {
      this.PayAmount = Math.abs(this.BalAmount) - this.PaidBySK;
      //this.PaymentHeaders.PayAmount = this.PayAmount;
      if (this.autoFetchDisAmount == 0) {
        if (this.DiscountApplied == false) {
          this.autoFetchAmount = this.PayAmount;
        }
        else {
          this.autoFetchAmount = 0;
        }
      }
      this.SalesBilling.BalanceAmt = Math.round(this.TotAmount - this.RecAmount) + this.PaidBySK; //Added on 10-Nov-2020
    }
    else {
      this.PayAmount = Math.abs(this.BalAmount) - this.PaidAmount;
      if (this.autoFetchDisAmount == 0) {
        if (this.DiscountApplied == false) {
          this.autoFetchAmount = this.PayAmount;
        }
        else {
          this.autoFetchAmount = 0;
        }
      }
      this.SalesBilling.BalanceAmt = Math.round(this.TotAmount - this.RecAmount) + this.PaidBySK; //Added on 10-Nov-2020
    }

    return this.PayAmount;
  }

  DeleteRow(index: number, arrayLines) {
    var ans = confirm("Do you want to delete");
    if (ans) {
      this.ArrayList.splice(index, 1)
      this.SalesBilling.lstOfPayToCustomer = [];
      this.SalesBilling.lstOfPayToCustomer = this.ArrayList;
      this.PaidByShopKeeper(this.ArrayList);
      this.toggle = "Invalid"
      this.PaymentForm.reset();
    }
  }

  ReceivedAmount() {
    this.RecAmount = this.SRTotAmount + this.OrderTotAmount + this.OGTotAmount;
    if (this.PaymentHeader == "Receipts") {
      this.RecAmount = this.RecAmount + this.PaidAmount;
      this.SalesBilling.ReceivableAmt = this.PaidAmount;
    }
    else {
      if (this.EnablePayToCust == true) {
        this.RecAmount = this.RecAmount + this.PaidAmount;
      }
      else {
        this.SalesBilling.ReceivableAmt = 0.00;
      }
    }
    return this.RecAmount;
  }

  TotalAmount() {
    //this.TotAmount = Math.round(this.SalesTotAmount - this.PurchaseTotAmount);
    if (this.NoSummarySales && this.DiscountApplied == false) {
      if (this.SalesData.Amount != 0 && this.SalesData.Amount != null) {
        this.TotAmount = this.SalesData.Amount;
      }
    }
    if (this.NoSummaryPurchase && this.DiscountApplied == false) {
      //this.TotAmount = this.SalesData.Amount - Math.round(this.PurchaseData.Amount);
      if (this.PurchaseData.Amount != 0 && this.PurchaseData.Amount != null) {
        this.TotAmount = this.SalesData.Amount - this.PurchaseData.Amount;
      }
    }
    return this.TotAmount;
  }

  Recalculation: any = [];
  differenceDiscountAmt: number = 0.00;
  DiscPer: number = 0.00;
  offerDiscount: number = 0.00;
  AddDiscAmt: number = 0.00;
  CessAmt: number = 0.00;
  SalesAmountExclTax: number = 0.00;
  GSTAmount: number = 0.00;
  SalesAmountInclTax: number = 0.00;
  DiscountApplied: boolean = false;
  McDiscAmt: number = 0.00;
  RndOffAmtFlag: boolean = false;
  DisAmt: number = 0;
  ReceivedFinalAmt: number = 0;
  PayableFinalAmt: number = 0;


  ToggleShopKeeperData() {
    if (this.model.option == "Order") {
      this.EnableShopKeeperTab = true;
      this.ArrayList = [];
      this.PaymentForm.reset();
      this.toggle = "Invalid";
    }
    else {
      this.EnableShopKeeperTab = !this.EnableShopKeeperTab;
    }
  }


  ReceivedAmt(RecAmt) {
    // if (RecAmt > this.BalanceAmount()) { // Commented on 05-04-2021 for optimization
    if (RecAmt > this.BalAmount) { // Added on 05-04-2021 for optimization
      swal("Warning!", "Received amount is greater than balance amount.", "warning");
      this.DiscountApplied = false;
      this.RecAmtEnable = false;
    }
    else {
      if (this.rowRevisionFlag == true) {
        this._salesService.getRowVersion(this.SalesBilling.EstNo).subscribe(
          response => {
            this.CurrentRowRevision = response;
            if (this.CurrentRowRevision.RowRevisionString == this.RowRevision.RowRevisionString) {
              this.differenceDiscountAmt = Number((this.diffDisAmtForCalc - RecAmt).toFixed(2))//Added on 05-Mar-2021
              if (RecAmt > 0) {
                if (RecAmt < 1) { //Added newly if condition to accept less than 0 on 12-Apr-2021
                  this.differenceDiscountAmt = Number((this.diffDisAmtForCalc - 0).toFixed(2))
                }
                //this.differenceDiscountAmt = this.BalanceAmount() - RecAmt;//Commented on 24-Feb-2021
                //this.differenceDiscountAmt = Number((this.TotAmount - RecAmt).toFixed(2));//Commented on 05-Mar-2021
                this._salesBillingService.postdifferenceDiscountAmt(this.SalesBilling.EstNo, this.differenceDiscountAmt, this.ccode, this.bcode).subscribe(
                  response => {
                    this.Recalculation = response;
                    if (this.Recalculation != null) {
                      this.SalesBilling.DifferenceDiscountAmt = Number(this.differenceDiscountAmt.toFixed(2));
                      this.EnablePaymentsTab = false;
                      //this.EnableShopKeeperTab = true;
                      this.ReceivedFinalAmt = RecAmt;
                      this.DiscountApplied = true;
                      this.RndOffAmtFlag = true;
                      this.DisAmt = RecAmt;
                      this.DiscPer = Number((this.Recalculation.DiscountPercent).toFixed(2));
                      this.McDiscAmt = this.Recalculation.MCDiscount;
                      this.AddDiscAmt = this.Recalculation.DiscountAmount;
                      this.CessAmt = this.Recalculation.GSTCessAmount;
                      this.SalesAmountExclTax = this.Recalculation.SalesAmountExclTax;
                      this.GSTAmount = this.Recalculation.GSTAmount;
                      this.SalesAmountInclTax = this.Recalculation.SalesAmountInclTax;
                      this.autoFetchDisAmount = this.BalAmount;////Added on 05-Mar-2021
                      //this.DisTotalAmount();//Commented on 05-Mar-2021
                      //this.autoFetchDisAmount = this.TotAmount; //Commented on 18-Feb-2021
                      //this.autoFetchDisAmount = Math.round(this.TotAmount - this.RecAmount); //Commented on 02-Mar-2021
                      //this.autoFetchDisAmount = this.ReceivedFinalAmt;//Commented on 05-Mar-2021
                      this.SalesBilling.salesInvoiceAttribute = this.Recalculation;
                      this.autoFetchAmount = 0;
                      this.leavePage = true;

                    }
                  },
                  (err) => {
                    this.DiscountApplied = false;
                    this.RecAmtEnable = false;
                    this.RecAmt.nativeElement.focus();
                  }
                )
              }
              else {
                this.getEstimationDetails(this.EstNo);
                this.EnableDiscountItemTab = false;
                this.SalesBilling.salesInvoiceAttribute = [];
                this.SalesBilling.DifferenceDiscountAmt = 0;
                this.DiscountApplied = false;
              }
            }
            else {
              swal("Warning!", "Someone altered the estimation.Kindly re-load the estimation", "warning");
              this.CurrentRowRevision = null;
            }
          }
        )
      }
      else {
        this.differenceDiscountAmt = Number((this.diffDisAmtForCalc - RecAmt).toFixed(2))//Added on 05-Mar-2021
        if (RecAmt > 0) {
          if (RecAmt < 1) { //Added newly if condition to accept less than 0 on 12-Apr-2021
            this.differenceDiscountAmt = Number((this.diffDisAmtForCalc - 0).toFixed(2))
          }
          //this.differenceDiscountAmt = this.BalanceAmount() - RecAmt;//Commented on 24-Feb-2021
          //this.differenceDiscountAmt = Number((this.TotAmount - RecAmt).toFixed(2));//Commented on 05-Mar-2021
          this._salesBillingService.postdifferenceDiscountAmt(this.SalesBilling.EstNo, this.differenceDiscountAmt, this.ccode, this.bcode).subscribe(
            response => {
              this.Recalculation = response;
              if (this.Recalculation != null) {
                this.SalesBilling.DifferenceDiscountAmt = Number(this.differenceDiscountAmt.toFixed(2));
                this.EnablePaymentsTab = false;
                //this.EnableShopKeeperTab = true;
                this.ReceivedFinalAmt = RecAmt;
                this.DiscountApplied = true;
                this.RndOffAmtFlag = true;
                this.DisAmt = RecAmt;
                this.DiscPer = Number((this.Recalculation.DiscountPercent).toFixed(2));
                this.McDiscAmt = this.Recalculation.MCDiscount;
                this.AddDiscAmt = this.Recalculation.DiscountAmount;
                this.CessAmt = this.Recalculation.GSTCessAmount;
                this.SalesAmountExclTax = this.Recalculation.SalesAmountExclTax;
                this.GSTAmount = this.Recalculation.GSTAmount;
                this.SalesAmountInclTax = this.Recalculation.SalesAmountInclTax;
                this.autoFetchDisAmount = this.BalAmount;////Added on 05-Mar-2021
                //this.DisTotalAmount();//Commented on 05-Mar-2021
                //this.autoFetchDisAmount = this.TotAmount; //Commented on 18-Feb-2021
                //this.autoFetchDisAmount = Math.round(this.TotAmount - this.RecAmount); //Commented on 02-Mar-2021
                //this.autoFetchDisAmount = this.ReceivedFinalAmt;//Commented on 05-Mar-2021
                this.SalesBilling.salesInvoiceAttribute = this.Recalculation;
                this.autoFetchAmount = 0;
                this.leavePage = true;

              }
            },
            (err) => {
              this.DiscountApplied = false;
              this.RecAmtEnable = false;
              this.RecAmt.nativeElement.focus();
            }
          )
        }
        else {
          this.getEstimationDetails(this.EstNo);
          this.EnableDiscountItemTab = false;
          this.SalesBilling.salesInvoiceAttribute = [];
          this.SalesBilling.DifferenceDiscountAmt = 0;
          this.DiscountApplied = false;
        }
      }
    }
  }

  DisTotalAmount() {
    // this.TotAmount = this.DisAmt;     //Commeneted on 18-Feb-2021

    //Added on 18-Feb-2021
    //this.TotAmount = this.SalesAmountInclTax;
    this.TotAmount = this.SalesAmountExclTax + this.GSTAmount
    if (this.NoSummaryPurchase == true) {
      this.TotAmount = this.TotAmount - this.PurchaseData.Amount;
    }
    //Ends here
    return this.TotAmount;
  }

  ClearValues() {
    this.ReceivableAmount = 0.00;
    this.SalesTotAmount = 0.00;
    this.OrderTotAmount = 0.00;
    this.PurchaseTotAmount = 0.00;
    this.SRTotAmount = 0.00;
    this.OGTotAmount = 0.00;
    this.TotAmount = 0.00;
    this.RecAmount = 0.00;
    this.BalAmount = 0.00;
    this.PayAmount = 0.00;
    this.PaidBySK = 0.00;
    this.PaidAmount = 0.00;
    this.Recalculation = [];
    this.offerDiscount = 0.00;
    this.differenceDiscountAmt = 0.00;
    this.DiscPer = 0.00;
    this.AddDiscAmt = 0.00;
    this.CessAmt = 0.00;
    this.SalesAmountExclTax = 0.00;
    this.GSTAmount = 0.00;
    this.SalesAmountInclTax = 0.00;
    this.RndOffAmtFlag = false;
    this.pageName = "";
    this.EnableDisablePayment = false;
    this.EnableSalesTab = true;
    this.EnablePurchaseTab = true;
    this.EnableOrderAttachmentTab = true;
    this.EnableSRAttachmentTab = true;
    this.EnableOldGoldAttachmentTab = true;
    this.GuaranteeFlag = false;
    this.DiscountApplied = false;
    this.SalesBilling.IsCreditNote = false;
    this.PayableFinalAmt = 0.00;
    this.ReceivedFinalAmt = 0.00;
  }


  validated: boolean = false;
  index: number = 0;
  ValidateOTPSubmit(OTPData) {
    this.index = 0;
    if (OTPData.length > 0) {
      for (let i = 0; i < OTPData.length; i++) {
        if (OTPData[i].IsValidated == false) {
          this.validated = false;
          this.index = i;
          break;
        }
        else {
          this.validated = true;
        }
      }
      if (this.validated == true) {
        $('#UpdateOTP').modal('hide');
      }
      else {
        swal("Warning!", 'Please Validate OTP for Order No:' + OTPData[this.index].OrderNo, "warning");
        $('#UpdateOTP').modal('show');
      }
    }
  }

  addRow(arrayLines) {
    if (this.toggle == "Cash") {
      if (arrayLines.value.PayAmount == null || arrayLines.value.PayAmount == "" || arrayLines.value.PayAmount == 0) {
        swal("Warning!", 'Please enter the amount', "warning");
      }
      else {
        this.SaveAddrow(arrayLines, this.toggle);
      }
    }
    else if (this.toggle == "EP") {
      if (arrayLines.value.PayDetails == null || arrayLines.value.PayDetails == "" || arrayLines.value.PayDetails == 0) {
        swal("Warning!", 'Please enter the remarks', "warning");
      }
      else if (arrayLines.value.PayAmount == null || arrayLines.value.PayAmount == "" || arrayLines.value.PayAmount == 0) {
        swal("Warning!", 'Please enter the amount', "warning");
      }
      else {
        this.SaveAddrow(arrayLines, this.toggle);
      }
    }
    else if (this.toggle == "Cheque") {
      if (arrayLines.value.BankName == null || arrayLines.value.BankName == "") {
        swal("Warning!", 'Please enter valid bank name', "warning");
      }
      else if (arrayLines.value.ChequeNo == null) {
        swal("Warning!", 'Please enter the cheque number', "warning");
      }
      else if (arrayLines.value.ChequeNo.length < 6) {
        swal("Warning!", 'Please enter valid Cheque No', "warning");
      }
      else if (arrayLines.value.PayAmount == null || arrayLines.value.PayAmount == "" || arrayLines.value.PayAmount == 0) {
        swal("Warning!", 'Please enter the amount', "warning");
      }
      else {
        this.SaveAddrow(arrayLines, this.toggle);
      }
    }
  }

  ArrayList: any = [];
  PaidBySK: number = 0;
  SaveAddrow(arrayLines, paymentMode) {
    if (this.SalesBilling.lstOfPayToCustomer.length > 0) {
      let data = this.SalesBilling.lstOfPayToCustomer.find(x => x.PayMode == arrayLines.value.PayMode);
      if (data != null && paymentMode != "Scheme" && paymentMode != "OrdAdj" && paymentMode != "PurEst" && paymentMode != "PurBill" && paymentMode != "SREst" && paymentMode != "SRBill" && paymentMode != "Card" && paymentMode != "EP" && paymentMode != "UPI Payment" && paymentMode != "Cheque") {
        swal("error", paymentMode + " mode is already added. Please select other mode of payment", "error");
      }
      else {
        arrayLines.value.CompanyCode = this.ccode;
        arrayLines.value.BranchCode = this.bcode;
        arrayLines.value.OperatorCode = localStorage.getItem('Login');
        this.SalesBilling.lstOfPayToCustomer.push(arrayLines.value);
        this.ArrayList = this.SalesBilling.lstOfPayToCustomer;
        this.toggle = "Invalid"
        this.PaymentForm.reset();
        this.EnablePaymentsTab = true;
      }
    }
    else {
      arrayLines.value.CompanyCode = this.ccode;
      arrayLines.value.BranchCode = this.bcode;
      arrayLines.value.OperatorCode = localStorage.getItem('Login');
      this.SalesBilling.lstOfPayToCustomer.push(arrayLines.value);
      this.ArrayList = this.SalesBilling.lstOfPayToCustomer;
      this.toggle = "Invalid"
      this.PaymentForm.reset();
      this.EnablePaymentsTab = true;
    }
  }



  PaidByShopKeeper(arg: any[] = []) {
    let total = 0;
    if (arg.length > 0) {
      arg.forEach((d) => {
        total += Number(d.PayAmount);
      });
      this.PaymentHeader = "Payments";
      this.PaidBySK = total;
      //this.BalanceAmount(); //Commented on 05-04-2021 for optimization
      //this.PayableAmount(); //Commented on 05-04-2021 for optimization
      this.leavePage = true;
    }
    else {
      this.PaymentHeader = "Payments";
      this.PaidBySK = total;
      //this.BalanceAmount(); //Commented on 05-04-2021 for optimization
      //this.PayableAmount(); //Commented on 05-04-2021 for optimization
    }
    return total;
  }
}