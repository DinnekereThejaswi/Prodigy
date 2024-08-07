import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { OnlineOrderManagementSystemService } from '../online-order-management-system.service';
import swal from 'sweetalert';
import { NgxSpinnerService } from "ngx-spinner";

@Component({
  selector: 'app-online-order-cancel',
  templateUrl: './online-order-cancel.component.html',
  styleUrls: ['./online-order-cancel.component.css']
})
export class OnlineOrderCancelComponent implements OnInit {
  constructor(private _onlineOrderManagementSystemService: OnlineOrderManagementSystemService,
    private fb: FormBuilder, private SpinnerService: NgxSpinnerService) {
  }

  CancelOnlineOrdersForm: FormGroup;
  marketPlaceList: any = [];

  ngOnInit() {
    this.marketPlace();
    this.CancelOnlineOrdersForm = this.fb.group({
      marketplaceID: [null],
      orderNo: [null],
      cancelRemarks: [null]
    });
  }

  marketPlace() {
    this._onlineOrderManagementSystemService.getmarketplaces().subscribe(response => {
      this.marketPlaceList = response;
    })
  }

  OnlineOrdersList: any = [];

  OnlineOrders(form) {
    this.CancelOnlineOrdersForm.controls['orderNo'].setValue(null);
    this.CancelOnlineOrdersForm.controls['cancelRemarks'].setValue(null);
    this.OnlineOrdersList = [];
    if (form.value.marketplaceID == "null") {
      swal("Warning!", 'Please select the Market Place', "warning");
    }
    else {
      this._onlineOrderManagementSystemService.getOnlineOrderCancel(form.value.marketplaceID).subscribe(
        response => {
          this.OnlineOrdersList = response;
        }
      )
    }
  }

  cancelOnlineOrder(form) {
    if (form.value.marketplaceID == "null" || form.value.marketplaceID == null) {
      swal("Warning!", 'Please select the Market Place', "warning");
      this.CancelOnlineOrdersForm.controls['orderNo'].setValue(null);
      this.CancelOnlineOrdersForm.controls['cancelRemarks'].setValue(null);
      this.OnlineOrdersList = [];
    }
    else if (form.value.orderNo == "null" || form.value.orderNo == null) {
      swal("Warning!", 'Please select the Order No', "warning");
    }
    else if (form.value.cancelRemarks == "null" || form.value.cancelRemarks == null || form.value.cancelRemarks == "") {
      swal("Warning!", 'Please enter the Remarks', "warning");
    }
    else {
      var ans = confirm("Do you want to cancel the order??");
      if (ans) {
        this.SpinnerService.show();
        this._onlineOrderManagementSystemService.CancelOnlineOrder(form.value).subscribe(
          response => {
            this.SpinnerService.hide();
            swal("Cancelled!", "Order number #" + form.value.orderNo + " has been cancelled successfully", "success");
            this.clear();
          },
          (err) => {
            this.SpinnerService.hide();
          }
        )
      }
    }
  }

  clear() {
    this.CancelOnlineOrdersForm.reset();
    this.OnlineOrdersList = [];
  }
}