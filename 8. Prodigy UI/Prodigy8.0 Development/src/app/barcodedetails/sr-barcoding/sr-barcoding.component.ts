import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { BarcodedetailsService } from '../barcodedetails.service';
import { AddBarcodeService } from '../../sales/add-barcode/add-barcode.service';
import { SRModel, SRModelVM, SRstoneModel } from '../barcodedetails.model';
import { MasterService } from '../../core/common/master.service';
import swal from 'sweetalert';
import { OrdersService } from '../../orders/orders.service';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
declare var $: any;

@Component({
  selector: 'app-sr-barcoding',
  templateUrl: './sr-barcoding.component.html',
  styleUrls: ['./sr-barcoding.component.css']
})
export class SrBarcodingComponent implements OnInit {

  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  EnableItemDetails: boolean = true;
  EnableStoneDetails: boolean = true;
  // EnableDiamondDetails: boolean = true;
  SRItemForm: FormGroup;
  // SRBarcodeDetails: SRModelVM;
  SRBarcodeDetails: any = {
    AddWt: null,
    BReceiptNo: null,
    BSNo: null,
    BarcodeNo: null,
    BarcodeStoneDetails: [],
    BarcodeTransactionDetails: [],
    BatchID: null,
    BatchNo: null,
    BranchCode: null,
    CatalogID: null,
    CertificationNo: null,
    CompanyCode: null,
    ConfirmedBy: null,
    ConfirmedDate: null,
    ConfirmedWeightRead: null,
    CounterCode: null,
    CurrentWt: null,
    DaimondAmount: null,
    Date: null,
    DesignName: null,
    DesignNo: null,
    DiamondNo: null,
    GSCode: null,
    Grade: null,
    Gwt: null,
    HallmarkCharges: null,
    IsConfirmed: null,
    IssueTo: null,
    ItemName: null,
    ItemSize: null,
    ItemSizeName: null,
    Karat: null,
    LotNo: null,
    MCFor: null,
    MakingChargePerRs: null,
    MasterDesignCode: null,
    MasterDesignName: null,
    McAmount: null,
    McPerPiece: null,
    McPercent: null,
    McType: null,
    Nwt: null,
    ObjID: null,
    OldBarcodeNo: null,
    OperatorCode: null,
    OrderNo: null,
    OrderedBranchCode: null,
    OrderedCompanyCode: null,
    PartyName: null,
    PieceRate: null,
    ProdIda: null,
    ProdTagNo: null,
    ProductCode: null,
    PurDiamondAmount: null,
    PurMcAmount: null,
    PurMcGram: null,
    PurMcType: null,
    PurPurityPercentage: null,
    PurRate: null,
    PurStoneAmount: null,
    PurWastageType: null,
    PurWastageTypeValue: null,
    Qty: null,
    ReceiptType: null,
    RefNo: null,
    Remarks: null,
    SalCode: null,
    SoldFlag: null,
    SrBatchId: null,
    StoneAmount: null,
    SupplierCode: null,
    Swt: null,
    TagWt: null,
    TaggingType: null,
    TotalPurchaseMc: null,
    TotalSellingMc: null,
    UpdateOn: null,
    VendorModelNo: null,
    WastPercent: null,
    WastageGrams: null,
    WeightRead: null,
  }


  constructor(private _barcodedetailsService: BarcodedetailsService,
    private _ordersService: OrdersService,
    private _appConfigService: AppConfigService,
    private barcodeService: AddBarcodeService,
    private _masterService: MasterService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }


  ngOnInit() {
    this.getMCTypes();
    this.DiamondColor();
    this.DiamondClarity();
    this.DiamondSize();
    this.DiamondShape();
    this.DiamondCut();
    this.DiamondPolish();
    this.DiamondFluorescence();
    this.DiamondSymmetry();
    this.DiamondCertificate();
  }

  ToggleitemDetails() {
    this.EnableItemDetails = !this.EnableItemDetails;
  }

  TogglestoneDetails() {
    this.EnableStoneDetails = !this.EnableStoneDetails;
  }

