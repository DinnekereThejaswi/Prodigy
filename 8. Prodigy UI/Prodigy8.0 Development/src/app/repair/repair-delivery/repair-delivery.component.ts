import { PaymentService } from './../../payment/payment.service';
import { ToastrService } from 'ngx-toastr';
import { RepairService } from './../repair.service';
import { CustomerService } from './../../masters/customer/customer.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { MasterService } from '../../core/common/master.service';
import swal from 'sweetalert';
import { Router } from '@angular/router';
import * as moment from 'moment'
import * as CryptoJS from 'crypto-js';
import { ComponentCanDeactivate } from '../../appconfirmation-guard';
import { AppConfigService } from '../../AppConfigService';

@Component({
  selector: 'app-repair-delivery',
  templateUrl: './repair-delivery.component.html',
  styleUrls: ['./repair-delivery.component.css']
})
export class RepairDeliveryComponent implements OnInit, OnDestroy, ComponentCanDeactivate {
  datePickerConfig: Partial<BsDatepickerConfig>;
  @ViewChild('RepairNo', { static: true }) RepairNo: ElementRef;
  today = new Date();
  RepairDeliveryForm: FormGroup;
  hadRPDetails: boolean = false;
  isDisabled: boolean = true;
  ccode: string = "";
  bcode: string = "";
  EnableReprintDelivery: boolean = false;
  EnableJson: boolean = false;
  password: string;
  routerUrl: string = "";
  paymentPaidAmt: number = 0;
  totalRepairAmt: number = 0;
  RepairAmount: number = 0;
  GstAmount: number = 0;

  EnablePayment: boolean = true;

