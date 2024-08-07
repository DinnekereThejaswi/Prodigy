import { CustomerService } from './../../masters/customer/customer.service';
import { OrdersService } from './../orders.service';
import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { ComponentCanDeactivate } from '../../appconfirmation-guard';
import swal from 'sweetalert';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
@Component({
  selector: 'app-receipt-cancel',
  templateUrl: './receipt-cancel.component.html',
  styleUrls: ['./receipt-cancel.component.css']
})
export class ReceiptCancelComponent implements OnInit, OnDestroy, ComponentCanDeactivate {
  @ViewChild('ReceiptNo', { static: true }) ReceiptNos: ElementRef;
  ReceiptCancelForm: FormGroup;
  EnableJson: boolean = false;
  leavePage: boolean = false;
  password: string;
  constructor(private fb: FormBuilder, private _OrdersService: OrdersService,
    private _CustomerService: CustomerService, private _router: Router,
    private _appConfigService: AppConfigService
  ) {
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
    this.getCB();
  }

  ngOnInit() {
    this.ReceiptCancelForm = this.fb.group({
      ReceiptNo: ['null', Validators.required],
      ID: null,
      CustName: null,
      Address1: null,
      AdvacnceOrderAmount: null,
      ReceiptDate: null,
      Phone: null,
      MobileNo: null,
      Area: null,
      EmailID: null,
      Remarks: null,
    });

    this.ReceiptCancelForm.controls["Remarks"].valueChanges.subscribe(change => {
      if (change != null && change != "") {
        this.leavePage = true;
      }
      else {
        this.leavePage = false;
      }
    })
  }
  receiptDetails: any = [];
  hasReceiptDetails: boolean = false;
  ReceiptNo: any;
  previousPaymentDetails: number;
  SelectedItemLines: any = [];
  customerDtls: any = [];
  ccode: string = "";
  bcode: string = "";
  getReceiptDetails(arg) {
    if (arg == '') {
      swal("Warning!", 'Please Enter Receipt Number', "warning");
    }
    else {
      this._OrdersService.getOrderReceipt(arg).subscribe(
        response => {
          this.ReceiptCancelObj = response;
          if (this.ReceiptCancelObj != null) {
            this.hasReceiptDetails = true;
            this.ReceiptNo = arg;
            this.ReceiptCancelObj.CompanyCode = this.ccode
            this.ReceiptCancelObj.BranchCode = this.bcode;
            this._CustomerService.getCustomerDtls(this.ReceiptCancelObj.SNo).subscribe(
              response => {
                this.customerDtls = response;
                this._CustomerService.sendCustomerDtls_To_Customer_Component(this.customerDtls);
                this.ReceiptNo = arg;
              }
            )
          }
        },
        (err) => {
          if (err.status === 404) {
            const validationError = err.error;
            swal("Warning!", validationError.description, "warning");
            this.hasReceiptDetails = false;
          }
          if (err.status === 400) {
            const validationError = err.error;
            swal("Warning!", validationError.description, "warning");
            this.hasReceiptDetails = false;
          }
        }
      )
    }
  }

  ReceiptCancelObj: any;

  ngOnDestroy() {
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
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

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  submitPost() {
    if (this.ReceiptCancelObj.CancelledRemarks == null || this.ReceiptCancelObj.CancelledRemarks == "") {
      swal("Warning!", 'Please enter the remarks', "warning");
    }
    else {
      var ans = confirm("Do you want to cancel order receipt: " + this.ReceiptCancelObj.ReceiptNo + "??");
      if (ans) {
        this._OrdersService.cancelReceipt(this.ReceiptCancelObj).subscribe(
          response => {
            swal("Cancelled!", "Receipt number " + this.ReceiptNo + " Cancelled Successfully", "success");
            this.ReceiptCancelForm.reset();
            this.hasReceiptDetails = false;
            this.ReceiptNos.nativeElement.value = "";
          }
        )
      }
    }
  }

  clear() {
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
      () => {
        this._router.navigate(['/orders/ReceiptCancel']);
      }
    )
  }
}
