import { Component, OnInit } from '@angular/core';
import { OnlineOrderManagementSystemService } from '../../online-order-management-system.service';
import swal from 'sweetalert';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AppConfigService } from '../../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import { MasterService } from '../../../core/common/master.service';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import * as moment from 'moment';

@Component({
  selector: 'app-pick-up-register',
  templateUrl: './pick-up-register.component.html',
  styleUrls: ['./pick-up-register.component.css']
})
export class PickUpRegisterComponent implements OnInit {

  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  datePickerConfig: Partial<BsDatepickerConfig>;

  constructor(private _onlineOrderManagementSystemService: OnlineOrderManagementSystemService,
    private fb: FormBuilder, private _appConfigService: AppConfigService,
    private _masterService: MasterService) {
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        dateInputFormat: 'dd/MM/yyyy hh:mm'
      });
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }

  PickUpRegisterForm: FormGroup;

  PickUpRegisterModel: any = {
    CompanyCode: null,
    BranchCode: null,
    OrderNo: null,
    ContactPerson: null,
    ContactPersonMobileNo: null,
    PickupDate: null,
  }

  AWBNo: number = null;
  AwbOutput: any;

  ngOnInit() {
    this.getApplicationDate();
    this.PickUpRegisterForm = this.fb.group({
      OrderNo: [null],
      AWBNo: [null],
      ContactPerson: [null],
      ContactPersonMobileNo: [null],
      PickupDate: [null]
    });
  }

  getAwb(form) {
    this._onlineOrderManagementSystemService.getAwb(form.value.OrderNo).subscribe(
      response => {
        this.AwbOutput = response;
        this.AWBNo = this.AwbOutput.DocumentNo;
        this.PickUpRegisterForm.controls.AWBNo.setValue(this.AWBNo);
      },
      (err) => {
        this.PickUpRegisterForm.controls.AWBNo.setValue(this.AWBNo);
      }
    )
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

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.PickUpRegisterModel.CompanyCode = this.ccode;
    this.PickUpRegisterModel.BranchCode = this.bcode;
  }

  time: string = "";
  date: string = "";

  byDate(applicationDate) {
    this.date = new Date(applicationDate).toLocaleDateString();
    this.date = moment(this.date, 'MM/DD/YYYY').format('YYYY-MM-DD');
    this.time = new Date(applicationDate).toLocaleTimeString();
    this.PickUpRegisterModel.PickupDate = this.date + " " + this.time;
  }

  outputData: any;

  pickUpRegister() {
    if (this.PickUpRegisterModel.OrderNo == null || this.PickUpRegisterModel.OrderNo == "") {
      swal("Warning!", 'Please enter the Order No', "warning");
    }
    else if (this.PickUpRegisterModel.ContactPerson == null || this.PickUpRegisterModel.ContactPerson == "") {
      swal("Warning!", 'Please enter the Contact Person', "warning");
    }
    else if (this.PickUpRegisterModel.ContactPersonMobileNo == null || this.PickUpRegisterModel.ContactPersonMobileNo == "") {
      swal("Warning!", 'Please enter the Contact Person Mobile No', "warning");
    }
    else if (this.PickUpRegisterModel.PickupDate == null || this.PickUpRegisterModel.PickupDate == "") {
      swal("Warning!", 'Please enter the Pickup Date', "warning");
    }
    else {
      this._onlineOrderManagementSystemService.registerpickup(this.PickUpRegisterModel).subscribe(
        response => {
          this.outputData = response;
          swal("Saved!", this.outputData.Message, "success");
          this.PickUpRegisterForm.reset();
          this.getApplicationDate();
        }
      )
    }
  }

  clearPickUpRegister() {
    this.PickUpRegisterForm.reset();
    this.getApplicationDate();
  }
}