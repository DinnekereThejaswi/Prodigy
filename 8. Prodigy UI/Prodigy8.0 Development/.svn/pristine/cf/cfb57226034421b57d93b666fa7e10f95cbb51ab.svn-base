import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { OnlineOrderManagementSystemService } from '../online-order-management-system.service';
import { AppConfigService } from '../../AppConfigService';
import { MasterService } from '../../core/common/master.service';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { NgxSpinnerService } from "ngx-spinner";
import * as moment from 'moment';
import { ToastrService } from 'ngx-toastr';
declare var $: any;

@Component({
  selector: 'app-item-dispatch',
  templateUrl: './item-dispatch.component.html',
  styleUrls: ['./item-dispatch.component.css']
})
export class ItemDispatchComponent implements OnInit {

  model = { option: 'Amazon' };
  radioItems: Array<string>;
  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  @ViewChild("AwbNo", { static: true }) AwbNo: ElementRef;

  postShipJson: any = {
    CompanyCode: "",
    BranchCode: "",
    PickupAgentName: "",
    PickupAgentMobileNo: "",
    PickupRemarks: "",
    OrderList: []
  }

  searchText: string;


  constructor(private _onlineOrderManagementSystemService: OnlineOrderManagementSystemService,
    private _appConfigService: AppConfigService, private SpinnerService: NgxSpinnerService,
    private _masterService: MasterService, private toastr: ToastrService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.postShipJson.CompanyCode = this.ccode;
    this.postShipJson.BranchCode = this.bcode;
  }

  ngOnInit() {
    this.marketPlace();
    this.getApplicationDate();
  }

  applicationDate: any;
  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
      }
    )
  }

  marketPlaceList: any = [];

  marketPlace() {
    this._onlineOrderManagementSystemService.getmarketplaces().subscribe(response => {
      this.marketPlaceList = response;
    })
  }

  date: string = "";
  time: string = "";
  date1: string = "";
  time1: string = "";
  currentDate: string = "";
  appDate: string = "";

  getDate() {
    var cdate = new Date();
    var aDate = this.applicationDate;
    this.date = new Date(cdate).toLocaleDateString();
    this.date = moment(this.date, 'MM/DD/YYYY').format('DD/MM/YYYY');
    this.date1 = moment(aDate, 'YYYY-MM-DD').format('DD/MM/YYYY');
    this.time = new Date(cdate).toLocaleTimeString();
    this.time1 = new Date(aDate).toLocaleTimeString();
    this.currentDate = this.date + " " + this.time;
    this.appDate = this.date1 + " " + this.time1;
  }

  Company: any;

  getCompany() {
    this._masterService.getCompanyMaster().subscribe(
      response => {
        this.Company = response;
      }
    )
  }


  orderstobeshippedList: any = [];

  orderstobeshipped(arg) {
    if (arg == "null") {
      swal("Warning!", 'Please select the Market Place', "warning");
    }
    else {
      this.marketPlaceID = arg;
      this.OriginalCount = 0;
      this.ShippedCount = 0;
      this._onlineOrderManagementSystemService.orderstobeshipped(arg).subscribe(
        response => {
          this.orderstobeshippedList = response;
          this.OriginalCount = this.orderstobeshippedList.length;
        },
        (err) => {
          this.orderstobeshippedList = [];
        }
      )
    }
  }

  checkedAll: boolean = false;
  checked: boolean = false;
  marketPlaceID: string = "";

  pushAllDataToShipList(e) {
    if (e.target.checked) {
      this.postShipJson.OrderList = [];
      for (let i = 0; i < this.orderstobeshippedList.length; i++) {
        this.postShipJson.OrderList.push(this.orderstobeshippedList[i]);
      }
      this.checkedAll = true;
      this.checked = true;
    }
    else {
      this.checkedAll = false;
      this.checked = false;
      this.postShipJson.OrderList = [];
    }
  }

  resetExistingOrderGrid() {
    this.orderstobeshippedList = [];
    this.postShipJson = {
      CompanyCode: "",
      BranchCode: "",
      PickupAgentName: "",
      PickupAgentMobileNo: "",
      PickupRemarks: "",
      OrderList: []
    }
    this.getCB();
  }

  pushDataToShipList(e, index, option) {
    if (e.target.checked) {
      this.postShipJson.OrderList.push(option);
    }
    else {
      // this.checkedAll = false;
      for (let i = 0; i < this.postShipJson.OrderList.length; i++) {
        if (this.postShipJson.OrderList[i].OrderNo == option.OrderNo) {
          this.postShipJson.OrderList.splice(i, 1);
          break;
        }
      }
    }
  }

  dataExistbyAwbNo(AwbNo) {
    if (this.orderstobeshippedList.length > 0 && AwbNo == "") {
      swal("Warning!", 'Please enter the Awb No', "warning");
    }
    else {
      let finaldata = this.postShipJson.OrderList.find(x => x.AwbNo == AwbNo);
      if (finaldata == null) {
        let defaultdata = this.orderstobeshippedList.find(x => x.AwbNo == AwbNo);
        if (defaultdata == null) {
          this.toastr.warning('Please enter the valid Awb No', 'Alert!');
          this.AwbNo.nativeElement.focus();
          this.AwbNo.nativeElement.value = null;
          return false;
        }
        else {
          this.AwbNo.nativeElement.value = null;
          this.postShipJson.OrderList.push(defaultdata);
          return true;
        }
      }
      else {
        this.toastr.warning('This Shipment ID has been already added to list', 'Alert!');
        this.AwbNo.nativeElement.value = null;
        return true;
      }
    }
  }

  dataExistbyOrderNo(OrderNo) {
    let data = this.postShipJson.OrderList.find(x => x.OrderNo == OrderNo);
    if (data == null) {
      return false;
    }
    else {
      return true;
    }
  }

  outputData: any = [];

  ShippedCount: number = 0;
  OriginalCount: number = 0;

  ItemToDispatch() {
    if (this.postShipJson.OrderList.length == 0) {
      swal("Warning!", 'No items selected for shipping', "warning");
    }
    else if (this.postShipJson.PickupAgentName == "") {
      swal("Warning!", 'Please enter the courier person name', "warning");
    }
    else if (this.postShipJson.PickupAgentMobileNo == "") {
      swal("Warning!", 'Please enter the courier person mobile number', "warning");
    }
    else {
      var ans = confirm("Do you want to dispatch the selected items?");
      if (ans) {
        this.SpinnerService.show();
        this.ShippedCount = this.postShipJson.OrderList.length;
        this._onlineOrderManagementSystemService.postShipment(this.postShipJson).subscribe(
          response => {
            this.outputData = response;
            this.SpinnerService.hide();
            swal("Saved!", "Items has been dispatched successfully", "success");
            this.resetExistingOrderGrid();
            this.getCB();
            if (this.OriginalCount != this.ShippedCount) {
              this.orderstobeshipped(this.marketPlaceID);
            }
          },
          (err) => {
            this.SpinnerService.hide();
          }
        )
      }
    }
  }

  enablePrint: boolean = false;

  printDispatchListDets() {
    if (this.postShipJson.OrderList.length == 0) {
      swal("Warning!", 'Please select atleast one items for print', "warning");
    }
    else {
      this.enablePrint = true;
      $('#printDispatchListModal').modal('show');
      this.getCompany();
      this.getDate();
    }
  }

  print() {
    let printContents, popupWin;
    printContents = document.getElementById('print-section').innerHTML;
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
      tbody > tr
      {
          border-style: solid;
          border: 3px solid rgb(0, 0, 0);
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