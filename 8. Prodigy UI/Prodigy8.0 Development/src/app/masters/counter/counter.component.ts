import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import { MastersService } from '../masters.service';
import swal from 'sweetalert';
declare var $: any;
import { Counter } from '../masters.model';
@Component({
  selector: 'app-counter',
  templateUrl: './counter.component.html',
  styleUrls: ['./counter.component.css']
})
export class CounterComponent implements OnInit {
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  isReadOnly: boolean = false;
  pagenumber: number = 1;
  top = 10;
  Modeltop = 10;
  EnableJson: boolean = false;
  skip = (this.pagenumber - 1) * this.top;
  mainLocation: any = [
    {
      "code": "ALL",
      "name": "NA",
      "obj_status": "O"
    },
    {
      "code": "GC",
      "name": "Gift Counter",
      "obj_status": "O"
    },
    {
      "code": "ALL",
      "name": "ALL",
      "obj_status": "C"
    }
  ];
  ccode: string;
  bcode: string;
  password: string;
  CounterForm: FormGroup;
  CounterHeader: Counter = {
    branchcode: null,
    companycode: null,
    CounterCode: null,
    CounterName: null,
    Maincountercode: null,
    ObjID: null,
    ObjectStatus: null,
    SubCounterName: null,
    SubCountercode: null

    // ,
    // tear_wt:10,
    // other_wt:10,

  }
  constructor(private _appConfigService: AppConfigService, private _masterservice: MastersService, private fb: FormBuilder) {

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
    this.getAllCounterList();

    this.CounterHeader.companycode = this.ccode;
    this.CounterHeader.branchcode = this.bcode;
    this.CounterForm = this.fb.group({
      frmCtrl_MaincounterCode: [null, Validators.required],
      frmCtrl_CounterCode: [null, Validators.required],
      frmCtrl_CounterName: [null, Validators.required],
      frmCtrl_TearWt: [null, Validators.required],
      frmCtrl_OtherWt: [null, Validators.required]
    });
  }


  CountersTable: any = [];
  MainLocation: any = [];
  getMainCounterList() {
    this._masterservice.getMaincounters().subscribe(
      Response => {
        this.MainLocation = Response;
      }
    )
  }
  ListOfAllCounter: any = [];
  getAllCounterList() {
    this._masterservice.getCounterList().subscribe(
      resposne => {
        this.ListOfAllCounter = resposne;
      }
    )
  }

  MainCode: string = "";
  ByMainCounter(arg) {

    this.MainCode = arg.target.value
    this._masterservice.GETCounterDetailsByCode(this.MainCode).subscribe(
      resposne => {
        this.ListOfAllCounter = resposne;
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

    if (form.value.frmCtrl_MaincounterCode == null || form.value.frmCtrl_MaincounterCode === '') {
      swal("!Warning", "Please select Main Counter", "warning");
    }
    else if (form.value.frmCtrl_CounterCode == null || form.value.frmCtrl_CounterCode === '') {
      swal("!Warning", "Please enter Sub Counter code", "warning");
    }
    else if (form.value.frmCtrl_CounterName == null || form.value.frmCtrl_CounterName === '') {
      swal("!Warning", "Please select Sub Counter Name", "warning");
    }
    else {

      this.CounterHeader.companycode = this.ccode;
      this.CounterHeader.branchcode = this.bcode;
      this.CounterHeader.ObjectStatus = 'O';
      var ans = confirm("Do you want to save??");
      if (ans) {
        this._masterservice.PostDetails(this.CounterHeader).subscribe(
          response => {
            swal("Success!", "Counter: " + this.CounterHeader.CounterName + "Saved", "success");
            this.getAllCounterList();
            this.CounterForm.reset();

          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error;
              swal("!Warning", validationError, "warning");
            }
            else {
              this.errors.push('something went wrong!');
            }
            // this.clear();
          }
        )
      }
    }
    this.getAllCounterList();

  }
  editField(arg) {
    if (arg.ObjectStatus == "C") {
      swal("!Warning", "Counter: " + arg.CounterName + " is Closed. Open to edit", "warning");
    }
    else {
      this.EnableAdd = false;
      this.EnableSave = true;
      this.CounterHeader = arg;
      this.isReadOnly = true;
    }
  }


  save(form) {
    if (form.value.frmCtrl_CounterName == null || form.value.frmCtrl_CounterName == "") {
      swal("!Warning", "Please enter  Counter Name", "warning");
    }
    else {
      var ans = confirm("Do you want to save??");
      if (ans) {
        this._masterservice.ModifyOrUpdateCounterByPUT(this.CounterHeader.ObjID, this.CounterHeader).subscribe(
          response => {
            swal("Updated!", "Saved " + this.CounterHeader.CounterName + " Saved", "success");
            this.CounterForm.reset();
            this.getMainCounterList();
            this.isReadOnly = false;
            this.EnableAdd = true;
            this.EnableSave = false;
          }

        )
      }
    }
    this.getAllCounterList();
  }

  openField(arg) {
    this.CounterHeader = arg;
    if (arg.ObjectStatus == "O") {
      swal("Warning!", "CounterName: " + arg.CounterName + " is already Open", "warning");
    }
    else {
      var ans = confirm("Do you want to Open CounterName: " + arg.CounterName + "?");
      if (ans) {
        this.CounterHeader.ObjectStatus = "O";
        this._masterservice.ModifyOrUpdateCounterByPUT(this.CounterHeader.ObjID, this.CounterHeader).subscribe(
          response => {
            swal("Updated!", "CounterName: " + arg.CounterName + " Opened", "success");
          }
        )
      }
      this.getAllCounterList();
    }
    this.getAllCounterList();
  }

  closeField(arg) {
    this.CounterHeader = arg;
    if (arg.ObjectStatus == "C") {
      swal("Warning!", "CounterName: " + arg.CounterName + " is already close", "warning");
    }
    else {
      var ans = confirm("Do you want to close the CounterName: " + arg.CounterName + "?");
      if (ans) {
        this.CounterHeader.ObjectStatus = "C";
        this._masterservice.ModifyOrUpdateCounterByPUT(this.CounterHeader.ObjID, this.CounterHeader).subscribe(
          response => {
            swal("Updated!", "CounterName: " + arg.CounterName + "closed", "success");
          }
        )
      }
      this.getAllCounterList();
    }
    this.getAllCounterList();
  }
  clear() {
    this.CounterForm.reset();
    this.isReadOnly = false;
    this.getMainCounterList();
    this.getAllCounterList();
  }
  ClearValues() {
    this.EnableAdd = true;
    this.EnableSave = false;
    this.clear();
    this.CounterForm.reset();
    this.CounterHeader = {
      branchcode: "",
      companycode: "",
      CounterCode: "",
      CounterName: "",
      Maincountercode: "",
      ObjID: "",
      ObjectStatus: "",
      SubCounterName: "",
      SubCountercode: ""
    }
    this.getAllCounterList();
  }
  printData: any = [];
  Print() {
    this.printData = [];
    if (this.printData != null) {
      var ans = confirm("Do you want to take print??");
      if (ans) {
        this._masterservice.getCounterList().subscribe(
          resposne => {
            this.printData = resposne
            if (this.printData != null) {
              $('#CounterDetailsTab').modal('show');
            }
          }
        )
      }
    }
  }
  print() {
    if (this.printData !== null) {
      let Details, popupWin;
      Details = document.getElementById('TableData').innerHTML;
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
      ${Details}</body>
        </html>`
      );
      popupWin.document.close();
    }
  }

}
