import { ComponentCanDeactivate } from './../appconfirmation-guard';
import { CustomerService } from './../masters/customer/customer.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { OrdersService } from './orders.service';
import { MasterService } from './../core/common/master.service';
import { AppConfigService } from '../AppConfigService';
import { PaymentService } from './../payment/payment.service';
import { Router } from '@angular/router'
import swal from 'sweetalert';
declare var $: any;
import { formatDate } from '@angular/common';
import * as moment from 'moment';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.css']
})
export class OrdersComponent implements OnInit, OnDestroy, ComponentCanDeactivate {

  toggle: boolean = false;
  radioItems: Array<string>;
  OrderNo: string = "";
  EnableSubmitButton: boolean = true;
  EnableModifyOrder: boolean = false;
  OrderType: string;
  ParentJSON: any = [];
  routerUrl: string = "";
  public pageName = "OR";
  NavigateUrl: string;
  EnableReprintOrder: boolean = false;
  EnableJson: boolean = false;
  password: string;
  ngOnInit() {
    // this.NewOrder = true;
    this._paymentService.Data.subscribe(
      response => {
        this.ParentJSON = response;
        if (this.isEmptyObject(this.ParentJSON) == false && this.isEmptyObject(this.ParentJSON) != null) {
          this.EnableSubmitButton = false;
        }
        else {
          this.EnableSubmitButton = true;
        }
      }
    )
    this.getOrderDetsFromModifyComp();
    this.DetailsToCustomerComp();
    this.GetItemsDetsfromItemComp();
    this.GetParentJson();
    this.GetPaymentSummary();
    if (this._router.url === "/orders/Customized") {
      this.OrderType = "Customized";
    }
    if (this._router.url === "/orders/OrderAdvance") {
      this.OrderType = "OrderAdvance";
    }
    if (this._router.url === "/orders/Reserved") {
      this.OrderType = "Reserved";
    }
    this.NavigateUrl = this._router.url;
  }

  constructor(private _CustomerService: CustomerService, private _OrdersService: OrdersService,
    private _router: Router, private _paymentService: PaymentService,
    private _masterService: MasterService, private _appConfigService: AppConfigService) {
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
  }

  cancel() {
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(() =>
      this._router.navigate(['/orders']))
  }

  //Check object is empty
  isEmptyObject(obj) {
    return (obj && (Object.keys(obj).length === 0));
  }

  //to customer Details to Customer Component
  Customer: any;
  PaymentSummary: any = [];
  EnableCustomerTab: boolean = true;
  EnableItemsPaymentsTab: boolean = true;
  NoRecordsItemsPayments: boolean = true;
  NoRecordsPaymentSummary: boolean = false;
  NoRecordsPayments: boolean = true;
  EnablePaymentsTab: boolean = true;
  ToggleCustomer: boolean = false;
  itemDetails: any = [];
  leavePage: boolean = false;

  DetailsToCustomerComp() {
    this._CustomerService.cast.subscribe(
      response => {
        this.Customer = response;
        if (this.isEmptyObject(this.Customer) == false && this.isEmptyObject(this.Customer) != null) {

          this.EnableCustomerTab = true;
          this.ToggleCustomer = true;
          this.NoRecordsItemsPayments = true;
          this.EnableItemsPaymentsTab = false;
          this.leavePage = true;
        }
        else {

          this.EnableCustomerTab = false;
          this.ToggleCustomer = false;
          this.NoRecordsItemsPayments = false;
          this.EnableItemsPaymentsTab = true;
        }
      });
  }

  GetParentJson() {
    this._paymentService.castParentJSON.subscribe(
      response => {
        this.ParentJSON = response;
      }
    )
  }

  GetPaymentSummary() {
    this._paymentService.CastPaymentSummaryData.subscribe(
      response => {
        this.PaymentSummary = response;
        if (this.isEmptyObject(this.PaymentSummary) == false && this.isEmptyObject(this.PaymentSummary) != null) {
          if (this.PaymentSummary.Amount == null) {
            this.NoRecordsPaymentSummary = false;
            //this.EnableSubmitButton = true;
          }
          else {
            this.NoRecordsPaymentSummary = true;
            this.leavePage = true;
            //this.EnableSubmitButton = false;
          }
        }
        else {
          this.NoRecordsPaymentSummary = false;
          //this.EnableSubmitButton = true;
        }
      }
    )
  }

  GetItemsDetsfromItemComp() {
    this._paymentService.Data.subscribe(
      response => {
        this.itemDetails = response;
        if (this.isEmptyObject(this.itemDetails) == false && this.isEmptyObject(this.itemDetails) != null) {
          this.EnablePaymentsTab = false;
          this.EnableItemsPaymentsTab = true;
          this.NoRecordsPayments = true;
          this.itemDetails.DeliveryDate = formatDate(this.itemDetails.DeliveryDate, 'dd/MM/yyyy', 'en_GB');
          this.leavePage = true;
        }
        else {
          this.NoRecordsPayments = false;
        }
      }
    )
  }

