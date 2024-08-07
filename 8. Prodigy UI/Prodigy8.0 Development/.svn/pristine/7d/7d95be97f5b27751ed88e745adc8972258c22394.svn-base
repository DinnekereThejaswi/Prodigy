import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from './../AppConfigService';
@Injectable({
    providedIn: 'root'
})
export class OpgprocessService {

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


    //OPG Seperation

    getOPGItemGS() {
        return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-separation/get-item-gs/' + this.ccode + '/' + this.bcode);
    }

    getOPGSeparationDetail(object) {
        var body = JSON.stringify(object);
        return this._http.post(this.apiBaseUrl + 'api/Transfers/OPG-separation/get-opgseparation-detail', body);
    }

    postOPGSeperation(object) {
        var body = JSON.stringify(object);
        return this._http.post(this.apiBaseUrl + 'api/Transfers/OPG-separation/post', body);
    }

    printOPGSeperation(issueNo) {
        return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-separation/Print/' + this.ccode + '/' + this.bcode + '/' + issueNo);
    }

    //ends here

    //Opg Melting Issues

    getMeltingIssueTo() {
        return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-melting-issue/get-issue-to/' + this.ccode + '/' + this.bcode);
    }

    getMeltingIssueItemGS() {
        return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-melting-issue/get-item-gs/' + this.ccode + '/' + this.bcode);
    }

    getMeltingIssueBatchList(gsCode) {
        return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-melting-issue/get-batch-list?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsCode=' + gsCode);
    }

    getMeltingIssueBatchDetail(batchID) {
        return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-melting-issue/get-batch-detail?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&batchID=' + batchID);
    }

    postMeltingIssue(object) {
        var body = JSON.stringify(object);
        return this._http.post(this.apiBaseUrl + 'api/Transfers/OPG-melting-issue/post', body);
    }

    getMeltingIssuePrint(issueNo) {
        return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-melting-issue/Print/' + this.ccode + '/' + this.bcode + '/' + issueNo);
    }

    cancelMeltingIssue(arg) {
        return this._http.post(this.apiBaseUrl + 'api/Transfers/OPG-melting-issue/cancel?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&issueNo=' + arg.issueno + '&cancelRemarks=' + arg.remarks, null);
    }

    //ends here


    //OPG MG Issue to CPC

    getMeltingGoldIssueTo() {
        return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-cpc-issue/get-issue-to/' + this.ccode + '/' + this.bcode);
    }

    getMeltingGoldDocNo() {
        return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-cpc-issue/get-document-numbers/' + this.ccode + '/' + this.bcode);
    }

    getMeltingGoldBatchList(documentNo) {
        return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-cpc-issue/get-batch-list?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&documentNo=' + documentNo);
    }

    postMeltedGold(object) {
        var body = JSON.stringify(object);
        return this._http.post(this.apiBaseUrl + 'api/Transfers/OPG-cpc-issue/post', body);
    }

    getMeltingGoldPrint(issueNo) {
        return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-separation/print/' + this.ccode + '/' + this.bcode + '/' + issueNo);
    }

    getMeltingGoldPrint1(issueNo) {
        return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-cpc-issue/Print/' + this.ccode + '/' + this.bcode + '/' + issueNo);
    }

    getMeltingGoldXML(issueNo) {
        return this._http.post(this.apiBaseUrl + 'api/Transfers/OPG-cpc-issue/generate-xml/' + this.ccode + '/' + this.bcode + '/' + issueNo, null);
    }

    cancelMGIssueToCPC(arg) {
        return this._http.post(this.apiBaseUrl + 'api/Transfers/OPG-cpc-issue/cancel?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&issueNo=' + arg.issueno + '&cancelRemarks=' + arg.remarks, null);
    }

    //Ends here


    //opg melting receipt

    getReceiptfrom() {
        return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-melting-receipt/get-receipt-from/' + this.ccode + '/' + this.bcode);
    }
    getIssueNo(arg) {
        return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-melting-receipt/get-pendingissue-list?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&receivedFrom=' + arg);
    }
    getMeltingReceiptItemGS() {
        return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-melting-receipt/get-item-gs/' + this.ccode + '/' + this.bcode);
    }
    getMeltingReceiptDatilsByIssueNo(IssueNo) {
        return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-melting-receipt/get-batch-for-issue?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&issueNo=' + IssueNo);
    }
    getMeltingReceiptBatchDetail(issueno, batchID) {
        return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-melting-receipt/get-batchinfo?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&issueno=' + issueno + '&batchID=' + batchID);
    }
    cancelMeltingReceipt(arg) {
        return this._http.post(this.apiBaseUrl + 'api/Transfers/OPG-melting-receipt/cancel?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&receiptNo=' + arg.receiptno + '&cancelRemarks=' + arg.remarks, null);
    }

    postMeltingReceipt(object) {
        var body = JSON.stringify(object);
        return this._http.post(this.apiBaseUrl + 'api/Transfers/OPG-melting-receipt/post', body);
    }

    getMeltingReceiptPrint(issueNo) {
        return this._http.get(this.apiBaseUrl + 'api/Transfers/OPG-melting-receipt/Print/' + this.ccode + '/' + this.bcode + '/' + issueNo);
    }
    getApplicationDate() {
        return this._http.get(this.apiBaseUrl + 'api/Masters/GetApplicationDate/' + this.ccode + '/' + this.bcode);
    }

    //ends here


    //OPG Receipt to Print

    loadIRListData(arg) {
        return this._http.get(this.apiBaseUrl + 'api/IRSetup/IRSetup/' + this.ccode + '/' + this.bcode + '/' + arg);
    }

    getReceiptNo(type, date) {
        return this._http.get(this.apiBaseUrl + 'api/Transfers/receipt-cancel/get-receipt-numbers/' + this.ccode + '/' + this.bcode + '/' + type + '/' + date);
    }

    getIssueNos(type, date) {
        return this._http.get(this.apiBaseUrl + 'api/Transfers/issue-cancel/get-issue-numbers/' + this.ccode + '/' + this.bcode + '/' + type + '/' + date);
    }

    //ends here

}