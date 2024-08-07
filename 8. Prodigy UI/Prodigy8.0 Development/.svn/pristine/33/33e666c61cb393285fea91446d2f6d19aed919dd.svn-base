import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { RepairService } from '../repair.service';
import * as CryptoJS from 'crypto-js';
import { Router } from '@angular/router';
import { MasterService } from '../../core/common/master.service';
import { AppConfigService } from '../../AppConfigService';
import swal from 'sweetalert';

declare var jquery: any;
declare var $: any;

@Component({
  selector: 'app-repair-reprint',
  templateUrl: './repair-reprint.component.html',
  styleUrls: ['./repair-reprint.component.css']
})
export class RepairReprintComponent implements OnInit {
  radioItems: Array<string>;
  radioReprintTypeItems: Array<string>;
  isChecked: boolean = false;
  isdirect: number = 0;
  @Input() RepairReceiptNo: string = "";
  @Input() RepairDeliveryNo: string = "";
  RepaireReprint: FormGroup;
  heading: string = "Repair Receipt";
  EnableHeader: boolean = true;
  ccode: string = "";
  bcode: string = "";
  RepairReceiptPlainTextDetails: any = [];
  RepairDeliveryPlainTextDetails: any = [];
  RepairNum: number;

  model = { option: 'Delivery No' };
  modelReprintType = { option: 'Original' };
  password: string;
  constructor(private formBuilder: FormBuilder, private _repairService: RepairService,
    private _router: Router, private _masterService: MasterService,
    private _appConfigService: AppConfigService) {
    this.radioItems = ['Delivery No', 'Receipt No'];
    this.radioReprintTypeItems = ['Original', 'Duplicate', 'Triplicate'];
    this.password = this._appConfigService.Pwd;
    this.getCB();
    if (this._router.url == "/repair/repair-reprint") {
      this._repairService.SendReceiptDeliveryNoToReprintComp(null);
    }
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }
  repairDeliverySummaryHeader: any = {
    RepairNo: null,
  }
  ReceiptDeliveryNoToReprintComp: string;
  ngOnInit() {
    this.RepaireReprint = this.formBuilder.group({
      DeliveryNo: null,
      ReceiptNo: null
    });

    this._repairService.castReceiptDeliveryNoToReprintComp.subscribe(
      response => {
        this.ReceiptDeliveryNoToReprintComp = response;
        if (this.ReceiptDeliveryNoToReprintComp != null) {
          if (this._router.url == "/repair/repair-receipts") {
            this.RepairReceiptNo = this.ReceiptDeliveryNoToReprintComp;
            this.EnableHeader = false;
            this.model.option = "Receipt No";
            this.ReprintType = "ORIGINAL";
            this.getReceiptPrint(this.RepairReceiptNo);
          }
          else if (this._router.url == "/repair/repair-delivery") {
            this.RepairDeliveryNo = this.ReceiptDeliveryNoToReprintComp;
            this.EnableHeader = false;
            this.model.option = "Delivery No";
            this.getDeliveryNoDetails(this.RepairDeliveryNo);
          }
        }
      }
    )
  }
  Changed(arg) {
    this.isChecked = false;
    if (arg === 'Delivery No') {
      this.model.option = arg;
      this.repairDeliverySummaryHeader.RepairNo = "";
    }
    else if (arg === 'Receipt No') {
      this.model.option = arg;
      this.repairDeliverySummaryHeader.ReceiptNo = "";
    }
  }

  onPrint(arg): void {
    if (this.model.option == 'Delivery No') {
      this.heading = "Delivery Receipt";
      this.getDeliveryNoDetails(arg.value.DeliveryNo);
    }
    else if (this.model.option == 'Receipt No') {
      this.heading = "REPAIR RECEIPT";
      //this.getReceiptPrint(arg.value.ReceiptNo);
      if (!arg.value.ReceiptNo) {
        swal("Warning!", 'Please enter ReceiptNo', "warning");
        $('#RepairModal').modal('hide');
      }
      else {
        this.modelReprintType.option = "Original";
        // $('#OrderTab').modal('hide');
        $('#ReprintTypeModal').modal('show');
      }
    }
  }

