import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MastersService } from './../masters.service';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { ToastrService } from 'ngx-toastr';
import { AppConfigService } from '../../AppConfigService';
declare var $: any;
@Component({
  selector: 'app-tds-master',
  templateUrl: './tds-master.component.html',
  styleUrls: ['./tds-master.component.css']
})
export class TdsMasterComponent implements OnInit {
  TDSMasterForm: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  isReadOnly: boolean = false;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  EnableJson: boolean = false;

  TdsListData = {
    ObjID: "",
    CompanyCode: "",
    BranchCode: "",
    GSCode: "",
    TDSName: "",
    TDSID: 0,
    TDS: null,
    UpdateOn: "",
    ObjectStatus: ""
  }
  constructor(private fb: FormBuilder, private _masterservice: MastersService, private _router: Router,
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
    this.TdsListData.CompanyCode = this.ccode;
    this.TdsListData.BranchCode = this.bcode;
    this.TdsListData.ObjectStatus = "O";
    this.getTDSlist();
    this.TDSMasterForm = this.fb.group({
      frmCtrl_TDSName: null,
      frmCtrl_TDS: null
    });
  }

  TDSRecordList: any;
  getTDSlist() {
    this._masterservice.getTDSListData().subscribe(
      Response => {
        this.TDSRecordList = Response;
      }
    )
  }
  ResponseData: any = [];
  Add(form) {
    if (form.value.frmCtrl_TDSName == "" || form.value.frmCtrl_TDSName == null) {
      swal("Warning!", "Please Enter TDS Name", "warning")
    }
    else if (form.value.frmCtrl_TDS == "") {
      swal("Warning!", "Please Enter TDS Percent", "warning")
    }
    else {
      var ans = confirm("Do you want to Add ??" + this.TdsListData.TDSName);
      if (ans) {
        this._masterservice.PostTDS(this.TdsListData).subscribe(
          Response => {
            this.ResponseData = Response;
            swal("Success!", "Saved Succesfully", "success");
            this.getTDSlist();
          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error.description;
              swal("Warning!", validationError, "warning");
            }
          }
        )
        this.TDSMasterForm.reset();
        this.ResponseData = [];
      }
    }
  }
  edit(arg) {
    if (arg.ObjectStatus == "C") {
      swal("!Warning", "TDS: " + arg.TDSName + " is Closed. Open to edit", "warning");
    }
    else {
      this.TDSMasterForm.reset();
      this.TdsListData = arg;
      this.EnableAdd = false;
      this.EnableSave = true;
    }
  }
  //
  save(form) {
    if (form.value.frmCtrl_TDSName == "" || form.value.frmCtrl_TDSName == null) {
      swal("Warning!", "Please Enter TDS Name", "warning")
    }
    else if (form.value.frmCtrl_TDS == "") {
      swal("Warning!", "Please Enter TDS Percent", "warning")
    }
    else {
      var ans = confirm("Do you want to Save ??");
      if (ans) {
        this._masterservice.putTDS(this.TdsListData.ObjID, this.TdsListData).subscribe(
          Response => {
            this.ResponseData = Response;
            swal("Success!", "Saved Successfully", "success");
            this.getTDSlist();
            this.TDSMasterForm.reset();
            this.ResponseData = [];
          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error.description;
              swal("Warning!", validationError, "warning");
            }
          }
        )
      }
    }
  }

  Activate(arg) {
    if (arg.ObjectStatus == "O") {
      swal("Warning!", "TDS: " + arg.TDSName + " is already Active", "warning");
    }
    else {
      var ans = confirm("Do you want to Open ??");
      if (ans) {
        this._masterservice.ActivateTDS(arg.ObjID).subscribe(
          Response => {
            this.ResponseData = Response;
            swal("Success!", "Saved Successfully", "success");
            this.TDSMasterForm.reset();
            this.ResponseData = [];
            this.getTDSlist();
          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error.description;
              swal("Warning!", validationError, "warning");
            }
          }
        )
      }
    }
  }
  Deactivate(arg) {
    if (arg.ObjectStatus == "C") {
      swal("Warning!", "TDS: " + arg.TDSName + " is already closed / Deactive", "warning");
    }
    else {
      var ans = confirm("Do you want to Close ??");
      if (ans) {
        this._masterservice.DeactivateTDS(arg.ObjID).subscribe(
          Response => {

            swal("Success!", "Saved Successfully", "success");
            this.TDSMasterForm.reset();
            this.ResponseData = [];
            this.getTDSlist();
          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error.description;
              swal("Warning!", validationError, "warning");
            }
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
  clear() {
    this.TDSMasterForm.reset();
    this.getTDSlist();
    this.EnableSave = false;
    this.EnableAdd = true;
    this.TdsListData = {
      ObjID: "",
      CompanyCode: "",
      BranchCode: "",
      GSCode: "",
      TDSName: "",
      TDSID: 0,
      TDS: 0,
      UpdateOn: "",
      ObjectStatus: ""
    }

  }

  printData: any = [];
  Print() {
    this.printData = [];
    if (this.printData != null) {
      var ans = confirm("Do you want to take print??");
      if (ans) {
        this._masterservice.getTDSListData().subscribe(
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
