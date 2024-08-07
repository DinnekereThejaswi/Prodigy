import { AfterContentChecked, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SalesBillingService } from '../sales-billing.service';
import swal from 'sweetalert';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import { ToastrService } from 'ngx-toastr';
import { MasterService } from '../../core/common/master.service';
declare var $: any;

@Component({
  selector: 'app-cancel-salesbilling',
  templateUrl: './cancel-salesbilling.component.html',
  styleUrls: ['./cancel-salesbilling.component.css']
})
export class CancelSalesbillingComponent implements OnInit, AfterContentChecked {
  SalesbillingCancelForm: FormGroup;
  applicationDate: any;
  @ViewChild("Pwd", { static: true }) Pwd: ElementRef;
  @ViewChild("BillNo", { static: true }) BillNo: ElementRef;

  Permission: string;
  ccode: string = "";
  bcode: string = "";
  PrintDetails: any = [];
  billNo: string = "";
  hasSalesBillDetails: boolean = false;
  cancelRemarks: string = "";
  password: string;
  SalesBillingData: any;
  AttachSalesBillForm: FormGroup;
  EnableJson: boolean = false;
  SalesBillingCancelObj: any = {
    CancelRemarks: null,
  }
  constructor(private _salesBillingService: SalesBillingService,
    private fb: FormBuilder, private _appConfigService: AppConfigService,
    private toastr: ToastrService, private _masterService: MasterService) {
    this.EnableJson = this._appConfigService.EnableJson;
    this.Permission = this._appConfigService.RateEditCode.SalesBillPermission;
    this.password = this._appConfigService.Pwd;
    this.getCB();
    $('#mySalesBillNoModal').modal('hide');
  }


  SearchParams: any = [
    {
      "Key": "BillNo",
      "Value": "BillNo"
    },
    {
      "Key": "Name",
      "Value": "Name"
    }
  ];



  ngOnInit() {
    this.SalesbillingCancelForm = this.fb.group({
      Remarks: null
    });
    this.getApplicationDate();
    this.AttachSalesBillForm = this.fb.group({
      searchby: [null, Validators.required],
      searchText: [null, Validators.required],
    });
  }

  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
      }
    )
  }

  permissonModel: any = {
    CompanyCode: null,
    BranchCode: null,
    PermissionID: null,
    PermissionData: null
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngAfterContentChecked() {
    this.Pwd.nativeElement.focus();
  }

  getSalesBillDetails(arg) {
    this.SalesBillingData=[];
    if (arg == null || arg == "") {
      swal("Warning!", 'Please enter the bill no', "warning");
      $('#PermissonSalesBillingModal').modal('hide');
    }
    else {
      this.SalesbillingCancelForm.reset();
      this.billNo = arg;
      $('#PermissonSalesBillingModal').modal('hide');
      this._salesBillingService.getCancelBillNo(this.billNo).subscribe(
        response => {
          this.PrintDetails = response;
          if (this.PrintDetails != null) {
            this.hasSalesBillDetails = true;
            this.SalesBillingData = this.PrintDetails;
            $('#PermissonSalesBillingModal').modal('hide');
          }
        },
        (err) => {
          if (err.status === 400) {
            const validationError = err.error;
            swal("Warning!", validationError.description, "warning");
            this.hasSalesBillDetails = false;
          }
        }
      )
    }
  }

  submitPost() {
    if (this.SalesBillingCancelObj.CancelRemarks == "" || this.SalesBillingCancelObj.CancelRemarks == null) {
      swal("Warning!", 'Please enter the remarks', "warning");
    }
    else {
      var ans = confirm("Do you want to cancel Sales Bill No." + this.billNo + "??");
      if (ans) {
        this._salesBillingService.CancelSalesBill(this.SalesBillingCancelObj.CancelRemarks, this.billNo).subscribe(
          response => {
            swal("Cancelled!", "Sales Bill No. " + this.billNo + " Cancelled Successfully", "success");
            this.SalesbillingCancelForm.reset();
            this.hasSalesBillDetails = false;
            this.Clear();
          }
        )
      }
    }
  }

  getSalesBillNos() {
    if (this.billNo != "0" && this.billNo != "") {
      $('#mySalesBillNoModal').modal('hide');
      $('#PermissonSalesBillingModal').modal('hide');
      this.Pwd.nativeElement.value = "";
    }
    else {
      $('#mySalesBillNoModal').modal('hide');
      $('#PermissonSalesBillingModal').modal('show');
      this.Pwd.nativeElement.value = "";
    }
  }


  passWord(arg) {
    if (arg == "") {
      swal("Warning!", 'Please Enter the Password', "warning");
      $('#PermissonRepairReceiptsModal').modal('show');
    }
    else {
      this.permissonModel.CompanyCode = this.ccode;
      this.permissonModel.BranchCode = this.bcode;
      this.permissonModel.PermissionID = this.Permission;
      this.permissonModel.PermissionData = btoa(arg);
      this._masterService.postelevatedpermission(this.permissonModel).subscribe(
        response => {
          $('#PermissonSalesBillingModal').modal('hide');
          $("#mySalesBillNoModal").modal('show');
          this.getSalesBillList();
        },
        (err) => {
          if (err.status === 401) {
            $('#PermissonRepairReceiptsModal').modal('show');
            $('#mySalesBillNoModal').modal('hide');
          }
        }
      )
    }
  }

  isChecked: boolean = false;
  ReprintbillNo: any;


  getSalesBillList() {
    this._salesBillingService.getBillNo(this.applicationDate, this.isChecked).subscribe(
      response => {
        this.ReprintbillNo = response;
      }
    )
  }

  selectRecord(arg) {
    this.BillNo.nativeElement.value = arg.BillNo;
    $("#mySalesBillNoModal").modal('hide');
    this.getSalesBillDetails(arg.BillNo);
  }

  attachSalesBillNoList: any;

  getSearchParams(form) {
    if (form.value.searchby != null && form.value.searchText != null) {
      this._salesBillingService.getAttachCancelBillNo(form.value.searchby, form.value.searchText).subscribe(
        response => {
          this.ReprintbillNo = response;
        }
      )
    }
  }

  close() {
    this.AttachSalesBillForm.reset();
  }

  ModalClose() {
    this.AttachSalesBillForm.reset();
  }

  submitted = false;

  onSubmit() {
    this.submitted = true;
    if (this.AttachSalesBillForm.invalid) {
      return;
    }
  }

  Clear() {
    this.BillNo.nativeElement.value = '';
    this.AttachSalesBillForm.reset();
    $('#mySalesBillNoModal').modal('hide');
    $('#PermissonSalesBillingModal').modal('hide');
    this.Pwd.nativeElement.value = "";
    this.PrintDetails = [];
    this.hasSalesBillDetails = false;
    this.billNo = "";
  }
}