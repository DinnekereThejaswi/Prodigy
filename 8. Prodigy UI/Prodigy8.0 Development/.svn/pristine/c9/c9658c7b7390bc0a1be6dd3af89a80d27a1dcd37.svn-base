import { Component, OnInit } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { BranchissueService } from '../branchissue.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { OrdersService } from '../../orders/orders.service';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { AddBarcodeService } from '../../sales/add-barcode/add-barcode.service';
import { NTIssueModel } from '../branchissue.model'
import { MasterService } from '../../core/common/master.service';
import { AccountsService } from '../../accounts/accounts.service'

import { AppConfigService } from '../../AppConfigService';
import { Router } from '@angular/router';
declare var $: any;

@Component({
  selector: 'app-nt-issue',
  templateUrl: './nt-issue.component.html',
  styleUrls: ['./nt-issue.component.css']
})
export class NtIssueComponent implements OnInit {
  routerUrl: string = "";
  datePickerConfig: Partial<BsDatepickerConfig>;
  EnableItemDetails: boolean = false;
  today = new Date();
  applicationDate
  NTHeaderForm: FormGroup;
  NTSummaryForm: FormGroup;

  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  HeaderModel = {
    IssueTo: null,
    Rate: null
  }

  constructor(private _branchissueService: BranchissueService, private fb: FormBuilder,
    private _appConfigService: AppConfigService, private _router: Router,
    private _ordersService: OrdersService, private barcodeService: AddBarcodeService,
    private _masterService: MasterService, private accountservice: AccountsService) {
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
    this.routerUrl = this._router.url;
    this.getApplicationdate();
    this.Counter();
    this.getGs();
    this.GetIssueTo();
    this.NTHeaderForm = this.fb.group({
      IssueTo: [null, Validators.required],
      Rate: [null, Validators.required],
      remarks: null
    });

    this.NTSummaryForm = this.fb.group({
      WastageGrams: [null, Validators.required],
      MakingChargesRs: [null, Validators.required],
      HallmarkingChargesRs: [null, Validators.required]
    });
  }

  getApplicationdate() {
    this.accountservice.getApplicationDate().subscribe(
      response => {
        this.applicationDate = response;
        this.applicationDate = this.applicationDate.applcationDate;
      }
    )
  }

  public Index: number

  ToggleitemDetails() {
    this.EnableItemDetails = !this.EnableItemDetails;
  }

  sendDataToStoneDiamondComp(form, index: number) {
    if (this.EnableSaveCnlbtn[index] == true) {
      swal("Warning!", 'Please save item details', "warning");
    }
    else {
      this.Index = index === this.Index ? -1 : index;
      this._branchissueService.SendStoneDiamondDetailsFromPurchaseComp(form.StoneDetails);
      this.LinesDetails = form;
    }
  }


  GSList: any = [];

  private newAttribute: NTIssueModel = {
    SlNo: null,
    CounterCode: null,
    GSCode: null,
    ItemCode: null,
    Qty: null,
    GrossWt: null,
    StoneWt: null,
    NetWt: null,
    Dcts: null,
    Rate: null,
    PurityPercent: null,
    PureWeight: null,
    AmountBeforeTax: null,
    CGSTAmount: null,
    SGSTAmount: null,
    IGSTAmount: null,
    AmountAfterTax: null,
    StoneDetails: []
  };

  getGs() {
    this._branchissueService.GSList().subscribe(
      Response => {
        this.GSList = Response;
      }
    )
  }

  IssueTo: any = [];

  GetIssueTo() {
    this._branchissueService.getNTIssueTo().subscribe(
      response => {
        this.IssueTo = response;
      }
    )
  }

  CounterList: any;

  Counter() {
    this._ordersService.getCounter().subscribe(
      response => {
        this.CounterList = response;
      }
    )
  }

  ItemList: any = [];
  Rate: any
  getItemList(counter, gs, index) {
    this._branchissueService.getNTItemList(counter, gs).subscribe(
      response => {
        this.ItemList[index] = response;
        this._branchissueService.getRate(gs).subscribe(
          response => {
            this.Rate = response;
            this.fieldArray[index].Rate = this.Rate.Rate;
          }
        )
      }
    )
  }

  public fieldArray: any = [];

  count: number = 0;

  add() {
    if (this.NTIssuePost.IssueTo == null) {
      swal("Warning!", 'Please select Issue To', "warning");
    }
    else {
      this.addrow();
    }
  }

