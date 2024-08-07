
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import * as CryptoJS from 'crypto-js';
import { MasterService } from '../../core/common/master.service';
import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';
import { Router } from '@angular/router';
declare var $: any;

@Component({
  selector: 'app-stone-estimation',
  templateUrl: './stone-estimation.component.html',
  styleUrls: ['./stone-estimation.component.css']
})
export class StoneEstimationComponent implements OnInit {

  // @ViewChild('Tagno', { static: true }) Tagno: ElementRef;
  StoneSalesForm: FormGroup;
  Headerform: FormGroup;
  datePickerConfig: Partial<BsDatepickerConfig>;
  EnableCustomerTab: boolean = true;
  EnablePurchaseTab: boolean = true;
  EnableSalesTab: boolean = true;

  today = new Date();
  applicationDate: any
  barcodereceiptHeaderForm: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  GetStoneBarcodeList: any = [];
  constructor(private fb: FormBuilder,
    private _appConfigService: AppConfigService, private _router: Router, private _masterService: MasterService) {
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
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this.getSalesMan();
    this.Headerform = this.fb.group({
      salesPerson: [null, Validators.required],
      estno: [null, Validators.required],
      date: [null, Validators.required],
      remarks: [null, Validators.required]
    });
    this.StoneSalesForm = this.fb.group({
      salesPerson: [null, Validators.required],
      barcode: [null, Validators.required],
      orderNo: [null, Validators.required],
      GSType: [null, Validators.required],
      ItemType: [null, Validators.required],
      batchNo: [null, Validators.required],
      qty: [null, Validators.required],
      carats: [null, Validators.required],
      weight: [null, Validators.required],
      ratePerCarat: [null, Validators.required],
      amount: [null, Validators.required]
    });
  }
  SalesManList: any;
  getSalesMan() {
    this._masterService.getSalesMan().subscribe(
      response => {
        this.SalesManList = response;
        //this.SalesPersonModel.salesPerson = "A";
      })
  }
  addBarCode(arg) {

  }
  ToggleCustomerData() {
    this.EnableCustomerTab = !this.EnableCustomerTab;
  }
  ToggleSalesDetailsTab() {
    this.EnableSalesTab = !this.EnableSalesTab;
  }
  TogglePurchaseDetailsTab() {
    this.EnablePurchaseTab = !this.EnablePurchaseTab;
  }

}
