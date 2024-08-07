import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
@Injectable({
  providedIn: 'root'
})
export class StockTakingService {

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

  StaffList() {
    return this._http.get(this.apiBaseUrl + 'api/Stock/StockTaking/Salesman?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }

  GetCounter() {
    return this._http.get(this.apiBaseUrl + 'api/Stock/StockTaking/Counter?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }

  getItem(counterCode) {
    return this._http.get(this.apiBaseUrl + 'api/Masters/ItemByCounter/' + this.ccode + '/' + this.bcode + '/' + counterCode);
  }

  getBatchInfo(batchNo) {
    return this._http.get(this.apiBaseUrl + 'api/Stock/StockTaking/Detail?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&batchNo=' + batchNo);
  }

  deleteBarcode(batchNo, barcodeNo) {
    return this._http.delete(this.apiBaseUrl + 'api/Stock/StockTaking/DeleteBarcode/' + this.ccode + '/' + this.bcode + '/' + batchNo + '/' + barcodeNo);
  }

  getSummary(batchNo) {
    return this._http.get(this.apiBaseUrl + 'api/Stock/StockTaking/BatchSummary?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&batchNo=' + batchNo);
  }

  getStoneDetails(batchNo) {
    return this._http.get(this.apiBaseUrl + 'api/Stock/StockTaking/StoneDetail/' + this.ccode + '/' + this.bcode + '/' + batchNo);
  }

  getStoneTotal(batchNo) {
    return this._http.get(this.apiBaseUrl + 'api/Stock/StockTaking/StoneDetailTotal/' + this.ccode + '/' + this.bcode + '/' + batchNo);
  }

  StockTakingPost(arg) {
    let body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Stock/StockTaking/Post', body);
  }

}