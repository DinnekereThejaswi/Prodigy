import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MastersService } from './../masters.service';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { ToastrService } from 'ngx-toastr';
import { AppConfigService } from '../../AppConfigService';
declare var $: any;
@Component({
  selector: 'app-paymentmaster',
  templateUrl: './paymentmaster.component.html',
  styleUrls: ['./paymentmaster.component.css']
})
export class PaymentmasterComponent implements OnInit {
  PaymentTypesForm: FormGroup;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  EnableJson: boolean = false;
  ccode: string = "";
  bcode: string = "";
  password: string = "";
  PaymentModel = {
    ObjId: "",
    CompanyCode: this.ccode,
    BranchCode: this.bcode,
    PaymentCode: "",
    PaymentName: "",
    SeqNo: null,
    Updateon: "",
    Uniqrowid: "",
    ObjStatus: "",
    AccCode: null,
    AccName: "",
    Active: true
  }
  constructor(private fb: FormBuilder, private _appConfig: AppConfigService, private _masterService: MastersService) {
    this.EnableJson = _appConfig.EnableJson;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.PaymentModel.CompanyCode = this.ccode;
    this.PaymentModel.BranchCode = this.bcode;

  }

  ngOnInit() {
    this.getInputAndOutputAc();
    this.getList();
    this.PaymentTypesForm = this.fb.group({
      frmCtrl_Code: null,
      frmCtrl_Name: null,
      frmCtrl_AccCode: null,

    });

  }

  AcList: any = [];
  getInputAndOutputAc() {
    this._masterService.getAcTypes().subscribe(
      Response => {
        this.AcList = Response;
      }
    )
  }

  AccChanged(arg) {
    var SelectedOption;
    SelectedOption = null;
    SelectedOption = this.AcList.filter(value => value.AccCode === Number(arg));
    this.PaymentModel.AccCode = SelectedOption[0].AccCode;
    this.PaymentModel.AccName = SelectedOption[0].AccName;
  }
  PaymentTypes: any = [];
  getList() {
    this._masterService.getPaymentTypes().subscribe(
      Response => {
        this.PaymentTypes = Response;
      }
    )
  }
  getStatusColor(Status) {

    switch (Status) {
      case true:
        return 'green';

      case false:
        return 'red';

    }
  }
  edit(arg) {
    this.EnableAdd = false;
    this.EnableSave = true;
    this.PaymentModel = arg;

  }

  Clear() {
    this.EnableAdd = true;
    this.EnableSave = false;
    this.PaymentTypesForm.reset();

  }



  ResponseData: any = [];
  Add(form) {
    if (form.value.frmCtrl_Code == "" || form.value.frmCtrl_Code == null) {
      swal("Warning!", "Please Enter Code", "warning")
    }
    else if (form.value.frmCtrl_Name == "" || form.value.frmCtrl_Name == null) {
      swal("Warning!", "Please Enter Name", "warning")
    }
    else if (form.value.frmCtrl_AccCode == "" || form.value.frmCtrl_AccCode == null) {
      swal("Warning!", "Please Select  A/C Name", "warning")
    }
    else {
      var ans = confirm("Do you want to Add ??" + this.PaymentModel.AccName);
      if (ans) {
        this._masterService.AddPaymentType(this.PaymentModel).subscribe(
          Response => {
            this.ResponseData = Response;
            swal("Success!", "Saved Succesfully", "success");
            this.getList();
          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error.description;
              swal("Warning!", validationError, "warning");
            }
          }
        )
        this.PaymentTypesForm.reset();
        this.ResponseData = [];
      }
    }
  }


  save(form) {
    if (form.value.frmCtrl_Name == "" || form.value.frmCtrl_Name == null) {
      swal("Warning!", "Please Enter Name", "warning")
    }
    else if (form.value.frmCtrl_AccCode == "" || form.value.frmCtrl_AccCode == null) {
      swal("Warning!", "Please Select  A/C Name", "warning")
    }
    else {
      var ans = confirm("Do you want to Save ??");
      if (ans) {
        this._masterService.editPaymentType(this.PaymentModel.PaymentCode, this.PaymentModel).subscribe(
          Response => {
            this.ResponseData = Response;
            swal("Success!", "Saved Successfully", "success");
            this.getList();
            this.PaymentTypesForm.reset();
            this.ResponseData = [];
          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error.description;
              swal("Warning!", validationError, "warning");
            }
          }
        )
      }
    }
  }


  delete(arg) {
    var ans = confirm("Do you want to Delete ?" + arg.PaymentName);
    if (ans) {
      this._masterService.deletePaymentType(arg.PaymentCode).subscribe(
        Response => {
          this.ResponseData = Response;
          swal("Success!", "Deleted Successfully", "success");
          this.getList();
          this.PaymentTypesForm.reset();
          this.ResponseData = [];
        },
        (err) => {
          if (err.status === 400) {
            const validationError = err.error.description;
            swal("Warning!", validationError, "warning");
          }
        }
      )
    }
  }



}
