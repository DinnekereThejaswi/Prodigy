import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../AppConfigService';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RepairService {
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

  getSalesManData() {
    return this._http.get(this.apiBaseUrl + 'api/masters/salesman/' + this.ccode + '/' + this.bcode);
  }

  getApplicationDate() {
    return this._http.get(this.apiBaseUrl + 'api/masters/GetApplicationDate/' + this.ccode + '/' + this.bcode);
  }

  //Repair Receipts
  getRepairGS() {
    return this._http.get(this.apiBaseUrl + 'api/Repair/RepairGS/' + this.ccode + '/' + this.bcode);
  }

  getRepairname(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Repair/RepairItem/' + this.ccode + '/' + this.bcode + '/' + arg);
  }

  post(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/Repair/Receipt/post/', body);
  }

  //cancel receipts
  //GET Receipt Number

  getReceiptNo(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Repair/Reciept/Get?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&repairNo=' + arg);
  }

  cancelReceipts(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/Repair/Reciept/CancelReceipt/', body);
  }

  //Cancel Delivery
  //GET Receipt Number
  getDeliveryNo(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Repair/Issue/GetForCancel/' + this.ccode + '/' + this.bcode + '/' + arg);
  }

  cancelDelivery(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/Repair/Issue/CancelDelivery/', body);
  }

  //Repair Delivery
  //Get Repair Delivery
  getRepairReceiptNo(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Repair/Issue/IssuesDetailsWithReceiptDetails/' + this.ccode + '/' + this.bcode + '/' + arg)
  }

  calculateGST(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/Repair/Issue/GetCalculation', body);
  }

  PostDelivery(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/Repair/Issue/Post', body);
  }
  // REPRINTS

  // getReceiptNoDetails(arg) {
  //   return this._http.get(this.apiBaseUrl + 'api/Repair/Reciept/GetForPrint/' + this.ccode + '/' + this.bcode + '/' + arg);
  // }

  getRepairReceiptPrint(repairNo, printType) {
    return this._http.get(this.apiBaseUrl + 'api/Repair/Receipt/RepairReceiptPrint/' + this.ccode + '/' + this.bcode + '/' + repairNo + '/' + printType);
  }

  getRepairDeliveryPrint(issueNo, printType, isDirect) {
    return this._http.get(this.apiBaseUrl + 'api/Repair/Issue/RepairIssuetPrint/' + this.ccode + '/' + this.bcode + '/' + issueNo + '/' + printType + '/' + isDirect);
  }


  getShowroom() {
    return this._http.get(this.apiBaseUrl + 'api/master/companymaster/get/' + this.ccode + '/' + this.bcode);
  }
  // getDeliveryNoDetails(arg) {
  //   return this._http.get(this.apiBaseUrl + 'api/Repair/Issue/GetForPrint/' + this.ccode + '/' + this.bcode + '/' + arg);
  // }
  getReceiptTotal(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Repair/Reciept/GetTotalForPrint/' + this.ccode + '/' + this.bcode + '/' + arg);
  }
  getDeliveryTotal(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Repair/Issue/GetTotalForPrint/' + this.ccode + '/' + this.bcode + '/' + arg);
  }

  getRepairReceiptPrintHTML(RepairNo) {
    return this._http.get(this.apiBaseUrl + 'api/Repair/Receipt/DotMatrixPrint/' + this.ccode + '/' + this.bcode + '/' + RepairNo);
  }

  getRepairDeliveryPrintHTML(issueNo) {
    return this._http.get(this.apiBaseUrl + 'api/Repair/Issue/DotMatrixPrint/' + this.ccode + '/' + this.bcode + '/' + issueNo);
  }

  getRepairReceiptSearchParams() {
    return this._http.get(this.apiBaseUrl + 'api/Repair/Receipt/SearchParams?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }

  getAllRepairReceipt(searchType, searchValue) {
    return this._http.get(this.apiBaseUrl + 'api/Repair/Receipt/AllReceipts/' + this.ccode + '/' + this.bcode + '/' + searchType + '/' + searchValue);
  }

  public ReceiptDeliveryNoToReprintComp: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castReceiptDeliveryNoToReprintComp = this.ReceiptDeliveryNoToReprintComp.asObservable();

  SendReceiptDeliveryNoToReprintComp(arg) {
    this.ReceiptDeliveryNoToReprintComp.next(arg);
    arg = '';
  }
}