import { PaymentService } from './../../payment/payment.service';
import { FormGroup } from '@angular/forms';
import { Validators } from '@angular/forms';
import { CustomerService } from './../../masters/customer/customer.service';
import { OrdersService } from './../orders.service';
import { FormBuilder } from '@angular/forms';
import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { ComponentCanDeactivate } from '../../appconfirmation-guard';
import swal from 'sweetalert';
import { Router } from '@angular/router';
import { AppConfigService } from '../../AppConfigService'

@Component({
  selector: 'app-close-order',
  templateUrl: './close-order.component.html',
  styleUrls: ['./close-order.component.css']
})
export class CloseOrderComponent implements OnInit, OnDestroy, ComponentCanDeactivate {

  constructor(private fb: FormBuilder, private _OrdersService: OrdersService,
    private _CustomerService: CustomerService, private _paymentservice: PaymentService,
    private _router: Router,
    private _appConfigService: AppConfigService) {
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
  }
  leavePage: boolean = false;
  OrderCloseForm: FormGroup;
  paymentPaidAmt: number = 0;
  OrderAmount: number = 0;

  // @ViewChild("OrderNo") OrderNo: ElementRef;

  @ViewChild("OrderNo", { static: true }) OrderNo: ElementRef;

  totalOrderAmt: number = 0;
  ifPaymentDetails: boolean = false;

  ClosedOrderNo: string = "";
  EnableReprintClosedOrderNo: boolean = false;

  EnableJson: boolean = false;
  password: string;
  autoFetchAmount: number = 0;

  paymentDetails: any = [];
  public pageName = "OC";

  ngOnInit() {
    this.DetailsToCustomerComp();
    this._paymentservice.castData.subscribe(
      response => {
        this.paymentDetails = response;
        if (this.paymentDetails != null) {
          this.paymentPaidAmt = 0;
          if (this.OrderCloseObj != null) {
            this.OrderCloseObj.lstOfPayment.forEach((d) => {
              this.paymentPaidAmt += parseInt(d.PayAmount);
            });
            this.totalOrderAmt = Math.round(this.OrderCloseObj.AdvacnceOrderAmount - this.paymentPaidAmt);
            this.autoFetchAmount = this.totalOrderAmt;
            this.leavePage = true;
          }
          this.ifPaymentDetails = true;
          if (this.paymentDetails.PayAmount == null) {
            this.ifPaymentDetails = false;
          }
          if (this.OrderCloseObj.lstOfPayment.length == 0) {
            this.leavePage = false;
          }
        }
        return this.paymentPaidAmt;
      }
    )

    this.OrderCloseForm = this.fb.group({
      OrderNo: ['null', Validators.required],
      CustID: null,
      CustName: null,
      Address1: null,
      AdvacnceOrderAmount: null,
      OrderDate: null,
      MobileNo: null,
      City: null
    });
  }
  hasOrderDetails: boolean = false;
  previousPaymentDetails: number;
  SelectedItemLines: any = [];
  customerDtls: any = [];
  getOrderDetails(arg) {
    if (arg == '') {
      alert("Enter Order Number");
    }
    else {
      this._OrdersService.getOrderReceiptDetails(arg).subscribe(
        response => {
          this.OrderCloseObj = response;
          if (this.OrderCloseObj != null) {
            this.hasOrderDetails = true;
            this.paymentPaidAmt = 0.00;
            this.OrderCloseObj.AdvacnceOrderAmount = Math.round(this.OrderCloseObj.AdvacnceOrderAmount);
            this.totalOrderAmt = this.OrderCloseObj.AdvacnceOrderAmount;
            this.autoFetchAmount = this.totalOrderAmt;
            this._CustomerService.getCustomerDtls(this.OrderCloseObj.CustID).subscribe(
              response => {
                this.customerDtls = response;
                this._CustomerService.sendCustomerDtls_To_Customer_Component(this.customerDtls);
                this._paymentservice.inputData(this.OrderCloseObj);
                //this.EnablePaymentsTab = false;
                this.OrderAmount = this.OrderCloseObj.AdvacnceOrderAmount;
              }
            )
          }
        },
        (err) => {
          if (err.status === 404) {
            const validationError = err.error;
            swal("Closed!", validationError.description, "error");
          }
          if (err.status === 400) {
            const validationError = err.error;
            swal("Closed!", validationError.description, "error");
          }
          this.ngOnDestroy();
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

  OrderCloseObj: any = {
    CustID: null,
    CustName: null,
    Address1: null,
    City: null,
    MobileNo: null,
    OrderDate: null,
    SalCode: null,
    OrderNo: null,
    AdvacnceOrderAmount: null,
    lstOfOrderItemDetailsVM: [],
    lstOfPayment: []
  }

  //Hide Show data when accordian collapsed(Header)
  Customer: any;
  EnableCustomerTab: boolean = true;
  ToggleCustomer: boolean = false;

  TogglePayments: boolean = false;
  EnablePaymentsTab: boolean = true;

  ToggleCustomerData() {
    if (this.ToggleCustomer == true) {
      this.EnableCustomerTab = !this.EnableCustomerTab;
    }
    else {
      swal("Warning", "Please enter Order No", "warning");
    }
  }

  TogglePaymentData() {
    if (this.ToggleCustomer == true) {
      this.EnablePaymentsTab = !this.EnablePaymentsTab;
    }
    else {
      swal("Warning", "Please enter Order No", "warning");
    }
  }

  DetailsToCustomerComp() {
    this._CustomerService.cast.subscribe(
      response => {
        this.Customer = response;
        if (this.isEmptyObject(this.Customer) == false && this.isEmptyObject(this.Customer) != null) {
          this.EnableCustomerTab = true;
          this.ToggleCustomer = true;

          this.EnablePaymentsTab = true;
          this.TogglePayments = true;

        }
        else {
          this.EnableCustomerTab = true;
          this.ToggleCustomer = false;

          this.EnablePaymentsTab = true;
          this.TogglePayments = false;
        }
      });
  }

  //Check object is empty
  isEmptyObject(obj) {
    return (obj && (Object.keys(obj).length === 0));
  }


  submitPost() {
    this.EnableReprintClosedOrderNo = false;
    if (this.paymentPaidAmt == this.OrderAmount) {
      var ans = confirm("Do you want to Close Order??");
      if (ans) {
        this._OrdersService.SendOrderNoToReprintComp(null);
        this._OrdersService.closeOrder(this.OrderCloseObj).subscribe(
          response => {
            this.EnableReprintClosedOrderNo = true;
            this.ClosedOrderNo = this.OrderCloseObj.OrderNo;
            swal("Closed!", "Order number " + this.OrderCloseObj.OrderNo + " Closed successfully!!", "success");
            this.OrderCloseForm.reset();
            this.ngOnDestroy();
            this.leavePage = false;
            this.OrderNo.nativeElement.value = "";
          }
        )
      }
    }
    else {
      swal("Warning", "Please enter Balance payment", "warning");
    }
  }

  Clear() {
    this.ngOnDestroy();
  }

  ngOnDestroy() {
    this.OrderNo.nativeElement.value = "";
    this._paymentservice.SendPaymentSummaryData(null);
    this._CustomerService.SendCustDataToEstComp(null);
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
    this._paymentservice.outputData(null);
    this.EnablePaymentsTab = true;
    this.OrderCloseObj = null;
    this.paymentPaidAmt = 0.00;
    this.totalOrderAmt = 0.00;
    this.hasOrderDetails = false;
  }
}