  // TogglediamondDetails() {
  //   this.EnableDiamondDetails = !this.EnableDiamondDetails;
  // }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  SRItems: any = [];

  SRGetModel: SRModel = {
    CompanyCode: null,
    BranchCode: null,
    SalesBillNo: null,
    SlNo: null,
    BarcodeNo: null,
    GSCode: null,
    CounterCode: null,
    ItemCode: null,
    GrossWt: null,
    StoneWt: null,
    NetWt: null
  }

  ListSRItems() {
    $('#SRTab').modal('show');
    this.SRItemsList();
    this.SRGetModel = {
      CompanyCode: null,
      BranchCode: null,
      SalesBillNo: null,
      SlNo: null,
      BarcodeNo: null,
      GSCode: null,
      CounterCode: null,
      ItemCode: null,
      GrossWt: null,
      StoneWt: null,
      NetWt: null
    }
  }

  SRItemsList() {
    this._barcodedetailsService.getSRItems().subscribe(
      response => {
        this.SRItems = response;
      }
    )
  }


  selectedIndex: number = -1;

  onCheckboxChange(option, event, index) {
    if (event.target.checked) {
      this.SRGetModel = option;
      this.selectedIndex = index;
    }
    else {
      event.target.checked = false;
      this.SRGetModel = {
        CompanyCode: null,
        BranchCode: null,
        SalesBillNo: null,
        SlNo: null,
        BarcodeNo: null,
        GSCode: null,
        CounterCode: null,
        ItemCode: null,
        GrossWt: null,
        StoneWt: null,
        NetWt: null
      }
    }
  }

  CounterList: any = [];

  Counter() {
    this._ordersService.getCounter().subscribe(
      response => {
        this.CounterList = response;
      }
    )
  }

  EnableSR: boolean = false;

  outputSR: any = [];

  fieldArray: any = [];
  // fieldArray1: any = [];

  CopyEditedRow: any = [];

  editFieldValue(index) {
    if (this.EnableAddRow == true) {
      swal("Warning!", 'Please save the enabled item', "warning");
    }
    else {
      this.CopyEditedRow[index] = Object.assign({}, this.fieldArray[index]);
      this.EnableEditDelbtn[index] = false;
      this.EnableSaveCnlbtn[index] = true;
      if (this.fieldArray[index].StoneGSType == "STN") {
        this.readonly1[index] = true;
      }
      else {
        this.readonly1[index] = false;
      }
      this.readonly[index] = false;
      this.EnableAddRow = true;
      this.EnableSubmitButton = true;
    }
  }


  Save() {
    if (this.SRGetModel.SalesBillNo == null) {
      swal("Warning!", 'Please select atleast one SR Items', "warning");
    }
    else {
      this.fieldArray = [];
      this._barcodedetailsService.getSRBarcodeDetail(this.SRGetModel).subscribe(
        response => {
          this.SRBarcodeDetails = response;
          // console.log(this.SRBarcodeDetails);
          $('#SRTab').modal('hide');
          this.EnableSR = true;
          //this.EnableItemDetails = false;
          this.Counter();
          this.getStoneType();
          if (this.SRBarcodeDetails.BarcodeStoneDetails.length > 0) {
            this.fieldArray = this.SRBarcodeDetails.BarcodeStoneDetails;
            // console.log( this.SRBarcodeDetails.BarcodeStoneDetails );
            
            for (let { } of this.fieldArray) {
              this.count++;
              this.getStoneName(this.fieldArray[this.count - 1].StoneGSType, this.count - 1);
              this.EnableSaveCnlbtn[this.count - 1] = false;
              this.EnableEditDelbtn[this.count - 1] = true;
              this.readonly[this.count - 1] = true
              this.StoneType(this.count - 1);

              if (this.fieldArray[this.count - 1].StoneGSType == "STN") {
                this.stoneAmt += this.fieldArray[this.count - 1].Amount;
                this.fieldArray[this.count - 1].Type = "S";
                this.stncarat += this.fieldArray[this.count - 1].Carrat;
              }
              else {
                this.dmdAmt += this.fieldArray[this.count - 1].Amount;
                this.fieldArray[this.count - 1].Type = "D";
                this.dmdcarat += this.fieldArray[this.count - 1].Carrat;
              }
            }
            this.count = 0;
            this.EnableAddRow = false;
            this.EnableSubmitButton = false;
            this.EnableDropdown = false;
          }
        }
      )
    }
  }

