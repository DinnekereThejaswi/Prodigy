import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AccountsService } from '../accounts.service';
import { TransactionPosting } from '../accounts.model';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import swal from 'sweetalert';
declare var $: any;

@Component({
  selector: 'app-account-posting-setup',
  templateUrl: './account-posting-setup.component.html',
  styleUrls: ['./account-posting-setup.component.css']
})
export class AccountPostingSetupComponent implements OnInit {

  CollapseTransactionsPostingTab: boolean = true;
  CollapsePayModePostingTab: boolean = true;
  CollapseIssueReceiptsTab: boolean = true;
  CollapseCPCPostingTab: boolean = true;
  LedgerForm: FormGroup;
  gsCode: any = [];
  payModes: any = [];
  ledgerPayModeList: any = [];
  ledgerSalesList: any = [];
  ledgerPurchaseList: any = [];
  ledgerStoneIssueList: any = [];
  ledgerIssueList: any = [];
  ledgerSalesReturnList: any = [];
  ledgerReceiptList: any = [];
  ledgerStonePurchaseList: any = [];
  ledgerStoneReceiptList: any = [];
  password: string;
  EnableJson: boolean = false;

  ledgerPost: TransactionPosting = {
    CompanyCode: null,
    BranchCode: null,
    GSCode: null,
    TransType: null,
    AccountCode: 0,
    OldAccountCode: 0
  }


  constructor(private _accountsService: AccountsService,
    private formBuilder: FormBuilder, private _appConfigService: AppConfigService) {
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
    this.getCB();
  }

