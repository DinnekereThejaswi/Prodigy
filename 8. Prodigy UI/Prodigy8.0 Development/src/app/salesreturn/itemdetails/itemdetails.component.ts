import { formatDate } from '@angular/common';
import { Validators, FormBuilder } from '@angular/forms';
import { FormGroup } from '@angular/forms';
import { SalesreturnService } from './../salesreturn.service';
import { Component, OnInit } from '@angular/core';
import { CustomerService } from './../../masters/customer/customer.service';
import { MasterService } from './../../core/common/master.service';
import swal from 'sweetalert';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';


@Component({
  selector: 'app-itemdetails',
  templateUrl: './itemdetails.component.html',
  styleUrls: ['./itemdetails.component.css']
})
export class ItemdetailsComponent implements OnInit {
  ItemDetails: any = [];
  public fieldArray: any = [];

  SRHeaderForm: FormGroup;
  radioItems: Array<string>;
  today = new Date();
  salesDate = '';
  model = { option: 'Local' };
  selectedRow: Number;
  setClickedRow: Function;
  ccode: string = "";
  bcode: string = "";
  EnableJson: boolean = false;
  password: string;
  constructor(private _SalesreturnService: SalesreturnService, private fb: FormBuilder,
    private _masterService: MasterService, private _router: Router, private _customerService: CustomerService,
    private toastr: ToastrService, private _appConfigService: AppConfigService) {
    this._SalesreturnService.cast.subscribe(
      response => {
        this.EnableJson = this._appConfigService.EnableJson;
        this.password = this._appConfigService.Pwd;
        this.getCB();
        this.radioItems = ['Other Branch', 'Local'];
        this.salesDate = formatDate(this.today, 'yyyy-MM-dd', 'en-US', '+0530');
        this.ItemDetails = response;
        if (this.ItemDetails != null) {
          this.fieldArray = this.ItemDetails.lstOfSalesReturnDetails;
          //   this.ItemDetails.BranchType = "Local";
          this.SalesReturnHeaderDetails.BilledBranch = null;
          this.SalesReturnHeaderDetails.SalCode = null;
          this.SalesReturnHeaderDetails.OperatorCode = localStorage.getItem('Login');
          this.SalesReturnHeaderDetails.CustomerID = this.ItemDetails.CustomerID;
          this.SalesReturnHeaderDetails.MobileNo = this.ItemDetails.MobileNo;
          this.SalesReturnHeaderDetails.VotureBillNo = this.ItemDetails.VotureBillNo;
          this.SalesReturnHeaderDetails.CompanyCode = this.ccode;
          this.SalesReturnHeaderDetails.BranchCode = this.bcode;
          this.totalAmtafterDiscount = 0.00;
          this.GST = 0.00;
          this.roundOffValue = 0.00;
          this.totalFinalAmt = 0.00;
          this.vpAmt = 0.00;
          this.payableAmt = 0.00;
          this.SalesReturnHeaderDetails.lstOfSalesReturnDetails = [];
          this.SelectedItemRow = [];
          this.SelectedItemLines = [];
        }
      }
    )
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this.SRHeaderForm = this.fb.group({
      BranchType: ['Local', Validators.required],
      BilledBranch: ['null', Validators.required],
      SalesBillNo: [null, Validators.required],
      Remarks: ['', Validators.required],
      SalesDate: null,
      SalCode: ['A', Validators.required],
      Rate: ['null', Validators.required],
    });
    this.getSalesMan();
    this.getBilledBranch();

    this.SRHeaderForm.controls['BilledBranch'].disable();
    //this.SRHeaderForm.controls['BranchType'] = "Local";
    this.SRHeaderForm.controls['BranchType'].setValue("Local");

  }

  BilledBranchRadio: boolean = false;
  OnRadioBtnChnge(arg) {
    if (arg == "Local") {
      this.model.option = arg;
      this.ItemDetails.BilledBranch = null;
      this.SRHeaderForm.controls['BilledBranch'].disable();
    }
    else {
      this.model.option = arg;
      this.SRHeaderForm.controls['BilledBranch'].enable();
    }
  }

  SalesReturnHeaderDetails: any = {
    CompanyCode: null,
    BranchCode: null,
    CustomerID: null,
    MobileNo: null,
    VotureBillNo: null,
    BilledBranch: null,
    SalCode: null,
    Remarks: null,
    OperatorCode: null,
    lstOfSalesReturnDetails: []
  }

  SalesManList: any;
  getSalesMan() {
    this._masterService.getSalesMan().subscribe(
      response => {
        this.SalesManList = response;
      })
  }

  BilledBranch: any;
  getBilledBranch() {
    this._SalesreturnService.getBilledBranch().subscribe(
      response => {
        this.BilledBranch = response;
      }
    )
  }

  SelectedItemRow: any = [];
  SelectedItemLines: any = [];

