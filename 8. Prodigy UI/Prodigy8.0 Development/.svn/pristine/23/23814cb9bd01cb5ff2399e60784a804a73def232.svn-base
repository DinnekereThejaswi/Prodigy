import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl, AbstractControl } from '@angular/forms';
import { AppConfigService } from '../../AppConfigService';
import { MainModule } from '../utilities.model';
import { UtilitiesService } from '../utilities.service'
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { Router } from '@angular/router';

declare var $: any;
@Component({
  selector: 'app-module-settings',
  templateUrl: './module-settings.component.html',
  styleUrls: ['./module-settings.component.css']
})
export class ModuleSettingsComponent implements OnInit {

  ModuleSettingsform: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  MainModuleList: any = [];
  EnableAdd: boolean = true;
  EnableUpdate: boolean = false;

  constructor(private fb: FormBuilder, private _utilitiesService: UtilitiesService,
    private _appConfigService: AppConfigService, private _router: Router) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();

  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.postMainModule.CompanyCode = this.ccode;
    this.postMainModule.BranchCode = this.bcode;
  }

  ngOnInit() {
    this.GetMainModuleList();
  }

  GetMainModuleList() {
    this._utilitiesService.getMainModuleList().subscribe(
      response => {
        this.MainModuleList = response;
      }
    )
  }

  getStatusColor(ObjStatus) {
    switch (ObjStatus) {
      case 'Active':
        return 'green';
      case 'Closed':
        return 'red';
    }
  }

  postMainModule: MainModule = {
    ID: 0,
    CompanyCode: null,
    BranchCode: null,
    Name: null,
    SortOrder: null,
    Status: "Active",
    UIRoute: null,
    Icon: null,
    Class: "has-arrow",
    Label: null,
    LabelClass: null,
  }

  Add() {
    if (this.postMainModule.Name == null || this.postMainModule.Name == "") {
      swal("Warning!", 'Please Enter the Module Name', "warning");
    }
    else if (this.postMainModule.SortOrder == null || this.postMainModule.SortOrder == 0) {
      swal("Warning!", 'Please Enter the Sequence No', "warning");
    }
    else {
      var ans = (confirm("Do you want to save Module " + this.postMainModule.Name + "??"))
      if (ans) {
        this._utilitiesService.postMainModule(this.postMainModule).subscribe(response => {
          this.outputData = response;
          swal("Saved!", this.outputData.Message, "success");
          this.GetMainModuleList();
          this.postMainModule = {
            ID: 0,
            CompanyCode: null,
            BranchCode: null,
            Name: null,
            SortOrder: null,
            Status: null,
            UIRoute: null,
            Icon: null,
            Class: null,
            Label: null,
            LabelClass: null,
          }
        })
      }
    }
  }

  outputData: any;

  Edit(arg) {
    if (arg.Status == "Closed") {
      swal("Warning!", 'Module ' + arg.Name + ' is closed, Open to Edit it', "warning");
    }
    else {
      this.postMainModule = arg;
      this.EnableAdd = false;
      this.EnableUpdate = true;
    }
  }

  Open(arg) {
    if (arg.Status == "Active") {
      swal("Warning!", 'Module ' + arg.Name + ' is already Opened', "warning");
    }
    else {
      var ans = (confirm("Do you want to Open Module " + arg.Name + "??"))
      if (ans) {
        this._utilitiesService.openMainModule(arg.ID).subscribe(
          response => {
            this.outputData = response;
            swal("Saved!", this.outputData.Message, "success");
            this.GetMainModuleList();
          }
        )
      }
    }
  }

  Close(arg) {
    if (arg.Status == "Closed") {
      swal("Warning!", 'Module ' + arg.Name + ' is already Closed', "warning");
    }
    else {
      var ans = (confirm("Do you want to Close Module " + arg.Name + "??"))
      if (ans) {
        this._utilitiesService.closeMainModule(arg.ID).subscribe(
          response => {
            this.outputData = response;
            swal("Saved!", this.outputData.Message, "success");
            this.GetMainModuleList();
          }
        )
      }
    }
  }

  Update() {
    if (this.postMainModule.Name == null || this.postMainModule.Name == "") {
      swal("Warning!", 'Please Enter the Module Name', "warning");
    }
    else if (this.postMainModule.SortOrder == null || this.postMainModule.SortOrder == 0) {
      swal("Warning!", 'Please Enter the Sequence No', "warning");
    }
    else {
      var ans = (confirm("Do you want to Modify Module " + this.postMainModule.Name + "??"))
      if (ans) {
        this._utilitiesService.putMainModule(this.postMainModule).subscribe(
          response => {
            this.outputData = response;
            swal("Saved!", this.outputData.Message, "success");
            this.GetMainModuleList();
            this.postMainModule = {
              ID: 0,
              CompanyCode: null,
              BranchCode: null,
              Name: null,
              SortOrder: null,
              Status: null,
              UIRoute: null,
              Icon: null,
              Class: null,
              Label: null,
              LabelClass: null,
            }
            this.EnableAdd = true;
            this.EnableUpdate = false;
          }
        )
      }
    }
  }

  Clear() {
    this.postMainModule = {
      ID: 0,
      CompanyCode: null,
      BranchCode: null,
      Name: null,
      SortOrder: null,
      Status: null,
      UIRoute: null,
      Icon: null,
      Class: null,
      Label: null,
      LabelClass: null,
    }
  }


  printData: any = [];
  Print() {
    this.printData = [];
    if (this.printData != null) {
      var ans = confirm("Do you want to take print??");
      if (ans) {
        this._utilitiesService.getMainModuleList().subscribe(
          response => {
            this.printData = response
            if (this.printData != null) {
              $('#PrintDetailsTab').modal('show');
            }
          }
        )
      }
    }
  }


  print() {
    let printContents, popupWin;
    printContents = document.getElementById('TableData').innerHTML;
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