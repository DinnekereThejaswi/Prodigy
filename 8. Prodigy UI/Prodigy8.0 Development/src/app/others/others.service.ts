import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from './../AppConfigService';

@Injectable({
    providedIn: 'root'
})

export class OthersService {

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

    pendingChits(arg) {
        return this._http.get(this.apiBaseUrl + 'api/batch-posting/chit-update/pending-chits/' + this.ccode + '/' + this.bcode + '/' + arg.date + '/' + arg.status);
    }
    downloadChits(arg) {
        return this._http.post(this.apiBaseUrl + 'api/batch-posting/chit-update/download?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&txnDate=' + arg.date, null);
    }
    billupdate(arg) {
        return this._http.post(this.apiBaseUrl + 'api/batch-posting/chit-update/bill-update?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&txnDate=' + arg.date, null);
    }
}