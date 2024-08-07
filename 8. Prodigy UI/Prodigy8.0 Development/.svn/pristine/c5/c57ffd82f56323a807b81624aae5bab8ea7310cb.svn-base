import { SalesService } from '../../sales/sales.service';
import { Component, OnInit, OnDestroy, Output, EventEmitter } from '@angular/core';
import { CustomerService } from '../../masters/customer/customer.service';
import { OrdersService } from './../orders.service';
import { PurchaseService } from './../../purchase/purchase.service';
import { estimationService } from '../../estimation/estimation.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import swal from 'sweetalert';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
declare var $: any;


@Component({
  selector: 'app-modify-order',
  templateUrl: './modify-order.component.html',
  styleUrls: ['./modify-order.component.css']
})
export class ModifyorderComponent implements OnInit, OnDestroy {
  ccode: string = "";
  bcode: string = "";
  OrderAmount: number = 0;
  AllRecordCount: any;
  searchText: string = "";
  CustomerName: any;
  PostOrderAttchJson: any = [];
  //SearchParamsList: any;
  AttachOrderList: any;
  AttachOrderForm: FormGroup;
  submitted = false;
  SelectedOrderList: any = [];
  EnableSubmitButton: boolean = true;
  @Output() valueChange = new EventEmitter();
  password: string;
  SalesEstNo: string = null;
  EnableDisablectrls: boolean = true;
  ModifyOrderList: any = [];

  GlobalOrderAmount: any = 0;
  totalItems: any;
  pagenumber: number = 1;
  top = 20;
  skip = (this.pagenumber - 1) * this.top;


  constructor(private _PurchaseService: PurchaseService, private formBuilder: FormBuilder,
    private _estimationService: estimationService, private _salesService: SalesService,
    private _CustomerService: CustomerService, private _OrdersService: OrdersService,
    private _router: Router, private toastr: ToastrService,
    private _appConfigService: AppConfigService
  ) {
    this.password = this._appConfigService.Pwd;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this._OrdersService.getModifyOrder().subscribe(
      response => {
        this.ModifyOrderList = response;
        if (this.ModifyOrderList != null) {
          this.totalItems = this.ModifyOrderList.length;
        }
      }
    )
  }



  onPageChange(p: number) {
    this.pagenumber = p;
    const skipno = (this.pagenumber - 1) * this.top;
    this._OrdersService.getTopModifyOrder(this.top, skipno).subscribe(
      response => {
        this.ModifyOrderList = response;
        this.ModifyOrderList.sort((a, b) => {
          return a.OrderNo - b.OrderNo;
        });
      }
    )
  }
  isDesc: boolean = false;
  column: string = 'OrderNo';
  getSortByOptions() {
    // this.ModifyOrderList.sort((a, b) => {
    //   return a.OrderNo - b.OrderNo;
    // });

    this.ModifyOrderList.sort((a, b) => 0 - (a.OrderNo > b.OrderNo ? 1 : -1));
  }

  valueChanged(Amount) { // You can give any function name
    this.OrderAmount = Amount;
    this.valueChange.emit(this.OrderAmount);
  }

  selectRecord(arg) {
    this._OrdersService.SendOrderDetsToOrderComp(arg);
    $('#myModal').modal('hide');
  }

  EnableDisableSubmitBtn() {
    if (this.SelectedOrderList.length <= 0) {
      this.EnableSubmitButton = true;
    }
    else {
      this.EnableSubmitButton = false;
    }
  }

  ngOnDestroy() {
    this._CustomerService.SendCustDataToEstComp(null);
    this._estimationService.SendOrderAttachmentSummaryData(null);
    this._estimationService.SendEstNo(null);
    //02/03/2021 below line addded bcz modifed order details still not clearing even if any other component 
    //navigated and came to order the old details not at all cleared 
    this._OrdersService.SendOrderDetsToOrderComp(null);
  }

  Cancel() {
    $('#myModal').modal('hide');
    this.AttachOrderForm.reset();
    this.AttachOrderList = [];
  }

  ClearField() {
    this.searchText = "";
  }
}