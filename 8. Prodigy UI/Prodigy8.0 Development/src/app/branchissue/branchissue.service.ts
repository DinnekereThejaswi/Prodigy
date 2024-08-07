import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from './../AppConfigService';
import { Observable } from 'rxjs/Rx'

@Injectable({
  providedIn: 'root'
})
export class BranchissueService {

  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;
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
  postelevatedpermission(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/elevatedpermission/post', body);
  }
  getApplicationDate() {
    return this._http.get(this.apiBaseUrl + 'api/Masters/GetApplicationDate/' + this.ccode + '/' + this.bcode);
  }


  //Tag Issue

  getIssuesTo() {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/Barcode-issue/get-issue-to?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }

  getBarcodeDetail(issueTo, gsCode, barcodeNo) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/Barcode-issue/get-barcode-detail/' + this.ccode + '/' + this.bcode + '/' + issueTo + '/' + gsCode + '/' + barcodeNo);
  }

  getRate(GsCode) {
    return this._http.get(this.apiBaseUrl + 'api/Masters/GetRatePerGramForSales?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsCode=' + GsCode + '&karat=22K');
  }

  getBarcodePrint(issueNo) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/Barcode-issue/Print/' + this.ccode + '/' + this.bcode + '/' + issueNo);
  }

  generateBarcodeXML(issueNo) {
    return this._http.post(this.apiBaseUrl + 'api/Transfers/Barcode-issue/generate-xml/' + this.ccode + '/' + this.bcode + '/' + issueNo, null);
  }

  post(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/Transfers/Barcode-issue/post', body);
  }

  // ends here

  //SR Issue

  getSRIssueTo() {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/SR-issue/get-issue-to?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }

  getSRIssueDetails(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/Transfers/SR-issue/get-sr-detail', body);
  }

  postSRIssue(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/Transfers/SR-issue/post', body);
  }

  getSRPrint(issueNo) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/SR-issue/Print?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&issueNo=' + issueNo);
  }

  generateSRXML(issueNo) {
    return this._http.post(this.apiBaseUrl + 'api/Transfers/SR-issue/generate-xml?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&issueNo=' + issueNo, null);
  }

  //Ends here

  //NT Issue

  getNTIssueTo() {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/Non-tag-issue/get-issue-to?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }

  getNTItemList(counter, gs) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/Non-tag-issue/item?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsCode=' + gs + '&counterCode=' + counter);
  }

  getCounterStock(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/Non-tag-issue/CounterStock/' + this.ccode + '/' + this.bcode + '/' + arg.GSCode + '/' + arg.CounterCode + '/' + arg.ItemCode);
  }

  postNTIssue(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/Transfers/Non-tag-issue/post', body);
  }

  generateNTIssueXML(issueNo) {
    return this._http.post(this.apiBaseUrl + 'api/Transfers/Non-tag-issue/generate-xml?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&issueNo=' + issueNo, null);
  }

  public StoneDiamondDetails: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castStoneDiamondDetails = this.StoneDiamondDetails.asObservable();
  SendStoneDiamondDetailsFromPurchaseComp(arg) {
    this.StoneDiamondDetails.next(arg);
    arg = '';
  }

  //Ends here

  //OPG Issue
  getOPGIssueTo() {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-issue/get-issue-to?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }

  getOPGDetail(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/Transfers/OPG-issue/get-opg-detail', body);
  }

  postOPGDetail(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/Transfers/OPG-issue/post', body);
  }

  getOPGPrint(issueNo) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-issue/Print?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&issueNo=' + issueNo);
  }

  generateOpgXML(issueNo) {
    return this._http.post(this.apiBaseUrl + 'api/Transfers/OPG-issue/generate-xml/' + this.ccode + '/' + this.bcode + '/' + issueNo, null);
  }

  //Ends here


  //////////////////// Reprint Branch issue 

  getAllissueNoByDate(Date) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/Barcode-issue/List/' + this.ccode + '/' + this.bcode + '/' + Date);
  }
  getAllOPGissueNoByDate(Date) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-issue/List/' + this.ccode + '/' + this.bcode + '/' + Date);
  }
  getAllSRissueNoByDate(Date) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/SR-issue/List/' + this.ccode + '/' + this.bcode + '/' + Date);
  }
  getBarcodeIssuePrint(issueNo) {

    return this._http.get(this.apiBaseUrl + 'api/Transfers/Barcode-issue/Print/' + this.ccode + '/' + this.bcode + '/' + issueNo);
  }
  getOPGIssuePrint(issueNo) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-issue/Print/' + this.ccode + '/' + this.bcode + '/' + issueNo);
  }
  getSRIssuePrint(issueNo) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/SR-issue/Print/' + this.ccode + '/' + this.bcode + '/' + issueNo);
  }
  /////////////////////////////////////////////////
  //////////////issue cancel
  getAllissueNoByDateAndIsssue(type, Date) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/issue-cancel/get-issue-numbers/' + this.ccode + '/' + this.bcode + '/' + type + '/' + Date);
  }
  loadIRListData(arg) {
    return this._http.get(this.apiBaseUrl + 'api/IRSetup/IRSetup/' + this.ccode + '/' + this.bcode + '/' + arg);
  }
  getOPGIssueNumbers(date) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-issue/List/' + this.ccode + '/' + this.bcode + '/' + date);
  }
  getSRissueNumbers(date) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/SR-issue/List/' + this.ccode + '/' + this.bcode + '/' + date);
  }
  cancelOPGIssue(issueNo, cancelRemarks) {
    return this._http.post(this.apiBaseUrl + 'api/Transfers/OPG-issue/cancel-issue/' + this.ccode + '/' + this.bcode + '/' + issueNo + '/' + cancelRemarks, null);
  }
  cancelSRIssue(issueNo, cancelRemarks) {
    return this._http.post(this.apiBaseUrl + 'api/Transfers/SR-issue/cancel-issue/' + this.ccode + '/' + this.bcode + '/' + issueNo + '/' + cancelRemarks, null);
  }
  /////////////////////////////////////////////////
  ////////////// cancel issue

  getIsssueNumbersByDate(Date) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/Barcode-issue/List/' + this.ccode + '/' + this.bcode + '/' + Date);
  }
  getPrintPreviewData(issueno, remarks) {
    return this._http.get(this.apiBaseUrl + 'api/Transfers/Barcode-issue/Print/' + this.ccode + '/' + this.bcode + '/' + issueno);
  }
  cancelIssueByIssueNo(issueno, remarks) {
    return this._http.post(this.apiBaseUrl + 'api/Transfers/Barcode-issue/cancel-issue/' + this.ccode + '/' + this.bcode + '/' + issueno + '/' + remarks, null);
  }
  getPermission() {
    return this._http.get(this.apiBaseUrl + 'api/elevatedpermission/get?permissionCode=' + this.permissionCode);
  }



}