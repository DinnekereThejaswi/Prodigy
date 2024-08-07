import { Component, OnInit } from '@angular/core';
import { OnlineOrderManagementSystemService } from '../online-order-management-system.service';
import swal from 'sweetalert';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import { MasterService } from '../../core/common/master.service';
import { formatDate } from '@angular/common';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-shipment-update',
  templateUrl: './shipment-update.component.html',
  styleUrls: ['./shipment-update.component.css']
})
export class ShipmentUpdateComponent implements OnInit {

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
        dateInputFormat: 'DD/MM/YYYY'
      });
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }

  EnableAWBPrint: boolean = true;
  EnablePickUpRegister: boolean = true;
  EnableCancelRegisterPickUp: boolean = true;

  ngOnInit() {
  }

  ToggleAWBPrint() {
    this.EnableAWBPrint = !this.EnableAWBPrint;
  }

  TogglePickUpRegister() {
    this.EnablePickUpRegister = !this.EnablePickUpRegister;
  }

  ToggleCancelRegisterPickUp() {
    this.EnableCancelRegisterPickUp = !this.EnableCancelRegisterPickUp;
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }
}