import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl, AbstractControl } from '@angular/forms';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { Alert } from 'bootstrap';
import { Tolerance } from '../utilities.model'
import { UtilitiesService } from '../utilities.service'
declare var $: any;

@Component({
  selector: 'app-tolerance',
  templateUrl: './tolerance.component.html',
  styleUrls: ['./tolerance.component.css']
})
export class ToleranceComponent implements OnInit {
  IsReadonly: boolean = false;
  tolerenceform: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;

  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  ToleranceModel: Tolerance = {
    ID: 0,
    CompanyCode: "",
    BranchCode: "",
    Description: "",
    MinValue: 0,
    MaxValue: 0
  }
  constructor(private fb: FormBuilder, private _utilitiesService: UtilitiesService, private _appConfigService: AppConfigService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();

  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this.ToleranceModel.ID = null;
    this.ToleranceModel.CompanyCode = this.ccode;
    this.ToleranceModel.BranchCode = this.bcode;
    this.getListOfTolerances();
    this.tolerenceform = this.fb.group({
      frmCtrl_ID: [null],
      frmCtrl_Description: null,
      frmCtrl_MinValue: [null],
      frmCtrl_MaxValue: [null],
    });
  }
  ListOfTolerance: any = [];
  getListOfTolerances() {
    this._utilitiesService.getToleranceList().subscribe(
      Response => {
        this.ListOfTolerance = Response;
      })
  }
  editTolerance(arg) {
    this.ToleranceModel = arg;
    this.EnableAdd = false;
    this.EnableSave = true;
    this.IsReadonly = true;
  }

  errors: any = [];
  add(form) {
    if (form.value.frmCtrl_ID == null) {
      swal("Warning!", "Please enter ID", "warning");
    }
    else if (form.value.frmCtrl_Description == null || form.value.frmCtrl_Description == "") {
      swal("Warning!", "Please enter Description", "warning");
    }
    else if (form.value.frmCtrl_MinValue == null || form.value.frmCtrl_MinValue == "") {
      swal("Warning!", "Please enter Min Value", "warning");
    }
    else if (form.value.frmCtrl_MaxValue == null || form.value.frmCtrl_MaxValue == "") {
      swal("Warning!", "Please enter Max Value", "warning");
    }
    else {
      var ans = confirm("Do you want to Add ??" + form.value.frmCtrl_Description);
      if (ans) {
        this.result = [];
        this._utilitiesService.AddNewTolerance(this.ToleranceModel).subscribe(
          response => {
            swal("Success!", "saved  " + this.ToleranceModel.Description + "  saved", "success");
            this.getListOfTolerances();
            this.EnableAdd = true;
            this.EnableSave = false;
            this.tolerenceform.reset();
            this.clear();
          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error;
              swal("Success!", validationError, "warning");
            }
            else {
              this.errors.push('something went wrong!');
            }

          }
        )
      }
    }
  }
  result: any = []
  save(form) {
    if (form.value.frmCtrl_ID == null) {
      swal("Warning!", "Please enter ID", "warning");
    }
    else if (form.value.frmCtrl_Description == null || form.value.frmCtrl_Description == "") {
      swal("Warning!", "Please enter Description", "warning");
    }
    else if (form.value.frmCtrl_MinValue == null || form.value.frmCtrl_MinValue == "") {
      swal("Warning!", "Please enter Min Value", "warning");
    }
    else if (form.value.frmCtrl_MaxValue == null || form.value.frmCtrl_MaxValue == "") {
      swal("Warning!", "Please enter Max Value", "warning");
    }
    else {
      this.result = [];
      this.ToleranceModel.CompanyCode = this.ccode;
      this.ToleranceModel.BranchCode = this.bcode;
      var ans = confirm("Do you want to save ??" + this.ToleranceModel.Description);
      if (ans) {
        this.result = [];
        this._utilitiesService.EditTolerance(this.ToleranceModel).subscribe(
          response => {
            swal("Success!", "saved  " + this.ToleranceModel.Description + "  saved", "success");
            this.getListOfTolerances();
            this.EnableAdd = true;
            this.EnableSave = false;
            this.tolerenceform.reset();
            this.clear();
          }
        )
      }
    }
  }
  deleteTolerence(arg) {
    var ans = confirm("Do you want to Delete ??" + arg.Description);
    if (ans) {
      this.result = [];
      this._utilitiesService.DeleteTolerance(arg.ID).subscribe(
        response => {

          swal("Success! ", "Deleted  " + arg.ID + "  Deleted", "success");
          this.getListOfTolerances();
          this.EnableAdd = true;
          this.EnableSave = false;
          this.tolerenceform.reset();
        }
      )
    }

  }

  printTolerance() {
    this.getListOfTolerances();
    $('#PrintToleranceTab').modal('show');
  }

  printToleranceData() {
    if (this.ListOfTolerance !== null) {
      let printListOfTolerance, popupWin;
      printListOfTolerance = document.getElementById('DisplayPrintListOfToleranceData').innerHTML;
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
      ${printListOfTolerance}</body>
        </html>`
      );
      popupWin.document.close();
    }
  }

  clear() {
    this.tolerenceform.reset();
    this.getListOfTolerances();
    this.EnableAdd = true;
    this.EnableSave = false;
    this.IsReadonly = false;
  }
}
