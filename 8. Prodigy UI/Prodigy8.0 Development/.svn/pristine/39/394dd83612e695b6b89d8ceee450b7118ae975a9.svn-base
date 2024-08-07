import { Component, OnInit } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { formatDate } from '@angular/common';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MasterService } from './../../core/common/master.service';
import { AccountsService } from '../accounts.service';
import { CashInHand } from '../accounts.model';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { Router } from '@angular/router';
import { AppConfigService } from '../../AppConfigService';

@Component({
  selector: 'app-cash-in-hand',
  templateUrl: './cash-in-hand.component.html',
  styleUrls: ['./cash-in-hand.component.css']
})
export class CashInHandComponent implements OnInit {

  datePickerConfig: Partial<BsDatepickerConfig>;
  CashInHandForm: FormGroup;
  today = new Date();
  ordDate = '';
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;
  EnableJson: boolean = false
  CashInHandModel: CashInHand = {
    ObjID: "",
    BranchCode: "",
    CompanyCode: "",
    SlNo: 0,
    CashBalance: 0,
    CashInHand: 0,
    BillDate: 0,
    FinYear: 0
  }

  constructor(private _masterService: MasterService, private AccountsService: AccountsService, private formBuilder: FormBuilder,
    private Router: Router, private appConfigService: AppConfigService) {
    this.ordDate = formatDate(this.today, 'MM-DD-YYYY', 'en-US', '+0530');
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.EnableJson = this.appConfigService.EnableJson;
    this.password = this.appConfigService.Pwd;
    this.getCB();
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        maxDate: new Date(2020, 7, 28),
        dateInputFormat: 'YYYY-MM-DD'
      });
  }

  ngOnInit() {
    this.getApplicationDate();
    this.CashInHandForm = this.formBuilder.group({
      CashBalance: ["", Validators.required],
      CashInHand: [0, Validators.required]
    });
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  applicationDate: any;
  appDate: any;
  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        this.appDate = response;
        this.applicationDate = formatDate(this.appDate["applcationDate"], 'dd/MM/yyyy', 'en_GB');
        this.getCashDetails(this.appDate["applcationDate"]);
      }
    )
  }

  CashDetails: any;
  getCashDetails(arg) {
    this.AccountsService.getCashInHand(arg).subscribe(
      response => {
        this.CashDetails = response;
        this.CashInHandModel.CompanyCode = this.ccode;
        this.CashInHandModel.BranchCode = this.bcode;
        this.CashInHandModel.BillDate = this.appDate["applcationDate"];
        this.CashInHandModel.CashBalance = this.CashDetails[0].C2;
      }
    )
  }

  Save() {

    if (this.CashInHandModel.CashInHand == null || this.CashInHandModel.CashInHand == 0) {
      swal("!Warning", "Please enter cash in hand?", "warning");
    }
    // else if (this.CashInHandModel.CashInHand > this.CashInHandModel.CashBalance) {
    //   swal("!Warning", "Cash in hand should not exceed cash balance ?", "warning");
    // }
    else {
      var ans = confirm("Do you want to save??");
      if (ans) {
        this.AccountsService.postCashInHand(this.CashInHandModel).subscribe(
          response => {
            swal("Saved!", "Cash In Hand saved successfully.", "success");
            this.Router.navigateByUrl('/redirect', { skipLocationChange: true }).then(() =>
              this.Router.navigate(['/accounts/cash-in-hand']))
          }
        )
      }
    }
  }

  exit() {
    this.Router.navigate(['/dashboard']);
  }

  cashDifference: string;
  CalDifference() {
    this.cashDifference = Number(this.CashInHandModel.CashBalance - this.CashInHandModel.CashInHand).toFixed(2);
  }
}