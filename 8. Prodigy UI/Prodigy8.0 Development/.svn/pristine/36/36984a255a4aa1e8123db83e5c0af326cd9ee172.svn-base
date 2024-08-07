import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MastersService } from '../masters.service';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';


@Component({
  selector: 'app-product-attributes',
  templateUrl: './product-attributes.component.html',
  styleUrls: ['./product-attributes.component.css']
})
export class ProductAttributesComponent implements OnInit {
  ProdctAtribform: FormGroup;
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;
  EnableJson: boolean = true;
  isReadOnly: boolean = false;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;

  stockKeepUnitModel: {
    gscode: null,
    itemName: null,
    designName: null,
    weight: null,
    skuid: null,
    ObjStatus: null
  }
  constructor(private fb: FormBuilder, private _mastersService: MastersService,
    private _appConfigService: AppConfigService,
    private _router: Router) {
    this.apiBaseUrl = this._appConfigService.apiBaseUrl;
    this.password = this._appConfigService.Pwd;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    // this.ReceivedOrderJson.CompanyCode = this.ccode;
    // this.ReceivedOrderJson.BranchCode = this.bcode;

  }


  ngOnInit() {

    this.ProdctAtribform = this.fb.group({
      frmCtrl_atrbCode: [null, Validators.required],
      frmCtrl_atrbName: [null, Validators.required],

    });
  }

}
