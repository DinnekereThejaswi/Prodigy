import { Component, OnInit } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';
import { AccountsService } from '../accounts.service';
import { VendorModel, ListOpenDet } from '../accounts.model'
declare var $: any;

@Component({
  selector: 'app-vendor-master',
  templateUrl: './vendor-master.component.html',
  styleUrls: ['./vendor-master.component.css']
})
export class VendorMasterComponent implements OnInit {
  /////
  OpeningDetailsHeaderForm: FormGroup;
  GroupToForm: FormGroup;
  TermsAndConditnsForm: FormGroup;
  SupplierDetailsForm: FormGroup;
  VendorSearchform: FormGroup;
  ////////////////////////////////////////
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  isReadOnly: boolean = false;
  ////////
  datePickerConfig: Partial<BsDatepickerConfig>;
  ////
  EnableSupplierDetails: boolean = true;
  EnableBankDetails: boolean = true;
  EnableOpeningDetails: boolean = true;
  EnableGroups: boolean = true;
  EnableTermsAndCondtns: boolean = true;
  /////
  EnableJson: boolean = false;
  ccode: string;
  bcode: string;
  password: string;
  ///
  today = new Date();
  applicationDate: string;
  vendorListData: any = {
    ObjID: "",
    CompanyCode: "",
    BranchCode: "",
    PartyCode: "",
    PartyName: "",
    VoucherCode: "VS",
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
    StateCode: 0,
    SchemeType: null,
    SupplierMetal: null,
    UniqRowID: "",
    ListGroupTo: [],
    ListOpenDet: [],
  }

