import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl, AbstractControl, FormArray } from '@angular/forms';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { UtilitiesService } from '../utilities.service';
import { Alert } from 'bootstrap';
import { ThrowStmt } from '@angular/compiler';
import { jsonpFactory } from '@angular/http/src/http_module';
declare var $: any;
@Component({
  selector: 'app-new-operator',
  templateUrl: './new-operator.component.html',
  styleUrls: ['./new-operator.component.css']
})
export class NewOperatorComponent implements OnInit {
  @ViewChild('OperatorCode', { static: true }) OperatorCode: ElementRef;
  @ViewChild('ConfirmPassword', { static: true }) ConfirmPassword: ElementRef; OperatorSearchform: FormGroup;
  Operatorform: FormGroup;
  changePasswordform: FormGroup;
  ccode: string;
  bcode: string;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  CurrentBranch: any;

  ////////////search filter
  public searchText: string;
  //////////////////
  password: string;
  EnableJson: boolean = false;
  OperatorType = [{
    "Code": "Others",
    "Name": "Others"
  },
  {
    "Code": "Admin",
    "Name": "Admin"
  }];
  OperatorsPostModel: any = {
    OperatorCode: "",
    OperatorName: "",
    OperatorType: null,
    MobileNo: "",
    BranchCode: "",
    CompanyCode: "",
    EmployeeID: null,
    Password: "",
    RoleID: null,
    Status: "",
    MaxDiscountPercentAllowed: 0,
    CounterCode: "",
    DefaultStore: "",
    MappedStores: []
  };
  IsCurrentBranch: boolean = true;
  EditBranches: boolean = false;
  AddBranches: boolean = true;
  readonly: boolean = false;
  confirmPassword: any;
  editconfirmPassword: any;
  constructor(private fb: FormBuilder, private _utilitiesService: UtilitiesService, private _appConfigService: AppConfigService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();

  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this.OperatorsPostModel.CompanyCode = this.ccode;
    this.OperatorsPostModel.BranchCode = this.bcode;
    this.OperatorsPostModel.DefaultStore = this.bcode;
    this.LoadAllOperator();
    this.GetAllCounter();
    // this.getAllCompanybranchCode();
    this.getRoleslist();
    this.currentBranch = this.bcode;
    this.Operatorform = this.fb.group({
      frmCtrl_OpCode: null,
      frmCtrl_OpName: null,
      frmCtrl_EmpNo: null,
      frmCtrl_Password: null,
      frmCtrl_confirmPassword: null,
      frmCtrl_Oprole: null,
      frmCtrl_discountPercent: null,
      frmCtrl_Opmobile: null,
      frmCtrl_OpCounter: null,
      frmCtrl_OpType: null,
      frmCtrl_OpDefltBranch: [null],
      frmCtrl_OpDefltBranchList: null,
    });
    this.OperatorSearchform = this.fb.group({
      companyCode: this.ccode,
      branchCode: this.bcode,
      FrmCtrl_SerachText: null,
    });
    this.changePasswordform = this.fb.group({
      companyCode: this.ccode,
      branchCode: this.bcode,
      FrmCtrl_OperatorCode: null,
      FrmCtrl_OldPassword: null,
      FrmCtrl_NewPassword: null,
    });
  }

