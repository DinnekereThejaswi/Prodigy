import { Component, OnInit } from '@angular/core';
import { TagSplitService } from './tag-split.service';
import { Router } from '@angular/router';
import { MasterService } from './../core/common/master.service';
import { TagSplit } from './tag-split.model';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { ToastrService } from 'ngx-toastr';
import { AppConfigService } from '../AppConfigService';

@Component({
  selector: 'app-tag-split',
  templateUrl: './tag-split.component.html',
  styleUrls: ['./tag-split.component.css']
})
export class TagSplitComponent implements OnInit {
  EnableTag1Details: boolean = false;
  EnableTag2Details: boolean = false;
  EnableSubmitCancel: boolean = false;
  EnableJson: boolean = false;
  password: string;
  BarCodeNo: string = "";
  Stone1Form: FormGroup;
  Stone2Form: FormGroup;
  EnableStone1Dets: boolean = false;
  public Tag1Index: number;
  public Tag2Index: number;
  GrWt: number = 0;
  NtWt: number = 0;
  TagNo: number = 0;
  TagInfo: any = [];
  DesignList: any = [];
  CounterList: any = [];
  SizeList: any = [];
  StoneGsTypeList: any = [];
  ItemList: any = [];
  SupplierList: any = [];
  MCTypeList: any = [];
  StoneNameAPI: any = [];
  HeaderItemName: string = "";
  SupplierName: string = "";
  Tag1ItemName: string = "";
  Tag2ItemName: string = "";
  Tag1DesignName: string = "";
  Tag2DesignName: string = "";
  HeaderDesignName: string = "";
  HeaderMcType: string = "";
  Tag1GsCode: string = "";
  Tag1Type: string = "";
  Tag2GsCode: string = "";
  Tag2Type: string = "";

  Tag1DetailsJson: TagSplit = {
    ObjID: null,
    CompanyCode: null,
    BranchCode: null,
    BarcodeNo: null,
    BatchNo: null,
    SalCode: null,
    OperatorCode: null,
    Date: null,
    CouterCode: null,
    GSCode: null,
    ItemName: null,
    Gwt: null,
    Swt: null,
    Nwt: null,
    Grade: null,
    CatalogID: null,
    MakingChargePerRs: null,
    WastePercent: null,
    Qty: null,
    ItemSize: null,
    DesignNo: null,
    PieceRate: null,
    DiamondAmount: null,
    StoneAmount: null,
    OrderNo: null,
    SoldFlag: null,
    ProductCode: null,
    HallmarkCharges: null,
    Remarks: null,
    SupplierCode: null,
    OrderedCompanyCode: null,
    OrderedBranchCode: null,
    Karat: null,
    McAmount: null,
    WastageGrams: null,
    McPercent: null,
    McType: null,
    OldBarcodeNo: null,
    ProdIda: null,
    ProdTagno: null,
    UpdateOn: null,
    LotNo: null,
    TagWt: null,
    IsConfirmed: null,
    ConfirmedBy: null,
    ConfirmedDate: null,
    CurrentWt: null,
    MCFor: null,
    DiamondNo: null,
    BatchId: null,
    AddWt: null,
    WeightRead: null,
    ConfirmedweightRead: null,
    PartyName: null,
    DesignName: null,
    ItemSizeName: null,
    MasterDesignCode: null,
    MasterDesignName: null,
    VendorModelNo: null,
    PurMcGram: null,
    McPerPiece: null,
    TaggingType: null,
    BReceiptNo: null,
    BSNo: null,
    IssueTo: null,
    PurMcAmount: null,
    PurMcType: null,
    PurRate: null,
    SRBatchID: null,
    TotalSellingMC: null,
    PurDiamondAmount: null,
    TotalPurchaseMc: null,
    PurStoneAmount: null,
    PurPurityPercentage: null,
    PurWastageType: null,
    PurWastageTypeValue: null,
    UniqRowID: null,
    CertificationNo: null,
    RefNo: null,
    ReceiptType: null,
    EntryDocType: null,
    EntryDate: null,
    EntryDocNo: null,
    ExitDocType: null,
    ExitDate: null,
    ExitDocNo: null,
    OnlineStock: null,
    IsShuffled: null,
    Shuffled_date: null,
    Collections: null,
    lstOfStone: [
      {
        ObjID: null,
        CompanyCode: null,
        BranchCode: null,
        BillNo: null,
        SlNo: null,
        EstNo: null,
        EstSrNo: null,
        BarcodeNo: null,
        Type: null,
        Name: null,
        Qty: null,
        Carrat: null,
        StoneWt: null,
        Rate: null,
        Amount: null,
        Tax: null,
        TaxAmount: null,
        TotalAmount: null,
        BillType: null,
        DealerSalesNo: null,
        BillDet11PK: null,
        UpdateOn: null,
        FinYear: null,
        Color: null,
        Clarity: null,
        Shape: null,
        Cut: null,
        Polish: null,
        Symmetry: null,
        Fluorescence: null,
        Certificate: null,
      }
    ],
  }