  StoneList(GS, Type, index) {
    if (GS == "STN") {
      this._masterService.getStoneList(Type).subscribe(
        response => {
          this.stoneName[index] = response;
          this.readonly1[index] = true;
        }
      )
    }
  }

  stoneName: any = [];
  getStoneName(arg, index) {
    this.fieldArray[index].StoneType = null;
    this.fieldArray[index].Name = null;
    this.stoneName[index] = null;
    if (arg != "STN") {
      this.barcodeService.getStoneName(arg).subscribe(
        response => {
          this.stoneName[index] = response;
          this.readonly1[index] = false;
        }
      )
    }
  }

  diamondColor: any = [];

  DiamondColor() {
    this._masterService.getDiamondColor().subscribe(
      response => {
        this.diamondColor = response;
      }
    )
  }

  diamondClarity: any = [];

  DiamondClarity() {
    this._masterService.getDiamondClarity().subscribe(
      response => {
        this.diamondClarity = response;
      }
    )
  }

  diamondSize: any = [];

  DiamondSize() {
    this._masterService.getDiamondSize().subscribe(
      response => {
        this.diamondSize = response;
      }
    )
  }

  diamondShape: any = [];

  DiamondShape() {
    this._masterService.getDiamondShape().subscribe(
      response => {
        this.diamondShape = response;
      }
    )
  }

  diamondCut: any = [];

  DiamondCut() {
    this._masterService.getDiamondCut().subscribe(
      response => {
        this.diamondCut = response;
      }
    )
  }

  diamondPolish: any = [];

  DiamondPolish() {
    this._masterService.getDiamondPolish().subscribe(
      response => {
        this.diamondPolish = response;
      }
    )
  }

  diamondSymmetry: any = [];

  DiamondSymmetry() {
    this._masterService.getDiamondSymmetry().subscribe(
      response => {
        this.diamondSymmetry = response;
      }
    )
  }

  diamondFluorescence: any = [];

  DiamondFluorescence() {
    this._masterService.getDiamondFluorescence().subscribe(
      response => {
        this.diamondFluorescence = response;
      }
    )
  }


  diamondCertificate: any = [];

  DiamondCertificate() {
    this._masterService.getDiamondCertificate().subscribe(
      response => {
        this.diamondCertificate = response;
      }
    )
  }
  // stoneName1: any = [];
  // getStoneName1(arg, index) {
  //   this.barcodeService.getStoneName(arg).subscribe(
  //     response => {
  //       this.stoneName1[index] = response;
  //     }
  //   )
  // }

  StoneTypes: any = [];

  stoneDtls: SRstoneModel;
  count: number = 0;
  EnableEditDelbtn = {};
  EnableSaveCnlbtn = {};
  readonly = {};
  readonly1 = {};


  // stoneDtls1: SRstoneModel;
  // count1: number = 0;
  // EnableEditDelbtn1 = {};
  // EnableSaveCnlbtn1 = {};
  // readonly1 = {};


  //To disable the GStype/Item dropdown once row added
  EnableDropdown: boolean = false;

  //To disable and enable Addrow Button
  EnableAddRow: boolean = false;

  //To disable and enable Submit Button
  EnableSubmitButton: boolean = true;

  // //To disable the GStype/Item dropdown once row added
  // EnableDropdown1: boolean = false;

  // //To disable and enable Addrow Button
  // EnableAddRow1: boolean = false;

  // //To disable and enable Submit Button
  // EnableSubmitButton1: boolean = true;

