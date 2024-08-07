import { Component, OnInit } from '@angular/core';
import { SalesreturnService } from '../salesreturn.service';
import { Router } from '@angular/router';
import { PurchaseService } from '../../purchase/purchase.service';
import { MasterService } from '../../core/common/master.service';
import { ToastrService } from 'ngx-toastr';
import swal from 'sweetalert';
import * as qz from 'qz-tray';
import { CreditReceiptService } from '../../credit-receipt/credit-receipt.service';
declare var qz: any;
declare var jquery: any;
declare var $: any;


@Component({
  selector: 'app-reprint-salesreturn',
  templateUrl: './reprint-salesreturn.component.html',
  styleUrls: ['./reprint-salesreturn.component.css']
})
export class ReprintSalesreturnComponent implements OnInit {
  radioItems: Array<string>;
  model = { option: 'SR Bill No' };

  constructor(private _SalesreturnService: SalesreturnService, private _purchaseService: PurchaseService,
    private _router: Router, private toastr: ToastrService, private _masterService: MasterService,
    private Service: CreditReceiptService) {
    this.radioItems = ['SR Bill No', 'SR Est No'];
  }

  EstimationType: string = "";
  estSRDetails: any = [];
  EnableSRReprintHeaders: boolean = false;
  ShowHide: boolean = false;
  SalesEstPrintTotals: any = [];

  SRDetails: any = [];
  getSRTotForPrint: any;
  heading: string;
  ReprintData: any;
  PrintDetails: any = [];

  isDisabled: boolean = false;

  SrEstNo: any;
  srBillNo: any;

  ngOnInit() {

    if (this._router.url === "/salesreturn/reprint-salesreturn") {
      this.ShowHide = true;
      this._SalesreturnService.SendSRNoToReprintComp(null);
      this._SalesreturnService.SendSRToHide(false);
      this._SalesreturnService.SendSREnablePrint(false);
    }

    this._SalesreturnService.castSRNoToReprintComp.subscribe(
      response => {
        this.SrEstNo = response;
        if (this.SrEstNo != null) {
          if (this._router.url == "/salesreturn" || this._router.url == "/salesreturn/delete-salesreturn") {
            this.model.option = "SR Est No";
            this.getSREstimationDetails(this.SrEstNo);
          }
        }
      }
    )


    this._SalesreturnService.castSRBillNoToReprintComp.subscribe(
      response => {
        this.srBillNo = response;
        if (this.srBillNo != null) {
          if (this.srBillNo.SalesBillNo != 0 && this.srBillNo.SalesBillNo != null) {
            if (this._router.url == "/salesreturn/ConfirmSalesReturn") {
              this.model.option == "SR Bill No"
              this.getSREstimationDetails(this.srBillNo.SalesBillNo);
            }
          }
        }
      }
    )

  }

  handleSelected($event) {
    if ($event.target.checked) {
      this.checked = true;
    }
    else {
      this.checked = false;
    }
  }


  SRBillNo: number;

  isEmptyObject(obj) {
    return (obj && (Object.keys(obj).length === 0));
  }

  PrintTypeBasedOnConfig: any;

