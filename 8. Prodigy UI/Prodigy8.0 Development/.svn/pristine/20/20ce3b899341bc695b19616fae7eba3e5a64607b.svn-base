import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MainLocation } from '../masters.model'
import { MastersService } from '../masters.service';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
declare var $: any;
@Component({
  selector: 'app-main-location',
  templateUrl: './main-location.component.html',

  styleUrls: ['./main-location.component.css']
})
export class MainLocationComponent implements OnInit {
  ccode: string;
  bcode: string;
  password: string;
  MainLocationForm: FormGroup;
  EnableJson: boolean = false;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  isReadOnly: boolean = false;

  MainLocationListData: MainLocation = {
    ObjID: null,
    CompanyCode: null,
    BranchCode: null,
    MainCounterCode: null,
    MainCounterName: null,
    ObjectStatus: null
  }
  constructor(private _appConfigService: AppConfigService, private fb: FormBuilder, private _mastersService: MastersService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this.getMainCounterList();
    this.MainLocationForm = this.fb.group({
      ObjID: null,
      CompanyCode: null,
      BranchCode: null,
      MainCounterCode: null,
      MainCounterName: null,
      ObjectStatus: null
    });
  }
  MainLocation: any = [];
  getMainCounterList() {
    this._mastersService.getMaincounters().subscribe(
      Response => {
        this.MainLocation = Response;

      }
    )
  }
  getStatusColor(ObjStatus) {
    switch (ObjStatus) {
      case 'O':
        return 'green';
      case 'C':
        return 'red';

    }
  }
  errors = [];
  add(form) {
    if (form.value.MainCounterCode == null || form.value.MainCounterCode == "") {
      swal("Warning!", " Please enter  code", "warning");
    }
    else if (form.value.MainCounterName == null || form.value.MainCounterName == "") {
      swal("Warning!", "Please enter  Name", "warning");
    }
    else {
      this.MainLocationListData.CompanyCode = this.ccode;
      this.MainLocationListData.BranchCode = this.bcode;
      this.MainLocationListData.ObjectStatus = "O";

      var ans = confirm("Do you want to save??");
      if (ans) {
        this._mastersService.PostLocation(this.MainLocationListData).subscribe(
          response => {
            response;
            swal("Saved!", "Saved " + this.MainLocationListData.MainCounterName + " Saved", "success");
            // this.ClearValues();

          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error.description;
              swal("Warning!", validationError, "warning");
            }
            else {
              this.errors.push('something went wrong!');
            }
            // this.clear();
          }
        )
      }
    }
  }
  editField(arg) {
    if (arg.ObjectStatus == "C") {
      swal("Warning!", "MainLocation: " + arg.MainCounterName + " is Closed Open to Edit", "warning");
    }

    else {
      this.EnableAdd = false;
      this.EnableSave = true;
      this.MainLocationListData = arg;
      this.isReadOnly = true;
    }
  }


  save(form) {
    if (form.value.MainCounterName == null || form.value.MainCounterName == "") {
      swal("Warning!", " Please enter  name", "warning");
    }
    else {
      var ans = confirm("Do you want to save??");
      if (ans) {
        this._mastersService.ModifyOrUpdateLocationByPUT(this.MainLocationListData.ObjID, this.MainLocationListData).subscribe(
          response => {
            swal("Updated!", "Saved " + this.MainLocationListData.MainCounterName + " Saved", "success");
            this.getMainCounterList();
            this.ClearValues();

          }

        )
      }
    }
  }

  openField(arg) {
    this.MainLocationListData = arg;
    if (arg.ObjectStatus == "O") {
      swal("Warning!", "MainCounterName: " + arg.MainCounterName + " is already Opend", "warning");
    }
    else {
      var ans = confirm("Do you want to Open MainCounterName: " + arg.MainCounterName + "?");
      if (ans) {
        this.MainLocationListData.ObjectStatus = "O";
        this._mastersService.ModifyOrUpdateLocationByPUT(this.MainLocationListData.ObjID, this.MainLocationListData).subscribe(
          response => {
            swal("Updated!", "MainCounterName: " + arg.MainCounterName + " Open", "success");
            this.getMainCounterList();
          }
        )
      }
    }
    this.getMainCounterList();
  }


  closeField(arg) {
    this.MainLocationListData = arg;
    if (arg.ObjectStatus == "C") {
      swal("Warning!", "MainCounterName: " + arg.MainCounterName + " is already closed", "warning");
    }
    else {
      var ans = confirm("Do you want to close the MainCounterName: " + arg.MainCounterName + "?");
      if (ans) {
        this.MainLocationListData.ObjectStatus = "C";
        this._mastersService.ModifyOrUpdateLocationByPUT(this.MainLocationListData.ObjID, this.MainLocationListData).subscribe(
          response => {
            swal("Updated!", "MainCounterName: " + arg.MainCounterName + " close", "success");
            this.getMainCounterList();
          }
        )
      }
    }
    this.getMainCounterList();
  }
  clear() {
    this.MainLocationForm.reset();
    this.isReadOnly = false;
    this.getMainCounterList();
  }
  ClearValues() {
    this.EnableAdd = true;
    this.EnableSave = false;
    this.clear();
    this.MainLocationForm.reset();
    this.MainLocationListData = {
      ObjID: "",
      CompanyCode: "",
      BranchCode: "",
      MainCounterCode: "",
      MainCounterName: "",
      ObjectStatus: ""
    }
  }

  printData: any = [];
  Print() {
    this.printData = [];
    if (this.printData != null) {
      var ans = confirm("Do you want to take print??");
      if (ans) {
        this._mastersService.getMaincounters().subscribe(
          Response => {
            this.printData = Response
            if (this.printData != null) {
              $('#MainLocationDetailsTab').modal('show');
            }
          }
        )
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

