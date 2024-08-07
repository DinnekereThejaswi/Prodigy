import { Component, OnInit, OnDestroy } from '@angular/core';
import { CustomerService } from './../../masters/customer/customer.service';
import { SalesreturnService } from '../salesreturn.service';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import * as CryptoJS from 'crypto-js';
import { Router } from '@angular/router';
import swal from 'sweetalert';
import { ComponentCanDeactivate } from '../../appconfirmation-guard';
import { AppConfigService } from '../../AppConfigService';

@Component({
  selector: 'app-cancel-salesreturn',
  templateUrl: './cancel-salesreturn.component.html',
  styleUrls: ['./cancel-salesreturn.component.css']
})
export class CancelSalesreturnComponent implements OnInit, OnDestroy, ComponentCanDeactivate {

  SRCancelForm: FormGroup;
  EnableJson: boolean = false;
  leavePage: boolean = false;
  password: string;
  constructor(private fb: FormBuilder, private _SalesreturnService: SalesreturnService,
    private _CustomerService: CustomerService, private _router: Router,
    private _appConfigService: AppConfigService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }

  ngOnInit() {
    this.SRCancelForm = this.fb.group({
      CustID: null,
      BillNo: null,
      BillAmount: null,
      GST: null,
      CustName: null,
      VotureBillNo: null,
      Phone: null,
      EstNo: null,
      Address1: null,
      Address2: null,
      Address3: null,
      Mobile: null,
      Remarks: null
    });
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

  hasSRDetails: boolean = false;
  ccode: string = "";
  bcode: string = "";
  SRBillData: any;

  getSRDetails(arg) {
    if (arg == '') {
      swal("Warning!", "Please Enter Bill No", "warning");
    }
    else {
      this._SalesreturnService.getSRBill(arg).subscribe(
        response => {
          this.SRBillData = response;
          if (this.SRBillData != null) {
            this.leavePage = true;
            this.hasSRDetails = true;
            this.SRCancelObj.CompanyCode = this.ccode;
            this.SRCancelObj.BranchCode = this.bcode;
            this.SRCancelObj.EstNo = this.SRBillData.EstNo;
            this.SRCancelObj.BillNo = this.SRBillData.NewBillNo;
            this.SRCancelObj.OperatorCode = localStorage.getItem('Login');
            this.SRCancelObj.CustName = this.SRBillData.CustName;
            this.SRCancelObj.PaidAmount = this.SRBillData.TotalPurchaseAmount;
          }
        },
        (err) => {
          if (err.status === 404) {
            const validationError = err.error;
            swal("Warning!", validationError.description, "warning");
            this.hasSRDetails = false;
          }
          if (err.status === 400) {
            const validationError = err.error;
            swal("Warning!", validationError.description, "warning");
            this.hasSRDetails = false;
          }
        }
      )
    }
  }

  SRCancelObj: any = {
    CompanyCode: null,
    BranchCode: null,
    EstNo: null,
    OperatorCode: null,
    BillCounter: null,
    Type: null,
    BillNo: 0,
    PaidAmount: null,
    CancelRemarks: null,
    lstOfPayment: []
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnDestroy() {
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
  }

  clear() {
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(() =>
      this._router.navigate(['/salesreturn/cancel-salesreturn']))
  }

  submitPost() {
    if (this.SRCancelObj.CancelRemarks == "" || this.SRCancelObj.CancelRemarks == null) {
      swal("Warning!", "Please enter the remarks", "warning");
    }
    else {
      var ans = confirm("Do you want to cancel SR Bill No." + this.SRCancelObj.BillNo + "??");
      if (ans) {
        this._SalesreturnService.CancelSRBill(this.SRCancelObj).subscribe(
          response => {
            swal("Cancelled!", "SR Bill No. " + this.SRCancelObj.BillNo + " Cancelled Successfully", "success");
            this.SRCancelForm.reset();
            this.hasSRDetails = false;
            this.leavePage = false;
          }
        )
      }
    }
  }

  Clear() {
    this.SRCancelForm.reset();
    this.hasSRDetails = false;
    this.SRBillData = null;
    this.SRCancelObj = {
      CompanyCode: null,
      BranchCode: null,
      EstNo: null,
      OperatorCode: null,
      BillCounter: null,
      Type: null,
      BillNo: 0,
      PaidAmount: null,
      CancelRemarks: null,
      lstOfPayment: []
    }
  }
}