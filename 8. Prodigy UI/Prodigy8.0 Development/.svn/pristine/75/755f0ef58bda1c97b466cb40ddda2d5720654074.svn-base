import { AfterContentChecked, AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { DeriveEstimationBalances } from './new-sales-billing.model';
import { NewSalesBillingService } from './new-sales-billing.service';
import { PaymentService } from '../payment/payment.service';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../AppConfigService';
import { CustomerService } from './../masters/customer/customer.service';
import swal from 'sweetalert';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SalesBilling } from '../sales-billing/sales-billing.model';
import { SalesService } from './../sales/sales.service';
import { NgxSpinnerService } from "ngx-spinner";
import { MasterService } from './../core/common/master.service';
import { formatDate } from '@angular/common';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { Router } from '@angular/router';

declare var $: any;

@Component({
  selector: 'app-new-sales-billing',
  templateUrl: './new-sales-billing.component.html',
  styleUrls: ['./new-sales-billing.component.css']
})
export class NewSalesBillingComponent implements OnInit {

  ccode: string = "";
  bcode: string = "";
  password: string;
  CustomerName: any;
  EnableDisablePayment: boolean = false;
  EnableDiscountItemTab: boolean = true;
  estDetails: any = [];
  EnableJson: boolean = false;
  GuarantorForm: FormGroup;
  CollapseCustomerDetailsTab: boolean = true;
  EnableSalesTab: boolean = true;
  EnablePayToCust: boolean = false;
  EnableDisablePaymentForOrderAdj: boolean = false;
  EnablePurchaseTab: boolean = true;
  PaymentForm: FormGroup;
  pageName: string = "S";
  EnableSubmitButton: boolean = true;
  PaymentModes: any;
  routerUrl: string = "";
  EnableOrderAttachmentTab: boolean = true;
  EnableReprint: boolean = false;
  EnableSRAttachmentTab: boolean = true;
  EnableOldGoldAttachmentTab: boolean = true;
  EnableBilling: boolean = false;
  ordDate = '';
  public CollapseCustomerTab: boolean = true;
  @ViewChild("RecAmt", { static: false }) RecAmt: ElementRef;
  @ViewChild("PayAmt", { static: false }) PayAmt: ElementRef;
  radioItems: Array<string>;
  model = { option: 'Payment' };
  rowRevisionFlag: boolean = false;
  LineAmountBeforeTax: number = 0.00;
  TaxAmount: number = 0.00;
  LineAmountAfterTax: number = 0.00;
  today = new Date();
  datePickerConfig: Partial<BsDatepickerConfig>;
  GuaranteedatePickerConfig: Partial<BsDatepickerConfig>;

  constructor(private _appConfigService: AppConfigService,
    private _newSalesBillingService: NewSalesBillingService,
    private _CustomerService: CustomerService, private formBuilder: FormBuilder,
    private _paymentService: PaymentService, private _salesService: SalesService,
    private SpinnerService: NgxSpinnerService,
    private _masterService: MasterService, private _router: Router) {
    this.routerUrl = this._router.url;
    this.ordDate = formatDate(this.today, 'dd/MM/yyyy', 'en-US', '+0530');
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.rowRevisionFlag = this._appConfigService.RowRevisionBilling;
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
    this.getCB();
  }

  DeriveEstimationBalances: DeriveEstimationBalances = {
    CompanyCode: this.ccode,
    BranchCode: this.bcode,
    EstNo: 0,
    CalculateFinalAmount: false,
    TotalReceiptAmount: 0.00,
    TotalPaymentAmount: 0.00,
    DifferenceDiscountAmt: 0.00,
    ReceivableAmount: 0.00,
    RowVersion: ""
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.DeriveEstimationBalances.CompanyCode = this.ccode;
    this.DeriveEstimationBalances.BranchCode = this.bcode;
  }

  ngOnInit() {
    this.SalesBillingForm = this.formBuilder.group({
      BillType: ["", Validators.required],
    });
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
    this.getApplicationdate();
    this.getPaymentMode();
    this.GetCustomerDetsFromCustComp();
    this.GetPaymentSummary();
    this.getBillTypes();
    this.GuarantorForm = this.formBuilder.group({
      name: null,
      date: null
    });
  }

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

  PurchaseLines: any = [];
  EstNo: number = 0;
  SalesBillingForm: FormGroup;
  TotalAmount: number = 0.00;

