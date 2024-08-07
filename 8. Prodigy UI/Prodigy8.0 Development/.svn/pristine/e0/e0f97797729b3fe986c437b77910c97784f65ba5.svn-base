import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { GstPostingSetupService } from './gst-posting-setup.service';
import { GstPostingSetup } from '../masters.model';
import { MastersService } from '../masters.service';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { formatDate } from '@angular/common';

@Component({
  selector: 'app-gst-posting-setup',
  templateUrl: './gst-posting-setup.component.html',
  styleUrls: ['./gst-posting-setup.component.scss']
})
export class GstPostingSetupComponent implements OnInit {
  GSTPostingSetupForm: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  isReadOnly: boolean = false;
  EnableJson: boolean = false;
  today = new Date();
  applicationDate: any;

  datePickerConfig: Partial<BsDatepickerConfig>;
  ComponentCodes = [{ "code": "I", "name": "IGST" },
  { "code": "S", "name": "SGST" }, { "code": "C", "name": "CGST" }]
  CalculationOrder = [{ "code": "1", "name": "One" },
  { "code": "2", "name": "Two" }, { "code": "3", "name": "Three" }]
  constructor(private _GstPostingSetupService: GstPostingSetupService,
    private fb: FormBuilder, private _appConfigService: AppConfigService,
    private _mastersService: MastersService) {
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        // maxDate: this.today,
        minDate: this.today,
        dateInputFormat: 'YYYY-MM-DD',
      });
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }
  GSTPostingSetupListData: GstPostingSetup = {
    ID: 0,
    GSTGroupCode: null,
    GSTComponentCode: null,
    EffectiveDate: null,
    GSTPercent: null,
    CalculationOrder: null,
    ReceivableAccount: null,
    ReceivableAccountName: "",
    PayableAccount: null,
    PaybaleAccountName: "",
    ExpenseAccount: null,
    RefundAccount: null,
    LastModifiedOn: "",
    LastModifiedBy: "",
    IsRegistered: true,
    CompanyCode: "",
    BranchCode: ""
  };

  ngOnInit() {
    this.applicationDate;
    this.GSTPostingSetupListData.CompanyCode = this.ccode;
    this.GSTPostingSetupListData.BranchCode = this.bcode;
    this.ComponentCodes;
    this.CalculationOrder;
    this.getApplicationDate();
    this.getGstPostingSetupList();
    this.getGroupCode();
    this.getInputAndOutputAc();
    this.getAndOutputAc();
    this.GSTPostingSetupForm = this.fb.group({
      frmCtrl_GSTGroupCode: [null, Validators.required],
      frmCtrl_GSTComponentCode: [null, Validators.required],
      frmCtrl_EffectiveDate: [null, Validators.required],
      frmCtrl_GSTPercent: [null, Validators.required],
      frmCtrl_CalculationOrder: [null, Validators.required],
      frmCtrl_ReceivableAccount: [null, Validators.required],
      frmCtrl_PayableAccount: [null, Validators.required],
      frmCtrl_IsRegistered: [Boolean, Validators.required]
    });
  }

  getApplicationDate() {
    this._mastersService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
      }
    )
  }
  byDate(applicationDate) {
    this.GSTPostingSetupListData.EffectiveDate = formatDate(applicationDate, 'yyyy-MM-dd', 'en-US');

  }
  SelectedInputDetails: any;
  LoadInputDetails(i) {
    var SelectedIndex = i - 1;
    this.getInputDetails(SelectedIndex);

  }
  getInputDetails(SelectedIndex) {
    this.SelectedInputDetails = this.GstPostingSetupAcList[SelectedIndex];
    this.GSTPostingSetupListData.ReceivableAccount = this.SelectedInputDetails.AccCode;
    this.GSTPostingSetupListData.ReceivableAccountName = this.SelectedInputDetails.AccName;
  }


  SelectedOutputDetails: any;
  LoadOutputDetails(i) {
    var SelectedIndex = i - 1;
    this.getOutputDetails(SelectedIndex);
  }
  getOutputDetails(SelectedIndex) {
    this.SelectedOutputDetails = this.GstPostingOutputAcAcList[SelectedIndex];
    this.GSTPostingSetupListData.PayableAccount = this.SelectedInputDetails.AccCode;
    this.GSTPostingSetupListData.PaybaleAccountName = this.SelectedOutputDetails.AccName;
  }
  GstPostingSetupList: any = [];
  getGstPostingSetupList() {
    this._mastersService.getGSTPostingSetup().subscribe(
      Response => {
        this.GstPostingSetupList = Response;
      }
    )
  }
  GstPostingSetupAcList: any = [];
  getInputAndOutputAc() {
    this._mastersService.getAcTypes().subscribe(
      Response => {
        this.GstPostingSetupAcList = Response;
      }
    )
  }
  GstPostingOutputAcAcList: any = [];
  getAndOutputAc() {
    this._mastersService.getAcTypes().subscribe(
      Response => {
        this.GstPostingOutputAcAcList = Response;
      }
    )
  }
  deleteGSTPostingRecord(ID) {
    var ans = confirm('Do you want to delete');
    if (ans) {
      this._mastersService.deleteGSTpostingSetup(ID).subscribe(
        data => {
          this.getGstPostingSetupList();
        }
      );
    }
  }
  GSTList: any = []
  getGroupCode() {
    this._mastersService.getGstGroupCode().subscribe(
      Response => {
        this.GSTList = Response;
      }
    )
  }
  IsRegistered(e) {
    if (e.target.checked == true) {
      this.GSTPostingSetupListData.IsRegistered = true;
    }
    if (e.target.checked == false) {
      this.GSTPostingSetupListData.IsRegistered = false;
    }
  }
  clear() {
    this.GSTPostingSetupForm.reset();
    this.EnableAdd = true;
    this.EnableSave = false;
    this.getGstPostingSetupList();
  }
  errors: any = [];
  add(form) {
    if (form.value.frmCtrl_GSTPercent == null || form.value.frmCtrl_GSTPercent == "") {
      swal("Warning!", "Please enter GST %", "warning");
    }
    else {
      var ans = confirm("Do you want to save??");
      if (ans) {
        this.GSTPostingSetupListData.CompanyCode = this.ccode;
        this.GSTPostingSetupListData.BranchCode = this.bcode;
        this.GSTPostingSetupListData.CalculationOrder = Number(this.GSTPostingSetupListData.CalculationOrder);
        this.GSTPostingSetupListData.GSTPercent = Number(this.GSTPostingSetupListData.GSTPercent);
        this._mastersService.postGSTPosting(this.GSTPostingSetupListData).subscribe(
          response => {
            this.errors = response;
            swal("Saved!", "Saved " + this.GSTPostingSetupListData.GSTComponentCode + " Saved", "success");
            this.GSTPostingSetupForm.reset();
          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error.description;
              swal("Warning!", validationError, "warning");
              this.getGstPostingSetupList();
            }
            else {
              this.errors.push('something went wrong!');
            }
            // this.clear();
          }
        )
      }
    }
  }
  edit(arg, form) {
    this.EnableSave = true;
    this.EnableAdd = false;
    // arg.CalculationOrder=this.CalculationOrder."";
    this.EnableAdd = false;
    this.EnableSave = true;
    this.GSTPostingSetupListData = arg;
  }
  save() {
    var ans = confirm("Do you want to save??");
    if (ans) {
      this._mastersService.putGstPosting(this.GSTPostingSetupListData.ID, this.GSTPostingSetupListData).subscribe(
        response => {

          swal("Updated!", "Saved GST % " + this.GSTPostingSetupListData.GSTPercent + "Saved", "success");
          this.clear();
        }
      )
    }
  }
  deletingRecord(arg) {
    this.GSTPostingSetupListData = arg;
    var ans = confirm("Do you want to Delete??");
    if (ans) {
      this._mastersService.deleteGstPost(this.GSTPostingSetupListData).subscribe(
        response => {
          swal("Updated!", "Deleted " + this.GSTPostingSetupListData.GSTPercent + " Deleted", "success");
          this.clear();
        }
      )
    }
  }
}




