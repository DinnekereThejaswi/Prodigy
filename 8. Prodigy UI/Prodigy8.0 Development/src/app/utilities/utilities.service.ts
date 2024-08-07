import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppConfigService } from '../AppConfigService';
import * as CryptoJS from 'crypto-js';
@Injectable({
  providedIn: 'root'
})
export class UtilitiesService {

  ccode: string;
  bcode: string;
  password: string;
  apiBaseUrl: string;
  permissionCode: string;
  constructor(private _http: HttpClient, private appConfigService: AppConfigService) {
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.password = this.appConfigService.Pwd;
    this.getCB();
    this.permissionCode = this.appConfigService.permissionCode;

  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }
  getAllCompanybranchCodes(arg) {
    return this._http.get(this.apiBaseUrl + 'api/OperatorBranchMappings/Get?operatorCode=' + arg);
  }
  getBranchCompanys(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Masters/Operator/DefaultBranch/' + arg)
  }

  //Role Permission

  getRoleAssignment(roleID) {
    return this._http.get(this.apiBaseUrl + 'api/rights-management/role-assignment/get/' + this.ccode + '/' + this.bcode + '/' + roleID);
  }

  postRoleAssignment(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/rights-management/role-assignment/post', body);
  }

  //ends here


  //Main Module Settings

  getMainModuleList() {
    return this._http.get(this.apiBaseUrl + 'api/rights-management/main-modules/list/' + this.ccode + '/' + this.bcode);
  }

  getMainModuleByID(id) {
    return this._http.get(this.apiBaseUrl + 'api/rights-management/main-modules/get?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&id=' + id);
  }

  postMainModule(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/rights-management/main-modules/post', body);
  }

  putMainModule(arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/rights-management/main-modules/put', body);
  }

  openMainModule(id) {
    return this._http.post(this.apiBaseUrl + 'api/rights-management/main-modules/activate?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&id=' + id, null);
  }

  closeMainModule(id) {
    return this._http.post(this.apiBaseUrl + 'api/rights-management/main-modules/de-activate?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&id=' + id, null);
  }

  //ends here


  //Sub Module Settings

  getSubModuleList() {
    return this._http.get(this.apiBaseUrl + 'api/rights-management/sub-modules/list/' + this.ccode + '/' + this.bcode);
  }

  getSubModuleByModuleID(moduleId) {
    return this._http.get(this.apiBaseUrl + 'api/rights-management/sub-modules/list-by-module?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&moduleId=' + moduleId);
  }

  getSubModuleByID(id) {
    return this._http.get(this.apiBaseUrl + 'api/rights-management/sub-modules/get?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&id=' + id);
  }

  postSubModule(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/rights-management/sub-modules/post', body);
  }

  putSubModule(arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/rights-management/sub-modules/put', body);
  }

  openSubModule(id) {
    return this._http.post(this.apiBaseUrl + 'api/rights-management/sub-modules/activate?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&id=' + id, null);
  }

  closeSubModule(id) {
    return this._http.post(this.apiBaseUrl + 'api/rights-management/sub-modules/de-activate?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&id=' + id, null);
  }

  //ends here


  //New Operators
  getArraysIntersection(a1, a2) {
    return a1.filter(function (n) { return a2.indexOf(n) !== -1; });
  }
  getOperatorsDetails() {
    return this._http.get(this.apiBaseUrl + 'api/rights-management/users/list?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }
  getCounterList() {
    return this._http.get(this.apiBaseUrl + 'api/Master/CounterMaster/List/' + this.ccode + '/' + this.bcode);
  }
  GetCounter() {
    return this._http.get(this.apiBaseUrl + 'api/Stock/StockTaking/Counter?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }
  rolesList() {
    return this._http.get(this.apiBaseUrl + 'api/rights-management/roles/list/' + this.ccode + '/' + this.bcode);
  }
  getRoleNames(ID) {
    return this._http.get(this.apiBaseUrl + 'api/rights-management/roles/get/' + this.ccode + '/' + this.bcode + '/' + ID);
  }
  PostOperatorsDetails(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/rights-management/users/post', body);
  }
  ModifyOperatorsDetails(arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/rights-management/users/put', body);
  }
  ActiveOperator(arg) {
    return this._http.post(this.apiBaseUrl + 'api/rights-management/users/activate?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&userID=' + arg, null);
  }
  InActiveOperator(arg) {
    return this._http.post(this.apiBaseUrl + 'api/rights-management/users/de-activate?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&userID=' + arg, null);
  }
  changeOprtrPassword(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/rights-management/users/change-password', body);
  }
  GetAllBranchesList() {
    return this._http.get(this.apiBaseUrl + 'api/rights-management/users/branches/' + this.ccode + '/' + this.bcode);
  }

  //ends here

  //new roles

  getRolesListData() {
    return this._http.get(this.apiBaseUrl + 'api/rights-management/roles/list/' + this.ccode + '/' + this.bcode);
  }
  PostNewroles(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/rights-management/roles/post', body);
  }
  EditNewroles(arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/rights-management/roles/put', body);
  }

  ActivateRole(arg) {
    return this._http.post(this.apiBaseUrl + 'api/rights-management/roles/activate?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&roleId=' + arg, null);
  }
  DeActivateRole(arg) {
    return this._http.post(this.apiBaseUrl + 'api/rights-management/roles/de-activate?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&roleId=' + arg, null);
  }

  //ends here
  //tolerance
  getToleranceList() {
    return this._http.get(this.apiBaseUrl + 'api/Master/Tolerance/list?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }
  AddNewTolerance(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Master/Tolerance/post', body);

  }
  EditTolerance(arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Master/Tolerance/put', body);
  }
  DeleteTolerance(arg) {
    return this._http.post(this.apiBaseUrl + 'api/Master/Tolerance/delete/' + this.ccode + '/' + this.bcode + '/' + arg, null);
  }
  //ends here


  //app passwords
  getPasswordList() {
    return this._http.get(this.apiBaseUrl + 'api/Master/Applicatoin-password/list?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }
  AddNewPassword(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Master/Applicatoin-password/post', body);
  }
  DeletePassword(arg) {
    return this._http.post(this.apiBaseUrl + 'api/Master/Tolerance/delete/' + this.ccode + '/' + this.bcode + '/' + arg, null);
  }

  ///end here

}
