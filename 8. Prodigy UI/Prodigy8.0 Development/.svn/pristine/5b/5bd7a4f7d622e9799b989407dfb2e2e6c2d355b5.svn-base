import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonService } from './../../core/common/common.service';
import { AppConfigService } from '../../AppConfigService';
@Injectable({
  providedIn: 'root'
})
export class TdsMasterService extends CommonService {

  constructor(private _http: HttpClient, appConfigService: AppConfigService) {
    super(appConfigService.apiBaseUrl + 'api/Master/TDSMaster/', _http, appConfigService);
  }
  getTDSList() {
    //return this._http.get(appConfigService.apiBaseUrl + 'api/Master/TDSMaster/List');
  }
  // openOrClose(object){
  //   var body = JSON.stringify(object);
  //   return this._http.post(AppSettings.API_ENDPOINT + 'api/Master/TDSMaster/OpenOrClose/' + body, )
  // }
}
