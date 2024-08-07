import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MastersService } from '../masters.service';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';
import { GSRTgroup } from '../masters.model';
@Component({
  selector: 'app-hsn-master',
  templateUrl: './hsn-master.component.html',
  styleUrls: ['./hsn-master.component.css']
})
export class HsnMasterComponent implements OnInit {
  HSNForm: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  isReadOnly: boolean = false;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  EnableJson: boolean = false;
  HSNTYpe = [{ "code": "H", "name": "HSN" },
  { "code": "S", "name": "SAC" }]
  HSNgroupModel = {
    ComapnyCode: null,
    BranchCode: null,
    Code: null,
    GSTGroupCode: null,
    Type: null,
    Description: null,
    LastModifiedOn: null,
    LastModifiedBy: null,
    IsActive: false
  }
  constructor(private fb: FormBuilder, private _mastersService: MastersService,
    private _appConfigService: AppConfigService,
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
    this.HSNTYpe;
    this.getHSNMasterList();
    this.getItemListGroup();
    this.HSNForm = this.fb.group({
      frmCtrl_GSTGroupCode: [null, Validators.required],
      frmCtrl_HSNtype: [null, Validators.required],
      frmCtrl_HSN_SAC_Code: [null, Validators.required],
      frmCtrl_Description: [null, Validators.required],
      frmCtrl_IsActive: [null, Validators.required]
    });
  }
  HSNMasterList: any = [];
  getHSNMasterList() {
    this._mastersService.getHSNMaster().subscribe(
      Response => {
        this.HSNMasterList = Response;
      }
    )
  }
  GSTList: any = []
  getItemListGroup() {

    this._mastersService.getGSTList().subscribe(
      Response => {
        this.GSTList = Response;
      }
    )
  }
  IsEnable(e) {
    if (e.target.checked == true) {
      this.HSNMasterList.IsActive = "Y";
    }
    if (e.target.checked == false) {
      this.HSNMasterList.IsActive = "N";
    }
  }
  clear() {
    this.HSNForm.reset();
    this.isReadOnly = false;
    this.EnableAdd = true;
    this.EnableSave = false;
  }
  errors: any = [];
  postHSNListData(form) {
    if (form.value.frmCtrl_GSTGroupCode == null || form.value.frmCtrl_GSTGroupCode == "") {
      swal("!Warning", "Please enter  GST Goup", "warning");
    }
    else if (form.value.frmCtrl_HSNtype == null || form.value.frmCtrl_HSNtype == "") {
      swal("!Warning", "Please enter  GST Type", "warning");
    }
    else if (form.value.frmCtrl_HSN_SAC_Code == null || form.value.frmCtrl_HSN_SAC_Code == "") {
      swal("!Warning", "Please enter  HSN/SAC code", "warning");
    }
    else if (form.value.frmCtrl_Description == null || form.value.frmCtrl_Description == "") {
      swal("!Warning", "Please enter  description", "warning");
    }
    else {
      this.HSNgroupModel.ComapnyCode = this.ccode;
      this.HSNgroupModel.BranchCode = this.bcode;
      var ans = confirm("Do you want to Add??");
      if (ans) {
        this._mastersService.PostHSNList(this.HSNgroupModel).subscribe(
          response => {
            this.errors = response;
            swal("Saved!", "Saved " + this.HSNgroupModel.Description + " Saved", "success");
            this.getItemListGroup();
            this.getHSNMasterList();
            this.clear();
            this.HSNForm.reset();
          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error;
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
  editField(arg) {
    this.EnableAdd = false;
    this.EnableSave = true;
    this.HSNgroupModel = arg;
    this.isReadOnly = true;
    this.getItemListGroup();
    this.getHSNMasterList();

  }
  save(form) {
    if (form.value.frmCtrl_Description == null || form.value.frmCtrl_Description == "") {
      swal("!Warning", "Please enter  description", "warning")
    }
    else {
      var ans = confirm("Do you want to save ??" + form.value.frmCtrl_Description);
      if (ans) {
        this.HSNgroupModel.ComapnyCode = this.ccode;
        this.HSNgroupModel.BranchCode = this.bcode;
        this._mastersService.putHSNListData(this.HSNgroupModel).subscribe(
          response => {
            swal("Updated!", "Saved " + this.HSNgroupModel.Description + " Saved", "success");
            this.getItemListGroup();
            this.getHSNMasterList();
            this.clear();
          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error;
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

}
