import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import { MastersService } from '../masters.service';
import swal from 'sweetalert';
declare var $: any;



@Component({
  selector: 'app-card-charges',
  templateUrl: './card-charges.component.html',
  styleUrls: ['./card-charges.component.css']
})
export class CardChargesComponent implements OnInit {
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  isReadOnly: boolean = false;
  EnableJson: boolean = false;
  CardChargesForm: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  CardChargesListData = {
    ObjID: "",
    CompanyCode: "",
    BranchCode: "",
    Bank: " ",
    Charge: 0,
    ServiceTax: 0,
    AccCode: null,
    BankAccCode: null,
    DispSeq: 0,
    UpdateOn: null,
    CustCharge: 0
  }
  constructor(private _appConfigService: AppConfigService, private _masterservice: MastersService, private fb: FormBuilder) {

    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);

  }

  ngOnInit() {
    this.CardChargesListData.CompanyCode = this.ccode;
    this.CardChargesListData.BranchCode = this.bcode;
    this.getListCardCharges();
    this.getBankNames();
    this.getAccNames();
    this.CardChargesForm = this.fb.group({
      frmCtrl_Account: null,
      frmCtrl_Bank: null,
      frmCtrl_BankCharges: null,
      frmCtrl_STax: null,
      frmCtrl_DisSeq: null,
      frmCtrl_ChargePercent: null
    });
  }
  AccNamesList: any = [];
  getAccNames() {
    this._masterservice.getAccName().subscribe(
      Response => {
        this.AccNamesList = Response;
      }
    )
  }
  BankNamesList: any = [];
  getBankNames() {
    this._masterservice.getBankName().subscribe(
      Response => {
        this.BankNamesList = Response;
      }
    )
  }
  CardChargesList: any = [];
  getListCardCharges() {
    this._masterservice.getCardChargesList().subscribe(
      Response => {
        this.CardChargesList = Response;

      }
    )
  }

  add(form) {
    // if (form.value.frmCtrl_Account == null || form.value.frmCtrl_Account === '') {
    //   alert('Please select Account Name');
    // }
    // else if (form.value.Bank == null || form.value.Bank === '') {
    //   alert('Please select Bank Name');
    // }
    // else if (form.value.Bank == null || form.value.Bank === '') {
    //   alert('Please select Bank Name');
    // }
    //   else if (form.value.Bank == null || form.value.Bank === '') {
    //   alert('Please select Bank Name');
    // }
    // else if (form.value.Bank == null || form.value.Bank === '') {
    //   alert('Please select Bank Name');
    // }
    // else if (form.value.Bank == null || form.value.Bank === '') {
    //   alert('Please select Bank Name');
    // }
    var ans = confirm("Do you want to Add??");
    if (ans) {
      this._masterservice.PostCardCharges(this.CardChargesListData).subscribe(
        Response => {
          swal("Success!", "Saved Successfully", "success");
          this.getListCardCharges();
        }
      )
    }
  }
  selectedBank: any
  BankDetails(index) {
    var slecetedIndex = index - 1;
    this.handleChange(slecetedIndex);
  }
  handleChange(slecetedIndex) {
    this.selectedBank = this.BankNamesList[slecetedIndex];
    this.CardChargesListData.Bank = this.selectedBank.Name;
  }
  edit(arg) {
    this.CardChargesListData = arg;
    this.EnableAdd = false;
    this.EnableSave = true;

  }
  save() {
    var ans = confirm("Do you want to Save??" + this.CardChargesListData.Bank);
    if (ans) {
      this._masterservice.putCardCharges(this.CardChargesListData.ObjID, this.CardChargesListData).subscribe(
        Response => {
          swal("Success!", "Saved Successfully", "success");
          this.getListCardCharges();
          this.EnableAdd = true;
          this.EnableSave = false;
          this.CardChargesForm.reset();

        }
      )
    }
  }
  delete(arg) {
    var ans = confirm("Do you want to Delete??" + arg.Bank);
    if (ans) {
      this._masterservice.deletCardCharges(arg.ObjID).subscribe(
        Response => {
          swal("Success!", "Deleted Successfully", "Saved");
          this.getListCardCharges();
          this.EnableAdd = true;
          this.EnableSave = false;
          this.CardChargesForm.reset();
        }
      )
    }

  }
  Clear() {
    this.CardChargesForm.reset();
    this.CardChargesListData = {
      ObjID: "",
      CompanyCode: "",
      BranchCode: "",
      Bank: " ",
      Charge: 0,
      ServiceTax: 0,
      AccCode: null,
      BankAccCode: null,
      DispSeq: 0,
      UpdateOn: null,
      CustCharge: 0
    }
  }
}
