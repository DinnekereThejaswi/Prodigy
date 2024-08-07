import { ComponentCanDeactivate } from './../../appconfirmation-guard';
import { Router } from '@angular/router';
import { RepairItemDetails } from './repair-receipts.model';
import { FormGroup, FormBuilder } from '@angular/forms';
import { CustomerService } from './../../masters/customer/customer.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { MasterService } from '../../core/common/master.service';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { RepairService } from '../repair.service';
import { ToastrService } from 'ngx-toastr';
import swal from 'sweetalert';
import { DatePipe, formatDate } from '@angular/common'
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';

@Component({
  selector: 'app-repair-receipts',
  templateUrl: './repair-receipts.component.html',
  styleUrls: ['./repair-receipts.component.css']
})
export class RepairReceiptsComponent implements OnInit, OnDestroy, ComponentCanDeactivate {
  date:any;
  datePickerConfig: Partial<BsDatepickerConfig>;
  ReceiptdatePickerConfig: Partial<BsDatepickerConfig>;
  ccode: string = "";
  bcode: string = "";
  EnableReprintReceipt: boolean = false;
  today = new Date();
  EnableJson: boolean = false;
  routerUrl: string = "";
  password: string;
  RepairReceiptForm: FormGroup;
  constructor(private _CustomerService: CustomerService, private _repairService: RepairService,
    private fb: FormBuilder, private toastr: ToastrService, private _router: Router,
    private _masterService: MasterService, private datepipe: DatePipe,
    private _appConfigService: AppConfigService) {
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        minDate: this.today,
        dateInputFormat: 'YYYY-MM-DD'
      });
    this.ReceiptdatePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        minDate: this.today,
        dateInputFormat: 'DD/MM/YYYY'
      });

    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
    this.getCB();
  }

  ngOnInit() {
    this.DetailsToCustomerComp();
    this.getSalesMan();
    this.getRepairGS();
    this.getApplicationdate();
    this.GetCustomerDetsFromCustComp();
  }

  ngOnDestroy() {
    this._CustomerService.SendCustDataToEstComp(null);
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
  }

  SalesManList: any;
  getSalesMan() {
    this._repairService.getSalesManData().subscribe(
      response => {
        this.SalesManList = response;
      })
  }

  CustomerName: any;

  GetCustomerDetsFromCustComp() {
    this._CustomerService.cast.subscribe(
      response => {
        this.CustomerName = response;
        if (this.isEmptyObject(this.CustomerName) == false && this.isEmptyObject(this.CustomerName) != null) {
          this.EnableReceiptItemsTab = false;
          this.EnableCustomerTab = true;
          this.leavePage = true;
        }
        else {
          this.EnableReceiptItemsTab = true;
        }
      });
  }

  //to customer Details to Customer Component
  Customer: any;
  PaymentSummary: any = [];
  EnableCustomerTab: boolean = true;
  EnableReceiptItemsTab: boolean = true;
  ToggleCustomer: boolean = false;

  ToggleCustomerData() {
    if (this.ToggleCustomer == true) {
      this.EnableCustomerTab = false;
    }
    else {
      // swal("Warning", "Please select Customer Details to continue ", "warning");
    }
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }


  ToggleReceiptItemsData() {
    if (this.ToggleCustomer == true) {
      this.EnableReceiptItemsTab = !this.EnableReceiptItemsTab;
    }
    else {
      swal("Warning", "Please select Customer Details to continue ", "warning");
    }
  }


  ParentJSON: any = [];
  itemDetails: any = [];
  leavePage: boolean = false;
  DetailsToCustomerComp() {
    this._CustomerService.cast.subscribe(
      response => {
        this.Customer = response;
        if (this.isEmptyObject(this.Customer) == false && this.isEmptyObject(this.Customer) != null) {
          this.RepairSummaryHeader.CustID = this.Customer.ID;
          this.RepairSummaryHeader.MobileNo = this.Customer.MobileNo;
          this.EnableCustomerTab = true;
          this.ToggleCustomer = true;
          this.EnableReceiptItemsTab = true;
          this.leavePage = true;
        }
        else {
          this.EnableCustomerTab = false;
          this.ToggleCustomer = false;
          this.EnableReceiptItemsTab = true;
        }
      });
  }

  //Check object is empty
  isEmptyObject(obj) {
    return (obj && (Object.keys(obj).length === 0));
  }


  //Repai Item Details
  public selectedValue: any;
  RepairGS: any;
  getRepairGS() {
    this._repairService.getRepairGS().subscribe(
      response => {
        this.RepairGS = response;
      }
    )
  }

  changeRepairGS(arg, index) {
    this.getRepairItemName(arg, index)
  }

  RepairItemName: any = [];
  getRepairItemName(arg, index) {
    this._repairService.getRepairname(arg).subscribe(
      response => {
        this.RepairItemName[index] = response;
      }
    )
  }

  RepairItemDetails: RepairItemDetails;
  fieldArray: any = [];
  count: number = 0;

  EnableEditDelbtn = {};
  EnableSaveCnlbtn = {};
  readonly = {};
  EnableDropdown: boolean = false;
  EnableAddRow: boolean = false;
  EnableSubmitButton: boolean = true;
  //Variable declared to take previous edited value
  CopyEditedRow: any = [];

  addrow() {
    this.RepairItemDetails = {
      ObjID: null,
      CompanyCode: null,
      BranchCode: null,
      RepaireNo: 0,
      SlNo: 0,
      Item: null,
      Units: null,
      GrossWt: null,
      StoneWt: 0,
      NetWt: 0,
      Description: null,
      UpdateOn: null,
      GSCode: null,
      FinYear: null,
      Rate: null,
      PartyCode: null,
      Dcts: null
    };
    this.fieldArray.push(this.RepairItemDetails);
    for (let { } of this.fieldArray) {
      this.count++;
    }
    this.EnableSaveCnlbtn[this.count - 1] = true;
    this.EnableEditDelbtn[this.count - 1] = false;
    this.readonly[this.count - 1] = false
    this.count = 0;
    this.EnableAddRow = true;
    this.EnableSubmitButton = true;
    this.EnableDropdown = true;
    this.leavePage = true;
  }

  RepairSummaryHeader: any = {
    RepairDate: null,
    DueDate: null,
    CompanyCode: null,
    BranchCode: null,
    CustID: null,
    MobileNo: null,
    Remarks: null,
    SalCode: null,
    tgwt: null,
    OperatorCode: null,
    lstOfRepairReceiptDetails: []
  }

  GetNetWT(index) {
    const GrossWt = this.fieldArray[index].GrossWt;
    const StneWt = this.fieldArray[index].StoneWt;
    if (parseInt(StneWt) > parseInt(GrossWt)) {
      swal("Input Error", "Stone Wt cannot be greater than Gross Wt.", "error");
    }
    else {
      this.fieldArray[index].NetWt = Number(<number>GrossWt - <number>StneWt).toFixed(3);
    }
  }

  GrossWtTotal(arg) {
    let total: any = 0.00;
    arg.forEach((d) => { total += parseFloat(d.GrossWt) });
    this.RepairSummaryHeader.tgwt = parseFloat(total);
    return total;
  }

  saveDataFieldValue(index) {
    if (this.fieldArray[index]["GSCode"] == null || this.fieldArray[index]["GSCode"] == 0) {
      swal("Warning!", "Please Select GS", "warning");
    }
    else if (this.fieldArray[index]["Item"] == null || this.fieldArray[index]["Item"] == 0) {
      swal("Warning!", 'Please Select Item Name', "warning");
    }
    else if (this.fieldArray[index]["Units"] == null || this.fieldArray[index]["Units"] == 0) {
      swal("Warning!", 'Please Select Quantity', "warning");
    }
    else if (this.fieldArray[index]["GrossWt"] == null || this.fieldArray[index]["GrossWt"] == 0) {
      swal("Warning!", 'Please Enter GrossWt', "warning");
    }
    // else if (this.fieldArray[index]["StoneWt"] == null || this.fieldArray[index]["StoneWt"] == 0) {
    //   this.toastr.warning('Please Enter StoneWt', 'Alert!');
    // }
    // else if (this.fieldArray[index]["Dcts"] == null || this.fieldArray[index]["Dcts"] == 0) {
    //   this.toastr.warning('Please Enter Dcts', 'Alert!');
    // }
    else if (this.fieldArray[index]["Description"] == null || this.fieldArray[index]["Description"] == 0) {
      swal("Warning!", 'Please Enter Description', "warning");
    }
    else {
      this.RepairSummaryHeader.lstOfRepairReceiptDetails[index] = this.fieldArray[index];
      this.EnableEditDelbtn[index] = true;
      this.EnableSaveCnlbtn[index] = false;
      this.readonly[index] = true;
      this.EnableAddRow = false;
      this.EnableSubmitButton = false;
      this.leavePage = true;
    }
  }

  cancelDataFieldValue(index) {
    this.EnableAddRow = false;
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

  editFieldValue(index) {
    if (this.EnableAddRow == true) {
      swal("Warning!", 'Please save the enabled item', "warning");
    }
    else {
      this.CopyEditedRow[index] = Object.assign({}, this.fieldArray[index]);
      this.EnableEditDelbtn[index] = false;
      this.EnableSaveCnlbtn[index] = true;
      this.readonly[index] = false;
      this.EnableAddRow = true;
      this.EnableSubmitButton = true;
    }
  }

  deleteFieldValue(index) {
    if (this.EnableAddRow == true) {
      swal("Warning!", 'Please save the enabled item', "warning");
    }
    else {
      this.RepairSummaryHeader.lstOfRepairReceiptDetails.splice(index, 1);
      this.fieldArray.splice(index, 1);
      this.EnableDisableSubmit();
    }
  }

  getApplicationdate() {
    this._repairService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        let rpDate = appDate["applcationDate"]
        this.RepairSummaryHeader.RepairDate = formatDate(rpDate, 'dd/MM/yyyy', 'en_GB');
        // this.RepairSummaryHeader.DueDate = rpDate;
      }
    )
  }


  ReceiptNo: string = "";
  RepairReceiptNo: string = "";
  submitPost() {
    if (this.RepairSummaryHeader.DueDate == null) {
      swal("Warning!", 'Please Select Due Date', "warning");

    }
    else if (this.RepairSummaryHeader.SalCode == null) {
      swal("Warning!", 'Please Select Received By', "warning");
    }
    else {
      this.RepairSummaryHeader.CompanyCode = this.ccode;
      this.RepairSummaryHeader.BranchCode = this.bcode;
      this.RepairSummaryHeader.OperatorCode = localStorage.getItem('Login');

      var ans = confirm("Do you want to Submit??");
      if (ans) {
        let receiptnumber;
        this._repairService.post(this.RepairSummaryHeader).subscribe(
          response => {
            receiptnumber = response;
            this.routerUrl = this._router.url;
            this.ReceiptNo = receiptnumber.repairNo;
            this.RepairReceiptNo = receiptnumber.repairNo;
            swal("Saved!", "Repair Receipt number " + receiptnumber.repairNo + " Created", "success");
            this.leavePage = false;
            var ans = confirm("Do You want to Print??");
            if (ans) {
              this.EnableReprintReceipt = true;
              this._repairService.SendReceiptDeliveryNoToReprintComp(receiptnumber.repairNo);
              this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
            }
            // this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
            //   () => {
            //     this._router.navigate(['repair/repair-receipts']);
            //   })
            this.ClearValues();
            this.getApplicationdate();
          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error;
              swal("Warning!", validationError.customDescription, "warning");
            }
          }
        )
      }
    }
  }

  ClearValues() {
    this.RepairSummaryHeader = {
      RepairDate: null,
      DueDate: null,
      CompanyCode: null,
      BranchCode: null,
      CustID: null,
      MobileNo: null,
      Remarks: null,
      SalCode: null,
      tgwt: null,
      OperatorCode: null,
      lstOfRepairReceiptDetails: []
    }
    this._CustomerService.SendCustDataToEstComp(null);
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
    this.fieldArray = [];
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