import { PurchaseService } from './../purchase.service';
import { estimationService } from './../../estimation/estimation.service';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import swal from 'sweetalert';
import { ToastrService } from 'ngx-toastr';
import * as CryptoJS from 'crypto-js';
import { Router } from '@angular/router';
import { AppConfigService } from '../../AppConfigService';

@Component({
  selector: 'app-stone',
  templateUrl: './stone.component.html',
  styleUrls: ['./stone.component.css']
})
export class StoneComponent implements OnInit {
  ccode: string = "";
  bcode: string = "";
  ShowHide: boolean = true;
  EnableJson: boolean = false;
  password: string;
  constructor(private toastr: ToastrService, private _purchaseService: PurchaseService,
    private _estimationService: estimationService, private _router: Router,
    private _appConfigService: AppConfigService) {
    this._purchaseService.castStoneDetails.subscribe(
      response => {
        this.fieldArray = response;
        for (var field in this.fieldArray) {
          this.EnableEditDelbtn[field] = true;
          this.EnableSaveCnlbtn[field] = false;
          this.readonly[field] = true;
          this.fieldArray[field].StWt = Number(this.fieldArray[field].Carrat / 5);
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

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  @Output() EnableStnBtn = new EventEmitter<any>();


  EnableDisableStoneBtn() {
    this.EnableStnBtn.emit(this.EnableSubmitButton);
  }

  myfunction(e) {
    var code = (e.which) ? e.which : e.keyCode;
    if (code > 31 && (code < 48 || code > 57)) {
      e.preventDefault();
    }
  }

  private fieldArray: any = [];
  @Input() PurchaseLinesDetails: any = [];
  SalesEstNo: string = "";
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
  StoneCarrat: number = 0;
  OldStoneList: any;
  @Output() stoneAmount = new EventEmitter<any>();
  stoneValue: any;

  //Input Output
  @Input()
  EstNo: number = 0;

  @Input()
  StoneCarats: number = 0;

  //

  //To disable the GStype/Item dropdown once row added
  EnableDropdown: boolean = false;

  //To disable and enable Addrow Button
  EnableAddRow: boolean = false;

  //To disable and enable Submit Button
  EnableSubmitButton: boolean = true;

  @Input() EnableStoneTab: boolean = false;

  ngOnInit() {
    this.getStoneDets();
  }

  getStoneDets() {
    this._purchaseService.getOldStone().subscribe(
      response => {
        this.OldStoneList = response;
      }
    )
  }

  private StoneDtls: any = {
    estno: this.EstNo,
    item_sno: null,
    type: 'p',
    Name: 'Old Diamond',
    Qty: null,
    StWt: null,
    Carrat: null,
    Rate: null,
    Amount: null,
    CompanyCode: this.ccode,
    BranchCode: this.bcode
  };

  addrow() {
    this.StoneDtls = {
      estno: this.EstNo,
      item_sno: null,
      type: 'p',
      Name: 'Old Stone',
      Qty: null,
      StWt: null,
      Carrat: null,
      Rate: null,
      Amount: null,
      CompanyCode: this.ccode,
      BranchCode: this.bcode
    };

    this.fieldArray.push(this.StoneDtls);
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
    this.EnableDisableStoneBtn();
  }

  //Calculations
  GetAmount(index: number) {
    let Carrat = this.fieldArray[index].Carrat;
    let Rate = this.fieldArray[index].Rate;
    this.fieldArray[index].Amount = Number(<number>Carrat * <number>Rate)
  }

  GetStoneKarat(index) {
    this.fieldArray[index].Carrat = Number(this.fieldArray[index].StWt * 5);
    this.GetAmount(index);
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
      this.EnableDisableStoneBtn();
    }
  }

  //Save Row
  saveDataFieldValue(index) {
    if (this.fieldArray[index]["Qty"] == null || this.fieldArray[index]["Qty"] == 0) {
      swal("Warning!", 'Please Enter Qty', "warning");
    }
    else if (this.fieldArray[index]["Carrat"] == null || this.fieldArray[index]["Carrat"] == 0) {
      swal("Warning!", 'Please Enter Carrat', "warning");
    }
    else if (this.fieldArray[index]["Rate"] == null || this.fieldArray[index]["Rate"] == 0) {
      swal("Warning!", 'Please Enter Rate', "warning");
    }
    else {
      this.StoneCarrat = this.PurchaseLinesDetails.StneWt;

      if (this.StoneCarratTotal(this.fieldArray) == true) {
        this.StoneCarats = this.fieldArray[index].StneWt;
        if (this.SalesEstNo == "") {
          this.PurchaseLinesDetails.lstPurchaseEstStoneDetailsVM[index] = this.fieldArray[index];
        }
        this.EnableEditDelbtn[index] = true;
        this.EnableSaveCnlbtn[index] = false;
        this.readonly[index] = true;
        this.EnableAddRow = false;
        this.EnableSubmitButton = false;
        this.SendStoneAmtToPurComp();//added for output
        this.EnableDisableStoneBtn();
      }
      else {
        if (this.Indicator == "A") {
          swal("Warning!", 'Entered Stone wt is greater than the Purchase stone wt', "warning");
        }
        else if (this.Indicator == "B" && this._router.url == "/estimation") {
          swal("Warning!", 'Entered Stone wt is lesser than the Purchase stone wt', "warning");
        }
        else {
          this.StoneCarats = this.fieldArray[index].StneWt;
          if (this.SalesEstNo == "") {
            this.PurchaseLinesDetails.lstPurchaseEstStoneDetailsVM[index] = this.fieldArray[index];
          }
          this.EnableEditDelbtn[index] = true;
          this.EnableSaveCnlbtn[index] = false;
          this.readonly[index] = true;
          this.EnableAddRow = false;
          this.EnableSubmitButton = false;
          this.SendStoneAmtToPurComp();//added for output
        }
        this.EnableDisableStoneBtn();
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
      this.EnableSubmitButton = true;
      this.EnableDropdown = false;
    }
    else {
      this.EnableSubmitButton = false;
      this.EnableDropdown = true;
    }
    this.EnableDisableStoneBtn();
  }

  deleteFieldValue(index) {
    if (this.EnableAddRow == true) {
      swal("Warning!", 'Please save the enabled item', "warning");
    }
    else {
      this.fieldArray.splice(index, 1);
      //this.PurchaseLinesDetails.lstPurchaseEstStoneDetailsVM.splice(index, 1);
      this.EnableDisableSubmit();
      this.SendStoneAmtToPurComp();
    }
  }

  Amount(arg) { let total: any = 0.00; arg.forEach((d) => { total += parseFloat(d.Amount); }); this.stoneValue = total; this.SendStoneAmtToPurComp(); return total; }

  SendStoneAmtToPurComp() {
    this.stoneAmount.emit(this.stoneValue);
  }

  StoneCount: number = 0.00;
  Stonetotal: number = 0.00;
  Indicator: string = "";

  StoneCarratTotal(Item): boolean {
    this.Stonetotal = 0.00;
    for (var field of Item) {
      this.Stonetotal += Number(this.fieldArray[this.StoneCount].StWt);
      this.StoneCount++;
    }
    this.StoneCount = 0.00;
    if (this.Stonetotal > this.StoneCarrat) {
      this.Indicator = "A";
      return false;
    }
    //Commented on 12-Feb-2021 due to sivanand reported
    // else if (this.Stonetotal < this.StoneCarrat) {
    //   this.Indicator = "B";
    //   return false;
    // }
    else {
      return true;
    }
  }
}