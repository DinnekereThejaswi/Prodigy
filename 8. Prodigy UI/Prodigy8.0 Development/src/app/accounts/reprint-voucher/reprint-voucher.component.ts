import { Component, OnInit } from '@angular/core';
import { MasterService } from '../../core/common/master.service';
import { AccountsService } from '../accounts.service';
import { Router } from '@angular/router';
import { formatDate } from '@angular/common';
import swal from 'sweetalert';

declare var $: any;

@Component({
  selector: 'app-reprint-voucher',
  templateUrl: './reprint-voucher.component.html',
  styleUrls: ['./reprint-voucher.component.css']
})
export class ReprintVoucherComponent implements OnInit {

  radioItems: Array<string>;
  model = { option: 'Cash' };

  constructor(private _masterService: MasterService, private AccountsService: AccountsService,
    private Router: Router) {
    this.radioItems = ['Cash', 'Bank', 'Contra', 'Journal', 'Petty Cash'];
  }

  ngOnInit() {
    this.getApplicationDate();
    this.getPaymentType();
  }

  applicationDate: any;
  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
      }
    )
  }

  PaymentType: any = [];

  getPaymentType() {
    this.AccountsService.getPaymentType().subscribe(
      Response => {
        this.PaymentType = Response;
      }
    )
  }

  OnRadioBtnChnge(arg) {
    this.model.option = arg;
    this.voucherNo = 0;
    if (this.model.option == "Cash") {
      this.MasterLedgerNo = 1;
      this.AccountsService.getCashVoucherTable(this.TransType, this.MasterLedgerNo, this.applicationDate).subscribe(
        response => {
          this.VoucherList = response;
        }
      )
    }
    else if (this.model.option == "Petty Cash") {
      this.MasterLedgerNo = 2000117;
      this.AccountsService.getCashVoucherTable(this.TransType, this.MasterLedgerNo, this.applicationDate).subscribe(
        response => {
          this.VoucherList = response;
        }
      )
    }
    else if (this.model.option == "Bank") {
      this.AccountsService.getBankVoucherPrintList(this.TransType, this.applicationDate).subscribe(
        response => {
          this.VoucherList = response;
        }
      )
    }
    else if (this.model.option == "Contra") {
      this.AccountsService.getContraPrintList(this.applicationDate).subscribe(
        response => {
          this.VoucherList = response;
        }
      )
    }
    else if (this.model.option == "Journal") {
      this.AccountsService.getPrintListJournalEntry(this.applicationDate).subscribe(
        response => {
          this.VoucherList = response;
        }
      )
    }
  }

  Cancel() {
    this.Router.navigate(['/dashboard']);
  }

  MasterLedgerNo: number = 0;
  VoucherList: any = [];
  TransType: string = "";
  getVoucherNo(arg) {
    this.TransType = arg;
    if (this.model.option == "Cash") {
      this.MasterLedgerNo = 1;
      this.AccountsService.getCashVoucherTable(arg, this.MasterLedgerNo, this.applicationDate).subscribe(
        response => {
          this.VoucherList = response;
        }
      )
    }
    else if (this.model.option == "Petty Cash") {
      this.MasterLedgerNo = 2000117;
      this.AccountsService.getCashVoucherTable(arg, this.MasterLedgerNo, this.applicationDate).subscribe(
        response => {
          this.VoucherList = response;
        }
      )
    }
    else if (this.model.option == "Bank") {
      this.AccountsService.getBankVoucherPrintList(this.TransType, this.applicationDate).subscribe(
        response => {
          this.VoucherList = response;
        }
      )
    }
    else if (this.model.option == "Contra") {
      this.AccountsService.getContraPrintList(this.applicationDate).subscribe(
        response => {
          this.VoucherList = response;
        }
      )
    }
    else if (this.model.option == "Journal") {
      this.AccountsService.getPrintListJournalEntry(this.applicationDate).subscribe(
        response => {
          this.VoucherList = response;
        }
      )
    }
  }

  voucherNo: number = 0;

  ChangeVoucherDets(arg) {
    this.voucherNo = arg;
  }

  PrintDetails: any;
  Voucherdets: any;

  rePrint() {
    if (this.model.option == "Cash" || this.model.option == "Petty Cash") {
      if (this.TransType == "") {
        swal("!Warning", "Please select the type", "warning")

      }
      else if (this.voucherNo == 0) {
        swal("!Warning", "Please select the voucher", "warning")
      }
      else {
        this.Voucherdets = this.VoucherList.filter(x => x.VoucherNo == this.voucherNo);
        this.AccountsService.print(this.voucherNo, this.Voucherdets[0].AccCodeMaster, this.TransType, this.Voucherdets[0].AccType, this.TransType).subscribe(
          Response => {
            this.PrintDetails = Response;
            $('#ReprintVoucherModal').modal('show');
            $('#DisplayData').html(this.PrintDetails);
          }
        )
      }
    }
    else if (this.model.option == "Bank") {
      if (this.TransType == "") {
        swal("!Warning", "Please select the type", "warning")
      }
      else if (this.voucherNo == 0) {
        swal("!Warning", "Please select the voucher", "warning")
      }
      else {
        this.Voucherdets = this.VoucherList.filter(x => x.VoucherNo == this.voucherNo);
        this.AccountsService.printBankVoucherEntry(this.voucherNo, this.Voucherdets[0].AccCodeMaster, this.TransType, "B").subscribe(
          Response => {
            this.PrintDetails = Response;
            $('#ReprintVoucherModal').modal('show');
            $('#DisplayData').html(this.PrintDetails);
          }
        )
      }
    }
    else if (this.model.option == "Contra") {
      if (this.voucherNo == 0) {
        swal("!Warning", "Please select the voucher", "warning")
      }
      else {
        this.Voucherdets = this.VoucherList.filter(x => x.VoucherNo == this.voucherNo);
        this.AccountsService.printContraEntry(this.voucherNo, this.Voucherdets[0].AccCodeMaster, "CONT", "N", "CONT").subscribe(
          Response => {
            this.PrintDetails = Response;
            $('#ReprintVoucherModal').modal('show');
            $('#DisplayData').html(this.PrintDetails);
          }
        )
      }
    }
    else if (this.model.option == "Journal") {
      if (this.voucherNo == 0) {
        swal("!Warning", "Please select the voucher", "warning")
      }
      else {
        this.Voucherdets = this.VoucherList.filter(x => x.VoucherNo == this.voucherNo);
        this.AccountsService.reprintJournalEntry(this.Voucherdets).subscribe(
          Response => {
            this.PrintDetails = Response;
            $('#ReprintVoucherModal').modal('show');
            $('#DisplayData').html(this.PrintDetails);
          }
        )
      }
    }
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
}