import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import { AccountsService } from '../accounts.service';
import swal from 'sweetalert';
declare var $: any;
// import { Counter } from '../masters.model';

@Component({
  selector: 'app-acc-code-setting',
  templateUrl: './acc-code-setting.component.html',
  styleUrls: ['./acc-code-setting.component.css']
})
export class AccCodeSettingComponent implements OnInit {
  AccCodesettingForm: FormGroup;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  isReadOnly: boolean = false;


  EnableJson: boolean = false;
  ccode: string;
  bcode: string;
  password: string;
  ///
  //----for page
  totalItems: any = [];
  NoRecords: boolean = false;
  pagenumber: number = 1;
  top = 10;
  Modeltop = 10;
  skip = (this.pagenumber - 1) * this.top;
  ////
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
  accSetingModel = {
    AccCode: null,
    AccName: null,
    BranchCode: null,
    CompanyCode: null,
    Description: null,
    GroupID: 0,
    GroupName: null,
    ObjID: null,
    ObjectStatus: null,
    SubGroupID: 0,
    SubGroupName: null
  }

  ngOnInit() {
    this.onPageChange(this.pagenumber);
    //this.getAccSettingList();
    this.getAccNames();
    this.AccCodesettingForm = this.fb.group({
      frmCtrl_ObjId: [null, Validators.required],
      frmCtrl_AcountName: [null, Validators.required],
      frmCtrl_Description: [null, Validators.required],
      frmCtrl_ObjStatus: [null, Validators.required],

    });

  }
  AccSetting: any = [];
  getAccSettingList() {
    this._acountsService.getAccSettingData().subscribe(
      resposne => {
        this.AccSetting = resposne;
      }
    )
  }
  getStatusColor(ObjStatus) {
    switch (ObjStatus) {
      case 'O':
        return 'green';
      case 'D':
        return 'Black';
      case 'C':
        return 'red';

    }
  }
  AccNames: any = [];
  getAccNames() {
    this._acountsService.getAccNamesList().subscribe(
      resposne => {
        this.AccNames = resposne;
      }
    )

  }
  errors = [];
  add(form) {
    if (form.value.frmCtrl_ObjId == null || form.value.frmCtrl_ObjId === '') {
      swal("!Warning", "Please Enter Obj id", "warning");
    }
    else if (form.value.frmCtrl_AcountName == null || form.value.frmCtrl_AcountName === '') {
      swal("!Warning", "Please select AcountName ", "warning");
    }
    // else if (form.value.frmCtrl_CounterName == null || form.value.frmCtrl_CounterName === '') {
    //   swal("!Warning", "Please select Sub Counter Name", "warning");
    // }
    else {
      this.accSetingModel.CompanyCode = this.ccode;
      this.accSetingModel.BranchCode = this.bcode;
      this.accSetingModel.ObjectStatus = 'O';
      var ans = confirm("Do you want to Add??");
      if (ans) {
        this._acountsService.PostDetails(this.accSetingModel).subscribe(
          response => {
            swal("Success!", "Account code: " + this.accSetingModel.AccName + "Saved", "success");
            this.getAccSettingList();
            this.AccCodesettingForm.reset();

          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error;
              swal("!Warning", validationError, "warning");
            }
            else {
              this.errors.push('something went wrong!');
            }
            // this.clear();
          }
        )
      }
    }
    this.getAccSettingList();

  }
  editField(arg) {
    this.isReadOnly = true;
    this.EnableAdd = false;
    this.EnableSave = true;
    this.accSetingModel = arg;
  }
  save(form) {
    if (form.value.frmCtrl_ObjId == null || form.value.frmCtrl_ObjId === '') {
      swal("!Warning", "Please Enter Obj id", "warning");
    }
    else if (form.value.frmCtrl_AcountName == null || form.value.frmCtrl_AcountName === '') {
      swal("!Warning", "Please select AcountName ", "warning");
    }
    else {
      var ans = confirm("Do you want to save??");
      if (ans) {
        this._acountsService.editAccSettingDetails(this.accSetingModel.ObjID, this.accSetingModel).subscribe(
          response => {
            swal("Updated!", "Saved " + this.accSetingModel.AccName + " Saved", "success");
            this.AccCodesettingForm.reset();
            this.getAccSettingList();
            this.isReadOnly = false;
            this.EnableAdd = true;
            this.EnableSave = false;
          }

        )
      }
    }
    this.getAccSettingList();
  }
  openField(arg) {
    //this.accSetingModel = arg;
    if (arg.ObjectStatus == "O") {
      swal("Warning!", "Acc: " + arg.AccName + " is already Open", "warning");
    }
    else {
      var ans = confirm("Do you want to Open Acc: " + arg.AccName + "?");
      if (ans) {
        arg.ObjectStatus = "O";
        this._acountsService.editAccSettingDetails(arg.ObjID, arg).subscribe(
          response => {
            swal("Updated!", "Acc: " + arg.AccName + " Opened", "success");
          }
        )
      }
      this.getAccSettingList();
    }
    this.getAccSettingList();
  }

  closeField(arg) {
    //)this.accSetingModel = arg;
    if (arg.ObjectStatus == "C") {
      swal("Warning!", "Acc: " + arg.AccName + " is already close", "warning");
    }
    else {
      var ans = confirm("Do you want to close the Acc: " + arg.AccName + "?");
      if (ans) {
        arg.ObjectStatus = "C";
        this._acountsService.editAccSettingDetails(arg.ObjID, arg).subscribe(
          response => {
            swal("Updated!", "Acc: " + arg.AccName + "closed", "success");
          }
        )
      }
      this.getAccSettingList();
    }
    this.getAccSettingList();
  }
  clear() {
    this.AccCodesettingForm.reset();
    this.isReadOnly = false;
    this.EnableAdd = true;
    this.EnableSave = false;
    this.getAccSettingList();
  }

  onPageChange(p: number) {
    this.pagenumber = p;
    // alert('skip' + this.skip);
    const skipno = (this.pagenumber - 1) * this.top;
    this.getSelectedPageNoRecords(this.top, skipno);
  }
  getSelectedPageNoRecords(top, skip) {
    this._acountsService.GetTopAndSkipRecords(top, skip).subscribe(
      response => {
        this.AccSetting = response;
        this.getSearchCount();
      }
    )
  }
  getSearchCount() {
    this._acountsService.GetTotalRecordCount().subscribe(
      response => {
        this.totalItems = response;
        this.totalItems = this.totalItems.RecordCount;
      }
    );
  }

}
