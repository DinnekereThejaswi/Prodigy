import { SalesService } from '../../sales/sales.service';
import { Component, OnInit, OnDestroy, Output, EventEmitter, ElementRef, ViewChild, AfterContentChecked } from '@angular/core';
import { CustomerService } from '../../masters/customer/customer.service';
import { OrdersService } from './../orders.service';
import { PurchaseService } from './../../purchase/purchase.service';
import { estimationService } from '../../estimation/estimation.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MasterService } from '../../core/common/master.service';
import { AppConfigService } from '../../AppConfigService';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import swal from 'sweetalert';
import * as CryptoJS from 'crypto-js';
declare var $: any;


@Component({
  selector: 'app-attachorder',
  templateUrl: './attachorder.component.html',
  styleUrls: ['./attachorder.component.css']
})
export class AttachorderComponent implements OnInit, OnDestroy, AfterContentChecked {
  ccode: string = "";
  bcode: string = "";
  password: string;
  OrderAmount: number = 0;
  AllRecordCount: any;
  searchText: string = "";
  CustomerName: any;
  PostOrderAttchJson: any = [];
  //SearchParamsList: any;
  AttachOrderList: any;
  AttachOrderForm: FormGroup;
  submitted = false;
  // @ViewChild("pwdOrderAttach") pwdOrderAttach: ElementRef;

  @ViewChild("pwdOrderAttach", { static: true }) pwdOrderAttach: ElementRef;

  SelectedOrderList: any = [];
  EnableSubmitButton: boolean = true;
  @Output() valueChange = new EventEmitter();

  SalesEstNo: string = null;
  EnableDisablectrls: boolean = true;
  OrderAttachmentPermission: string;
  EnableJson: boolean = false;

  OrderAttachmentSummaryData: any = {
    Amount: 0.00
  };

  constructor(private _PurchaseService: PurchaseService, private formBuilder: FormBuilder,
    private _estimationService: estimationService, private _salesService: SalesService,
    private _CustomerService: CustomerService, private _OrdersService: OrdersService,
    private _router: Router, private toastr: ToastrService, private appConfigService: AppConfigService,
    private _masterService: MasterService) {
    this.OrderAttachmentPermission = this.appConfigService.RateEditCode.OrderAttachmentPermission;
    this.EnableJson = this.appConfigService.EnableJson;
    this.password = this.appConfigService.Pwd;
    this.getCB();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }


  GlobalOrderAmount: any = 0;

  OrderJson: any = {
    CompanyCode: this.ccode,
    BranchCode: this.bcode,
    CustomerID: null,
    SeriesNo: null,     // SeriesNo: this.EstNoFromPurchaseComp,
    RefBillNo: null,
    OperatorCode: null,
    SlNo: null
  };

  ngOnInit() {
    this.OrderAttachmentSummaryData = {
      Amount: 0.00
    };

    this._estimationService.SubjectEstNo.subscribe(
      response => {
        this.SalesEstNo = response;
        if (this.SalesEstNo != null && this.SalesEstNo != "") {
          if (this._router.url != "/purchase") {
            this._OrdersService.getOrderAttachedList(this.SalesEstNo).subscribe(
              response => {
                this.SelectedOrderList = [];
                this.PostOrderAttchJson = [];
                this.SelectedOrderList = response;
                if (this.SelectedOrderList.length > 0) {
                  this._estimationService.SendOrderAttachmentSummaryData(this.OrderAttachmentSummaryData);
                  for (var i = 0; i < this.SelectedOrderList.length; i++) {
                    this.OrderJson.SeriesNo = this.SalesEstNo;
                    this.OrderJson.RefBillNo = this.SelectedOrderList[i]["OrderNo"];
                    this.OrderJson.OperatorCode = localStorage.getItem('Login');
                    this.OrderJson.CompanyCode = this.ccode,
                      this.OrderJson.CustomerID = this.SelectedOrderList[i]["CustomerID"];
                    this.OrderJson.BranchCode = this.bcode
                    this.PostOrderAttchJson.push(this.OrderJson);
                    this.OrderJson = {};
                    this.markFormGroupPristine(this.AttachOrderForm);
                  }
                }
              }
            )
          }
        }
      }
    )


    this.AttachOrderForm = this.formBuilder.group({
      searchby: [null, Validators.required],
      searchText: [null, Validators.required],
    });

    // this._OrdersService.getSearchParams().subscribe(
    //   response => {
    //     this.SearchParamsList = response;
    //   }
    // )
    this.getSalesEstNofromSalesService();
    this.LoadCustomerDetails();
    if (this._router.url == "/sales-billing") {
      this.EnableDisablectrls = false;
    }
    if (this._router.url == "/estimation") {
      this.EnableDisablectrls = true;
    }
  }

  ngAfterContentChecked() {
    this.pwdOrderAttach.nativeElement.focus();
  }