  //Data visible when collapse
  ToggleCustomerData() {
    if (this.ToggleCustomer == true) {
      this.EnableCustomerTab = false;
    }
  }

  ToggleItemsPaymentData() {
    if (this.NoRecordsItemsPayments == false) {
      swal("Warning", "Please select Customer Details to continue ", "warning");
    }
    else {
      this.EnableItemsPaymentsTab = !this.EnableItemsPaymentsTab;
      // this._paymentService.Data.subscribe(
      //   response => {
      //     this.itemDetails = response;
      //     if (this.isEmptyObject(this.itemDetails) == false && this.isEmptyObject(this.itemDetails) != null) {
      //       this.EnableItemsPaymentsTab = false;
      //       this.NoRecordsPayments = true;
      //     }
      //     else {
      //       this.NoRecordsPayments = false;
      //     }
      //   });
    }
  }

  TogglePaymentData() {
    if (this.NoRecordsPayments == false) {
      // swal("Warning", "Please select Item Details ", "warning");
      swal("Warning", "Please save order details", "warning");
    }
    else {
      this.EnablePaymentsTab = !this.EnablePaymentsTab;
      // this._paymentService.CastPaymentSummaryData.subscribe(
      //   response => {
      //     this.PaymentSummary = response;
      //     if (this.isEmptyObject(this.PaymentSummary) == false && this.isEmptyObject(this.PaymentSummary) != null) {
      //       if (this.PaymentSummary.Amount == null) {
      //         this.NoRecordsPaymentSummary = false;
      //       }
      //       else {
      //         this.NoRecordsPaymentSummary = true;
      //       }
      //     }
      //     else {
      //       this.NoRecordsPaymentSummary = false;
      //     }
      //   }
      // )
    }
  }

  orderDetails: any = []
  getOrderDetails(arg) {
    this._OrdersService.get(arg).subscribe(
      response => {
        this.orderDetails = response;
        if (this.orderDetails != null) {
          this._CustomerService.getCustomerDtls(this.orderDetails.CustID).subscribe(
            response => {
              const customerDtls = response;
              this._CustomerService.sendCustomerDtls_To_Customer_Component(customerDtls);
              this._OrdersService.SendExistingOrderDetailsToSubComp(this.orderDetails);
              this._OrdersService.SendOrderNoToComp(arg);
              this._paymentService.inputData(this.orderDetails);
              this.NoRecordsPayments = true;
            }
          )
        }
      }
    )
  }

  ngOnDestroy() {
    this._CustomerService.SendCustDataToEstComp(null);
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
    this._OrdersService.SendOrderNoToComp(null);
    this._paymentService.outputData(null);
    this._paymentService.inputData(null);
    this._paymentService.OutputParentJSONFunction(null);
    this._paymentService.SendPaymentSummaryData(null);
    this._OrdersService.SendModifyOrderNoToItemComp(null);
  }

