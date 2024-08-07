import { PurchaseService } from './../purchase.service';
import { estimationService } from './../../estimation/estimation.service';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { DiamondModel } from '../purchase.model';
import swal from 'sweetalert';
import { ToastrService } from 'ngx-toastr';
import * as CryptoJS from 'crypto-js';
import { Route, Router } from '@angular/router';
import { AppConfigService } from '../../AppConfigService';

@Component({
  selector: 'app-diamond',
  templateUrl: './diamond.component.html',
  styleUrls: ['./diamond.component.css']
})
export class DiamondComponent implements OnInit {

  ccode: string = "";
  bcode: string = "";
  ShowHide: boolean = true;
  EnableJson: boolean = false;
  password: string;
  constructor(private toastr: ToastrService, private _purchaseService: PurchaseService,
    private _estimationService: estimationService, private _router: Router,
    private _appConfigService: AppConfigService) {
    this._purchaseService.castDiamondDetails.subscribe(
      response => {
        this.fieldArray = response;
        for (var field in this.fieldArray) {
          this.EnableEditDelbtn[field] = true;
          this.EnableSaveCnlbtn[field] = false;
          this.readonly[field] = true;
        }
      }
    )

    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
    this.getCB();

    if (this._router.url == '/purchase' || this._router.url == '/estimation') {
      this.ShowHide = true;
    }
    else {
      this.ShowHide = false;
    }
  }
  private fieldArray: any = [];
  @Input() PurchaseLinesDetails: any = [];
  SalesEstNo: string = "";

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this.getDiamondDets();
    this.getDiamondName();
  }

  ItemList: any = [];

  getDiamondName() {
    this._purchaseService.getOldDmdName().subscribe(
      response => {
        this.ItemList = response;
      }
    )
  }

  OldDiamondList: any;
  getDiamondDets() {
    this._purchaseService.getOldDiamond().subscribe(
      response => {
        this.OldDiamondList = response;
      }
    )
  }


  EnableEditDelbtn = {};
  EnableSaveCnlbtn = {};
  readonly = {};

  GS: any;
  RateType: any;
  Item: any;
  count: number = 0;
  Rate: any = 0.00;
  CustomerDets: any;
  purchaseDetails: any = [];
  DiamondCarrat: number = 0;

  @Output() diamondAmount = new EventEmitter<any>();
  diamondValue: any;


  @Output() EnableDmdBtn = new EventEmitter<any>();


  //Input Output
  @Input()
  EstNo: number = 0;

  @Input()
  DmCarats: number = 0;

  //

  //To disable the GStype/Item dropdown once row added
  EnableDropdown: boolean = false;

  //To disable and enable Addrow Button
  EnableAddRow: boolean = false;

  //To disable and enable Submit Button
  EnableSubmitButton: boolean = true;

  @Input() EnableDiamondTab: boolean = false;

  ///////////////////////////////////////////////////////////////////////
  addrow() {
    this.OldDiamondDtls = {
      estno: this.EstNo,
      item_sno: null,
      type: 'p',
      Name: null,
      Qty: null,
      Carrat: null,
      Rate: null,
      OrgRate: null,
      Amount: null,
      CompanyCode: this.ccode,
      BranchCode: this.bcode
    };

    this.fieldArray.push(this.OldDiamondDtls);
    for (let { } of this.fieldArray) {
      this.count++;
    }
    this.EnableSaveCnlbtn[this.count - 1] = true;
    this.EnableEditDelbtn[this.count - 1] = false;
    this.readonly[this.count - 1] = false
    this.count = 0;
    this.EnableAddRow = true;
    this.EnableSubmitButton = true;
    this.EnableDropdown = true;
    this.EnableDisableDiamondBtn();
  }

  private OldDiamondDtls: DiamondModel = {
    estno: null,
    item_sno: null,
    type: 'p',
    Name: null,
    Qty: null,
    Carrat: null,
    OrgRate: null,
    Rate: null,
    Amount: null,
    CompanyCode: this.ccode,
    BranchCode: this.bcode
  };

  // validateCarat(index: number) {
  //   let Carrat = this.fieldArray[index].Carrat;
  //   if (Carrat > this.DmCarats) {
  //     this.toastr.error('Entered Diamond Carat is greater than the Purchase Diamond Carat', 'Alert!');
  //     this.fieldArray[index].Carrat = 0;
  //   }
  // }


  //Calculations
  GetAmount(index: number) {
    if (this.fieldArray[index].OrgRate != 0) {
      if (this.fieldArray[index].Rate > this.fieldArray[index].OrgRate) {
        swal("Warning!", 'Entered Diamond Carat Rate is greater than the Max Diamond Carat Rate', "warning");
      }
      else {
        let Carrat = this.fieldArray[index].Carrat;
        let Rate = this.fieldArray[index].Rate;
        this.fieldArray[index].Amount = Number(<number>Carrat * <number>Rate)
      }
    }
    else {
      let Carrat = this.fieldArray[index].Carrat;
      let Rate = this.fieldArray[index].Rate;
      this.fieldArray[index].Amount = Number(<number>Carrat * <number>Rate)
    }
  }


  stnDmdRate: any;
  ItemName: string;

  GetItemName(arg) {
    this.ItemName = arg;
  }

  GetStnDmdRate(index: number) {
    let StnDmdCarrat = this.fieldArray[index].Carrat;
    this._purchaseService.getStnDmdRate(this.ItemName, StnDmdCarrat).subscribe(
      response => {
        this.stnDmdRate = response;
        //this.fieldArray[index].Rate = this.stnDmdRate.OrgRate;
        this.fieldArray[index].OrgRate = this.stnDmdRate.OrgRate;
        this.fieldArray[index].Amount = Number(<number>StnDmdCarrat * <number>this.fieldArray[index].Rate)
      }
    )
  }

  //Variable declared to take previous edited value
  CopyEditedRow: any = [];

  editFieldValue(index) {
    if (this.EnableAddRow == true) {
      swal("Warning!", 'Please save the enabled item', "warning");
    }
    else {
      this.CopyEditedRow[index] = Object.assign({}, this.fieldArray[index]);
      this.EnableEditDelbtn[index] = false;
      this.EnableSaveCnlbtn[index] = true;
      this.readonly[index] = false;
      this.EnableAddRow = true;
      this.EnableSubmitButton = true;
      this.EnableDisableDiamondBtn();
    }
  }

  //Save Row
  saveDataFieldValue(index) {
    if (this.fieldArray[index]["Name"] == null || this.fieldArray[index]["Name"] == 0) {
      swal("Warning!", 'Please select the Item Name', "warning");
    }
    else if (this.fieldArray[index]["Qty"] == null || this.fieldArray[index]["Qty"] == 0) {
      swal("Warning!", 'Please Enter Qty', "warning");
    }
    else if (this.fieldArray[index]["Carrat"] == null || this.fieldArray[index]["Carrat"] == 0) {
      swal("Warning!", 'Please Enter Carrat', "warning");
    }
    else if (this.fieldArray[index]["Rate"] == null || this.fieldArray[index]["Rate"] == 0) {
      swal("Warning!", 'Please Enter Rate', "warning");
    }
    else if (this.fieldArray[index]["OrgRate"] != 0 && this.fieldArray[index]["Rate"] > this.fieldArray[index]["OrgRate"]) {
      swal("Warning!", 'Entered Diamond Carat Rate is greater than the Max Diamond Carat Rate.', "warning");
    }
    else {
      this.DiamondCarrat = this.PurchaseLinesDetails.Dcts;
      if (this.DiamondCarratTotal(this.fieldArray) == true) {
        this.DmCarats = this.fieldArray[index].Dcts;
        if (this.SalesEstNo == "") {
          this.PurchaseLinesDetails.lstPurchaseEstDiamondDetailsVM[index] = this.fieldArray[index];
        }
        this.EnableEditDelbtn[index] = true;
        this.EnableSaveCnlbtn[index] = false;
        this.readonly[index] = true;
        this.EnableAddRow = false;
        this.EnableSubmitButton = false;
        this.SendDiamondAmtToPurComp();//added for output
        this.EnableDisableDiamondBtn();
      }
      else {
        if (this.Indicator == "A") {
          swal("Warning!", 'Entered Diamond Carat is greater than the Purchase Diamond Carat', "warning");
        }
        else if (this.Indicator == "B" && this._router.url == "/estimation") {
          swal("Warning!", 'Entered Diamond Carat is lesser than the Purchase Diamond Carat', "warning");
        }
        else {
          this.DmCarats = this.fieldArray[index].Dcts;
          if (this.SalesEstNo == "") {
            this.PurchaseLinesDetails.lstPurchaseEstDiamondDetailsVM[index] = this.fieldArray[index];
          }
          this.EnableEditDelbtn[index] = true;
          this.EnableSaveCnlbtn[index] = false;
          this.readonly[index] = true;
          this.EnableAddRow = false;
          this.EnableSubmitButton = false;
          this.SendDiamondAmtToPurComp();//added for output
        }
        this.EnableDisableDiamondBtn();
      }
    }
  }

  cancelDataFieldValue(index) {
    this.EnableAddRow = false;
    if (this.CopyEditedRow[index] != null) {
      this.fieldArray[index] = this.CopyEditedRow[index];
      this.CopyEditedRow[index] = null;
      this.EnableSaveCnlbtn[index] = false;
      this.EnableEditDelbtn[index] = true;
      this.readonly[index] = true;
      this.CopyEditedRow = [];
    }
    else {
      this.fieldArray.splice(index, 1);
    }
    this.EnableDisableSubmit();
  }

  EnableDisableSubmit() {
    if (this.fieldArray.length <= 0) {
      // this.EnableSubmitButton = true;
      this.EnableDropdown = false;
    }
    else {
      // this.EnableSubmitButton = false;
      this.EnableDropdown = true;
      this.EnableDisableDiamondBtn();
    }
  }

  deleteFieldValue(index) {
    if (this.EnableAddRow == true) {
      swal("Warning!", 'Please save the enabled item', "warning");
    }
    else {
      this.fieldArray.splice(index, 1);
      //this.PurchaseLinesDetails.lstPurchaseEstStoneDetailsVM.splice(index, 1);
      this.EnableDisableSubmit();
      this.SendDiamondAmtToPurComp();
    }
  }


  Amount(arg) { let total: any = 0.00; arg.forEach((d) => { total += parseFloat(d.Amount); }); this.diamondValue = total; this.SendDiamondAmtToPurComp(); return total; }

  SendDiamondAmtToPurComp() {
    this.diamondAmount.emit(this.diamondValue);
  }

  EnableDisableDiamondBtn() {
    this.EnableDmdBtn.emit(this.EnableSubmitButton);
  }

  DiamondCount: number = 0.00;
  Diamondtotal: number = 0.00;
  Indicator: string = "";
  DiamondCarratTotal(Item): boolean {
    this.Diamondtotal = 0.00;
    for (var field of Item) {
      this.Diamondtotal += Number(this.fieldArray[this.DiamondCount].Carrat);
      this.DiamondCount++;
    }
    this.DiamondCount = 0.00;
    if (this.Diamondtotal > this.DiamondCarrat) {
      this.Indicator = "A";
      return false;
    }
    //Commented on 12-Feb-2021 due to sivanand reported
    // else if (this.Diamondtotal < this.DiamondCarrat) {
    //   this.Indicator = "B";
    //   return false;
    // }
    else {
      return true;
    }
  }
}
