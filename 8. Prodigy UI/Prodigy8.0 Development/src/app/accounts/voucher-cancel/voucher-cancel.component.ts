import { Component, OnInit } from '@angular/core';
import { AccountsService } from '../accounts.service';
import swal from 'sweetalert';
import { Router } from '@angular/router';
import { AppConfigService } from '../../AppConfigService';
@Component({
  selector: 'app-voucher-cancel',
  templateUrl: './voucher-cancel.component.html',
  styleUrls: ['./voucher-cancel.component.css']
})
export class VoucherCancelComponent implements OnInit {

  radioItems: Array<string>;
  model = { option: 'Journal' };
  modelvoucher = { option: 'Cash' };
  TransType: string = "";
  CashType: string = "";
  BankType: string = "";
  EnableJson: boolean = false;
  password: string;
  VoucherNo: number = 0;
  VoucherItems: Array<string>;

  constructor(private _accountsService: AccountsService,
    private router: Router, private _appConfigService: AppConfigService) {
    this.radioItems = ['Journal', 'Contra', 'Voucher'];
    this.VoucherItems = ['Cash', 'Bank'];
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
  }

  ngOnInit() {
    this.getPaymentType();
    this.getCashTypeList();
    this.getMasterLedgerList();
  }

  OnRadioBtnChnge(arg) {
    if (arg == "Journal") {

    }
    else if (arg == "Contra") {

    }
    else if (arg == "Voucher") {

    }
    this.model.option = arg;
  }

  OnVoucherRadioBtnChnge(arg) {
    this.modelvoucher.option = arg;
  }

  DataList: any = [];

  getVoucherDets(arg) {
    if (arg == "" || arg == null) {
      swal("!Warning", "Please select the voucher number", "warning")
    }
    else {
      this.VoucherNo = arg;
      if (this.model.option == "Journal") {
        this._accountsService.getJournalEntryDetails(arg).subscribe(
          response => {
            this.DataList = response;
          }
        )
      }
      else if (this.model.option == "Contra") {
        this._accountsService.getContraDetails(arg).subscribe(
          response => {
            this.DataList = response;
          }
        )
      }
      else if (this.model.option == "Voucher") {
        if (this.TransType == "") {
          swal("!Warning", "Please select the type", "warning")
        }
        else if (this.modelvoucher.option == "Cash") {
          if (this.CashType == "") {
            swal("!Warning", "Please select the cash", "warning")
          }
          else {
            this._accountsService.getCashVoucherForCancel(this.VoucherNo, this.CashType, this.TransType).subscribe(
              response => {
                this.DataList = response;
              }
            )
          }
        }
        else if (this.modelvoucher.option == "Bank") {
          if (this.BankType == "") {
            swal("!Warning", "Please select the bank", "warning")
          }
          else {
            this._accountsService.getBankVoucherDetailsForCancel(this.VoucherNo, this.BankType, this.TransType).subscribe(
              response => {
                this.DataList = response;
              }
            )
          }
        }
      }
    }
  }

  PaymentType: any = [];

  getPaymentType() {
    this._accountsService.getPaymentType().subscribe(
      Response => {
        this.PaymentType = Response;
      }
    )
  }

  CashTypeList: any = [];

  getCashTypeList() {
    this._accountsService.getCashType().subscribe(
      Response => {
        this.CashTypeList = Response;
      }
    )
  }

  MasterLedgerList: any = [];
  getMasterLedgerList() {
    this._accountsService.getMasterLedgerList().subscribe(
      Response => {
        this.MasterLedgerList = Response;
      }
    )
  }

  GetTransType(arg) {
    this.TransType = arg;
  }

  GetCashType(arg) {
    this.CashType = arg;
  }
  GetBankType(arg) {
    this.BankType = arg;
  }
  cancel(arg) {
    if (arg == "" || arg == null) {
      swal("!Warning", "Please select the remarks", "warning");
    }
    else {
      var ans = confirm('Do you want to cancel');
      if (ans) {
        if (this.model.option == "Journal") {
          this.DataList[0].CancelledRemarks = arg;
          this.DataList[0].CancelledBy = localStorage.getItem('Login');
          this._accountsService.cancelJournalEntry(this.DataList).subscribe(
            response => {
              swal("Cancelled!", "Voucher no " + this.VoucherNo + " Cancelled successfully.", "success");
            }
          )
        }
        else if (this.model.option == "Contra") {
          this.DataList.CancelledRemarks = arg;
          this.DataList.CancelledBy = localStorage.getItem('Login');
          this._accountsService.cancelContraEntry(this.DataList).subscribe(
            response => {
              swal("Cancelled!", "Voucher no " + this.VoucherNo + " Cancelled successfully.", "success");
            }
          )
        }
        else if (this.model.option == "Voucher") {
          if (this.modelvoucher.option == "Bank") {
            this.DataList[0].CancelledRemarks = arg;
            this.DataList[0].CancelledBy = localStorage.getItem('Login');
            this._accountsService.deleteBankVoucherEntry(this.DataList).subscribe(
              response => {
                swal("Cancelled!", "Voucher no " + this.VoucherNo + " Cancelled successfully.", "success");
              }
            )
          }
          else if (this.modelvoucher.option == "Cash") {
            this.DataList.CancelledRemarks = arg;
            this.DataList.CancelledBy = localStorage.getItem('Login');
            this._accountsService.deleteCashVoucherDet(this.DataList).subscribe(
              response => {
                swal("Cancelled!", "Voucher no " + this.VoucherNo + " Cancelled successfully.", "success");
              }
            )
          }
        }
        this.router.navigateByUrl('/redirect', { skipLocationChange: true }).then(() =>
          this.router.navigate(['/accounts/voucher-cancel']))
      }
    }
  }
}