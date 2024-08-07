import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';

@Injectable()
export class CommonService {

    ccode: string = "";
    bcode: string = "";
    password: string;
    apiBaseUrl: string;

    constructor(private url: string, private http: HttpClient, private appConfigService: AppConfigService) {
        this.apiBaseUrl = this.appConfigService.apiBaseUrl;
        this.password = this.appConfigService.Pwd;
        this.getCB();
    }
    AuthJWT: any = localStorage.getItem('Token');

    // path: string = "http://192.168.10.42/Magnawebpos/"

    getValueFromTop(top, skip) {
        return this.http.get(this.url + 'list?$top=' + top + '&$skip=' + skip)
    }

    getSearchCustomer(top, skip, object) {
        //var body = JSON.stringify(object);
        return this.http.post(this.url + 'searchByParam?$top=' + top + '&$skip=' + skip + '&$orderby=UpdatedDate desc', object);
    }

    getCB() {
        this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
        this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    }

    getAll() {
        return this.http.get(this.url + 'list')
    }

    get() {
        return this.http.get(this.url)
    }

    getCount() {
        return this.http.get(this.url + 'count')
    }

    getSearchCount(SearchBy, Type) {
        return this.http.get(this.url + 'searchByParamCount/' + SearchBy + '/' + Type + '/' + this.ccode + '/' + this.bcode);
    }

    getSearchDataPagination(top, skip, object) {
        var body = JSON.stringify(object);
        return this.http.post(this.url + 'searchByParam?$top=' + top + '&$skip=' + skip, body);
    }

    getValueFromId(id) {
        return this.http.get(this.url + "GetByID/" + id + "/" + this.ccode + "/" + this.bcode);
    }

    post(object) {
        var body = JSON.stringify(object);
        return this.http.post(this.url + 'post', body);
    }

    put(object) {
        var body = JSON.stringify(object);
        return this.http.put(this.url + 'put' + '?id=' + object.ID, body);
    }

    tempPut(object) {
        var body = JSON.stringify(object);
        return this.http.put(this.url + 'post', body);
    }

    delete(object) {
        return this.http.delete(this.url + "delete/?id=" + object)
    }

    //Delete operation for post method
    deleteRecord(object) {
        var body = '{"id":' + object + '}';
        return this.http.post(this.url + 'delete', body);
    }

    getReportData(top, skip) {
        var body = JSON.stringify('{ "SimpleTextBox":"sample","BranchSelector":"RJR", "fromDate":"01/01/2019","toDate":"01/01/2019","Summary":1,"Detailde":0}');
        return this.http.post(this.url + '&top=' + top + '&skip=' + skip, body)
    }

    getReportCount() {
        var body = JSON.stringify('{"SimpleTextBox":"sample","BranchSelector":"RJR", "fromDate":"01/01/2019","toDate":"01/01/2019","Summary":1,"Detailde":0}');
        return this.http.post(this.apiBaseUrl + "api/genericreport/count?id=1", body);
    }

    getList(top, skip) {
        return this.http.get(this.url + 'list?pageIndex=' + skip + '&pageSize=' + top)
    }

    getPdfExcelReport() {
        var body = JSON.stringify('{"SimpleTextBox":"sample","BranchSelector":"RJR", "fromDate":"01/01/2019","toDate":"01/01/2019","Summary":1,"Detailde":0}');
        return this.http.post(this.apiBaseUrl + 'api/genericreport/allrecords?id=1', body);
    }

}
