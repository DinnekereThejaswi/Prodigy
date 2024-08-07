import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AppConfigService {

  public appConfig: any;

  constructor(private http: HttpClient) { }

  loadAppConfig() {
    return this.http.get('assets/config/config.json')
      .toPromise()
      .then(data => {
        this.appConfig = data;
      });
  }

  get apiBaseUrl() {
    if (!this.appConfig) {
      throw Error('Config file not loaded!');
    }
    return this.appConfig.apiBaseUrl;
  }

  get apiAccessTokenUrl() {
    if (!this.appConfig) {
      throw Error('Config file not loaded!');
    }
    return this.appConfig.apiAccessTokenUrl;
  }

  get apiRefreshTokenUrl() {
    if (!this.appConfig) {
      throw Error('Config file not loaded!');
    }
    return this.appConfig.apiRefreshTokenUrl;
  }

  get Pwd() {
    if (!this.appConfig) {
      throw Error('Config file not loaded!');
    }
    return this.appConfig.Pwd;
  }

  get GST() {
    if (!this.appConfig) {
      throw Error('Config file not loaded!');
    }
    return this.appConfig.GST;
  }

  get IGST() {
    if (!this.appConfig) {
      throw Error('Config file not loaded!');
    }
    return this.appConfig.IGST;
  }

  get permissionCode() {
    if (!this.appConfig) {
      throw Error('Config file not loaded!');
    }
    return this.appConfig.permissionCode;
  }

  get BcFormat() {
    if (!this.appConfig) {
      throw Error('Config file not loaded!');
    }
    return this.appConfig.BcFormat;
  }

  get RateEditCode() {
    if (!this.appConfig) {
      throw Error('Config file not loaded!');
    }
    return this.appConfig.ElevatedPermissionCodes;
  }

  get AmazonAPI() {
    if (!this.appConfig) {
      throw Error('Config file not loaded!');
    }
    return this.appConfig.AmazonAPI;
  }

  get EnableJson() {
    if (!this.appConfig) {
      throw Error('Config file not loaded!');
    }
    return this.appConfig.EnableJson;
  }

  get RowRevisionBilling() {
    if (!this.appConfig) {
      throw Error('Config file not loaded!');
    }
    return this.appConfig.RowRevisionBilling;
  }

  get ShipmentEndDate() {
    if (!this.appConfig) {
      throw Error('Config file not loaded!');
    }
    return this.appConfig.ShipmentEndDate;
  }

  get clientID() {
    if (!this.appConfig) {
      throw Error('Config file not loaded!');
    }
    return this.appConfig.clientID;
  }

  get clientSecret() {
    if (!this.appConfig) {
      throw Error('Config file not loaded!');
    }
    return this.appConfig.clientSecret;
  }

  get telerikReportUrl() {
    if (!this.appConfig) {
      throw Error('Config file not loaded!');
    }
    return this.appConfig.telerikReportUrl;
  }
  get IsOffer() {
    if (!this.appConfig) {
      throw Error('Config file not loaded!');
    }
    return this.appConfig.ApplyFab2020Offer;
  }
}