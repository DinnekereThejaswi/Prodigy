import { CustomerService } from './../masters/customer/customer.service';
import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterContentChecked } from '@angular/core';
import { Validators, FormGroup, FormBuilder, Validator } from '@angular/forms';
import { formatDate } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import * as CryptoJS from 'crypto-js';
import { SalesreturnService } from './salesreturn.service';
import { MasterService } from './../core/common/master.service';
import { OrdersService } from './../orders/orders.service';
import { AppConfigService } from './../AppConfigService';
import { DatePipe } from '@angular/common'
import { ComponentCanDeactivate } from './../appconfirmation-guard';

import swal from 'sweetalert';
import { Router } from '@angular/router';
declare var $: any;


@Component({
  selector: 'app-salesreturn',
  templateUrl: './salesreturn.component.html',
  styleUrls: ['./salesreturn.component.css']
})

export class SalesreturnComponent implements OnInit, OnDestroy, AfterContentChecked {
  @ViewChild('salesBillNo', { static: true }) salesBillNo: ElementRef;

  SRHeaderForm1: FormGroup;
  radioItems: Array<string>;
  EnableItemDetails: boolean = false;
  EnableReprintSR: boolean = false;
  EnableJson: boolean = false;
  routerUrl: string = "";
  ccode: string = "";
  bcode: string = "";
  leavePage: boolean = false;
  SRPermission: string;
  password: string;
  apiBaseUrl: string;
  today = new Date();
  @ViewChild("PwdSalesRet", { static: true }) PwdSalesRet: ElementRef;
  salesDate = '';
  model = { option: 'Local' };
  datePickerConfig: Partial<BsDatepickerConfig>;
  constructor(private fb: FormBuilder, private toastr: ToastrService, private _SalesreturnService: SalesreturnService,
    private _CustomerService: CustomerService, private _masterService: MasterService,
    private datepipe: DatePipe, private _orderservice: OrdersService, private _appConfigService: AppConfigService,
    private _router: Router) {
    this.radioItems = ['Other Branch', 'Local'];
    this.salesDate = formatDate(this.today, 'yyyy-MM-dd', 'en-US', '+0530');
    this.SRPermission = this._appConfigService.RateEditCode.SalesReturnPermission;
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }

  ngOnInit() {
    this.SRHeaderForm1 = this.fb.group({
      SalesBillNo: [null, Validators.required],
      SalesDate: null,
    });
    this.leavePage = false;
    this.getApplicationDate();
    this.getSalesMan();
    this.getBilledBranch();
    this.SalesReturnHeaderDetails.BranchType = "Local";
    this.GetCustomerDetsFromCustComp();
    this._SalesreturnService.castSRHide.subscribe(
      response => {
        this.EnableItemDetails = response;
        if (this.EnableItemDetails == false) {
          this.leavePage = false;
          this.salesBillNo.nativeElement.value = ""
        }
      }
    )
    this._SalesreturnService.castSREnablePrint.subscribe(
      response => {
        this.EnableReprintSR = response;
      }
    )
  }

