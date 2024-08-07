import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { estimationService } from '../estimation.service';
import { Router } from '@angular/router';
import { MasterService } from '../../core/common/master.service';
import { PurchaseService } from '../../purchase/purchase.service';
import { ToastrService } from 'ngx-toastr';
import { ComponentCanDeactivate } from '../../appconfirmation-guard';
import swal from 'sweetalert';
import { SalesreturnService } from '../../salesreturn/salesreturn.service';
declare var $: any;

@Component({
  selector: 'app-reprint-salesestimation',
  templateUrl: './reprint-salesestimation.component.html',
  styleUrls: ['./reprint-salesestimation.component.css']
})
export class ReprintSalesestimationComponent implements OnInit, ComponentCanDeactivate {

  constructor(private _estmationService: estimationService, private _purchaseService: PurchaseService,
    private _router: Router, private toastr: ToastrService, private _masterService: MasterService,
    private _SalesreturnService: SalesreturnService,) { }
  ContinuousEstNo: Number;
  EstimationType: string = "";
  estSalesDetails: any = [];
  estPurchaseDetails: any = [];
  estOGDetails: any = [];
  estSRDetails: any = [];
  // @ViewChild("estNo") estNo: ElementRef;
  @ViewChild("estNo", { static: true }) estNo: ElementRef;
  leavePage: boolean = false;

  EnableSalesReprintHeaders: boolean = false;
  EnablePurchaseReprintHeaders: boolean = false;
  EnableOGReprintHeaders: boolean = false;
  EnableSRReprintHeaders: boolean = false;
  ShowHide: boolean = true;
  SalesEstPrintTotals: any = [];
  isChecked: boolean = false;
  SalesDetails: any = [];
  PurchaseDetails: any = [];
  OGDetails: any = [];
  SRDetails: any = [];
  getSalesTotForPrint: any;
  getPurchaseTotForPrint: any;
  getPurchaseDataTotForPrint: any;
  getOGTotForPrint: any;
  getSRTotForPrint: any;
  heading: string;
  @Input() EstNofromParentComp: any = [];
  @Input() EstType: string = "";
  ReprintData: any;
  ReprintSRData: any = [];

  EstNo: number = 0;

  ngOnInit() {
    if (this._router.url == "/estimation/reprintsalesestimation") {
      this.ShowHide = true;
      this.EstimationType = "S";
    }
    else {
      this._estmationService.castReprint.subscribe(
        response => {
          this.ReprintData = response;
          if (this.isEmptyObject(this.ReprintData) == false && this.isEmptyObject(this.ReprintData) != null) {
            this.EstimationType = this.ReprintData.EstType;
            this.isChecked = false;
            this.EstNo = this.ReprintData.EstNo;
            this.getEstBasedonType(this.ReprintData.EstNo);
            // this._estmationService.SendReprintData(null);
          }
          this.ShowHide = false;
        })

      this._estmationService.castReprintSR.subscribe(
        response => {
          this.ReprintSRData = response;
          if (this.isEmptyObject(this.ReprintSRData) == false && this.isEmptyObject(this.ReprintSRData) != null) {
            // this.EstimationType = this.ReprintData.EstType;
            // this.EstNo=this.ReprintData.EstNo;
            // this.getEstBasedonType(this.ReprintData.EstNo);
            this.isChecked = false;
            for (let i = 0; i < this.ReprintSRData.length; i++) {
              this.getSREstPlainText(this.ReprintSRData[i].RefBillNo);
            }
          }
          this.ShowHide = false;
        })
    }
  }

  isEmptyObject(obj) {
    return (obj && (Object.keys(obj).length === 0));
  }



  getEstimationDetails(arg) {
    this.ContinuousEstNo = arg;
    if (this.EstimationType == "") {
      swal("Warning!", 'Please select the estimation', "warning");
    }
    else if (arg == "") {
      swal("Warning!", 'Please enter the estimation', "warning");
    }
    else {
      this.getEstBasedonType(this.ContinuousEstNo);
    }
  }

  GetType(arg) {
    this.EstimationType = arg;
  }

  confirmBeforeLeave(): boolean {
    if (this.leavePage == true) {
      var ans = (confirm("You have unsaved changes! If you leave, your changes will be lost."))
      if (ans) {
        this.leavePage = false;
        return true;
      }
      else {
        return false;
      }
    }
    else {
      return true;
    }
  }


  FieldsChange(values: any) {
    this.isChecked = values.currentTarget.checked;
  }