  EnableAddRow: boolean = false;
  LinesDetails: Array<any> = [];


  NTIssuePost: any = {
    CompanyCode: null,
    BranchCode: null,
    IssueTo: null,
    Remarks: null,
    BoardRate: null,
    WastageGrams: null,
    MakingChargesRs: null,
    HallmarkingChargesRs: null,
    Rate: null,
    IssueLines: []
  }

  diamondcarrat: number = 0;

  receivestoneCarat() {
    this.diamondcarrat = 0;
    for (var field in this.fieldArray) {
      if (this.NTIssuePost.IssueLines[field].StoneDetails.length > 0) {
        for (var i = 0; i < this.NTIssuePost.IssueLines[field].StoneDetails.length; i++) {
          if (this.NTIssuePost.IssueLines[field].StoneDetails[i].Type == "DMD") {
            this.diamondcarrat += Number(this.NTIssuePost.IssueLines[field].StoneDetails[i].Carrat);
          }
        }
      }
      else {
        this.diamondcarrat = 0;
      }

      this.fieldArray[field].Dcts = this.diamondcarrat;
    }
  }


  addrow() {
    this.newAttribute = {
      SlNo: null,
      CounterCode: null,
      GSCode: null,
      ItemCode: null,
      Qty: null,
      GrossWt: null,
      StoneWt: null,
      NetWt: null,
      Dcts: null,
      Rate: null,
      PurityPercent: null,
      PureWeight: null,
      AmountBeforeTax: null,
      CGSTAmount: null,
      SGSTAmount: null,
      IGSTAmount: null,
      AmountAfterTax: null,
      StoneDetails: []
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

  EnableEditDelbtn = {};
  EnableSaveCnlbtn = {};
  readonly = {};

  calcNwtByGwt(index) {
    this.fieldArray[index].NetWt = this.fieldArray[index].GrossWt;
  }
  calcNwtBySwt(index) {
    this.fieldArray[index].NetWt = this.fieldArray[index].GrossWt - this.fieldArray[index].StoneWt;
  }

  calcPurWt(index) {
    this.fieldArray[index].PureWeight = Number(this.fieldArray[index].NetWt * (this.fieldArray[index].PurityPercent / 100)).toFixed(3);
  }

  saveDataFieldValue(index) {
    if (this.fieldArray[index]["GSCode"] == null || this.fieldArray[index]["GSCode"] == 0) {
      swal("Warning!", 'Please select the GS', "warning");
    }
    else if (this.fieldArray[index]["ItemCode"] == null || this.fieldArray[index]["ItemCode"] == 0) {
      swal("Warning!", 'Please select the Item', "warning");
    }
    else if (this.fieldArray[index]["GrossWt"] == null || this.fieldArray[index]["GrossWt"] == 0) {
      swal("Warning!", 'Please enter the Gross Wt', "warning");
    }
    else if (this.fieldArray[index]["PurityPercent"] == null || this.fieldArray[index]["PurityPercent"] == 0) {
      swal("Warning!", 'Please enter the Purity Percent', "warning");
    }
    else if (this.fieldArray[index]["Rate"] == null || this.fieldArray[index]["Rate"] == 0) {
      swal("Warning!", 'Please enter the Rate', "warning");
    }
    else {
      this.EnableEditDelbtn[index] = true;
      this.EnableSaveCnlbtn[index] = false;
      this.readonly[index] = true;
      this.EnableAddRow = false;
      this.NTIssuePost.IssueLines[index] = this.fieldArray[index];
    }
  }

  NTIssueOutput: any = [];
  NTIssuePrint: any = [];

  counterStock: any = [];

  CounterStock(arg) {
    this._branchissueService.getCounterStock(arg).subscribe(
      response => {
        this.counterStock = response;
      }
    )
  }

  PrintTypeBasedOnConfig: any;
  outputXml: any;

  SaveNTIssue() {
    if (this.NTIssuePost.IssueTo == null) {
      swal("Warning!", 'Please select Issue To', "warning");
    }
    // else if (this.NTIssuePost.Rate == null || this.NTIssuePost.Rate == "") {
    //   swal("Warning!", 'Please enter valid Rate/Gram', "warning");
    // }
    else if (this.NTIssuePost.IssueLines.length == 0) {
      swal("Warning!", 'Please enter item details to save', "warning");
    }
    else if (Number(this.NTIssuePost.WastageGrams) > this.NetWt()) {
      swal("Warning!", 'Wastage is exceeding the Net weight. Please Check.', "warning");
    }
    else {
      var ans = confirm("Do you want to submit??");
      if (ans) {
        this.NTIssuePost.CompanyCode = this.ccode;
        this.NTIssuePost.BranchCode = this.bcode;
        this._branchissueService.postNTIssue(this.NTIssuePost).subscribe(
          response => {
            this.NTIssueOutput = response;
            swal("Saved!", this.NTIssueOutput.Message, "success");
            this.NTHeaderForm.reset();
            this.NTSummaryForm.reset();
            this.NTIssuePost = {
              CompanyCode: null,
              BranchCode: null,
              IssueTo: null,
              Remarks: null,
              BoardRate: null,
              WastageGrams: null,
              MakingChargesRs: null,
              HallmarkingChargesRs: null,
              IssueLines: []
            }
            this.fieldArray = [];
            this._branchissueService.generateNTIssueXML(this.NTIssueOutput.DocumentNo).subscribe(
              response => {
                this.outputXml = response;
                var ans = confirm("Do you want to take print??");
                if (ans) {
                  this._branchissueService.getBarcodePrint(this.NTIssueOutput.DocumentNo).subscribe(
                    response => {
                      this.PrintTypeBasedOnConfig = response;
                      if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
                        $('#NTIssueTab').modal('show');
                        this.NTIssuePrint = atob(this.PrintTypeBasedOnConfig.Data);
                        $('#DisplayntIssueData').html(this.NTIssuePrint);
                      }
                      else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
                        $('#NTIssueTab').modal('show');
                        this.NTIssuePrint = this.PrintTypeBasedOnConfig.Data;
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

  printNtIssue() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.NTIssuePrint);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printntIssueData, popupWin;
      printntIssueData = document.getElementById('DisplayntIssueData').innerHTML;
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

  GrossWt() {
    let total: any = 0.00; this.fieldArray.forEach((d) => {
      if (d.GrossWt != null) {
        total += parseFloat(d.GrossWt);
      }
    });
    return total;
  }
  StoneWt() {
    let total: any = 0.00; this.fieldArray.forEach((d) => {
      if (d.StoneWt != null) {
        total += parseFloat(d.StoneWt);
      }
    });
    return total;
  }
  PureWt() {
    let total: any = 0.00; this.fieldArray.forEach((d) => {
      if (d.PureWeight != null) {
        total += parseFloat(d.PureWeight);
      }
    });
    return total;
  }
  NetWt() {
    let total: any = 0.00; this.fieldArray.forEach((d) => {
      if (d.NetWt != null) {
        total += parseFloat(d.NetWt);
      }
    });
    return total;
  }
  Dcts() {
    let total: any = 0.00; this.fieldArray.forEach((d) => {
      if (d.Dcts != null) {
        total += parseFloat(d.Dcts);
      }
    });
    return total;
  }
  Amount() {
    let total: any = 0.00; this.fieldArray.forEach((d) => {
      if (d.Rate != null) {
        total += parseFloat(d.Rate);
      }
    });
    return total;
  }

  cancelDataFieldValue(index) {
    this.EnableAddRow = false;
    this.fieldArray.splice(index, 1);
    this.NTIssuePost.IssueLines.splice(index, 1);
  }

  deleteFieldValue(index) {
    if (this.EnableAddRow == true) {
      swal("Warning!", 'Please save the enabled item', "warning");
    }
    else {
      var ans = confirm("Do you want to delete??");
      if (ans) {
        //this.valueChanged(this.fieldArray[index].ItemAmount);
        this.fieldArray.splice(index, 1);
        this.NTIssuePost.IssueLines = [];
        if (this.fieldArray.length > 0) {
          this.NTIssuePost.IssueLines = this.fieldArray;
        }
      }
    }
  }

  validateWastageGms() {
    if (this.NTIssuePost.WastageGrams != "" && this.NTIssuePost.WastageGrams != null) {
      if (Number(this.NTIssuePost.WastageGrams) > this.NetWt()) {
        swal("Warning!", 'Wastage is exceeding the Net weight. Please Check.', "warning");
      }
    }
  }

  cancel() {
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
      () => {
        this._router.navigate(['/branchissue/nt-issue']);
      }
    )
  }
}