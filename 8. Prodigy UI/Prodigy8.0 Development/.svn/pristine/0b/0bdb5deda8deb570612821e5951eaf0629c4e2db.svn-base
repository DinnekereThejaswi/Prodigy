import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MastersService } from '../masters.service';
import { FixedOrderTypes } from '../masters.model';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
declare var $: any;
@Component({
  selector: 'app-order-fixed-types',
  templateUrl: './order-fixed-types.component.html',
  styleUrls: ['./order-fixed-types.component.css']
})
export class OrderFixedTypesComponent implements OnInit {
  fixedOrderTypeForm: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  isReadOnly: boolean = false;
  EnableJson: boolean = false;
  disabled: boolean = false;
  FixedOrderTypeData: FixedOrderTypes = {
    AdvAmountPer: null,
    BookingCode: null,
    BookingName: "",
    BranchCode: "",
    Cflag: "",
    CompanyCode: "",
    Description: "",
    FixedDays: null,
    ID: 0,
    MinWeight: null,
    OperatorCode: null,
    UpdateOn: null
  }
  constructor(private fb: FormBuilder,
    private _router: Router, private _mastersService: MastersService,
    private _appConfigService: AppConfigService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {

    this.getPlanTypes();
    this.EnableAdd = true;
    this.fixedOrderTypeForm = this.fb.group({
      fromCtrl_BookingName: ["", Validators.required],
      fromCtrl_Description: ["", Validators.required],
      fromCtrl_AdvAmountPer: [0, Validators.required],
      fromCtrl_FixedDays: [0, Validators.required],
      fromCtrl_MinWeight: [0, Validators.required],
    });

  }
  fixedOrders: any = [];
  getFixedOrderList(e) {
    this._mastersService.getfixedOrders(this.FixedOrderTypeData.BookingCode).subscribe(
      response => {
        this.fixedOrders = response;
      }
    )
  }
  plantypes: any = [];
  getPlanTypes() {
    this._mastersService.planNames().subscribe(
      response => {
        this.plantypes = response;
      }
    )
  }
  errors: any = [];
  add(form) {
    if (form.value.fromCtrl_Description == null || form.value.fromCtrl_Description == "") {
      swal("Warning!", "Please enter Description", "warning");
    }
    else {
      this.FixedOrderTypeData.CompanyCode = this.ccode;
      this.FixedOrderTypeData.BranchCode = this.bcode;
      this.FixedOrderTypeData.Cflag = "O";
      this.FixedOrderTypeData.ID = 0;
      this.FixedOrderTypeData.BookingName = "null";

      var ans = confirm("Do you want to Add??");
      if (ans) {
        this._mastersService.PostFixedOrder(this.FixedOrderTypeData).subscribe(
          response => {
            this.errors = response;
            swal("Saved!", "Saved " + this.FixedOrderTypeData.BookingName + " Saved", "success");
            this.ClearValues();

          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error.description;
              swal("Warning!", validationError, "warning");
            }
            else {
              this.errors.push('something went wrong!');
            }
            this.isReadOnly = false;
            this.disabled = false;
            this.fixedOrderTypeForm.reset();
          }
        )
      }
    }
  }

  save(form) {
    if (form.value.fromCtrl_BookingName == null || form.value.fromCtrl_BookingName == "") {
      swal("!Warning", "Please enter Description", "warning");
    }
    else {
      var ans = confirm("Do you want to save??");
      if (ans) {
        this._mastersService.putFixedOrder(this.FixedOrderTypeData.ID, this.FixedOrderTypeData).subscribe(
          response => {
            swal("Updated!", "Saved " + this.FixedOrderTypeData.BookingName + " Saved", "success");
            this.fixedOrders = [];
            this._mastersService.getfixedOrders(this.FixedOrderTypeData.BookingCode).subscribe(
              response => {
                this.fixedOrders = response;
              }
            )
            this.isReadOnly = false;
            this.disabled = false;
            this.EnableAdd = true;
            this.EnableSave = false;
            this.fixedOrderTypeForm.reset();
          }
        )
      }
    }

  }
  clear() {

    this.isReadOnly = false;
    this.EnableAdd = true;
    this.EnableSave = false;
    this.FixedOrderTypeData = {
      AdvAmountPer: 0,
      BookingCode: null,
      BookingName: null,
      BranchCode: null,
      Cflag: null,
      CompanyCode: null,
      Description: null,
      FixedDays: 0,
      ID: 0,
      MinWeight: 0,
      OperatorCode: null,
      UpdateOn: ""
    }
    this.fixedOrderTypeForm.reset();

  }
  ClearValues() {
    this.EnableAdd = true;
    this.EnableSave = false;
    this.fixedOrderTypeForm.reset();
    this.FixedOrderTypeData = {
      AdvAmountPer: 0,
      BookingCode: null,
      BookingName: null,
      BranchCode: null,
      Cflag: null,
      CompanyCode: null,
      Description: null,
      FixedDays: 0,
      ID: 0,
      MinWeight: 0,
      OperatorCode: null,
      UpdateOn: ""
    }
  }
  editField(arg) {

    if (arg.Cflag == "C") {
      swal("Warning!", "Order: " + arg.BookingName + " is Closed. Open to edit", "warning");
    }
    else {
      this.EnableAdd = false;
      this.EnableSave = true;
      this.FixedOrderTypeData = arg;
      this.isReadOnly = true;
      this.disabled = true;
    }

  }

  openField(arg) {
    if (arg.Cflag == "O") {
      swal("Warning!", "Oder: " + arg.BookingName + " is already Open", "warning");
    }
    else {
      var ans = confirm("Do you want to Open Order: " + arg.BookingName + "?");
      if (ans) {
        arg.Cflag = "O";
        this._mastersService.putFixedOrder(arg.ID, arg).subscribe(
          response => {
            swal("Updated!", "Order: " + arg.BookingName + " Open", "success");
          }
        )
      }
    }
  }

  closeField(arg) {
    if (arg.Cflag == "C") {
      swal("Warning!", "Order: " + arg.BookingName + " is already closed", "warning");
    }
    else {
      var ans = confirm("Do you want to close the Order: " + arg.BookingName + "?");
      if (ans) {
        arg.Cflag = "C";
        this._mastersService.putFixedOrder(arg.ID, arg).subscribe(
          response => {
            swal("Updated!", "Order: " + arg.BookingName + " Open", "success");
          }
        )
      }
    }
  }

  getStatusColor(ObjStatus) {
    switch (ObjStatus) {
      case 'O':
        return 'green';
      case 'C':
        return 'red';

    }
  }
  printData: any = [];
  Print() {
    if (this.FixedOrderTypeData.BookingCode == null || this.FixedOrderTypeData.BookingCode == "") {
      swal("Warning!", "Please Select Booking Name", "warning");
    }
    else {
      this.printData = [];
      if (this.printData != null) {
        var ans = confirm("Do you want to take print??");
        if (ans) {
          this._mastersService.getfixedOrders(this.FixedOrderTypeData.BookingCode).subscribe(
            response => {
              this.printData = response
              if (this.printData != null) {
                $('#FixedOrderDetailsTab').modal('show');
              }
            }
          )
        }
      }
    }
  }

  print() {
    if (this.printData !== null) {
      let Deatils, popupWin;
      Deatils = document.getElementById('TableData').innerHTML;
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
      ${Deatils}</body>
        </html>`
      );
      popupWin.document.close();
    }
  }

}
