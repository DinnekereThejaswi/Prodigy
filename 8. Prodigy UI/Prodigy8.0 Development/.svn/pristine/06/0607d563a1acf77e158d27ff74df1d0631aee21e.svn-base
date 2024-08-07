import { Component, OnInit } from '@angular/core';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { Router } from '@angular/router';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { BranchreceiptsService } from '../branchreceipts.service';
import { BranchissueService } from '../../branchissue/branchissue.service';
import { AppConfigService } from '../../AppConfigService';
import { MasterService } from '../../core/common/master.service';
import { NTReceiptModel } from '../branchreceipts.model';
import { OrdersService } from '../../orders/orders.service';
import { FormBuilder, FormGroup } from '@angular/forms';

declare var $: any;

@Component({
  selector: 'app-nontagreceipts',
  templateUrl: './nontagreceipts.component.html',
  styleUrls: ['./nontagreceipts.component.css']
})
export class NontagreceiptsComponent implements OnInit {
  routerUrl: string = "";
  NonbarcodereceiptHeaderForm: FormGroup;
  // radioItems: Array<string>;
  // model = { option: 'Ornaments Receipts' };
  EnableItemDetails: boolean = false;
  readonlyBranch: boolean = false;
  IssueTo: any = [];
  receivedtype: any = [];
  applicationDate: any;
  ccode: string;
  bcode: string;
  password: string;
  CounterList: any;
  GSList: any = [];
  EnableJson: boolean = false;
  datePickerConfig: Partial<BsDatepickerConfig>;
  today = new Date();
  EnableAddRow: boolean = false;

