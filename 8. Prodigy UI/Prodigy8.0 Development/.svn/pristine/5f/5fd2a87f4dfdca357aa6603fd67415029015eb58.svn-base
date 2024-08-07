import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../AppConfigService';


@Injectable({
  providedIn: 'root'
})
export class StocksService {

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
  getShowroom() {
    return this._http.get(this.apiBaseUrl + 'api/Master/CompanyMaster/get/' + this.ccode + '/' + this.bcode);
  }
  getApplicationDate() {
    return this._http.get(this.apiBaseUrl + 'api/masters/GetApplicationDate/' + this.ccode + '/' + this.bcode);
  }

  getBatchNo() {
    return this._http.get(this.apiBaseUrl + 'api/Stock/StockTaking/BatchNumbers?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&sortBy=Batch');
  }

  getItemBasedonCounter(counterCode) {
    return this._http.get(this.apiBaseUrl + 'api/Stock/StockTaking/Item?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&counterCode=' + counterCode);
  }

  getBatchHeader(batchNo) {
    return this._http.get(this.apiBaseUrl + 'api/Stock/StockTaking/BatchHeader/' + this.ccode + '/' + this.bcode + '/' + batchNo);
  }

  getStockTakingDetail(batchNo) {
    return this._http.get(this.apiBaseUrl + 'api/Stock/StockTaking/DetailReport/' + this.ccode + '/' + this.bcode + '/' + batchNo);
  }

  getStockTakingDetailBySummary(batchNo) {
    return this._http.get(this.apiBaseUrl + 'api/Stock/StockTaking/BatchSummaryReport/' + this.ccode + '/' + this.bcode + '/' + batchNo);
  }

  getStockTakingStoneDetail(batchNo) {
    return this._http.get(this.apiBaseUrl + 'api/Stock/StockTaking/StoneDetail/' + this.ccode + '/' + this.bcode + '/' + batchNo);
  }

  getCounterList(counter) {
    return this._http.get(this.apiBaseUrl + 'api/Stock/StockCheck/Counter?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsCode=' + counter);
  }

}