  openModal() {
    this.pwdOrderAttach.nativeElement.value = "";
    $("#myOrderModal").modal('hide');
    $("#OrderAttachmentPermissonModal").modal('show');
    this.AttachOrderForm.reset();
    this.AttachOrderList = [];
  }

  totalItems: any;
  pagenumber: number = 1;
  top = 50;
  skip = (this.pagenumber - 1) * this.top;

  onPageChange(p: number) {
    this.pagenumber = p;
    const skipno = (this.pagenumber - 1) * this.top;
    this.getOrderList(this.top, skipno);
  }

  getOrderList(top, skip) {
    this.top = 50;
    this.skip = 0;
    this.AttachOrderForm.reset();
    this._OrdersService.getAllOrdersCount().subscribe(
      response => {
        this.AllRecordCount = response;
        this.totalItems = this.AllRecordCount.RecordCount;
        if (this.totalItems > 0) {
          this._OrdersService.getOrderList(top, skip).subscribe(
            response => {
              this.AttachOrderList = response;
            }
          )
        }
      }
    )
  }

  valueChanged(Amount) { // You can give any function name
    this.OrderAmount = Amount;
    this.valueChange.emit(this.OrderAmount);
  }

  OrderTotalAmount: number = 0;

  @Output() OrderAmountChange = new EventEmitter();

  OrderAmountChanged(SalesAmount) {
    this.OrderTotalAmount = SalesAmount;
    this.OrderAmountChange.emit(this.OrderTotalAmount);
  }

  LoadCustomerDetails() {
    this._CustomerService.cast.subscribe(
      response => {
        this.CustomerName = response;
      })
  }

  enableBtnIfSalesEstNo: boolean = true;

  getSalesEstNofromSalesService() {

    this._salesService.EstNo.subscribe(
      response => {
        this.SalesEstNo = response;
        if (this.SalesEstNo != null && this.SalesEstNo != "") {
          this.enableBtnIfSalesEstNo = false;
        }
      }
    )
  }

  Saved: any;


  ReprintData: any = {
    EstNo: null,
    EstType: null,
  }

  Submit() {
    if (this.EnableSubmitButton == true && this.PostOrderAttchJson.length == 0) {
      swal("Warning!", "Please attach the order", "warning");
    }
    // else if (this.EnableSubmitButton == true) {
    //   swal("Warning!", "No changes were made to submit", "warning");
    // }
    else if (this.PostOrderAttchJson.length == 0) {
      swal("Warning!", 'Please attach the order', "warning");
    }
    else if (this.AttachOrderForm.pristine == true) {
      swal("Warning!", "No changes were made to submit", "warning");
    }
    else {
      this._OrdersService.PostAttachementOrder(this.PostOrderAttchJson).subscribe(
        response => {
          this.Saved = response;
          swal("Saved!", "Orders updated against Estimation number " + this.SalesEstNo, "success");
          this.ReprintData = {
            EstNo: this.SalesEstNo,
            EstType: "OG"
          }
          this._estimationService.SendReprintData(this.ReprintData);
          this._estimationService.SendOrderAttachmentSummaryData(this.OrderAttachmentSummaryData);
          this.markFormGroupPristine(this.AttachOrderForm);
          //this.getOrderSalesEstimationPlainText(this.SalesEstNo);
          // this._router.navigateByUrl('/branch', { skipLocationChange: true }).then(() =>
          //     this._router.navigate(['/estimation'])) 
        }
      )
    }
  }

  OrdersalesEstimationPlainTextDetails: any = [];

  getOrderSalesEstimationPlainText(arg) {
    this._estimationService.getSalesEstimationPlainText(arg).subscribe(
      response => {
        this.OrdersalesEstimationPlainTextDetails = response;
        $('#OrderSalesEstimationPlainTextTab').modal('show');
      }
    )
  }

  Close() {
    $('#OrderSalesEstimationPlainTextTab').modal('show');
    this._estimationService.SendOrderAttachmentSummaryData(this.OrderAttachmentSummaryData);
  }

  printOrderSalesEstimationPlainText() {
    this._masterService.printPlainText(this.OrdersalesEstimationPlainTextDetails);
    //this._estimationService.SendOrderAttachmentSummaryData(this.OrderAttachmentSummaryData);
  }

  onSubmit() {
    this.submitted = true;
    if (this.AttachOrderForm.pristine) {
      return;
    }
  }

