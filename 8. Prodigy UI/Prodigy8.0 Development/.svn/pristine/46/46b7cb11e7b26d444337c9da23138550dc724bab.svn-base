import { AddBarcodeService } from './add-barcode/add-barcode.service';
import { Subscription } from 'rxjs';
import { ModelError } from './sales.model';
import { Router } from '@angular/router';
import { CustomerService } from './../masters/customer/customer.service';
import { SalesService } from './sales.service';
import { estimationService } from '../estimation/estimation.service';
import { Component, OnInit, OnDestroy, Input, Output, EventEmitter, ViewChild, ElementRef, AfterContentChecked, AfterViewInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SalesSummaryModel } from './sales.model';
import swal from 'sweetalert';
import { BarcodedetailsService } from '../barcodedetails/barcodedetails.service';
import { OrdersService } from '../orders/orders.service';
import { MasterService } from '../core/common/master.service';
import * as CryptoJS from 'crypto-js';
import { ToastrService } from 'ngx-toastr';
import { AppConfigService } from '../AppConfigService';
import { offerDiscountModel } from './sales.model';
import { formatDate } from '@angular/common';


declare var $: any;

@Component({
  selector: 'app-sales',
  templateUrl: './sales.component.html',
  styleUrls: ['./sales.component.css']
})

export class SalesComponent implements OnInit, OnDestroy, AfterContentChecked  {
  today = new Date();
  ApplyFab2020Offer: boolean = false;
  OfferModel2022 =
    {
      CompanyCode: "",
      BranchCode: "",
      EstimationNo: 0,
      Date: "",
      Rate: null,
      SalesEstDetail: []
    }
  OfferModel2022LineItems = {
    SlNo: 0,
    GSCode: "",
    BarcodeNo: "",
    ItemCode: "",
    SalesmanCode: "",
    CounterCode: "",
    MRPItem: true,
    Qty: 0,
    Grosswt: 0,
    Stonewt: 0,
    Netwt: 0,
    MetalValue: 0,
    VAAmount: 0,
    StoneCharges: 0,
    DiamondCharges: 0,
    Dcts: 0,
    ItemAmount: 0
  };
  @ViewChild("PwdInterState", { static: true }) PwdInterState: ElementRef;
  EnableDisablectrls: boolean = true;
  @ViewChild("PwdAddBarcode", { static: true }) PwdAddBarcode: ElementRef;
  GroupBarcodingForm: FormGroup;
  EnableSubmitButton: boolean = true;
  SalesForm: FormGroup;
  SalesGSForm: FormGroup;
  submitted = false;
  Customer: any;
  OrderNoFromEstComp: Number = 0;
  GetBarcodeList: any = [];
  ArrayList: any = [];
  OrderList: any = [];
  KaratForm: FormGroup;
  errors: string[];
  hasErrors = false;
  id: number = 0;
  readonly = true;
  @Output() valueChange = new EventEmitter();
  ccode: string = "";
  bcode: string = "";
  password: string;
  GST: number = 0;
  IGST: number = 0;
  PlaceOfSupplyEdit: string;
  AddBarcodePermission: string;
  EnableJson: boolean = false;


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

  offerDiscountModel: offerDiscountModel = {
    SalesAmountExclTax: null,
    DiscountAmount: null,
    GSTAmount: null,
    GSTCessAmount: null,
    GSTAmountInclCess: null,
    SalesAmountInclTax: null,
    DiscountPercent: null,
    OldPurchaseAmount: null,
    OrderAmount: null,
    OrderRateDiscount: null,
    OfferDiscount: null,
    MCDiscount: null,
    AdditionalDiscount: null
  }

  constructor(private formBuilder: FormBuilder,
    private _salesService: SalesService, private _CustomerService: CustomerService,
    private _estimationService: estimationService, private _addBarcodeService: AddBarcodeService,
    private _barcodedetailsService: BarcodedetailsService, private _orderservice: OrdersService,
    private _masterService: MasterService, private _router: Router, private toastr: ToastrService,
    private _appConfigService: AppConfigService
  ) {
    this.PlaceOfSupplyEdit = this._appConfigService.RateEditCode.PlaceOfSupplyEdit;
    this.AddBarcodePermission = this._appConfigService.RateEditCode.AddBarcodePermission;
    this.GST = Number(<number>this._appConfigService.GST / 100);
    this.IGST = Number(<number>this._appConfigService.IGST / 100);
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
    this.getCB();
  }


  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  EnableDisableSubmitBtn() {
    if (this.GetAddBarcodeList.length <= 0) {
      this.EnableSubmitButton = true;
    }
    else {
      this.EnableSubmitButton = false;
    }
  }

  SalesLinesPost: any = {
    CompanyCode: this.ccode,
    BranchCode: this.bcode,
    CustID: 0,
    GSType: '',
    EstNo: 0,
    MobileNo: null,
    OperatorCode: null,
    OrderNo: 0,
    OrderAmount: 0,
    Pos: null,
    IsOfferApplied: false,
    IsOfferSkipped: false,
    DiscountAmount: 0,
    salesEstimatonVM: [],
    offerDiscount: this.offerDiscountModel,
    RowRevisionString: "",
    Overwrite: false
  }

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

  CustomerDets: any = [];
  SalesEstNo: string = null;
  BarCodeNo: string = "";
  arrayObj: any = [];
  pos: any = [];
  Unsubscribe_obj: Subscription;
  GetAddBarcodeList: any = [];
  GetReservedBarcodeList: any = [];
  CoinOfferBarcodeList: any;
  CoinOfferData: any;


  groupBarcodingModel: any = {
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
    TaggingType: null,
    Quantity: null,
    BillingGrossWt: null,
    BillingStoneWt: null,
    BillingNetWt: null,
    salesEstStoneVM: []
  }


  DiffMetalValid: any;
  SalesData: any;
  RowReverisonNo: any;

