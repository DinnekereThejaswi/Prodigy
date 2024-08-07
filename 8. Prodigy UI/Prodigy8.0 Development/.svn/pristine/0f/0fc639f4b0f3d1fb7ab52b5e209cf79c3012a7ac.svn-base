import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonService } from './../../core/common/common.service';
import { AppConfigService } from '../../AppConfigService';
@Injectable({
  providedIn: 'root'
})
export class HsnMasterService extends CommonService {

  constructor(private _http: HttpClient, appConfigService: AppConfigService, private appConfigService1: AppConfigService) {
    super(appConfigService.apiBaseUrl + 'api/Master/HSNMaster/', _http, appConfigService);
  }
  getGSTGroupCode() {
    return this._http.get(this.appConfigService1.apiBaseUrl + 'api/Master/GSTMaster/List');
  }
  getType() {
    return this._http.get(this.appConfigService1.apiBaseUrl + 'api/Views/ListOfTypes');
  }
  getHSNMaster() {
    return this._http.get(this.appConfigService1.apiBaseUrl + 'api/Master/HSNMaster/List');
  }
  getDetFromId(Code) {
    return this._http.get(this.appConfigService1.apiBaseUrl + "api/Master/HSNMaster/Get/" + Code);
  }
}
