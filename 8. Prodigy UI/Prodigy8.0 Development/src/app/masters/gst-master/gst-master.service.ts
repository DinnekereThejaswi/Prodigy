import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppConfigService } from '../../AppConfigService';
@Injectable({
  providedIn: 'root'
})
export class GstMasterService {

  constructor(private _http: HttpClient, private _appConfigService: AppConfigService) { }
  getGroupTypes() {
    return this._http.get(this._appConfigService.apiBaseUrl + 'api/Views/ListOfGroupTypes');
  }
}