  addrow() {
    this.stoneDtls = {
      ObjID: null,
      CompanyCode: this.ccode,
      BranchCode: this.bcode,
      SlNo: null,
      BarcodeNo: null,
      Type: "S",
      Name: null,
      Qty: null,
      Carrat: null,
      Rate: null,
      Amount: null,
      Clarity: null,
      Color: null,
      ProdIDA: null,
      ProdTagNo: null,
      OldBarcodeNo: null,
      UpdateOn: null,
      StoneType: null,
      StoneGSType: null,
      Fin_Year: null,
      UOM: null,
      PurCost: null,
      StoneCode: null,
      Shape: null,
      Cut: null,
      Polish: null,
      Symmetry: null,
      Fluorescence: null,
      Certificate: null,
      PurRate: null,
      Size: null,
    };

    this.fieldArray.push(this.stoneDtls);
    for (let { } of this.fieldArray) {
      this.count++;
      this.StoneType(this.count - 1);
    }
    this.EnableSaveCnlbtn[this.count - 1] = true;
    this.EnableEditDelbtn[this.count - 1] = false;
    this.readonly[this.count - 1] = false
    this.count = 0;
    this.EnableAddRow = true;
    this.EnableSubmitButton = true;
    this.EnableDropdown = true;
  }

  StoneType(index) {
    this._masterService.getStoneType().subscribe(
      response => {
        this.StoneTypes[index] = response;
      }
    )
  }

  // addrow1() {
  //   this.stoneDtls1 = {
  //     ObjID: null,
  //     CompanyCode: this.ccode,
  //     BranchCode: this.bcode,
  //     SlNo: null,
  //     BarcodeNo: null,
  //     Type: "D",
  //     Name: null,
  //     Qty: null,
  //     Carrat: null,
  //     Rate: null,
  //     Amount: null,
  //     Clarity: null,
  //     Color: null,
  //     ProdIDA: null,
  //     ProdTagNo: null,
  //     OldBarcodeNo: null,
  //     UpdateOn: null,
  //     StoneType: null,
  //     StoneGSType: null,
  //     Fin_Year: null,
  //     UOM: null,
  //     PurCost: null,
  //     StoneCode: null,
  //     Shape: null,
  //     Cut: null,
  //     Polish: null,
  //     Symmetry: null,
  //     Fluorescence: null,
  //     Certificate: null,
  //     PurRate: null,
  //     Size: null,
  //   };

  //   this.fieldArray1.push(this.stoneDtls1);
  //   for (let { } of this.fieldArray1) {
  //     this.count1++;
  //   }
  //   this.EnableSaveCnlbtn1[this.count1 - 1] = true;
  //   this.EnableEditDelbtn1[this.count1 - 1] = false;
  //   this.readonly1[this.count1 - 1] = false
  //   this.count1 = 0;
  //   this.EnableAddRow1 = true;
  //   this.EnableSubmitButton1 = true;
  //   this.EnableDropdown1 = true;
  // }

  stoneType: any = [];
  getStoneType() {
    this.barcodeService.getStoneType().subscribe(
      response => {
        this.stoneType = response;
      }
    )
  }

  deleteFieldValue(index) {
    if (this.EnableAddRow == true) {
      swal("Warning!", 'Please save the enabled item', "warning");
    }
    else {
      var ans = confirm("Do you want to delete??");
      if (ans) {
        this.fieldArray.splice(index, 1);
        this.SRBarcodeDetails.BarcodeStoneDetails.splice(index, 1);
        for (var field in this.fieldArray) {
          this.getStoneName(this.fieldArray[field].StoneGSType, field);
        }
      }
    }
  }

  cancelDataFieldValue(index) {
    this.EnableAddRow = false;
    if (this.CopyEditedRow[index] != null) {
      this.fieldArray[index] = this.CopyEditedRow[index];
      this.CopyEditedRow[index] = null;
      this.EnableSaveCnlbtn[index] = false;
      this.EnableEditDelbtn[index] = true;
      this.readonly[index] = true;
      this.readonly1[index] = true;
      this.CopyEditedRow = [];
    }
    else {
      this.fieldArray.splice(index, 1);
    }
  }

  GetNetWT() {
    const GrossWt = this.SRBarcodeDetails.Gwt;
    const StneWt = this.SRBarcodeDetails.Swt;
    // this.BarcodeSummaryHeader.StoneCharges = this.totalAmtStn;
    this.SRBarcodeDetails.Nwt = parseFloat((<number>GrossWt - <number>StneWt).toFixed(3));
  }

