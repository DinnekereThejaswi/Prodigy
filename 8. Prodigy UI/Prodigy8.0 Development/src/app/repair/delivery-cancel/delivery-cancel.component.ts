import { AppConfigService } from '../../AppConfigService';
import { ComponentCanDeactivate } from './../../appconfirmation-guard';
import { RepairService } from './../repair.service';
import { CustomerService } from './../../masters/customer/customer.service';
import { FormGroup, Validators } from '@angular/forms';
import { FormBuilder } from '@angular/forms';
import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import swal from 'sweetalert';
import * as CryptoJS from 'crypto-js';

@Component({
  selector: 'app-delivery-cancel',
  templateUrl: './delivery-cancel.component.html',
  styleUrls: ['./delivery-cancel.component.css']
})
export class DeliveryCancelComponent implements OnInit, ComponentCanDeactivate {

  DeliveryCancelForm: FormGroup;
  @ViewChild("IssueNo", { static: true }) IssueNo: ElementRef;
  EnableJson: boolean = false;
  password: string;
  constructor(
    private fb: FormBuilder, private _CustomerService: CustomerService,
    private _repairService: RepairService, private _appConfigService: AppConfigService

  ) {
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
    this.getCB();
  }

  ccode: string = "";
  bcode: string = "";

  hasRepairDeliveryDetails: boolean = false;
  leavePage: boolean = false;

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this.getApplicationdate()
    this.getSalesMan();
    this.DeliveryCancelForm = this.fb.group({
      IssueNo: ['null', Validators.required],
      CustID: null,
      CustName: null,
      Address1: null,
      MobileNo: null,
      Country: null,
      City: null,
      Address2: null,
      Address3: null,
      Remarks: null,
      SalCode: null
    });
    this.DeliveryCancelForm.controls["Remarks"].valueChanges.subscribe(change => {
      if (change != null && change != "") {
        this.leavePage = true;
      }
      else {
        this.leavePage = false;
      }
    })
  }

  customerDtls: any = [];
  getDeliveryDetails(arg) {
    if (arg == '') {
      swal("Warning!", 'Enter Delivery Number', "warning");
    }
    else {
      this._repairService.getDeliveryNo(arg).subscribe(
        response => {
          this.DeliveryCancelObj = response;
          this.getSalesMan();
          if (this.DeliveryCancelObj != null) {
            this.hasRepairDeliveryDetails = true;
            this._CustomerService.getCustomerDtls(this.DeliveryCancelObj.CustID).subscribe(
              response => {
                this.customerDtls = response;
                this._CustomerService.sendCustomerDtls_To_Customer_Component(this.customerDtls);
                this.DeliveryCancelObj.Country = this.customerDtls.CountryName;
              }
            )
          }
        },
        (err) => {
          if (err.status === 404) {
            const validationError = err.error;
            swal("Warning!", validationError.description, "warning");
            this.hasRepairDeliveryDetails = false;
            this.IssueNo.nativeElement.value = "";
          }
          if (err.status === 400) {
            const validationError = err.error;
            swal("Warning!", validationError.description, "warning");
            this.hasRepairDeliveryDetails = false;
            this.IssueNo.nativeElement.value = "";
          }
        }
      )
    }
  }

  SalesManList: any;
  getSalesMan() {
    this._repairService.getSalesManData().subscribe(
      response => {
        this.SalesManList = response;
      })
  }


  DeliveryCancelObj: any = {
    IssueNo: null,
    CustID: null,
    CustName: null,
    Address1: null,
    City: null,
    MobileNo: null,
    Country: null,
    SalCode: null,
    Address2: null,
    Address3: null,
    Remarks: null,
    CompanyCode: null,
    BranchCode: null,
    DueDate: null,
    lstOfRepairReceiptDetails: [],
  }

  ngOnDestroy() {
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
  }

  submitPost() {
    this.DeliveryCancelObj.CompanyCode = this.ccode;
    this.DeliveryCancelObj.BranchCode = this.bcode;
    if (this.DeliveryCancelObj.Remarks == "" || this.DeliveryCancelObj.Remarks == null) {
      swal("Warning!", 'Please Enter Remarks to Cancel', "warning");
    }
    else {
      var ans = confirm("Do you want to cancel??");
      if (ans) {
        this._repairService.cancelDelivery(this.DeliveryCancelObj).subscribe(
          response => {
            swal("Cancelled!", "Repair Delivery No:" + this.IssueNo.nativeElement.value + " Cancelled Successfully!", "success");
            this.DeliveryCancelForm.reset();
            this.hasRepairDeliveryDetails = false;
            this.IssueNo.nativeElement.value = "";
            this.leavePage = false;
          }
        )
      }
    }
  }

  confirmBeforeLeave(): boolean {
    if (this.leavePage == true) {
      var ans = (confirm("You have unsaved changes! If you leave, your changes will be lost."))
      if (ans) {
        this.leavePage = false;
        return true;
      }
      else {
        return false;
      }
    }
    else {
      return true;
    }
  }

  getApplicationdate() {
    this._repairService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        let rpDate = appDate["applcationDate"]
        this.DeliveryCancelObj.DueDate = rpDate;
      }
    )
  }
}
