import { FormGroup, FormBuilder } from '@angular/forms';
import { CbwidgetService } from './cbwidget.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as CryptoJS from 'crypto-js';
import { SigninService } from '../authentication/login/signin.service';
import { AppConfigService } from '../AppConfigService';

declare var $: any;
@Component({
  selector: 'app-cbwidget',
  templateUrl: './cbwidget.component.html',
  styleUrls: ['./cbwidget.component.css']
})
export class CbwidgetComponent implements OnInit {
  cbWidgetForm: FormGroup;
  defaultBranch: any;
  ccode: string = "";
  bcode: string = "";
  isDisabled: boolean = false;
  ValueChange: boolean = false;
  textToConvert: string;
  password: string;

  cbWidgetHeader: any = {
    ccode: null,
    bcode: null
  }

  constructor(private _cbwidgetService: CbwidgetService, private fb: FormBuilder,
    public _router: Router, private service: SigninService,
    private _appConfigService: AppConfigService) {
    this.password = this._appConfigService.Pwd;
  }

  ngOnInit() {
    $('#addBarcode').modal('show');
    this.getAllCompanybranchCode();
    this.ValueChange = false;
    this.cbWidgetForm = this.fb.group({
      ccode: null,
      bcode: null
    });
  }

  cbWidgetData: any = [];
  getAllCompanybranchCode() {
    let arg = localStorage.getItem('OperatorCode');
    this._cbwidgetService.getAllCompanybranchCode(arg).subscribe(
      response => {
        this.cbWidgetData = response;
        if (this.cbWidgetData.length === 1) {
          this.isDisabled = true;
          this.getDefaultBranch();
        }
        else {
          this.getDefaultBranch();
        }
      }
    )
  }

  encryptMode(arg) {
    this.ccode = CryptoJS.AES.encrypt(arg.value.ccode, this.password.trim()).toString();
    this.bcode = CryptoJS.AES.encrypt(arg.value.bcode, this.password.trim()).toString();
    localStorage.setItem('ccode', this.ccode);
    localStorage.setItem('bcode', this.bcode);
    this.service.SendBranchCode(this.bcode);

  }

  ChngCompanyCode() {
    this.ValueChange = true;
  }

  ChngBranchCode() {
    this.ValueChange = true;
  }

  storedata(arg) {
    localStorage.removeItem('ccode');
    localStorage.removeItem('bcode');
    if (this.ValueChange == true) {
      var ans = confirm("Do you want to proceed with " + arg.value.ccode + ' Company and ' + arg.value.bcode + " Branch??");
      if (ans) {
        setTimeout(() => {
          this.encryptMode(arg);
          this._router.navigate(['/dashboard']);
          $('#addBarcode').modal('hide');
        }, 300);
      }
      else {
        return false;
      }
    }
    else {
      setTimeout(() => {
        this.encryptMode(arg);
        this._router.navigate(['/dashboard']);
        $('#addBarcode').modal('hide');
      }, 300);
    }
  }




  // ChangeBranch(arg) {
  //   var ans = confirm("Do you want to change Branch");
  //   if (ans) {
  //     localStorage.setItem('bcode', arg);
  //   }
  //   else {
  //     this.cbWidgetHeader.bcode = this.defaultBranch.BranchCode;
  //   }
  // }

  getDefaultBranch() {
    let arg = localStorage.getItem('OperatorCode');
    this._cbwidgetService.getBranchCompany(arg).subscribe(
      response => {
        this.defaultBranch = response;
        this.cbWidgetHeader.ccode = this.defaultBranch.CompanyCode;
        this.cbWidgetHeader.bcode = this.defaultBranch.BranchCode;
      }
    )
  }
}
