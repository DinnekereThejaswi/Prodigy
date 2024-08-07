import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../AppConfigService';

@Injectable({
  providedIn: 'root'
})
export class TagSplitService {

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

  getTagSplitDesignAPI() {
    return this._http.get(this.apiBaseUrl + 'api/TagSplit/Design/' + this.ccode + '/' + this.bcode);
  }

  getTagSplitSizeAPI() {
    return this._http.get(this.apiBaseUrl + 'api/TagSplit/Size/' + this.ccode + '/' + this.bcode);
  }

  getTagSplitStoneGSTypeAPI() {
    return this._http.get(this.apiBaseUrl + 'api/TagSplit/StoneGSType/' + this.ccode + '/' + this.bcode);
  }

  getTagSplitStoneNameAPI(stoneGSType, stoneType) {
    return this._http.get(this.apiBaseUrl + 'api/TagSplit/StoneName/' + this.ccode + '/' + this.bcode + '/' + stoneGSType + '/' + stoneType);
  }

  getTagSplitTagInfoAPI(tagNo) {
    return this._http.get(this.apiBaseUrl + 'api/TagSplit/TagInfo/' + this.ccode + '/' + this.bcode + '/' + tagNo);
  }

  getItemfromAPI(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Masters/GetItems?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsCode=' + arg);
  }

  postSplitBarcode(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/TagSplit/SplitBarcode/' + this.ccode + '/' + this.bcode, body);
  }

  postGenerateBarcode(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/TagSplit/GenerateBarcode', body);
  }
  GetSupplier() {
    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/SupplierNames/' + this.ccode + '/' + this.bcode);
  }
}