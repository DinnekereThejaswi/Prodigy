import { BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';

@Injectable({
  providedIn: 'root'
})
export class AddBarcodeService {

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

  getItemfromAPI(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Masters/GetItems?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsCode=' + arg);
  }

  //stone Details
  getStoneType() {
    return this._http.get(this.apiBaseUrl + 'api/sales/estimation/GetStoneType?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }
  getStoneName(arg) {
    return this._http.get(this.apiBaseUrl + 'api/sales/estimation/GetStoneDiamondName?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&StoneType=' + arg);
  }
  // getBarcodeDetails(arg) {
  //   return this._http.get(this.apiBaseUrl + 'api/Masters/Barcode/BarcodeWithStone?companyCode='+this.ccode+'&branchCode='+this.bcode+'&barcodeNo=' + arg);
  // }

  calculateBarcode(arg) {
    let body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/sales/estimation/BarcodeCalculation', body);
  }

  BarcodePrint(arg) {
    let body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Masters/Barcode/BarcodePrint', body);
  }

  addWtBarcodeCalculation(arg1, arg2) {
    return this._http.get(this.apiBaseUrl + 'api/Masters/Barcode/BarcodeWithStoneForAddWt?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&barcodeNo=' + arg1 + '&addWtBarcode=' + arg2);
  }

  getAddWightItemGrossWeight(companyCode, branchCode, gsCode, counterCode, itemName) {
    return this._http.get(this.apiBaseUrl + 'api/sales/estimation/GetAddWightItemGrossWeight?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsCode=' + gsCode + '&counterCode=' + counterCode + '&itemName=' + itemName);
  }


  getBarcodeAge(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Masters/Barcode/BarcodeAge/' + this.ccode + '/' + this.bcode + '/' + arg);
  }

  public GetAddBarcodeData: BehaviorSubject<any> = new BehaviorSubject<any>({});
  cast = this.GetAddBarcodeData.asObservable();
  SendBarcodeDataToSalesComp(arg) {
    this.GetAddBarcodeData.next(arg);
    arg = '';
  }

  public CancelBarcodeData: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castCancelBarcodeData = this.CancelBarcodeData.asObservable();
  SendCancelBarcodeDataToSalesComp(arg) {
    this.CancelBarcodeData.next(arg);
    arg = '';
  }

  public GetInterState: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castInterState = this.GetInterState.asObservable();

  SendInterStateDataToAddBarCodeComp(arg) {
    this.GetInterState.next(arg);
    arg = '';
  }

  public GetSalCode: BehaviorSubject<string> = new BehaviorSubject<any>("");
  castGetSalCode = this.GetSalCode.asObservable();

  SendGetSalCodeToAddBarCodeComp(arg) {
    this.GetSalCode.next(arg);
    arg = '';
  }


}