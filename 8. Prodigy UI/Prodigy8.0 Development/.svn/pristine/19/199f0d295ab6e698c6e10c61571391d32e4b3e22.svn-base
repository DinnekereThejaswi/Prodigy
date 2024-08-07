import { Component, OnInit, OnDestroy } from '@angular/core';
import { CreditReceiptService } from '../credit-receipt.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import swal from 'sweetalert';
import { CustomerService } from './../../masters/customer/customer.service';
import * as CryptoJS from 'crypto-js';
import { PaymentService } from '../../payment/payment.service';
import { Router } from '@angular/router';
import { AppConfigService } from '../../AppConfigService';
import { MasterService } from '../../core/common/master.service';
import { DatePipe, formatDate } from '@angular/common'
import * as moment from 'moment'
import { ComponentCanDeactivate } from '../../appconfirmation-guard';

@Component({
  selector: 'app-credit-receipt',
  templateUrl: './credit-receipt.component.html',
  styleUrls: ['./credit-receipt.component.css']
})
export class CreditReceiptComponent implements OnInit, OnDestroy, ComponentCanDeactivate {
  leavePage: boolean = false;
  ccode: string = "";
  bcode: string = "";
  password: string;
  paymentDetails: any;
  apiBaseUrl: string;
  paymentPaidAmt: number = 0;
  routerUrl: string = "";
  CreditrecieptForm: FormGroup;
  radioItems: Array<string>;
  model = { option: 'Current Fin Year' };
  NoRecordsPaymentSummary: boolean = false;
  toggleCurFinYear: boolean = true;
  EnableCustomer: boolean = false;
  AccFinYear: any;
  EnablePayments: boolean = false;
  togglePrevFinYear: boolean = false;
  EnableReadOnlyControls: boolean = false;
  EnableSubmitButton: boolean = true;
  BalanceAmount: number = 0;
  EnableReprint: boolean = false;
  EnableJson: boolean = false;
  public pageName = "CR";
  // PrevYear = [
  //   { YearCode: 2013 },
  //   { YearCode: 2014 },
  //   { YearCode: 2015 },
  //   { YearCode: 2016 },
  //   { YearCode: 2017 },
  //   { YearCode: 2018 },
  //   { YearCode: 2019 }
  // ];
  CurYear = []
  PrevYear = [];
  constructor(private fb: FormBuilder, private creditreceiptservice: CreditReceiptService, private appConfigService: AppConfigService,
    private _CustomerService: CustomerService, private _paymentservice: PaymentService, private _router: Router,
    private _masterService: MasterService, private datepipe: DatePipe) {
    this.radioItems = ['Current Fin Year', 'Previous Fin Years'];
    this.EnableJson = this.appConfigService.EnableJson;
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.password = this.appConfigService.Pwd;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.creditreceiptservice.getAccFinYear().subscribe(
      response => {
        this.AccFinYear = response;
        this.CurYear.push(this.AccFinYear);
        this.CreditrecieptForm.controls.FinancialYear.setValue(this.AccFinYear.FinYear);
        this.getPrevFinYear();
      }
    )
  }
  CreditrecieptSummaryHeader: any = {
    BillNo: null,
    LastReceiptNo: null,
    ReceiptDate: null,
    CustomerName: null,
    FinancialYear: null,
    SalesAmount: null,
    PaidAmount: null,
    BalanceAmount: null,
    CurrentBalance: null,
    Address1: null,
    Address2: null,
    Address3: null,
    City: null,
    State: null,
    Pincode: null,
    MobileNo: null,
    PhoneNo: null,
    EmailID: null,
    PANNo: null,
    IDType: null,
    DateOfBirth: null,
    lstOfPayment: []
  };
  ReprintData: any = {
    ReceiptNo: null
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
    this.getApplicationDate();
    this.DetailsToCustomerComp();
    this.GetPaymentSummary();
    this.CreditrecieptSummaryHeader.FinancialYear = 2019;
    this.CreditrecieptForm = this.fb.group({
      BillNo: null,
      LastReceiptNo: null,
      ReceiptDate: null,
      CustomerName: null,
      FinancialYear: 2019,
      SalesAmount: null,
      PaidAmount: null,
      BalanceAmount: null,
      CurrentBalance: null,
      Address1: null,
      Address2: null,
      Address3: null,
      City: null,
      State: null,
      Pincode: null,
      MobileNo: null,
      PhoneNo: null,
      EmailID: null,
      PANNo: null,
      IDType: null,
      DateOfBirth: null
    })

    this._paymentservice.castParentJSON.subscribe(
      response => {
        this.paymentDetails = response;
        if (this.isEmptyObject(this.paymentDetails) == false && this.isEmptyObject(this.paymentDetails) != null) {
          this.paymentPaidAmt = 0;
          this.paymentDetails.lstOfPayment.forEach((d) => {
            this.paymentPaidAmt += parseInt(d.PayAmount);
          });
          this.CreditrecieptSummaryHeader.CurrentBalance = Number(this.CreditrecieptSummaryHeader.SalesAmount - this.CreditrecieptSummaryHeader.PaidAmount - this.paymentPaidAmt);
          this.leavePage = true;
          if (this.paymentDetails.lstOfPayment.length == 0) {
            this.leavePage = false;
          }
        }
        else {
          this.leavePage = false;
        }
      }
    )
  }

