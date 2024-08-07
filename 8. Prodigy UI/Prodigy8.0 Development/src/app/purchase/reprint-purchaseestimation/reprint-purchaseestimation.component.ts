import { Component, OnInit } from '@angular/core';
import { estimationService } from '../../estimation/estimation.service';
import { Router } from '@angular/router';
import { PurchaseService } from '../../purchase/purchase.service';
import { ToastrService } from 'ngx-toastr';
import { MasterService } from '../../core/common/master.service';
import swal from 'sweetalert';
declare var jquery: any;
declare var $: any;

@Component({
  selector: 'app-reprint-purchaseestimation',
  templateUrl: './reprint-purchaseestimation.component.html',
  styleUrls: ['./reprint-purchaseestimation.component.css']
})
export class ReprintPurchaseestimationComponent implements OnInit {
  constructor(private _estmationService: estimationService, private _purchaseService: PurchaseService,
    private _router: Router, private toastr: ToastrService, private _masterService: MasterService) { }

  estPurchaseDetails: any = [];
  EnablePurchaseReprintHeaders: boolean = false;
  ShowHide: boolean = true;
  PurchaseDetails: any = [];
  getPurchaseTotForPrint: any;
  getPurchaseDataTotForPrint: any;
  heading: string;
  ReprintData: any;

  ngOnInit() {
    if (this._router.url == "/purchase/reprintpurchaseestimation") {
      this.ShowHide = true;
    }
    else {
      this._purchaseService.castReprintPurchaseData.subscribe(
        response => {
          this.ReprintData = response;
          if (this.isEmptyObject(this.ReprintData) == false && this.isEmptyObject(this.ReprintData) != null) {
            // this.getPurchaseEstDetails(this.ReprintData.EstNo);
            this.getPurEstPlainText(this.ReprintData.EstNo);
            this._purchaseService.SendReprintPurchaseData(null);
          }
          this.ShowHide = false;
        })
    }
  }

  isEmptyObject(obj) {
    return (obj && (Object.keys(obj).length === 0));
  }

  getPurchaseEstimationDetails(arg) {
    if (arg == "") {
      swal("Warning!", 'Please enter the estimation', "warning");
    }
    else {
      // this.getPurchaseEstDetails(arg);
      this.getPurEstPlainText(arg);
    }
  }

  PurEstNum: number;

  // getPurchaseEstDetails(arg) {
  //   this._purchaseService.getPurchaseDetailsfromAPI(arg).subscribe(
  //     response => {
  //       this.estPurchaseDetails = response;
  //       this.PurEstNum = arg;
  //       if (this.estPurchaseDetails != null && this.estPurchaseDetails.lstPurchaseEstDetailsVM.length > 0) {
  //         this.EnablePurchaseReprintHeaders = true;
  //         this.PurchaseDetails = this.estPurchaseDetails.lstPurchaseEstDetailsVM;
  //         this.heading = "Purchase Estimation";
  //         this._estmationService.getPurchaseForPrintTotal(arg).subscribe(
  //           response => {
  //             this.getPurchaseDataTotForPrint = response;
  //             this.getPurchaseTotForPrint = this.getPurchaseDataTotForPrint.lstPurchaseEstDetailsVM;
  //             $('#PurchaseModal').modal('show');
  //           }
  //         )
  //       }
  //       else
  //       {
  //         this.toastr.warning('Purchase Estimation No does not exists.', 'Alert!')          
  //       }
  //     }
  //   )
  // }

  // for printing the form

  printPurchase() {
    let printContents, popupWin;
    printContents = document.getElementById('print-sectionPurchase').innerHTML;
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


  // Added for Plain Text printing related to Order Receipt

  purEstPlainTextDetails: any = [];

  closePurEstTab() {
    // if (this._router.url === "/purchase") {
    this.getPurEstPlainText(this.PurEstNum);
    // }
    // else
    // {
    //   $('#PurEstPlainTextTab').modal('hide');              
    // }
  }


  PrintTypeBasedOnConfig: any;

  getPurEstPlainText(arg) {
    this._purchaseService.PrintPurEstDotMatrix(arg).subscribe(
      response => {
        this.PrintTypeBasedOnConfig = response;
        if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
          this.purEstPlainTextDetails = atob(this.PrintTypeBasedOnConfig.Data);
          $('#DisplayData').html(this.purEstPlainTextDetails);
          $('#PurEstPlainTextTab').modal('show');
        }
        else {
          this.purEstPlainTextDetails = this.PrintTypeBasedOnConfig.Data;
          $('#PurEstPlainTextTab').modal('show');
        }
      }
    )
  }

  printPurEst() {

    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.purEstPlainTextDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      this.printPurEstimationHtml();
    }
  }


  printPurEstimationHtml() {
    let printContentsPurPrint, popupWin;
    printContentsPurPrint = document.getElementById('DisplayData').innerHTML;
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

    ${printContentsPurPrint}</body>
      </html>`
    );
    popupWin.document.close();
  }



  // Ends Here
}