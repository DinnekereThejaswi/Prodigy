import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MastersService } from '../masters.service';
import { AppConfigService } from '../../AppConfigService';
import { SalesManModel } from '../masters.model';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
declare var $: any;
@Component({
  selector: 'app-salesman',
  templateUrl: './salesman.component.html',
  styleUrls: ['./salesman.component.css']
})
export class SalesmanComponent implements OnInit {
  SalesMenData: any = [];
  ccode: string;
  bcode: string;
  SalesmanForm: FormGroup;
  Searchform: FormGroup;
  password: string;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  isReadOnly: boolean = false;
  EnableJson: boolean = false;
  //----for pagination
  totalItems: any;
  NoRecords: boolean = false;
  pagenumber: number = 1;
  top = 10;
  skip = (this.pagenumber - 1) * this.top;
  /////////searching by text///////
  searchText: string;
  ///////
  SalesmanListData: SalesManModel = {
    ObjID: "",
    CompanyCode: "",
    BranchCode: "",
    SalesManCode: "",
    SalesManName: "",
    ObjStatus: ""
  }
  constructor(private fb: FormBuilder, private _appConfigService: AppConfigService, private mastersService: MastersService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }
  ngOnInit() {

    this.getSalesMens();
    this.onPageChange(this.pagenumber);
    this.SalesmanForm = this.fb.group({
      SalesManCodes: ["", Validators.required],
      SalesManNames: ["", Validators.required]
    });
    this.Searchform = this.fb.group({
      companyCode: this.ccode,
      branchCode: this.bcode,
      searchText: null,
    })
  }

  SearchFilterData: any = {
    companyCode: null,
    branchCode: null,
    searchText: null
  }

  GetDataBySearch(form) {
    if (form.value.searchText == null) {
      swal("Warning!", "Please enter the data to search", "warning");
    }
    else {
      this.NoRecords = true;
      this.getSearchCustomer(this.top, this.skip, form);
    }
  }

  getSearchCustomer(top, skip, form) {
    this.SearchFilterData.companyCode = this.ccode;
    this.SearchFilterData.branchCode = this.bcode;
    this.mastersService.getSearchSalesMan(this.SearchFilterData.searchText).subscribe(
      response => {
        this.SalesMenData = response;
        this.totalItems = this.SalesMenData.length;

      }
    );
  }
  getSearchedCount(UserData) {
    this.mastersService.getSearchCount(UserData).subscribe(
      Data => {
        this.totalItems = Data;
      }
    )
  }

  ClearSearch() {
    this.Searchform.reset();
    this.getSalesMens();

  }

  ///////////////above for search by text

  getSalesMens() {
    this.onPageChange(this.pagenumber);
    this.mastersService.GetSalesMen().subscribe(
      response => {
        this.SalesMenData = response;
        this.getSearchCount();
      }
    )
  }
  errors: any = []
  PostedSalesManData: any;
  add(form) {
    if (form.value.SalesManCodes == null || form.value.SalesManCodes == "") {
      swal("Warning!", "Please enter Salesman code", "warning");
    }
    else if (form.value.SalesManNames == null || form.value.SalesManNames == "") {
      swal("Warning!", "Please enter Salesman Name", "warning");
    }
    else {
      this.SalesmanListData.CompanyCode = this.ccode;
      this.SalesmanListData.BranchCode = this.bcode;
      this.SalesmanListData.ObjStatus = "O";

      var ans = confirm("Do you want to save??");
      if (ans) {
        this.mastersService.PostSalesMen(this.SalesmanListData).subscribe(
          response => {
            this.PostedSalesManData = response;
            swal("Saved!", "Saved " + this.SalesmanListData.SalesManName + " Saved", "success");
            this.ClearValues();
            this.onPageChange(this.pagenumber);
          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error.description;
              swal("Warning!", validationError, "warning");
            }
            else {
              this.errors.push('something went wrong!');
            }
            this.clear();
          }
        )
      }
    }
  }

  save(form) {
    if (form.value.SalesManNames == null || form.value.SalesManNames == "") {
      swal("Warning!", "Please enter Salesman Name", "warning");
    }
    else {
      var ans = confirm("Do you want to save??");
      if (ans) {
        this.mastersService.putSalesMen(this.SalesmanListData.SalesManCode, this.SalesmanListData).subscribe(
          response => {
            swal("Updated!", "Saved " + this.SalesmanListData.SalesManName + " Saved", "success");
            this.ClearValues();
            this.onPageChange(this.pagenumber);
          }

        )
      }
    }



  }

  ClearValues() {
    this.EnableAdd = true;
    this.EnableSave = false;
    this.clear();
    this.SalesmanForm.reset();
    this.SalesmanListData = {
      ObjID: "",
      CompanyCode: "",
      BranchCode: "",
      SalesManCode: "",
      SalesManName: "",
      ObjStatus: ""
    }
  }
  editField(arg) {
    if (arg.ObjStatus == "C") {
      swal("Warning!", "SalesMan: " + arg.SalesManName + " is Closed. Open to edit", "warning");
    }
    else {

      this.EnableAdd = false;
      this.EnableSave = true;
      this.SalesmanListData = arg;
      this.isReadOnly = true;
    }

  }

  openField(arg) {
    if (arg.ObjStatus == "O") {
      swal("Warning!", "SalesMan: " + arg.SalesManName + " is already Open", "warning");
    }
    else {
      var ans = confirm("Do you want to Open SalesMan: " + arg.SalesManName + "?");
      if (ans) {
        arg.ObjStatus = "O";
        this.mastersService.SalesManStatus(arg, arg.SalesManCode).subscribe(
          response => {
            swal("Updated!", "SalesMan: " + arg.SalesManName + " Open", "success");
          }
        )
      }
    }
  }


  closeField(arg) {
    if (arg.ObjStatus == "C") {
      swal("Warning!", "SalesMan: " + arg.SalesManName + " is already closed", "warning");
    }
    else {
      var ans = confirm("Do you want to close the SalesMan: " + arg.SalesManName + "?");
      if (ans) {
        arg.ObjStatus = "C";
        this.mastersService.SalesManStatus(arg, arg.SalesManCode).subscribe(
          response => {
            swal("Updated!", "SalesMan: " + arg.SalesManName + " close", "success");
          }
        )
      }
    }
  }

  clear() {
    this.isReadOnly = false;
    this.searchText = null;
    this.mastersService.GetSalesMen();
    this.SalesmanForm.reset();
  }

  onPageChange(p: number) {
    this.pagenumber = p;
    const skipno = (this.pagenumber - 1) * this.top;
    this.mastersService.GetTopValueSalesMen(this.top, skipno).subscribe
      (
        response => {
          this.SalesMenData = response;
        }
      )
  }
  GetSalesManTopValues() {
    this.mastersService.GetTopValueSalesMen(this.top, this.skip).subscribe(
      response => {
        this.SalesMenData = response;
      }
    );
  }
  getSearchCount() {
    this.mastersService.GetTotalSalesManRecordCount().subscribe(
      Data => {
        this.totalItems = Data;
        this.totalItems = this.totalItems.RecordCount;
        this.GetSalesManTopValues();

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

  printData: any = [];
  Print() {
    this.printData = [];
    if (this.printData != null) {
      var ans = confirm("Do you want to take print??");
      if (ans) {
        this.mastersService.GetSalesMen().subscribe(
          response => {
            this.printData = response
            if (this.printData != null) {
              $('#SalesManDetailsTab').modal('show');
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
