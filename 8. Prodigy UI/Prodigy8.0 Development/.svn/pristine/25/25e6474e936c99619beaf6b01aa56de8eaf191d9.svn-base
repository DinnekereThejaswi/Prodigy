import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { AccountsService } from '../accounts.service';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import { PaymentService } from '../../payment/payment.service';
import { AccVoucherTransactionsVM } from '../accounts.model';
import { DatePipe, formatDate } from '@angular/common';
import swal from 'sweetalert';
declare var $: any;

@Component({
  selector: 'app-journal-entry',
  templateUrl: './journal-entry.component.html',
  styleUrls: ['./journal-entry.component.css'],
  providers: [DatePipe]
})
export class JournalEntryComponent implements OnInit {

  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;
  JournalEntryForm: FormGroup;
  datePickerConfig: Partial<BsDatepickerConfig>;
  today = new Date();
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  EnableJson: boolean = false;
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  constructor(private accountservice: AccountsService,
    private _paymentService: PaymentService, private appConfigService: AppConfigService) {
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        maxDate: this.today,
        dateInputFormat: 'YYYY-MM-DD'
      });
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.EnableJson = this.appConfigService.EnableJson;
    this.password = this.appConfigService.Pwd;
    this.getCB();
  }

  JournalEntryHeader: AccVoucherTransactionsVM = {
    CompanyCode: this.ccode,
    BranchCode: this.bcode,
    TxtSeqNo: 0,
    VoucherSeqNo: 0,
    VoucherNo: 0,
    AccountCode: 0,
    AccountType: null,
    DebitAmount: null,
    CreditAmount: null,
    ChequeNo: "",
    ChequeDate: null,
    FinalPeriod: 0,
    FinalYear: 0,
    AccountCodeMaster: 0,
    Narration: null,
    ReceiptNo: null,
    TransType: null,
    ApprovedBy: null,
    CancelledBy: null,
    CancelledRemarks: null,
    Partyname: null,
    VoucherType: null,
    ReconsileBy: null,
    CurrencyType: null,
    NewVoucherNo: 0,
    SectionID: null,
    VerifiedBy: null,
    VerifiedRemarks: null,
    AuthorizedBy: null,
    AuthorizedRemarks: null,
    CurrencyValue: null,
    ExchangeRate: 0,
    ContraSeqNo: 0,
    ReconsileFlag: null,
    Cflag: null,
    IsApproved: null,
    SubledgerAccCode: 0
  }


  ngOnInit() {
    this.getAccountName();
    this.JournalEntryHeader.CompanyCode = this.ccode,
      this.JournalEntryHeader.BranchCode = this.bcode,
      this.getApplicationdate();
    this.getPaymentType();
    this.getNarrationList();
    $('#ReprintJournalEntryModal').modal('hide');
  }


  voucherDate: any;

  getApplicationdate() {
    this.accountservice.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        let rpDate = appDate["applcationDate"]
        this.voucherDate = rpDate;
        //this.JournalEntryHeader.ChequeDate = rpDate;
      }
    )
  }

  ChequeNo: any = [];
  getChequeNumber() {
    this._paymentService.getChequeList(this.JournalEntryHeader.AccountCodeMaster).subscribe(
      response => {
        this.ChequeNo = response;
      }
    )
  }

  AccountNameList: any = [];
  getAccountName() {
    this.accountservice.getAccountNameforJournalEntry().subscribe(
      Response => {
        this.AccountNameList = Response;
      }
    )
  }

  NarrationList: any = [];
  getNarrationList() {
    this.accountservice.GetNarration().subscribe(
      Response => {
        this.NarrationList = Response;
      }
    )
  }

  clear() {
    this.JournalEntryHeader = {
      CompanyCode: this.ccode,
      BranchCode: this.bcode,
      TxtSeqNo: 0,
      VoucherSeqNo: 0,
      VoucherNo: 0,
      AccountCode: 0,
      AccountType: null,
      DebitAmount: null,
      CreditAmount: null,
      ChequeNo: "",
      ChequeDate: null,
      FinalPeriod: 0,
      FinalYear: 0,
      AccountCodeMaster: null,
      Narration: null,
      ReceiptNo: null,
      TransType: null,
      ApprovedBy: null,
      CancelledBy: null,
      CancelledRemarks: null,
      Partyname: null,
      VoucherType: null,
      ReconsileBy: null,
      CurrencyType: null,
      NewVoucherNo: 0,
      SectionID: null,
      VerifiedBy: null,
      VerifiedRemarks: null,
      AuthorizedBy: null,
      AuthorizedRemarks: null,
      CurrencyValue: null,
      ExchangeRate: 0,
      ContraSeqNo: 0,
      ReconsileFlag: null,
      Cflag: null,
      IsApproved: null,
      SubledgerAccCode: 0
    }
    this.EnableAdd = true;
    this.EnableSave = false;
  }

  PostVoucher: any;
  PrintObject: any;
  MasterLedgerList: any = [];
  JournalList: any = [];
  add() {
    if (this.JournalEntryHeader.AccountCode == null || this.JournalEntryHeader.AccountCode == 0) {
      swal("!Warning", "Please select the Account Name", "warning");
    }
    else if (this.JournalEntryHeader.DebitAmount == null && this.JournalEntryHeader.CreditAmount == null) {
      swal("!Warning", "Please enter Credit/Debit Amount", "warning");
    }
    // else if (this.JournalEntryHeader.Narration != null && this.JournalEntryHeader.Narration != null) {
    //   swal("Warning!", "Please enter Credit/Debit Amount", "warning");
    // }
    else if (this.JournalEntryHeader.DebitAmount != null && this.JournalEntryHeader.CreditAmount != null) {
      swal("!Warning", "Please enter Credit/Debit Amount", "warning");
    }
    else {
      this.JournalEntryHeader.AccountCodeMaster = 0;
      this.JournalEntryHeader.ChequeDate = this.voucherDate;
      this.JournalEntryHeader.ChequeNo = "";
      if (this.JournalEntryHeader.CreditAmount == null) {
        this.JournalEntryHeader.CreditAmount = 0;
      }
      if (this.JournalEntryHeader.DebitAmount == null) {
        this.JournalEntryHeader.DebitAmount = 0;
      }
      this.JournalList.push(this.JournalEntryHeader);
      this.clear();
    }
  }

  getMasterLedgerList() {
    this.accountservice.getMasterLedgerList().subscribe(
      Response => {
        this.MasterLedgerList = Response;
      }
    )
  }

  PaymentType: any = [];

  getPaymentType() {
    this.accountservice.getPaymentType().subscribe(
      Response => {
        this.PaymentType = Response;
      }
    )
  }

  PaymentFlag: string = "";
  EnableCredit: boolean = true;
  EnableDebit: boolean = true;

  chngType(arg) {
    if (arg == "PAY") {
      this.EnableCredit = true;
      this.EnableDebit = false;
      // this.getAccountName(arg);
    }
    else if (arg == "REC") {
      this.EnableCredit = false;
      this.EnableDebit = true;
      // this.getAccountName(arg);
    }
    else {
      this.EnableCredit = true;
      this.EnableDebit = true;
    }
  }

  index: number = 0;

  EditRow(arg, index) {
    this.index = index;
    this.JournalEntryHeader.AccountCodeMaster = 0;
    this.JournalEntryHeader.AccountCode = arg.AccountCode;
    this.JournalEntryHeader.DebitAmount = arg.DebitAmount;
    this.JournalEntryHeader.CreditAmount = arg.CreditAmount;
    this.JournalEntryHeader.Narration = arg.Narration;
    this.JournalEntryHeader.ReceiptNo = arg.ReceiptNo;
    this.JournalEntryHeader.ChequeDate = this.voucherDate;
    this.JournalEntryHeader.ChequeNo = "";
    this.EnableAdd = false;
    this.EnableSave = true;
  }

  DeleteRow(index) {
    var ans = confirm("Do you want to delete??");
    if (ans) {
      this.JournalList.splice(index, 1);
      swal("Deleted!", "Deleted successfully.", "success");
    }
  }

  save() {
    if (this.JournalEntryHeader.AccountCode == null || this.JournalEntryHeader.AccountCode == 0) {
      swal("!Warning", "Please select the Account Name", "warning");
    }
    else if (this.JournalEntryHeader.DebitAmount == null && this.JournalEntryHeader.CreditAmount == null) {
      swal("!Warning", "Please enter Credit/Debit Amount", "warning");
    }
    else if (this.JournalEntryHeader.DebitAmount != null && this.JournalEntryHeader.CreditAmount != null) {
      swal("!Warning", "Please enter Credit/Debit Amount", "warning");
    }
    else {
      this.JournalEntryHeader.ChequeDate = this.voucherDate;
      this.JournalEntryHeader.ChequeNo = "";
      this.JournalEntryHeader.AccountCodeMaster = 0;
      this.JournalList.splice(this.index, 1);
      if (this.JournalEntryHeader.CreditAmount == null) {
        this.JournalEntryHeader.CreditAmount = 0;
      }
      if (this.JournalEntryHeader.DebitAmount == null) {
        this.JournalEntryHeader.DebitAmount = 0;
      }

      this.JournalList.push(this.JournalEntryHeader);
      this.clear();
    }
  }

  PrintDetails: any;

  onPrint(arg) {
    this.accountservice.printJournalEntry(arg).subscribe(
      response => {
        this.PrintDetails = response;
        $('#ReprintJournalEntryModal').modal('show');
        $('#DisplayData').html(this.PrintDetails);
      }
    )
  }

  print() {
    let printContents, popupWin;
    printContents = document.getElementById('DisplayData').innerHTML;
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    popupWin.document.open();
    popupWin.document.write(`
      <html>
        <head>
          <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css" integrity="sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm" crossorigin="anonymous">
          <title>Print tab</title>
          <style>
         .htmlPrint{display:none;}

          @media print {
            .table-bordered
        {
            border-style: solid;
            border: 3px solid rgb(0, 0, 0);
        }
        .margin{
          margin-top:-24px;
        }
        .mb{
          vertical-align:middle;position:absolute;
        }
        tr.spaceUnder>td {
          padding-bottom: 40px !important;
        }
        .top {
          margin-top:10px;
          border-bottom: 3px solid rgb(0, 0, 0) !important;
          line-height: 1.6;
        }
        .modal-content{
          font-family: "Times New Roman", Times, serif;

        }
       .padding-top {
         padding-top:20px;
       }
        .watermark{
          -webkit-transform: rotate(331deg);
          -moz-transform: rotate(331deg);
          -o-transform: rotate(331deg);
          transform: rotate(331deg);
          font-size: 15em;
          color: rgba(255, 5, 5, 0.37);
          position: absolute;
          text-transform:uppercase;
          padding-left: 10%;
          margin-top:-10px;
        }
        .right{
          text-align:left;
        }
        .px-2 {
          padding-left: 10px !important;
        }
        thead > tr
        {
            border-style: solid;
            border: 3px solid rgb(0, 0, 0);
        }
        table tr td.classname{
          border-right: 3px solid rgb(0, 0, 0) !important;
        }
        table tr td.borderLeft{
          border-left: 3px solid rgb(0, 0, 0) !important;
        }
        table tr td.bottom{
          border-bottom: 3px solid rgb(0, 0, 0) !important;
        }
        table tr th.classname{
          border-right: 3px solid rgb(0, 0, 0) !important;
        }
        .divborder {
          border-right: 3px solid rgb(0, 0, 0) !important;
          line-height: 1.6;
        }
        .lastdivborder{
          line-height: 1.6;
        }
    .border{
      border-right: 3px solid rgb(0, 0, 0) !important;
      border-bottom: 3px solid rgb(0, 0, 0) !important;
    }

    .tdborder{
      border-right: 3px solid rgb(0, 0, 0) !important;
    }
        .invoice {
          margin-top: 250px;
      }
        .card{
          border-style: solid;
          border-width: 5px;
          border: 3px solid rgb(0, 0, 0);
      }
        .printMe {
          display: none !important;
        }
      }
      body{
            font-size: 15px;
            line-height: 18px;
      }
 </style>
        </head>
    <body onload="window.print();window.close()">

    ${printContents}</body>
      </html>`
    );
    popupWin.document.close();
  }

  voucherNo: any;

  Submit() {
    if (this.DifferenceFinalAmount != 0.00 || this.DifferenceFinalAmount != 0) {
      swal("Warning!", "Cannot save entry until difference is nill", "warning");
    }
    else {
      var ans = confirm("Do you want to submit??");
      if (ans) {
        this.accountservice.postJournalEntry(this.JournalList).subscribe(
          response => {
            this.voucherNo = response;
            swal("Sucess!", "Voucher no " + this.voucherNo.VoucherNo + " saved successfully.", "success");
            this.clear();
            this.JournalList = [];
            var ans = confirm("Do you want to print??");
            if (ans) {
              this.onPrint(this.voucherNo);
            }
          }
        )
      }
    }
  }

  DifferenceFinalAmount: number = 0;

  differenceAmount() {
    this.DifferenceFinalAmount = this.CreditFinalAmount - this.DebitFinalAmount;
    return this.DifferenceFinalAmount;
  }

  DebitFinalAmount: number = 0;
  CreditFinalAmount: number = 0;

  DebitAmount(arg: any[] = []) { let total = 0; arg.forEach((d) => { total += Number(<number>d.DebitAmount); }); this.DebitFinalAmount = total; return total; }
  CreditAmount(arg: any[] = []) { let total = 0; arg.forEach((d) => { total += Number(<number>d.CreditAmount); }); this.CreditFinalAmount = total; return total; }
}
