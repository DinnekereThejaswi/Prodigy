import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../AppConfigService';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class NewSalesBillingService {

  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string = "";

  constructor(private _http: HttpClient, private appConfigService: AppConfigService) {
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.password = this.appConfigService.Pwd;
    this.getCB();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  getBillType() {
    return this._http.get(this.apiBaseUrl + 'api/SalesBilling/BillType/' + this.ccode + '/' + this.bcode);
  }

  PostDeriveEstimationBalances(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/SalesBilling/Derive-Estimation-Balances', body);
  }

  postSalesBilling(object) {
    var body = JSON.stringify(object)
    return this._http.post(this.apiBaseUrl + 'api/SalesBilling/Post', body);
  }

  getAttachedInfoBill(salesBillNo) {
    return this._http.get(this.apiBaseUrl + 'api/SalesBilling/AttachedInfoBill/' + this.ccode + '/' + this.bcode + '/' + salesBillNo);
  }

  postdifferenceDiscountAmt(estNo, diffAmt, companyCode, branchCode) {
    return this._http.post(this.apiBaseUrl + 'api/SalesBilling/ReceivableCalc?companyCode=' + companyCode + '&branchCode=' + branchCode + '&estimateNo=' + estNo + '&differenceDiscountAmt=' + diffAmt, null);
  }

  ValidateOTP(MobileNo, OrderNo, SmsID, pwd) {
    return this._http.post(this.apiBaseUrl + 'api/SalesBilling/ValidateOTP/' + this.ccode + '/' + this.bcode + '/' + MobileNo + '/' + OrderNo + '/' + SmsID + '/' + pwd, null);
  }

}