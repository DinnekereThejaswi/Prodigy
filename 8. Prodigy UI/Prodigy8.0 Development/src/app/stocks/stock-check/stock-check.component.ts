import { Component, OnInit, ɵConsole } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { StockCheckService } from './stock-check.service';
import { StocksService } from './../stocks.service';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
declare var $: any;
@Component({
  selector: 'app-stock-check',
  templateUrl: './stock-check.component.html',
  styleUrls: ['./stock-check.component.css']
})
export class StockCheckComponent implements OnInit {
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;
  StockCheckForm: FormGroup;
  existingForm: FormGroup;
  ScannedForm: FormGroup;
  RemainingForm: FormGroup;
  constructor(private stockcheckService: StockCheckService, private fb: FormBuilder,
    private stocksService: StocksService, private appConfigService: AppConfigService) {
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.password = this.appConfigService.Pwd;
    this.getCB();
  }
  StockcheckSummaryHeader: any = {
    Qty: null,
    Grwt: null,
    Nwt: null,
    Dcts: null,
    companyCode: this.ccode,
    BranchCode: this.bcode,
  }
  ScannedSummaryHeader: any = {
    Qty: null,
    Grwt: null,
    Nwt: null,
    Dcts: null,
  }
  RemainingSummaryHeader: any = {
    Qty: null,
    Grwt: null,
    Nwt: null,
    Dcts: null,
    // bcode:null,
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }
  ngOnInit() {
    this.getGs();
    this.getApplicationDate();
    this.getShowroomDetails();
    this.StockcheckSummaryHeader.bcode = this.bcode;
    this.StockCheckForm = this.fb.group({
      companyCode: null,
      BranchCode: null,
      GS: null,
      CounterCode: null,
      Item: null,
    });
    this.existingForm = this.fb.group({
      Qty: null,
      Grwt: null,
      Nwt: null,
      Dcts: null,
    });
    this.ScannedForm = this.fb.group({
      Qty: null,
      Grwt: null,
      Nwt: null,
      Dcts: null,
    });
    this.RemainingForm = this.fb.group({
      Qty: null,
      Grwt: null,
      Nwt: null,
      Dcts: null,
    });
  }
  CounterList: any = [];
  getCounter(arg) {
    this.stocksService.getCounterList(arg).subscribe(
      Response => {
        this.CounterList = Response;
      }
    )
  }
  GSList: any = [];
  getGs() {
    this.stockcheckService.GSList().subscribe(
      Response => {
        this.GSList = Response;
      }
    )
  }

  itempush: any = {
    ItemCode: "ALL",
    ItemName: "ALL"
  }


