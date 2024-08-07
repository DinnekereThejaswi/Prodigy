import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { PaymentService } from '../../app/payment/payment.service';
import { Component, OnInit, OnDestroy, Input, Output, EventEmitter, OnChanges } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { SalesBillingService } from './../sales-billing/sales-billing.service';
import { OrdersService } from '../orders/orders.service';
import { PurchaseService } from './../purchase/purchase.service';
import { SalesreturnService } from './../salesreturn/salesreturn.service';
import { MasterService } from './../core/common/master.service';
import { AppConfigService } from '../AppConfigService';
import { Router } from '@angular/router';
import swal from 'sweetalert';
import * as CryptoJS from 'crypto-js';
import { ToastrService } from 'ngx-toastr';
declare var $: any;

@Component({
  selector: 'app-payment',
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.css']

})
export class PaymentComponent implements OnInit, OnChanges {

  PaymentForm: FormGroup;
  outstandinAmtForm: FormGroup;
  orderNo: string = null;
  modifyOrderNo: string = null;
  SchemeAdjustmentForm: FormGroup;
  AttachOldGoldForm: FormGroup;
  SearchParamsList: any = [];
  SearchSRBillParamsList: any = [];
  SearcList: any = [];
  datePickerConfig: Partial<BsDatepickerConfig>;
  ExpirydatePickerConfig: Partial<BsDatepickerConfig>;
  @Input() PayAmount: number = 0;
  EnableSubmitButton: boolean = true;
  EnableDisPayControls: boolean = true;
  EnableReadOnlyControls: boolean = false;
  @Input()
  OrderAmount: number = 0;
  isReadOnly: boolean = false;
  @Input()
  RepairBalanceAmount: number = 0;

  @Input()
  repairAmt: number = 0;

  @Output() PaidAmountChange = new EventEmitter();

  @Input()
  autoFillAmount: number = 0;

  @Input()
  autoFillDisAmount: number = 0;


  AttachOrderForm: FormGroup;
  AttachPurBillForm: FormGroup;
  AttachSREstForm: FormGroup;
  AttachSRBillForm: FormGroup;
  readonly: {};
  placeholder: string = "";
  EnableJson: boolean = false;
  password: string;
  enableClosedOrderDetails: boolean = false;
  ParentJSON: any = [];
  SchemeData: any = [];

  //sixmonths = new Date(this.today.setMonth(this.today.getMonth() + 6));

  @Input() public argPageName;

