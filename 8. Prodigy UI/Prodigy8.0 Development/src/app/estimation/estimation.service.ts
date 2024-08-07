import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from './../AppConfigService';

@Injectable({
  providedIn: 'root'
})

export class estimationService {

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

  //private SalesURL:string= this.apiBaseUrl +"api/sales/estimation/get?EstNo=";
  //private SalesURL:string= this.apiBaseUrl +'api/sales/estimation/Get?companyCode='+ this.ccode + '&branchCode=' + this.bcode+'&estNo=';

  getEstimationDetailsfromAPI(arg) {
    return this._http.get(this.apiBaseUrl + 'api/sales/estimation/Get?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&estNo=' + arg);
  }

  getSalesEstimationForPrint(arg) {
    return this._http.get(this.apiBaseUrl + 'api/sales/estimation/GetForPrint?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&estNo=' + arg);
  }
  getOGEstimationForPrint(arg) {
    return this._http.get(this.apiBaseUrl + 'api/purchase/GetAttachedPurchaseEstimationDetailsForPrint?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&estNo=' + arg);
  }
  getEstimationOrderDetailsfromAPI(arg) {
    return this._http.get(this.apiBaseUrl + 'api/sales/estimation/GetOrderForSales?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&orderNo=' + arg);
  }

  getSalesForPrintTotal(arg) {
    return this._http.get(this.apiBaseUrl + 'api/sales/estimation/GetForPrintTotal?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&estNo=' + arg);
  }

  getPurchaseForPrintTotal(arg) {
    return this._http.get(this.apiBaseUrl + 'api/purchase/GetForPrintTotal?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&estNo=' + arg);
  }

  getOGForPrintTotal(arg) {
    return this._http.get(this.apiBaseUrl + 'api/purchase/GetAttachedPurchaseEstimationTotalPrint?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&estNo=' + arg);
  }

  getSRForPrint(arg) {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturnEst/GetAttachedSRForPrint?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&estNo=' + arg);
  }

  getSRForPrintTotal(arg) {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturnEst/GetAttachedSRTotalForPrint?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&estNo=' + arg);
  }

  getAllEstimation() {
    return this._http.get(this.apiBaseUrl + 'api/sales/estimation/AllEstimations/' + this.ccode + '/' + this.bcode);
  }

  getSalesEstimationPlainText(estNo) {
    return this._http.get(this.apiBaseUrl + 'api/sales/estimation/DotMatrixPrint/' + this.ccode + '/' + this.bcode + '/' + estNo);
  }

  PrintBillByEstimation(estNo) {
    return this._http.get(this.apiBaseUrl + 'api/sales/estimation/PrintBillByEstimation/' + this.ccode + '/' + this.bcode + '/' + estNo);
  }

  getSalesReturnEstDotMatrixPrint(estNo) {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturnEst/DotMatrixPrint/' + this.ccode + '/' + this.bcode + '/' + estNo)
  }
  getSalesReturnEstPrint(estNo) {
    return this._http.get(this.apiBaseUrl + 'api/SalesReturnEst/Print/' + this.ccode + '/' + this.bcode + '/' + estNo)
  }

  getAllEstimationNumbers() {
    return this._http.get(this.apiBaseUrl + 'api/sales/estimation/AllEstimationDet/' + this.ccode + '/' + this.bcode);
  }


  PostMergeEstimation(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/sales/estimation/MergeEstimation', body);
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

  public Reprint: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castReprint = this.Reprint.asObservable();
  SendReprintData(arg) {
    this.Reprint.next(arg);
    arg = '';
  }

  public ReprintSR: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castReprintSR = this.ReprintSR.asObservable();
  SendReprintSR(arg) {
    this.ReprintSR.next(arg);
    arg = '';
  }

  public CoinOfferDatatoSales: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castCoinOfferDatatoSales = this.CoinOfferDatatoSales.asObservable();
  SendCoinOfferDatatoSales(arg) {
    this.CoinOfferDatatoSales.next(arg);
    arg = '';
  }


  public OrderNo: BehaviorSubject<Number> = new BehaviorSubject<Number>(0);
  SubjectOrderNo = this.OrderNo.asObservable();

  SendOrderNoToSalesComp(arg) {
    this.OrderNo.next(arg);
    arg = '';
  }


  public TotalCoinOffer: BehaviorSubject<Number> = new BehaviorSubject<Number>(0);
  SubjectTotalCoinOffer = this.TotalCoinOffer.asObservable();

  SendTotalCoinOffer(arg) {
    this.TotalCoinOffer.next(arg);
    arg = '';
  }

  public MergeEstNo: BehaviorSubject<Number> = new BehaviorSubject<any>({});
  SubjectMergeEstNo = this.MergeEstNo.asObservable();

  SendMergeEstNoToEstComp(arg) {
    this.MergeEstNo.next(arg);
    arg = '';
  }


  public BarcodeDetails: BehaviorSubject<any> = new BehaviorSubject<any>({});
  castBarcodeDetails = this.BarcodeDetails.asObservable();
  SendBarcodeDetailsFromMergeEstComp(arg) {
    this.BarcodeDetails.next(arg);
    arg = '';
  }

  getAllCompanybranchCodes(arg) {
    return this._http.get(this.apiBaseUrl + 'api/OperatorBranchMappings/Get?operatorCode=' + arg);
  }
  GetAllBranchesList() {
    return this._http.get(this.apiBaseUrl + 'api/rights-management/users/branches/' + this.ccode + '/' + this.bcode);
  }
  PrintPurEstDotMatrix(estNo) {
    return this._http.get(this.apiBaseUrl + 'api/purchase/DotMatrixPrint/' + this.ccode + '/' + this.bcode + '/' + estNo);
  }
}