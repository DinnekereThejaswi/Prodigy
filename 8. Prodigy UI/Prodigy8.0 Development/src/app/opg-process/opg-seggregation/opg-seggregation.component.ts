import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { OpgprocessService } from '../opg-process.service';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { formatDate } from '@angular/common';
import { Alert } from 'bootstrap';
import { MasterService } from '../../core/common/master.service';
import { OPGSeggregationModel, OPGStoneModel } from '../opg-process.model';
import { Router } from '@angular/router';
declare var $: any;

@Component({
  selector: 'app-opg-seggregation',
  templateUrl: './opg-seggregation.component.html',
  styleUrls: ['./opg-seggregation.component.css']
})

export class OpgSeggregationComponent implements OnInit {
  routerUrl: string = "";
  opgSeggregationDetail: any = [];
  fieldArray: any = [];
  CollapseInvoiceDetailsTab: boolean = true;
  CollapseStoneDetailsTab: boolean = true;
  CollapseTotalDetailsTab: boolean = true;
  CollapseSeggragationDetailsTab: boolean = true;
  opgSeggregationHeaderForm: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  isReadOnly: boolean = false;
  EnableJson: boolean = false;
  EnableSummary: boolean = false;
  datePickerConfig: Partial<BsDatepickerConfig>;
  today = new Date();
  ///date
  fromDate = new Date();
  Todate = new Date();
  applicationDate: any;
  EnableAddRow: boolean = false;
  count: number = 0;
  EnableEditDelbtn = {};
  EnableSaveCnlbtn = {};
  readonly = {};

  oldstone: any = [
    {
      "Name": "OLD STONE",
      "Code": "OS"
    }
  ]

  olddiamond: any = [
    {
      "Name": "OLD DIAMOND",
      "Code": "OD"
    }
  ]

  SummaryModel = {
    OpgTotalGwt: null,
    TotalDcts: null,
    totalSwt: null,
    toleranceGwt: null,
    SegregatedTotalWt: null,
    SegregatedTotalDcts: null,
    SegragatedTotalTotalSwt: null,
    PendingWt: null,
    PendingDcts: null,
    PendingSwt: null,
    LastNo: null,
    Remarks: null
  }


  private newAttribute: OPGSeggregationModel = {
    GSCode: null,
    GrossWeight: null,
    StoneGSCode: null,
    StoneWeight: null,
    DiamondGSCode: null,
    DiamondCaretWeight: null,
    NetWt: null,
    PurityPercent: null,
    PureWeight: null
  };

  constructor(private fb: FormBuilder, private _appConfigService: AppConfigService,
    private _opgProcessService: OpgprocessService,
    private _masterService: MasterService,
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
  }

  opgSeggregation: any = {
    CompanyCode: null,
    BranchCode: null,
    GSCode: null,
    FromDate: null,
    ToDate: null,
    SalesmanCode: null,
    OPGSeparationInputLines: []
  }

  getCB() {
    this.routerUrl = this._router.url;
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.opgSeggregation.CompanyCode = this.ccode;
    this.opgSeggregation.BranchCode = this.bcode;
  }

  ngOnInit() {
    this.routerUrl = this._router.url;
    this.getApplicationdate();
    this.getGSList();
    this.getSalesMan();
    this.getItemGSList();
    this.opgSeggregationHeaderForm = this.fb.group({
      SalesmanCode: null,
      GSCode: null,
      FromDate: null,
      ToDate: null
    });
  }

  add() {
    // if (this.NTIssuePost.IssueTo == null) {
    //   swal("Warning!", 'Please select Issue To', "warning");
    // }
    // else {
    this.addrow();
    //}
  }

