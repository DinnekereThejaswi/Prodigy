import { BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../AppConfigService';

@Injectable({
  providedIn: 'root'
})
export class PurchaseService {
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

  getPurchaseDetailsfromAPI(arg) {
    return this._http.get(this.apiBaseUrl + 'api/purchase/Get?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&estNo=' + arg);
  }


  post(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/purchase/Post', body)
  }

  getSearchParams() {
    return this._http.get(this.apiBaseUrl + 'api/purchase/searchParams');
  }

  getOldStone() {
    return this._http.get(this.apiBaseUrl + 'api/purchase/OldStone/' + this.ccode + '/' + this.bcode);
  }

  getOldDiamond() {
    return this._http.get(this.apiBaseUrl + 'api/purchase/OldDiamond/' + this.ccode + '/' + this.bcode);
  }

  PostAttachementOldGold(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/purchase/PostAttachment', body);
  }

  getAttachOldGoldList(searchType, searchValue) {
    return this._http.get(this.apiBaseUrl + 'api/purchase/Search?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&searchType=' + searchType + '&searchValue=' + searchValue);
  }

  getOldGoldttachedList(EstNo) {
   return this._http.get(this.apiBaseUrl + 'api/purchase/GetAttachedOGEst?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&estNo=' + EstNo);
  }

  deleteOldGoldAttachment(arg) {
    return this._http.get(this.apiBaseUrl + 'api/purchase/RemoveAttachment?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&estNo=' + arg);
  }

  getOldGoldList(top, skip) {
    //return this._http.get(this.apiBaseUrl + 'api/purchase/allPurchaseEst' + '?$top=' + top + '&$skip=' + skip)
    return this._http.get(this.apiBaseUrl + 'api/purchase/AllPurchaseEst?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&$top=' + top + '&$skip=' + skip);
  }

  postPurchaseBilling(object) {
    var body = JSON.stringify(object)
    return this._http.post(this.apiBaseUrl + 'api/Purchase/Billing/Post', body);
  }

  getPurchaseBill(billNo) {
    return this._http.get(this.apiBaseUrl + 'api/Purchase/Billing/Get?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&billNo=' + billNo)
  }

  CancelPurchaseBill(object) {
    var body = JSON.stringify(object)
    return this._http.post(this.apiBaseUrl + 'api/Purchase/Billing/CancelPurchaseBill', body);
  }

  AllPurchaseBill(date, isCancelled) {
    return this._http.get(this.apiBaseUrl + 'api/Purchase/Billing/AllPurchaseBill?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&date=' + date + '&isCancelled=' + isCancelled);
  }

  PrintPurchaseBill(billNo) {
    return this._http.get(this.apiBaseUrl + 'api/Purchase/Billing/Print?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&billNo=' + billNo);
  }


  PrintPurEstDotMatrix(estNo) {
    return this._http.get(this.apiBaseUrl + 'api/purchase/DotMatrixPrint/' + this.ccode + '/' + this.bcode + '/' + estNo);
  }

  PrintPurBillDotMatrix(billNo) {
    return this._http.get(this.apiBaseUrl + 'api/Purchase/Billing/DotMatrixPrint/' + this.ccode + '/' + this.bcode + '/' + billNo);
  }

  getOldDmdName() {
    return this._http.get(this.apiBaseUrl + 'api/purchase/StoneDiamondName/' + this.ccode + '/' + this.bcode + '/OD');
  }

  getStnDmdRate(ItemName, Karat) {
    return this._http.get(this.apiBaseUrl + 'api/purchase/StoneDiamondRate/' + this.ccode + '/' + this.bcode + '/' + ItemName + '/' + Karat);
  }

  ///////////start///////////////////////////////////////

  public purchaseSummaryData: BehaviorSubject<any> = new BehaviorSubject<any>({});
  cast = this.purchaseSummaryData.asObservable();
  sendPurchaseDatatoEstComp(arg) {
    this.purchaseSummaryData.next(arg);
    arg = ''
  }

  ///////////End///////////////////////////////////////



  ///////////start///////////////////////////////////////

  public CustDtls: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castCustNo = this.CustDtls.asObservable();
  SendSalesDataToEstComp(arg) {
    this.CustDtls.next(arg);
    arg = '';
  }
  ///////////End///////////////////////////////////////

	postelevatedpermission(arg) {
		const body = JSON.stringify(arg);
		return this._http.post(this.apiBaseUrl + 'api/elevatedpermission/post', body);
	}
  ///////////start///////////////////////////////////////

  public DiamondAmount: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castDiamondAmt = this.CustDtls.asObservable();
  SendDiamondAmtFromDiamondComp(arg) {
    this.DiamondAmount.next(arg);
    arg = '';
  }
  ///////////End///////////////////////////////////////

  public DiamondDetails: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castDiamondDetails = this.DiamondDetails.asObservable();
  SendDiamondDetailsFromPurchaseComp(arg) {
    this.DiamondDetails.next(arg);
    arg = '';
  }

  public StoneDetails: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castStoneDetails = this.StoneDetails.asObservable();
  SendStoneDetailsFromPurchaseComp(arg) {
    this.StoneDetails.next(arg);
    arg = '';
  }

  public ReprintPurchaseData: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castReprintPurchaseData = this.ReprintPurchaseData.asObservable();
  SendReprintPurchaseData(arg) {
    this.ReprintPurchaseData.next(arg);
    arg = '';
  }
}