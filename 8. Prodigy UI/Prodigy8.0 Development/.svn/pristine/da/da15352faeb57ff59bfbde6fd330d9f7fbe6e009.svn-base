import { SalesBillingService } from '../sales-billing.service';
import { Component, OnInit, OnDestroy, Input, Output, EventEmitter, OnChanges, ViewChild, ElementRef } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { ComponentCanDeactivate } from '../../appconfirmation-guard';
import { BillReceiptModel } from '../sales-billing.model';
import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import { formatDate } from '@angular/common';
import { PaymentService } from '../../payment/payment.service';
import { OrdersService } from '../../orders/orders.service';
import { ToastrService } from 'ngx-toastr';
import { SalesreturnService } from '../../salesreturn/salesreturn.service';
import { PurchaseService } from '../../purchase/purchase.service';
import { MasterService } from '../../core/common/master.service';
declare var $: any;

@Component({
  selector: 'app-bill-receipt-details',
  templateUrl: './bill-receipt-details.component.html',
  styleUrls: ['./bill-receipt-details.component.css']
})
export class BillReceiptDetailsComponent implements OnInit, ComponentCanDeactivate {
  @ViewChild("BillNo", { static: true }) BillNo: ElementRef;
  leavePage: boolean = false;
  EnableJson: boolean = false;
  billReceiptDetails: any;
  pageName: string;
  EnableDisablePaymentForOrderAdj: boolean = false;
  password: string;
  ccode: string = "";
  bcode: string = "";
  BalanceAmount: number = 0;
  PaymentForm: FormGroup;
  orderNo: string = null;
  modifyOrderNo: string = null;
  SchemeAdjustmentForm: FormGroup;
  AttachOldGoldForm: FormGroup;
  SearchParamsList: any = [];
  SearchSRBillParamsList: any = [];
  SearcList: any = [];
  datePickerConfig: Partial<BsDatepickerConfig>;
  ExpirydatePickerConfig: Partial<BsDatepickerConfig>;

  EnableSubmitButton: boolean = true;
  EnableDisPayControls: boolean = true;
  EnableReadOnlyControls: boolean = false;

  AttachOrderForm: FormGroup;
  AttachPurBillForm: FormGroup;
  AttachSREstForm: FormGroup;
  AttachSRBillForm: FormGroup;
  readonly: {};
  placeholder: string = "";
  enableClosedOrderDetails: boolean = false;
  SchemeData: any = [];

  //sixmonths = new Date(this.today.setMonth(this.today.getMonth() + 6));

  argPageName: string = "S";


  BillReceipt: BillReceiptModel = {
    CompanyCode: null,
    BranchCode: null,
    BillNo: 0,
    AddPayments: []
  }


