import { Component, OnInit } from '@angular/core';
import { AccountsService } from '../../accounts/accounts.service'
import { OPGBatchLines } from '../opg-process.model';
import { OpgprocessService } from '../opg-process.service';
import { AppConfigService } from '../../AppConfigService';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { MasterService } from '../../core/common/master.service';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { Router } from '@angular/router';
declare var $: any;


@Component({
  selector: 'app-opg-melting-issue',
  templateUrl: './opg-melting-issue.component.html',
  styleUrls: ['./opg-melting-issue.component.css']
})
export class OpgMeltingIssueComponent implements OnInit {
  routerUrl: string = "";
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
    private _router: Router, private _masterService: MasterService) {
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
    this.MeltingIssuePost.CompanyCode = this.ccode;
    this.MeltingIssuePost.BranchCode = this.bcode;
  }

  ngOnInit() {
    this.routerUrl = this._router.url;
    this.getApplicationdate();
    this.getGSList();
    this.getIssueTo();
  }

  private newAttribute: OPGBatchLines = {
    GSCode: null,
    BatchId: null,
    GrossWt: null,
    StoneWt: null,
    NetWt: null,
    Rate: null,
    Amount: null,
  };


  fieldArray: any = [];
  count: number = 0;
  EnableAddRow: boolean = false;
  EnableEditDelbtn = {};
  EnableSaveCnlbtn = {};
  readonly = {};

  add() {
    this.newAttribute = {
      GSCode: null,
      BatchId: null,
      GrossWt: null,
      StoneWt: null,
      NetWt: null,
      Rate: null,
      Amount: null,
    };

    this.fieldArray.push(this.newAttribute);
    for (let { } of this.fieldArray) {
      this.count++;
    }
    this.EnableAddRow = true;
    this.EnableSaveCnlbtn[this.count - 1] = true;
    this.EnableEditDelbtn[this.count - 1] = false;
    this.readonly[this.count - 1] = false
    this.count = 0;
  }

  GsList: any = [];
  IssueTo: any = [];
  BatchList: any = [];
  BatchDetails: any = [];

  MeltingIssuePost: any = {
    CompanyCode: null,
    BranchCode: null,
    IssueTo: null,
    Remarks: null,
    OPGBatchLines: []
  }


  getApplicationdate() {
    this.accountservice.getApplicationDate().subscribe(
      response => {
        this.applicationDate = response;
        this.applicationDate = this.applicationDate.applcationDate;
      }
    )
  }

  getGSList() {
    this._opgprocessService.getMeltingIssueItemGS().subscribe(
      response => {
        this.GsList = response;
      }
    )
  }

  getIssueTo() {
    this._opgprocessService.getMeltingIssueTo().subscribe(
      response => {
        this.IssueTo = response;
      }
    )
  }

  getBatchID(gsCode, index) {
    this._opgprocessService.getMeltingIssueBatchList(gsCode).subscribe(
      response => {
        this.BatchList[index] = response;
      },
      (err) => {
        this.BatchList[index] = [];
        this.fieldArray[index].GrossWt = null;
        this.fieldArray[index].NetWt = null;
        this.fieldArray[index].Rate = null;
        this.fieldArray[index].Amount = null;
      }
    )
  }

  getBatchDetails(BatchID, index) {
    this._opgprocessService.getMeltingIssueBatchDetail(BatchID).subscribe(
      response => {
        this.BatchDetails[index] = response;
        this.fieldArray[index] = this.BatchDetails[index];
      }
    )
  }

  calcAmount(index) {
    this.fieldArray[index].Amount = Math.round(<number>this.fieldArray[index].Rate * <number>this.fieldArray[index].NetWt);
  }

  cancel() {
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
      () => {
        this._router.navigate(['/opg-process/opg-melting-issue']);
      }
    )
  }


  saveDataFieldValue(index) {
    if (this.fieldArray[index].GSCode == null) {
      swal("Warning!", 'Please select the GS', "warning");
    }
    else if (this.fieldArray[index].BatchId == null) {
      swal("Warning!", 'Please select the BatchId', "warning");
    }
    else if (this.fieldArray[index].Rate == null || this.fieldArray[index].Rate == 0) {
      swal("Warning!", 'Please enter the Rate', "warning");
    }
    else {
      this.EnableEditDelbtn[index] = true;
      this.EnableSaveCnlbtn[index] = false;
      this.readonly[index] = true;
      this.EnableAddRow = false;
      this.MeltingIssuePost.OPGBatchLines[index] = this.fieldArray[index];


      // if(this.MeltingIssuePost.OPGBatchLines.length >= 0 ){
      //   for (let elValue of this.MeltingIssuePost.OPGBatchLines){
  
      //   }
      // }
    
    }
  }

  cancelDataFieldValue(index) {
    this.EnableAddRow = false;
    if (this.CopyEditedRow[index] != null) {
      this.fieldArray[index] = this.CopyEditedRow[index];
      this.getBatchID(this.fieldArray[index].GSCode, index);
      this.CopyEditedRow[index] = null;
      this.EnableSaveCnlbtn[index] = false;
      this.EnableEditDelbtn[index] = true;
      this.readonly[index] = true;
      this.CopyEditedRow = [];
    }
    else {
      this.fieldArray.splice(index, 1);
    }
  }

  CopyEditedRow: any = [];

  editFieldValue(index) {
    if (this.EnableAddRow == true) {
      swal("Warning!", 'Please save the enabled item', "warning");
    }
    else {
      this.CopyEditedRow[index] = Object.assign({}, this.fieldArray[index]);
      this.EnableEditDelbtn[index] = false;
      this.EnableSaveCnlbtn[index] = true;
      this.readonly[index] = false;
      this.EnableAddRow = true;
    }
  }

  EnableItemDetails: boolean = false;

  ToggleitemDetails() {
    this.EnableItemDetails = !this.EnableItemDetails;
  }

  deleteFieldValue(index) {
    if (this.EnableAddRow == true) {
      swal("Warning!", 'Please save the enabled item', "warning");
    }
    else {
      var ans = confirm("Do you want to delete??");
      if (ans) {
        this.fieldArray.splice(index, 1);
        this.MeltingIssuePost.OPGBatchLines = [];
        this.MeltingIssuePost.OPGBatchLines = this.fieldArray;
      }
    }
  }

  outputMeltingIssue: any = [];

  PrintTypeBasedOnConfig: any;

  opgMeltingIssuePrint: any = [];


  SaveMeltingIssue() {
    if (this.MeltingIssuePost.IssueTo == null) {
      swal("Warning!", 'Please select the Issue To', "warning");
    }
    else if (this.MeltingIssuePost.OPGBatchLines.length == 0) {
      swal("Warning!", 'Please enter the Item Details', "warning");
    }
    else {
      var ans = confirm("Do you want to submit??");
      if (ans) {
        this._opgprocessService.postMeltingIssue(this.MeltingIssuePost).subscribe(
          response => {
            this.outputMeltingIssue = response;
            swal("Saved!", this.outputMeltingIssue.Message, "success");
            this.fieldArray = [];
            this.MeltingIssuePost = {
              CompanyCode: null,
              BranchCode: null,
              IssueTo: null,
              Remarks: null,
              OPGBatchLines: []
            }
            var ans = confirm("Do you want to take print??");
            if (ans) {
              this._opgprocessService.getMeltingIssuePrint(this.outputMeltingIssue.DocumentNo).subscribe(
                response => {
                  this.PrintTypeBasedOnConfig = response;
                  if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
                    $('#OPGMeltingIssueTab').modal('show');
                    this.opgMeltingIssuePrint = atob(this.PrintTypeBasedOnConfig.Data);
                    $('#DisplayopgMeltingIssueData').html(this.opgMeltingIssuePrint);
                  }
                  else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
                    $('#OPGMeltingIssueTab').modal('show');
                    this.opgMeltingIssuePrint = this.PrintTypeBasedOnConfig.Data;
                  }
                }
              )
            }
          }
        )
      }
    }
  }


  printOpgMeltingIssue() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.opgMeltingIssuePrint);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printopgMeltingIssueData, popupWin;
      printopgMeltingIssueData = document.getElementById('DisplayopgMeltingIssueData').innerHTML;
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
    ${printopgMeltingIssueData}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }


}