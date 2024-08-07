import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonService } from './../../core/common/common.service';
import { AppConfigService } from '../../AppConfigService';

@Injectable({
  providedIn: 'root'
})
export class GstPostingSetupService extends CommonService {
  constructor(private _http: HttpClient, appConfigService: AppConfigService, private appConfigService1: AppConfigService) {
    super(appConfigService.apiBaseUrl + 'api/Master/GSTPostingSetUp/', _http, appConfigService);
  }
  getGSTPostingSetup() {
    return this._http.get(this.appConfigService1.apiBaseUrl + 'api/Master/GSTPostingSetUp/List');
  }
  deleteGSTpostingSetup(ID) {
    return this._http.delete(this.appConfigService1.apiBaseUrl + 'api/Master/GSTPostingSetUp/Delete/' + ID);
  }
  getGroupCode() {
    return this._http.get(this.appConfigService1.apiBaseUrl + 'api/Master/GSTMaster/List');
  }
  getComponentCodeAndCalculationOrder() {
    return this._http.get(this.appConfigService1.apiBaseUrl + 'api/Views/ListofGSTComponents');
  }
  getInputAndOutputRecords() {
    return this._http.get(this.appConfigService1.apiBaseUrl + 'api/Views/ListOfAccLedgerDetails');
  }
  getDetailsFromID(ID) {
    return this._http.get(this.appConfigService1.apiBaseUrl + 'api/Master/GSTPostingSetUp/Get/' + ID);

  }
  putGSTPosting(object) {
    var body = JSON.stringify(object);
    return this._http.put(this.appConfigService1.apiBaseUrl + 'api/Master/GSTPostingSetUp/Put/' + object.ID, body);
  }
}
