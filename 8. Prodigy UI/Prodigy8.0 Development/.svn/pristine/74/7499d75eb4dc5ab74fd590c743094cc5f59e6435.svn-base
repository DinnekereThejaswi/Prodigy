import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { AccountsService } from '../../accounts/accounts.service'
import { OthersService } from '../others.service';
import { formatDate } from '@angular/common';
import swal from 'sweetalert';

@Component({
  selector: 'app-chit-update',
  templateUrl: './chit-update.component.html',
  styleUrls: ['./chit-update.component.css']
})
export class ChitUpdateComponent implements OnInit {

  chitupdate: FormGroup;
  datePickerConfig: Partial<BsDatepickerConfig>;
  today = new Date();
  date: any = null;
  status: any = [
    {
      "Name": "Pending",
      "Code": true
    },
    {
      "Name": "Non-Pending",
      "Code": false
    }
  ]

  constructor(private accountservice: AccountsService,
    private fb: FormBuilder, private _othersService: OthersService) {
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        maxDate: this.today,
        dateInputFormat: 'YYYY-MM-DD'
      });
    this.chitupdate = this.fb.group({
      status: [null],
      date: null,
    });
  }

  ngOnInit() {
    this.getApplicationdate();
  }

  getApplicationdate() {
    this.accountservice.getApplicationDate().subscribe(
      response => {
        this.date = response;
        this.date = this.date.applcationDate;
        this.chitupdate.controls['date'].setValue(this.date);
      }
    )
  }

  UpdateStatus(form) {
    if (form.value.status == "true") {
      this.chitupdate.controls['status'].setValue(true);
    }
    else if (form.value.status == "false") {
      this.chitupdate.controls['status'].setValue(false);
    }
  }


  byDate(applicationDate) {
    if (applicationDate != null) {
      this.date = formatDate(applicationDate, 'yyyy-MM-dd', 'en-US');;
    }
  }

  pendingChitList: any = [];
  outputData: any = [];

  Go(form) {
    if (form.value.status == null) {
      swal("Warning!", 'Please select the status', "warning");
    }
    else {
      this._othersService.pendingChits(form.value).subscribe(response => {
        this.pendingChitList = response;
      })
    }
  }

  download(form) {
    this._othersService.downloadChits(form.value).subscribe(response => {
      this.outputData = response;
      swal("Saved!", this.outputData.Message, "success");
      this.pendingChitList = [];
      this.chitupdate.reset();
      this.getApplicationdate();
    })
  }

  billUpdate(form) {
    this._othersService.billupdate(form.value).subscribe(response => {
      this.outputData = response;
      swal("Saved!", this.outputData.Message, "success");
      this.pendingChitList = [];
      this.chitupdate.reset();
      this.getApplicationdate();
    })
  }
}