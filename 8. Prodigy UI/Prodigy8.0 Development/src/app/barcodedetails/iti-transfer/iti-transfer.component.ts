import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { OrdersService } from '../../orders/orders.service';
import { AccountsService } from '../../accounts/accounts.service'
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { AppConfigService } from '../../AppConfigService';
import { MasterService } from '../../core/common/master.service';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { FormBuilder, FormGroup } from '@angular/forms';
import { BarcodedetailsService } from '../barcodedetails.service';
import { Router } from '@angular/router';
import { AddBarcodeService } from '../../sales/add-barcode/add-barcode.service';

@Component({
  selector: 'app-iti-transfer',
  templateUrl: './iti-transfer.component.html',
  styleUrls: ['./iti-transfer.component.css']
})
export class ItiTransferComponent implements OnInit {
  @ViewChild("BarcodeText", { static: true }) BarcodeText: ElementRef;
  ITIForm: FormGroup;
  datePickerConfig: Partial<BsDatepickerConfig>;
  GsList: any = [];
  applicationDate: any;
  today = new Date();
  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  constructor(private _ordersService: OrdersService,
    private accountservice: AccountsService,
    private _appConfigService: AppConfigService,
    private fb: FormBuilder, private _barcodedetailsService: BarcodedetailsService,
    private _router: Router, private _masterService: MasterService,
    private barcodeService: AddBarcodeService) {
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        maxDate: this.today,
        dateInputFormat: 'YYYY-MM-DD'
      });
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }

  BarcodePost: any = {
    CompanyCode: null,
    BranchCode: null,
    TransferToGSCode: null,
    TransferToItemCode: null,
    Remarks: null,
    BarcodeLines: []
  }


  ngOnInit() {
    this.getGSList();
    this.ITIForm = this.fb.group({
      GSCode: [null],
      Item: [null],
      Barcode: [null]
    });
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

  EnableBarcodeDetails: boolean = false;

  ToggleBarcodeDetails() {
    this.EnableBarcodeDetails = !this.EnableBarcodeDetails;
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.BarcodePost.CompanyCode = this.ccode;
    this.BarcodePost.BranchCode = this.bcode;
  }

  BarcodeList: any = [];
  BarcodeDetails: any = [];

  getBarcodeDetails(form) {
    if (form.value.GSCode == null) {
      swal("Warning!", 'Please select the GS', "warning");
    }
    else if (form.value.Item == null) {
      swal("Warning!", 'Please select the Item', "warning");
    }
    else if (form.value.Barcode == null || form.value.Barcode == "") {
      swal("Warning!", 'Please enter the Barcode', "warning");
    }
    else {
      let data = this.BarcodeList.find(x => x.BarcodeNo == form.value.Barcode);
      if (data == null) {
        this._barcodedetailsService.getITIBarcodeDetail(form.value).subscribe(
          response => {
            this.BarcodeDetails = response;
            this.BarcodeList.push(this.BarcodeDetails);
            this.BarcodePost.BarcodeLines = [];
            this.BarcodePost.BarcodeLines = this.BarcodeList;
            this.ITIForm.controls["Barcode"].setValue("");
          }
        )
      }
      else {
        swal("Warning!", "Barcode number already scanned", "warning");
        const inputElem = <HTMLInputElement>this.BarcodeText.nativeElement;
        inputElem.select();
      }
    }
  }

  deleteFieldValue(index) {
    var ans = confirm("Do you want to delete??");
    if (ans) {
      this.BarcodeList.splice(index, 1);
      this.BarcodePost.BarcodeLines = [];
      this.BarcodePost.BarcodeLines = this.BarcodeList;
    }
  }

  outputData: any;

  Submit() {
    if (this.BarcodePost.BarcodeLines.length == 0) {
      swal("Warning!", 'No items found to transfer', "warning");
    }
    else {
      var ans = confirm("Do you want to submit??");
      if (ans) {
        this._barcodedetailsService.postITIBarcodeDetail(this.BarcodePost).subscribe(
          response => {
            this.outputData = response;
            swal("Saved!", this.outputData.Message, "success");
            this.ITIForm.reset();
            this.BarcodeList = [];
            this.BarcodePost = {
              CompanyCode: null,
              BranchCode: null,
              TransferToGSCode: null,
              TransferToItemCode: null,
              Remarks: null,
              BarcodeLines: []
            }
            this.getCB()
          }
        )
      }
    }
  }

  Cancel() {
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
      () => {
        this._router.navigate(['/barcode/iti-transfer']);
      }
    )
  }


  Qty(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      if (d.Qty != null && d.Qty != 0) {
        total += Number(<number>d.Qty);
      }
    });
    return total;
  }

  GrWt(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      if (d.GrossWt != null && d.GrossWt != 0) {
        total += Number(<number>d.GrossWt);
      }
    });
    return total;
  }

  NtWt(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      if (d.NetWt != null && d.NetWt != 0) {
        total += Number(<number>d.NetWt);
      }
    });
    return total;
  }

}