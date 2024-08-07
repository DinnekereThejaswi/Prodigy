import { Component, OnInit } from '@angular/core';
import { AccountsService } from '../accounts.service';
import { Cheque } from '../accounts.model';
import swal from 'sweetalert';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';

@Component({
  selector: 'app-cheque-entry',
  templateUrl: './cheque-entry.component.html',
  styleUrls: ['./cheque-entry.component.css']
})
export class ChequeEntryComponent implements OnInit {

  ChequeForm: FormGroup;
  EnableJson: boolean = false;
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;

  ChequeModel: Cheque = {
    ObjID: "",
    CompanyCode: "",
    BranchCode: "",
    AccCode: null,
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


  constructor(private AccountsService: AccountsService, private formBuilder: FormBuilder,
    private appConfigService: AppConfigService
  ) {
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.EnableJson = this.appConfigService.EnableJson;
    this.password = this.appConfigService.Pwd;
    this.getCB();
  }

  ngOnInit() {
    this.getChequeData();
    this.getMasterLedgerList();
    this.getApplicationdate();
    this.ChequeForm = this.formBuilder.group({
      AccCode: [null, Validators.required],
      NoOfCheques: [0, Validators.required],
      ChqStart: [0, Validators.required],
      ChqEnd: [0, Validators.required],
      MaxAmount: [0, Validators.required]
    });
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ChequeList: any = [];

  getChequeData() {
    this.AccountsService.getChequelist().subscribe(response => {
      this.ChequeList = response;
    })
  }

  DeleteRow(arg) {
    var ans = confirm('Do you want to delete');
    if (ans) {
      this.ChequeModel.CompanyCode = this.ccode;
      this.ChequeModel.BranchCode = this.bcode;
      this.ChequeModel.AccCode = arg.AccCode;
      this.ChequeModel.NoOfChqs = arg.Nos;
      this.ChequeModel.ChqStartNo = arg.StartNo;
      this.ChequeModel.ChqEndNo = arg.EndNo;
      this.ChequeModel.MaxAmount = arg.MaxAmt;
      this.ChequeModel.ChqIssueDate = this.voucherDate;
      this.AccountsService.DeleteCheque(this.ChequeModel).subscribe(
        response => {
          swal("Deleted!", "Cheque details deleted successfully.", "success");
          this.getChequeData();
          this.clear();
        }
      )
    }
  }

  add(form) {
    if (form.value.AccCode == null || form.value.AccCode == 0) {
      swal("!Warning", "Please select the bank.", "warning");
    }
    else if (form.value.NoOfCheques == null || form.value.NoOfCheques == 0) {
      swal("!Warning", "Please enter the valid no of cheques.", "warning");
    }
    else if (form.value.ChqStart == null || form.value.ChqStart == 0) {
      swal("!Warning", "Please enter the valid Cheque Start No.", "warning");
    }
    else {
      this.ChequeModel.CompanyCode = this.ccode;
      this.ChequeModel.BranchCode = this.bcode;
      this.ChequeModel.ChqIssueDate = this.voucherDate;
      var ans = confirm("Do you want to save??");
      if (ans) {
        this.AccountsService.postCheque(this.ChequeModel).subscribe(
          response => {
            swal("Saved!", "Saved successfully", "success");
            this.getChequeData();
            this.clear();
          }
        )
      }
    }
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

  clear() {
    this.ChequeModel = {
      ObjID: "",
      CompanyCode: "",
      BranchCode: "",
      AccCode: null,
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

  MasterLedgerList: any = [];
  getMasterLedgerList() {
    this.AccountsService.getMasterLedgerList().subscribe(
      Response => {
        this.MasterLedgerList = Response;
      }
    )
  }

  calcChqEndNo() {
    this.ChequeModel.ChqEndNo = Number(this.ChequeModel.ChqStartNo - 1) + Number(this.ChequeModel.NoOfChqs);
  }
}