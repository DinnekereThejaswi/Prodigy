import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { OrdersService } from '../orders.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { formatDate } from '@angular/common';
import { MasterService } from './../../core/common/master.service';
import { AppConfigService } from '../../AppConfigService';
import swal from 'sweetalert';
import * as qz from 'qz-tray';
declare var qz: any;
declare var jquery: any;
declare var $: any;
import { ToastrService } from 'ngx-toastr';
import * as CryptoJS from 'crypto-js';

@Component({
  selector: 'app-reprint-order',
  templateUrl: './reprint-order.component.html',
  styleUrls: ['./reprint-order.component.css']
})

export class ReprintOrderComponent implements OnInit {
  radioItems: Array<string>;
  radioItemPrintType: Array<string>;
  radioReprintTypeItems: Array<string>;

  model = { option: 'Order No' };
  modelReprintType = { option: 'Original' };
  modelPrintType = { option: 'HTML' };

  ReprintForm: FormGroup;
  Orderheading: string = "Order Form";
  OrderReceiptheading: string = "Order Receipt";
  ClosedOrderheading: string;
  ReceiptList: any = [];
  ReceiptListdisplay: any = [];
  CustomerID: any;
  Linesarray: any = [];
  Paymentarray: any = [];
  orderDetails: any = [];
  orderPlainTextDetails: any = [];
  orderReceiptPlainTextDetails: any = [];
  closedOrderPlainTextDetails: any = [];
  @Input()
  OrderNo: string = "";
  @Input()
  ReceiptNo: string = "";
  @Input()
  ClosedOrderNo: string = "";
  ShowroomList: any = [];
  totalFinalAmt: number;
  totalPayAmountBeforeTax: number;
  CGSTAmount: number;
  CGSTPercent: number;
  SGSTPercent: number;
  IGSTPercent: number;
  SGSTAmount: number;
  IGSTAmount: number;
  SelectedItemLines: any = [];
  ordDate = '';
  isActive = false;
  width = 1;
  height = 25;
  datePickerConfig: Partial<BsDatepickerConfig>;
  today = new Date();

  ///permission

