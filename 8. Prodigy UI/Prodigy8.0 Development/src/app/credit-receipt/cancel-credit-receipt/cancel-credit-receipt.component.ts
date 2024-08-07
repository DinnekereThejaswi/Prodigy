import { Component, OnInit } from '@angular/core';
import * as CryptoJS from 'crypto-js';
import { CreditReceiptService } from '../credit-receipt.service';
import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';
import { ComponentCanDeactivate } from '../../appconfirmation-guard';
import { DatePipe } from '@angular/common';
@Component({
  selector: 'app-cancel-credit-receipt',
  templateUrl: './cancel-credit-receipt.component.html',
  styleUrls: ['./cancel-credit-receipt.component.css']
})
export class CancelCreditReceiptComponent implements OnInit, ComponentCanDeactivate {
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;
  leavePage: boolean = false;
  EnableJson: boolean = false;
  constructor(private creditreceiptservice: CreditReceiptService, private appConfigService: AppConfigService, private datepipe: DatePipe) {
    this.EnableJson = this.appConfigService.EnableJson;
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.password = this.appConfigService.Pwd;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }
  CreditrecieptSummaryHeader: any = {
    CompanyCode: this.ccode,
    BranchCode: this.bcode,
    ReceiptNo: null,
    ReceiptDate: null,
    CustomerName: null,
    CustID: null,
    Address1: null,
    Address2: null,
    Address3: null,
    City: null,
    State: null,
    Pincode: null,
    MobileNo: null,
    PhoneNo: null,
    Amount: null,
    Remarks: null,
  };
  ngOnInit() {

  }
  ReceiptList: any = [];
  GetReceiptValues(arg) {
    this.creditreceiptservice.getReceiptValues(arg).subscribe(
      Response => {
        this.ReceiptList = Response;
        if (this.ReceiptList != null) {
          this.CreditrecieptSummaryHeader = this.ReceiptList;
          this.CreditrecieptSummaryHeader.ReceiptDate = this.datepipe.transform(this.CreditrecieptSummaryHeader.ReceiptDate, 'dd/MM/yyyy')
        }
      },(err) => {
        if (err.status === 400) {
          const validationError = err.error.description;
          swal("Warning!", validationError, "warning");
        }
      }
    )
  }
  clear() {
    this.CreditrecieptSummaryHeader.ReceiptNo = null;
    this.CreditrecieptSummaryHeader.CustomerName = null;
    this.CreditrecieptSummaryHeader.ReceiptDate = null;
    this.CreditrecieptSummaryHeader.PhoneNo = null;
    this.CreditrecieptSummaryHeader.Remarks = null;
    this.CreditrecieptSummaryHeader.Amount = null;
    this.CreditrecieptSummaryHeader.Address1 = null;
    this.CreditrecieptSummaryHeader.Address2 = null;
    this.CreditrecieptSummaryHeader.Address3 = null;
    this.CreditrecieptSummaryHeader.MobileNo = null;
  }
  Post: any;
  PostCancelReceipt() {
    if (this.CreditrecieptSummaryHeader.ReceiptNo == null) {
      swal('warning', 'Please enter Receipt Number', 'warning');
    }
    else if (confirm('Do you want to cancel the Credit Receipt - ' + this.CreditrecieptSummaryHeader.ReceiptNo)) {
      this.creditreceiptservice.postCancelReceipt(this.CreditrecieptSummaryHeader).subscribe(
        Response => {
          this.Post = Response;
          swal('success', this.CreditrecieptSummaryHeader.ReceiptNo + ' - Cancelled Succeccfully', 'success');
          this.clear();
          this.leavePage = false;
        },
        error => {
          this.clear();
          this.CreditrecieptSummaryHeader.ReceiptNo = null;
        }
      )
    }
  }


  confirmBeforeLeave(): boolean {

    if (this.CreditrecieptSummaryHeader.Remarks == null || this.CreditrecieptSummaryHeader.Remarks == "") {
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

}
