import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { MastersService } from './../masters.service'
import { Router, ActivatedRoute } from '@angular/router';
import { PackingMaterial } from '../masters.model';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';

@Component({
  selector: 'app-design-master',
  templateUrl: './design-master.component.html',
  styleUrls: ['./design-master.component.css']
})
export class DesignMasterComponent implements OnInit {

  DesignMasterForm: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  isReadOnly: boolean = false;
  EnableJson: boolean = false;
  DesignMasterListData = {
    ObjID: "",
    CompanyCode: "",
    BranchCode: "",
    DesignCode: "",
    DesignName: "",
    MasterDesignCode: "NA",
    UniqRowID: "00000000-0000-0000-0000-000000000000",
    ObjectStatus: ""
  }

  constructor(private fb: FormBuilder,
    private _router: Router, private _mastersService: MastersService,
    private _appConfigService: AppConfigService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }
  ngOnInit() {
    this.DesignMasterListData.CompanyCode = this.ccode;
    this.DesignMasterListData.BranchCode = this.bcode;
    this.DesignMasterListData.ObjectStatus = "O";
    this.DesignMasterListData.MasterDesignCode = "NA";
    this.getDesignsByCodeMasterDesign();
    this.DesignMasterForm = this.fb.group({
      MasterDesign: null,
      Code: null,
      Name: null
    });
  }
  DesignList: any = [];
  getDesignsByCodeMasterDesign() {
    this._mastersService.getdesigns(this.DesignMasterListData.MasterDesignCode).subscribe(
      response => {
        this.DesignList = response;
      }
    );
  }
  errors: any = [];
  Add(form) {
    if (form.value.Code == null || form.value.Code == "") {
      swal("Warning!", "Please select  Design Code", "warning")
    }
    else if (form.value.Name == null || form.value.Name == "") {
      swal("Warning!", "Please select  Design Name", "warning")
    }
    else {
      var ans = confirm("Do you want to Add??" + this.DesignMasterListData.DesignCode);
      if (ans) {
        this._mastersService.PostNewDesign(this.DesignMasterListData).subscribe(
          response => {
            response;
            swal("Success!", "Saved " + this.DesignMasterListData.DesignCode + " Saved", "success");
            this.getDesignsByCodeMasterDesign();
            this.DesignMasterForm.reset();
          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error.description;
              swal("Warning!", validationError, "warning");
            }
            else {
              this.errors.push('something went wrong!');
            }
            // this.clear();
          }
        )
      }
    }
    this.errors.length = 0;

  }
  edit(arg) {
    if (arg.ObjStatus == "C") {
      swal("Warning!", "Order: " + arg.DesignName + " is Closed", "warning");
    }
    else {
      this.EnableAdd = false;
      this.EnableSave = true;
      this.DesignMasterListData = arg;
      this.isReadOnly = true;
    }
  }
  save(form) {
    if (form.value.Code == null || form.value.Code == "") {
      swal("Warning!", "Please select  Design Code", "warning")
    }
    else if (form.value.Name == null || form.value.Name == "") {
      swal("Warning!", "Please select  Design Name", "warning")
    }
    else {
      var ans = confirm("Do you want to save??" + this.DesignMasterListData.DesignName);
      if (ans) {
        this._mastersService.EditDesignDetails(this.DesignMasterListData.ObjID, this.DesignMasterListData).subscribe(
          response => {
            swal("Success!", "Saved " + this.DesignMasterListData.DesignName + " Saved", "success");
            this.EnableSave = false;
            this.EnableAdd = true;
            this.isReadOnly = false;
            this.DesignMasterForm.reset();
            this.getDesignsByCodeMasterDesign();
          }
        )
      }
    }
  }
  clear() {
    this.DesignMasterForm.reset();
    this.isReadOnly = false;
    this.EnableAdd = true;
    this.EnableSave = false;
    this.getDesignsByCodeMasterDesign();
    this.DesignMasterListData = {
      ObjID: "",
      CompanyCode: "",
      BranchCode: "",
      DesignCode: "",
      DesignName: "",
      MasterDesignCode: "",
      UniqRowID: "00000000-0000-0000-0000-000000000000",
      ObjectStatus: ""
    };
    this.DesignMasterListData.MasterDesignCode = "NA";
  }

  getStatusColor(ObjStatus) {
    switch (ObjStatus) {
      case 'O':
        return 'green';
      case 'C':
        return 'Red';
    }
  }

  openField(arg) {
    this.DesignMasterListData = arg;
    if (arg.ObjectStatus == "O") {
      swal("Warning!", "Design: " + this.DesignMasterListData.DesignName + " is already Open", "warning");
    }
    else {
      var ans = confirm("Do you want to Open Design: " + arg.CounterName + "?");
      if (ans) {
        this.DesignMasterListData.ObjectStatus = "O";
        this._mastersService.OpenDesign(this.DesignMasterListData.ObjID).subscribe(
          response => {
            swal("Success!", "Design: " + arg.CounterName + " Opened", "success");
            this.getDesignsByCodeMasterDesign();
            this.DesignMasterForm.reset();
          }
        )
      }

    }

  }

  closeField(arg) {
    this.DesignMasterListData = arg;
    if (arg.ObjectStatus == "C") {
      swal("Warning!", "Design: " + this.DesignMasterListData.DesignName + " is already close", "warning");
    }
    else {
      var ans = confirm("Do you want to close the Design: " + arg.CounterName + "?");
      if (ans) {
        this.DesignMasterListData.ObjectStatus = "C";
        this._mastersService.CloseDeign(this.DesignMasterListData.ObjID).subscribe(
          response => {
            swal("Success!", "Design: " + arg.CounterName + "closed", "success");
            this.getDesignsByCodeMasterDesign();
            this.DesignMasterForm.reset();
          }
        )
      }
    }
  }
}
