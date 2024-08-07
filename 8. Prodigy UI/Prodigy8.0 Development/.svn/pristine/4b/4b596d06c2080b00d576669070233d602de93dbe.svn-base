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
  selector: 'app-stone-master',
  templateUrl: './stone-master.component.html',
  styleUrls: ['./stone-master.component.css']
})
export class StoneMasterComponent implements OnInit {

  StoneMasterForm: FormGroup;
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

  ]

  StoneMasterPostModel = {
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
    ObjStatus: "",
    ObjID: null,
    Code: "",
    Batch: "",
    Uom: "",
    HSN: "",
    StoneValue: null,
    GSTGroupCode: ""
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
    this.StoneMasterPostModel.CompanyCode = this.ccode;
    this.StoneMasterPostModel.BranchCode = this.bcode;
    this.GetAllCounter();
    this.StoneMasterPostModel.CompanyCode = this.ccode;
    this.StoneMasterPostModel.BranchCode = this.bcode;
    this.getDetailsByInitial.type = "DMD";
    this.getDetailsByInitial.StoneType = 'Precious';
    this.StoneMasterPostModel.StoneType = 'Precious';
    this.model.option == 'Precious';


    this.getListOfStone(this.getDetailsByInitial);
    this.StoneLsit;
    this.radioItems;
    this.model.option == 'Precious';
    this.StoneMasterForm = this.fb.group({
      frmCtrl_Gs: null,
      frmCtrl_GsName: null,
      frmCtrl_Code: null,
      frmCtrl_HSN: null,
      frmCtrl_Counter: null

    });
  }
  ListOfStone: any = [];
  getListOfStone(arg) {
    this._masterservice.getListOfStonesByType(arg.StoneGs, arg.StoneType).subscribe(
      Response => {
        this.ListOfStone = Response
      }
    )
  }


  Changed(arg) {
    // alert(arg.target.value);
    this.StoneMasterPostModel.Type = arg.target.value;
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
        this.ListOfStone = Response
      }
    )
  }

  ChangedOption(arg) {
    this.getDetailsByInitial.StoneType = "";
    if (arg === "Precious" || arg === "Semi Precious" || arg === "Ordinary") {
      this.getDetailsByInitial.StoneType = String(arg);
      this.StoneMasterPostModel.StoneType = String(arg);
      this._masterservice.getStoneMasterNewLIst(this.getDetailsByInitial.type, this.getDetailsByInitial.StoneType).subscribe(
        Response => {
          this.ListOfStone = Response

        }
      )

    }
    else if (arg === "Precious") {
      this.getDetailsByInitial.StoneType = String(arg);
      this.StoneMasterPostModel.StoneType = String(arg);
      this._masterservice.getStoneMasterNewLIst(this.getDetailsByInitial.type, this.getDetailsByInitial.StoneType).subscribe(
        Response => {
          this.ListOfStone = Response
          // console.log(this.NewList);
        }
      )

    }
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
      })
  }

  Add(form) {
    if (form.value.frmCtrl_GsName == "" || form.value.frmCtrl_GsName == null) {
      swal("Warning!", "Please Enter The Stone Name", "warning")
    }
    else {
      var ans = confirm("Do you want to Add ??");
      if (ans) {
        this._masterservice.PostStoneDetailsNew(this.StoneMasterPostModel).subscribe(
          Response => {
            swal("Success!", "Saved Successfully", "success");
            this.ListOfStone = [];
            this._masterservice.getStoneMasterNewLIst(this.StoneMasterPostModel.Type, this.StoneMasterPostModel.StoneType).subscribe(
              Response => {
                this.ListOfStone = Response
              },
              (err) => {
                if (err.status === 400) {
                  const validationError = err.error.description;
                  swal("Warning!", validationError, "warning");
                }

              }
            )
            this.StoneMasterForm.reset();
          }
        )
      }

    }
  }
  oldStoneName: string;
  edit(arg) {
    this.EnableAdd = false;
    this.EnableSave = true;
    this.StoneMasterPostModel = arg;
    this.oldStoneName = arg.StoneName;
  }
  save(form) {
    if (form.value.frmCtrl_GsName == "" || form.value.frmCtrl_GsName == null) {
      swal("Warning!", "Please Enter The Stone Name", "warning")
    }
    else {
      var ans = confirm("Do you want to Save ??");
      if (ans) {
        this._masterservice.EditStoneDetailsNew(this.StoneMasterPostModel, this.StoneMasterPostModel.Type, this.oldStoneName).subscribe(
          Response => {
            swal("Success!", "Saved Successfully", "success");
            this.ListOfStone = [];
            this._masterservice.getStoneMasterNewLIst(this.StoneMasterPostModel.Type, this.StoneMasterPostModel.StoneType).subscribe(
              Response => {
                this.ListOfStone = Response
              },
              (err) => {
                if (err.status === 400) {
                  const validationError = err.error.description;
                  swal("Warning!", validationError, "warning");
                }

              }
            )
            this.StoneMasterForm.reset();
          }
        )
      }
    }
  }



  Activate(arg) {
    var ans = confirm("Do you want to Activate ??");
    if (ans) {
      this._masterservice.ActivateStone(arg.Type, arg.StoneName).subscribe(
        Response => {
          swal("Success!", "Saved Successfully", "success");
          this.ListOfStone = [];
          this._masterservice.getStoneMasterNewLIst(arg.Type, arg.StoneType).subscribe(
            Response => {
              this.ListOfStone = Response
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
          this.ListOfStone = [];
          this._masterservice.getStoneMasterNewLIst(arg.Type, arg.StoneType).subscribe(
            Response => {
              this.ListOfStone = Response
            }
          )
        }
      )

    }
  }

  printStoneData: any = [];
  Print() {
    this.printStoneData = [];
    if (this.StoneMasterPostModel.Type != null) {
      var ans = confirm("Do you want to take print??");
      if (ans) {
        this._masterservice.getStoneMasterNewLIst(this.getDetailsByInitial.type, this.getDetailsByInitial.StoneType).subscribe(
          Response => {
            this.printStoneData = Response
            if (this.printStoneData != null) {
              $('#StoneMAster').modal('show');
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
    this.StoneMasterForm.reset()
    this.EnableAdd = true;
    this.EnableSave = false;
    this.radioItems = [];
    this.radioItems.push('Precious', 'Semi Precious', 'Ordinary')
    this.ListOfStone = [];
    this._masterservice.getStoneMasterNewLIst(this.StoneMasterPostModel.Type, this.StoneMasterPostModel.StoneType).subscribe(
      Response => {
        this.ListOfStone = Response
      }
    )
  }


}
