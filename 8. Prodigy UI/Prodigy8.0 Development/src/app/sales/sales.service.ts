import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../AppConfigService';

@Injectable({
  providedIn: 'root'
})

export class SalesService {

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


  getBarcodefromAPI(arg, orderNo, interState, isOfferCoin) {
    return this._http.get(this.apiBaseUrl + 'api/Masters/Barcode/BarcodeWithStone?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&barcodeNo=' + arg + '&orderNo=' + orderNo + '&isInterstate=' + interState + '&isOfferCoin=' + isOfferCoin);
  }

  getBarcodeWithStoneWithoutValidation(barCodeNo) {
    return this._http.get(this.apiBaseUrl + 'api/Masters/Barcode/BarcodeWithStoneWithoutValidation/' + this.ccode + '/' + this.bcode + '/' + barCodeNo);
  }

  getStoneDetailsfromAPI(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Masters/Barcode/GetBarcodeStoneInfo?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&barcodeNo=' + arg);
  }

  post(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/sales/estimation/Post/', body);
  }

  put(object, estNo) {
    var body = JSON.stringify(object);
   
    return this._http.put(this.apiBaseUrl + 'api/sales/estimation/Put?estNo=' + estNo, body);
  }

  //Get Barcode Information from Order No
  getOrderEst(arg) {
    return this._http.get(this.apiBaseUrl + 'api/order/getOrderForSales/' + arg);
  }

  getRowVersion(estNo) {
    
    return this._http.get(this.apiBaseUrl + 'api/sales/estimation/RowVersion/' + this.ccode + '/' + this.bcode + '/' + estNo);
  }

  getExchangeTax(arg) {
    var body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/sales/estimation/ExchangeTax', body);
  }

  getOfferDiscount(estNo) {
    return this._http.post(this.apiBaseUrl + 'api/sales/estimation/OfferDiscount/' + this.ccode + '/' + this.bcode + '/' + estNo, null);
  }

  cancelOfferDiscount(estNo) {
    return this._http.post(this.apiBaseUrl + 'api/sales/estimation/CancelOfferDiscount/' + this.ccode + '/' + this.bcode + '/' + estNo, null);
  }
  apply2022Offer(arg) {
    var body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/sales/estimation/NewOfferDiscount', body);

  }

  BarcodeValidation(arg, barcodeNo, gsCode) {
    var body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/sales/estimation/BarcodeValidation?barcodeNo=' + barcodeNo + '&gsCode=' + gsCode, body);
  }

  public SalesEstNo: BehaviorSubject<string> = new BehaviorSubject<string>(null);
  castEstNo = this.SalesEstNo.asObservable();

  SaveSalesEstNo(EstNo) {
    this.SalesEstNo.next(EstNo);
    EstNo = '';
  }
  //fab feb offer 25/jan/2020
  get2022OfferDiscount(estNo) {
    return this._http.post(this.apiBaseUrl + 'api/sales/estimation/2022Offer/', estNo);
  }

  ///////////start///////////////////////////////////////

  public SalesData: BehaviorSubject<any> = new BehaviorSubject<any>({});
  cast = this.SalesData.asObservable();
  SendSalesDataToEstComp(arg) {
    this.SalesData.next(arg);
    arg = '';
  }
  ///////////End///////////////////////////////////////

  public SalesDtls: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castSalesDtls = this.SalesDtls.asObservable();

  salesDetails(arg) {
    this.SalesDtls.next(arg);
    arg = '';
  }


  //////////////////////////Sending Est Number to Purchase Component//////////////////////

  public SendSalesEstNo: BehaviorSubject<string> = new BehaviorSubject<string>("");
  EstNo = this.SendSalesEstNo.asObservable();
  SendEstNo_To_Purchase(arg) {
    this.SendSalesEstNo.next(arg);
    arg = '';
  }

  public AddEditBarcodeDetails: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castAddEditBarcodeDetails = this.AddEditBarcodeDetails.asObservable();

  SendSalesDataToAddBarcode(arg) {
    this.AddEditBarcodeDetails.next(arg);
    arg = '';
  }

  public SalesBarcode: BehaviorSubject<any> = new BehaviorSubject<string>("");
  castSalesBarcode = this.SalesBarcode.asObservable();

  SendSalesBarcode(arg) {
    this.SalesBarcode.next(arg);
    arg = '';
  }
}