  constructor(private _paymentService: PaymentService, private formBuilder: FormBuilder,
    private _orderservice: OrdersService, private _salesBillingService: SalesBillingService,
    private Toastr: ToastrService, private _purchaseService: PurchaseService,
    private _salesreturnService: SalesreturnService, private _masterService: MasterService,
    private _router: Router, private _appConfigService: AppConfigService
  ) {
    this.isReadOnly = true;
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

  ccode: string = "";
  bcode: string = "";
  abc: string;

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    // alert('pagename'+ this._router.url+this.argPageName)
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
    this.outstandinAmtForm = this.formBuilder.group({
      OSAmount: null
    });


    this.getPaymentMode();
    this.getApplicationdate();
    this._paymentService.Data.subscribe(
      response => {
        this.ParentJSON = response;
        if (this.ParentJSON != null) {
          if (this.modifyOrderNo == "0" || this.modifyOrderNo == null) {
            this.ArrayList = this.ParentJSON.lstOfPayment;
            this.EnableDisPayControls = true;
            this.EnableReadOnlyControls = false;
            //Executed From View Order Component
            this._orderservice.castOrderNoFromViewOrderComp.subscribe(
              response => {
                this.orderNo = response;
                if (this.orderNo != null) {
                  this.EnableDisPayControls = false;
                  this.EnableReadOnlyControls = true;
                  if (this.ParentJSON.ClosedFlag == "Y") {
                    this._paymentService.getClosedOrderReceiptDetails(this.orderNo).subscribe(
                      data => {
                        const validateClosedDetails = data;
                        if (validateClosedDetails != null) {
                          this.enableClosedOrderDetails = true;
                          this.ArrayListClosed = validateClosedDetails;
                        }
                      }
                    )
                  }
                  else {
                    this.enableClosedOrderDetails = false;
                  }
                }
              })
            this._paymentService.SendPaymentSummaryData(this.PaymentSummaryData);
          }

          if (this.ParentJSON.OrderNo != undefined) {
            for (var i = 0; i < this.ParentJSON.lstOfOrderItemDetailsVM.length; i++) {
              this.ParentJSON.lstOfOrderItemDetailsVM[i].OrderNo = this.ParentJSON.OrderNo;
            }
          }

        }
        else {

          //commented on 30-Mar-2021 as part of display order receipt history details for transactions
          // if (this._router.url == "/salesreturn/ConfirmSalesReturn" || this._router.url == "/purchase/purchase-billing" || this._router.url == "/sales-billing") {
          //   this.EnableDisPayControls = true;
          // }
          // else {
          //   this.EnableDisPayControls = false;
          // }

          //Added on 30-Mar-2021 as part of not to display order receipt history details for other transactions except view order
          if (this._router.url == "/orders/vieworders") {
            this.EnableDisPayControls = false;
          }
          else {
            this.EnableDisPayControls = true;
          }
        }
      }
    )



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

    // this._salesBillingService.Subjectabc.subscribe(
    //   response => {
    //     this.abc = response;
    //     console.log(this.abc);
    //     if (this.abc != "") {
    //       this._paymentService.getPaymentMode(this.abc).subscribe(
    //         response => {
    //           this.PaymentModes = response;
    //         }
    //       )
    //     }
    //   })

    this._orderservice.castModifyOrderDetsToOrderComp.subscribe(
      response => {
        this.modifyOrderNo = response;
        if (this.modifyOrderNo != null) {
          this.EnableDisPayControls = false;
        }
      }
    )
  }



  getBankdetailsonchange(arg) {
    // this.PaymentHeaders.BankName = arg;
    this.PaymentHeaders.Bank = arg;
  }

  PaymentModes: any;
  getPaymentMode() {
    if (this.argPageName != null && this.argPageName != "") {
      this._paymentService.getPaymentMode(this.argPageName).subscribe(
        response => {
          this.PaymentModes = response;
        }
      )
    }
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

  PaymentHeaders: any = {
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
      this.SchemeAdjusment.BranchCode = this.bcode;
      this.getScheme(this.SchemeAdjusment.BranchCode);
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
        this.SchemeAdjusment = {
          BranchCode: BranchCode,
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
        this.SchemeAdjusment = {
          BranchCode: this.BranchCode,
          SchemeCode: this.SchemeCode,
          GroupCode: null,
          StartMSNNo: null,
          EndMSNNo: null,
          ChitAmount: null,
          TotalAmount: null,
          BonusAmount: null,
          WinnerAmount: null,
          PayMode: "CT"
        }
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
        this.SchemeAdjusment = {
          BranchCode: this.BranchCode,
          SchemeCode: this.SchemeCode,
          GroupCode: this.GroupCode,
          StartMSNNo: null,
          EndMSNNo: null,
          ChitAmount: null,
          TotalAmount: null,
          BonusAmount: null,
          WinnerAmount: null,
          PayMode: "CT"
        }
      }
    )
  }

  StartMSNo: string = "";
  getStartMSNo(StartMSNo) {
    this.StartMSNo = StartMSNo;
    this.SchemeAdjusment = {
      BranchCode: this.BranchCode,
      SchemeCode: this.SchemeCode,
      GroupCode: this.GroupCode,
      StartMSNNo: this.StartMSNo,
      EndMSNNo: null,
      ChitAmount: null,
      TotalAmount: null,
      BonusAmount: null,
      WinnerAmount: null,
      PayMode: "CT"
    }
  }