  // confirmBeforeLeave(): boolean {
  //   if (this.leavePage == true) {
  //     var ans = (confirm("You have unsaved changes! If you leave, your changes will be lost."))
  //     if (ans) {
  //       this.leavePage = false;
  //       return true;
  //     }
  //     else {
  //       return false;
  //     }
  //   }
  //   else {
  //     return true;
  //   }
  // }


  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  applicationDate: any;
  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
      }
    )
  }


  ngAfterContentChecked() {
    this.PwdSalesRet.nativeElement.focus();
  }



  BilledBranchRadio: boolean = false;
  OnRadioBtnChnge(arg) {
    if (arg == "Local") {
      this.model.option = arg;
      this.SalesReturnHeaderDetails.BilledBranch = null;
    }
    else {
      this.model.option = arg;
    }
  }
  SalesManList: any;
  getSalesMan() {
    this._masterService.getSalesMan().subscribe(
      response => {
        this.SalesManList = response;
      })
  }

  BilledBranch: any;
  getBilledBranch() {
    this._SalesreturnService.getBilledBranch().subscribe(
      response => {
        this.BilledBranch = response;
      }
    )
  }

  Age: number;

  getSalesReturnData: any;
  GetsalesBillNo(arg) {
    if (arg == '') {
      swal("Warning!", 'Please Enter the Bill Number', "warning");
    }
    else {
      this._SalesreturnService.sendItemDatatoItemComp(null);
      this._SalesreturnService.GetsalesBillNo(arg).subscribe(
        response => {
          this.getSalesReturnData = response;
          if (this.getSalesReturnData != null) {
            this.leavePage = true;
            var days = this.diffDays(this.getSalesReturnData.BillDate, this.applicationDate);
            this.Age = days - 1;
            if (days > 2) {
              var ans = confirm("Age of this sales invoice is more than 2 days (Age:" + this.Age + "-DAYS).Do you want to continue?");
              if (ans) {
                //password protection check
                $('#PermissonModal').modal('show');

              }
            }
            else {
              this.EnableItemDetails = true;
              this._CustomerService.getCustomerDtls(this.getSalesReturnData.CustomerID).subscribe(
                data => {
                  const customerDtls = data;
                  this._CustomerService.sendCustomerDtls_To_Customer_Component(customerDtls);
                  this._SalesreturnService.sendItemDatatoItemComp(this.getSalesReturnData);
                }
              )
            }
          }
        },
        (err) => {
          this.EnableItemDetails = false;
        }
      )
    }
  }

  EnableSR: boolean = false;

  permissonModel: any = {
    CompanyCode: null,
    BranchCode: null,
    PermissionID: null,
    PermissionData: null
  }

  passWord(arg) {
    if (arg == "") {
      swal("Warning!", 'Please Enter the Password', "warning");
      $('#PermissonModal').modal('show');
      this.EnableItemDetails = false;
    }
    else {
      this.permissonModel.CompanyCode = this.ccode;
      this.permissonModel.BranchCode = this.bcode;
      this.permissonModel.PermissionID = this.SRPermission;
      this.permissonModel.PermissionData = btoa(arg);
      this._masterService.postelevatedpermission(this.permissonModel).subscribe(
        response => {
          this.EnableItemDetails = true;
          this._CustomerService.getCustomerDtls(this.getSalesReturnData.CustomerID).subscribe(
            data => {
              const customerDtls = data;
              this._CustomerService.sendCustomerDtls_To_Customer_Component(customerDtls);
              this._SalesreturnService.sendItemDatatoItemComp(this.getSalesReturnData);
            }
          )
          $('#PermissonModal').modal('hide');
        },
        (err) => {
          if (err.status === 401) {
            $('#PermissonModal').modal('show');
            this.EnableItemDetails = false;
          }
        }
      )
    }
  }

  diffDays(d1, d2) {
    var dt1;
    var dt2;
    dt1 = new Date(d1);
    dt2 = new Date(d2);
    return Math.floor((Date.UTC(dt2.getFullYear(), dt2.getMonth(), dt2.getDate()) - Date.UTC(dt1.getFullYear(), dt1.getMonth(), dt1.getDate())) / (1000 * 60 * 60 * 24));
  }

  CustomerName: any;
  //Hide Show data when accordian collapsed(Header)
  Customer: any;
  EnableCustomerTab: boolean = true;
  EnableItemsTab: boolean = false;
  NoRecordsItems: boolean = true;
  EnablePaymentsTab: boolean = false;
  ToggleCustomer: boolean = false;
  ToggleItem: boolean = false;

  public CollapseCustomerTab: boolean = true;
  public CollapseCustomerDetailsTab: boolean = true;
  NoRecordsCustomer: boolean = false;
  filterRadioBtns: boolean = false;


  GetCustomerDetsFromCustComp() {
    this._CustomerService.cast.subscribe(
      response => {
        this.CustomerName = response;
        if (this.isEmptyObject(this.CustomerName) == false && this.isEmptyObject(this.CustomerName) != null) {
          this.CollapseCustomerTab = true;
          this.NoRecordsCustomer = true;
          this.CollapseCustomerDetailsTab = true;
        }
        else {
          this.CollapseCustomerTab = false;
          this.NoRecordsCustomer = false;
        }
      });
  }

  isEmptyObject(obj) {
    return (obj && (Object.keys(obj).length === 0));
  }

  //Hide Show data when accordian collapsed(Customer)
  //Data visible when collapse
  ToggleCustomerData() {
    this.CollapseCustomerTab = !this.CollapseCustomerTab;
    this.CollapseCustomerDetailsTab = !this.CollapseCustomerDetailsTab;
  }

  ToggleItemData() {
    this.EnableItemsTab = !this.EnableItemsTab;
  }

  SalesReturnHeaderDetails: any = {
    CustomerID: null,
    BilledBranch: null,
    SalCode: null,
    Remarks: null,
    SalesReturnDate: null
  }

  ngOnDestroy() {
    this._CustomerService.SendCustDataToEstComp(null);
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
    this._SalesreturnService.SendSRBillNoToReprintComp(null);
    this._SalesreturnService.SendSRNoToReprintComp(null);
    this.routerUrl = this._router.url;
  }
}