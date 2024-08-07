import { Component, OnInit } from '@angular/core';
import { AccountsService } from '../accounts.service';
import { Cheque } from '../accounts.model';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import swal from 'sweetalert';
declare var $: any;


@Component({
  selector: 'app-cheque-closing',
  templateUrl: './cheque-closing.component.html',
  styleUrls: ['./cheque-closing.component.css']
})
export class ChequeClosingComponent implements OnInit {

  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;
  EnableJson: boolean = false;
  ChequeModel: Cheque = {
    ObjID: "",
    CompanyCode: "",
    BranchCode: "",
    AccCode: 0,
    ChqNo: 0,
    NoOfChqs: 0,
    ChqStartNo: 0,
    ChqEndNo: 0,
    ChqIssueDate: null,
    ChqIssued: "",
    ChqClosed: "",
    ChqClosedBy: "",
    ChqClosedRemarks: "",
    MaxAmount: 0
  }

  constructor(private AccountsService: AccountsService,
    private appConfigService: AppConfigService
  ) {
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.EnableJson = this.appConfigService.EnableJson;
    this.password = this.appConfigService.Pwd;
    this.getCB();
  }

  ngOnInit() {
    this.getMasterLedgerList();
    this.getApplicationdate();
    $('#ChqClosing').modal('hide');
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  voucherDate: any;
  getApplicationdate() {
    this.AccountsService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        let rpDate = appDate["applcationDate"]
        this.voucherDate = rpDate;
        //this.BankVoucherEntryHeader.ChequeDate = rpDate;
      }
    )
  }


  MasterLedgerList: any = [];
  getMasterLedgerList() {
    this.AccountsService.getMasterLedgerList().subscribe(
      Response => {
        this.MasterLedgerList = Response;
      }
    )
  }

  ChequeList: any = [];
  BankCode: Number = 0;

  getChqList(arg) {
    this.AccountsService.getChequeClosinglist(arg).subscribe(response => {
      this.ChequeList = response;
      this.BankCode = arg;
    })
  }

  open(arg) {
    if (arg.ChqStatus == "O") {
      swal("!Warning", "Cheque no:" + arg.ChequeNo + "is already open", "warning");
    }
    else {
      var ans = confirm('Do you want to open cheque no = ' + arg.ChequeNo + ' ?');
      if (ans) {
        this.ChequeModel.ChqNo = arg.ChequeNo;
        //this.ChequeModel.=arg.ChqStatus;
        this.ChequeModel.CompanyCode = this.ccode;
        this.ChequeModel.BranchCode = this.bcode;
        this.ChequeModel.ChqIssueDate = this.voucherDate
        this.ChequeModel.ChqClosedRemarks = "";
        this.ChequeModel.ChqClosed = "N";
        this.ChequeModel.ChqClosedBy = localStorage.getItem('Login');
        this.AccountsService.postOpenCloseCheque(this.ChequeModel).subscribe(
          response => {
            swal("Opened!", "Cheque opened successfully.", "success");
            this.getChqList(this.BankCode);
          }
        )
      }
    }
  }

  close(arg) {
    if (arg.ChqStatus == "C") {
      swal("!Warning", "Cheque no : " + arg.ChequeNo + " is already closed", "warning")
    }
    else {
      var ans = confirm('Do you want to close cheque no = ' + arg.ChequeNo + ' ?');
      if (ans) {
        this.ChequeModel.ChqNo = arg.ChequeNo;
        //this.ChequeModel.=arg.ChqStatus;
        this.ChequeModel.CompanyCode = this.ccode;
        this.ChequeModel.BranchCode = this.bcode;
        this.ChequeModel.ChqIssueDate = this.voucherDate;
        this.ChequeModel.ChqClosed = "Y";
        this.ChequeModel.ChqClosedRemarks = arg.ClosedRemarks;
        this.ChequeModel.ChqClosedBy = localStorage.getItem('Login');
        $('#ChqClosing').modal('show');
      }
    }
  }

  SaveRemaks(arg) {
    this.ChequeModel.ChqClosedRemarks = arg;
    this.AccountsService.postOpenCloseCheque(this.ChequeModel).subscribe(
      response => {
        swal("Success!", "Cheque closed successfully.", "success");
        this.getChqList(this.BankCode);
      }
    )
    $('#ChqClosing').modal('hide');
  }

  Cancel() {
    $('#ChqClosing').modal('hide');
    this.ChequeModel = {
      ObjID: "",
      CompanyCode: "",
      BranchCode: "",
      AccCode: 0,
      ChqNo: 0,
      NoOfChqs: 0,
      ChqStartNo: 0,
      ChqEndNo: 0,
      ChqIssueDate: null,
      ChqIssued: "",
      ChqClosed: "",
      ChqClosedBy: "",
      ChqClosedRemarks: "",
      MaxAmount: 0
    }
  }
}