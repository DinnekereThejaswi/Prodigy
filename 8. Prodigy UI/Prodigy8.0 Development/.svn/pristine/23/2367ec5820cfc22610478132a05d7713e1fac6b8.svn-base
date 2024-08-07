import { Component, OnInit } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { BranchissueService } from '../branchissue.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import * as CryptoJS from 'crypto-js';
import { MasterService } from '../../core/common/master.service';
import { SRIssue } from '../branchissue.model';

import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';
import { Router } from '@angular/router';
declare var $: any;
@Component({
  selector: 'app-opgissue',
  templateUrl: './opgissue.component.html',
  styleUrls: ['./opgissue.component.css']
})
export class OpgissueComponent implements OnInit {
  routerUrl: string = "";
  datePickerConfig: Partial<BsDatepickerConfig>;
  EnableItemDetails: boolean = true;
  EnableFittedStoneDetails: boolean = true;
  EnableStoneDmddtls: boolean = true;
  today = new Date();
  opgissueHeaderForm: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  constructor(private _branchissueService: BranchissueService, private fb: FormBuilder,
    private _appConfigService: AppConfigService, private _masterService: MasterService,
    private _router: Router) {
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
    this.getApplicationDate();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.opgissue.CompanyCode = this.ccode;
    this.opgissue.BranchCode = this.bcode;
  }

  applicationDate: any;
  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
        this.opgissue.FromDate = this.applicationDate;
        this.opgissue.ToDate = this.applicationDate;
      }
    )
  }

  opgissue: SRIssue = {
    CompanyCode: null,
    BranchCode: null,
    IssueTo: null,
    GSCode: null,
    FromDate: null,
    ToDate: null
  }

  opgDetails: any = [];
  LineItems: any = [];
  StoneDetails: any = [];

  Go(form) {
    if (form.value.IssueTo == null) {
      swal("Warning!", 'Please select the IssueTo', "warning");
    }
    else if (form.value.GSCode == null) {
      swal("Warning!", 'Please select the GS', "warning");
    }
    else {
      this._branchissueService.getOPGDetail(this.opgissue).subscribe(
        response => {
          this.opgDetails = response;
          this.LineItems = this.opgDetails.IssueLines;
          this.getStoneDetails()
        }
      )
    }
  }

  ngOnInit() {
    this.routerUrl = this._router.url;
    this.OPGIssueTo();
    this.getGSList();
    this.opgissueHeaderForm = this.fb.group({
      IssueTo: null,
      GSCode: null,
      FromDate: null,
      ToDate: null
    });
  }
  ToggleitemDetails() {
    this.EnableItemDetails = !this.EnableItemDetails;
  }
  ToggleFittedStonesDetails() {
    this.EnableFittedStoneDetails = !this.EnableFittedStoneDetails;
  }
  GSList: any = [];
  getGSList() {
    this._masterService.getNewGsList().subscribe(
      response => {
        this.GSList = response;
      }
    )
  }
  IssueTo: any = [];
  OPGIssueTo() {
    this._branchissueService.getNTIssueTo().subscribe(
      Response => {
        this.IssueTo = Response;
      }
    )
  }

  outputOPGData: any = [];

  PrintTypeBasedOnConfig: any;

  opgIssuePrint: any = [];

  outputXml: any;

  SaveOPGIssue() {
    var ans = confirm("Do you want to submit??");
    if (ans) {
      this._branchissueService.postOPGDetail(this.opgissue).subscribe(
        response => {
          this.outputOPGData = response;
          swal("Saved!", this.outputOPGData.Message, "success");
          this.LineItems = [];
          this.StoneDetails = [];
          this.opgissueHeaderForm.reset();
          this._branchissueService.generateOpgXML(this.outputOPGData.DocumentNo).subscribe(
            response => {
              this.outputXml = response;
              var ans = confirm("Do you want to take print??");
              if (ans) {
                this._branchissueService.getOPGPrint(this.outputOPGData.DocumentNo).subscribe(
                  response => {
                    this.PrintTypeBasedOnConfig = response;
                    if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
                      $('#OPGIssueTab').modal('show');
                      this.opgIssuePrint = atob(this.PrintTypeBasedOnConfig.Data);
                      $('#DisplayopgIssueData').html(this.opgIssuePrint);
                    }
                    else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
                      $('#OPGIssueTab').modal('show');
                      this.opgIssuePrint = this.PrintTypeBasedOnConfig.Data;
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

  cancel() {
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
      () => {
        this._router.navigate(['/branchissue/opgissue']);
      }
    )
  }

  printOpgIssue() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.opgIssuePrint);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printopgIssueData, popupWin;
      printopgIssueData = document.getElementById('DisplayopgIssueData').innerHTML;
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
    ${printopgIssueData}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }

  BRANCH: string = "";

  getStoneDetails() {
    for (let i = 0; i < this.LineItems.length; i++) {
      if (this.LineItems[i].StoneDetails.length > 0) {
        for (let j = 0; j < this.LineItems[i].StoneDetails.length; j++) {
          this.StoneDetails.push(this.LineItems[i].StoneDetails[j]);
        }
      }
    }
  }

  MeltingLoss: number;

  MeltingLossCalc() {
    this.MeltingLoss = 0.00;
    for (let i = 0; i < this.LineItems.length; i++) {
      this.MeltingLoss += parseFloat(this.LineItems[i].OtherLineAttributes.MeltingLoss);
    }
    return this.MeltingLoss;
  }
}