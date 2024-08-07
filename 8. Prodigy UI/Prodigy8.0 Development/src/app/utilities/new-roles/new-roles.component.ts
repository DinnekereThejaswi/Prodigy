import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { UtilitiesService } from '../utilities.service'

declare var $: any;

@Component({
  selector: 'app-new-roles',
  templateUrl: './new-roles.component.html',
  styleUrls: ['./new-roles.component.css']
})
export class NewRolesComponent implements OnInit {

  NewRoleform: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  constructor(private fb: FormBuilder, private _utilitiesService: UtilitiesService, private _appConfigService: AppConfigService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }
  NewrolesPostModel = {
    CompanyCode: "",
    BranchCode: "",
    ID: 0,
    Name: "",
    Status: "",
    OTPValidationEnabled: false
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this.NewrolesPostModel.CompanyCode = this.ccode;
    this.NewrolesPostModel.BranchCode = this.bcode;
    this.NewrolesPostModel.Status = "Active";

    this.getroles();
    this.NewRoleform = this.fb.group({
      frmCtrl_RoleName: null,
      frmCtrl_OtpValidate: null
    });
  }
  RolesList: any = [];
  getroles() {
    this._utilitiesService.getRolesListData().subscribe(
      Response => {
        this.RolesList = Response;
      })
  }
  result: any = [];
  onSubmit(form) {
    if (form.value.frmCtrl_RoleName == null || form.value.frmCtrl_RoleName == "") {
      swal("Warning! ", "Please Enter Role Name ", "warning");
    }
    else {
      var ans = confirm("Do you want to Add??");
      if (ans) {
        this.result = [];
        this._utilitiesService.PostNewroles(this.NewrolesPostModel).subscribe(
          Response => {
            this.result = Response;
            swal("Success!", this.result.Message, "success");
            this.getroles();
            this.NewRoleform.reset();

          }
        )
      }
    }
  }
  Edit(arg) {
    if (arg.Status === "Closed") {
      swal("!Warning", "Role: " + arg.Name + " Status is Closed. Activate to edit", "warning");
    }
    else {
      this.NewrolesPostModel = arg;
      this.EnableAdd = false;
      this.EnableSave = true;
    }
  }
  save(form) {
    if (form.value.frmCtrl_RoleName == null || form.value.frmCtrl_RoleName == "") {
      swal("Warning! ", "Please Enter Role Name ", "warning");
    }
    else {
      var ans = confirm("Do you want to save??");
      if (ans) {
        this.result = [];
        this._utilitiesService.EditNewroles(this.NewrolesPostModel).subscribe(
          Response => {
            this.result = Response;
            swal("Success!", this.result.Message, "success");
            this.getroles();
            this.NewRoleform.reset();
          }
        )
      }
    }
  }
  getStatusColor(Status) {
    switch (Status) {
      case 'Active':
        return 'green';

      case 'Closed':
        return 'red';

    }
  }
  clear() {
    this.NewRoleform.reset();
    this.EnableAdd = true;
    this.EnableSave = false;
  }
  ClearNewRoles() {
    this.NewRoleform.reset();
    this.EnableAdd = true;
    this.EnableSave = false;
  }
  ActivateRole(arg) {
    if (arg.Status === "Active") {
      swal("Warning!", "Role: " + arg.Name + "  is already Active", "warning");
    }
    else {
      var ans = confirm("Do you want to Active Role?" + arg.Name);
      if (ans) {
        this.result = [];
        this._utilitiesService.ActivateRole(arg.ID).subscribe(
          response => {
            this.result = response;
            swal("Success!", this.result.Message, "success");
            this.getroles();
          }
        )
      }
    }
  }
  DeActivateRole(arg) {
    if (arg.Status === "Closed") {
      swal("Warning!", "Role:  " + arg.Name + "  is already Closed", "warning");
    }
    else {
      var ans = confirm("Do you want to DeActivate Role?" + arg.Name);
      if (ans) {
        this.result = [];
        this._utilitiesService.DeActivateRole(arg.ID).subscribe(
          response => {
            this.result = response;
            swal("Success!", this.result.Message, "success");
            this.getroles();
          }
        )
      }
    }
  }

  printRoleDetails() {
    this.getroles();
    $('#NewRolesTab').modal('show');
  }

  printRoleData() {
    if (this.RolesList !== null) {
      let printntRolesData, popupWin;
      printntRolesData = document.getElementById('DisplayprintRolesData').innerHTML;
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
      ${printntRolesData}</body>
        </html>`
      );
      popupWin.document.close();
    }
  }
}
