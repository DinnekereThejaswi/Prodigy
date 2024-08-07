import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { BranchreceiptsService } from '../branchreceipts.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import * as CryptoJS from 'crypto-js';
import { MasterService } from '../../core/common/master.service';

import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';
import { Router } from '@angular/router';
declare var $: any;
@Component({
  selector: 'app-barcodereceipts',
  templateUrl: './barcodereceipts.component.html',
  styleUrls: ['./barcodereceipts.component.css']
})
export class BarcodereceiptsComponent implements OnInit {

  @ViewChild('Tagno', { static: true }) Tagno: ElementRef;

  datePickerConfig: Partial<BsDatepickerConfig>;
  routerUrl: string = "";
  readonly :boolean =false;
  Disabled:string="false";
  EnableImportedBarcodes: boolean = true;
  EnableStoneDmddtls: boolean = true;
  EnableScannedBarcodes: boolean = true;
  today = new Date();
  applicationDate: any
  barcodereceiptHeaderForm: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  barcodereceiptsHeaderFormModel = {
    receiptfrom: null,
    issueno: null,
    receivedBY: null,
    issueDate: null,

  }

  constructor(private _branchreceiptsService: BranchreceiptsService, private fb: FormBuilder,
    private _appConfigService: AppConfigService, private _router: Router, private _masterService: MasterService) {
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
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this.readonly=false;
    this.routerUrl = this._router.url;
    this.geReceiptFrom();
    this.applicationDate = this.today;
    this.barcodereceiptHeaderForm = this.fb.group({
      frmCtrl_ReceiptFrom: null,
      frmCtrl_IssueNo: null,
      frmCtrl_ReceiptDate: null,
      frmCtrl_ReceivedBy: '',

    });
  }
  ToggleScannedBarcodes() {
    this.EnableScannedBarcodes = !this.EnableScannedBarcodes;
  }
  ToggleImportedBarcodes() {
    this.EnableImportedBarcodes = !this.EnableImportedBarcodes;
  }
  ToggleFittedStonesDetails() {
    this.EnableStoneDmddtls = !this.EnableStoneDmddtls;
  }
  ReceiptFrom: any = [];
  geReceiptFrom() {
    this._branchreceiptsService.GetReceiptFrom().subscribe(
      Response => {
        this.ReceiptFrom = Response;
      }
    )
  }

  ItemDetails: any = [];
  GetReceiptDetails() {
    if (this.barcodereceiptsHeaderFormModel.receiptfrom == null || this.barcodereceiptsHeaderFormModel.receiptfrom == "") {
      swal("Warning!", "Please Select Branch", "warning");
    }
    else if (this.barcodereceiptsHeaderFormModel.issueno == null || this.barcodereceiptsHeaderFormModel.issueno == "") {
      swal("Warning!", "Please enter Issue No", "warning");
    }
    else {
      this._branchreceiptsService.GetIssueDetails(this.barcodereceiptsHeaderFormModel.receiptfrom, this.barcodereceiptsHeaderFormModel.issueno).subscribe(
        Response => {
          this.ItemDetails = Response;
          this.readonly=true;
          this.barcodereceiptHeaderForm.get('frmCtrl_ReceiptFrom').disable();
          this.EnableImportedBarcodes = false;
        },
        (err) => {
          if (err.ErrorStatusCode === 400) {
            const validationError = err.error;
            swal("!Warning", validationError, "warning");
          }
          else {
            this.ItemDetails = null;
            this.barcodereceiptHeaderForm.reset();
          }
          // this.clear();
        }
      )
    }
    this.ItemDetails = [];

  }

  getDetails(arg) {
    if (this.ItemDetails.ScannedBarcodes.find(ele => ele.BarcodeNo === arg)) {
      swal("Warning!", "Item already scanned", "warning");
    }
    else {
      let filterData = this.ItemDetails.ReceiptDetails.find(ele => ele.BarcodeNo === arg);
      this.finalBarcodeDetails.push(filterData);
      this.totalItems(this.finalBarcodeDetails);
      this.ItemDetails.ScannedBarcodes.push(filterData);
      this.Tagno.nativeElement.value = '';
      this.getStoneDetails();
    }
  }
  totalItems(arg) {
    let total = 0;
    arg.forEach((d) => {
      total += d.Qty;
    });
    this.grossWT(arg);
    return total;

  }

  grossWT(arg) {
    let total = 0;
    arg.forEach((d) => {
      total += d.GrossWt;
    });
    this.stoneWt(arg);
    return total;
  }

  stoneWt(arg) {
    let total = 0;
    arg.forEach((d) => {
      total += d.StoneWt;
    });
    this.netWT(arg);
    return total;
  }

