import { Component, OnInit } from '@angular/core';
import { OpgprocessService } from '../opg-process.service';
import { AppConfigService } from '../../AppConfigService';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { Router } from '@angular/router';
import { AccountsService } from '../../accounts/accounts.service';
import { OPGMeltingLines } from '../opg-process.model';
import { MastersService } from '../../masters/masters.service';
import { formatDate } from '@angular/common';
declare var $: any;
@Component({
  selector: 'app-opg-melting-receipt',
  templateUrl: './opg-melting-receipt.component.html',
  styleUrls: ['./opg-melting-receipt.component.css']
})
export class OpgMeltingReceiptComponent implements OnInit {
  routerUrl: string = "";
  EnableDiv: boolean = false;
  applicationDate: any;
  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  datePickerConfig: Partial<BsDatepickerConfig>;
  today = new Date();
  constructor(private accountservice: AccountsService,
    public _opgprocessService: OpgprocessService,
    private _appConfigService: AppConfigService,
    private _router: Router, private _mastersService: MastersService) {
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
  TodayExchangeRate: any;


  PurchaseRate: any = [];
  PurchaseRate1: any = {};


  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this.getApplicationdate();
    this.routerUrl = this._router.url;
    this.getreceiptFrom();
    this.getGSList();
    this.opgMeltingReceiptPost.CompanyCode = this.ccode;
    this.opgMeltingReceiptPost.BranchCode = this.bcode;
  }

  EnableItemDetails: boolean = false;

  ToggleitemDetails() {
    this.EnableItemDetails = !this.EnableItemDetails;
  }
  private newAttribute: OPGMeltingLines = {
    IssueNo: null,
    BatchId: "",
    GSCode: "",
    ItemCode: "",
    Qty: null,
    GrossWt: null,
    StoneWt: null,
    NetWt: null,
    WastageWt: null,
    PurityPercent: null,
    AlloyWt: null,
    Amount: null,
    Rate: null,
  };


  fieldArray: any = [];
  count: number = 0;
  disabledEditbutton: boolean = false;
  EnableEditDelbtn = {};
  EnableSaveCnlbtn = {};
  readonly = {};

  getApplicationdate() {
    let appDate : any;
    this.accountservice.getApplicationDate().subscribe(
      response => {
        appDate = response;
        this.applicationDate =  formatDate(appDate["applcationDate"], 'dd/MM/yyyy', 'en_GB');
      }
    )
  }
  receiptfrom: any = [];
  getreceiptFrom() {
    this._opgprocessService.getReceiptfrom().subscribe(
      response => {
        this.receiptfrom = response;

      }
    )
  }
  GsList: any = [];
  getGSList() {
    this._opgprocessService.getMeltingReceiptItemGS().subscribe(
      response => {
        this.GsList = response;

      }
    )
  }
  opgMeltingReceiptPost: any = {
    CompanyCode: null,
    BranchCode: null,
    ReceiptFrom: null,
    IssueNo: null,
    Remarks: "",
    OPGReceiptLines: []
  };
  OPGReceiptLines: any =
    {
      BatchId: "",
      GSCode: "",
      IssueGrossWt: 0,
      ReceiptGrossWt: 0,
      KovaWeight: 0,
      MeltingLossWeight: 0,
      Rate: 0,
      Amount: 0,
      CalculatedMeltingLoss: 0,
      CalculatedTotalReceiptWeight: 0,
      CalculatedAmount: 0
    }


  IssueNumbers: any = [];
  getAllIssueNo(arg) {
    this._opgprocessService.getIssueNo(arg.target.value).subscribe(
      response => {
        this.IssueNumbers = response;
      }
    )
  }
  MeltingReceiptItems: any = [];
  getMletingReceiptDetails(arg) {
    this.fieldArray =[];
    this._opgprocessService.getMeltingReceiptDatilsByIssueNo(arg.target.value).subscribe(
      response => {
        this.MeltingReceiptItems = response;
        if (this.MeltingReceiptItems.length > 0) {
          for (let i = 0; i < this.MeltingReceiptItems.length; i++) {
            this.getBatchDetails(this.MeltingReceiptItems[i].BatchId, i)
          }
        }
      }
    )

  }