  constructor(private _tagSplitService: TagSplitService, private _masterService: MasterService,
    private formBuilder: FormBuilder, private _router: Router, private toastr: ToastrService,
    private _appConfigService: AppConfigService) {
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
  }

  ngOnInit() {
    this.Tag1Index = -1;
    this.Tag2Index = -1;
    this.getSizeList();
    this.getStoneGsTypeList();
    this.getCounterList();
    this.getSupplierList();
    this.Stone1Form = this.formBuilder.group({
      ObjID: null,
      CompanyCode: null,
      BranchCode: null,
      BillNo: null,
      SlNo: null,
      EstNo: null,
      EstSrNo: null,
      BarcodeNo: null,
      Type: null,
      Name: null,
      Qty: null,
      Carrat: null,
      StoneWt: null,
      Rate: null,
      Amount: null,
      Tax: null,
      TaxAmount: null,
      TotalAmount: null,
      BillType: null,
      DealerSalesNo: null,
      BillDet11PK: null,
      UpdateOn: null,
      FinYear: null,
      Color: null,
      Clarity: null,
      Shape: null,
      Cut: null,
      Polish: null,
      Symmetry: null,
      Fluorescence: null,
      Certificate: null,
      SalCode: null,
      RateType: null,
      Item: null,
      RatePerGram: null
    });
    this.Stone2Form = this.formBuilder.group({
      ObjID: null,
      CompanyCode: null,
      BranchCode: null,
      BillNo: null,
      SlNo: null,
      EstNo: null,
      EstSrNo: null,
      BarcodeNo: null,
      Type: null,
      Name: null,
      Qty: null,
      Carrat: null,
      StoneWt: null,
      Rate: null,
      Amount: null,
      Tax: null,
      TaxAmount: null,
      TotalAmount: null,
      BillType: null,
      DealerSalesNo: null,
      BillDet11PK: null,
      UpdateOn: null,
      FinYear: null,
      Color: null,
      Clarity: null,
      Shape: null,
      Cut: null,
      Polish: null,
      Symmetry: null,
      Fluorescence: null,
      Certificate: null,
      SalCode: null,
      RateType: null,
      Item: null,
      RatePerGram: null
    });
  }

