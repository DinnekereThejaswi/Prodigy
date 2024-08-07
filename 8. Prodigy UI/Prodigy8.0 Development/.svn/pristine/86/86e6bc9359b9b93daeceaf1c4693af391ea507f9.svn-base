import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MastersService } from '../masters.service';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { StockGroup } from '../masters.model';

@Component({
  selector: 'app-stock-group',
  templateUrl: './stock-group.component.html',
  styleUrls: ['./stock-group.component.css']
})
export class StockGroupComponent implements OnInit {
  StockGroupForm: FormGroup;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  isReadOnly: boolean = false;
  ccode: string;
  bcode: string;
  EnableJson: boolean = false;
  password: string;
  StockGroupListData: StockGroup = {
    BranchCode: "",
    CompanyCode: "",
    MetalCode: "",
    MetalName: "",
    ObjID: "",
    ObjectStatus: "",
  }
  constructor(private _appConfigService: AppConfigService, private fb: FormBuilder, private _router: Router, private _mastersService: MastersService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }
  ngOnInit() {
    this.getStockList();
    this.StockGroupForm = this.fb.group({
      MetalCode: null,
      MetalName: null
    });
  }
  StockGroupList: any = [];
  getStockList() {
    this._mastersService.getStockGroup().subscribe(
      Response => {
        this.StockGroupList = Response;
        // console.log( this.StockGroupList);

      }
    )
  }

  getStatusColor(ObjStatus) {
    switch (ObjStatus) {
      case 'O':
        return 'green';
      case 'C':
        return 'red';
    }
  }
  errors: any = [];
  add(form) {
    if (form.value.MetalCode == null || form.value.MetalCode == "") {
      swal("Warning!", "Please enter Metal code", "warning");
    }
    else if (form.value.MetalName == null || form.value.MetalName == "") {
      swal("Warning!", "Please enter Metal Name", "warning");
    }
    else {

      var ans = confirm("Do you want to save??");
      if (ans) {
        this.StockGroupListData.CompanyCode = this.ccode;
        this.StockGroupListData.BranchCode = this.bcode;
        this.StockGroupListData.ObjectStatus = "O";
        this._mastersService.PostStock(this.StockGroupListData).subscribe(
          response => {
            swal("Saved!", "Saved " + this.StockGroupListData.MetalName + " Saved", "success");
            this.getStockList();
            this.StockGroupForm.reset();

          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error.description;
              swal("Warning!", validationError, "warning");
            }
            else {
              this.errors.push('something went wrong!');
            }
            this.clear();
          }
        )
      }
    }

    this.ClearValues();
  }
  clear() {
    this.StockGroupForm.reset();
    this.isReadOnly = false;
    this.getStockList();
  }
  ClearValues() {
    this.EnableAdd = true;
    this.EnableSave = false;
    this.clear();
    this.StockGroupForm.reset();
    this.StockGroupListData = {
      BranchCode: "",
      CompanyCode: "",
      MetalCode: "",
      MetalName: "",
      ObjID: '',
      ObjectStatus: "",
    }
    this.getStockList();
  }

  save(form) {

    if (form.value.MetalName == null || form.value.MetalName == "") {
      swal("Warning!", "Please enter Metal Name", "warning");
    }
    else {
      var ans = confirm("Do you want to save??");
      if (ans) {
        this.StockGroupListData.MetalName = form.value.MetalName;
        this._mastersService.ModifyStock(this.StockGroupListData.ObjID, this.StockGroupListData).subscribe(
          response => {
            swal("Updated!", "Saved " + this.StockGroupListData.MetalName + " Saved", "success");
            this.ClearValues();


          }
        )
      }
    }
    this.getStockList();
    this.ClearValues();
  }
  editField(arg) {
    if (arg.ObjectStatus == "C") {
      swal("Warning!", "MetalName: " + arg.MetalName + " is Closeed, Open to edit", "warning");
    }
    else {
      arg.UpdateOn = null;
      arg.UniqRowID = null;
      this.StockGroupListData = arg;
      this.EnableAdd = false;
      this.EnableSave = true;
      this.isReadOnly = true;
    }
  }

  openField(arg) {
    if (arg.ObjectStatus == "O") {
      swal("Warning!", "MetalName: " + arg.MetalName + " is already Open", "warning");
    }
    else {
      var ans = confirm("Do you want to Open Metal: " + arg.MetalName + "?");
      if (ans) {
        arg.ObjectStatus = "O";
        this._mastersService.StockStatus(arg.ObjID, arg).subscribe(
          response => {
            swal("Updated!", "Metal: " + arg.MetalName + " Open", "success");
          }
        )
      }
    }
  }

  closeField(arg) {
    if (arg.ObjectStatus == "C") {
      swal("Warning!", "Metal: " + arg.MetalName + " is already close", "warning");
    }
    else {
      var ans = confirm("Do you want to close Metal: " + arg.MetalName + "?");
      if (ans) {
        arg.ObjectStatus = "C";
        this._mastersService.StockStatus(arg.ObjID, arg).subscribe(
          response => {
            swal("Updated!", "Metal: " + arg.MetalName + " close", "success");
          }
        )
      }
    }
  }

}
