import { CustomerService } from './../../masters/customer/customer.service';
import { OrdersService } from './../orders.service';
import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';

@Component({
  selector: 'app-order-cancel',
  templateUrl: './order-cancel.component.html',
  styleUrls: ['./order-cancel.component.css']
})
export class OrderCancelComponent implements OnInit, OnDestroy {
  OrderCancelForm: FormGroup;
  EnableJson: boolean = false;
  @ViewChild('OrderNo', { static: true }) OrderNo: ElementRef;

  password: string;
  constructor(private fb: FormBuilder, private _OrdersService: OrdersService,
    private _CustomerService: CustomerService, private _router: Router,
    private _appConfigService: AppConfigService) {
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
  }

  ngOnInit() {
    this.OrderCancelForm = this.fb.group({
      OrderNo: ['null', Validators.required],
      CustID: null,
      CustName: null,
      Address1: null,
      AdvacnceOrderAmount: null,
      OrderDate: null,
      MobileNo: null,
      City: null,
      EmailID: null
    });
  }

  orderDetails: any = [];
  hasOrderDetails: boolean = false;
  previousPaymentDetails: number;
  SelectedItemLines: any = [];
  customerDtls: any = [];
  getOrderDetails(arg) {
    if (arg == '') {
      swal("Warning!", 'Enter Order Number', "warning");
    }
    else {
      this._OrdersService.getCancelOrderView(arg).subscribe(
        response => {
          this.OrderCancelObj = response;
          if (this.OrderCancelObj != null) {
            this.hasOrderDetails = true;
            this._CustomerService.getCustomerDtls(this.OrderCancelObj.CustID).subscribe(
              response => {
                this.customerDtls = response;
                this._CustomerService.sendCustomerDtls_To_Customer_Component(this.customerDtls);
              }
            )
          }
        },
        (err) => {
          if (err.status === 404) {
            const validationError = err.error;
            swal("Warning!", validationError.description, "warning");
            this.hasOrderDetails = false;
          }
          if (err.status === 400) {
            const validationError = err.error;
            swal("Warning!", validationError.description, "warning");
            this.hasOrderDetails = false;
          }
        }
      )
    }
  }

  clear() {
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(() =>
      this._router.navigate(['/orders/OrderCancel']))
  }

  OrderCancelObj: any = {
    CustID: null,
    CustName: null,
    Address1: null,
    City: null,
    MobileNo: null,
    OrderDate: null,
    SalCode: null,
    OrderNo: null,
    EmailID: null,
    AdvacnceOrderAmount: null,
    lstOfOrderItemDetailsVM: [],
    lstOfPayment: []
  }

  ngOnDestroy() {
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
  }

  submitPost() {
    var ans = confirm("Do you want to cancel??");
    if (ans) {
      this._OrdersService.cancelOrder(this.OrderCancelObj).subscribe(
        response => {
          swal("Cancelled!", "Order number " + this.OrderCancelObj.OrderNo + " Cancelled Successfully", "success");
          this.OrderCancelForm.reset();
          this.hasOrderDetails = false;
          this.OrderNo.nativeElement.value = "";
        }
      )
    }
  }
}