  getEstimationDetails(arg) {
    this.clearValues();
    this.DeriveEstimationBalances.EstNo = arg;
    this.model.option = "Payment";
    this.BillNo = null;
    this._newSalesBillingService.PostDeriveEstimationBalances(this.DeriveEstimationBalances).subscribe(
      response => {
        this.estDetails = response;
        this.offerDiscount = this.estDetails.OfferDiscount;
        this.EnableDisablePayment = true;
        this.DeriveEstimationBalances.RowVersion = this.estDetails.RowVersion;
        this.SalesBilling.RowRevision = this.estDetails.RowVersion;
        this.LineAmountBeforeTax = this.estDetails.LineAmountBeforeTax;
        this.TaxAmount = this.estDetails.TaxAmount;
        this.LineAmountAfterTax = this.estDetails.LineAmountAfterTax;
        this.TotalAmount = this.estDetails.GrossAmount;
        this.diffDisAmtForCalc = this.estDetails.Balance;
        this.EnableSubmitButton = false;
        this.EnableDiscountItemTab = false;
        this.SalesBilling.IsCreditNote = false;
        this.SalesBilling.IsOrder = false;
        this.SalesBilling.BillType = "N";
        this.SalesBilling.EstNo = arg;
        this.EstNo = arg;
        this.SalesBilling.OperatorCode = localStorage.getItem('Login');
        this.SalesBilling.CompanyCode = this.ccode;
        this.SalesBilling.BranchCode = this.bcode;
        this._paymentService.inputData(this.SalesBilling);
        if (this.estDetails.InlinePurchase != null) {
          this.PurchaseLines = this.estDetails.InlinePurchase.PurchaseLines;
        }
        this._CustomerService.getCustomerDtls(this.estDetails.CustomerInfo.CustId).subscribe(
          response => {
            const customerDtls = response;
            this._CustomerService.sendCustomerDtls_To_Customer_Component(customerDtls);
            if (this.estDetails.Balance > 0) {
              this.EnableDisablePaymentForOrderAdj = true;
              this.EnablePayToCust = false;
              this.RecAmt.nativeElement.focus();
            }
            else if (this.estDetails.Balance < 0) {
              this.PayAmt.nativeElement.focus();
              this.EnablePayToCust = true;
            }
          }
        )
        this.EnableBilling = true;
      },
      (err) => {
        this.EnableBilling = false;
      }
    );
  }


  GetCustomerDetsFromCustComp() {
    this._CustomerService.cast.subscribe(
      response => {
        this.CustomerName = response;
        if (this.isEmptyObject(this.CustomerName) == false && this.isEmptyObject(this.CustomerName) != null && this.CustomerName != 0) {
          this.SalesBilling.CustID = this.CustomerName.ID;
          this.SalesBilling.MobileNo = this.CustomerName.MobileNo;
          // //this.EnableSalesTab = false;   //Commented on 08-JAN-2020
          this.CollapseCustomerTab = true;
        }
        else {
          this.CollapseCustomerTab = false;
          this.CustomerName = null;
        }
      });
  }

  isEmptyObject(obj) {
    return (obj && (Object.keys(obj).length === 0));
  }

  ToggleCustomerData() {
    this.CollapseCustomerDetailsTab = !this.CollapseCustomerDetailsTab;
  }

  ToggleSalesData() {
    this.EnableSalesTab = !this.EnableSalesTab;
  }

  TogglePurchaseData() {
    this.EnablePurchaseTab = !this.EnablePurchaseTab;
  }

  ToggleOrderAttachment() {
    this.EnableOrderAttachmentTab = !this.EnableOrderAttachmentTab;
  }

  ToggleSRAttachment() {
    this.EnableSRAttachmentTab = !this.EnableSRAttachmentTab;
  }

  ToggleOldGoldAttachment() {
    this.EnableOldGoldAttachmentTab = !this.EnableOldGoldAttachmentTab;
  }

  DisItemSummary() {
    this.EnableDiscountItemTab = !this.EnableDiscountItemTab;
  }

  getPaymentMode() {
    this._paymentService.getPaymentMode("OC").subscribe(
      response => {
        this.PaymentModes = response;
      }
    )
  }

