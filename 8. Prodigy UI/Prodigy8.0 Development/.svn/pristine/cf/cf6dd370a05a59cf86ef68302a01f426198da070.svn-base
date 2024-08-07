import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../AppConfigService';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BranchreceiptsService {

  apiBaseUrl: string;
  password: string;
  ccode: string;
  bcode: string;
  permissionCode: string;
  constructor(private _http: HttpClient, private appConfigService: AppConfigService) {
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.password = this.appConfigService.Pwd;
    this.permissionCode = this.appConfigService.permissionCode;
    this.getCB();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  GSList() {
    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/GSNames/' + this.ccode + '/' + this.bcode);
  }

  //Barcode Receipts

  GetReceiptFrom() {
    return this._http.get(this.apiBaseUrl + '/api/Transfers/Barcode-receipt/get-receipt-from/' + this.ccode + '/' + this.bcode);
  }
  GetIssueDetails(brnahccode, issueno) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/Barcode-receipt/get-issue-detail/' + this.ccode + '/' + this.bcode + '/' + brnahccode + '/' + issueno);
  }

  post(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/Transfers/Barcode-receipt/post', body);
  }

  getPendingPrint(arg) {
    var body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Transfers/Barcode-receipt/pending-barcodes', body);
  }
  getScannedPrint(arg) {
    var body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Transfers/Barcode-receipt/scanned-barcode-summary', body);
  }
  getImportedPrint(arg) {
    var body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Transfers/Barcode-receipt/imported-barcode-summary', body);
  }
  getBarcodeReceiptPrint(receiptNo) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/Barcode-receipt/PrintSummary/' + this.ccode + '/' + this.bcode + '/' + receiptNo);
  }

  // ends here

  //Non Tag Receipts

  GetNTReceiptFrom() {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/Non-tag-receipt/get-receipt-from/' + this.ccode + '/' + this.bcode);
  }

  GetNTReceiptIssueDetail(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/Non-tag-receipt/get-issue-detail/' + this.ccode + '/' + this.bcode + '/' + arg.IssueBranchCode + '/' + arg.IssueNo);
  }

  postNTReceipt(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/Transfers/Non-tag-receipt/post', body);
  }

  public StoneDiamondDetails: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castStoneDiamondDetails = this.StoneDiamondDetails.asObservable();
  SendStoneDiamondDetailsFromPurchaseComp(arg) {
    this.StoneDiamondDetails.next(arg);
    arg = '';
  }

  //Ends here

  //Auto Barcode Receipts

  GetAutoBarcodeReceiptFrom() {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/Barcode-receipt/get-receipt-from/' + this.ccode + '/' + this.bcode);
  }

  GetAutoBarcodeReceiptIssueDetail(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/Barcode-receipt/get-issue-detail/' + this.ccode + '/' + this.bcode + '/' + arg.IssueBranchCode + '/' + arg.IssueNo);
  }

  //ends here


  /////////////////reprint receipts
  getApplicationDate() {
    return this._http.get(this.apiBaseUrl + 'api/Masters/GetApplicationDate/' + this.ccode + '/' + this.bcode);
  }
  getReceiptNoByDate(date) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/Barcode-receipt/list/' + this.ccode + '/' + this.bcode + '/' + date);
  }
  getReceiptPrintSummary(receiptNo) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/Barcode-receipt/PrintSummary/' + this.ccode + '/' + this.bcode + '/' + receiptNo);
  }
  getReceiptPrintDetailed(receiptNo) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/Barcode-receipt/PrintDet/' + this.ccode + '/' + this.bcode + '/' + receiptNo);
  }
  //////////////////////////////////////////////////
  ////////////////cancel receipts
  getPermission() {
    return this._http.get(this.apiBaseUrl + 'api/elevatedpermission/get?permissionCode=' + this.permissionCode);
  }
  postelevatedpermission(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/elevatedpermission/post', body);
  }
  getReceiptNumbersByDateAndType(Date) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/Barcode-receipt/list/' + this.ccode + '/' + this.bcode + '/' + Date);
  }
  cancelReceiptPost(receiptNo, remarks) {
    return this._http.post(this.apiBaseUrl + 'api/Transfers/Barcode-receipt/cancel-receipt/' + this.ccode + '/' + this.bcode + '/' + receiptNo + '/' + remarks, null);
  }

}