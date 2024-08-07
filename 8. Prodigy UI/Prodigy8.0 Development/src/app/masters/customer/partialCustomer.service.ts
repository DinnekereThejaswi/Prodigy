import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';

@Injectable()
export class PartialCustomerService {
    AuthJWT: any = localStorage.getItem('Token');

    public CustomerData: any;
    password: string;
    apiBaseUrl: string;

    constructor(private _http: HttpClient, appConfigService: AppConfigService) {
        this.apiBaseUrl = appConfigService.apiBaseUrl;
        this.password = appConfigService.Pwd;
    }
    SetHeaders = new HttpHeaders().set('Authorization', 'Bearer ' + localStorage.getItem('Token'));

    AssignCustomerData(arg) {
        this.CustomerData = JSON.stringify(arg)
        return this.CustomerData;
    }

    GetAssignCustomerData() {
        return this.CustomerData;
    }

    // getContryCode() {
    //     return this._http.get(AppSettings.API_ENDPOINT +'api/countryRegion/list');
    // }
    getStateCode() {
        return this._http.get(this.apiBaseUrl + 'api/masters/state/list')
    }

    getIDType() {
        return this._http.get(this.apiBaseUrl + 'api/masters/idproof/list')
    }

    getAjaxURLParameters() {
        return this._http.get('http://192.168.10.148/Digicatmultidemo/Data/digi/CustomerDetails.aspx?data=CL&mobileno=9538681656');
    }
}