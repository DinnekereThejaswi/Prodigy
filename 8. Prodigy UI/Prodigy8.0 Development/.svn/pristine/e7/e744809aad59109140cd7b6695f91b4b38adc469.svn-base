import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { MasterService } from '../../core/common/master.service';
import { OpgprocessService } from '../opg-process.service';
import swal from 'sweetalert';
declare var $: any;

@Component({
  selector: 'app-opg-receipt-cancel',
  templateUrl: './opg-receipt-cancel.component.html',
  styleUrls: ['./opg-receipt-cancel.component.css']
})
export class OpgReceiptCancelComponent implements OnInit {

  radioItems: Array<string>;
  model = { option: 'Melting Receipts' };
  today = new Date();
  datePickerConfig: Partial<BsDatepickerConfig>;
  CancelForm: FormGroup;

  constructor(private _masterService: MasterService,
    private formBuilder: FormBuilder,
    private _opgprocessService: OpgprocessService) {
    this.radioItems = ['Melting Receipts', 'FT Receipt(After Melting)', 'Purification Receipt', 'FT Receipt(After Purification)'];
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        maxDate: this.today,
        // minDate: this.today,
        dateInputFormat: 'YYYY-MM-DD',

      });
  }

  ngOnInit() {
    this.getApplicationDate();
    this.CancelForm = this.formBuilder.group({
      type: [null],
      receiptno: null,
      remarks: null,
      applicationDate: null
    });
  }

  type: any = [
    {
      "Code": "RL",
      "Name": "Melting Receipts",
    }
  ]

  applicationDate: any;
  disAppDate: any;
  test: any = [];
  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
      }
    )
  }

  radioBtnValue: string = '';
  Changed(arg) {
    this.radioBtnValue = arg;
    if (arg === 'Melting Receipts') {
      this.model.option = arg;
    }
    else if (arg === 'FT Receipt(After Melting)') {
      this.model.option = arg;
    }
    else if (arg === 'Purification Receipt') {
      this.model.option = arg;
    }
    else if (arg === 'FT Receipt(After Purification)') {
      this.model.option = arg;
    }
  }

  cancel(form) {
    if (form.value.type == null) {
      swal("Warning!", 'Please select the Type', "warning");
    }
    else if (form.value.receiptno == null) {
      swal("Warning!", 'Please enter the Receipt No', "warning");
    }
    else if (form.value.remarks == null) {
      swal("Warning!", 'Please enter the Remarks', "warning");
    }
    else {
      var ans = confirm("Do you want to cancel Receipt No: " + form.value.receiptno + "??");
      if (ans) {
        this._opgprocessService.cancelMeltingReceipt(form.value).subscribe(
          response => {
            swal("Cancelled!", "Receipt number " + form.value.receiptno + " Cancelled Successfully", "success");
            this.CancelForm.reset();
            this.getApplicationDate();
          }
        )
      }
    }
  }

  PrintTypeBasedOnConfig: any;
  printData: any = [];

  print(form) {
    if (form.value.type == null) {
      swal("Warning!", 'Please select the Type', "warning");
    }
    else if (form.value.receiptno == null) {
      swal("Warning!", 'Please enter the Receipt No', "warning");
    }
    else {
      this._opgprocessService.getMeltingReceiptPrint(form.value.receiptno).subscribe(
        response => {
          this.PrintTypeBasedOnConfig = response;
          if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
            $('#DetailsTab').modal('show');
            this.printData = atob(this.PrintTypeBasedOnConfig.Data);
            $('#DisplayData').html(this.printData);
          }
          else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
            $('#DetailsTab').modal('show');
            this.printData = this.PrintTypeBasedOnConfig.Data;
          }
        }
      )
    }
  }


  // for printing the form
  printDetails() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.printData);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
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

}