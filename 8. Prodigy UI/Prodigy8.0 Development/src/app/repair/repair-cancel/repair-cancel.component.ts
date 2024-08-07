import { RepairService } from './../repair.service';
import { CustomerService } from './../../masters/customer/customer.service';
import { FormGroup, Validators } from '@angular/forms';
import { FormBuilder } from '@angular/forms';
import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterContentChecked } from '@angular/core';
import swal from 'sweetalert';
import { ComponentCanDeactivate } from './../../appconfirmation-guard';
import { AppConfigService } from '../../AppConfigService';
import { MasterService } from '../../core/common/master.service';
import { OrdersService } from '../../orders/orders.service';
import { ToastrService } from 'ngx-toastr';
import * as CryptoJS from 'crypto-js';
declare var $: any;

@Component({
  selector: 'app-repair-cancel',
  templateUrl: './repair-cancel.component.html',
  styleUrls: ['./repair-cancel.component.css']
})
export class RepairCancelComponent implements OnInit, OnDestroy, ComponentCanDeactivate, AfterContentChecked {
  RepairCancelForm: FormGroup;
  password: string;
  EnableJson: boolean = false;
  RepairReceiptCancelPermission: string;
  constructor(private fb: FormBuilder, private _CustomerService: CustomerService,
    private _repairService: RepairService, private _orderservice: OrdersService,
    private formBuilder: FormBuilder, private toastr: ToastrService,
    private appConfigService: AppConfigService, private _masterService: MasterService) {
    this.RepairReceiptCancelPermission = this.appConfigService.RateEditCode.RepairReceiptCancelPermission;
    this.EnableJson = this.appConfigService.EnableJson;
    this.password = this.appConfigService.Pwd;
    this.getCB();
    $('#myRepairReceiptModal').modal('hide');
  }
  ccode: string = "";
  bcode: string = "";
  AttachRepairReceiptForm: FormGroup;
  RepairReceiptList: any;
  SearchParams: any;
  @ViewChild("Pwd", { static: true }) Pwd: ElementRef;

  @ViewChild("RepairNo", { static: true }) Repairno: ElementRef;

  RepairNo: string = "";


  hasRepairDetails: boolean = false;
  leavePage: boolean = false;

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this.getSalesMan();
    this.RepairCancelForm = this.fb.group({
      RepairNo: ['null', Validators.required],
      CustID: null,
      CustName: null,
      Address1: null,
      Address2: null,
      Address3: null,
      Country: null,
      RepairDate: null,
      DueDate: null,
      MobileNo: null,
      City: null,
      EmailID: null,
      Remarks: null,
      SalCode: null
    });


    this.AttachRepairReceiptForm = this.formBuilder.group({
      searchby: [null, Validators.required],
      searchText: [null, Validators.required],
    });