  constructor(private _paymentService: PaymentService, private formBuilder: FormBuilder,
    private _orderservice: OrdersService, private _salesBillingService: SalesBillingService,
    private Toastr: ToastrService, private _purchaseService: PurchaseService,
    private _salesreturnService: SalesreturnService, private _masterService: MasterService,
    private _router: Router, private _appConfigService: AppConfigService) {
    let today = new Date();
    let minMonth = today.getMonth();
    let minYear = today.getFullYear();
    let minDate = today.getDate();

    let sixmonths = new Date(today.setMonth(today.getMonth() + 3));
    let maxMonth = sixmonths.getMonth();
    let maxYear = sixmonths.getFullYear();
    let maxDate = sixmonths.getDate();

    let minimumDate = new Date(minYear, minMonth, minDate);
    let maximumDate = new Date(maxYear, maxMonth, maxDate);

    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        minDate: minimumDate,
        maxDate: maximumDate,
        dateInputFormat: 'YYYY-MM-DD'
      });

    this.ExpirydatePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        minDate: minimumDate,
        dateInputFormat: 'DD-MM-YYYY'
      });
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
    this.getCB()
  }

  ngOnInit() {
    this.AttachOrderForm = this.formBuilder.group({
      searchby: [null, Validators.required],
      searchText: [null, Validators.required],
    });

    this.AttachOldGoldForm = this.formBuilder.group({
      searchby: [null, Validators.required],
      searchText: [null, Validators.required],
    });

    this.AttachPurBillForm = this.formBuilder.group({
      searchby: [null, Validators.required],
      searchText: [null, Validators.required],
    });

    this.AttachSREstForm = this.formBuilder.group({
      searchby: [null, Validators.required],
      searchText: [null, Validators.required],
    });

    this.AttachSRBillForm = this.formBuilder.group({
      searchby: [null, Validators.required],
      searchText: [null, Validators.required],
    });

    this._purchaseService.getSearchParams().subscribe(
      response => {
        this.SearcList = response;
        for (let i = 0; i < this.SearcList.length; i++) {
          if (i < 6) {
            this.SearchParamsList.push(this.SearcList[i]);
          }
        }
      }
    )

    this._paymentService.getSearchParamsSRBill().subscribe(
      response => {
        this.SearchSRBillParamsList = response;
      }
    )

    this.getCardType();
    this.getPaymentMode();
    this.getBank();
    this.getSalesMan();
    this.getApplicationdate();

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

    this.SchemeAdjustmentForm = this.formBuilder.group({
      BranchCode: null,
      SchemeCode: null,
      GroupCode: null,
      StartMSNNo: null,
      EndMSNNo: null,
      ChitAmount: null,
      TotalAmount: null,
      BonusAmount: null,
      WinnerAmount: null,
      PayMode: null
    });

    this._orderservice.castModifyOrderDetsToOrderComp.subscribe(
      response => {
        this.modifyOrderNo = response;
        if (this.modifyOrderNo != null) {
          this.EnableDisPayControls = false;
        }
      }
    )
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
  orderDetails: any = [];
  previousPaymentDetails: number;
  SelectedItemLines: any = [];
  customerDtls: any = [];
  getBillReceiptDetails(arg) {
    if (arg == '') {
      swal("Warning!", 'Please enter valid credit bill no', "warning");
      this.billReceiptDetails = null;
    }
    else {
      this._salesBillingService.getByBillNo(arg).subscribe(
        response => {
          this.billReceiptDetails = response;
          if (this.billReceiptDetails != null) {
            this.leavePage = true;
            this.billReceiptDetails.BillDate = formatDate(this.billReceiptDetails.BillDate, 'dd/MM/yyyy', 'en_GB');
            this.billReceiptDetails.TotalSaleAmount = Math.round(this.billReceiptDetails.TotalSaleAmount);
            this.EnableDisablePaymentForOrderAdj = true;
            this.pageName = "S";
            this.BillReceipt.CompanyCode = this.ccode;
            this.BillReceipt.BranchCode = this.bcode;
            this.BillReceipt.BillNo = this.billReceiptDetails.BillNo;
            this.BillReceipt.AddPayments = this.billReceiptDetails.PaymentVM;
            this.ArrayList = this.BillReceipt.AddPayments;
          }
        },
        (err) => {
          if (err.ErrorStatusCode == 400) {
            const validationError = err.error;
            swal("!Warning", validationError.description, "warning");
            this.billReceiptDetails = [];
          }
          this.clear();
        }
      )
    }
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  getBankdetailsonchange(arg) {
    // this.PaymentHeaders.BankName = arg;
    this.PaymentHeaders.Bank = arg;
  }

  PaymentModes: any;
  getPaymentMode() {
    this._salesBillingService.getBillReceiptPayModes().subscribe(
      response => {
        this.PaymentModes = response;
      }
    )
  }

  CardType: any;
  getCardType() {
    this._masterService.getCardType().subscribe(
      response => {
        this.CardType = response;
      }
    )
  }

  PaymentSummaryData: any = {
    Amount: null
  };

  BankList: any;
  getBank() {
    this._paymentService.getBank().subscribe(
      response => {
        this.BankList = response;
      }
    )
  }

  ngOnDestroy() {
    this._paymentService.inputData(null);
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


  SchemeAdjusment: any = {
    BranchCode: null,
    SchemeCode: null,
    GroupCode: null,
    StartMSNNo: null,
    EndMSNNo: null,
    ChitAmount: null,
    TotalAmount: null,
    BonusAmount: null,
    WinnerAmount: null,
    PayMode: "CT"
  }

  openChitAdjustment() {
    if (this.toggle == "SR") {
      swal("Warning!", 'Reference No not found for SR Bill', "warning");
    }
    else {
      $('#ChitAdjustment').modal('show');
      this.SchemeAdjustmentForm.reset();
      this.getBranchList();
      this.SchemeList = null;
      this.GroupList = null;
      this.MSNNoList = null;
      this.MSNNoList = null;
      this.chitAmountList = null;
    }
  }

  BranchList: any = [];
  getBranchList() {
    this._paymentService.getBranch().subscribe(
      response => {
        this.BranchList = response;
      }
    )
  }

  SchemeList: any = [];
  BranchCode: string = "";
  getScheme(BranchCode) {
    this.BranchCode = BranchCode;
    this._paymentService.getScheme(this.BranchCode).subscribe(
      response => {
        this.SchemeList = response;
      }
    )
  }

  GroupList: any = [];
  SchemeCode: string = "";
  getGroup(SchemeCode) {
    this.SchemeCode = SchemeCode;
    this._paymentService.getGroup(this.BranchCode, this.SchemeCode).subscribe(
      response => {
        this.GroupList = response;
      }
    )
  }

  MSNNoList: any = [];
  GroupCode: string = "";
  getMSNNo(GroupCode) {
    this.GroupCode = GroupCode;
    this._paymentService.getMSNNo(this.BranchCode, this.SchemeCode, this.GroupCode).subscribe(
      response => {
        this.MSNNoList = response;
      }
    )
  }

  StartMSNo: string = "";
  getStartMSNo(StartMSNo) {
    this.StartMSNo = StartMSNo;
  }

  ChitAmount: number = 0;
  TotalAmount: number = 0;
  BonusAmount: number = 0;
  WinnerAmount: number = 0;
  chitAmountList: any = [];
  EndMSNo: string = "";
  getchitAmount(EndMSNo) {
    this.EndMSNo = EndMSNo;
    this._paymentService.getchitAmount(this.BranchCode, this.SchemeCode, this.GroupCode, this.StartMSNo, EndMSNo).subscribe(
      response => {
        this.chitAmountList = response;
        this.SchemeAdjusment.ChitAmount = this.chitAmountList.ChitAmount;
        this.SchemeAdjusment.TotalAmount = this.chitAmountList.TotalAmount;
        this.SchemeAdjusment.BonusAmount = this.chitAmountList.BonusAmount;
        this.SchemeAdjusment.WinnerAmount = this.chitAmountList.WinnerAount;
      }
    )
  }

  toggle: string = "Invalid";
  onchangePaymode(paymodeArg) {
    this.PaymentHeaders.PayDetails = null;
    this.PaymentHeaders.PayAmount = null;
    this.PaymentHeaders.BankName = null;
    this.PaymentHeaders.RefBillNo = null;
    this.PaymentHeaders.CardType = null;
    this.PaymentHeaders.Bank = null;
    this.readonly = false;
    this.placeholder = "Enter Amount Received";
    switch (paymodeArg) {
      case "Q": {
        this.toggle = "Cheque";
        if (this.argPageName == "OC") {
          this.getChequeName();
        }
        break;
      }
      case "D": {
        this.toggle = "DD";
        break;
      }
      case "C": {
        this.toggle = "Cash";
        break;
      }
      case "R": {
        this.toggle = "Card";
        break;
      }
      case "CT": {
        this.toggle = "Scheme";
        this.readonly = true;
        this.placeholder = "";
        break;
      }
      case "BC": {
        this.toggle = "BC";
        break;
      }
      case "GV": {
        this.toggle = "GV";
        break;
      }
      case "SR": {
        this.toggle = "SRBill";
        break;
      }
      case "EP": {
        this.toggle = "EP";
        break;
      }
      case "OP": {
        this.toggle = "OrdAdj";
        break;
      }
      case "PE": {
        this.toggle = "PurEst";
        break;
      }
      case "PB": {
        this.toggle = "PurBill";
        break;
      }
      case "SE": {
        this.toggle = "SREst";
        break;
      }
      case "SR": {
        this.toggle = "SRBill";
        break;
      }
      case "SP": {
        this.toggle = "SalesPromotion";
        break;
      }
      case "UPI": {
        this.toggle = "UPI Payment";
        break;
      }
      case "CN": {
        this.toggle = "CREDIT NOTE";
        break;
      }
      case "BD": {
        this.toggle = "DKN";
        break;
      }
      case "BU": {
        this.toggle = "UDP";
        break;
      }
      case "BK": {
        this.toggle = "KRM";
        break;
      }
      case "BJ": {
        this.toggle = "JNR";
        break;
      }
      case "BH": {
        this.toggle = "HBR";
        break;
      }
      case "BM": {
        this.toggle = "MNG";
        break;
      }
      case "BN": {
        this.toggle = "HSN";
        break;
      }
      case "BP": {
        this.toggle = "PNX";
        break;
      }
      case "BS": {
        this.toggle = "DKNBS";
        break;
      }
      case "BMT": {
        this.toggle = "MRT";
        break;
      }
      case "BSM": {
        this.toggle = "SMG";
        break;
      }
      default: {
        this.toggle = "Invalid";
        break;
      }
    }
  }


  payMode(arg) {
    switch (arg) {
      case "Q": {
        this.toggle = "Cheque";
        if (this.argPageName == "OC") {
          this.getChequeName();
        }
        break;
      }
      case "D": {
        this.toggle = "DD";
        break;
      }
      case "C": {
        this.toggle = "Cash";
        break;
      }
      case "R": {
        this.toggle = "Card";
        break;
      }
      case "CT": {
        this.toggle = "Scheme";
        this.readonly = true;
        this.placeholder = "";
        break;
      }
      case "BC": {
        this.toggle = "BC";
        break;
      }
      case "GV": {
        this.toggle = "GV";
        break;
      }
      case "SR": {
        this.toggle = "SRBill";
        break;
      }
      case "EP": {
        this.toggle = "EP";
        break;
      }
      case "OP": {
        this.toggle = "OrdAdj";
        break;
      }
      case "PE": {
        this.toggle = "PurEst";
        break;
      }
      case "PB": {
        this.toggle = "PurBill";
        break;
      }
      case "SE": {
        this.toggle = "SREst";
        break;
      }
      case "SR": {
        this.toggle = "SRBill";
        break;
      }
      case "SP": {
        this.toggle = "SalesPromotion";
        break;
      }
      case "UPI": {
        this.toggle = "UPI Payment";
        break;
      }
      case "CN": {
        this.toggle = "CREDIT NOTE";
        break;
      }
      case "BD": {
        this.toggle = "DKN";
        break;
      }
      case "BU": {
        this.toggle = "UDP";
        break;
      }
      case "BK": {
        this.toggle = "KRM";
        break;
      }
      case "BJ": {
        this.toggle = "JNR";
        break;
      }
      case "BH": {
        this.toggle = "HBR";
        break;
      }
      case "BM": {
        this.toggle = "MNG";
        break;
      }
      case "BN": {
        this.toggle = "HSN";
        break;
      }
      case "BP": {
        this.toggle = "PNX";
        break;
      }
      case "BS": {
        this.toggle = "DKNBS";
        break;
      }
      case "BMT": {
        this.toggle = "MRT";
        break;
      }
      case "BSM": {
        this.toggle = "SMG";
        break;
      }
      default: {
        this.toggle = "Invalid";
        break;
      }
    }
  }

  ApplicationDate: any;
  getApplicationdate() {
    this._orderservice.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.ApplicationDate = appDate["applcationDate"]
        this.PaymentHeaders.ChequeDate = this.ApplicationDate;
      }
    )
  }



  // OrderDetails: any;
  DuplicateSchemeAdj: boolean = false;
  ArrayList: any = [];
  PaymentList: any = [];
  addRow(arrayLines) {
    if (this.toggle == "Cash" || this.toggle == "BC" || this.toggle == "EP" || this.toggle == "GV" || this.toggle == "SR" || this.toggle == "OrdAdj" || this.toggle == "PurEst" || this.toggle == "PurBill" || this.toggle == "SREst" || this.toggle == "SRBill" || this.toggle == "SalesPromotion" || this.toggle == "UPI Payment" || this.toggle == "CREDIT NOTE") {
      if (this.toggle == "BC" && (this.PaymentHeaders.PayDetails == null || this.PaymentHeaders.PayDetails == "")) {
        swal("Warning!", 'Please Enter Details', "warning");
      }
      else if (this.PaymentHeaders.PayAmount == null || this.PaymentHeaders.PayAmount == "" || this.PaymentHeaders.PayAmount == 0) {
        swal("Warning!", 'Please enter the amount', "warning");
      }
      else {
        this.SaveAddrow(arrayLines, this.toggle);
      }
    }
    else if (this.toggle == "Card") {
      if (this.PaymentHeaders.RefBillNo == null) {
        swal("Warning!", 'Please enter the card number', "warning");
      }
      else if (this.PaymentHeaders.CardType == null) {
        swal("Warning!", 'Please select the card type', "warning");
      }
      else if (this.PaymentHeaders.Bank == null) {
        swal("Warning!", 'Please select the bank', "warning");
      }
      else if (this.PaymentHeaders.RefBillNo.length < 4) {
        swal("Warning!", 'Please enter valid card no', "warning");
      }
      else if (this.PaymentHeaders.PayAmount == null) {
        swal("Warning!", 'Please enter the amount', "warning");
      }
      else if (this.PaymentHeaders.CardAppNo == null || this.PaymentHeaders.CardAppNo == "") {
        swal("Warning!", 'Please enter card approval no', "warning");
      }
      else {
        this.SaveAddrow(arrayLines, this.toggle);
      }
    }
    else if (this.toggle == "Cheque") {
      if (this.PaymentHeaders.BankName == null || this.PaymentHeaders.BankName == "") {
        swal("Warning!", 'Please enter valid bank name', "warning");
      }
      else if (this.PaymentHeaders.ChequeNo == null) {
        swal("Warning!", 'Please enter the cheque number', "warning");
      }
      else if (this.PaymentHeaders.ChequeNo.length < 6) {
        swal("Warning!", 'Please enter valid Cheque/DD No', "warning");
      }
      else if (this.PaymentHeaders.PayAmount == null || this.PaymentHeaders.PayAmount == "" || this.PaymentHeaders.PayAmount == 0) {
        swal("Warning!", 'Please enter the amount', "warning");
      }
      else {
        this.SaveAddrow(arrayLines, this.toggle);
      }
    }
    else if (this.toggle == "DD") {
      if (this.PaymentHeaders.BankName == null || this.PaymentHeaders.BankName == "") {
        swal("Warning!", 'Please enter valid bank name', "warning");
      }
      else if (this.PaymentHeaders.ChequeNo == null) {
        swal("Warning!", 'Please enter the DD number', "warning");
      }
      else if (this.PaymentHeaders.ChequeNo.length < 6) {
        swal("Warning!", 'Please enter valid Cheque/DD No', "warning");
      }
      else if (this.PaymentHeaders.PayAmount == null || this.PaymentHeaders.PayAmount == "" || this.PaymentHeaders.PayAmount == 0) {
        swal("Warning!", 'Please enter the amount', "warning");
      }
      else {
        this.SaveAddrow(arrayLines, this.toggle);
      }
    }
    else if (this.toggle == "DKN" || this.toggle == "UDP" || this.toggle == "KRM" || this.toggle == "JNR" || this.toggle == "HBR" || this.toggle == "MNG" || this.toggle == "HSN" || this.toggle == "PNX" || this.toggle == "DKNBS" || this.toggle == "MRT" || this.toggle == "SMG") {
      if (this.PaymentHeaders.PayAmount == null || this.PaymentHeaders.PayAmount == "") {
        swal("Warning!", 'Please enter the amount', "warning");
      }
      else if (this.PaymentHeaders.RefBillNo == null || this.PaymentHeaders.RefBillNo == "") {
        swal("Warning!", 'Please enter the valid refno', "warning");
      }
      else if (this.PaymentHeaders.PayDetails == null || this.PaymentHeaders.PayDetails == "") {
        swal("Warning!", 'Please enter narration', "warning");
      }
      else {
        this.SaveAddrow(arrayLines, this.toggle);
      }
    }
    else if (this.toggle == "Scheme") {
      if (this.PaymentHeaders.PayAmount == null || this.PaymentHeaders.PayAmount == "" || this.PaymentHeaders.PayAmount == "" || this.PaymentHeaders.PayAmount == 0) {
        swal("Warning!", 'Please enter the amount', "warning");
      }
      else {
        this._paymentService.getchitDetails(this.SchemeAdjustmentForm.value.BranchCode, this.SchemeAdjustmentForm.value.SchemeCode,
          this.SchemeAdjustmentForm.value.GroupCode, this.SchemeAdjustmentForm.value.StartMSNNo, this.SchemeAdjustmentForm.value.EndMSNNo).subscribe(
            response => {
              this.SchemeData = response;
              if (this.SchemeData != null) {
                this.DuplicateSchemeAdj = false;
                if (this.billReceiptDetails.lstOfPayment.length > 0) {
                  for (var i = 0; i < this.SchemeData.length; i++) {
                    let SchemeAdjData = this.billReceiptDetails.PaymentVM.find(x => x.RefBillNo == this.SchemeData[i].RefBillNo);
                    if (SchemeAdjData != null) {
                      swal("error", "Reference bill is already added", "error");
                      this.DuplicateSchemeAdj = true;
                    }
                  }
                  if (this.DuplicateSchemeAdj == false) {
                    this.saveSchemeAdjustData();
                  }
                }
                else {
                  if (this.DuplicateSchemeAdj == false) {
                    this.saveSchemeAdjustData();
                  }
                }
              }
            }
          )
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

  ChequeNo: any = [];
  getChequeNumber(arg) {
    this._paymentService.getChequeList(arg).subscribe(
      response => {
        this.ChequeNo = response;
      }
    )
  }

  saveSchemeAdjustData() {
    for (var i = 0; i < this.SchemeData.length; i++) {
      this.PaymentHeaders.PayMode = this.SchemeData[i].PayMode;
      this.PaymentHeaders.RefBillNo = this.SchemeData[i].RefBillNo;
      // this.PaymentHeaders.CTBranch = this.SchemeData[i].CTBranch;
      this.PaymentHeaders.CTBranch = this.bcode;
      this.PaymentHeaders.PartyCode = this.SchemeData[i].CTBranch;
      this.PaymentHeaders.PayAmount = this.SchemeData[i].PayAmount;
      // this.PaymentHeaders.PayDetails = this.SchemeData[i].GroupCode ;
      this.PaymentHeaders.PayDetails = this.SchemeData[i].SchemeCode + '/' + this.SchemeData[i].GroupCode + '/' + this.SchemeData[i].RefBillNo;
      this.PaymentHeaders.GroupCode = this.SchemeData[i].GroupCode;
      this.PaymentHeaders.SchemeCode = this.SchemeData[i].SchemeCode;
      this.PaymentHeaders.PayAmountBeforeTax = this.SchemeData[i].PayAmount;
      this.PaymentHeaders.CompanyCode = this.ccode;
      this.PaymentHeaders.BranchCode = this.bcode;
      this.PaymentHeaders.OperatorCode = localStorage.getItem('Login');
      this.PaymentHeaders.AmtReceived = this.SchemeData[i].PayAmount;
      this.PaymentHeaders.CardSwipedBy = "";
      this.billReceiptDetails.PaymentVM.push(this.PaymentHeaders);
      this._paymentService.outputData(this.PaymentHeaders);
      this.PaymentHeaders = {
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
    }
    this.EnableSubmitButton = false;
    this._paymentService.SendPaymentSummaryData(this.PaymentSummaryData);
    this.EnableSubmitButton = false;
    this.toggle = "Invalid"
    this.PaymentForm.reset();
    //send full JSON data to subject
    this.payAmount = 0;
  }

  //scheme Lines



  SaveAddrow(arrayLines, paymentMode) {
    if (this.billReceiptDetails.PaymentVM.length > 0) {
      let data = this.billReceiptDetails.PaymentVM.find(x => x.PayMode == arrayLines.value.PayMode);
      if (data != null && paymentMode != "Scheme" && paymentMode != "OrdAdj" && paymentMode != "PurEst" && paymentMode != "PurBill" && paymentMode != "SREst" && paymentMode != "SRBill" && paymentMode != "Card" && paymentMode != "EP" && paymentMode != "UPI Payment" && paymentMode != "Cheque" && this.EditPayment == false) {
        swal("error", paymentMode + " mode is already added. Please select other mode of payment", "error");
      }
      else {
        if (paymentMode == "Scheme") {
          let SchemeAdjData = this.billReceiptDetails.PaymentVM.find(x => x.RefBillNo == arrayLines.value.RefBillNo);
          if (SchemeAdjData != null) {
            swal("error", "Reference bill is already added", "error");
          }
          else {
            this.SavePaymentData(arrayLines, paymentMode);
          }
        }
        else if (paymentMode == "OrdAdj" || paymentMode == "PurEst" || paymentMode == "PurBill" || paymentMode == "SREst" || paymentMode == "SRBill") {
          let OutputData = this.billReceiptDetails.PaymentVM.find(x => x.RefBillNo == arrayLines.value.RefBillNo);
          if (OutputData != null) {
            swal("error", "Reference bill is already added", "error");
          }
          else {
            this.SavePaymentData(arrayLines, paymentMode);
          }
        }
        else {
          this.SavePaymentData(arrayLines, paymentMode);
        }
      }
    }
    else {
      this.SavePaymentData(arrayLines, paymentMode);
    }
  }

  payAmount: number = 0;
  SavePaymentData(arrayLines, paymentMode) {
    this.PaymentData(arrayLines, paymentMode);
  }

  PaymentLength: Number = 0;

  PaymentData(arrayLines, paymentMode) {
    if (paymentMode == "Card") {
      arrayLines.value.RefBillNo = arrayLines.value.RefBillNo; //Added since field Card No/Ref.No is common in grid for both card and reference no
    }
    else if (paymentMode == "OrdAdj" || paymentMode == "PurEst" || paymentMode == "PurBill" || paymentMode == "SREst" || paymentMode == "SRBill") { //Added on 17-Mar-2020
      arrayLines.value.TransType = "S"; //Added on 17-Mar-2020
    }
    arrayLines.value.CompanyCode = this.ccode;
    arrayLines.value.BranchCode = this.bcode;
    arrayLines.value.SeriesNo = 0;
    arrayLines.value.ReceiptNo = 0;
    arrayLines.value.SNo = 0;
    arrayLines.value.OperatorCode = localStorage.getItem('Login');
    if (this.EditPayment == true) {
      this.billReceiptDetails.PaymentVM[this.Editindex] = arrayLines.value;
    }
    else {
      this.billReceiptDetails.PaymentVM.push(arrayLines.value);
    }
    this.EnableSubmitButton = false;
    this.PaymentLength = this.billReceiptDetails.PaymentVM.length - 1;
    //arrayLines.value.TransType = this.ParentJSON.OrderType; //Commented on 17-Mar-2020
    // this._paymentService.outputData(arrayLines.value);
    // this._paymentService.SendPaymentSummaryData(this.PaymentSummaryData);
    //this.ArrayList.push(arrayLines.value);
    this.EnableSubmitButton = false;
    this.toggle = "Invalid"
    this.PaymentForm.reset();
    this.EditPayment = false;
  }


  saveSchemeAdjustment() {
    if (this.SchemeAdjusment.BranchCode == null) {
      swal("Warning!", 'Please select the branch code', "warning");
    }
    else if (this.SchemeAdjusment.SchemeCode == null) {
      swal("Warning!", 'Please select the scheme code', "warning");
    }
    else if (this.SchemeAdjusment.GroupCode == null) {
      swal("Warning!", 'Please select the group code', "warning");
    }
    else if (this.SchemeAdjusment.StartMSNNo == null) {
      swal("Warning!", 'Please select the start membership no', "warning");
    }
    else if (this.SchemeAdjusment.EndMSNNo == null) {
      swal("Warning!", 'Please select the end membership no', "warning");
    }
    else {
      this.PaymentHeaders.RefBillNo = this.SchemeAdjusment.EndMSNNo;
      this.PaymentHeaders.PayDetails = this.SchemeAdjusment.GroupCode;
      this.PaymentHeaders.PayAmount = this.SchemeAdjusment.TotalAmount;
      this.PaymentHeaders.SchemeCode = this.SchemeAdjusment.SchemeCode;
      this.PaymentHeaders.GroupCode = this.SchemeAdjusment.GroupCode;
      this.PaymentHeaders.CTBranch = this.SchemeAdjusment.BranchCode;
      this.PaymentHeaders.CompanyCode = this.ccode;
      this.PaymentHeaders.BranchCode = this.bcode;
      this.PaymentHeaders.OperatorCode = localStorage.getItem('Login');
      $('#ChitAdjustment').modal('hide');
    }
  }


  CancelSchemeAdjustment() {
    $('#ChitAdjustment').modal('hide');
  }


  SalesManList: any;
  getSalesMan() {
    this._orderservice.getSalesManData().subscribe(
      response => {
        this.SalesManList = response;
        this.PaymentHeaders.CardSwipedBy = "A";
      })
  }

  DeleteRow(index: number, arrayLines) {
    var ans = confirm("Do you want to delete");
    if (ans) {
      this.BillReceipt.AddPayments.splice(index, 1);
    }
  }

  Editindex = 0;
  EditPayment: boolean = false;

  EditRow(index: number, arrayLines) {
    this.payMode(arrayLines.PayMode);
    this.EditPayment = true;
    this.Editindex = index;
    this.EnableDisPayControls = true;
    this.PaymentHeaders = arrayLines;
  }

  Amount(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => { total += Number(d.PayAmount); });
    this.PaymentSummaryData.Amount = total;
    return total;
  }


  totalItems: any;
  pagenumber: number = 1;
  top = 50;
  skip = (this.pagenumber - 1) * this.top;
  AllRecordCount: any;
  AttachOrderList: any;
  AttachOldGoldList: any;
  SelectedOldGoldList: any = [];
  AttachPurBillList: any;
  AttachSalesReturnList: any;
  AttachSRBillList: any;
  //Order Adj Screen

  openOrderAdjustment() {
    $('#OrderAdjustment').modal('show');
    this.getOrderList(this.top, this.skip);
  }

  getOrderList(top, skip) {
    this.top = 50;
    this.skip = 0;
    this.AttachOrderForm.reset();
    this._orderservice.getAllOrdersCount().subscribe(
      response => {
        this.AllRecordCount = response;
        this.totalItems = this.AllRecordCount.RecordCount;
        if (this.totalItems > 0) {
          this._orderservice.getOrderList(top, skip).subscribe(
            response => {
              this.AttachOrderList = response;
            }
          )
        }
      }
    )
  }

  submitted = false;


  onSubmit() {
    this.submitted = true;
    if (this.AttachOrderForm.invalid) {
      return;
    }
  }

  getSearchParamsForOrderAdj(form) {
    if ((form.value.searchby != null && form.value.searchby != "") && (form.value.searchText != null && form.value.searchText != "")) {
      this._orderservice.getAttachOrderList(form.value.searchby, form.value.searchText).subscribe(
        response => {
          this.AttachOrderList = response;
          this.top = 50;
          this.skip = 0;
          this.totalItems = 50;
        },
        (err) => {
          if (err.status === 400) {
            this.AttachOrderList = [];
          }
        }
      )
    }
    else {
      swal("Warning!", 'Please select required fields', "warning");
      // this.AttachOrderList=[];
    }
  }

  onOrdAdjPageChange(p: number) {
    this.pagenumber = p;
    const skipno = (this.pagenumber - 1) * this.top;
    this.getOrderList(this.top, skipno);
  }

  OrdAdjselectRecord(arg) {
    this.PaymentHeaders.RefBillNo = arg.RefNo;
    this.PaymentHeaders.PayAmount = arg.Amount;
    this.PaymentHeaders.CompanyCode = this.ccode;
    this.PaymentHeaders.BranchCode = this.bcode;
    this.PaymentHeaders.OperatorCode = localStorage.getItem('Login');
    this.PaymentHeaders.PayMode = "OP";
    $('#OrderAdjustment').modal('hide');
  }

  //Ends here


  //Pur.Est Adjustment screen

  openPurEstAdjustment() {
    $("#PurEstAdj").modal('show');
    this.getOGList(this.top, this.skip);
  }

  getOGList(top, skip) {
    this._purchaseService.getOldGoldList(top, skip).subscribe(
      response => {
        this.AttachOldGoldList = response;
      }
    )
  }

  PurEstAdjselectRecord(arg) {
    this.PaymentHeaders.RefBillNo = arg.EstNo;
    this.PaymentHeaders.PayAmount = arg.Amount;
    this.PaymentHeaders.CompanyCode = this.ccode;
    this.PaymentHeaders.BranchCode = this.bcode;
    this.PaymentHeaders.OperatorCode = localStorage.getItem('Login');
    this.PaymentHeaders.PayMode = "PE";
    $('#PurEstAdj').modal('hide');
  }

  getSearchParamsforPurEstAdj(form) {
    if (form.value.searchby != null && form.value.searchText != null) {
      this._purchaseService.getAttachOldGoldList(form.value.searchby, form.value.searchText).subscribe(
        response => {
          this.AttachOldGoldList = response;
        }
      )
    }
  }

  PurEstAdjonPageChange(p: number) {
    this.pagenumber = p;
    const skipno = (this.pagenumber - 1) * this.top;
    this.getOGList(this.top, skipno);
  }
  //Ends here


  //Pur.Bill Adjustment screen

  SearchParamsPurBill: any = [];

  openPurBillAdjustment() {
    $('#PurBillAdj').modal('show');
    //this.getOrderList(this.top, this.skip);    
    this.AttachPurBillForm.reset();
    // this.totalItems = this.AllRecordCount.RecordCount;
    this._paymentService.getSearchParamsPurBill().subscribe(
      response => {
        this.SearchParamsPurBill = response;
        this._paymentService.getPurBillList().subscribe(
          response => {
            this.AttachPurBillList = response;
          }
        )
      }
    )
  }

  getSearchParamsforPurBillAdj(form) {
    if ((form.value.searchby != null && form.value.searchby != "") && (form.value.searchText != null && form.value.searchText != "")) {
      this._paymentService.getPurBillSearch(form.value.searchby, form.value.searchText, this.ApplicationDate).subscribe(
        response => {
          this.AttachPurBillList = response;
          this.top = 50;
          this.skip = 0;
          this.totalItems = 50;
        },
        (err) => {
          if (err.status === 400) {
            this.AttachPurBillList = [];
          }
        }
      )
    }
    else {
      swal("Warning!", 'Please select required fields', "warning");
      // this.AttachOrderList=[];
    }
  }


  getSearchParamsforSRBillAdj(form) {
    if ((form.value.searchby != null && form.value.searchby != "") && (form.value.searchText != null && form.value.searchText != "")) {
      this._paymentService.getSRBillSearch(form.value.searchby, form.value.searchText).subscribe(
        response => {
          this.AttachSRBillList = response;
          this.top = 50;
          this.skip = 0;
          this.totalItems = 50;
        },
        (err) => {
          if (err.status === 400) {
            this.AttachSRBillList = [];
          }
        }
      )
    }
    else {
      swal("Warning!", 'Please select required fields', "warning");
      // this.AttachOrderList=[];
    }
  }



  PurBillAdjonPageChange(p: number) {
    this.pagenumber = p;
    const skipno = (this.pagenumber - 1) * this.top;
  }

  PurBillAdjselectRecord(arg) {
    this.PaymentHeaders.RefBillNo = arg.BillNo;
    this.PaymentHeaders.PayAmount = arg.Amount;
    this.PaymentHeaders.CompanyCode = this.ccode;
    this.PaymentHeaders.BranchCode = this.bcode;
    this.PaymentHeaders.OperatorCode = localStorage.getItem('Login');
    this.PaymentHeaders.PayMode = "PB";
    $('#PurBillAdj').modal('hide');
    // this.PaymentHeaders.PayDetails = this.SchemeAdjusment.GroupCode;
    // this.PaymentHeaders.PayAmount = this.SchemeAdjusment.TotalAmount;
    // this.PaymentHeaders.SchemeCode = this.SchemeAdjusment.SchemeCode;
    // this.PaymentHeaders.GroupCode = this.SchemeAdjusment.GroupCode;
    // this.PaymentHeaders.CTBranch = this.SchemeAdjusment.BranchCode;   
  }

  //ends here

  //SR Est Adj Screen

  SearchSEParamsList: any = [];

  openSREstAdjustment() {
    $("#SREstModal").modal('show');
    this.SearchSEParamsList = [];
    this._purchaseService.getSearchParams().subscribe(
      response => {
        this.SearcList = response;
        for (let i = 0; i < this.SearcList.length; i++) {
          if (i < 4 && i != 1) {
            this.SearchSEParamsList.push(this.SearcList[i]);
          }
        }
        this.getSalesReturnList(this.top, this.skip);
      }
    )
  }

  getSalesReturnList(top, skip) {
    this._salesreturnService.getSRList(top, skip).subscribe(
      response => {
        this.AttachSalesReturnList = response;
      }
    )
  }

  SREstonPageChange(p: number) {
    this.pagenumber = p;
    const skipno = (this.pagenumber - 1) * this.top;
    this.getSalesReturnList(this.top, skipno);
  }

  SREstselectRecord(arg) {
    this.PaymentHeaders.RefBillNo = arg.SrEstNo;
    this.PaymentHeaders.PayAmount = arg.Amount;
    this.PaymentHeaders.CompanyCode = this.ccode;
    this.PaymentHeaders.BranchCode = this.bcode;
    this.PaymentHeaders.OperatorCode = localStorage.getItem('Login');
    this.PaymentHeaders.PayMode = "SE";
    $('#SREstModal').modal('hide');
  }

  //ends here


  //SR.Bill Adjustment screen

  openSRBillAdjustment() {
    $('#SRBillAdj').modal('show');
    this.AttachSRBillForm.reset();

    //this.getOrderList(this.top, this.skip);    
    // this.totalItems = this.AllRecordCount.RecordCount;
    this._paymentService.getSRBillList().subscribe(
      response => {
        this.AttachSRBillList = response;
      }
    )
  }

  SRBillAdjonPageChange(p: number) {
    this.pagenumber = p;
    const skipno = (this.pagenumber - 1) * this.top;
  }

  SRBillAdjselectRecord(arg) {
    this.PaymentHeaders.RefBillNo = arg.SRNo;
    this.PaymentHeaders.PayAmount = arg.Amount;
    this.PaymentHeaders.CompanyCode = this.ccode;
    this.PaymentHeaders.BranchCode = this.bcode;
    this.PaymentHeaders.OperatorCode = localStorage.getItem('Login');
    this.PaymentHeaders.PayMode = "SR";
    $('#SRBillAdj').modal('hide');
  }
  //ends here

  outputData: any;

  Submit() {
    if (this.BillReceipt.AddPayments.length == 0) {
      swal("Warning!", 'Please add the payments', "warning");
    }
    else {
      var ans = confirm("Do you want to save?");
      if (ans) {
        this._salesBillingService.postBillReceipt(this.BillReceipt).subscribe(
          response => {
            this.outputData = response;
            swal("Saved!", "Bill Details Saved Successfully!", "success");
            this.clear();
          },
          (err) => {
            if (err.status === 400) {
              this.PaymentForm.reset();
            }
          }
        )
      }
    }
  }

  clear() {
    this.BillReceipt = {
      CompanyCode: null,
      BranchCode: null,
      BillNo: 0,
      AddPayments: []
    }
    this.billReceiptDetails = null;
    this.BillNo.nativeElement.value = "";
    this.EnableDisablePaymentForOrderAdj = false;
    this.ArrayList = [];
    this.PaymentForm.reset();
  }
}