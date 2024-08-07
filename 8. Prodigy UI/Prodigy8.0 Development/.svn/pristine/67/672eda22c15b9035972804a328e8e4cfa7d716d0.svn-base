import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MastersService } from '../masters.service';
import { PackingMaterial } from '../masters.model';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { ToastrService } from 'ngx-toastr';
@Component({
  selector: 'app-fixed-order',
  templateUrl: './fixed-order.component.html',
  styleUrls: ['./fixed-order.component.css']
})
export class FixedOrderComponent implements OnInit {
  fixedOrderForm: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  isReadOnly: boolean = false;
  EnableJson: boolean = false;
  FixedOrderListData = {
    ObjID: null,
    CompanyCode: null,
    BranchCode: null,
    BookingCode: null,
    BookingName: null,
    AccCode: null,
    ccName: null,
    OperatorCode: null,
    UpdateOn: null,
    ObjStatus: null,
    OrderRateType: null,
  }
  constructor(private toastr: ToastrService, private fb: FormBuilder,
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
    this.getFixedOrderList();
    this.getTypesOfRates();
    this.getAccTypes();
    this.EnableAdd = true;
    this.fixedOrderForm = this.fb.group({
      fromCtrl_Ptype: null,
      fromCtrl_Pcode: null,
      fromCtrl_Pname: null,
      fromCtrl_MapedLedger: null
    })

  }
  fixedOrders: any = [];
  getFixedOrderList() {
    this._mastersService.getFixedOrderList().subscribe(
      response => {
        this.fixedOrders = response;
      }
    )
  }
  ratetypes: any = [];
  getTypesOfRates() {
    this._mastersService.getTypesRate().subscribe(
      response => {
        this.ratetypes = response;
      }
    )

  }
  ledgers: any = [];
  getAccTypes() {
    this._mastersService.getLedgerTypes().subscribe(
      response => {
        this.ledgers = response;
      }
    )
  }
  errors: any = [];
  add(form) {

    if (form.value.fromCtrl_Ptype == null || form.value.fromCtrl_Ptype == "") {
      swal("Warning!", "Please select rate type", "warning");
    }
    else if (form.value.fromCtrl_Pcode == null || form.value.fromCtrl_Pcode == "") {
      swal("Warning!", "Please enter code", "warning");
    }
    else if (form.value.fromCtrl_Pname == null || form.value.fromCtrl_Pname == "") {
      swal("Warning!", "Please enter the Name", "warning");
    }
    else if (form.value.fromCtrl_Pname == null || form.value.fromCtrl_Pname == "") {
      swal("Warning!", "Please select rate ledger", "warning");
    }
    else if (form.value.fromCtrl_MapedLedger == null || form.value.fromCtrl_MapedLedger == "null" || form.value.fromCtrl_MapedLedger == "") {
      swal("Warning!", "Please  select  ledger Name", "warning");
    }
    else {
      this.FixedOrderListData.CompanyCode = this.ccode;
      this.FixedOrderListData.BranchCode = this.bcode;
      this.FixedOrderListData.ObjStatus = "O";
      this.FixedOrderListData.OperatorCode = localStorage.getItem('Login');

      var ans = confirm("Do you want to save??");
      if (ans) {
        this._mastersService.PostFixedOrders(this.FixedOrderListData).subscribe(
          response => {
            response;
            swal("Saved!", "Saved " + this.FixedOrderListData.BookingName + " Saved", "success");
            this.getFixedOrderList();
            this.fixedOrderForm.reset();
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
  editField(arg) {
    if (arg.ObjStatus == "C") {
      swal("Warning!", "Order: " + arg.BookingName + " is Closed", "warning");
    }
    else {
      this.EnableAdd = false;
      this.EnableSave = true;
      this.FixedOrderListData = arg;
      this.isReadOnly = true;
    }

  }
  save(form) {
    if (form.value.fromCtrl_Ptype == null || form.value.fromCtrl_Ptype == "") {
      swal("Warning!", "Please select rate type", "warning");
    }
    else if (form.value.fromCtrl_Pcode == null || form.value.fromCtrl_Pcode == "") {
      swal("Warning!", "Please enter code", "warning");
    }
    else if (form.value.fromCtrl_Pname == null || form.value.fromCtrl_Pname == "") {
      swal("Warning!", "Please enter the Name", "warning");
    }
    else if (form.value.fromCtrl_Pname == null || form.value.fromCtrl_Pname == "") {
      swal("Warning!", "Please  select rate ledger", "warning");
    }
    else if (form.value.fromCtrl_MapedLedger == null || form.value.fromCtrl_MapedLedger == "null" || form.value.fromCtrl_MapedLedger == "") {
      swal("Warning!", "Please  select  ledger Name", "warning");
    }
    else {


      var ans = confirm("Do you want to save??");
      if (ans) {
        this.FixedOrderListData.OperatorCode = localStorage.getItem('Login');
        this._mastersService.putOrderFixedDetails(this.FixedOrderListData.ObjID, this.FixedOrderListData).subscribe(
          response => {
            swal("Updated!", "Saved " + this.FixedOrderListData.BookingName + " Saved", "success");
            this.EnableSave = false;
            this.EnableAdd = true;
            this.isReadOnly = false;
            this.fixedOrderForm.reset();
            this.fixedOrders = [];
            this._mastersService.getFixedOrderList().subscribe(
              response => {
                this.fixedOrders = response;
              }
            )
          }
        )
      }
    }
  }
  clear() {
    this.fixedOrderForm.reset();
    this.isReadOnly = false;
    this.EnableAdd = true;
    this.EnableSave = false;
    this.fixedOrders = [];
    this._mastersService.getFixedOrderList().subscribe(
      response => {
        this.fixedOrders = response;
      }
    )
    this.FixedOrderListData = {
      ObjID: null,
      CompanyCode: null,
      BranchCode: null,
      BookingCode: null,
      BookingName: null,
      AccCode: null,
      ccName: null,
      OperatorCode: null,
      UpdateOn: null,
      ObjStatus: null,
      OrderRateType: null,
    }
  }

  closeField(arg) {
    this.FixedOrderListData = arg;
    if (arg.ObjStatus == "C") {
      swal("Warning!", "Order: " + arg.BookingName + " is already Closed", "warning");
    }
    else {
      var ans = confirm("Do you want to close??" + arg.BookingName);
      if (ans) {
        this.FixedOrderListData.OperatorCode = localStorage.getItem('Login');
        this.FixedOrderListData.ObjStatus = "C";
        this._mastersService.putOrderFixedDetails(this.FixedOrderListData.ObjID, this.FixedOrderListData).subscribe(
          response => {
            swal("Updated!", "Saved " + this.FixedOrderListData.BookingName + " Saved", "success");
            this.EnableSave = false;
            this.EnableAdd = true;
            this.isReadOnly = false;
            this.fixedOrderForm.reset();
            this.fixedOrders = [];
            this._mastersService.getFixedOrderList().subscribe(
              response => {
                this.fixedOrders = response;
              }
            )
          }
        )
      }
    }
  }
  getStatusColor(ObjStatus) {
    switch (ObjStatus) {
      case 'O':
        return 'green';
      case 'C':
        return 'red';

    }
  }
}