  BatchDetails: any = [];
  getBatchDetails(BatchID, index) {
    this._opgprocessService.getMeltingReceiptBatchDetail(this.opgMeltingReceiptPost.IssueNo, BatchID).subscribe(
      response => {
        this.BatchDetails[index] = response;
        this.MeltingReceiptItems[index] = this.BatchDetails[index];

      }
    )
  }


  edit(arg, index) {
   
    if (this.fieldArray.length > 0) {
      this.fieldArray.splice(index, 1);
    }
    arg.NetWt = 0;

    this.disabledEditbutton = true;
    this.newAttribute = {
      IssueNo: null,
      BatchId: "",
      GSCode: "",
      ItemCode: "",
      Qty: null,
      GrossWt: null,
      StoneWt: null,
      NetWt: null,
      WastageWt: null,
      PurityPercent: null,
      AlloyWt: null,
      Amount: null,
      Rate: null,
    }
    this.fieldArray=[];
    this.fieldArray.push(arg);
    for (let { } of this.fieldArray) {
      this.count++;
    }
    this.disabledEditbutton = true;
    this.EnableSaveCnlbtn[this.count - 1] = true;
    this.EnableEditDelbtn[this.count - 1] = false;
    this.readonly[this.count - 1] = false
    this.count = 0;
  }
  CopyEditedRow: any = [];
  CalculateNetweight(index) {
    if (this.fieldArray[index].NetWt > this.fieldArray[index].GrossWt) {
      swal("Warning!", "Recevied weight should not be more than Issued Weight", "warning");
    }
    else if (this.fieldArray[index].NetWt > 0) {
      const WastageWt = Number(this.fieldArray[index].GrossWt) - Number(this.fieldArray[index].NetWt);
      this.fieldArray[index].WastageWt = Number(WastageWt.toFixed(3));
    }

  }


  calcAmount(index) {
    this.fieldArray[index].Amount = Math.round(<number>this.fieldArray[index].Rate * <number>this.fieldArray[index].NetWt).toFixed(3);
  }
  calculateKovaWeight(index) {
    this.CalculateNetweight(index);
    if (this.fieldArray[index].AlloyWt >= 0) {
      const KovaCalcuation = Number(this.fieldArray[index].WastageWt) - Number(this.fieldArray[index].AlloyWt);
      this.fieldArray[index].WastageWt = Number(KovaCalcuation.toFixed(3));
    }
  }
  //in case calculation shave to day by ui side 
  // calculatePurity() {

  // }
  // calcMeltingLossWeight() {

  // }
  // calcCalculatedMeltingLoss() {

  // }
  // CalculatedTotalReceiptWeight() {

  // }
  // CalculatedAmount() {

