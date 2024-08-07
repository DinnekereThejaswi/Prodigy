import { Component, OnInit, OnDestroy, Output, EventEmitter } from '@angular/core';
import { SalesreturnService } from '../salesreturn.service';
import { CustomerService } from '../../masters/customer/customer.service';
import { SalesService } from '../../sales/sales.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { estimationService } from '../../estimation/estimation.service';
import { Router } from '@angular/router';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import swal from 'sweetalert';
import { ToastrService } from 'ngx-toastr';
declare var $: any;


@Component({
  selector: 'app-attachsalesreturn',
  templateUrl: './attachsalesreturn.component.html',
  styleUrls: ['./attachsalesreturn.component.css']
})

export class AttachsalesreturnComponent implements OnInit, OnDestroy {
  EnableJson: boolean = false;
  EnableDisablectrls: boolean = true;
  ccode: string = "";
  bcode: string = "";
  password: string;
  CustomerName: any;
  PostSalesReturnAttchJson: any = [];
  SearchParamsList: any;
  AttachSalesReturnList: any;
  AttachSalesReturnForm: FormGroup;
  submitted = false;
  SelectedSalesReturnList: any = [];
  EnableSubmitButton: boolean = true;
  SalesEstNo: string = null;
  OrderJson: any = {
    CompanyCode: this.ccode,
    BranchCode: this.bcode,
    SeriesNo: null,     // SeriesNo: this.EstNoFromPurchaseComp,
    RefBillNo: null,
    OperatorCode: null
  };



  @Output() valueChange = new EventEmitter();
  SRAmount: number = 0;


  SRAttachmentSummaryData: any = {
    Quantity: null,
    Amount: 0.00
  };


