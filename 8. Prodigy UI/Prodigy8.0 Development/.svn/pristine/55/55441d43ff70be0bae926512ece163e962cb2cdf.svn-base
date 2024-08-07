import { Component, OnInit } from '@angular/core';
import { AccountsService } from '../accounts.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MasterGroupVM } from '../accounts.model';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import swal from 'sweetalert';
declare var $: any;

@Component({
  selector: 'app-master-group',
  templateUrl: './master-group.component.html',
  styleUrls: ['./master-group.component.css']
})

export class MasterGroupComponent implements OnInit {

  MasterGroupType: any = [];
  MasterGroupList: any = [];
  MasterGroupForm: FormGroup;
  GroupType: string = "I";
  IsTrading: string = "N";
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  chkchkbxchked: boolean = false;
  searchText: string;
  EnableJson: boolean = false;
  MasterGroupModel: MasterGroupVM = {
    ObjID: "",
    CompanyCode: "",
    BranchCode: "",
    GroupID: 0,
    GroupName: "",
    GroupType: null,
    Under: "",
    GroupDescription: "",
    IsTrading: "",
    ObjStatus: "",
    ParentGroupID: 0,
    NewGroupCode: "",
    NewSubGroupCode: ""
  }

  constructor(private accountsService: AccountsService, private formBuilder: FormBuilder,
    private appConfigService: AppConfigService) {
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.EnableJson = this.appConfigService.EnableJson;
    this.password = this.appConfigService.Pwd;
    this.getCB();
  }

  ngOnInit() {
    this.getMasterGroupType();
    this.getMasterGroupList();
    this.MasterGroupForm = this.formBuilder.group({
      GroupName: ["", Validators.required],
      GroupType: [null, Validators.required],
      IsTrading: ["", Validators.required],
    });
  }


  getMasterGroupType() {
    this.accountsService.getMasterGroupType().subscribe(
      response => {
        this.MasterGroupType = response;
      }
    )
  }

  getMasterGroupList() {
    this.accountsService.getMasterGroupList().subscribe(
      response => {
        this.MasterGroupList = response;
      }
    )
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  add(form) {
    if (form.value.GroupName == null || form.value.GroupName == "") {
      swal("Warning!", " Please enter group name", "warning");
    }
    else if (form.value.GroupType == null || form.value.GroupType == "") {
      swal("Warning!", " Please select group type", "warning");
    }
    else {
      this.MasterGroupModel.CompanyCode = this.ccode;
      this.MasterGroupModel.BranchCode = this.bcode;
      this.MasterGroupModel.ObjStatus = "O";
      this.MasterGroupModel.GroupType = this.GroupType;
      var ans = confirm("Do you want to save??");
      if (ans) {
        this.accountsService.postMasterGroup(this.MasterGroupModel).subscribe(
          response => {
            swal("Saved!", "Group " + this.MasterGroupModel.GroupName + " Saved", "success");
            this.getMasterGroupList();
            this.ClearValues();
          }
        )
      }
    }
  }

  save(form) {
    if (form.value.GroupName == null || form.value.GroupName == "") {
      swal("Warning!", " Please enter group name", "warning");
    }
    else if (form.value.GroupType == null || form.value.GroupType == "") {
      swal("Warning!", " Please select group type", "warning");
    }
    else {
      var ans = confirm("Do you want to save??");
      if (ans) {
        this.accountsService.putMasterGroup(this.MasterGroupModel, this.MasterGroupModel.ObjID).subscribe(
          response => {
            swal("Updated!", "Group " + this.MasterGroupModel.GroupName + " Updated", "success");
            this.getMasterGroupList();
            this.ClearValues();
          }
        )
      }
    }
  }

  ClearValues() {
    this.MasterGroupForm.reset();
    this.EnableAdd = true;
    this.EnableSave = false;
    this.MasterGroupModel = {
      ObjID: "",
      CompanyCode: "",
      BranchCode: "",
      GroupID: 0,
      GroupName: "",
      GroupType: null,
      Under: "",
      GroupDescription: "",
      IsTrading: "",
      ObjStatus: "",
      ParentGroupID: 0,
      NewGroupCode: "",
      NewSubGroupCode: ""
    }
  }

  clear() {
    this.MasterGroupForm.reset();
  }

  chngGroupName(form) {
    this.GroupType = form.value.GroupType;
    this.chkchkbxchked = false;
  }

  chkbxchng(e) {
    if (e.target.checked) {
      this.MasterGroupModel.IsTrading = "Y";
    }
    else {
      this.MasterGroupModel.IsTrading = "N";
    }
  }

  editField(arg) {
    if (arg.ObjStatus == "C") {
      swal("Warning!", arg.GroupName + " is closed", "warning");
    }
    else {
      this.EnableAdd = false;
      this.EnableSave = true;
      this.MasterGroupModel = arg;
      if (this.MasterGroupModel.IsTrading == "Y") {
        this.chkchkbxchked = true;
      }
      else {
        this.chkchkbxchked = false;
      }
    }
  }

  openField(arg) {
    if (arg.ObjStatus == "O") {
      swal("Warning!", "Group: " + arg.GroupName + " is already opened", "warning");
    }
    else {
      var ans = confirm("Do you want to open Group: " + arg.GroupName + "?");
      if (ans) {
        arg.ObjStatus = "O";
        this.accountsService.putMasterGroup(arg, arg.ObjID).subscribe(
          response => {
            swal("Opened!", "Group (" + arg.GroupID + "): " + arg.GroupName + " is opened", "success");
            this.getMasterGroupList();
          }
        )
      }
    }
  }

  closeField(arg) {
    if (arg.ObjStatus == "C") {
      swal("Warning!", "Group: " + arg.GroupName + " is already closed", "warning");
    }
    else {
      var ans = confirm("Do you want to close the group?");
      if (ans) {
        arg.ObjStatus = "C";
        this.accountsService.putMasterGroup(arg, arg.ObjID).subscribe(
          response => {
            swal("Closed!", "Group: " + arg.GroupName + " closed", "success");
            this.getMasterGroupList();
          }
        )
      }
    }
  }

  printData: any = [];
  Print() {
    this.printData = [];
    if (this.printData != null) {
      var ans = confirm("Do you want to take print??");
      if (ans) {
        this.accountsService.getMasterGroupList().subscribe(
          response => {
            this.printData = response
            if (this.printData != null) {
              $('#PrintDataModal').modal('show');
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