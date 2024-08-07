import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MastersService } from '../masters.service';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { formatDate } from '@angular/common';
@Component({
  selector: 'app-category-wise-discount-offer',
  templateUrl: './category-wise-discount-offer.component.html',
  styleUrls: ['./category-wise-discount-offer.component.css']
})
export class CategoryWiseDiscountOfferComponent implements OnInit {
  CategoryWiseDiscountOfferform: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  isReadOnly: boolean = false;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  EnableJson: boolean = false;
  datePickerConfig: Partial<BsDatepickerConfig>;
  today = new Date();
  StartDate: any = null;
  EndDate: any = null;
  DiscountCategory =
    {
      "CompanyCode": "",
      "BranchCode": "",
      "OfferId": "",
      "StartDate": null,
      "EndDate": null,
      "BasicDiscRatePerRram": "",
      "BasicDiscRatePerCarat": "",
      "IsActive": "",
      "OperatorCode": "",
      "EntryDate": "",
      "BasicDiscVaPerentage": "",
      "DiscountCategory": [{
        "OfferId": "",
        "CatagoryCode": "",
        "CatagoryName": "",
        "AddDiscRatePerGram": "",
        "AddDiscRatePerCarat": "",
        "AddDiscVaPerentage": ""
      }],
      "CategoryGrouping": [{
        "OfferId": "",
        "CatagoryCode": "",
        "GsCode": "",
        "ItemCode": "",
        "CounterCode": ""
      }]
    }
  constructor(private _masterService: MastersService,
    private formBuilder: FormBuilder,
    private _appConfigService: AppConfigService, private fb: FormBuilder) {
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        // maxDate: this.today,
        dateInputFormat: 'YYYY-MM-DD',

      });
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8
    );
  }

  ngOnInit() {
    this.getApplicationDate();
    this.CategoryWiseDiscountOfferform = this.fb.group({
      StartDate: '',
      EndDate: '',
      RatePerGram: null,
      RatePerCarat: null,
      VAPercent: null,
      Category: null,
      GsType: null,
      Counter: null,
      Item: null

    });
  }
  byStartDate(arg) {
    if (arg != null) {
      this.DiscountCategory.StartDate = formatDate(arg, 'yyyy-MM-dd', 'en-US');
    }
  }
  byEndDate(arg) {
    if (arg != null) {
      this.DiscountCategory.EndDate = formatDate(arg, 'yyyy-MM-dd', 'en-US');
      // this.DiscountCategory.StartDate = this.applicationDate;
      // this.DiscountCategory.EndDate = this.applicationDate;
    }

  }

  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;

        this.StartDate = formatDate(appDate["applcationDate"], 'dd/MM/yyyy', 'en_GB');
        this.EndDate = formatDate(appDate["applcationDate"], 'dd/MM/yyyy', 'en_GB');
        // this.DiscountCategory.StartDate = this.applicationDate;
        // this.DiscountCategory.EndDate = this.applicationDate;
      }
    )
  }

}
