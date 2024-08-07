import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from './../AppConfigService';

@Injectable({
  providedIn: 'root'
})
export class LastdocnoService {

  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;

  constructor(private _http: HttpClient, private appConfigService: AppConfigService) {
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.password = this.appConfigService.Pwd;
    this.getCB();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  getLastDocNum(docType) {
    return this._http.get(this.apiBaseUrl + 'api/docmgmt/last-series?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&docType=' + docType);
  }
}
