import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { AccountsService } from '../accounts.service';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';
declare var jquery: any;
declare var $: any;
@Component({
  selector: 'app-cash-voucher-entry',
  templateUrl: './cash-voucher-entry.component.html',
  styleUrls: ['./cash-voucher-entry.component.css']
})
export class CashVoucherEntryComponent implements OnInit {
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  EnableJson: boolean = false;
  isDisabled = false;
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }
  constructor(private fb: FormBuilder, private service: AccountsService,

    private appConfigService: AppConfigService) {
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.EnableJson = this.appConfigService.EnableJson;
    this.password = this.appConfigService.Pwd;
    this.getCB();
  }

  CashSummaryHeader: any = {
    CompanyCode: this.ccode,
    BranchCode: this.bcode,
    TxtSeqNo: 0,
    VoucherSeqNo: 0,
    VoucherNo: 0,
    VoucherDate: null,
    AccountCode: null,
    AccountType: null,
    AccountCodeMaster: null,
    ApprovedBy: null,
    AuthorizedBy: null,
    AuthorizedDate: null,
    AuthorizedRemarks: null,
    CancelledBy: null,
    CancelledRemarks: null,
    ChequeDate: null,
    ChequeNo: null,
    CreditAmount: "",
    CurrencyType: null,
    CurrencyValue: null,
    ContraSeqNo: 0,
    DebitAmount: "",
    ExchangeRate: 0,
    FinalPeriod: 0,
    FinalYear: 0,
    Narration: null,
    NewVoucherNo: 0,
    Partyname: null,
    ReceiptNo: null,
    ReconsileBy: null,
    ReconsileFlag: null,
    SectionID: null,
    TransType: null,
    Cflag: null,
    IsApproved: null,
    SubledgerAccCode: 0,
    VerifiedBy: null,
    VerifiedRemarks: null,
    VoucherType: null,
  }
  ngOnInit() {
    this.getMasterLedger();
    this.CashSummaryHeader.CompanyCode = this.ccode,
      this.CashSummaryHeader.BranchCode = this.bcode,

      this.getApplicationdate();
    this.getType();
    this.getNarrationList();
  }
  MasterLedger: any = [];
  getMasterLedger() {
    this.service.GetMasterLedger().subscribe(
      Response => {
        this.MasterLedger = Response;
      }
    )
  }
  TypeList: any = [];
  getType() {
    this.service.gettype().subscribe(
      Response => {
        this.TypeList = Response;

      }
    )
  }
  appDate: any = [];
  rpDate: any = [];
  getApplicationdate() {
    this.service.getApplicationDate().subscribe(
      response => {
        this.appDate = response;
        this.rpDate = this.appDate["applcationDate"];
        this.CashSummaryHeader.VoucherDate = this.rpDate;
        this.CashSummaryHeader.ChequeDate = this.rpDate;
      }
    )
  }
  ApplicationNameList: any = [];
  getAccountName(arg) {
    this.service.GetAccountNames(arg).subscribe(
      Response => {
        this.ApplicationNameList = Response;
      }
    )

  }
  NarrationList: any = [];
  getNarrationList() {
    this.service.GetNarration().subscribe(
      Response => {
        this.NarrationList = Response;
      }
    )
  }

  CashVoucherList: any = []
  getcashvouchertable() {
    this.service.getCashVoucherTable(this.CashSummaryHeader.TransType, this.CashSummaryHeader.AccountCodeMaster, this.rpDate).subscribe(
      Response => {
        this.CashVoucherList = Response;
      }
    )
  }
  getList: any = [];
  Data(arg) {
    this.EnableAdd = false;
    this.EnableSave = true;
    this.isDisabled = true;
    this.service.editGet(arg.VoucherNo, arg.AccCode, arg.AccCodeMaster).subscribe(
      Response => {
        this.getList = Response;
        this.CashSummaryHeader = this.getList;
      });
  }
  clear() {
    this.EnableAdd = true;
    this.EnableSave = false;
    this.isDisabled = false;
    this.CashSummaryHeader.CreditAmount = null;
    this.CashSummaryHeader.DebitAmount = null;
    this.CashSummaryHeader.AccountCode = null;
    this.CashSummaryHeader.ReceiptNo = null;
    this.CashSummaryHeader.Narration = null;
  }
  deleteField(arg) {

    this.CashSummaryHeader.CreditAmount = arg.Credit;
    this.CashSummaryHeader.DebitAmount = arg.Debit;
    this.CashSummaryHeader.AccountCode = arg.AccCode;
    this.CashSummaryHeader.ReceiptNo = arg.RefNo;
    this.CashSummaryHeader.Narration = arg.Narration;
    this.CashSummaryHeader.VoucherNo = arg.VoucherNo;
    var ans = confirm("Do you want to delete??");
    if (ans) {
      this.service.deleteCashVoucherDet(this.CashSummaryHeader).subscribe(
        response => {
          swal("Deleted!", "Cash Voucher Entry deleted successfully.", "success");
          this.getcashvouchertable();
          this.clear();
        }
      )
    }
  }

  debitAmt() {
    this.CashSummaryHeader.CreditAmount = 0;
  }

  creditAmt() {
    this.CashSummaryHeader.DebitAmount = 0;
  }

  PostVoucher: any;
  PostCashVoucher() {
    if (this.CashSummaryHeader.AccountCodeMaster == null) {
      swal("!Warning", "Please select master ledger", "warning");
    }
    else if (this.CashSummaryHeader.TransType == null) {
      swal("!Warning", "Please Select Type", "warning");
    }
    else if (this.CashSummaryHeader.AccountCode == null) {
      swal("!Warning", "please select Account Name", "warning");
    }
    else if ((this.CashSummaryHeader.TransType == 'PAY') && (this.CashSummaryHeader.DebitAmount == 0 || this.CashSummaryHeader.DebitAmount == null)) {
      swal("!Warning", "Please enter Debit Amount", "warning");
    }
    else if ((this.CashSummaryHeader.TransType == 'REC') && (this.CashSummaryHeader.CreditAmount == 0 || this.CashSummaryHeader.CreditAmount == null)) {
      swal("!Warning", "Please enter Credit Amount", "warning");
    }
    else if (this.CashSummaryHeader.Narration == null) {
      ;
      swal("!Warning", "Please select Narration", "warning");
    }
    else
      var ans = confirm('Do you want to Save');
    if (ans) {
      this.service.PostCashVoucher(this.CashSummaryHeader).subscribe(
        Response => {
          this.PostVoucher = Response;
          swal("success!", "Added successfully", "success");
          this.getcashvouchertable();
          this.getNarrationList();
          this.service.print(this.PostVoucher.VoucherNo, this.PostVoucher.AccountCodeMaster, this.PostVoucher.TransType, this.PostVoucher.AccountType, this.PostVoucher.TransType).subscribe(
            Response => {
              this.PrintDetails = Response;
              $('#ReprintCashVoucher').modal('show');
              $('#DisplayData').html(this.PrintDetails);
            }
          )
          this.clear();
        }
      )
    }
  }
  PutCashVoucher() {
    if (this.CashSummaryHeader.AccountCodeMaster == null) {
      swal("!Warning", "Please select master ledger", "warning");
    }
    else if (this.CashSummaryHeader.TransType == null) {
      swal("!Warning", "Please select type", "warning");
    }
    else if (this.CashSummaryHeader.AccountCode == null) {
      swal("!Warning", "Please select A/C Name", "warning");
    }
    else if ((this.CashSummaryHeader.TransType == 'PAY') && (this.CashSummaryHeader.DebitAmount == 0 || this.CashSummaryHeader.DebitAmount == null)) {

      swal("!Warning", "Please select debit amount", "warning");
    }
    else if ((this.CashSummaryHeader.TransType == 'REC') && (this.CashSummaryHeader.CreditAmount == 0 || this.CashSummaryHeader.CreditAmount == null)) {
      swal("!Warning", "Please enter credit amount", "warning");

    }
    else if (this.CashSummaryHeader.Narration == null) {
      swal("!Warning", "Please select narration", "warning");
    }
    else
      var ans = confirm('Do you want to Save');
    if (ans) {
      this.service.PutCashVoucher(this.CashSummaryHeader, this.CashSummaryHeader.VoucherNo).subscribe(
        Response => {
          this.PostVoucher = Response;
          swal("success!", "Saved successfully", "success");
          this.getcashvouchertable();
          this.getNarrationList();
          this.clear();
        }
      )
    }
  }
  PrintDetails: any;
  PrintVoucher(arg) {
    this.service.print(arg.VoucherNo, arg.AccCodeMaster, this.CashSummaryHeader.TransType, arg.AccType, this.CashSummaryHeader.TransType).subscribe(
      Response => {
        this.PrintDetails = Response;
        $('#ReprintCashVoucher').modal('show');
        $('#DisplayData').html(this.PrintDetails);
      }
    )

  }
  print() {
    let printContents, popupWin;
    printContents = document.getElementById('DisplayData').innerHTML;
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    popupWin.document.open();
    popupWin.document.write(printContents);
    popupWin.document.close();
  }

  changeLeagueOwner(arg): void {
    this.CashSummaryHeader.Narration = arg.Narration;
  }
}