  CanelPrint() {
    $('#ReprintTypeModal').modal('hide');
  }


  ReprintType: string = "";

  onReprint(arg) {
    this.ReceiptNo = arg.value.ReceiptNo;
    this.ReprintType = this.modelReprintType.option;
    this.getReceiptPrint(arg.value.ReceiptNo);
  }

  Cash: any;

  ReceiptNo: any;
  DeliveryNo: any;
  Deliveryarray: any = [];
  DeliveryNoDetails: any = [];
  getDeliveryNoDetails(arg) {
    if (!arg) {
      swal("Warning!", 'Please enter Delivery No', "warning");
      $('#RepairModal').modal('hide');
    }
    else {
      // this._repairService.getDeliveryNoDetails(arg).subscribe(
      //   Response => {
      //     this.DeliveryNoDetails = Response;
      //     if (this.DeliveryNoDetails.lstOfPayment != null) {
      //       this.Cash = this.DeliveryNoDetails.lstOfPayment.filter(value => value.PayMode === "C")
      //     }
      //     this.ReceiptNo = this.DeliveryNoDetails.ReceiptNo;
      //     this.DeliveryNo = arg;
      //     this.Deliveryarray = this.DeliveryNoDetails.lstOfRepairIssueDetails;
      //     this.getReceiptPrint(this.ReceiptNo);
      //     this.getDeliveryNoTotal(arg);
      //     this.getShowroomDetails();
      //     if (this.isChecked == false) {
      //       $('#RepairModal').modal('show');
      //       $('#RepairDeliveryDirectModal').modal('hide');
      //     }
      //     else {
      //       $('#RepairModal').modal('hide');
      //       $('#RepairDeliveryDirectModal').modal('show');
      //     }
      //   }
      // )

      this.ReprintType = "ORIGINAL";

      this._repairService.getRepairDeliveryPrint(arg, this.ReprintType, this.isdirect).subscribe(
        response => {
          this.PrintTypeBasedOnConfig = response;
          if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
            this.DeliveryNoDetails = atob(this.PrintTypeBasedOnConfig.Data);
            $('#RepairDeliveryDirectModal').modal('show');
            $('#DisplayRepairDeliveryData').html(this.DeliveryNoDetails);
          }
          else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
            this.DeliveryNoDetails = this.PrintTypeBasedOnConfig.Data;
            $('#RepairDeliveryDirectModal').modal('show');
          }
          // if (this.isChecked == false) {
          //   $('#RepairModal').modal('show');
          //   $('#RepairDeliveryDirectModal').modal('hide');
          // }
          // else {
          //   $('#RepairModal').modal('hide');
          //   $('#RepairDeliveryDirectModal').modal('show');
          // }
        }
      )

    }
  }
  ReceiptDetails: any = [];
  Linesarray: any = [];
  PrintTypeBasedOnConfig: any;
  getReceiptPrint(arg) {
    if (!arg) {
      swal("Warning!", 'Please enter ReceiptNo', "warning");
      $('#RepairModal').modal('hide');
    }
    else {
      // this._repairService.getReceiptNoDetails(arg).subscribe(
      //   Response => {
      //     this.ReceiptDetails = Response;
      //     this.Linesarray = this.ReceiptDetails.lstOfRepairReceiptDetails;
      //     if (this._router.url == "/repair/repair-receipts") {
      //       this.ReprintType = "Original";
      //     }
      //     this.getShowroomDetails();
      //     this.getReceiptTotal(arg);
      //     $('#RepairModal').modal('show');
      //     $('#ReprintTypeModal').modal('hide');
      //   }
      // )

      this._repairService.getRepairReceiptPrint(arg, this.ReprintType.toUpperCase()).subscribe(
        response => {
          this.PrintTypeBasedOnConfig = response;
          if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
            this.ReceiptDetails = atob(this.PrintTypeBasedOnConfig.Data);
            $('#RepairModal').modal('show');
            $('#ReprintTypeModal').modal('hide');
            $('#DisplayRepairReceiptsData').html(this.ReceiptDetails);
          }
          else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
            this.ReceiptDetails = this.PrintTypeBasedOnConfig.Data;
            $('#RepairModal').modal('show');
            $('#ReprintTypeModal').modal('hide');
          }
        }
      )
    }
  }
  ReceiptTotal: any;
  getReceiptTotal(arg) {
    this._repairService.getReceiptTotal(arg).subscribe(
      Response => {
        this.ReceiptTotal = Response;

      }
    )
  }

  FieldsChange(values: any) {
    this.isChecked = values.currentTarget.checked;
    if (this.isChecked) {
      this.isdirect = 1;
    }
    else {
      this.isdirect = 0;
    }
  }

  DeliveryNoTotal: any = [];
  DeliveryTotalArray: any;
  payment: any;
  getDeliveryNoTotal(arg) {
    this._repairService.getDeliveryTotal(arg).subscribe(
      Response => {
        this.DeliveryNoTotal = Response;
        this.DeliveryTotalArray = this.DeliveryNoTotal.lstOfRepairIssueDetails;
        this.payment = this.DeliveryNoTotal.lstOfPayment;
      },
      (err) => {
        this.DeliveryNoTotal = null;
      }
    )
  }
  ShowroomList: any = [];
  getShowroomDetails() {
    this._repairService.getShowroom().subscribe(
      response => {
        this.ShowroomList = response;
      }
    );
  }


  ChangedReprintType(arg) {
    this.modelReprintType.option = arg;
  }


  printRepairReceipts() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.ReceiptDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      this.printRepair();
    }
  }


  printRepair() {
    let printRepairContents, popupWin;
    printRepairContents = document.getElementById('DisplayRepairReceiptsData').innerHTML;
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
        table tr td.left{
          border-left: 3px solid rgb(0, 0, 0) !important;
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
    ${printRepairContents}</body>
      </html>`
    );
    popupWin.document.close();
  }

  printRepairDelivery() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.DeliveryNoDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printRepairDeliveryContents, popupWin;
      printRepairDeliveryContents = document.getElementById('DisplayRepairDeliveryData').innerHTML;
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
          table tr td.left{
            border-left: 3px solid rgb(0, 0, 0) !important;
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
      ${printRepairDeliveryContents}</body>
        </html>`
      );
      popupWin.document.close();
    }
  }


  closeRepairTab() {
    //Commented on 10-Mar-2021 as per sivanand not to include dot-matrix print
    // if (this._router.url === "/repair/repair-receipts") {
    //   this.getRepairReceiptPlainText(this.RepairReceiptNo);
    // }
    // else if (this._router.url === "/repair/repair-delivery") {
    //   this.getRepairDeliveryPlainText(this.RepairDeliveryNo);
    // }
    // else if (this._router.url === "/repair/repair-reprint") {
    //   if (this.model.option == "Delivery No") {
    //     this.getRepairDeliveryPlainText(this.DeliveryNo);
    //   }
    //   else if (this.model.option == "Receipt No") {
    //     this.getRepairReceiptPlainText(this.ReceiptNo);
    //   }
    // }
    // else {
    //   $('#RepairReceiptPlainTextTab').modal('hide');
    //   $('#RepairDeliveryPlainTextTab').modal('hide');
    // }
    //ends here
  }


  // Added for Plain Text printing related to Repair Receipt

  getRepairReceiptPlainText(arg) {
    this._repairService.getRepairReceiptPrintHTML(arg).subscribe(
      response => {
        this.RepairReceiptPlainTextDetails = response;
        $('#RepairReceiptPlainTextTab').modal('show');
      }
    )
  }


  printRepairReceiptPlainText() {
    this._masterService.printPlainText(this.RepairReceiptPlainTextDetails);
  }

  // Ends Here


  // Added for Plain Text printing related to Repair Delivery

  getRepairDeliveryPlainText(arg) {
    this._repairService.getRepairDeliveryPrintHTML(arg).subscribe(
      response => {
        this.RepairDeliveryPlainTextDetails = response;
        $('#RepairDeliveryPlainTextTab').modal('show');
      }
    )
  }

  printRepairDeliveryPlainText() {
    this._masterService.printPlainText(this.RepairDeliveryPlainTextDetails);
  }

  // Ends Here
}
