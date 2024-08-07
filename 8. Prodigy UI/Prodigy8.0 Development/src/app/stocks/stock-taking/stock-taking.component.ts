import { Component, OnInit, ViewChild, ElementRef, AfterContentChecked } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { StockTakingService } from './stock-taking.service';
import { ToastrService } from 'ngx-toastr';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import { StocksService } from './../stocks.service';
import { MasterService } from '../../core/common/master.service';
import * as XLSX from 'xlsx';
import swal from 'sweetalert';
import { Router } from '@angular/router';
declare var $: any;

@Component({
  selector: 'app-stock-taking',
  templateUrl: './stock-taking.component.html',
  styleUrls: ['./stock-taking.component.css']
})

export class StockTakingComponent implements OnInit, AfterContentChecked {
  @ViewChild("BarcodeNo", { static: true }) BarcodeNo: ElementRef;
  @ViewChild('TABLE', { static: true }) TABLE: ElementRef;
  printData: any;
  title = 'Excel';
  ccode: string = "";
  bcode: string = "";
  radioItems: Array<string>;
  model = { option: 'ST. Summary' };
  StockTakingForm: FormGroup;
  BatchNo: Number = 0;
  isDisabled: boolean = false;
  EnableJson: boolean = false;
  password: string;
  constructor(private fb: FormBuilder, private stocktakingService: StockTakingService,
    private _stocksService: StocksService, private toastr: ToastrService,
    private _masterService: MasterService, private _appConfigService: AppConfigService,
    private _router: Router) {
    this.radioItems = ['ST. Summary', 'ST. Details', 'ST. Details(stone)'];
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
    this.getCB();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  StockTakingSummaryHeader: any = {
    CompanyCode: this.ccode,
    BranchCode: this.bcode,
    BatchNo: this.BatchNo,
    BarcodeNo: null,
    ItemName: null,
    Qty: 0,
    GrossWt: 0,
    NetWt: 0,
    SalCode: null,
    CounterCode: null,
    Dcts: 0,
    UpdateOn: null,
  }

  printHeader: any = {
    radioItems: null,
  }

  ngOnInit() {
    this.getApplicationDate();
    this.getCounter();
    this.getSalesman();
    this.getShowroomDetails();
    this.StockTakingSummaryHeader.CompanyCode = this.ccode;
    this.StockTakingSummaryHeader.BranchCode = this.bcode;
    this.StockTakingForm = this.fb.group({
      BatchNo: null,
      LastBatchNo: null,
      BarcodeNo: null,
      CounterCode: null,
      ItemName: null,
      SalCode: null,
      CompanyCode: this.ccode,
      BranchCode: this.bcode,
    });
  }


  ngAfterContentChecked() {
    //this.isDisabled = false;
    //this.BarcodeNo.nativeElement.focus();
  }

  SalesmanList: any = [];
  getSalesman() {
    this.stocktakingService.StaffList().subscribe(
      Response => {
        this.SalesmanList = Response;
      }
    )
  }

  CounterList: any = [];
  getCounter() {
    this.stocktakingService.GetCounter().subscribe(
      Response => {
        this.CounterList = Response;
      }
    )
  }

  // itempush: any = {
  //   ItemCode: "ALL",
  //   ItemName: "ALL"
  // }

  itempush: any = {
    Code: "ALL",
    Name: "ALL"
  }


  // Item: any = [];
  // getItem(arg) {
  //   this._stocksService.getItemBasedonCounter(arg).subscribe(
  //     Response => {
  //       this.Item = Response;
  //       this.Item.push(this.itempush);
  //     }
  //   )
  // }

  Item: any = [];
  getItem(arg) {
    this.stocktakingService.getItem(arg).subscribe(
      Response => {
        this.Item = Response;
        this.Item.push(this.itempush);
      }
    )
  }

  BatchList: any = [];
  BatchHeaderDets: any = [];
  getBatchDetails(arg) {
    this.stocktakingService.getBatchInfo(arg).subscribe(
      Response => {
        this.postArray = Response;
        this.StockTakingSummaryHeader.CompanyCode = this.ccode;
        this.StockTakingSummaryHeader.BranchCode = this.bcode;
        if (this.postArray.length > 0) {
          this._stocksService.getBatchHeader(arg).subscribe(
            response => {
              this.BatchHeaderDets = response;
              this.StockTakingSummaryHeader.CounterCode = this.BatchHeaderDets.Counter;
              this.StockTakingSummaryHeader.ItemName = this.BatchHeaderDets.Item;
              this.StockTakingSummaryHeader.SalCode = this.BatchHeaderDets.Salesman;
              this.getItem(this.BatchHeaderDets.Counter);
            }
          )
          this.isDisabled = true;
        }
        else {
          this.isDisabled = false;
        }
        this.getBatchSummary(arg);
      });
    this.BarcodeNo.nativeElement.focus();
  }

  summary: any = [];
  getBatchSummary(arg) {
    this.stocktakingService.getSummary(arg).subscribe(
      Response => {
        this.summary = Response;
        if (this.summary.length > 0) {
          this.StockTakingSummaryHeader.BatchNo = this.summary[0].BatchNo;
        }
        else {

        }
      }
    )
  }

  Stone: any = [];
  getStoneDetails(arg) {
    this.stocktakingService.getStoneDetails(arg).subscribe(
      Response => {
        this.Stone = Response;
        this.StockTakingSummaryHeader.BatchNo = this.Stone[0].BatchNo;
        this.getStonetotal(this.StockTakingSummaryHeader.BatchNo);
      }
    )
  }

  StTotal: any = [];
  getStonetotal(arg) {
    this.stocktakingService.getStoneTotal(arg).subscribe(
      Response => {
        this.StTotal = Response;
      }
    )
  }

  reloadData: any = [];

  Delete(index, arg) {
    var ans = confirm("Do you want to delete Tag No:" + arg.BarcodeNo + "??");
    if (ans) {
      if (this.postArray.length == 1) {
        this.StockTakingSummaryHeader.BatchNo = 0;
        this.stocktakingService.deleteBarcode(arg.BatchNo, arg.BarcodeNo).subscribe(
          response => {
            this.reloadData = response;
            //swal("Success!", "Tag No:" + arg.BarcodeNo + " deleted successfully", "success");
            this.getBatchDetails(arg.BatchNo);
            this.isDisabled = false;
          }
        )
      }
      else {
        this.stocktakingService.deleteBarcode(arg.BatchNo, arg.BarcodeNo).subscribe(
          response => {
            this.reloadData = response;
            //swal("Success!", "Tag No:" + arg.BarcodeNo + " deleted successfully", "success");
            this.getBatchDetails(arg.BatchNo);
            this.isDisabled = true;
          }
        )
      }
    }
    else {
      this.BarcodeNo.nativeElement.focus();
    }
  }

  ShowroomList: any = [];
  getShowroomDetails() {
    this._stocksService.getShowroom().subscribe(
      response => {
        this.ShowroomList = response;
      }
    );
  }

  PostResult: any = [];
  postArray: any = [];
  result: any = [];
  routerUrl: string = "";

  StocktakingPost() {
    this.stocktakingService.StockTakingPost(this.StockTakingSummaryHeader).subscribe(
      Response => {
        this.PostResult = Response;
        this.postArray.push(this.PostResult);
        this.getBatchSummary(this.PostResult.BatchNo);
        this.StockTakingSummaryHeader.BarcodeNo = null;
        if (this.PostResult != null) {
          this.StockTakingSummaryHeader.BatchNo = this.PostResult.BatchNo;
          this.getBatchSummary(this.StockTakingSummaryHeader.BatchNo);
          this.isDisabled = true;
          this.routerUrl = this._router.url;
        }
      },
      err => {
        if (err.status === 409) {
          const validationError = err.error;
          // this.toastr.warning(this.StockTakingSummaryHeader.BarcodeNo + validationError.description, 'Alert!!');
          this.toastr.warning("Tag No: " + this.StockTakingSummaryHeader.BarcodeNo + " is already scanned", 'Alert!!');
          this.isDisabled = false;
          this.BarcodeNo.nativeElement.value = '';
        }
        else if (err.status === 404) {
          const validationError = err.error;
          this.toastr.warning(validationError.description, 'warning');
          this.isDisabled = false;
          this.BarcodeNo.nativeElement.value = '';
        }
        else if (err.status === 400) {
          if (this.StockTakingSummaryHeader.BatchNo == 0 && this.StockTakingSummaryHeader.BarcodeNo == null) {
            const validationError = err.error;
            this.toastr.warning(validationError.description, 'Waring!!');
            this.StockTakingSummaryHeader.BarcodeNo = null;
            this.isDisabled = false;
            this.BarcodeNo.nativeElement.value = '';
          }
        }
      }
    );
  }

  Changed(arg) {
    if (arg === 'ST. Summary') {
      this.model.option = arg;
    }
    else if (arg === 'ST. Details') {
      this.model.option = arg;
    }
    else if (arg === 'ST. Details(stone)') {
      this.model.option = arg;
    }
  }

  print() {
    // if (this.StockTakingSummaryHeader.BatchNo == null || this.BatchList.length == 0) {
    if (this.StockTakingSummaryHeader.BatchNo == null || this.StockTakingSummaryHeader.BatchNo == 0) {
      alert("Please enter the Batch No");
      $('#BatchSummary').modal('hide');
    }
    else {
      $('#BatchSummary').modal('show');
      if (this.model.option == "ST. Summary") {
        this._stocksService.getStockTakingDetailBySummary(this.StockTakingSummaryHeader.BatchNo).subscribe(
          response => {
            this.printData = response;
          }
        )
      }
      else if (this.model.option == "ST. Details") {
        this._stocksService.getStockTakingDetail(this.StockTakingSummaryHeader.BatchNo).subscribe(
          response => {
            this.printData = response;
          }
        )
      }
      else if (this.model.option == "ST. Details(stone)") {
        this._stocksService.getStockTakingStoneDetail(this.StockTakingSummaryHeader.BatchNo).subscribe(
          response => {
            this.printData = response;
          }
        )
      }
    }
  }

  closeModal() {
    $('#Summary').modal('hide');
    $('#Details').modal('hide');
    $('#stone').modal('hide');
  }

  reprintHeaders = {
    applicationDate: null,
    appViewDate: null,
  }

  applicationDate: any;

  appViewDate: any;

  getApplicationDate() {
    this._stocksService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
        this.appViewDate = appDate['appViewDate'];
        this.reprintHeaders.applicationDate = this.applicationDate;
        this.reprintHeaders.appViewDate = this.appViewDate;
      }
    )
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

