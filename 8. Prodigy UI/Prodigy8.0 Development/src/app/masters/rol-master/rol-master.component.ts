import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MastersService } from '../masters.service';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { Alert } from 'bootstrap';
declare var $: any;
@Component({
  selector: 'app-rol-master',
  templateUrl: './rol-master.component.html',
  styleUrls: ['./rol-master.component.css']
})
export class RolMasterComponent implements OnInit {
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  isReadOnly: boolean = false;
  ccode: string;
  bcode: string;
  EnableJson: boolean = false;
  password: string;
  ROLMasterForm: FormGroup;
  ROLMasterListData = {
    ObjID: "",
    CompanyCode: "",
    BranchCode: "",
    GSCode: null,
    CategoryCode: null,
    CategoryName: "",
    DesignCode: null,
    DesignName: "",
    CounterCode: null,
    CounterName: "",
    MinQty: 0,
    MinWt: 0.00,
    MaxQty: 0,
    MaxWt: 0.00,
    IsSved: "",
    UpdatdOn: null,
    UniqRowID: null
  }
  CounterListHeader = {
    gscode: "",
    itemcode: ""

  }
  constructor(private _appConfigService: AppConfigService,
    private fb: FormBuilder, private _router: Router, private _mastersService: MastersService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this.getGs();
    this.getDesign();
    // this.geList()
    this.ROLMasterForm = this.fb.group({
      frmCtrl_GS: null,
      frmCtrl_Item: null,
      frmCtrl_Desgin: null,
      frmCtrl_counter: null,
      frmCtrl_MinQty: null,
      frmCtrl_MinWt: null,
      frmCtrl_MaxQty: null,
      frmCtrl_MaxWt: null
    });
  }
  GSList: any = [];


  getGs() {
    this._mastersService.getROLGSitems().subscribe(
      Response => {
        this.GSList = Response;
      }
    )
  }
  //added

  ItemLists: any = [];
  getItemsByGs(arg) {
    this.CounterListHeader.gscode = arg.target.value;
    this.ROLMasterListData.GSCode = arg.target.value;
    this._mastersService.GetItemList(arg.target.value).subscribe(
      Response => {
      }
    )
  }

  selectedValue: any;
  getItemsCode(index) {
    var slecetedIndex = index - 1;
    this.handleChange1(slecetedIndex);
  }
  handleChange1(slecetedIndex) {
    this.selectedValue = this.ItemLists[slecetedIndex];
    this.ROLMasterListData.CategoryCode = this.selectedValue.ItemCode;
    this.CounterListHeader.itemcode = this.ROLMasterListData.CategoryCode;
    this.ROLMasterListData.CategoryName = this.selectedValue.ItemName;
    this.rolStockData(this.ROLMasterListData);
    this.getRolCounterList(this.CounterListHeader)



  }

  slectedDesign: any;
  DesignNameCode(index) {
    var slecetedIndex = index - 1;
    this.DesignNamehandleChange(slecetedIndex);
  }
  DesignNamehandleChange(slecetedIndex) {
    this.slectedDesign = this.DesignList[slecetedIndex];
    this.ROLMasterListData.DesignCode = this.slectedDesign.Code;
    this.ROLMasterListData.DesignName = this.slectedDesign.Name;

  }

  DesignList: any = [];
  getDesign() {
    this._mastersService.RolDesignName().subscribe(
      Response => {
        this.DesignList = Response;
        // console.log(this.DesignList);
      }
    )

  }

  CounterList: any = [];
  getRolCounterList(arg) {
    //alert('tyring to get counter details');
    this.CounterListHeader = arg;
    //  console.log(this.CounterListHeader);
    //alert(this.ROLMasterListData.GSCode);
    //alert(this.ROLMasterListData.DesignCode);
    this._mastersService.getRolCounter(this.CounterListHeader.gscode, this.CounterListHeader.itemcode).subscribe(
      Response => {
        this.CounterList = Response;
        // console.log(this.DesignList);
      }
    )
    // this.rolStockData(this.ROLMasterListData);
  }
  //counter drop down
  selectedCounter: any;
  CounterNameCode(index) {
    var selectedindex = index - 1;
    this.handleChange2(selectedindex);
  }
  handleChange2(slecetedIndex) {
    this.selectedCounter = this.CounterList[slecetedIndex];
    // console.log(this.selectedValue);
    this.ROLMasterListData.CounterCode = this.selectedCounter.Code;
    this.ROLMasterListData.CounterName = this.selectedCounter.Name;
    // alert(this.CounterListHeader.itemcode)
    // console.log(this.CounterListHeader);
    //this.getRolCounterList(this.CounterListHeader)

  }
  RolStock: any = [];
  rolStockData(arg) {
    this._mastersService.getRolStock(arg.GSCode, arg.CategoryCode).subscribe(
      Response => {
        this.RolStock = Response;
        // console.log(this.RolStock);
      }
    )
  }
  errors: any = []

