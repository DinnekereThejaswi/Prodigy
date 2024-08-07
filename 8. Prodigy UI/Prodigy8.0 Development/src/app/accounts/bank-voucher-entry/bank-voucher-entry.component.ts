import { Component, OnInit } from '@angular/core'
import { FormGroup, FormBuilder } from '@angular/forms';
import { AccountsService } from '../accounts.service';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { AccVoucherTransactionsVM } from '../accounts.model';
import * as CryptoJS from 'crypto-js';
import { PaymentService } from '../../payment/payment.service';
import { DatePipe } from '@angular/common';
import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';
declare var $: any;

@Component({
  selector: 'app-bank-voucher-entry',
  templateUrl: './bank-voucher-entry.component.html',
  styleUrls: ['./bank-voucher-entry.component.css'],
  providers: [DatePipe]
})

export class BankVoucherEntryComponent implements OnInit {
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;
  BankVoucherEntryForm: FormGroup;
  datePickerConfig: Partial<BsDatepickerConfig>;
  today = new Date();
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  AccountMaster: number = 0;
  TransType: string = null;
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
        dateInputFormat: 'DD/MM/YYYY'
      });
    this.EnableJson = this.appConfigService.EnableJson;
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.password = this.appConfigService.Pwd;
    this.getCB();
  }

  BankVoucherEntryHeader: AccVoucherTransactionsVM = {
    CompanyCode: this.ccode,
    BranchCode: this.bcode,
    TxtSeqNo: 0,
    VoucherSeqNo: 0,
    VoucherNo: 0,
    AccountCode: 0,
    AccountType: null,
    DebitAmount: 0,
    CreditAmount: 0,
    ChequeNo: null,
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

  ngOnInit() {
    this.getMasterLedger();
    this.BankVoucherEntryHeader.CompanyCode = this.ccode,
      this.BankVoucherEntryHeader.BranchCode = this.bcode,
      this.getApplicationdate();
    this.getPaymentType();
    this.getNarrationList();
    this.getMasterLedgerList();
    $('#ReprintBankVoucherModal').modal('hide');
  }

  MasterLedger: any = [];
  getMasterLedger() {
    this.accountservice.GetMasterLedger().subscribe(
      Response => {
        this.MasterLedger = Response;
      }
    )
  }

  voucherDate: any;

  getApplicationdate() {
    this.accountservice.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        let rpDate = appDate["applcationDate"]
        this.voucherDate = rpDate;
        //this.BankVoucherEntryHeader.ChequeDate = rpDate;
      }
    )
  }

  ChequeNo: any = [];
  getChequeNumber() {
    this._paymentService.getChequeList(this.BankVoucherEntryHeader.AccountCodeMaster).subscribe(
      response => {
        this.ChequeNo = response;
      }
    )
  }

  BankVoucherList: any = [];

  BankVoucherFinalJsonList: any = [];

  ApplicationNameList: any;
  MasterLedgerCode: number = 0;
  getAccountName(arg) {
    this.accountservice.GetAccountNames(arg).subscribe(
      Response => {
        this.ApplicationNameList = Response;
        this.getBankVoucherEntryList(arg);
      }
    )
  }

  AccName: any;
  getAccName(arg) {
    this.AccName = this.ApplicationNameList.filter(x => x.acc_code == arg);
    this.AccName = this.AccName[0].acc_name;
  }

  getBankVoucherEntryList(arg) {
    this.MasterLedgerCode = this.BankVoucherEntryHeader.AccountCodeMaster;
    this.accountservice.getBankVoucherEntryList(this.BankVoucherEntryHeader.AccountCodeMaster, arg, this.voucherDate).subscribe(
      response => {
        this.BankVoucherList = response;
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
    this.BankVoucherEntryHeader = {
      CompanyCode: this.ccode,
      BranchCode: this.bcode,
      TxtSeqNo: 0,
      VoucherSeqNo: 0,
      VoucherNo: 0,
      AccountCode: 0,
      AccountType: null,
      DebitAmount: 0,
      CreditAmount: 0,
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
  add() {
    if (this.BankVoucherEntryHeader.AccountCodeMaster == null || this.BankVoucherEntryHeader.AccountCodeMaster == 0) {
      swal("!Warning", "Please select the Master Ledger", "warning");
    }
    else if (this.BankVoucherEntryHeader.TransType == null) {
      swal("!Warning", "Please select the Type", "warning");
    }
    else if (this.BankVoucherEntryHeader.AccountCode == null || this.BankVoucherEntryHeader.AccountCode == 0) {
      swal("!Warning", "Please select the Account Name", "warning");
    }
    else if (this.EnableDebit == false && (this.BankVoucherEntryHeader.DebitAmount == 0 || this.BankVoucherEntryHeader.DebitAmount == null)) {
      swal("!Warning", "Please enter the Debit Amount", "warning");
    }
    else if (this.EnableCredit == false && (this.BankVoucherEntryHeader.CreditAmount == 0 || this.BankVoucherEntryHeader.CreditAmount == null)) {
      swal("!Warning", "Please enter Credit Amount", "warning");
    }
    else {
      //Post BankVoucher Entry
      // var ans = confirm("Do you want to save??");
      // if (ans) {
      // this.accountservice.postBankVoucherEntry(this.BankVoucherEntryHeader).subscribe(
      //   response => {
      //     this.PrintObject = response;
      //     this.BankVoucherEntryHeader.NewVoucherNo=this.PrintObject.VoucherNo;
      //     this.onPrint(this.PrintObject);
      //     swal("Saved!", "Bank voucher added successfully.", "success");
      //     this.getBankVoucherEntryList(this.BankVoucherEntryHeader.TransType);
      //     this.clear();
      //   }
      // )        
      //}

      this.AccountMaster = this.BankVoucherEntryHeader.AccountCodeMaster;
      this.TransType = this.BankVoucherEntryHeader.TransType;
      this.BankVoucherEntryHeader.Partyname = this.AccName;
      this.BankVoucherFinalJsonList.push(this.BankVoucherEntryHeader);
      this.clear();
      this.BankVoucherEntryHeader.AccountCodeMaster = this.AccountMaster;
      this.BankVoucherEntryHeader.TransType = this.TransType;
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
let intitalValue =this.BankVoucherEntryHeader.AccountCodeMaster;
    this.BankVoucherEntryHeader = {
      CompanyCode: this.ccode,
      BranchCode: this.bcode,
      TxtSeqNo: 0,
      VoucherSeqNo: 0,
      VoucherNo: 0,
      AccountCode: 0,
      AccountType: null,
      DebitAmount: 0,
      CreditAmount: 0,
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
    this.BankVoucherEntryHeader.TransType=arg;
    this.BankVoucherEntryHeader.AccountCodeMaster=intitalValue;
    if (arg == "PAY") {

      this.EnableCredit = true;
      this.EnableDebit = false;
      this.BankVoucherEntryHeader.CreditAmount=0;
      this.getAccountName(arg);

    }
    else if (arg == "REC") {
      this.EnableCredit = false;
      this.EnableDebit = true;
      this.BankVoucherEntryHeader.DebitAmount=0;
      this.getAccountName(arg);

    }
    else {
      this.EnableCredit = true;
      this.EnableDebit = true;
    }
  }

  VoucherDetails: any = [];

  EditRow(arg) {
    // this.BankVoucherEntryHeader.VoucherNo = arg.VoucherNo;
    // this.BankVoucherEntryHeader.AccountCode = arg.AccCode;
    // this.BankVoucherEntryHeader.ChequeDate = arg.ChequeDate;
    // this.BankVoucherEntryHeader.DebitAmount = arg.Debit;
    // this.BankVoucherEntryHeader.CreditAmount = arg.Credit;
    // this.BankVoucherEntryHeader.Narration = arg.Narration;
    // this.BankVoucherEntryHeader.ReceiptNo = arg.RefNo;
    // this.BankVoucherEntryHeader.AccountCodeMaster = arg.AccCodeMaster;
    // this.BankVoucherEntryHeader.ChequeNo = arg.ChequeNo;
    // this.EnableAdd = false;
    // this.EnableSave = true;

    this.accountservice.getBankVoucherDetails(arg, this.BankVoucherEntryHeader.TransType).subscribe(
      response => {
        this.VoucherDetails = response;
        this.BankVoucherFinalJsonList = this.VoucherDetails;
      }
    )
  }

  index: number = 0;

  EditTempRow(arg, index) {
    this.index = index;
    this.BankVoucherEntryHeader.TxtSeqNo = arg.TxtSeqNo;
    this.BankVoucherEntryHeader.VoucherNo = arg.VoucherNo;
    this.BankVoucherEntryHeader.AccountCode = arg.AccountCode;
    this.BankVoucherEntryHeader.ChequeDate = arg.ChequeDate;
    this.BankVoucherEntryHeader.DebitAmount = arg.DebitAmount;
    this.BankVoucherEntryHeader.CreditAmount = arg.CreditAmount;
    this.BankVoucherEntryHeader.Narration = arg.Narration;
    this.BankVoucherEntryHeader.ReceiptNo = arg.ReceiptNo;
    this.BankVoucherEntryHeader.AccountCodeMaster = arg.AccountCodeMaster;
    this.BankVoucherEntryHeader.TransType = arg.TransType;
    this.BankVoucherEntryHeader.ChequeNo = arg.ChequeNo;
    this.BankVoucherEntryHeader.Partyname = arg.Partyname;
    this.EnableAdd = false;
    this.EnableSave = true;
  }

  DeleteRow(arg) {
    var ans = confirm("Do you want to delete??");
    if (ans) {
      this.BankVoucherEntryHeader.VoucherNo = arg.VoucherNo;
      this.BankVoucherEntryHeader.AccountCode = arg.AccCode;
      this.BankVoucherEntryHeader.ChequeDate = arg.ChequeDate;
      this.BankVoucherEntryHeader.DebitAmount = arg.Debit;
      this.BankVoucherEntryHeader.CreditAmount = arg.Credit;
      this.BankVoucherEntryHeader.Narration = arg.Narration;
      this.BankVoucherEntryHeader.ReceiptNo = arg.RefNo;
      this.BankVoucherEntryHeader.AccountCodeMaster = arg.AccCodeMaster;
      this.BankVoucherEntryHeader.ChequeNo = arg.ChequeNo;
      this.accountservice.deleteBankVoucherEntry(this.BankVoucherEntryHeader).subscribe(
        response => {
          swal("Deleted!", "Deleted successfully.", "success");
          this.getBankVoucherEntryList(this.BankVoucherEntryHeader.TransType);
          this.clear();
        }
      )
    }
  }

  save() {
    if (this.BankVoucherEntryHeader.AccountCodeMaster == null || this.BankVoucherEntryHeader.AccountCodeMaster == 0) {
      swal("!Warning", "'Please select the Master Ledgert", "warning");
    }
    else if (this.BankVoucherEntryHeader.TransType == null) {
      swal("!Warning", "Please select the Type", "warning");
    }
    else if (this.BankVoucherEntryHeader.AccountCode == null || this.BankVoucherEntryHeader.AccountCode == 0) {
      swal("!Warning", "Please select the Account Name", "warning");
    }
    else if (this.EnableDebit == false && (this.BankVoucherEntryHeader.DebitAmount == 0 || this.BankVoucherEntryHeader.DebitAmount == null)) {
      swal("!Warning", "Please enter the Debit Amount", "warning");
    }
    else if (this.EnableCredit == false && (this.BankVoucherEntryHeader.CreditAmount == 0 || this.BankVoucherEntryHeader.CreditAmount == null)) {
      swal("!Warning", "Please enter Credit Amount", "warning");
    }
    else {
      //Post BankVoucher Entry
      // var ans = confirm("Do you want to save??");
      // if (ans) {
      //   this.accountservice.putBankVoucherEntry(this.BankVoucherEntryHeader).subscribe(
      //     response => {
      //       swal("Saved!", "Bank voucher saved successfully.", "success");
      //       this.BankVoucherEntryHeader.NewVoucherNo=this.BankVoucherEntryHeader.VoucherNo;
      //       this.onPrint(this.BankVoucherEntryHeader);
      //       this.getBankVoucherEntryList(this.BankVoucherEntryHeader.TransType);
      //       this.clear();
      //     }
      //   )
      // }
      if (this.AccName != undefined) {
        this.BankVoucherEntryHeader.Partyname = this.AccName;
      }
      this.BankVoucherFinalJsonList.splice(this.index, 1);
      this.BankVoucherFinalJsonList.push(this.BankVoucherEntryHeader);
      this.clear();
    }
  }

  PrintDetails: any;

  onPrint(arg) {
    this.accountservice.printBankVoucherEntry(arg.VoucherNo, arg.AccountCodeMaster, arg.TransType, arg.AccountType).subscribe(
      response => {
        this.PrintDetails = response;
        $('#ReprintBankVoucherModal').modal('show');
        $('#DisplayData').html(this.PrintDetails);
      }
    )
  }

  DeleteTempRow(arg, index) {
    var ans = confirm("Do you want to delete??");
    if (ans) {
      this.BankVoucherFinalJsonList.splice(index, 1);
    }
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

  Submitted: any;

  Submit() {
    //Post BankVoucher Entry
    var ans = confirm("Do you want to submit??");
    if (ans) {
      if (this.BankVoucherFinalJsonList[0].VoucherNo != "0" && this.BankVoucherFinalJsonList[0].VoucherNo != "") {
        this.accountservice.putBankVoucherEntry(this.BankVoucherFinalJsonList).subscribe(
          response => {
            this.Submitted = response;
            swal("Updated!", "Bank voucher number: " + this.Submitted.VoucherNo + "  updated successfully.", "success");
            this.BankVoucherEntryHeader.NewVoucherNo = this.Submitted.VoucherNo;
            this.accountservice.getBankVoucherEntryList(this.MasterLedgerCode, this.Submitted.TransType, this.voucherDate).subscribe(
              response => {
                this.BankVoucherList = response;
                this.onPrint(this.Submitted);
                this.BankVoucherEntryHeader.AccountCodeMaster = this.MasterLedgerCode;
                this.BankVoucherEntryHeader.TransType = this.TransType;
                this.clear();
                this.BankVoucherFinalJsonList = [];
              }
            )
          }
        )
      }
      else {
        this.accountservice.postBankVoucherEntry(this.BankVoucherFinalJsonList).subscribe(
          response => {
            this.Submitted = response;
            swal("Submitted!", "Bank voucher number: " + this.Submitted.VoucherNo + " submitted successfully.", "success");
            this.BankVoucherEntryHeader.NewVoucherNo = this.Submitted.VoucherNo;
            this.accountservice.getBankVoucherEntryList(this.MasterLedgerCode, this.Submitted.TransType, this.voucherDate).subscribe(
              response => {
                this.BankVoucherList = response;
                this.onPrint(this.Submitted);
                this.BankVoucherEntryHeader.AccountCodeMaster = this.MasterLedgerCode;
                this.BankVoucherEntryHeader.TransType = this.TransType;
                this.clear();
                this.BankVoucherFinalJsonList = [];
              }
            )
          }
        )
      }
    }
  }
}