import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl, AbstractControl } from '@angular/forms';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { Alert } from 'bootstrap';
import { UtilitiesService } from '../utilities.service'

@Component({
  selector: 'app-close-operator',
  templateUrl: './close-operator.component.html',
  styleUrls: ['./close-operator.component.css']
})
export class CloseOperatorComponent implements OnInit {

  CloseOperatorform: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  constructor(private fb: FormBuilder, private _utilitiesService: UtilitiesService, private _appConfigService: AppConfigService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();

  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
  }

}