  add(form) {
    if (form.value.frmCtrl_GS == null || form.value.frmCtrl_GS == "") {
      swal("Warning!", "Please Select GS", "warning");
    }
    else if (form.value.frmCtrl_Item == null || form.value.frmCtrl_Item == "") {
      swal("Warning!", "Please Select Item ", "warning");
    }
    else if (form.value.frmCtrl_Desgin == null || form.value.frmCtrl_Desgin == "") {
      swal("Warning!", "Please Select Design ", "warning");
    } else if (form.value.frmCtrl_counter == null || form.value.frmCtrl_counter == "") {
      swal("Warning!", "Please Select Counter", "warning");
    }
    else if (form.value.frmCtrl_MinQty == null || form.value.frmCtrl_MinQty == "") {
      swal("Warning!", "Please enter Minimum quantity", "warning");
    }
    else if (form.value.frmCtrl_MinWt == null || form.value.frmCtrl_MinWt == "") {
      swal("Warning!", "Please enter Minimum Weight", "warning");
    }
    else {
      this.ROLMasterListData.CompanyCode = this.ccode;
      this.ROLMasterListData.BranchCode = this.bcode;
      this.ROLMasterListData.IsSved = "N";
      var ans = confirm("Do you want to save??");
      if (ans) {
        this._mastersService.PostRolrders(this.ROLMasterListData).subscribe(
          response => {
            this.errors = response;
            swal("Saved!", "Saved " + this.ROLMasterListData.GSCode + " Saved", "success");
            this.ROLMasterForm.reset();
          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error.description;
              swal("Warning!", validationError, "warning");
            }
            else {
              this.errors.push('something went wrong!');
            }
            //this.clear();
          }
        )
      }
    }
  }
  editField(arg) {
    var ans = confirm("Do you want to Edit ?");
    if (ans) {
      this.EnableAdd = false;
      this.EnableSave = true;
      this.ROLMasterListData = arg;
      this.isReadOnly = true;
    }
    else {
      // this.EnableAdd = false;
      // this.EnableSave = true;
      // this.ROLMasterListData = arg;
      // this.isReadOnly = true;
    }

  }
  save(form) {
    if (form.value.frmCtrl_MinQty == null || form.value.frmCtrl_MinQty == "") {
      swal("Warning!", "Please enter Minimum quantity", "warning");
    }
    else if (form.value.frmCtrl_MinWt == null || form.value.frmCtrl_MinWt == "") {
      swal("Warning!", "Please enter Minimum Weight", "warning");
    }
    else {
      var ans = confirm("Do you want to save??");
      if (ans) {
        this._mastersService.putRolStock(this.ROLMasterListData).subscribe(
          response => {
            swal("Updated!", "Saved " + this.ROLMasterListData.GSCode + " Saved", "success");
            this.ROLMasterForm.reset();
            this.EnableAdd = true;
            this.EnableSave = false;

          }

        )
      }
    }
  }
  Delete(arg) {
    this.ROLMasterListData = arg;
    // alert(arg.ObjID)
    var ans = confirm("Do you want to Delete ?");
    if (ans) {
      this._mastersService.DeleteRolStock(this.ROLMasterListData.ObjID).subscribe(
        response => {
          swal("Deleted!", " Deleted successfully.", "success");
          // this.ROLMasterForm.reset();
          this.EnableAdd = true;
          this.EnableSave = false;
          this.rolStockData(this.ROLMasterListData);
        }
      )
    }
    else {

    }
  }

  PrintDetails: any = [];
  PrintTypeBasedOnConfig: any;
  Printing() {
    this._mastersService.printRolStock(this.ROLMasterListData.GSCode, this.ROLMasterListData.CategoryCode).subscribe(
      response => {
        this.PrintTypeBasedOnConfig = response;
        this.PrintDetails.push(atob(this.PrintTypeBasedOnConfig.Data));
        $('#printRolModal').modal('show');
        $('#DisplayData').html(this.PrintDetails);
      }
    )
  }
  clear() {
    this.ROLMasterForm.reset();
  }
  print() {
    let printContents, popupWin;
    printContents = document.getElementById('DisplayData').innerHTML;
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
        table tr td.borderLeft{
          border-left: 3px solid rgb(0, 0, 0) !important;
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