  //For Calculation Varible declaration
  totalFinalAmt: number;
  vpAmt: number;
  payableAmt: number;
  salesGST: number;
  roundOff: number;
  totalAmtafterDiscount: number;
  GST: number;

  AppendItemDataRow(form, index: number, event) {
    //var indexedRow = this.SelectedItemLines.findIndex(x => x.this.fieldArray==form);
    const checked = event.target.checked;
    if (checked) {
      this.SelectedItemRow = form;
      this.SelectedItemLines.push(this.SelectedItemRow);
      this.calculateFooter();
      this.SalesReturnHeaderDetails.lstOfSalesReturnDetails.push(this.SelectedItemRow);
      this.selectedRow = index;

    }
    else {
      let indexOf = this.SelectedItemLines.indexOf(form);
      this.SelectedItemLines.splice(indexOf, 1);
      this.SalesReturnHeaderDetails.lstOfSalesReturnDetails.splice(indexOf, 1);
      this.calculateFooter();
    }
  }

  plusFlag: boolean = false;
  minusFlag: boolean = false;
  roundOffValue: number = 0.00;

  calculateFooter() {
    this.totalFinalAmt = Math.round(this.SelectedItemLines.reduce((sum, item) => sum + item.ItemFinalAmount, 0));
    this.vpAmt = this.ItemDetails.VPAmount;
    this.payableAmt = Number(<number>this.totalFinalAmt - <number>this.vpAmt);
    this.totalAmtafterDiscount = this.SelectedItemLines.reduce((sum, item) => sum + item.ItemTotalAfterDiscount, 0)
    this.GST = this.SelectedItemLines.reduce((sum, item) => sum + item.SGSTAmount * 2, 0);
    this.roundOff = (this.totalFinalAmt) - (this.totalAmtafterDiscount + this.GST);
    this.roundOffValue = Math.abs(this.roundOff);
    //const total = response.carts.value.reduce((sum, item) => sum + item.Amt, 0);
  }

  stoneDiamondDetails: any;
  getStoneDiamDetails(form, index: number) {
    this.stoneDiamondDetails = form;
  }

  EnableReprintSR: boolean = false;

  submitPost(form: any) {
    if (form.value.SalCode == null) {
      swal("Warning!", 'Please Select Sales Person', "warning");
    }
    else if (this.SalesReturnHeaderDetails.lstOfSalesReturnDetails.length == 0) {
      swal("Warning!", 'Please Select Sales Return Item Details', "warning");
    }
    else {
      let estNo;
      var ans = confirm("Do you want to save the Sales return estimation??");
      if (ans) {
        this._SalesreturnService.SendSREnablePrint(false);
        this._SalesreturnService.post(this.SalesReturnHeaderDetails).subscribe(
          response => {
            estNo = response;
            swal("Saved!", "Sales Return estimation number " + estNo.SREstimationNo + " Created Successfully", "success");
            var confirmPrint = confirm("Do you want to take Sales Return estimation Print Out??");
            if (confirmPrint) {
              this.EnableReprintSR = true,
                this._SalesreturnService.SendSRNoToReprintComp(estNo.SREstimationNo);
              this._SalesreturnService.SendSRToHide(false);
              this._SalesreturnService.SendSREnablePrint(true);
              // setTimeout(()=>{
              //   this._SalesreturnService.SendSRToHide(false);
              //  },300);
            }
            else {
              this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(() =>
                this._router.navigate(['/salesreturn'])
              );
            }
          })
      }
    }
  }

  clearComponents() {
    // this.ItemDetails = [];
    // this.fieldArray = [];
    // this._SalesreturnService.sendItemDatatoItemComp(null);
    // this._customerService.SendCustDataToEstComp(null);
    // this._customerService.sendCustomerDtls_To_Customer_Component(null);

  }

  Grosswt: number = 0.000;
  AddedWt: number = 0.000;
  Ntwt: number = 0.000;
  TotAmount: number = 0.000;
  Quantity: number = 0;
  DeductedWt: number = 0.000;

  GrossWtTotal(arg) { let total: any = 0.00; arg.forEach((d) => { total += parseFloat(d.GrossWt) }); this.Grosswt = total; return total; }

  AddWt(arg) { let total: any = 0.00; arg.forEach((d) => { total += parseFloat(d.AddWt); }); this.AddedWt = total; return total; }

  DedWt(arg) { let total: any = 0.00; arg.forEach((d) => { total += parseFloat(d.DeductWt); }); this.DeductedWt = total; return total; }

  NetWt(arg) { let total: any = 0.00; arg.forEach((d) => { total += parseFloat(d.NetWt); }); this.Ntwt = total; return total; }

  Qty(arg) { let total: any = 0; arg.forEach((d) => { total += parseFloat(d.Quantity); }); this.Quantity = total; return total; }

  Amount(arg) { let total: any = 0.00; arg.forEach((d) => { total += parseFloat(d.ItemFinalAmount); }); this.TotAmount = total; return total; }

}