  GetAmount(index) {
    this.fieldArray[index].Amount = null;
  }

  GetRate(index) {
    this.fieldArray[index].Rate = this.fieldArray[index].Amount / <number>this.fieldArray[index].Carrat;
  }

  stoneAmt: number = 0;
  dmdAmt: number = 0;
  stncarat: number = 0;
  dmdcarat: number = 0;

  saveDataFieldValue(index) {

    if (this.fieldArray[index].StoneGSType == null) {
      swal("Warning!", 'Please select GS', "warning");
    }
    else if (this.fieldArray[index].StoneGSType == "STN" && this.fieldArray[index].StoneType == null) {
      swal("Warning!", 'Please select type', "warning");
    }
    else if (this.fieldArray[index].Name == null) {
      swal("Warning!", 'Please select Name', "warning");
    }
    else if (this.fieldArray[index].Carrat == null) {
      swal("Warning!", 'Please select Carat', "warning");
    }
    else if (this.fieldArray[index].Amount == null) {
      swal("Warning!", 'Please enter Selling Amount', "warning");
    }
    else {
      this.stoneAmt = 0;
      this.dmdAmt = 0;
      this.stncarat = 0;
      for (var field in this.fieldArray) {
        if (this.fieldArray[field].StoneGSType == "STN") {
          this.stoneAmt += this.fieldArray[field].Amount;
          this.fieldArray[field].Type = "S";
          this.stncarat += this.fieldArray[field].Carrat;
        }
        else {
          this.dmdAmt += this.fieldArray[field].Amount;
          this.fieldArray[field].Type = "D";
          this.dmdcarat += this.fieldArray[field].Carrat;
        }
      }
      this.SRBarcodeDetails.BarcodeStoneDetails[index] = this.fieldArray[index];
      this.SRBarcodeDetails.StoneAmount = this.stoneAmt;
      this.SRBarcodeDetails.DaimondAmount = this.dmdAmt;
      this.EnableEditDelbtn[index] = true;
      this.EnableSaveCnlbtn[index] = false;
      this.readonly[index] = true;
      this.readonly1[index] = true;
      this.EnableAddRow = false;
      this.EnableSubmitButton = false;
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


  // saveDataFieldValue1(index) {
  //   this.SRBarcodeDetails.BarcodeStoneDetails.push(this.fieldArray1[index]);
  //   for (var field in this.fieldArray1) {
  //     this.dmdAmt += this.fieldArray1[field].Amount;
  //   }
  //   this.SRBarcodeDetails.DaimondAmount = this.dmdAmt;
  //   this.EnableEditDelbtn1[index] = true;
  //   this.EnableSaveCnlbtn1[index] = false;
  //   this.readonly1[index] = true;
  //   this.EnableAddRow1 = false;
  //   this.EnableSubmitButton1 = false;
  // }

  // deleteFieldValue1(index) {
  //   if (this.EnableAddRow1 == true) {
  //     swal("Warning!", 'Please save the enabled item', "warning");
  //   }
  //   else {
  //     var ans = confirm("Do you want to delete??");
  //     if (ans) {
  //       this.fieldArray1.splice(index, 1);
  //       this.SRBarcodeDetails.BarcodeStoneDetails.splice(index, 1);
  //       for (var field in this.fieldArray1) {
  //         this.getStoneName1(this.fieldArray1[field].StoneGSType, field);
  //       }
  //     }
  //   }
  // }

  // cancelDataFieldValue1(index) {
  //   this.EnableAddRow1 = false;
  //   this.fieldArray1.splice(index, 1);
  // }

  Submit() {
    var ans = confirm("Do you want to submit??");
    if (ans) {
      this._barcodedetailsService.postSRBarcoding(this.SRBarcodeDetails, this.SRGetModel.SalesBillNo).subscribe(
        response => {
          this.outputSR = response;
          swal("Saved!", this.outputSR.Message, "success");
          this.EnableSR = false;
        }
      )
    }
  }
}