  @ViewChild("Pwd", { static: true }) Pwd: ElementRef;
  PasswordValid: string;
  public Index: number = -1;
  ccode: string;
  bcode: string;
  password: string;
  EnableDate: boolean = true;
  ///
  constructor(private _orderService: OrdersService, private formBuilder: FormBuilder, private _router: Router,
    private _masterService: MasterService, private appConfigService: AppConfigService
    , private toastr: ToastrService) {
    this.password = this.appConfigService.Pwd;
    this.radioItemPrintType = ['HTML', 'DotMatrix'];
    this.radioItems = ['Order No', 'Receipt No', 'Closed Orders'];
    this.radioReprintTypeItems = ['Original', 'Duplicate', 'Triplicate'];
    this.PasswordValid = this.appConfigService.RateEditCode.SalesBillPermission;
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        maxDate: this.today,
        // minDate: this.today,
        dateInputFormat: 'YYYY-MM-DD',

      });
    if (this._router.url == "/orders/reprint") {
      this._orderService.SendOrderNoToReprintComp(null);
      this._orderService.SendReceiptNoToReprintComp(null);
      $('#OrderPlainTextTab').modal('hide');
    }
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  isReprint: boolean = false;
  OrderFromOrderScreen: any;
  OrderReceiptFromReceiptScreen: any;
  OrderNum: number;
  ReceiptNum: number;
  ClosedOrderNum: number;
  ngOnInit() {

    this.getApplicationDate();

    this._orderService.castOrderNoToReprintComp.subscribe(
      response => {
        this.OrderFromOrderScreen = response;
        if (this.OrderFromOrderScreen != null) {
          this.model.option == 'Order No';
          this.OrderNum = this.OrderFromOrderScreen.OrderNo;
          this.getOrderDetails(this.OrderFromOrderScreen.OrderNo);
          // Below chain block commented on 19-Apr-2021 since order and receipt print handled from api side
          // if (this.OrderFromOrderScreen.ReceiptNo != 0 && this.OrderFromOrderScreen.ReceiptNo != null) {
          //   this.model.option == 'Receipt No';
          //   this.getReceiptDetails(this.OrderFromOrderScreen.ReceiptNo);
          // }
          // else {
          //   this.ReceiptList = [];
          //   this.ReceiptTotal = [];
          // }  
          // ends here
        }
      }
    )


    this._orderService.castReceiptNoToReprintComp.subscribe(
      response => {
        this.OrderReceiptFromReceiptScreen = response;
        if (this.OrderReceiptFromReceiptScreen != 0 && this.OrderReceiptFromReceiptScreen != null) {
          this.model.option == 'Receipt No';
          this.getReceiptDetails(this.OrderReceiptFromReceiptScreen);
        }
      }
    )



    this.ReprintForm = this.formBuilder.group({
      selectedOption: [null],
      applicationDate: null,
      date: null
    });

    if (this._router.url === "/orders/reprint") {
      this.isReprint = true;
    }

    if (this.OrderNo != "") {
      this.model.option = "Order No";
      this.onPrint(this.OrderNo);
    }

    if (this.ReceiptNo != "") {
      this.model.option = "Receipt No";
      this.onPrint(this.ReceiptNo);
    }
    if (this.ClosedOrderNo != "") {
      this.model.option = "Closed Orders";
      this.onPrint(this.ClosedOrderNo);
    }
  }


  reprintHeaders = {
    selectedOption: null,
    applicationDate: null
  }

  // for clearing the field
  onSubmit() {
    if (this.ReprintForm.valid) {
      //this.ReprintForm.reset();
    }
  }




  // onChangeAppdate(appdate){
  //   alert()
  //   this.reprintHeaders.applicationDate = appdate;
  //   this.getOrderlist();
  //   this.getReceiptlist();

  // }


  byDate(applicationDate) {
    this.reprintHeaders.applicationDate = null,
      this.reprintHeaders.applicationDate = applicationDate;
    this.reprintHeaders.selectedOption = null;
    this.applicationDate = formatDate(applicationDate, 'yyyy-MM-dd', 'en-US');;
    this.getOrderlist();
    this.getReceiptlist();
    this.getClosedOrderList();
  }
  applicationDate: any;
  disAppDate: any;
  test: any = [];
  getApplicationDate() {
    this._orderService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
        this.disAppDate = formatDate(appDate["applcationDate"], 'dd/MM/yyyy', 'en_GB');
        this.reprintHeaders.applicationDate = this.applicationDate;
        this.getOrderlist();
        this.getReceiptlist();
        this.getClosedOrderList();
      }
    )
  }

  OrderNoList: any = [];
  getOrderlist() {
    this._orderService.getOrderNo(this.applicationDate, this.isCheckedCHBX).subscribe(
      response => {
        this.OrderNoList = response;
      }
    )
  }
  ReceiptNoList: any = [];
  getReceiptlist() {
    this._orderService.getReceiptNo(this.applicationDate, this.isCheckedCHBX).subscribe(
      response => {
        this.ReceiptNoList = response;
      }
    )
  }

  ClosedOrderList: any = [];
  getClosedOrderList() {
    this._orderService.getClosedOrder(this.applicationDate).subscribe(
      response => {
        this.ClosedOrderList = response;

      }
    )
  }
  // CancelledOrderList: any =[];
  // getCancelledOrder(){
  //   this._orderService.getCancelledOrder(this.applicationDate).subscribe(
  //     response => {
  //       this.CancelledOrderList = response;
  //     }
  //   )
  // }
  // CancelledReceiptList: any =[];
  // getCancelledReceipt(){
  //   this._orderService.getCancelledReceipt(this.applicationDate).subscribe(
  //     response => {
  //       this.CancelledReceiptList = response;
  //     }
  //   )
  // }

  isCheckedCHBX: boolean = false;
  isChecked(e) {
    this.isCheckedCHBX = e.target.checked;
    this.getApplicationDate();
    //this.model.option = this.radioBtnValue;
  }

  toggleOrderNo: boolean = true;
  toggleReceiptNo: boolean = false;
  toggleClosedOrders: boolean = false;
  toggleChecked: boolean = false;
  EnableCanOrders: boolean = true;
  PrintTypeBasedOnConfig: any;


  // to know which radio button is selected
  radioBtnValue: string = '';
  Changed(arg) {
    this.reprintHeaders.selectedOption = null;
    this.radioBtnValue = arg;
    if (arg === 'Order No') {
      this.toggleOrderNo = true;
      this.toggleReceiptNo = false;
      this.toggleClosedOrders = false;
      this.model.option = arg;
      this.EnableCanOrders = true;
    }
    else if (arg === 'Receipt No') {
      this.toggleOrderNo = false;
      this.toggleReceiptNo = true;
      this.toggleClosedOrders = false;
      this.model.option = arg;
      this.EnableCanOrders = true;
    }
    else if (arg === 'Closed Orders') {
      this.toggleOrderNo = false;
      this.toggleReceiptNo = false;
      this.toggleClosedOrders = true;
      this.model.option = arg;
      this.EnableCanOrders = false;
    }

  }

  ChangedPrintType(arg) {
    this.modelPrintType.option = arg;
  }

  ChangedReprintType(arg) {
    this.modelReprintType.option = arg;
  }

  receiptDets: any = [];

  // To display the Order Form Details
  getOrderDetails(arg) {
    if (!arg) {
      swal("Warning!", 'Please enter Order number', "warning");
      $('#OrderTab').modal('hide');
    }
    else {
      // this._orderService.getPrintOrder(arg).subscribe(
      //   response => {
      //     this.orderDetails = response;
      //     this.OrderNum = arg;
      //     this.Linesarray = this.orderDetails.lstOfOrderItemDetailsVM;
      //     this.Paymentarray = this.orderDetails.lstOfPayment;
      //     this.getShowroomDetails();
      //     this.getWeightTotal(this.orderDetails.OrderNo);
      //     this.getTotal(this.orderDetails.OrderNo);
      //     $('#ReprintTypeModal').modal('hide');
      //     $('#OrderTab').modal('show');
      //     if (this._router.url == "/orders/reprint") {
      //       this._orderService.getReceptDetByOrder(arg).subscribe(
      //         response => {
      //           this.receiptDets = response;
      //           if (this.receiptDets.length > 0) {
      //             this.getReceiptDetails(this.receiptDets[0]);
      //           }
      //         }
      //       )
      //     }
      //   }
      // );

      this._orderService.getOrderPrint(arg, this.ReprintType.toUpperCase()).subscribe(
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
  }

  // calculateFooter() {
  //   this.totalFinalAmt = this.Paymentarray.reduce((sum, item) => sum + item.PayAmount, 0);
  //   this.totalPayAmountBeforeTax = this.Paymentarray.reduce((sum, item) => sum + item.PayAmountBeforeTax, 0);
  //   this.CGSTAmount = this.Paymentarray.reduce((sum, item) => sum + item.CGSTAmount, 0);
  //   this.SGSTAmount = this.Paymentarray.reduce((sum, item) => sum + item.SGSTAmount, 0);
  //   this.IGSTAmount = this.Paymentarray.reduce((sum, item) => sum + item.IGSTAmount, 0);
  //   this.CGSTPercent = this.Paymentarray.CGSTPercent;
  //   this.SGSTPercent = this.Paymentarray.SGSTPercent;
  //   this.IGSTPercent = this.Paymentarray.IGSTPercent;

  // }

  // To display the Showroom Details
  getShowroomDetails() {
    this._orderService.getShowroom().subscribe(
      response => {
        this.ShowroomList = response;
        this.onSubmit();
        // this.calculateFooter();
      }
    );
  }

  // To display the Order Receipt Details
  getReceiptDetails(arg) {
    if (!arg) {
      swal("Warning!", 'Please select Receipt number', "warning");
      $('#ReceiptTab').modal('hide');
    }
    else {
      // this._orderService.getPrintReceipt(arg).subscribe(
      //   response => {
      //     this.ReceiptList = response;
      //     this.ReceiptListdisplay = this.ReceiptList[0];
      //     this.CustomerID = this.ReceiptList[0].SeriesNo;
      //     this.ReceiptNum = arg;
      //     //this.getOrderDetails(this.CustomerID);
      //     this._orderService.getPrintOrder(this.CustomerID).subscribe(
      //       response => {
      //         this.orderDetails = response;
      //         this.Linesarray = this.orderDetails.lstOfOrderItemDetailsVM;
      //         this.Paymentarray = this.orderDetails.lstOfPayment;
      //         this.getShowroomDetails();
      //         this.getWeightTotal(this.orderDetails.OrderNo);
      //         this.getTotal(this.orderDetails.OrderNo);
      //       }
      //     );
      //     this.getReceiptTotal(arg);
      //     if (this._router.url == "/orders/reprint") {
      //       if (this.model.option == 'Order No') {
      //         $('#ReceiptTab').modal('hide');
      //       }
      //       else {
      //         $('#ReceiptTab').modal('show');
      //       }
      //     }
      //     else {
      //       if (this._router.url == "/orders/OrderReceipt") {
      //         $('#ReceiptTab').modal('show');
      //       }
      //       else {
      //         $('#ReceiptTab').modal('hide');
      //       }
      //     }
      //   }
      // );

      this._orderService.getOrderReceiptPrint(arg).subscribe(
        response => {
          this.PrintTypeBasedOnConfig = response;
          if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
            this.orderDetails = atob(this.PrintTypeBasedOnConfig.Data);
            $('#ReceiptTab').modal('show');
            $('#DisplayOrderReceiptData').html(this.orderDetails);
          }
          else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
            this.orderDetails = this.PrintTypeBasedOnConfig.Data;
            $('#ReceiptTab').modal('show');
          }
          if (this._router.url == "/orders/reprint") {
            if (this.model.option == 'Order No') {
              $('#ReceiptTab').modal('hide');
            }
            else {
              $('#ReceiptTab').modal('show');
            }
          }
          else {
            if (this._router.url == "/orders/OrderReceipt") {
              $('#ReceiptTab').modal('show');
            }
            else {
              $('#ReceiptTab').modal('hide');
            }
          }
        }
      );
    }
  }
  TotalList: any = [];
  getTotal(arg) {
    this._orderService.getTotal(arg).subscribe(
      response => {
        this.TotalList = response;
      },
      (err) => {
        if (err.status === 404) {
          this.TotalList = null;
          // const validationError = err.error;
          // if (validationError.description != "No payment details for the selected order.") {

          // }
        }
      }
    )
  }
  ClosedTotalList: any = []
  getClosedTotal(arg) {
    this._orderService.getClosedTotal(arg).subscribe(
      response => {
        this.ClosedTotalList = response;
      }
    )
  }
  ReceiptTotal: any = []
  getReceiptTotal(arg) {
    this._orderService.getReceiptTotal(arg).subscribe(
      response => {
        this.ReceiptTotal = response;
      }
    )
  }
  WeightTotalList: any = [];
  getWeightTotal(arg) {
    this._orderService.getWeightTotal(arg).subscribe(
      response => {
        this.WeightTotalList = response;
      }
    )
  }
  getClosedOrderDetails(arg) {
    if (!arg) {
      swal("Warning!", 'Please select Closed Order', "warning");
      $('#ClosedOrderTab').modal('hide');
    }
    // this._orderService.getPrintOrder(arg).subscribe(
    //   response => {
    //     this.orderDetails = response;
    //     this.ClosedOrderNum = arg;
    //     this.Linesarray = this.orderDetails.lstOfOrderItemDetailsVM;
    //     this.Paymentarray = this.orderDetails.lstOfPayment;
    //     this.getShowroomDetails();
    //     this.getClosedTotal(this.orderDetails.OrderNo);
    //     $('#ClosedOrderTab').modal('show');
    //   }
    // );

    this._orderService.getClosedOrderPrint(arg).subscribe(
      response => {
        this.PrintTypeBasedOnConfig = response;
        if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
          this.orderDetails = atob(this.PrintTypeBasedOnConfig.Data);
          $('#ClosedOrderTab').modal('show');
          $('#DisplayClosedOrderData').html(this.orderDetails);
        }
        else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
          this.orderDetails = this.PrintTypeBasedOnConfig.Data;
          $('#ClosedOrderTab').modal('show');
        }
      }
    )
  }

  close() {
    this.getApplicationDate();
  }

  CanelPrint() {
    $('#ReprintTypeModal').modal('hide');
  }



  EditDate() {
    this._orderService.getPermission().subscribe(
      response => {
        this.EnableDate = false;
      },
      error => {
        swal("Warning!", "Unauthorized", "warning");
        this.EnableDate = true;
      }
    )
  }

  ValidatePermisiion() {
    $("#PermissonModals").modal('show');
    this.Pwd.nativeElement.value = "";
  }
  passWord(arg) {
    if (arg == "") {
      swal("Warning!", 'Please Enter the Password', "warning");
      $('#PermissonModals').modal('show');
    }
    else {
      this.permissonModels.CompanyCode = this.ccode;
      this.permissonModels.BranchCode = this.bcode;
      this.permissonModels.PermissionID = this.PasswordValid;
      this.permissonModels.PermissionData = btoa(arg);

      this._orderService.postelevatedpermission(this.permissonModels).subscribe(
        response => {
          this.EnableDate = false;
          $('#PermissonModals').modal('hide');
          this.Index = -1;
        },
        (err) => {
          if (err.status === 401) {
            this.EnableDate = true;
          }
        }
      )
    }
  }

  permissonModels: any = {
    CompanyCode: null,
    BranchCode: null,
    PermissionID: null,
    PermissionData: null
  }

  Orders: string = "";
  // Reprint button event handled here
  onPrint(arg): void {
    if (this.model.option == 'Order No') {
      if (this.isCheckedCHBX == true) {
        this.Orderheading = 'Order Form/Cancelled';
      }
      else {
        this.Orderheading = 'Order Form';
      }
      if (!arg) {
        swal("Warning!", 'Please select Order number', "warning");
      }
      else {
        this.modelReprintType.option = "Original";
        if (this.modelPrintType.option == "DotMatrix") {
          $('#ReprintTypeModal').modal('hide');
          $('#OrderTab').modal('hide');
          this.getOrderPlainText(arg);
          this.onSubmit();
        }
        else {
          $('#OrderPlainTextTab').modal('hide');
          $('#ReprintTypeModal').modal('show');
        }
      }
    }
    else if (this.model.option == 'Receipt No') {
      if (this.isCheckedCHBX == true) {
        this.OrderReceiptheading = 'Order Receipt/Cancelled';
      }
      else {
        this.OrderReceiptheading = 'Order Receipt';
      }
      if (!arg) {
        swal("Warning!", 'Please select Receipt number', "warning");
        $('#ReceiptTab').modal('hide');
      }
      else {
        if (this.modelPrintType.option == "DotMatrix") {
          $('#ReprintTypeModal').modal('hide');
          $('#ReceiptTab').modal('hide');
          this.getOrderReceiptPlainText(arg);
          this.onSubmit();
        }
        else {
          $('#OrderReceiptPlainTextTab').modal('hide');
          $('#ReprintTypeModal').modal('hide');
          this.getReceiptDetails(arg);
        }
      }
    }
    else if (this.model.option == 'Closed Orders') {
      this.ClosedOrderheading = 'Closed Order Payment Voucher';
      if (!arg) {
        swal("Warning!", 'Please select Order number', "warning");
      }
      else {
        if (this.modelPrintType.option == "DotMatrix") {
          $('#ReprintTypeModal').modal('hide');
          $('#ClosedOrderTab').modal('hide');
          this.getClosedOrderPlainText(arg);
          this.onSubmit();
        }
        else {
          $('#ClosedOrderPlainTextTab').modal('hide');
          $('#ReprintTypeModal').modal('hide');
          this.getClosedOrderDetails(arg);
        }
      }
    }
  }

  ReprintType: string = "Original";

  onReprint(arg) {
    if (this.isCheckedCHBX == true) {
      this.Orderheading = 'Order Form/Cancelled';
    }
    else {
      this.Orderheading = 'Order Form';
    }
    this.ReprintType = this.modelReprintType.option;
    this.getOrderDetails(arg);
  }

  // for printing the form
  printOrder() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.orderDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printOrderContents, popupWin;
      printOrderContents = document.getElementById('DisplayOrderData').innerHTML;
      popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
      popupWin.document.open();
      popupWin.document.write(`
      <html>
        <head>
          <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css" integrity="sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm" crossorigin="anonymous">
          <title>Print tab</title>
          <style>
         .htmlPrint{display:none;}

          @media print {
            .table-bordered
        {
            border-style: solid;
            border: 3px solid rgb(0, 0, 0);
        }
        .margin{
          margin-top:-24px;
        }
        .mb{
          vertical-align:middle;position:absolute;
        }
        tr.spaceUnder>td {
          padding-bottom: 40px !important;
        }
        .top {
          margin-top:10px;
          border-bottom: 3px solid rgb(0, 0, 0) !important;
          line-height: 1.6;
        }
        .modal-content{
          font-family: "Times New Roman", Times, serif;

        }
       .padding-top {
         padding-top:20px;
       }
        .watermark{
          -webkit-transform: rotate(331deg);
          -moz-transform: rotate(331deg);
          -o-transform: rotate(331deg);
          transform: rotate(331deg);
          font-size: 15em;
          color: rgba(255, 5, 5, 0.37);
          position: absolute;
          text-transform:uppercase;
          padding-left: 10%;
          margin-top:-10px;
        }
        .right{
          text-align:left;
        }
        .px-2 {
          padding-left: 10px !important;
        }
        thead > tr
        {
            border-style: solid;
            border: 3px solid rgb(0, 0, 0);
        }
        table tr td.classname{
          border-right: 3px solid rgb(0, 0, 0) !important;
        }
        table tr td.bottom{
          border-bottom: 3px solid rgb(0, 0, 0) !important;
        }
        table tr th.classname{
          border-right: 3px solid rgb(0, 0, 0) !important;
        }
        .divborder {
          border-right: 3px solid rgb(0, 0, 0) !important;
          line-height: 1.6;
        }
        .lastdivborder{
          line-height: 1.6;
        }
    .border{
      border-right: 3px solid rgb(0, 0, 0) !important;
      border-bottom: 3px solid rgb(0, 0, 0) !important;
    }

    .tdborder{
      border-right: 3px solid rgb(0, 0, 0) !important;
    }
        .invoice {
          margin-top: 250px;
      }
        .card{
          border-style: solid;
          border-width: 5px;
          border: 3px solid rgb(0, 0, 0);
      }
        .printMe {
          display: none !important;
        }
      }
      body{
            font-size: 15px;
            line-height: 18px;
      }
 </style>
        </head>
    <body onload="window.print();window.close()">
    ${printOrderContents}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }

  printOrderReceipt() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.orderDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printOrderReceiptContents, popupWin;
      printOrderReceiptContents = document.getElementById('DisplayOrderReceiptData').innerHTML;
      popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
      popupWin.document.open();
      popupWin.document.write(`
      <html>
        <head>
          <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css" integrity="sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm" crossorigin="anonymous">
          <title>Print tab</title>
          <style>
         .htmlPrint{display:none;}

          @media print {
            .table-bordered
        {
            border-style: solid;
            border: 3px solid rgb(0, 0, 0);
        }
        .margin{
          margin-top:-24px;
        }
        .mb{
          vertical-align:middle;position:absolute;
        }
        tr.spaceUnder>td {
          padding-bottom: 40px !important;
        }
        .top {
          margin-top:10px;
          border-bottom: 3px solid rgb(0, 0, 0) !important;
          line-height: 1.6;
        }
        .modal-content{
          font-family: "Times New Roman", Times, serif;

        }
       .padding-top {
         padding-top:20px;
       }
        .watermark{
          -webkit-transform: rotate(331deg);
          -moz-transform: rotate(331deg);
          -o-transform: rotate(331deg);
          transform: rotate(331deg);
          font-size: 15em;
          color: rgba(255, 5, 5, 0.37);
          position: absolute;
          text-transform:uppercase;
          padding-left: 10%;
          margin-top:-10px;
        }
        .right{
          text-align:left;
        }
        .px-2 {
          padding-left: 10px !important;
        }
        thead > tr
        {
            border-style: solid;
            border: 3px solid rgb(0, 0, 0);
        }
        table tr td.classname{
          border-right: 3px solid rgb(0, 0, 0) !important;
        }
        table tr td.bottom{
          border-bottom: 3px solid rgb(0, 0, 0) !important;
        }
        table tr th.classname{
          border-right: 3px solid rgb(0, 0, 0) !important;
        }
        .divborder {
          border-right: 3px solid rgb(0, 0, 0) !important;
          line-height: 1.6;
        }
        .lastdivborder{
          line-height: 1.6;
        }
    .border{
      border-right: 3px solid rgb(0, 0, 0) !important;
      border-bottom: 3px solid rgb(0, 0, 0) !important;
    }

    .tdborder{
      border-right: 3px solid rgb(0, 0, 0) !important;
    }
        .invoice {
          margin-top: 250px;
      }
        .card{
          border-style: solid;
          border-width: 5px;
          border: 3px solid rgb(0, 0, 0);
      }
        .printMe {
          display: none !important;
        }
      }
      body{
            font-size: 15px;
            line-height: 18px;
      }
 </style>
        </head>
    <body onload="window.print();window.close()">
    ${printOrderReceiptContents}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }

  printClosedOrder() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.orderDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printClosedOrderContents, popupWin;
      printClosedOrderContents = document.getElementById('DisplayClosedOrderData').innerHTML;
      popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
      popupWin.document.open();
      popupWin.document.write(`
      <html>
        <head>
          <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css" integrity="sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm" crossorigin="anonymous">
          <title>Print tab</title>
          <style>
         .htmlPrint{display:none;}

          @media print {
            .table-bordered
        {
            border-style: solid;
            border: 3px solid rgb(0, 0, 0);
        }
        .margin{
          margin-top:-24px;
        }
        .mb{
          vertical-align:middle;position:absolute;
        }
        tr.spaceUnder>td {
          padding-bottom: 40px !important;
        }
        .top {
          margin-top:10px;
          border-bottom: 3px solid rgb(0, 0, 0) !important;
          line-height: 1.6;
        }
        .modal-content{
          font-family: "Times New Roman", Times, serif;

        }
       .padding-top {
         padding-top:20px;
       }
        .watermark{
          -webkit-transform: rotate(331deg);
          -moz-transform: rotate(331deg);
          -o-transform: rotate(331deg);
          transform: rotate(331deg);
          font-size: 15em;
          color: rgba(255, 5, 5, 0.37);
          position: absolute;
          text-transform:uppercase;
          padding-left: 10%;
          margin-top:-10px;
        }
        .right{
          text-align:left;
        }
        .px-2 {
          padding-left: 10px !important;
        }
        thead > tr
        {
            border-style: solid;
            border: 3px solid rgb(0, 0, 0);
        }
        table tr td.classname{
          border-right: 3px solid rgb(0, 0, 0) !important;
        }
        table tr td.bottom{
          border-bottom: 3px solid rgb(0, 0, 0) !important;
        }
        table tr th.classname{
          border-right: 3px solid rgb(0, 0, 0) !important;
        }
        .divborder {
          border-right: 3px solid rgb(0, 0, 0) !important;
          line-height: 1.6;
        }
        .lastdivborder{
          line-height: 1.6;
        }
    .border{
      border-right: 3px solid rgb(0, 0, 0) !important;
      border-bottom: 3px solid rgb(0, 0, 0) !important;
    }

    .tdborder{
      border-right: 3px solid rgb(0, 0, 0) !important;
    }
        .invoice {
          margin-top: 250px;
      }
        .card{
          border-style: solid;
          border-width: 5px;
          border: 3px solid rgb(0, 0, 0);
      }
        .printMe {
          display: none !important;
        }
      }
      body{
            font-size: 15px;
            line-height: 18px;
      }
 </style>
        </head>
    <body onload="window.print();window.close()">
    ${printClosedOrderContents}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }


  // Added for Plain Text printing related to Order

  closeOrderTab() {
    // if (this._router.url === "/orders") {
    //   this.getOrderPlainText(this.OrderNum);
    // }
    // else {
    //   $('#OrderPlainTextTab').modal('hide');
    // }
  }


  getOrderPlainText(arg) {
    this._orderService.getOrderPrintHTML(arg).subscribe(
      response => {
        this.orderPlainTextDetails = response;
        $('#OrderPlainTextTab').modal('show');
      }
    )
  }


  printOrderPlainText() {
    this._masterService.printPlainText(this.orderPlainTextDetails);
  }

  // Ends Here


  // Added for Plain Text printing related to Order Receipt

  closeOrderReceiptTab() {
    if (this._router.url === "/orders" || this._router.url === "/orders/OrderReceipt") {
      //this.getOrderReceiptPlainText(this.ReceiptNum);
    }
    else {
      $('#OrderReceiptPlainTextTab').modal('hide');
    }
  }


  getOrderReceiptPlainText(arg) {
    this._orderService.getOrderReceiptPrintHTML(arg).subscribe(
      response => {
        this.orderReceiptPlainTextDetails = response;
        $('#OrderReceiptPlainTextTab').modal('show');
      }
    )
  }

  printOrderReceiptPlainText() {
    this._masterService.printPlainText(this.orderReceiptPlainTextDetails);
  }

  // Ends Here



  // Added for Plain Text printing related to Closed Order

  closeOrdersTab() {
    if (this._router.url === "/orders/CloseOrder") {
      this.getClosedOrderPlainText(this.ClosedOrderNum);
    }
    else {
      $('#ClosedOrderPlainTextTab').modal('hide');
    }
  }


  getClosedOrderPlainText(arg) {
    this._orderService.getClosedOrderPrintHTML(arg).subscribe(
      response => {
        this.closedOrderPlainTextDetails = response;
        $('#ClosedOrderTab').modal('hide');
        $('#ClosedOrderPlainTextTab').modal('show');
      }
    )
  }

  printClosedOrderPlainText() {
    this._masterService.printPlainText(this.closedOrderPlainTextDetails);
  }

  // Ends Here

}