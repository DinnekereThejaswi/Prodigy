import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonService } from './../../core/common/common.service';
import { AppConfigService } from '../../AppConfigService';

@Injectable({
  providedIn: 'root'
})
export class GsGroupingService extends CommonService {

  constructor(private _http: HttpClient, appConfigService: AppConfigService, private appConfigService1: AppConfigService) {
    super(appConfigService.apiBaseUrl + 'api/Master/GSGroup/', _http, appConfigService);
  }
  getGsGroupingList(IRCode) {
    return this._http.get(this.appConfigService1.apiBaseUrl + 'api/Master/GSGroup/List/' + IRCode);

  }
  getGSList() {
    return this._http.get(this.appConfigService1.apiBaseUrl + 'api/Master/GSItemEntryMaster/List');
  }
  getIRType() {
    return this._http.get(this.appConfigService1.apiBaseUrl + 'api/Views/ListOfIssueReceiptTypes')
  }
  deleteGSGroup(ObjID) {
    return this._http.delete(this.appConfigService1.apiBaseUrl + 'api/Master/GSGroup/Delete/' + ObjID);
  }
  editRecord(ObjID) {
    return this._http.get(this.appConfigService1.apiBaseUrl + 'api/Master/GSGroup/ListOfDetails/' + ObjID);
  }
  updateRecord(object) {
    var body = JSON.stringify(object);
    return this._http.put(this.appConfigService1.apiBaseUrl + 'api/Master/GSGroup/Put/' + object.ObjID, body);
  }

}
