import { OrderHeader, ItemDetailsLines } from './../orders.model';
import { Validators, FormGroup, FormBuilder, Validator } from '@angular/forms';
import { OrdersService } from './../orders.service';
import { CustomerService } from './../../masters/customer/customer.service';
import { Component, OnInit, OnDestroy, Input, ElementRef, AfterContentChecked } from '@angular/core';
import { formatDate } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { PaymentService } from '../../payment/payment.service';
import swal from 'sweetalert';
import * as CryptoJS from 'crypto-js';
import { MasterService } from './../../core/common/master.service';
import { Http } from '@angular/http';
import { ViewChild } from '@angular/core';
import { AppConfigService } from '../../AppConfigService';
import { DatePipe } from '@angular/common'
import { count } from 'rxjs/operators';
import { SalesService } from '../../sales/sales.service';
import { Router } from '@angular/router';
declare var $: any;


@Component({
  selector: 'app-itemdetails',
  templateUrl: './itemdetails.component.html',
  styleUrls: ['./itemdetails.component.css']
})
export class ItemdetailsComponent implements OnInit, OnDestroy, AfterContentChecked {
  OrdersHeaderForm: FormGroup;
  Rate: any = 0.00;
  GS: any;
  Karat: any;
  radioItems: Array<string>;
  EnableCustomerDetails: boolean = false;
  @Input()
  OrderType: string;
  readonly = {};
  enabledAppSwt: boolean = true;
  MCPer = {};
  MCPerPiece = {}
  MCPercent = {}
  AppSwt = {};
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;
  setWidth: boolean = false;
  EnableModifyOrderctrls: boolean = false;
  SalesGSForm: FormGroup;

  @ViewChild("PwdOrderRate", { static: true }) PwdOrderRate: ElementRef;

  @ViewChild('myInput', { static: true })
  myInputVariable: ElementRef;

  OrderTypeData = [
    {
      'name': 'Customised Order',
      'value': 'O'
    },
    {
      'name': ' Order Advance',
      'value': 'A'
    },
    {
      'name': ' Reserved Order',
      'value': 'R'
    },
  ]

  permissonModel: any = {
    CompanyCode: null,
    BranchCode: null,
    PermissionID: null,
    PermissionData: null
  }


