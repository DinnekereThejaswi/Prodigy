import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AccountsService } from '../accounts.service';
import swal from 'sweetalert';
import { Router } from '@angular/router';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import { TDSExpenseCancelVM } from './../accounts.model';

@Component({
  selector: 'app-expense-voucher-cancel',
  templateUrl: './expense-voucher-cancel.component.html',
  styleUrls: ['./expense-voucher-cancel.component.css']
})
export class ExpenseVoucherCancelComponent implements OnInit {

  @ViewChild("Remarks", { static: true }) Remarks: ElementRef;
  ccode: string = "";
  bcode: string = "";
  password: string;

  postExpenseVoucherCancel: TDSExpenseCancelVM = {
    ExpenseNo: 0,
    CompanyCode: "",
    BranchCode: "",
    Remarks: ""
  }

  constructor(private service: AccountsService,
    private _router: Router,
    private appConfigService: AppConfigService) {
    this.password = this.appConfigService.Pwd;
    this.getCB();
  }

  ngOnInit() {
    this.getApplicationdate();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  appDate: any = [];
  rpDate: any = [];
  expensevouchercancelData: any = [];
  getApplicationdate() {
    this.service.getApplicationDate().subscribe(
      response => {
        this.appDate = response;
        this.appDate = this.appDate["applcationDate"];
      }
    )
  }

  expenseVoucherCancel(arg) {
    if (arg == null || arg == "") {
      swal("Warning", "Please enter the voucher no", "warning");
    }
    else {
      this.service.getExpenseVoucherCancel(arg).subscribe(
        response => {
          this.expensevouchercancelData = response;
          this.postExpenseVoucherCancel.CompanyCode = this.ccode;
          this.postExpenseVoucherCancel.BranchCode = this.bcode;
          this.postExpenseVoucherCancel.ExpenseNo = Number(arg);
        }
      )
    }
  }

  output: any = [];

  postexpensevouchercancel() {
    if (this.Remarks.nativeElement.value == "" || this.Remarks.nativeElement.value == null) {
      swal("Warning", "Please enter the remarks", "warning");
    }
    else {
      var ans = confirm("Do you want to cancel??");
      if (ans) {
        this.postExpenseVoucherCancel.Remarks = this.Remarks.nativeElement.value;
        this.service.postExpenseVoucherCancel(this.postExpenseVoucherCancel).subscribe(
          response => {
            this.output = response;
            swal("Saved!", "Expense Voucher cancelled successfully.", "success");
            this.expensevouchercancelData = [];
          }
        )
      }
    }
  }

  clear() {
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
      () => {
        this._router.navigate(['/accounts/expense-voucher-cancel']);
      })
  }
}