  addrow() {
    this.newAttribute = {
      GSCode: null,
      GrossWeight: null,
      StoneGSCode: null,
      StoneWeight: null,
      DiamondGSCode: null,
      DiamondCaretWeight: null,
      NetWt: null,
      PurityPercent: null,
      PureWeight: null
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

  ToggleInvoiceDetails() {
    this.CollapseInvoiceDetailsTab = !this.CollapseInvoiceDetailsTab;
  }

  ToggleStoneDetails() {
    this.CollapseStoneDetailsTab = !this.CollapseStoneDetailsTab;
  }

  ToggleTotalDetails() {
    this.CollapseTotalDetailsTab = !this.CollapseTotalDetailsTab;
  }

  ToggleSeggragationDetails() {
    this.CollapseSeggragationDetailsTab = !this.CollapseSeggragationDetailsTab;
  }

  getApplicationdate() {
    this._opgProcessService.getApplicationDate().subscribe(
      response => {
        this.applicationDate = response;
        this.applicationDate = this.applicationDate["applcationDate"];
        // this.applicationDate = formatDate(this.applicationDate, 'dd/MM/yyyy', 'en_GB');
        this.opgSeggregation.FromDate = this.applicationDate;
        this.opgSeggregation.ToDate = this.applicationDate;
      }
    )
  }

  GSList: any = [];
  getGSList() {
    this._masterService.getNewGsList().subscribe(
      response => {
        this.GSList = response;
      }
    )
  }

  ItemGSList: any = [];
  getItemGSList() {
    this._opgProcessService.getOPGItemGS().subscribe(
      response => {
        this.ItemGSList = response;
      }
    )
  }

  SalesManList: any;
  getSalesMan() {
    this._masterService.getSalesMan().subscribe(
      response => {
        this.SalesManList = response;
      })
  }



  opgInvoiceDetails: any = [];
  opgStoneDetails: OPGStoneModel = {
    SlNo: 0,
    BillNo: 0,
    GS: null,
    Name: null,
    Qty: null,
    Carrat: null,
    Rate: null,
    Amount: null
  }

  opgStoneFinalDets: any = [];
  Summary: any = [];
  Go() {
    this.Summary = [];
    this.opgSeggregationDetail=[];
    if (this.opgSeggregation.GSCode == null) {
      swal("Warning!", 'Please select the GS', "warning");
    }
    else {
      this.opgSeggregationDetail = [];
      this._opgProcessService.getOPGSeparationDetail(this.opgSeggregation).subscribe(
        response => {
          this.opgSeggregationDetail = response;
          this.laodAllTotal(this.opgSeggregationDetail);
          this.SummaryModel.OpgTotalGwt = this.opgSeggregationDetail.GrossWt;
          this.SummaryModel.totalSwt = this.opgSeggregationDetail.StoneWt;
          this.SummaryModel.TotalDcts = this.opgSeggregationDetail.Dcts;
          this.SummaryModel.toleranceGwt = this.opgSeggregationDetail.ToleranceWt;
          this.SummaryModel.PendingWt = this.opgSeggregationDetail.GrossWt;
          this.SummaryModel.PendingSwt = this.opgSeggregationDetail.StoneWt;
          this.SummaryModel.PendingDcts = this.opgSeggregationDetail.Dcts;
          this.Summary.push(this.SummaryModel);
          this.opgInvoiceDetails = this.opgSeggregationDetail.LineDetails;
          this.EnableSummary = true;
          this.getStoneDetails();
        }
      )
    }
  }
  name: string;
  GetName(arg) {
    this.name = arg;
  }
  Totals: any = [];
  laodAllTotal(arg) {
    this.Totals = [];
    this.Totals.push(arg);
    this.CollapseTotalDetailsTab = false;
  }

  getStoneDetails() {
    if (this.opgInvoiceDetails.length == 0) {
      //this.opgStoneDetails = [];
      this.opgStoneDetails = {
        SlNo: 0,
        BillNo: 0,
        GS: null,
        Name: null,
        Qty: null,
        Carrat: null,
        Rate: null,
        Amount: null
      }
    }
    else {
      for (let i = 0; i < this.opgInvoiceDetails.length; i++) {
        if (this.opgInvoiceDetails[i].StoneDetails.length > 0) {
          for (let j = 0; j < this.opgInvoiceDetails[i].StoneDetails.length; j++) {
            this.opgStoneDetails = this.opgInvoiceDetails[i].StoneDetails[j];
            this.opgStoneDetails.BillNo = this.opgInvoiceDetails[i].BillNo;
            this.opgStoneFinalDets.push(this.opgStoneDetails);
            this.opgStoneDetails = {
              SlNo: 0,
              BillNo: 0,
              GS: null,
              Name: null,
              Qty: null,
              Carrat: null,
              Rate: null,
              Amount: null
            }
          }
        }
      }
    }
  }

  cancel() {
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
      () => {
        this._router.navigate(['/opg-process/opg-seggregation']);
      }
    )
  }
  calculations: any = [];
  saveDataFieldValue(index) {
    if (this.fieldArray[index]["GSCode"] == null || this.fieldArray[index]["GSCode"] == 0) {
      swal("Warning!", 'Please select the GS', "warning");
    }
    else if (this.fieldArray[index]["GrossWeight"] == null || this.fieldArray[index]["GrossWeight"] == 0) {
      swal("Warning!", 'Please enter the Gross Weight', "warning");
    }
    else if (this.fieldArray[index]["PurityPercent"] == null || this.fieldArray[index]["ItemCode"] == 0) {
      swal("Warning!", 'Please enter the Purity Percent', "warning");
    }
    else {
      this.EnableEditDelbtn[index] = true;
      this.EnableSaveCnlbtn[index] = false;
      this.readonly[index] = true;
      this.EnableAddRow = false;
      this.opgSeggregation.OPGSeparationInputLines[index] = this.fieldArray[index];
      // this.calculations.push(this.opgSeggregation.OPGSeparationInputLines[index]);
      // this.grossWT(this.calculations);


    }
  }

  cancelDataFieldValue(index) {
    this.EnableAddRow = false;
    this.fieldArray.splice(index, 1);
    this.opgSeggregation.OPGSeparationInputLines.splice(index, 1);
  }

  deleteFieldValue(index) {
    if (this.EnableAddRow == true) {
      swal("Warning!", 'Please save the enabled item', "warning");
    }
    else {
      var ans = confirm("Do you want to delete??");
      if (ans) {
        this.fieldArray.splice(index, 1);
        this.opgSeggregation.OPGSeparationInputLines = [];
        if (this.fieldArray.length > 0) {
          this.opgSeggregation.OPGSeparationInputLines = this.fieldArray;

        }
      }
    }
  }

  calcPurWt(index) {
    this.fieldArray[index].PureWeight = this.fieldArray[index].NetWt * (this.fieldArray[index].PurityPercent / 100);
  }

  calcNwtByGwt(index) {
    this.fieldArray[index].NetWt = this.fieldArray[index].GrossWeight;
    this.SummaryModel.PendingWt = Number(this.SummaryModel.PendingWt - this.fieldArray[index].GrossWeight);

  }
  calcNwtBySwt(index) {
    this.fieldArray[index].NetWt = this.fieldArray[index].GrossWeight - this.fieldArray[index].StoneWeight;
    this.SummaryModel.PendingSwt = Number(this.SummaryModel.PendingSwt - this.fieldArray[index].StoneWeight);
  }
  calculateDMDweight(index) {
    this.SummaryModel.PendingDcts = Number(this.SummaryModel.PendingDcts - this.fieldArray[index].DiamondCaretWeight);
  }


  PendingWt: number = 0;
  PendingDcts: number = 0;
  PendingSwt: number = 0;

  grossWT(arg) {
    let totalGrossWeight = 0;
    arg.forEach((d) => {
      totalGrossWeight += Number(d.GrossWeight);
    });
    this.PendingWt = Number(this.SummaryModel.OpgTotalGwt) - totalGrossWeight;
    return totalGrossWeight;
  }

  stoneWt(arg) {
    let totalStoneWeight = 0;
    arg.forEach((d) => {
      totalStoneWeight += Number(d.StoneWeight);
    });
    this.PendingSwt = Number(this.SummaryModel.totalSwt) - totalStoneWeight;
    return totalStoneWeight;
  }

  dcts(arg) {
    let totalDiamondCaretWeight = 0;
    arg.forEach((d) => {
      totalDiamondCaretWeight += Number(d.DiamondCaretWeight);
    });
    this.PendingDcts = Number(this.SummaryModel.TotalDcts) - totalDiamondCaretWeight;
    return totalDiamondCaretWeight;
  }


  outputOPGSeggregation: any;
  PrintTypeBasedOnConfig: any;
  OPGSeggregationPrint: any = [];

  //handled confirmation validation 
  saveOPGSeggreation() {
    if (this.opgSeggregation.SalesmanCode == null) {
      swal("Warning!", 'Please select the Authorized By', "warning");
    }
    else if (this.opgSeggregation.OPGSeparationInputLines.length == 0) {
      swal("Warning!", 'Please enter the seggregation details', "warning");
    }
    else {
     var ans = confirm("Do you want to save??");
     if (ans) {
      this._opgProcessService.postOPGSeperation(this.opgSeggregation).subscribe(
        response => {
          this.outputOPGSeggregation = response;
          swal("Saved!", this.outputOPGSeggregation.Message, "success");
          this.opgSeggregationHeaderForm.reset();
          this.opgSeggregation = {
            CompanyCode: null,
            BranchCode: null,
            GSCode: null,
            FromDate: null,
            ToDate: null,
            SalesmanCode: null,
            OPGSeparationInputLines: []
          }
          this.fieldArray = [];
          this.opgInvoiceDetails = [];
          var ans = confirm("Do you want to take print??");
          if (ans) {
            this._opgProcessService.printOPGSeperation(this.outputOPGSeggregation.DocumentNo).subscribe(
              response => {
                this.PrintTypeBasedOnConfig = response;
                if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
                  $('#OPGSeggregationTab').modal('show');
                  this.OPGSeggregationPrint = atob(this.PrintTypeBasedOnConfig.Data);
                  $('#DisplayOPGSeggregationData').html(this.OPGSeggregationPrint);
                }
                else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
                  $('#OPGSeggregationTab').modal('show');
                  this.OPGSeggregationPrint = this.PrintTypeBasedOnConfig.Data;
                }
              }
            )
          }
        }
      )
     }
    }
  }

  printOPGSeggregation() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.OPGSeggregationPrint);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printopgSeggregationData, popupWin;
      printopgSeggregationData = document.getElementById('DisplayOPGSeggregationData').innerHTML;
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
    ${printopgSeggregationData}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }
}