  StoneDetails: any = [];

  getEstBasedonType(arg) {
    if (this.EstimationType == "S") {
      // this._estmationService.getSalesEstimationForPrint(arg).subscribe(
      //   response => {
      //     this.EstNo=arg;
      //     this.leavePage=true;
      //     this.StoneDetails=[];
      //     this.estSalesDetails = response;
      //     if (this.estSalesDetails != null && this.estSalesDetails.salesEstimatonVM.length > 0) {
      //       this.EnableSalesReprintHeaders = true;
      //       this.SalesDetails = this.estSalesDetails.salesEstimatonVM;
      //       this.heading = "Sales Estimation";
      //       for(let i=0; i<this.estSalesDetails.salesEstimatonVM.length;i++){
      //         for(let j=0; j<this.estSalesDetails.salesEstimatonVM[i].salesEstStoneVM.length;j++){
      //           this.StoneDetails.push(this.estSalesDetails.salesEstimatonVM[i].salesEstStoneVM[j]);
      //         }
      //       }
      //       this._estmationService.getSalesForPrintTotal(arg).subscribe(
      //         response => {
      //           this.getSalesTotForPrint = response;
      //           this.SalesEstPrintTotals=this.getSalesTotForPrint.lstOfSalesEstPrintTotals;
      //           $('#SalesModal').modal('show');
      //         }
      //       )
      //       this.estNo.nativeElement.value="";
      //     }
      //   }
      // )


      this.getSalesEstimationPlainText(arg);
      this.estNo.nativeElement.value = "";
    }
    else if (this.EstimationType == "P") {
      // this._purchaseService.getPurchaseDetailsfromAPI(arg).subscribe(
      //   response => {
      //     this.estPurchaseDetails = response;
      //     if (this.estPurchaseDetails != null && this.estPurchaseDetails.lstPurchaseEstDetailsVM.length > 0) {
      //       this.EnablePurchaseReprintHeaders = true;
      //       this.PurchaseDetails = this.estPurchaseDetails.lstPurchaseEstDetailsVM;
      //       this.heading = "Purchase Estimation";
      //       this._estmationService.getPurchaseForPrintTotal(arg).subscribe(
      //         response => {
      //           this.getPurchaseDataTotForPrint = response;
      //           this.getPurchaseTotForPrint = this.getPurchaseDataTotForPrint.lstPurchaseEstDetailsVM;
      //           $('#PurchaseModal').modal('show');
      //         }
      //       )
      //     }
      //   }
      // )
      this.getPurchaseEstPlainText(arg);
    }
    else if (this.EstimationType == "OG") {
      this._estmationService.getOGEstimationForPrint(arg).subscribe(
        response => {
          this.estOGDetails = response;
          if (this.estOGDetails != null && this.estOGDetails.lstPurchaseEstDetailsVM.length > 0) {
            this.EnableOGReprintHeaders = true;
            this.heading = "Purchase Estimation";
            this.OGDetails = this.estOGDetails.lstPurchaseEstDetailsVM;
            this._estmationService.getOGForPrintTotal(arg).subscribe(
              response => {
                this.getOGTotForPrint = response;
                this.getOGTotForPrint = this.getOGTotForPrint.lstPurchaseEstDetailsVM;
                $('#OGModal').modal('show');
              }
            )
          }
        }
      )
      //this.getPurchaseEstPlainText(arg);
    }
    else if (this.EstimationType == "SR") {
      this._estmationService.getSRForPrint(arg).subscribe(
        response => {
          this.estSRDetails = response;
          if (this.estSRDetails != null && this.estSRDetails.lstOfSalesReturnDetails.length > 0) {
            this.EnableSRReprintHeaders = true;
            this.SRDetails = this.estSRDetails.lstOfSalesReturnDetails;
            this.heading = "Sales Return Estimation";
            this._estmationService.getSRForPrintTotal(arg).subscribe(
              response => {
                this.getSRTotForPrint = response;
                this.getSRTotForPrint = this.getSRTotForPrint.lstOfSalesReturnDetails;
                $('#SRModal').modal('show');
              }
            )
          }
        }
      )
      //this.getSREstPlainText(arg);
    }
  }

