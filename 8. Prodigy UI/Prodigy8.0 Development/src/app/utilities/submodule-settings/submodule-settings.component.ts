import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl, AbstractControl } from '@angular/forms';
import { AppConfigService } from '../../AppConfigService';
import { SubModule } from '../utilities.model';
import { UtilitiesService } from '../utilities.service'
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
declare var $: any
@Component({
  selector: 'app-submodule-settings',
  templateUrl: './submodule-settings.component.html',
  styleUrls: ['./submodule-settings.component.css']
})
export class SubmoduleSettingsComponent implements OnInit {

  SubModuleform: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  constructor(private fb: FormBuilder, private _utilitiesService: UtilitiesService, private _appConfigService: AppConfigService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();

  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.postSubMainModule.CompanyCode = this.ccode;
    this.postSubMainModule.BranchCode = this.bcode;
  }

  ModuleList: any = [];
  SubModuleList: any = [];
  ModuleId: number = 1;
  EnableAdd: boolean = true;
  EnableUpdate: boolean = false;
  postSubMainModule: SubModule = {
    CompanyCode: null,
    BranchCode: null,
    ID: null,
    Name: null,
    ModuleID: 1,
    SortOrder: 0,
    Status: null,
    AutoApprove: true,
    Flag: null,
    UIRoute: null,
    Icon: null,
    Class: null,
    Label: null,
    LabelClass: null,
    FormType: null,
    ReportServerType: null,
    ReportApiRoute: null
  }

  ngOnInit() {
    this.getModuleList();
    this.getSubModuleByID();
    this.SubModuleform = this.fb.group({
      ModuleID: [1],
      ID: [null],
      Name: [null],
      SortOrder: [0],
      UIRoute: [null],
      Icon: [null]
    });
  }

  getStatusColor(ObjStatus) {
    switch (ObjStatus) {
      case 'Active':
        return 'green';
      case 'Closed':
        return 'red';
    }
  }

  getModuleList() {
    this._utilitiesService.getMainModuleList().subscribe(response => {
      this.ModuleList = response;
    })
  }

  getSubModuleByID() {
    this._utilitiesService.getSubModuleByModuleID(this.postSubMainModule.ModuleID).subscribe(response => {
      this.SubModuleList = response;
    })
  }

  outputData: any;

  Edit(arg) {
    if (arg.Status == "Closed") {
      swal("Warning!", 'Sub Module ' + arg.Name + ' is closed, Open to Edit it', "warning");
    }
    else {
      this.postSubMainModule = arg;
      this.EnableAdd = false;
      this.EnableUpdate = true;
    }
  }

  Open(arg) {
    if (arg.Status == "Active") {
      swal("Warning!", 'Sub Module ' + arg.Name + ' is already Opened', "warning");
    }
    else {
      var ans = (confirm("Do you want to Open Sub Module " + arg.Name + "??"))
      if (ans) {
        this._utilitiesService.openSubModule(arg.ID).subscribe(
          response => {
            this.outputData = response;
            swal("Saved!", this.outputData.Message, "success");
            this.getSubModuleByID();
          }
        )
      }
    }
  }

  Close(arg) {
    if (arg.Status == "Closed") {
      swal("Warning!", 'Sub Module ' + arg.Name + ' is already Closed', "warning");
    }
    else {
      var ans = (confirm("Do you want to Close Sub Module " + arg.Name + "??"))
      if (ans) {
        this._utilitiesService.closeSubModule(arg.ID).subscribe(
          response => {
            this.outputData = response;
            swal("Saved!", this.outputData.Message, "success");
            this.getSubModuleByID();
          }
        )
      }
    }
  }

  Add() {
    if (this.postSubMainModule.Name == null || this.postSubMainModule.Name == "") {
      swal("Warning!", 'Please Enter the Sub Module Name', "warning");
    }
    else if (this.postSubMainModule.ID == null || this.postSubMainModule.ID == 0) {
      swal("Warning!", 'Please Enter the Sub Module ID', "warning");
    }
    else {
      var ans = (confirm("Do you want to save Sub Module " + this.postSubMainModule.Name + "??"))
      if (ans) {
        this._utilitiesService.postSubModule(this.postSubMainModule).subscribe(response => {
          this.outputData = response;
          swal("Saved!", this.outputData.Message, "success");
          this.getSubModuleByID();
          this.postSubMainModule.Name = null;
          this.postSubMainModule.ID = null;
          this.postSubMainModule.SortOrder = 0;
          this.postSubMainModule.Icon = null;
          this.postSubMainModule.UIRoute = null;
        })
      }
    }
  }

  Update() {
    if (this.postSubMainModule.Name == null || this.postSubMainModule.Name == "") {
      swal("Warning!", 'Please Enter the Sub Module Name', "warning");
    }
    else if (this.postSubMainModule.ID == null || this.postSubMainModule.ID == 0) {
      swal("Warning!", 'Please Enter the Sub Module ID', "warning");
    }
    else {
      var ans = (confirm("Do you want to Modify Sub Module " + this.postSubMainModule.Name + "??"))
      if (ans) {
        this._utilitiesService.putSubModule(this.postSubMainModule).subscribe(
          response => {
            this.outputData = response;
            swal("Saved!", this.outputData.Message, "success");
            this.getSubModuleByID();
            this.postSubMainModule.Name = null;
            this.postSubMainModule.ID = null;
            this.postSubMainModule.SortOrder = 0;
            this.postSubMainModule.Icon = null;
            this.postSubMainModule.UIRoute = null;
            this.EnableAdd = true;
            this.EnableUpdate = false;
          }
        )
      }
    }
  }

  Clear() {
    this.postSubMainModule.Name = null;
    this.postSubMainModule.ID = null;
    this.postSubMainModule.SortOrder = 0;
    this.postSubMainModule.Icon = null;
    this.postSubMainModule.UIRoute = null;
  }
  printData: any = [];
  Print() {
    this.printData = [];
    if (this.printData != null) {
      var ans = confirm("Do you want to take print??");
      if (ans) {
        this._utilitiesService.getSubModuleByModuleID(this.postSubMainModule.ModuleID).subscribe(
          response => {
            this.printData = response;
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