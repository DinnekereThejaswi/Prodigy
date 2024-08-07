import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MastersService } from '../masters.service';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { GSentrymodel } from '../masters.model';
declare var $: any;
@Component({
  selector: 'app-gs-entry',
  templateUrl: './gs-entry.component.html',
  styleUrls: ['./gs-entry.component.css']
})
export class GsEntryComponent implements OnInit {
  ccode: string;
  bcode: string;
  password: string;
  radioItems: Array<string>;
  seqNumber: number;
  EnableJson: boolean = false;
  chkbxRechked: boolean = false;
  uom = [{ "code": "W", "name": "weight" },
  { "code": "P", "name": "pieces" },
  { "code": "C", "name": "Carat" }]

  iRadioButtonItem = [
    { code: 'S', name: 'Sales' },
    { code: 'P', name: 'Purchase' },
    { code: 'R', name: 'Repair' }
  ];
  //

  //
  chkbxchked: boolean = false;
  GsEntryListData: GSentrymodel = {
    BillType: null,
    BranchCode: null,
    CTax: null,
    CommodityCode: null,
    CompanyCode: null,
    DisplayOrder: null,
    EduCess: null,
    ExciseDuty: null,
    GSName: null,
    GSTGoodsGroupCode: null,
    GSTServicesGroupCode: null,
    GsCode: null,
    HSN: null,
    HighEle: null,
    ITax: null,
    IsStone: null,
    ItemLevel1ID: 0,
    ItemLevel1Name: null,
    Karat: null,
    MeasureType: null,
    MetalType: null,
    ObjID: null,
    ObjectStatus: null,
    OpeningGwt: null,
    OpeningGwtValue: null,
    OpeningNwt: null,
    OpeningNwtValue: 0,
    OpeningUnits: null,
    Purity: null,
    STax: 0,
    Tax: 0,
    Tcs: 0,
    TcsPerc: null,
    UpdateOn: null
  }
  model = { option: 'Sales' };
  EnableAdd: boolean = true;
  EnableSave: boolean = true; //f
  isReadOnly: boolean = false; //f
  checkItems: Array<string>;
  GsEntryFrom: FormGroup;
  constructor(private fb: FormBuilder,
    private _router: Router, private _mastersService: MastersService,
    private _appConfigService: AppConfigService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
    this.checkItems = ['Sales', 'Purchase', 'Repair', 'Packing', 'Gift'];

  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.radioItems = ['Sales', 'Purchase', 'Repair', 'Packing', 'GiftItems'];
  }



  ngOnInit() {
    this.StockGroupList;
    this.EnableAdd = false;
    this.GetGSListItem();
    this.getStockList();
    this.getKaratData();
    this.GoodsType();
    this.ServiceType();
    this.hsn();
    this.GsEntryFrom = this.fb.group({
      FormGSCode: [null, Validators.required],
      FormGSName: [null, Validators.required],
      Formkarat: [null, Validators.required],
      FormMeasureType: [null, Validators.required],
      FormStockName: [null, Validators.required],
      FormPurtiy: [null, Validators.required],
      FormIsStoneAttached: [null, Validators.required],
      FormIsClosed: [null, Validators.required],
      FormGoodsTye: [null, Validators.required],
      FormServiceType: [null, Validators.required],
      FormHsnCode: [null, Validators.required],
      FormOpnPiece: [null, Validators.required],
      FormOpnValue: [null, Validators.required],
      FormOpnGWT: [null, Validators.required],
      FormOpnNWT: [null, Validators.required],
      FormSEQNo: [null, Validators.required],
      FormCommodityCode: [null, Validators.required],
      FormIsSales: ["", Validators.required],
      FormIsPurchase: ["", Validators.required],
      FormIsRepair: ["", Validators.required],
      FormIsPacking: ["", Validators.required],
      FormIsGift: ["", Validators.required],
      FormBillType: ["", Validators.required],
    });
  }
  GSList: any = [];
  GetGSListItem() {
    this._mastersService.GetGSListItems().subscribe(
      Response => {
        this.GSList = Response;
      }
    )
  }


  StockGroupList: any = [];
  getStockList() {
    this._mastersService.getStockGroup().subscribe(
      Response => {
        this.StockGroupList = Response;
        this.StockGroupList = this.StockGroupList.filter(p => p.ObjectStatus === 'O');
      }
    )
  }
  KaratList: any = [];
  getKaratData() {
    this._mastersService.GETKaratsToTable().subscribe(
      Response => {
        this.KaratList = Response;
      })
  }
  SelectedStockGroupCode: string = "";
  StockGroupChanged(arg) {
    this.SelectedStockGroupCode = arg.target.value;
  }
  checkkboxchng(e) {
    if (e.target.checked == true) {
      this.GsEntryListData.IsStone = "Y";
    }
    if (e.target.checked == false) {
      this.GsEntryListData.IsStone = "N";
    }
  }
  CheckIsSales: String = "";