  getSREstimationDetails(arg) {
    if (arg == "") {
      swal("Warning!", 'Please Enter SR No', "warning");
    }
    else {
      if (this.model.option == "SR Bill No") {
        this.SRBillNo = arg;
        if (this.checked == true) {
          this._SalesreturnService.getSRPrintbyBillNo(arg).subscribe(
            response => {
              this.PrintTypeBasedOnConfig = response;
              if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
                this.PrintDetails = atob(this.PrintTypeBasedOnConfig.Data);
                $('#ReprintSRBillModal').modal('show');
                $('#DisplaySRBillData').html(this.PrintDetails);
                $('#ReprintSREstModal').modal('hide');
              }
              else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
                this.PrintDetails = this.PrintTypeBasedOnConfig.Data;
              }
              // this._SalesreturnService.getSRPrintbyBillNo(arg).subscribe(
              //   response => {
              //     $('#ReprintSRBillModal').modal('show');
              //     $('#DisplayData').html(this.PrintDetails);
              //     $('#ReprintSREstModal').modal('hide');
              //   });
            }
          )
        }
        else {
          this._SalesreturnService.getSRPrintDotMatrixbyBillNo(arg).subscribe(
            response => {
              this.PrintDetails = response;
              $('#ReprintSRBillModal').modal('hide');
              $('#ReprintSREstModal').modal('show');
            }
          )
        }
      }
      else if (this.model.option == "SR Est No") {
        if (this._router.url != "/salesreturn/ConfirmSalesReturn") {
          this._SalesreturnService.getSRPrintbyEstNo(arg).subscribe(
            response => {
              this.PrintTypeBasedOnConfig = response;
              if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
                this.PrintDetails = atob(this.PrintTypeBasedOnConfig.Data);
                $('#DisplaySREstData').html(this.PrintDetails);
                $('#ReprintSREstModal').modal('show');
                $('#ReprintSRBillModal').modal('hide');
              }
              else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
                this.PrintDetails = this.PrintTypeBasedOnConfig.Data;
                $('#ReprintSREstModal').modal('show');
                $('#ReprintSRBillModal').modal('hide');
              }
            }
          )
        }
      }
    }
  }

  radioBtnValue: string = '';

  checked: boolean = true;

  Changed(arg) {
    this.radioBtnValue = arg;
    this.model.option = arg;
    if (this.model.option == "SR Bill No") {
      this.isDisabled = false;
      this.checked = true;
    }
    else {
      this.isDisabled = true;
      this.checked = false;
    }
  }

  printSREst() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.PrintDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printSREstContents, popupWin;
      printSREstContents = document.getElementById('DisplaySREstData').innerHTML;
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

      ${printSREstContents}</body>
        </html>`
      );
      popupWin.document.close();
    }
  }

  printSRBill() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.PrintDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printSRBillContents, popupWin;
      printSRBillContents = document.getElementById('DisplaySRBillData').innerHTML;
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

      ${printSRBillContents}</body>
        </html>`
      );
      popupWin.document.close();
    }
  }


  closeSRBill() {
    // if (this._router.url === "/salesreturn/ConfirmSalesReturn") {
    //   this._SalesreturnService.getSRPrintDotMatrixbyBillNo(this.SRBillNo).subscribe(
    //     response => {
    //       this.PrintDetails = response;
    //       $('#ReprintSRBillModal').modal('hide');
    //       $('#ReprintSREstModal').modal('show');
    //     }
    //   )
    // }
    // else {
    //   $('#ReprintSRBillModal').modal('hide');
    //   $('#ReprintSREstModal').modal('hide');
    // }
    $('#ReprintSRBillModal').modal('hide');
  }

  ReceiptDetails: any = [];
  Linesarray: any = [];
  PrintCreditReceipt() {
    if (this._router.url == "/salesreturn/ConfirmSalesReturn") {
      if (this.srBillNo.CreditReceiptNo != 0 && this.srBillNo.CreditReceiptNo != null) {
        // this.Service.getReceiptValuesForPrint(this.srBillNo.CreditReceiptNo).subscribe(
        //   Response => {
        //     this.ReceiptDetails = Response;
        //     this.Linesarray = this.ReceiptDetails.lstOfPayment;
        //     this.getShowroomDetails();
        //     this.getbillDetails(this.ReceiptDetails.BillNo, this.ReceiptDetails.FinYear);
        //     $('#CreditReceiptModal').modal('show');
        //     $('#ReprintSRBillModal').modal('hide');
        //     $('#ReprintSREstModal').modal('hide');
        //   }
        // )
        this.Service.getReceiptValuesForPrint(this.srBillNo.CreditReceiptNo).subscribe(
          response => {
            this.PrintTypeBasedOnConfig = response;
            if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
              this.ReceiptDetails = atob(this.PrintTypeBasedOnConfig.Data);
              $('#DisplayCreditReceiptData').html(this.ReceiptDetails);
              $('#CreditReceiptModal').modal('show');
              $('#ReprintSRBillModal').modal('hide');
              $('#ReprintSREstModal').modal('hide');
            }
            else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
              this.ReceiptDetails = this.PrintTypeBasedOnConfig.Data;
              $('#CreditReceiptModal').modal('show');
              $('#ReprintSRBillModal').modal('hide');
              $('#ReprintSREstModal').modal('hide');
            }
          }
        );
      }
    }
  }

  SummaryHeader: any = {
    ReceiptNo: null,
    BillNo: null,
    BalanceAmount: null,
    BilledDate: null
  }

  ShowroomList: any = [];
  getShowroomDetails() {
    this.Service.getShowroom().subscribe(
      response => {
        this.ShowroomList = response;

      }
    );
  }
  BillDetails: any;
  getbillDetails(arg1, arg2) {
    this.Service.getValues(arg1, arg2).subscribe(
      Response => {
        this.BillDetails = Response;
        this.SummaryHeader.BillNo = this.BillDetails.BillNo;
        this.SummaryHeader.BalanceAmount = this.BillDetails.BalanceAmount;

      }
    )
  }
  print() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.ReceiptDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printCreditReceiptContents, popupWin;
      printCreditReceiptContents = document.getElementById('DisplayCreditReceiptData').innerHTML;
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
        // thead > tr
        // {
        //     border-style: solid;
        //     border: 3px solid rgb(0, 0, 0);
        // }
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
    ${printCreditReceiptContents}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }
}