  getPrevFinYear() {
    for (let i = 0; i < 6; i++) {
      this.PrevYear.push(this.CurYear[0].FinYear - i);
    }
    
  }


  Customer: any;
  EnableCustomerTab: boolean = true;
  ToggleCustomer: boolean = false;

  TogglePayments: boolean = false;
  EnablePaymentsTab: boolean = false;

  ToggleCustomerData() {
    // if (this.ToggleCustomer == true) {
    //   this.EnableCustomerTab = false;
    // }
    this.EnableCustomerTab = !this.EnableCustomerTab;
  }
  TogglePaymentData() {
    // if (this.ToggleCustomer == true) {
    //   this.EnablePaymentsTab = false;
    // }
    // else {
    //   swal("Warning", "Please select Customer Details to continue", "warning");
    // }
    this.EnablePaymentsTab = !this.EnablePaymentsTab;
  }
  DetailsToCustomerComp() {
    this._CustomerService.cast.subscribe(
      response => {
        this.Customer = response;
        if (this.isEmptyObject(this.Customer) == false && this.isEmptyObject(this.Customer) != null) {
          this.EnableCustomerTab = true;
          this.ToggleCustomer = true;
          this.EnablePaymentsTab = true;
          this.TogglePayments = true;
        }
        else {
          this.EnableCustomerTab = true;
          this.ToggleCustomer = false;
          this.EnablePaymentsTab = true;
          this.TogglePayments = false;
        }
      });
  }
  //Check object is empty
  isEmptyObject(obj) {
    return (obj && (Object.keys(obj).length === 0));
  }
  Changed(arg) {
    if (arg === 'Current Fin Year') {
      this.toggleCurFinYear = true;
      this.togglePrevFinYear = false;
      this.model.option = arg;
      this.CreditrecieptForm.reset();
      this.CreditrecieptForm.controls.FinancialYear.setValue(this.AccFinYear.FinYear);
    }
    else if (arg === 'Previous Fin Years') {
      this.toggleCurFinYear = false;
      this.togglePrevFinYear = true;
      this.model.option = arg;
      this.CreditrecieptForm.reset();
    }
  }

