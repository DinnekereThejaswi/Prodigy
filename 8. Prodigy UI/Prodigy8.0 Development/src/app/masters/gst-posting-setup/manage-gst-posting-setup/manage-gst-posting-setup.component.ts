import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { GstPostingSetup } from '../gst-posting-setup.model';
import { FormGroup, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { ToastrService } from 'ngx-toastr';
import { GstPostingSetupService } from '../gst-posting-setup.service';
@Component({
  selector: 'app-manage-gst-posting-setup',
  templateUrl: './manage-gst-posting-setup.component.html',
  styleUrls: ['./manage-gst-posting-setup.component.css']
})
export class ManageGstPostingSetupComponent implements OnInit {
  GSTPostingSetupForm: FormGroup;
  datePickerConfig: Partial<BsDatepickerConfig>;
  id: number = 0;
  successfulSave: boolean;
  errors: string[];
  hasErrors = false;
  constructor(private fb: FormBuilder,
    private _router: Router,
    private _avRoute: ActivatedRoute,
    private toastr: ToastrService,
    private gstpostingService: GstPostingSetupService) {
    if (this._avRoute.snapshot.params["id"]) {
      this.id = parseInt(this._avRoute.snapshot.params["id"]);
    }
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        dateInputFormat: 'YYYY-MM-DD'
      });
  }
  GSTPostingSetupListData: GstPostingSetup = {
    ID: 0,
    GSTGroupCode: null,
    GSTComponentCode: null,
    EffectiveDate: null,
    GSTPercent: null,
    CalculationOrder: null,
    ReceivableAccount: null,
    PayableAccount: null,
    ExpenseAccount: null,
    RefundAccount: null,
    IsRegistered: false
  };
  GroupCodeList: any;
  getGroupCode() {
    this.gstpostingService.getGroupCode().subscribe(
      Response => {
        this.GroupCodeList = Response;
      }
    )
  }
  ComponentCodeAndCalcList: any;
  getComponentCodeAndCalculationOrder() {
    this.gstpostingService.getComponentCodeAndCalculationOrder().subscribe(
      Response => {
        this.ComponentCodeAndCalcList = Response;
      }
    )
  }
  InputOutputList: any;
  getInputOutput() {
    this.gstpostingService.getInputAndOutputRecords().subscribe(
      Response => {
        this.InputOutputList = Response;
      }
    )
  }
  ngOnInit() {
    this.getInputOutput();
    this.getComponentCodeAndCalculationOrder();
    this.getGroupCode();
    this.GSTPostingSetupForm = this.fb.group({
      ID: this.id,
      GSTGroupCode: null,
      GSTComponentCode: null,
      EffectiveDate: null,
      GSTPercent: null,
      CalculationOrder: null,
      ReceivableAccount: null,
      PayableAccount: null,
      ExpenseAccount: null,
      RefundAccount: null,
      IsRegistered: null
    });
    if (this.id > 0) {
      this.gstpostingService.getDetailsFromID(this.id)
        .subscribe((data: GstPostingSetup) => {
          this.GSTPostingSetupListData = data;
        });
    }
  }
  onSubmit(form) {
    if (form.value.GSTGroupCode == null || form.value.GSTGroupCode === '') {
      alert("Please Select Group Code");
    }
    else if (form.value.GSTComponentCode == null || form.value.GSTComponentCode === '') {
      alert("Please select Component Code");
    }
    else if (form.value.EffectiveDate == null || form.value.EffectiveDate === '') {
      alert("Please enter Effective Date");
    }
    else if (form.value.GSTPercent == null || form.value.GSTPercent === '') {
      alert("Please enter GST %");
    }
    else if (form.value.CalculationOrder == null || form.value.CalculationOrder === '') {
      alert("Please select Calculation Order");
    }
    else {
      var ans = confirm("Do you want to Save??");
      if (ans) {
        if (form.value.ID === 0) {
          this.errors = [];
          this.gstpostingService.post(form.value)
            .subscribe(
              suc => {
                this.successfulSave = true;
                this.toastr.success('New Record Added Succcessfully', 'Success!');
                this._router.navigate(['/gst-posting-setup']);
              },
              (err) => {
                this.successfulSave = false;
              }
            )
        }
        else {
          this.gstpostingService.putGSTPosting(form.value)
            .subscribe(
              suc => {
                this.successfulSave = true;
                this.toastr.success('Record Updated Succcessfully', 'Success!');
                this._router.navigate(['/gst-posting-setup']);
              },
              (err) => {
                this.successfulSave = false;
              });


        }
      }
    }
  }
}
