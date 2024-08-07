import { Component, OnInit, OnDestroy, Output, EventEmitter } from '@angular/core';
import { PurchaseService } from './../purchase.service';
import { CustomerService } from '../../masters/customer/customer.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { estimationService } from '../../estimation/estimation.service';
import { SalesService } from '../../sales/sales.service';
import { Router } from '@angular/router';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import { ToastrService } from 'ngx-toastr';
import swal from 'sweetalert';
declare var $: any;

@Component({
  selector: 'app-attacholdgold',
  templateUrl: './attacholdgold.component.html',
  styleUrls: ['./attacholdgold.component.css']
})
export class AttacholdgoldComponent implements OnInit, OnDestroy {
  EnableDisablectrls: boolean = true;
  ccode: string = "";
  bcode: string = "";
  EnableJson: boolean = false;
  password: string;
  CustomerName: any;
  PostOldGoldAttchJson: any = [];
  SearchParamsList: any;
  AttachOldGoldList: any;
  AttachOldGoldForm: FormGroup;
  submitted = false;
  SelectedOldGoldList: any = [];
  EnableSubmitButton: boolean = true;
  SalesEstNo: string = null;
  OrderJson: any = {
    SeriesNo: null,     // SeriesNo: this.EstNoFromPurchaseComp,
    RefBillNo: null,
    OperatorCode: null,
    CompanyCode: this.ccode,
    BranchCode: this.bcode
  };


  OldGoldAttachmentSummaryData: any = {
    Quantity: null,
    Amount: 0.00
  };

  @Output() valueChange = new EventEmitter();

  constructor(private _PurchaseService: PurchaseService, private formBuilder: FormBuilder,
    private _estimationService: estimationService, private _salesService: SalesService,
    private _CustomerService: CustomerService, private _router: Router,
    private _appConfigService: AppConfigService, private toastr: ToastrService) {
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }
  ngOnInit() {

    this.OldGoldAttachmentSummaryData = {
      Quantity: null,
      Amount: 0.00
    };

    this._salesService.EstNo.subscribe(
      response => {
        this.SalesEstNo = response;
      }
    )

    this._estimationService.SubjectEstNo.subscribe(
      response => {
        this.SalesEstNo = response;
        if (this.SalesEstNo != null && this.SalesEstNo != "0") {
          if (this._router.url != "/purchase") {
            this._PurchaseService.getOldGoldttachedList(this.SalesEstNo).subscribe(
              response => {
                this.SelectedOldGoldList = [];
                this.PostOldGoldAttchJson = [];
                this.SelectedOldGoldList = response;
                if (this.SelectedOldGoldList != null) {
                  if (this.SelectedOldGoldList.length > 0) {
                    this._estimationService.SendOldGoldAttachmentSummaryData(this.OldGoldAttachmentSummaryData);
                    for (var i = 0; i < this.SelectedOldGoldList.length; i++) {
                      this.OrderJson.SeriesNo = this.SalesEstNo;
                      this.OrderJson.RefBillNo = this.SelectedOldGoldList[i]["EstNo"];
                      this.OrderJson.OperatorCode = localStorage.getItem('Login');
                      this.OrderJson.CompanyCode = this.ccode,
                        this.OrderJson.BranchCode = this.bcode
                      this.PostOldGoldAttchJson.push(this.OrderJson);
                      this.OrderJson = {};
                      this.markFormGroupPristine(this.AttachOldGoldForm);
                    }
                  }
                }
                else {
                  this.SelectedOldGoldList = [];
                  this.PostOldGoldAttchJson = [];
                }
              }
            )
          }
        }
      }
    )


    this.AttachOldGoldForm = this.formBuilder.group({
      searchby: [null, Validators.required],
      searchText: [null, Validators.required],
    });

    this._PurchaseService.getSearchParams().subscribe(
      response => {
        this.SearchParamsList = response;
      }
    )
    this.getSalesEstNofromSalesService();
    this.LoadCustomerDetails();
    if (this._router.url == "/sales-billing") {
      this.EnableDisablectrls = false;
    }
    if (this._router.url == "/estimation") {
      this.EnableDisablectrls = true;
    }
  }

  LoadCustomerDetails() {
    this._CustomerService.cast.subscribe(
      response => {
        this.CustomerName = response;
      })
  }

  OldGoldAmount: number = 0;

  openModal() {
    $("#myOldGoldModal").modal('show');
    this.getOGList(this.top, this.skip);
    this.AttachOldGoldForm.reset();
    this.AttachOldGoldList = [];
  }

  totalItems: any;
  pagenumber: number = 1;
  top = 10;
  skip = (this.pagenumber - 1) * this.top;

  onPageChange(p: number) {
    this.pagenumber = p;
    const skipno = (this.pagenumber - 1) * this.top;
    this.getOGList(this.top, skipno);
  }

  getOGList(top, skip) {
    this._PurchaseService.getOldGoldList(top, skip).subscribe(
      response => {
        this.AttachOldGoldList = response;
      }
    )
  }


  valueChanged(Amount) { // You can give any function name
    this.OldGoldAmount = Amount;
    this.valueChange.emit(this.OldGoldAmount);
  }

  OGTotalAmount: number = 0;
  @Output() OGAmountChange = new EventEmitter();


  OGAmountChanged(SalesAmount) {
    this.OGTotalAmount = SalesAmount;
    this.OGAmountChange.emit(this.OGTotalAmount);
  }

