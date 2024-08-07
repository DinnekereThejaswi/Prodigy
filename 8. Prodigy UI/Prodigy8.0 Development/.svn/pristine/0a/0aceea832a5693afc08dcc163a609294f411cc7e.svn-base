import { Input, EventEmitter, Output, HostListener } from '@angular/core';
import { estimationService } from './../estimation/estimation.service';
import { SalesService } from './../sales/sales.service';
import { PurchaseModel, PurchaseSummaryModel } from './purchase.model';
import { FormGroup } from '@angular/forms';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { PurchaseService } from './purchase.service';
import { ToastrService } from 'ngx-toastr';
import { FormBuilder, Validators } from '@angular/forms';
import { CustomerService } from '../masters/customer/customer.service';
import { MasterService } from '../core/common/master.service';
import swal from 'sweetalert';
import { AppConfigService } from '../AppConfigService';
import { Router } from '@angular/router';
import * as CryptoJS from 'crypto-js';
import { Observable } from 'rxjs';
import { ComponentCanDeactivate } from './../appconfirmation-guard';
declare var $: any;

@Component({
  selector: 'app-purchase',
  templateUrl: './purchase.component.html',
  styleUrls: ['./purchase.component.css']
})

export class PurchaseComponent implements OnInit, OnDestroy, ComponentCanDeactivate {
  GS: any;
  RateType: any;
  Item: any;
  count: number = 0;
  PurchaseForm: FormGroup;
  Rate: any = 0.00;
  routerUrl: string = "";
  SalesEstNo: string;
  CustomerDets: any = [];
  purchaseDetails: any = [];
  DmCarats: number = 0;
  LinesDetails: Array<any> = [];
  EnableReprint: boolean = false;
  EnableDisablectrls: boolean = true;
  public Index: number
  EditModePurchase: boolean = false;
  StoneCarats: number = 0;
  EnableDisablePurPer: boolean = false;
  PurSalCode: string;
  leavePage: boolean = false;
  @Output() valueChange = new EventEmitter();
  EnableJson: boolean = false;
  password: string;
  ccode: string = "";
  bcode: string = "";

  radioItems: Array<string>;
  model = { option: 'New Estimation' };
  NewEstimation: boolean = true;

  PurchaseSummaryData: PurchaseSummaryModel = {
    Qty: 0,
    GrossWt: null,
    NtWt: null,
    StoneWt: null,
    carats: null,
    Amount: 0.00
  };


  //Hide Show data when accordian collapsed(Header)
  public ToggleTotal: boolean = true;
  public CollapseCustomerTab: boolean = true;
  public CollapseCustomerDetailsTab: boolean = true;


  /////If Purchase

