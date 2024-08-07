import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from './../AppConfigService';

@Injectable({
  providedIn: 'root'
})

export class BarcodedetailsService {
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

  //SR Barcoding

  getSRItems() {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/SR-barcoding/get-sritems-to-barcode/' + this.ccode + '/' + this.bcode);
  }

  getSRBarcodeDetail(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Transfers/SR-barcoding/barcode-detail/' + this.ccode + '/' + this.bcode, body);
  }

  postSRBarcoding(arg, SalesBillNo) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Transfers/SR-barcoding/post?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&billNo=' + SalesBillNo + '&barcodeNo=' + arg.BarcodeNo, body);
  }

  //Ends here


  //CTC Transfer

  getCTCBarcodeDetail(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Barcoding/ctc-transfer/get-barcode-detail/' + this.ccode + '/' + this.bcode + '/' + arg.CounterCode + '/' + arg.Barcode);
  }

  postCTCBarcodeDetail(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Barcoding/ctc-transfer/post', body);
  }

  //Ends here

  //Item to Item Transfer

  getITIBarcodeDetail(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Barcoding/item-to-item-transfer/get-barcode-detail/' + this.ccode + '/' + this.bcode + '/' + arg.GSCode + '/' + arg.Item + '/' + arg.Barcode);
  }

  postITIBarcodeDetail(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Barcoding/item-to-item-transfer/post', body);
  }

  //Ends here


  //OrderNo edit in barcode

  getBarcodeDetails(barcodeNo) {
    return this._http.get(this.apiBaseUrl + 'api/Barcoding/order-number-update/get-barcode-detail/' + this.ccode + '/' + this.bcode + '/' + barcodeNo);
  }

  updateOrderNoToBarcodeDetail(arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Barcoding/order-number-update/put/' + this.ccode + '/' + this.bcode, body);
  }

  //ends here

  /////reprint barcode
  postReprintBarcodes(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + '/api/Barcoding/barcode-print/print', body);

  }
}