  constructor(private _CustomerService: CustomerService, private _repairService: RepairService,
    private fb: FormBuilder, private toastr: ToastrService, private _paymentservice: PaymentService, private _router: Router,
    private _masterService: MasterService, private _appConfigService: AppConfigService) {
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        minDate: this.today,
        dateInputFormat: 'YYYY-MM-DD'
      });
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
    this.getCB();
  }
  fieldArray: any = [];
  public pageName = "RD";
  leavePage: boolean = false;
  autoFetchAmount: number = 0;
  paymentDetails: any = [];
  ngOnInit() {
    this.DetailsToCustomerComp();
    this.getSalesMan();
    this.GetCustomerDetsFromCustComp();
    // this.getApplicationdate();

    this._paymentservice.castData.subscribe(
      response => {
        this.paymentDetails = response;
        if (this.paymentDetails != null) {

          this.paymentPaidAmt = 0;
          this.RepairAmount = 0;
          this.GstAmount = 0;
          this.totalRepairAmt = 0;
          if (this.repairDeliverySummaryHeader != null) {
            this.repairDeliverySummaryHeader.lstOfRepairIssueDetails.forEach((d) => {
              if (d.RepairAmount != 0 && d.RepairAmount != null) {
                this.RepairAmount += d.RepairAmount;
              }
            });

            this.GstAmount = this.GST(this.fieldArray);

            this.RepairAmount = Math.round(this.RepairAmount + this.GstAmount);

            this.repairDeliverySummaryHeader.lstOfPayment.forEach((d) => {
              if (d.PayAmount != 0 && d.PayAmount != null) {
                this.paymentPaidAmt += parseInt(d.PayAmount);
              }
            });

            this.totalRepairAmt = (this.RepairAmount - this.paymentPaidAmt);
            this.autoFetchAmount = this.totalRepairAmt;
          }
        }
      }
    )
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }


  // getApplicationdate() {
  //   this._repairService.getApplicationDate().subscribe(
  //     response => {
  //       let appDate = response;
  //       //  let rpDate = appDate["applcationDate"]
  //       //   this. repairDeliverySummaryHeader.IssueDate = rpDate;
  //     }
  //   )
  // }


  ngOnDestroy() {
    this._CustomerService.SendCustDataToEstComp(null);
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
    this._paymentservice.outputData(null);
  }

  SalesManList: any;
  getSalesMan() {
    this._repairService.getSalesManData().subscribe(
      response => {
        this.SalesManList = response;
      })
  }

  RDDetails: any = [];
  previousPaymentDetails: number;
  SelectedItemLines: any = [];
  getDetails(arg) {
    if (arg == '') {
      alert("Enter Repair Receipt Number");
    }
    else {
      this._repairService.getRepairReceiptNo(arg).subscribe(
        response => {
          this.RDDetails = response;
          if (this.RDDetails != null) {
            this.fieldArray = this.RDDetails.lstOfRepairIssueDetails;
            this.repairDeliverySummaryHeader.lstOfRepairIssueDetails = this.fieldArray;
            this.repairDeliverySummaryHeader.CompanyCode = this.ccode;
            this.repairDeliverySummaryHeader.BranchCode = this.bcode;
            this.repairDeliverySummaryHeader.OperatorCode = localStorage.getItem('Login');
            this._paymentservice.inputData(null);
            this.paymentPaidAmt = 0;
            this._CustomerService.getCustomerDtls(this.RDDetails.CustID).subscribe(
              response => {
                const customerDtls = response;
                this._CustomerService.sendCustomerDtls_To_Customer_Component(customerDtls);
                for (var field in this.fieldArray) {
                  this.EnableEditDelbtn[field] = true;
                  this.EnableSaveCnlbtn[field] = false;
                  this.readonly[field] = true;
                }
                //  this._paymentservice.inputData(this.RDDetails);
                this.EnablePaymentsTab = false;
                this.hadRPDetails = true;
              }
            )
          }
        },
        (err) => {
          if (err.status === 404) {
            const validationError = err.error;
            swal("Warning!", validationError.description, "error");
            this.ClearValues();
          }
          if (err.status === 400) {
            const validationError = err.error;
            swal("Warning!", validationError.Message, "error");
            this.ClearValues();
          }
        }
      )
    }
  }
  //Hide Show data when accordian collapsed(Header)
  Customer: any;
  EnableCustomerTab: boolean = true;
  ToggleCustomer: boolean = false;

  EnableItemsTab: boolean = false;

  ToggleItems: boolean = false;
  TogglePayments: boolean = false;
  EnablePaymentsTab: boolean = false;


  PaymentSummary: any = [];
  EnableItemsPaymentsTab: boolean = true;
  NoRecordsItemsPayments: boolean = true;
  NoRecordsPaymentSummary: boolean = false;
  NoRecordsPayments: boolean = true;
  ParentJSON: any = [];
  itemDetails: any = [];

  ToggleCustomerData() {
    if (this.ToggleCustomer == true) {
      this.EnableCustomerTab = !this.EnableCustomerTab;
    }
    else {
      swal("Warning", "Please enter Repair Receipt No continue ", "warning");
    }
  }

  ToggleItemsData() {
    this.EnableItemsTab = !this.EnableItemsTab;
  }

  TogglePaymentData() {
    if (this.EnablePayment == false) {
      swal("Warning", "Please save the item details", "warning");
    }
    else if (this.repairAmt == 0) {
      this.EnablePaymentsTab = true;
      swal("Warning", "There is no repair Amount to Pay", "warning");
    }
    // else if (this.CheckRepairAmount()==false) {
    //   this.EnablePaymentsTab = true;
    //   swal("Warning", "Please enter the repair amount", "warning");
    // }
    else {
      if (this.repairAmt > 0) {
        this.EnableItemsTab = true;
        this.ToggleItems = true;
        this.EnablePaymentsTab = !this.EnablePaymentsTab;
        this._paymentservice.inputData(null);
        this._paymentservice.inputData(this.repairDeliverySummaryHeader);
      }
    }
  }

  GetPaymentSummary() {
    this._paymentservice.CastPaymentSummaryData.subscribe(
      response => {
        this.PaymentSummary = response;
        if (this.isEmptyObject(this.PaymentSummary) == false && this.isEmptyObject(this.PaymentSummary) != null) {
          if (this.PaymentSummary.Amount == null) {
            this.NoRecordsPaymentSummary = false;
            this.EnableSubmitButton = true;
          }
          else {
            this.NoRecordsPaymentSummary = true;
            this.EnableSubmitButton = false;
          }
        }
        else {
          this.NoRecordsPaymentSummary = false;
          this.EnableSubmitButton = true;
        }
      }
    )
  }


  DetailsToCustomerComp() {
    this._CustomerService.cast.subscribe(
      response => {
        this.Customer = response;
        if (this.isEmptyObject(this.Customer) == false && this.isEmptyObject(this.Customer) != null) {
          this.EnableCustomerTab = true;
          this.ToggleCustomer = true;

          this.EnableItemsTab = true;
          this.EnablePaymentsTab = false;
          this.TogglePayments = true;
          this.leavePage = true;
        }
        else {
          this.EnableCustomerTab = true;
          this.ToggleCustomer = false;

          this.EnableItemsTab = false;

          this.EnablePaymentsTab = true;
          this.TogglePayments = false;
        }
      });
  }



  //Check object is empty
  isEmptyObject(obj) {
    return (obj && (Object.keys(obj).length === 0));
  }

  EnableEditDelbtn = {};
  EnableSaveCnlbtn = {};
  readonly = {};
  EnableDropdown: boolean = false;
  EnableAddRow: boolean = false;
  EnableSubmitButton: boolean = true;
  //Variable declared to take previous edited value
  CopyEditedRow: any = [];

  saveDataFieldValue(index) {
    if (this.paymentPaidAmt > 0) {
      swal("Warning", "Please remove the receipt details before changing the repair amount", "warning");
      this.cancelDataFieldValue(index)
    }
    else {
      if (this.repairAmt > 0) {
        this.repairDeliverySummaryHeader.lstOfPayment = [];
      }
      else {
        this.EnablePaymentsTab = true;
        this.repairDeliverySummaryHeader.lstOfPayment = null;
        this.paymentPaidAmt = 0;
        this.totalRepairAmt = 0;
      }
      this.repairDeliverySummaryHeader.lstOfRepairIssueDetails[index] = this.fieldArray[index];
      this._repairService.calculateGST(this.fieldArray[index]).subscribe(
        response => {
          this.fieldArray[index] = response;
          this.calcTotalRepairAmt();
          this.leavePage = true;
        }
      )

      this.EnableEditDelbtn[index] = true;
      this.EnableSaveCnlbtn[index] = false;
      this.readonly[index] = true;
      this.EnableAddRow = false;
      this.EnableSubmitButton = false;
      this.EnablePayment = true;
    }
  }

  calcTotalRepairAmt() {
    this.totalRepairAmt = 0;
    this.autoFetchAmount = 0;
    for (let i = 0; i < this.fieldArray.length; i++) {
      this.totalRepairAmt += Math.round(this.fieldArray[i].RepairAmount + this.fieldArray[i].SGSTAmount + this.fieldArray[i].CGSTAmount + + this.fieldArray[i].IGSTAmount);
      this.autoFetchAmount = this.totalRepairAmt;
    }
  }

  SendItemDetsToPaymentComp() {
    if (this.EnablePayment == false) {
      swal("Warning", "Please save the item details", "warning");
    }
    else {
      if (this.CheckRepairAmount() == false) {
        this.EnablePaymentsTab = true;
        swal("Warning", "Please enter the repair amount", "warning");
      }
      else {
        this.EnableItemsTab = false;
        this.ToggleItems = true;
        this.EnablePaymentsTab = false;
        this._paymentservice.inputData(this.repairDeliverySummaryHeader);
      }
    }
  }

  NoAmount: boolean = true;

  CheckRepairAmount() {
    this.NoAmount = true;
    if (this.repairDeliverySummaryHeader.lstOfRepairIssueDetails.length > 0) {
      for (let i = 0; i < this.repairDeliverySummaryHeader.lstOfRepairIssueDetails.length; i++) {
        if (this.repairDeliverySummaryHeader.lstOfRepairIssueDetails[i].RepairAmount == null) {
          this.NoAmount = false;
          break;
        }
      }
    }
    return this.NoAmount;
  }

  EnableDisableSubmit() {
    if (this.fieldArray.length <= 0) {
      this.EnableSubmitButton = true;
      this.EnableDropdown = false;
    }
    else {
      this.EnableSubmitButton = false;
      this.EnableDropdown = true;
    }
  }


  cancelDataFieldValue(index) {
    this.EnableAddRow = false;
    this.EnablePayment = true;
    if (this.CopyEditedRow[index] != null) {
      this.fieldArray[index] = this.CopyEditedRow[index];
      this.CopyEditedRow[index] = null;
      this.EnableSaveCnlbtn[index] = false;
      this.EnableEditDelbtn[index] = true;
      this.readonly[index] = true;
      this.CopyEditedRow = [];
    }
    else {
      this.fieldArray.splice(index, 1);
    }
    this.EnableDisableSubmit();
  }

  CustomerName: any;
  GetCustomerDetsFromCustComp() {
    this._CustomerService.cast.subscribe(
      response => {
        this.CustomerName = response;
        if (this.isEmptyObject(this.CustomerName) == false && this.isEmptyObject(this.CustomerName) != null) {
          this.EnableItemsTab = false;
          this.EnableCustomerTab = true;
          this.EnablePaymentsTab = true;
        }
        else {
          this.EnableItemsTab = true;
        }
      });
  }



  editFieldValue(index) {
    if (this.EnableAddRow == true) {
      swal("Warning!", 'Please save the enabled item', "warning");
    }
    else {
      this.EnablePayment = false;
      this.CopyEditedRow[index] = Object.assign({}, this.fieldArray[index]);
      this.EnableEditDelbtn[index] = false;
      this.EnableSaveCnlbtn[index] = true;
      this.readonly[index] = false;
      this.EnableAddRow = true;
      this.EnableSubmitButton = true;
    }
  }

  GSTAmt: number = 0;
  repairAmt: number = 0;
  NtWt(arg: any[] = []) { let total = 0; arg.forEach((d) => { total += Number(d.NetWt); }); return total; }
  RepairAmt(arg: any[] = []) { let total = 0; arg.forEach((d) => { total += Number(d.RepairAmount); }); this.repairAmt = total; this.repairDeliverySummaryHeader.TotalRepairAmount = total; return total; }

  GST(arg: any[] = []) {
    let gst = 0;
    arg.forEach((d) => {
      gst += Number(d.SGSTAmount);
    });
    arg.forEach((d) => {
      gst += Number(d.CGSTAmount);
    });
    this.GSTAmt = gst;
    return gst;
  }

  repairDeliverySummaryHeader: any = {
    RepairNo: null,
    IssueDate: null,
    SalCode: null,
    OperatorCode: null,
    TotalRepairAmount: null,
    ItemType: null,
    Remarks: null,
    BranchCode: null,
    CompanyCode: null,
    lstOfRepairIssueDetails: [],
    lstOfPayment: []
  }
  DeliveryNo: string = "";
  RepairDeliveryNo: string = "";
  submitPost() {
    if (this.repairDeliverySummaryHeader.SalCode == null) {
      swal("Oops!", "Please select issued by", "error");
    }
    else if (this.EnablePayment == false) {
      swal("Warning", "Please save the item details", "warning");
    }
    // else if (this.CheckRepairAmount()==false) {
    //   this.EnablePaymentsTab = true;
    //   swal("Warning", "Please enter the repair amount", "warning");
    // }
    else if (this.repairAmt > 0 && this.repairDeliverySummaryHeader.lstOfPayment.length == 0) {
      swal("Oops!", "Please enter the receipt details", "error");
    }
    else if (this.repairAmt > 0 && this.paymentPaidAmt < this.RepairAmount) {
      swal("Oops!", "Paid Amount is less than Repair Amount", "error");
    }
    else if (this.repairAmt > 0 && this.paymentPaidAmt > this.RepairAmount) {
      swal("Oops!", "Paid Amount is greater than Repair Amount", "error");
    }
    else {
      var ans = confirm("Do you want to Submit??");
      if (ans) {
        let deliverynumber;
        if (this.repairAmt == 0 || this.repairAmt == null) {
          var ans = confirm("Repair Charges is Nil.Do you want to Save????");
          if (ans) {
            this._repairService.PostDelivery(this.repairDeliverySummaryHeader).subscribe(
              response => {
                deliverynumber = response;
                this.routerUrl = this._router.url;
                this.DeliveryNo = deliverynumber.repairNo;
                this.RepairDeliveryNo = deliverynumber.deliveryNo;
                swal("Saved!", "Delivery number " + deliverynumber.deliveryNo + " Created", "success");
                this.leavePage = false;
                var ans = confirm("Do You want to Print??");
                if (ans) {
                  this.EnableReprintDelivery = true;
                  this._repairService.SendReceiptDeliveryNoToReprintComp(deliverynumber.deliveryNo);
                }
                this.ClearValues();
                this.hadRPDetails = false;
              },
              (err) => {
                if (err.status === 400) {
                  const validationError = err.error;
                  swal("Warning!", validationError.description, "warning");
                }
                if (err.status === 404) {
                  const validationError = err.error;
                  swal("Warning!", validationError.description, "warning");
                }
                if (err.status === 500) {
                  const validationError = err.error;
                  swal("Warning!", validationError.description, "warning");
                }
              }
            )
          }
        }
        else {
          this._repairService.PostDelivery(this.repairDeliverySummaryHeader).subscribe(
            response => {
              deliverynumber = response;
              this.routerUrl = this._router.url;
              this.DeliveryNo = deliverynumber.repairNo;
              this.RepairDeliveryNo = deliverynumber.deliveryNo;
              swal("Saved!", "Delivery number " + deliverynumber.deliveryNo + " Created", "success");
              this.leavePage = false;
              var ans = confirm("Do You want to Print??");
              if (ans) {
                this.EnableReprintDelivery = true;
                this._repairService.SendReceiptDeliveryNoToReprintComp(deliverynumber.deliveryNo);
              }

              this.ClearValues();
              this.hadRPDetails = false;
            },
            (err) => {
              if (err.status === 400) {
                const validationError = err.error;
                swal("Warning!", validationError.description, "warning");
              }
              if (err.status === 404) {
                const validationError = err.error;
                swal("Warning!", validationError.description, "warning");
              }
              if (err.status === 500) {
                const validationError = err.error;
                swal("Warning!", validationError.description, "warning");
              }
            }
          )
        }
      }
    }
  }

  ClearValues() {
    this.RepairNo.nativeElement.value = "";
    this.repairDeliverySummaryHeader = {
      RepairNo: null,
      IssueDate: null,
      SalCode: null,
      OperatorCode: null,
      TotalRepairAmount: null,
      ItemType: null,
      Remarks: null,
      BranchCode: null,
      CompanyCode: null,
      lstOfRepairIssueDetails: [],
      lstOfPayment: []
    }
    this._CustomerService.SendCustDataToEstComp(null);
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
    this._paymentservice.outputData(null);
    this.fieldArray = [];
    this.hadRPDetails = false;
  }

  confirmBeforeLeave(): boolean {
    if (this.leavePage == true) {
      var ans = (confirm("You have unsaved changes! If you leave, your changes will be lost."))
      if (ans) {
        this.leavePage = false;
        return true;
      }
      else {
        return false;
      }
    }
    else {
      return true;
    }
  }
}