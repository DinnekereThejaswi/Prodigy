import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import swal from 'sweetalert';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import { AccountsService } from '../accounts.service';
import { cashbackModal } from '../accounts.model';
declare var $: any;
@Component({
  selector: 'app-cashback',
  templateUrl: './cashback.component.html',
  styleUrls: ['./cashback.component.css']
})
export class CashbackComponent implements OnInit {
  @ViewChild("billNO", { static: true }) billNO: ElementRef;
  EnableJson: boolean = false;
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;


  cashbackModal = {
    CompanyCode: this.ccode,
    BranchCode: this.bcode,
    BillNo: null,
    BillAmt: null,
    EligableCashBack: null,
    ActualCashBack: null,
    Remarks: ""
  }
  cashbackPostModal: cashbackModal = {
    CompanyCode: this.ccode,
    BranchCode: this.bcode,
    TotalBillAmount: 0,
    TotalEligableCashBackAmount: 0,
    TotalActualCashBackAmount: null,
    OperatorCode: "",
    IsEPayment: false,
    ListCashBackVM: [
      {
        CompanyCode: "",
        BranchCode: "",
        BillNo: 0,
        BillAmt: 0,
        EligableCashBack: 0,
        ActualCashBack: 0,
        Remarks: ""
      }
    ]

  }

  constructor(private _accountsService: AccountsService, private formBuilder: FormBuilder,
    private appConfigService: AppConfigService
  ) {
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.EnableJson = this.appConfigService.EnableJson;
    this.password = this.appConfigService.Pwd;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }


  ngOnInit() {
    this.billNO.nativeElement.focus();
    this.cashbackPostModal.IsEPayment = false;
  }
  cashbackListData: any = [];
  cashbackList: any;
  abc: any = [];
  ActualCashBack = 0
  EligableCashBack
  BillAmt = 0

  checkduplicatebillno(arg) {
    if (arg != null) {

      let data = this.cashbackListData.find(x => x.BillNo == arg);
      if (data == null) {
        this._accountsService.cashbackList(arg).subscribe(
          data => {
            this.cashbackList = data;
            this.cashbackListData.push(this.cashbackList)
            if (this.cashbackListData.length != 0 && this.cashbackListData.length >= 0) {
              this.billNO.nativeElement.focus();
              this.billNO.nativeElement.value = "";
            }
          }, (err) => {
            if (err.status === 400) {
              const validationError = err.error.description;
              swal("Warning!", validationError, "warning");
              this.billNO.nativeElement.focus();
            }
            else {
              this.errors.push('something went wrong!');
            }
          }
        )
      }
      else {
        if (this.cashbackListData.length >= 1) {
          swal("Warning!", "Bill No already added", "warning");
          this.billNO.nativeElement.focus();
        }
      }
    }

  }
  CalulatedBillamount(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => { total += d.BillAmt; });
    this.cashbackListData.BillAmt = total;
    return total;
  }


  CalulatedActualCashBack(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => { total += d.EligableCashBack; });
    this.cashbackListData.EligableCashBack = total;
    return total;
  }
  CalulatedEligableCashBack(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => { total += d.ActualCashBack; });
    this.cashbackListData.ActualCashBack = total;
    return total;
  }
  getcahbacklistItems(arg) {
    if (this.billNO.nativeElement.value = null) {
      swal("!Warning", "Please enter  Bill No", "warning");
    }
    this.checkduplicatebillno(arg);


  }
  Delete(i) {
    this.cashbackListData.splice(i, 1);

  }
  UpdateActualCashbackAmt(arg, index, amount) {
    this.cashbackListData[index].ActualCashBack = 0;
    this.cashbackListData[index].ActualCashBack = Number(amount);
    this.CalulatedEligableCashBack();
    this.CalulatedActualCashBack();
    this.CalulatedBillamount();

  }
  updateremarks(index, usertext) {
    this.cashbackListData[index].Remarks = usertext;

  }
  result: any = [];
  errors = [];
  postCashback() {
    this.cashbackPostModal.ListCashBackVM = this.cashbackListData;
    let FinalBillAmount: number = this.cashbackListData.map(a => a.BillAmt).reduce(function (a, b) {
      return a + b;
    });
    let FinalEligableCashBackAmount: number = this.cashbackListData.map(a => a.EligableCashBack).reduce(function (a, b) {
      return a + b;
    });
    let FinalActualCashBackAmountt: number = this.cashbackListData.map(a => a.ActualCashBack).reduce(function (a, b) {
      return a + b;
    });
    this.cashbackPostModal.TotalBillAmount = FinalBillAmount;
    if (this.cashbackPostModal.TotalBillAmount == null || this.cashbackPostModal.TotalBillAmount == 0) {
      swal("Warning!", "Please enter Bill No", "warning");
    }

    else {
      this.cashbackPostModal.CompanyCode = this.ccode;
      this.cashbackPostModal.BranchCode = this.bcode;
      this.cashbackPostModal.OperatorCode = localStorage.getItem('Login');
      this.cashbackPostModal.CompanyCode = this.ccode,
        this.cashbackPostModal.BranchCode = this.bcode,
        this.cashbackPostModal.TotalBillAmount = FinalBillAmount,
        this.cashbackPostModal.TotalEligableCashBackAmount = FinalEligableCashBackAmount,
        this.cashbackPostModal.TotalActualCashBackAmount = FinalActualCashBackAmountt,
        this.cashbackPostModal.OperatorCode = localStorage.getItem('Login'),
        this.cashbackPostModal.ListCashBackVM = this.cashbackListData
      var text: string
      text = "Cash back offer saved successfully";
      var ans = confirm("Do you want to Save cash back offer??");
      if (ans) {
        this._accountsService.PostCashbackDetails(this.cashbackPostModal).subscribe(
          response => {
            this.result = response;
            swal("Success!", "Cash back offer saved successfully", "success");
            this.cashbackListData = [];
            var ans = confirm("Do you want to print??");
            if (ans) {
              this.Printing(this.result);
            }

          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error.description;
              swal("Warning!", validationError, "warning");
            }
            else {
              this.errors.push('something went wrong!');
            }
          }
        )
      }

    }
  }
  PrintDetails: any;
  Printing(arg) {
    this.PrintDetails = null;
    if (arg.TranType === "JOU") {
      this._accountsService.printJournalEntrys(arg.VoucherNo,
        arg.AccouuntCodeMaster, arg.TranType,
        arg.AccType).subscribe(
          response => {
            this.PrintDetails = response;
            $('#printJournalEntryModal').modal('show');
            $('#DisplayData').html(this.PrintDetails);
          }
        )


    }
    else if (arg.TranType === "PAY") {
      this._accountsService.print(arg.VoucherNo,
        arg.AccouuntCodeMaster, arg.TranType,
        arg.AccType, arg.TranType).subscribe(
          Response => {
            this.PrintDetails = Response;
            $('#printCashVoucher').modal('show');
            $('#DisplayData').html(this.PrintDetails);
          }
        )
    }


  }



  cashvoucherentryPrint(arg) {
    this._accountsService.print(this.result.VoucherNo,
      this.result.AccountCodeMaster, this.result.TransType,
      this.result.AccountType, this.result.TransType).subscribe(
        Response => {
          this.PrintDetails = Response;
          $('#ReprintCashVoucher').modal('show');
          $('#DisplayData').html(this.PrintDetails);
        }
      )
  }
  print() {
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


  chkbxchng(e) {
    if (e.target.checked == true) {
      this.cashbackPostModal.IsEPayment = e.target.checked;
    }
    else {
      this.cashbackPostModal.IsEPayment = e.target.checked;
    }
  }
}