  //Radio Buton change
  ToggleEstimation: boolean = false;
  OnRadioBtnChnge(arg) {
    if (arg == "New Estimation") {
      this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
        () => {
          this._router.navigate(['/purchase']);
          this.model.option = arg;
          this.ToggleEstimation = false;
          this.NewEstimation = true;
          this.ShowHidePurEst = true;
        })
    }
    else {
      this.model.option = arg;
      this.ToggleEstimation = true;
      this._CustomerService.SendCustDataToEstComp(null);
      this.NewEstimation = false;
      this.ShowHidePurEst = false;
    }
  }

  constructor(private _purchaseService: PurchaseService, private formBuilder: FormBuilder,
    private toastr: ToastrService, private _CustomerService: CustomerService, private _salesService: SalesService,
    private _router: Router, private _masterService: MasterService,
    private _estimationService: estimationService, private _appConfigService: AppConfigService) {
    this.radioItems = ['New Estimation', 'Existing Estimation'];
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
    this.getCB();
    this.EnableRateTypeBasedonRoute();
  }

  PurchaseAmount: number = 0;

  valueChanged(Amount) { // You can give any function name
    this.PurchaseAmount = Amount;
    this.valueChange.emit(this.PurchaseAmount);
  }

  chngDmdCrt(index) {
    if (this.fieldArray[index].Dcts == 0 || this.fieldArray[index].Dcts == "") {
      this.PurchaseLinesPost.GrandTotal = this.PurchaseLinesPost.GrandTotal - this.fieldArray[index].DiamondAmount;
      this.PurchaseLinesPost.TotalPurchaseAmount = this.PurchaseLinesPost.TotalPurchaseAmount - this.fieldArray[index].DiamondAmount;
      this.fieldArray[index].ItemAmount = this.fieldArray[index].ItemAmount - this.fieldArray[index].DiamondAmount;
      this.fieldArray[index].DiamondAmount = 0;
      this.PurchaseLinesPost.lstPurchaseEstDetailsVM[index].DiamondAmount = 0;
      this.PurchaseLinesPost.lstPurchaseEstDetailsVM[index].lstPurchaseEstDiamondDetailsVM = [];
      this.sendDataToDiamondComp(this.fieldArray[index], index);
      this.EnableDmdSubmit = false;
    }
  }


  confirmBeforeLeave(): boolean {
    if (this.leavePage == true) {
      var ans = (confirm("You have unsaved changes! If you leave, your changes will be lost."))
      if (ans) {
        this.leavePage = false;
        return true;
      }
      else {
        return false;
      }
    }
    else {
      return true;
    }
  }


  PurchaseTotalAmount: number = 0;
  @Output() PurchaseAmountChange = new EventEmitter();
  @Output() PurchaseNtWt = new EventEmitter();
  @Output() PurchaseGrossWt = new EventEmitter();

  PurchaseAmountChanged(SalesAmount) {
    this.PurchaseTotalAmount = SalesAmount;
    this.PurchaseAmountChange.emit(this.PurchaseTotalAmount);
  }


  // diamondAmount: string;
  DiamondAmount: number = 0;
  ItemAmount: number = 0;
  stoneAmount: string;
  StoneAmount: number = 0;


  receiveStoneDiamondAmt() {

    for (var field in this.fieldArray) {
      if (this.PurchaseLinesPost.lstPurchaseEstDetailsVM[field].lstPurchaseEstDiamondDetailsVM.length > 0) {
        for (var i = 0; i < this.PurchaseLinesPost.lstPurchaseEstDetailsVM[field].lstPurchaseEstDiamondDetailsVM.length; i++) {
          this.DiamondAmount += Number(this.PurchaseLinesPost.lstPurchaseEstDetailsVM[field].lstPurchaseEstDiamondDetailsVM[i].Amount);
        }
      }
      else {
        this.DiamondAmount = 0;
      }
      if (this.PurchaseLinesPost.lstPurchaseEstDetailsVM[field].lstPurchaseEstStoneDetailsVM.length > 0) {
        for (var i = 0; i < this.PurchaseLinesPost.lstPurchaseEstDetailsVM[field].lstPurchaseEstStoneDetailsVM.length; i++) {
          this.StoneAmount += Number(this.PurchaseLinesPost.lstPurchaseEstDetailsVM[field].lstPurchaseEstStoneDetailsVM[i].Amount);
        }
      }
      else {
        this.StoneAmount = 0;
      }
      this.fieldArray[field].DiamondAmount = this.DiamondAmount + this.StoneAmount;
      //this.GetMLLoss_MLPercentage(field);
      this.recalculate(field);
      this.DiamondAmount = 0;
      this.StoneAmount = 0;
      this.markFormGroupDirty(this.PurchaseForm);
    }
  }

  EnableStneSubmit: any;
  EnableDmdSubmit: any;


  EnableDisableStnBtn(arg) {
    this.EnableStneSubmit = arg;
  }

  EnableDisableDmdBtn(arg) {
    this.EnableDmdSubmit = arg;
  }


  ifPurchase: boolean = false;
  EnableDisableSalesPerson: boolean = false;

  ReprintData: any = {
    EstNo: null,
    EstType: null
  }

  arrayObj: any = [];

  OrderNoFromEstComp: Number = 0;

  ngOnInit() {
    this.getApplicationDate();
    this.EnableCustomerTab = false;
    this.PurchaseForm = this.formBuilder.group({
      GS: [null, Validators.required],
      SalCode: [null, Validators.required],
      RateType: ['E', Validators.required],
      Item: [null, Validators.required],
      RatePerGram: [null, Validators.required]
    });

    this.PurchaseSummaryData = {
      Qty: 0,
      GrossWt: null,
      NtWt: null,
      StoneWt: null,
      carats: null,
      Amount: 0.00
    };

    if (this._router.url === "/purchase") {
      this.ifPurchase = true;
      this.EnableDisableSalesPerson = false
      this.PurSalCode = null;
      this._CustomerService.cast.subscribe(
        response => {
          this.customerDtls = response;
          this.leavePage = true;
        }
      )
    }
    else {
      this.EnableDisableSalesPerson = true;
    }

    this._salesService.EstNo.subscribe(
      response => {
        this.SalesEstNo = response;
      }
    )



    // this._estimationService.SubjectEstNo.subscribe(
    //   response => {
    //     this.SalesEstNo = response;
    //     if (this.SalesEstNo != null && this.SalesEstNo != "0") {
    //       if (this._router.url != "/purchase") {
    //         this._estimationService.getEstimationDetailsfromAPI(this.SalesEstNo).subscribe(
    //           response => {
    //             this.arrayObj = response;
    //             if (this.arrayObj != null && this.arrayObj.salesEstimatonVM.length > 0) {
    //               this.PurSalCode = this.arrayObj.salesEstimatonVM[0].SalCode;
    //             }
    //           }
    //         )
    //       }
    //     }
    //   });

    this._estimationService.SubjectOrderNo.subscribe(
      response => {
        this.OrderNoFromEstComp = response;
        if (this.OrderNoFromEstComp != 0) {
          this.fieldArray = [];
          this.PurchaseForm.reset();
          this.PurchaseForm = this.formBuilder.group({
            GS: [null, Validators.required],
            SalCode: [null, Validators.required],
            RateType: ['E', Validators.required],
            Item: [null, Validators.required],
            RatePerGram: [null, Validators.required]
          });
          this.PurchaseLinesPost = {
            CustID: 0,
            MobileNo: null,
            EstNo: 0,
            PurItem: null,
            TodayRate: null,
            OperatorCode: null,
            GrandTotal: null,
            PType: "E",
            CompanyCode: this.ccode,
            BranchCode: this.bcode,
            TotalPurchaseAmount: null,
            lstPurchaseEstDetailsVM: [],
            PaymentVM: []
          }
        }
      });

    this._estimationService.SubjectEstNo.subscribe(
      response => {
        this.SalesEstNo = response;
        if (this.SalesEstNo != null && this.SalesEstNo != "0") {
          if (this._router.url != "/purchase" && this._router.url != "/purchase/purchase-billing") {
            this._estimationService.getEstimationDetailsfromAPI(this.SalesEstNo).subscribe(
              response => {
                this.arrayObj = response;
                this.PurchaseLinesPost.EstNo = this.SalesEstNo;
                this.markFormGroupDirty(this.PurchaseForm);
                this.EnableSubmitButton = true;
                if (this.arrayObj != null) {
                  this.PurSalCode = this.arrayObj.salesEstimatonVM[0].SalCode;
                  this.EnableDisableSubmit();
                }
              }
            )
          }
        }
      });


    this.Index = -1;
    this.getGSList();
    this.getSalesMan();
    this._CustomerService.cast.subscribe(
      response => {
        this.CustomerDets = response;
        if (this.CustomerDets != null) {
          this.PurchaseLinesPost.CustID = this.CustomerDets.ID;
          this.PurchaseLinesPost.MobileNo = this.CustomerDets.MobileNo;
          this.PurchaseLinesPost.OperatorCode = localStorage.getItem('Login');
          this.PurchaseLinesPost.CompanyCode = this.ccode,
            this.PurchaseLinesPost.BranchCode = this.bcode
          this.EnableCustomerTab = true;
          this.ToggleCustomer = true;
        }
        else {
          this.PurchaseLinesPost.CustID = null;
          this.PurchaseLinesPost.OperatorCode = localStorage.getItem('Login');
          this.PurchaseLinesPost.CompanyCode = this.ccode,
            this.PurchaseLinesPost.BranchCode = this.bcode
          this.EnableCustomerTab = false;
          this.ToggleCustomer = false;
        }
        this.EnableAddRow = false;
      });


    this._estimationService.SubjectEstNo.subscribe(
      response => {
        this.SalesEstNo = response;
        if (this.SalesEstNo != null && this.SalesEstNo != "0") {
          this._purchaseService.getPurchaseDetailsfromAPI(this.SalesEstNo).subscribe(
            response => {
              this.purchaseDetails = response;
              if (this.purchaseDetails != null) {
                this.EnableSubmitButton = true;
                if (this.purchaseDetails.lstPurchaseEstDetailsVM.length > 0) {
                  this.fieldArray = this.purchaseDetails.lstPurchaseEstDetailsVM;
                  this._purchaseService.sendPurchaseDatatoEstComp(this.PurchaseSummaryData);
                  this.getItemExistingList(this.fieldArray[0].GSCode);
                  for (var field in this.fieldArray) {
                    this.EnableEditDelbtn[field] = true;
                    this.EnableSaveCnlbtn[field] = false;
                    this.readonly[field] = true;
                  }

                  this.PurchaseForm.controls.GS.setValue(this.fieldArray[0].GSCode);
                  this.PurchaseForm.controls.SalCode.setValue(this.fieldArray[0].SalCode);
                  this.PurchaseForm.controls.Item.setValue(this.purchaseDetails.PurItem);
                  this.PurchaseForm.controls.RateType.setValue("E");
                  this.Rate = this.purchaseDetails.TodayRate;
                  this.PurchaseLinesPost.TotalPurchaseAmount = this.purchaseDetails.TotalPurchaseAmount;
                  this.PurchaseLinesPost.GrandTotal = this.purchaseDetails.GrandTotal;
                  this.PurchaseLinesPost.TodayRate = this.Rate;
                  this.PurchaseLinesPost.PType = this.PurchaseForm.value.RateType;
                  this.PurchaseLinesPost.PurItem = this.PurchaseForm.value.Item;
                  this.PurchaseLinesPost.EstNo = this.SalesEstNo;

                  this.PurchaseLinesPost.lstPurchaseEstDetailsVM = this.fieldArray;
                  this.EnableSubmitButton = false;
                  this.markFormGroupPristine(this.PurchaseForm);
                  this.EnableDisableSubmit();
                }
              }
              else {
                this.fieldArray = [];
                this.PurchaseForm.reset();
              }
            },
            (err) => {
              if (err.status === 404) {
                this.fieldArray = [];
                this.PurchaseForm.reset();
                this.PurchaseForm = this.formBuilder.group({
                  GS: [null, Validators.required],
                  SalCode: [null, Validators.required],
                  RateType: ['E', Validators.required],
                  Item: [null, Validators.required],
                  RatePerGram: [null, Validators.required]
                });
                this.PurchaseLinesPost = {
                  CustID: 0,
                  MobileNo: null,
                  EstNo: 0,
                  PurItem: null,
                  TodayRate: null,
                  OperatorCode: null,
                  GrandTotal: null,
                  PType: "E",
                  CompanyCode: this.ccode,
                  BranchCode: this.bcode,
                  TotalPurchaseAmount: null,
                  lstPurchaseEstDetailsVM: [],
                  PaymentVM: []
                }
              }
            }
          )
        }
      }
    )
    if (this._router.url == "/sales-billing" || this._router.url == "/purchase/purchase-billing") {
      this.EnableDisablectrls = false;
    }
    if (this._router.url == "/estimation" || this._router.url == "/purchase") {
      this.EnableDisablectrls = true;
    }
  }


  applicationDate: any;
  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
      }
    )
  }



  customerDtls: any = [];
  ShowHidePurEst: boolean = true;

  EstimationDetails(arg) {
    this.SalesEstNo = arg;
    if (this.SalesEstNo != null && this.SalesEstNo != "0" && this.SalesEstNo != "") {
      this._purchaseService.getPurchaseDetailsfromAPI(this.SalesEstNo).subscribe(
        response => {
          this.purchaseDetails = response;
          if (this.purchaseDetails != null) {
            this.leavePage = true;
            this.ShowHidePurEst = true;
            this._estimationService.SendEstNo(this.SalesEstNo);
            this._CustomerService.getCustomerDtls(this.purchaseDetails.CustID).subscribe(
              response => {
                this.customerDtls = response;
                this._CustomerService.sendCustomerDtls_To_Customer_Component(this.customerDtls);
                this.fieldArray = this.purchaseDetails.lstPurchaseEstDetailsVM;
                this._purchaseService.sendPurchaseDatatoEstComp(this.PurchaseSummaryData);
                this.getItemExistingList(this.fieldArray[0].GSCode);
                for (var field in this.fieldArray) {
                  this.EnableEditDelbtn[field] = true;
                  this.EnableSaveCnlbtn[field] = false;
                  this.readonly[field] = true;
                }
                this.PurchaseForm.controls.GS.setValue(this.fieldArray[0].GSCode);
                this.PurchaseForm.controls.SalCode.setValue(this.fieldArray[0].SalCode);
                this.PurchaseForm.controls.Item.setValue(this.purchaseDetails.PurItem);
                this.PurchaseForm.controls.RateType.setValue("E");
                //this.PurchaseForm.controls.RateType.setValue(this.purchaseDetails.PType);
                this.Rate = this.purchaseDetails.TodayRate;
                this.PurchaseLinesPost.TotalPurchaseAmount = this.purchaseDetails.TotalPurchaseAmount;
                this.PurchaseLinesPost.GrandTotal = this.purchaseDetails.GrandTotal;
                this.PurchaseLinesPost.TodayRate = this.Rate;
                this.PurchaseLinesPost.PType = this.PurchaseForm.value.RateType;
                this.PurchaseLinesPost.PurItem = this.PurchaseForm.value.Item;
                this.PurchaseLinesPost.EstNo = this.SalesEstNo;
                this.PurchaseLinesPost.lstPurchaseEstDetailsVM = this.fieldArray;
                this.EnableSubmitButton = false;
                this.EnableRateTypeBasedonRoute();
                this.markFormGroupPristine(this.PurchaseForm);
                this.EnableDisableSubmit();
              }
            )
          }
          else {
            if (this._router.url == "/purchase") {
              swal("Warning!", 'Purchase Estimation No does not exists', "warning");
              this.ShowHidePurEst = false;
              this.ClearValues();
            }
          }
        },
        (err) => {
          if (err.status === 404) {
            const validationError = err.error;
            // if (validationError.description != "Estimation No Does Not Exist" ) {
            if (this._router.url == "/purchase") {
              swal("Warning!", validationError.description, "warning");
              this.ShowHidePurEst = false;
              this.ClearValues();
            }
            else if (validationError.description != "Estimation No Does Not Exist" && this._router.url != "/purchase") {
              swal("Warning!", validationError.description, "warning");
              this.ShowHidePurEst = false;
              this.ClearValues();
            }
          }
          else {
          }
        }
      )
    }
  }

  ngOnDestroy() {
    this._salesService.SendEstNo_To_Purchase(null);
    this._purchaseService.sendPurchaseDatatoEstComp(null);
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
    this._CustomerService.SendCustDataToEstComp(null);
    this._estimationService.SendEstNo(0);
  }

  //Get GS DropDown
  GSList: any;
  getGSList() {
    this._masterService.getNewGsList().subscribe(
      response => {
        this.GSList = response;
        this.PurchaseForm.controls['GS'].setValue("OGO");
        this.getItemList(this.PurchaseForm);
      }
    )
  }

  //Get Rate Type
  RateTypeData = [
    {
      'name': 'Exchange Rate',
      'value': 'E'
    },
    {
      'name': 'Cash Rate',
      'value': 'C'
    }
  ]

  //Get Item DropDown
  ItemList: any;
  getItemList(arg) {
    this.PurchaseLinesPost.PurItem = arg.value.GS;
    this._masterService.getItemfromAPI(arg.value).subscribe(
      response => {
        this.ItemList = response;
      }
    )
  }

  getItemExistingList(arg) {
    this._masterService.getItemfromExistingAPI(arg).subscribe(
      response => {
        this.ItemList = response;
      }
    )
  }

  SalesManList: any;

  getSalesMan() {
    this._masterService.getSalesMan().subscribe(
      response => {
        this.SalesManList = response;
      })
  }

  //Toggle RateType DropDown
  toggleRateType(arg) {
    //this.getRateperGram(arg);
    this.Rate = 0.00;
  }

  ItemName: string;
  ItemType: string;
  EnablePurchaseRate: boolean = false;
  Karat: any;

  AJAXRateperGram(arg, index) {
    this.ItemType = arg;
    this.Karat = this.ItemList.filter(Item => Item.Item_code === arg);
    this.ItemName = this.Karat[0].Karat;
    this.Rate = null;
    this._masterService.RefreshRateperGram(this.PurchaseForm.value, this.ItemName).subscribe(
      response => {
        this.Rate = response;
        this.Rate = this.Rate.Rate;
        this.EnableDisablePurPer = false;

        //Clearing existing data

        this.fieldArray[index].PurityPercent = 0.00;
        this.fieldArray[index].GrossWt = null;
        this.fieldArray[index].StneWt = null;
        this.fieldArray[index].Dcts = null;
        this.fieldArray[index].NetWt = null;

        //get Melting % Percent  
        this.fieldArray[index].MeltingPercent = 0;
        //get Melting Loss
        this.fieldArray[index].MeltingLoss = 0;
        //Get Purchase Rate
        this.fieldArray[index].PurchaseRate = 0;
        //Get Final Amount
        this.fieldArray[index].ItemAmount = 0;

        if (this.fieldArray[index].ItemName != "24K") {
          this.EnableDisablePurPer = true;
        }
        else {
          this.EnableDisablePurPer = false;
        }
        this.EnableDisablePurRate();
        //End of Clearing existing data
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

  EnableDisablePurRate() {
    if (this._router.url == '/purchase') {
      this.EnablePurchaseRate = false;
    }
    else {
      this.EnablePurchaseRate = true;
    }
  }

  ItemAmountCalc(index) {
    if (this.fieldArray[index].PurchaseRate > this.Rate) {
      swal("Warning!", 'Purchase rate should not be greater than ' + this.Rate + '.00', "warning");
      this.fieldArray[index].ItemAmount = 0;
    }
    else {
      this.fieldArray[index].ItemAmount = Number(<number>this.fieldArray[index].NetWt * <number>this.fieldArray[index].PurchaseRate).toFixed(2);
    }
  }

  getRateperGram(form) {
    this.Rate = null;
    this._masterService.RefreshRateperGram(form.value, this.ItemName).subscribe(
      response => {
        this.Rate = response;
        this.Rate = this.Rate.Rate;
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

  onSubmit() {
    if (this.PurchaseForm.pristine) {
      return;
    }
  }

  //To disable the GStype/Item dropdown once row added
  EnableDropdown: boolean = false;

  //To disable and enable Addrow Button
  EnableAddRow: boolean = false;
  EnableRateType: boolean = false;

  //To disable and enable Submit Button
  EnableSubmitButton: boolean = true;

  add(form) {
    if (form.value.SalCode == null) {
      swal("Warning!", 'Please select Sales Person', "warning");
    }
    else if (form.value.GS == null) {
      swal("Warning!", 'Please select GS', "warning");
    }
    // else if (form.value.RateType == null) {
    //   this.toastr.warning('Please select Rate Type', 'Alert!');      
    // }
    else {
      this.addrow();
    }
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ///////////////////////////////////////////////////////////////////////
  addrow() {
    this.newAttribute = {
      SalCode: null,
      GSCode: null,
      ItemName: null,
      GrossWt: null,
      StneWt: null,
      NetWt: null,
      PurityPercent: null,
      MeltingPercent: null,
      CompanyCode: this.ccode,
      BranchCode: this.bcode,
      MeltingLoss: null,
      PurchaseRate: null,
      DiamondAmount: null,
      Dcts: null,
      GoldAmount: null,
      ItemAmount: null,
      lstPurchaseEstStoneDetailsVM: [],
      lstPurchaseEstDiamondDetailsVM: []
    };

    this.fieldArray.push(this.newAttribute);
    for (let { } of this.fieldArray) {
      this.count++;
    }

    this.EnableSaveCnlbtn[this.count - 1] = true;
    this.EnableEditDelbtn[this.count - 1] = false;
    this.readonly[this.count - 1] = false
    this.count = 0;
    this.EnableAddRow = true;
    this.EnableSubmitButton = true;
    this.EnableDropdown = true;

    this.EnableRateTypeBasedonRoute();
    this.leavePage = true;
    this.EditModePurchase = true;
  }

  public fieldArray: any = [];

  private newAttribute: PurchaseModel = {
    SalCode: null,
    GSCode: null,
    ItemName: null,
    GrossWt: null,
    StneWt: null,
    NetWt: null,
    PurityPercent: null,
    MeltingPercent: null,
    CompanyCode: this.ccode,
    BranchCode: this.bcode,
    MeltingLoss: null,
    PurchaseRate: null,
    DiamondAmount: null,
    Dcts: null,
    GoldAmount: null,
    ItemAmount: null,
    lstPurchaseEstStoneDetailsVM: [],
    lstPurchaseEstDiamondDetailsVM: []
  };


  GetNetWT(index) {
    this.fieldArray[index].PurityPercent = 0.00;
    const GrossWt = this.fieldArray[index].GrossWt;
    const StneWt = this.fieldArray[index].StneWt;
    if (parseInt(StneWt) > parseInt(GrossWt)) {
      swal("Input Error", "Stone Wt cannot be greater than Gross Wt.", "error");
      this.fieldArray[index].StneWt = null;
    }
    else {
      this.fieldArray[index].NetWt = Number(<number>GrossWt - <number>StneWt);
      if (this.fieldArray[index].ItemName != "OG24K" && this.fieldArray[index].ItemName != "OGOL" && this.fieldArray[index].ItemName != "OGCN24K" && this.fieldArray[index].ItemName != "OGCN") {
        this.GetMLLoss_MLPercentage(index);
        this.EnableDisablePurPer = true;
      }
      else {
        this.EnableDisablePurPer = false;
      }
    }
  }




  recalculate(index) {
    if (this.fieldArray[index].ItemName == "OG24K" || this.fieldArray[index].ItemName == "OGOL" || this.fieldArray[index].ItemName == "OGCN24K" || this.fieldArray[index].ItemName == "OGCN") {
      if (this.fieldArray[index].ItemName == "OG24K") {
        this.fieldArray[index].ItemAmount = Number(<number>this.fieldArray[index].NetWt * <number>this.fieldArray[index].PurchaseRate).toFixed(2);
      }
      else if (this.fieldArray[index].ItemName == "OGCN") {
        this.fieldArray[index].ItemAmount = Number(<number>this.fieldArray[index].NetWt * <number>this.fieldArray[index].PurchaseRate).toFixed(2);
      }
      else if (this.fieldArray[index].ItemName == "OGCN24K") {
        this.fieldArray[index].ItemAmount = Number(<number>this.fieldArray[index].NetWt * <number>this.fieldArray[index].PurchaseRate).toFixed(2);
      }
      else {
        this.fieldArray[index].ItemAmount = Number(<number>this.fieldArray[index].NetWt * <number>this.fieldArray[index].PurchaseRate).toFixed(2);
      }
    }
    else if (this.fieldArray[index].ItemName == "BJOR" || this.fieldArray[index].ItemName == "OG22") {
      this.fieldArray[index].ItemAmount = Number(<number>this.fieldArray[index].PurchaseRate * <number>this.fieldArray[index].NetWt);
    }

    else if (this.fieldArray[index].ItemName == "OG18") {
      this.fieldArray[index].ItemAmount = Number(<number>this.fieldArray[index].PurchaseRate * <number>this.fieldArray[index].NetWt);
    }

    else if (this.fieldArray[index].ItemName == "BJ18K") {
      this.fieldArray[index].ItemAmount = Number(<number>this.fieldArray[index].PurchaseRate * <number>this.fieldArray[index].NetWt);
    }

    if (this.fieldArray[index].DiamondAmount != null) {
      const DmdAmt = this.fieldArray[index].DiamondAmount;
      const ItmAmt = this.fieldArray[index].ItemAmount;
      this.fieldArray[index].ItemAmount = parseFloat((+ItmAmt + +DmdAmt).toFixed(2));
    }
  }



  StandardMeltingLoss: number = 2;
  PurityPercent: number = 0;
  GetMLLoss_MLPercentage(index) {
    if (this.fieldArray[index].PurityPercent > 100) {
      swal("Input Error", "Purity Percentage should not be greater than 100 %", "error");
      this.fieldArray[index].PurityPercent = null;
      this.PurityPercent = null;
    }
    else if (this.fieldArray[index].PurityPercent >= 100 && this.fieldArray[index].ItemName != "OG24K") {
      swal("Input Error", "Purity Percentage has to be less than 100", "error");
      this.fieldArray[index].PurityPercent = null;
      this.PurityPercent = null;
    }
    else {
      if (this.fieldArray[index].ItemName == "OG24K" || this.fieldArray[index].ItemName == "OGOL" || this.fieldArray[index].ItemName == "OGCN24K" || this.fieldArray[index].ItemName == "OGCN") {
        this.PurityPercent = this.fieldArray[index].PurityPercent;
        this.EnableDisablePurPer = false;

        if (this.fieldArray[index].ItemName == "OG24K") {
          this.fieldArray[index].PurchaseRate = Number(<number>this.Rate * <number>this.PurityPercent / 100).toFixed(2);
          // if (this._router.url == "/purchase") {
          //   this.fieldArray[index].PurchaseRate = Math.round(<number>this.Rate * <number>this.PurityPercent / 100);
          // }
          // else {
          //   this.fieldArray[index].PurchaseRate = Number(<number>this.Rate * <number>this.PurityPercent / 100).toFixed(2);
          // }
          this.fieldArray[index].MeltingPercent = Number(((<number>this.Rate - <number>this.fieldArray[index].PurchaseRate) * 100) / (<number>this.Rate)).toFixed(2);
          this.fieldArray[index].MeltingLoss = Math.abs(Number(<number>this.fieldArray[index].NetWt * <number>this.fieldArray[index].MeltingPercent / 100)).toFixed(3);
          this.fieldArray[index].ItemAmount = Number(<number>this.fieldArray[index].NetWt * <number>this.fieldArray[index].PurchaseRate).toFixed(2);
        }
        else if (this.fieldArray[index].ItemName == "OGCN") {
          this.fieldArray[index].PurchaseRate = Number(<number>this.Rate * <number>this.PurityPercent / 91.6).toFixed(2);
          // if (this._router.url == "/purchase") {
          //   this.fieldArray[index].PurchaseRate = Math.round(<number>this.Rate * <number>this.PurityPercent / 91.6);
          // }
          // else {
          //   this.fieldArray[index].PurchaseRate = Number(<number>this.Rate * <number>this.PurityPercent / 91.6).toFixed(2);
          // }
          this.fieldArray[index].MeltingPercent = Number(((<number>this.Rate - <number>this.fieldArray[index].PurchaseRate) * 100) / (<number>this.Rate)).toFixed(2);
          this.fieldArray[index].MeltingLoss = Math.abs(Number(<number>this.fieldArray[index].NetWt * <number>this.fieldArray[index].MeltingPercent / 100)).toFixed(3);
          this.fieldArray[index].ItemAmount = Number(<number>this.fieldArray[index].NetWt * <number>this.fieldArray[index].PurchaseRate).toFixed(2);
        }
        else if (this.fieldArray[index].ItemName == "OGCN24K") {
          this.fieldArray[index].PurchaseRate = Number(<number>this.Rate * <number>this.PurityPercent / 100).toFixed(2);
          // if (this._router.url == "/purchase") {
          //   this.fieldArray[index].PurchaseRate = Math.round(<number>this.Rate * <number>this.PurityPercent / 100);
          // }
          // else {
          //   this.fieldArray[index].PurchaseRate = Number(<number>this.Rate * <number>this.PurityPercent / 100).toFixed(2);
          // }
          this.fieldArray[index].MeltingPercent = Number(((<number>this.Rate - <number>this.fieldArray[index].PurchaseRate) * 100) / (<number>this.Rate)).toFixed(2);
          this.fieldArray[index].MeltingLoss = Math.abs(Number(<number>this.fieldArray[index].NetWt * <number>this.fieldArray[index].MeltingPercent / 100)).toFixed(3);
          this.fieldArray[index].ItemAmount = Number(<number>this.fieldArray[index].NetWt * <number>this.fieldArray[index].PurchaseRate).toFixed(2);
        }
        else {
          // this.fieldArray[index].PurchaseRate = Math.abs(Number(<number>this.Rate * <number>this.PurityPercent / 100) - Number(<number>this.Rate * <number>this.PurityPercent * 0.02 / 100)).toFixed(2);
          this.fieldArray[index].PurchaseRate = Math.abs(Number(<number>this.Rate * <number>this.PurityPercent / 100) - Number(<number>this.Rate * <number>this.PurityPercent * 0.045 / 100)).toFixed(2);
          // if (this._router.url == "/purchase") {
          //   this.fieldArray[index].PurchaseRate = Math.round(Number(<number>this.Rate * <number>this.PurityPercent / 100) - Number(<number>this.Rate * <number>this.PurityPercent * 0.02 / 100));
          // }
          // else {
          //   this.fieldArray[index].PurchaseRate = Math.abs(Number(<number>this.Rate * <number>this.PurityPercent / 100) - Number(<number>this.Rate * <number>this.PurityPercent * 0.02 / 100)).toFixed(2);
          // }
          this.fieldArray[index].MeltingPercent = Number(((<number>this.Rate - <number>this.fieldArray[index].PurchaseRate) * 100) / (<number>this.Rate)).toFixed(2);
          this.fieldArray[index].MeltingLoss = Number(<number>this.fieldArray[index].NetWt * <number>this.fieldArray[index].MeltingPercent / 100).toFixed(3);
          this.fieldArray[index].ItemAmount = Number(<number>this.fieldArray[index].NetWt * <number>this.fieldArray[index].PurchaseRate).toFixed(2);
        }
      }
      else if (this.fieldArray[index].ItemName == "BJOR" || this.fieldArray[index].ItemName == "OG22") {
        this.PurityPercent = 91.6;
        this.fieldArray[index].PurityPercent = this.PurityPercent;
        //get Melting % Percent  
        this.fieldArray[index].MeltingPercent = 0;
        //get Melting Loss
        this.fieldArray[index].MeltingLoss = 0.000;
        //Get Purchase Rate
        this.fieldArray[index].PurchaseRate = this.Rate;
        //Get Final Amount
        this.fieldArray[index].ItemAmount = Number(<number>this.Rate * <number>this.fieldArray[index].NetWt);
      }
      else if (this.fieldArray[index].ItemName == "OG18") {
        this.PurityPercent = 75;
        this.fieldArray[index].PurityPercent = this.PurityPercent;
        //get Melting % Percent  
        this.fieldArray[index].MeltingPercent = 0;
        //get Melting Loss
        this.fieldArray[index].MeltingLoss = 0.000;
        //Get Purchase Rate
        this.fieldArray[index].PurchaseRate = this.Rate;
        //Get Final Amount
        this.fieldArray[index].ItemAmount = Number(<number>this.Rate * <number>this.fieldArray[index].NetWt);
      }

      else if (this.fieldArray[index].ItemName == "BJ18K") {
        this.PurityPercent = 75;
        this.fieldArray[index].PurityPercent = this.PurityPercent;
        //get Melting % Percent  
        this.fieldArray[index].MeltingPercent = 0;
        //get Melting Loss
        this.fieldArray[index].MeltingLoss = 0.000;
        //Get Purchase Rate
        this.fieldArray[index].PurchaseRate = this.Rate;
        //Get Final Amount
        this.fieldArray[index].ItemAmount = Number(<number>this.Rate * <number>this.fieldArray[index].NetWt);
      }

      //Added on 13-Oct-2020
      else if (this.fieldArray[index].ItemName == "OSSL" || this.fieldArray[index].ItemName == "SGDE" || this.fieldArray[index].ItemName == "SLOL") {
        this.fieldArray[index].ItemAmount = Number(<number>this.fieldArray[index].PurchaseRate * <number>this.fieldArray[index].NetWt);
      }
      //Ends here
      if (this.fieldArray[index].DiamondAmount != null) {
        this.fieldArray[index].ItemAmount += this.fieldArray[index].DiamondAmount;
      }
    }
  }

  EnableEditDelbtn = {};
  EnableSaveCnlbtn = {};
  readonly = {};
  //Variable declared to take previous edited value
  CopyEditedRow: any = [];
  EnableDiamondTab: boolean = false;
  EnableStoneTab: boolean = false;

  editFieldValue(index) {
    if (this.EnableAddRow == true) {
      swal("Warning!", 'Please save the enabled item', "warning");
    }
    else {
      if (this.fieldArray[index].ItemName != "OG24K" && this.fieldArray[index].ItemName != "OGOL" && this.fieldArray[index].ItemName != "OGCN24K" && this.fieldArray[index].ItemName != "OGCN") {
        this.EnableDisablePurPer = true;
      }
      else {
        this.EnableDisablePurPer = false;
      }
      this.ItemType = this.fieldArray[index].ItemName;
      this.CopyEditedRow[index] = Object.assign({}, this.fieldArray[index]);
      this.EnableEditDelbtn[index] = false;
      this.EnableSaveCnlbtn[index] = true;
      this.readonly[index] = false;
      this.EnableAddRow = true;
      this.EnableSubmitButton = true;
      //this.EnableDisablePurRate();
      this.EnableRateTypeBasedonRoute();
      this.EditModePurchase = true;
      this.markFormGroupDirty(this.PurchaseForm);
    }
  }

  saveDataFieldValue(index) {
    if (this.fieldArray[index]["ItemName"] == null || this.fieldArray[index]["ItemName"] == 0) {
      swal("Warning!", 'Please Select ItemName from Drop Down', "warning");
    }
    else if (this.fieldArray[index]["GrossWt"] == null || this.fieldArray[index]["GrossWt"] == 0) {
      swal("Warning!", 'Please Enter Gross Wt', "warning");
    }
    else if (this.PurchaseLinesPost.PurItem != "OP" && this.PurchaseLinesPost.PurItem != "OS" && (this.fieldArray[index]["PurityPercent"] == null || this.fieldArray[index]["PurityPercent"] == 0)) {
      swal("Warning!", 'Please Enter Purity Percentage', "warning");
    }
    else if ((this.PurchaseLinesPost.PurItem == 'OP' || this.PurchaseLinesPost.PurItem == 'OS') && (this.fieldArray[index].PurchaseRate > this.Rate)) {
      swal("Warning!", 'Purchase rate should not be greater than ' + this.Rate + '.00', "warning");
      this.fieldArray[index].ItemAmount = 0;
    }
    else if (this.fieldArray[index]["ItemAmount"] == null || this.fieldArray[index]["ItemAmount"] == 0) {
      swal("Warning!", 'Amount should not empty. Please select proper item from dropdown', "warning");
    }
    else {
      this.leavePage = true;
      this.EnableEditDelbtn[index] = true;
      this.EnableSaveCnlbtn[index] = false;
      this.readonly[index] = true;
      this.EnableAddRow = false;
      this.EnableSubmitButton = false;
      // this.SendPurchaseDataToEstComp();
      this.PurchaseLinesPost.lstPurchaseEstDetailsVM[index] = this.fieldArray[index];

      //Pushing values to PurchaseLinesPost
      this.PurchaseLinesPost.TotalPurchaseAmount = this.PurchaseSummaryData.Amount;
      this.PurchaseLinesPost.GrandTotal = this.PurchaseSummaryData.Amount;
      this.PurchaseLinesPost.TodayRate = this.Rate;
      this.PurchaseLinesPost.PType = this.PurchaseForm.value.RateType;
      this.PurchaseLinesPost.PurItem = this.PurchaseForm.value.GS;
      this.PurchaseLinesPost.OperatorCode = localStorage.getItem('Login');
      //this.PurchaseLinesPost.EstNo = this.SalesEstNo;
      if (this.NewEstimation == true && this._router.url === "/purchase") {
        this.SalesEstNo = "0";
        this.PurchaseLinesPost.EstNo = this.SalesEstNo;
      }
      else {
        this.PurchaseLinesPost.EstNo = this.SalesEstNo;
      }
      this.fieldArray[index].GSCode = this.PurchaseForm.value.GS;
      this.fieldArray[index].SalCode = this.PurchaseForm.value.SalCode;
      this.DmCarats = this.fieldArray[index].Dcts;
      this.StoneCarats = this.fieldArray[index].StneWt;
      if (this.fieldArray[index].ItemName != "OG24K" && this.fieldArray[index].ItemName != "OGOL" && this.fieldArray[index].ItemName != "OGCN24K" && this.fieldArray[index].ItemName != "OGCN") {
        this.EnableDisablePurPer = true;
      }
      else {
        this.EnableDisablePurPer = false;
      }
      this.EnableDisableRateType();
      this.EditModePurchase = false;
      this.markFormGroupDirty(this.PurchaseForm);
    }
  }

  EnableDisableRateType() {
    if (this.fieldArray.length > 0) {
      this.EnableRateType = true;
    }
    else {
      this.EnableRateType = false;
    }
  }



  EnableRateTypeBasedonRoute() {
    if (this._router.url == "/purchase") {
      this.EnableDisableRateType();
    }
    if (this._router.url == "/estimation" || this._router.url == "/sales-billing") {
      this.EnableRateType = true;
      //below code added as per suggestion from sivanand
      this.RateTypeData = [
        {
          'name': 'Exchange Rate',
          'value': 'E'
        }
      ]
    }
  }



  cancelDataFieldValue(index) {
    this.EnableAddRow = false;
    if (this.CopyEditedRow[index] != null) {
      if (this.fieldArray[index].ItemName != "OG24K" && this.fieldArray[index].ItemName != "OGOL" && this.fieldArray[index].ItemName != "OGCN24K" && this.fieldArray[index].ItemName != "OGCN") {
        this.EnableDisablePurPer = true;
      }
      else {
        this.EnableDisablePurPer = false;
      }
      this.fieldArray[index] = this.CopyEditedRow[index];
      this.PurchaseLinesPost.lstPurchaseEstDetailsVM[index] = this.CopyEditedRow[index];
      this.CopyEditedRow[index] = null;
      this.EnableSaveCnlbtn[index] = false;
      this.EnableEditDelbtn[index] = true;
      this.readonly[index] = true;
      this.CopyEditedRow = [];
    }
    else {
      this.fieldArray.splice(index, 1);
      this.PurchaseLinesPost.lstPurchaseEstDetailsVM.splice(index, 1);
    }
    //this.EnableDisableRateType();
    this.EnableRateTypeBasedonRoute();
    this.EnableDisableSubmit();
    this.markFormGroupPristine(this.PurchaseForm);
    this.EditModePurchase = false;
  }

  EnableDisableSubmit() {
    if (this.fieldArray.length == 0) {
      this.EnableSubmitButton = true;
      this.EnableDropdown = false;
    }
    else {
      this.EnableSubmitButton = false;
      this.EnableDropdown = true;
    }
  }

  cancelPurchaseEst() {
    var ans = confirm("Do you want to cancel");
    if (ans) {
      if (this._router.url == '/purchase') {
        this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
          () => {
            this._router.navigate(['/purchase']);
          }
        )
      }
      else {
        this.Rate = 0.00;
        this.EnableDropdown = false;
        this.fieldArray = [];
        this.PurchaseForm.reset();
        //this._CustomerService.SendCustDataToEstComp(null);
        //this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
        this.PurchaseLinesPost = {
          CustID: 0,
          EstNo: 0,
          PurItem: null,
          TodayRate: null,
          OperatorCode: null,
          GrandTotal: null,
          PType: null,
          CompanyCode: this.ccode,
          BranchCode: this.bcode,
          TotalPurchaseAmount: null,
          lstPurchaseEstDetailsVM: [],
          PaymentVM: []
        }
        this.PurchaseForm = this.formBuilder.group({
          GS: [null, Validators.required],
          SalCode: [null, Validators.required],
          RateType: ['E', Validators.required],
          Item: [null, Validators.required],
          RatePerGram: [null, Validators.required]
        });

        this._CustomerService.cast.subscribe(
          response => {
            this.CustomerDets = response;
            if (this.CustomerDets != null) {
              this.PurchaseLinesPost.CustID = this.CustomerDets.ID;
              this.PurchaseLinesPost.OperatorCode = localStorage.getItem('Login');
              this.PurchaseLinesPost.CompanyCode = this.ccode,
                this.PurchaseLinesPost.BranchCode = this.bcode
            }
            else {
              this.PurchaseLinesPost.CustID = null;
              this.PurchaseLinesPost.OperatorCode = localStorage.getItem('Login');
              this.PurchaseLinesPost.CompanyCode = this.ccode,
                this.PurchaseLinesPost.BranchCode = this.bcode
            }
          });
        //after cancel the purhcase at estimation screen add button not enabling back 
        //but in purhase above if condition will work so make addrow to false
        this.EnableAddRow = false;
      }
    }
  }

  deleteFieldValue(index) {
    if (this.EnableAddRow == true) {
      swal("Warning!", 'Please save the enabled item', "warning");
    }
    else {
      var ans = confirm("Do you want to delete");
      if (ans) {
        //this.valueChanged(this.fieldArray[index].ItemAmount);
        this.fieldArray.splice(index, 1);
        this.PurchaseLinesPost.lstPurchaseEstDetailsVM = [];
        this.PurchaseLinesPost.lstPurchaseEstDetailsVM = this.fieldArray;
        //this.EnableDisableRateType();
        this.EnableRateTypeBasedonRoute();
        this.EnableSubmitButton = false;
        this.EnableDisableSubmit();
        this.markFormGroupDirty(this.PurchaseForm);
      }
    }
  }

  SendPurchaseDataToEstComp() {
    this._purchaseService.sendPurchaseDatatoEstComp(this.PurchaseSummaryData);
  }

  PurchaseLinesPost: any = {
    CustID: 0,
    MobileNo: null,
    EstNo: 0,
    PurItem: null,
    TodayRate: null,
    OperatorCode: null,
    GrandTotal: null,
    PType: null,
    CompanyCode: this.ccode,
    BranchCode: this.bcode,
    TotalPurchaseAmount: null,
    lstPurchaseEstDetailsVM: [],
    PaymentVM: []
  }

  //Send Object to Stone Component
  sendDataToStoneComp(form, index: number) {
    this.Index = index === this.Index ? -1 : index;
    if (this._router.url === "/purchase") {
      if (this.EnableSaveCnlbtn[index] == true) {
        swal("Warning!", 'Please save purchase lines', "warning");
        this.EnableStoneTab = false;
        this.EnableDiamondTab = false;
      }
      else if (this.fieldArray[index].StneWt == null || this.fieldArray[index].StneWt == 0) {
        swal("Warning!", 'Please enter stone carrat', "warning");
        this.EnableStoneTab = false;
        this.EnableDiamondTab = false;
      }
      else {
        this.EnableStoneTab = true;
        this._purchaseService.SendStoneDetailsFromPurchaseComp(form.lstPurchaseEstStoneDetailsVM);
        this.LinesDetails = form;
        this.EnableDiamondTab = false;
      }
    }
    else {
      if (this.SalesEstNo != "") {
        if (this.EnableSaveCnlbtn[index] == true) {
          swal("Warning!", 'Please save purchase lines', "warning");
          this.EnableStoneTab = false;
          this.EnableDiamondTab = false;
        }
        else if (this.fieldArray[index].StneWt == null || this.fieldArray[index].StneWt == 0) {
          swal("Warning!", 'Please enter stone carrat', "warning");
          this.EnableStoneTab = false;
          this.EnableDiamondTab = false;
        }
        else {
          this.EnableStoneTab = true;
          this._purchaseService.SendStoneDetailsFromPurchaseComp(form.lstPurchaseEstStoneDetailsVM);
          this.LinesDetails = form;
          this.EnableDiamondTab = false;
        }
      }
    }
  }

  //Send Object to Diamond Component
  sendDataToDiamondComp(form, index: number) {
    this.Index = index === this.Index ? -1 : index;
    if (this._router.url === "/purchase") {
      if (this.EnableSaveCnlbtn[index] == true) {
        swal("Warning!", 'Please save purchase lines', "warning");
        this.EnableDiamondTab = false;
        this.EnableStoneTab = false;
      }
      else if ((this.fieldArray[index].Dcts == null || this.fieldArray[index].Dcts == 0) && (this.fieldArray[index].ItemName != "OG24K" && this.fieldArray[index].ItemName != "OGCN" && this.fieldArray[index].ItemName != "OGCN24K")) {
        swal("Warning!", 'Please enter diamond carrat', "warning");
        this.EnableDiamondTab = false;
        this.EnableStoneTab = false;
      }
      else if (this.fieldArray[index].ItemName == "OG24K" || this.fieldArray[index].ItemName == "OGCN" || this.fieldArray[index].ItemName == "OGCN24K") {
        swal("Warning!", this.fieldArray[index].ItemName + " has no Diamond details", "warning");
        this.EnableDiamondTab = false;
        this.EnableStoneTab = false;
      }
      else {
        if (this.fieldArray[index].ItemName == "OSSL" || this.fieldArray[index].ItemName == "SGDE" || this.fieldArray[index].ItemName == "SLOL") {
          swal("Warning!", this.fieldArray[index].ItemName + ' has no Diamond details', "warning");
        }
        else {
          this.EnableDiamondTab = true;
          this._purchaseService.SendDiamondDetailsFromPurchaseComp(form.lstPurchaseEstDiamondDetailsVM);
          this.LinesDetails = form;
          this.EnableStoneTab = false;
        }
      }
    }
    else {
      if (this.SalesEstNo != "") {
        if (this.EnableSaveCnlbtn[index] == true) {
          swal("Warning!", 'Please save purchase lines', "warning");
          this.EnableDiamondTab = false;
          this.EnableStoneTab = false;
        }
        else if (this.fieldArray[index].Dcts == null || this.fieldArray[index].Dcts == 0) {
          swal("Warning!", 'Please enter diamond carrat', "warning");
          this.EnableDiamondTab = false;
          this.EnableStoneTab = false;
        }
        else {
          if (this.fieldArray[index].ItemName == "OSSL" || this.fieldArray[index].ItemName == "SGDE" || this.fieldArray[index].ItemName == "SLOL") {
            swal("Warning!", this.fieldArray[index].ItemName + ' has no Diamond details', "warning");
          }
          else {
            this.EnableDiamondTab = true;
            this._purchaseService.SendDiamondDetailsFromPurchaseComp(form.lstPurchaseEstDiamondDetailsVM);
            this.LinesDetails = form;
            this.EnableStoneTab = false;
          }
        }
      }
    }
  }


  private markFormGroupPristine(form: FormGroup) {
    Object.values(form.controls).forEach(control => {
      control.markAsPristine();

      if ((control as any).controls) {
        this.markFormGroupPristine(control as FormGroup);
      }
    });
  }

  EnableCustomerTab: boolean = false;
  ToggleCustomer: boolean = false;



  private markFormGroupDirty(form: FormGroup) {
    Object.values(form.controls).forEach(control => {
      control.markAsDirty();

      if ((control as any).controls) {
        this.markFormGroupDirty(control as FormGroup);
      }
    });
  }

  //Data visible when collapse
  ToggleCustomerData() {
    this.EnableCustomerTab = !this.EnableCustomerTab;
  }

  SavePurchaseEst() {
    let estNo;
    if ((this.PurchaseLinesPost.EstNo == null || this.PurchaseLinesPost.EstNo == 0) && (this._router.url == '/estimation')) {
      swal("Warning!", 'Please submit the sales details', "warning");
    }
    else if (this.PurchaseForm.pristine == true) {
      swal("Warning!", "No changes were made to submit", "warning");
    }
    else if ((this.PurchaseLinesPost.CustID == null || this.PurchaseLinesPost.CustID == 0) && (this._router.url == '/purchase')) {
      swal("Warning!", 'Please select the customer', "warning");
    }
    else if (this.EditModePurchase == true) {
      swal("Warning!", 'Please save item details to submit', "warning");
    }
    else if (this.PurchaseLinesPost.lstPurchaseEstDetailsVM.length == 0) {
      swal("Warning!", 'Please enter/save item details to submit', "warning");
    }

    else if (this.EnableSubmitButton == true) {
      swal("Warning!", "No changes were made to submit", "warning");
    }
    else if (this.EnableStneSubmit == true) {
      swal("Warning!", 'Please save stone details to submit', "warning");
    }
    else if (this.EnableDmdSubmit == true) {
      swal("Warning!", 'Please save diamond details to submit', "warning");
    }
    else {
      if (this.validateDiamondCarats() == true) {
        swal("Warning!", 'Please enter valid diamond details', "warning");
      }
      else {
        var ans = confirm("Do you want to save??");
        if (ans) {
          this._purchaseService.post(this.PurchaseLinesPost).subscribe(
            response => {
              estNo = response;
              swal("Saved!", "Purchase Estimation number " + estNo.EstMationNo + " Saved", "success");
              this.SendPurchaseDataToEstComp();
              if (this._router.url === "/purchase") {
                this.routerUrl = this._router.url;
                this.EnableReprint = true;
                this.ReprintData = {
                  EstNo: estNo.EstMationNo,
                  EstType: "P"
                }
                this._purchaseService.SendReprintPurchaseData(this.ReprintData);
                // this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(() =>
                //   this._router.navigate(['/purchase']))
                this.ClearValues();
                this.EnableRateType = false;
                this.leavePage = false;
                this.PurchaseForm = this.formBuilder.group({
                  GS: [null, Validators.required],
                  SalCode: [null, Validators.required],
                  RateType: ['E', Validators.required],
                  Item: [null, Validators.required],
                  RatePerGram: [null, Validators.required]
                });
                this.markFormGroupDirty(this.PurchaseForm);
                this.model.option = "New Estimation";
                this.ToggleEstimation = false;
                this.NewEstimation = true;
                this.ShowHidePurEst = true;
                this.EnableCustomerTab = false;
              }
              else {
                this.EnableReprint = false;
                this.ReprintData = {
                  EstNo: estNo.EstMationNo,
                  EstType: "P"
                }
                this._estimationService.SendReprintData(this.ReprintData);
                //$('#PurchaseEstimationPlainTextTab').modal('show');             
                //this.getPurchaseEstPlainText(estNo.EstMationNo);
                this.markFormGroupPristine(this.PurchaseForm);
              }
            }
          )
        }
      }
    }
  }
  ///////////////////////////////////////////////////////////////////////


  purchaseEstimationPlainTextDetails: any = [];

  getPurchaseEstPlainText(arg) {
    this._purchaseService.PrintPurEstDotMatrix(arg).subscribe(
      response => {
        this.purchaseEstimationPlainTextDetails = response;
        $('#PurchaseEstimationPlainTextTab').modal('show');
      }
    )
  }

  printPurchaseEstimationPlainText() {
    this._masterService.printPlainText(this.purchaseEstimationPlainTextDetails);
  }

  stwt: number = 0;
  dcts: number = 0;
  Flag: Boolean = false;

  validateDiamondCarats() {
    this.Flag = false;
    for (let i = 0; i < this.PurchaseLinesPost.lstPurchaseEstDetailsVM.length; i++) {
      this.dcts = 0;
      if (this.PurchaseLinesPost.lstPurchaseEstDetailsVM[i].lstPurchaseEstDiamondDetailsVM.length > 0) {
        for (let j = 0; j < this.PurchaseLinesPost.lstPurchaseEstDetailsVM[i].lstPurchaseEstDiamondDetailsVM.length; j++) {
          this.dcts += Number(this.PurchaseLinesPost.lstPurchaseEstDetailsVM[i].lstPurchaseEstDiamondDetailsVM[j].Carrat);
        }
        if (this.dcts < this.PurchaseLinesPost.lstPurchaseEstDetailsVM[i].Dcts || this.dcts > this.PurchaseLinesPost.lstPurchaseEstDetailsVM[i].Dcts) {
          this.Flag = true;
        }
        else {
          this.Flag = false;
        }
      }
      else if (this.PurchaseLinesPost.lstPurchaseEstDetailsVM[i].Dcts != null && this.PurchaseLinesPost.lstPurchaseEstDetailsVM[i].Dcts != 0) {
        if (this.PurchaseLinesPost.lstPurchaseEstDetailsVM[i].lstPurchaseEstDiamondDetailsVM.length == 0) {
          this.Flag = true;
        }
        else {
          this.Flag = false;
        }
      }
      else {
        this.Flag = false;
      }
    }
    if (this.Flag == true) {
      return true;
    }
    else {
      return false;
    }
  }

  rndoff: number = 0.00;
  //Calculation///////////
  GrossWtTotal(arg) {
    let total: any = 0.00; arg.forEach((d) => { total += parseFloat(d.GrossWt) });
    this.PurchaseSummaryData.GrossWt = parseFloat(total);
    this.PurchaseGrossWt.emit(parseFloat(total));
    return total;
  }

  StneWt(arg) {
    let total: any = 0.00;
    arg.forEach((d) => {
      if (d.StneWt != 0 && d.StneWt != null) {
        total += parseFloat(d.StneWt);
      }
    }); this.PurchaseSummaryData.StoneWt = parseFloat(total); return total;
  }

  Dcts(arg) { let total: any = 0.00; arg.forEach((d) => { total += parseFloat(d.Dcts); }); this.PurchaseSummaryData.carats = parseFloat(total); return total; }

  NetWt(arg) {
    let total: any = 0.00; arg.forEach((d) => { total += parseFloat(d.NetWt); });
    this.PurchaseSummaryData.NtWt = parseFloat(total);
    this.PurchaseNtWt.emit(parseFloat(total));
    return total;
  }

  Amount(arg) {
    let total: any = 0.00;
    arg.forEach((d) => { total += parseFloat(d.ItemAmount); }); this.PurchaseSummaryData.Amount = parseFloat(total); this.PurchaseAmountChanged(total); this.PurchaseLinesPost.GrandTotal = total; this.PurchaseLinesPost.TotalPurchaseAmount = total;
    this.rndoff = Math.round(total) - total;
    return total;
  }

  ClearValues() {
    this.Rate = 0.00;
    this.EnableDropdown = false;
    this.fieldArray = [];
    this.PurchaseForm.reset();
    this._CustomerService.SendCustDataToEstComp(null);
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
    this.PurchaseLinesPost = {
      CustID: 0,
      MobileNo: null,
      EstNo: 0,
      PurItem: null,
      TodayRate: null,
      OperatorCode: null,
      GrandTotal: null,
      PType: null,
      CompanyCode: this.ccode,
      BranchCode: this.bcode,
      TotalPurchaseAmount: null,
      lstPurchaseEstDetailsVM: [],
      PaymentVM: []
    }
  }
}