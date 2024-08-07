import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MastersService } from './../masters.service';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { ToastrService } from 'ngx-toastr';
import { AppConfigService } from '../../AppConfigService';
import { formatDate } from '@angular/common';
declare var $: any;
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
@Component({
  selector: 'app-tcs-master',
  templateUrl: './tcs-master.component.html',
  styleUrls: ['./tcs-master.component.css']
})
export class TcsMasterComponent implements OnInit {
  TCSForm: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  isReadOnly: boolean = false;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  EnableJson: boolean = false;

  TCSListData = {
    ObjID: "",
    CompanyCode: "",
    BranchCode: "",
    IsWithKYC: null,
    AmountLimit: null,
    TCSPercent: null,
    AccCode: null,
    AccName: null,
    CalculatedOn: null,
    EffectiveDate: "",
    UpdateOn: "",
    TransactionType: null,
    IsTDS: ""
  }
  datePickerConfig: Partial<BsDatepickerConfig>;

  constructor(private fb: FormBuilder, private _masterservice: MastersService, private _router: Router,
    private _appConfigService: AppConfigService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        // maxDate: this.today,
        dateInputFormat: 'YYYY-MM-DD',

      });
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);

  }

  ngOnInit() {
    this.getAccnames();
    this.getkyc();
    this.getTransTypes();
    this.getcalculatedBy();
    this.getApplicationDate();
    this.TCSForm = this.fb.group({
      frmCtrl_TCSLimitAmt: null,
      frmCtrl_AccountName: null,
      frmCtrl_IsWithKYC: null,
      frmCtrl_TCSPercent: null,
      frmCtrl_Transtype: null,
      frmCtrl_Calculated: null,
      frmCtrl_Effective: null,
    });
  }
  AccDeatils: any = [];
  getAccnames() {
    this._masterservice.GetAccName().subscribe(
      Response => {
        this.AccDeatils = Response;

      }
    )
  }
  kyc: any = [];
  getkyc() {
    this._masterservice.getKYCTypes().subscribe(
      Response => {
        this.kyc = Response;

      }
    )
  }
  Transtypes: any = [];
  getTransTypes() {
    this._masterservice.getTranType().subscribe(
      Response => {
        this.Transtypes = Response;

      }
    )
  }
  calculatedBy: any = [];
  getcalculatedBy() {
    this._masterservice.calculatedOn().subscribe(
      Response => {
        this.calculatedBy = Response;

      }
    )
  }
  applicationDate: any;
  getApplicationDate() {
    this._masterservice.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
        this.applicationDate = formatDate(appDate["applcationDate"], 'dd/MM/yyyy', 'en_GB');
      }
    )
  }
  byDate(applicationDate) {
    this.applicationDate = formatDate(applicationDate, 'yyyy-MM-dd', 'en-US');;
    this.TCSListData.EffectiveDate = this.applicationDate;
  }


}