  IsSales(e) {
    if (e.target.checked == true) {
      this.GsEntryListData.BillType = "S";
    }

  }
  IsPurchase(e) {
    if (e.target.checked == true) {
      this.GsEntryListData.BillType = "P";
    }
  }
  IsRepair(e) {
    if (e.target.checked) {
      this.GsEntryListData.BillType = "R";
    }
  }
  IsPacking(e) {
    if (e.target.checked == true) {
      this.GsEntryListData.BillType = "Z";
    }
  }
  ISGiftItems(e) {
    if (e.target.checked == true) {
      this.GsEntryListData.BillType = "G";
    }
  }

  goodsType: any = [];
  GoodsType() {
    this._mastersService.GetGoodsType().subscribe(
      Response => {
        this.goodsType = Response;
      })
  }
  serviceType: any = [];
  ServiceType() {
    this._mastersService.getServiceType().subscribe(
      Response => {
        this.serviceType = Response;
      })
  }
  HSN: any = [];
  hsn() {
    this._mastersService.getHSN().subscribe(
      Response => {
        this.HSN = Response;
      })

  }
  errors = [];
  AddNewGS(form) {
    if (form.value.FormGSCode == null || form.value.FormGSCode == "") {
      swal("Warning!", "Please enter GS Code", "warning");
    }
    else if (form.value.FormGSName == null || form.value.FormGSName == "") {
      swal("Warning!", "Please enter GS Name", "warning");
    }
    else if (form.value.FormMeasureType == null || form.value.FormMeasureType == "") {
      swal("Warning!", "Please Select MeasureType", "warning");
    }
    else if (form.value.FormStockName == null || form.value.FormStockName == "") {
      swal("Warning!", "Please select Stock Group", "warning");
    }

    else if (form.value.FormGoodsTye == null || form.value.FormGoodsTye == "") {
      swal("Warning", "Please Select Goods Type", "warning");
    }
    else if (form.value.FormServiceType == null || form.value.FormServiceType == "") {
      swal("Warning!", "Please select Service Type", "warning");
    }
    else if (form.value.FormHsnCode == null || form.value.FormHsnCode == "") {
      swal("Warning!", "Please Select HSN", "warning");
    }
    else {
      this.GsEntryListData.CompanyCode = this.ccode;
      this.GsEntryListData.BranchCode = this.bcode;
      this.GsEntryListData.ObjectStatus = "O";
      var ans = confirm("Do you want to Add??");
      if (ans) {
        this._mastersService.PostGsEntry(this.GsEntryListData).subscribe(
          response => {
            swal("Saved!", "Saved " + this.GsEntryListData.ItemLevel1Name + " Saved", "success");
            this.GSList = [];
            this.GetGSListItem();
            this.GsEntryFrom.reset();

          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error.description;
              swal("Warning!", validationError, "warning");
            }
            else {
              this.errors.push('something went wrong!');
            }
          }
        )
      }
    }
  }
  modifyModel() {
    this.GsEntryFrom.reset();
    $('#ModifyGSModel').modal('show');
    this.EnableAdd = false;
    this.GetGSListItem();
  }
  isChecked: boolean = false
  LoadBackGsDetails(arg) {
    this.GsEntryListData.BillType = arg.BillType;
    this.GsEntryListData.BranchCode = arg.BranchCode;
    this.GsEntryListData.CTax = arg.CTax;
    this.GsEntryListData.CommodityCode = arg.CommodityCode;
    this.GsEntryListData.CompanyCode = arg.CompanyCode;
    this.GsEntryListData.DisplayOrder = arg.DisplayOrder;
    this.GsEntryListData.EduCess = arg.EduCess;
    this.GsEntryListData.ExciseDuty = arg.ExciseDuty;
    this.GsEntryListData.GSName = arg.GSName;
    this.GsEntryListData.GSTGoodsGroupCode = arg.GSTGoodsGroupCode;
    this.GsEntryListData.GSTServicesGroupCode = arg.GSTServicesGroupCode;
    this.GsEntryListData.GsCode = arg.GsCode;
    this.GsEntryListData.HSN = arg.HSN;
    this.GsEntryListData.HighEle = arg.HighEle;
    this.GsEntryListData.ITax = arg.ITax;
    this.GsEntryListData.IsStone = arg.IsStone;
    this.GsEntryListData.ItemLevel1ID = arg.ItemLevel1ID;
    this.GsEntryListData.ItemLevel1Name = arg.ItemLevel1Name;
    this.GsEntryListData.Karat = arg.Karat;
    this.GsEntryListData.MeasureType = arg.MeasureType;
    this.GsEntryListData.MetalType = arg.MetalType;
    this.GsEntryListData.ObjID = arg.ObjID;
    this.GsEntryListData.ObjectStatus = arg.ObjectStatus;
    this.GsEntryListData.OpeningGwt = arg.OpeningGwt;
    this.GsEntryListData.OpeningGwtValue = arg.OpeningGwtValue;
    this.GsEntryListData.OpeningNwt = arg.OpeningNwt;
    this.GsEntryListData.OpeningNwtValue = arg.OpeningNwtValue;
    this.GsEntryListData.OpeningUnits = arg.OpeningUnits;
    this.GsEntryListData.Purity = arg.Purity;
    this.GsEntryListData.STax = arg.STax;
    this.GsEntryListData.Tax = arg.Tax;
    this.GsEntryListData.Tcs = arg.Tcs;
    this.GsEntryListData.TcsPerc = arg.TcsPerc;
    this.GsEntryListData.UpdateOn = arg.UpdateOn;
    this.EnableAdd = true;
    this.EnableSave = false;
    this.isReadOnly = true;

  }
  clear() {
    this.GsEntryFrom.reset();
    this.isReadOnly = false;
    this.EnableAdd = false;
    this.EnableSave = true;
  }
  UpdateGSList(form) {
    if (form.value.FormGSCode == null || form.value.FormGSCode == "") {
      swal("Warning!", "Please enter GS Code", "warning");
    }
    else if (form.value.FormGSName == null || form.value.FormGSName == "") {
      swal("Warning!", "Please enter GS Name", "warning");
    }
    else if (form.value.FormMeasureType == null || form.value.FormMeasureType == "") {
      swal("Warning!", "Please  Select MeasureTyp", "warning");
    }
    else if (form.value.FormStockName == null || form.value.FormStockName == "") {
      swal("Warning!", "Please select Stock Groupt", "warning");
    }
    else if (form.value.FormGoodsTye == null || form.value.FormGoodsTye == "") {
      swal("Warning!", "Please Select Goods Type", "warning");
    }
    else if (form.value.FormServiceType == null || form.value.FormServiceType == "") {
      swal("Warning!", "Please select Service Type", "warning");
    }
    else if (form.value.FormHsnCode == null || form.value.FormHsnCode == "") {
      swal("Warning!", "Please Select HSN", "warning");
    }

    else {
      var ans = confirm("Do you want to save??");
      if (ans) {
        this._mastersService.modifyGSList(this.GsEntryListData.ItemLevel1ID, this.GsEntryListData).subscribe(
          response => {
            swal("Updated!", "Updated " + this.GsEntryListData.ItemLevel1Name + "Updated", "success");
            this.isReadOnly = false;// this.GsEntryFrom.reset();
            this.GsEntryFrom.reset();
            this.GSList = [];
            this.GetGSListItem();

          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error.description;
              swal("Warning!", validationError, "warning");
            }
            else {
              this.errors.push('something went wrong!');
            }
            this.GsEntryFrom.reset();
            this.EnableAdd = true;
          }
        )

      }
    }

  }
  ModalClose() {
    //this.isReadOnly = true;


  }
  PrintDetails: any = [];
  onPrint() {
    this.PrintDetails = [];
    this._mastersService.GetGSListItems().subscribe(
      Response => {
        this.PrintDetails = Response;
        $('#PrintGSModel').modal('show');
      }
    )
  }

  printData: any = [];
  Print() {
    this.printData = [];
    if (this.printData != null) {
      var ans = confirm("Do you want to take print??");
      if (ans) {
        this._mastersService.GetGSListItems().subscribe(
          Response => {
            this.printData = Response
            if (this.printData != null) {
              $('#GSDetailsTab').modal('show');
            }
          }
        )
      }
    }
  }

  print() {
    if (this.printData !== null) {
      let GSDeatils, popupWin;
      GSDeatils = document.getElementById('TableData').innerHTML;
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
      ${GSDeatils}</body>
        </html>`
      );
      popupWin.document.close();
    }
  }
}




