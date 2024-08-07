import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class SigninService {

  Token: string;
  refreshToken: string;
  accessTokenURL: string;
  refreshTokenURL: string;
  apiBaseUrl: string;
  accessTokenbody: any;
  refreshTokenbody: any;

  constructor(private http: HttpClient, private appConfigService: AppConfigService) {
    if (this.appConfigService.appConfig != undefined) {
      this.apiBaseUrl = this.appConfigService.apiBaseUrl;
      this.accessTokenURL = this.appConfigService.apiAccessTokenUrl
    }
  }

  getAccessToken(arg) {
    const passWord = btoa(arg.password);
    const userName = arg.username;
    const clientID = this.appConfigService.clientID;
    const clientSecret = this.appConfigService.clientSecret;

    this.accessTokenbody = {
      "UserID": userName,
      "Password": passWord,
      "ClientID": clientID,
      "ClientSecret": clientSecret
    }
    return this.http.post(this.accessTokenURL, this.accessTokenbody);
  }

  getRefreshToken() {
    const authToken = localStorage.getItem('Token');
    const refreshToken = localStorage.getItem('refreshToken');
    const clientID = this.appConfigService.clientID;
    const clientSecret = this.appConfigService.clientSecret;

    this.refreshTokenbody = {
      "AuthToken": authToken,
      "RefreshToken": refreshToken,
      "ClientID": clientID,
      "ClientSecret": clientSecret
    }
    this.refreshTokenURL = this.appConfigService.apiRefreshTokenUrl;
    return this.http.post(this.refreshTokenURL, this.refreshTokenbody);
  }

  GetAccessBranches() {
    return this.http.get(this.apiBaseUrl + 'api/Usermenu/GetAccessBranches');
  }

  getAllCompanybranchCode(arg) {
    return this.http.get(this.apiBaseUrl + 'api/OperatorBranchMappings/Get?operatorCode=' + arg);
  }

  public BranchCode: BehaviorSubject<any> = new BehaviorSubject<string>("");
  castBranchCode = this.BranchCode.asObservable();

  SendBranchCode(arg) {
    this.BranchCode.next(arg);
    arg = '';
  }
}