  constructor(private _branchreceiptsService: BranchreceiptsService,
    private _masterService: MasterService, private _branchissueService: BranchissueService,
    private _ordersService: OrdersService, private _appConfigService: AppConfigService,
    private _router: Router,private fb :FormBuilder) {
    // this.radioItems = ['Ornaments Receipts', 'Old Entry'];
    this.receivedtype = [
      {
        "Name": "MANUFACTURING",
        "Value": "MANUFACTURING"
      },
      {
        "Name": "BRANCH",
        "Value": "BRANCH"
      }
    ]
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


  public fieldArray: any = [];

  count: number = 0;

  EnableEditDelbtn = {};
  EnableSaveCnlbtn = {};
  readonly = {};

  NTReceiptPost: any = {
    CompanyCode: null,
    BranchCode: null,
    IssueBranchCode: null,
    IssueNo: null,
    IssueDate: null,
    TotalLineCount: null,
    TotalQty: null,
    TotalGrossWt: null,
    TotalStoneWt: null,
    TotalNetWt: null,
    TotalDcts: null,
    TotalAmount: null,
    ReceiptDetails: [],
    ScannedBarcodes: []
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.NTReceiptPost.CompanyCode = this.ccode;
    this.NTReceiptPost.BranchCode = this.bcode;
  }

  ngOnInit() {
    this.NTReceiptPost.IssueBranchCode=null;
    this.routerUrl = this._router.url;
    this.NonbarcodereceiptHeaderForm = this.fb.group({
      frmCtrl_ReceiptFrom: null,
      frmCtrl_IssueNo: null,
      frmCtrl_ReceiptDate: null,
      frmCtrl_Receipttype: null,

    });
    this.getIssueTo();
    this.getApplicationDate();
    this.Counter();
    this.getGs();
  }

  getGs() {
    this._branchissueService.GSList().subscribe(
      Response => {
        this.GSList = Response;
      }
    )
  }

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
        this.readonlyBranch=true
        this._branchissueService.getRate(gs).subscribe(
          response => {
            this.Rate = response;
            this.fieldArray[index].Rate = this.Rate.Rate;
          }
        )
      }
    )
  }

  calcNwtByGwt(index) {
    this.fieldArray[index].NetWt = this.fieldArray[index].GrossWt;
    this.calcAmt(index);
  }
  calcNwtBySwt(index) {
    this.fieldArray[index].NetWt = this.fieldArray[index].GrossWt - this.fieldArray[index].StoneWt;
    this.calcAmt(index);
  }

  calcPurWt(index) {
    this.fieldArray[index].PureWeight = this.fieldArray[index].NetWt * (this.fieldArray[index].PurityPercent / 100);
  }

  calcAmt(index) {
    this.fieldArray[index].Amount = this.fieldArray[index].NetWt * this.fieldArray[index].Rate;
  }

  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
      }
    )
  }

  getIssueTo() {
    this._branchreceiptsService.GetNTReceiptFrom().subscribe(
      response => {
        this.IssueTo = response;
      }
    )
  }

  ToggleitemDetails() {
    this.EnableItemDetails = !this.EnableItemDetails;
  }

  private newAttribute: NTReceiptModel = {
    GSCode: null,
    ItemCode: null,
    BarcodeNo: null,
    CounterCode: null,
    SlNo: null,
    Qty: null,
    GrossWt: null,
    StoneWt: null,
    NetWt: null,
    Dcts: null,
    PurityPercent: null,
    PureWeight: null,
    Rate: null,
    Amount: null,
    BarcodeReceiptStoneDetails: []
  };

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
      ItemCode: null,
      BarcodeNo: null,
      CounterCode: null,
      SlNo: null,
      Qty: null,
      GrossWt: null,
      StoneWt: null,
      NetWt: null,
      Dcts: null,
      PurityPercent: null,
      PureWeight: null,
      Rate: null,
      Amount: null,
      BarcodeReceiptStoneDetails: []
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
      this.NTReceiptPost.ReceiptDetails[index] = this.fieldArray[index];
    }
  }

  NTReceiptOutput: any = [];
  PrintTypeBasedOnConfig: any;
  NTreceiptPrint: any = [];

  SaveNTReceipt() {
    if (this.NTReceiptPost.IssueBranchCode == null) {
      swal("Warning!", 'Please select the Receipt From', "warning");
    }
    else if (this.NTReceiptPost.IssueNo == null) {
      swal("Warning!", 'Please enter the Issue No', "warning");
    }
    else {
      var ans = confirm("Do you want to submit??");
      if (ans) {
        this._branchreceiptsService.postNTReceipt(this.NTReceiptPost).subscribe(
          response => {
            this.NTReceiptOutput = response;
            swal("Saved!", this.NTReceiptOutput.Message, "success");
            this.NTReceiptPost = {
              CompanyCode: null,
              BranchCode: null,
              IssueBranchCode: null,
              IssueNo: null,
              IssueDate: null,
              TotalLineCount: null,
              TotalQty: null,
              TotalGrossWt: null,
              TotalStoneWt: null,
              TotalNetWt: null,
              TotalDcts: null,
              TotalAmount: null,
              ReceiptDetails: [],
              ScannedBarcodes: []
            }
            this.fieldArray = [];
            this._branchreceiptsService.getBarcodeReceiptPrint(this.NTReceiptOutput.DocumentNo).subscribe(
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
  }
  
  Reset(){
this.readonlyBranch=false;
      this.ItemList =[];
      this.NTReceiptPost = {
        CompanyCode: null,
        BranchCode: null,
        IssueBranchCode: null,
        IssueNo: null,
        IssueDate: null,
        TotalLineCount: null,
        TotalQty: null,
        TotalGrossWt: null,
        TotalStoneWt: null,
        TotalNetWt: null,
        TotalDcts: null,
        TotalAmount: null,
        ReceiptDetails: [],
        ScannedBarcodes: []
      }
      this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
        () => {
          this._router.navigate(['/branchreceipts/nontagreceipts']);
          
        }
      )

    

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

  cancelDataFieldValue(index) {
    this.EnableAddRow = false;
    if (this.CopyEditedRow[index] != null) {
      this.fieldArray[index] = this.CopyEditedRow[index];
      this.NTReceiptPost.ReceiptDetails[index] = this.CopyEditedRow[index];
      this.CopyEditedRow[index] = null;
      this.EnableSaveCnlbtn[index] = false;
      this.EnableEditDelbtn[index] = true;
      this.readonly[index] = true;
      this.CopyEditedRow = [];
    }
    else {
      this.fieldArray.splice(index, 1);
      this.NTReceiptPost.ReceiptDetails.splice(index, 1);
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


  deleteFieldValue(index) {
    if (this.EnableAddRow == true) {
      swal("Warning!", 'Please save the enabled item', "warning");
    }
    else {
      var ans = confirm("Do you want to delete??");
      if (ans) {
        this.fieldArray.splice(index, 1);
        this.NTReceiptPost.ReceiptDetails = [];
        if (this.fieldArray.length > 0) {
          this.NTReceiptPost.ReceiptDetails = this.fieldArray;
        }
      }
    }
  }

  issueDetail: any = [];


  go() {
    if (this.NTReceiptPost.IssueBranchCode == null || this.NTReceiptPost.IssueBranchCode == "") {
      swal("Warning!", 'Please select the Receipt From', "warning");
    }
   
    else if (this.NTReceiptPost.IssueNo == null || this.NTReceiptPost.IssueNo == "") {
      swal("Warning!", 'Please enter the Issue No', "warning");
    }
    else {
      this._branchreceiptsService.GetNTReceiptIssueDetail(this.NTReceiptPost).subscribe(
        response => {
          this.issueDetail = response;
          this.NonbarcodereceiptHeaderForm.get('frmCtrl_ReceiptFrom').disable();
          this.NonbarcodereceiptHeaderForm.get('frmCtrl_IssueNo').disable();
          this.NonbarcodereceiptHeaderForm.get('frmCtrl_Receipttype').disable();
          this.NTReceiptPost = this.issueDetail;
          this.fieldArray = this.NTReceiptPost.ReceiptDetails;
          for (let i = 0; i < this.fieldArray.length; i++) {
            this._branchissueService.getNTItemList(this.fieldArray[i].CounterCode, this.fieldArray[i].GSCode).subscribe(
              response => {
                this.ItemList[i] = response;
                this.EnableEditDelbtn[i] = true;
                this.EnableSaveCnlbtn[i] = false;
                this.readonly[i] = true;
                this.EnableAddRow = false;
              }
            )
          }
        }
      )
    }
  }

  public Index: number;
  LinesDetails: Array<any> = [];


  sendDataToStoneDiamondComp(form, index: number) {
    if (this.EnableSaveCnlbtn[index] == true) {
      swal("Warning!", 'Please save item details', "warning");
    }
    else {
      this.Index = index === this.Index ? -1 : index;
      this._branchreceiptsService.SendStoneDiamondDetailsFromPurchaseComp(form.BarcodeReceiptStoneDetails);
      this.LinesDetails = form;
    }
  }

  diamondcarrat: number = 0;

  receivestoneCarat() {
    this.diamondcarrat = 0;
    for (var field in this.fieldArray) {
      if (this.NTReceiptPost.ReceiptDetails[field].BarcodeReceiptStoneDetails.length > 0) {
        for (var i = 0; i < this.NTReceiptPost.ReceiptDetails[field].BarcodeReceiptStoneDetails.length; i++) {
          if (this.NTReceiptPost.ReceiptDetails[field].BarcodeReceiptStoneDetails[i].Type == "DMD") {
            this.diamondcarrat += Number(this.NTReceiptPost.ReceiptDetails[field].BarcodeReceiptStoneDetails[i].Carrat);
          }
        }
      }
      else {
        this.diamondcarrat = 0;
      }

      this.fieldArray[field].Dcts = this.diamondcarrat;
    }
  }

  Amount() {
    let total: any = 0.00; this.fieldArray.forEach((d) => {
      if (d.Amount != null) {
        total += parseFloat(d.Amount);
      }
    });
    total = Math.round(total);
    return total;
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

  NetWt() {
    let total: any = 0.00; this.fieldArray.forEach((d) => {
      if (d.NetWt != null) {
        total += parseFloat(d.NetWt);
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

  DctsWt() {
    let total: any = 0.00; this.fieldArray.forEach((d) => {
      if (d.Dcts != null) {
        total += parseFloat(d.Dcts);
      }
    });
    return total;
  }




  cancel() {
    this.NonbarcodereceiptHeaderForm.get('frmCtrl_ReceiptFrom').enable();
    this.NonbarcodereceiptHeaderForm.get('frmCtrl_IssueNo').enable();
    this.NonbarcodereceiptHeaderForm.get('frmCtrl_Receipttype').enable();
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
      () => {
        this._router.navigate(['/branchreceipts/nontagreceipts']);
        
      }
    )
  }
}