  Amount: number = 0;
  add(form) {
    if (this.Tag1GsCode == "" || this.Tag1GsCode == "null") {
      this.toastr.warning('Please select the GS.', 'Alert!');
    }
    else if (this.Tag1Type == "" || this.Tag1Type == "null") {
      this.toastr.warning('Please select the Type.', 'Alert!');
    }
    else if (form.value.Type == null) {
      this.toastr.warning('Please select the Name.', 'Alert!');
    }
    else if (form.value.Qty == null || form.value.Qty == null) {
      this.toastr.warning('Please enter the Qty.', 'Alert!');
    }
    else if (form.value.Carrat == null || form.value.Carrat == 0) {
      this.toastr.warning('Please enter the Carrat.', 'Alert!');
    }
    else if (form.value.Rate == null || form.value.Rate == 0) {
      this.toastr.warning('Please enter the Selling amt.', 'Alert!');
    }
    else {
      this.Amount = parseFloat(form.value.Carrat) * parseFloat(form.value.Rate);
      this.Stone1Form.controls.CompanyCode.setValue(CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8));
      this.Stone1Form.controls.BranchCode.setValue(CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8));
      this.Stone1Form.controls.BarcodeNo.setValue(this.BarCodeNo);
      this.Stone1Form.controls.StoneWt.setValue(this.Tag1DetailsJson.Swt);
      this.Stone1Form.controls.Amount.setValue(this.Amount);
      this.Tag1DetailsJson[0].lstOfStone.push(this.Stone1Form.value);
      this.Stone1Form.reset();
    }
  }

  add1(form) {
    if (this.Tag2GsCode == "" || this.Tag2GsCode == "null") {
      this.toastr.warning('Please select the GS.', 'Alert!');
    }
    else if (this.Tag2Type == "" || this.Tag2Type == "null") {
      this.toastr.warning('Please select the Type.', 'Alert!');
    }
    else if (form.value.Type == null) {
      this.toastr.warning('Please select the Name.', 'Alert!');
    }
    else if (form.value.Qty == null || form.value.Qty == null) {
      this.toastr.warning('Please enter the Qty.', 'Alert!');
    }
    else if (form.value.Carrat == null || form.value.Carrat == 0) {
      this.toastr.warning('Please enter the Carrat.', 'Alert!');
    }
    else if (form.value.Rate == null || form.value.Rate == 0) {
      this.toastr.warning('Please enter the Selling amt.', 'Alert!');
    }
    else {
      this.Amount = parseFloat(form.value.Carrat) * parseFloat(form.value.Rate);
      this.Stone2Form.controls.CompanyCode.setValue(CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8));
      this.Stone2Form.controls.BranchCode.setValue(CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8));
      this.Stone2Form.controls.BarcodeNo.setValue(this.BarCodeNo);
      this.Stone2Form.controls.StoneWt.setValue(this.Tag1DetailsJson.Swt);
      this.Stone2Form.controls.Amount.setValue(this.Amount);
      this.Tag1DetailsJson[1].lstOfStone.push(this.Stone2Form.value);
      this.Stone2Form.reset();
    }
  }

  SplittedBarcodeData: any = [];

  GetSplitBarcodeDetails() {
    this.SplitBarcode();
  }

  getTagDetails(arg) {
    if (arg == null) {
      alert("Please enter barcode");
    }
    else if (arg != null && arg != "") {
      if (this.AlphaNumericValidations(arg) == false) {
        alert("Please enter valid Barcode No");
      }
      else {
        this.BarCodeNo = arg;
        this._tagSplitService.getTagSplitTagInfoAPI(arg).subscribe(
          response => {
            this.TagInfo = response;
            if (this.TagInfo != null) {
              this.Tag1DetailsJson = this.TagInfo;
              this.SupplierName = this.Tag1DetailsJson[0].SupplierCode;
              this.GrWt = this.Tag1DetailsJson[0].Gwt;
              this.NtWt = this.Tag1DetailsJson[0].Nwt;
              this.EnableTag1Details = true;
              this.getItemName(this.Tag1DetailsJson[0].GSCode);
              this.TagInfo = [];
              this.EnableTag2Details = false;
            }
          }
        )
      }
    }
  }

  AlphaNumericValidations(arg) {
    if (arg != null && arg != "") {
      const isPinRegEx = /^[a-zA-Z0-9]+$/g;
      const validPin = isPinRegEx.test(arg);
      return validPin;
    }
    else {
      return true;
    }
  }

  deleteStone1FieldValue(index, arg) {
    var ans = confirm("Do you want to delete");
    if (ans) {
      this.Tag1DetailsJson[0].lstOfStone.splice(this.Tag1DetailsJson[0].lstOfStone.indexOf(arg), 1);
    }
  }

  getDesignList() {
    this._tagSplitService.getTagSplitDesignAPI().subscribe(
      response => {
        this.DesignList = response;
        this.HeaderDesignName = this.Tag1DetailsJson[0].DesignNo;
        this.Tag1DesignName = this.Tag1DetailsJson[0].DesignNo;
        this.Tag2DesignName = this.Tag1DetailsJson[0].DesignNo;
      }
    )
  }



  getSizeList() {
    this._tagSplitService.getTagSplitSizeAPI().subscribe(
      response => {
        this.SizeList = response;
      }
    )
  }

  getStoneGsTypeList() {
    this._tagSplitService.getTagSplitStoneGSTypeAPI().subscribe(
      response => {
        this.StoneGsTypeList = response;
      }
    )
  }

  getItemName(arg) {
    this._tagSplitService.getItemfromAPI(arg).subscribe(
      response => {
        this.ItemList = response;
        this.HeaderItemName = this.Tag1DetailsJson[0].ItemName;
        this.Tag1ItemName = this.Tag1DetailsJson[0].ItemName;
        this.Tag2ItemName = this.Tag1DetailsJson[0].ItemName;
        this.getDesignList();
        this.getMCType();
      }
    )
  }

  getTagSplitStoneNameAPI(GS, Type) {
    this._tagSplitService.getTagSplitStoneNameAPI(GS, Type).subscribe(
      response => {
        this.StoneNameAPI = response;
      }
    )
  }

  getMCType() {
    this._masterService.getMCTypes().subscribe(
      response => {
        this.MCTypeList = response;
        this.HeaderMcType = this.Tag1DetailsJson[0].McType;
      }
    )
  }

  getSupplierList() {
    this._tagSplitService.GetSupplier().subscribe(
      response => {
        this.SupplierList = response;
      }
    )
  }

  getCounterList() {
    this._masterService.GetCounterForBarcode().subscribe(
      response => {
        this.CounterList = response;
      }
    )
  }

  getGScodeTag1(arg) {
    this.Tag1GsCode = arg;
  }

  getGScodeTag2(arg) {
    this.Tag2GsCode = arg;
  }

  getTypeTag1(arg) {
    this.Tag1Type = arg;
    this.getTagSplitStoneNameAPI(this.Tag1GsCode, this.Tag1Type);
  }

  getTypeTag2(arg) {
    this.Tag2Type = arg;
    this.getTagSplitStoneNameAPI(this.Tag2GsCode, this.Tag2Type);
  }

  Tag1StoneDets(index) {
    if (this.Tag1DetailsJson[0].lstOfStone.length != 0) {
      this.Tag1Index = index === this.Tag1Index ? -1 : index;
    }
    else {
      this.toastr.error('Stone details not available.', 'Alert!');
    }
  }
  Tag2StoneDets(index) {
    if (this.Tag1DetailsJson[1].lstOfStone.length != 0) {
      this.Tag2Index = index === this.Tag2Index ? -1 : index;
    }
    else {
      this.toastr.error('Stone details not available.', 'Alert!');
    }
  }

  SplitBarcode() {
    this._tagSplitService.postSplitBarcode(this.Tag1DetailsJson).subscribe(
      response => {
        this.SplittedBarcodeData = response;
        this.Tag1DetailsJson = this.SplittedBarcodeData;
        if (this.SplittedBarcodeData.length >= 2) {
          this.EnableTag2Details = true;
          this.EnableSubmitCancel = true;
        }
      }
    )
  }

  SuccessMessage: string = "";

  SubmitBarcode() {
    if (this.Tag1DetailsJson[0].ItemName == "" || this.Tag1DetailsJson[0].ItemName == "null") {
      this.toastr.warning('Please select the Tag 1 Item.', 'Alert!');
    }
    else if (this.Tag1DetailsJson[0].DesignNo == "" || this.Tag1DetailsJson[0].DesignNo == "null") {
      this.toastr.warning('Please select the Tag 1 Design.', 'Alert!');
    }
    else if (this.EnableTag2Details == true) {
      if (this.Tag1DetailsJson[1].ItemName == "" || this.Tag1DetailsJson[0].ItemName == "null") {
        this.toastr.warning('Please select the Tag 2 Item.', 'Alert!');
      }
      else if (this.Tag1DetailsJson[1].DesignNo == "" || this.Tag1DetailsJson[0].DesignNo == "null") {
        this.toastr.warning('Please select the Tag 2 Design.', 'Alert!');
      }
      else {
        this.SubmitDetails();
      }
    }
    else {
      this.SubmitDetails();
    }
  }

  SubmitDetails() {
    this._tagSplitService.postGenerateBarcode(this.Tag1DetailsJson).subscribe(
      (response: string) => {
        this.SuccessMessage = response;
        swal("Success!", this.SuccessMessage, "success");
        this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
          () => {
            this._router.navigate(['/tag-split']);
          })
      },
      (err) => {
        if (err.status === 400) {
          const validationError = err.error;
          swal("Warning!", validationError.description, "warning");
        }
      }
    )
  }

  CancelBarcode() {
    this.EnableTag1Details = false;
    this.EnableTag2Details = false;
  }
}