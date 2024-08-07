import { BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../AppConfigService';

@Injectable({
  providedIn: 'root'
})
export class SalesreturnService {

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


  getBilledBranch() {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturnEst/BilledBranch?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }

  GetsalesBillNo(arg) {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturnEst/Get?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&billNo=' + arg);
  }

  post(arg) {
    var body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/SalesReturnEst/Post', body);
  }

  //Confirm Sales Return 
  getSalesReturnDetails(arg) {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturn/Get/' + this.ccode + '/' + this.bcode + '/' + arg);
  }

  ConfirmSR(arg) {
    var body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/SalesReturn/Post', body);
  }

  getSearchParams() {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturnEst/searchParams');
  }

  getAttachSalesReturnList(searchType, searchValue) {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturnEst/GetSRAttachSearch?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&searchType=' + searchType + '&searchValue=' + searchValue);
  }

  getSalesReturnAttachedList(EstNo) {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturnEst/GetAttachedSR?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&estNo=' + EstNo);
  }

  PostAttachementSalesReturn(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/SalesReturnEst/PostSRAttachment', body);
  }

  deleteSRAttachment(arg) {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturnEst/RemoveAttachment?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&estNo=' + arg);
  }

  //Sales Attchment List(Estimation)
  getSRList(top, skip) {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturnEst/GetAllSRAttach?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&$top=' + top + '&$skip=' + skip);
  }

  getSRForPrint(arg) {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturnEst/GetAttachedSRForPrint?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&estNo=' + arg);
  }

  getSRForPrintTotal(arg) {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturnEst/GetAttachedSRTotalForPrint?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&estNo=' + arg);
  }

  getSRBill(billNo) {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturn/GetDetByBillNo/' + this.ccode + '/' + this.bcode + '/' + billNo)
  }

  CancelSRBill(object) {
    var body = JSON.stringify(object)
    return this._http.post(this.apiBaseUrl + 'api/SalesReturn/CancelSRBill', body);
  }

  getSRPrintbyEstNo(estNo) {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturnEst/Print/' + this.ccode + '/' + this.bcode + '/' + estNo)
  }

  getSRPrintbyBillNo(billNo) {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturn/Print/' + this.ccode + '/' + this.bcode + '/' + billNo)
  }

  getSRPrintDotMatrixbyBillNo(billNo) {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturn/DotMatrixPrint/' + this.ccode + '/' + this.bcode + '/' + billNo)
  }

  getSREstNoList() {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturnEst/GetAllSREst/' + this.ccode + '/' + this.bcode);
  }

  deleteSREstNo(SREstNo) {
    return this._http.delete(this.apiBaseUrl + 'api/SalesReturnEst/Delete/' + this.ccode + '/' + this.bcode + '/' + SREstNo);
  }


  ///////////start///////////////////////////////////////

  public ItemLinesSummaryData: BehaviorSubject<any> = new BehaviorSubject<any>({});
  cast = this.ItemLinesSummaryData.asObservable();
  sendItemDatatoItemComp(arg) {
    this.ItemLinesSummaryData.next(arg);
    arg = ''
  }

  public SRNoToReprintComp: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castSRNoToReprintComp = this.SRNoToReprintComp.asObservable();

  SendSRNoToReprintComp(arg) {
    this.SRNoToReprintComp.next(arg);
    arg = '';
  }



  public SRBillNoToReprintComp: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castSRBillNoToReprintComp = this.SRBillNoToReprintComp.asObservable();

  SendSRBillNoToReprintComp(arg) {
    this.SRBillNoToReprintComp.next(arg);
    arg = '';
  }

  public SRHide: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  castSRHide = this.SRHide.asObservable();

  SendSRToHide(arg) {
    this.SRHide.next(arg);
    arg = '';
  }


  public SREnablePrint: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  castSREnablePrint = this.SREnablePrint.asObservable();

  SendSREnablePrint(arg) {
    this.SREnablePrint.next(arg);
    arg = '';
  }

  ///////////End///////////////////////////////////////  
}