  ngOnInit() {

    this.EnableSubmitButton = true;

    this.GroupBarcodingForm = this.formBuilder.group({
      BarcodeNo: ["", Validators.required],
      ItemName: ["", Validators.required],
      ItemQty: ["", Validators.required],
      Quantity: ["", Validators.required],
      Grosswt: ["", Validators.required],
      BillingGrossWt: ["", Validators.required],
      Stonewt: ["", Validators.required],
      BillingStoneWt: ["", Validators.required],
      Netwt: ["", Validators.required],
      BillingNetWt: ["", Validators.required],
      ObjStatus: ["", Validators.required],
      GSSeqNo: ["", Validators.required],
      SchemeCode: ["", Validators.required],
      VATID: ["", Validators.required],
      OdLimit: ["", Validators.required],
      UpdateOn: ["", Validators.required],
      JFlag: ["", Validators.required],
      PANCard: ["", Validators.required],
      TIN: ["", Validators.required],
      LedgerType: ["", Validators.required],
      Address1: ["", Validators.required],
      Address2: ["", Validators.required],
      Address3: ["", Validators.required],
      City: ["", Validators.required],
      State: ["", Validators.required],
      District: ["", Validators.required],
      Country: ["", Validators.required],
      PinCode: ["", Validators.required],
      Phone: ["", Validators.required],
      Mobile: ["", Validators.required],
      FaxNo: ["", Validators.required],
      WebSite: ["", Validators.required],
      CSTNo: ["", Validators.required],
      BudgetAmt: ["", Validators.required],
      TDSID: ["", Validators.required],
      EmailID: ["", Validators.required],
      HSN: ["", Validators.required],
      GSTGoodsGroupCode: ["", Validators.required],
      GSTServicesGroupCode: ["", Validators.required],
      StateCode: ["", Validators.required],
      VTYPE: ["", Validators.required],
      UniqRowID: ["", Validators.required],
      TransType: ["", Validators.required],
      IsAutomatic: ["", Validators.required],
      Schedule_Name: ["", Validators.required],
      NewAccCode: ["", Validators.required],
      PartyACNo: ["", Validators.required],
      PartyACName: ["", Validators.required],
      PartyMICR_No: ["", Validators.required],
      PartyBankName: ["", Validators.required],
      PartyBankBranch: ["", Validators.required],
      PartyBankAddress: ["", Validators.required],
      PartyRTGScode: ["", Validators.required],
      PartyNEFTcode: ["", Validators.required],
      PartyIFSCcode: ["", Validators.required],
      swiftcode: ["", Validators.required],
    });

    this.SalesSummaryData = {
      Grwt: null,
      NtWt: null,
      SGst: null,
      CGst: null,
      IGst: null,
      TotalItems: null,
      Amount: null,
      taxable_Amt: null,
    };
    this.SalesForm = this.formBuilder.group({
      salesPerson: [null, Validators.required],
      barcode: [null, Validators.required],
      orderNo: [null, Validators.required],
      GS: [null, Validators.required],
      StateCode: [null, Validators.required]
    });

    this.SalesGSForm = this.formBuilder.group({
      GS: [null, Validators.required],
      ItemName: [null, Validators.required],
    });


    this.Unsubscribe_obj = this._CustomerService.cast.subscribe(
      response => {
        this.CustomerDets = response;
        if (this.CustomerDets != null && this.CustomerDets != 0) {
          this.SalesLinesPost.CustID = this.CustomerDets.ID;
          this.SalesLinesPost.MobileNo = this.CustomerDets.MobileNo;
          this.SalesLinesPost.OperatorCode = localStorage.getItem('Login');
          this.SalesLinesPost.CompanyCode = this.ccode,
            this.SalesLinesPost.BranchCode = this.bcode
          this.markFormGroupDirty(this.SalesForm);
        }
        else {
          this.SalesLinesPost.CustID = null;
          this.SalesLinesPost.OperatorCode = localStorage.getItem('Login');
          this.SalesLinesPost.CompanyCode = this.ccode,
            this.SalesLinesPost.BranchCode = this.bcode
        }
      });

    this.Unsubscribe_obj = this._estimationService.SubjectEstNo.subscribe(
      response => {
        this.SalesEstNo = response;
        if (this.SalesEstNo != null && this.SalesEstNo != "0") {
          if (this._router.url != "/sales-billing") {
            this._salesService.getRowVersion(this.SalesEstNo).subscribe(
              response => {
                this.RowReverisonNo = response;
                this.SalesLinesPost.RowRevisionString = this.RowReverisonNo.RowRevisionString;
              });
          }
          if (this._router.url != "/purchase") {
            this.EnableSubmitButton = true;
            this.markFormGroupPristine(this.SalesForm);
            this._salesService.castSalesDtls.subscribe(
              response => {
                this.arrayObj = response;
                if (this.arrayObj != null) {
                  this.EnableSubmitButton = true;
                  if (this.pos != "") {
                    this.SalesLinesPost.Pos = this.pos;
                  }
                  else if (this.arrayObj.Pos != undefined) {
                    this.SalesLinesPost.Pos = this.arrayObj.Pos;
                  }
                  this.SalesPersonModel.salesPerson = this.arrayObj.salesEstimatonVM[0].SalCode;//Added on 22-Jun-2020
                  this.SalesLinesPost.GSType = this.arrayObj.GSType;
                  this.SalesLinesPost.OrderNo = this.arrayObj.OrderNo;
                  this.SalesLinesPost.EstNo = this.SalesEstNo;
                  this.SalesLinesPost.OrderAmount = this.arrayObj.OrderAmount;
                  this.SalesLinesPost.DiscountAmount = this.arrayObj.DiscountAmount;
                  this.SalesLinesPost.Overwrite = false;
                  this.GetBarcodeList = this.arrayObj.salesEstimatonVM;
                  this.SalesLinesPost.salesEstimatonVM = this.arrayObj.salesEstimatonVM;
                  this.ToggleCalculation = true;
                  this.SalesLinesPost.offerDiscount = this.arrayObj.offerDiscount;
                  this.SendSalesDataToEstComp();
                  for (let i = 0; i < this.SalesLinesPost.salesEstimatonVM.length; i++) {
                    if (this.SalesLinesPost.salesEstimatonVM[i].BarcodeNo == "") {
                      this.getGstPercent(this.SalesLinesPost.salesEstimatonVM[i].GsCode, this.SalesLinesPost.salesEstimatonVM[i].ItemName);
                    }
                  }
                }
              }
            )
          }
        }
        //Below else condition has been added as per sivanand to redirect to fresh estimation
        // else {
        //   this.SalesLinesPost = {
        //     CompanyCode: this.ccode,
        //     BranchCode: this.bcode,
        //     CustID: 0,
        //     GSType: '',
        //     EstNo: 0,
        //     MobileNo: null,
        //     OperatorCode: null,
        //     OrderNo: 0,
        //     OrderAmount: 0,
        //     Pos: null,
        //     IsOfferApplied: false,
        //     IsOfferSkipped: false,
        //     DiscountAmount: 0,
        //     salesEstimatonVM: [],
        //     offerDiscount: this.offerDiscountModel,
        //     RowRevisionString: "",
        //     Overwrite: false
        //   }
        //   this.getStateList();
        //   this.SalesPersonModel.salesPerson = null;
        //   this.SalesLinesPost.OperatorCode = localStorage.getItem('Login');
        //   this.EnableSubmitButton = false;
        //   this.GetBarcodeList = [];
        // }
        //ends here
      });


    this.Unsubscribe_obj = this._addBarcodeService.cast.subscribe(
      response => {
        this.GetAddBarcodeList = response;
        if (this.SalesEstNo != null && this.SalesEstNo != "0") {
          if (this.isEmptyObject(this.GetAddBarcodeList) == false && this.isEmptyObject(this.GetAddBarcodeList) != null) {
            if (this.SalesLinesPost.salesEstimatonVM.length == 0) {
              this.GetBarcodeList = [];
              if (this.EditMode == true) {
                this.GetBarcodeList.splice(this.IndexNo, 1);
                //this.EditMode = false; //Commented on 03-Nov-2021 as part of bug id:67(i.e. Barcode already added)
              }

              if (this.isEmptyObject(this.GetAddBarcodeList) == false && this.isEmptyObject(this.GetAddBarcodeList) != null) {
                this.GetBarcodeList.push(this.GetAddBarcodeList);
                this.SalesLinesPost.salesEstimatonVM = [];
                this.SalesLinesPost.salesEstimatonVM = this.GetBarcodeList;
              }

              for (let i = 0; i < this.SalesLinesPost.salesEstimatonVM.length; i++) {
                this.getGstPercent(this.SalesLinesPost.salesEstimatonVM[i].GsCode, this.SalesLinesPost.salesEstimatonVM[i].ItemName);
              }

              this._addBarcodeService.SendBarcodeDataToSalesComp(null);
              this.GetAddBarcodeList = [];
              this.ToggleCalculation = true;
              this.markFormGroupDirty(this.SalesForm);
            }
            else {
              if (this.EditMode == true) {
                this.GetBarcodeList.splice(this.IndexNo, 1);
                //this.EditMode = false; //Commented on 03-Nov-2021 as part of bug id:67(i.e. Barcode already added)
                if (this.isEmptyObject(this.GetAddBarcodeList) == false && this.isEmptyObject(this.GetAddBarcodeList) != null) {
                  this.GetBarcodeList.push(this.GetAddBarcodeList);
                  this.SalesLinesPost.salesEstimatonVM = [];
                  this.SalesLinesPost.salesEstimatonVM = this.GetBarcodeList;
                }

                for (let i = 0; i < this.SalesLinesPost.salesEstimatonVM.length; i++) {
                  this.getGstPercent(this.SalesLinesPost.salesEstimatonVM[i].GsCode, this.SalesLinesPost.salesEstimatonVM[i].ItemName);
                }

                this.GetAddBarcodeList = [];
                this.ToggleCalculation = true;
                this._addBarcodeService.SendBarcodeDataToSalesComp(null);
                this.markFormGroupDirty(this.SalesForm);
              }
              else {
                if (this.EditMode == false) { // Added on 03-Nov-2021 as part of bug id:67(i.e. Barcode already added)
                  this._salesService.BarcodeValidation(this.SalesLinesPost, this.GetAddBarcodeList.BarcodeNo, this.GetAddBarcodeList.GsCode).subscribe(
                    response => {
                      this.DiffMetalValid = response;
                      if (this.DiffMetalCheck.length == 0) {

                        if (this.isEmptyObject(this.GetAddBarcodeList) == false && this.isEmptyObject(this.GetAddBarcodeList) != null) {
                          this.GetBarcodeList.push(this.GetAddBarcodeList);
                          this.SalesLinesPost.salesEstimatonVM = [];
                          this.SalesLinesPost.salesEstimatonVM = this.GetBarcodeList;
                        }

                        for (let i = 0; i < this.SalesLinesPost.salesEstimatonVM.length; i++) {
                          this.getGstPercent(this.SalesLinesPost.salesEstimatonVM[i].GsCode, this.SalesLinesPost.salesEstimatonVM[i].ItemName);
                        }

                        this.GetAddBarcodeList = [];
                        this.ToggleCalculation = true;
                        this._addBarcodeService.SendBarcodeDataToSalesComp(null);
                        this.markFormGroupDirty(this.SalesForm);
                      }
                    },
                    (err) => {
                      $('#addBarcode').modal('hide');
                    }
                  )
                }
              }
            }
          }
        }
        $('#addBarcode').modal('hide');
      })


    //Commented on 16-Mar-2021

    // this.Unsubscribe_obj = this._addBarcodeService.cast.subscribe(
    //   response => {
    //     this.GetAddBarcodeList = response;
    //     if (this.SalesEstNo == null || this.SalesEstNo == "0") {
    //       if (this.isEmptyObject(this.GetAddBarcodeList) == false && this.isEmptyObject(this.GetAddBarcodeList) != null) {
    //         if (this.SalesLinesPost.salesEstimatonVM.length == 0) {
    //           if (this.EditMode == true) {
    //             // if (this.SalesLinesPost.GSType != "") {
    //             //   this.GetBarcodeList.splice(this.IndexNo, 1);
    //             //   this.GetBarcodeList.push(this.GetAddBarcodeList);
    //             //   // this.SalesLinesPost.salesEstimatonVM.splice(this.SalesLinesPost.salesEstimatonVM.indexOf(this.IndexNo), 1);
    //             //   this.SalesLinesPost.salesEstimatonVM.splice(this.IndexNo, 1);
    //             //   this.SalesLinesPost.salesEstimatonVM.push(this.GetAddBarcodeList);
    //             //   this.EditMode = false;
    //             // }
    //             // else {
    //             this.GetBarcodeList.splice(this.IndexNo, 1);
    //             if (this.isEmptyObject(this.GetAddBarcodeList) == false && this.isEmptyObject(this.GetAddBarcodeList) != null) {
    //               this.GetBarcodeList.push(this.GetAddBarcodeList);
    //             }
    //             //this.GetBarcodeList.push(this.GetAddBarcodeList);
    //             this.SalesLinesPost.salesEstimatonVM.splice(this.IndexNo, 1);
    //             this.SalesLinesPost.salesEstimatonVM.push(this.GetAddBarcodeList);
    //             this.EditMode = false;
    //             this.markFormGroupDirty(this.SalesForm);
    //             //}
    //           }
    //           else {
    //             if (this.SalesLinesPost.GSType != "") {
    //               if (this.isEmptyObject(this.GetAddBarcodeList) == false && this.isEmptyObject(this.GetAddBarcodeList) != null) {
    //                 this.GetBarcodeList.push(this.GetAddBarcodeList);
    //               }
    //               //this.GetBarcodeList.push(this.GetAddBarcodeList);
    //               this.SalesLinesPost.salesEstimatonVM.push(this.GetAddBarcodeList);
    //               this.markFormGroupDirty(this.SalesForm);
    //             }
    //             else {
    //               this.SalesLinesPost.GSType = this.GetAddBarcodeList.GsCode;
    //               if (this.isEmptyObject(this.GetAddBarcodeList) == false && this.isEmptyObject(this.GetAddBarcodeList) != null) {
    //                 this.GetBarcodeList.push(this.GetAddBarcodeList);
    //               }
    //               //this.GetBarcodeList.push(this.GetAddBarcodeList);
    //               this.SalesLinesPost.salesEstimatonVM.push(this.GetAddBarcodeList);
    //               this.markFormGroupDirty(this.SalesForm);
    //             }
    //           }
    //           for (let i = 0; i < this.SalesLinesPost.salesEstimatonVM.length; i++) {
    //             this.getGstPercent(this.SalesLinesPost.salesEstimatonVM[i].GsCode, this.SalesLinesPost.salesEstimatonVM[i].ItemName);
    //           }
    //           this._addBarcodeService.SendBarcodeDataToSalesComp(null);
    //           this.GetAddBarcodeList = [];
    //           this.ToggleCalculation = true;
    //         }
    //         else {
    //           if (this.EditMode == true) {
    //             // if (this.SalesLinesPost.GSType != "") {
    //             // this.GetBarcodeList.splice(this.IndexNo, 1);
    //             // this.GetBarcodeList.push(this.GetAddBarcodeList);
    //             // // this.SalesLinesPost.salesEstimatonVM.splice(this.SalesLinesPost.salesEstimatonVM.indexOf(this.IndexNo), 1);
    //             // this.SalesLinesPost.salesEstimatonVM.splice(this.IndexNo, 1);
    //             // this.SalesLinesPost.salesEstimatonVM.push(this.GetAddBarcodeList);
    //             // this.EditMode = false;
    //             // }
    //             // else {
    //             this.GetBarcodeList.splice(this.IndexNo, 1);
    //             //this.GetBarcodeList.push(this.GetAddBarcodeList);
    //             if (this.isEmptyObject(this.GetAddBarcodeList) == false && this.isEmptyObject(this.GetAddBarcodeList) != null) {
    //               this.GetBarcodeList.push(this.GetAddBarcodeList);
    //             }
    //             this.SalesLinesPost.salesEstimatonVM.splice(this.IndexNo, 1);
    //             this.SalesLinesPost.salesEstimatonVM.push(this.GetAddBarcodeList);
    //             this.EditMode = false;
    //             // }
    //             for (let i = 0; i < this.SalesLinesPost.salesEstimatonVM.length; i++) {
    //               this.getGstPercent(this.SalesLinesPost.salesEstimatonVM[i].GsCode, this.SalesLinesPost.salesEstimatonVM[i].ItemName);
    //             }
    //             this._addBarcodeService.SendBarcodeDataToSalesComp(null);
    //             this.GetAddBarcodeList = [];
    //             this.ToggleCalculation = true;
    //             this.markFormGroupDirty(this.SalesForm);
    //           }
    //           else {
    //             this._salesService.BarcodeValidation(this.SalesLinesPost, this.GetAddBarcodeList.BarcodeNo, this.GetAddBarcodeList.GsCode).subscribe(
    //               response => {
    //                 this.DiffMetalValid = response;
    //                 if (this.DiffMetalCheck.length == 0) {
    //                   if (this.SalesLinesPost.GSType != "") {
    //                     if (this.isEmptyObject(this.GetAddBarcodeList) == false && this.isEmptyObject(this.GetAddBarcodeList) != null) {
    //                       this.GetBarcodeList.push(this.GetAddBarcodeList);
    //                     }
    //                     //this.GetBarcodeList.push(this.GetAddBarcodeList);
    //                     this.SalesLinesPost.salesEstimatonVM.push(this.GetAddBarcodeList);
    //                   }
    //                   else {
    //                     this.SalesLinesPost.GSType = this.GetAddBarcodeList.GsCode;
    //                     if (this.isEmptyObject(this.GetAddBarcodeList) == false && this.isEmptyObject(this.GetAddBarcodeList) != null) {
    //                       this.GetBarcodeList.push(this.GetAddBarcodeList);
    //                     }
    //                     //this.GetBarcodeList.push(this.GetAddBarcodeList);
    //                     this.SalesLinesPost.salesEstimatonVM.push(this.GetAddBarcodeList);
    //                   }

    //                   for (let i = 0; i < this.SalesLinesPost.salesEstimatonVM.length; i++) {
    //                     this.getGstPercent(this.SalesLinesPost.salesEstimatonVM[i].GsCode, this.SalesLinesPost.salesEstimatonVM[i].ItemName);
    //                   }
    //                   this._addBarcodeService.SendBarcodeDataToSalesComp(null);
    //                   this.GetAddBarcodeList = [];
    //                   this.ToggleCalculation = true;
    //                   this.markFormGroupDirty(this.SalesForm);
    //                 }
    //               },
    //               (err) => {
    //                 $('#addBarcode').modal('hide');
    //               }
    //             )
    //           }
    //         }
    //       }
    //     }
    //     $('#addBarcode').modal('hide');
    //   });


    //Added on 16-Mar-2021

    this.Unsubscribe_obj = this._addBarcodeService.cast.subscribe(
      response => {
        this.GetAddBarcodeList = response;
        if (this.SalesEstNo == null || this.SalesEstNo == "0") {
          if (this.isEmptyObject(this.GetAddBarcodeList) == false && this.isEmptyObject(this.GetAddBarcodeList) != null) {
            if (this.SalesLinesPost.salesEstimatonVM.length == 0) {
              this.GetBarcodeList = [];
              if (this.EditMode == true) {
                this.GetBarcodeList.splice(this.IndexNo, 1);
                if (this.isEmptyObject(this.GetAddBarcodeList) == false && this.isEmptyObject(this.GetAddBarcodeList) != null) {
                  this.GetBarcodeList.push(this.GetAddBarcodeList);
                  this.SalesLinesPost.salesEstimatonVM = [];
                  this.SalesLinesPost.salesEstimatonVM = this.GetBarcodeList;
                }
                //this.EditMode = false; //Commented on 03-Nov-2021 as part of bug id:67(i.e. Barcode already added)
                this.markFormGroupDirty(this.SalesForm);
              }
              else {
                if (this.SalesLinesPost.GSType != "") {
                  if (this.isEmptyObject(this.GetAddBarcodeList) == false && this.isEmptyObject(this.GetAddBarcodeList) != null) {
                    this.GetBarcodeList.push(this.GetAddBarcodeList);
                    this.SalesLinesPost.salesEstimatonVM = [];
                    this.SalesLinesPost.salesEstimatonVM = this.GetBarcodeList;
                  }
                  this.markFormGroupDirty(this.SalesForm);
                }
                else {
                  this.SalesLinesPost.GSType = this.GetAddBarcodeList.GsCode;
                  if (this.isEmptyObject(this.GetAddBarcodeList) == false && this.isEmptyObject(this.GetAddBarcodeList) != null) {
                    this.GetBarcodeList.push(this.GetAddBarcodeList);
                    this.SalesLinesPost.salesEstimatonVM = [];
                    this.SalesLinesPost.salesEstimatonVM = this.GetBarcodeList;
                  }
                  this.markFormGroupDirty(this.SalesForm);
                }
              }
              for (let i = 0; i < this.SalesLinesPost.salesEstimatonVM.length; i++) {
                this.getGstPercent(this.SalesLinesPost.salesEstimatonVM[i].GsCode, this.SalesLinesPost.salesEstimatonVM[i].ItemName);
              }
              this._addBarcodeService.SendBarcodeDataToSalesComp(null);
              this.GetAddBarcodeList = [];
              this.ToggleCalculation = true;
            }
            else {
              if (this.EditMode == true) {
                this.GetBarcodeList.splice(this.IndexNo, 1);
                if (this.isEmptyObject(this.GetAddBarcodeList) == false && this.isEmptyObject(this.GetAddBarcodeList) != null) {
                  this.GetBarcodeList.push(this.GetAddBarcodeList);
                  this.SalesLinesPost.salesEstimatonVM = [];
                  this.SalesLinesPost.salesEstimatonVM = this.GetBarcodeList;
                }
                //this.EditMode = false; //Commented on 03-Nov-2021 as part of bug id:67(i.e. Barcode already added)
                for (let i = 0; i < this.SalesLinesPost.salesEstimatonVM.length; i++) {
                  this.getGstPercent(this.SalesLinesPost.salesEstimatonVM[i].GsCode, this.SalesLinesPost.salesEstimatonVM[i].ItemName);
                }
                this._addBarcodeService.SendBarcodeDataToSalesComp(null);
                this.GetAddBarcodeList = [];
                this.ToggleCalculation = true;
                this.markFormGroupDirty(this.SalesForm);
              }
              else {
                this._salesService.BarcodeValidation(this.SalesLinesPost, this.GetAddBarcodeList.BarcodeNo, this.GetAddBarcodeList.GsCode).subscribe(
                  response => {
                    this.DiffMetalValid = response;
                    if (this.DiffMetalCheck.length == 0) {
                      if (this.isEmptyObject(this.GetAddBarcodeList) == false && this.isEmptyObject(this.GetAddBarcodeList) != null) {
                        this.GetBarcodeList.push(this.GetAddBarcodeList);
                        this.SalesLinesPost.salesEstimatonVM = [];
                        this.SalesLinesPost.salesEstimatonVM = this.GetBarcodeList;
                      }
                      for (let i = 0; i < this.SalesLinesPost.salesEstimatonVM.length; i++) {
                        this.getGstPercent(this.SalesLinesPost.salesEstimatonVM[i].GsCode, this.SalesLinesPost.salesEstimatonVM[i].ItemName);
                      }
                      this.GetAddBarcodeList = [];
                      this.ToggleCalculation = true;
                      this._addBarcodeService.SendBarcodeDataToSalesComp(null);
                      this.markFormGroupDirty(this.SalesForm);
                    }
                  },
                  (err) => {
                    $('#addBarcode').modal('hide');
                  }
                )
              }
            }
          }
        }
        $('#addBarcode').modal('hide');
      });

    //ends here


    this.Unsubscribe_obj = this._estimationService.SubjectOrderNo.subscribe(
      response => {
        this.OrderNoFromEstComp = response;
        if (this.OrderNoFromEstComp != 0) {
          this.GetBarcodeList = []; //Added on 18-Mar-2021
          this.SalesLinesPost.salesEstimatonVM = this.GetBarcodeList; //Added on 18-Mar-2021
          this._estimationService.getEstimationOrderDetailsfromAPI(this.OrderNoFromEstComp).subscribe(
            data => {
              this.arrayObj = data;
              this.SalesLinesPost.OrderNo = this.arrayObj.OrderNo;
              this.SalesLinesPost.Pos = this.arrayObj.Pos;
              this.SalesLinesPost.OrderAmount = this.arrayObj.OrderAmount;
              this.SalesLinesPost.RowRevisionString = this.arrayObj.RowRevisionString;
              this.SalesLinesPost.Overwrite = false;
              if (this.arrayObj.salesEstimatonVM != null) {
                this.GetBarcodeList = this.arrayObj.salesEstimatonVM;
                this.SalesLinesPost.salesEstimatonVM = this.GetBarcodeList;
                this.ToggleCalculation = true;
                this.SendSalesDataToEstComp();
                if (this.SalesLinesPost.salesEstimatonVM.length > 0) {
                  for (let i = 0; i < this.SalesLinesPost.salesEstimatonVM.length; i++) {
                    this.getGstPercent(this.SalesLinesPost.salesEstimatonVM[i].GsCode, this.SalesLinesPost.salesEstimatonVM[i].ItemName);
                  }
                }
              }
              else {
                this.GetBarcodeList = []; //Added on 18-Mar-2021
                this.SalesLinesPost.salesEstimatonVM = this.GetBarcodeList; //Added on 18-Mar-2021
              }
            }
          )
        }
      });


    this.Unsubscribe_obj = this._orderservice.castReservedOrderDetsToSalesComp.subscribe(
      response => {
        this.GetReservedBarcodeList = response;
        if (this.isEmptyObject(this.GetReservedBarcodeList) == false && this.isEmptyObject(this.GetReservedBarcodeList) != null) {
          for (let i = 0; i < this.GetReservedBarcodeList.length; i++) {
            this.GetBarcodeList.push(this.GetReservedBarcodeList[i]);
            this.removeDuplicateReservedOrders(this.GetReservedBarcodeList[i])
            this.SalesLinesPost.salesEstimatonVM.push(this.GetReservedBarcodeList[i]);
            this.removeDuplicateSalesEstimatonVMByReserOrder(this.GetReservedBarcodeList[i]);
            // this.submitted = false;
            // if (this.SalesForm.dirty) {
            //   this.EnableSubmitButton = true;
            //   return;
            // }
          }
        }
      }
    );





    this.getStateList();
    this.getSalesMan();
    this.getGSList();
    this.errors = [];
    this.modelErrors = [];
    this.hasErrors = false;
    if (this._router.url == "/sales-billing") {
      this.EnableDisablectrls = false;
    }
    if (this._router.url == "/estimation") {
      this.EnableDisablectrls = true;
    }
    this.Unsubscribe_obj = this._estimationService.castCoinOfferDatatoSales.subscribe(
      response => {
        this.CoinOfferBarcodeList = response;
        if (this.isEmptyObject(this.CoinOfferBarcodeList) == false && this.isEmptyObject(this.CoinOfferBarcodeList) != null) {
          if (this.CoinOfferBarcodeList.BarcodeNo != null) {
            this.getBarcodeDetails(this.CoinOfferBarcodeList.BarcodeNo, "", 1);
          }
          this.SalesLinesPost.DiscountAmount = this.CoinOfferBarcodeList.DisAmount;
          this.SalesLinesPost.IsOfferApplied = true;
          this.SalesLinesPost.IsOfferSkipped = false;
        }
      }
    )
  }
  DuplicateReservedobject:any;
  removeDuplicateReservedOrders(arg) {
    for (let obj of arg) {
      for (let ele of this.GetBarcodeList) {
        if (obj == ele) {
          continue;
        }
        if (obj.BarcodeNo != "") {
          if (ele.BarcodeNo === obj.BarcodeNo) {
            this.DuplicateReservedobject = obj;
            break;
          }
        }
      }
    }
  }
  DuplicateSalesLineobject:any;
  removeDuplicateSalesEstimatonVMByReserOrder(arg) {
    for (let obj of  arg) {
      for (let ele of  this.SalesLinesPost.salesEstimatonVM) {
        if (obj == ele) {
          continue;
        }
        if (obj.BarcodeNo != "") {
          if (ele.BarcodeNo === obj.BarcodeNo) {
            this.DuplicateSalesLineobject = obj;
            break;
          }
        }
      }
    }
  }
  RemoveReservredOrder(){

  }



