import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MastersService } from '../masters.service';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';
import { GSRTgroup } from '../masters.model';
@Component({
  selector: 'app-gst-master',
  templateUrl: './gst-master.component.html',
  styleUrls: ['./gst-master.component.css']
})
export class GstMasterComponent implements OnInit {
  GSTForm: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  isReadOnly: boolean = false;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  EnableJson: boolean = false;
  GroupTYpe = [{ "code": "G", "name": "Goods" },
  { "code": "S", "name": "Service" }]
  GSTgroupModel: GSRTgroup = {
    BranchCode: null,
    Code: null,
    CompanyCode: null,
    Description: null,
    GSTGroupType: null,
    IsActive: null,
    SortOrder: 0
  }

  constructor(private fb: FormBuilder, private _mastersService: MastersService, private _appConfigService: AppConfigService,
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
    this.GroupTYpe;
    this.getItemListGroup();
    this.GSTForm = this.fb.group({
      frmCtrl_Code: [null, Validators.required],
      frmCtrl_GSTGroupType: [null, Validators.required],
      frmCtrl_Description: [null, Validators.required],
      frmCtrl_IsActive: [null, Validators.required],
      frmCtrl_SortOrder: null

    })

  }
  addGSTMasterRecord() {
    this._router.navigate(['gst-master/add']);
  }
  GSTgroupList() {

  }
  GSTgroupItemList: any = []
  getItemListGroup() {
    this._mastersService.getGSTList().subscribe(
      Response => {
        this.GSTgroupItemList = Response
      }
    )
  }
  IsEnable(e) {
    if (e.target.checked == true) {
      this.GSTgroupItemList.IsActive = "Y";
    }
    if (e.target.checked == false) {
      this.GSTgroupItemList.IsActive = "N";
    }
  }

  clear() {
    this.GSTForm.reset();
    this.isReadOnly = false;
    this.EnableAdd = true;
    this.EnableSave = false;
  }
  errors: any = [];
  postGSTListData(form) {
    if (form.value.frmCtrl_Code == null || form.value.frmCtrl_Code == "") {
      swal("Warning!", "Please enter  GST Code", "warning");
    }
    else if (form.value.frmCtrl_GSTGroupType == null || form.value.frmCtrl_GSTGroupType == "") {
      swal("Warning!", "Please select GST Type", "warning");
    }
    else if (form.value.frmCtrl_Description == null || form.value.frmCtrl_Description == "") {
      swal("Warning!", "Please enter description", "warning");
    }

    else {
      if (ans) {
        this.GSTgroupModel.BranchCode = this.bcode;
        this.GSTgroupModel.CompanyCode = this.ccode;
        var ans = confirm("Do you want to Add??");
        this._mastersService.PostGSTList(this.GSTgroupModel).subscribe(
          response => {
            this.errors = response;
            swal("Saved!", "Saved " + this.GSTgroupModel.Description + " Saved", "success");
            this.getItemListGroup();
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
      this.clear();
      this.getItemListGroup();
      this.GSTForm.reset();
    }

  }
  editField(arg) {
    this.EnableAdd = false;
    this.EnableSave = true;
    this.GSTgroupModel = arg;
    this.isReadOnly = true;
    this.getItemListGroup();

  }
  save(form) {
    if (form.value.frmCtrl_Description == null || form.value.frmCtrl_Description == "") {
      swal("Warning!", "Please enter description", "warning");
    }
    else {
      var ans = confirm("Do you want to save ??" + form.value.frmCtrl_Description);
      if (ans) {
        this._mastersService.putGSTListData(this.GSTgroupModel.Code, this.GSTgroupModel).subscribe(
          response => {
            swal("Updated!", "Saved " + this.GSTgroupModel.Description + " Saved", "success");
            this.getItemListGroup();
            this.clear();
          }
        )
      }
    }
  }
}