  applicationDate: any;
  disAppDate: any;
  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
        this.CreditrecieptSummaryHeader.ReceiptDate = this.datepipe.transform(this.applicationDate, 'dd/MM/yyyy')
      }
    )
  }



  creditList: any = [];
  getvalues(arg) {
    if (arg.value.FinancialYear == null || arg.value.FinancialYear == "") {
      swal("Warning!", "Please select the financial year", "warning");
    }
    else {
      this.BalanceAmount = 0;
      this.paymentPaidAmt = 0;
      this.creditreceiptservice.getValues(arg.value.BillNo, arg.value.FinancialYear).subscribe(
        Response => {
          this.creditList = Response;
          // this.creditList={
          //   BillNo: 2135923,
          //   LastReceiptNo: 182,
          //   ReceiptDate: null,
          //   CustomerName: "new customer",
          //   CustID: 201544,
          //   FinancialYear: 2021,
          //   SalesAmount: 39291,
          //   PaidAmount: 39250,
          //   BalanceAmount: 41,
          //   Address1: "test",
          //   Address2: null,
          //   Address3: null,
          //   City: "BENGALURU",
          //   State: "Karnataka",
          //   Pincode: null,
          //   MobileNo: "7418529633",
          //   PhoneNo: null,
          //   EmailID: null,
          //   PANNo: "QWERT5678K",
          //   IDType: null,
          //   DateOfBirth: null,
          //   lstOfPayment: []
          // };
          this.CreditrecieptSummaryHeader = Response;
          if (this.creditList != null) {
            this.CreditrecieptSummaryHeader.SalesAmount = Math.round(this.creditList.SalesAmount);
            this.CreditrecieptSummaryHeader.BalanceAmount = Math.round(this.creditList.BalanceAmount);
            this.BalanceAmount = Math.round(this.creditList.BalanceAmount);
            this.EnablePayments = true;
            // if (this.BalanceAmount == 0) {
            //   this.EnablePayments = false;
            //   swal('warning', 'Balance Amount is NIL', 'warning');
            // }
            // else {
            //   this.EnablePayments = true;
            // }
            this.CreditrecieptSummaryHeader.CurrentBalance = Number(this.CreditrecieptSummaryHeader.SalesAmount - this.CreditrecieptSummaryHeader.PaidAmount);
            this._CustomerService.getCustomerDtls(this.creditList.CustID).subscribe(
              response => {
                const customerDtls = response;
                if (customerDtls != null) {
                  this._CustomerService.sendCustomerDtls_To_Customer_Component(customerDtls);
                  this._paymentservice.inputData(this.CreditrecieptSummaryHeader);
                  this.EnablePaymentsTab = false;
                  this.EnableCustomer = true;
                }
              });
          }
        },
        (err) => {
          this.clearCreditReceiptComponents();
        }
      )
    }

  }
  PaymentSummary: any = [];
  GetPaymentSummary() {
    this._paymentservice.CastPaymentSummaryData.subscribe(
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
          }
        }
        else {
          this.NoRecordsPaymentSummary = false;
          this.EnableSubmitButton = true;
        }
      }
    )
  }
  Save: any;
  CreditPost() {
    if (this.CreditrecieptSummaryHeader.BillNo == null) {
      swal('warning', 'Please enter bill number to continue', 'warning');
    }
    else if (this.CreditrecieptSummaryHeader.lstOfPayment.length == 0) {
      swal('warning', 'Please enter receipt details', 'warning');
    }
    else {
      var ans = confirm("Do you want to save Credit Receipt??");
      if (ans) {
        this.creditreceiptservice.postCreditReceipt(this.CreditrecieptSummaryHeader.FinancialYear,
          this.CreditrecieptSummaryHeader.BillNo,
          this.CreditrecieptSummaryHeader.lstOfPayment).subscribe(
            Response => {
              this.Save = Response;
              this.routerUrl = this._router.url;
              swal('success', 'Credit Receipt No:' + this.Save.ReceiptNo + ' Saved Successfully!!!', 'success');
              if (this._router.url === "/credit-receipt/credit-receipt") {
                this.EnableReprint = true;
                this.ReprintData = {
                  ReceiptNo: this.Save.ReceiptNo
                }
                this.creditreceiptservice.SendReceiptNoToComp(this.ReprintData);
                this.clearCreditReceiptComponents();
              }
              else {
                this.EnableReprint = false;
              }
            });
        this.leavePage = false;
      }
    }
  }
  clearCreditReceiptComponents() {
    this._CustomerService.SendCustDataToEstComp(null);
    this._paymentservice.SendPaymentSummaryData(null);
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
    this._paymentservice.outputData(null);
    this._paymentservice.inputData(null);
    this._paymentservice.OutputParentJSONFunction(null);
    this.paymentPaidAmt = 0;
    this.EnableCustomer = false;
    this.EnablePayments = false;
    this.CreditrecieptSummaryHeader.FinancialYear = 2019;
    this.CreditrecieptSummaryHeader.BillNo = null;
    this.CreditrecieptSummaryHeader.LastReceiptNo = null;
    this.CreditrecieptSummaryHeader.ReceiptDate = null;
    this.CreditrecieptSummaryHeader.SalesAmount = null;
    this.CreditrecieptSummaryHeader.CustomerName = null;
    this.CreditrecieptSummaryHeader.PaidAmount = null;
    this.CreditrecieptSummaryHeader.BalanceAmount = null;
    this.CreditrecieptSummaryHeader.CurrentBalance = null;
  }


  ngOnDestroy() {
    this._CustomerService.SendCustDataToEstComp(null);
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
    this._paymentservice.outputData(null);
    this._paymentservice.inputData(null);
    this._paymentservice.OutputParentJSONFunction(null);
    this._paymentservice.SendPaymentSummaryData(null);
  }
}