  model = { option: 'Delivery Rate' };
  EnableDisResOrderControls: boolean = true;
  EnableDisCusOrderControls: boolean = true;
  EnableReadOnlyControls: boolean = false;
  OrderRateEditCode: string;
  orderNo: string = null;
  modifiedOrderNo: string = null;
  today = new Date();
  estDate = '';
  EnableJson: boolean = false;
  ordDate: string;
  reserveForm: FormGroup;
  datePickerConfig: Partial<BsDatepickerConfig>;
  constructor(private _customerService: CustomerService, private _orderservice: OrdersService,
    private fb: FormBuilder, private toastr: ToastrService,
    private _paymentService: PaymentService, private _masterService: MasterService,
    private _httpClient: Http, private appConfigService: AppConfigService, private datepipe: DatePipe,
    private _salesService: SalesService, private _router: Router) {
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.EnableJson = this.appConfigService.EnableJson;
    this.password = this.appConfigService.Pwd;
    this.OrderRateEditCode = this.appConfigService.RateEditCode.OrderRateEdit;
    this.radioItems = ['Delivery', 'Fixed', 'Flexi'];
    // this.ordDate = formatDate(this.today, 'yyyy-MM-dd', 'en-US', '+0530');
    this.OrderHeaderDetails.OperatorCode = localStorage.getItem('Login');
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        minDate: this.today,
        dateInputFormat: 'DD/MM/YYYY'
      });
    this.getCB();
    $('#myViewModal').modal('hide');
  }

  ngAfterContentChecked() {
    this.PwdOrderRate.nativeElement.focus();
  }
  ngOnInit() {
    this.OrdersHeaderForm = this.fb.group({
      GS: ['NGO', Validators.required],
      Karat: ['22K', Validators.required],
      OT: [null, Validators.required],
      SalCode: [null, Validators.required],
      OrderType: [null, Validators.required],
      ManagerCode: [null, Validators.required],
      BookingType: [null, Validators.required],
      AdvancePercent: [null, Validators.required],
      OrderRateType: ['null', Validators.required],
      OrderAdvanceRateType: ['null', Validators.required],
      Remarks: ['null', Validators.required],
      OrderDate: null,
      DeliveryDate: ['null', Validators.required],
      Rate: null,
    });
    this.getApplicationdate();
    this.getGSList();
    this.getKaratList();
    this.getSalesMan();
    this.getMCTypes();
    this.getCounter();
    this.getOrderItemType();
    //this.getOrderType(this.OrderType); //Commented on 21-JAN-2020

    this.getorderRateType("Delivery");

    this.CustDetails();
    this.reserveForm = this.fb.group({
      barcode: [null, Validators.required]
    });

    this.SalesGSForm = this.fb.group({
      GS: [null, Validators.required],
      ItemName: [null, Validators.required],
    });


    this.OrderHeaderDetails.OrderType = "O";
    this.OrderHeaderDetails.BookingType = "N";
    this.OrderHeaderDetails.OrderAdvanceRateType = "N";
    this.getAdvancePercent("N");
    this.toggle = "Customized Order";


    this.OrderHeaderDetails.OrderRateType = "Delivery";
    this._orderservice.castOrderNoFromViewOrderComp.subscribe(
      response => {
        this.orderNo = response;
        if (this.orderNo != null) {
          this.EnableReadOnlyControls = true;
          this._orderservice.castexistingOrderDetails.subscribe(
            response => {
              this.OrderHeaderDetails = response;
              if (this.OrderHeaderDetails != null) {
                this.OnRadioBtnChnge(this.OrderHeaderDetails.OrderRateType);
                this.EnableSubmitButton = true;
                if (this.OrderHeaderDetails.OrderType == "A") {
                  this.barcodeLines = [];
                  this.toggle = "OrderAdvance";
                  this.newAttribute.CounterCode = "NA";
                  this.newAttribute.ItemName = this.OrderHeaderDetails.lstOfOrderItemDetailsVM[0].ItemName;
                  this.newAttribute.Description = this.OrderHeaderDetails.lstOfOrderItemDetailsVM[0].Description;
                  this.newAttribute.GSCode = this.OrderHeaderDetails.GSCode;
                  this.newAttribute.SalCode = this.OrderHeaderDetails.SalCode;
                  this.barcodeLines.push(this.newAttribute);
                }
                else if (this.OrderHeaderDetails.OrderType == "R") {
                  this.toggle = "Reserved";
                  this.barcodeLines = this.OrderHeaderDetails.lstOfOrderItemDetailsVM;
                  this.EnableSubmitButton = true;
                  this.EnableDisResOrderControls = false;
                }
                else {
                  this.toggle = "Customized Order";
                  // this.getItemName();
                  this.fieldArray = this.OrderHeaderDetails.lstOfOrderItemDetailsVM;
                  this.EnableDisCusOrderControls = false;
                  this.EnableSubmitButton = true;
                  for (let i = 0; i < this.fieldArray.length; i++) {
                    this.readonly[i] = true;
                    this.MCPer[i] = true;
                    this.MCPerPiece[i] = true;
                    this.MCPercent[i] = true;
                    this.AppSwt[i] = true;
                    this._orderservice.getItemName(this.fieldArray[i].GSCode, this.fieldArray[i].CounterCode).subscribe(
                      response => {
                        this.ItemName[i] = response;
                      }
                    )
                  }
                }
              }
            }
          )
        }
      }
    )

    this._orderservice.castModifyOrderDetsToOrderComp.subscribe(
      response => {
        this.modifiedOrderNo = response;
        if (this.modifiedOrderNo != null) {
          this.EnableReadOnlyControls = true;
          this.EnableModifyOrderctrls = true;
          this._orderservice.castexistingOrderDetails.subscribe(
            response => {
              this.OrderHeaderDetails = response;
              console.log(  this.OrderHeaderDetails); 
              if (this.OrderHeaderDetails != null) {
                this.OnRadioBtnChnge(this.OrderHeaderDetails.OrderRateType);
                if (this.OrderHeaderDetails.OrderType == "A") {
                  this.barcodeLines = [];
                  this.toggle = "OrderAdvance";
                  this.newAttribute.CounterCode = "NA";
                  this.newAttribute.ItemName = this.OrderHeaderDetails.lstOfOrderItemDetailsVM[0].ItemName;
                  this.newAttribute.Description = this.OrderHeaderDetails.lstOfOrderItemDetailsVM[0].Description;
                  this.newAttribute.GSCode = this.OrderHeaderDetails.GSCode;
                  this.newAttribute.SalCode = this.OrderHeaderDetails.SalCode;
                  this.barcodeLines.push(this.newAttribute);
                }
                else if (this.OrderHeaderDetails.OrderType == "R") {
                  this.toggle = "Reserved";
                  this.barcodeLines = this.OrderHeaderDetails.lstOfOrderItemDetailsVM;
                  this.EnableSubmitButton = false;
                  this.EnableDisResOrderControls = false;
                }
                else {
                  this.toggle = "Customized Order";
                  // this.getItemName();
                  this.fieldArray = this.OrderHeaderDetails.lstOfOrderItemDetailsVM;
                  this.EnableDisCusOrderControls = false;
                  this.EnableSubmitButton = false;
                  for (let i = 0; i < this.fieldArray.length; i++) {
                    this.readonly[i] = true;
                    this.MCPer[i] = true;
                    this.MCPerPiece[i] = true;
                    this.MCPercent[i] = true;
                    this.AppSwt[i] = true;
                    this._orderservice.getItemName(this.fieldArray[i].GSCode, this.fieldArray[i].CounterCode).subscribe(
                      response => {
                        this.ItemName[i] = response;
                      }
                    )
                  }
                }
              }
            }
          )
        }
      }
    )
  }

  // downloadImage(downloadLink) {
  //   window.open(downloadLink,"_top");    
  // }

  getApplicationdate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.ordDate = appDate["applcationDate"]
        this.OrderHeaderDetails.OrderDate = this.ordDate;
        this.ordDate = formatDate(this.ordDate, 'dd/MM/yyyy', 'en_GB');
        this.OrderHeaderDetails.DeliveryDate = this.ordDate;
        this.OrderHeaderDetails.OrderDate = this.ordDate;
        this.getManagerList();
        this.getBookingTypeList();
      }
    )
  }

  ManagerList: any;
  BookingTypeList: any;
  getManagerList() {
    this._masterService.getManagerList().subscribe(
      response => {
        this.ManagerList = response;
      }
    )
  }

  getBookingTypeList() {
    this._masterService.getBookingTypeList().subscribe(
      response => {
        this.BookingTypeList = response;
      }
    )
  }

  MinimumWeightGrams: number = 0;

  SetRateValidityDays(arg) {
    let data = this.AdvancePercent.find(x => x.AdvanceAmountPercent == arg);
    if (data != null) {
      this.OrderHeaderDetails.RateValidityDays = data.FixedDays;
      this.OrderHeaderDetails.RateValidityTill = this.OrderHeaderDetails.OrderDate;
      this.MinimumWeightGrams = data.MinimumWeightGrams;
    }
  }

  EnableRate: boolean = true;
  test: any;

  EditRate() {
    if (this.OrderHeaderDetails.OrderRateType == "Fixed" || this.OrderHeaderDetails.OrderRateType == "Flexi") {
      // this._orderservice.getPermission().subscribe(
      //   response => {
      //     this.EnableRate = false;
      //   },
      //   error => {
      //     swal("Warning!", "Unauthorized", "warning");
      //     this.EnableRate = true;
      //   }
      // )
      this.PwdOrderRate.nativeElement.value = "";
      $('#PermissonModal').modal('show');
    }
    else {
      $('#PermissonModal').modal('hide');
    }
    this.EnableRate = true;
  }

  passWord(arg) {
    if (arg == "") {
      swal("Warning!", 'Please Enter the Password', "warning");
      $('#PermissonModal').modal('show');
      this.EnableRate = true;
    }
    else {
      this.permissonModel.CompanyCode = this.ccode;
      this.permissonModel.BranchCode = this.bcode;
      this.permissonModel.PermissionID = this.OrderRateEditCode;
      this.permissonModel.PermissionData = btoa(arg);
      this._masterService.postelevatedpermission(this.permissonModel).subscribe(
        response => {
          this.EnableRate = false;
          $('#PermissonModal').modal('hide');
        },
        (err) => {
          if (err.status === 401) {
            $('#PermissonModal').modal('show');
            this.EnableRate = true;
          }
        }
      )
    }
  }

  selectedFile: File = null;
  fd: FormData = new FormData();
  fileName: string;
  fileBrowseUrl: string;

  handleBrowse(event) {
    this.fileName = "";
    this.fileBrowseUrl = "";
    this.selectedFile = event.target.files[0];
    this.fd.append('uploadFile', this.selectedFile, this.selectedFile.name);
    this._httpClient.put(this.apiBaseUrl + 'api/order/UploadOrderImage/' + this.ccode + '/' + this.bcode + '/' + this.OrderHeaderDetails.SalCode + '/' + this.OrderHeaderDetails.ManagerCode, this.fd).subscribe(
      (Response: any) => {
        let bodyBrowse1 = Response.json()
        this.fileBrowseUrl = bodyBrowse1.Path;
        this.fileName = this.selectedFile.name;
      }
    )
  }

  OrderItemType: any;

  getOrderItemType() {
    this._orderservice.getOrderItemType().subscribe(
      response => {
        this.OrderItemType = response;
      }
    )
  }

  PurchasePlanDets: any = [];


  getorderRateType(orderRateType) {
    this._orderservice.getorderRateType(orderRateType).subscribe(
      response => {
        this.PurchasePlanDets = response;
      }
    )
  }

  UpdateRate(arg) {
    if (arg > 0) {
      this.Rate = arg;
      this.OrderHeaderDetails.Rate = this.Rate;
      this.EnableRate = true;
    }
    else {
      swal("Warning!", 'Please Enter the Rate', "warning");
    }
  }

  AdvancePercent: any = [];

  getAdvancePercent(purchasePlanCode) {
    this._orderservice.getAdvancePercentDetail(purchasePlanCode).subscribe(
      response => {
        this.AdvancePercent = response;
      }
    )
  }


  ngOnDestroy() {
    this._customerService.SendCustDataToEstComp(null);
    this._orderservice.SendOrderNoToComp(null);
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  DeliveryRate: boolean = false;
  OnRadioBtnChnge(arg) {
    this.EnableRate = true;
    this.getorderRateType(arg);
    if (arg == "Delivery") {
      this.Rate = "0.00";
      this.model.option = arg;
      this.DeliveryRate = true;
      //handled if slected type is sindhoor advance % disable02022022
      this.AdvancePercent.length =0;
    }
    else {
      if (this._router.url == "/orders/vieworders") {
        this.Rate = this.OrderHeaderDetails.Rate;
      }
      else {
        this.getRateperGram(this.OrdersHeaderForm);
      }
      this.model.option = arg;
      this.DeliveryRate = false;
    }
  }

  GSList: any;
  KaratList: any;
  getGSList() {
    this._orderservice.getGS().subscribe(
      response => {
        this.GSList = response;
        if (this.orderNo == null) {
          this.OrderHeaderDetails.gsCode = "NGO";
        }
      }
    )
  }
  getKaratList() {
    this._orderservice.getKarat().subscribe(
      response => {
        this.KaratList = response;
        if (this.orderNo == null) {
          this.OrderHeaderDetails.Karat = "22K"
        }
      }
    )
  }
  MCTypes: any;
  getMCTypes() {
    this._orderservice.getMCTypes().subscribe(
      response => {
        this.MCTypes = response;
      }
    )
  }

  Counter: any;
  getCounter() {
    this._orderservice.getCounter().subscribe(
      response => {
        this.Counter = response;
      }
    )
  }

  SalesManList: any;
  getSalesMan() {
    this._orderservice.getSalesManData().subscribe(
      response => {
        this.SalesManList = response;
        if (this.orderNo == null) {
          //this.OrderHeaderDetails.SalCode = "A";
        }
      })
  }

  getRateperGram(arg) {
    this.Rate = 0.00;
    this._orderservice.getRateperGram(arg.value).subscribe(
      response => {
        this.Rate = response;
        this.Rate = this.Rate.rate;
        this.OrderHeaderDetails.Rate = this.Rate;
      },
      (err) => {
        if (err.status === 400) {
          const validationError = err.error;
          swal("Warning!", validationError.description, "warning");
        }
        else {
        }
      }
    )
  }

  ItemName: any = [];
  getItemName(arg, index) {
    this._orderservice.getItemName(this.OrdersHeaderForm.value.GS, arg).subscribe(
      response => {
        this.ItemName[index] = response;
      }
    )
  }
  DisableIfReserved: boolean =false;
  toggle: string;
  OrderTypes: string = "";
  getOrderType(arg) {
    this.OrderTypes = arg;
    if(this.OrderType =='R'){
      this.DisableIfReserved =true;
     //disbaled flexi types 02/1/2022 
    }
    this.barcodeLines = [];
    this.EnableSubmitButton = true;
    this.OrderHeaderDetails.OrderRateType = "Delivery";
    this.OnRadioBtnChnge(this.OrderHeaderDetails.OrderRateType);
    switch (this.OrderTypes) {
      case "O": {
        this.toggle = "Customized Order";
        this.barcodeLines = [];
        this.fieldArray = [];
        this.OrderHeaderDetails.OrderType = "O"
        this.OrderHeaderDetails.lstOfOrderItemDetailsVM = this.barcodeLines;
        this.EnableAddRow = false;
        break;
      }
      case "A": {
        this.toggle = "OrderAdvance";
        this.newAttribute.CounterCode = "NA";
        this.newAttribute.ItemName = "Advance Amount";
        this.newAttribute.Description = "Advance Amount";
        this.newAttribute.Quantity = 0;
        this.newAttribute.FromGrossWt = 0;
        this.newAttribute.SlNo = 0;
        this.newAttribute.ToGrossWt = 0;
        this.newAttribute.MCPer = 0.00;
        this.newAttribute.MCPerPiece = '0.00';
        this.newAttribute.Amount = 0.00;

        this.newAttribute.GSCode = this.OrdersHeaderForm.value.GS;
        this.newAttribute.SalCode = this.OrderHeaderDetails.SalCode;
        this.barcodeLines.push(this.newAttribute);
        this.OrderHeaderDetails.lstOfOrderItemDetailsVM = this.barcodeLines;
        this.OrderHeaderDetails.lstOfOrderItemDetailsVM[0].CompanyCode = this.ccode;
        this.OrderHeaderDetails.lstOfOrderItemDetailsVM[0].BranchCode = this.bcode;
        this.EnableSubmitButton = false;
        this.OrderHeaderDetails.OrderType = "A"
        break;
      }
      case "R": {
        this.toggle = "Reserved";
        this.barcodeLines = [];
        this.OrderHeaderDetails.lstOfOrderItemDetailsVM = this.barcodeLines;
        this.OrderHeaderDetails.OrderType = "R"
        break;
      }
      default: {
        this.toggle = "Invalid";
        break;
      }
    }
  }



  OrderHeaderDetails: any = {
    CustID: null,
    CompanyCode: null,
    BranchCode: null,
    PANNo: null,
    OrderType: null,
    MobileNo: null,
    Remarks: null,
    OrderDate: null,
    OperatorCode: null,
    SalCode: null,
    DeliveryDate: null,
    OrderRateType: null,
    gsCode: null,
    Rate: 0.00,
    ManagerCode: null,
    BookingType: null,
    AdvacnceOrderAmount: null,
    GrandTotal: 0,
    ObjectStatus: null,
    BillNo: 0,
    CFlag: null,
    CancelledBy: null,
    BillCounter: null,
    Karat: null,
    OrderAdvanceRateType: null,
    RateValidityDays: 0,
    RateValidityTill: null,
    AdvancePercent: null,
    lstOfOrderItemDetailsVM: [],
    lstOfPayment: []
  }


  count: number = 0;
  EnableEditDelbtn = {};
  EnableSaveCnlbtn = {};
  //Add Row
  addrow() {
    this.newAttribute = {
      ObjID: null,
      CompanyCode: null,
      BranchCode: null,
      OrderNo: 0,
      SlNo: 0,
      ItemName: null,
      Quantity: null,
      Description: null,
      FromGrossWt: 0,
      ToGrossWt: 0,
      ItemType: "",
      AppSwt: 0,
      MCPer: 0,
      WastagePercent: 0,
      ManagerCode: null,
      BookingType: null,
      Amount: 0,
      SFlag: null,
      SParty: null,
      BillNo: 0,
      ImgID: null,
      UpdateOn: null,
      VAPercent: 0,
      ItemAmount: 0,
      GSCode: null,
      FinYear: null,
      ItemwiseTaxPercentage: null,
      ItemwiseTaxAmount: null,
      MCPerPiece: "",
      ProductID: null,
      IsIssued: null,
      CounterCode: null,
      SalCode: null,
      MCPercent: 0,
      MCType: "",
      EstNo: null
    };

    if (this.MinimumWeightGrams != 0) {
      this.newAttribute.FromGrossWt = this.MinimumWeightGrams;
      this.newAttribute.Amount = Number(this.Rate * this.MinimumWeightGrams);
    }

    this.fieldArray.push(this.newAttribute);
    for (let { } of this.fieldArray) {
      this.count++;
    }

    // if(this.MinimumWeightGrams != 0 && this.count == 1){
    // this.fieldArray[this.count - 1].FromGrossWt = this.MinimumWeightGrams;
    // this.fieldArray[this.count - 1].Amount = Number(this.Rate * this.MinimumWeightGrams);
    // }


    this.setWidth = true;
    this.EnableSaveCnlbtn[this.count - 1] = true;
    this.EnableEditDelbtn[this.count - 1] = false;
    this.readonly[this.count - 1] = false;
    this.MCPer[this.count - 1] = true;
    this.MCPerPiece[this.count - 1] = true;
    this.MCPercent[this.count - 1] = true;
    this.AppSwt[this.count - 1] = true;
    this.count = 0;
    this.EnableAddRow = true;
    this.EnableSubmitButton = true;
    this.EnableDropdown = true;
  }

  private fieldArray: any = [];
  private newAttribute: ItemDetailsLines = {
    ObjID: null,
    CompanyCode: null,
    BranchCode: null,
    OrderNo: 0,
    SlNo: 0,
    ItemName: null,
    Quantity: null,
    Description: null,
    FromGrossWt: 0,
    ToGrossWt: 0,
    ItemType: "",
    AppSwt: 0,
    MCPer: null,
    WastagePercent: 0,
    Amount: 0,
    SFlag: null,
    SParty: null,
    ManagerCode: null,
    BookingType: null,
    BillNo: 0,
    ImgID: null,
    UpdateOn: null,
    VAPercent: 0,
    ItemAmount: 0,
    GSCode: null,
    FinYear: null,
    ItemwiseTaxPercentage: null,
    ItemwiseTaxAmount: null,
    MCPerPiece: "",
    ProductID: null,
    IsIssued: null,
    CounterCode: null,
    SalCode: null,
    MCPercent: 0,
    MCType: "",
    EstNo: null
  };

  //To disable the GStype/Item dropdown once row added
  EnableDropdown: boolean = false;

  //To disable and enable Addrow Button
  EnableAddRow: boolean = false;

  //To disable and enable Submit Button
  EnableSubmitButton: boolean = true;


  chngOrderType(arg, i) {
    if (arg == "P") {
      this.AppSwt[i] = true;
      this.fieldArray[i].AppSwt = null;
    }
    else {
      this.AppSwt[i] = false;
      this.fieldArray[i].AppSwt = null;
    }
  }


  calculateAmount(arg, index) {
    if (this.OrderHeaderDetails.AdvancePercent != null) {
      this.fieldArray[index].Amount = Number(this.Rate * arg);
    }
  }


  saveDataFieldValue(index) {
    if (this.fieldArray[index]["CounterCode"] == null || this.fieldArray[index]["CounterCode"] == 0) {
      swal("Warning!", 'Please Select Counter', "warning");
    }
    else if (this.fieldArray[index]["ItemName"] == null || this.fieldArray[index]["ItemName"] == 0) {
      swal("Warning!", 'Please Select Item', "warning");
    }
    else if (this.fieldArray[index]["Description"] == null || this.fieldArray[index]["Description"] == 0) {
      swal("Warning!", 'Please Enter Item Description', "warning");
    }
    else if (this.fieldArray[index]["Quantity"] == null || this.fieldArray[index]["Quantity"] == 0) {
      swal("Warning!", 'Please Enter Quantity', "warning");
    }
    else if (this.fieldArray[index]["FromGrossWt"] == null || this.fieldArray[index]["FromGrossWt"] == 0) {
      swal("Warning!", 'Please Enter Appr.Wt', "warning");
    }
    else if (this.fieldArray[index]["ItemType"] == "S" && (this.fieldArray[index]["AppSwt"] == null || this.fieldArray[index]["AppSwt"] == 0)) {
      swal("Warning!", 'Please Enter Appr.SWt', "warning");
    }
    else if (this.fieldArray[index]["AppSwt"] >= this.fieldArray[index]["FromGrossWt"]) {
      swal("Warning!", 'Appr.SWt should be less than Appr.Wt', "warning");
    }
    else {
      this.EnableEditDelbtn[index] = true;
      this.EnableSaveCnlbtn[index] = false;
      this.readonly[index] = true;
      this.MCPer[index] = true;
      this.MCPerPiece[index] = true;
      this.MCPercent[index] = true;
      this.AppSwt[index] = true;
      this.EnableAddRow = false;
      this.EnableSubmitButton = false;
      this.fieldArray[index].GSCode = this.OrdersHeaderForm.value.GS;
      if (this.CopyEditedRow[index] != null && this.fileBrowseUrl == "") {
        this.fieldArray[index].ImgID = this.CopyEditedRow[index].ImgID;
      }
      else {
        this.fieldArray[index].ImgID = this.fileBrowseUrl;
      }
      this.fieldArray[index].SlNo = index + 1;
      this.fieldArray[index].SalCode = this.OrderHeaderDetails.SalCode;
      this.OrderHeaderDetails.lstOfOrderItemDetailsVM[index] = this.fieldArray[index];
      this.OrderHeaderDetails.lstOfOrderItemDetailsVM[index].CompanyCode = this.ccode;
      this.OrderHeaderDetails.lstOfOrderItemDetailsVM[index].BranchCode = this.bcode;
      this.myInputVariable.nativeElement.value = "";
      this.fileBrowseUrl = "";
      this.fileName = "";
      this.AdvanceAmount();
    }
  }

  AdvanceAmount() {
    let total = 0;
    this.OrderHeaderDetails.lstOfOrderItemDetailsVM.forEach((d) => {
      if (d.Amount != null && d.Amount != 0) {
        total += Number(<number>d.Amount);
      }
    });
    total = Number(<number>total * (<number>this.OrderHeaderDetails.AdvancePercent / 100));
    return total;
  }

  editFieldValue(index) {
    if (this.EnableAddRow == true) {
      swal("Warning!", 'Please save the enabled item', "warning");
    }
    else {
      this.CopyEditedRow[index] = Object.assign({}, this.fieldArray[index]);
      this.EnableEditDelbtn[index] = false;
      this.EnableSaveCnlbtn[index] = true;
      this.readonly[index] = false;
      this.EnableAddRow = true;
      this.EnableSubmitButton = true;
      this.toggleMCtype(this.fieldArray[index].MCType, index);
    }
  }

  CopyEditedRow: any = [];

  cancelDataFieldValue(index) {
    this.EnableAddRow = false;
    if (this.CopyEditedRow[index] != null) {
      this.fieldArray[index] = this.CopyEditedRow[index];
      this.OrderHeaderDetails.lstOfOrderItemDetailsVM[index] = this.CopyEditedRow[index];
      this.CopyEditedRow[index] = null;
      this.EnableSaveCnlbtn[index] = false;
      this.EnableEditDelbtn[index] = true;
      this.MCPer[index] = true;
      this.MCPerPiece[index] = true;
      this.MCPercent[index] = true;
      this.AppSwt[index] = true;
      this.readonly[index] = true;
      this.CopyEditedRow = [];
      this.toggleMCtype(this.fieldArray[index].MCType, index);
      this.getItemName(this.fieldArray[index].CounterCode, index);
      this.chngOrderType(this.fieldArray[index].ItemType, index);
      this.CopyEditedRow = [];
    }
    else {
      this.fieldArray.splice(index, 1);
      this.OrderHeaderDetails.lstOfOrderItemDetailsVM.splice(index, 1);
    }
    this.EnableDisableSubmit();
  }


  EnableDisableSubmit() {
    if (this.fieldArray.length <= 0) {
      this.EnableSubmitButton = true;
      this.EnableDropdown = false;
    }
    else {
      this.EnableSubmitButton = false;
      this.EnableDropdown = true;
    }
  }

  deleteFieldValue(index) {
    if (this.EnableAddRow == true) {
      swal("Warning!", 'Please save the enabled item', "warning");
    }
    else {
      this.fieldArray.splice(index, 1);
      this.EnableDisableSubmit();
      this.OrderHeaderDetails.lstOfOrderItemDetailsVM.splice(index, 1);
      this.getItemName(this.fieldArray[index].CounterCode, index);
    }
  }


  //Customer Details from Customer Service (BS)
  Customer: any = []
  CustDetails() {
    this._customerService.cast.subscribe(
      response => {
        this.Customer = response;
        if (this.Customer != null) {
          if (this.OrderHeaderDetails != null) {
            this.OrderHeaderDetails.CustID = this.Customer.ID;
            this.OrderHeaderDetails.PANNo = this.Customer.PANNo;
            this.OrderHeaderDetails.MobileNo = this.Customer.MobileNo;
            //Added on 21-JAN-2021 to make default to customized order
            // this.OrderHeaderDetails.OrderType = "O";
            // this.OrderHeaderDetails.BookingType = "N";
            // this.OrderHeaderDetails.OrderAdvanceRateType = "N";
            // this.getAdvancePercent("N");
            // this.toggle = "Customized Order";
            //this.addrow();

            //Ends here
          }
          this.EnableCustomerDetails = true;
        }
      });
  }


  toggleMCtype(arg, index) {
    switch (arg) {
      //MC%
      case "1": {
        this.MCPercent[index] = true;
        this.MCPer[index] = false;
        this.MCPerPiece[index] = true;
        this.fieldArray[index].MCPerPiece = 0.00;
        this.fieldArray[index].MCPercent = 0.00;
        break;
      }
      //MC/gram
      case "5": {
        this.MCPercent[index] = false;
        this.MCPer[index] = true;
        this.MCPerPiece[index] = true;
        this.fieldArray[index].MCPer = 0.00;
        this.fieldArray[index].MCPerPiece = 0.00;
        break;
      }
      //MC/piece
      case "6": {
        this.MCPercent[index] = true;
        this.MCPer[index] = true;
        this.MCPerPiece[index] = false;
        this.fieldArray[index].MCPer = 0.00;
        this.fieldArray[index].MCPercent = 0.00;
        break;
      }
      default: {
        //alert("Invalid Choice")
        this.MCPercent[index] = true;
        this.MCPer[index] = true;
        this.MCPerPiece[index] = true;
        break;
      }
    }
  }

  barcodeLines: any = [];
  getDetails(barcodeNo: any) {
    if (barcodeNo.value == null || barcodeNo.value == "") {
      swal("Warning!", 'Please enter barcode', "warning");
    }
    else if (barcodeNo.value != null && barcodeNo.value != "") {
      if (this.AlphaNumericValidations(barcodeNo.value) == false) {
        swal("Warning!", 'Please enter valid Barcode No', "warning");
      }
      else {
        this.addBarCode(barcodeNo.value);
      }
    }
  }

  SalesPersonModel: any = {
    CustID: null,
    salesPerson: null,
    barcode: null,
    GS: null,
    ItemName: null
  }

  modalPopUPBarcode: any;
  ToggleCalculation: boolean = false;

  GetBarcodeInfo(arg) {
    this._salesService.getBarcodeWithStoneWithoutValidation(arg.ItemName).subscribe(
      response => {
        this.modalPopUPBarcode = response;
        $('#myViewModal').modal('show');
        this.getGSList();
        this.SalesPersonModel.GS = this.modalPopUPBarcode.GsCode;
        this._masterService.getItemName(this.SalesPersonModel.GS).subscribe(
          response => {
            this.ItemName = response;
            this.SalesPersonModel.ItemName = this.modalPopUPBarcode.ItemName;
          });
        this.getGetStoneListDetailsViewBarcode(this.modalPopUPBarcode);
        this.getMCTypesForViewBarcode(this.modalPopUPBarcode.McType);
      }
    );
  }



  MCTypeList: any;
  McType: any;


  getMCTypesForViewBarcode(arg) {
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
  getGetStoneListDetailsViewBarcode(arg) {
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


  isEmptyObject(obj) {
    return (obj && (Object.keys(obj).length === 0));
  }

  ArrayList: any = [];
  addBarCode(arg) {
    let data = this.barcodeLines.find(x => x.ItemName == arg);
    if (data == null) {
      this._orderservice.getBarcodefromAPI(arg).subscribe(
        response => {
          this.ArrayList.push(response);
          this.barcodeLines.push(this.ArrayList[0]);
          this.OrderHeaderDetails.lstOfOrderItemDetailsVM = this.barcodeLines;
          this.reserveForm.reset();
          this.EnableDisableSubmitForReservedOrder();
          this.ArrayList = [];
        }
      )
    }
    else {
      swal("Warning!", 'BarcodeNo is already added', "warning");
      this.reserveForm.reset();
    }
  }


  DeleteRow(index: number) {
    var ans = confirm("Do you want to delete");
    if (ans) {
      this.barcodeLines.splice(index, 1);
      this.EnableDisableSubmitForReservedOrder();
    }
  }

  EnableDisableSubmitForReservedOrder() {
    if (this.barcodeLines.length <= 0) {
      this.EnableSubmitButton = true;
    }
    else {
      this.EnableSubmitButton = false;
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


  SendItemDetsToPaymentComp() {
    if (this.OrderHeaderDetails.gsCode == null) {
      swal("Warning!", 'Please select GS Type', "warning");
    }
    else if (this.OrderHeaderDetails.Karat == null) {
      swal("Warning!", 'Please select Karat', "warning");
    }
    else if (this.OrderHeaderDetails.DeliveryDate == null) {
      swal("Warning!", 'Please select Due Date', "warning");
    }
    else if (this.OrderHeaderDetails.lstOfOrderItemDetailsVM.length == 0) {
      swal("Warning!", 'Please enter the Order Details', "warning");
    }
    // else if (this.OrderHeaderDetails.ManagerCode == null) {
    //   this.toastr.warning('Please select manager', 'Alert!');
    // }
    else if (this.OrderHeaderDetails.BookingType == null) {
      swal("Warning!", 'Please select customer type', "warning");
    }
    else if (this.OrderHeaderDetails.OrderAdvanceRateType == null) {
      swal("Warning!", 'Please select purchase plan', "warning");
    }
    else if (this.OrderHeaderDetails.SalCode == null) {
      swal("Warning!", 'Please select sales person', "warning");
    }
    else if (this.OrderHeaderDetails.AdvancePercent == null && this.AdvancePercent.length > 0) {
      swal("Warning!", 'Please select advance %', "warning");
    }
    else {
      //var days = this.diffDays(this.datepipe.transform(this.OrderHeaderDetails.OrderDate, 'yyyyMMdd'), this.datepipe.transform(this.OrderHeaderDetails.DeliveryDate, 'yyyyMMdd'));
      //if(days == 0)
      if (this.OrderHeaderDetails.DeliveryDate == this.OrderHeaderDetails.OrderDate) {
        var ans = confirm("Due date is same as the Order date. Do you want to save??");
        if (ans) {
          this.OrderHeaderDetails.CompanyCode = this.ccode;
          this.OrderHeaderDetails.BranchCode = this.bcode;
          this._paymentService.inputData(this.OrderHeaderDetails);
        }
      }
      else {
        this.OrderHeaderDetails.CompanyCode = this.ccode;
        this.OrderHeaderDetails.BranchCode = this.bcode;
        this._paymentService.inputData(this.OrderHeaderDetails);
      }
    }
  }


  // diffDays(d1, d2) {
  //   var ndays;
  //   ndays = (d2 - d1);
  //   return ndays;
  // }
}