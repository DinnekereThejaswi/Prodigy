import { Component, OnInit, Input } from '@angular/core';
import { CreditReceiptService } from '../credit-receipt.service';
import { MasterService } from '../../core/common/master.service';
import { Router } from '@angular/router';
import { Alert } from 'selenium-webdriver';
import swal from 'sweetalert';
declare var jquery: any;
declare var $: any;

@Component({
  selector: 'app-reprint-credit-receipt',
  templateUrl: './reprint-credit-receipt.component.html',
  styleUrls: ['./reprint-credit-receipt.component.css']
})
export class ReprintCreditReceiptComponent implements OnInit {
  // @Input()
  // ReceiptNo: string = "";
  ShowHide: boolean = true;
  ReprintData: any;
  SummaryHeader: any = {
    ReceiptNo: null,
    BillNo: null,
    BalanceAmount: null,
    BilledDate: null
  }
  constructor(private Service: CreditReceiptService,
    private _router: Router, private _masterService: MasterService) { }

  ngOnInit() {
    if (this._router.url == "/credit-receipt/reprint-credit-receipt") {
      this.ShowHide = true;
    }
    else {
      this.Service.castReceiptNo.subscribe(
        response => {
          this.ReprintData = response;
          if (this.isEmptyObject(this.ReprintData) == false && this.isEmptyObject(this.ReprintData) != null) {
            this.getReceiptPrint(this.ReprintData.ReceiptNo);
            this.Service.SendReceiptNoToComp(null);
          }
          this.ShowHide = false;
        })
    }
  }
  isEmptyObject(obj) {
    return (obj && (Object.keys(obj).length === 0));
  }
  ReceiptDetails: any = [];
  Linesarray: any = [];
  PrintTypeBasedOnConfig: any;

  getReceiptPrint(arg) {
    if (!arg) {
      swal("Warning!", 'Please enter ReceiptNo', "warning");
      $('#exampleModal').modal('hide');
    }
    else {
      // this.Service.getReceiptValuesForPrint(arg).subscribe(
      //   Response => {
      //     this.ReceiptDetails = Response;
      //     this.Linesarray = this.ReceiptDetails.lstOfPayment;
      //     this.getShowroomDetails();
      //     this.getbillDetails(this.ReceiptDetails.BillNo, this.ReceiptDetails.FinYear);
      //     $('#exampleModal').modal('show');

      //   }
      // )
      this.Service.getReceiptValuesForPrint(arg).subscribe(
        response => {
          this.PrintTypeBasedOnConfig = response;
          if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
            this.ReceiptDetails = atob(this.PrintTypeBasedOnConfig.Data);
            $('#DisplayCreditReceiptData').html(this.ReceiptDetails);
            $('#CreditReceiptModal').modal('show');
          }
          else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
            this.ReceiptDetails = this.PrintTypeBasedOnConfig.Data;
            $('#CreditReceiptModal').modal('show');
          }
        }, (err) => {
          if (err.status === 400) {
            const validationError = err.error;
            swal("Warning!", validationError.description, "warning");
          }
        }
      );
    }
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