import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MastersService } from '../masters.service';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { ToastrService } from 'ngx-toastr';
import { AppConfigService } from '../../AppConfigService';
@Component({
  selector: 'app-sku-master',
  templateUrl: './sku-master.component.html',
  styleUrls: ['./sku-master.component.css']
})
export class SkuMasterComponent implements OnInit {
  SKUform: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  isReadOnly: boolean = false;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  EnableJson: boolean = false;
  stockKeepUnitModel = {
    ObjID: "",
    CompanyCode: "",
    BranchCode: "",
    GSCode: null,
    SKUID: "",
    DesignCode: null,
    Weight: 0,
    ItemCode: null,
    ObjStatus: "",
    UniqRowID: ""
  }
  constructor(private fb: FormBuilder, private _mastersService: MastersService,
    private toastr: ToastrService, private _appConfigService: AppConfigService,
    private _router: Router) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);

  }
  ngOnInit() {
    this.getSkuDetails();
    this.getGs();
    this.stockKeepUnitModel.CompanyCode = this.ccode;
    this.stockKeepUnitModel.BranchCode = this.bcode;
    this.SKUform = this.fb.group({
      frmCtrl_gsname: [null, Validators.required],
      frmCtrl_itemname: [null, Validators.required],
      frmCtrl_designname: [null, Validators.required],
      frmCtrl_weight: [null, Validators.required],
      frmCtrl_skuid: [null, Validators.required]
    });
  }
  ListOfSKU: any = [];
  getSkuDetails() {
    this._mastersService.getSKUList().subscribe(
      Response => {
        this.ListOfSKU = Response;
      }
    )
  }
  GSList: any = [];
  getGs() {
    this._mastersService.GSList().subscribe(
      Response => {
        this.GSList = Response;
      }
    )
  }

  ItemName: any = [];
  getItemName(arg) {
    this.stockKeepUnitModel.GSCode = arg.target.value;
    this._mastersService.getDesginBygsCode(this.stockKeepUnitModel.GSCode).subscribe(
      Response => {
        this.ItemName = Response;
      }
    )
  }
  DesignList: any = [];
  getDesign(arg) {
    this._mastersService.getDesginByItemCode(this.stockKeepUnitModel.GSCode, arg.target.value).subscribe(
      Response => {
        this.DesignList = Response; console.log();

      }
    )
  }
  ItemLists: any = [];
  getItemsByGs(arg) {
    this._mastersService.GetItemList(arg.value.GS).subscribe(
      Response => {
        this.ItemLists = Response;
      }
    )
  }
  
  Clear() {
    this.SKUform.reset();
  }

  add(form) {

  }

}
