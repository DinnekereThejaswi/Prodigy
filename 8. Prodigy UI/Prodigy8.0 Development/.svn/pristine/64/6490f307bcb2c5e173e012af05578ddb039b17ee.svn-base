import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MastersService } from './../masters.service';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { ToastrService } from 'ngx-toastr';
import { AppConfigService } from '../../AppConfigService';
import { MasterService } from '../../core/common/master.service';
@Component({
  selector: 'app-branch-master',
  templateUrl: './branch-master.component.html',
  styleUrls: ['./branch-master.component.css']
})
export class BranchMasterComponent implements OnInit {

  @ViewChild("RefNo", { static: true }) RefNo: ElementRef;
  @ViewChild("Codes", { static: true }) Codes: ElementRef;
  DefaultInfo: any = [];
  ccode: string;
  bcode: string;
  password: string;
  isReadOnly: boolean = false;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  EnableJson: boolean = false;
  EnableSupplierDetails: boolean = true;
  EnableOpeningDetails: boolean = true;
  OpeningDetailsHeaderForm: FormGroup;
  SupplierDetailsForm: FormGroup;
  BranchListData: any = {
    ObjID: "",
    CompanyCode: "",
    BranchCode: "",
    PartyCode: "",
    PartyName: "",
    VoucherCode: "VB",
    Address1: "",
    Address2: "",
    Address3: "",
    District: "",
    State: "",
    StateStatus: "O",
    City: "",
    Country: "",
    PinCode: "",
    Phone: "",
    Mobile: "",
    FaxNo: "",
    PAN: null,
    Website: "",
    TIN: "",
    TDS: "",
    VAT: "N",
    CSTNo: "",
    ObjectStatus: "O",
    UpdateOn: "",
    PartyRTGScode: "",
    PartyNEFTcode: "",
    PartyIFSCcode: "",
    PartyAccountNo: "",
    PartyMICRNo: "",
    PartyBankName: "",
    PartyBankBranch: "",
    PartyBankAddress: "",
    ContactPerson: "",
    ContactEmail: "",
    Email: "",
    ContactMobileNo: "",
    PartyACName: "",
    OpnBal: 0.00,
    OpnBalType: "C",
    OpnWeight: 0.00,
    OpnWeightType: "C",
    CreditPeriod: 0,
    LeadTime: 0,
    MaxPayment: null,
    ConvRate: null,
    SwiftCode: "",
    BranchType: "R",
    TDSPercent: 0.00,
    CreditWeight: null,
    TDSId: 0,
    IsSameEntity: "N",
    StateCode: null,
    SchemeType: null,
    SupplierMetal: null,
    UniqRowID: "",
    ListGroupTo: [],
    ListOpenDet: [],
  }
  ListOpenDetModel =
    {
      ObjID: "",
      CompanyCode: "",
      BranchCode: "",
      PartyCode: "",
      VoucherCode: "VS",
      OpnBal: 0.00,
      BalType: null,
      WeightType: null,
      ObjectStatus: "O",
      UpdateOn: "",
      MetalCode: null,
      OpnPureWeight: 0.00,
      OpnNetWeight: 0.00,
      FinYear: 2014,
      GSCode: null,
      RefNo: 0,
      UniqRowID: "",
      OpnWeight: 0.00
    }
  uom = [{ "code": "W", "name": "weight" },
  { "code": "P", "name": "piece" },
  { "code": "C", "name": "Carat" }]
  DebitOrCredit = [{ "code": "D", "name": "Debit" },
  { "code": "C", "name": "Credit" }];
  constructor(private fb: FormBuilder, private _masterservice: MastersService, private _router: Router,
    private _appConfigService: AppConfigService, private _masterService: MasterService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);

  }
  ngOnInit() {
    this.EnableAdd = true;
    this.uom;
    this.DebitOrCredit;
    this.opentype();
    this.vendorGSTypes();
    this.getStateCode();
    this.BranchListData.VoucherCode = "VB";
    this.BranchListData.CompanyCode = this.ccode;
    this.BranchListData.BranchCode = this.bcode;
    this._masterService.getCompanyMaster().subscribe(
      Response => {
        this.DefaultInfo = Response;
        this.BranchListData.StateCode = this.DefaultInfo.StateCode;
        this.BranchListData.City = this.DefaultInfo.city;
      }
    )
    this.TDS();
    this.SupplierDetailsForm = this.fb.group({
      frmCtrl_Code: null,
      frmCtrl_GSTIN: null,
      frmCtrl_Name: null,
      frmCtrl_PAN: null,
      frmCtrl_TDs: null,
      frmCtrl_Address1: null,
      frmCtrl_Address2: null,
      frmCtrl_country: null,
      frmCtrl_Area: null,
      frmCtrl_pin: null,
      frmCtrl_city: null,
      frmCtrl_state: null,
      frmCtrl_Country: null,
      frmCtrl_District: null,
      frmCtrl_Phone: null,
      frmCtrl_Mob: null,
      frmCtrl_Fax: null,
      frmCtrl_EmailID: null,
      frmCtrl_Website: null,
      frmCtrl_ContactPerson: null,
      frmCtrl_ContactEmailID: null,
      frmCtrl_International: null,
      frmCtrl_Local: null,
      frmCtrl_InterState: null,
      frmCtrl_Contactno: null,
      frmCtrl_CST: null,
      frmCtrl_TDS: null,
      frmCtrl_scheme: null,
      frmCtrl_Metal: null,

    });
    ///////////////////
    this.OpeningDetailsHeaderForm = this.fb.group({
      frmCtrl_ObjID: null,
      frmCtrl_CompanyCode: null,
      frmCtrl_BranchCode: null,
      frmCtrl_PartyCode: null,
      frmCtrl_VoucherCode: null,
      frmCtrl_OpnBal: null,
      frmCtrl_BalnceType: null,
      frmCtrl_OpnWeight: null,
      frmCtrl_WeightType: null,
      frmCtrl_ObjectStatus: null,
      frmCtrl_UpdateOn: null,
      frmCtrl_MetalCode: null,
      frmCtrl_OpnPureWeight: null,
      frmCtrl_OpnNetWeight: null,
      frmCtrl_FinYear: null,
      frmCtrl_GSCode: null,
      frmCtrl_RefNo: null,
      frmCtrl_UniqRowID: null
    });

  }
  SameEntityCheck(e) {
    if (e.target.checked == true) {
      this.BranchListData.IsSameEntity = "Y";
    }
    if (e.target.checked == false) {

      this.BranchListData.IsSameEntity = "N";
    }
  }
  IsChecked() {

  }
  ToggleSupplierDetails() {
    this.EnableSupplierDetails = !this.EnableSupplierDetails;
    this.Codes.nativeElement.focus();
  }
  ToggleOpeningDetails() {
    this.EnableOpeningDetails = !this.EnableOpeningDetails;
  }

  GSTypes: any = [];
  vendorGSTypes() {
    this._masterservice.getBrnachGSTypeList().subscribe(
      Response => {
        this.GSTypes = Response;
      }
    )
  }
  openTypes: any = [];
  opentype() {
    this._masterservice.GetBrnachOpenTypeList().subscribe(
      Response => {
        this.openTypes = Response;
      }
    )
  }
  StateList: any;
  getStateCode() {
    this._masterservice.getStateCode().subscribe(
      Response => {
        this.StateList = Response;
      }
    );
  }
  TDSVendors: any = [];
  TDS() {
    this._masterservice.GetTDS().subscribe(
      Response => {
        this.TDSVendors = Response;
      }
    )
  }
  selectedTDSValue: any;
  TDSDropdownChange(index) {
    var slecetIndex = index - 1;
    this.handleChange(slecetIndex);
  }
  handleChange(slecetIndex) {
    this.selectedTDSValue = this.TDSVendors[slecetIndex];
    this.BranchListData.TDSPercent = this.selectedTDSValue.tds;
    this.BranchListData.TDS = "N";
    this.BranchListData.TDSId = this.selectedTDSValue.tdsID;
  }
  selectedstate: any;
  stateDetails(index) {
    var slecetIndex = index - 1;
    this.selectedstatehandleChange(slecetIndex);
  }
  selectedstatehandleChange(slecetIndex) {
    this.selectedstate = this.StateList[slecetIndex];
    this.BranchListData.State = this.selectedstate.StateName;
    this.BranchListData.StateCode = this.selectedstate.TINNo;
    this.BranchListData.StateStatus == this.selectedstate.ObjectStatus;
  }
  CalculatePureWt(arg) {
    var pureWt = this.ListOpenDetModel.OpnWeight - arg;
    this.ListOpenDetModel.OpnPureWeight = Number(pureWt);
  }
  openingDetailsObj: any = [];
  AddOpeningDetails() {
    this.openingDetailsObj.push(this.ListOpenDetModel);
    // this.OpeningDetailsHeaderForm.reset();
    this.ListOpenDetModel =
    {
      ObjID: "",
      CompanyCode: "",
      BranchCode: "",
      PartyCode: "",
      VoucherCode: "VS",
      OpnBal: 0.00,
      BalType: null,
      WeightType: null,
      ObjectStatus: "O",
      UpdateOn: "",
      MetalCode: null,
      OpnPureWeight: 0.00,
      OpnNetWeight: 0.00,
      FinYear: 2014,
      GSCode: null,
      RefNo: 0,
      UniqRowID: "",
      OpnWeight: 0.00
    };
    this.RefNo.nativeElement.focus();

  }
  SaveOpeningDetails(arg) {

  }
  Clear() {
    this.ListOpenDetModel =
    {
      ObjID: "",
      CompanyCode: "",
      BranchCode: "",
      PartyCode: "",
      VoucherCode: "VS",
      OpnBal: 0.00,
      BalType: null,
      WeightType: null,
      ObjectStatus: "O",
      UpdateOn: "",
      MetalCode: null,
      OpnPureWeight: 0.00,
      OpnNetWeight: 0.00,
      FinYear: 2014,
      GSCode: null,
      RefNo: 0,
      UniqRowID: "",
      OpnWeight: 0.00
    };
    this.RefNo.nativeElement.focus();

  }

  delete(i) {
    this.openingDetailsObj.splice(i);
    this.RefNo.nativeElement.focus();
  }
  Submit() {
    if (this.EmailIdValidations(this.BranchListData.Email) == false) {
      swal("Warning!", 'Please enter valid EmailID', "warning");
    }
    else if (this.EmailIdValidations(this.BranchListData.ContactEmail) == false) {

    }
    this.BranchListData.ListOpenDet = this.openingDetailsObj
  }
  EmailValidation(arg) {
    if (this.EmailIdValidations(arg) == false) {
      swal("Warning!", 'Please enter valid EmailID', "warning");
    }

  }
  EmailIdValidations(EmailID) {
    if (EmailID != null && EmailID != "") {
      const emailRegEx = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i;
      const validEmail = emailRegEx.test(EmailID);
      return validEmail;
    }
    else {
      return true;
    }
  }
  TINValidation(arg) {
    if (this.AlphaNumericValidations(arg) == false) {
      swal("Warning!", 'Please enter valid GSTTIN', "warning");
    }

  }
  AlphaNumericValidations(arg) {
    if (arg != null && arg != "") {
      const isPinRegEx = /^[a-zA-Z0-9]+$/g;
      const validPin = isPinRegEx.test(arg);
      return validPin;
    }
    else {
      return true;
    }
  }

}