  constructor(private _SalesreturnService: SalesreturnService, private formBuilder: FormBuilder,
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
    this.SRAttachmentSummaryData = {
      Quantity: null,
      Amount: 0.00
    };

    this.getSalesEstNofromSalesService();


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
            this._SalesreturnService.getSalesReturnAttachedList(this.SalesEstNo).subscribe(
              response => {
                this.SelectedSalesReturnList = [];
                this.PostSalesReturnAttchJson = [];
                this.SelectedSalesReturnList = response;
                if (this.SelectedSalesReturnList.length > 0) {
                  this._estimationService.SendSRAttachmentSummaryData(this.SRAttachmentSummaryData);
                  for (var i = 0; i < this.SelectedSalesReturnList.length; i++) {
                    this.OrderJson.SeriesNo = this.SalesEstNo;
                    this.OrderJson.RefBillNo = this.SelectedSalesReturnList[i]["SrEstNo"];
                    this.OrderJson.OperatorCode = localStorage.getItem('Login');
                    this.OrderJson.CompanyCode = this.ccode,
                      this.OrderJson.BranchCode = this.bcode
                    this.PostSalesReturnAttchJson.push(this.OrderJson);
                    this.OrderJson = {};
                    this.markFormGroupPristine(this.AttachSalesReturnForm);
                  }
                }
              }
            )
          }
        }
      }
    )

    this.AttachSalesReturnForm = this.formBuilder.group({
      searchby: [null, Validators.required],
      searchText: [null, Validators.required],
    });

    this.LoadCustomerDetails();

    if (this._router.url == "/sales-billing") {
      this.EnableDisablectrls = false;
    }
    if (this._router.url == "/estimation") {
      this.EnableDisablectrls = true;
    }
  }

  openModal() {
    $("#mySalesReturnModal").modal('show');
    this.getSalesReturnList(this.top, this.skip);
    this.AttachSalesReturnForm.reset();
    this.AttachSalesReturnList = [];
  }

  totalItems: any;
  pagenumber: number = 1;
  top = 10;
  skip = (this.pagenumber - 1) * this.top;

  onPageChange(p: number) {
    this.pagenumber = p;
    const skipno = (this.pagenumber - 1) * this.top;
    this.getSalesReturnList(this.top, skipno);
  }

  getSalesReturnList(top, skip) {
    this._SalesreturnService.getSRList(top, skip).subscribe(
      response => {
        this.AttachSalesReturnList = response;
      }
    )
  }

  LoadCustomerDetails() {
    this._CustomerService.cast.subscribe(
      response => {
        this.CustomerName = response;
      })
  }

  getSalesEstNofromSalesService() {

    // this._salesService.EstNo.subscribe(
    //   response => {
    //     this.SalesEstNo = response;
    //     if (this.SalesEstNo != null && this.SalesEstNo != "") {
    // this.enableBtnIfSalesEstNo = false;
    this._SalesreturnService.getSearchParams().subscribe(
      response => {
        this.SearchParamsList = response;
      }
    )
  }
  //   }
  // )
  //}

  Saved: any;


  valueChanged(Amount) { // You can give any function name
    this.SRAmount = Amount;
    this.valueChange.emit(this.SRAmount);
  }

  SRTotalAmount: number = 0;
  @Output() SRAmountChange = new EventEmitter();


  SRAmountChanged(SalesAmount) {
    this.SRTotalAmount = SalesAmount;
    this.SRAmountChange.emit(this.SRTotalAmount);
  }

  ReprintData: any = {
    EstNo: null,
    EstType: null,
  }

  Submit() {
    if (this.EnableSubmitButton == true && this.PostSalesReturnAttchJson.length == 0) {
      swal("Warning!", "Please attach the estimate", "warning");
    }
    // else if (this.EnableSubmitButton == true) {
    //   swal("Warning!", "No changes were made to submit", "warning");
    // }
    else if (this.PostSalesReturnAttchJson.length == 0) {
      swal("Warning!", "Please attach the estimate", "warning");
    }
    else if (this.AttachSalesReturnForm.pristine == true) {
      swal("Warning!", "No changes were made to submit", "warning");
    }
    else {
      this._SalesreturnService.PostAttachementSalesReturn(this.PostSalesReturnAttchJson).subscribe(
        response => {
          this.Saved = response;
          swal("Saved!", "SR updated against Estimation number " + this.SalesEstNo, "success");
          this.ReprintData = {
            EstNo: this.SalesEstNo,
            EstType: "SR"
          }
          this._estimationService.SendReprintSR(this.PostSalesReturnAttchJson);
          this._estimationService.SendReprintData(this.ReprintData);
          this._estimationService.SendSRAttachmentSummaryData(this.SRAttachmentSummaryData);
          this.markFormGroupPristine(this.AttachSalesReturnForm);
          // this._router.navigateByUrl('/branch', { skipLocationChange: true }).then(() =>
          //     this._router.navigate(['/estimation'])) 
        }
      )
    }
  }

  onSubmit() {
    this.submitted = true;
    if (this.AttachSalesReturnForm.pristine) {
      return;
    }
  }

  DeleteRow(index: number) {
    var ans = confirm("Do you want to delete");
    if (ans) {
      if (this.SelectedSalesReturnList.length == 1) {
        this._SalesreturnService.deleteSRAttachment(this.SalesEstNo).subscribe(
          response => {
            this.Saved = response;
            this.EnableSubmitButton = false;
          }
        )
      }
      this.valueChanged(this.SelectedSalesReturnList[index].Amount);
      this.SelectedSalesReturnList.splice(index, 1);
      this.PostSalesReturnAttchJson.splice(index, 1);
      this.EnableDisableSubmitBtn();
      this.markFormGroupDirty(this.AttachSalesReturnForm);
    }
  }

  selectRecord(arg) {
    if (this.SalesEstNo == "" || this.SalesEstNo == null) {
      swal("Warning!", "Please submit the sales details", "warning");
    }
    else {
      var Duplicate=this.SelectedSalesReturnList.filter(Estno => Estno.SrEstNo ===arg.SrEstNo);
      if(Duplicate.length > 0){
      swal("Warning!","Sr Estimation " +arg.SrEstNo + "already Attached","warning");
      }
      else{
      this.SelectedSalesReturnList.push(arg);
      $('#mySalesReturnModal').modal('hide');
      this.OrderJson.SeriesNo = this.SalesEstNo;
      this.OrderJson.RefBillNo = arg.SrEstNo;
      this.OrderJson.OperatorCode = localStorage.getItem('Login');
      this.OrderJson.CompanyCode = this.ccode,
        this.OrderJson.BranchCode = this.bcode
      this.PostSalesReturnAttchJson.push(this.OrderJson);
      this.OrderJson = {};
      this.EnableDisableSubmitBtn();
      this.markFormGroupDirty(this.AttachSalesReturnForm);
      }
    }
  }

  EnableDisableSubmitBtn() {
    if (this.SelectedSalesReturnList.length <= 0) {
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
    $('#myModal').modal('hide');
    this.AttachSalesReturnForm.reset();
    this.AttachSalesReturnList = [];
  }

  getSearchParams(form) {
    if (form.value.searchby != null && form.value.searchText != null) {
      this._SalesreturnService.getAttachSalesReturnList(form.value.searchby, form.value.searchText).subscribe(
        response => {
          this.AttachSalesReturnList = response;
        }
      )
    }
  }
  Amount(arg: any[] = []) { let total = 0; arg.forEach((d) => { total += d.Amount; }); this.SRAttachmentSummaryData.Amount = total; this.SRAmountChanged(total); return total; }
  Quantity(arg: any[] = []) { let total = 0; arg.forEach((d) => { total += d.Quantity; }); this.SRAttachmentSummaryData.Quantity = total; return total; }

}