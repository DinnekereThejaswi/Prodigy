import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AppConfigService } from '../../AppConfigService';
import { MetalOffer } from '../masters.model';
import { MastersService } from '../masters.service';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
@Component({
  selector: 'app-metal-rate-offer',
  templateUrl: './metal-rate-offer.component.html',
  styleUrls: ['./metal-rate-offer.component.css']
})
export class MetalRateOfferComponent implements OnInit {
  MetalOfferFrom: FormGroup;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  EnableJson: boolean = false;
  ccode: string;
  bcode: string;
  password: string;
  MetalRateOfferModel: MetalOffer = {
    ObjID: 0,
    CompanyCode: "",
    BranchCode: "",
    Id: 0,
    GsCode: "",
    Purity: 0,
    FromMcPer: 0,
    ToMcPer: 0,
    BoardRate: 0,
    OfferRate: 0,
    DiscountPer: 0,
    DiscountAmt: 0,

  }
  constructor(private fb: FormBuilder, private _appConfigService: AppConfigService, private mastersService: MastersService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {

    this.MetalOfferFrom = this.fb.group({
      FrmCtrl_GsCode: "",
      FrmCtrl_Purity: 0,
      FrmCtrl_FromMcPer: 0,
      FrmCtrl_ToMcPer: "",
      FrmCtrl_BoardRate: 0,
      FrmCtrl_OfferRate: 0,
      FrmCtrl_DiscountPer: 0,
      FrmCtrl_DiscountAmt: 0,
    });
  }
  clear() { }
  Print() { }

  Clear() { }
}
