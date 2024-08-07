import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppConfigService } from './../../AppConfigService';


@Injectable({
    providedIn: 'root'
})

export class SideBarService {

    constructor(private _http: HttpClient, private appConfigService: AppConfigService) { }

    // getSidebar() {
    //     return this._http.get(AppSettings.API_ENDPOINT + 'api/menu/MenuByUserTemp?UserID=1')
    // }

    getMenus() {
        return this._http.get(this.appConfigService.apiBaseUrl + 'api/usermenu/get');
    }
}