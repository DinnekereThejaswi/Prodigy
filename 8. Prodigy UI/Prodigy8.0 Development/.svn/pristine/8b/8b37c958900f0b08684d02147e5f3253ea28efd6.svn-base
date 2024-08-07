import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import * as CryptoJS from 'crypto-js';
import { MastersService } from '../masters.service';
import swal from 'sweetalert';
import { Router } from '@angular/router';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { ModelError } from '../masters.model';
import { formatDate } from '@angular/common';
import { MasterService } from './../../core/common/master.service';
import "rxjs/Rx";
import { AppConfigService } from '../../AppConfigService';
import { DatePipe } from '@angular/common'
declare var $: any;

@Component({
  selector: 'app-daily-rates',
  templateUrl: './daily-rates.component.html',
  styleUrls: ['./daily-rates.component.css']
})

export class DailyRatesComponent implements OnInit {

  ccode: string;
  bcode: string;
  password: string;
  DailyRate: any = [];
  EnableSalesRateTab: boolean = true;
  EnablePurchaseRateTab: boolean = true;
  RateForm: FormGroup;
  EnableDate: boolean = true;
  EnableJson: boolean = false;
  errors: any = [];
  ModelError: ModelError[];
  @ViewChild("Pwd", { static: true }) Pwd: ElementRef;
  public Index: number = -1;
  PasswordValid: string;
  datePickerConfig: Partial<BsDatepickerConfig>;
  today = new Date();
  performDayend: boolean = false;
  applicationDate: any = null;

  constructor(private _masterService: MastersService,
    private formBuilder: FormBuilder,
    private _appConfigService: AppConfigService, private _router: Router, private _masterServices: MasterService
    , private datepipe: DatePipe) {
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        // maxDate: this.today,
        dateInputFormat: 'YYYY-MM-DD',

      });
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.PasswordValid = this._appConfigService.RateEditCode.DailyRatesPermission;
    this.getCB();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8
    );
  }

  HeaderDetails: any = {
    Date: null,
  }

  ngOnInit() {
    this.ValidatePermission();
    this.getApplicationDate();
    this.getDailyRates();
    this.RateForm = this.formBuilder.group({
      applicationDate: null,
    });
    this.ModelError = [];
  }

  ValidatePermission() {
    $("#PermissonModal").modal('show');
    this.Pwd.nativeElement.value = "";
  }

  passWord(arg) {
    if (arg == "") {
      swal("Warning!", "Please Enter the Password", "warning");
      $('#PermissonModal').modal('show');
    }
    else {
      this.permissonModel.CompanyCode = this.ccode;
      this.permissonModel.BranchCode = this.bcode;
      this.permissonModel.PermissionID = this.PasswordValid;
      this.permissonModel.PermissionData = btoa(arg);
      this._masterServices.postelevatedpermission(this.permissonModel).subscribe(
        response => {
          $('#PermissonModal').modal('hide');
          this.Index = -1;
          this.EnableSalesRateTab = false;
          this.EnablePurchaseRateTab = false;
        }
      )
    }
  }

  permissonModel: any = {
    CompanyCode: null,
    BranchCode: null,
    PermissionID: null,
    PermissionData: null
  }

  byDate(applicationDate) {
    if (applicationDate != null) {
      this.applicationDate = formatDate(applicationDate, 'yyyy-MM-dd', 'en-US');
      this.DailyRate.Date = applicationDate;
    }
  }

  getApplicationDate() {
    this._masterServices.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
      }
    )
  }

  ToggleSalesRate() {
    this.EnableSalesRateTab = !this.EnableSalesRateTab;
  }

  TogglePurchase() {
    this.EnablePurchaseRateTab = !this.EnablePurchaseRateTab;
  }

  getDailyRates() {
    this._masterService.GetDailyRate().subscribe(
      Response => {
        this.DailyRate = Response;
      }
    )
  }

  UpdateSalesRate(index, rate) {
    this.DailyRate.SellingRates[index].SellingBoardRate = rate;
  }

  UpdatePurchaseExchngRate(purIndex, PurExchngchaseRate) {
    this.DailyRate.PurchaseRates[purIndex].ExchangeRate = PurExchngchaseRate;
  }

  UpdatePurchaseRate(purIndex, PurchaseRate) {
    this.DailyRate.PurchaseRates[purIndex].CashRate = PurchaseRate;
  }

  UpdateDailyRates(form) {
    var date: string;
    date = form.value.applicationDate;
    this.ModelError = [];
    this.errors = [];
    var ans = confirm("Do you want to save??");
    if (ans) {
      this.DailyRate.Date = this.datepipe.transform(date, 'MM/dd/yyyy');
      this._masterService.postDailyRate(this.DailyRate, this.performDayend)
        .subscribe(
          response => {
            swal("success!", "Rate Updated successfully", "success");
          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error;
              this.ModelError = [];
              this.errors = [];
              for (const errors in validationError.ModelState) {
                if (validationError.ModelState[errors]) {
                  this.errors.push(new ModelError(null, errors.substr(0), validationError.ModelState[errors][0]));
                }
              }
            }
          }
        )
    }
  }

  public findErrorByField(modelField: string, index?: number): string {
    const modelErr = this.errors.find(m => (m.field === "dailyRates.SellingRates[" + index + "]." + modelField));
    if (modelErr != null) {
      return modelErr.description;
    } else {
      return '';
    }
  }

  public findError(modelField: string, index?: number): string {
    const modelErr = this.errors.find(m => (m.field === "dailyRates.PurchaseRates[" + index + "]." + modelField));
    if (modelErr != null) {
      return modelErr.description;
    } else {
      return '';
    }
  }

  BackTodashBoard() {
    this._router.navigate(['/dashboard']);
  }
}