  ItemName: any;
  getItem() {
    this._masterService.getItemName(this.SalesGSForm.value.GS).subscribe(
      response => {
        this.ItemName = response;
      }
    )
  }

  StateList: any;
  CompanyMaster: any;
  interState: number = 0;
  getStateList() {
    this._CustomerService.getStateCode().subscribe(
      Response => {
        this.StateList = Response;
        this._masterService.getCompanyMaster().subscribe(
          Response => {
            this.CompanyMaster = Response;
            this.SalesLinesPost.Pos = this.CompanyMaster.StateCode;
          }
        )
      }
    );
  }

  SetInterState(arg) {
    this.EnableSubmitButton = false;
    if (this.GetBarcodeList != null) {
      if (arg != this.CompanyMaster.StateCode) {
        this.EnablePos = true;
        for (let i = 0; i < this.GetBarcodeList.length; i++) {
          this.GetBarcodeList[i].interState = 1;
          this.GetBarcodeList[i].isInterstate = 1;
          this.GetBarcodeList[i].IGSTAmount = Math.abs(<number>this.GetBarcodeList[i].ItemTotalAfterDiscount * this.IGST).toFixed(2);
          this.GetBarcodeList[i].IGSTPercent = 3;
          this.GetBarcodeList[i].SGSTAmount = 0.00;
          this.GetBarcodeList[i].CGSTAmount = 0.00;
          this.GetBarcodeList[i].CGSTPercent = 0;
          this.GetBarcodeList[i].SGSTPercent = 0;
        }
      }
      else {
        this.EnablePos = true;
        for (let i = 0; i < this.GetBarcodeList.length; i++) {
          this.GetBarcodeList[i].interState = 0;
          this.GetBarcodeList[i].isInterstate = 0;
          this.GetBarcodeList[i].SGSTAmount = Math.abs(<number>this.GetBarcodeList[i].ItemTotalAfterDiscount * this.GST).toFixed(2);
          this.GetBarcodeList[i].CGSTAmount = Math.abs(<number>this.GetBarcodeList[i].ItemTotalAfterDiscount * this.GST).toFixed(2);
          this.GetBarcodeList[i].CGSTPercent = 1.5;
          this.GetBarcodeList[i].SGSTPercent = 1.5;
          this.GetBarcodeList[i].IGSTPercent = 0;
          this.GetBarcodeList[i].IGSTAmount = 0.00;
        }
      }
    }
    if (arg != this.CompanyMaster.StateCode) {
      this.interState = 1;
    }
    else {
      this.interState = 0;
    }
  }

