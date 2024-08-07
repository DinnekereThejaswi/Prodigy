import { AfterContentChecked, AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
declare var $: any;
import { Router } from '@angular/router';
import { BarcodedetailsService } from '../barcodedetails.service';
import swal from 'sweetalert';
import { ToastrService } from 'ngx-toastr';
@Component({
  selector: 'app-reprint-barcode',
  templateUrl: './reprint-barcode.component.html',
  styleUrls: ['./reprint-barcode.component.css']
})
export class ReprintBarcodeComponent implements OnInit, AfterViewInit {
  @ViewChild('Barcode1', { static: true }) Barcode1: ElementRef;
  @ViewChild('Barcode2', { static: true }) Barcode2: ElementRef;


  ReprintBarcodeForm: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  constructor(private _router: Router,
    private _appConfigService: AppConfigService,
    private fb: FormBuilder, private _barcodedetailsService: BarcodedetailsService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.ReprintBarcodeHeader.CompanyCode = this.ccode;
    this.ReprintBarcodeHeader.BranchCode = this.bcode;

  }
  ReprintBarcodeHeader = {
    CompanyCode: "",
    BranchCode: "",
    Barcodes: [],
    PrintType: ""
  }

  ReprintBarcodeHeaderModel = {
    Barcode1: "",
    Barcode2: ""

  }
  ngOnInit() {
    this.ReprintBarcodeForm = this.fb.group({
      Barcode1: null,
      Barcode2: null,
    });
    this.clear();
  }



  PrintTypeBasedOnConfig: any;
  ReponseData: any = [];
  postReprintBarcode() {
    this._barcodedetailsService.postReprintBarcodes(this.ReprintBarcodeHeader).subscribe(
      Response => {
        this.PrintTypeBasedOnConfig = Response;
        if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
          this.ReponseData = atob(this.PrintTypeBasedOnConfig.Data);
          this.clear();
        }
        else if (this.PrintTypeBasedOnConfig.PrintType == "" && this.PrintTypeBasedOnConfig.Data != "") {
          this.ReponseData = atob(this.PrintTypeBasedOnConfig.Data);
          this.clear();
        }
      },
      (err) => {
        if (err.status === 400) {
          const validationError = err.error;
          swal("Warning!", validationError.description, "warning").then(
            (result) => {
              if (result == true) {
                this.Barcode1.nativeElement.focus();
                // this.Barcode1.nativeElement.style.border = "1px solid red";
                //this.selectInput();
              }
            })
        }
      }
    )
  }
  selectInput() {
    const inputElem = <HTMLInputElement>this.Barcode1.nativeElement;
    inputElem.select();
  }

  FetchBarcodeNo1(arg) {
    if (arg != "" && arg != null) {
      this.Barcode2.nativeElement.focus();
    }

  }
  FetchBarcodeNo2() {

    this.getFocusto();
  }
  getFocusto() {
    document.getElementById("single").focus();
  }

  getFocus() {
    document.getElementById("double").focus();
  }
  // removefocus() {
  //   this.Barcode1.nativeElement.focus();
  // }

  left() {
    this.ReprintBarcodeHeader.CompanyCode = this.ccode;
    this.ReprintBarcodeHeader.BranchCode = this.bcode;
    this.ReprintBarcodeHeader.Barcodes = [];
    this.ReprintBarcodeHeader.PrintType = "";
    if (this.ReprintBarcodeHeaderModel.Barcode1 == "" || this.ReprintBarcodeHeaderModel.Barcode1 == null) {
      swal("Warning!", 'Please Enter Barcode No 1', "warning").then(
        (result) => {
          if (result == true) {
            this.Barcode1.nativeElement.focus();
          }
        })
    }
    else if (this.ReprintBarcodeHeaderModel.Barcode1 != null || this.ReprintBarcodeHeaderModel.Barcode1 != null) {
      this.ReprintBarcodeHeader.PrintType = "LEFT";
      if (this.ReprintBarcodeHeaderModel.Barcode1.length > 0) {
        this.ReprintBarcodeHeader.Barcodes.push(this.ReprintBarcodeHeaderModel.Barcode1);
      }
      else if (this.ReprintBarcodeHeaderModel.Barcode2.length > 0) {

        this.ReprintBarcodeHeader.Barcodes.push(this.ReprintBarcodeHeaderModel.Barcode2);

      }
      this.postReprintBarcode();
    }
  }

  right() {
    this.ReprintBarcodeHeader.CompanyCode = this.ccode;
    this.ReprintBarcodeHeader.BranchCode = this.bcode;
    this.ReprintBarcodeHeader.Barcodes = [];
    this.ReprintBarcodeHeader.PrintType = "";

    if (this.ReprintBarcodeHeaderModel.Barcode1 == "" || this.ReprintBarcodeHeaderModel.Barcode1 == null) {
      swal("Warning!", 'Please Enter Barcode No 1', "warning").then(
        (result) => {
          if (result == true) {
            this.Barcode1.nativeElement.focus();
          }
        })
    }
    else {
      this.ReprintBarcodeHeader.PrintType = "RIGHT";

      if (this.ReprintBarcodeHeaderModel.Barcode1.length > 0) {
        this.ReprintBarcodeHeader.Barcodes.push(this.ReprintBarcodeHeaderModel.Barcode1);
      }
      else if (this.ReprintBarcodeHeaderModel.Barcode2.length > 0) {
        this.ReprintBarcodeHeader.Barcodes.push(this.ReprintBarcodeHeaderModel.Barcode2);

      }
      this.postReprintBarcode();
    }

    this.Barcode1.nativeElement.focus();
  }


  double() {
    if (this.ReprintBarcodeHeaderModel.Barcode1 == "" || this.ReprintBarcodeHeaderModel.Barcode1 == null) {
      swal("Warning!", 'Please Enter Barcode No 1', "warning").then(
        (result) => {
          if (result == true) {
            this.Barcode1.nativeElement.focus();
          }
        });
    }
    else if (this.ReprintBarcodeHeaderModel.Barcode2 == "" || this.ReprintBarcodeHeaderModel.Barcode2 == null) {
      swal("Warning!", 'Please Enter Barcode No 2', "warning").then(
        (result) => {
          if (result == true) {
            this.Barcode2.nativeElement.focus();
          }
        });
    }
    else {
      this.ReprintBarcodeHeader.Barcodes = [];
      this.ReprintBarcodeHeader.PrintType = "";
      this.ReprintBarcodeHeader.CompanyCode = this.ccode;
      this.ReprintBarcodeHeader.BranchCode = this.bcode;
      this.ReprintBarcodeHeader.PrintType = "DOUBLE";
      if (this.ReprintBarcodeHeaderModel.Barcode1 != null && this.ReprintBarcodeHeaderModel.Barcode2 != null) {
        this.ReprintBarcodeHeader.Barcodes.push(this.ReprintBarcodeHeaderModel.Barcode1);
        this.ReprintBarcodeHeader.Barcodes.push(this.ReprintBarcodeHeaderModel.Barcode2);

      }
      this.postReprintBarcode();
    }

  }
  clear() {
    this.ReprintBarcodeHeader = {
      CompanyCode: "",
      BranchCode: "",
      Barcodes: [],
      PrintType: ""
    };
    this.ReprintBarcodeHeaderModel = {
      Barcode1: null,
      Barcode2: null
    };
    this.Barcode1.nativeElement.focus();
  }


  close() {
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
      () => {
        this._router.navigate(['/dashboard']);
      }
    )
  }
  ngAfterViewInit() {
    this.Barcode1.nativeElement.focus();
  }

}

