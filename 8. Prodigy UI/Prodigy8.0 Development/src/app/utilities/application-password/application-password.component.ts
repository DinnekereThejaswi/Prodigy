import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl, AbstractControl } from '@angular/forms';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { Alert } from 'bootstrap';
import { UtilitiesService } from '../utilities.service'
@Component({
  selector: 'app-application-password',
  templateUrl: './application-password.component.html',
  styleUrls: ['./application-password.component.css']
})
export class ApplicationPasswordComponent implements OnInit {

  ApplicationPasswordform: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  PasswordPostModel = {
    CompanyCode: "",
    BranchCode: "",
    PasswordNo: 0,
    Password: ""
  }
  confirmPassword: any;
  constructor(private fb: FormBuilder, private _utilitiesService: UtilitiesService, private _appConfigService: AppConfigService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();

  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this.PasswordPostModel.CompanyCode = this.ccode;
    this.PasswordPostModel.BranchCode = this.bcode;
    this.getPasswordList();
    this.ApplicationPasswordform = this.fb.group({
      frmCtrl_PasswordNo: 0,
      frmCtrl_Password: "",
      frmCtrl_ConfirmPassword: null
    });

  }
  PasswordList: any = [];
  getPasswordList() {
    this._utilitiesService.getPasswordList().subscribe(
      Response => {
        this.PasswordList = Response;
      })
  }
  result: any = [];
  clear() {
    this.PasswordPostModel.Password = null;
    this.ApplicationPasswordform.reset();
    this.PasswordPostModel.Password = "";
    this.getPasswordList();
  }
  add(form) {
    if (form.value.frmCtrl_PasswordNo == "" || form.value.frmCtrl_PasswordNo == 0) {
      swal("Warning!", "Please Enter Password Number", "warning");
    }
    else if (form.value.frmCtrl_Password == "" || form.value.frmCtrl_Password == null) {
      swal("Warning!", "Please Enter Password", "warning");
    }
    else if (form.value.frmCtrl_ConfirmPassword == "" || form.value.frmCtrl_ConfirmPassword == null) {

      swal("Warning!", "Please Confirm Password", "warning");
    }
    else if (this.confirmPassword !== this.PasswordPostModel.Password) {
      swal("Warning!", "confirm passowrd is incorrect", "warning");
      this.confirmPassword = "";
    }
    else {
      var ans = confirm("Do you want to Save ??" + form.value.frmCtrl_PasswordNo);
      if (ans) {
        this.result = [];
        this.PasswordPostModel.Password = btoa(this.confirmPassword);
        this._utilitiesService.AddNewPassword(this.PasswordPostModel).subscribe(
          response => {
            this.result = response;
            swal("Success!", form.value.frmCtrl_PasswordNo, "success");
            this.getPasswordList();
            this.clear();
          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error;
              swal("Success!", validationError, "warning");
              this.confirmPassword = "";
            }
            else {
              this.result.push('something went wrong!');
            }
          }
        )
      }
    }
  }
  deletePassword(arg) {
    this.result = [];
    var ans = confirm("Do you want to Delete?" + arg.PasswordNo);
    if (ans) {
      this._utilitiesService.DeletePassword(arg.PasswordNo).subscribe(
        response => {
          this.result = response;
          swal("Updated!", "success" + arg.PasswordNo + "success", "success");
          this.getPasswordList();
          this.clear();
        }
      )
    }
  }


}