  ccode: string = "";
  bcode: string = "";

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.ledgerPost.CompanyCode = this.ccode;
    this.ledgerPost.BranchCode = this.bcode;
  }

  ngOnInit() {
    this.LoadAllLedgers();
    this.LedgerForm = this.formBuilder.group({
      ledger: null
    });
  }

  LoadAllLedgers() {
    this.GsCodes();
    this.PayModes();
    this.ledgerSales();
    this.ledgerPurchase();
    this.ledgerStoneIssue();
    this.ledgerIssues();
    this.ledgerSalesReturn();
    this.ledgerReceipt();
    this.ledgerStonePurchase();
    this.ledgerStoneReceipt();
    this.ledgerPaymode();
  }

  SalesLedgerName: any;
  SRLedgerName: any;
  PurchaseLedgerName: any;
  IssuesLedgerName: any;
  ReceiptLedgerName: any;
  SILedgerName: any;
  StoneReceiptLedgerName: any;
  SPLedgerName: any;
  PayModeLedgerName: any;

  TransType: any;

  ToggleTransactionPosting() {
    this.CollapseTransactionsPostingTab = !this.CollapseTransactionsPostingTab;
  }

  TogglePayModePosting() {
    this.CollapsePayModePostingTab = !this.CollapsePayModePostingTab;
  }

  getSalesLedger(arg) {
    this.SalesLedgerName = this.ledgerSalesList.filter(value => value.GSCode === arg);
    if (this.SalesLedgerName.length > 0) {
      this.SalesLedgerName = this.SalesLedgerName[0].Name + " - " + this.SalesLedgerName[0].Code;
    }
    else {
      this.SalesLedgerName = "";
    }
    return this.SalesLedgerName;
  }

  getSRLedger(arg) {
    this.SRLedgerName = this.ledgerSalesReturnList.filter(value => value.GSCode === arg);
    if (this.SRLedgerName.length > 0) {
      this.SRLedgerName = this.SRLedgerName[0].Name + " - " + this.SRLedgerName[0].Code;
    }
    else {
      this.SRLedgerName = "";
    }
    return this.SRLedgerName;
  }

  getPurchaseLedger(arg) {
    this.PurchaseLedgerName = this.ledgerPurchaseList.filter(value => value.GSCode === arg);
    if (this.PurchaseLedgerName.length > 0) {
      this.PurchaseLedgerName = this.PurchaseLedgerName[0].Name + " - " + this.PurchaseLedgerName[0].Code;
    }
    else {
      this.PurchaseLedgerName = "";
    }
    return this.PurchaseLedgerName;
  }

  getIssuesLedger(arg) {
    this.IssuesLedgerName = this.ledgerIssueList.filter(value => value.GSCode === arg);
    if (this.IssuesLedgerName.length > 0) {
      this.IssuesLedgerName = this.IssuesLedgerName[0].Name + " - " + this.IssuesLedgerName[0].Code;
    }
    else {
      this.IssuesLedgerName = "";
    }
    return this.IssuesLedgerName;
  }

  getReceiptLedger(arg) {
    this.ReceiptLedgerName = this.ledgerReceiptList.filter(value => value.GSCode === arg);
    if (this.ReceiptLedgerName.length > 0) {
      this.ReceiptLedgerName = this.ReceiptLedgerName[0].Name + " - " + this.ReceiptLedgerName[0].Code;
    }
    else {
      this.ReceiptLedgerName = "";
    }
    return this.ReceiptLedgerName;
  }

  getSILedger(arg) {
    this.SILedgerName = this.ledgerStoneIssueList.filter(value => value.GSCode === arg);
    if (this.SILedgerName.length > 0) {
      this.SILedgerName = this.SILedgerName[0].Name + " - " + this.SILedgerName[0].Code;
    }
    else {
      this.SILedgerName = "";
    }
    return this.SILedgerName;
  }

  getStoneReceiptLedger(arg) {
    this.StoneReceiptLedgerName = this.ledgerStoneReceiptList.filter(value => value.GSCode === arg);
    if (this.StoneReceiptLedgerName.length > 0) {
      this.StoneReceiptLedgerName = this.StoneReceiptLedgerName[0].Name + " - " + this.StoneReceiptLedgerName[0].Code;
    }
    else {
      this.StoneReceiptLedgerName = "";
    }
    return this.StoneReceiptLedgerName;
  }

  getSPLedger(arg) {
    this.SPLedgerName = this.ledgerStonePurchaseList.filter(value => value.GSCode === arg);
    if (this.SPLedgerName.length > 0) {
      this.SPLedgerName = this.SPLedgerName[0].Name + " - " + this.SPLedgerName[0].Code;
    }
    else {
      this.SPLedgerName = "";
    }
    return this.SPLedgerName;
  }

  getPayModeLedger(arg) {
    this.PayModeLedgerName = this.ledgerPayModeList.filter(value => value.Code === arg);
    if (this.PayModeLedgerName.length > 0) {
      this.PayModeLedgerName = this.PayModeLedgerName[0].Name + " - " + this.PayModeLedgerName[0].Code;
    }
    else {
      this.PayModeLedgerName = "";
    }
    return this.PayModeLedgerName;
  }

  GsCodes() {
    this._accountsService.getGS().subscribe(
      response => {
        this.gsCode = response;
      }
    )
  }

  PayModes() {
    this._accountsService.getPayMode().subscribe(
      response => {
        this.payModes = response;
      }
    )
  }

  ledgerSales() {
    this._accountsService.getLedger("S").subscribe(
      response => {
        this.ledgerSalesList = response;
      }
    )
  }

  ledgerPurchase() {
    this._accountsService.getLedger("P").subscribe(
      response => {
        this.ledgerPurchaseList = response;
      }
    )
  }

  ledgerStoneIssue() {
    this._accountsService.getLedger("SI").subscribe(
      response => {
        this.ledgerStoneIssueList = response;
      }
    )
  }

  ledgerIssues() {
    this._accountsService.getLedger("I").subscribe(
      response => {
        this.ledgerIssueList = response;
      }
    )
  }

  ledgerSalesReturn() {
    this._accountsService.getLedger("RS").subscribe(
      response => {
        this.ledgerSalesReturnList = response;
      }
    )
  }

  ledgerReceipt() {
    this._accountsService.getLedger("R").subscribe(
      response => {
        this.ledgerReceiptList = response;
      }
    )
  }

  ledgerStonePurchase() {
    this._accountsService.getLedger("SD").subscribe(
      response => {
        this.ledgerStonePurchaseList = response;
      }
    )
  }

  ledgerStoneReceipt() {
    this._accountsService.getLedger("SR").subscribe(
      response => {
        this.ledgerStoneReceiptList = response;
      }
    )
  }

  ledgerPaymode() {
    this._accountsService.getLedger("PM").subscribe(
      response => {
        this.ledgerPayModeList = response;
      }
    )
  }

  ledgerList: any = [];
  ledgerSplit: any = [];

  EditDetails(TransType, arg, gsCode) {
    $('#LedgerPopup').modal('show');
    switch (TransType) {
      case "S": {
        this.ledgerList = this.ledgerSalesList;
        this.ledgerPost.TransType = TransType;
        this.ledgerPost.GSCode = gsCode;
        if (arg != null && arg != "") {
          this.ledgerSplit = arg.split('-');
          this.ledgerPost.AccountCode = Number(this.ledgerSplit[1].trim());
          this.ledgerPost.OldAccountCode = Number(this.ledgerSplit[1].trim());
        }
        else {
          this.ledgerPost.AccountCode = 0;
          this.ledgerPost.OldAccountCode = 0;
        }
        break;
      }
      case "RS": {
        this.ledgerList = this.ledgerSalesReturnList;
        this.ledgerPost.TransType = TransType;
        this.ledgerPost.GSCode = gsCode;
        if (arg != null && arg != "") {
          this.ledgerSplit = arg.split('-');
          this.ledgerPost.AccountCode = Number(this.ledgerSplit[1].trim());
          this.ledgerPost.OldAccountCode = Number(this.ledgerSplit[1].trim());
        }
        else {
          this.ledgerPost.AccountCode = 0;
          this.ledgerPost.OldAccountCode = 0;
        }
        break;
      }
      case "P": {
        this.ledgerList = this.ledgerPurchaseList;
        this.ledgerPost.TransType = TransType;
        this.ledgerPost.GSCode = gsCode;
        if (arg != null && arg != "") {
          this.ledgerSplit = arg.split('-');
          this.ledgerPost.AccountCode = Number(this.ledgerSplit[1].trim());
          this.ledgerPost.OldAccountCode = Number(this.ledgerSplit[1].trim());
        }
        else {
          this.ledgerPost.AccountCode = 0;
          this.ledgerPost.OldAccountCode = 0;
        }
        break;
      }
      case "I": {
        this.ledgerList = this.ledgerIssueList;
        this.ledgerPost.TransType = TransType;
        this.ledgerPost.GSCode = gsCode;
        if (arg != null && arg != "") {
          this.ledgerSplit = arg.split('-');
          this.ledgerPost.AccountCode = Number(this.ledgerSplit[1].trim());
          this.ledgerPost.OldAccountCode = Number(this.ledgerSplit[1].trim());
        }
        else {
          this.ledgerPost.AccountCode = 0;
          this.ledgerPost.OldAccountCode = 0;
        }
        break;
      }
      case "R": {
        this.ledgerList = this.ledgerReceiptList;
        this.ledgerPost.TransType = TransType;
        this.ledgerPost.GSCode = gsCode;
        if (arg != null && arg != "") {
          this.ledgerSplit = arg.split('-');
          this.ledgerPost.AccountCode = Number(this.ledgerSplit[1].trim());
          this.ledgerPost.OldAccountCode = Number(this.ledgerSplit[1].trim());
        }
        else {
          this.ledgerPost.AccountCode = 0;
          this.ledgerPost.OldAccountCode = 0;
        }
        break;
      }
      case "SI": {
        this.ledgerList = this.ledgerStoneIssueList;
        this.ledgerPost.TransType = TransType;
        this.ledgerPost.GSCode = gsCode;
        if (arg != null && arg != "") {
          this.ledgerSplit = arg.split('-');
          this.ledgerPost.AccountCode = Number(this.ledgerSplit[1].trim());
          this.ledgerPost.OldAccountCode = Number(this.ledgerSplit[1].trim());
        }
        else {
          this.ledgerPost.AccountCode = 0;
          this.ledgerPost.OldAccountCode = 0;
        }
        break;
      }
      case "SD": {
        this.ledgerList = this.ledgerStonePurchaseList;
        this.ledgerPost.TransType = TransType;
        this.ledgerPost.GSCode = gsCode;
        if (arg != null && arg != "") {
          this.ledgerSplit = arg.split('-');
          this.ledgerPost.AccountCode = Number(this.ledgerSplit[1].trim());
          this.ledgerPost.OldAccountCode = Number(this.ledgerSplit[1].trim());
        }
        else {
          this.ledgerPost.AccountCode = 0;
          this.ledgerPost.OldAccountCode = 0;
        }
        break;
      }
      case "SR": {
        this.ledgerList = this.ledgerStoneReceiptList;
        this.ledgerPost.TransType = TransType;
        this.ledgerPost.GSCode = gsCode;
        if (arg != null && arg != "") {
          this.ledgerSplit = arg.split('-');
          this.ledgerPost.AccountCode = Number(this.ledgerSplit[1].trim());
          this.ledgerPost.OldAccountCode = Number(this.ledgerSplit[1].trim());
        }
        else {
          this.ledgerPost.AccountCode = 0;
          this.ledgerPost.OldAccountCode = 0;
        }
        break;
      }
      case "PM": {
        this.ledgerList = this.ledgerPayModeList;
        this.ledgerPost.TransType = TransType;
        this.ledgerPost.GSCode = null;
        if (arg != null && arg != "") {
          this.ledgerSplit = arg.split('-');
          this.ledgerPost.AccountCode = Number(this.ledgerSplit[1].trim());
          this.ledgerPost.OldAccountCode = Number(this.ledgerSplit[1].trim());
        }
        else {
          this.ledgerPost.AccountCode = 0;
          this.ledgerPost.OldAccountCode = 0;
        }
        break;
      }
    }
  }

  chngLedger() {
    this.ledgerPost.AccountCode = Number(this.ledgerPost.AccountCode);
  }

  cancelModal() {
    $('#LedgerPopup').modal('hide');
  }

  saveLedger() {
    if (this.ledgerPost.AccountCode == 0) {
      swal("Warning!", 'Please select the ledger', "warning");
    }
    else {
      this._accountsService.postTransactionPosting(this.ledgerPost).subscribe(
        response => {
          swal("Submitted!", "Ledger updated successfully!", "success");
          $('#LedgerPopup').modal('hide');
          this.LoadAllLedgers();
        },
        (err) => {
          const validationError = err.error;
          swal("Warning!", validationError.description, "warning");
        }
      )
    }
  }
}