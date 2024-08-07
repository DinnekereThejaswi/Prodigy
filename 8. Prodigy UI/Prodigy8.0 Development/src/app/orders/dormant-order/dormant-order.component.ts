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
  selector: 'app-dormant-order',
  templateUrl: './dormant-order.component.html',
  styleUrls: ['./dormant-order.component.css']
})
export class DormantOrderComponent implements OnInit {

  DormantOrderForm: FormGroup;
  radioItems: Array<string>;
  model = { option: 'Dormant Order No' };
  EnableOrderDetails: boolean = false;
  datePickerConfig: Partial<BsDatepickerConfig>;
  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  today = new Date();
  applicationDate: any;

  constructor(private _appConfigService: AppConfigService,
    private accountservice: AccountsService, private fb: FormBuilder,
    private _router: Router, private _ordersService: OrdersService
  ) {
    this.radioItems = ['Dormant Order No', 'Proposed Dormant Order Date'];
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        maxDate: this.today,
        dateInputFormat: 'YYYY-MM-DD'
      });
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }

  ToggleOrderDetails() {
    this.EnableOrderDetails = !this.EnableOrderDetails;
  }

  ngOnInit() {
    this.getApplicationdate()
    this.DormantOrderForm = this.fb.group({
      OrderNo: [null],
      applicationDate: [null],
      Remarks: [null]
    });
  }


  getApplicationdate() {
    this.accountservice.getApplicationDate().subscribe(
      response => {
        this.applicationDate = response;
        this.applicationDate = this.applicationDate.applcationDate;
      }
    )
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }


  OnRadioBtnChnge(arg) {
    this.model.option = arg;
    this.DormantOrderForm.controls["OrderNo"].setValue(null);
    this.DormantOrderForm.controls["Remarks"].setValue(null);
  }

  byDate(applicationDate) {
    if (applicationDate != undefined) {
      this.applicationDate = formatDate(applicationDate, 'yyyy-MM-dd', 'en-US');;
    }
  }

  dormantOrderDets: any = [];
  postDormantOrderList: any = [];
  orderDets: any;

  Add(form) {
    if (this.model.option == "Proposed Dormant Order Date") {
      if (form.value.Remarks == null || form.value.Remarks == "") {
        swal("Warning!", 'Please enter the valid remarks', "warning");
      }
      else {
        this._ordersService.getDormantOrderList(this.applicationDate).subscribe(response => {
          this.dormantOrderDets = response;
          this.autofillRemarks(form.value.Remarks);
        })
      }
    }
    else {
      if (form.value.OrderNo == null || form.value.OrderNo == "") {
        swal("Warning!", 'Please enter the valid order no', "warning");
      }
      else if (form.value.Remarks == null || form.value.Remarks == "") {
        swal("Warning!", 'Please enter the valid remarks', "warning");
      }
      else {
        let data = this.dormantOrderDets.find(x => x.OrderNo == form.value.OrderNo);
        if (data == null) {
          this._ordersService.getDormantOrderbyOrderNo(form.value.OrderNo).subscribe(response => {
            this.orderDets = response;
            this.orderDets.Remarks = form.value.Remarks;
            this.dormantOrderDets.push(this.orderDets);
          })
        }
        else {
          swal("Warning!", "Order number already added", "warning");
        }
      }
    }
  }

  autofillRemarks(arg) {
    for (let i = 0; i < this.dormantOrderDets.length; i++) {
      this.dormantOrderDets[i].Remarks = arg;
    }
  }

  Cancel() {
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
      () => {
        this._router.navigate(['/orders/dormant-order']);
      }
    )
  }

  checkAll(event) {
    if (event.target.checked) {
      this.postDormantOrderList = this.dormantOrderDets;
      this.postDormantOrderList.forEach(x => x.state = event.target.checked)
    }
    else {
      this.postDormantOrderList.forEach(x => x.state = event.target.unchecked)
      this.postDormantOrderList = [];
    }
  }

  public Index: number = -1;


  onCheckboxChange(option, event, index) {
    if (event.target.checked) {
      this.postDormantOrderList.push(option);
    }
    else {
      this.postDormantOrderList.splice(index, 1);
      for (let i = 0; i < this.postDormantOrderList.length; i++) {
        if (this.postDormantOrderList[i].OrderNo == option.OrderNo) {
          this.postDormantOrderList.splice(i, 1);
          break;
        }
      }
    }
  }

  isAllChecked() {
    return this.dormantOrderDets.every(_ => _.state);
  }

  deleteFieldValue(index) {
    var ans = confirm("Do you want to delete??");
    if (ans) {
      this.dormantOrderDets.splice(index, 1);
      this.postDormantOrderList = [];
      this.postDormantOrderList = this.dormantOrderDets;
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
      swal("Warning!", 'Please select atleast one order', "warning");
    }
    else {
      this._ordersService.lockDormantOrder(this.postDormantOrderList).subscribe(
        response => {
          this.outputData = response;
          swal("Saved!", "Orders are Locked to Dormant successfully!", "success");
          this.postDormantOrderList = [];
          this.dormantOrderDets = [];
          this.DormantOrderForm.reset();
          this.getApplicationdate();
        }
      )
    }
  }
}