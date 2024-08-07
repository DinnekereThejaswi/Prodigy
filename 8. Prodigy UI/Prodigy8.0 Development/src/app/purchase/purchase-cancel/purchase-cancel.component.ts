import { Component, OnInit, OnDestroy, ElementRef, ViewChild } from '@angular/core';
import { CustomerService } from './../../masters/customer/customer.service';
import { PurchaseService } from './../purchase.service';
import { ComponentCanDeactivate } from '../../appconfirmation-guard';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import swal from 'sweetalert';

@Component({
  selector: 'app-purchase-cancel',
  templateUrl: './purchase-cancel.component.html',
  styleUrls: ['./purchase-cancel.component.css']
})
export class PurchaseCancelComponent implements OnInit, OnDestroy, ComponentCanDeactivate {
  EnableJson: boolean = false;
  PurchaseCancelForm: FormGroup;
  @ViewChild('BillNo', { static: true }) BillNo: ElementRef;
  leavePage: boolean = false;
  password: string;
  constructor(private fb: FormBuilder, private _PurchaseService: PurchaseService,
    private _CustomerService: CustomerService,
    private _appConfigService: AppConfigService) {
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
    this.getCB();
  }

  ngOnInit() {
    this.PurchaseCancelForm = this.fb.group({
      BillNo: null,
      BillAmount: null,
      GST: null,
      CustName: null,
      Phone: null,
      Address1: null,
      Address2: null,
      Address3: null,
      Mobile: null,
      Remarks: null
    });

    this.PurchaseCancelForm.controls["Remarks"].valueChanges.subscribe(change => {
      if (change != null && change != "") {
        this.leavePage = true;
      }
      else {
        this.leavePage = false;
      }
    })

  }
  orderDetails: any = [];
  hasPurchaseDetails: boolean = false;
  customerDtls: any = [];
  ccode: string = "";
  bcode: string = "";
  PurchaseBillData: any;
  getPurchaseDetails(arg) {
    if (arg == '') {
      swal("Warning!", 'Please Enter Bill Number', "warning");
    }
    else {
      this._PurchaseService.getPurchaseBill(arg).subscribe(
        response => {
          this.PurchaseBillData = response;
          if (this.PurchaseBillData != null) {
            this.hasPurchaseDetails = true;
            this.PurchaseCancelObj.CompanyCode = this.ccode;
            this.PurchaseCancelObj.BranchCode = this.bcode;
            this.PurchaseCancelObj.BillNo = arg;
            this.PurchaseCancelObj.OperatorCode = localStorage.getItem('Login');
            this.PurchaseCancelObj.CustName = this.PurchaseBillData.CustName;
            this.PurchaseCancelObj.PaidAmount = this.PurchaseBillData.TotalPurchaseAmount;
          }
        },
        (err) => {
          if (err.status === 404) {
            const validationError = err.error;
            swal("Warning!", validationError.description, "warning");
            this.hasPurchaseDetails = false;
          }
          if (err.status === 400) {
            const validationError = err.error;
            swal("Warning!", validationError.description, "warning");
            this.hasPurchaseDetails = false;
          }
        }
      )
    }
  }

  PurchaseCancelObj: any = {
    CompanyCode: null,
    BranchCode: null,
    EstNo: null,
    OperatorCode: null,
    BillCounter: null,
    Type: null,
    BillNo: 0,
    CustName: null,
    PaidAmount: null,
    CancelRemarks: null,
    lstOfPayment: []
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
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


  ngOnDestroy() {
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
  }

  submitPost() {
    if (this.PurchaseCancelObj.CancelRemarks == "" || this.PurchaseCancelObj.CancelRemarks == null) {
      swal("Warning!", 'Please enter the remarks', "warning");
    }
    else {
      var ans = confirm("Do you want to cancel Purchase Bill No." + this.PurchaseCancelObj.BillNo + "??");
      if (ans) {
        this._PurchaseService.CancelPurchaseBill(this.PurchaseCancelObj).subscribe(
          response => {
            swal("Cancelled!", "Purchase Bill No. " + this.PurchaseCancelObj.BillNo + " Cancelled Successfully", "success");
            this.PurchaseCancelForm.reset();
            this.hasPurchaseDetails = false;
            this.BillNo.nativeElement.value = '';
          }
        )
      }
    }
  }

  Clear() {
    this.BillNo.nativeElement.value = '';
    this.hasPurchaseDetails = false;
    this.PurchaseBillData = null;
    this.PurchaseCancelObj = {
      CompanyCode: null,
      BranchCode: null,
      EstNo: null,
      OperatorCode: null,
      BillCounter: null,
      Type: null,
      BillNo: 0,
      CustName: null,
      PaidAmount: null,
      CancelRemarks: null,
      lstOfPayment: []
    }
  }
}