  enableBtnIfSalesEstNo: boolean = true;
  getSalesEstNofromSalesService() {

    this._salesService.EstNo.subscribe(
      response => {
        this.SalesEstNo = response;
        if (this.SalesEstNo != null && this.SalesEstNo != "") {
          this.enableBtnIfSalesEstNo = false;
        }
      }
    )
  }

  Saved: any;
  ReprintData: any = {
    EstNo: null,
    EstType: null,
  }

  Submit() {
    if (this.EnableSubmitButton == true && this.PostOldGoldAttchJson.length == 0) {
      swal("Warning!", "Please attach the estimate", "warning");
    }
    // else if (this.EnableSubmitButton == true) {
    //   swal("Warning!", "No changes were made to submit", "warning");
    // }
    else if (this.PostOldGoldAttchJson.length == 0) {
      swal("Warning!", 'Please attach the estimate', "warning");
    }
    else if (this.AttachOldGoldForm.pristine == true) {
      swal("Warning!", "No changes were made to submit", "warning");
    }
    else {
      this._PurchaseService.PostAttachementOldGold(this.PostOldGoldAttchJson).subscribe(
        response => {
          this.Saved = response;
          swal("Saved!", "Old Gold est adjusted against Estimation number " + this.SalesEstNo, "success");
          this.ReprintData = {
            EstNo: this.SalesEstNo,
            EstType: "OG"
          }
          this._estimationService.SendOldGoldAttachmentSummaryData(this.OldGoldAttachmentSummaryData);
          this._estimationService.SendReprintData(this.ReprintData);
          this.markFormGroupPristine(this.AttachOldGoldForm);
          // this._router.navigateByUrl('/branch', { skipLocationChange: true }).then(() =>
          //     this._router.navigate(['/estimation'])) 
        }
      )
    }
  }

  onSubmit() {
    this.submitted = true;
    if (this.AttachOldGoldForm.pristine) {
      return;
    }
  }

  DeleteRow(index: number) {
    var ans = confirm("Do you want to delete");
    if (ans) {
      if (this.SelectedOldGoldList.length == 1) {
        this._PurchaseService.deleteOldGoldAttachment(this.SalesEstNo).subscribe(
          response => {
            this.Saved = response;
            this.EnableSubmitButton = false;
          }
        )
      }
      this.valueChanged(this.SelectedOldGoldList[index].Amount);
      this.SelectedOldGoldList.splice(index, 1);
      this.PostOldGoldAttchJson.splice(index, 1);
      this.EnableDisableSubmitBtn();
      this.markFormGroupDirty(this.AttachOldGoldForm);
    }
  }

  selectRecord(arg) {
    if (this.SalesEstNo == "" || this.SalesEstNo == null) {
      swal("Warning!", "Please submit the sales details", "warning");
    }
    else {
      let data = this.SelectedOldGoldList.find(x => x.EstNo == arg.EstNo);
      if (data != null) {
        swal("error", "Old Gold Estimation already attached", "error");
      }
      else {
        this.SelectedOldGoldList.push(arg);
        $('#myOldGoldModal').modal('hide');
        this.OrderJson.SeriesNo = this.SalesEstNo;
        this.OrderJson.RefBillNo = arg.EstNo;
        this.OrderJson.OperatorCode = localStorage.getItem('Login');
        this.OrderJson.CompanyCode = this.ccode,
          this.OrderJson.BranchCode = this.bcode
        this.PostOldGoldAttchJson.push(this.OrderJson);
        this.OrderJson = {};
        this.EnableDisableSubmitBtn();
        this.markFormGroupDirty(this.AttachOldGoldForm);
      }
    }
  }

  EnableDisableSubmitBtn() {
    if (this.SelectedOldGoldList.length <= 0) {
      this.EnableSubmitButton = true;
    }
    else {
      this.EnableSubmitButton = false;
    }
  }

  private markFormGroupPristine(form: FormGroup) {
    Object.values(form.controls).forEach(control => {
      control.markAsPristine();

      if ((control as any).controls) {
        this.markFormGroupPristine(control as FormGroup);
      }
    });
  }


  private markFormGroupDirty(form: FormGroup) {
    Object.values(form.controls).forEach(control => {
      control.markAsDirty();

      if ((control as any).controls) {
        this.markFormGroupDirty(control as FormGroup);
      }
    });
  }

  ngOnDestroy() {
    this._CustomerService.SendCustDataToEstComp(null);
    this._estimationService.SendSRAttachmentSummaryData(null);
  }

  Cancel() {
    $('#myOldGoldModal').modal('hide');
    this.AttachOldGoldForm.reset();
    this.AttachOldGoldList = [];
  }

  getSearchParams(form) {
    if (form.value.searchby != null && form.value.searchText != null) {
      this._PurchaseService.getAttachOldGoldList(form.value.searchby, form.value.searchText).subscribe(
        response => {
          this.AttachOldGoldList = response;
        }
      )
    }
  }
  Amount(arg: any[] = []) { let total = 0; arg.forEach((d) => { total += Number(<number>d.Amount) }); this.OldGoldAttachmentSummaryData.Amount = total; this.OGAmountChanged(total); return total; }
  Quantity(arg: any[] = []) { let total = 0; arg.forEach((d) => { total += d.Qty; }); this.OldGoldAttachmentSummaryData.Quantity = total; return total; }
}