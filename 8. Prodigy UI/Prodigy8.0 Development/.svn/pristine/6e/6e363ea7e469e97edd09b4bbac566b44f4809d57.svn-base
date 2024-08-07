import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonService } from './../../core/common/common.service';
import { AppConfigService } from '../../AppConfigService';
@Injectable({
  providedIn: 'root'
})
export class ReligionMasterService extends CommonService {

  constructor(private _http: HttpClient, appConfigService: AppConfigService, private appConfigService1: AppConfigService) {
    super(appConfigService.apiBaseUrl + 'api/Master/Religion/', _http, appConfigService);
  }
  getReligionMaster() {
    return this._http.get(this.appConfigService1.apiBaseUrl + 'api/Master/Religion/List');
  }
  religionput(object) {
    var body = JSON.stringify(object);
    return this._http.put(this.appConfigService1.apiBaseUrl + 'AppSettings.API_ENDPOINT ' + object.ID, body);
  }
}
