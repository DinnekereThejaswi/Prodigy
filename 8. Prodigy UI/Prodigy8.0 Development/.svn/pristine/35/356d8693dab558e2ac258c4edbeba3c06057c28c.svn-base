import { Component, OnInit } from '@angular/core';
import { AccountsService } from '../accounts.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MasterGroupVM, SubGroupVM } from '../accounts.model';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';
declare var $: any;
@Component({
  selector: 'app-sub-group',
  templateUrl: './sub-group.component.html',
  styleUrls: ['./sub-group.component.css']
})
export class SubGroupComponent implements OnInit {

  SubGroupType: any = [];
  SubGroupList: any = [];
  SubGroupForm: FormGroup;
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  searchText: string;
  EnableJson: boolean = false;
  SubGroupModel: SubGroupVM = {
    ParentGroupName: "",
    ObjID: "",
    CompanyCode: "",
    BranchCode: "",
    GroupID: 0,
    GroupName: "",
    GroupType: "A",
    Under: "",
    GroupDescription: "",
    IsTrading: "",
    ObjStatus: "",
    ParentGroupID: null,
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
    this.getSubGroupType();
    this.getSubGroupList();
    this.SubGroupForm = this.formBuilder.group({
      GroupName: ["", Validators.required],
      ParentGroupID: [null, Validators.required]
    });
  }


  getSubGroupType() {
    this.accountsService.getSubGroup().subscribe(
      response => {
        this.SubGroupType = response;
      }
    )
  }

  getSubGroupList() {
    this.accountsService.getSubGroupList().subscribe(
      response => {
        this.SubGroupList = response;
      }
    )
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  add(form) {
    if (form.value.GroupName == null || form.value.GroupName == "") {
      swal("Warning!", " Please enter sub-group name", "warning");
    }
    else if (form.value.ParentGroupID == null || form.value.ParentGroupID == "") {
      swal("Warning!", " Please select group name", "warning");
    }
    else {
      this.SubGroupModel.CompanyCode = this.ccode;
      this.SubGroupModel.BranchCode = this.bcode;
      this.SubGroupModel.ObjStatus = "O";
      var ans = confirm("Do you want to save??");
      if (ans) {
        this.accountsService.postSubGroup(this.SubGroupModel).subscribe(
          response => {
            swal("Saved!", "Sub-Group " + this.SubGroupModel.GroupName + " Saved", "success");
            this.getSubGroupList();
            this.ClearValues();
          }
        )
      }
    }
  }

  save(form) {
    if (form.value.GroupName == null || form.value.GroupName == "") {
      swal("Warning!", " Please enter sub-group name", "warning");
    }
    else if (form.value.ParentGroupID == null || form.value.ParentGroupID == "") {
      swal("Warning!", " Please select group name", "warning");
    }
    else {
      var ans = confirm("Do you want to save??");
      if (ans) {
        this.accountsService.putMasterGroup(this.SubGroupModel, this.SubGroupModel.ObjID).subscribe(
          response => {
            swal("Updated!", "Group " + this.SubGroupModel.GroupName + " Updated", "success");
            this.getSubGroupList();
            this.ClearValues();
          }
        )
      }
    }
  }

  ClearValues() {
    this.SubGroupForm.reset();
    this.EnableAdd = true;
    this.EnableSave = false;
    this.SubGroupModel = {
      ParentGroupName: "",
      ObjID: "",
      CompanyCode: "",
      BranchCode: "",
      GroupID: 0,
      GroupName: "",
      GroupType: "",
      Under: "",
      GroupDescription: "",
      IsTrading: "",
      ObjStatus: "",
      ParentGroupID: null,
      NewGroupCode: "",
      NewSubGroupCode: ""
    }
  }

  clear() {
    this.SubGroupForm.reset();
  }


  editField(arg) {
    if (arg.ObjStatus == "C") {
      swal("Warning!", "Sub-Group: " + arg.GroupName + " is closed", "warning");
    }
    else {
      this.EnableAdd = false;
      this.EnableSave = true;
      this.SubGroupModel = arg;
    }
  }

  openField(arg) {
    if (arg.ObjStatus == "O") {
      swal("Warning!", "Sub-Group: " + arg.GroupName + " is already opened", "warning");
    }
    else {
      var ans = confirm("Do you want to open Sub-Group: " + arg.GroupName + "?");
      if (ans) {
        arg.ObjStatus = "O";
        this.accountsService.putMasterGroup(arg, arg.ObjID).subscribe(
          response => {
            swal("Opened!", "Sub-Group: " + arg.GroupName + " opened", "success");
            this.getSubGroupList();
          }
        )
      }
    }
  }

  closeField(arg) {
    if (arg.ObjStatus == "C") {
      swal("Warning!", "Sub-Group: " + arg.GroupName + " is already closed", "warning");
    }
    else {
      var ans = confirm("Do you want to close the Sub-Group?");
      if (ans) {
        arg.ObjStatus = "C";
        this.accountsService.putMasterGroup(arg, arg.ObjID).subscribe(
          response => {
            swal("Closed!", "Sub-Group: " + arg.GroupName + " closed", "success");
            this.getSubGroupList();
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
        this.accountsService.getSubGroupList().subscribe(
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