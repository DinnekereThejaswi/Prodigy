import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../AppConfigService';

@Injectable({
  providedIn: 'root'
})
export class SalesBillingService {

  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string = "";

  constructor(private _http: HttpClient, private appConfigService: AppConfigService) {
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.password = this.appConfigService.Pwd;
    this.getCB();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  private SalesURL: string = this.apiBaseUrl + 'api/Masters/Salesman?companyCode=' + this.ccode + '&branchCode=' + this.bcode;


  getEstimationDetailsfromAPI(arg) {
    return this._http.get(this.SalesURL + arg);
  }

  getEstimationOrderDetailsfromAPI(arg) {
    return this._http.get(this.apiBaseUrl + "api/sales/estimation/getOrderForSales/" + arg);
  }

  postSalesBilling(object) {
    var body = JSON.stringify(object)
    return this._http.post(this.apiBaseUrl + 'api/SalesBilling/Post', body);
  }

  postdifferenceDiscountAmt(estNo, diffAmt, companyCode, branchCode) {
  
    return this._http.post(this.apiBaseUrl + 'api/SalesBilling/ReceivableCalc?companyCode=' + companyCode + '&branchCode=' + branchCode + '&estimateNo=' + estNo + '&differenceDiscountAmt=' + diffAmt, null);
  }

  getBillNoDetails(billno) {
    return this._http.get(this.apiBaseUrl + 'api/SalesBilling/Print/' + this.ccode + '/' + this.bcode + '/' + billno);
  }

  getBillNo(appDate, isCancelled) {
    return this._http.get(this.apiBaseUrl + 'api/SalesBilling/AllBill/' + this.ccode + '/' + this.bcode + '/' + appDate + '/' + isCancelled);
  }

  getCancelBillNo(billno) {
    return this._http.get(this.apiBaseUrl + 'api/SalesBilling/BillInfo/' + this.ccode + '/' + this.bcode + '/' + billno);
  }

  getByBillNo(billno) {
    return this._http.get(this.apiBaseUrl + 'api/SalesBilling/GetByBillNo/' + this.ccode + '/' + this.bcode + '/' + billno);
  }

  CancelSalesBill(remarks, billNo) {
    return this._http.post(this.apiBaseUrl + 'api/SalesBilling/Cancel/' + this.ccode + '/' + this.bcode + '/' + billNo + '/' + remarks, null);
  }

  getBillReceiptPayModes() {
    return this._http.get(this.apiBaseUrl + 'api/BillReceipt/PayModes/' + this.ccode + '/' + this.bcode);
  }


  postBillReceipt(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/BillReceipt/post', body);
  }

  getBillType() {
    return this._http.get(this.apiBaseUrl + 'api/SalesBilling/BillType/' + this.ccode + '/' + this.bcode);
  }

  getAttachedInfoBill(salesBillNo) {
    return this._http.get(this.apiBaseUrl + 'api/SalesBilling/AttachedInfoBill/' + this.ccode + '/' + this.bcode + '/' + salesBillNo);
  }

  getAttachCancelBillNo(searchType, searchValue) {
    if (searchType == "Name") {
      searchValue = "'" + searchValue + "'";
      searchValue = searchValue.toString().replace(/"/g, "");
      return this._http.get(this.apiBaseUrl + 'api/SalesBilling/BillInfo/' + this.ccode + '/' + this.bcode + '?$top=100&$skip=0&$filter=startswith(' + searchType + ',' + searchValue + ')');
    }
    else {
      return this._http.get(this.apiBaseUrl + 'api/SalesBilling/BillInfo/' + this.ccode + '/' + this.bcode + '?$top=100&$skip=0&$filter=' + searchType + ' eq ' + searchValue);
    }
  }

  ValidatePayMode(object) {
    var body = JSON.stringify(object)
    return this._http.post(this.apiBaseUrl + 'api/SalesBilling/ValidatePayMode', body);
  }

  ValidateOTP(MobileNo, OrderNo, SmsID, pwd) {
    return this._http.post(this.apiBaseUrl + 'api/SalesBilling/ValidateOTP/' + this.ccode + '/' + this.bcode + '/' + MobileNo + '/' + OrderNo + '/' + SmsID + '/' + pwd, null);
  }


  //////////////////////////Sending Est Number to Purchase Component//////////////////////

  public EstNo: BehaviorSubject<string> = new BehaviorSubject<string>("");
  SubjectEstNo = this.EstNo.asObservable();

  SendEstNo(arg) {
    this.EstNo.next(arg);
    arg = '';
  }


  public NewExistingEstNo: BehaviorSubject<string> = new BehaviorSubject<string>("");
  SubjectNewExistingEstNo = this.NewExistingEstNo.asObservable();

  SendNewExistingEstNo(arg) {
    this.NewExistingEstNo.next(arg);
    arg = '';
  }

  public OrderAttachmentSummaryData: BehaviorSubject<any> = new BehaviorSubject<any>({});
  CastOrderAttachmentSummaryData = this.OrderAttachmentSummaryData.asObservable();

  SendOrderAttachmentSummaryData(arg) {
    this.OrderAttachmentSummaryData.next(arg);
    arg = '';
  }

  public SRAttachmentSummaryData: BehaviorSubject<any> = new BehaviorSubject<any>({});
  CastSRAttachmentSummaryData = this.SRAttachmentSummaryData.asObservable();

  SendSRAttachmentSummaryData(arg) {
    this.SRAttachmentSummaryData.next(arg);
    arg = '';
  }

  public NewExistingEstNoCtrls: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  CastNewExistingEstNoCtrls = this.NewExistingEstNoCtrls.asObservable();

  SendNewExistingEstNoCtrls(arg) {
    this.NewExistingEstNoCtrls.next(arg);
    arg = '';
  }

  public OldGoldAttachmentSummaryData: BehaviorSubject<any> = new BehaviorSubject<any>({});
  CastOldGoldAttachmentSummaryData = this.OldGoldAttachmentSummaryData.asObservable();

  SendOldGoldAttachmentSummaryData(arg) {
    this.OldGoldAttachmentSummaryData.next(arg);
    arg = '';
  }


  public paymentorReceipt: BehaviorSubject<string> = new BehaviorSubject<string>("");
  SubjectpaymentorReceipt = this.paymentorReceipt.asObservable();

  SendpaymentorReceipt(arg) {
    this.paymentorReceipt.next(arg);
    arg = '';
  }
}