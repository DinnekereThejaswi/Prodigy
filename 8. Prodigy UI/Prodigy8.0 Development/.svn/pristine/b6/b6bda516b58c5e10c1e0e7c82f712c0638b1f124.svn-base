import { Component, OnInit, Input, ViewChild, ElementRef, AfterContentChecked } from '@angular/core';
import { AddBarcodeService } from './add-barcode.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { AddBarcode, stoneModel } from './add-barcode.model';
import { SalesService } from './../sales.service';
import { ToastrService } from 'ngx-toastr';
import { MasterService } from '../../core/common/master.service';
import swal from 'sweetalert';
import { estimationService } from './../../estimation/estimation.service';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import { Router } from '@angular/router';

declare var $: any;

@Component({
  selector: 'app-add-barcode',
  templateUrl: './add-barcode.component.html',
  styleUrls: ['./add-barcode.component.css']
})

export class AddBarcodeComponent implements OnInit, AfterContentChecked {
  @Input() BarcodeFromSales: boolean = false;
  @ViewChild("PwdRate", { static: true }) PwdRate: ElementRef;
  @ViewChild("PwdMC", { static: true }) PwdMC: ElementRef;
  @ViewChild("PwdStnDet", { static: true }) PwdStnDet: ElementRef;
  @ViewChild("tagno", { static: true }) tagno: ElementRef;
  @ViewChild("VaporLossEdit", { static: true }) VaporLossEdit: ElementRef;