  ItemList: any = [];
  getItem(arg) {
    this.stockcheckService.getItem(arg.value.CounterCode, arg.value.GS).subscribe(
      Response => {
        this.ItemList = Response;
        this.ItemList.push(this.itempush);
      }
    )
  }
  RemainingTag: any = [];
  getRemainingtags(arg) {
    if (this.StockCheckForm.value.GS == null) {
      alert("Please select GS");
    }
    else if (this.StockCheckForm.value.CounterCode == null) {
      alert("Plese select Counter Code");
    }
    else if (this.StockCheckForm.value.Item == null) {
      alert("Please select Item");
    }
    else {
      this.StockcheckSummaryHeader.bcode = this.bcode;
      $('#RemainingTag').modal('show');
      this.stockcheckService.getRemainingTags(arg.value.GS, arg.value.CounterCode, arg.value.Item).subscribe(
        Response => {
          this.RemainingTag = Response;
        }
      )
    }

  }
  reprintHeaders = {
    applicationDate: null,
    appViewDate: null,
  }
  applicationDate: any;
  appViewDate: any;
  getApplicationDate() {
    this.stocksService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
        this.appViewDate = appDate['appViewDate'];
        this.reprintHeaders.applicationDate = this.applicationDate;
        this.reprintHeaders.appViewDate = this.appViewDate;
      }
    )
  }
  ShowroomList: any = [];
  getShowroomDetails() {
    this.stocksService.getShowroom().subscribe(
      response => {
        this.ShowroomList = response;
        // this.calculateFooter();
      }
    );
  }
  ScannedTag: any = [];
  getScannedtags(arg) {
    if (this.StockCheckForm.value.GS == null) {
      alert("Please select GS");
    }
    else if (this.StockCheckForm.value.CounterCode == null) {
      alert("Plese select Counter Code");
    }
    else if (this.StockCheckForm.value.Item == null) {
      alert("Please select Item");
    }
    else {
      this.StockcheckSummaryHeader.bcode = this.bcode;
      $('#ScannedTag').modal('show');
      this.stockcheckService.getScannedTags(arg.value.GS, arg.value.CounterCode, arg.value.Item).subscribe(
        Response => {
          this.ScannedTag = Response;
        }
      )
    }
  }
  Tagsummary: any;
  BarcodedTags: any;
  GetTagSummary(arg) {

    this.stockcheckService.getTagSummary(arg.value.GS, arg.value.CounterCode, arg.value.Item).subscribe(
      Response => {
        this.existingForm.reset();
        this.ScannedForm.reset();
        this.RemainingForm.reset();
        this.Tagsummary = Response;
        // alert(this.Tagsummary.BarcodedTags.Qty + 'remaining'+ this.RemainingSummaryHeader.Qty );

        this.StockcheckSummaryHeader.Qty = this.Tagsummary.BarcodedTags.Qty;
        this.StockcheckSummaryHeader.Grwt = this.Tagsummary.BarcodedTags.GrossWt;
        this.StockcheckSummaryHeader.Nwt = this.Tagsummary.BarcodedTags.NetWt;
        this.StockcheckSummaryHeader.Dcts = this.Tagsummary.BarcodedTags.Dcts;

        this.RemainingSummaryHeader.Qty = this.Tagsummary.RemainingTags.Qty;
        this.RemainingSummaryHeader.Grwt = this.Tagsummary.RemainingTags.GrossWt;
        this.RemainingSummaryHeader.Nwt = this.Tagsummary.RemainingTags.NetWt;
        this.RemainingSummaryHeader.Dcts = this.Tagsummary.RemainingTags.Dcts;

        this.ScannedSummaryHeader.Qty = this.Tagsummary.ScannedTags.Qty;
        this.ScannedSummaryHeader.Grwt = this.Tagsummary.ScannedTags.GrossWt;
        this.ScannedSummaryHeader.Nwt = this.Tagsummary.ScannedTags.NetWt;
        this.ScannedSummaryHeader.Dcts = this.Tagsummary.ScannedTags.Dcts;
      }
    )
  }
  closeModal() {
    $('#RemainingTag').modal('hide');
    $('#ScannedTag').modal('hide');

  }
  stockclear: any;
  ClearStock(arg) {
    var ans = confirm('Do you want to Clear the scanned stock');
    if (ans) {
      this.stockcheckService.clearStock(arg.value.GS, arg.value.CounterCode, arg.value.Item).subscribe(
        data => {
          this.stockclear = data;
          this.GetTagSummary(arg);

        }
      )
    }
  }
  printScanned() {
    let printContents, popupWin;
    printContents = document.getElementById('print-sectionScanned').innerHTML;
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

  Qty(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      if (d.Qty != null && d.Qty != 0) {
        total += Number(<number>d.Qty);
      }
    });
    return total;
  }

  GrWt(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      if (d.GrossWt != null && d.GrossWt != 0) {
        total += Number(<number>d.GrossWt);
      }
    });
    return total;
  }

  NtWt(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      if (d.Netwt != null && d.Netwt != 0) {
        total += Number(<number>d.Netwt);
      }
    });
    return total;
  }
}