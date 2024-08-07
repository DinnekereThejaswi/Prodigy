import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { OnlineOrderManagementSystemService } from '../../online-order-management-system/online-order-management-system.service';
import { CreatePackageModel } from '../../online-order-management-system/online-order-management-system.model';
import { AppConfigService } from '../../AppConfigService';
import { FormBuilder, FormGroup } from '@angular/forms';
import swal from 'sweetalert';
import * as CryptoJS from 'crypto-js';
import { NgxSpinnerService } from "ngx-spinner";


@Component({
  selector: 'app-item-packaging',
  templateUrl: './item-packaging.component.html',
  styleUrls: ['./item-packaging.component.css']
})
export class ItemPackagingComponent implements OnInit {

  password: string;
  EnableJson: boolean = false;
  OTLForm: FormGroup;
  ccode: string = "";
  bcode: string = "";
  AssignmentNo: string = null;


  constructor(private _OMSService: OnlineOrderManagementSystemService,
    private _appConfigService: AppConfigService, private fb: FormBuilder,
    private SpinnerService: NgxSpinnerService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.postcreatePackage.CompanyCode = this.ccode;
    this.postcreatePackage.BranchCode = this.bcode;
  }

  ngOnInit() {
    this.getPickListNo();
    this.packageMaster();
    this.OTLForm = this.fb.group({
      OTLNO: [null],
    });
  }

  orderStage: string = "pack";
  PickListNumber: any = [];


  getPickListNo() {
    this._OMSService.getPickListNumber(this.orderStage).subscribe(
      response => {
        this.PickListNumber = response;
        this.OrdersForPackingDets = [];
        this.packageMaster();
        this.selectedIndex = -1;
        this.OTLList = [];
      }
    )
  }

  refreshPickListNo() {
    this._OMSService.getPickListNumber(this.orderStage).subscribe(
      response => {
        this.PickListNumber = response;
      });
  }


  otlAttribute: any = {
    OTLNO: null
  }

  OrdersForPackingDets: any = [];

  packageMasterList: any = [];

  PackageCode: string = null;

  postcreatePackage: CreatePackageModel = {
    CompanyCode: "",
    BranchCode: "",
    OrderNo: 0,
    OrderReferenceNo: "",
    ItemName: "",
    BarcodeNo: "",
    Qty: 0,
    MarketplaceSlNo: 0,
    CentralRefNo: 0,
    PackageCode: null,
    OTLNo: "",
    Length: 0,
    LengthUom: "",
    Width: 0,
    WidthUom: "",
    Height: 0,
    HeightUom: "",
    Weight: 0,
    WeightUom: "",
    PackageID: "",
    OrderSource: "",
  }

  OTLList: any = [];

  GetOrdersForPacking() {
    this.AssignmentNo = null;
    this.OrdersForPackingDets = [];
    this._OMSService.getOrdersForPacking(this.AssignmentNo).subscribe(
      response => {
        this.OrdersForPackingDets = response;
        this.packageMaster();
        this.selectedIndex = -1;
        this.OTLList = [];
      }
    )
  }

  OrdersForPacking() {
    if (this.AssignmentNo == null) {
      swal("Warning!", 'Please select the Assignment No', "warning");
    }
    else {
      this.OrdersForPackingDets = [];
      this._OMSService.getOrdersForPacking(this.AssignmentNo).subscribe(
        response => {
          this.OrdersForPackingDets = response;
          // console.log(this.OrdersForPackingDets);
          this.packageMaster();
          // console.log(this.OrdersForPackingDets);
          this.selectedIndex = -1;
          this.OTLList = [];
        }
      )
    }
  }

  packageMaster() {
    this._OMSService.getPackageMaster().subscribe(response => {
      this.packageMasterList = response;
      if (this.packageMasterList.length == 1) {
        //this.postcreatePackage.PackageCode = this.packageMasterList[0].Code;
        this.PackageCode = this.packageMasterList[0].Code;;
        this.PackageMasterAttributes();
      }
    })
  }

  PackageMasterAttributesDets: any = [];

  PackageMasterAttributes() {
    if (this.PackageCode == null || this.PackageCode == "null") {
      this.postcreatePackage.PackageCode = null;
      this.postcreatePackage.Weight = 0;
    }
    else {
      if (this.packageMasterList.length == 1) {
        this._OMSService.getPackageMasterAttributes(this.packageMasterList[0].Code).subscribe(
          response => {
            this.PackageMasterAttributesDets = response;
            this.postcreatePackage.Weight = this.PackageMasterAttributesDets.Weight;
          }
        )
      }
      else {
        this.PackageMasterAttributesDets = [];
        this.postcreatePackage.Length = 0;
        this.postcreatePackage.LengthUom = "";
        this.postcreatePackage.Width = 0;
        this.postcreatePackage.WidthUom = "";
        this.postcreatePackage.Height = 0;
        this.postcreatePackage.HeightUom = "";
        this.postcreatePackage.Weight = 0;
        this.postcreatePackage.WeightUom = "";
        this.postcreatePackage.PackageCode = null;
      }
    }
  }