  Submit() {
    let orderNo;
    if (this.ParentJSON.lstOfOrderItemDetailsVM.length == 0) {
      swal("Warning", "Please enter Order Details to continue ", "warning");
    }
    // else if (this.ParentJSON.ManagerCode == null) {
    //   swal("Warning", "Please select manager", "warning");
    // }
    else if (this.ParentJSON.BookingType == null) {
      swal("Warning", "Please select booking type", "warning");
    }
    else {
      if (this.ParentJSON.OrderRateType == "Flexi" && this.ParentJSON.OrderType == "R" && this.ParentJSON.lstOfPayment.length == 0) {
        swal("Warning", "Please Enter Receipts Details", "warning");
      }
      else if (this.ParentJSON.OrderRateType == "Fixed" && this.ParentJSON.lstOfPayment.length == 0) {
        swal("Warning", "Please Enter Receipts Details", "warning");
      }
      //empty paymant object validation02022022
      else if(this.ParentJSON.lstOfPayment.length == 0){
        swal("Warning", "Please Enter Receipts Details", "warning");
      }
      else {
        var ans = confirm("Do you want to save??");
        if (ans) {
          if (this.ParentJSON.OrderNo == undefined) {
            this.ParentJSON.OrderDate = moment(this.ParentJSON.OrderDate, 'DD/MM/YYYY').format('YYYY-MM-DD');
            this.ParentJSON.DeliveryDate = moment(this.ParentJSON.DeliveryDate, 'DD/MM/YYYY').format('YYYY-MM-DD');
            // this.ParentJSON.RateValidityTill = this.ParentJSON.OrderDate;//chnaged bcz of model refresh
            this.ParentJSON.RateValidityTill = formatDate(this.ParentJSON.DeliveryDate, 'yyyy-MM-dd', 'en-US');;
            this._OrdersService.postOrder(this.ParentJSON).subscribe(
              response => {
                orderNo = response;
                this.routerUrl = this._router.url;
                if (orderNo.ReceiptNo != 0) {
                  swal("Saved!", "New Order No: " + orderNo.OrderNo + " and Receipt No: " + orderNo.ReceiptNo + " generated successfully!", "success");
                }
                else {
                  swal("Saved!", "New Order No: " + orderNo.OrderNo + " generated successfully!", "success");
                }
                this.leavePage = false;
                var ans = confirm("Do You want to take order Print Out??");
                if (ans) {
                  this.EnableReprintOrder = true,
                    this.OrderNo = orderNo.OrderNo
                  this._OrdersService.SendOrderNoToReprintComp(orderNo);
                }
                this.clearOrderComponents();

                //this.OrderType = "";
                // this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(() =>
                //   this._router.navigate([this.NavigateUrl]),
                // );
              },
              (err) => {
                if (err.status === 400) {
                  const validationError = err.error;
                  swal("Warning!", validationError.description, "warning");
                  this.ParentJSON.OrderDate = moment(this.ParentJSON.OrderDate, 'YYYY-MM-DD').format('DD/MM/YYYY');
                  this.ParentJSON.DeliveryDate = moment(this.ParentJSON.DeliveryDate, 'YYYY-MM-DD').format('DD/MM/YYYY');
                  // this.ParentJSON.RateValidityTill = this.ParentJSON.OrderDate;
                  this.ParentJSON.RateValidityTill = moment(this.ParentJSON.DeliveryDate, 'DD/MM/YYYY').format('YYYY-MM-DD');
                }
              }
            )
          }
          else {
            this.ParentJSON.OrderDate = moment(this.ParentJSON.OrderDate, 'DD/MM/YYYY').format('YYYY-MM-DD');
            this.ParentJSON.DeliveryDate = moment(this.ParentJSON.DeliveryDate, 'DD/MM/YYYY').format('YYYY-MM-DD');
            this._OrdersService.putOrder(this.ParentJSON, this.ParentJSON.OrderNo).subscribe(
              response => {
                orderNo = response;
                this.routerUrl = this._router.url;
                swal("Updated!", "Order number " + orderNo.OrderNo + " Updated", "success");
                this.leavePage = false;
                var ans = confirm("Do You want to take order Print Out??");
                if (ans) {
                  this.EnableReprintOrder = true,
                    this.OrderNo = orderNo.OrderNo
                  this._OrdersService.SendOrderNoToReprintComp(orderNo);
                }
                this.clearOrderComponents();
              }
            )
          }
        }
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


  chngModifyOrder() {
    this.ngOnDestroy();
    this.EnableModifyOrder = true;
    $("#myModal").modal('show');
  }

  clearOrderComponents() {
    this._CustomerService.SendCustDataToEstComp(null);
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
    this.DetailsToCustomerComp();
    this._paymentService.inputData(null);
    this.GetItemsDetsfromItemComp();
    this._paymentService.OutputParentJSONFunction(null);
    this.GetParentJson();
    this._paymentService.SendPaymentSummaryData(null);
    this.GetPaymentSummary();
    this._OrdersService.SendOrderNoToComp(null);
    this._paymentService.outputData(null);
    this.EnablePaymentsTab = true;
    this._OrdersService.SendModifyOrderNoToItemComp(null);
    this.ParentJSON.OrderDate = moment(this.ParentJSON.OrderDate, 'YYYY-MM-DD').format('DD/MM/YYYY');
    this.ParentJSON.DeliveryDate = moment(this.ParentJSON.DeliveryDate, 'YYYY-MM-DD').format('DD/MM/YYYY');
    this.ParentJSON.RateValidityTill = this.ParentJSON.OrderDate;
  }

  orderDetsfromModifyComp: any;

  modifiedItemDetails: any;

  getOrderDetsFromModifyComp() {
    this._OrdersService.castOrderDetsToOrderComp.subscribe(
      response => {
        this.orderDetsfromModifyComp = response;
        if (this.orderDetsfromModifyComp != null) {
          this._OrdersService.viewOrder(this.orderDetsfromModifyComp.OrderNo).subscribe(
            response => {
              this.modifiedItemDetails = response;
              if (this.modifiedItemDetails != null) {
                this._CustomerService.getCustomerDtls(this.modifiedItemDetails.CustID).subscribe(
                  response => {
                    this.Customer = response;
                    this._CustomerService.sendCustomerDtls_To_Customer_Component(this.Customer);
                    this._CustomerService.SendCustDataToEstComp(this.Customer);
                    this._OrdersService.SendModifyOrderNoToItemComp(this.orderDetsfromModifyComp.OrderNo);
                    this._OrdersService.SendExistingOrderDetailsToSubComp(this.modifiedItemDetails);
                    this._paymentService.inputData(this.modifiedItemDetails);
                    this.ParentJSON = this.modifiedItemDetails;
                  }
                )
              }
            }
          )
        }
      }
    )
  }
}