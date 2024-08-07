import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl, AbstractControl } from '@angular/forms';
import { MastersService } from '../masters.service';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { Alert } from 'bootstrap';
declare var $: any;
@Component({
  selector: 'app-ip-address-mngmt',
  templateUrl: './ip-address-mngmt.component.html',
  styleUrls: ['./ip-address-mngmt.component.css']
})
export class IpAddressMngmtComponent implements OnInit {
  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  ///
  filterform: FormGroup;
  AddIPform: FormGroup;
  Searchform: FormGroup;
  EditIPform: FormGroup;
  //serach from  filter
  userFilter: any = { FromIP: '' };
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  currentBtranch: any;
  constructor(private fb: FormBuilder, private _mastersService: MastersService, private _appConfigService: AppConfigService, private mastersService: MastersService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
    this.apidata;
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }
  AllowedOrDenied: any = [
    {
      "code": "Allow",
      "name": "ALLOW"
    },
    {
      "code": "Deny",
      "name": "DENY"
    },
    {
      "code": "All",
      "name": "ALL"
    }
  ];
  AllowedOrDeniedForEditAndAdd: any = [
    {
      "code": "Allow",
      "name": "ALLOW"
    },
    {
      "code": "Deny",
      "name": "DENY"
    }
  ];
  Enabled: any = [{
    "code": true,
    "name": "YES"
  },
  {
    "code": false,
    "name": "NO"
  }
  ]
  ipDataList = {
    ID: 0,
    AllowDeny: "All",
    FromIP: "",
    ToIP: "",
    BranchCode: "",
    Remarks: "",
    Active: null
  }
  AddipDataModel = {
    ID: 0,
    AllowDeny: null,
    FromIP: "",
    ToIP: "",
    BranchCode: null,
    Remarks: "",
    Active: false
  }
  EditIpDataModel = {
    ID: 0,
    AllowDeny: null,
    FromIP: "",
    ToIP: "",
    BranchCode: null,
    Remarks: "",
    Active: false
  }
  ngOnInit() {
    this.AllowedOrDenied;
    this.Enabled;
    this.getAllCompanybranchCode();
    this.filterform = this.fb.group({
      companyCode: this.ccode,
      branchCode: this.bcode,
      FrmCtrl_iptext: null,
      FrmCtrl_active: null,
      FrmCtrl_status: null
    });
    this.AddIPform = this.fb.group({
      FrmCtrl_fromIptext: null,
      FrmCtrl_ToIptext: null,
      FrmCtrl_allowOrDenied: null,
      FrmCtrl_branch: null,
      FrmCtrl_remarks: null
    });
    this.Searchform = this.fb.group({
      FrmCtrl_SerachText: null,
    });
    this.EditIPform = this.fb.group({
      FrmCtrl_fromIptext: null,
      FrmCtrl_ToIptext: null,
      FrmCtrl_allowOrDenied: null,
      FrmCtrl_branch: null,
      FrmCtrl_remarks: null
    });
  }
  AddnewIP() {
    this.AddIPform.reset();
    $('#NewIPAddModel').modal('show');
  }
  closeIPModel() {
    this.AddIPform.reset();
    $('#NewIPAddModel').modal('hide');
    this.AddipDataModel = {
      ID: 0,
      AllowDeny: null,
      FromIP: "",
      ToIP: "",
      BranchCode: null,
      Remarks: "",
      Active: false
    }
  }
  closEditIPAddModel() {
    this.apidata;
    this._mastersService.serachApiData(this.ipDataList.AllowDeny, this.ipDataList.Active).subscribe(
      response => {
        this.apidata = response
      })
    $('#EditIPAddModel').modal('hide');
    this.EditIpDataModel = {
      ID: 0,
      AllowDeny: null,
      FromIP: "",
      ToIP: "",
      BranchCode: null,
      Remarks: "",
      Active: false
    }
  }
  ClearSearch() {
    this.filterform.reset();
  }
  Clear(form) {
    this.AddIPform.reset();
    this.EditIPform.reset();
    this._mastersService.serachApiData(this.ipDataList.AllowDeny, this.ipDataList.Active).subscribe(
      response => {
        this.apidata = response;
      })
    this.EditIpDataModel = {
      ID: 0,
      AllowDeny: null,
      FromIP: "",
      ToIP: "",
      BranchCode: null,
      Remarks: "",
      Active: false
    }
  }
  allbranches: any = [];
  getAllCompanybranchCode() {
    let arg = localStorage.getItem('OperatorCode');
    this._mastersService.getAllCompanybranchCodes(arg).subscribe(
      response => {
        this.allbranches = response;
        if (this.allbranches.length === 1) {
          this.getDefaultBranch();
        }
        else {
          this.getDefaultBranch();
        }
      }
    )
  }
  getDefaultBranch() {
    let arg = localStorage.getItem('OperatorCode');
    this._mastersService.getBranchCompanys(arg).subscribe(
      response => {
        this.currentBtranch = response;
        this.allbranches.ccode = this.currentBtranch.CompanyCode;
        this.allbranches.bcode = this.currentBtranch.BranchCode;
      }
    )
  }
  apidata: any = [];
  getIPaddressList() {

    this._mastersService.getListOFip().subscribe(
      response => {
        this.apidata = response;
      })
  }
  getApibySearch(form) {
    if (this.ipDataList.AllowDeny == null || this.ipDataList.AllowDeny == "") {
      swal("Warning!", "Please slect Allowed/Denied ", "warning");
    }
    else if (this.ipDataList.Active == null) {
      swal("Warning!", "Please Select  Enabled ", "warning");
    }
    else {
      this._mastersService.serachApiData(this.ipDataList.AllowDeny, this.ipDataList.Active).subscribe(
        response => {
          this.apidata = response;
        })
    }

  }
  errors = [];
  addNeIPaddress(form) {
    if (form.value.FrmCtrl_fromIptext == null || form.value.FrmCtrl_fromIptext == "") {
      swal("Warning!", "Please enter From IP", "warning");
    }
    else if (form.value.FrmCtrl_ToIptext == null || form.value.FrmCtrl_ToIptext == "") {
      swal("Warning!", "Please enter To IP", "warning");
    }
    else if (this.AddipDataModel.AllowDeny == null || form.value.FrmCtrl_allowOrDenied == "") {
      swal("Warning!", "Please Select Allow Or Deny ", "warning");
    }
    else if (form.value.FrmCtrl_remarks == null || form.value.FrmCtrl_remarks == "") {
      swal("Warning!", "Please enter  Remarks", "warning");
    }
    else if (form.value.FrmCtrl_branch === null || form.value.FrmCtrl_branch == "") {
      swal("Warning!", "Please Select Branch", "warning");
    }
    else {
      var ans = confirm("Do you want to Add ??");
      if (ans) {
        this._mastersService.postNewIPData(this.AddipDataModel).subscribe(
          response => {
            swal("Saved!", "Saved " + this.AddipDataModel.FromIP + " Saved", "success");
            this.AddIPform.reset();
            $('#NewIPAddModel').modal('hide');
            this.apidata = [];
            this._mastersService.serachApiData(this.ipDataList.AllowDeny, this.ipDataList.Active).subscribe(
              response => {
                this.apidata = response;
              });
          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error.description;
              swal("Warning!", validationError, "warning");
            }
            else {
              this.errors.push('something went wrong!');
            }
          }
        )
      }
    }

  }
  editSelectedIp(arg) {
    this.EditIpDataModel = arg;
    $('#EditIPAddModel').modal('show');
    // if (arg.Active == false) {
    //   swal("Warning!", "The: " + arg.FromIP + " is already Denied Please do Enable to edit", "warning");
    // }
    // else {
    //   this.EditIpDataModel = arg;
    //   $('#EditIPAddModel').modal('show');
    // }
  }
  IPadressValidations(arg) {
    var value = arg.target.value;
    if (value != null && value != "") {
      const IpaddressREGEX = /\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b/;
      const validIP = IpaddressREGEX.test(value);
      if (validIP == false) {
        swal("!Warning", "IP ADDRESS IS NOT VALID", "warning");
        return false;
      }
      else {
        return validIP;
      }
      return false;
    }
    else {
      return true;
    }
  }
  openIP(arg) {
    this.EditIpDataModel = arg;
    if (this.EditIpDataModel.Active == true) {
      swal("Warning!", "IP address is: " + this.EditIpDataModel.FromIP + " is already Enabled", "warning");
    }
    else {
      var ans = confirm("Do you want to Open IP adddress: " + arg.FromIP + "?");
      if (ans) {
        this.EditIpDataModel.Active = true;
        this.mastersService.editIPdetails(this.EditIpDataModel).subscribe(
          response => {
            swal("Updated!", "The IP Address: " + arg.FromIP + "Enabled", "success");
            this._mastersService.serachApiData(this.ipDataList.AllowDeny, this.ipDataList.Active).subscribe(
              response => {
                this.apidata = response;

              })
            this.EditIpDataModel = {
              ID: 0,
              AllowDeny: null,
              FromIP: "",
              ToIP: "",
              BranchCode: null,
              Remarks: "",
              Active: false
            }
          }
        )
      }
    }
  }
  closeIP(arg) {
    this.EditIpDataModel = arg;
    if (this.EditIpDataModel.Active == false) {
      swal("Warning!", "IP address is: " + this.EditIpDataModel.FromIP + " is already Disabled", "warning");
    }
    else {
      var ans = confirm("Do you want to close the IP Address: " + arg.FromIP + "?");
      if (ans) {
        this.EditIpDataModel.Active = false;
        this.mastersService.editIPdetails(this.EditIpDataModel).subscribe(
          response => {
            swal("Updated!", "The IP Address: " + arg.FromIP + "Denied", "success");
            this._mastersService.serachApiData(this.ipDataList.AllowDeny, this.ipDataList.Active).subscribe(
              response => {
                this.apidata = response;

              })
            this.EditIpDataModel = {
              ID: 0,
              AllowDeny: null,
              FromIP: "",
              ToIP: "",
              BranchCode: null,
              Remarks: "",
              Active: false
            }
          }
        )
      }
    }
  }
  EditAnDSave(form) {
    if (this.EditIpDataModel.FromIP == null || this.EditIpDataModel.FromIP == "") {
      swal("Warning!", "Please enter  From IP Address", "warning");
    }
    else if (this.EditIpDataModel.ToIP == null || this.EditIpDataModel.ToIP == "") {
      swal("Warning!", "Please enter To IP Address ", "warning");
    }
    else if (this.EditIpDataModel.AllowDeny == null || this.EditIpDataModel.AllowDeny == "") {
      swal("Warning!", "Please Select  AllowDeny ", "warning");
    }
    else if (this.EditIpDataModel.Remarks == null || this.EditIpDataModel.Remarks === "") {
      swal("Warning!", "Please enter Remarks ", "warning");
    }
    else if (this.EditIpDataModel.BranchCode == null || this.EditIpDataModel.BranchCode == "") {
      swal("Warning!", "Please Select  Brnach ", "warning");
    }
    else {
      var ans = confirm("Do you want to Update??");
      if (ans) {
        this.mastersService.editIPdetails(this.EditIpDataModel).subscribe(
          response => {
            swal("Updated!", "Saved " + this.EditIpDataModel.FromIP + " Saved", "success");
            this.EditIPform.reset();
            $('#EditIPAddModel').modal('hide');
            this._mastersService.serachApiData(this.ipDataList.AllowDeny, this.ipDataList.Active).subscribe(
              response => {
                this.apidata = response;
              });

          }

        )
      }
    }
  }

}
