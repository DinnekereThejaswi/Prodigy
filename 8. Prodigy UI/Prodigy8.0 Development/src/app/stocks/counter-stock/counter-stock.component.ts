import { Component, OnInit, ɵConsole } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { CounterStockService } from './counter-stock.service';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { ToastrService } from 'ngx-toastr';
declare var $: any;
import { DatePipe } from '@angular/common';
import * as CryptoJS from 'crypto-js';
import { StocksService } from './../stocks.service';
import { AppConfigService } from '../../AppConfigService';
import { MasterService } from '../../core/common/master.service';

@Component({
  selector: 'app-counter-stock',
  templateUrl: './counter-stock.component.html',
  styleUrls: ['./counter-stock.component.css'],
  providers: [DatePipe]
})
export class CounterStockComponent implements OnInit {
  searchText;
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;
  CounterStockForm: FormGroup;
  ModelForm: FormGroup;
  short: boolean = true;
  excess: boolean = false;
  datePickerConfig: Partial<BsDatepickerConfig>;
  today = new Date();
  EnableJson: boolean = false;
  constructor(private counterService: CounterStockService,
    private fb: FormBuilder, private datePipe: DatePipe,
    private service: StocksService, private toastr: ToastrService, private appConfigService: AppConfigService,
    private _masterService: MasterService) {
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        maxDate: this.today,
        dateInputFormat: 'YYYY-MM-DD',

      });
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.EnableJson = this.appConfigService.EnableJson;
    this.password = this.appConfigService.Pwd;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }
  CounterStockSummaryHeader: any = {
    CompanyCode: this.ccode,
    BranchCode: this.bcode,
    TranaactionNo: 0,
    GSCode: null,
    CounterCode: null,
    ItemCode: null,
    Qty: null,
    GrossWt: null,
    StoneWt: null,
    NetWt: null,
    SalCode: null,
    TransactionDate: null,
    Remarks: null,
    GrWt: null,
  };

  ngOnInit() {
    this.getApplicationDate();
    this.getGs();
    this.getCounter();
    this.getShowroomDetails();
    this.CounterStockSummaryHeader.CompanyCode = this.ccode;
    this.CounterStockSummaryHeader.BranchCode = this.bcode;
    this.CounterStockForm = this.fb.group({
      applicationDate: null,
      companyCode: this.ccode,
      BranchCode: this.bcode,
      GS: null,
      CounterCode: null,
      CurQty: null,
    });
    this.ModelForm = this.fb.group({
      GSCode: null,
      companyCode: this.ccode,
      BranchCode: this.bcode,
      CounterCode: null,
      ItemCode: null,
      Qty: null,
      GrossWt: null,
      StoneWt: null,
      NetWt: null,
      SalCode: null,
      TransactionDate: null,
      Remarks: null,
      shortRefNo: null,
      excessRefNo: null,
      IssuedBy: null,
      ReceiptBy: null,
      GrWt: null,
      CurQty: null,
    });
  }
  ShowroomList: any = [];
  getShowroomDetails() {
    this.service.getShowroom().subscribe(
      response => {
        this.ShowroomList = response;
        // this.calculateFooter();
      }
    );
  }
  reprintHeaders = {
    applicationDate: null,
    appViewDate: null,
  }
  applicationDate: any;
  appViewDate: any;
  getApplicationDate() {
    this.counterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
        this.appViewDate = appDate['appViewDate'];
        this.reprintHeaders.applicationDate = this.applicationDate;
        this.reprintHeaders.appViewDate = this.appViewDate;
      }
    )
  }
  CounterList: any = [];
  getCounter() {
    this.counterService.GetCounter().subscribe(
      Response => {
        this.CounterList = Response;
      }
    );
  }
  GSList: any = [];
  getGs() {
    this.counterService.GSList().subscribe(
      Response => {
        this.GSList = Response;
      }
    );
  }
  GSSummaryList: any = [];
  GSWiseSummary(arg) {
    const oldDate = this.applicationDate;
    const date = new Date(oldDate);
    const gsdate = this.datePipe.transform(date, 'yyyy-MM-dd');
    if (arg.value.GS === null) {
      alert("Please select GS");
    }
    else if (arg.value.CounterCode == null) {
      alert("Please select CounterCode");
    }
    else {
      $('#GSWiseSummary').modal('show');
      this.counterService.getGSWiseSummary(arg.value.GS, arg.value.CounterCode, gsdate).subscribe(
        Response => {
          this.GSSummaryList = Response;
        }
      );
    }
  }

  ClosingStockSummaryList: any = [];

  ClosingStockSummary(arg) {
    const oldDate = this.applicationDate;
    const date = new Date(oldDate);
    const gsdate = this.datePipe.transform(date, 'yyyy-MM-dd');
    if (arg.value.GS === null) {
      alert("Please select GS");
    }
    else if (arg.value.CounterCode == null) {
      alert("Please select CounterCode");
    }
    else {
      $('#ClosingStockSummary').modal('show');
      this.counterService.getClosingStockReport(arg.value.GS, arg.value.CounterCode, gsdate).subscribe(
        Response => {
          this.ClosingStockSummaryList = Response;
        }
      );
    }
  }


  CounterSummaryList: any = [];

  CounterSummary(arg) {
    const oldDate = this.applicationDate;
    const date = new Date(oldDate);
    const gsdate = this.datePipe.transform(date, 'yyyy-MM-dd');
    if (arg.value.GS === null) {
      alert("Please select GS");
    }
    else if (arg.value.CounterCode == null) {
      alert("Please select CounterCode");
    }
    else {
      $('#CounterSummary').modal('show');
      this.counterService.getCounterSummaryReport(arg.value.GS, arg.value.CounterCode, gsdate).subscribe(
        Response => {
          this.CounterSummaryList = Response;
          $('#DisplayData').html(this.CounterSummaryList);
        }
      );
    }
  }







  ItemWiseList: any = [];
  ItemWiseSummary(arg) {
    const oldDate = this.applicationDate;
    const date = new Date(oldDate);
    const gsdate = this.datePipe.transform(date, 'yyyy-MM-dd');
    if (arg.value.GS === null) {
      alert("Please select GS");
    }
    else if (arg.value.CounterCode == null) {
      alert("Please select CounterCode");
    }
    else {
      $('#ItemWiseSummary').modal('show');
      this.counterService.getCounterStockDetailsReport(arg.value.GS, arg.value.CounterCode, gsdate).subscribe(
        Response => {
          this.ItemWiseList = Response;
        }
      );
    }
  }

  CounterStockList: any = [];
  CounterStockMain(arg) {
    const oldDate = this.applicationDate;
    const date = new Date(oldDate);
    const gsdate = this.datePipe.transform(date, 'yyyy-MM-dd');

    if (arg.value.GS == null) {
      alert('Please select GS');
    }
    else if (arg.value.CounterCode == null) {
      alert('Please select Sub Location');
    }
    this.counterService.getCounterStockDetails(arg.value.GS, arg.value.CounterCode, gsdate).subscribe(
      Response => {
        this.CounterStockList = Response;
        this.GetTotal(arg);
      }
    );
  }
  TotalList: any = [];
  GetTotal(arg) {
    const oldDate = this.applicationDate;
    const date = new Date(oldDate);
    const gsdate = this.datePipe.transform(date, 'yyyy-MM-dd');
    this.counterService.getSumDetails(arg.value.GS, arg.value.CounterCode, gsdate).subscribe(
      Response => {
        this.TotalList = Response;
      }
    );
  }
  closeModal() {
    $('#short').modal('hide');
    $('#GSWiseSummary').modal('hide');
    $('#ItemWiseSummary').modal('hide');

  }
  sendExcessData(arg) {
    this.short = false;
    this.excess = true;
    $('#short').modal('show');
    this.CounterStockSummaryHeader.CounterCode = arg.Cntr;
    this.CounterStockSummaryHeader.CompanyCode = this.ccode;
    this.CounterStockSummaryHeader.BranchCode = this.bcode;
    this.CounterStockSummaryHeader.GSCode = arg.GS;
    this.CounterStockSummaryHeader.ItemCode = arg.ItemName;
    this.CounterStockSummaryHeader.TransactionDate = this.CounterStockForm.value.Date;
    this.CounterStockSummaryHeader.SalCode = localStorage.getItem('Login');
    this.CounterStockSummaryHeader.GrWt = arg.ClsGwt;
    this.CounterStockSummaryHeader.CurQty = arg.ClsQty;
  }
  sendShortData(arg) {
    this.short = true;
    this.excess = false;
    $('#short').modal('show');
    this.CounterStockSummaryHeader.CounterCode = arg.Cntr;
    this.CounterStockSummaryHeader.CompanyCode = this.ccode;
    this.CounterStockSummaryHeader.BranchCode = this.bcode;
    this.CounterStockSummaryHeader.GSCode = arg.GS;
    this.CounterStockSummaryHeader.ItemCode = arg.ItemName;
    this.CounterStockSummaryHeader.TransactionDate = this.CounterStockForm.value.Date;
    this.CounterStockSummaryHeader.SalCode = localStorage.getItem('Login');
    this.CounterStockSummaryHeader.GrWt = arg.ClsGwt;
    this.CounterStockSummaryHeader.CurQty = arg.ClsQty;
  }
  counterPut: any;
  CounterExcessPut() {
    if (this.CounterStockSummaryHeader.GrossWt == null || this.CounterStockSummaryHeader.GrossWt == '') {
      alert("Please Enter Qty/Gross wt to Receipt");
    }
    else if (this.CounterStockSummaryHeader.Remarks == null || this.CounterStockSummaryHeader.Remarks == '') {
      alert("Please Enter Remark");
    }
    else {
      this.counterService.putExcess(this.CounterStockSummaryHeader).subscribe(
        Response => {
          this.counterPut = Response;
          this.CounterStockMain(this.CounterStockForm);
          $('#short').modal('hide');
          this.ModelForm.reset();
        },
        err => {
          if (err.status === 409) {
            const validationError = err.error;
            this.toastr.warning(validationError.description, 'Alert!!');
          }
          else if (err.status === 200) {
            // const validationError = err.error;
            this.toastr.warning("Updated Successfully", 'Alert!!');
          }
          else if (err.status === 404) {
            const validationError = err.error;
            this.toastr.warning(validationError.description, 'warning');
          }
          else if (err.status === 400) {
            const validationError = err.error;
            this.toastr.warning(validationError.description, 'Waring!!');
          }
        }
      );
    }
  }
  netwt() {
    this.CounterStockSummaryHeader.NetWt = this.CounterStockSummaryHeader.GrossWt - this.CounterStockSummaryHeader.StoneWt;
  }
  counterShort: any = [];
  CounterShortPut() {
    if (this.CounterStockSummaryHeader.GrossWt == null || this.CounterStockSummaryHeader.GrossWt == '') {
      alert("Please Enter Qty/Gross wt/Stone wt to Issue");
    }
    else if (this.CounterStockSummaryHeader.Remarks == null || this.CounterStockSummaryHeader.Remarks == '') {
      alert("Please Enter Remark")
    }
    else {
      this.counterService.putShort(this.CounterStockSummaryHeader).subscribe(
        Response => {
          this.counterShort = Response;
          this.CounterStockMain(this.CounterStockForm);
          $('#short').modal('hide');
          this.ModelForm.reset();
        }
      );
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
  //   printItemWise(){
  //     let printContents, popupWin;
  //     printContents = document.getElementById('print-sectionItemwise').innerHTML;
  //     popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
  //     popupWin.document.open();
  //     popupWin.document.write(`
  //       <html>
  //         <head>
  //           <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css" integrity="sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm" crossorigin="anonymous">
  //           <title>Print tab</title>
  //           <style>
  //          .htmlPrint{display:none;}

  //           @media print {
  //             .table-bordered
  //         {
  //             border-style: solid;
  //             border: 3px solid rgb(0, 0, 0);
  //         }
  //         .margin{
  //           margin-top:-24px;
  //         }
  //         .modal-content{
  //           font-family: "Times New Roman", Times, serif;

  //         }
  //         .padding {
  //           padding: 0.10rem !important;
  //       }
  //        .padding-top {
  //          padding-top:20px;
  //        }
  //         .watermark{
  //           -webkit-transform: rotate(331deg);
  //           -moz-transform: rotate(331deg);
  //           -o-transform: rotate(331deg);
  //           transform: rotate(331deg);
  //           font-size: 15em;
  //           color: rgba(255, 5, 5, 0.37);
  //           position: absolute;
  //           text-transform:uppercase;
  //           padding-left: 10%;
  //           margin-top:-10px;
  //         }
  //         .right{
  //           text-align:left;
  //         }
  //         .px-2 {
  //           padding-left: 10px !important;
  //         }
  //         thead > tr
  //         {
  //             border-style: solid;
  //             font-size: 12px !important;
  //             border: 3px solid rgb(0, 0, 0);
  //         }
  //         tbody > tr
  //         {
  //             border-style: solid;
  //             font-size: 12px !important;
  //             border: 3px solid rgb(0, 0, 0);
  //         }
  //           .invoice {
  //           margin-top: 250px;
  //       }
  //         .card{
  //           border-style: solid;
  //           border-width: 5px;
  //           border: 3px solid rgb(0, 0, 0);
  //       }
  //         .printMe {
  //           display: none !important;
  //         }
  //       }
  //       body{
  //             font-size: 1px;
  //             line-height: 18px;
  //       }
  //  </style>
  //         </head>
  //     <body onload="window.print();window.close()">
  //     ${printContents}</body>
  //       </html>`
  //     );
  //     popupWin.document.close();
  //   }


  printCounterStockSummary() {
    this._masterService.printPlainText(this.GSSummaryList);
  }


  printItemWise() {
    this._masterService.printPlainText(this.ItemWiseList);
  }

  printCounterSummary() {
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