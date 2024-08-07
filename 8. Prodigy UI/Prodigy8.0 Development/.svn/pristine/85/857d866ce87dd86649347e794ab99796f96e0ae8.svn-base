import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import { MastersService } from '../masters.service';
import swal from 'sweetalert';
declare var $: any;
import { companyModel } from '../masters.model';
@Component({
  selector: 'app-company',
  templateUrl: './company.component.html',
  styleUrls: ['./company.component.css']
})
export class CompanyComponent implements OnInit {
  ccode: string;
  bcode: string;
  password: string;
  CompanyForm: FormGroup;
  EnableJson: boolean = false;
  CompanyListData: companyModel = {
    ObjID: null,
    CompanyCode: null,
    BranchCode: null,
    CompanyName: null,
    ShortName: null,
    Address1: null,
    Address2: null,
    Address3: null,
    city: null,
    State: null,
    PhoneNo: null,
    FAX: null,
    EMailID: null,
    MobileNo: null,
    PAN: null,
    TinNo: null,
    CSTNo: null,
    Website: null,
    CompanyFooter: null,
    DisplayNo: null,
    ObjectStatus: null,
    BranchName: null,
    UpdateOn: null,
    HOCODE: null,
    EDRegNo: null,
    Header1: null,
    Header2: null,
    Header3: null,
    Header4: null,
    Header5: null,
    Header6: null,
    Header7: null,
    Footer1: null,
    Footer2: null,
    DefaultCurrencyCode: null,
    StateCode: null,
    CurrencyCode: null,
    CountryName: null,
  }
  constructor(private _appConfigService: AppConfigService, private _masterservice: MastersService, private fb: FormBuilder, private toastr: ToastrService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);

  }

  ngOnInit() {
    this.getcompanyDetails();
    this.getStateList();
    this.CompanyForm = this.fb.group({
      // frmCtrl_ObjID: [null,Validators.required],
      frmCtrl_CompanyCode: [null, Validators.required],
      frmCtrl_BranchCode: [null, Validators.required],
      frmCtrl_CompanyName: [null, Validators.required],
      frmCtrl_ShortName: [null, Validators.required],
      frmCtrl_Address1: [null, Validators.required],
      frmCtrl_Address2: [null, Validators.required],
      frmCtrl_Address3: [null, Validators.required],
      frmCtrl_city: [null, Validators.required],
      frmCtrl_State: ["", Validators.required],
      frmCtrl_PhoneNo: [null, Validators.required],
      frmCtrl_FAX: [null, Validators.required],
      frmCtrl_EMailID: [null, Validators.required],
      frmCtrl_MobileNo: [null, Validators.required],
      frmCtrl_PAN: [null, Validators.required],
      frmCtrl_TinNo: [null, Validators.required],
      frmCtrl_CSTNo: [null, Validators.required],
      frmCtrl_Website: [null, Validators.required],
      frmCtrl_CompanyFooter: [null, Validators.required],
      frmCtrl_DisplayNo: [null, Validators.required],
      frmCtrl_ObjectStatus: [null, Validators.required],
      frmCtrl_BranchName: [null, Validators.required],
      frmCtrl_UpdateOn: [null, Validators.required],
      frmCtrl_HOCODE: [null, Validators.required],
      frmCtrl_EDRegNo: [null, Validators.required],
      frmCtrl_Header1: [null, Validators.required],
      frmCtrl_Header2: [null, Validators.required],
      frmCtrl_Header3: [null, Validators.required],
      frmCtrl_Header4: [null, Validators.required],
      frmCtrl_Header5: [null, Validators.required],
      frmCtrl_Header6: [null, Validators.required],
      frmCtrl_Header7: [null, Validators.required],
      frmCtrl_Footer1: [null, Validators.required],
      frmCtrl_Footer2: [null, Validators.required],
      frmCtrl_DefaultCurrencyCode: [null, Validators.required],
      frmCtrl_StateCode: [null, Validators.required],
      frmCtrl_CurrencyCode: [null, Validators.required],
      frmCtrl_CountryName: [null, Validators.required],
      frmCtrl_Area: [null, Validators.required]
    });
  }
  companyData: any = [];
  getcompanyDetails() {
    this._masterservice.getcompany().subscribe(
      Response => {
        this.companyData = Response;
      }

    )
  }
  states: any = [];
  getStateList() {
    this._masterservice.getStateCodes().subscribe(
      response => {
        this.states = response;
      }
    )

  }
  onSubmit(form) {
    if (form.value.frmCtrl_CompanyName == null || form.value.frmCtrl_CompanyName === '') {
      swal("!Warning", "Please enter CompanyName", "warning");
    }
    else if (form.value.frmCtrl_Address1 == null || form.value.frmCtrl_Address1 === '') {
      swal("!Warning", "Please enter Address 1", "warning");
    }
    // else if (form.value.Pin == null || form.value.Pin === '') {
    //   alert('Please enter Pin');
    // }
    // else if (form.value.City == null || form.value.City === '') {
    //   alert('Please enter City');
    // }
    // else if (form.value.State == null || form.value.State === '') {
    //   alert('Please select State');
    // }
    // else if (form.value.Country == null || form.value.Country === '') {
    //   alert('Please enter Country');
    // }
    // else if (form.value.Phone == null || form.value.Phone === '') {
    //   alert('Please enter Phone Number');
    // }
    var ans = ('Do You want to save!')
    if (ans) {
      this._masterservice.EditCompanyDetails(this.companyData.ObjID, this.companyData).subscribe(
        Response => {
          swal("Updated!", "Saved " + this.companyData.CompanyName + " Saved", "success");
          this.getcompanyDetails();
        }
      )

    }
  }

}
