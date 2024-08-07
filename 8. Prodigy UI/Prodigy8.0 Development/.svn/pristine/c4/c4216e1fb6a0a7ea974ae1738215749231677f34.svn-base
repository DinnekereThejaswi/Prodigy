import { Component, OnInit } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { formatDate } from '@angular/common';
import { MasterService } from './../../core/common/master.service';
import { AccountsService } from '../accounts.service';
import swal from 'sweetalert';

@Component({
  selector: 'app-accounts-update',
  templateUrl: './accounts-update.component.html',
  styleUrls: ['./accounts-update.component.css']
})
export class AccountsUpdateComponent implements OnInit {

  datePickerConfig: Partial<BsDatepickerConfig>;
  today = new Date();
  ordDate = '';
  constructor(private _masterService: MasterService,
    private _accountsService: AccountsService) {
    this.ordDate = formatDate(this.today, 'MM-DD-YYYY', 'en-US', '+0530');
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        maxDate: new Date(2020, 7, 28),
        dateInputFormat: 'YYYY-MM-DD'
      });
  }

  ngOnInit() {
    this.getApplicationDate();
  }

  applicationDate: any;
  
  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = formatDate(appDate["applcationDate"], 'dd/MM/yyyy', 'en_GB');
      }
    )
  }

  output: any;

  UpdateAccounts() {
    var ans = confirm("Do you want to update accounts??");
    if (ans) {
      this._accountsService.updateAccounts(this.applicationDate).subscribe(
        response => {
          this.output = response;
          swal("Updated!", "Updated Successfully", "success");
        }
      )
    }
  }
}