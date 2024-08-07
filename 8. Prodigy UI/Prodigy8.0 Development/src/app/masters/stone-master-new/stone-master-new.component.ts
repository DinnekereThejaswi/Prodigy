import { Component, OnInit, Type } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MastersService } from './../masters.service';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { ToastrService } from 'ngx-toastr';
import { AppConfigService } from '../../AppConfigService';
declare var $: any;
@Component({
  selector: 'app-stone-master-new',
  templateUrl: './stone-master-new.component.html',
  styleUrls: ['./stone-master-new.component.css']
})
export class StoneMasterNewComponent implements OnInit {

  StoneMasterFormNew: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  isReadOnly: boolean = false;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  EnableJson: boolean = false;
  radioItems: Array<string>;
  model = { option: 'Precious' };
  StoneLsit = [{
    Code: "DMD", Name: "DIAMOND"
  },
  {
    Code: "OD", Name: "OLD DIAMOND"
  },
  {
    Code: "OST", Name: "OLD STONE"
  },
  {
    Code: "DL", Name: "LOOSE DIAMOND"
  },
  {
    Code: "STN", Name: "STONES"
  }
  ];
  uom = [
    { "code": "P", "name": "piece" },
    { "code": "C", "name": "Carat" }]
  StoneMasterNewListData =
    {
      CompanyCode: "",
      BranchCode: "",
      Type: null,
      StoneType: "",
      StoneName: "",
      CounterCode: null,
      BrandName: "",
      Color: "",
      Cut: "",
      Clarity: "",
      Status: "Active",
      Code: "",
      Batch: "",
      Uom: null,
      HSN: null,
      StoneValue: null,
      GSTGroupCode: null
    }
  getDetailsByInitial = {
    type: "",
    StoneType: ""
  }

