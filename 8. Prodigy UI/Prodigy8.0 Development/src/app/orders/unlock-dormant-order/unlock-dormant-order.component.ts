import { Component, OnInit } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { AppConfigService } from '../../AppConfigService';
import { OrdersService } from '../orders.service';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { AccountsService } from '../../accounts/accounts.service'
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { formatDate } from '@angular/common';
declare var $: any;

@Component({
  selector: 'app-unlock-dormant-order',
  templateUrl: './unlock-dormant-order.component.html',
  styleUrls: ['./unlock-dormant-order.component.css']
})
export class UnlockDormantOrderComponent implements OnInit {
  UnLockDormantOrderForm: FormGroup;
  EnableOrderDetails: boolean = false;
  password: string;
  EnableJson: boolean = false;

  constructor(private _appConfigService: AppConfigService,
    private fb: FormBuilder,
    private _router: Router, private _ordersService: OrdersService
  ) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
  }

  ToggleOrderDetails() {
    this.EnableOrderDetails = !this.EnableOrderDetails;
  }

  ngOnInit() {
    this.UnLockDormantOrderForm = this.fb.group({
      OrderNo: [null],
      Remarks: [null]
    });
  }

  dormantOrderDets: any;
  postDormantOrderList: any = [];

  Add(form) {
    if (form.value.OrderNo == null || form.value.OrderNo == "") {
      swal("Warning!", 'Please enter the valid order no', "warning");
    }
    else if (form.value.Remarks == null || form.value.Remarks == "") {
      swal("Warning!", 'Please enter the valid remarks', "warning");
    }
    else {
      let data = this.postDormantOrderList.find(x => x.OrderNo == form.value.OrderNo);
      if (data == null) {
        this._ordersService.getLockedOrderDetails(form.value.OrderNo).subscribe(response => {
          this.dormantOrderDets = response;
          this.dormantOrderDets.Remarks = form.value.Remarks;
          this.postDormantOrderList.push(this.dormantOrderDets);
          this.UnLockDormantOrderForm.reset();
        })
      }
      else {
        swal("Warning!", "Order number already added", "warning");
      }
    }
  }

  Cancel() {
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
      () => {
        this._router.navigate(['/orders/unlock-dormant-order']);
      }
    )
  }

  deleteFieldValue(index) {
    var ans = confirm("Do you want to delete??");
    if (ans) {
      this.postDormantOrderList.splice(index, 1);
    }
  }

  orderDetails: any = [];
  PrintTypeBasedOnConfig: any;
  ReprintType: string = "Original";

  viewFieldValue(arg) {
    this._ordersService.getOrderPrint(arg, this.ReprintType.toUpperCase()).subscribe(
      response => {
        this.PrintTypeBasedOnConfig = response;
        if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
          $('#ReprintTypeModal').modal('hide');
          $('#OrderTab').modal('show');
          this.orderDetails = atob(this.PrintTypeBasedOnConfig.Data);
          $('#DisplayOrderData').html(this.orderDetails);
        }
        else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
          $('#ReprintTypeModal').modal('hide');
          $('#OrderTab').modal('show');
          this.orderDetails = this.PrintTypeBasedOnConfig.Data;
        }
      }
    )
  }

  outputData: any = [];

  Submit() {
    if (this.postDormantOrderList.length == 0) {
      swal("Warning!", 'Please enter atleast one order', "warning");
    }
    else {
      this._ordersService.unlockDormantOrder(this.postDormantOrderList).subscribe(
        response => {
          this.outputData = response;
          swal("Saved!", "Orders are UnLocked to Dormant successfully!", "success");
          this.postDormantOrderList = [];
          this.UnLockDormantOrderForm.reset();
        }
      )
    }
  }
}