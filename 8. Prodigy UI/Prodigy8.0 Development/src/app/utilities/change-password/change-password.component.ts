import { Component, OnInit, Input, ElementRef, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl, AbstractControl } from '@angular/forms';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { Alert } from 'bootstrap';
import { UtilitiesService } from '../utilities.service'
import { Router } from '@angular/router';
declare var $: any;

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent implements OnInit {
  @ViewChild('ConfirmPassword', { static: true }) ConfirmPassword: ElementRef; OperatorSearchform: FormGroup;
  changePasswordform: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  confirmPassword: any;
  LoggedIn: string;
  constructor(private _router: Router, private fb: FormBuilder, private _utilitiesService: UtilitiesService, private _appConfigService: AppConfigService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();

  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }
  ngOnInit() {
    this.LoggedIn = localStorage.getItem("UserID");
    this.ChangePasswordModel.CompanyCode = this.ccode;
    this.ChangePasswordModel.BranchCode = this.bcode;
    $('#ChangePasswordModel').modal('show');
    this.changePasswordform = this.fb.group({
      companyCode: this.ccode,
      branchCode: this.bcode,
      FrmCtrl_OperatorCode: null,
      FrmCtrl_OldPassword: null,
      FrmCtrl_NewPassword: null,
      FormCtrl_NewPassword: null,
      FormCtr_ConfirmPswd: null
    });
  }
  ChangePasswordModel = {
    BranchCode: "",
    CompanyCode: "",
    OldPassword: "",
    NewPassword: ""
  }
  result: any = [];
  ChangePasswrodModel(arg) {
    if (arg.Status === "Closed") {
      swal("Warning!", "Operator :" + arg.OperatorName + "  Status Is closed so Password Can't Changed", "warning")
    }
    else {
      this.ChangePasswordModel.CompanyCode = this.ccode;
      this.ChangePasswordModel.BranchCode = this.bcode;
      $('#ChangePasswordModel').modal('show');
    }
  }
  ChangePassword(form) {
    if (form.value.FrmCtrl_OldPassword == null || form.value.FrmCtrl_OldPassword == "") {
      swal("Warning!", "Please enter Old Password", "warning");
    }
    else if (form.value.FrmCtrl_NewPassword == null || form.value.FrmCtrl_NewPassword == "") {
      swal("Warning!", "Please enter New Password", "warning");
    }
    else {
      var ans = confirm("Do you want to Change Password?");
      if (ans) {
        this.result = [];
        this.ChangePasswordModel.OldPassword = btoa(this.ChangePasswordModel.OldPassword);
        this.ChangePasswordModel.NewPassword = btoa(this.confirmPassword);
        this._utilitiesService.changeOprtrPassword(this.ChangePasswordModel).subscribe(
          response => {
            this.result = response;
            swal("Success!", this.result.Message, "success");
            this.ChangePasswordModel.OldPassword = "";
            this.ChangePasswordModel.NewPassword = "";
            $('#ChangePasswordModel').modal('hide');
            this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
              () => {
                this._router.navigate(['/dashboard']);
              }
            )
          }
        )
      }
    }
  }
  // ValidatePassword(arg) {
  //   if (this.confirmPassword != null) {
  //     if (arg !== this.ChangePasswordModel.NewPassword) {
  //       swal("Warning!", "Confirm Password is incorrect", "warning");
  //       this.confirmPassword = "";
  //       this.ConfirmPassword.nativeElement.focus();
  //     }
  //   }
  // }

  checkPassword(str) {
    var regix = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{8,})");
    if (regix.test(str) == false) {
    swal("Warning!","password must be a minimum of 8 characters including number, Upper, Lower And  one special character", "warning");
    this.confirmPassword = "";
    this.ChangePasswordModel.OldPassword="";
    this.ConfirmPassword.nativeElement.focus();
    }
   
  }
  ChangePasswordClear() {
    this.changePasswordform.reset();
  }
  close() {
    $('#ChangePasswordModel').modal('hide');
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
      () => {
        this._router.navigate(['/dashboard']);
      }
    )
  }
  // confirmPassword: any;
  ValidatePassword(arg) {
    if (this.confirmPassword != null) {
      if (arg !== this.ChangePasswordModel.NewPassword) {
        swal("Warning!", "Confirm Password is incorrect", "warning");
        this.confirmPassword = "";
        this.ConfirmPassword.nativeElement.focus();
      }
    }
  }

}