  toggle: string = "Invalid";
  autoFetchAmount: number = 0;
  autoFetchDisAmount: number = 0;

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
          }
        }
        else {
          this.NoRecordsPaymentSummary = false;
          //this.EnableSubmitButton = true;
        }
      }
    )
  }

  BalAmount: number = 0.00;

  BalanceAmount() {
    if (this.estDetails != null) {
      this.BalAmount = this.estDetails.Balance;
      if (this.PaymentSummary != null) {
        this.BalAmount = this.TotalAmount - this.PaymentSummary.Amount;
        this.DeriveEstimationBalances.TotalReceiptAmount = this.PaymentSummary.Amount;
      }
      if (this.PaidBySK != 0) {
        this.BalAmount = this.BalAmount + this.PaidBySK;
        this.DeriveEstimationBalances.TotalPaymentAmount = this.PaidBySK;
      }

      this.BalAmount = Math.round(this.BalAmount);

      this.SalesBilling.BalanceAmt = this.BalAmount;


      if (this.BalAmount < 0) {
        this.EnablePayToCust = true;
        this.PaymentHeader = "Payments";
        if (this.SalesBilling.lstOfPayment.length > 0) {
          this.EnableDisablePaymentForOrderAdj = true;
        }
        else {
          this.EnableDisablePaymentForOrderAdj = false;
        }
        this.autoFetchAmount = Math.abs(this.BalAmount);
      }
      else {
        if (this.BalAmount == 0) {
          if (this.SalesBilling.lstOfPayToCustomer.length > 0) {
            this.EnablePayToCust = true;
          }
          if (this.SalesBilling.lstOfPayment.length > 0) {
            this.EnableDisablePaymentForOrderAdj = true;
          }
        }
        else {
          this.EnableDisablePaymentForOrderAdj = true;
          this.PaymentHeader = "Receipts";
          if (this.SalesBilling.lstOfPayToCustomer.length > 0) {
            this.EnablePayToCust = true;
          }
          else {
            this.EnablePayToCust = false;
          }
        }
        this.autoFetchAmount = this.BalAmount;
      }
      return this.BalAmount
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
    }
  }

  PaymentHeader: string = "";
  PaidBySK: number = 0;

  PaidByShopKeeper(arg: any[] = []) {
    let total = 0;
    if (arg.length > 0) {
      arg.forEach((d) => {
        total += Number(d.PayAmount);
      });
      this.PaymentHeader = "Payments";
      this.PaidBySK = total;
      this.DeriveEstimationBalances.TotalPaymentAmount = this.PaidBySK;
    }
    else {
      this.PaymentHeader = "Payments";
      this.PaidBySK = total;
      this.DeriveEstimationBalances.TotalPaymentAmount = this.PaidBySK;
    }
    return total;
  }

  FinAmt: number = 0;

  EnablePaymentsTab: boolean = false;

  billtype: any = [];

  getBillTypes() {
    this._newSalesBillingService.getBillType().subscribe(
      response => {
        this.billtype = response;
        this.SalesBilling.BillType = "N";
      }
    )
  }

  TogglePaymentData() {
    if (this.estDetails != null) {
      this.FinAmt = this.estDetails.Balance;
      if (this.PaymentSummary != null) {
        this.FinAmt = Math.round(this.PaymentSummary.Amount - this.FinAmt);
        if (this.FinAmt > 1) {
          this.EnablePaymentsTab = true;
        }
        else {
          this.EnablePaymentsTab = !this.EnablePaymentsTab;
        }
      }
    }
  }

  EnableShopKeeperTab: boolean = true;

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

  offerDiscount: number = 0.00;

  clearValues() {
    this.DiscPer = 0.00;
    this.offerDiscount = 0.00;
    this.McDiscAmt = 0.00;
    this.AddDiscAmt = 0.00;
    this.EnableBilling = false;
    this.estDetails = [];
    this.BalAmount = 0.00;
    this.PaidBySK = 0.00;
    this.FinAmt = 0.00;
    this.PaymentSummary = [];
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
    this.DeriveEstimationBalances = {
      CompanyCode: this.ccode,
      BranchCode: this.bcode,
      EstNo: 0,
      CalculateFinalAmount: false,
      TotalReceiptAmount: 0.00,
      TotalPaymentAmount: 0.00,
      DifferenceDiscountAmt: 0.00,
      ReceivableAmount: 0.00,
      RowVersion: ""
    }
  }

  BillNo: any;
  CurrentRowRevision: any;
  RowRevision: any;
  GuaranteeFlag: boolean = false;
  RndOffAmtFlag: boolean = false;
  leavePage: boolean = false;
  OTPData: any;
  timeLeft: number = 120;

  Submit() {
    if (this.SalesBilling.CustID == 0) {
      swal("Warning!", 'Please select the customer', "warning");
    }
    else {
      if (this.BillNo == null) {
        if (this.model.option == "Order") {
          var ans = confirm("Do you want to save??");
          if (ans) {
            this.SpinnerService.show();
            this._newSalesBillingService.postSalesBilling(this.SalesBilling).subscribe(
              response => {
                this.BillNo = response;
                if (this.BillNo != null) {
                  this.EnableReprint = true;
                  //swal("Submitted!", "Bill No: " + this.BillNo.billNo + " generated successfully!", "success");
                  swal("Submitted!", "New Order No: " + this.BillNo.orderNo + " and Receipt No: " + this.BillNo.receiptNo + " generated successfully!", "success");
                  this.clearValues();
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
                  //console.log(this.OTPData);
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
        else {
          if (this.GuaranteeFlag == true) {
            var ans = confirm("Do you want to save??");
            if (ans) {
              this.SpinnerService.show();
              this._newSalesBillingService.postSalesBilling(this.SalesBilling).subscribe(
                response => {
                  this.BillNo = response;
                  if (this.BillNo != null) {
                    this.EnableReprint = true;
                    swal("Submitted!", "Bill No: " + this.BillNo.billNo + " generated successfully!", "success");
                    this.clearValues();
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
                    //console.log(this.OTPData);
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
          else {
            var bAmt;
            if (Number(this.BalanceAmount()) > 0 && this.PaymentHeader != 'Payments') {
              var ans = confirm("Received amount is less than the sales amount. Do you want to generate Credit Bill??");
              if (ans) {
                $('#GuaranteePopup').modal('show');
                this.GuaranteeFlag = true;
              }
            }
            else {
              var ans = confirm("Do you want to save??");
              if (ans) {
                this.SpinnerService.show();
                this._newSalesBillingService.postSalesBilling(this.SalesBilling).subscribe(
                  response => {
                    this.BillNo = response;
                    if (this.BillNo != null) {
                      this.EnableReprint = true;
                      swal("Submitted!", "Bill No: " + this.BillNo.billNo + " generated successfully!", "success");
                      this.clearValues();
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
                      //console.log(this.OTPData);
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

  differenceDiscountAmt: number = 0.00;
  Recalculation: any = [];
  RecAmtEnable: boolean = false;
  ReceivedFinalAmt: number = 0;
  DiscountApplied: boolean = false;
  DisAmt: number = 0;
  DiscPer: number = 0.00;
  McDiscAmt: number = 0.00;
  AddDiscAmt: number = 0.00;
  CessAmt: number = 0.00;
  SalesAmountExclTax: number = 0.00;
  GSTAmount: number = 0.00;
  SalesAmountInclTax: number = 0.00;


  ReceivedAmt(RecAmt) {
    this.differenceDiscountAmt = Number((this.diffDisAmtForCalc - RecAmt).toFixed(2));
    if (RecAmt > 0) {
      this.DeriveEstimationBalances.CalculateFinalAmount = true;
      this.DeriveEstimationBalances.ReceivableAmount = RecAmt;
      this._newSalesBillingService.PostDeriveEstimationBalances(this.DeriveEstimationBalances).subscribe(
        response => {
          this.Recalculation = response;
          if (this.Recalculation != null) {
            this.DiscPer = Number((this.Recalculation.SalesInvoiceAttribute.DiscountPercent).toFixed(2));
            this.McDiscAmt = this.Recalculation.SalesInvoiceAttribute.MCDiscount;
            this.AddDiscAmt = this.Recalculation.SalesInvoiceAttribute.DiscountAmount;
            this.CessAmt = this.Recalculation.SalesInvoiceAttribute.GSTCessAmount;
            this.SalesAmountExclTax = this.Recalculation.SalesInvoiceAttribute.SalesAmountExclTax;
            this.GSTAmount = this.Recalculation.SalesInvoiceAttribute.GSTAmount;
            this.SalesAmountInclTax = this.Recalculation.SalesInvoiceAttribute.SalesAmountInclTax;
            this.LineAmountBeforeTax = this.Recalculation.SalesInvoiceAttribute.SalesAmountExclTax;
            this.TaxAmount = this.Recalculation.SalesInvoiceAttribute.GSTAmount;
            this.LineAmountAfterTax = this.Recalculation.SalesInvoiceAttribute.SalesAmountInclTaxWithoutRoundOff;
            this.TotalAmount = this.Recalculation.GrossAmount;
            this.SalesBilling.salesInvoiceAttribute = this.Recalculation.SalesInvoiceAttribute;
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
      this.DiscountApplied = false;
    }

  }

  // TotalAmountCalc() {
  //   let totAmount = 0;
  //   if (this.estDetails != null) {
  //     if (this.Recalculation.SalesInvoiceAttribute != null) {
  //       totAmount = this.LineAmountAfterTax;
  //     }
  //     else {
  //       totAmount = this.estDetails.LineAmountAfterTax;
  //     }
  //     if (this.estDetails.InlinePurchase != null) {
  //       totAmount = totAmount - this.estDetails.InlinePurchase.LineAmountAfterTax;
  //     }
  //     if (this.estDetails.AttachedSalesReturn.AttachmentLines.length > 0) {
  //       totAmount = totAmount - this.estDetails.AttachedSalesReturn.TotalAmount;
  //     }
  //     if (this.estDetails.AttachedCustomerOrder.AttachmentLines.length > 0) {
  //       totAmount = totAmount - this.estDetails.AttachedCustomerOrder.TotalAmount;
  //     }
  //     if (this.estDetails.AttachedOldPurchase.AttachmentLines.length > 0) {
  //       totAmount = totAmount - this.estDetails.AttachedOldPurchase.TotalAmount;
  //     }
  //   }
  //   return totAmount;
  // }

  diffDisAmtForCalc: number = 0.00;

  paymentAmt(arg) {
    this.differenceDiscountAmt += arg - this.diffDisAmtForCalc * -1;
    this.DeriveEstimationBalances.CalculateFinalAmount = true;
    this.DeriveEstimationBalances.ReceivableAmount = arg * -1;
    this._newSalesBillingService.PostDeriveEstimationBalances(this.DeriveEstimationBalances).subscribe(
      response => {
        this.Recalculation = response;
        if (this.Recalculation != null) {
          this.DiscPer = Number((this.Recalculation.SalesInvoiceAttribute.DiscountPercent).toFixed(2));
          this.AddDiscAmt = this.Recalculation.SalesInvoiceAttribute.DiscountAmount;
          this.CessAmt = this.Recalculation.SalesInvoiceAttribute.GSTCessAmount;
          this.SalesAmountExclTax = this.Recalculation.SalesInvoiceAttribute.SalesAmountExclTax;
          this.GSTAmount = this.Recalculation.SalesInvoiceAttribute.GSTAmount;
          this.SalesAmountInclTax = this.Recalculation.SalesInvoiceAttribute.SalesAmountInclTax;
          this.LineAmountBeforeTax = this.Recalculation.SalesInvoiceAttribute.SalesAmountExclTax;
          this.TaxAmount = this.Recalculation.SalesInvoiceAttribute.GSTAmount;
          this.LineAmountAfterTax = this.Recalculation.SalesInvoiceAttribute.SalesAmountInclTaxWithoutRoundOff;
          this.TotalAmount = this.Recalculation.GrossAmount;
          this.SalesBilling.salesInvoiceAttribute = this.Recalculation.SalesInvoiceAttribute;
        }
      },
      (err) => {
        this.DiscountApplied = false;
        // this.PayAmtEnable = false;
        this.PayAmt.nativeElement.focus();
        //Added on 30-Mar-2021 due to Payment Discount Amount getting disable after operator discount error from api
        if (this.ReceivedFinalAmt > 0) {
          this.EnablePaymentsTab = true;
        }
        else {
          this.differenceDiscountAmt = 0;
        }
        //ends here
      }
    )
  }


  @ViewChild("estNo", { static: false }) estNo: ElementRef;

  AttachedInfoBill: any = [];

  getAttachedBillForSalesBilling() {
    this.estNo.nativeElement.value = "";
    this._newSalesBillingService.getAttachedInfoBill(this.BillNo.billNo).subscribe(
      response => {
        this.AttachedInfoBill = response;
      }
    )
  }

  interval;


  startTimer() {
    this.interval = setInterval(() => {
      if (this.timeLeft > 0) {
        this.timeLeft--;
      } else {
        this.timeLeft = 120;
      }
    }, 1000)
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

  cancelModal() {
    $('#GuaranteePopup').modal('hide');
    this.GuaranteeFlag = false;
    this.RecAmt.nativeElement.focus();
  }

  updateOTP(arg, index, otp) {
    this._newSalesBillingService.ValidateOTP(arg.MobileNo, arg.OrderNo, arg.SMSID, otp).subscribe(
      response => {
        this.OTPData[index].IsValidated = true;
        this.SalesBilling.OrderOTPAttributes.push(this.OTPData[index]);
      },
      (err) => {
        this.OTPData[index].IsValidated = false;
      }
    )
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

}