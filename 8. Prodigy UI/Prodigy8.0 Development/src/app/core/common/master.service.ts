import { BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import * as qz from 'qz-tray';
declare var qz: any;

@Injectable({
    providedIn: 'root'
})
export class MasterService {
    ccode: string = "";
    bcode: string = "";
    password: string;
    apiBaseUrl: string;

    constructor(private http: HttpClient, private appConfigService: AppConfigService) {
        this.apiBaseUrl = this.appConfigService.apiBaseUrl;
        this.password = this.appConfigService.Pwd;
        this.getCB();
    }

    getCB() {
        this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
        this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    }

    GetDailyRate() {
        return this.http.get(this.apiBaseUrl + 'api/Master/DailyRates/Get?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
    }


    //Start of common masters

    getSalesMan() {
        return this.http.get(this.apiBaseUrl + 'api/Masters/Salesman?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
    }

    getGsList(type) {
        return this.http.get(this.apiBaseUrl + 'api/Masters/LoadGS?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&type=' + type);
    }

    getNewGsList() {
        return this.http.get(this.apiBaseUrl + 'api/purchase/GS/' + this.ccode + '/' + this.bcode);
    }

    getMCTypes() {
        return this.http.get(this.apiBaseUrl + 'api/Masters/MCTypes?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
    }

    getCounter(GsCode, ItemCode) {
        return this.http.get(this.apiBaseUrl + 'api/Masters/Counter?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsName=' + GsCode + '&itemCode=' + ItemCode);
    }

    getKarat() {
        return this.http.get(this.apiBaseUrl + 'api/Masters/Karat?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
    }


    getRate(arg) {
        return this.http.get(this.apiBaseUrl + 'api/Masters/GetRatePerGramForSales?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsCode=' + arg.GsCode + '&karat=' + arg.Karat);
    }

    getRateforAddBarcode(arg) {
        return this.http.get(this.apiBaseUrl + 'api/Masters/GetRatePerGramForSales?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsCode=' + arg.GsCode + '&karat=' + arg.Karat);
    }

    getItemName(GS) {
        return this.http.get(this.apiBaseUrl + 'api/Masters/Item?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsName=' + GS);
    }

    getItemfromAPI(arg) {
        return this.http.get(this.apiBaseUrl + 'api/Masters/GetItems?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsCode=' + arg.GS);
    }


    getItemfromExistingAPI(arg) {
        return this.http.get(this.apiBaseUrl + 'api/Masters/GetItems?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsCode=' + arg);
    }

    getkaratAndPieceItem(GsCode, ItemCode) {
        return this.http.get(this.apiBaseUrl + 'api/Masters/GetItemDet?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsCode=' + GsCode + '&itemCode=' + ItemCode);
    }

    getRateperGram(arg) {
        return this.http.get(this.apiBaseUrl + 'api/Masters/RatePerGram?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsCode=' + arg.GS + '&karat=' + arg.Item + '&rateType=' + arg.RateType);
    }

    RefreshRateperGram(arg, Item) {
        return this.http.get(this.apiBaseUrl + 'api/Masters/RatePerGram?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsCode=' + arg.GS + '&karat=' + Item + '&rateType=' + arg.RateType);
    }

    getApplicationDate() {
        return this.http.get(this.apiBaseUrl + 'api/Masters/GetApplicationDate/' + this.ccode + '/' + this.bcode);
    }

    getGSTPercent(GsCode, ItemCode) {
        return this.http.get(this.apiBaseUrl + 'api/Masters/GetGSTPercent?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsCode=' + GsCode + '&itemCode=' + ItemCode);
    }

    GetCounterForBarcode() {
        return this.http.get(this.apiBaseUrl + 'api/Stock/StockTaking/Counter?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
    }

    getCardType() {
        return this.http.get(this.apiBaseUrl + 'api/Masters/CardType/' + this.ccode + '/' + this.bcode);
    }

    getCompanyMaster() {
        return this.http.get(this.apiBaseUrl + 'api/Master/CompanyMaster/Get/' + this.ccode + '/' + this.bcode);
    }

    getManagerList() {
        return this.http.get(this.apiBaseUrl + 'api/order/ManagerList/' + this.ccode + '/' + this.bcode);
    }

    getBookingTypeList() {
        return this.http.get(this.apiBaseUrl + 'api/order/BookingType/' + this.ccode + '/' + this.bcode);
    }

    getLastDocNum(docType) {
        return this.http.get(this.apiBaseUrl + 'api/docmgmt/last-series?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&docType=' + docType);
    }

    printPlainText(jsonDetails) {

        let data = [{
            type: 'raw',
            data: jsonDetails
        }];

        qz.websocket.connect()
            .then(qz.printers.getDefault)
            .then(function (printer) {
                let config = qz.configs.create(printer);
                return qz.print(config, data);
            })
            .then(() => {
                return qz.websocket.disconnect();
            })
            .catch((err) => {
                console.error(err);
            });
    }

    //End of common master



    //Permission Masters

    getPermissions() {
        return this.http.get(this.apiBaseUrl + 'api/permission-role-map/Permissions');
    }

    getRoles() {
        return this.http.get(this.apiBaseUrl + 'api/permission-role-map/Roles?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
    }

    getRolesPermissionList() {
        return this.http.get(this.apiBaseUrl + 'api/permission-role-map/List?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
    }

    postPermissions(arg) {
        const body = JSON.stringify(arg);
        return this.http.post(this.apiBaseUrl + 'api/permission-role-map/Post', body);
    }

    putActivatePermissions(arg) {
        const body = JSON.stringify(arg);
        return this.http.put(this.apiBaseUrl + 'api/permission-role-map/Activate', body);
    }

    putDeactivatePermissions(arg) {
        const body = JSON.stringify(arg);
        return this.http.put(this.apiBaseUrl + 'api/permission-role-map/De-activate', body);
    }

    getUserPermissions() {
        return this.http.get(this.apiBaseUrl + 'api/permission-user-map/Permissions');
    }

    getusers() {
        return this.http.get(this.apiBaseUrl + 'api/permission-user-map/Users?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
    }

    getUsersPermissionList() {
        return this.http.get(this.apiBaseUrl + 'api/permission-user-map/List?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
    }

    postUsersPermissions(arg) {
        const body = JSON.stringify(arg);
        return this.http.post(this.apiBaseUrl + 'api/permission-user-map/Post', body);
    }

    putUsersActivatePermissions(arg) {
        const body = JSON.stringify(arg);
        return this.http.put(this.apiBaseUrl + 'api/permission-user-map/Activate', body);
    }

    putUsersDeactivatePermissions(arg) {
        const body = JSON.stringify(arg);
        return this.http.put(this.apiBaseUrl + 'api/permission-user-map/De-activate', body);
    }

    //Ends Here


    postelevatedpermission(arg) {
        const body = JSON.stringify(arg);
        return this.http.post(this.apiBaseUrl + 'api/elevatedpermission/post', body);
    }



    //Stone Diamond Master

    getStoneType() {
        return this.http.get(this.apiBaseUrl + 'api/Master/StoneDiamondMaster/get-stone-type');
    }

    getDiamondColor() {
        return this.http.get(this.apiBaseUrl + 'api/Master/StoneDiamondMaster/get-diamond-colour/' + this.ccode + '/' + this.bcode);
    }

    getDiamondCut() {
        return this.http.get(this.apiBaseUrl + 'api/Master/StoneDiamondMaster/get-diamond-cut/' + this.ccode + '/' + this.bcode);
    }

    getDiamondClarity() {
        return this.http.get(this.apiBaseUrl + 'api/Master/StoneDiamondMaster/get-diamond-clarity/' + this.ccode + '/' + this.bcode);
    }

    getDiamondCertificate() {
        return this.http.get(this.apiBaseUrl + 'api/Master/StoneDiamondMaster/get-diamond-certificate/' + this.ccode + '/' + this.bcode);
    }

    getDiamondShape() {
        return this.http.get(this.apiBaseUrl + 'api/Master/StoneDiamondMaster/get-diamond-shape/' + this.ccode + '/' + this.bcode);
    }

    getDiamondPolish() {
        return this.http.get(this.apiBaseUrl + 'api/Master/StoneDiamondMaster/get-diamond-polish/' + this.ccode + '/' + this.bcode);
    }

    getDiamondSymmetry() {
        return this.http.get(this.apiBaseUrl + 'api/Master/StoneDiamondMaster/get-diamond-symmetry/' + this.ccode + '/' + this.bcode);
    }

    getDiamondFluorescence() {
        return this.http.get(this.apiBaseUrl + 'api/Master/StoneDiamondMaster/get-diamond-fluorescence/' + this.ccode + '/' + this.bcode);
    }

    getDiamondSize() {
        return this.http.get(this.apiBaseUrl + 'api/Master/StoneDiamondMaster/get-diamond-size/' + this.ccode + '/' + this.bcode);
    }

    getStoneList(stoneType) {
        return this.http.get(this.apiBaseUrl + 'api/Master/StoneDiamondMaster/get-stone-list/' + this.ccode + '/' + this.bcode + '/' + stoneType);
    }

    getStoneDetail(code) {
        return this.http.get(this.apiBaseUrl + 'api/Master/StoneDiamondMaster/get-stone-detail/' + this.ccode + '/' + this.bcode + '/' + code);
    }

    getDiamondList() {
        return this.http.get(this.apiBaseUrl + 'api/Master/StoneDiamondMaster/get-diamond-list/' + this.ccode + '/' + this.bcode);
    }

    getDiamondDetail(code) {
        return this.http.get(this.apiBaseUrl + 'api/Master/StoneDiamondMaster/get-diamond-detail/' + this.ccode + '/' + this.bcode + '/' + code);
    }

    //ends here
}