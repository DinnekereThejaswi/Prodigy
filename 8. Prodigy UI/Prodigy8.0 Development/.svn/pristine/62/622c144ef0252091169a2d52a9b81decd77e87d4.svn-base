import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
@Injectable({
  providedIn: 'root'
})
export class StockCheckService {
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
  GSList() {
    return this._http.get(this.apiBaseUrl + 'api/order/OrderGSType/' + this.ccode + '/' + this.bcode);
  }
  GetCounter() {
    return this._http.get(this.apiBaseUrl + 'api/Stock/StockTaking/Counter?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }
  getItem(counterCode, gsCode) {
    return this._http.get(this.apiBaseUrl + 'api/Stock/StockCheck/Item/' + this.ccode + '/' + this.bcode + '/' + gsCode + '/' + counterCode);
  }
  getRemainingTags(GSCode, counterCode, Item) {
    return this._http.get(this.apiBaseUrl + 'api/Stock/StockCheck/RemainingTags/' + this.ccode + '/' + this.bcode + '/' + GSCode + '/' + counterCode + '/' + Item);
  }
  getScannedTags(GSCode, counterCode, Item) {
    return this._http.get(this.apiBaseUrl + 'api/Stock/StockCheck/ScannedTags/' + this.ccode + '/' + this.bcode + '/' + GSCode + '/' + counterCode + '/' + Item);
  }
  getTagSummary(GSCode, counterCode, Item) {
    return this._http.get(this.apiBaseUrl + 'api/Stock/StockCheck/TagSummary/' + this.ccode + '/' + this.bcode + '/' + GSCode + '/' + counterCode + '/' + Item);
  }
  clearStock(GSCode, counterCode, Item) {
    return this._http.delete(this.apiBaseUrl + 'api/Stock/StockCheck/ClearStockTaking/' + this.ccode + '/' + this.bcode + '/' + GSCode + '/' + counterCode + '/' + Item);
  }
}
