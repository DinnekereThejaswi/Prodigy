import { Component, OnInit, ɵConsole } from '@angular/core'
import { FormGroup, FormBuilder } from '@angular/forms';
import { AccountsService } from '../accounts.service';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import { Response } from '@angular/http';
import { DatePipe, formatDate } from '@angular/common';
import swal from 'sweetalert';
declare var jquery: any;
declare var $: any;
@Component({
  selector: 'app-contra-entry',
  templateUrl: './contra-entry.component.html',
  styleUrls: ['./contra-entry.component.css'],
  providers: [DatePipe]
})

export class ContraEntryComponent implements OnInit {
  searchText;
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  contraEntryForm: FormGroup;
  datePickerConfig: Partial<BsDatepickerConfig>;
  today = new Date();
  EnableDebit: boolean = true;
  EnableCredit: boolean = true;
  EnableChequeNo: boolean = true;
  EnableChequeDate: boolean = true;
  Type: any;
  typevalue = null;
  EnableJson: boolean = false;
  constructor(private fb: FormBuilder,
    private service: AccountsService,
    private datePipe: DatePipe, private appConfigService: AppConfigService) {

    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        maxDate: this.today,
        dateInputFormat: 'DD-MM-YYYY'
      });
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.EnableJson = this.appConfigService.EnableJson;
    this.password = this.appConfigService.Pwd;
    this.getCB();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ContraEntryHeader: any = {
    objID: null,
    CompanyCode: this.ccode,
    BranchCode: this.bcode,
    TxtSeqNo: 0,
    VoucherSeqNo: 0,
    VoucherNo: 0,
    AccountCode: null,
    AccountType: "N",
    DebitAmount: 0,
    CreditAmount: 0,
    ChequeNo: 0,
    ChequeDate: null,
    FinalYear: 0,
    FinalPeriod: 0,
    AccountCodeMaster: null,
    Narration: null,
    ReceiptNo: 0,
    TransType: "CONT",
    ApprovedBy: null,
    CancelledRemarks: null,
    CancelledBy: null,
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
    ExchangeRate: 1.0,
    CurrencyValue: 1.0,
    ContraSeqNo: 0,
    ReconsileFlag: null,
    Cflag: null,
    IsApproved: null,
    SubledgerAccCode: 1
  }

  ngOnInit() {
    this.ContraEntryHeader.CompanyCode = this.ccode;
    this.ContraEntryHeader.BranchCode = this.bcode;
    this.getApplicationDate();
    this.getType();
    this.getAccountNames();
    this.getNarrationList();

    this.contraEntryForm = this.fb.group({
      applicationDate: null,
      CompanyCode: this.ccode,
      BranchCode: this.bcode
    });

  }

  applicationDate: any;
  appViewDate: any;
  appDate: any = [];
  rpDate: any = [];
  getApplicationDate() {
    this.service.getApplicationDate().subscribe(
      Response => {
        this.appDate = Response;
        this.rpDate = this.appDate["applcationDate"];
        this.applicationDate = this.appDate["applcationDate"];
        this.ContraEntryHeader.ChequeDate = this.rpDate;
      }
    )
  }

  TransTypeList: any = [];
  getType() {
    this.service.getContraType().subscribe(
      Response => {
        this.TransTypeList = Response;
      }
    )
  }

  TypeName: string = null;
  TypeCode: string = "";
  onTypeSelected(arg) {
    this.Type = this.TransTypeList.filter(x => x.Name == arg);
    this.TypeName = this.Type[0].Name;
    this.TypeCode = this.Type[0].Code;

    if (arg == "CASH DEPOSIT") {
      this.EnableDebit = false;
      this.EnableCredit = true;
      this.EnableChequeNo = true;
      this.EnableChequeDate = true;
    }
    else if (arg == "CASH WITHDRAWAL") {
      this.EnableDebit = true;
      this.EnableCredit = false;
      this.EnableChequeNo = true;
      this.EnableChequeDate = true;
    }
    else {
      this.EnableDebit = false;
      this.EnableCredit = true;
      this.EnableChequeNo = false;
      this.EnableChequeDate = false;

    }

    this.service.getContraLedger(this.TypeCode).subscribe(
      Response => {
        this.LedgerList = Response;
        this.getContraEntryTable();
      }
    )
  }

  LedgerList: any = [];
  typearg: any = [];
  getLedger() {
    this.service.getContraLedger(this.Type).subscribe(
      Response => {

        this.LedgerList = Response;
        this.getContraEntryTable();
      }
    )
  }

  ApplicationNameList: any = [];
  getAccountNames() {
    this.service.GetContraAccountNames().subscribe(
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

  ContraEntryList: any = []
  getContraEntryTable() {
    this.ContraEntryHeader.TransType = 'CONT';
    this.service.getContraEntryList(this.ContraEntryHeader.AccountCodeMaster, this.rpDate).subscribe(
      Response => {
        this.ContraEntryList = Response;
      }
    )
  }

  GetContraList: any = [];
  getContraEntryList(arg) {
    this.EnableAdd = false;
    this.EnableSave = true;
    this.service.editGetContraEntry(arg.VoucherNo, arg.AccCode, arg.AccCodeMaster).subscribe(
      Response => {
        this.GetContraList = Response;
        this.ContraEntryHeader = this.GetContraList;
      }
    )
  }

  clear() {
    this.EnableAdd = true;
    this.EnableSave = false;
    this.ContraEntryHeader.AccountCode = null;
    this.ContraEntryHeader.DebitAmount = 0;
    this.ContraEntryHeader.CreditAmount = 0;
    this.ContraEntryHeader.Narration = null;
    this.ContraEntryHeader.ReceiptNo = 0;
    this.ContraEntryHeader.ChequeNo = 0;
  }

  PrintContraDetails: any;
  PrintContraEntry(arg) {
    arg.TransType = this.ContraEntryHeader.TransType;
    this.service.printContraEntry(arg.VoucherNo, arg.AccCodeMaster, arg.TransType, arg.AccType, arg.TransType).subscribe(
      Response => {
        this.PrintContraDetails = Response;
        $('#ReprintContraEntry').modal('show');
        $('#DisplayData').html(this.PrintContraDetails);
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

  PutContraEntry: any;
  putContraEntryTable() {
    if (this.ContraEntryHeader.AccountCode == null) {
      swal("!Warning", "Please select the Account Name.", "warning");
    }
    else if ((this.ContraEntryHeader.TransType == "PAY") && (this.ContraEntryHeader.DebitAmount == 0)) {
      swal("!Warning", "Please enter the Debit Amount", "warning");
    }
    else if ((this.ContraEntryHeader.TransType == "REC") && (this.ContraEntryHeader.CreditAmount == 0)) {
      swal("!Warning", "Please enter the Credit Amount", "warning");
    }
    else if (this.ContraEntryHeader.ReceiptNo == 0) {
      swal("!Warning", "Please enter the Ref No.", "warning");
    }
    else if ((this.Type == 'B') && (this.ContraEntryHeader.AccountCodeMaster == this.ContraEntryHeader.AccountCode)) {
      swal("!Warning", "Master Ledger and Account Name could not be same", "warning");
    }
    else if (this.Type == 'B' && this.ContraEntryHeader.ChequeNo == 0) {
      swal("!Warning", "Please enter the Cheque No.", "warning");
    }
    else
      var ans = confirm('Do you want to Save');
    if (ans) {
      this.ContraEntryHeader.TransType = 'CONT';
      this.service.PutContraVoucher(this.ContraEntryHeader, this.ContraEntryHeader.VoucherNo).subscribe(
        Response => {
          this.PostContra = Response;
          swal("success!", "Saved successfully", "success");
          this.getContraEntryTable();
          this.clear();
        }
      )
    }
  }
  PostContra: any;
  postContraEntry() {

    if (this.ContraEntryHeader.AccountCodeMaster == null) {
      swal("!Warning", "Please select the Master Ledger.", "warning");
    }
    else if (this.ContraEntryHeader.AccountCode == null) {
      swal("!Warning", "Please select the Account Name.", "warning");
    }

    else if ((this.ContraEntryHeader.TransType == "PAY") && (this.ContraEntryHeader.DebitAmount == 0)) {
      swal("!!Warning", "Please enter the Debit Amount", "warning");
    }

    else if ((this.ContraEntryHeader.TransType == "REC") && (this.ContraEntryHeader.CreditAmount == 0)) {
      swal("!!Warning", "Please enter the Credit Amount", "warning");
    }

    else if (this.ContraEntryHeader.ReceiptNo == 0) {
      swal("!!Warning", "Please enter the Ref No.", "warning");
    }

    else if ((this.Type == 'B') && (this.ContraEntryHeader.AccountCodeMaster == this.ContraEntryHeader.AccountCode)) {
      swal("!!Warning", "Master Ledger and Account Name could not be same.", "warning");
    }

    else if (this.Type == 'B' && this.ContraEntryHeader.ChequeNo == 0) {
      swal("!!Warning", "Please enter the Cheque No.", "warning");
    }

    else
      var ans = confirm('Do you want to Save');

    if (ans) {
      this.service.PostContraVoucher(this.ContraEntryHeader).subscribe(
        Response => {
          this.PostContra = Response;
          swal("success!", "Saved successfully", "success");
          this.getContraEntryTable();
          this.clear();
        }
      )

    }
  }
}