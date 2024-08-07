import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { OnlineOrderManagementSystemService } from '../../online-order-management-system/online-order-management-system.service';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import { ToastrService } from 'ngx-toastr';
import swal from 'sweetalert';
declare var $: any;

@Component({
  selector: 'app-item-picking',
  templateUrl: './item-picking.component.html',
  styleUrls: ['./item-picking.component.css']
})
export class ItemPickingComponent implements OnInit {
  ccode: string;
  bcode: string;
  password: string;
  isReadOnly: boolean = false;
  EnableJson: boolean = false;
  PickListNumber: any = [];
  checkedRePick: boolean = false;

  AssignmentNo: string = null;
  @ViewChild("BarcodeNo", { static: true }) BarcodeNo: ElementRef;

  constructor(
    private fb: FormBuilder, private _appConfigService: AppConfigService,
    private _OMSserice: OnlineOrderManagementSystemService, private toastr: ToastrService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
    $('#printAssignmentModal').modal('hide');
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this.getPickListNo();
  }

  orderStage: string = "pick";

  getPickListNo() {
    this._OMSserice.getPickListNumber(this.orderStage).subscribe(
      response => {
        this.PickListNumber = response;
        this.PickListDetails = [];
        this.AssignmentNo = null;
        this.BarcodeNo.nativeElement.value = null;
        this.EnableAssignmentNo = false;
      }
    )
  }

  PickListDetails: any = [];
  EnableAssignmentNo: boolean = false;

  getPickListDetails() {
    if (this.AssignmentNo == null) {
      swal("Warning!", 'Please select the Assignment No', "warning");
    }
    else {
      this.PickListDetails = [];
      this._OMSserice.getPickListDetails(this.AssignmentNo).subscribe(
        response => {
          this.PickListDetails = response;
          this.CalcPickListDetails();
          this.EnableAssignmentNo = true;
        }
      )
    }
  }

  refreshPickListNo() {
    this.orderStage = "pick";
    this._OMSserice.getPickListNumber(this.orderStage).subscribe(
      response => {
        this.PickListNumber = response;
        this.checkedRePick = false;
      });
  }

  clear() {
    this.getPickListNo();
    this.PickListDetails = [];
    this.AssignmentNo = null;
    this.BarcodeNo.nativeElement.value = null;
    this.EnableAssignmentNo = false;
    this.pickedItems = 0;
    this.pendingItems = 0;
  }

  unassignBarcode(arg) {
    this._OMSserice.unassignbarcode(arg).subscribe(
      response => {
        //this.PickListDetails = response;
        //console.log(this.PickListDetails);
        this.refreshPickListNo();
        this.getPickListDetails();
        this.checkedRePick = false;
      }
    )
  }


  repick(e) {
    if (e.target.checked) {
      this.PickListDetails = [];
      this.orderStage = "re-pick";
      this.getPickListNo();
      this.pickedItems = 0;
      this.pendingItems = 0;
    }
    else {
      this.EnableAssignmentNo = false;
      this.pickedItems = 0;
      this.pendingItems = 0;
      this.PickListDetails = [];
      this.refreshPickListNo();
    }
  }

  assignBarcodeList: any = [];
  pickedItems: number = 0;
  pendingItems: number = 0;

  updateBarcodeNo(barcodeNo) {
    if (this.PickListDetails.length == 0) {
      swal("Warning!", 'Please add atleast one items to map the barcode', "warning");
    }
    else {
      this._OMSserice.assignBarcode(this.AssignmentNo, barcodeNo).subscribe(
        response => {
          this.assignBarcodeList = response;
          this.PickListDetails = this.assignBarcodeList;
          //swal("Saved!", "BarcodeNo: " + barcodeNo + " has been updated against Assignment No: " + this.AssignmentNo + " successfully", "success");
          this.toastr.success("BarcodeNo: " + barcodeNo + " has been updated against Assignment No: " + this.AssignmentNo + " successfully", 'Alert!');
          this.BarcodeNo.nativeElement.value = null;
          this.BarcodeNo.nativeElement.focus();
          this.CalcPickListDetails();
        },
        (err) => {
          if (err.status === 400) {
            const validationError = err.error;
            this.toastr.warning(validationError.description, 'Alert!');
          }
          this.BarcodeNo.nativeElement.value = null;
          this.BarcodeNo.nativeElement.focus();
        }
      )
    }
  }

  CalcPickListDetails() {
    this.pickedItems = 0;
    this.pendingItems = 0;
    for (let i = 0; i < this.PickListDetails.length; i++) {
      if (this.PickListDetails[i].BarcodeNo != null && this.PickListDetails[i].BarcodeNo != "") {
        ++this.pickedItems;
      }
      else {
        ++this.pendingItems;
      }
    }
  }

  printPickList: any;

  printPickListDets() {
    if (this.AssignmentNo == "null" || this.AssignmentNo == null) {
      swal("Warning!", 'Please select the Assignment No', "warning");
    }
    else {
      this._OMSserice.PrintPickList(this.AssignmentNo).subscribe(
        response => {
          this.printPickList = response;
          $('#printPickListModal').modal('show');
          $('#DisplayData').html(this.printPickList);
        }
      )
    }
  }

  outputData: any = [];

  updatePickList() {
    var ans = confirm("Do you want to Submit?");
    if (ans) {
      this._OMSserice.updatePickList(this.PickListDetails).subscribe(
        response => {
          this.outputData = response;
        }
      )
    }
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