  // }
  ////
  saveDataFieldValue(arg, index) {
    if (this.fieldArray[index].GSCode == null) {
      swal("Warning!", 'Please select the GS', "warning");
    }
    else if (this.fieldArray[index].BatchId == null) {
      swal("Warning!", 'Please select the BatchId', "warning");
    }
    else if (arg.NetWt == 0) {
      swal("Warning!", 'Please Enter Received Weight', "warning");
    }
    else if (this.fieldArray[index].Rate == null || this.fieldArray[index].Rate == 0) {
      swal("Warning!", 'Please enter the Rate', "warning");
    }
    else {
      this.MeltingReceiptItems[index] = this.fieldArray[index];
      if (this.fieldArray.length >= 0) {
        for (let entry of this.fieldArray) {
          this.OPGReceiptLines.BatchId = entry.BatchId;
          this.OPGReceiptLines.GSCode = entry.GSCode;
          this.OPGReceiptLines.IssueGrossWt = entry.GrossWt;
          this.OPGReceiptLines.ReceiptGrossWt = entry.NetWt;
          this.OPGReceiptLines.KovaWeight = entry.AlloyWt;
          this.OPGReceiptLines.MeltingLossWeight = entry.WastageWt;
          this.OPGReceiptLines.Rate = entry.Rate;
          this.OPGReceiptLines.Amount = entry.Amount;
          this.OPGReceiptLines.CalculatedMeltingLoss = 0;
          this.OPGReceiptLines.CalculatedTotalReceiptWeight = 0;
          this.OPGReceiptLines.CalculatedAmount = 0;
          this.opgMeltingReceiptPost.OPGReceiptLines.push(this.OPGReceiptLines);
          //console.log(JSON.stringify(this.opgMeltingReceiptPost.OPGReceiptLines.push(this.OPGReceiptLines)));
          this.disabledEditbutton = false;
          this.EnableEditDelbtn[index] = false;
          this.EnableSaveCnlbtn[index] = true;
          this.readonly[index] = false;
          this.fieldArray.splice(index, 1);
        }
      }
    }
  }

  editFieldValue(index) {
    if (this.EnableDiv == true) {
      swal("Warning!", 'Please save the enabled item', "warning");
    }
    else {
      this.CopyEditedRow[index] = Object.assign({}, this.fieldArray[index]);
      this.EnableEditDelbtn[index] = false;
      this.EnableSaveCnlbtn[index] = true;
      this.readonly[index] = false;
      this.EnableDiv = true;
    }
  }

  cancelDataFieldValue(index) {
    this.fieldArray.splice(index, 1);
    this.fieldArray = [];
    this.disabledEditbutton = false;
    this.newAttribute = {
      IssueNo: null,
      BatchId: "",
      GSCode: "",
      ItemCode: "",
      Qty: null,
      GrossWt: null,
      StoneWt: null,
      NetWt: null,
      WastageWt: null,
      PurityPercent: null,
      AlloyWt: null,
      Amount: null,
      Rate: null,
    }


  }
  meltingreceiptPrintData: any = [];
  PrintTypeBasedOnConfig: any;
  outputMeltingReceipt: any = [];
  SaveMeltingReceipt() {
    if (this.opgMeltingReceiptPost.IssueNo == null) {
      swal("Warning!", 'Please select the Issue To', "warning");
    }
    else if (this.opgMeltingReceiptPost.OPGReceiptLines.length == 0) {
      swal("Warning!", 'Please enter the Item Details', "warning");
    }
    else {
      var ans = confirm("Do you want to submit??");
      if (ans) {
        this._opgprocessService.postMeltingReceipt(this.opgMeltingReceiptPost).subscribe(
          response => {
            this.outputMeltingReceipt = response;
            swal("Saved!", this.outputMeltingReceipt.Message, "success");
            this.fieldArray = [];
            var ans = confirm("Do you want to take print??");
            if (ans) {
              this._opgprocessService.getMeltingReceiptPrint(this.opgMeltingReceiptPost.IssueNo).subscribe(
                response => {
                  this.PrintTypeBasedOnConfig = response;
                  if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
                    $('#MeltingReceiptTab').modal('show');
                    this.meltingreceiptPrintData = atob(this.PrintTypeBasedOnConfig.Data);
                    $('#DisplayntReceiptData').html(this.meltingreceiptPrintData);
                  }
                  this.opgMeltingReceiptPost = {
                    CompanyCode: null,
                    BranchCode: null,
                    ReceiptFrom: null,
                    IssueNo: null,
                    Remarks: "",
                    OPGReceiptLines: []
                  };
                }
              )
            }
          }
        )
      }
    }
  }


  print() {
    if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printntIssueData, popupWin;
      printntIssueData = document.getElementById('DisplayntReceiptData').innerHTML;
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
  
  cancel(){
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
      () => {
        this._router.navigate(['/opg-process/opg-melting-receipt']);
      }
    )
  }

}