    this.RepairCancelForm.controls["Remarks"].valueChanges.subscribe(change => {
      if (change != null && change != "") {
        this.leavePage = true;
      }
      else {
        this.leavePage = false;
      }
    })
  }

  SalesManList: any;
  getSalesMan() {
    this._repairService.getSalesManData().subscribe(
      response => {
        this.SalesManList = response;
      })
  }

  customerDtls: any = [];
  getReceiptDetails(arg) {
    if (arg == null || arg == "") {
      swal("Warning!", 'Please Enter Receipt Number', "warning");
      $('#PermissonRepairReceiptsModal').modal('hide');
    }
    else {
      this.RepairNo = arg;
      this.getReceiptNoDetails(arg);
    }
  }

  ngAfterContentChecked() {
    this.Pwd.nativeElement.focus();
  }

  RepairCancelObj: any = {
    CustID: null,
    CustName: null,
    Address1: null,
    Address2: null,
    Address3: null,
    Country: null,
    City: null,
    MobileNo: null,
    OrderDate: null,
    SalCode: null,
    RepairDate: null,
    DueDate: null,
    EmailID: null,
    Remarks: null,
    CompanyCode: null,
    BranchCode: null,
    RepairNo: null,
    lstOfOrderItemDetailsVM: [],
    lstOfPayment: []
  }

  ngOnDestroy() {
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
  }

  submitPost(arg) {
    this.RepairCancelObj.CompanyCode = this.ccode;
    this.RepairCancelObj.BranchCode = this.bcode;
    if (this.RepairCancelObj.Remarks == "" || this.RepairCancelObj.Remarks == null) {
      swal("Warning!", 'Please Enter Remarks to Cancel', "warning");
    }
    else {
      var ans = confirm("Do you want to cancel??");
      if (ans) {
        this._repairService.cancelReceipts(this.RepairCancelObj).subscribe(
          response => {
            swal("Cancelled!", "Repair number " + this.RepairCancelObj.RepairNo + " Cancelled Successfully", "success");
            this.leavePage = false;
            this.Clear();
          }
        )
      }
    }
  }

  RepairReceipt() {
    if (this.RepairNo != "0" && this.RepairNo != "") {
      $('#myRepairReceiptModal').modal('hide');
      $('#PermissonRepairReceiptsModal').modal('hide');
      this.Pwd.nativeElement.value = "";
    }
    else {
      $('#myRepairReceiptModal').modal('hide');
      $('#PermissonRepairReceiptsModal').modal('show');
      this.Pwd.nativeElement.value = "";
    }
  }

  permissonModel: any = {
    CompanyCode: null,
    BranchCode: null,
    PermissionID: null,
    PermissionData: null
  }


  passWord(arg) {
    if (arg == "") {
      swal("Warning!", 'Please Enter the Password', "warning");
      $('#PermissonRepairReceiptsModal').modal('show');
    }
    else {
      this.permissonModel.CompanyCode = this.ccode;
      this.permissonModel.BranchCode = this.bcode;
      this.permissonModel.PermissionID = this.RepairReceiptCancelPermission;
      this.permissonModel.PermissionData = btoa(arg);
      this._masterService.postelevatedpermission(this.permissonModel).subscribe(
        response => {
          $('#PermissonRepairReceiptsModal').modal('hide');
          $("#myRepairReceiptModal").modal('show');
          this.getSearchParamsList();
          this.getRepairReceiptList(null, null);
        },
        (err) => {
          if (err.status === 401) {
            $('#PermissonRepairReceiptsModal').modal('show');
            $('#myRepairReceiptModal').modal('hide');
          }
        }
      )
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

  getSearchParamsList() {
    this._repairService.getRepairReceiptSearchParams().subscribe(
      response => {
        this.SearchParams = response;
      }
    )
  }

  getRepairReceiptList(searchType, searchValue) {
    this._repairService.getAllRepairReceipt(searchType, searchValue).subscribe(
      response => {
        this.RepairReceiptList = response;
      }
    )
  }


  selectRecord(arg) {
    this.Repairno.nativeElement.value = arg.RepairNo;
    $("#myRepairReceiptModal").modal('hide');
    this.getReceiptNoDetails(arg.RepairNo);
    this.AttachRepairReceiptForm.reset();
  }

  submitted = false;

  close() {
    this.AttachRepairReceiptForm.reset();
  }

  ModalClose() {
    this.AttachRepairReceiptForm.reset();
  }

  onSubmit() {
    this.submitted = true;
    if (this.AttachRepairReceiptForm.invalid) {
      return;
    }
  }

  getReceiptNoDetails(arg) {
    $('#PermissonRepairReceiptsModal').modal('hide');
    this._repairService.getReceiptNo(arg).subscribe(
      response => {
        this.RepairCancelObj = response;
        this.getSalesMan()
        // this.RepairCancelObj.SalCode = this.RepairCancelObj.SalCode;
        if (this.RepairCancelObj != null) {
          this.hasRepairDetails = true;
          this._CustomerService.getCustomerDtls(this.RepairCancelObj.CustID).subscribe(
            response => {
              this.customerDtls = response;
              this._CustomerService.sendCustomerDtls_To_Customer_Component(this.customerDtls);
              this.RepairCancelObj.Country = this.customerDtls.CountryName;
              $('#PermissonRepairReceiptsModal').modal('hide');
            }
          )
        }
      },
      (err) => {
        if (err.status === 404) {
          const validationError = err.error;
          swal("Warning!", validationError.description, "warning");
          this.Clear();
        }
        if (err.status === 400) {
          const validationError = err.error;
          swal("Warning!", validationError.description, "warning");
          this.Clear();
        }
      }
    )
  }
  getSearchParams(form) {
    if (form.value.searchby != null && form.value.searchText != null) {
      this.getRepairReceiptList(form.value.searchby, form.value.searchText);
    }
    else {
      swal("Warning!", "Please select the search fields", "warning");
    }
  }

  Clear() {
    this.AttachRepairReceiptForm.reset();
    $('#myRepairReceiptModal').modal('hide');
    $('#PermissonRepairReceiptsModal').modal('hide');
    this.RepairCancelForm.reset();
    this.Repairno.nativeElement.value = "";
    this.Pwd.nativeElement.value = "";
    this.hasRepairDetails = false;
    this.RepairNo = "";
    this.RepairCancelObj = {
      CustID: null,
      CustName: null,
      Address1: null,
      Address2: null,
      Address3: null,
      Country: null,
      City: null,
      MobileNo: null,
      OrderDate: null,
      SalCode: null,
      RepairDate: null,
      DueDate: null,
      EmailID: null,
      Remarks: null,
      CompanyCode: null,
      BranchCode: null,
      RepairNo: null,
      lstOfOrderItemDetailsVM: [],
      lstOfPayment: []
    }
  }
}