  PackageMasterAttributesForExisting() {
    this._OMSService.getPackageMasterAttributes(this.postcreatePackage.PackageCode).subscribe(
      response => {
        this.PackageMasterAttributesDets = response;
        this.postcreatePackage.Length = this.PackageMasterAttributesDets.Length;
        this.postcreatePackage.LengthUom = this.PackageMasterAttributesDets.LenghUoM;
        this.postcreatePackage.Width = this.PackageMasterAttributesDets.Width;
        this.postcreatePackage.WidthUom = this.PackageMasterAttributesDets.WidthUoM;
        this.postcreatePackage.Height = this.PackageMasterAttributesDets.Height;
        this.postcreatePackage.HeightUom = this.PackageMasterAttributesDets.HeightUoM;
        this.postcreatePackage.WeightUom = this.PackageMasterAttributesDets.WeightUoM;
      }
    )
  }

  addOTLNo(form) {
    if (form.value.OTLNO == null) {
      swal("Warning!", 'Please enter the OTLNo', "warning");
    }
    else {
      let data = this.OTLList.find(x => x.OTLNO == form.value.OTLNO);
      if (data == null) {
        this.otlAttribute.OTLNO = form.value.OTLNO.toUpperCase();
        this.OTLList.push(this.otlAttribute);
        this.otlAttribute = {
          OTLNO: null
        }
        this.OTLForm.reset();
        this.OTLListByPipe();
      }
      else {
        swal("Warning!", "OTLNo already added", "warning");
      }
    }
  }

  clear() {
    this.OTLForm.reset();
  }

  selectedIndex: number = -1;
  splitOTLNo: any = [];

  onCheckboxChange(option, event, index) {
    if (event.target.checked) {
      this.selectedIndex = index;
      this.postcreatePackage = option;
      if (option.PackageCode != "") {
        this.OTLList = [];
        this.PackageCode = option.PackageCode;
        this.PackageMasterAttributesForExisting();
        this.splitOTLNo = this.postcreatePackage.OTLNo.split('|');
        for (let j = 0; j < this.splitOTLNo.length; j++) {
          this.otlAttribute.OTLNO = this.splitOTLNo[j];
          this.OTLList.push(this.otlAttribute);
          this.otlAttribute = {
            OTLNO: null
          }
        }
      }
      else {
        this.OTLList = [];
        this.packageMaster();
      }
    }
    else {
      this.OTLList = [];
      event.target.checked = false;
      this.selectedIndex = -1;
    }
  }

  deleteOTLNo(index) {
    this.OTLList.splice(index, 1);
    this.OTLListByPipe();
  }

  clsAlphaNoOnly(e) {  // Accept only alpha numerics, no special characters 
    var regex = new RegExp("^[a-zA-Z0-9 ]+$");
    var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
    if (regex.test(str)) {
      return true;
    }
    e.preventDefault();
    return false;
  }

  OTLListByPipe() {
    this.postcreatePackage.OTLNo = "";
    for (let i = 0; i < this.OTLList.length; i++) {
      this.postcreatePackage.OTLNo += this.OTLList[i].OTLNO + "|";
    }
    this.postcreatePackage.OTLNo = this.postcreatePackage.OTLNo.substring(0, this.postcreatePackage.OTLNo.length - 1);
    return this.postcreatePackage.OTLNo;
  }

  outputData: any = [];

  AddPackage() {
    if (this.selectedIndex == -1) {
      swal("Warning!", 'Please select atleast one item for packing', "warning");
    }
    else if (this.PackageCode == null || this.PackageCode == "") {
      swal("Warning!", 'Please select the packing box', "warning");
    }
    else if (this.postcreatePackage.Weight == 0 || this.postcreatePackage.Weight == null) {
      swal("Warning!", 'Please enter the package material weight', "warning");
    }
    else if (this.postcreatePackage.OTLNo == "") {
      swal("Warning!", 'Please enter the OTL No', "warning");
    }
    else {
      var ans = confirm("Do you want to submit?");
      if (ans) {
        this.SpinnerService.show();
        this.postcreatePackage.Length = this.PackageMasterAttributesDets.Length;
        this.postcreatePackage.LengthUom = this.PackageMasterAttributesDets.LenghUoM;
        this.postcreatePackage.Width = this.PackageMasterAttributesDets.Width;
        this.postcreatePackage.WidthUom = this.PackageMasterAttributesDets.WidthUoM;
        this.postcreatePackage.Height = this.PackageMasterAttributesDets.Height;
        this.postcreatePackage.HeightUom = this.PackageMasterAttributesDets.HeightUoM;
        this.postcreatePackage.WeightUom = this.PackageMasterAttributesDets.WeightUoM;
        this.postcreatePackage.PackageCode = this.PackageCode;
        this._OMSService.createPackage(this.postcreatePackage).subscribe(
          response => {
            this.outputData = response;
            swal("Saved!", "Package has been created successfully", "success");
            this.SpinnerService.hide();
            //this.OrdersForPackingDets = [];
            this.selectedIndex = -1;
            this.PackageMasterAttributesDets = [];
            this.OTLList = [];
            this.postcreatePackage = {
              CompanyCode: "",
              BranchCode: "",
              OrderNo: 0,
              OrderReferenceNo: "",
              ItemName: "",
              BarcodeNo: "",
              Qty: 0,
              MarketplaceSlNo: 0,
              CentralRefNo: 0,
              PackageCode: null,
              OTLNo: "",
              Length: 0,
              LengthUom: "",
              Width: 0,
              WidthUom: "",
              Height: 0,
              HeightUom: "",
              Weight: 0,
              WeightUom: "",
              PackageID: "",
              OrderSource: "",
            }
            this.getCB();
          },
          (err) => {
            this.SpinnerService.hide();
          }
        )
      }
    }
  }
}