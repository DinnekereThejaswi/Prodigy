import { ComponentCanDeactivate } from './../../appconfirmation-guard';
import { PaymentService } from './../../payment/payment.service';
import { CustomerService } from './../../masters/customer/customer.service';
import { FormGroup } from '@angular/forms';
import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { OrdersService } from '../orders.service';
import swal from 'sweetalert';
import { Router } from '@angular/router';
import { MasterService } from '../../core/common/master.service';
import { AppConfigService } from '../../AppConfigService';

declare var $: any;

@Component({
  selector: 'app-order-receipt',
  templateUrl: './order-receipt.component.html',
  styleUrls: ['./order-receipt.component.css']
})

export class OrderReceiptComponent implements OnInit, ComponentCanDeactivate {

  OrderReceiptForm: FormGroup;
  paymentDetails: any = [];
  paymentPaidAmt: number = 0;
  routerUrl: string = "";
  totalOrderAmt: number;
  ifPaymentDetails: boolean = false;
  @ViewChild('OrderNo', { static: true }) OrderNo: ElementRef;

  public pageName = "OR";
  EnableReprintReceipt: boolean = false;
  hasOrderDetails: boolean = false;
  leavePage: boolean = false;
  EnableJson: boolean = false;
  password: string;
  constructor(private _OrdersService: OrdersService, private _CustomerService: CustomerService,
    private _paymentservice: PaymentService, private _router: Router,
    private _masterService: MasterService, private _appConfigService: AppConfigService
  ) {
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
  }

  ngOnInit() {
    this.DetailsToCustomerComp();
    this._paymentservice.castData.subscribe(
      response => {
        this.paymentDetails = response;
        if (this.isEmptyObject(this.paymentDetails) == false && this.isEmptyObject(this.paymentDetails) != null) {
          this.leavePage = true;
          this.paymentPaidAmt = 0;
          this.orderDetails.lstOfPayment.forEach((d) => {
            this.paymentPaidAmt += parseInt(d.PayAmount);
          });
          this.totalOrderAmt = (this.orderDetails.lstOfOrderItemDetailsVM[0].Amount + this.paymentPaidAmt);

          if (this.orderDetails.lstOfPayment.length == 0) {
            this.leavePage = false;
          }
          return this.paymentPaidAmt;
        }
      }
    )
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

  cancel() {
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(() =>
      this._router.navigate(['/orders/OrderReceipt']))
  }

  orderDetails: any = [];
  previousPaymentDetails: number;
  SelectedItemLines: any = [];
  getOrderDetails(arg) {
    if (arg == '') {
      swal("Warning", "Enter Order Number", "warning");
    }

    else {
      this._OrdersService.getOrderReceiptDetails(arg).subscribe(
        response => {
          this.orderDetails = response;
          if (this.orderDetails != null) {
            this.previousPaymentDetails = this.orderDetails.lstOfOrderItemDetailsVM[0].Amount;
            this.totalOrderAmt = (this.orderDetails.lstOfOrderItemDetailsVM[0].Amount + this.paymentPaidAmt);
            this.ifPaymentDetails = true;
            this._CustomerService.getCustomerDtls(this.orderDetails.CustID).subscribe(
              response => {
                const customerDtls = response;
                this._CustomerService.sendCustomerDtls_To_Customer_Component(customerDtls);
                this._paymentservice.inputData(this.orderDetails);
                this.EnablePaymentsTab = false;
                this.hasOrderDetails = true;
              }
            )
          }
          // if(this.orderDetails.OrderRateType=="Fixed Rate"){
          //   swal("Warning!", "Fixed order cannot create order receipt. Please generate new order.", "error");
          //   this.clearOrderReceiptComponents();
          // }
          // else{
          // }
        },
        (err) => {
          if (err.status === 404) {
            const validationError = err.error;
            swal("Warning!", validationError.description, "error");
            this.clearOrderReceiptComponents();
          }
          if (err.status === 400) {
            const validationError = err.error;
            swal("Warning!", validationError.Message, "error");
          }
        }
      )
    }
  }

  updatePan(arg) {
    if (arg != null && arg != "") {
      const panRegEx = /^[A-Z]{5}\d{4}[A-Z]{1}/g;
      const validPan = panRegEx.test(arg);
      if (validPan == false) {
        swal("Warning!", 'Please Enter Valid Pan Number', "warning");
      }
      else {
        swal("Submitted!", "Pan Updated Successfully", "success");
        this.orderDetails.PANNo = arg;
        $('#UpdatePan').modal('hide');
      }
    }
    else {
      swal("Warning!", 'Please Enter Pan Number', "warning");
    }
  }

  //Hide Show data when accordian collapsed(Header)
  Customer: any;
  EnableCustomerTab: boolean = true;
  ToggleCustomer: boolean = false;

  TogglePayments: boolean = false;
  EnablePaymentsTab: boolean = false;

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
      swal("Warning", "Please select Customer Details to continue ", "warning");
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

  ReceiptNo: string = "";

  submitPost() {
    if (this.orderDetails.lstOfPayment.length == 0) {
      swal("Warning", "Please Enter Receipts Details", "warning");
    }
    else {
      var ans = confirm("Do you want to Submit??");
      if (ans) {
        let receiptnumber;
        this.EnableReprintReceipt = false;
        this._OrdersService.SendOrderNoToReprintComp(null);
        this._OrdersService.postorderReceipt(this.orderDetails).subscribe(
          response => {
            receiptnumber = response;
            this.routerUrl = this._router.url;
            swal("Saved!", "New Order Receipt number: " + receiptnumber.ReceiptNo + " generated successfully!", "success");
            this.leavePage = false;
            var ans = confirm("Do you want to Print??");
            if (ans) {
              this.EnableReprintReceipt = true;
              this.ReceiptNo = receiptnumber.ReceiptNo;
              this._OrdersService.SendReceiptNoToReprintComp(this.ReceiptNo);
            }
            this.clearOrderReceiptComponents();
          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error;
              swal("Warning!", validationError.customDescription, "warning");
              setTimeout(function () {
                $('#UpdatePan').modal('show');
              }, 2000);
            }
          }
        )
      }
    }
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

  ngOnDestroy() {
    this._CustomerService.SendCustDataToEstComp(null);
    this._paymentservice.SendPaymentSummaryData(null);
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
    this._paymentservice.outputData(null);
    this._paymentservice.OutputParentJSONFunction(null);
    this._paymentservice.inputData(null);
    this.clearOrderReceiptComponents();
  }

  clearOrderReceiptComponents() {
    this._CustomerService.SendCustDataToEstComp(null);
    this._paymentservice.SendPaymentSummaryData(null);
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
    this._paymentservice.outputData(null);
    this._paymentservice.inputData(null);
    this._paymentservice.OutputParentJSONFunction(null);
    this.EnablePaymentsTab = true;
    this.previousPaymentDetails = 0.00;
    this.paymentPaidAmt = 0.00;
    this.totalOrderAmt = 0.00;
    this.orderDetails.OrderNo = null;
    this.ifPaymentDetails = false;
    this.OrderNo.nativeElement.value = "";
  }
}