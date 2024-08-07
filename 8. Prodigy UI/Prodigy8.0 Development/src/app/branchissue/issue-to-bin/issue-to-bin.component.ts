import { Component, OnInit } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { BranchissueService } from '../branchissue.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';
declare var $: any;

@Component({
  selector: 'app-issue-to-bin',
  templateUrl: './issue-to-bin.component.html',
  styleUrls: ['./issue-to-bin.component.css']
})
export class IssueToBinComponent implements OnInit {

  datePickerConfig: Partial<BsDatepickerConfig>;
  EnableItemDetails: boolean = true;
  EnableFittedStoneDetails: boolean = true;
  EnableStoneDmddtls: boolean = true;
  today = new Date();
  applicationDate
  opgissueHeaderForm: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;
  HeaderFormModel = {
    BinBo: null,
    issueTo: null,
    issueDate: null,
    AgeValidation: false

  }

  constructor(private _branchissueService: BranchissueService, private fb: FormBuilder, private _appConfigService: AppConfigService) {
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
    this.applicationDate = this.today;
  }
  ToggleitemDetails() {
    this.EnableItemDetails = !this.EnableItemDetails;
  }
  ToggleFittedStonesDetails() {
    this.EnableFittedStoneDetails = !this.EnableFittedStoneDetails;
  }
}