  SalesPersonModel: any = {
    CustID: null,
    salesPerson: null,
    barcode: null,
    GS: null,
    ItemName: null
  }

  ReprintData: any = {
    EstNo: null,
    EstType: null,
  }

  // convenience getter for easy access to form fields
  get f() { return this.SalesForm.controls; }

  SalesManList: any;
  getSalesMan() {
    this._masterService.getSalesMan().subscribe(
      response => {
        this.SalesManList = response;
        //this.SalesPersonModel.salesPerson = "A";
      })
  }

  MCList: any;
  MCNameList: any;
  getMc() {
    this._masterService.getMCTypes().subscribe(
      response => {
        this.MCList = response;
      }
    );
  }

  GSList: any;
  getGSList() {
    this._masterService.getGsList('S').subscribe(
      response => {
        this.GSList = response;
      }
    )
  }

  getDetails(form) {
    if (form.value.salesPerson == null) {
      swal("Warning!", 'Please select Sales Person', "warning");
    }
    else if (form.value.barcode == null || form.value.barcode == "") {
      swal("Warning!", 'Please enter barcode', "warning");
    }
    else if (form.value.barcode != null && form.value.barcode != "") {
      if (this.AlphaNumericValidations(form.value.barcode) == false) {
        swal("Warning!", 'Please enter valid Barcode No', "warning");
      }
      else {
        this.addBarCode(form);
        this.SalesPersonModel.barcode = '';
        this.getGSList();
        this.getMc();
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


  ngAfterContentChecked() {
    this.PwdAddBarcode.nativeElement.focus();
    this.PwdInterState.nativeElement.focus();
  }

  AddBarcode(form) {
    if (form.value.salesPerson == null) {
      swal("Warning!", 'Please select Sales Person', "warning");
    }
    else {
      // this._salesService.SendBarcodeNoToAddBarcode("");
      this.EnableSubmitButton = false;
      this._addBarcodeService.SendInterStateDataToAddBarCodeComp(this.SalesLinesPost);
      this._addBarcodeService.SendGetSalCodeToAddBarCodeComp(this.SalesPersonModel.salesPerson);
      if (form.value.barcode == null || form.value.barcode == "") {
        this.PwdAddBarcode.nativeElement.value = "";
        $('#PermissonModalAddbarcode').modal('show');
        // this.Pwd1.nativeElement.focus();
        // $('#addBarcode').modal('show');l
        // this._salesService.SendSalesBarcode("true");
        // this.EditMode = false; //Commented on 03-Nov-2021 as part of bug id:67(i.e. Barcode already added)
      }
      else {
        this.getDetails(form);
        $('#addBarcode').modal('hide');
        $('#PermissonModalAddbarcode').modal('hide');
      }
    }
  }

  addBarCode(arg) {
    if (this.GetBarcodeList != null) {
      arg.value.barcode = arg.value.barcode.trim();
      let data = this.GetBarcodeList.find(x => x.BarcodeNo == arg.value.barcode);
      if (data == null) {
        this.getBarcodeDetails(arg.value.barcode, arg.value.salesPerson, 0);
        //this._salesService.SendBarcodeNoToAddBarcode(arg.value.barcode);
        this.SalesPersonModel.barcode = '';
      }
      else {
        if (this.GetBarcodeList.length >= 1) {
          swal("Warning!", "Barcode number already added", "warning");
        }
      }
    }
    else {
      this.GetBarcodeList = [];
      this.getBarcodeDetails(arg.value.barcode, arg.value.salesPerson, 0);
      this.SalesPersonModel.barcode = '';
    }
  }


  validateDiffMetal() {

  }


  getOrderBarcodeDetails(OrderNo) {
    this._salesService.getOrderEst(OrderNo).subscribe(
      response => {
        this.OrderList.push(response);
        for (let i = 0; i < this.OrderList.length; i++) {
          this.ArrayList.push(this.OrderList[0][i]);
        }
        this.GetBarcodeList.push(this.ArrayList[0]);
        if (this.SalesEstNo == null) {
          this.SalesLinesPost.salesEstimatonVM.push(this.ArrayList[0]);
          this.SalesLinesPost.GSType = this.ArrayList[0].GsCode;
        }
        this.ToggleCalculation = true;
        this.ArrayList = [];
        this.OrderList = [];
      },
      (err) => {
        if (err.status === 400) {
          const validationError = err.error;
          swal("Warning!", validationError.description, "warning");
        }
        else {
          this.errors.push('something went wrong!');
        }
      }
    )
  }

  OrderNo: Number = 0;
  DiffMetalCheck: any = [];

  salesPerson: string = "";


  closeGroupBarcode() {
    this.ArrayList = [];
    this.DiffMetalCheck = [];
    this.GroupBarcodeDets = null;
    this.groupBarcodingModel = {
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
      TaggingType: null,
      Quantity: null,
      BillingGrossWt: null,
      BillingStoneWt: null,
      BillingNetWt: null,
      salesEstStoneVM: []
    }
  }



  getBarcodeDetails(barcode, salePerson, isOfferCoin) {
    this._estimationService.SubjectOrderNo.subscribe(
      response => {
        this.OrderNo = response;
        if (this.OrderNo != 0) {
          this.SalesEstNo = null;
          this.SalesLinesPost.OrderNo = this.OrderNo;
        }
      });

    this.salesPerson = salePerson;

    this._salesService.getBarcodefromAPI(barcode, this.OrderNo, this.interState, isOfferCoin).subscribe(
      response => {
        //this.ArrayList.push(response);
        this.DiffMetalCheck.push(response);
        if (this.DiffMetalCheck[0].TaggingType == "G") {
          $('#GroupBarcodingModal').modal('show');
          this.groupBarcodingModel = this.DiffMetalCheck[0];
        }
        else {
          $('#GroupBarcodingModal').modal('hide');
          this._salesService.BarcodeValidation(this.SalesLinesPost, barcode, this.DiffMetalCheck[0].GsCode).subscribe(
            response => {
              // if (this.SalesLinesPost.GSType != "") {
              this.ArrayList = this.DiffMetalCheck;
              this.ArrayList[0].SalCode = this.salesPerson;
              this.getGstPercent(this.ArrayList[0].GsCode, this.ArrayList[0].ItemName);
              this.GetBarcodeList.push(this.ArrayList[0]);
              //if (this.SalesEstNo == null ) {
              //this.SalesLinesPost.salesEstimatonVM.push(this.ArrayList[0]); //Commented on 24-02-2021 for duplications
              this.SalesLinesPost.salesEstimatonVM = []; //Added on 24-02-2021 for duplications
              this.SalesLinesPost.salesEstimatonVM = this.GetBarcodeList; //Added on 24-02-2021 for duplications                
              //}
              this.SalesLinesPost.GSType = this.ArrayList[0].GsCode;
              this.ToggleCalculation = true;
              // }
              // else {
              //   this.ArrayList = this.DiffMetalCheck;
              //   this.ArrayList[0].SalCode = salesPerson;
              //   this.getGstPercent(this.ArrayList[0].GsCode, this.ArrayList[0].ItemName);
              //   this.GetBarcodeList.push(this.ArrayList[0]);
              //   if (this.SalesEstNo == null) {
              //     this.SalesLinesPost.salesEstimatonVM.push(this.ArrayList[0]);
              //   }
              //   this.SalesLinesPost.GSType = this.ArrayList[0].GsCode;
              //   this.ToggleCalculation = true;
              // }
              this.SendSalesDataToEstComp();
              this.ArrayList = [];
              this.DiffMetalCheck = [];
            },
            (err) => {
              this.ArrayList = [];
              this.DiffMetalCheck = [];
            }
          )
        }
      },
      (err) => {
        if (err.status === 400) {
          const validationError = err.error;
        }
        else {
          this.errors.push('something went wrong!');
        }
        this.ArrayList = [];
        this.DiffMetalCheck = [];
      }
    )
  }


  modalPopUPBarcode: any;
  Toggle: boolean = false;
  ToggleCalculation: boolean = false;

  GetBarcodeInfo(arg) {
    this.Toggle = true;
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
    this.getMCTypes(this.modalPopUPBarcode.McType);
  }

  MCTypeList: any;
  McType: any;

  getMCTypes(arg) {
    this._orderservice.getMCTypes().subscribe(
      response => {
        this.MCTypeList = response;
        this.McType = this.MCTypeList.filter(value => value.MC_ID === arg);
        this.McType = this.McType[0].MC_NAME;
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
    if (this.SalesForm.pristine) {
      this.EnableSubmitButton = false;
      return;
    }
  }

  IndexNo: number;
  EditMode: boolean = false;


  EditTagRow(index: number, arg) {
    if (arg.TaggingType == "G") {
      swal("Warning!", "Group Barcode cannot be edited", "warning");
    }
    else {
      if (arg.IsEDApplicable == "" || arg.IsEDApplicable == null) {
        this.IndexNo = index;
        this._salesService.SendSalesDataToAddBarcode(null);
        this._salesService.SendSalesDataToAddBarcode(arg);
        this._estimationService.SendOrderNoToSalesComp(this.SalesLinesPost.OrderNo);
        $('#addBarcode').modal('show');
        this.EditMode = true;
        this.EnableSubmitButton = false;
      }
      else {
        swal("Warning!", "Offer coins cannot be edited", "warning");
      }
    }
  }

 
  Submit() {
    let estNo;
    // if (this.EnableSubmitButton == true) {
    //   swal("Warning!", "No changes were made to submit", "warning");
    // }
    if (this.SalesForm.pristine == true) {
      swal("Warning!", "No changes were made to submit", "warning");
    }
    else if (this.SalesLinesPost.Pos == null) {
      swal("Warning!", 'Please select Place of Supply', "warning");
    }
    else if (this.SalesLinesPost.salesEstimatonVM.length <= 0) {
      swal("Warning!", 'Please enter Sales Details', "warning");
    }
    else {
      this.Duplicateobject = null;
      this.removeDuplicates();
      if (this.Duplicateobject != null) {
        swal("Warning!", 'Please remove the duplicate barcode', "warning");
      }
      else {
        this.pos = this.SalesLinesPost.Pos;
        var ans = confirm("Do you want to save??");
        if (ans) {
          this.SendSalesDataToEstComp();
          if (this.SalesLinesPost.EstNo == null || this.SalesLinesPost.EstNo == 0) {
            this._salesService.post(this.SalesLinesPost).subscribe(
              response => {
                estNo = response;
                swal("Saved!", "Estimation number " + estNo.EstMationNo + " Created", "success");
                this._salesService.SendEstNo_To_Purchase(estNo.EstMationNo);
                this._estimationService.SendEstNo(estNo.EstMationNo);
                this._salesService.salesDetails(this.SalesLinesPost);
                this.SalesLinesPost.EstNo = estNo.EstMationNo;
                this.ReprintData = {
                  EstNo: estNo.EstMationNo,
                  EstType: "S"
                }
                this._estimationService.SendReprintData(this.ReprintData);
                this.EnablePos = true;
                this.markFormGroupPristine(this.SalesForm);
              },
              (err) => {
                if (err.status === 403) {
                  const validationError = err.error;
                  var ans = confirm("Estimation is Eligible for Free Gift. Do You Want to Issue??");
                  if (ans) {
                    this._estimationService.SendTotalCoinOffer(validationError.Value);
                  }
                  else {
                    var ans = confirm("Do you want to save Sales Estimation??");
                    if (ans) {
                      this.SalesLinesPost.IsOfferApplied = false;
                      this.SalesLinesPost.IsOfferSkipped = true;
                      this.SendSalesDataToEstComp();
                      if (this.SalesLinesPost.EstNo == null || this.SalesLinesPost.EstNo == 0) {
                        this._salesService.post(this.SalesLinesPost).subscribe(
                          response => {
                            estNo = response;
                            swal("Saved!", "Estimation number " + estNo.EstMationNo + " Created", "success");
                            this._salesService.SendEstNo_To_Purchase(estNo.EstMationNo);
                            this._estimationService.SendEstNo(estNo.EstMationNo);
                            this._salesService.salesDetails(this.SalesLinesPost);
                            this.SalesLinesPost.EstNo = estNo.EstMationNo;
                            this.ReprintData = {
                              EstNo: estNo.EstMationNo,
                              EstType: "S"
                            }
                            this._estimationService.SendReprintData(this.ReprintData);
                            this.EnablePos = true;
                            this.markFormGroupPristine(this.SalesForm);
                          })
                      }
                    }
                  }
                }
              }
            )
          }
          else {
            this._salesService.put(this.SalesLinesPost, this.SalesLinesPost.EstNo).subscribe(
              response => {
                estNo = response;
                swal("Updated!", "Estimation number " + estNo.EstMationNo + " Updated", "success");
                this._salesService.SendEstNo_To_Purchase(estNo.EstMationNo);
                this._estimationService.SendEstNo(estNo.EstMationNo);
                this._salesService.salesDetails(this.SalesLinesPost);
                this.ReprintData = {
                  EstNo: estNo.EstMationNo,
                  EstType: "S"
                }
                this._estimationService.SendReprintData(this.ReprintData);
                this.markFormGroupPristine(this.SalesForm);
              },
              (err) => {
                if (err.status === 403) {
                  const validationError = err.error;
                  var ans = confirm("Estimation is Eligible for Free Gift. Do You Want to Issue??");
                  if (ans) {
                    this._estimationService.SendTotalCoinOffer(validationError.Value);
                  }
                  else {
                    var ans = confirm("Do you want to save Sales Estimation??");
                    if (ans) {
                      this.SalesLinesPost.IsOfferApplied = false;
                      this.SalesLinesPost.IsOfferSkipped = true;
                      this.SendSalesDataToEstComp();
                      this._salesService.put(this.SalesLinesPost, this.SalesLinesPost.EstNo).subscribe(
                        response => {
                          estNo = response;
                          swal("Updated!", "Estimation number " + estNo.EstMationNo + " Updated", "success");
                          this._salesService.SendEstNo_To_Purchase(estNo.EstMationNo);
                          this._estimationService.SendEstNo(estNo.EstMationNo);
                          this._salesService.salesDetails(this.SalesLinesPost);
                          this.ReprintData = {
                            EstNo: estNo.EstMationNo,
                            EstType: "S"
                          }
                          this._estimationService.SendReprintData(this.ReprintData);
                          this.markFormGroupPristine(this.SalesForm);
                        }
                      )
                    }
                  }
                }
                else if (err.status === 400) {
                  const validationErrorForBadRequest = err.error;
                  if (validationErrorForBadRequest.description == "Somebody altered the Estimation, Do want to overwrite?") {
                    var ans = confirm(validationErrorForBadRequest.description);
                    if (ans) {
                      //Put to be handled with boolean true
                      this.SalesLinesPost.IsOfferApplied = false;
                      this.SalesLinesPost.IsOfferSkipped = true;
                      this.SalesLinesPost.Overwrite = true;
                      this.SendSalesDataToEstComp();
                      this._salesService.put(this.SalesLinesPost, this.SalesLinesPost.EstNo).subscribe(
                        response => {
                          estNo = response;
                          swal("Updated!", "Estimation number " + estNo.EstMationNo + " Updated", "success");
                          this._salesService.SendEstNo_To_Purchase(estNo.EstMationNo);
                          this._estimationService.SendEstNo(estNo.EstMationNo);
                          this._salesService.salesDetails(this.SalesLinesPost);
                          this.ReprintData = {
                            EstNo: estNo.EstMationNo,
                            EstType: "S"
                          }
                          this._estimationService.SendReprintData(this.ReprintData);
                          this.markFormGroupPristine(this.SalesForm);
                        }
                      )
                    }
                    else {
                      var ans = confirm("Do you want to reload the Estimation??");
                      if (ans) {
                        //reload the estimation with boolean false
                        this.SalesLinesPost.IsOfferApplied = false;
                        this.SalesLinesPost.IsOfferSkipped = true;
                        this._estimationService.getEstimationDetailsfromAPI(this.SalesLinesPost.EstNo).subscribe(
                          response => {
                            this.arrayObj = response;
                            if (this.arrayObj != null) {
                              this.EnableSubmitButton = true;
                              this.SalesLinesPost.Pos = this.arrayObj.Pos;
                              this.SalesPersonModel.salesPerson = this.arrayObj.salesEstimatonVM[0].SalCode;//Added on 22-Jun-2020
                              this.SalesLinesPost.GSType = this.arrayObj.GSType;
                              this.SalesLinesPost.OrderNo = this.arrayObj.OrderNo;
                              this.SalesLinesPost.EstNo = this.SalesEstNo;
                              this.SalesLinesPost.OrderAmount = this.arrayObj.OrderAmount;
                              this.SalesLinesPost.DiscountAmount = this.arrayObj.DiscountAmount;
                              this.SalesLinesPost.RowRevisionString = this.arrayObj.RowRevisionString;
                              this.SalesLinesPost.Overwrite = this.arrayObj.RowRevisionString;
                              this.GetBarcodeList = this.arrayObj.salesEstimatonVM;
                              this.SalesLinesPost.salesEstimatonVM = this.arrayObj.salesEstimatonVM;
                              this.ToggleCalculation = true;
                              this.SalesLinesPost.offerDiscount = this.arrayObj.offerDiscount;
                              this.SendSalesDataToEstComp();
                              for (let i = 0; i < this.SalesLinesPost.salesEstimatonVM.length; i++) {
                                if (this.SalesLinesPost.salesEstimatonVM[i].BarcodeNo == "") {
                                  this.getGstPercent(this.SalesLinesPost.salesEstimatonVM[i].GsCode, this.SalesLinesPost.salesEstimatonVM[i].ItemName);
                                }
                              }
                              this.markFormGroupPristine(this.SalesForm);
                            }
                          }
                        )
                      }
                    }
                  }
                }
              }
            )
          }
        }
      }
    }
  }


  //2022 offer implementation
  OfferResponse: any = [];
  ApplyFab2022Offer() {
    this.SalesLinesPost.IsOfferApplied = true;
    this.OfferModel2022 = {
      CompanyCode: "",
      BranchCode: "",
      EstimationNo: 0,
      Date: "",
      Rate: null,
      SalesEstDetail: []
    }
    this.OfferModel2022.CompanyCode = this.ccode;
    this.OfferModel2022.BranchCode = this.bcode;
    this.OfferModel2022.EstimationNo = Number(this.SalesLinesPost.EstNo);
    this.OfferModel2022.Date = "2022/19/01" //aplication date/estimation date
    this.OfferModel2022.Rate = 4255;
    if (this.SalesLinesPost.salesEstimatonVM.length > 0) {
      for (let i of this.SalesLinesPost.salesEstimatonVM) {
        this.OfferModel2022.Rate = i.Rate;
        this.OfferModel2022LineItems.SlNo = i.SlNo;
        this.OfferModel2022LineItems.GSCode = i.GsCode;
        this.OfferModel2022LineItems.BarcodeNo = i.BarcodeNo;
        this.OfferModel2022LineItems.ItemCode = i.ItemName;
        this.OfferModel2022LineItems.SalesmanCode = i.SalCode;
        this.OfferModel2022LineItems.CounterCode = i.CounterCode;
        this.OfferModel2022LineItems.MRPItem = true;
        this.OfferModel2022LineItems.Qty = i.ItemQty;
        this.OfferModel2022LineItems.Grosswt = i.Grosswt;
        this.OfferModel2022LineItems.Stonewt = i.Stonewt;
        this.OfferModel2022LineItems.Netwt = i.Netwt;
        this.OfferModel2022LineItems.MetalValue = i.GoldValue;
        this.OfferModel2022LineItems.VAAmount = i.VaAmount;
        this.OfferModel2022LineItems.StoneCharges = i.StoneCharges;
        this.OfferModel2022LineItems.DiamondCharges = i.DiamondCharges;
        this.OfferModel2022LineItems.ItemAmount = i.TotalAmount;
        if (i.salesEstStoneVM.length > 0) {
          for (let j of i.salesEstStoneVM) {
            let carat = j.Carat;
            this.OfferModel2022LineItems.Dcts = j.Carat;

          }
        }
        this.OfferModel2022.SalesEstDetail.push(this.OfferModel2022LineItems);
        this.OfferModel2022LineItems = {
          SlNo: 0,
          GSCode: "",
          BarcodeNo: "",
          ItemCode: "",
          SalesmanCode: "",
          CounterCode: "",
          MRPItem: true,
          Qty: 0,
          Grosswt: 0,
          Stonewt: 0,
          Netwt: 0,
          MetalValue: 0,
          VAAmount: 0,
          StoneCharges: 0,
          DiamondCharges: 0,
          Dcts: 0,
          ItemAmount: 0
        };
        this._salesService.apply2022Offer(this.OfferModel2022).subscribe(
          response => {
            this.OfferResponse = response;
            this.SalesForm.pristine == false;
            this.SalesLinesPost.DiscountAmount = this.OfferResponse.DiscountAmount;
            this.SalesForm.pristine == true;
            swal("Success!", this.OfferResponse.OfferName, "success");
          }
        )
      }

    }

  }
  RemoveFab2022Offer() {
    
  }
  private markFormGroupPristine(form: FormGroup) {
    Object.values(form.controls).forEach(control => {
      control.markAsPristine();

      if ((control as any).controls) {
        this.markFormGroupPristine(control as FormGroup);
      }
    });
  }


  private markFormGroupDirty(form: FormGroup) {
    Object.values(form.controls).forEach(control => {
      control.markAsDirty();

      if ((control as any).controls) {
        this.markFormGroupDirty(control as FormGroup);
      }
    });
  }

  ngOnDestroy() {
    this._salesService.SaveSalesEstNo(null);
    this._CustomerService.SendCustDataToEstComp(null);
    this._salesService.salesDetails(null);
  }

  SalesAmount: number = 0;
  SalesTotalAmount: number = 0;
  @Output() SalesAmountChange = new EventEmitter();
  @Output() DelCoinOffer = new EventEmitter();
  @Output() SalesTaxableAmountChange = new EventEmitter();
  @Output() SalesTotalItemsChange = new EventEmitter();
  @Output() SalesGrossWtChange = new EventEmitter();
  @Output() SalesNetWtChange = new EventEmitter();

  valueChanged(Amount) { // You can give any function name
    this.SalesAmount = Amount;
    this.valueChange.emit(this.SalesAmount);
  }

  SalesAmountChanged(SalesAmount) {
    this.SalesTotalAmount = SalesAmount;
    this.SalesAmountChange.emit(this.SalesTotalAmount);
  }

  DeleteDiscountCoinOffer(arg) {
    this.DelCoinOffer.emit(arg);
  }


  Duplicateobject: any;

  removeDuplicates() {
    for (let obj of this.SalesLinesPost.salesEstimatonVM) {
      for (let ele of this.SalesLinesPost.salesEstimatonVM) {
        if (obj == ele) {
          continue;
        }

        if (obj.BarcodeNo != "") {
          if (ele.BarcodeNo === obj.BarcodeNo) {
            this.Duplicateobject = obj;
            break;
          }
        }
      }
    }
  }

  checkCoinOffer: any;

  DeleteTagRow(index: number, arg) {
    if (this.GetBarcodeList[index].IsEDApplicable != "" && this.GetBarcodeList[index].IsEDApplicable != null) {
      var ans = confirm("Do you want to delete");
      if (ans) {
        var ans = confirm("All gift items are removed,Re-issue the gift items.Do you want to continue?");
        if (ans) {

          this.DeleteDiscountCoinOffer(1);
          this.GetBarcodeList.splice(index, 1);
          this.SalesLinesPost.salesEstimatonVM = [];
          this.SalesLinesPost.salesEstimatonVM = this.GetBarcodeList;
          //this.SalesLinesPost.salesEstimatonVM.splice(index, 1);
          this.ClearBarcodeDets();
          this.SalesLinesPost.IsOfferApplied = false;
          this.SalesLinesPost.IsOfferSkipped = true;
          //this.EnableDisableSubmitBtn();
          this.EnableSubmitButton = false;
          this.GetAddBarcodeList = [];
        }
      }
    }
    else {
      var ans = confirm("Do you want to delete");
      if (ans) {
        this.EnablePos = true;
        this.DeleteDiscountCoinOffer(0);
        this.valueChanged(this.GetBarcodeList[index].ItemFinalAmount);
        this.GetBarcodeList.splice(index, 1);
        this.SalesLinesPost.salesEstimatonVM = [];
        this.SalesLinesPost.salesEstimatonVM = this.GetBarcodeList;
        //this.SalesLinesPost.salesEstimatonVM.splice(index, 1);
        this.ClearBarcodeDets();
        //this.EnableDisableSubmitBtn();
        this.EnableSubmitButton = false;
        this.markFormGroupDirty(this.SalesForm);
        this.GetAddBarcodeList = [];
      }
    }
  }

  ClearBarcodeDets() {
    if (this.GetBarcodeList.length <= 0) {
      this.SalesLinesPost.GSType = "";
      this.GetBarcodeList = [];
    }
  }

  total_items(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      total += parseInt(d.ItemQty);
    });
    this.SalesSummaryData.TotalItems = total;
    this.SalesTotalItemsChange.emit(total);
    return total;
  }

  grossWT(arg: any[] = []) {
    let total = 0; arg.forEach((d) => {
      if (d.VaporLossWeight != null) {
        total += d.Grosswt + d.AddWt - d.DeductWt - d.VaporLossWeight;
      }
      else {
        total += d.Grosswt + d.AddWt - d.DeductWt;
      }
    });
    this.SalesSummaryData.Grwt = total;
    this.SalesGrossWtChange.emit(total);
    return total;
  }

  netWT(arg: any[] = []) {
    let total = 0; arg.forEach((d) => {
      if (d.VaporLossWeight != null) {
        total += d.Grosswt + d.AddWt - d.Stonewt - d.DeductWt - d.VaporLossWeight;
      }
      else {
        total += d.Grosswt + d.AddWt - d.Stonewt - d.DeductWt;
      }
    });
    this.SalesSummaryData.NtWt = total;
    this.SalesNetWtChange.emit(total);
    return total;
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

  Rate(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      total += d.Rate;
    });
    return total;
  }

  metal_amount(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      if (d.IsEDApplicable == "" || d.IsEDApplicable == null) {
        total += d.GoldValue;
      }
    });
    return total;
  }