  AddBarcodeForm: FormGroup;
  ccode: string = "";
  bcode: string = "";
  password: string;
  AddForm: FormGroup;
  BillForm: FormGroup;
  DeductForm: FormGroup;
  EnableBarcodeAge: boolean = false;
  GsCode: any;
  OptionMode: any;
  readOnly: boolean = true;
  ItemName: any;
  isDisabled: boolean = false;
  Karat: any;
  SalesEstRateEditCode: string;
  VaporLossEditCode: string;
  MCRateEdit: string;
  StoneDetailsEdit: string;
  HideRadioBtn: boolean = true;
  EnableDisableAddWt: boolean = true;
  EnableDisableDedGrWt: boolean = true;
  NonTag: Boolean = false;
  tag: Boolean = true;
  radioItems: Array<Object>;
  model = { 'name': 'Tag' };
  BarCodeNo: string = "";
  BarCodeNoFromSalesComp: string = "";
  Non_tag: Boolean = true;
  radioButtons: Array<Object>;
  radiomodel = { 'name': 'Non Tag' };
  EnableJson: boolean = false;
  constructor(private toastr: ToastrService, private salesService: SalesService
    , private barcodeService: AddBarcodeService, private fb: FormBuilder,
    private _masterService: MasterService, private _estimationService: estimationService,
    private appConfigService: AppConfigService, private _router: Router) {
    this.password = this.appConfigService.Pwd
    this.EnableJson = this.appConfigService.EnableJson;
    this.SalesEstRateEditCode = appConfigService.RateEditCode.SalesEstRateEdit;
    this.VaporLossEditCode = appConfigService.RateEditCode.VaporLessPermission;
    this.MCRateEdit = appConfigService.RateEditCode.MCRateEdit;
    this.StoneDetailsEdit = appConfigService.RateEditCode.StoneDetailsEdit;

    this.getCB();
    this.radioItems = [{ 'name': 'Tag', 'Tooltip': 'Existing Barcode' },
    { 'name': 'Non Tag', 'Tooltip': 'Non Existing' }
    ];
    this.radioButtons = [{ 'name': 'Tag', 'Tooltip': 'Existing Barcode' },
    { 'name': 'Non Tag', 'Tooltip': 'Non Existing' }
    ];
    $('#PrintDetailsTextTab').modal('hide');
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngAfterContentChecked() {
    this.PwdMC.nativeElement.focus();
    this.PwdRate.nativeElement.focus();
    this.PwdStnDet.nativeElement.focus();
    this.VaporLossEdit.nativeElement.focus();
  }

  BarcodeSummaryHeader: any = {
    ObjID: null,
    ItemQty: null,
    CompanyCode: this.ccode,
    BranchCode: this.bcode,
    EstNo: null,
    SlNo: null,
    BillNo: null,
    BarcodeNo: this.BarCodeNo,
    SalCode: null,
    CounterCode: null,
    ItemName: null,
    ItemNo: null,
    Grosswt: null,
    Stonewt: null,
    Netwt: null,
    AddWt: null,
    BillQty: null,
    DeductWt: null,
    MakingChargePerRs: null,
    WastPercent: null,
    GoldValue: null,
    VaAmount: null,
    StoneCharges: null,
    DiamondCharges: null,
    TotalAmount: null,
    Hallmarkarges: null,
    McAmount: null,
    WastageGrms: null,
    McPercent: null,
    AddQty: null,
    DeductQty: null,
    OfferValue: null,
    UpdateOn: null,
    GsCode: null,
    Rate: null,
    Karat: null,
    AdBarcode: null,
    AdCounter: null,
    AdItem: null,
    IsEDApplicable: null,
    McType: null,
    Fin_Year: null,
    NewBillNo: null,
    ItemTotalAfterDiscount: null,
    ItemAdditionalDiscount: null,
    TaxPercentage: null,
    TaxAmount: null,
    ItemFinalAmount: null,
    SupplierCode: null,
    ItemSize: null,
    ImgID: null,
    DesignCode: null,
    DesignName: null,
    BatchID: null,
    Rf_ID: null,
    McPerPiece: null,
    DiscountMc: null,
    TotalSalesMc: null,
    McDiscountAmt: null,
    purchaseMc: null,
    GSTGroupCode: null,
    SGSTPercent: null,
    SGSTAmount: null,
    CGSTPercent: null,
    CGSTAmount: null,
    IGSTPercent: null,
    IGSTAmount: null,
    HSN: null,
    PieceRate: null,
    DeductSWt: null,
    OrdDiscountAmt: null,
    DedCounter: null,
    DedItem: null,
    isInterstate: 0,
    VaporLossWeight: null,
    VaporLossAmount: null,
    salesEstStoneVM: []
  }

  BarcodePopupHeader = {
    ItQty: null,
    CounterCode: null,
    ItemName: null,
    ItemNo: null,
    Grwt: null,
    Stwt: null,
    Ntwt: null,
    AddWt: null,
    DeductWt: null,
    AdBarcode: null,
    AdCounter: null,
    AdItem: null,
    AdWt: null,
    AdQty: null,
    tagno: null,
  };
  BillHeader = {
    BillGrosswt: null,
    NW: null
  };

  DeductHeader = {
    Qty: null,
    CounterCode: null,
    DDCounter: null,
    GW: null,
    SW: null,
    NW: null,
    DedQty: null,
    Dedwt: null,
    DedSW: null,
    TagNo: null,
    ItemName: null,
    Iname: null,
    ItemQty: null
  }

  GetAddBarcodeList: any = {};
  SalesBarcode: string = "";
  OrderNo: Number = 0;
  EnableOrderDetails: boolean = false;
  OrderRate: any = 0;
  OrderKarat: any = "";
  OrderDetails: any = null;

  storeExistingBarcodeDets: any = {};
  storeExistingBarcodeDets1: any = {};
  editMode: boolean = false;
  salCode: string = "";

  EnableDisableCtrls: boolean = false;

  ngOnInit() {
    this.getStaffList();
    this.getGSList();
    this.getMCTypes();
    this.BarcodeSummaryHeader.McType = 5;
    this.getStoneType();
    this.onchangeOption(5);

    this.salesService.castSalesBarcode.subscribe(
      response => {
        this.SalesBarcode = response;
        this.EnableAddRow = false;
        if (this.SalesBarcode == "true") {
          this.isDisabled = false;
          this.OnRadioBtnChnge("Tag");
          this.CommonJson();
          //Added on 19-Mar-2021
          this.barcodeService.castInterState.subscribe(
            response => {
              this.interstateDetails = response;
              this._masterService.getCompanyMaster().subscribe(
                Response => {
                  this.CompanyMaster = Response;
                  if (this.CompanyMaster.StateCode != this.interstateDetails.Pos) {
                    this.interstate = 1;
                  }
                  else {
                    this.interstate = 0;
                  }
                  this.BarcodeSummaryHeader.isInterstate = this.interstate;
                }
              )
            }
          )
          //Ends here
        }
      }
    )

    if (this._router.url == "/barcode") {
      this.EnableDisableCtrls = false;
    }
    else {
      this.EnableDisableCtrls = true;
    }

    //Added to get order info based on created order

    this._estimationService.SubjectOrderNo.subscribe(
      response => {
        this.OrderNo = response;
        if (this.OrderNo != 0) {
          this.EnableAddRow = false;
          this.EnableOrderDetails = true;
          this._estimationService.getEstimationOrderDetailsfromAPI(this.OrderNo).subscribe(
            response => {
              this.OrderDetails = response;
              if (this.OrderDetails != null) {
                this.OrderKarat = this.OrderDetails.Karat;
                this.OrderRate = this.OrderDetails.Rate;
              }
            }
          )
        }
      }
    )

    //Ends here

    this.salesService.castAddEditBarcodeDetails.subscribe(
      response => {
        this.GetAddBarcodeList = response;
        this.EnableRate = true;
        this.EnableMCRate = true;
        this.EnableStnDets = true;
        if (this.isEmptyObject(this.GetAddBarcodeList) == false && this.isEmptyObject(this.GetAddBarcodeList) != null) {
          this.HideRadioBtn = false;
          this.EnableAddRow = false;
          this.readOnly = true;
          this.editMode = true;
          this.BarCodeNo = this.GetAddBarcodeList.BarcodeNo;
          this.getAgeofTheBarcode(this.GetAddBarcodeList.BarcodeNo);
          this.onchangeOption(this.GetAddBarcodeList.McType);
          // this.getItemList(this.GetAddBarcodeList.GsCode); //Commented as part of code cleaning.
          // this.getAddWtItemList(this.GetAddBarcodeList.GsCode); //Commented as part of code cleaning.
          // this.getDedWtItemList(this.GetAddBarcodeList.GsCode); //Commented as part of code cleaning.
          this.BarcodeSummaryHeader.CompanyCode = this.ccode;
          this.BarcodeSummaryHeader.BranchCode = this.bcode;
          this.BarcodeSummaryHeader.BarcodeNo = this.BarCodeNo;
          this.BarcodeSummaryHeader.SalCode = this.GetAddBarcodeList.SalCode;
          this.BarcodeSummaryHeader.AdItem = this.GetAddBarcodeList.AdItem;
          this.BarcodeSummaryHeader.AdCounter = this.GetAddBarcodeList.AdCounter;
          this.BarcodeSummaryHeader.isInterstate = this.GetAddBarcodeList.isInterstate;
          this.BarcodeSummaryHeader.VaporLossWeight = this.GetAddBarcodeList.VaporLossWeight;
          this.BarcodeSummaryHeader.VaporLossAmount = this.GetAddBarcodeList.VaporLossAmount;
          //this.BarcodeSummaryHeader.DedItem = this.GetAddBarcodeList.DedItem;
          this.BarcodeSummaryHeader.DedItem = this.BarcodeSummaryHeader.ItemName;

          this.BarcodeSummaryHeader.DedCounter = this.GetAddBarcodeList.DedCounter;
          this.isDisabled = true;
          if (this.GetAddBarcodeList.BarcodeNo != null && this.GetAddBarcodeList.BarcodeNo != "") {
            this.model.name = "Tag";
            this.EnableMCRate = true;
          }
          else {
            this.model.name = "Non Tag";
            this.EnableMCRate = false;
            //this.BarcodeSummaryHeader.BarCodeNo="";
          }
          this._masterService.getCounter(this.GetAddBarcodeList.GsCode, this.GetAddBarcodeList.ItemName).subscribe(
            Response => {
              this.CounterList = Response;
              //this.getkaratAndPieceItem(this.GetAddBarcodeList.GsCode, this.GetAddBarcodeList.ItemName);
              this.getkaratAndPieceItemForEditBarcode(this.GetAddBarcodeList.GsCode, this.GetAddBarcodeList.ItemName);
            }
          )
          this.getBarcodeDetails(this.GetAddBarcodeList);
        }
        else {
          this.HideRadioBtn = true;
        }
      }
    )

    this.AddForm = this.fb.group({
      ItQty: null,
      CounterCode: null,
      ItemName: null,
      ItemNo: null,
      Grwt: null,
      Stwt: null,
      Ntwt: null,
      AddWt: null,
      DeductWt: null,
      AdBarcode: null,
      AdCounter: null,
      AdItem: null,
      AdQty: null,
      AdWt: null,
      tagno: null,
    });
    this.BillForm = this.fb.group({
      BillGrosswt: null,
      NW: null
    });
    this.DeductForm = this.fb.group({
      Qty: null,
      CounterCode: null,
      DDCounter: null,
      GW: null,
      SW: null,
      NW: null,
      DedQty: null,
      Dedwt: null,
      DedSW: null,
      TagNo: null,
      ItemName: null,
      Iname: null,
      ItemQty: null
    });

    this.AddBarcodeForm = this.fb.group({
      ObjID: null,
      ItemQty: null,
      CompanyCode: this.ccode,
      BranchCode: this.bcode,
      EstNo: null,
      SlNo: null,
      BillNo: null,
      BarcodeNo: this.BarCodeNo,
      SalCode: null,
      CounterCode: null,
      ItemName: null,
      ItemNo: null,
      Grosswt: null,
      Stonewt: null,
      Netwt: null,
      AddWt: null,
      BillQty: null,
      DeductWt: null,
      MakingChargePerRs: null,
      WastPercent: null,
      GoldValue: null,
      VaAmount: null,
      StoneCharges: null,
      DiamondCharges: null,
      TotalAmount: null,
      Hallmarkarges: null,
      McAmount: null,
      WastageGrms: null,
      McPercent: null,
      AddQty: null,
      DeductQty: null,
      OfferValue: null,
      UpdateOn: null,
      GsCode: null,
      Rate: null,
      Karat: null,
      AdBarcode: null,
      AdCounter: null,
      AdItem: null,
      IsEDApplicable: null,
      McType: null,
      Fin_Year: null,
      NewBillNo: null,
      ItemTotalAfterDiscount: null,
      ItemAdditionalDiscount: null,
      TaxPercentage: null,
      TaxAmount: null,
      ItemFinalAmount: null,
      SupplierCode: null,
      ItemSize: null,
      ImgID: null,
      DesignCode: null,
      DesignName: null,
      BatchID: null,
      Rf_ID: null,
      McPerPiece: null,
      DiscountMc: null,
      TotalSalesMc: null,
      McDiscountAmt: null,
      purchaseMc: null,
      GSTGroupCode: null,
      SGSTPercent: null,
      SGSTAmount: null,
      CGSTPercent: null,
      CGSTAmount: null,
      IGSTPercent: null,
      IGSTAmount: null,
      HSN: null,
      PieceRate: null,
      DeductSWt: null,
      OrdDiscountAmt: null,
      DedCounter: null,
      DedItem: null,
      VaporLossWeight: null,
      VaporLossAmount: null,
      salesEstStoneVM: []
    });
  }
  OnRadioBtnChnge(arg) {
    if (arg === 'Tag') {
      this.NonTag = false;
      this.model.name = arg;
      this.AddBarcodeForm.reset();
      this.totalAmtStn = 0;
      this.totalAmtDmd = 0;
      this.BillForm.reset();
      this.AddForm.reset();
      this.fieldArray1 = [];
      this.BarcodeSummaryHeader.StoneCharges = 0;
      this.BarcodeSummaryHeader.DiamondCharges = 0;
      this.getStaffList();
      this.getMCTypes();
      //this.BarcodeSummaryHeader.McType = 5;
      //this.onchangeOption(5);
      this.BarcodeSummaryHeader.salesEstStoneVM = [];
      this.readOnly = false;
      this.EnableMCRate = true;
    }
    else if (arg === 'Non Tag') {
      this.model.name = arg;
      this.NonTag = true;
      this.totalAmtStn = 0;
      this.totalAmtDmd = 0;
      this.AddBarcodeForm.reset();
      this.BillForm.reset();
      this.AddForm.reset();
      this.fieldArray1 = [];
      this.BarcodeSummaryHeader.StoneCharges = 0;
      this.BarcodeSummaryHeader.DiamondCharges = 0;
      this.getStaffList();
      this.getMCTypes();
      this.BarcodeSummaryHeader.McType = 5;
      this.onchangeOption(5);
      this.BarcodeSummaryHeader.salesEstStoneVM = [];
      this.BarcodeSummaryHeader.BarcodeNo = "";
      this.readOnly = true;
      this.EnableMCRate = false;
    }
  }


  PopupRadioBtnChnge(arg) {
    if (arg === 'Non Tag') {
      this.Non_tag = true;
      this.radiomodel.name = arg;
      this.AddForm.reset();
      this.BarcodeSummaryHeader.AdBarcode = null;
      // this.BarcodePopupHeader.tagno = this.BarcodeList.BarcodeNo;
    }
    else if (arg === 'Tag') {
      this.Non_tag = false;
      this.radiomodel.name = arg;
      this.AddForm.reset();
      //  this.BarcodePopupHeader.tagno = this.BarcodeList.BarcodeNo;
    }

  }
  isEmptyObject(obj) {
    return (obj && (Object.keys(obj).length === 0));
  }
  addWeight(AddBarcodeForm) {
    if (AddBarcodeForm.value.BarcodeNo == null || AddBarcodeForm.value.BarcodeNo === '') {
      swal("Warning!", 'Please Enter Barcode/TagNo', "warning");
      $('#addWeight').modal('hide');
    }
    else {
      if (this.BarcodeSummaryHeader.GsCode == "PTM") {
        $('#addWeight').modal('hide');
      }
      else {
        $('#addWeight').modal('show');
        this.BarcodePopupHeader.tagno = this.BarcodeList.BarcodeNo;
        this.AddWtPopup();
      }
    }
  }

  AddWtPopup() {
    if (this.BarcodeSummaryHeader.AddWt != 0) {
      if (this.BarcodeSummaryHeader.AdBarcode != null && this.BarcodeSummaryHeader.AdBarcode != "") {
        this.radiomodel.name = "Tag";
        this.barcodeService.addWtBarcodeCalculation(this.BarcodeSummaryHeader.BarcodeNo, this.BarcodeSummaryHeader.AdBarcode).subscribe(
          Response => {
            this.AddWtDts = Response;
            this.BarcodePopupHeader.Grwt = this.AddWtDts.Grosswt;
            this.BarcodePopupHeader.ItQty = this.AddWtDts.ItemQty;
            this.BarcodePopupHeader.Stwt = this.AddWtDts.Stonewt;
            this.BarcodePopupHeader.Ntwt = this.AddWtDts.Netwt;
            this.BarcodePopupHeader.tagno = this.BarcodeList.BarcodeNo;
            this.BarcodePopupHeader.ItemName = this.BarcodeSummaryHeader.AdItem;
            this.BarcodePopupHeader.CounterCode = this.BarcodeSummaryHeader.AdCounter;
            this.BarcodeSummaryHeader.AdBarcode = this.BarcodeSummaryHeader.AdBarcode;
          }
        )
        this.BarcodePopupHeader.AdWt = this.BarcodeSummaryHeader.AddWt;
        this.BarcodePopupHeader.AdQty = this.BarcodeSummaryHeader.AddQty;
      }
      else {
        this.radiomodel.name = "Non Tag";
        this.BarcodePopupHeader.AdWt = this.BarcodeSummaryHeader.AddWt;
        this.BarcodePopupHeader.AdQty = this.BarcodeSummaryHeader.AddQty;
        this.BarcodePopupHeader.ItemName = this.BarcodeSummaryHeader.AdItem;
        this.BarcodePopupHeader.CounterCode = this.BarcodeSummaryHeader.AdCounter;
      }
    }
    else {
      this.radiomodel.name = "Non Tag";
    }
  }

  refreshExistData() {
    this.EnableStnDets = true;
    this.barcodeService.SendBarcodeDataToSalesComp({});
    // if (this.editMode == true) {
    //   this.barcodeService.SendBarcodeDataToSalesComp(this.BarcodeSummaryHeader);
    //   alert().log(this.BarcodeSummaryHeader);
    // }
  }

  DedGrWt(arg) {
    if (arg > this.BarcodeSummaryHeader.Grosswt) {
      swal("Warning!", 'Deduct wt should not be greater than gross wt', "warning");
    }
    else {

    }
  }

  EditDedStwt() {
    if (this.BarcodeSummaryHeader.Stonewt == 0 || this.BarcodeSummaryHeader.Stonewt == null) {
      swal("Warning!", 'No stone wt to deduct', "warning");
    }
    else {
      swal("Warning!", 'Item has stone details. Edit or delete from stone details for deduction', "warning");
    }
  }

  DeductWeight(AddBarcodeForm) {
    // if (AddBarcodeForm.value.BarcodeNo == null || AddBarcodeForm.value.BarcodeNo === '') {
    //   $('#DeductWeight').modal('hide');
    // }
    // else {
    //   $('#DeductWeight').modal('show');
    //   this.DeductHeader.TagNo = this.BarcodeList.BarcodeNo;
    // }
    this.DedWtPopup();
  }


  GetDeductWt() {
    if (this.BarcodeSummaryHeader.DeductWt > this.BarcodeSummaryHeader.Grosswt) {
      swal("Warning!", 'Enter valid deduct wt. It cannot be greater than gross wt', "warning");
    }
    else {
      const billGrWt = this.BarcodeSummaryHeader.Grosswt;
      const billNetWt = this.BarcodeSummaryHeader.Netwt;
      const qty = this.BarcodeSummaryHeader.ItemQty;
      const Stwt = this.BarcodeSummaryHeader.Stonewt;
      this.BarcodeSummaryHeader.DedItem = this.BarcodeSummaryHeader.ItemName;
      this.BillHeader.BillGrosswt = parseFloat((+billGrWt + +this.BarcodeSummaryHeader.AddWt - +this.BarcodeSummaryHeader.DeductWt).toFixed(3));
      this.BillHeader.NW = parseFloat((+billGrWt + +this.BarcodeSummaryHeader.AddWt - +this.BarcodeSummaryHeader.Stonewt - +this.BarcodeSummaryHeader.DeductWt).toFixed(3)); //added on 31-Mar-20
      this.calculateBarcodeDetails();
    }
  }

  DedWtPopup() {
    if (this.BarcodeSummaryHeader.DeductWt != 0) {
      this.DeductHeader.Dedwt = this.BarcodeSummaryHeader.DeductWt;
      this.DeductHeader.DDCounter = this.BarcodeSummaryHeader.DedCounter;
      //this.DeductHeader.ItemName = this.BarcodeSummaryHeader.DedItem;
      this.DeductHeader.ItemName = this.BarcodeSummaryHeader.ItemName;

    }
  }

  toggle: string = "Invalid";
  McAmt: Number;
  onchangeOption(OptionmodeArg) {
    // this.clearValues();
    if (OptionmodeArg == "5") {
      this.toggle = "MC%";
      this.BarcodeSummaryHeader.MakingChargePerRs = null;
      this.BarcodeSummaryHeader.McPerPiece = null;
      this.BarcodeSummaryHeader.WastPercent = null;
      this.BarcodeSummaryHeader.McAmount = null;
      this.BarcodeSummaryHeader.WastageGrms = null;
    }
    else if (OptionmodeArg == "1") {
      this.toggle = "MC PER GRAM";
      this.BarcodeSummaryHeader.McPerPiece = null;
      this.BarcodeSummaryHeader.McPercent = null;
      this.BarcodeSummaryHeader.McAmount = null;
      this.BarcodeSummaryHeader.WastageGrms = null;
      this.BarcodeSummaryHeader.TotalSalesMc = null;
    }
    else if (OptionmodeArg == "4") {
      this.toggle = "MC Amount - Wastg";
      this.BarcodeSummaryHeader.MakingChargePerRs = null;
      this.BarcodeSummaryHeader.McPerPiece = null;
      this.BarcodeSummaryHeader.McPercent = null;
      this.BarcodeSummaryHeader.WastPercent = null;
    }
    else if (OptionmodeArg == "6") {
      this.toggle = "MC PER PIECE";
      this.BarcodeSummaryHeader.MakingChargePerRs = null;
      this.BarcodeSummaryHeader.McPercent = null;
      this.BarcodeSummaryHeader.WastPercent = null;
      this.BarcodeSummaryHeader.McAmount = null;
      this.BarcodeSummaryHeader.WastageGrms = null;
    }
    else {
      this.toggle = "NA";
      this.BarcodeSummaryHeader.MakingChargePerRs = null;
      this.BarcodeSummaryHeader.McPerPiece = null;
      this.BarcodeSummaryHeader.McPercent = null;
      this.BarcodeSummaryHeader.WastPercent = null;
      this.BarcodeSummaryHeader.McAmount = null;
      this.BarcodeSummaryHeader.WastageGrms = null;
    }
  }

  StaffList: any;
  getStaffList() {
    this._masterService.getSalesMan().subscribe(
      Response => {
        this.StaffList = Response;
        this.barcodeService.castGetSalCode.subscribe(
          response => {
            this.salCode = response;
            this.BarcodeSummaryHeader.SalCode = this.salCode;
            this.EnableBarcodeAge = false;
          }
        );
      }
    )
  }
  GSList: any;
  getGSList() {
    this._masterService.getGsList('S').subscribe(
      Response => {
        this.GSList = Response;
      }
    )
  }
  ItemList: any;
  getItemList(arg) {
    this.barcodeService.getItemfromAPI(arg).subscribe(
      response => {
        this.ItemList = response;
      }
    )
  }
  CounterList: any;
  getCounter(arg) {
    if (arg == "CHCN") {
      this.EnableAddRow = true;
      this.fieldArray1 = [];
    }
    else {
      this.EnableAddRow = false;
    }
    this._masterService.getCounter(this.AddBarcodeForm.value.GsCode, arg).subscribe(
      Response => {
        this.CounterList = Response;
        if (this.CounterList.length == 1) {
          this.BarcodeSummaryHeader.CounterCode = this.CounterList[0];
        }
        this.getkaratAndPieceItem(this.AddBarcodeForm.value.GsCode, arg);
      }
    )
  }
  AddWtItemList: any;
  getAddWtItemList(arg) {
    this.barcodeService.getItemfromAPI(arg).subscribe(
      response => {
        this.AddWtItemList = response;
      }
    )
  }
  AddWtCounterList: any;
  getAddWtCounter(arg) {
    this._masterService.getCounter(this.AddBarcodeForm.value.GsCode, arg).subscribe(
      Response => {
        this.AddWtCounterList = Response;
        //this.getkaratAndPieceItem(this.AddBarcodeForm.value.GsCode, arg);
      }
    )
  }
  DedWtItemList: any;
  getDedWtItemList(arg) {
    this.barcodeService.getItemfromAPI(arg).subscribe(
      response => {
        this.DedWtItemList = response;
      }
    )
  }
  DedWtCounterList: any;
  getDedWtCounter(arg) {
    this._masterService.getCounter(this.AddBarcodeForm.value.GsCode, arg).subscribe(
      Response => {
        this.DedWtCounterList = Response;
        //this.getkaratAndPieceItem(this.AddBarcodeForm.value.GsCode, arg);
      }
    )
  }
  KaratAndPieceItemList: any;
  PieceRate: string = "N";
  rateParams: any = {
    GsCode: null,
    Karat: null,
  }

  getkaratAndPieceItem(GsCode, ItemName) {
    this._masterService.getkaratAndPieceItem(GsCode, ItemName).subscribe(
      Response => {
        this.KaratAndPieceItemList = Response;
        if (this.KaratAndPieceItemList.length > 0) {
          this.PieceRate = this.KaratAndPieceItemList[0].IsPiece;
          if (this.PieceRate == "Y" && this.BarcodeSummaryHeader.GsCode == "SL") {
            this.EnableDisableAddWt = false;
            this.EnableDisableDedGrWt = false;
          }
          else {
            this.EnableDisableAddWt = true;
            this.EnableDisableDedGrWt = true;
          }
          this.BarcodeSummaryHeader.PieceRate = this.PieceRate;
          this.BarcodeSummaryHeader.Karat = this.KaratAndPieceItemList[0].Karat;
          this.rateParams = {
            GsCode: GsCode,
            Karat: this.KaratAndPieceItemList[0].Karat,
          }
          let RateList;
          if (this.OrderRate != 0) {
            this.BarcodeSummaryHeader.Rate = this.OrderRate;
          }
          else {
            this._masterService.getRateforAddBarcode(this.rateParams).subscribe(
              Response => {
                RateList = Response;
                this.BarcodeSummaryHeader.Rate = RateList.Rate;
              }
            );
          }
        }
        this.getGstPercent(GsCode, ItemName);
      }
    )
  }



  getkaratAndPieceItemForEditBarcode(GsCode, ItemName) {
    this._masterService.getkaratAndPieceItem(GsCode, ItemName).subscribe(
      Response => {
        this.KaratAndPieceItemList = Response;
        if (this.KaratAndPieceItemList.length > 0) {
          this.PieceRate = this.KaratAndPieceItemList[0].IsPiece;
          if (this.PieceRate == "Y" && this.BarcodeSummaryHeader.GsCode == "SL") {
            this.EnableDisableAddWt = false;
            this.EnableDisableDedGrWt = false;
          }
          else {
            this.EnableDisableAddWt = true;
            this.EnableDisableDedGrWt = true;
          }
          this.BarcodeSummaryHeader.PieceRate = this.PieceRate;
          this.BarcodeSummaryHeader.Karat = this.KaratAndPieceItemList[0].Karat;
          this.rateParams = {
            GsCode: GsCode,
            Karat: this.KaratAndPieceItemList[0].Karat,
          }
          // let RateList;
          // if (this.OrderRate != 0) {
          //   this.BarcodeSummaryHeader.Rate = this.OrderRate;
          // }
          // else {
          //   this._masterService.getRateforAddBarcode(this.rateParams).subscribe(
          //     Response => {
          //       RateList = Response;
          //       this.BarcodeSummaryHeader.Rate = RateList.Rate;
          //     }
          //   );
          // }
        }
        this.getGstPercent(GsCode, ItemName);
      }
    )
  }




  getRate(arg) {
    let RateList;
    let rate;
    if (this.OrderRate != 0) {
      this.BarcodeSummaryHeader.Rate = this.OrderRate;
    }
    else {
      this._masterService.getRate(arg.value).subscribe(
        Response => {
          RateList = Response;
          rate = RateList.Rate;
          this.BarcodeSummaryHeader.Rate = rate;
        }
      );
    }
  }
  MCList: any;
  getMCTypes() {
    this._masterService.getMCTypes().subscribe(
      Response => {
        this.MCList = Response;
      }
    )
  }

  interstateDetails: any;
  CompanyMaster: any;
  interstate: number = 0;

  getInterState() {
    this.EnableRate = true;
    this.EnableMCRate = true;
    this.EnableStnDets = true;
    this.barcodeService.castInterState.subscribe(
      response => {
        this.interstateDetails = response;
        this._masterService.getCompanyMaster().subscribe(
          Response => {
            this.CompanyMaster = Response;
            if (this.CompanyMaster.StateCode != this.interstateDetails.Pos) {
              this.interstate = 1;
            }
            else {
              this.interstate = 0;
            }
          }
        )
      }
    )
  }

  BarcodeList: any = [];
  BarcodeNo: string = "";
  getDetails(arg) {
    this.BarcodeNo = arg;
    this.barcodeService.castInterState.subscribe(
      response => {
        this.interstateDetails = response;
        this._masterService.getCompanyMaster().subscribe(
          Response => {
            this.CompanyMaster = Response;
            if (this.CompanyMaster.StateCode != this.interstateDetails.Pos) {
              this.interstate = 1;
            }
            else {
              this.interstate = 0;
            }
            if (this.BarcodeNo != "" && this.BarcodeNo != "0") {
              this.salesService.getBarcodefromAPI(this.BarcodeNo, this.interstateDetails.OrderNo, this.interstate, 0).subscribe(
                Response => {
                  this.BarcodeList = Response;
                  if (this.BarcodeList.TaggingType == "G") {
                    swal("Warning!", "Group Barcode cannot be added", "warning");
                    this.BarcodeList = [];
                    this.tagno.nativeElement.value = "";
                    this.BarcodeNo = "";
                  }
                  else {
                    this.BarCodeNo = arg;
                    this.readOnly = true;
                    this.getAgeofTheBarcode(arg);
                    this.BarcodeSummaryHeader.CompanyCode = this.ccode;
                    this.BarcodeSummaryHeader.BranchCode = this.bcode;
                    this.BarcodeSummaryHeader.AdItem = this.BarcodeList.AdItem;
                    this.BarcodeSummaryHeader.AdCounter = this.BarcodeList.AdCounter;
                    this.BarcodeSummaryHeader.DedItem = this.BarcodeList.DedItem;
                    this.BarcodeSummaryHeader.DedCounter = this.BarcodeList.DedCounter;
                    this.BarcodeSummaryHeader.Grosswt = this.BarcodeList.Grosswt;
                    this.BarcodeSummaryHeader.ItemQty = this.BarcodeList.ItemQty;
                    this.BarcodeSummaryHeader.Stonewt = this.BarcodeList.Stonewt;
                    this.BarcodeSummaryHeader.Netwt = this.BarcodeList.Netwt;
                    this.BarcodeSummaryHeader.AddWt = this.BarcodeList.AddWt;
                    // this.BarcodeSummaryHeader.isInterstate = this.BarcodeList.interstate;
                    this.BarcodeSummaryHeader.isInterstate = this.interstate;
                    this.BarcodeSummaryHeader.DesignName = this.BarcodeList.DesignName;
                    this.BarcodeSummaryHeader.CounterCode = this.BarcodeList.CounterCode;
                    this.BarcodeSummaryHeader.Rate = this.BarcodeList.Rate;
                    this.BarcodeSummaryHeader.ItemSize = this.BarcodeList.ItemSize;
                    this.BarcodeSummaryHeader.PieceRate = this.BarcodeList.PieceRate;
                    this.BarcodeSummaryHeader.McPerPiece = this.BarcodeList.McPerPiece;
                    this.BarcodeSummaryHeader.McPercent = this.BarcodeList.McPercent;
                    this.BarcodeSummaryHeader.WastPercent = this.BarcodeList.WastPercent;
                    this.BarcodeSummaryHeader.McAmount = this.BarcodeList.McAmount;
                    this.BarcodeSummaryHeader.WastageGrms = this.BarcodeList.WastageGrms;
                    this.BarcodeSummaryHeader.Karat = this.BarcodeList.Karat;
                    this.BarcodeSummaryHeader.GsCode = this.BarcodeList.GsCode;
                    this.BarcodeSummaryHeader.DeductWt = this.BarcodeList.DeductWt;
                    this.BarcodeSummaryHeader.ItemName = this.BarcodeList.ItemName;
                    this.BarcodeSummaryHeader.CounterCode = this.BarcodeList.CounterCode;
                    this.BarcodeSummaryHeader.TotalSalesMc = this.BarcodeList.TotalSalesMc;
                    this.BarcodeSummaryHeader.McDiscountAmt = this.BarcodeList.McDiscountAmt;
                    this.BarcodeSummaryHeader.GoldValue = this.BarcodeList.GoldValue;
                    this.BarcodeSummaryHeader.VaAmount = parseFloat((this.BarcodeList.VaAmount).toFixed(2));
                    this.BarcodeSummaryHeader.StoneCharges = this.BarcodeList.StoneCharges;
                    this.BarcodeSummaryHeader.DiamondCharges = this.BarcodeList.DiamondCharges;
                    this.BarcodeSummaryHeader.ItemTotalAfterDiscount = this.BarcodeList.ItemTotalAfterDiscount;
                    this.BarcodeSummaryHeader.SGSTAmount = this.BarcodeList.SGSTAmount;
                    this.BarcodeSummaryHeader.CGSTAmount = this.BarcodeList.CGSTAmount;
                    this.BarcodeSummaryHeader.IGSTAmount = this.BarcodeList.IGSTAmount;
                    this.BarcodeSummaryHeader.SGSTPercent = this.BarcodeList.SGSTPercent;//Added on 19-Mar-2021
                    this.BarcodeSummaryHeader.CGSTPercent = this.BarcodeList.CGSTPercent;//Added on 19-Mar-2021
                    this.BarcodeSummaryHeader.IGSTPercent = this.BarcodeList.IGSTPercent;//Added on 19-Mar-2021
                    this.BarcodeSummaryHeader.ItemFinalAmount = this.BarcodeList.ItemFinalAmount;
                    this.BarcodeSummaryHeader.DiscountMc = this.BarcodeList.DiscountMc;
                    this.BarcodeSummaryHeader.VaporLossWeight = this.BarcodeList.VaporLossWeight;
                    this.BarcodeSummaryHeader.VaporLossAmount = this.BarcodeList.VaporLossAmount;
                    this.BarcodeSummaryHeader.TotalAmount = this.BarcodeList.ItemTotalAfterDiscount;
                    //this.BarcodeSummaryHeader.TotalAmount = this.BarcodeList.TotalAmount;
                    this.BarcodeSummaryHeader.ItemAdditionalDiscount = this.BarcodeList.ItemAdditionalDiscount;
                    this.BarcodeSummaryHeader.BillQty = parseInt(this.BarcodeList.ItemQty) + parseInt(this.BarcodeList.AddQty);
                    this.BarcodeSummaryHeader.McType = this.BarcodeList.McType;
                    this.BarcodeSummaryHeader.MakingChargePerRs = this.BarcodeList.MakingChargePerRs;
                    this.BarcodeSummaryHeader.GSTGroupCode = this.BarcodeList.GSTGroupCode;
                    const billGrWt = this.BarcodeSummaryHeader.Grosswt;
                    const billNetWt = this.BarcodeSummaryHeader.Netwt;
                    this.BillHeader.BillGrosswt = Number(+billGrWt + +this.BarcodePopupHeader.AdWt);
                    this.BillHeader.NW = Number(+billNetWt + +this.BarcodePopupHeader.AdWt);
                    this.BarcodeSummaryHeader.salesEstStoneVM = this.BarcodeList.salesEstStoneVM;
                    this.BarcodeSummaryHeader.GSTGroupCode = this.BarcodeList.GSTGroupCode;
                    //this.BarcodeSummaryHeader.isInterstate = this.BarcodeList.isInterstate;
                    if (parseFloat(this.BarcodeSummaryHeader.DeductWt) > 0) {
                      this.BillHeader.BillGrosswt = parseFloat((+this.BarcodeSummaryHeader.Grosswt - +this.BarcodeSummaryHeader.DeductWt - +this.DeductHeader.DedSW).toFixed(3));
                      this.BillHeader.NW = parseFloat((+this.BarcodeSummaryHeader.Grosswt - +this.BarcodeSummaryHeader.DeductWt - +this.DeductHeader.DedSW).toFixed(3));
                    }
                    //  this.BarcodeList.salesEstStoneVM = this.fieldArray1;
                    //this.onchangeOption(this.BarcodeSummaryHeader.McType);
                    this.onchangeOption(this.BarcodeSummaryHeader.McType);
                    this.fieldArray1 = this.BarcodeList.salesEstStoneVM;
                    for (var field in this.fieldArray1) {
                      this.EnableEditDelbtn[field] = true;
                      this.EnableSaveCnlbtn[field] = false;
                      this.readonly[field] = true;
                      this.readonly1[field] = true;
                      this.readonlyRate[field] = true;
                      this.getStoneName(this.BarcodeList.salesEstStoneVM[field].Type, field);
                    }
                    this.getItemList(this.BarcodeList.GsCode);
                    //this.getAddWtItemList(this.BarcodeList.GsCode); //Commented as part of code cleaning.
                    //this.getDedWtItemList(this.BarcodeList.GsCode); //Commented as part of code cleaning.
                    //get Counter and bind to Counter dropdown
                    this._masterService.getCounter(this.BarcodeList.GsCode, this.BarcodeList.ItemName).subscribe(
                      Response => {
                        this.CounterList = Response;
                        this.getkaratAndPieceItem(this.BarcodeList.GsCode, this.BarcodeList.ItemName);
                      }
                    )
                  }
                }
              )
            }
          }
        )
      }
    )
  }

  getBarcodeDetails(arg) {
    this.storeExistingBarcodeDets = Object.assign({}, arg);
    this.fieldArray1 = [];
    this.BarCodeNo = arg.BarcodeNo;
    this.BarcodeList = arg;
    this.BarcodeSummaryHeader.CompanyCode = this.ccode;
    this.BarcodeSummaryHeader.BranchCode = this.bcode;
    this.BarcodeSummaryHeader.AdItem = this.BarcodeList.AdItem;
    this.BarcodeSummaryHeader.AdCounter = this.BarcodeList.AdCounter;
    this.BarcodeSummaryHeader.DedItem = this.BarcodeList.DedItem;
    this.BarcodeSummaryHeader.DedCounter = this.BarcodeList.DedCounter;
    this.getItemList(this.BarcodeList.GsCode);
    //this.getAddWtItemList(this.BarcodeList.GsCode); //Commented as part of code cleaning.
    //this.getDedWtItemList(this.BarcodeList.GsCode); //Commented as part of code cleaning.
    this.BarcodeSummaryHeader.BarcodeNo = this.BarCodeNo;
    this.BarcodeSummaryHeader.SalCode = this.BarcodeList.SalCode;
    this.BarcodeSummaryHeader.Grosswt = this.BarcodeList.Grosswt;
    this.BarcodeSummaryHeader.ItemQty = this.BarcodeList.ItemQty;
    this.BarcodeSummaryHeader.Stonewt = this.BarcodeList.Stonewt;
    this.BarcodeSummaryHeader.AdBarcode = this.BarcodeList.AdBarcode;
    this.BarcodeSummaryHeader.Netwt = this.BarcodeList.Netwt;
    this.BarcodeSummaryHeader.AddWt = this.BarcodeList.AddWt;
    this.BarcodeSummaryHeader.AddQty = this.BarcodeList.AddQty;
    this.BarcodeSummaryHeader.DesignName = this.BarcodeList.DesignName;
    this.BarcodeSummaryHeader.Rate = this.BarcodeList.Rate;
    this.BarcodeSummaryHeader.ItemSize = this.BarcodeList.ItemSize;
    this.BarcodeSummaryHeader.PieceRate = this.BarcodeList.PieceRate;
    this.BarcodeSummaryHeader.McType = this.BarcodeList.McType;
    this.BarcodeSummaryHeader.McPerPiece = this.BarcodeList.McPerPiece;
    this.BarcodeSummaryHeader.McPercent = this.BarcodeList.McPercent;
    this.BarcodeSummaryHeader.WastPercent = this.BarcodeList.WastPercent;
    this.BarcodeSummaryHeader.McAmount = this.BarcodeList.McAmount;
    this.BarcodeSummaryHeader.WastageGrms = this.BarcodeList.WastageGrms;
    this.BarcodeSummaryHeader.Karat = this.BarcodeList.Karat;
    this.BarcodeSummaryHeader.GsCode = this.BarcodeList.GsCode;
    this.BarcodeSummaryHeader.ItemName = this.BarcodeList.ItemName;
    this.BarcodeSummaryHeader.DiscountMc = this.BarcodeList.DiscountMc;
    this.BarcodeSummaryHeader.VaporLossWeight = this.BarcodeList.VaporLossWeight;
    this.BarcodeSummaryHeader.VaporLossAmount = this.BarcodeList.VaporLossAmount;
    this.BarcodeSummaryHeader.CounterCode = this.BarcodeList.CounterCode;
    this.BarcodeSummaryHeader.SGSTPercent = this.BarcodeList.SGSTPercent;//Added on 19-Mar-2021
    this.BarcodeSummaryHeader.CGSTPercent = this.BarcodeList.CGSTPercent;//Added on 19-Mar-2021
    this.BarcodeSummaryHeader.IGSTPercent = this.BarcodeList.IGSTPercent;//Added on 19-Mar-2021
    this.BarcodeSummaryHeader.DeductWt = this.BarcodeList.DeductWt;
    this.BarcodeSummaryHeader.TotalSalesMc = this.BarcodeList.TotalSalesMc;
    this.BarcodeSummaryHeader.McDiscountAmt = this.BarcodeList.McDiscountAmt;
    this.BarcodeSummaryHeader.GoldValue = this.BarcodeList.GoldValue;
    this.BarcodeSummaryHeader.VaAmount = parseFloat((this.BarcodeList.VaAmount).toFixed(2));
    // if(this.BarcodeList.salesEstStoneVM.length > 0){
    //   for (var field in this.BarcodeList.salesEstStoneVM) {
    //    if(this.BarcodeList.salesEstStoneVM[field].Type == "STN"){
    //     this.BarcodeSummaryHeader.StoneCharges += this.BarcodeList.salesEstStoneVM[field].Amount;
    //    }
    //   }
    // }
    this.BarcodeSummaryHeader.StoneCharges = this.BarcodeList.StoneCharges;
    this.BarcodeSummaryHeader.DiamondCharges = this.BarcodeList.DiamondCharges;
    // this.BarcodeSummaryHeader.ItemTotalAfterDiscount = this.BarcodeList.ItemTotalAfterDiscount;
    this.BarcodeSummaryHeader.ItemTotalAfterDiscount = this.BarcodeList.ItemTotalAfterDiscount;
    this.BarcodeSummaryHeader.SGSTAmount = this.BarcodeList.SGSTAmount;
    this.BarcodeSummaryHeader.CGSTAmount = this.BarcodeList.CGSTAmount;
    this.BarcodeSummaryHeader.IGSTAmount = this.BarcodeList.IGSTAmount;
    this.BarcodeSummaryHeader.ItemFinalAmount = this.BarcodeList.ItemFinalAmount;
    this.BarcodeSummaryHeader.GSTGroupCode = this.BarcodeList.GSTGroupCode;
    //this.BarcodeSummaryHeader.TotalAmount = this.BarcodeList.ItemTotalAfterDiscount;
    this.BarcodeSummaryHeader.TotalAmount = this.BarcodeList.TotalAmount;
    this.BarcodeSummaryHeader.ItemAdditionalDiscount = this.BarcodeList.ItemAdditionalDiscount;
    this.BarcodeSummaryHeader.MakingChargePerRs = this.BarcodeList.MakingChargePerRs;
    if (this.BarcodeList.AddQty != null && this.BarcodeList.AddQty != 0) {
      this.BarcodeSummaryHeader.BillQty = parseInt(this.BarcodeList.ItemQty) + parseInt(this.BarcodeList.AddQty);
    }
    else {
      this.BarcodeSummaryHeader.BillQty = parseInt(this.BarcodeList.ItemQty);
    }
    this.fieldArray1 = this.BarcodeList.salesEstStoneVM;

    //this.calculateBarcodeDetails();


    const billGrWt = this.BarcodeSummaryHeader.Grosswt;
    const billNetWt = this.BarcodeSummaryHeader.Netwt;
    this.BillHeader.BillGrosswt = Number(+billGrWt + +this.BarcodePopupHeader.AdWt);
    this.BillHeader.NW = Number(+billNetWt + +this.BarcodePopupHeader.AdWt);

    if (parseFloat(this.BarcodeSummaryHeader.DeductWt) > 0) {
      this.BillHeader.BillGrosswt = parseFloat((+this.BarcodeSummaryHeader.Grosswt - +this.BarcodeSummaryHeader.DeductWt - +this.DeductHeader.DedSW).toFixed(3));
      this.BillHeader.NW = parseFloat((+this.BarcodeSummaryHeader.Grosswt - +this.BarcodeSummaryHeader.DeductWt - +this.DeductHeader.DedSW).toFixed(3));
    }

    for (var field in this.fieldArray1) {
      this.EnableEditDelbtn[field] = true;
      this.EnableSaveCnlbtn[field] = false;
      this.readonly[field] = true;
      this.readonly1[field] = true;
      this.readonlyRate[field] = true;
      this.BarcodeSummaryHeader.salesEstStoneVM[field] = this.fieldArray1[field];
      this.getStoneName(this.BarcodeList.salesEstStoneVM[field].Type, field);
    }
    this.BarcodeList = [];

    if (this.BarcodeSummaryHeader.VaporLossWeight != null) {
      this.BillHeader.BillGrosswt = parseFloat((this.BarcodeSummaryHeader.Grosswt + +this.BarcodeSummaryHeader.AddWt - +this.BarcodeSummaryHeader.DeductWt - +this.BarcodeSummaryHeader.VaporLossWeight).toFixed(3));
      this.BillHeader.NW = parseFloat((this.BarcodeSummaryHeader.Grosswt + +this.BarcodeSummaryHeader.AddWt - +this.BarcodeSummaryHeader.Stonewt - +this.BarcodeSummaryHeader.DeductWt - +this.BarcodeSummaryHeader.VaporLossWeight).toFixed(3)); //added on 31-Mar-20
    }
    else {
      this.BillHeader.BillGrosswt = parseFloat((this.BarcodeSummaryHeader.Grosswt + +this.BarcodeSummaryHeader.AddWt - +this.BarcodeSummaryHeader.DeductWt).toFixed(3));
      this.BillHeader.NW = parseFloat((this.BarcodeSummaryHeader.Grosswt + +this.BarcodeSummaryHeader.AddWt - +this.BarcodeSummaryHeader.Stonewt - +this.BarcodeSummaryHeader.DeductWt).toFixed(3)); //added on 31-Mar-20
    }

  }


  AddWtDts: any;
  Addwt(arg) {
    if (arg != null && arg != 0) {
      if (this.AlphaNumericValidations(arg) == false) {
        swal("Warning!", 'Please enter valid Barcode No', "warning");
      }
      else {
        this.barcodeService.addWtBarcodeCalculation(this.BarcodeSummaryHeader.BarcodeNo, arg).subscribe(
          Response => {
            this.AddWtDts = Response;
            this.BarcodePopupHeader.Grwt = this.AddWtDts.Grosswt;
            this.BarcodePopupHeader.ItQty = this.AddWtDts.ItemQty;
            this.BarcodePopupHeader.Stwt = this.AddWtDts.Stonewt;
            this.BarcodePopupHeader.Ntwt = this.AddWtDts.Netwt;
            this.BarcodePopupHeader.AdWt = this.AddWtDts.AddWt;
            this.BarcodePopupHeader.AdQty = this.AddWtDts.AddQty;
            this.BarcodePopupHeader.tagno = this.BarcodeList.BarcodeNo;
            this.BarcodePopupHeader.ItemName = this.AddWtDts.ItemName;
            this.BarcodeSummaryHeader.AdBarcode = arg;
            this._masterService.getCounter(this.AddBarcodeForm.value.GsCode, this.BarcodePopupHeader.ItemName).subscribe(
              Response => {
                this.AddWtCounterList = Response;
                this.BarcodePopupHeader.CounterCode = this.AddWtDts.CounterCode;
                //this.getkaratAndPieceItem(this.AddBarcodeForm.value.GsCode, arg);
              }
            )
          },
          (err) => {
            if (err.status === 404) {
              const validationError = err.error;
              swal("Warning!", validationError.description, "warning");
              this.PopupRadioBtnChnge('Tag');
            }
            else {
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

  //get GST Percentage 
  GstPercentList: any;
  CGSTPer: any;
  SGSTPer: any;
  IGSTPer: any;
  getGstPercent(GsCode, ItemName) {
    this._masterService.getGSTPercent(GsCode, ItemName).subscribe(
      Response => {
        this.GstPercentList = Response;
        this.CGSTPer = this.GstPercentList[0].GSTPercent;
        this.SGSTPer = this.GstPercentList[1].GSTPercent;
        this.IGSTPer = this.GstPercentList[2].GSTPercent;
      }
    )
  }

  weight: any;
  //Calculation 
  SaveAddWt() {
    if (this.BarcodePopupHeader.AdWt == null || this.BarcodePopupHeader.AdWt == '') {
      swal("Warning!", 'Please fill in the Add Weight field', "warning");
    }
    else {
      this.barcodeService.getAddWightItemGrossWeight(this.ccode, this.bcode, this.BarcodeSummaryHeader.GsCode,
        this.BarcodePopupHeader.CounterCode, this.BarcodePopupHeader.ItemName).subscribe(
          response => {
            this.weight = response;
            if (this.weight.wight == 0) {
              swal("Warning!", 'Item weight is Nil, It cannot be added', "warning");
            }
            else if (this.BarcodePopupHeader.AdWt > this.weight.wight) {
              swal("Warning!", 'Item weight is less than ' + this.BarcodePopupHeader.AdWt + ' grams,It cannot be added.', "warning");
            }
            else {
              $('#addWeight').modal('hide');
              const GrossWt = this.BarcodeSummaryHeader.Grosswt;
              const AddWt = this.BarcodePopupHeader.AdWt;
              const qty = this.BarcodePopupHeader.AdQty;
              this.BarcodeSummaryHeader.AddQty = qty;
              //this.BarcodeSummaryHeader.ItemQty = parseFloat((+qty + +this.BarcodePopupHeader.AdQty).toFixed(3));
              this.BarcodeSummaryHeader.AddWt = this.BarcodePopupHeader.AdWt;
              this.BarcodeSummaryHeader.AdItem = this.BarcodePopupHeader.ItemName;
              this.BarcodeSummaryHeader.AdCounter = this.BarcodePopupHeader.CounterCode;
              this.BillHeader.BillGrosswt = parseFloat((+GrossWt + +AddWt).toFixed(3));
              this.BillHeader.BillGrosswt = parseFloat((+this.BillHeader.BillGrosswt - +this.BarcodeSummaryHeader.DeductWt).toFixed(3));
              this.BillHeader.NW = parseFloat((+this.BarcodeSummaryHeader.AddWt + +this.BarcodeSummaryHeader.Netwt).toFixed(3));
              this.BillHeader.NW = parseFloat((+this.BillHeader.NW - +this.BarcodeSummaryHeader.DeductWt).toFixed(3));
              //this.BillHeader.NW = Number(+this.BarcodeSummaryHeader.Netwt + +this.BarcodeSummaryHeader.AddWt); //added on 30-Mar-20

              this.BarcodeSummaryHeader.BillQty = parseInt(this.BarcodeSummaryHeader.ItemQty) + parseInt(this.BarcodeSummaryHeader.AddQty);
              // this.BarcodeSummaryHeader.ItemQty = parseInt(this.BarcodeSummaryHeader.ItemQty) + parseInt(this.BarcodePopupHeader.AdQty);
              this.calculateBarcodeDetails();
              this.BarcodePopupHeader = {
                ItQty: null,
                CounterCode: null,
                ItemName: null,
                ItemNo: null,
                Grwt: null,
                Stwt: null,
                Ntwt: null,
                AddWt: null,
                DeductWt: null,
                AdBarcode: null,
                AdCounter: null,
                AdItem: null,
                AdWt: null,
                AdQty: null,
                tagno: null,
              };
            }
          }
        )
    }
  }

  SaveDeductWt() {
    if (this.DeductHeader.ItemName == null || this.DeductHeader.ItemName == 0) {
      swal("Warning!", 'Please select item', "warning");
    }
    else if (this.DeductHeader.DedQty >= this.BarcodeSummaryHeader.ItemQty) {
      swal("Warning!", 'Deduct qty should not be greater than or equal to Item quantity.It cannot be deducted.', "warning");
    }
    else if (this.DeductHeader.Dedwt == null || this.DeductHeader.Dedwt == '') {
      swal("Warning!", 'Please fill in the Deduct Weight field', "warning");
    }
    else if (this.DeductHeader.Dedwt >= this.BarcodeSummaryHeader.Grosswt) {
      swal("Warning!", 'Item Weight is less or equal ' + this.DeductHeader.Dedwt + ' grams, It cannot be deducted.', "warning");
    }
    else if (this.DeductHeader.DedSW > this.BarcodeSummaryHeader.Stonewt) {
      swal("Warning!", 'Deduct stone weight is greater than Item stone weight.It cannot be deducted.', "warning");
    }
    else if (this.DeductHeader.Dedwt == 0) {
      var ans = confirm("Weight Deduction is Zero Grams, Do you want to Save?");
      if (ans) {
        this.DeductWtCalc();
      }
    }
    else {
      this.DeductWtCalc();
    }
  }

  GetBillQty() {
    this.BarcodeSummaryHeader.BillQty = parseInt(this.BarcodeSummaryHeader.ItemQty);
  }


  DeductStWtCalc(arg) { //Added Newly on 24-Jun-2020
    this.BarcodeSummaryHeader.DeductSWt = arg;
    const billGrWt = this.BarcodeSummaryHeader.Grosswt;
    const billNetWt = this.BarcodeSummaryHeader.Netwt;
    const qty = this.BarcodeSummaryHeader.ItemQty;
    const Stwt = this.BarcodeSummaryHeader.Stonewt;
    this.BarcodeSummaryHeader.DedCounter = this.DeductHeader.DDCounter;
    // this.BarcodeSummaryHeader.DedItem = this.DeductHeader.ItemName;
    this.BarcodeSummaryHeader.DedItem = this.BarcodeSummaryHeader.ItemName;
    this.BarcodeSummaryHeader.ItemQty = parseFloat((+qty - +this.DeductHeader.DedQty).toFixed(2));
    this.BarcodeSummaryHeader.DeductWt = this.DeductHeader.Dedwt;
    this.BarcodeSummaryHeader.Stonewt = parseFloat((+Stwt - +this.BarcodeSummaryHeader.DeductSWt).toFixed(3));
    this.BarcodeSummaryHeader.GrossWt = parseFloat((+billGrWt - +this.BarcodeSummaryHeader.DeductSWt).toFixed(3));
    //this.BillHeader.BillGrosswt = parseFloat((+billGrWt - +this.DeductHeader.Dedwt - +this.DeductHeader.DedSW).toFixed(3));
    this.BillHeader.BillGrosswt = parseFloat((+billGrWt + +this.BarcodeSummaryHeader.AddWt - +this.BarcodeSummaryHeader.DeductSWt).toFixed(3));
    //this.BillHeader.NW = parseFloat((+billGrWt - +this.DeductHeader.Dedwt - +this.DeductHeader.DedSW).toFixed(3));
    this.BillHeader.NW = parseFloat((+this.BarcodeSummaryHeader.GrossWt + +this.BarcodeSummaryHeader.AddWt - +this.BarcodeSummaryHeader.Stonewt - +this.BarcodeSummaryHeader.DeductWt).toFixed(3)); //added on 31-Mar-20
    this.calculateBarcodeDetails();
  }

  DeductWtCalc() {
    $('#DeductWeight').modal('hide');
    const billGrWt = this.BarcodeSummaryHeader.Grosswt;
    const billNetWt = this.BarcodeSummaryHeader.Netwt;
    const qty = this.BarcodeSummaryHeader.ItemQty;
    const Stwt = this.BarcodeSummaryHeader.Stonewt;
    this.BarcodeSummaryHeader.DedCounter = this.DeductHeader.DDCounter;
    // this.BarcodeSummaryHeader.DedItem = this.DeductHeader.ItemName;
    this.BarcodeSummaryHeader.DedItem = this.BarcodeSummaryHeader.ItemName;
    this.BarcodeSummaryHeader.ItemQty = parseFloat((+qty - +this.DeductHeader.DedQty).toFixed(2));
    this.BarcodeSummaryHeader.DeductWt = this.DeductHeader.Dedwt;
    this.BarcodeSummaryHeader.Stonewt = parseFloat((+Stwt - +this.DeductHeader.DedSW).toFixed(3));
    this.BarcodeSummaryHeader.GrossWt = parseFloat((+billGrWt - +this.DeductHeader.DedSW).toFixed(3));
    //this.BillHeader.BillGrosswt = parseFloat((+billGrWt - +this.DeductHeader.Dedwt - +this.DeductHeader.DedSW).toFixed(3));
    this.BillHeader.BillGrosswt = parseFloat((+billGrWt + +this.BarcodeSummaryHeader.AddWt - +this.BarcodeSummaryHeader.DeductWt).toFixed(3));
    //this.BillHeader.NW = parseFloat((+billGrWt - +this.DeductHeader.Dedwt - +this.DeductHeader.DedSW).toFixed(3));
    this.BillHeader.NW = parseFloat((+this.BarcodeSummaryHeader.GrossWt + +this.BarcodeSummaryHeader.AddWt - +this.BarcodeSummaryHeader.DeductWt).toFixed(3)); //added on 31-Mar-20

    this.calculateBarcodeDetails();
    this.DeductHeader = {
      Qty: null,
      CounterCode: null,
      DDCounter: null,
      GW: null,
      SW: null,
      NW: null,
      DedQty: null,
      Dedwt: null,
      DedSW: null,
      TagNo: null,
      ItemName: null,
      Iname: null,
      ItemQty: null
    }
  }

  GetVaporLossWeight(arg) {
    this.BarcodeSummaryHeader.VaporLossWeight = arg;
    this.calculateBarcodeDetails();
  }

  calculatedData: any;
  calculateBarcodeDetails() {
    //this.getInterState();
    //this.BarcodeSummaryHeader.isInterstate=this.interstate;
    this.barcodeService.calculateBarcode(this.BarcodeSummaryHeader).subscribe(
      response => {
        this.BarcodeSummaryHeader = null;
        this.calculatedData = response;
        this.BarcodeSummaryHeader = this.calculatedData;
        //this.BillHeader.NW = this.BarcodeSummaryHeader.Netwt; //commented on 30-Mar-20
        this.EnableVaporLoss = true;
        if (this.BarcodeSummaryHeader.VaporLossWeight != null) {
          this.BillHeader.BillGrosswt = parseFloat((this.BarcodeSummaryHeader.Grosswt + +this.BarcodeSummaryHeader.AddWt - +this.BarcodeSummaryHeader.DeductWt - +this.BarcodeSummaryHeader.VaporLossWeight).toFixed(3));
          this.BillHeader.NW = parseFloat((this.BarcodeSummaryHeader.Grosswt + +this.BarcodeSummaryHeader.AddWt - +this.BarcodeSummaryHeader.Stonewt - +this.BarcodeSummaryHeader.DeductWt - +this.BarcodeSummaryHeader.VaporLossWeight).toFixed(3)); //added on 31-Mar-20
        }
      },
      (err) => {
        if (err.status === 400) {
          const validationError = err.error;
          swal("Warning!", validationError.description, "warning");
          if (validationError.description == "MC Amount less than Min MC") {
            this.BarcodeSummaryHeader.McPercent = null;
            this.calculateBarcodeDetails();
            this.EnableVaporLoss = false;
            this.BarcodeSummaryHeader.VaporLossWeight = null;
          }
        }
      }
    )
  }

  closeModal() {
    $('#addWeight').modal('hide');
    $('#DeductWeight').modal('hide');
    this.BarcodePopupHeader = {
      ItQty: null,
      CounterCode: null,
      ItemName: null,
      ItemNo: null,
      Grwt: null,
      Stwt: null,
      Ntwt: null,
      AddWt: null,
      DeductWt: null,
      AdBarcode: null,
      AdCounter: null,
      AdItem: null,
      AdWt: null,
      AdQty: null,
      tagno: null,
    };
  }

  GetNetWT() {
    if (this.BarcodeSummaryHeader.SalCode == null || this.BarcodeSummaryHeader.SalCode == '') {
      swal("Warning!", 'Please select Staff', "warning");
      this.BarcodeSummaryHeader.Grosswt = 0;
      this.BarcodeSummaryHeader.Stonewt = 0;
    }
    else if (this.BarcodeSummaryHeader.GsCode == null || this.BarcodeSummaryHeader.GsCode == '') {
      swal("Warning!", 'Please select GS', "warning");
      this.BarcodeSummaryHeader.Grosswt = 0;
      this.BarcodeSummaryHeader.Stonewt = 0;
    }
    else if (this.BarcodeSummaryHeader.ItemName == null || this.BarcodeSummaryHeader.ItemName == '') {
      swal("Warning!", 'Please select Item', "warning");
      this.BarcodeSummaryHeader.Grosswt = 0;
      this.BarcodeSummaryHeader.Stonewt = 0;
    }
    else if (this.BarcodeSummaryHeader.Karat == null || this.BarcodeSummaryHeader.Karat == '') {
      swal("Warning!", 'Please select Karat', "warning");
      this.BarcodeSummaryHeader.Grosswt = 0;
      this.BarcodeSummaryHeader.Stonewt = 0;
    }
    else if (this.BarcodeSummaryHeader.Stonewt >= this.BarcodeSummaryHeader.Grosswt) {
      swal("Warning!", 'Please enter valid St.Wt(g)', "warning");
      this.BarcodeSummaryHeader.Stonewt = 0;
    }
    else {

      this.calculateBarcodeDetails(); //Added on 17-MAR-2020



      //#region Commented on 17-MAR-2020  

      const GrossWt = this.BarcodeSummaryHeader.Grosswt;
      const StneWt = this.BarcodeSummaryHeader.Stonewt;
      // this.BarcodeSummaryHeader.StoneCharges = this.totalAmtStn;
      this.BarcodeSummaryHeader.Netwt = parseFloat((<number>GrossWt - <number>StneWt).toFixed(3));
      this.BillHeader.BillGrosswt = parseFloat((+GrossWt + +this.BarcodePopupHeader.AdWt).toFixed(3));
      this.BillHeader.NW = this.BarcodeSummaryHeader.Netwt;
      // this.BarcodeSummaryHeader.GoldValue = parseFloat((<number>this.BarcodeSummaryHeader.Netwt * <number>this.BarcodeSummaryHeader.Rate).toFixed(3));

      // this.BarcodeSummaryHeader.ItemTotalAfterDiscount = +this.BarcodeSummaryHeader.GoldValue + +this.BarcodeSummaryHeader.VaAmount + +this.BarcodeSummaryHeader.StoneCharges + +this.BarcodeSummaryHeader.DiamondCharges;
      // this.BarcodeSummaryHeader.CGSTAmount = parseFloat((<number>this.BarcodeSummaryHeader.ItemTotalAfterDiscount * <number>this.CGSTPer / 100).toFixed(3));
      // this.BarcodeSummaryHeader.SGSTAmount = parseFloat((<number>this.BarcodeSummaryHeader.ItemTotalAfterDiscount * <number>this.SGSTPer / 100).toFixed(3));
      // // this.BarcodeSummaryHeader.IGSTAmount = Number(<number>this.BarcodeSummaryHeader.ItemTotalAfterDiscount * <number>this.IGSTPer / 100);
      // this.BarcodeSummaryHeader.ItemFinalAmount = parseFloat((<number>this.BarcodeSummaryHeader.ItemTotalAfterDiscount + <number>this.BarcodeSummaryHeader.SGSTAmount + <number>this.BarcodeSummaryHeader.CGSTAmount).toFixed(3));
      // this.BarcodeSummaryHeader.TotalAmount = parseFloat((<number>this.BarcodeSummaryHeader.ItemTotalAfterDiscount).toFixed(3));

      //#endregion
    }
  }

  wastage: any;
  rate: any;
  getMCAmt() {
    //if DropDown value 5 - MC% in DropDown(MC Details)
    if (this.BarcodeSummaryHeader.McType == 5) {
      this.McAmt = 0;
      this.McAmt = this.BarcodeSummaryHeader.GoldValue;
      this.BarcodeSummaryHeader.TotalSalesMc = <number>this.BarcodeSummaryHeader.McPercent;
      this.BarcodeSummaryHeader.VaAmount = parseFloat((<number>this.McAmt * <number>this.BarcodeSummaryHeader.McPercent / 100).toFixed(2));
      this.BarcodeSummaryHeader.ItemTotalAfterDiscount = parseFloat((<number>this.BarcodeSummaryHeader.GoldValue + <number>this.BarcodeSummaryHeader.VaAmount + <number>this.BarcodeSummaryHeader.StoneCharges + +this.BarcodeSummaryHeader.DiamondCharges).toFixed(2));
      this.BarcodeSummaryHeader.CGSTAmount = parseFloat((<number>this.BarcodeSummaryHeader.ItemTotalAfterDiscount * <number>this.CGSTPer / 100).toFixed(2));
      this.BarcodeSummaryHeader.SGSTAmount = parseFloat((<number>this.BarcodeSummaryHeader.ItemTotalAfterDiscount * <number>this.SGSTPer / 100).toFixed(2));
      this.BarcodeSummaryHeader.ItemFinalAmount = parseFloat((<number>this.BarcodeSummaryHeader.ItemTotalAfterDiscount + <number>this.BarcodeSummaryHeader.SGSTAmount + <number>this.BarcodeSummaryHeader.CGSTAmount).toFixed(2));
      this.BarcodeSummaryHeader.TotalAmount = parseFloat((<number>this.BarcodeSummaryHeader.ItemTotalAfterDiscount).toFixed(2));
    }
    // (if DropDown value 1 - MC PER GRAM )
    else if (this.BarcodeSummaryHeader.McType == 1) {
      this.BarcodeSummaryHeader.TotalSalesMc = <number>this.BarcodeSummaryHeader.MakingChargePerRs;
      this.BarcodeSummaryHeader.VaAmount = parseFloat((<number>this.BarcodeSummaryHeader.Netwt * <number>this.BarcodeSummaryHeader.MakingChargePerRs + (<number>this.BarcodeSummaryHeader.Netwt * <number>this.BarcodeSummaryHeader.WastPercent / 100 * <number>this.BarcodeSummaryHeader.Rate)).toFixed(2));
      this.BarcodeSummaryHeader.McDiscountAmt = <number>this.BarcodeSummaryHeader.Netwt * <number>this.BarcodeSummaryHeader.WastPercent / 100 * <number>this.BarcodeSummaryHeader.Rate;
      this.BarcodeSummaryHeader.ItemAdditionalDiscount = this.BarcodeSummaryHeader.McDiscountAmt;
      this.BarcodeSummaryHeader.ItemTotalAfterDiscount = Number(<number>this.BarcodeSummaryHeader.GoldValue + <number>this.BarcodeSummaryHeader.VaAmount + <number>this.BarcodeSummaryHeader.StoneCharges + +this.BarcodeSummaryHeader.DiamondCharges);
      this.BarcodeSummaryHeader.CGSTAmount = Number(<number>this.BarcodeSummaryHeader.ItemTotalAfterDiscount * <number>this.CGSTPer / 100);
      this.BarcodeSummaryHeader.SGSTAmount = Number(<number>this.BarcodeSummaryHeader.ItemTotalAfterDiscount * <number>this.SGSTPer / 100);
      this.BarcodeSummaryHeader.ItemFinalAmount = Number(<number>this.BarcodeSummaryHeader.ItemTotalAfterDiscount + <number>this.BarcodeSummaryHeader.SGSTAmount + <number>this.BarcodeSummaryHeader.CGSTAmount);
      this.BarcodeSummaryHeader.TotalAmount = parseFloat((<number>this.BarcodeSummaryHeader.ItemTotalAfterDiscount).toFixed(2));
    }
    // (if DropDown value 4 - MC Amount - Wastage)
    else if (this.BarcodeSummaryHeader.McType == 4) {
      var wastage = this.BarcodeSummaryHeader.WastageGrms;
      var rate = this.BarcodeSummaryHeader.Rate;
      this.BarcodeSummaryHeader.VaAmount = parseFloat((+this.BarcodeSummaryHeader.McAmount + (+wastage) * (+rate)).toFixed(2));
      this.BarcodeSummaryHeader.ItemTotalAfterDiscount = Number(<number>this.BarcodeSummaryHeader.GoldValue + <number>this.BarcodeSummaryHeader.VaAmount + <number>this.BarcodeSummaryHeader.StoneCharges + +this.BarcodeSummaryHeader.DiamondCharges);
      this.BarcodeSummaryHeader.CGSTAmount = Number(<number>this.BarcodeSummaryHeader.ItemTotalAfterDiscount * <number>this.CGSTPer / 100);
      this.BarcodeSummaryHeader.SGSTAmount = Number(<number>this.BarcodeSummaryHeader.ItemTotalAfterDiscount * <number>this.SGSTPer / 100);
      this.BarcodeSummaryHeader.ItemFinalAmount = Number(<number>this.BarcodeSummaryHeader.ItemTotalAfterDiscount + <number>this.BarcodeSummaryHeader.SGSTAmount + <number>this.BarcodeSummaryHeader.CGSTAmount);
      this.BarcodeSummaryHeader.TotalAmount = parseFloat((<number>this.BarcodeSummaryHeader.ItemTotalAfterDiscount).toFixed(2));
    }
    // (if DropDown value 6 - MC PER PIECE )
    else if (this.BarcodeSummaryHeader.McType == 6) {
      this.BarcodeSummaryHeader.TotalSalesMc = <number>this.BarcodeSummaryHeader.McPerPiece;
      this.BarcodeSummaryHeader.VaAmount = parseFloat((<number>this.BarcodeSummaryHeader.McPerPiece * <number>this.BarcodeSummaryHeader.ItemQty).toFixed(2));
      this.BarcodeSummaryHeader.ItemTotalAfterDiscount = Number(<number>this.BarcodeSummaryHeader.GoldValue + <number>this.BarcodeSummaryHeader.VaAmount + <number>this.BarcodeSummaryHeader.StoneCharges);
      this.BarcodeSummaryHeader.CGSTAmount = Number(<number>this.BarcodeSummaryHeader.ItemTotalAfterDiscount * <number>this.CGSTPer / 100);
      this.BarcodeSummaryHeader.SGSTAmount = Number(<number>this.BarcodeSummaryHeader.ItemTotalAfterDiscount * <number>this.SGSTPer / 100);
      this.BarcodeSummaryHeader.ItemFinalAmount = Number(<number>this.BarcodeSummaryHeader.ItemTotalAfterDiscount + <number>this.BarcodeSummaryHeader.SGSTAmount + <number>this.BarcodeSummaryHeader.CGSTAmount);
      this.BarcodeSummaryHeader.TotalAmount = parseFloat((<number>this.BarcodeSummaryHeader.ItemTotalAfterDiscount).toFixed(2));
    }
  }

  //Stone Calculation
  getStoneWt(index) {
    this.getAmount(index);
    this.fieldArray1[index].StoneWt = parseFloat((this.fieldArray1[index].Carrat * 0.2).toFixed(3));
  }

  getAmount(index) {
    if (this.fieldArray1[index].Carrat != 0 && this.fieldArray1[index].Carrat != null) {
      this.fieldArray1[index].Amount = this.fieldArray1[index].Carrat * this.fieldArray1[index].Rate;
    }
    else {
      this.fieldArray1[index].Amount = this.fieldArray1[index].Qty * this.fieldArray1[index].Rate;
    }
  }


  //Stone Details`
  fieldArray1: any = [];
  count: number = 0;
  public stoneDtls: stoneModel;


  EnableEditDelbtn = {};
  EnableSaveCnlbtn = {};
  readonly = {};
  readonly1 = {};
  readonlyRate = {};
  EnableDropdown: boolean = false;
  EnableAddRow: boolean = false;
  EnableSubmitButton: boolean = true;
  //Variable declared to take previous edited value
  CopyEditedRow: any = [];

  addrow() {
    this.stoneDtls = {
      ObjID: null,
      CompanyCode: this.ccode,
      BranchCode: this.bcode,
      BillNo: null,
      SlNo: null,
      EstNo: null,
      EstSrNo: null,
      BarcodeNo: this.BarCodeNo,
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
    };

    this.fieldArray1.push(this.stoneDtls);
    for (let { } of this.fieldArray1) {
      this.count++;
    }
    this.EnableSaveCnlbtn[this.count - 1] = true;
    this.EnableEditDelbtn[this.count - 1] = false;
    this.readonly[this.count - 1] = false
    this.readonly1[this.count - 1] = false
    this.readonlyRate[this.count - 1] = false;
    this.count = 0;
    this.EnableAddRow = true;
    this.EnableSubmitButton = true;
    this.EnableDropdown = true;
  }

  //Save Row
  totalAmtDmd: number = 0;
  totalAmtStn: number = 0;
  // saveDataFieldValue(index) {
  //   if (this.fieldArray1[index]["Type"] == null || this.fieldArray1[index]["Type"] == 0) {
  //     this.toastr.warning('Please Select Stone/Diamond', 'Alert!');
  //   }
  //   else if (this.fieldArray1[index]["Name"] == null || this.fieldArray1[index]["Name"] == 0) {
  //     this.toastr.warning('Please Select ItemName', 'Alert!');
  //   }
  //   else if (this.fieldArray1[index]["Qty"] == null || this.fieldArray1[index]["Qty"] == 0) {
  //     this.toastr.warning('Please Enter Quantity', 'Alert!');
  //   }
  //   else if (this.fieldArray1[index]["Carrat"] == null || this.fieldArray1[index]["Carrat"] == 0) {
  //     this.toastr.warning('Please Enter Carat', 'Alert!');
  //   }
  //   else if (this.fieldArray1[index]["StoneWt"] == null || this.fieldArray1[index]["StoneWt"] == 0) {
  //     this.toastr.warning('Please Enter Weight', 'Alert!');
  //   }
  //   else if (this.fieldArray1[index]["Rate"] == null || this.fieldArray1[index]["Rate"] == 0) {
  //     this.toastr.warning('Please Enter Rate/Carat', 'Alert!');
  //   }
  //   else {
  //     this.BarcodeSummaryHeader.salesEstStoneVM[index] = this.fieldArray1[index];
  //     this.EnableEditDelbtn[index] = true;
  //     this.EnableSaveCnlbtn[index] = false;
  //     this.readonly[index] = true;
  //     this.EnableAddRow = false;
  //     this.EnableSubmitButton = false;
  //     if (this.fieldArray1[index].Type == "DMD") {
  //       // this.totalAmtDmd += this.fieldArray1[index]["Amount"];
  //       // this.BarcodeSummaryHeader.DiamondCharges = this.totalAmtDmd;
  //       // this.GetNetWT();
  //       // return this.totalAmtDmd;
  //       this.calculateBarcodeDetails();
  //     }
  //     else if (this.fieldArray1[index].Type == "STN") {
  //       if (this.model.name == "Tag") {
  //         this.BarcodeSummaryHeader.StoneCharges = this.totalAmtStn;
  //         this.BarcodeSummaryHeader.Stonewt =parseFloat((+this.BarcodeSummaryHeader.Stonewt + +this.fieldArray1[index].StoneWt).toFixed());
  //         this.BarcodeSummaryHeader.Grosswt = parseFloat((<number>this.BarcodeSummaryHeader.Grosswt + <number>+this.fieldArray1[index].StoneWt).toFixed(3));
  //         this.BillHeader.BillGrosswt = parseFloat((+this.BarcodeSummaryHeader.Grosswt + +this.BarcodePopupHeader.AdWt).toFixed(3));
  //         this.BillHeader.NW = parseFloat((+this.BarcodeSummaryHeader.Netwt + +this.BarcodePopupHeader.AdWt).toFixed(3));
  //         this.calculateBarcodeDetails();
  //       }
  //       else {
  //         this.BarcodeSummaryHeader.Stonewt = +this.BarcodeSummaryHeader.Stonewt + +this.fieldArray1[index].StoneWt;
  //         this.BillHeader.BillGrosswt = parseFloat((+this.BarcodeSummaryHeader.Grosswt + +this.BarcodePopupHeader.AdWt).toFixed(3));
  //         this.BillHeader.NW = parseFloat((+this.BarcodeSummaryHeader.Netwt + +this.BarcodePopupHeader.AdWt).toFixed(3));
  //         this.calculateBarcodeDetails();
  //       }
  //     }
  //   }
  // }


  StneWt: number = 0.00;
  GrWt: number = 0.00;
  saveDataFieldValue(index) {
    if (this.BarcodeSummaryHeader.Grosswt == null || this.BarcodeSummaryHeader.Grosswt == 0) {
      swal("Warning!", 'Please Enter item weight', "warning");
    }
    else if (this.fieldArray1[index]["Type"] == null || this.fieldArray1[index]["Type"] == 0) {
      swal("Warning!", 'Please Select Stone/Diamond', "warning");
    }
    else if (this.fieldArray1[index]["Type"] == null || this.fieldArray1[index]["Type"] == 0) {
      swal("Warning!", 'Please Select Stone/Diamond', "warning");
    }
    else if (this.fieldArray1[index]["Name"] == null || this.fieldArray1[index]["Name"] == 0) {
      swal("Warning!", 'Please Select ItemName', "warning");
    }
    else if (this.fieldArray1[index]["Qty"] == null || this.fieldArray1[index]["Qty"] == 0) {
      swal("Warning!", 'Please Enter Quantity', "warning");
    }
    // else if (this.fieldArray1[index]["Carrat"] == null || this.fieldArray1[index]["Carrat"] == 0) {
    //   this.toastr.warning('Please Enter Carat', 'Alert!');
    // }
    // else if (this.fieldArray1[index]["StoneWt"] == null || this.fieldArray1[index]["StoneWt"] == 0) {
    //   this.toastr.warning('Please Enter Weight', 'Alert!');
    // }
    else if (this.fieldArray1[index]["Rate"] == null || this.fieldArray1[index]["Rate"] == 0) {
      swal("Warning!", 'Please Enter Rate/Carat', "warning");
    }
    else {

      this.BarcodeSummaryHeader.salesEstStoneVM[index] = this.fieldArray1[index];
      this.EnableEditDelbtn[index] = true;
      this.EnableSaveCnlbtn[index] = false;
      this.readonly[index] = true;
      this.readonly1[index] = true;
      this.readonlyRate[index] = true;
      this.EnableAddRow = false;
      this.EnableSubmitButton = false;


      if (this.fieldArray1[index].Type == "DMD") {
        this.totalAmtDmd += this.fieldArray1[index]["Amount"];
        this.BarcodeSummaryHeader.DiamondCharges = this.totalAmtDmd;
        this.calculateBarcodeDetails();
        // this.GetNetWT();
        //  return this.totalAmtDmd;
      }
      else if (this.fieldArray1[index].Type == "STN") {
        this.totalAmtStn += this.fieldArray1[index]["Amount"];
        this.BarcodeSummaryHeader.StoneCharges = this.totalAmtStn;
        if (this.model.name == "Tag") {
          //this.BarcodeSummaryHeader.StoneCharges = this.totalAmtStn;
          //         this.BarcodeSummaryHeader.Stonewt =parseFloat((+this.BarcodeSummaryHeader.Stonewt + +this.fieldArray1[index].StoneWt).toFixed());
          //         this.BarcodeSummaryHeader.Grosswt = parseFloat((<number>this.BarcodeSummaryHeader.Grosswt + <number>+this.fieldArray1[index].StoneWt).toFixed(3));
          //         this.BillHeader.BillGrosswt = parseFloat((+this.BarcodeSummaryHeader.Grosswt + +this.BarcodePopupHeader.AdWt).toFixed(3));
          //         this.BillHeader.NW = parseFloat((+this.BarcodeSummaryHeader.Netwt + +this.BarcodePopupHeader.AdWt).toFixed(3));
          //         this.calculateBarcodeDetails();
          //    this.BarcodeSummaryHeader.StoneCharges = this.totalAmtStn;
          if (this.CopyEditedRow[index] != null) {
            this.calculateBarcodeDetails();
          }
          else {
            this.BarcodeSummaryHeader.Stonewt = +this.BarcodeSummaryHeader.Stonewt + +this.fieldArray1[index].StoneWt;
            this.BarcodeSummaryHeader.Grosswt = Number(<number>this.BarcodeSummaryHeader.Grosswt + <number>+this.fieldArray1[index].StoneWt).toFixed(3);
            // this.BarcodeSummaryHeader.Netwt = Number(<number>this.BarcodeSummaryHeader.Grosswt - <number>this.BarcodeSummaryHeader.Stonewt);
            this.BillHeader.BillGrosswt = Number(+this.BarcodeSummaryHeader.Grosswt + +this.BarcodePopupHeader.AdWt).toFixed(3);
            this.BillHeader.NW = Number(+this.BarcodeSummaryHeader.Netwt + +this.BarcodePopupHeader.AdWt).toFixed(3);
            //      this.GetNetWT();
            //      this.getMCAmt();
            this.calculateBarcodeDetails();
          }
        }
        else {
          if (this.CopyEditedRow[index] != null) {
            this.calculateBarcodeDetails();
          }
          else {
            this.StneWt = +this.BarcodeSummaryHeader.Stonewt + +this.fieldArray1[index].StoneWt;
            this.GrWt = this.BarcodeSummaryHeader.Grosswt;
            if (this.StneWt > this.BarcodeSummaryHeader.Grosswt) {
              swal("Warning!", 'Please enter valid St.Wt', "warning");
              this.EnableSaveCnlbtn[index] = true;
              this.EnableEditDelbtn[index] = false;
              this.readonly[index] = false
              this.readonly1[index] = false
              this.readonlyRate[index] = false;
              this.EnableAddRow = true;
              this.EnableDropdown = true;
            }
            else {
              this.BarcodeSummaryHeader.Stonewt = parseFloat((+this.BarcodeSummaryHeader.Stonewt + +this.fieldArray1[index].StoneWt).toFixed(3));
              this.BarcodeSummaryHeader.Netwt = Number(<number>this.BarcodeSummaryHeader.Grosswt - <number>this.BarcodeSummaryHeader.Stonewt);
              this.BillHeader.BillGrosswt = Number(+this.BarcodeSummaryHeader.Grosswt + +this.BarcodePopupHeader.AdWt);
              this.BillHeader.NW = Number(+this.BarcodeSummaryHeader.Netwt + +this.BarcodePopupHeader.AdWt);
              //      this.GetNetWT();
              //       this.getMCAmt();
              this.calculateBarcodeDetails();
            }
          }
        }
        //  return this.totalAmtStn;  
      }
      this.CopyEditedRow = [];
    }
  }

  cancelDataFieldValue(index) {
    this.EnableAddRow = false;
    if (this.CopyEditedRow[index] != null) {
      this.fieldArray1[index] = this.CopyEditedRow[index];
      if (this.fieldArray1[index].Type == "STN") {
        this.totalAmtStn = this.totalAmtStn + this.fieldArray1[index]["Amount"];
        this.BarcodeSummaryHeader.StoneCharges = this.totalAmtStn;
      }
      if (this.fieldArray1[index].Type == "DMD") {
        this.totalAmtDmd = this.totalAmtDmd + this.fieldArray1[index]["Amount"];
        this.BarcodeSummaryHeader.DiamondCharges = this.totalAmtDmd;
      }
      this.CopyEditedRow[index] = null;
      this.EnableSaveCnlbtn[index] = false;
      this.EnableEditDelbtn[index] = true;
      this.readonly[index] = true;
      this.readonly1[index] = true;
      this.readonlyRate[index] = true;
      this.CopyEditedRow = [];
    }
    else {
      this.fieldArray1.splice(index, 1);
    }
    this.EnableDisableSubmit();
  }

  EnableDisableSubmit() {
    if (this.fieldArray1.length <= 0) {
      this.EnableSubmitButton = true;
      this.EnableDropdown = false;
    }
    else {
      this.EnableSubmitButton = false;
      this.EnableDropdown = true;
    }
  }

  editFieldValue(index) {
    if (this.EnableAddRow == true) {
      swal("Warning!", 'Please save the enabled item', "warning");
    }
    else {
      this.CopyEditedRow[index] = Object.assign({}, this.fieldArray1[index]);
      // if (this.fieldArray1[index].Type == "STN") {
      //   this.totalAmtStn = this.totalAmtStn - this.fieldArray1[index]["Amount"];
      //   this.BarcodeSummaryHeader.StoneCharges = this.totalAmtStn;
      // }
      // if (this.fieldArray1[index].Type == "DMD") {
      //   this.totalAmtDmd = this.totalAmtDmd - this.fieldArray1[index]["Amount"];
      //   this.BarcodeSummaryHeader.DiamondCharges = this.totalAmtDmd;
      // }
      this.calculateBarcodeDetails();
      this.EnableEditDelbtn[index] = false;
      this.EnableSaveCnlbtn[index] = true;
      //this.readonly[index] = false;
      this.readonly[index] = true;
      this.readonly1[index] = true;
      this.readonlyRate[index] = false;
      this.EnableAddRow = true;
      this.EnableSubmitButton = true;
    }
  }

  deleteFieldValue(index) {
    if (this.EnableAddRow == true) {
      swal("Warning!", 'Please save the enabled item', "warning");
    }
    else {
      // if (this.fieldArray1[index].Type == "STN") {
      //   this.totalAmtStn = this.totalAmtStn - this.fieldArray1[index]["Amount"];
      //   this.BarcodeSummaryHeader.StoneCharges = this.totalAmtStn;
      // }
      // if (this.fieldArray1[index].Type == "DMD") {
      //   this.totalAmtDmd = this.totalAmtDmd - this.fieldArray1[index]["Amount"];
      //   this.BarcodeSummaryHeader.DiamondCharges = this.totalAmtDmd;
      // }
      this.BarcodeSummaryHeader.salesEstStoneVM.splice(index, 1);
      if (this.fieldArray1[index].Type == "STN") {
        const stwt = this.fieldArray1[index].StoneWt;
        this.BarcodeSummaryHeader.Stonewt = Number(<number>this.BarcodeSummaryHeader.Stonewt - <number>this.fieldArray1[index].StoneWt).toFixed(3);
        this.BarcodeSummaryHeader.Netwt = Number(<number>this.BarcodeSummaryHeader.Grosswt - <number>this.fieldArray1[index].StoneWt).toFixed(3);
        this.BillHeader.NW = parseFloat((+stwt + +this.BarcodeSummaryHeader.Netwt).toFixed(3)); //added on 31-Mar-20

      }
      this.fieldArray1.splice(index, 1);
      this.calculateBarcodeDetails();
      this.EnableDisableSubmit();
    }
  }


  stoneType: any = [];
  getStoneType() {
    this.barcodeService.getStoneType().subscribe(
      response => {
        this.stoneType = response;
      }
    )
  }

  stoneName: any = [];
  getStoneName(arg, index) {
    this.barcodeService.getStoneName(arg).subscribe(
      response => {
        this.stoneName[index] = response;
      }
    )
  }

  SubmitBarcodeDetails() {
    if (this.model.name == "Tag") {
      if (this.BarcodeSummaryHeader.BarcodeNo == null || this.BarcodeSummaryHeader.BarcodeNo == "") {
        swal("Warning!", 'Please enter the Tag No', "warning");
      }
      else if (this.BarcodeSummaryHeader.SalCode == null || this.BarcodeSummaryHeader.SalCode == "") {
        swal("Warning!", 'Please select the Staff', "warning");
      }
      else if ((this.BarcodeSummaryHeader.VaAmount == null || this.BarcodeSummaryHeader.VaAmount == 0) 
      && (this.BarcodeSummaryHeader.PieceRate == 'N'  )) {
        swal("Warning!", 'Please enter the Mc Amt', "warning");
      }
      //below validation check weather gscode is SGO and mctype  is 0
      // else if (this.BarcodeSummaryHeader.McType == '0'  && this.BarcodeSummaryHeader.GsCode =="SGO"){
      //   this.SaveBarcodeDetails();
      // }
      else {
        this.SaveBarcodeDetails();
      }
    }
    else if (this.model.name == "Non Tag") {
      if (this.BarcodeSummaryHeader.GsCode == null || this.BarcodeSummaryHeader.GsCode == "") {
        swal("Warning!", 'Please select the GS', "warning");
      }
      else if (this.BarcodeSummaryHeader.ItemName == null || this.BarcodeSummaryHeader.ItemName == "") {
        swal("Warning!", 'Please select the Item', "warning");
      }
      else if (this.BarcodeSummaryHeader.CounterCode == null || this.BarcodeSummaryHeader.CounterCode == "") {
        swal("Warning!", 'Please select the Counter', "warning");
      }
      else if (this.BarcodeSummaryHeader.PieceRate == null || this.BarcodeSummaryHeader.PieceRate == "") {
        swal("Warning!", 'Please select the Piece Item', "warning");
      }
      else if (this.BarcodeSummaryHeader.Karat == null || this.BarcodeSummaryHeader.Karat == "") {
        swal("Warning!", 'Please select the Karat', "warning");
      }
      else if (this.BarcodeSummaryHeader.PieceRate != "Y") {
        if (this.BarcodeSummaryHeader.ItemQty == null) {
          swal("Warning!", 'Please enter the Item Qty', "warning");
        }
        else if (this.BarcodeSummaryHeader.Grosswt == null || this.BarcodeSummaryHeader.Grosswt == 0) {
          swal("Warning!", 'Please enter the valid Gr.Wt(g)', "warning");
        }
        else if (this.BarcodeSummaryHeader.McType == 5) {
          if ((this.BarcodeSummaryHeader.VaAmount == null || this.BarcodeSummaryHeader.VaAmount == 0) && (this.BarcodeSummaryHeader.McPercent == null || this.BarcodeSummaryHeader.McPercent == 0)) {
            swal("Warning!", 'Please enter the Mc Percent', "warning");
          }
          else if ((this.BarcodeSummaryHeader.VaAmount != null || this.BarcodeSummaryHeader.VaAmount == 0) && (this.BarcodeSummaryHeader.McPercent == null || this.BarcodeSummaryHeader.McPercent == 0)) {
            swal("Warning!", 'Please enter the Mc Percent', "warning");
          }
          else {
            this.SaveBarcodeDetails();
          }
        }
        else if (this.BarcodeSummaryHeader.McType == 1) {
          if ((this.BarcodeSummaryHeader.VaAmount == null || this.BarcodeSummaryHeader.VaAmount == 0) && (this.BarcodeSummaryHeader.MakingChargePerRs == null || this.BarcodeSummaryHeader.MakingChargePerRs == 0)) {
            swal("Warning!", 'Please enter the MC/Gram', "warning");
          }
          else if ((this.BarcodeSummaryHeader.VaAmount == null || this.BarcodeSummaryHeader.VaAmount == 0) && (this.BarcodeSummaryHeader.WastPercent == null || this.BarcodeSummaryHeader.WastPercent == 0)) {
            swal("Warning!", 'Please enter the MakingChargePerRs', "warning");
          }
          else if ((this.BarcodeSummaryHeader.VaAmount != null || this.BarcodeSummaryHeader.VaAmount == 0) && (this.BarcodeSummaryHeader.MakingChargePerRs == null || this.BarcodeSummaryHeader.MakingChargePerRs == 0) && (this.BarcodeSummaryHeader.WastPercent == null || this.BarcodeSummaryHeader.WastPercent == 0)) {
            swal("Warning!", 'Please enter the MC/Gram', "warning");
          }
          else {
            this.SaveBarcodeDetails();
          }
        }
        else if (this.BarcodeSummaryHeader.McType == 4) {
          if ((this.BarcodeSummaryHeader.VaAmount == null || this.BarcodeSummaryHeader.VaAmount == 0) && (this.BarcodeSummaryHeader.McAmount == null || this.BarcodeSummaryHeader.McAmount == 0)) {
            swal("Warning!", 'Please enter the McAmount', "warning");
          }
          else if ((this.BarcodeSummaryHeader.VaAmount == null || this.BarcodeSummaryHeader.VaAmount == 0) && (this.BarcodeSummaryHeader.WastageGrms == null || this.BarcodeSummaryHeader.WastageGrms == 0)) {
            swal("Warning!", 'Please enter the WastageGrms', "warning");
          }
          else if ((this.BarcodeSummaryHeader.VaAmount != null || this.BarcodeSummaryHeader.VaAmount == 0) && (this.BarcodeSummaryHeader.McAmount == null || this.BarcodeSummaryHeader.McAmount == 0) && (this.BarcodeSummaryHeader.WastageGrms == null || this.BarcodeSummaryHeader.WastageGrms == 0)) {
            swal("Warning!", 'Please enter the McAmount', "warning");
          }
          else {
            this.SaveBarcodeDetails();
          }
        }
        else if (this.BarcodeSummaryHeader.McType == 6) {
          if ((this.BarcodeSummaryHeader.VaAmount == null || this.BarcodeSummaryHeader.VaAmount == 0) && (this.BarcodeSummaryHeader.McPerPiece == null || this.BarcodeSummaryHeader.McPerPiece == 0)) {
            swal("Warning!", 'Please enter the MC/Piece', "warning");
          }
          else if ((this.BarcodeSummaryHeader.VaAmount != null || this.BarcodeSummaryHeader.VaAmount == 0) && (this.BarcodeSummaryHeader.McPerPiece == null || this.BarcodeSummaryHeader.McPerPiece == 0)) {
            swal("Warning!", 'Please enter the MC/Piece', "warning");
          }
          else {
            this.SaveBarcodeDetails();
          }
        }
        else {
          this.SaveBarcodeDetails();
        }
      }
      else {
        this.SaveBarcodeDetails();
      }
    }
  }

  SaveBarcodeDetails() {
    if (this.BarcodeSummaryHeader.ItemFinalAmount == 0 || this.BarcodeSummaryHeader.ItemFinalAmount == null) {
      swal("Warning!", 'Item Amount is Zero. It cannot be billed', "warning");
    }
    else {
      this.EnableStnDets = true;
      this.barcodeService.SendBarcodeDataToSalesComp(null);
      this.barcodeService.SendBarcodeDataToSalesComp(this.BarcodeSummaryHeader);
      //this.storeExistingBarcodeDets = this.BarcodeSummaryHeader;
      this.HideRadioBtn = true;
      this.fieldArray1 = [];
      this.CommonJson();
    }
  }

  EnableRate: boolean = true;
  EnableVaporLoss: boolean = true;

  EditRate() {
    this.PwdRate.nativeElement.value = "";
    $('#PermissonModalRateGram').modal('show');
    this.EnableRate = true;
  }

  EditVaporLoss() {
    this.VaporLossEdit.nativeElement.value = "";
    $('#PermissonModalVaporLess').modal('show');
    this.EnableVaporLoss = true;
  }

  close() {
    $('#PermissonModalRateGram').modal('hide');
  }

  closeVaporLoss() {
    $('#PermissonModalVaporLess').modal('hide');
  }

  closeMC() {
    $('#PermissonMCModal').modal('hide');
  }

  permissonModel: any = {
    CompanyCode: null,
    BranchCode: null,
    PermissionID: null,
    PermissionData: null
  }

  passWordRateGram(arg) {
    if (arg == "") {
      swal("Warning!", 'Please Enter the Password', "warning");
      $('#PermissonModalRateGram').modal('show');
      this.EnableRate = true;
    }
    else {
      this.permissonModel.CompanyCode = this.ccode;
      this.permissonModel.BranchCode = this.bcode;
      this.permissonModel.PermissionID = this.SalesEstRateEditCode;
      this.permissonModel.PermissionData = btoa(arg);
      this._masterService.postelevatedpermission(this.permissonModel).subscribe(
        response => {
          this.EnableRate = false;
          $('#PermissonModalRateGram').modal('hide');
        },
        (err) => {
          if (err.status === 401) {
            $('#PermissonModalRateGram').modal('show');
            this.EnableRate = true;
          }
        }
      )
    }
  }


  passWordVaporLess(arg) {
    if (arg == "") {
      swal("Warning!", 'Please Enter the Password', "warning");
      $('#PermissonModalVaporLess').modal('show');
      this.EnableVaporLoss = true;
    }
    else {
      this.permissonModel.CompanyCode = this.ccode;
      this.permissonModel.BranchCode = this.bcode;
      this.permissonModel.PermissionID = this.VaporLossEditCode;
      this.permissonModel.PermissionData = btoa(arg);
      this._masterService.postelevatedpermission(this.permissonModel).subscribe(
        response => {
          this.EnableVaporLoss = false;
          $('#PermissonModalVaporLess').modal('hide');
        },
        (err) => {
          if (err.status === 401) {
            $('#PermissonModalVaporLess').modal('show');
            this.EnableVaporLoss = true;
          }
        }
      )
    }
  }


  editRateGram() {
    this.EnableRate = true;
    this.calculateBarcodeDetails();
  }

  PrintDetailsTextDetails: any = [];

  PrintItemDetails() {
    this.barcodeService.BarcodePrint(this.BarcodeSummaryHeader).subscribe(
      response => {
        this.PrintDetailsTextDetails = response;
        $('#PrintDetailsTextTab').modal('show');
      },
      (err) => {
        $('#PrintDetailsTextTab').modal('hide');
      }
    )
  }

  printDetailsPlainText() {
    this._masterService.printPlainText(this.PrintDetailsTextDetails);
  }

  ClosePrint() {
    $('#PrintDetailsTextTab').modal('hide');
  }


  EnableMCRate: boolean = true;
  EnableStnDets: boolean = true;

  EditMC() {
    if (this.BarcodeSummaryHeader.PieceRate == 'Y') {
      swal("Warning!", 'This is a piece item', "warning");
      $('#PermissonMCModal').modal('hide');
    }
    else {
      this.PwdMC.nativeElement.value = "";
      $('#PermissonMCModal').modal('show');
    }
  }

  passWordMC(arg) {
    if (arg == "") {
      swal("Warning!", 'Please Enter the Password', "warning");
      $('#PermissonMCModal').modal('show');
      this.EnableMCRate = true;
    }
    else {
      this.permissonModel.CompanyCode = this.ccode;
      this.permissonModel.BranchCode = this.bcode;
      this.permissonModel.PermissionID = this.MCRateEdit;
      this.permissonModel.PermissionData = btoa(arg);
      this._masterService.postelevatedpermission(this.permissonModel).subscribe(
        response => {
          this.EnableMCRate = false;
          $('#PermissonMCModal').modal('hide');
        },
        (err) => {
          if (err.status === 401) {
            $('#PermissonMCModal').modal('show');
            this.EnableMCRate = true;
          }
        }
      )
    }
  }

  editStoneDets() {
    this.PwdStnDet.nativeElement.value = "";
    $('#PermissonStnDetsModal').modal('show');
    this.EnableStnDets = true;
  }

  closeStnDets() {
    $('#PermissonStnDetsModal').modal('hide');
  }

  passWordStnDets(arg) {
    if (arg == "") {
      swal("Warning!", 'Please Enter the Password', "warning");
      $('#PermissonStnDetsModal').modal('show');
      this.EnableStnDets = true;
    }
    else {
      this.permissonModel.CompanyCode = this.ccode;
      this.permissonModel.BranchCode = this.bcode;
      this.permissonModel.PermissionID = this.StoneDetailsEdit;
      this.permissonModel.PermissionData = btoa(arg);
      this._masterService.postelevatedpermission(this.permissonModel).subscribe(
        response => {
          this.EnableStnDets = false;
          $('#PermissonStnDetsModal').modal('hide');
        },
        (err) => {
          if (err.status === 401) {
            $('#PermissonStnDetsModal').modal('show');
            this.EnableStnDets = true;
          }
        }
      )
    }
  }

  CommonJson() {
    this.BarcodeSummaryHeader = {
      ObjID: null,
      ItemQty: null,
      CompanyCode: this.ccode,
      BranchCode: this.bcode,
      EstNo: null,
      SlNo: null,
      BillNo: null,
      BarcodeNo: null,
      SalCode: null,
      CounterCode: null,
      ItemName: null,
      ItemNo: null,
      Grosswt: null,
      Stonewt: null,
      Netwt: null,
      AddWt: null,
      DeductWt: null,
      MakingChargePerRs: null,
      WastPercent: null,
      GoldValue: null,
      VaAmount: null,
      StoneCharges: null,
      DiamondCharges: null,
      TotalAmount: null,
      Hallmarkarges: null,
      McAmount: null,
      WastageGrms: null,
      McPercent: null,
      AddQty: null,
      DeductQty: null,
      OfferValue: null,
      UpdateOn: null,
      GsCode: null,
      Rate: null,
      Karat: null,
      AdBarcode: null,
      AdCounter: null,
      AdItem: null,
      IsEDApplicable: null,
      McType: null,
      Fin_Year: null,
      NewBillNo: null,
      ItemTotalAfterDiscount: null,
      ItemAdditionalDiscount: null,
      TaxPercentage: null,
      TaxAmount: null,
      ItemFinalAmount: null,
      SupplierCode: null,
      ItemSize: null,
      ImgID: null,
      DesignCode: null,
      DesignName: null,
      BatchID: null,
      Rf_ID: null,
      McPerPiece: null,
      DiscountMc: null,
      TotalSalesMc: null,
      McDiscountAmt: null,
      purchaseMc: null,
      GSTGroupCode: null,
      SGSTPercent: null,
      SGSTAmount: null,
      CGSTPercent: null,
      CGSTAmount: null,
      IGSTPercent: null,
      VaporLossWeight: null,
      VaporLossAmount: null,
      IGSTAmount: null,
      HSN: null,
      PieceRate: null,
      DeductSWt: null,
      OrdDiscountAmt: null,
      DedCounter: null,
      DedItem: null,
      isInterstate: 0,
      salesEstStoneVM: []
    }
  }

  BarcodeAge: any = [];
  getAgeofTheBarcode(arg) {
    this.BarcodeAge = [];
    if (arg != "") {
      this.barcodeService.getBarcodeAge(arg).subscribe(
        response => {
          this.BarcodeAge = response;
          this.EnableBarcodeAge = true;
        },
        (err) => {
          this.EnableBarcodeAge = false;
        }
      )
    }
  }

  getStatusColor(ObjStatus) {
    switch (ObjStatus) {
      case 'O':
        return 'green';
      case 'C':
        return 'red';

    }
  }
}