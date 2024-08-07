import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonService } from './../../core/common/common.service';
import { AppConfigService } from '../../AppConfigService';
@Injectable({
  providedIn: 'root'
})
export class MachineRegistrationService extends CommonService {

  constructor(private _http: HttpClient, appConfigService: AppConfigService, private appConfigService1: AppConfigService) {
    super(appConfigService.apiBaseUrl + 'api/Master/MachineRegistration/', _http, appConfigService);
  }
  getMachine() {
    return this._http.get(this.appConfigService1.apiBaseUrl + 'api/Master/MachineRegistration/List');
  }
  getMachineDropdown() {
    return this._http.get(this.appConfigService1.apiBaseUrl + 'api/Views/ListofMachineNames');
  }
  getBillCounter() {
    return this._http.get(this.appConfigService1.apiBaseUrl + 'api/Views/ListofBillcodeNames');
  }
  deleteMachine(ObjID) {
    return this._http.delete(this.appConfigService1.apiBaseUrl + 'api/Master/MachineRegistration/Delete/' + ObjID);
  }
}
