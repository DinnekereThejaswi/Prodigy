import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from './../AppConfigService';

@Injectable({
  providedIn: 'root'
})
export class OnlineOrderManagementSystemService {

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

  getOrderStages() {
    return this._http.get(this.apiBaseUrl + 'api/marketplace-ops/order-stages');
  }

  getPendingOrderCount() {
    return this._http.get(this.apiBaseUrl + 'api/marketplace-ops/pending-order-count/' + this.ccode + '/' + this.bcode);
  }

  getOnlineOrders(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/marketplace-ops/get-online-orders', body);
  }

  createPickList(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/marketplace-ops/create-picklist', body);
  }

  PrintPickList(AssignmentNo) {
    return this._http.get(this.apiBaseUrl + 'api/marketplace-ops/picklist/' + this.ccode + '/' + this.bcode + '/' + AssignmentNo);
  }

  getPickListNumber(orderStage) {
    return this._http.get(this.apiBaseUrl + 'api/marketplace-ops/picklist-number/' + this.ccode + '/' + this.bcode + '/' + orderStage);
  }

  getPickListDetails(assignmentNo) {
    return this._http.get(this.apiBaseUrl + 'api/marketplace-ops/picklist-detail/' + this.ccode + '/' + this.bcode + '/' + assignmentNo);
  }

  updatePickList(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/marketplace-ops/update-picklist-detail', body);
  }

  getOrdersForPacking(assignmentNo) {
    return this._http.get(this.apiBaseUrl + 'api/marketplace-ops/orders-for-packing/' + this.ccode + '/' + this.bcode + '/' + assignmentNo);
  }

  getPackageMaster() {
    return this._http.get(this.apiBaseUrl + 'api/marketplace-ops/package-master/' + this.ccode + '/' + this.bcode);
  }

  getPackageMasterAttributes(packageCode) {
    return this._http.get(this.apiBaseUrl + 'api/marketplace-ops/package-master-attributes/' + this.ccode + '/' + this.bcode + '/' + packageCode);
  }

  assignBarcode(assignmentNo, barcodeNo) {
    return this._http.put(this.apiBaseUrl + 'api/marketplace-ops/assign-barcode/' + this.ccode + '/' + this.bcode + '/' + assignmentNo + '/' + barcodeNo, null);
  }

  unassignbarcode(object) {
    var body = JSON.stringify(object);
    return this._http.put(this.apiBaseUrl + 'api/marketplace-ops/un-assign-barcode', body);
  }

  getmarketplaces() {
    return this._http.get(this.apiBaseUrl + 'api/marketplace-ops/marketplaces');
  }

  createPackage(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/marketplace-ops/create-package', body);
  }

  orderstobeinvoiced(arg) {
    return this._http.get(this.apiBaseUrl + 'api/marketplace-ops/orders-tobe-invoiced?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&fromShipDate=' + arg.StartShipDate + '&toShipDate=' + arg.EndShipDate + '&assignmentNo=' + arg.AssignmentNo);
  }

  generateInvoice(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/marketplace-ops/generate-invoice?companyCode=' + this.ccode + '&branchCode=' + this.bcode, body);
  }

  generateshiplabel(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/marketplace-ops/generate-shiplabel?companyCode=' + this.ccode + '&branchCode=' + this.bcode, body);
  }

  orderstobeshipped(marketPlace) {
    return this._http.get(this.apiBaseUrl + 'api/marketplace-ops/orders-tobe-shipped?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&marketplaceCode=' + marketPlace);
  }

  postShipment(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/marketplace-ops/ship', body);
  }

  getOnlineOrderCancel(marketplaceId) {
    return this._http.get(this.apiBaseUrl + 'api/marketplace-ops/orders-canbe-cancelled/' + this.ccode + '/' + this.bcode + '/' + marketplaceId);
  }

  CancelOnlineOrder(arg) {
    return this._http.post(this.apiBaseUrl + 'api/marketplace-ops/cancel-order/' + this.ccode + '/' + this.bcode + '/' + arg.marketplaceID + '/' + arg.orderNo + '/' + arg.cancelRemarks, null);
  }

  //Shipment Update for Online Orders

  getAwb(orderNo) {
    return this._http.get(this.apiBaseUrl + 'api/marketplace-ops/get-awb/' + this.ccode + '/' + this.bcode + '/' + orderNo);
  }

  getShipLabelContent(orderNo) {
    return this._http.get(this.apiBaseUrl + 'api/marketplace-ops/get-shiplabel-content/' + this.ccode + '/' + this.bcode + '/' + orderNo);
  }

  registerpickup(object) {
    var body = JSON.stringify(object);
    return this._http.post(this.apiBaseUrl + 'api/marketplace-ops/register-pickup', body);
  }

  cancelregisterpickup(arg) {
    return this._http.post(this.apiBaseUrl + 'api/marketplace-ops/cancel-register-pickup?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&tokenNo=' + arg.tokenNo + '&remarks=' + arg.remarks + '&registrationDate=' + arg.registrationDate, null);
  }

  cancelawb(orderNo) {
    return this._http.post(this.apiBaseUrl + 'api/marketplace-ops/cancel-awb?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&orderNo=' + orderNo, null);
  }

  //Ends here

}