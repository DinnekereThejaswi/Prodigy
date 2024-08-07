import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppConfigService } from '../AppConfigService';

@Injectable({
  providedIn: 'root'
})
export class CbwidgetService {

  password: string;
  apiBaseUrl: string;

  constructor(private _http: HttpClient, private appConfigService: AppConfigService) {
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.password = this.appConfigService.Pwd;
  }

  getAllCompanybranchCode(arg) {
    return this._http.get(this.apiBaseUrl + 'api/OperatorBranchMappings/Get?operatorCode=' + arg);
  }

  getBranchCompany(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Masters/Operator/DefaultBranch/' + arg)
  }
}