  netWT(arg) {
    let total = 0;
    arg.forEach((d) => {
      total += d.NetWt;
    });
    return total;
  }

  dcts(arg) {
    let total = 0;
    arg.forEach((d) => {
      total += d.Dcts;
    });
    return total;
  }

  amount(arg) {
    let total = 0;
    arg.forEach((d) => {
      total += d.Amount;
    });
    return total;
  }
  finalBarcodeDetails: any = [];
  StoneDetails: any = [];
  getStoneDetails() {
    this.StoneDetails = [];
    if (this.ItemDetails.length == 0) {
      this.StoneDetails = [];
    }
    else {
      for (let i = 0; i < this.ItemDetails.ScannedBarcodes.length; i++) {
        if (this.ItemDetails.ScannedBarcodes[i].BarcodeReceiptStoneDetails.length > 0) {
          for (let j = 0; j < this.ItemDetails.ScannedBarcodes[i].BarcodeReceiptStoneDetails.length; j++) {
            this.StoneDetails.push(this.ItemDetails.ScannedBarcodes[i].BarcodeReceiptStoneDetails[j]);
          }
        }
      }
    }
  }
  outputBarcodeIssue: any = [];
  NTreceiptPrint: any = [];
  submit() {
    if (this.ItemDetails.ScannedBarcodes == null) {
      swal("!Warning", "Please Scan the items", "Warning");
    }
    else {
      this._branchreceiptsService.post(this.ItemDetails).subscribe(
        response => {
          this.outputBarcodeIssue = response;
          swal("Saved!", this.outputBarcodeIssue.Message, "success");
          this.finalBarcodeDetails = [];
          this.StoneDetails = [];
          this.ItemDetails = [];
          this.barcodereceiptHeaderForm.reset();
          this._branchreceiptsService.getBarcodeReceiptPrint(this.outputBarcodeIssue.DocumentNo).subscribe(
            response => {
              this.PrintTypeBasedOnConfig = response;
              if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
                $('#NTReceiptsTab').modal('show');
                this.NTreceiptPrint = atob(this.PrintTypeBasedOnConfig.Data);
                $('#DisplayntreceiptData').html(this.NTreceiptPrint);
              }
              else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
                $('#NTReceiptsTab').modal('show');
                this.NTreceiptPrint = this.PrintTypeBasedOnConfig.Data;
              }
            }
          )
        }
      )

    }
  }

  printNtreceipt() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.NTreceiptPrint);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printntReceiptData, popupWin;
      printntReceiptData = document.getElementById('DisplayntreceiptData').innerHTML;
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
    ${printntReceiptData}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }


  printPendingtData: any = [];
  PrintTypeBasedOnConfig: any;
  printPending() {
    this.printPendingtData = [];
    this._branchreceiptsService.getPendingPrint(this.ItemDetails).subscribe(
      response => {
        this.printPendingtData = response;
        if (this.printSacannedData.length > 0) {
          $('#pendingTab').modal('show');
        }
      }
    )

  }
  printSacannedData: any = [];
  printScanned() {
    this.printSacannedData = [];
    this._branchreceiptsService.getScannedPrint(this.ItemDetails).subscribe(
      response => {
        this.printSacannedData = response;
        if (this.printSacannedData.length > 0) {
          $('#scannedTab').modal('show');
        }
      }
    )
  }
  printImportData: any = []
  printImported() {
    this.printImportData = [];
    this._branchreceiptsService.getImportedPrint(this.ItemDetails).subscribe(
      response => {
        this.printImportData = response;
        if (this.printImportData.length > 0) {
          $('#importTab').modal('show');
        }
      }
    )

  }
  cancel() {
    this.readonly=false;
    this.barcodereceiptHeaderForm.get('frmCtrl_ReceiptFrom').enable();
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
      () => {
        this._router.navigate(['/branchreceipts/barcodereceipts']);
      }
    )
  }

  printPendingData() {
    if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let pending, popupWin;
      pending = document.getElementById('DisplayntPendingData').innerHTML;
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
      ${pending}</body>
        </html>`
      );
      popupWin.document.close();
    }
  }
  printScannedData() {
    if (this.printSacannedData != null) {
      let printntIssueData, popupWin;
      printntIssueData = document.getElementById('DisplaynScannedData').innerHTML;
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
      ${printntIssueData}</body>
        </html>`
      );
      popupWin.document.close();
    }
  }
  printImportedData() {
    if (this.printImportData !== null) {
      let printntIssueData, popupWin;
      printntIssueData = document.getElementById('DisplayntprintImportedData').innerHTML;
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
      ${printntIssueData}</body>
        </html>`
      );
      popupWin.document.close();
    }
  }

}
