import { ModelError } from './../sales/model/modelerror.model';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AfterContentChecked, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { SalesSummaryModel } from '../sales/sales.model';
import { SalesService } from '../sales/sales.service';
import { BarcodedetailsService } from '../barcodedetails/barcodedetails.service'
import { OrdersService } from './../orders/orders.service';
import { MasterService } from './../core/common/master.service';
import { AddBarcode, stoneModel } from './../sales/add-barcode/add-barcode.model';
import { AddBarcodeService } from './../sales/add-barcode/add-barcode.service';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../AppConfigService';
import { estimationService } from '../estimation/estimation.service';
import { ToastrService } from 'ngx-toastr';
import swal from 'sweetalert';
declare var $: any;

@Component({
  selector: 'app-barcodedetails',
  templateUrl: './barcodedetails.component.html',
  styleUrls: ['./barcodedetails.component.css']
})
export class BarcodedetailsComponent implements OnInit {

  SalesForm: FormGroup;
  SalesGSForm: FormGroup;
  submitted = false;
  Customer: any;
  GetBarcodeList: any = [];
  ArrayList: any = [];
  KaratForm: FormGroup;
  errors: string[];
  hasErrors = false;
  id: number = 0;
  readonly = true;

  SalesSummaryData: SalesSummaryModel = {
    Grwt: null,
    NtWt: null,
    SGst: null,
    CGst: null,
    IGst: null,
    TotalItems: null,
    Amount: null,
    taxable_Amt: null,
  };


  @Input() BarcodeFromSales: boolean = false;
  @ViewChild("PwdRate", { static: true }) PwdRate: ElementRef;
  @ViewChild("PwdMC", { static: true }) PwdMC: ElementRef;
  @ViewChild("PwdStnDet", { static: true }) PwdStnDet: ElementRef;
  @ViewChild("tagno", { static: true }) tagno: ElementRef;
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

