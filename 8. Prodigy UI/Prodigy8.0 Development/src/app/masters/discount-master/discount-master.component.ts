import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
declare var $: any;
import { AppConfigService } from '../../AppConfigService';

import * as CryptoJS from 'crypto-js';
import { MastersService } from '../masters.service';
import { OfferOptions } from '../masters.model';
import swal from 'sweetalert';
import { ToastrService } from 'ngx-toastr';
import { DatePipe } from '@angular/common'

@Component({
  selector: 'app-discount-master',
  templateUrl: './discount-master.component.html',
  styleUrls: ['./discount-master.component.css']
})
export class DiscountMasterComponent implements OnInit {
  OfferItemsForm: FormGroup;
  OfferOptionsForm: FormGroup;
  DiscModelForm: FormGroup;
  ischecked = "N";
  ccode: string;
  bcode: string;
  password: string;
  datePickerConfig: Partial<BsDatepickerConfig>;
  today = new Date();
  chkchkbxchked: boolean = false;
  EnableJson: boolean = false;
  Date: string;
  OfferStartDate: string;


  constructor(private formBuilder: FormBuilder, private toastr: ToastrService, private _mastersService: MastersService,
    private datepipe: DatePipe,
    private _appConfigService: AppConfigService

  ) {
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        minDate: this.today,
        dateInputFormat: 'dd/MM/yyyy'
      });

    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }
  GetOfferOn = [
    {
      "Id": " NG",
      "Name": "New Gold Item"
    },
    {
      "Id": "DI",
      "Name": "Stone Fitted Items"
    },
    {
      "Id": "SA",
      "Name": "Silver Article"
    },
    {
      "Id": "SO",
      "Name": "Silver Ornaments"
    },
    {
      "Id": "PL",
      "Name": "Platinum"
    }


  ]

  Types = [
    {
      "Id": "G",
      "Name": "Gram"
    },
    {
      "Id": "A",
      "Name": "Amount"
    },
    {
      "Id": "P",
      "Name": "Percentage"
    }
  ]
  OfferCoins: any = {
    OfferOnHeader: null,
    OfferTypeHeader: null,
    GsCodeHeader: null,
    CounterHeader: null,
    ItemHeader: null,

  }

  OffetOptionsListData: OfferOptions = {
    ObjID: "",
    CompanyCode: "",
    BranchCode: "",
    SRDiscount: "",
    StWtDeduction: "",
    PurDiscount: "",
    OrderDiscount: "",
    DisDCounter: "",
    DisRCounter: "",
    StartDate: "",
    EndDate: "",
    DiscountType: ""
  }

  ngOnInit() {
    this.datepipe.transform(this.OfferOptions.StartDate, 'dd/MM/yyyy');
    this.datepipe.transform(this.OfferOptions.EndDate, 'dd/MM/yyyy');
    this.GetOfferOn;
    this.getGs();
    this.getOfferOption();

    this.DiscModelForm = this.formBuilder.group({
      OfferOn: null,
      OfferType: null,
      OfferGs: null,
      OfferCounter: null,
      OfferItem: null
    });
    this.OfferOptionsForm = this.formBuilder.group({
      CompanyCode: ["", Validators.required],
      BranchCode: ["", Validators.required],
      SRDiscount: ["", Validators.required],
      StWtDeduction: ["", Validators.required],
      PurDiscount: ["", Validators.required],
      OrderDiscount: ["", Validators.required],
      DisDCounter: ["", Validators.required],
      DisRCounter: ["", Validators.required],
      OfferStartDate: null,
      OfferEndtDate: null,
      DiscountType: ["", Validators.required]
    });
  }
  GSList: any = [];
  getGs() {
    this._mastersService.GSList().subscribe(
      Response => {
        this.GSList = Response;
      }
    )
  }

  ItemLists: any = [];
  getItemsByGs(arg) {
    this._mastersService.GetItemList(arg.value.GS).subscribe(
      Response => {
        this.ItemLists = Response;

      }
    )
  }
  Add() {
    $('#DiscountMasterModel').modal('show');
  }
  getOfferOptions() {
    this.getOfferOption();
  }
  OfferOptions: any = [];
  getOfferOption() {
    this._mastersService.getOfferOptionList().subscribe(
      Response => {
        this.OfferOptions = Response;
      }
    )

  }
  checkkboxchng(e) {
    if (e.target.checked) {
      this.OfferOptions.SRDiscount = "Y";
    }
    else {
      this.OfferOptions.SRDiscount = "N";
    }
  }
  checkkboxchng1(e) {
    if (e.target.checked) {
      this.OfferOptions.PurDiscount = "Y";
    }
    else {
      this.OfferOptions.PurDiscount = "N";
    }
  }
  checkkboxchng2(e) {
    if (e.target.checked) {
      this.OfferOptions.OrderDiscount = "Y";
    }
    else {
      this.OfferOptions.OrderDiscount = "N";
    }

  }

  checkkboxchng3(e) {

    if (e.target.checked) {
      this.OfferOptions.StWtDeduction = "Y";
    }
    else {
      this.OfferOptions.StWtDeduction = "N";
    }

  }

  errors: any = []
  add() {
    this.OfferOptions.CompanyCode = this.ccode;
    this.OfferOptions.BranchCode = this.bcode;
    this.OfferOptions.DisRCounter = "N";
    this.OfferOptions.DisDCounter = "N";
    this.OfferOptions.DiscountType = "G";
    var ans = confirm("Do you want to save??");
    if (ans) {
      this._mastersService.PostOfferOptions(this.OfferOptions).subscribe(
        response => {
          swal("Saved!", "Discount details are updated Sucessfully", "success");
          this.getOfferOption();
        },
        (err) => {
          if (err.status === 400) {
            const validationError = err.error.description;
            swal("Warning!", validationError, "warning");
          }
          else {
            this.errors.push('something went wrong!');
          }
          // this.clear();
          this.getOfferOption();
        }
      )
    }
    this.getOfferOption();
  }
}







