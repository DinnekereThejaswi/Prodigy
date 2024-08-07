import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { BranchissueService } from '../branchissue.service';
import { FormGroup, FormBuilder, Validators, FormGroupName } from '@angular/forms';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';
import { formatDate } from '@angular/common';
declare var $: any;

@Component({
  selector: 'app-issue-cancel',
  templateUrl: './issue-cancel.component.html',
  styleUrls: ['./issue-cancel.component.css']
})
export class IssueCancelComponent implements OnInit {

  cancelIssueForm: FormGroup;
  ReprintForm: FormGroup;
  @ViewChild("Pwd", { static: true }) Pwd: ElementRef;
  PasswordValid: string;
  public Index: number = -1;
  ccode: string;
  bcode: string;
  password: string;
  EnableDate: boolean = true;
  datePickerConfig: Partial<BsDatepickerConfig>;
  EnableItemDetails: boolean = true;
  EnableFittedStoneDetails: boolean = true;
  EnableStoneDmddtls: boolean = true;
  today = new Date();
  applicationDate: any;
  disAppDate: any;
  IssueCancelHeaderForm: FormGroup;

  EnableJson: boolean = false;
  isReprint: boolean = false;
  HeaderFormModel = {
    BinBo: null,
    issueTo: null,
    issueDate: null,
    AgeValidation: false

  }
  IRListData: any = [{ code: "OPG", name: "OPG Issue" }, { code: "SR", name: "SR Issue" }, { code: "RP", name: "RP Issue" }]
  //radio items
  CancelIssueHeaders = {
    companyCode: null,
    branchCode: null,
    selectedOption: null,
    IssueDate: null,
    IssueNo: null,
    Remarks: null
  }
  constructor(private _branchissueService: BranchissueService, private fb: FormBuilder, private _appConfigService: AppConfigService) {
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        maxDate: this.today,
        dateInputFormat: 'YYYY-MM-DD'
      });
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.PasswordValid = this._appConfigService.RateEditCode.ReceiptCancelPermission;
    this.getCB();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this.CancelIssueHeaders.companyCode = this.ccode;
    this.CancelIssueHeaders.branchCode = this.bcode;
    this.getApplicationDate();
    this.cancelIssueForm = this.fb.group({
      FrmCtrl_selectedOption: [null],
      frmCtrl_applicationDate: null,
      frmCtrl_IssueNo: null,
      frmCtrl_Remarks: null,
      frmCtrl_selectedOption: null
    });
  }

  ValidatePermisiion() {
    $("#PermissonModals").modal('show');
    this.Pwd.nativeElement.value = "";
  }
  permissonModels: any = {
    CompanyCode: null,
    BranchCode: null,
    PermissionID: null,
    PermissionData: null
  }

  passWord(arg) {
    if (arg == "") {
      swal("Warning!", 'Please Enter the Password', "warning");
      $('#PermissonModals').modal('show');
    }
    else {
      this.permissonModels.CompanyCode = this.ccode;
      this.permissonModels.BranchCode = this.bcode;
      this.permissonModels.PermissionID = this.PasswordValid;
      this.permissonModels.PermissionData = btoa(arg);

      this._branchissueService.postelevatedpermission(this.permissonModels).subscribe(
        response => {
          this.EnableDate = false;
          $('#PermissonModals').modal('hide');
          this.Index = -1;
        },
        (err) => {
          if (err.status === 401) {
            this.EnableDate = true;
          }
        }
      )
    }
  }


  byDate(applicationDate) {
    this.CancelIssueHeaders.IssueDate = null,
      this.CancelIssueHeaders.selectedOption = null;
    this.CancelIssueHeaders.IssueDate = formatDate(applicationDate, 'yyyy-MM-dd', 'en-US');;
    this.getOPGIsuueNo(this.CancelIssueHeaders.IssueDate);
  }
  getApplicationDate() {
    this._branchissueService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
        if (this.CancelIssueHeaders.selectedOption === 'OPG') {
          this.CancelIssueHeaders.IssueDate = formatDate(this.applicationDate, 'yyyy-MM-dd', 'en-US');;
          this.getOPGIsuueNo(this.CancelIssueHeaders.IssueDate);
        }
      }
    )
  }
  Changed(arg) {
    this.CancelIssueHeaders.selectedOption = arg.target.value;
    if (this.CancelIssueHeaders.selectedOption === 'OPG') {
      this.CancelIssueHeaders.IssueDate = formatDate(this.applicationDate, 'yyyy-MM-dd', 'en-US');;
      this.getOPGIsuueNo(this.CancelIssueHeaders.IssueDate);

    }
    else if (this.CancelIssueHeaders.selectedOption === 'SR') {
      this.IssueNumbers = [];
      this.getSRIsuueNo(this.CancelIssueHeaders.IssueDate);
    }
    else if (this.CancelIssueHeaders.selectedOption === 'RP') {
      this.IssueNumbers = [];


    }
  }
  IssueNumbers: any = [];
  getOPGIsuueNo(date) {
    this.IssueNumbers = [];
    this._branchissueService.getOPGIssueNumbers(date).subscribe(
      response => {
        this.IssueNumbers = response;
      }
    )
  }
  getSRIsuueNo(date) {
    this.IssueNumbers = [];
    this._branchissueService.getSRissueNumbers(date).subscribe(
      response => {
        this.IssueNumbers = response;
      }
    )
  }

  ResponseData: any = [];

  postCancelIssue(form) {
    if (form.value.frmCtrl_IssueNo == null || form.value.frmCtrl_IssueNo == "") {
      swal("!Warning", "Please select Issue No", "warning");
    }
    else if (form.value.frmCtrl_Remarks == null || form.value.frmCtrl_Remarks == "") {
      swal("!Warning", "Please enter remarks", "warning");
    }
    else {
      if (this.CancelIssueHeaders.selectedOption === "OPG") {
        var ans = confirm("Do you want to Cancel OPG Issue No??" + this.CancelIssueHeaders.IssueNo);
        if (ans) {
          this._branchissueService.cancelOPGIssue(this.CancelIssueHeaders.IssueNo, this.CancelIssueHeaders.Remarks).subscribe(
            response => {
              this.ResponseData = response;
              swal("Success", "OPG Issue No" + this.CancelIssueHeaders.IssueNo + "Cancelled", "success");
              this.cancelIssueForm.reset();
              this.CancelIssueHeaders = {
                companyCode: null,
                branchCode: null,
                selectedOption: null,
                IssueDate: null,
                IssueNo: null,
                Remarks: null
              }
              this.getApplicationDate();
            }

          )
        }
      }
      else if (this.CancelIssueHeaders.selectedOption === "SR") {
        var ans = confirm("Do you want to Cancel SR Issue No??" + this.CancelIssueHeaders.IssueNo);
        if (ans) {
          this._branchissueService.cancelSRIssue(this.CancelIssueHeaders.IssueNo, this.CancelIssueHeaders.Remarks).subscribe(
            response => {
              this.ResponseData = response;
              swal("Success!", "SR Issue" + this.CancelIssueHeaders.IssueNo + "Cancelled", "success");
              this.cancelIssueForm.reset();
              this.CancelIssueHeaders = {
                companyCode: null,
                branchCode: null,
                selectedOption: null,
                IssueDate: null,
                IssueNo: null,
                Remarks: null
              }
              this.getApplicationDate();
            }

          )
        }
      }
    }
  }
  clear() {
    this.cancelIssueForm.reset();
    this.getApplicationDate();

  }
}