  // for printing the form
  printSales() {
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
  printSR() {
    let printContents, popupWin;
    printContents = document.getElementById('print-sectionSR').innerHTML;
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
  printOG() {
    let printContents, popupWin;
    printContents = document.getElementById('print-sectionOG').innerHTML;
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
  purEstPlainTextDetails: any = [];
  CloseSalesPrint() {
    $('#SalesEstimationPlainTextTab').modal('hide');
    this.estNo.nativeElement.value = "";
    
    //01/03/2022
    //commented below code bcz all of the estimations print are came at single page only so
    //closing and calling individal print by passing estimation no not doing futher
    // if (this.PrintTypeBasedOnConfig.ContinueNextPrint == true) {
    //   this._purchaseService.PrintPurEstDotMatrix(this.ContinuousEstNo).subscribe(
    //     response => {
    //       this.purEstPlainTextDetails = response;
    //       if (this.purEstPlainTextDetails.Data != "") {
    //         if (this.purEstPlainTextDetails.PrintType == "HTML") {
    //           this.purEstPlainTextDetails = atob(this.purEstPlainTextDetails.Data);
    //           $('#DisplayPurchaseData').html(this.purEstPlainTextDetails);
    //         }
    //         else if (this.purEstPlainTextDetails.PrintType == "RAW") {
    //           this.purEstPlainTextDetails = this.purEstPlainTextDetails.Data;
    //         }
    //         $('#SalesEstimationPlainTextTab').modal('hide');
    //         $('#PurEstPlainTextTab').modal('show');
    //         $('#OGPlainTextTab').modal('hide');
    //         $('#SREstPlainTextTab').modal('hide');
    //       }
    //       else {
    //         $('#SalesEstimationPlainTextTab').modal('hide');
    //         $('#PurEstPlainTextTab').modal('hide');
    //         this.ClosePurchasePrint();
    //       }
    //     }
    //   )
    // }
    // else {
    //   $('#SalesEstimationPlainTextTab').modal('hide');
    // }
  }

  SelectedSalesReturnList: any = [];

  ClosePurchasePrint() {
    this._SalesreturnService.getSalesReturnAttachedList(this.ContinuousEstNo).subscribe(
      response => {
        this.SelectedSalesReturnList = response;
        if (this.SelectedSalesReturnList.length > 0) {
          for (var i = 0; i < this.SelectedSalesReturnList.length; i++) {
            this.getSREstPlainText(this.SelectedSalesReturnList[i]["SrEstNo"]);
          }
        }
        else {
          $('#PurEstPlainTextTab').modal('hide');
          $('#SalesEstimationPlainTextTab').modal('hide');
          $('#SREstPlainTextTab').modal('show');
          $('#OGPlainTextTab').modal('hide');
          this.CloseSRPrint();
        }
      }
    )
  }
  SelectedOldGoldList: any = [];
  SREstimationPlainTextDetails: any = [];
  getSREstPlainText(arg) {
    this._estmationService.getSalesReturnEstDotMatrixPrint(arg).subscribe(
      response => {
        this.SREstimationPlainTextDetails = response;
        $('#PurEstPlainTextTab').modal('hide');
        $('#SalesEstimationPlainTextTab').modal('hide');
        $('#SREstPlainTextTab').modal('show');
      }
    )
  }
  CloseSRPrint() {
    this._purchaseService.getOldGoldttachedList(this.ContinuousEstNo).subscribe(
      response => {
        this.SelectedOldGoldList = response;
        if (this.SelectedOldGoldList.length > 0) {
          for (var i = 0; i < this.SelectedOldGoldList.length; i++) {
            this.getOGPlainText(this.ContinuousEstNo);
          }
        }
        else {
          $('#PurEstPlainTextTab').modal('hide');
          $('#SalesEstimationPlainTextTab').modal('hide');
          $('#SREstPlainTextTab').modal('hide');
          $('#OGPlainTextTab').modal('hide');
        }
      }
    )
  }
  OGPlainTextDetails: any = [];
  getOGPlainText(arg) {
    this._purchaseService.PrintPurEstDotMatrix(this.ContinuousEstNo).subscribe(
      response => {
        this.OGPlainTextDetails = response;
        if (this.OGPlainTextDetails.PrintType == "RAW" && this.OGPlainTextDetails.Data != "") {
          this.OGPlainTextDetails = this.OGPlainTextDetails.Data;
          $('#SalesEstimationPlainTextTab').modal('hide');
          $('#PurEstPlainTextTab').modal('hide');
          $('#SREstPlainTextTab').modal('hide');
          $('#OGPlainTextTab').modal('show');
        }
        else if (this.OGPlainTextDetails.PrintType == "HTML" && this.OGPlainTextDetails.Data != "") {
          this.OGPlainTextDetails = atob(this.OGPlainTextDetails.Data);
          $('#DisplayOGData').html(this.OGPlainTextDetails);
          $('#SalesEstimationPlainTextTab').modal('hide');
          $('#PurEstPlainTextTab').modal('hide');
          $('#SREstPlainTextTab').modal('hide');
          $('#OGPlainTextTab').modal('show');
        }
        else {
          $('#SalesEstimationPlainTextTab').modal('hide');
          $('#PurEstPlainTextTab').modal('hide');
          $('#SREstPlainTextTab').modal('hide');
          $('#OGPlainTextTab').modal('hide');
        }
      }
    )
  }

  printPurEst() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.purEstPlainTextDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      this.printPurchaseEstimationHtml();
    }
  }

  printPurchaseEstimationHtml() {
    let printContentsPurchasePrint, popupWin;
    printContentsPurchasePrint = document.getElementById('DisplayPurchaseData').innerHTML;
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

    ${printContentsPurchasePrint}</body>
      </html>`
    );
    popupWin.document.close();
  }

  salesEstimationPlainTextDetails: any = [];
  PrintDetails: any = [];

  PrintTypeBasedOnConfig: any;

  getSalesEstimationPlainText(arg) {
    if (this.isChecked == false) {
      this._estmationService.getSalesEstimationPlainText(arg).subscribe(
        response => {
          this.PrintTypeBasedOnConfig = response;
          if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
            this.salesEstimationPlainTextDetails = this.PrintTypeBasedOnConfig.Data;
            $('#SalesEstimationPlainTextTab').modal('show');
            $('#ReprintSalesBillingModal').modal('hide');
            $('#PurchaseEstimationPlainTextTab').modal('hide');
            $('#SREstPlainTextTab').modal('hide');
          }
          else if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
            this.salesEstimationPlainTextDetails = atob(this.PrintTypeBasedOnConfig.Data);
            $('#SalesEstimationPlainTextTab').modal('show');
            $('#ReprintSalesBillingModal').modal('hide');
            $('#PurchaseEstimationPlainTextTab').modal('hide');
            $('#SREstPlainTextTab').modal('hide');
            $('#DisplaySalesData').html(this.salesEstimationPlainTextDetails);
          }
        })
    }
    else {
      this._estmationService.PrintBillByEstimation(arg).subscribe(
        response => {
          this.PrintDetails = response;
          this.PrintDetails = atob(this.PrintDetails);
          this._estmationService.PrintBillByEstimation(arg).subscribe(
            response => {
              $('#ReprintSalesBillingModal').modal('show');
              $('#SalesEstimationPlainTextTab').modal('hide');
              $('#PurchaseEstimationPlainTextTab').modal('hide');
              $('#SREstPlainTextTab').modal('hide');
              $('#DisplayData').html(this.PrintDetails);
            }
          );
        },
        (err) => {
          if (err.status === 404) {
            const validationError = err.error;
            swal("Warning1", validationError, "warning");
          }
          else if (err.status === 400) {
            const validationError = err.error;
            swal("Warning!", validationError, "warning");
          }
          else {
            swal("Warning!", "The Sales Etimtaion not attached to bill", "warning");
          }

          // this.clear();
        }
      )
    }
  }

  printSalesEstimation() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.salesEstimationPlainTextDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      this.printSalesEstimationHtml();
    }
  }

  purchaseEstimationPlainTextDetails: any = [];

  getPurchaseEstPlainText(arg) {
    this._purchaseService.PrintPurEstDotMatrix(arg).subscribe(
      response => {
        this.purchaseEstimationPlainTextDetails = response;
        $('#PurchaseEstimationPlainTextTab').modal('show');
        $('#SalesEstimationPlainTextTab').modal('hide');
        $('#SREstPlainTextTab').modal('hide');
      }
    )
  }

  printPurchaseEstimationPlainText() {
    this._masterService.printPlainText(this.purchaseEstimationPlainTextDetails);
  }



  printSREstimationPlainText() {
    this._masterService.printPlainText(this.SREstimationPlainTextDetails);
  }


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


  printSalesEstimationHtml() {
    let printContentsSalesPrint, popupWin;
    printContentsSalesPrint = document.getElementById('DisplaySalesData').innerHTML;
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

    ${printContentsSalesPrint}</body>
      </html>`
    );
    popupWin.document.close();
  }
}