  allbranches: any = [];
  getAllCompanybranchCode() {
    let arg = localStorage.getItem('OperatorCode');
    this._utilitiesService.getAllCompanybranchCodes(arg).subscribe(
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
  currentBranch: any;
  getDefaultBranch() {
    let arg = localStorage.getItem('OperatorCode');
    this._utilitiesService.getBranchCompanys(arg).subscribe(
      response => {
        this.currentBranch = response;
        this.allbranches.ccode = this.currentBranch.CompanyCode;
        this.allbranches.bcode = this.currentBranch.BranchCode;
      }
    )
  }
  AddNewOperatorModel() {

    this.EditBranches = false;
    this.AddBranches = true;
    this.EnableSave = false;
    this.EnableAdd = true;
    this.readonly = false;
    this.allbranches = [];
    this.OperatorsPostModel = {
      OperatorCode: "",
      OperatorName: "",
      OperatorType: "",
      MobileNo: "",
      BranchCode: "",
      CompanyCode: "",
      EmployeeID: null,
      Password: null,
      RoleID: null,
      Status: "",
      MaxDiscountPercentAllowed: 0,
      CounterCode: "",
      DefaultStore: "",
      MappedStores: []
    };

    this.StoreList = [];
    this._utilitiesService.GetAllBranchesList().subscribe(
      Response => {
        this.StoreList = Response;
        if (this.StoreList.length > 0) {
          const objIndex = this.StoreList.findIndex(obj => obj.Code === this.bcode);
          if (objIndex > -1) {
            this.StoreList.splice(objIndex, 1);
          }
        }
      }
    )
    this.allbranches.push({ Code: this.bcode, Name: this.bcode });
    this.OperatorsPostModel.CounterCode = "ALL";
    this.OperatorsPostModel.OperatorType = "Others";
    this.OperatorsPostModel.DefaultStore = this.currentBranch;
    this.OperatorsPostModel.MappedStores.push({ Code: this.bcode, Name: this.bcode });
    $('#AddNewOperatorsModel').modal('show');
    this.OperatorCode.nativeElement.focus();
  }
  AllOperatorsList: any = [];
  getRoleNames: any = [];
  LoadAllOperator() {
    this._utilitiesService.getOperatorsDetails().subscribe(
      response => {
        this.AllOperatorsList = response;
        for (let i = 0; i < this.AllOperatorsList.length; i++) {
          this.AllOperatorsList[i].RoleID;
          this.AllOperatorsList[i].OperatorType = "";
          this._utilitiesService.getRoleNames(this.AllOperatorsList[i].RoleID).subscribe(
            Response => {
              this.getRoleNames = Response;
              this.AllOperatorsList[i].OperatorType = this.getRoleNames.Name
            }
          )
        }
      }
    )
  }

  itempush: any = {
    CounterCode: "ALL",
    CounterName: "ALL"
  }
  counterList: any = [];
  GetAllCounter() {
    this._utilitiesService.getCounterList().subscribe(
      Response => {
        this.counterList = Response;
        this.counterList.push(this.itempush);
      })
  }
  Roleslist: any = [];
  getRoleslist() {
    this._utilitiesService.rolesList().subscribe(
      Response => {
        this.Roleslist = Response;
        this.Roleslist = this.Roleslist.filter(p => p.Status === 'Active');
      })
  }
  mappedBranchesofOperator: any = [];
  StoreList: any = [];
  getStores() {
    this._utilitiesService.GetAllBranchesList().subscribe(
      Response => {
        this.StoreList = Response;
      }
    )
  }
  assignMaster: any = {
    Code: "",
    Name: "",
    isChecked: false
  }
  abc: any = []
  MainDropdownMaster: any = [];
  Edit(arg) {
    this.LoadAllOperator();
    this.OperatorsPostModel = {
      OperatorCode: "",
      OperatorName: "",
      OperatorType: "",
      MobileNo: "",
      BranchCode: "",
      CompanyCode: "",
      EmployeeID: null,
      Password: null,
      RoleID: null,
      Status: "",
      MaxDiscountPercentAllowed: 0,
      CounterCode: "",
      DefaultStore: "",
      MappedStores: []
    };

    this.EditBranches = true;
    this.AddBranches = false;
    this.EnableAdd = false;
    this.EnableSave = true;
    this.StoreList = [];
    this.MainDropdownMaster = [];
    this.allbranches = [];
    if (arg.CounterCode === "ALL") {
      this.OperatorsPostModel.CounterCode = "ALL";
    }
    this.OperatorsPostModel = arg;
    this.allbranches = this.OperatorsPostModel.MappedStores;
    this._utilitiesService.GetAllBranchesList().subscribe(
      Response => {
        this.StoreList = Response;
        for (let i = 0; i < this.StoreList.length; i++) {
          this.assignMaster.Code = this.StoreList[i].Code;
          this.assignMaster.Name = this.StoreList[i].Name;
          this.assignMaster.isChecked = false;
          this.MainDropdownMaster.push(this.assignMaster);
          this.assignMaster = {
            Code: "",
            Name: "",
            isChecked: false
          }
        }
        for (let i = 0; i < this.MainDropdownMaster.length; i++) {
          for (let j = 0; j < this.OperatorsPostModel.MappedStores.length; j++) {
            if (this.MainDropdownMaster[i].Code == this.OperatorsPostModel.MappedStores[j].Code) {
              this.MainDropdownMaster[i].isChecked = true;
              break;
            }
            else {
              this.MainDropdownMaster[i].isChecked = false;
            }
          }
        }
      }
    )
    $('#AddNewOperatorsModel').modal('show');
    this.OperatorCode.nativeElement.focus();
  }
  LoadSelectedBranches(option, event, index) {
    if (event.target.checked) {
      this.OperatorsPostModel.MappedStores.push(option);
      this.allbranches.push(option)
    }
    else {
      for (let j = 0; j < this.OperatorsPostModel.MappedStores.length; j++) {
        if (option.Code == this.OperatorsPostModel.MappedStores[j].Code) {
          this.OperatorsPostModel.MappedStores.splice(j, 1);
          this.allbranches.splice(j, 1);
          break;
        }
      }
    }
  }
  onCheckboxChange(option, event, index) {
    if (event.target.checked) {
      this.MainDropdownMaster[index].isChecked = true;
      this.OperatorsPostModel.MappedStores.push(option);

    }
    else {
      for (let j = 0; j < this.OperatorsPostModel.MappedStores.length; j++) {
        if (option.Code == this.OperatorsPostModel.MappedStores[j].Code) {
          this.OperatorsPostModel.MappedStores.splice(j, 1);
          break;
        }
      }
    }
  }
  getStatusColor(Status) {
    switch (Status) {
      case 'Active':
        return 'green';

      case 'Closed':
        return 'red';

    }
  }
  ValidatePassword(arg) {
    if (this.confirmPassword != null) {
      if (arg !== this.OperatorsPostModel.Password) {
        swal("Warning!", "Confirm Password is incorrect", "warning");
        this.confirmPassword = "";
        this.ConfirmPassword.nativeElement.focus();
      }
      else if (arg == this.OperatorsPostModel.Password) {
        this.checkPassword(arg);
      }

    }
  }

  checkPassword(str) {
    var regix = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{8,})");
    if (regix.test(str) == false) {
    swal("Warning!","password must be a minimum of 8 characters including number, Upper, Lower And  one special character", "warning");
    this.confirmPassword = "";
    this.OperatorsPostModel.Password="";
    this.ConfirmPassword.nativeElement.focus();
    }
   
  }
  errors: any = [];
  addNewOperator(form) {
    if (form.value.frmCtrl_OpCode == null || form.value.frmCtrl_OpCode == "") {
      swal("Warning!", "Please Enter Operator Code", "warning");
    }
    else if (form.value.frmCtrl_OpName == null || form.value.frmCtrl_OpName == "") {
      swal("Warning!", "Please enter Operator Name", "warning");
    }
    else if (form.value.frmCtrl_EmpNo == null || form.value.frmCtrl_EmpNo == "") {
      swal("Warning!", "Please enter Employee Number", "warning");
    }
    else if (form.value.frmCtrl_Password == null || form.value.frmCtrl_Password == "") {
      swal("Warning!", "Please enter Password", "warning");
    }
    else if (form.value.frmCtrl_confirmPassword == null || form.value.frmCtrl_confirmPassword == "") {
      swal("Warning!", "Please enter  confirm Password", "warning");
    }
    else if (form.value.frmCtrl_Oprole == null || form.value.frmCtrl_Oprole == "") {
      swal("Warning!", "Please Select Opearator Role", "warning");
    }
    else if (form.value.frmCtrl_OpCounter == null || form.value.frmCtrl_OpCounter == "") {
      swal("Warning!", "Please Select counter", "warning");
    }
    else {
      var ans = confirm("Do you want to Add New Operator " + this.OperatorsPostModel.OperatorName + "? ");
      if (ans) {
        this.OperatorsPostModel.CompanyCode = this.ccode;
        this.OperatorsPostModel.BranchCode = this.bcode;
        this.OperatorsPostModel.Status = "Active";
        this.OperatorsPostModel.Password = btoa(this.confirmPassword);
        this._utilitiesService.PostOperatorsDetails(this.OperatorsPostModel).subscribe(
          response => {
            this.errors = response;
            swal("Saved!", "Saved " + this.OperatorsPostModel.OperatorName + " Saved", "success");
            this.OperatorsPostModel = {
              OperatorCode: "",
              OperatorName: "",
              OperatorType: "",
              MobileNo: "",
              BranchCode: "",
              CompanyCode: "",
              EmployeeID: null,
              Password: null,
              RoleID: null,
              Status: "",
              MaxDiscountPercentAllowed: 0,
              CounterCode: "",
              DefaultStore: "",
              MappedStores: []
            };
            this.OperatorsPostModel.CompanyCode = this.ccode;
            this.OperatorsPostModel.BranchCode = this.bcode;
            this.OperatorsPostModel.CounterCode = "ALL";
            this.OperatorsPostModel.DefaultStore = String(this.bcode);
            this.OperatorsPostModel.Password = null;
            this.confirmPassword = null;
            this.OperatorsPostModel.OperatorType = "Others";
            this.EditBranches = false;
            this.AddBranches = true;
            this.EnableSave = false;
            this.EnableAdd = true;
            this.readonly = false;
            this.confirmPassword = null;
            this.allbranches = [];
            this.StoreList = [];
            this.allbranches.push({ Code: this.bcode, Name: this.bcode });
            this.OperatorsPostModel.MappedStores.push({ Code: this.bcode, Name: this.bcode })
            this.OperatorsPostModel.DefaultStore = String(this.bcode);
            this.getStores();
            this.LoadAllOperator();
            this.GetAllCounter();
            this.getRoleslist();
            this.OperatorCode.nativeElement.focus();
          },
          (err) => {
            if (err.status === 400) {
              this.OperatorsPostModel.Password = atob(this.OperatorsPostModel.Password);
              const validationError = err.error;
              swal("Warning!", validationError, "warning");
              this.OperatorsPostModel = {
                OperatorCode: "",
                OperatorName: "",
                OperatorType: "",
                MobileNo: "",
                BranchCode: "",
                CompanyCode: "",
                EmployeeID: null,
                Password: null,
                RoleID: null,
                Status: "",
                MaxDiscountPercentAllowed: 0,
                CounterCode: "",
                DefaultStore: null,
                MappedStores: []
              };
              this.OperatorsPostModel.CompanyCode = this.ccode;
              this.OperatorsPostModel.BranchCode = this.bcode;
              this.OperatorsPostModel.CounterCode = "ALL";
              this.OperatorsPostModel.DefaultStore = String(this.bcode);
              this.OperatorsPostModel.Password = null;
              this.confirmPassword = null;
              this.OperatorsPostModel.OperatorType = "Others";
              this.EditBranches = false;
              this.AddBranches = true;
              this.EnableSave = false;
              this.EnableAdd = true;
              this.readonly = false;
              this.confirmPassword = null;
              this.allbranches = [];
              this.StoreList = [];
              this.allbranches.push({ Code: this.bcode, Name: this.bcode });
              this.OperatorsPostModel.MappedStores.push({ Code: this.bcode, Name: this.bcode })
              this.OperatorsPostModel.DefaultStore = String(this.bcode);
              this.getStores();
              this.LoadAllOperator();
              this.GetAllCounter();
              this.getRoleslist();
              this.OperatorCode.nativeElement.focus();
            }
            else {
              this.errors.push('something went wrong!');
              this.OperatorsPostModel = {
                OperatorCode: "",
                OperatorName: "",
                OperatorType: "",
                MobileNo: "",
                BranchCode: "",
                CompanyCode: "",
                EmployeeID: null,
                Password: null,
                RoleID: null,
                Status: "",
                MaxDiscountPercentAllowed: 0,
                CounterCode: "",
                DefaultStore: "",
                MappedStores: []
              };
              this.OperatorsPostModel.CompanyCode = this.ccode;
              this.OperatorsPostModel.BranchCode = this.bcode;
              this.OperatorsPostModel.CounterCode = "ALL";
              this.OperatorsPostModel.DefaultStore = String(this.bcode);
              this.OperatorsPostModel.Password = null;
              this.confirmPassword = null;
              this.OperatorsPostModel.OperatorType = "Others";
              this.EditBranches = false;
              this.AddBranches = true;
              this.EnableSave = false;
              this.EnableAdd = true;
              this.readonly = false;
              this.confirmPassword = null;
              this.allbranches = [];
              this.StoreList = [];
              this.allbranches.push({ Code: this.bcode, Name: this.bcode });
              this.OperatorsPostModel.MappedStores.push({ Code: this.bcode, Name: this.bcode })
              this.OperatorsPostModel.DefaultStore = String(this.bcode);
              this.getStores();
              this.LoadAllOperator();
              this.GetAllCounter();
              this.getRoleslist();
              this.OperatorCode.nativeElement.focus();
            }
          }
        )
      }
    }
  }
  save(form) {
    if (form.value.frmCtrl_OpCode == null || form.value.frmCtrl_OpCode == "") {
      swal("Warning!", "Please Enter Operator Code", "warning");
    }
    else if (form.value.frmCtrl_OpName == null || form.value.frmCtrl_OpName == "") {
      swal("Warning!", "Please enter Operator Name", "warning");
    }
    else if (form.value.frmCtrl_EmpNo == null || form.value.frmCtrl_EmpNo == "") {
      swal("Warning!", "Please enter Employee Number", "warning");
    }
    else if (form.value.frmCtrl_Oprole == null || form.value.frmCtrl_Oprole == "") {
      swal("Warning!", "Please Select Opearator Role", "warning");
    }
    else if (form.value.frmCtrl_OpCounter == null || form.value.frmCtrl_OpCounter == "") {
      swal("Warning!", "Please Select counter", "warning");
    }
    else {
      this.OperatorsPostModel.Status = "Active";
      this.OperatorsPostModel.Password = btoa(this.confirmPassword);
      // this.OperatorsPostModel.Password = "";
      var ans = confirm("Do you want to save Operator" + this.OperatorsPostModel.OperatorName + "?");
      if (ans) {
        this._utilitiesService.ModifyOperatorsDetails(this.OperatorsPostModel).subscribe(
          response => {
            swal("Updated!", "Saved " + this.OperatorsPostModel.OperatorName + " Saved", "success");
            this.clear();
            $('#AddNewOperatorsModel').modal('hide');

          },
          (err) => {
            if (err.status === 400) {
              this.OperatorsPostModel.Password = atob(this.OperatorsPostModel.Password);
              const validationError = err.error;
              swal("Warning!", validationError, "warning");
              this.OperatorsPostModel = {
                OperatorCode: "",
                OperatorName: "",
                OperatorType: "",
                MobileNo: "",
                BranchCode: "",
                CompanyCode: "",
                EmployeeID: null,
                Password: null,
                RoleID: null,
                Status: "",
                MaxDiscountPercentAllowed: 0,
                CounterCode: "",
                DefaultStore: "",
                MappedStores: []
              };
              this.OperatorsPostModel.CompanyCode = this.ccode;
              this.OperatorsPostModel.BranchCode = this.bcode;
              this.OperatorsPostModel.CounterCode = "ALL";
              this.OperatorsPostModel.DefaultStore = String(this.bcode);
              this.OperatorsPostModel.Password = null;
              this.confirmPassword = null;
              this.OperatorsPostModel.OperatorType = "Others";
              this.EditBranches = false;
              this.AddBranches = true;
              this.EnableSave = false;
              this.EnableAdd = true;
              this.readonly = false;
              this.confirmPassword = null;
              this.allbranches = [];
              this.StoreList = [];
              this.allbranches.push({ Code: this.bcode, Name: this.bcode });
              this.OperatorsPostModel.MappedStores.push({ Code: this.bcode, Name: this.bcode })
              this.OperatorsPostModel.DefaultStore = String(this.bcode);
              this.getStores();
              this.LoadAllOperator();
              this.GetAllCounter();
              this.getRoleslist();
              this.OperatorCode.nativeElement.focus();
            }
            else {
              this.errors.push('something went wrong!');
              this.OperatorsPostModel = {
                OperatorCode: "",
                OperatorName: "",
                OperatorType: "",
                MobileNo: "",
                BranchCode: "",
                CompanyCode: "",
                EmployeeID: null,
                Password: null,
                RoleID: null,
                Status: "",
                MaxDiscountPercentAllowed: 0,
                CounterCode: "",
                DefaultStore: null,
                MappedStores: []
              };
              this.OperatorsPostModel.CompanyCode = this.ccode;
              this.OperatorsPostModel.BranchCode = this.bcode;
              this.OperatorsPostModel.CounterCode = "ALL";
              this.OperatorsPostModel.DefaultStore = String(this.bcode);
              this.OperatorsPostModel.Password = null;
              this.confirmPassword = null;
              this.OperatorsPostModel.OperatorType = "Others";
              this.EditBranches = false;
              this.AddBranches = true;
              this.EnableSave = false;
              this.EnableAdd = true;
              this.readonly = false;
              this.confirmPassword = null;
              this.allbranches = [];
              this.StoreList = [];
              this.allbranches.push({ Code: this.bcode, Name: this.bcode });
              this.OperatorsPostModel.MappedStores.push({ Code: this.bcode, Name: this.bcode })
              this.OperatorsPostModel.DefaultStore = String(this.bcode);
              this.getStores();
              this.LoadAllOperator();
              this.GetAllCounter();
              this.getRoleslist();
              this.OperatorCode.nativeElement.focus();
            }
          }
        )
      }
    }
  }
  result: any = [];
  ActiveOrInactiveOperator(arg) {
    if (arg.Status == "Closed") {
      var ans = confirm("Do you want to Activate Operator " + arg.OperatorName + "?");
      if (ans) {
        this.result = [];
        this._utilitiesService.ActiveOperator(arg.OperatorCode).subscribe(
          response => {
            this.result = response;
            swal("Success!", this.result.Message, "success");
            this.LoadAllOperator();
          }
        )
      }
    }
    else if (arg.Status == "Active") {
      this.result = [];
      var ans = confirm("Do you want to Close Operator " + arg.OperatorName + "? ");
      if (ans) {
        this._utilitiesService.InActiveOperator(arg.OperatorCode).subscribe(
          response => {
            this.result = response;
            swal("Updated!", this.result.Message, "success");
            this.LoadAllOperator();
          }
        )
      }
    }
  }
  CloseModel() {
    this.Operatorform.reset();
    this.EnableSave = false;
    this.EnableAdd = true;
    this.OperatorsPostModel = {
      OperatorCode: "",
      OperatorName: "",
      OperatorType: "",
      MobileNo: "",
      BranchCode: "",
      CompanyCode: "",
      EmployeeID: null,
      Password: null,
      RoleID: null,
      Status: "",
      MaxDiscountPercentAllowed: 0,
      CounterCode: "",
      DefaultStore: null,
      MappedStores: []
    };
    this.confirmPassword = null;
    this.editconfirmPassword = null;
    this.allbranches = [];
    this.StoreList = [];
    this.getStores();
    this.LoadAllOperator();
    this.GetAllCounter();
    this.getRoleslist();
    $('#AddNewOperatorsModel').modal('hide');

  }
  clear() {
    this.EditBranches = false;
    this.AddBranches = true;
    this.EnableSave = false;
    this.EnableAdd = true;
    this.readonly = false;
    this.confirmPassword = null;
    this.allbranches = [];
    this.StoreList = [];
    this.allbranches = [];
    this.MainDropdownMaster = [];
    this.OperatorsPostModel = {
      OperatorCode: "",
      OperatorName: "",
      OperatorType: "",
      MobileNo: "",
      BranchCode: "",
      CompanyCode: "",
      EmployeeID: null,
      Password: null,
      RoleID: null,
      Status: "",
      MaxDiscountPercentAllowed: 0,
      CounterCode: "",
      DefaultStore: null,
      MappedStores: []
    };

    this.allbranches.push({ Code: this.bcode, Name: this.bcode });
    this.OperatorsPostModel.DefaultStore = String(this.bcode);
    this.OperatorsPostModel.CompanyCode = this.ccode;
    this.OperatorsPostModel.BranchCode = this.bcode;
    this.OperatorsPostModel.CounterCode = "ALL";
    this.OperatorsPostModel.OperatorType = "Others";
    this.getStores();
    this.LoadAllOperator();
    this.GetAllCounter();
    this.getRoleslist();
    this.OperatorCode.nativeElement.focus();
  }
  printData: any = [];
  Print() {
    this.printData = [];
    if (this.printData != null) {
      var ans = confirm("Do you want to take print??");
      if (ans) {
        this._utilitiesService.getOperatorsDetails().subscribe(
          response => {
            this.printData = response
            if (this.printData != null) {
              $('#PrintDetailsTab').modal('show');
            }
          }
        )
      }
    }
  }


  print() {
    let printContents, popupWin;
    printContents = document.getElementById('TableData').innerHTML;
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


}

