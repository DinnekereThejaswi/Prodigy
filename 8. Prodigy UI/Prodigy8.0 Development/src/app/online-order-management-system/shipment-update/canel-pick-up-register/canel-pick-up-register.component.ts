import { Component, OnInit } from '@angular/core';
import { OnlineOrderManagementSystemService } from '../../online-order-management-system.service';
import swal from 'sweetalert';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AppConfigService } from '../../../AppConfigService';
import { MasterService } from '../../../core/common/master.service';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-canel-pick-up-register',
  templateUrl: './canel-pick-up-register.component.html',
  styleUrls: ['./canel-pick-up-register.component.css']
})
export class CanelPickUpRegisterComponent implements OnInit {
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
    this.EnableJson = this._appConfigService.EnableJson;
  }

  CancelPickUpRegisterForm: FormGroup;

  ngOnInit() {
    this.getApplicationDate();
    this.CancelPickUpRegisterForm = this.fb.group({
      tokenNo: [null],
      remarks: [null],
      registrationDate: [null],
    });
  }

  AwbNo: number = null;
  AwbOutput: any;

  applicationDate: any;
  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
      }
    )
  }

  CancelPickUpRegister(form) {
    if (form.value.tokenNo == null || form.value.tokenNo == "") {
      swal("Warning!", 'Please enter the Token No', "warning");
    }
    else if (form.value.remarks == null || form.value.remarks == "") {
      swal("Warning!", 'Please enter the Remarks', "warning");
    }
    else {
      this._onlineOrderManagementSystemService.cancelregisterpickup(form.value).subscribe(
        response => {
          swal("Saved!", "Register Pick Up has been cancelled successfully", "success");
        }
      )
    }
  }

  clearcancelpickup() {
    this.CancelPickUpRegisterForm.reset();
    this.getApplicationDate();
  }
}