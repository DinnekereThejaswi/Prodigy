import { BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../AppConfigService';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
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

  getPaymentMode(pageName) {
    // alert('loged all all payment modes WRT  pagename'+pageName)
    return this._http.get(this.apiBaseUrl + 'api/Masters/PayMode/' + this.ccode + '/' + this.bcode + '/' + pageName);
  }

  getBank() {
    return this._http.get(this.apiBaseUrl + 'api/order/bank/' + this.ccode + '/' + this.bcode);
  }

  //Scheme Details
  getBranch() {
    return this._http.get(this.apiBaseUrl + "api/order/chit/branch/" + this.bcode);
  }

  getScheme(BranchCode) {
    return this._http.get(this.apiBaseUrl + "api/order/chit/schemeCode/" + BranchCode + "/" + this.bcode);
  }

  getGroup(BranchCode, SchemeCode) {
    return this._http.get(this.apiBaseUrl + "api/order/chit/groupCode/" + BranchCode + "/" + SchemeCode + "/" + this.bcode);
  }

  getMSNNo(BranchCode, SchemeCode, GroupCode) {
    return this._http.get(this.apiBaseUrl + "api/order/chit/startMSNNo/" + BranchCode + "/" + SchemeCode + "/" + GroupCode + "/" + this.bcode);
  }

  getchitDetails(BranchCode, SchemeCode, GroupCode, StartMSNNo, EndMSNNo) {
    return this._http.get(this.apiBaseUrl + "api/order/chit/chitDetails/" + BranchCode + "/" + SchemeCode + "/" + GroupCode + "/" + StartMSNNo + "/" + EndMSNNo + "/" + this.bcode);
  }

  getchitAmount(BranchCode, SchemeCode, GroupCode, StartMSNNo, EndMSNNo) {
    return this._http.get(this.apiBaseUrl + "api/order/chit/chitAmount/" + BranchCode + "/" + SchemeCode + "/" + GroupCode + "/" + StartMSNNo + "/" + EndMSNNo + "/" + this.bcode);
  }

  //Get Closed Receipt Details
  getClosedOrderReceiptDetails(orderno) {
    return this._http.get(this.apiBaseUrl + 'api/order/GetViewOrderWithClosedInfo/' + this.ccode + '/' + this.bcode + '/' + orderno);
  }

  getChequeName() {
    return this._http.get(this.apiBaseUrl + 'api/order/ChequeBank/' + this.ccode + '/' + this.bcode);
  }

  getChequeList(acc_no) {
    return this._http.get(this.apiBaseUrl + 'api/order/ChequeList/' + this.ccode + '/' + this.bcode + '/' + acc_no)
  }

  getPurBillList() {
    return this._http.get(this.apiBaseUrl + 'api/SalesBilling/AdjustedPurchaseBill/' + this.ccode + '/' + this.bcode);
  }

  getSRBillList() {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturn/AdjustedSRBill/' + this.ccode + '/' + this.bcode);
  }

  getSearchParamsPurBill() {
    return this._http.get(this.apiBaseUrl + 'api/Purchase/Billing/SearchParams/' + this.ccode + '/' + this.bcode);
  }

  getPurBillSearch(SearchBy, SearchByText, appDate) {
    return this._http.get(this.apiBaseUrl + 'api/Purchase/Billing/PurchaseBillSearch/' + this.ccode + '/' + this.bcode + "/" + SearchBy + "/" + SearchByText + "/" + appDate);
  }

  getSearchParamsSRBill() {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturn/SearchParams');
  }

  getSRBillSearch(SearchBy, SearchByText) {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturn/Search/' + this.ccode + '/' + this.bcode + "/" + SearchBy + "/" + SearchByText);
  }


  public CustDtls: BehaviorSubject<any> = new BehaviorSubject<any>({});

  public Details: BehaviorSubject<any> = new BehaviorSubject<any>({});
  Data = this.Details.asObservable();

  inputData(arg) {
    this.Details.next(arg);
    arg = '';
  }

  public OutputDetails: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castData = this.OutputDetails.asObservable();

  outputData(arg) {
    this.OutputDetails.next(arg);
    arg = '';
  }


  public ItemDetsToPaymentComp: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castItemDetsToPaymentComp = this.ItemDetsToPaymentComp.asObservable();

  SendItemDetsToPaymentComp(arg) {
    this.ItemDetsToPaymentComp.next(arg);
    arg = '';
  }

  public OutputParentJSON: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castParentJSON = this.OutputParentJSON.asObservable();

  OutputParentJSONFunction(arg) {
    this.OutputParentJSON.next(arg);
    arg = '';
  }


  public ItemDetsFromOrderComp: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castItemDetsFromOrderComp = this.ItemDetsFromOrderComp.asObservable();

  SendItemDetsFromOrderComp(arg) {
    this.ItemDetsFromOrderComp.next(arg);
    arg = '';
  }

  public PaymentSummaryData: BehaviorSubject<any> = new BehaviorSubject<any>({});
  CastPaymentSummaryData = this.PaymentSummaryData.asObservable();

  SendPaymentSummaryData(arg) {
    this.PaymentSummaryData.next(arg);
    arg = '';
  }
}