  ChitAmount: number = 0;
  TotalAmount: number = 0;
  BonusAmount: number = 0;
  WinnerAmount: number = 0;
  chitAmountList: any = [];
  EndMSNo: string = "";
  getchitAmount(EndMSNo) {
    this.EndMSNo = EndMSNo;
    this.SchemeAdjusment = {
      BranchCode: this.BranchCode,
      SchemeCode: this.SchemeCode,
      GroupCode: this.GroupCode,
      StartMSNNo: this.StartMSNo,
      EndMSNNo: this.EndMSNo,
      ChitAmount: null,
      TotalAmount: null,
      BonusAmount: null,
      WinnerAmount: null,
      PayMode: "CT"
    }
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
    this.PaymentHeaders.CardAppNo = null;
    this.readonly = false;
    this.placeholder = "Enter Amount Received";
    this.ngOnChanges();
    switch (paymodeArg) {
      case "Q": {
        this.toggle = "Cheque";  
       // if (this.argPageName == "OC" || this.argPageName == "SR" ||this.argPageName == "S") need to verify all payment modes
        //10022022   || this.argPageName == "SR" also checked if for payment type cheque load
       if (this.argPageName == "OC" || this.argPageName == "SR") {
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
        this.getCardType();
        this.getBank();
        this.getSalesMan();
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
  ArrayListClosed: any = [];
  PaymentList: any = [];
  addRow(arrayLines) {
    if (this.toggle == "Cash" || this.toggle == "BC" || this.toggle == "GV"
      || this.toggle == "SR" || this.toggle == "OrdAdj"
      || this.toggle == "PurEst" || this.toggle == "PurBill" || this.toggle == "SREst"
      || this.toggle == "SRBill" || this.toggle == "SalesPromotion"
      || this.toggle == "CREDIT NOTE") {
      if (this.toggle == "BC" && (this.PaymentHeaders.PayDetails == null || this.PaymentHeaders.PayDetails == "")) {
        swal("Warning!", 'Please Enter Details', "warning");
      }
      if (this.toggle == "GV" && (this.PaymentHeaders.PayDetails == null || this.PaymentHeaders.PayDetails == "")) {
        swal("Warning!", 'Please Enter Naration', "warning");
      }
      else if (this.PaymentHeaders.PayAmount == null || this.PaymentHeaders.PayAmount == "" || this.PaymentHeaders.PayAmount == 0) {
        swal("Warning!", 'Please enter the amount', "warning");
      }
      else if ((this.toggle == "OrdAdj" || this.toggle == "PurEst" || this.toggle == "PurBill" || this.toggle == "SREst" || this.toggle == "SRBill") && (this.PaymentHeaders.RefBillNo == null || this.PaymentHeaders.RefBillNo == "")) {
        swal("Warning!", 'Please enter the valid refno', "warning");
      }
      else {
        this.SaveAddrow(arrayLines, this.toggle);
      }
    }
    else if (this.toggle == "EP" || this.toggle == "UPI Payment") {
       if ( this.toggle == "EP" &&  (this.PaymentHeaders.RefBillNo == null || this.PaymentHeaders.RefBillNo == "")) {
        swal("Warning!", 'Please enter the valid refno', "warning");
      }
      else if (this.toggle == "UPI Payment" && (this.PaymentHeaders.RefBillNo == null || this.PaymentHeaders.RefBillNo == "")) {
        swal("Warning!", 'Please enter the valid refno', "warning");
      }
      else if (this.PaymentHeaders.PayAmount == null || this.PaymentHeaders.PayAmount == "" || this.PaymentHeaders.PayAmount == 0) {
        swal("Warning!", 'Please enter the amount', "warning");
      }
      // else if (this.PaymentHeaders.PayDetails == null || this.PaymentHeaders.PayDetails == "") {
      //   swal("Warning!", 'Please Enter Remarks', "warning");
      // }
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
        swal("Warning!", 'Please enter Transcation approval no', "warning");
      }
      else if (this.PaymentHeaders.CardSwipedBy == null || this.PaymentHeaders.CardSwipedBy == "") {
        swal("Warning!", 'Please select card swiped by', "warning");
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
    else if (this.toggle == "DKN" || this.toggle == "UDP" || this.toggle == "KRM" || this.toggle == "JNR" || this.toggle == "HBR" || this.toggle == "MNG" || this.toggle == "HSN" || this.toggle == "PNX" || this.toggle == "DKNBS" || this.toggle == "MRT" || this.toggle == "SMG" ) {
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
                if (this.ParentJSON.lstOfPayment.length > 0) {
                  for (var i = 0; i < this.SchemeData.length; i++) {
                    let SchemeAdjData = this.ParentJSON.lstOfPayment.find(x => x.RefBillNo == this.SchemeData[i].RefBillNo);
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
      this.ParentJSON.lstOfPayment.push(this.PaymentHeaders);
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
    // arrayLines.value.TransType = this.ParentJSON.OrderType;
    //this._paymentService.outputData(this.SchemeData);
    this._paymentService.SendPaymentSummaryData(this.PaymentSummaryData);
    this.EnableSubmitButton = false;
    this.toggle = "Invalid"
    this.PaymentForm.reset();
    //send full JSON data to subject
    this._paymentService.OutputParentJSONFunction(this.ParentJSON);
    this.payAmount = 0;
  }

  //scheme Lines


//17/02/2022 according to KIRAN sir testing paymode GiftVocher GV can aslo 
//add mutltiple times so  && paymentMode != "GV"" at line no 1019
  SaveAddrow(arrayLines, paymentMode) {
    if (this.ParentJSON.lstOfPayment.length > 0) {
      let data = this.ParentJSON.lstOfPayment.find(x => x.PayMode == arrayLines.value.PayMode);
      if (data != null && paymentMode != "Scheme" && paymentMode != "OrdAdj" && paymentMode != "PurEst" && paymentMode != "PurBill" && paymentMode != "SREst" && paymentMode != "SRBill" && paymentMode != "Card" && paymentMode != "EP" && paymentMode != "UPI Payment" && paymentMode != "Cheque"  && paymentMode != "GV") {
        swal("error", paymentMode + " mode is already added. Please select other mode of payment", "error");
      }
      else {
        if (paymentMode == "Scheme") {
          let SchemeAdjData = this.ParentJSON.lstOfPayment.find(x => x.RefBillNo == arrayLines.value.RefBillNo);
          if (SchemeAdjData != null) {
            swal("error", "Reference bill is already added", "error");
          }
          else {
            this.SavePaymentData(arrayLines, paymentMode);
          }
        }
        else if (paymentMode == "OrdAdj" || paymentMode == "PurEst" || paymentMode == "PurBill" || paymentMode == "SREst" || paymentMode == "SRBill") {
          let OutputData = this.ParentJSON.lstOfPayment.find(x => x.RefBillNo == arrayLines.value.RefBillNo);
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
    if (this.OrderAmount == 0 && this.RepairBalanceAmount == 0) {
      this.PaymentData(arrayLines, paymentMode);
    }
    else {
      if (this.OrderAmount != 0) {
        for (var i = 0; i < this.ParentJSON.lstOfPayment.length; i++) {
          this.payAmount += parseInt(this.ParentJSON.lstOfPayment[i].PayAmount);
        }
        this.payAmount += parseInt(arrayLines.value.PayAmount);
        if (this.payAmount > this.OrderAmount) {
          // swal("warning", "Paid Amount should not be greater than Order Amount", "warning");
          swal("warning", "Amount is greater than Balance", "warning");
          this.payAmount = 0;
        }
        else {
          this.PaymentData(arrayLines, paymentMode);
        }
      }
      if (this.RepairBalanceAmount != 0) {
        for (var i = 0; i < this.ParentJSON.lstOfPayment.length; i++) {
          this.payAmount += parseInt(this.ParentJSON.lstOfPayment[i].PayAmount);
        }
        this.payAmount += parseInt(arrayLines.value.PayAmount);
        if (this.payAmount > this.RepairBalanceAmount) {
          // swal("warning", "Paid Amount should not be greater than Order Amount", "warning");
          swal("warning", "Payment Amount is greater than Bill Amount", "warning");
          this.payAmount = 0;
        }
        else {
          this.PaymentData(arrayLines, paymentMode);
        }
      }
    }
  }

  PaymentLength: Number = 0;

  PaymentData(arrayLines, paymentMode) {
    if (paymentMode == "Card") {
      arrayLines.value.RefBillNo = arrayLines.value.RefBillNo; //Added since field Card No/Ref.No is common in grid for both card and reference no
    }
    else if (paymentMode == "OrdAdj" || paymentMode == "PurEst" || paymentMode == "PurBill" || paymentMode == "SREst" || paymentMode == "SRBill") { //Added on 17-Mar-2020
      arrayLines.value.TransType = "S"; //Added on 17-Mar-2020
    }
    arrayLines.value.SeriesNo = 0;
    arrayLines.value.ReceiptNo = 0;
    arrayLines.value.SNo = 0;
    arrayLines.value.CompanyCode = this.ccode;
    arrayLines.value.BranchCode = this.bcode;
    arrayLines.value.OperatorCode = localStorage.getItem('Login');
    this.ParentJSON.lstOfPayment.push(arrayLines.value);
    this.EnableSubmitButton = false;
    this.PaymentLength = this.ParentJSON.lstOfPayment.length - 1;
    //arrayLines.value.TransType = this.ParentJSON.OrderType; //Commented on 17-Mar-2020

    //this.ArrayList.push(arrayLines.value);
    this.EnableSubmitButton = false;
    this.toggle = "Invalid"

    //send full JSON data to subject


    if (this._router.url == "/sales-billing") {
      this._salesBillingService.ValidatePayMode(this.ParentJSON).subscribe(
        response => {
          this._paymentService.OutputParentJSONFunction(this.ParentJSON);
          this._paymentService.outputData(arrayLines.value);
          this._paymentService.SendPaymentSummaryData(this.PaymentSummaryData);
          this.PaymentForm.reset();
        },
        (err) => {
          if (err.status === 404) {
            const validationError = err.error;
            swal("Warning!", validationError.description, "warning");
          }
          if (err.status === 400) {
            const validationError = err.error;
            swal("Warning!", validationError.description, "warning");
            this.onchangePaymode(this.PaymentHeaders.PayMode);
            this.ParentJSON.lstOfPayment.splice(this.PaymentLength, 1);
            this._paymentService.OutputParentJSONFunction(this.ParentJSON);
          }
        }
      )
    }
    else {
      this._paymentService.OutputParentJSONFunction(this.ParentJSON);
      this._paymentService.outputData(arrayLines.value);
      this._paymentService.SendPaymentSummaryData(this.PaymentSummaryData);
      this.PaymentForm.reset();
    }

    this.payAmount = 0;
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
    else if (this.SchemeAdjusment.TotalAmount == 0) {
      swal("Warning!", 'Total Amount cannot be zero', "warning");
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
        //this.PaymentHeaders.CardSwipedBy = "A";
      })
  }

  DeleteRow(index: number, arrayLines) {
    var ans = confirm("Do you want to delete");
    if (ans) {
      // this.ArrayList.splice(index, 1)
      this.ParentJSON.lstOfPayment.splice(index, 1);
      // this.OrderDetails.lstOfPaymentVM.splice(index,1);
      this.EnableDisableSubmit();
      this._paymentService.outputData(arrayLines.value);
      if (this._router.url == "/credit-receipt/credit-receipt") {
        this._paymentService.OutputParentJSONFunction(this.ParentJSON);
      }
      this.ngOnChanges();
    }
  }

  EnableDisableSubmit() {
    if (this.ArrayList.length <= 0) {
      this._paymentService.SendPaymentSummaryData(null);
    }
  }

  Amount(arg: any[] = []) { let total = 0; arg.forEach((d) => { total += Number(d.PayAmount); }); this.PaymentSummaryData.Amount = total; this.PaymentAmountChanged(total); return total; }

  //Total for View Orders to display in Closed Order Table
  AmountClosed(arg: any[] = []) { let total = 0; arg.forEach((d) => { total += Number(d.PayAmount); }); this.PaymentSummaryData.Amount = total; return total; }

  PaidAmount: number = 0.00;

  PaymentAmountChanged(SalesAmount) {
    this.PaidAmount = SalesAmount;
    this.PaidAmountChange.emit(this.PaidAmount);
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
    // if (this.toggle == "OrdAdj") {
    //   alert("Reference No not found for OrderAdj.");
    // }
    // else {
    $('#OrderAdjustment').modal('show');
    this.getOrderList(this.top, this.skip);
    // }
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
    // this.PaymentHeaders.PayDetails = this.SchemeAdjusment.GroupCode;
    // this.PaymentHeaders.PayAmount = this.SchemeAdjusment.TotalAmount;
    // this.PaymentHeaders.SchemeCode = this.SchemeAdjusment.SchemeCode;
    // this.PaymentHeaders.GroupCode = this.SchemeAdjusment.GroupCode;
    // this.PaymentHeaders.CTBranch = this.SchemeAdjusment.BranchCode;   
  }

  //Ends here


  //Pur.Est Adjustment screen

  openPurEstAdjustment() {
    $("#PurEstAdj").modal('show');
    this._purchaseService.getSearchParams().subscribe(
      response => {
        this.SearcList = response;
        for (let i = 0; i < this.SearcList.length; i++) {
          if (i < 6) {
            this.SearchParamsList.push(this.SearcList[i]);
          }
        }
        this.getOGList(this.top, this.skip);
      }
    )
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

  // getOrderList(top, skip) {
  //   this.top = 50;
  //   this.skip = 0;
  //   this.AttachOrderForm.reset();
  //   this._orderservice.getAllOrdersCount().subscribe(
  //     response => {
  //       this.AllRecordCount = response;
  //       this.totalItems = this.AllRecordCount.RecordCount;
  //       if (this.totalItems > 0) {
  //         this._orderservice.getOrderList(top, skip).subscribe(
  //           response => {
  //             this.AttachOrderList = response;
  //           }
  //         )
  //       }
  //     }
  //   )
  // }



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
    this._paymentService.getSearchParamsSRBill().subscribe(
      response => {
        this.SearchSRBillParamsList = response;
        this._paymentService.getSRBillList().subscribe(
          response => {
            this.AttachSRBillList = response;
          }
        )
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
    // this.PaymentHeaders.PayDetails = this.SchemeAdjusment.GroupCode;
    // this.PaymentHeaders.PayAmount = this.SchemeAdjusment.TotalAmount;
    // this.PaymentHeaders.SchemeCode = this.SchemeAdjusment.SchemeCode;
    // this.PaymentHeaders.GroupCode = this.SchemeAdjusment.GroupCode;
    // this.PaymentHeaders.CTBranch = this.SchemeAdjusment.BranchCode;   
  }
  //ends here


  ReceivedCash: number = 0;
  ngOnChanges() {
    this.getPaymentMode(); //Commented as part of code cleaning.
    if (this.autoFillAmount != 0) {
      this.PaymentHeaders.PayAmount = this.autoFillAmount;
      this.ReceivedCash = this.autoFillAmount;
    }
    if (this.autoFillDisAmount != 0) {
      this.PaymentHeaders.PayAmount = this.autoFillDisAmount;
      this.ReceivedCash = this.autoFillDisAmount;
    }
    if (this.autoFillAmount == 0 && this.autoFillDisAmount == 0) {
      this.ReceivedCash = 0;
    }
  }
}