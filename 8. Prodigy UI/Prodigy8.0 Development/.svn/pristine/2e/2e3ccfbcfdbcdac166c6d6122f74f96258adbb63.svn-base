import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders } from '@angular/common/http'

@Injectable({
    providedIn:'root'
})
export class HeaderService {
    AuthJWT: any = localStorage.getItem('Token');
  constructor(private _http: HttpClient) {
  }
  SetHeaders = new HttpHeaders().set('Authorization', 'Bearer ' + localStorage.getItem('Token'));
  getSearchMenuData() {
        return this._http.get('http://192.168.10.42/Magnawebpos/api/menu/AllMenusForSearch');
    }
}