  constructor(private formBuilder: FormBuilder, private _salesService: SalesService,
    private _barcodedetailsService: BarcodedetailsService, private _orderservice: OrdersService,
    private _masterService: MasterService, private _AddBarcodeService: AddBarcodeService,
    private toastr: ToastrService, private salesService: SalesService
    , private fb: FormBuilder,
    private _estimationService: estimationService,
    private appConfigService: AppConfigService) {
    this.password = this.appConfigService.Pwd
    this.EnableJson = this.appConfigService.EnableJson;
    this.SalesEstRateEditCode = appConfigService.RateEditCode.SalesEstRateEdit;
    this.MCRateEdit = appConfigService.RateEditCode.MCRateEdit;
    this.StoneDetailsEdit = appConfigService.RateEditCode.StoneDetailsEdit;
    this.getCB();
    this.radioItems = [{ 'name': 'Tag', 'Tooltip': 'Existing Barcode' },
    { 'name': 'Non Tag', 'Tooltip': 'Non Existing' }
    ];
    this.radioButtons = [{ 'name': 'Tag', 'Tooltip': 'Existing Barcode' },
    { 'name': 'Non Tag', 'Tooltip': 'Non Existing' }
    ];

  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
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

  // For ServerSide Validations
  modelErrors: ModelError[];
  public findErrorByField(modelField: string, index?: number): string {
    const modelErr = this.modelErrors.find(m => (m.field === modelField && m.index === index));
    if (modelErr != null) {
      return modelErr.description;
    } else {
      return '';
    }
  }

  SalesEstNo: string = null;
  arrayObj: any = [];
  Unsubscribe_obj: Subscription;
  ngOnInit() {
    this.SalesForm = this.formBuilder.group({
      salesPerson: [null, Validators.required],
      barcode: [null, Validators.required],
      GS: [null, Validators.required],
    });
    this.SalesGSForm = this.formBuilder.group({
      GS: [null, Validators.required],
      ItemName: [null, Validators.required],
    });

    this.getSalesMan();
    this.errors = [];
    this.modelErrors = [];
    this.hasErrors = false;


    this.getStaffList();
    this.getGSList();
    this.getMCTypes();
    this.BarcodeSummaryHeader.McType = 5;
    this.getStoneType();
    this.onchangeOption(5);


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

  addWeight(AddBarcodeForm) {
    if (AddBarcodeForm.value.BarcodeNo == null || AddBarcodeForm.value.BarcodeNo === '') {
      this.toastr.warning('Please Enter Barcode/TagNo', 'Alert!');
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
        this._AddBarcodeService.addWtBarcodeCalculation(this.BarcodeSummaryHeader.BarcodeNo, this.BarcodeSummaryHeader.AdBarcode).subscribe(
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
    this._AddBarcodeService.SendBarcodeDataToSalesComp({});
    // if (this.editMode == true) {
    //   this.barcodeService.SendBarcodeDataToSalesComp(this.BarcodeSummaryHeader);
    //   console.log(this.BarcodeSummaryHeader);
    // }
  }

  DedGrWt(arg) {
    if (arg > this.BarcodeSummaryHeader.Grosswt) {
      this.toastr.warning('Deduct wt should not be greater than gross wt', 'Alert!');
    }
    else {

    }
  }

  EditDedStwt() {
    if (this.BarcodeSummaryHeader.Stonewt == 0 || this.BarcodeSummaryHeader.Stonewt == null) {
      this.toastr.warning('No stone wt to deduct', 'Alert!');
    }
    else {
      this.toastr.warning('Item has stone details. Edit or delete from stone details for deduction', 'Alert!');
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
      this.toastr.warning('Enter valid deduct wt. It cannot be greater than gross wt', 'Alert!');
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
        this._AddBarcodeService.castGetSalCode.subscribe(
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

  ItemList: any;
  getItemList(arg) {
    this._AddBarcodeService.getItemfromAPI(arg).subscribe(
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
    this._AddBarcodeService.getItemfromAPI(arg).subscribe(
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
    this._AddBarcodeService.getItemfromAPI(arg).subscribe(
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

  interstateDetails: any;
  CompanyMaster: any;
  interstate: number = 0;

  getInterState() {
    this.EnableRate = true;
    this.EnableMCRate = true;
    this.EnableStnDets = true;
    this._AddBarcodeService.castInterState.subscribe(
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


  getViewBarcodeDetails(arg) {
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
    this.getAddWtItemList(this.BarcodeList.GsCode);
    this.getDedWtItemList(this.BarcodeList.GsCode);
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
    this.BarcodeSummaryHeader.CounterCode = this.BarcodeList.CounterCode;
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
  }


  AddWtDts: any;
  Addwt(arg) {
    if (arg != null && arg != 0) {
      if (this.AlphaNumericValidations(arg) == false) {
        swal("Warning!","Please enter valid Barcode No","warning");
      }
      else {
        this._AddBarcodeService.addWtBarcodeCalculation(this.BarcodeSummaryHeader.BarcodeNo, arg).subscribe(
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
      this.toastr.warning('Please fill in the Add Weight field', 'Alert!');
    }
    else {
      this._AddBarcodeService.getAddWightItemGrossWeight(this.ccode, this.bcode, this.BarcodeSummaryHeader.GsCode,
        this.BarcodePopupHeader.CounterCode, this.BarcodePopupHeader.ItemName).subscribe(
          response => {
            this.weight = response;
            if (this.weight.wight == 0) {
              this.toastr.warning('Item weight is Nil, It cannot be added.', 'Alert!');
            }
            else if (this.BarcodePopupHeader.AdWt > this.weight.wight) {
              this.toastr.warning('Item weight is less than ' + this.BarcodePopupHeader.AdWt + ' grams,It cannot be added.', 'Alert!');
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
      this.toastr.warning('Please select item.', 'Alert!');
    }
    else if (this.DeductHeader.DedQty >= this.BarcodeSummaryHeader.ItemQty) {
      this.toastr.warning('Deduct qty should not be greater than or equal to Item quantity.It cannot be deducted.', 'Alert!');
    }
    else if (this.DeductHeader.Dedwt == null || this.DeductHeader.Dedwt == '') {
      this.toastr.warning('Please fill in the Deduct Weight field', 'Alert!');

    }
    else if (this.DeductHeader.Dedwt >= this.BarcodeSummaryHeader.Grosswt) {
      this.toastr.warning('Item Weight is less or equal ' + this.DeductHeader.Dedwt + ' grams, It cannot be deducted.', 'Alert!');
    }
    else if (this.DeductHeader.DedSW > this.BarcodeSummaryHeader.Stonewt) {
      this.toastr.warning('Deduct stone weight is greater than Item stone weight.It cannot be deducted.', 'Alert!');
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

  calculatedData: any;
  calculateBarcodeDetails() {
    //this.getInterState();
    //this.BarcodeSummaryHeader.isInterstate=this.interstate;
    this._AddBarcodeService.calculateBarcode(this.BarcodeSummaryHeader).subscribe(
      response => {
        this.BarcodeSummaryHeader = null;
        this.calculatedData = response;
        this.BarcodeSummaryHeader = this.calculatedData;
        //this.BillHeader.NW = this.BarcodeSummaryHeader.Netwt; //commented on 30-Mar-20
      },
      (err) => {
        if (err.status === 400) {
          const validationError = err.error;
          swal("Warning!", validationError.description, "warning");
          if (validationError.description == "MC Amount less than Min MC") {
            this.BarcodeSummaryHeader.McPercent = null;
            this.calculateBarcodeDetails();
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
      this.toastr.warning('Please select Staff', 'Alert!');
      this.BarcodeSummaryHeader.Grosswt = 0;
      this.BarcodeSummaryHeader.Stonewt = 0;
    }
    else if (this.BarcodeSummaryHeader.GsCode == null || this.BarcodeSummaryHeader.GsCode == '') {
      this.toastr.warning('Please select GS', 'Alert!');
      this.BarcodeSummaryHeader.Grosswt = 0;
      this.BarcodeSummaryHeader.Stonewt = 0;
    }
    else if (this.BarcodeSummaryHeader.ItemName == null || this.BarcodeSummaryHeader.ItemName == '') {
      this.toastr.warning('Please select Item', 'Alert!');
      this.BarcodeSummaryHeader.Grosswt = 0;
      this.BarcodeSummaryHeader.Stonewt = 0;
    }
    else if (this.BarcodeSummaryHeader.Karat == null || this.BarcodeSummaryHeader.Karat == '') {
      this.toastr.warning('Please select Karat', 'Alert!');
      this.BarcodeSummaryHeader.Grosswt = 0;
      this.BarcodeSummaryHeader.Stonewt = 0;
    }
    else if (this.BarcodeSummaryHeader.Stonewt >= this.BarcodeSummaryHeader.Grosswt) {
      this.toastr.warning('Please enter valid St.Wt(g)', 'Alert!');
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
      this.toastr.warning('Please Enter item weight', 'Alert!');
    }
    else if (this.fieldArray1[index]["Type"] == null || this.fieldArray1[index]["Type"] == 0) {
      this.toastr.warning('Please Select Stone/Diamond', 'Alert!');
    }
    else if (this.fieldArray1[index]["Type"] == null || this.fieldArray1[index]["Type"] == 0) {
      this.toastr.warning('Please Select Stone/Diamond', 'Alert!');
    }
    else if (this.fieldArray1[index]["Name"] == null || this.fieldArray1[index]["Name"] == 0) {
      this.toastr.warning('Please Select ItemName', 'Alert!');
    }
    else if (this.fieldArray1[index]["Qty"] == null || this.fieldArray1[index]["Qty"] == 0) {
      this.toastr.warning('Please Enter Quantity', 'Alert!');
    }
    // else if (this.fieldArray1[index]["Carrat"] == null || this.fieldArray1[index]["Carrat"] == 0) {
    //   this.toastr.warning('Please Enter Carat', 'Alert!');
    // }
    // else if (this.fieldArray1[index]["StoneWt"] == null || this.fieldArray1[index]["StoneWt"] == 0) {
    //   this.toastr.warning('Please Enter Weight', 'Alert!');
    // }
    else if (this.fieldArray1[index]["Rate"] == null || this.fieldArray1[index]["Rate"] == 0) {
      this.toastr.warning('Please Enter Rate/Carat', 'Alert!');
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
              this.toastr.warning('Please enter valid St.Wt', 'Alert!');
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
      this.toastr.warning('Please save the enabled item', 'Alert!');
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
      this.toastr.warning('Please save the enabled item', 'Alert!');
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
    this._AddBarcodeService.getStoneType().subscribe(
      response => {
        this.stoneType = response;
      }
    )
  }

  stoneName: any = [];
  getStoneName(arg, index) {
    this._AddBarcodeService.getStoneName(arg).subscribe(
      response => {
        this.stoneName[index] = response;
      }
    )
  }

  EnableRate: boolean = true;

  EditRate() {
    this.PwdRate.nativeElement.value = "";
    $('#PermissonModalRateGram').modal('show');
    this.EnableRate = true;
  }

  close() {
    $('#PermissonModalRateGram').modal('hide');
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
      this.toastr.warning('Please Enter the Password', 'Alert!');
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

  editRateGram() {
    this.EnableRate = true;
    this.calculateBarcodeDetails();
  }

  EnableMCRate: boolean = true;
  EnableStnDets: boolean = true;

  EditMC() {
    if (this.BarcodeSummaryHeader.PieceRate == 'Y') {
      this.toastr.warning('This is a piece item', 'Alert!');
      $('#PermissonMCModal').modal('hide');
    }
    else {
      this.PwdMC.nativeElement.value = "";
      $('#PermissonMCModal').modal('show');
    }
  }

  passWordMC(arg) {
    if (arg == "") {
      this.toastr.warning('Please Enter the Password', 'Alert!');
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
      this.toastr.warning('Please Enter the Password', 'Alert!');
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

  BarcodeAge: any = []
  // getAgeofTheBarcode(arg) {
  //   this._AddBarcodeService.getBarcodeAge(arg).subscribe(
  //     response => {
  //       this.BarcodeAge = response;
  //       // console.log( this.BarcodeAge);
  //       this.EnableBarcodeAge = true;
  //     }
  //   )
  // }


  getItem() {
    this._masterService.getItemName(this.SalesGSForm.value.GS).subscribe(
      response => {
        this.ItemName = response;
      }

    )
  }

  SalesLinesPost: any = {
    CustID: 0,
    GSType: 'NGO',
    OperatorCode: null,
    salesEstimatonVM: []
  }


  SalesPersonModel: any = {
    CustID: null,
    salesPerson: null,
    barcode: null,
    GS: null,
    ItemName: null
  }

  // convenience getter for easy access to form fields
  get f() { return this.SalesForm.controls; }

  SalesManList: any;
  getSalesMan() {
    this._masterService.getSalesMan().subscribe(
      response => {
        this.SalesManList = response;
      })
  }

  MCNameList: any;
  getMc() {
    this._masterService.getMCTypes().subscribe(
      response => {
        this.MCList = response;
        // this.MCNameList = this.MCList[];
      }
    );
  }

  getGSList() {
    this._masterService.getGsList('S').subscribe(
      response => {
        this.GSList = response;
        this.getItem();
      }
    );
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

  getDetails(form) {
    if (form.value.barcode == null || form.value.barcode == "") {
      alert("Please enter barcode");
    }
    else if (form.value.barcode != null && form.value.barcode != "") {
      if (this.AlphaNumericValidations(form.value.barcode) == false) {
        alert("Please enter valid Barcode No");
      }
      else {
        this.addBarCode(form);
        this.SalesPersonModel.barcode = '';
        this.getGSList();
        this.getMc();
      }
    }
  }

  addBarCode(arg) {
    this.GetBarcodeList = [];
    this.getBarcodeDetails(arg.value.barcode);
    this.SalesPersonModel.barcode = '';
  }


  getBarcodeDetails(barcode) {
    this._salesService.getBarcodeWithStoneWithoutValidation(barcode).subscribe(
      response => {
        this.ArrayList.push(response);
        // this.ArrayList[0].CounterCode = salesPerson;
        this.GetBarcodeList.push(this.ArrayList[0]);
        if (this.SalesEstNo == null) {
          this.SalesLinesPost.salesEstimatonVM.push(this.ArrayList[0]);
        }
        this.ToggleCalculation = true;
        //this.SendSalesDataToEstComp();
        this.ArrayList = [];
        this.GetBarcodeInfo(this.GetBarcodeList[0]);
      },
      // (err) => {
      //   if (err.status === 400) {
      //     const validationError = err.error;
      //     alert(validationError.description);
      //   }
      //   else {
      //     this.errors.push('something went wrong!');
      //   }
      // }
    );
  }


  modalPopUPBarcode: any;
  Toggle: boolean = false;
  ToggleCalculation: boolean = false;
  GetBarcodeInfo(arg) {
    this.Toggle = true;
    this.GetAddBarcodeList = arg;
    this.EnableRate = true;
    this.EnableMCRate = true;
    this.EnableStnDets = true;
    if (this.isEmptyObject(this.GetAddBarcodeList) == false && this.isEmptyObject(this.GetAddBarcodeList) != null) {
      this._salesService.SendSalesDataToAddBarcode(null);
      this._salesService.SendSalesDataToAddBarcode(arg);
      this._estimationService.SendOrderNoToSalesComp(0);
      $('#addBarcode').modal('show');
      this.EnableSubmitButton = false;
      this.HideRadioBtn = false;
      this.EnableAddRow = false;
      this.readOnly = true;
      this.editMode = true;
      this.BarCodeNo = this.GetAddBarcodeList.BarcodeNo;
      //this.getAgeofTheBarcode(this.GetAddBarcodeList.BarcodeNo);
      this.onchangeOption(this.GetAddBarcodeList.McType);
      this.getItemList(this.GetAddBarcodeList.GsCode);
      this.getAddWtItemList(this.GetAddBarcodeList.GsCode);
      this.getDedWtItemList(this.GetAddBarcodeList.GsCode);
      this.BarcodeSummaryHeader.CompanyCode = this.ccode;
      this.BarcodeSummaryHeader.BranchCode = this.bcode;
      this.BarcodeSummaryHeader.BarcodeNo = this.BarCodeNo;
      this.BarcodeSummaryHeader.SalCode = this.GetAddBarcodeList.SalCode;
      this.BarcodeSummaryHeader.AdItem = this.GetAddBarcodeList.AdItem;
      this.BarcodeSummaryHeader.AdCounter = this.GetAddBarcodeList.AdCounter;
      this.BarcodeSummaryHeader.isInterstate = this.GetAddBarcodeList.isInterstate;
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
    }
    else {
      this.HideRadioBtn = true;
    }


    this.modalPopUPBarcode = null;
    this.modalPopUPBarcode = arg;
    this.getGSList();
    this.SalesPersonModel.GS = this.modalPopUPBarcode.GsCode;
    this._masterService.getItemName(this.SalesPersonModel.GS).subscribe(
      response => {
        this.ItemName = response;
        this.SalesPersonModel.ItemName = this.modalPopUPBarcode.ItemName;
      });
    this.getGetStoneListDetails(arg);
    //this.getAgeofTheBarcode(arg.BarcodeNo);
  }

  MCTypeList: any;
  McType: any;

  getMCTypes() {
    this._orderservice.getMCTypes().subscribe(
      response => {
        this.MCTypeList = response;
      }
    )
  }

  NoRecords: boolean = false;
  GetStoneList: any = [];
  GetStonearray: any = [];
  getGetStoneListDetails(arg) {
    this.GetStoneList = [];

    this.GetStonearray = arg.salesEstStoneVM;
    for (let i = 0; i < this.GetStonearray.length; i++) {
      this.GetStoneList.push(this.GetStonearray[i]);
    }
    if (this.isEmptyObject(this.GetStoneList) == false) {
      this.NoRecords = true;
    }
    else {
      this.NoRecords = false;
    }
  }

  SendSalesDataToEstComp() {
    this._salesService.SendSalesDataToEstComp(this.SalesSummaryData);
  }

  isEmptyObject(obj) {
    return (obj && (Object.keys(obj).length === 0));
  }

  onSubmit() {
    this.submitted = true;
    if (this.SalesForm.invalid) {
      return;
    }
  }

  ngOnDestroy() {
    this._salesService.SaveSalesEstNo(null);
  }

  //Danger Zone
  DeleteTagRow(index: number, arg) {
    var ans = confirm("Do you want to delete");
    if (ans) {
      this.GetBarcodeList.splice(index, 1);
      if (this.SalesEstNo == null) {
        this.SalesLinesPost.salesEstimatonVM.splice(this.SalesLinesPost.salesEstimatonVM.indexOf(arg), 1);
      }
    }
  }

  total_items(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      total += parseInt(d.ItemQty);
    });
    this.SalesSummaryData.TotalItems = total;
    return total;
  }

  grossWT(arg: any[] = []) {
    let total = 0; arg.forEach((d) => { total += d.Grosswt; }); this.SalesSummaryData.Grwt = total; return total;
  }

  netWT(arg: any[] = []) {
    let total = 0; arg.forEach((d) => { total += d.Netwt; }); this.SalesSummaryData.NtWt = total; return total;
  }

  wastage_grms(arg: any[] = []) { let total = 0; arg.forEach((d) => { total += d.WastageGrms; }); return total; }

  metal_amount(arg: any[] = []) { let total = 0; arg.forEach((d) => { total += d.GoldValue; }); return total; }

  stone_amount(arg: any[] = []) { let total = 0; arg.forEach((d) => { total += d.StoneCharges; }); return total; }

  va_amount(arg: any[] = []) { let total = 0; arg.forEach((d) => { total += d.VaAmount; }); return total; }

  taxable_amount(arg: any[] = []) { let total = 0; arg.forEach((d) => { total += d.TotalAmount; }); this.SalesSummaryData.taxable_Amt = total; return total; }

  SGST_amount(arg: any[] = []) { let total = 0; arg.forEach((d) => { total += d.TotalAmount * 0.015; }); this.SalesSummaryData.CGst = total; return total; }

  CGST_amount(arg: any[] = []) { let total = 0; arg.forEach((d) => { total += d.TotalAmount * 0.015; }); this.SalesSummaryData.SGst = total; return total; }

  SalesTotal_amount(arg: any[] = []) { let total = 0; arg.forEach((d) => { total += d.TotalAmount + d.TotalAmount * 0.03 }); this.SalesSummaryData.Amount = total; return total; }

}