  DeleteRow(index: number) {
    var ans = confirm("Do you want to delete");
    if (ans) {
      if (this.SelectedOrderList.length == 1) {
        // alert('Order Reserved'+this.SelectedOrderList[index].orderNo)
        // this. _OrdersService.SendReservedOrderNoToSalesComp(this.SelectedOrderList[index].orderNo);
        this._OrdersService.deleteOrderAttachment(this.SalesEstNo).subscribe(
          response => {
            this.Saved = response;
            this.EnableSubmitButton = false;
          }
        )
      }

      this.valueChanged(this.SelectedOrderList[index].Amount);
      this.SelectedOrderList.splice(index, 1);
      this.PostOrderAttchJson.splice(index, 1);
      this.EnableDisableSubmitBtn();
      this.markFormGroupDirty(this.AttachOrderForm);
      
    }
  }

  ReservedOrderList: any;

  selectRecord(arg, index) {
    if (this.SalesEstNo == "" || this.SalesEstNo == null) {
      swal("Warning!", 'Please submit the sales details', "warning");
    }
    else {
      let data = this.SelectedOrderList.find(x => x.OrderNo == arg.OrderNo);
      if (data != null) {
        swal("error", "Order already attached", "error");
      }
      else {
        this._OrdersService.ValidateAttachOrder(this.SalesEstNo, arg.OrderNo).subscribe(
          response => {
            this.ReservedOrderList = response;
            if (this.ReservedOrderList != null) {
              this._OrdersService.SendReservedOrderDetsToSalesComp(this.ReservedOrderList);
              this.ReservedOrderList = null;
            }
            this.SelectedOrderList.push(arg);
            $('#myOrderModal').modal('hide');
            this.OrderJson.SeriesNo = this.SalesEstNo;
            this.OrderJson.RefBillNo = arg.OrderNo;
            this.OrderJson.OperatorCode = localStorage.getItem('Login');
            this.OrderJson.CompanyCode = this.ccode,
              this.OrderJson.BranchCode = this.bcode
            this.OrderJson.SlNo = index + 1;
            this.PostOrderAttchJson.push(this.OrderJson);
            this.OrderJson = {};
            this.EnableDisableSubmitBtn();
            this.markFormGroupDirty(this.AttachOrderForm);
          }
        )
      }
    }
  }

  EnableDisableSubmitBtn() {
    if (this.SelectedOrderList.length <= 0) {
      this.EnableSubmitButton = true;
    }
    else {
      this.EnableSubmitButton = false;
    }
  }

  private markFormGroupPristine(form: FormGroup) {
    Object.values(form.controls).forEach(control => {
      control.markAsPristine();
      if ((control as any).controls) {
        this.markFormGroupPristine(control as FormGroup);
      }
    });
  }


  private markFormGroupDirty(form: FormGroup) {
    Object.values(form.controls).forEach(control => {
      control.markAsDirty();
      if ((control as any).controls) {
        this.markFormGroupDirty(control as FormGroup);
      }
    });
  }



  permissonModel: any = {
    CompanyCode: null,
    BranchCode: null,
    PermissionID: null,
    PermissionData: null
  }


  passWordOrderAttachment(arg) {
    if (arg == "") {
      swal("Warning!", 'Please Enter the Password', "warning");
      $('#OrderAttachmentPermissonModal').modal('show');
    }
    else {
      this.permissonModel.CompanyCode = this.ccode;
      this.permissonModel.BranchCode = this.bcode;
      this.permissonModel.PermissionID = this.OrderAttachmentPermission;
      this.permissonModel.PermissionData = btoa(arg);
      this._masterService.postelevatedpermission(this.permissonModel).subscribe(
        response => {
          $('#OrderAttachmentPermissonModal').modal('hide');
          $("#myOrderModal").modal('show');
          this.getOrderList(this.top, this.skip);
        },
        (err) => {
          if (err.status === 401) {
            $('#OrderAttachmentPermissonModal').modal('show');
          }
        }
      )
    }
  }

  ngOnDestroy() {
    this._CustomerService.SendCustDataToEstComp(null);
    this._estimationService.SendOrderAttachmentSummaryData(null);
    this._estimationService.SendEstNo(null);
  }

  Cancel() {
    $('#myOrderModal').modal('hide');
    this.AttachOrderForm.reset();
    this.AttachOrderList = [];
  }

  ClearField() {
    this.searchText = "";
  }

  getSearchParams(form) {
    if ((form.value.searchby != null && form.value.searchby != "") && (form.value.searchText != null && form.value.searchText != "")) {
      this._OrdersService.getAttachOrderList(form.value.searchby, form.value.searchText).subscribe(
        response => {
          this.AttachOrderList = response;
          this.top = 50;
          this.skip = 0;
          this.totalItems = 50;
        },
        (err) => {
          if (err.status === 400) {
            this.AttachOrderList = [];
          }
        }
      )
    }
    else {
      swal("Warning!", 'Please select required fields', "warning");
      // this.AttachOrderList=[];
    }
  }
  Amount(arg: any[] = []) { let total = 0; arg.forEach((d) => { total += d.Amount; }); this.OrderAttachmentSummaryData.Amount = total; this.OrderAmountChanged(total); return total; }
}