  stone_amount(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      if (d.IsEDApplicable == "" || d.IsEDApplicable == null) {
        total += d.StoneCharges;
      }
    });
    return total;
  }

  diamond_amount(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      if (d.IsEDApplicable == "" || d.IsEDApplicable == null) {
        total += d.DiamondCharges;
      }
    });
    return total;
  }

  va_amount(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      if (d.IsEDApplicable == "" || d.IsEDApplicable == null) {
        total += d.VaAmount;
      }
    });
    return total;
  }

  taxable_amount(arg: any[] = []) {
    let total = 0; arg.forEach((d) => {
      if (d.IsEDApplicable == "" || d.IsEDApplicable == null) {
        total += d.ItemTotalAfterDiscount;
      }
    });
    this.SalesSummaryData.taxable_Amt = total;
    this.SalesTaxableAmountChange.emit(total);
    return total;
  }

  SGST_amount(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      if (d.SGSTAmount != null && d.SGSTAmount != 0) {
        if (d.IsEDApplicable == "" || d.IsEDApplicable == null) {
          total += Number(<number>d.SGSTAmount);
        }
      }
    });
    this.SalesSummaryData.SGst = total;
    return total;
  }

  CGST_amount(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      if (d.CGSTAmount != null && d.CGSTAmount != 0) {
        if (d.IsEDApplicable == "" || d.IsEDApplicable == null) {
          total += Number(<number>d.CGSTAmount);
        }
      }
    });
    this.SalesSummaryData.CGst = total;
    return total;
  }

  IGST_amount(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      if (d.IGSTAmount != null && d.IGSTAmount != 0) {
        if (d.IsEDApplicable == "" || d.IsEDApplicable == null) {
          total += Number(<number>d.IGSTAmount);
        }
      }
    });
    this.SalesSummaryData.IGst = total;
    return total;
  }

  SalesTotal_amount(arg: any[] = []) {
    let total = 0; arg.forEach((d) => {
      if (d.IsEDApplicable == "" || d.IsEDApplicable == null) {
        total += Number(<number>d.ItemFinalAmount)
      }
    }
    );
    this.SalesSummaryData.Amount = total;
    this.SalesAmountChanged(total);
    return total;
  }

  EnablePos: boolean = true;

  EditPos() {
    if (this.GetBarcodeList.length >= 1) {
      $('#PermissonModalPos').modal('hide');
      swal("Warning!", "Please delete the sales line Items to change the POS", "warning");
    }

    else {
      $('#PermissonModalPos').modal('show');
      this.PwdInterState.nativeElement.value = "";
      this.EnableSubmitButton = false;
    }
  }

  permissonModel: any = {
    CompanyCode: null,
    BranchCode: null,
    PermissionID: null,
    PermissionData: null
  }

  passWordPos(arg) {
    if (arg == "") {
      swal("Warning!", 'Please Enter the Password', "warning");
      $('#PermissonModalPos').modal('show');
      this.EnablePos = true;
    }
    else {
      this.permissonModel.CompanyCode = this.ccode;
      this.permissonModel.BranchCode = this.bcode;
      this.permissonModel.PermissionID = this.PlaceOfSupplyEdit;
      this.permissonModel.PermissionData = btoa(arg);
      this._masterService.postelevatedpermission(this.permissonModel).subscribe(
        response => {
          this.EnablePos = false;
          $('#PermissonModalPos').modal('hide');
        },
        (err) => {
          if (err.status === 401) {
            $('#PermissonModalPos').modal('show');
            this.EnablePos = true;
          }
        }
      )
    }
  }

  EnableAddBarcode: boolean = true;

  passWordAddbarcode(arg) {
    if (arg == "") {
      swal("Warning!", 'Please Enter the Password', "warning");
      $('#PermissonModalAddbarcode').modal('show');
      this.EnableAddBarcode = true;
    }
    else {
      this.permissonModel.CompanyCode = this.ccode;
      this.permissonModel.BranchCode = this.bcode;
      this.permissonModel.PermissionID = this.AddBarcodePermission;
      this.permissonModel.PermissionData = btoa(arg);
      this._masterService.postelevatedpermission(this.permissonModel).subscribe(
        response => {
          this.EnableAddBarcode = false;
          $('#PermissonModalAddbarcode').modal('hide');
          $('#addBarcode').modal('show');
          this._salesService.SendSalesBarcode("true");
          this.EditMode = false;
          this.PwdAddBarcode.nativeElement.value = "";
        },
        (err) => {
          if (err.status === 401) {
            $('#PermissonModalAddbarcode').modal('show');
            this.EnableAddBarcode = true;
          }
        }
      )
    }
  }

  closePos() {
    $('#PermissonModalPos').modal('hide');
  }

  closeAddbarcode() {
    $('#PermissonModalAddbarcode').modal('hide');
  }

  GroupBarcodeDets: any;

  Save() {
    this._addBarcodeService.calculateBarcode(this.groupBarcodingModel).subscribe(
      response => {
        this.GroupBarcodeDets = response;
        this.ArrayList.push(this.GroupBarcodeDets);
        this.ArrayList[0].SalCode = this.salesPerson;
        this.getGstPercent(this.ArrayList[0].GsCode, this.ArrayList[0].ItemName);
        this.GetBarcodeList.push(this.ArrayList[0]);
        if (this.SalesEstNo == null) {
          //this.SalesLinesPost.salesEstimatonVM.push(this.ArrayList[0]);
          this.SalesLinesPost.salesEstimatonVM = [];
          this.SalesLinesPost.salesEstimatonVM = this.GetBarcodeList;
        }
        this.SalesLinesPost.GSType = this.ArrayList[0].GsCode;
        this.ToggleCalculation = true;
        this.SendSalesDataToEstComp();
        this.ArrayList = [];
        this.DiffMetalCheck = [];
        this.GroupBarcodeDets = null;
        $('#GroupBarcodingModal').modal('hide');
      }
    )
  }


  CalNtWt() {
    this.groupBarcodingModel.BillingNetWt = this.groupBarcodingModel.BillingGrossWt - this.groupBarcodingModel.BillingStoneWt;
  }

  DeleteReservedBarcode:any;

  DeleteReservedBarcodeFromAttachOrder(arg) {
    for (let obj of  this.SalesLinesPost.salesEstimatonVM) {
      for (let ele of  this.SalesLinesPost.salesEstimatonVM) {
        if (obj == ele) {
          continue;
        }
        if (obj.BarcodeNo != "") {
          if (ele.BarcodeNo === obj.BarcodeNo) {
            this.DeleteReservedBarcode = obj;
            break;
          }
        }
      }
    }
  }
  // DeleteReservedBarcodeFromAttachOrderFofSalesVM(arg) {
  //   for (let obj of  this.SalesLinesPost.salesEstimatonVM) {
  //     for (let ele of  this.SalesLinesPost.salesEstimatonVM) {
  //       if (obj == ele) {
  //         continue;
  //       }
  //       if (obj.BarcodeNo != "") {
  //         if (ele.BarcodeNo === obj.BarcodeNo) {
  //           this.DeleteReservedBarcode = obj;
  //           break;
  //         }
  //       }
  //     }
  //   }
  // }


}