  constructor(private fb: FormBuilder, private _masterservice: MastersService, private _router: Router,
    private _appConfigService: AppConfigService) {
    this.radioItems = ['Precious', 'Semi Precious', 'Ordinary'];
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);

  }

  ngOnInit() {
    this.GetAllCounter();
    this.getGST();
    this.hsnList();
    this.uom;
    this.StoneMasterNewListData.Uom = "C";
    this.StoneMasterNewListData.CompanyCode = this.ccode;
    this.StoneMasterNewListData.BranchCode = this.bcode;
    this.getDetailsByInitial.type = "DMD";
    this.getDetailsByInitial.StoneType = 'Precious';
    this.StoneMasterNewListData.StoneType = 'Precious';
    this.model.option == 'Precious';
    this.getListOfTheStones();
    this.StoneMasterFormNew = this.fb.group({
      Gs: null,
      UOM: null,
      Name: null,
      batch: null,
      code: null,
      Counter: null,
      Brand: null,
      GSTtype: null,
      hsn: null
    });

  }
  Add(form) {
    if (form.value.Name == "" || form.value.Name == null) {
      swal("Warning!", "Please Enter The Stone Name", "warning")
    }
    else {
      var ans = confirm("Do you want to Add ??");
      if (ans) {
        this._masterservice.PostStoneDetailsNew(this.StoneMasterNewListData).subscribe(
          Response => {
            swal("Success!", "Saved Successfully", "success");
            this.NewList = [];
            this._masterservice.getStoneMasterNewLIst(this.StoneMasterNewListData.Type, this.StoneMasterNewListData.StoneType).subscribe(
              Response => {
                this.NewList = Response
              },
              (err) => {
                if (err.status === 400) {
                  const validationError = err.error.description;
                  swal("Warning!", validationError, "warning");
                }

              }
            )
            this.StoneMasterFormNew.reset();
          }
        )
      }

    }
  }
  oldStoneName: string;
  edit(arg) {
    this.EnableAdd = false;
    this.EnableSave = true;
    this.StoneMasterNewListData = arg;
    this.oldStoneName = arg.StoneName;
  }
  save(form) {
    if (form.value.Name == "" || form.value.Name == null) {
      swal("Warning!", "Please Enter The Stone Name", "warning")
    }
    else {
      var ans = confirm("Do you want to Save ??");
      if (ans) {
        this._masterservice.EditStoneDetailsNew(this.StoneMasterNewListData, this.StoneMasterNewListData.Type, this.oldStoneName).subscribe(
          Response => {
            swal("Success!", "Saved Successfully", "success");
            this.NewList = [];
            this._masterservice.getStoneMasterNewLIst(this.StoneMasterNewListData.Type, this.StoneMasterNewListData.StoneType).subscribe(
              Response => {
                this.NewList = Response
              },
              (err) => {
                if (err.status === 400) {
                  const validationError = err.error.description;
                  swal("Warning!", validationError, "warning");
                }

              }
            )
            this.StoneMasterFormNew.reset();
          }
        )
      }
    }
  }


  Changed(arg) {
    this.StoneMasterNewListData.Type = arg.target.value;
    this.getDetailsByInitial.type = arg.target.value;
    if (arg.target.value === 'DMD' || arg.target.value === 'OD' || arg.target.value === 'DL') {
      this.radioItems = [];
      this.radioItems.push('Precious');
      this.model.option == 'Precious';

    }
    else if (arg.target.value === 'OST' || arg.target.value === 'STN') {
      this.radioItems = [];
      this.radioItems.push('Precious', 'Semi Precious', 'Ordinary');
      this.model.option == 'Precious';
    }
    this._masterservice.getStoneMasterNewLIst(arg.target.value, this.model.option).subscribe(
      Response => {
        this.NewList = Response
      }
    )
  }

  ChangedOption(arg) {
    this.getDetailsByInitial.StoneType = "";
    if (arg === "Precious" || arg === "Semi Precious" || arg === "Ordinary") {
      this.getDetailsByInitial.StoneType = String(arg);
      this.StoneMasterNewListData.StoneType = String(arg);
      this._masterservice.getStoneMasterNewLIst(this.getDetailsByInitial.type, this.getDetailsByInitial.StoneType).subscribe(
        Response => {
          this.NewList = Response
          // console.log(this.NewList);
        }
      )

    }
    else if (arg === "Precious") {
      this.getDetailsByInitial.StoneType = String(arg);
      this.StoneMasterNewListData.StoneType = String(arg);
      this._masterservice.getStoneMasterNewLIst(this.getDetailsByInitial.type, this.getDetailsByInitial.StoneType).subscribe(
        Response => {
          this.NewList = Response
          // console.log(this.NewList);
        }
      )

    }
  }
  NewList: any = [];
  getListOfTheStones() {
    this._masterservice.getStoneMasterNewLIst(this.getDetailsByInitial.type, this.getDetailsByInitial.StoneType).subscribe(
      Response => {
        this.NewList = Response
        // console.log(this.NewList);
      }
    )
  }
  getStatusColor(ObjStatus) {
    switch (ObjStatus) {
      case 'Active':
        return 'green';
      case 'Closed':
        return 'Red';
    }
  }
  counterList: any = [];
  GetAllCounter() {
    this._masterservice.getCounterList().subscribe(
      Response => {
        this.counterList = Response;
        // console.log(this.counterList);
      })
  }
  gst: any = [];
  getGST() {
    this._masterservice.getGSTList().subscribe(
      response => {
        this.gst = response;
        // console.log(this.gst);
      }
    )
  }
  HSN: any = [];
  hsnList() {
    this._masterservice.getHSN().subscribe(
      Response => {
        this.HSN = Response;
      })
  }
  Activate(arg) {
    var ans = confirm("Do you want to Activate ??");
    if (ans) {
      this._masterservice.ActivateStone(arg.Type, arg.StoneName).subscribe(
        Response => {
          swal("Success!", "Saved Successfully", "success");
          this.NewList = [];
          this._masterservice.getStoneMasterNewLIst(arg.Type, arg.StoneType).subscribe(
            Response => {
              this.NewList = Response
              // console.log(this.NewList);
            }
          )
        }
      )
    }
  }
  Deactivate(arg) {
    var ans = confirm("Do you want to Deactivate ??");
    if (ans) {
      this._masterservice.DeactivateStone(arg.Type, arg.StoneName).subscribe(
        Response => {
          swal("Success!", "Saved Successfully", "success");
          this.NewList = [];
          this._masterservice.getStoneMasterNewLIst(arg.Type, arg.StoneType).subscribe(
            Response => {
              this.NewList = Response
              // console.log(this.NewList);
            }
          )
        }
      )

    }
  }


  printStoneData: any = [];
  Print() {
    this.printStoneData = [];
    if (this.StoneMasterNewListData.Type != null) {
      var ans = confirm("Do you want to take print??");
      if (ans) {
        this._masterservice.getStoneMasterNewLIst(this.getDetailsByInitial.type, this.getDetailsByInitial.StoneType).subscribe(
          Response => {
            this.printStoneData = Response
            if (this.printStoneData != null) {
              $('#StoneMAsterNew').modal('show');
            }
          }
        )
      }
    }
    else {
      swal("Warning!", "Please Select Stone", "warning")
    }
  }
  printStonedData() {
    if (this.printStoneData !== null) {
      let StoneData, popupWin;
      StoneData = document.getElementById('DisplayntStoneData').innerHTML;
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
      ${StoneData}</body>
        </html>`
      );
      popupWin.document.close();
    }
  }
  Clear() {
    this.StoneMasterFormNew.reset()
    this.EnableAdd = true;
    this.EnableSave = false;
    this.radioItems = [];
    this.radioItems.push('Precious', 'Semi Precious', 'Ordinary');
    this.NewList = [];
    this._masterservice.getStoneMasterNewLIst(this.StoneMasterNewListData.Type, this.StoneMasterNewListData.StoneType).subscribe(
      Response => {
        this.NewList = Response
      }
    )
  }
}
