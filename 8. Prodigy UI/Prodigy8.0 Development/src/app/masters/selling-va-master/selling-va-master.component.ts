import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import { MastersService } from '../masters.service';
import swal from 'sweetalert';
declare var $: any;



@Component({
  selector: 'app-selling-va-master',
  templateUrl: './selling-va-master.component.html',
  styleUrls: ['./selling-va-master.component.css']
})
export class SellingVaMasterComponent implements OnInit {
  ccode: string;
  bcode: string;
  password: string;
  VAMasterForm: FormGroup;
  copyVAForm: FormGroup;
  VaModelForm: FormGroup;
  radioItems: Array<string>;
  isReadOnly: boolean = true;
  EnablePagination: boolean = false;
  model = { option: 'Itemwise' };
  EnableJson: boolean = false;
  //----for page
  totalItems: any = [];
  NoRecords: boolean = false;
  pagenumber: number = 1;
  top = 10;
  Modeltop = 10;
  skip = (this.pagenumber - 1) * this.top;
  ////////////////
  constructor(private vaservice: MastersService, private fb: FormBuilder, private _appConfigService: AppConfigService
  ) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.radioItems = ['Itemwise', 'SupplierWise'];

  }
  SellingVASummaryHeader: any = {
    ItemName: null,
    DesignCode: null,
    FromWt: null,
    ToWt: null,
    McPerPiece: null,
    McPerGram: null,
    McPercent: null,
    McAmount: null,
    WastageGrms: null,
    WastPercentage: null,
    TypeID: null,
    TypeName: null,
    PartyCode: null,
    GSCode: null,
    ValueAdded: null,
    MakingChargePerRs: null,
    WastPercent: null,
  };
  copyVASummaryHeadert: any = {
    supplier1: null,
    supplier2: null,
    gscode: null,
    itemname: null,
    designname: null,

  }
  vaModelHeader: any = {
    PartyName: null,
    GsName: null,
    ItemName: null
  }

  ngOnInit() {
    this.getSupplier1();
    this.getSupplier();
    this.getGs();
    this.getDesign();
    this.getMc();
    this.SellingVASummaryHeader.CompanyCode = this.ccode;
    this.SellingVASummaryHeader.BranchCode = this.bcode;
    this.printHeader.CompanyCode = this.ccode;
    this.printHeader.BranchCode = this.bcode;
    this.VAMasterForm = this.fb.group({
      Supplier: null,
      GS: null,
      ItemName: null,
      DesignName: null,
      McType: null,
      McPerGram: null,
      McPerPiece: null,
      McPercent: null,
      ValueAdded: null,
      McAmount: null,
      WastageGrms: null,
      fromWt: null,
      ToWt: null,
      CompanyCode: null,
      BranchCode: null,
    })

    this.VaModelForm = this.fb.group({
      GsName: null,
      PartyName: null,
      ItemName: null
    });
    this.copyVAForm = this.fb.group({
      frmCtrl_Frmsupplier: null,
      frmCtrl_toSuplier: null,
      frmCTrl_Gs: null,
      frmCtrl_ItemName: null,
      frmCtrl_Design: null
      ,

    })

  }

  SupplierList1: any = [];
  getSupplier1() {
    this.vaservice.Supplier().subscribe(
      Response => {
        this.SupplierList1 = Response;
      }
    )
  }
  SelectedPartyCode: string = "";
  selectchanges(args) {
    this.SelectedPartyCode = args.target.value;
  }
  SupplierList: any = [];
  getSupplier() {
    this.vaservice.Supplier().subscribe(
      Response => {
        this.SupplierList = Response;
      }
    )
  }
  GSList: any = [];
  getGs() {
    this.vaservice.GSList().subscribe(
      Response => {
        this.GSList = Response;
      }
    )
  }
  //added
  ItemLists: any = [];
  getItemsByGs(arg) {
    this.vaservice.GetItemList(arg.value.GS).subscribe(
      Response => {
        this.ItemLists = Response;
      }
    )
  }
  //adeed
  ItemList: any = [];
  GS: string = "";
  Item: string = "";

  getGsAndItem(arg) {
    this.GS = arg;
    this.vaservice.GetItemList(arg).subscribe(
      Response => {
        this.ItemList = Response;
      }
    )
  }

  getItemValue(arg) {
    this.Item = arg;
  }

  DesignList: any = [];
  getDesign() {
    this.vaservice.DesignName().subscribe(
      Response => {
        this.DesignList = Response;
      }
    )
  }
  MCList: any = [];
  getMc() {
    this.vaservice.getMCTypes().subscribe(
      Response => {
        this.MCList = Response;
      }
    )
  }

  toggle: string = "Invalid";
  McAmt: Number;
  onchangeOption(OptionmodeArg) {
    if (OptionmodeArg == "5") {
      this.toggle = "MC%";
      this.SellingVASummaryHeader.MakingChargePerRs = 0.000;
      this.SellingVASummaryHeader.McPerPiece = 0.000;
      this.SellingVASummaryHeader.WastPercent = 0.000;
      this.SellingVASummaryHeader.McAmount = 0.000;
      this.SellingVASummaryHeader.WastageGrms = 0.000;
      this.SellingVASummaryHeader.ValueAdded = 0.000;
      this.VAClear();
    }
    else if (OptionmodeArg == "1") {
      this.toggle = "MC PER GRAM";
      this.SellingVASummaryHeader.McPerPiece = 0.000;
      this.SellingVASummaryHeader.McPercent = 0.000;
      this.SellingVASummaryHeader.McAmount = 0.000;
      this.SellingVASummaryHeader.WastageGrms = 0.000;
      this.SellingVASummaryHeader.TotalSalesMc = 0.000;
      this.SellingVASummaryHeader.ValueAdded = 0.000;
      this.VAClear();
    }
    else if (OptionmodeArg == "4") {
      this.toggle = "MC Amount - Wastg";
      this.SellingVASummaryHeader.MakingChargePerRs = 0.000;
      this.SellingVASummaryHeader.McPerPiece = 0.000;
      this.SellingVASummaryHeader.McPercent = 0.000;
      this.SellingVASummaryHeader.WastPercent = 0.000;
      this.SellingVASummaryHeader.ValueAdded = 0.000;
      this.VAClear();
    }

    else if (OptionmodeArg == "0") {
      this.toggle = "NA";
      this.SellingVASummaryHeader.MakingChargePerRs = 0.000;
      this.SellingVASummaryHeader.McPerPiece = 0.000;
      this.SellingVASummaryHeader.McPercent = 0.000;
      this.SellingVASummaryHeader.WastPercent = 0.000;
      this.SellingVASummaryHeader.McAmount = 0.000;
      this.SellingVASummaryHeader.WastageGrms = 0.000;
      this.SellingVASummaryHeader.ValueAdded = 0.000;
      this.VAClear();
    }
    else if (OptionmodeArg == "6") {
      this.toggle = "MC PER PIECE";
      this.SellingVASummaryHeader.MakingChargePerRs = 0.000;
      this.SellingVASummaryHeader.McPercent = 0.000;
      this.SellingVASummaryHeader.WastPercent = 0.000;
      this.SellingVASummaryHeader.McAmount = 0.000;
      this.SellingVASummaryHeader.WastageGrms = 0.000;
      this.SellingVASummaryHeader.ValueAdded = 0.000;
      this.VAClear();
    }
  }
  VAmasterTab: any = [];
  getTable(arg) {
    this.vaservice.getTable(arg.value.Supplier, arg.value.GS, arg.value.ItemName, arg.value.DesignName).subscribe(
      Response => {
        this.VAmasterTab = Response;
        this.getFromWt(arg);
        this.onPageChangeForParent(this.pagenumber);

      }
    )
  }
  FromWT: any;
  getFromWt(arg) {
    this.vaservice.getFromWT(arg.value.Supplier, arg.value.GS, arg.value.ItemName, arg.value.DesignName).subscribe(
      Response => {
        this.FromWT = Response;
        this.SellingVASummaryHeader.FromWt = this.FromWT.FromWt;
      }
    )
  }
  printHeader = {
    CompanyCode: "",
    BranchCode: "",
    SupplierCode: "",
    GSCode: "",
    ItemCode: "",
    DesignCode: ""
  }
  PostRes: any = [];
  errors: any = [];
  add() {
    if (this.VAMasterForm.value.Supplier == null ||
      this.VAMasterForm.value.Supplier == "") {
      swal("Warning!", "Please select supplier", "warning");
    }
    else if (this.VAMasterForm.value.GS == null) {
      swal("Warning!", "Please select GS", "warning");
    }
    else if (this.VAMasterForm.value.ItemName == null) {
      swal("Warning!", "Please select Item Name", "warning");
    }
    else if (this.VAMasterForm.value.DesignName == null) {
      swal("Warning!", "Please select Design Name", "warning");
    }
    else if (this.SellingVASummaryHeader.TypeID == null) {
      swal("Warning!", "Please Select  MC types", "warning");
    }
    else if (this.SellingVASummaryHeader.ToWt == null || this.SellingVASummaryHeader.ToWt == 0) {
      swal("Warning!", "Please enter  To weight", "warning");
    }
    else if (this.SellingVASummaryHeader.fromWt > this.VAMasterForm.value.ToWt) {
      swal("Warning!", "Please enter valid To weight", "warning");
    }
    else if (this.VAmasterTab.filter(x => x.ToWt == this.SellingVASummaryHeader.ToWt
    ).length == 1) {
      swal("Warning!", "ToWt already exist", "warning");
      this.VAMasterForm.reset();
      return false;
    }
    else if (!this.ValidateControls())
      return false;
    else {
      this.vaservice.VAMasterPost(this.SellingVASummaryHeader).subscribe(
        Response => {
          this.printHeader.CompanyCode = this.ccode;
          this.printHeader.BranchCode = this.bcode;
          this.printHeader.SupplierCode = this.SellingVASummaryHeader.PartyCode;
          this.printHeader.GSCode = this.SellingVASummaryHeader.GSCode;
          this.printHeader.ItemCode = this.SellingVASummaryHeader.ItemName;
          this.printHeader.DesignCode = this.SellingVASummaryHeader.DesignCode
          this.PostRes = Response;
          swal("Saved!", "Saved " + this.SellingVASummaryHeader.ItemName + " Saved", "success");
          this.getTable(this.VAMasterForm);
          this.SellingVASummaryHeader.ToWt = 0;
          this.SellingVASummaryHeader.FromWt = this.FromWT.FromWt;
          this.SellingVASummaryHeader.McPerPiece = 0;
          this.SellingVASummaryHeader.McPercent = 0;
          this.SellingVASummaryHeader.McAmount = 0;
          this.SellingVASummaryHeader.WastageGrms = 0;
          this.SellingVASummaryHeader.ValueAdded = 0;
          // this.VAMasterForm.reset();
          // this.SellingVASummaryHeader.reset();
          // this.VAClear();
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

    this.VAClear();
  }

  LoadPrintObject(arg) {
    this.printHeader.CompanyCode = this.ccode;
    this.printHeader.BranchCode = this.bcode;
    this.printHeader.SupplierCode = this.SellingVASummaryHeader.PartyCode;
    this.printHeader.GSCode = this.SellingVASummaryHeader.GSCode;
    this.printHeader.ItemCode = this.SellingVASummaryHeader.ItemName;
    this.printHeader.DesignCode = this.SellingVASummaryHeader.DesignCode
  }
  PrintDetails: any = [];
  printVADetails() {
    this.printHeader.CompanyCode = this.ccode;
    this.printHeader.BranchCode = this.bcode;
    this.printHeader.SupplierCode = this.SellingVASummaryHeader.PartyCode;
    this.printHeader.GSCode = this.SellingVASummaryHeader.GSCode;
    this.printHeader.ItemCode = this.SellingVASummaryHeader.ItemName;
    this.printHeader.DesignCode = this.SellingVASummaryHeader.DesignCode
    this.vaservice.printVADetail(this.printHeader).subscribe(
      Response => {
        this.PrintDetails = Response;
        this.PrintDetails = atob(this.PrintDetails.Data);
        $('#PrintVAModal').modal('show');
        $('#DisplayData').html(this.PrintDetails);
      }
    )
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
  ValidateControls() {
    if (this.SellingVASummaryHeader.TypeID == 4) {
      if ((this.SellingVASummaryHeader.ValueAdded == null ||
        this.SellingVASummaryHeader.ValueAdded == 0) &&
        (this.SellingVASummaryHeader.McAmount == null ||
          this.SellingVASummaryHeader.McAmount == 0)) {
        swal("Warning!", "Please enter the McAmount", "warning");
        return false;
      }
      return true;
    }
    else if (this.SellingVASummaryHeader.TypeID == 1) {
      if ((this.SellingVASummaryHeader.ValueAdded == null ||
        this.SellingVASummaryHeader.ValueAdded == 0) &&
        (this.SellingVASummaryHeader.McPerGram == null ||
          this.SellingVASummaryHeader.McPerGram == 0)) {
        swal("Warning!", "Please enter the MC/Grams", "warning");
        return false;
      }
      return true;
    }
    else if (this.SellingVASummaryHeader.TypeID == 6) {
      if ((this.SellingVASummaryHeader.ValueAdded == null ||
        this.SellingVASummaryHeader.ValueAdded == 0) &&
        (this.SellingVASummaryHeader.McPerPiece == null ||
          this.SellingVASummaryHeader.McPerPiece == 0)) {
        swal("Warning!", "Please enter the MC/Piece", "warning");
        return false;
      }
      return true;

    }

    else if (this.SellingVASummaryHeader.TypeID == 5) {
      if ((this.SellingVASummaryHeader.ValueAdded == null ||
        this.SellingVASummaryHeader.ValueAdded == 0) &&
        (this.SellingVASummaryHeader.McPercent == null ||
          this.SellingVASummaryHeader.McPercent == 0)) {
        swal("Warning!", "Please enter the Mc Percent", "warning");
        return false;
      }
      return true;
    }

  }

  VAClear() {
    this.SellingVASummaryHeader.ToWt = 0.000;
    this.SellingVASummaryHeader.McPerPiece = 0.000;
    this.SellingVASummaryHeader.McPercent = 0.000;
    this.SellingVASummaryHeader.ValueAdded = 0.000;
    this.SellingVASummaryHeader.McAmount = 0.000;
    this.SellingVASummaryHeader.WastageGrms = 0.000;
    this.SellingVASummaryHeader.McPerGram = 0.000;

  }
  VaformClear() {

    this.VAMasterForm.reset();
    this.VAmasterTab = [];
    this.totalItems = [];

  }

  View() {
    this.VAMasterForm.reset();
    this.VAmasterTab = [];
    this.VaBySupplier = [];
    this.EnablePagination = true;
    this.totalItems = [];
    $('#VaMaster').modal('show');
  }
  ModalClears() {
    this.EnablePagination = false;
    this.totalItems = [];
    this.VaModelForm.reset();
    this.VaBySupplier = [];
    this.model.option = "Itemwise";
  }

  VaByGsAndItem: any = [];
  getVADetailsByGSandItem() {
    if (this.GS == null || this.GS == "") {
      swal("Warning!", "Please select GS", "warning");
    }
    else if (this.Item == null || this.Item == "") {
      swal("Warning!", "Please select Item Name", "warning");
    }
    this.printHeader.GSCode = this.GS;
    this.printHeader.ItemCode = this.Item;
    this.vaservice.printVADetail(this.printHeader).subscribe(
      Response => {
        this.PrintDetails = Response;
        this.PrintDetails = atob(this.PrintDetails.Data);
        $('#PrintVAModal').modal('show');
        $('#DisplayData').html(this.PrintDetails);
      }
    )
    // this.vaservice.getVaByGsAndItems(this.GS, this.Item).subscribe(
    //   Response => {
    //     this.VaBySupplier = Response;
    //     this.onPageChangedModelByGdAndItm(this.pagenumber);
    //   }
    // )
  }
  VaBySupplier: any = [];
  getVADetailsBySupplier(arg) {
    if (this.SelectedPartyCode == null || this.SelectedPartyCode == "") {
      swal("Warning!", "Please select supplier", "warning");
    }
    this.printHeader.SupplierCode = this.SelectedPartyCode;
    this.vaservice.printVADetail(this.printHeader).subscribe(
      Response => {
        this.PrintDetails = Response;
        this.PrintDetails = atob(this.PrintDetails.Data);
        $('#PrintVAModal').modal('show');
        $('#DisplayData').html(this.PrintDetails);
      }
    )
    // this.vaservice.getVaBySupplier(this.SelectedPartyCode).subscribe(
    //   Response => {
    //     this.VaBySupplier = Response;
    //     this.getFromWt(arg);
    //     this.onPageChangedModelBySupplr(this.pagenumber);
    //   }
    // )
  }
  ModelChanged(arg) {
    this.model.option = arg;
    if (this.model.option == arg) {
      if (this.model.option === "SupplierWise") {
        this.totalItems = [];
      }
      if (this.model.option === "Itemwise") {
        this.totalItems = [];
      }
      this.VaModelForm.reset();
      this.VaBySupplier = [];
    }

  }


  onPageChangeForParent(p: number) {
    this.pagenumber = p;
    const skipno = (this.pagenumber - 1) * this.top;
    this.getSearchCountVA(this.VAMasterForm.value.Supplier, this.VAMasterForm.value.GS, this.VAMasterForm.value.ItemName,
      this.VAMasterForm.value.DesignName, this.top, skipno);
  }
  getSearchCountVA(supplier, gs, item, design, top, skip) {

    this.vaservice.GetAllRecordToPage(supplier, gs, item, design, top, skip).subscribe(
      response => {

        this.VAmasterTab = response;

        this.getSearchCount(supplier, gs, item, design);
      }
    )
  }
  getSearchCount(supplier, gs, item, design) {
    this.vaservice.GetVAREcordCountBySGID(supplier, gs, item, design).subscribe(
      response => {

        this.totalItems = response;
        this.totalItems = this.totalItems.RecordCount;
      }
    );
  }

  onPageChangedModelByGdAndItm(p: number) {
    this.pagenumber = p;
    const skipno = (this.pagenumber - 1) * this.top;
    this.getSearchCountVAModel(this.GS, this.Item, this.top, skipno);

  }
  getSearchCountVAModel(gs, item, top, skip) {
    this.vaservice.GetVARecordToModelByGSAndItem(gs, item, top, skip).subscribe(
      response => {
        this.VaBySupplier = response;
        this.getSearchCountToModel(gs, item);

      }
    )
  }
  modelitems: any;
  getSearchCountToModel(gs, item) {
    this.vaservice.GetVATotalRecordOfGsAndItemToModel(gs, item).subscribe(
      response => {
        this.totalItems = response;
        this.totalItems = this.totalItems.RecordCount;
      }
    );
  }

  onPageChangedModelBySupplr(p: number) {
    this.pagenumber = p;
    const skipno = (this.pagenumber - 1) * this.top;
    this.getSearchCountModelSupplier(this.SelectedPartyCode, this.top, skipno);
  }
  getSearchCountModelSupplier(suuplier, top, skip) {
    this.vaservice.GetVATotalRecordOfSupplier(suuplier, top, skip).subscribe(
      response => {
        this.VaBySupplier = response;
        this.getSearchCountToModels(suuplier);
      }
    )
  }
  getSearchCountToModels(suuplier) {
    this.vaservice.GetVATotalRecordOfSupplierToModel(suuplier).subscribe(
      response => {
        this.totalItems = response;
        this.totalItems = this.totalItems.RecordCount;
      }
    );
  }

  ////copy va to one supplier to another supplier////////
  GSdata: any = [];
  getGsToCopyForm() {
    this.vaservice.GSList().subscribe(
      Response => {
        this.GSdata = Response;
      }
    )
  }

  Itemsdata: any = [];
  getItemsByGsToCopyForm(arg) {
    this.vaservice.GetItemList(arg.target.value).subscribe(
      Response => {
        this.Itemsdata = Response;
      }
    )
  }

  DesignsData: any = [];
  getDesignToCopyForm() {
    this.vaservice.DesignName().subscribe(
      Response => {
        this.DesignsData = Response;
      }
    )
  }
  copyVADataModel = {
    CompanyCode: "",
    BranchCode: "",
    FromSupplierCode: null,
    ToSupplierCode: null,
    GSCode: null,
    ItemCode: null,
    DesignCode: null,
  }
  copy() {
    this.getGsToCopyForm();
    this.getDesignToCopyForm();
    $('#CopyVaModel').modal('show');
  }
  VAmasterCopyTabl: any = [];
  getCopyTable() {
    // if (this.copyVADataModel.FromSupplierCode == null || this.copyVADataModel.FromSupplierCode == "") {
    //   swal("Warning!", "Please Select From Supplier", "warning");
    // }
    // else if (this.copyVADataModel.ToSupplierCode == null || this.copyVADataModel.ToSupplierCode == "") {
    //   swal("Warning!", "Please Select  To SupplierCode", "warning");
    // }
    // else if (this.copyVADataModel.GSCode == null || this.copyVADataModel.GSCode == "") {
    //   swal("Warning!", "Please Select GSCode", "warning");
    // }
    // else if (this.copyVADataModel.ItemCode == null || this.copyVADataModel.ItemCode == "") {
    //   swal("Warning!", "Please Select ItemCode", "warning");
    // }
    // else if (this.copyVADataModel.DesignCode == null || this.copyVADataModel.DesignCode == "") {
    //   swal("Warning!", "Please Select DesignCode", "warning");
    // }
    // else {
    this.copyVADataModel.CompanyCode = this.ccode;
    this.copyVADataModel.BranchCode = this.bcode;
    this.vaservice.getTable(this.copyVADataModel.FromSupplierCode, this.copyVADataModel.GSCode, this.copyVADataModel.ItemCode, this.copyVADataModel.DesignCode).subscribe(
      Response => {
        this.VAmasterCopyTabl = Response;
        // console.log(this.VAmasterCopyTabl);
        // this.onPageChangeForCopy(this.pagenumber);
        ////
        ///
        ////if inincase need to work on pagination
      }
    )
    //}
  }

  // onPageChangeForCopy(p: number) {
  //   this.pagenumber = p;
  //   const skipno = (this.pagenumber - 1) * this.top;
  //   this.getSearchCountVA(this.VAMasterForm.value.Supplier, this.VAMasterForm.value.GS, this.VAMasterForm.value.ItemName,
  //     this.VAMasterForm.value.DesignName, this.top, skipno);
  // }

  copyFromSuplrToSuplr(form) {
    if (form.value.frmCtrl_Frmsupplier == null || form.value.frmCtrl_Frmsupplier == "") {
      swal("Warning!", "Please Select From Supplier", "warning");
    }
    else if (form.value.frmCtrl_toSuplier == null || form.value.frmCtrl_toSuplier == "") {
      swal("Warning!", "Please Select To Suplier ", "warning");
    }
    else if (form.value.frmCTrl_Gs == null || form.value.frmCTrl_Gs == "") {
      swal("Warning!", "Please Select GS ", "warning");
    } else if (form.value.frmCtrl_ItemName == null || form.value.frmCtrl_ItemName == "") {
      swal("Warning!", "Please Select Item Name", "warning");
    }
    else if (form.value.frmCtrl_Design == null || form.value.frmCtrl_Design == "") {
      swal("Warning!", "Please Select Design", "warning");
    }
    else {
      var ans = confirm("Do you want to Copy??");
      if (ans) {
        this.copyVADataModel.CompanyCode = this.ccode;
        this.copyVADataModel.BranchCode = this.bcode;
        this.vaservice.copySupplierToSupplier(this.copyVADataModel).subscribe(
          response => {
            swal("Saved!", "VA Details Copied Successfully !", "success");
            this.copyVAForm.reset();
            this.VAmasterCopyTabl = [];
          }
        );

      }

    }

  }

  closecopyVaModelForm() {
    $('#CopyVaModel').modal('hide');
    this.copyVAForm.reset();
  }
  Cancel(form) {
    this.copyVAForm.reset();
    form.reset();
    this.GSdata = [];
    this.Itemsdata = [];
    this.DesignsData = [];
    this.VAmasterCopyTabl = [];
  }

}
