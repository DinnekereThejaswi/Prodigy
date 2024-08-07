import { Component, OnInit } from '@angular/core';
import { OpgprocessService } from '../opg-process.service';
import { AccountsService } from '../../accounts/accounts.service'
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import swal from 'sweetalert';
import { Router } from '@angular/router';
import { AppConfigService } from '../../AppConfigService';
import { MasterService } from '../../core/common/master.service';
import * as CryptoJS from 'crypto-js';
declare var $: any;

@Component({
  selector: 'app-mg-issue-cpc',
  templateUrl: './mg-issue-cpc.component.html',
  styleUrls: ['./mg-issue-cpc.component.css']
})
export class MgIssueCpcComponent implements OnInit {

  routerUrl: string = "";
  applicationDate: any;
  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  datePickerConfig: Partial<BsDatepickerConfig>;
  today = new Date(); IssueTo: any = [];

  constructor(private _opgprocessService: OpgprocessService,
    private accountservice: AccountsService,
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

  ngOnInit() {
    this.routerUrl = this._router.url;
    this.getApplicationdate();
    this.getIssueTo();
    this.getDocumentNo();
    this.getSalesMan();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.MeltedGoldPost.CompanyCode = this.ccode;
    this.MeltedGoldPost.BranchCode = this.bcode;
  }

  SalesManList: any;
  getSalesMan() {
    this._masterService.getSalesMan().subscribe(
      response => {
        this.SalesManList = response;
      })
  }

  MeltedGoldPost: any = {
    CompanyCode: null,
    BranchCode: null,
    IssueTo: null,
    DocumentNo: null,
    Remarks: null,
    OPGIssueLines: []
  }

  getApplicationdate() {
    this.accountservice.getApplicationDate().subscribe(
      response => {
        this.applicationDate = response;
        this.applicationDate = this.applicationDate.applcationDate;
      }
    )
  }


  getIssueTo() {
    this._opgprocessService.getMeltingGoldIssueTo().subscribe(
      response => {
        this.IssueTo = response;
      }
    )
  }

  DocumentNo: any = [];

  getDocumentNo() {
    this._opgprocessService.getMeltingGoldDocNo().subscribe(
      response => {
        this.DocumentNo = response;
      }
    )
  }

  EnableItemDetails: boolean = false;

  ToggleitemDetails() {
    this.EnableItemDetails = !this.EnableItemDetails;
  }

  BatchList: any = [];

  Go() {
    if (this.MeltedGoldPost.DocumentNo == null) {
      swal("Warning!", 'Please select the OPG Seggregation Number', "warning");
    }
    else {
      this._opgprocessService.getMeltingGoldBatchList(this.MeltedGoldPost.DocumentNo).subscribe(
        response => {
          this.BatchList = response;
          this.MeltedGoldPost.OPGIssueLines = this.BatchList;
        }
      )
    }
  }

  totalWt(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      total += d.GrossWt;
    });
    return total;
  }

  taxAmt(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      total += d.Amount;
    });
    return total;
  }

  finAmt(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      total += d.Amount;
    });
    return total;
  }

  cancel() {
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
      () => {
        this._router.navigate(['/opg-process/opg-mg-issue-cpc']);
      }
    )
  }

  outputMeltedGold: any = [];
  PrintTypeBasedOnConfig: any;
  opgMeltingGoldPrint: any = [];
  outputXml: any;

  SaveMeltingGold() {
    if (this.MeltedGoldPost.IssueTo == null) {
      swal("Warning!", 'Please select the Issue To', "warning");
    }
    else if (this.MeltedGoldPost.OPGIssueLines.length == 0) {
      swal("Warning!", 'Please enter the Item Details', "warning");
    }
    else {
      var ans = confirm("Do you want to submit??");
      if (ans) {
        this._opgprocessService.postMeltedGold(this.MeltedGoldPost).subscribe(
          response => {
            this.outputMeltedGold = response;
            swal("Saved!", this.outputMeltedGold.Message, "success");
            this.BatchList = [];
            this.MeltedGoldPost = {
              CompanyCode: null,
              BranchCode: null,
              IssueTo: null,
              DocumentNo: null,
              Remarks: null,
              OPGIssueLines: []
            }
            this._opgprocessService.getMeltingGoldXML(this.outputMeltedGold.DocumentNo).subscribe(
              response => {
                this.outputXml = response;
                var ans = confirm("Do you want to take print??");
                if (ans) {
                  this._opgprocessService.getMeltingGoldPrint(this.outputMeltedGold.DocumentNo).subscribe(
                    response => {
                      this.PrintTypeBasedOnConfig = response;
                      if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
                        $('#OPGMeltingGoldTab').modal('show');
                        this.opgMeltingGoldPrint = atob(this.PrintTypeBasedOnConfig.Data);
                        $('#DisplayopgMeltingGoldData').html(this.opgMeltingGoldPrint);
                      }
                      else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
                        $('#OPGMeltingGoldTab').modal('show');
                        this.opgMeltingGoldPrint = this.PrintTypeBasedOnConfig.Data;
                      }
                    }
                  )
                }
              }
            )
          }
        )
      }
    }
  }

  printOpgMeltingGold() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.opgMeltingGoldPrint);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printopgMeltingGoldData, popupWin;
      printopgMeltingGoldData = document.getElementById('DisplayopgMeltingGoldData').innerHTML;
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
    ${printopgMeltingGoldData}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }

}