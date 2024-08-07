import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CommonService } from './../../core/common/common.service';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import * as CryptoJS from 'crypto-js';
import { RequestOptions } from '@angular/http';
import { AppConfigService } from '../../AppConfigService';

@Injectable({
  providedIn: 'root'
})
export class CustomerService extends CommonService {
  ccode: string = "";
  bcode: string = "";
  password: string;
  constructor(private _httpClient: HttpClient, appConfigService: AppConfigService) {
    super(appConfigService.apiBaseUrl + 'api/masters/customer/', _httpClient, appConfigService);
    this.apiBaseUrl = appConfigService.apiBaseUrl;
    this.password = appConfigService.Pwd;
    this.getCB();
  }

  public customer: BehaviorSubject<any> = new BehaviorSubject<any>({});
  cast = this.customer.asObservable();

  SendCustDataToEstComp(arg) {
    this.customer.next(arg);
    arg = '';
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  getIDProof() {
    return this._httpClient.get(this.apiBaseUrl + 'api/masters/customer/GetIDProof/' + this.ccode + "/" + this.bcode);
  }

  getStateCode() {
    return this._httpClient.get(this.apiBaseUrl + 'api/masters/state/list')
  }

  // uploadFile(selectedFile,selectedFileName) {
  //   return this._httpClient.post(this.apiBaseUrl +'api/masters/UploadDoc3',formData,{headers: headerOptions});
  // }


  pickCustomer(custID, mobileNo) {
    return this._httpClient.get(this.apiBaseUrl + 'api/masters/customer/PickCustomer/' + custID + "/" + mobileNo + "/" + this.ccode + "/" + this.bcode);
  }


  getCustomerDtls(arg) {
    return this._httpClient.get(this.apiBaseUrl + 'api/masters/customer/GetByID/' + arg + "/" + this.ccode + "/" + this.bcode);
  }

  getCustDetsFromMobNo(MobileNo) {
    return this._httpClient.get(this.apiBaseUrl + "api/masters/customer/Get/" + MobileNo + "/" + this.ccode + "/" + this.bcode);
  }

  public customerDtls: BehaviorSubject<any> = new BehaviorSubject<any>({});
  cast_arg = this.customerDtls.asObservable();

  sendCustomerDtls_To_Customer_Component(arg) {
    this.customerDtls.next(arg);
  }
}