  NetWt(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      if (d.NetWt != null && d.NetWt != 0) {
        total += Number(<number>d.NetWt);
      }
    });
    return total;
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

  Dcts(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      if (d.Dcts != null && d.Dcts != 0) {
        total += Number(<number>d.Dcts);
      }
    });
    return total;
  }

  printSummary() {
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
      .watermark {
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
  ${printContents}
  </body>
    </html>`
    );
    popupWin.document.close();
  }
  printDetails() {
    let printContents, popupWin;
    printContents = document.getElementById('print-sectionDetails').innerHTML;
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
      .watermark {
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
      .right {
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
  printStone() {
    let printContents, popupWin;
    printContents = document.getElementById('print-sectionStone').innerHTML;
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
      .watermark {
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
      .right {
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


  ExportToExcel() {
    if (this.StockTakingSummaryHeader.BatchNo == null || this.StockTakingSummaryHeader.BatchNo == 0) {
      alert("Please enter the Batch No");
      $('#BatchSummary').modal('hide');
    }
    else {
      const ws: XLSX.WorkSheet = XLSX.utils.table_to_sheet(this.TABLE.nativeElement);
      const wb: XLSX.WorkBook = XLSX.utils.book_new();
      XLSX.utils.book_append_sheet(wb, ws, 'Sheet1');
      XLSX.writeFile(wb, 'StockTaking.xlsx');
    }
  }


  printBatchSummary() {
    this._masterService.printPlainText(this.printData);
  }
}