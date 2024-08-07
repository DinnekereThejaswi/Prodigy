import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';

@Injectable({
  providedIn: 'root'
})
export class CounterStockService {

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
    return this._http.get(this.apiBaseUrl + 'api/Stock/CounterStock/LoadGs/' + this.ccode + '/' + this.bcode);
  }
  GetCounter() {
    return this._http.get(this.apiBaseUrl + 'api/Stock/CounterStock/counter/' + this.ccode + '/' + this.bcode);
  }
  // getItem(companyCode, branchCode, counterCode){
  //   return this._http.get(this.apiBaseUrl + 'api/Stock/StockTaking/Item?companyCode=' + companyCode + '&branchCode=' +branchCode + '&counterCode=' + counterCode);
  // }
  getGSWiseSummary(gsCode, counterCode, date) {
    return this._http.get(this.apiBaseUrl + 'api/Stock/CounterStock/CounterStockSummaryReport?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsCode=' + gsCode + '&asOnDate=' + date);  
}
  getCounterStockDetails(gsCode, counterCode, date) {
  
    return this._http.get(this.apiBaseUrl + 'api/Stock/CounterStock/GetStockInfo/' + this.ccode + '/' + this.bcode + '/' + gsCode + '/' + counterCode + '/' + date);
  }

  getCounterStockDetailsReport(gsCode, counterCode, date) {
    return this._http.get(this.apiBaseUrl + 'api/Stock/CounterStock/CounterStockDetailReport?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsCode=' + gsCode + '&counterCode=' + counterCode + '&asOnDate=' + date);
  }

  getCounterSummaryReport(gsCode, counterCode, date) {
    return this._http.get(this.apiBaseUrl + 'api/Stock/CounterStock/CounterSummaryReport?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsCode=' + gsCode + '&counterCode=' + counterCode + '&asOnDate=' + date);
  }

  getClosingStockReport(gsCode, counterCode, date) {
    return this._http.get(this.apiBaseUrl + 'api/Stock/CounterStock/ClosingStockReport?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsCode=' + gsCode + '&counterCode=' + counterCode + '&asOnDate=' + date);
  }


  getSumDetails(gsCode, counterCode, date) {
    return this._http.get(this.apiBaseUrl + 'api/Stock/CounterStock/GetStockSumInfo/' + this.ccode + '/' + this.bcode + '/' + gsCode + '/' + counterCode + '/' + date);
  }
  getItemwiseSummary(gsCode, counterCode, date) {
    return this._http.get(this.apiBaseUrl + 'api/Stock/CounterStock/ItemWiseSummary/' + this.ccode + '/' + this.bcode + '/' + gsCode + '/' + counterCode + '/' + date);

  }
  putExcess(arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Stock/CounterStock/PositiveAdjustment', body);
  }
  putShort(arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Stock/CounterStock/NegativeAdjustment', body);
  }
  getApplicationDate() {
    return this._http.get(this.apiBaseUrl + 'api/masters/GetApplicationDate/' + this.ccode + '/' + this.bcode);
  }
}
