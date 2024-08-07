import { Component, OnInit } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { ReceiveOrder } from '../../online-order-management-system/online-order-management-system.model';
import { MasterService } from '../../core/common/master.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { formatDate } from '@angular/common';
import { AppConfigService } from '../../AppConfigService';
import { OnlineOrderManagementSystemService } from '../../online-order-management-system/online-order-management-system.service';
import { ToastrService } from 'ngx-toastr';
import * as moment from 'moment';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
declare var $: any;


@Component({
  selector: 'app-assignment-generation',
  templateUrl: './assignment-generation.component.html',
  styleUrls: ['./assignment-generation.component.css']
})
export class AssignmentGenerationComponent implements OnInit {
  ReceivedOrderJson: ReceiveOrder = {
    CompanyCode: "",
    BranchCode: "",
    FromDate: "",
    ToDate: "",
    MarketplaceID: null,
    Type: null
  };
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;
  EnableJson: boolean = true;
  ReceivedOrdersForm: FormGroup;
  datePickerConfig: Partial<BsDatepickerConfig>;
  checkedAll: boolean = false;
  checkedTopOrders: boolean = true;
  orderStages: any;
  checkItems: Array<string>;
  model = { option: 'Open' };
  today = new Date();
  defaultOrderCount: number = 20;
  OrderPerAssignmentByDefault: number = 0;
  OrderPerAssignment: number = 0;
  shipmentEndDate: number = 0;
  constructor(private _masterService: MasterService, private fb: FormBuilder, private appConfigService: AppConfigService,
    private _onlineOrderManagementSystemService: OnlineOrderManagementSystemService,
    private toastr: ToastrService) {
    this.checkItems = ['All', 'Open', 'Under Process', 'Picked', 'Packed', 'Ready', 'Shipped', 'Cancelled'];
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        dateInputFormat: 'DD/MM/YYYY'
      });
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.EnableJson = this.appConfigService.EnableJson;
    this.password = this.appConfigService.Pwd;
    this.shipmentEndDate = this.appConfigService.ShipmentEndDate;
    this.getCB();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.ReceivedOrderJson.CompanyCode = this.ccode;
    this.ReceivedOrderJson.BranchCode = this.bcode;
  }

  ngOnInit() {
    this.getApplicationDate();
    this.marketPlace();
    this.getOrderStates();
    this.getPendingOrderCount();
    this.ReceivedOrdersForm = this.fb.group({
      FromDate: [null, Validators.required],
      ToDate: [null, Validators.required],
      MarketplaceID: [null, Validators.required],
      Type: [null, Validators.required],
    });
    $('#printReceivedOrderModal').modal('hide');
  }

  getOrderStates() {
    this._onlineOrderManagementSystemService.getOrderStages().subscribe(response => {
      this.orderStages = response;
      this.ReceivedOrderJson.Type = "Open";
    })
  }

  PendingOrderCount: any = [];

  getPendingOrderCount() {
    this._onlineOrderManagementSystemService.getPendingOrderCount().subscribe(response => {
      this.PendingOrderCount = response;
    })
  }

  LoadOrderPerAssignment(form) {
    if (form.value.MarketplaceID == null || form.value.MarketplaceID == "") {
      swal("Warning!", 'Please select the Market Place', "warning");
    }
    else {
      this.OnlineOrder = [];
      this.pushReceivedOrder = [];
      let data = this.PendingOrderCount.find(x => x.MarketPlace == form.value.MarketplaceID);
      if (data != null) {
        //this.OrderPerAssignment = data.Count;
        this.OrderPerAssignmentByDefault = data.Count;
        this.OrderPerAssignment = this.defaultOrderCount;
      }
    }
  }

  applicationDate: any;
  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
        this.ReceivedOrderJson.FromDate = formatDate(this.applicationDate, 'dd/MM/yyyy', 'en_GB');
        this.addDays();
      }
    )
  }

  futureDate: any;
  addDays() {
    this.futureDate = moment(this.applicationDate).add(this.shipmentEndDate, 'days');
    this.ReceivedOrderJson.ToDate = formatDate(this.futureDate, 'dd/MM/yyyy', 'en_GB');
  }

  pushReceivedOrder: any = [];

  // checked: boolean = false;
  otherStatuschecked: boolean = false;

  pushDataToPickList(e, index, option) {
    if (e.target.checked) {
      this.pushReceivedOrder.push(this.OnlineOrder[index]);
    }
    else {
      for (let i = 0; i < this.pushReceivedOrder.length; i++) {
        if (this.pushReceivedOrder[i].OrderNo == option.OrderNo) {
          this.pushReceivedOrder.splice(i, 1);
          break;
        }
      }
    }
  }

  marketPlaceList: any = [];

  marketPlace() {
    this._onlineOrderManagementSystemService.getmarketplaces().subscribe(response => {
      this.marketPlaceList = response;
    })
  }

  pushAllDataToPickList(e) {
    if (e.target.checked) {
      this.pushReceivedOrder = [];
      for (let i = 0; i < this.OnlineOrder.length; i++) {
        if (this.OnlineOrder[i].Status == "Open") {
          this.pushReceivedOrder.push(this.OnlineOrder[i]);
        }
      }
      this.checkedAll = true;
      //this.checked = true;
    }
    else {
      this.checkedAll = false;
      //this.checked = false;
      this.pushReceivedOrder = [];
    }
  }

  pushTopOrdersDataToPickList(e) {
    if (e.target.checked) {
      this.TopOrderDataToPickList();
      // this.checkedAll = true;
      // this.checked = true;
    }
    else {
      this.checkedAll = false;
      //this.checked = false;
      this.pushReceivedOrder = [];
    }
  }

  TopOrderDataToPickList() {
    this.pushReceivedOrder = [];
    if (this.OnlineOrder.length > 0) {
      for (let i = 0; i < this.OrderPerAssignment; i++) {
        if (this.OnlineOrder[i].Status == "Open") {
          this.pushReceivedOrder.push(this.OnlineOrder[i]);
        }
      }
    }
  }

  dataExist(orderNo) {
    let data = this.pushReceivedOrder.find(x => x.OrderNo == orderNo);
    if (data == null) {
      return false;
    }
    else {
      return true;
    }
  }



  OnlineOrder: any = [];
  NoRecords: boolean = false;

  // pagenumber: number = 1;
  // top = 25;
  // skip = (this.pagenumber - 1) * this.top;


  getOnlineOrders() {
    this.ReceivedOrderJson.FromDate = moment(this.ReceivedOrderJson.FromDate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    this.ReceivedOrderJson.ToDate = moment(this.ReceivedOrderJson.ToDate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    this._onlineOrderManagementSystemService.getOnlineOrders(this.ReceivedOrderJson).subscribe(
      response => {
        this.OnlineOrder = response;
        this.ReceivedOrderJson.FromDate = moment(this.ReceivedOrderJson.FromDate, 'YYYY-MM-DD').format('DD/MM/YYYY');
        this.ReceivedOrderJson.ToDate = moment(this.ReceivedOrderJson.ToDate, 'YYYY-MM-DD').format('DD/MM/YYYY');
        this.checkedTopOrders = true;
        this.TopOrderDataToPickList();
        this.NoRecords = true;
      },
      (err) => {
        this.ReceivedOrderJson.FromDate = moment(this.ReceivedOrderJson.FromDate, 'YYYY-MM-DD').format('DD/MM/YYYY');
        this.ReceivedOrderJson.ToDate = moment(this.ReceivedOrderJson.ToDate, 'YYYY-MM-DD').format('DD/MM/YYYY');
      }
    )
  }

  validateFromDate: string = "";
  validateToDate: string = "";

  Search() {
    this.validateFromDate = moment(this.ReceivedOrderJson.FromDate, 'DD/MM/YYYY').format('YYYYMMDD');
    this.validateToDate = moment(this.ReceivedOrderJson.ToDate, 'DD/MM/YYYY').format('YYYYMMDD');
    if (this.validateFromDate > this.validateToDate) {
      swal("Warning!", 'Shipment End Date should be greater than or equal to Shipment Start Date', "warning");
      this.OnlineOrder = [];
      this.resetValues();
    }
    else if (this.ReceivedOrderJson.MarketplaceID == null || this.ReceivedOrderJson.MarketplaceID == "null") {
      swal("Warning!", 'Please select the Market Place', "warning");
    }
    else if (this.ReceivedOrderJson.Type == null || this.ReceivedOrderJson.Type == "null") {
      swal("Warning!", 'Please select the Status', "warning");
    }
    else {
      this.OnlineOrder = [];
      this.resetValues();
      this.getOnlineOrders();
    }
  }

  PickListResult: any;
  PrintPickList: any;

  AddItemToPickList() {
    if (this.pushReceivedOrder.length == 0) {
      swal("Warning!", 'Please select the items for adding to pick list', "warning");
    }
    else {
      var ans = confirm("Do you want to add items to pick list?");
      if (ans) {
        this._onlineOrderManagementSystemService.createPickList(this.pushReceivedOrder).subscribe(
          response => {
            this.PickListResult = response;
            swal("Saved!", "Item pick-up list added successfully.Assignment No:" + this.PickListResult.AssignmentNo, "success").then(
              (result) => {
                if (result == true) {
                  this.resetValues();
                  this.getOnlineOrders();
                  var ans = confirm("Do You want to print the details?");
                  if (ans) {
                    this._onlineOrderManagementSystemService.PrintPickList(this.PickListResult.AssignmentNo).subscribe(
                      response => {
                        this.PrintPickList = response;
                        $('#printReceivedOrderModal').modal('show');
                      }
                    )
                  }
                }
              })
          }
        )
      }
    }
  }

  chngStatus() {
    this.OnlineOrder = [];
  }

  resetValues() {
    this.checkedAll = false;
    //this.checked = false;
    this.pushReceivedOrder = [];
  }

  // onPageChange(p: number) {
  //   this.pagenumber = p;
  //   this.skip = (this.pagenumber - 1) * this.top;
  //   this.getOnlineOrders();
  // }



  print() {
    let printContents, popupWin;
    printContents = document.getElementById('DisplayData').innerHTML;
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
        table tr td.borderLeft{
          border-left: 3px solid rgb(0, 0, 0) !important;
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

    ${printContents}</body>
      </html>`
    );
    popupWin.document.close();
  }
}