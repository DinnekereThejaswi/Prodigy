import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MastersService } from '../masters.service';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';
@Component({
  selector: 'app-scheme-collection',
  templateUrl: './scheme-collection.component.html',
  styleUrls: ['./scheme-collection.component.css']
})
export class SchemeCollectionComponent implements OnInit {
  SchemeCollectionform: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  isReadOnly: boolean = false;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  EnableJson: boolean = false;
  SchemeCollectionListData = {
    CompanyCode: "",
    BranchCode: "",
    Code: null,
    Description: "",
    AccCode: null,
    AccName: ""
  }
  Paymode = [{ "Code": "O", "Description": "Online Payment" },
  { "Code": "B", "Description": "E- Payment" },
  { "Code": "N", "Description": "UPI" }];
  constructor(private _masterService: MastersService,
    private formBuilder: FormBuilder,
    private _appConfigService: AppConfigService,
    private fb: FormBuilder) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8
    );
  }
  ngOnInit() {
    this.Paymode;
    this.getLedgers();
    this.SchemeCollectionListData.CompanyCode = this.ccode;
    this.SchemeCollectionListData.BranchCode = this.bcode;
    this.SchemeCollectionform = this.fb.group({
      Paymode: null,
      LedgerName: null
    });
  }
  Ledgers: any = [];
  getLedgers() {
    this._masterService.getLedgerTypes().subscribe(
      Response => {
        this.Ledgers = Response;

      }
    )
  }
}