  ListGroupToModel =
    {
      ObjID: "",
      CompanyCode: "",
      BranchCode: "",
      PartyCode: "",
      VoucherCode: "",
      IRCode: "",
      GroupName: "",
      UpdateOn: "",
      UniqRowID: "",
    }
  ListOpenDetModel: ListOpenDet =
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
  schemes = [{ "code": "Y", "name": "Yes" },
  { "code": "N", "name": "No" }];
  ///searching
  userFilter: any = { PartyName: '', Mobile: '', Phone: '' };
  /////////
  constructor(private _appConfigService: AppConfigService,
    private _acountsService: AccountsService, private fb: FormBuilder) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);

  }
  config = {
    displayKey: "IRName", //if objects array passed which key to be displayed defaults to description
    search: false,//true/false for the search functionlity defaults to false,
    height: 'auto', //height of the list so that if there are more no of items it can show a scroll defaults to auto. With auto height scroll will never appear
    placeholder: 'Select Gropus', // text to be displayed when no item is selected defaults to Select,
    customComparator: () => { }, // a custom function using which user wants to sort the items. default is undefined and Array.sort() will be used in that case,
    limitTo: 0, // number thats limits the no of options displayed in the UI (if zero, options will not be limited)
    moreText: 'more', // text to be displayed whenmore than one items are selected like Option 1 + 5 more
    noResultsFound: 'No results found!',// text to be displayed when no items are found while searching
    searchPlaceholder: 'Search Gropus', // label thats displayed in search input,
    searchOnKey: 'name', // key on which search should be performed this will be selective search. if undefined this will be extensive search on all keys
    clearOnSelection: false, // clears search criteria when an option is selected if set to true, default is false
    inputDirection: 'ltr', // the direction of the search input can be rtl or ltr(default)
  }
  SelectedCounters: {};
  ngOnInit() {
    this.vendorListData.CompanyCode = this.ccode;
    this.vendorListData.BranchCode = this.bcode;
    this.uom;
    this.DebitOrCredit;
    this.getStateCode();
    this.vendorTDS();
    this.vendorGSTypes();
    this.opentype();
    this.vendorMetalTypes();
    this.vendorGroups();
    /////////////////////
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
      frmCtrl_International: null, frmCtrl_Local: null, frmCtrl_InterState: null,
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
    this.GroupToForm = this.fb.group({
      frmCtrl_GroupOptions: null,
    });
    this.TermsAndConditnsForm = this.fb.group({
      frmCtrl_creditPeriod: null,
      frmCtrl_LeadTime: null,
      frmCtrl_maxPaymnt: null,
      frmCtrl_ConvRate: null,
      frmCtrl_creditLimit: null


    });
    this.VendorSearchform = this.fb.group({
      companyCode: this.ccode,
      branchCode: this.bcode,
      FrmCtrl_SerachText: null,
    })
  }
  //add new record to opening details
  modifyModel() {
    // this.GsEntryFrom.reset();
    $('#SupplierSearchModel').modal('show');
    this.EnableAdd = false;
    this.loadvendors();
  }

  LoadAllVendors: any = [];
  loadvendors() {
    this._acountsService.getAllVendor().subscribe(
      Response => {
        this.LoadAllVendors = Response;
        //.log(this.LoadAllVendors);
      }
    )
  }
  PrintDetails: any = [];
  printAllVendors(arg) {
    //alert('printAllVendors');
    // alert(arg.ObjID);
    this._acountsService.printVendorDetails(arg.ObjID).subscribe(
      Response => {
        this.PrintDetails = Response;
        // console.log(this.PrintDetails);
        this.PrintDetails = atob(this.PrintDetails.Data);
        $('#PrintVendorModal').modal('show');
        $('#DisplayData').html(this.PrintDetails);
      }
    )
  }
  LoadBackAllVendors(arg) {
    this.vendorListData = arg;
    this.EnableAdd = false;
    this.EnableSave = true;


  }
  CalculatePureWt(arg) {
    this.ListOpenDetModel.OpnPureWeight = null;
    this.ListOpenDetModel.OpnPureWeight = Number(this.ListOpenDetModel.OpnWeight) - Number(arg);
  }

  ValidateGSTIN(arg){
    var reggst = /^([0-9]){2}([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}([0-9]){1}([a-zA-Z]){1}([0-9]){1}?$/;
if(!reggst.test(arg) && arg!=''){
  swal("Warning!","GST Identification Number is not valid. It should be in this"+ "11AAAAA1111Z1A1" +"format" ,"warning");
}
  }
  print() {
    let printContents, popupWin;
    printContents = document.getElementById('DisplayData').innerHTML;
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    popupWin.document.open();
    popupWin.document.write(`
      <html>
        <head>
          <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css" integrity="sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm" crossorigin="anonymous">
          <title>Print tab</title>
          <style>
         .htmlPrint{display:none;}

          @media print {
            .table-bordered
        {
            border-style: solid;
            border: 3px solid rgb(0, 0, 0);
        }
        .margin{
          margin-top:-24px;
        }
        .mb{
          vertical-align:middle;position:absolute;
        }
        tr.spaceUnder>td {
          padding-bottom: 40px !important;
        }
        .top {
          margin-top:10px;
          border-bottom: 3px solid rgb(0, 0, 0) !important;
          line-height: 1.6;
        }
        .modal-content{
          font-family: "Times New Roman", Times, serif;

        }
       .padding-top {
         padding-top:20px;
       }
        .watermark{
          -webkit-transform: rotate(331deg);
          -moz-transform: rotate(331deg);
          -o-transform: rotate(331deg);
          transform: rotate(331deg);
          font-size: 15em;
          color: rgba(255, 5, 5, 0.37);
          position: absolute;
          text-transform:uppercase;
          padding-left: 10%;
          margin-top:-10px;
        }
        .right{
          text-align:left;
        }
        .px-2 {
          padding-left: 10px !important;
        }
        thead > tr
        {
            border-style: solid;
            border: 3px solid rgb(0, 0, 0);
        }
        table tr td.classname{
          border-right: 3px solid rgb(0, 0, 0) !important;
        }
        table tr td.borderLeft{
          border-left: 3px solid rgb(0, 0, 0) !important;
        }
        table tr td.bottom{
          border-bottom: 3px solid rgb(0, 0, 0) !important;
        }
        table tr th.classname{
          border-right: 3px solid rgb(0, 0, 0) !important;
        }
        .divborder {
          border-right: 3px solid rgb(0, 0, 0) !important;
          line-height: 1.6;
        }
        .lastdivborder{
          line-height: 1.6;
        }
    .border{
      border-right: 3px solid rgb(0, 0, 0) !important;
      border-bottom: 3px solid rgb(0, 0, 0) !important;
    }

    .tdborder{
      border-right: 3px solid rgb(0, 0, 0) !important;
    }
        .invoice {
          margin-top: 250px;
      }
        .card{
          border-style: solid;
          border-width: 5px;
          border: 3px solid rgb(0, 0, 0);
      }
        .printMe {
          display: none !important;
        }
      }
      body{
            font-size: 15px;
            line-height: 18px;
      }
 </style>
        </head>
    <body onload="window.print();window.close()">

    ${printContents}</body>
      </html>`
    );
    popupWin.document.close();
  }
  Submit() {
    // alert('final submit');
    if (this.vendorListData.PartyCode == "" || this.vendorListData.PartyCode == null) {
      swal("Warning!", "Please Enter Code", "warning");
    }
    else if (this.vendorListData.TIN == "" || this.vendorListData.TIN == null) {
      swal("Warning!", "Please Enter  GSTIN", "warning");
    }
    else if (this.vendorListData.PartyName == "" || this.vendorListData.PartyName == null) {
      swal("Warning!", "Please Enter  GS Name", "warning");
    }
    else if (this.vendorListData.PAN == "" || this.vendorListData.PAN == null) {
      swal("Warning!", "Please Enter  PAN", "warning");
    }
    ///bank details
    else if (this.vendorListData.PartyAccountNo == "" || this.vendorListData.PartyAccountNo == null) {
      swal("Warning!", "Please Enter  Account Name", "warning");
    }
    else if (this.vendorListData.PartyBankName == "" || this.vendorListData.PartyBankName == null) {
      swal("Warning!", "Please Enter  BankName Name", "warning");
    }
    else {
      var ans = confirm("Do you want to save??");
      if (ans) {

        this._acountsService.postVendorDetails(this.vendorListData).subscribe(
          response => {
            swal("Saved!", "Venor", " Saved", "success");
            // this.getSubGroupList();
            // this.ClearValues();
          }
        )
      }
    }

  }
  openingDetailsObj: any = [];
  AddOpeningDetails(arg) {

    //console.log(this.ListOpenDetModel);
    this.ListOpenDetModel.CompanyCode = this.ccode;
    this.ListOpenDetModel.BranchCode = this.bcode;
    // this.ListOpenDetModel.PartyCode = this.vendorListData.PartyCode;
    // this.OpeningDetailsHeaderForm.reset();

    if (arg.GSCode == null || arg.GSCode == "") {
      swal("Warning!", "Please Select  GS Type", "warning");
    }
    else if (arg.WeightType == null || arg.WeightType == "") {
      swal("Warning!", "Please Select  Weight Type", "warning");
    }

    else if (arg.OpnWeight == null || arg.OpnWeight == Number("")) {
      swal("Warning!", "Please Enter  Open Weight", "warning");
    }
    else if (arg.OpnNetWeight == null || arg.OpnNetWeight == Number("")) {
      swal("Warning!", "Please Enter  Open Net Weight", "warning");
    }
    else if (arg.BalType == null || arg.BalType == "") {
      swal("Warning!", "Please Select Balance Type", "warning");
    }
    else {
      this.vendorListData.ListOpenDet.push(arg)
      this.Clear();
    }
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
    }
  }


  selectedstate: any;
  stateDetails(index) {
    var slecetIndex = index - 1;
    this.selectedstatehandleChange(slecetIndex);
  }
  selectedstatehandleChange(slecetIndex) {
    this.selectedstate = this.StateList[slecetIndex];
    // console.log(this.selectedstate);

    this.vendorListData.State = this.selectedstate.StateName;
    this.vendorListData.StateCode = this.selectedstate.TINNo;
    this.vendorListData.StateStatus == this.selectedstate.ObjectStatus;

  }
  checkStatus(arg) {
    // alert(arg)
    // alert(arg.target.value)
  }
  selectedTDSValue: any;
  TDSDropdownChange(index) {
    var slecetIndex = index - 1;
    this.handleChange(slecetIndex);
  }
  handleChange(slecetIndex) {
    // console.log(this.LedgerTypes[index]);
    this.selectedTDSValue = this.TDSVendors[slecetIndex];
    // console.log(this.selectedValue);
    this.vendorListData.TDSPercent = this.selectedTDSValue.tds;
    // this.vendorListData.TDS = this.selectedTDSValue.tds_name;
    this.vendorListData.TDS = "N";
    this.vendorListData.TDSId = this.selectedTDSValue.tdsID;
  }
  listarry: any = {};
  ListOfGroupTo(option) {
    // console.log(option);
    this.ListGroupToModel =
    {
      ObjID: "",
      CompanyCode: "",
      BranchCode: "",
      PartyCode: "",
      VoucherCode: "",
      IRCode: "",
      GroupName: "",
      UpdateOn: "",
      UniqRowID: "",
    }
    this.ListGroupToModel.CompanyCode = this.ccode;
    this.ListGroupToModel.BranchCode = this.bcode;
    this.ListGroupToModel.PartyCode = this.vendorListData.PartyCode;
    this.ListGroupToModel.VoucherCode = option.VoucherCode;
    this.ListGroupToModel.IRCode = option.IRCode;
    this.ListGroupToModel.GroupName = option.IRName;
    this.vendorListData.ListGroupTo.push(this.ListGroupToModel);
  }
  GroupsTo: any = [];
  vendorGroups() {
    this._acountsService.GetVendorGroups().subscribe(
      Response => {
        this.GroupsTo = Response;
        // console.log(this.GroupsTo);
      }
    )
  }

  t: boolean = false;
  check(arg) {


    for (let i = 0; i < this.GroupsTo.length; i++) {
      for (let j = 0; j < this.vendorListData.ListGroupTo.length; j++) {
        if (this.GroupsTo[i].IRCode == this.vendorListData.ListGroupTo[j].IRCode) {
          this.t = true;
          break;

        }
        else {
          return false;
        }
      }
    }


    // this.assignMaster.Code = this.StoreList[i].Code;
    // this.assignMaster.Name = this.StoreList[i].Name;
    // this.assignMaster.isChecked = false;
    // this.MainDropdownMaster.push(this.assignMaster);
    // this.assignMaster = {
    //   Code: "",
    //   Name: "",
    //   isChecked: false
    // }

  }
  metals: any = [];
  vendorMetalTypes() {
    this._acountsService.getVendorMetalList().subscribe(
      Response => {
        this.metals = Response;
      }
    )
  }
  TDSVendors: any = [];
  vendorTDS() {
    this._acountsService.GetVendorTDS().subscribe(
      Response => {
        this.TDSVendors = Response;
      }
    )
  }
  GSTypes: any = [];
  vendorGSTypes() {
    this._acountsService.getVendorGSTypeList().subscribe(
      Response => {
        this.GSTypes = Response;
      }
    )
  }
  openTypes: any = [];
  opentype() {
    this._acountsService.GetVendorOpenTypeList().subscribe(
      Response => {
        this.openTypes = Response;
      }
    )
  }

  ToggleSupplierDetails() {
    this.EnableSupplierDetails = !this.EnableSupplierDetails;
  }
  ToggleBankDetails() {
    this.EnableBankDetails = !this.EnableBankDetails;
  }
  ToggleOpeningDetails() {
    this.EnableOpeningDetails = !this.EnableOpeningDetails;
  }
  ToggleGroups() {
    this.EnableGroups = !this.EnableGroups;
  }
  ToggleTermsAndCondtns() {

    this.EnableTermsAndCondtns = !this.EnableTermsAndCondtns;
  }
  StateList: any;
  getStateCode() {
    this._acountsService.getStateCode().subscribe(
      Response => {
        this.StateList = Response;
      }
    );
  }


}
