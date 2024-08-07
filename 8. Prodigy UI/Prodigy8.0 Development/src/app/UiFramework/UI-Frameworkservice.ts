import { HttpClient } from '@angular/common/http';
import { CommonService} from '../core/common/common.service'
import { Injectable } from '../../../node_modules/@angular/core';
import { AppConfigService } from '../AppConfigService';

@Injectable()
export class UIFrameworkservice extends CommonService {
  constructor(private _httpClient:HttpClient,appConfigService: AppConfigService ){
      super('http://192.168.10.202/Magnawebpos/api/genericreport/generatereport?id=2',_httpClient,appConfigService);
  }
}