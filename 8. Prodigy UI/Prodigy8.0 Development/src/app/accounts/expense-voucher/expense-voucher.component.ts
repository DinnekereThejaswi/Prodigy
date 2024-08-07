import { Component, OnInit } from '@angular/core';
import { AccountsService } from '../accounts.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MasterGroupVM, SubGroupVM } from '../accounts.model';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';
import { DatePipe, formatDate } from '@angular/common';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { ExpenseVocherModel } from '../accounts.model';

@Component({
  selector: 'app-expense-voucher',
  templateUrl: './expense-voucher.component.html',
  styleUrls: ['./expense-voucher.component.css']
})
export class ExpenseVoucherComponent implements OnInit {
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  EnableJson: boolean = false;
  ///form tags
  ExpenseHeaderForms: FormGroup;
  ExpenseVoucherDetailForm: FormGroup;
  ////modelslistdata
  ExpnsHeaderListData = {
    vendorName: null,
    banckLedger: null,
    cashLedger: null,
    voucherNo: null,
    voucherDate: null,
    lastvoucherNo: null,
    gstin: null,
    pan: null
  }

  expnseDetailsInput: any = [];

  ExpnsDetailsChildModel: ExpenseVocherModel = {
    ObjID: "",
    ExpenseNo: null,
    ExpenseDate: "",
    CompanyCode: "",
    BranchCode: "",
    SupplierCode: "",
    SupplierName: "",
    SNo: null,
    AccCode: null,
    AccName: "",
    InvoiceNo: "",
    InvoiceDate: "",
    Amount: null,
    TaxName: "",
    TaxPercentage: 0,
    TaxAmount: 0,
    TotalAmount: 0,
    TDSPercentage: 0,
    TDSAmount: 0,
    CFlag: "",
    CancelledBy: "",
    OperatorCode: "",
    Description: "",
    ObjStatus: "O",
    SGSTPercent: 0,
    SGSTAmount: 0,
    CGSTPercent: 0,
    CGSTAmount: 0,
    IGSTPercent: 0,
    IGSTAmount: 0,
    HSN: "",
    GSTGroupCode: "",
    AccType: "",
    ChqNo: 0,
    ChqDate: "",
    GSTNo: "",
    OtherPartyName: "",
    PartyCode: "",
    CESSPercent: 0,
    CESSAmount: 0,
    Remarks: "",
    PANNo: "",
    IsEligible: "",
    HSNDescription: "",
    RoundOff: 0,
    FinalAmount: 0,
    TDSID: 0,
    CessAccountCode: 0,
    AccountType: "",
    IsRegistered: ""

  }
  ////
  radioItems: Array<string>;
  model = { option: 'Journal' };
  isChecked: boolean = false;
  ///date variable
  applicationDate: any;
  InvoiceDate: any;
  ChequeDate: any;
  datePickerConfig: Partial<BsDatepickerConfig>;
  today = new Date();
  ////////////////////////////////////////
  isReadOnly: boolean = false;
  CashReadOnly: boolean = false;
  BankReadOnly: boolean = false;
  isReadTDSAmount: boolean = false;
  StateGSTReadOnly: boolean = false;
  IGSTReadOnly: boolean = false;

  ////////
  chkchkbxchked: boolean = false;
  ///aray list
  Childarray: any = [];
  // ChildListObject: Array<{}> = [];
  ChildListObject: any = [];
  ///
  date: string;
  constructor(private _appConfigService: AppConfigService,
    private _acountsService: AccountsService, private fb: FormBuilder, private datepipe: DatePipe) {
    this.apiBaseUrl = this._appConfigService.apiBaseUrl;
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
    this.getCB();
    this.radioItems = ['Journal', 'Cash', 'Bank'];
    //date
    //this.OrderHeaderDetails.OperatorCode = localStorage.getItem('Login');
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        minDate: this.today,
        dateInputFormat: 'YYYY-MM-DD'
      });
  }

  ngOnInit() {
    //for radio button click
    this.TDS;
    this.radioBtnValue = 'J';
    this.getVendorNames('J');
    if (this.radioBtnValue = 'J') {
      this.isReadOnly = true;
      this.CashReadOnly = true;

    }
    //dates text box
    this.getApplicationDate();
    this.getVendorNames(this.radioBtnValue);
    this.getLedgerList(this.radioBtnValue);
    this.getGST();
    //////
    this.ExpenseHeaderForms = this.fb.group({
      vendorname: null,
      cash: null,
      bank: null,
      voucherno: '',
      lastvoucherno: '',
      voucherdate: ['', Validators.required],
      gstinNo: '',
      panNo: ''
    });
    this.ExpenseVoucherDetailForm = this.fb.group({
      frmCtrl_LedgerNames: '',
      frmCtrl_InvcNo: ['', Validators.required],
      frmCtrl_InvcDate: ['', Validators.required],
      frmCtrl_Description: ['', Validators.required],
      frmCtrl_hsn: ['', Validators.required],
      frmCtrl_gstin: "",
      frmCtrl_pan: "",
      frmCtrl_gstno: ['', Validators.required],
      frmCtrl_partyName: ['', Validators.required],
      frmCtrl_gst: ['', Validators.required],
      frmCtrl_chqNo: "",
      frmCtrl_ChqDate: "",
      frmCtrl_Amount: ['', Validators.required],
      frmCtrl_cgst: ['', Validators.required],
      frmCtrl_sgst: ['', Validators.required],
      frmCtrl_igst: ['', Validators.required],
      frmCtrl_cessName: "",
      frmCtrl_cessPercent: "",
      frmCtrl_cessAmount: ['', Validators.required],
      frmCtrl_TotalAmt: ['', Validators.required],
      frmCtrl_roundOff: '',
      frmCtrl_finalAmt: ['', Validators.required],
      frmCtrl_TdsAccnt: ['', Validators.required],
      frmCtrl_CheckTds: ['', Validators.required],
      frmCtrl_TdsPercent: "",
      frmCtrl_TdsAmt: ['', Validators.required],
    });
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }
  radioBtnValue: string = '';
  Changed(arg) {
    if (arg === 'Journal') {
      this.ExpenseHeaderForms.reset();
      this.clearExpenseVoucherDetailForm();
      this.ExpenseVoucherDetailForm.reset();

      this.model.option = arg;
      this.radioBtnValue = 'J';
      this.ExpnsDetailsChildModel.AccountType = this.radioBtnValue;
      this.isReadOnly = true;
      this.CashReadOnly = true;
    }
    else if (arg === 'Cash') {
      this.ExpenseHeaderForms.reset();
      this.clearExpenseVoucherDetailForm();
      this.ExpenseVoucherDetailForm.reset();
      this.model.option = arg;
      this.radioBtnValue = 'C';
      this.ExpnsDetailsChildModel.AccountType = this.radioBtnValue;
      this.isReadOnly = false;
      this.CashReadOnly = true;
    }
    else if (arg === 'Bank') {
      this.ExpenseHeaderForms.reset();
      this.clearExpenseVoucherDetailForm();
      this.ExpenseVoucherDetailForm.reset();
      this.model.option = arg;
      this.radioBtnValue = 'B';
      this.ExpnsDetailsChildModel.AccountType = this.radioBtnValue;
      this.isReadOnly = false;
      this.CashReadOnly = false;
    }

    this.ExpnsDetailsChildModel = {
      ObjID: "",
      ExpenseNo: null,
      ExpenseDate: "",
      CompanyCode: "",
      BranchCode: "",
      SupplierCode: "",
      SupplierName: "",
      SNo: null,
      AccCode: null,
      AccName: "",
      InvoiceNo: "",
      InvoiceDate: "",
      Amount: null,
      TaxName: "",
      TaxPercentage: 0,
      TaxAmount: 0,
      TotalAmount: 0,
      TDSPercentage: 0,
      TDSAmount: 0,
      CFlag: "",
      CancelledBy: "",
      OperatorCode: "",
      Description: "",
      ObjStatus: "O",
      SGSTPercent: 0,
      SGSTAmount: 0,
      CGSTPercent: 0,
      CGSTAmount: 0,
      IGSTPercent: 0,
      IGSTAmount: 0,
      HSN: "",
      GSTGroupCode: "",
      AccType: "",
      ChqNo: 0,
      ChqDate: "",
      GSTNo: "",
      OtherPartyName: "",
      PartyCode: "",
      CESSPercent: 0,
      CESSAmount: 0,
      Remarks: "",
      PANNo: "",
      IsEligible: "",
      HSNDescription: "",
      RoundOff: 0,
      FinalAmount: 0,
      TDSID: 0,
      CessAccountCode: 0,
      AccountType: "",
      IsRegistered: ""

    };
    this.ListOfExpenses = [];

    this.getVendorNames(this.radioBtnValue);
  }

  Vendor: any = [];
  getVendorNames(arg) {
    this._acountsService.getVendorNameList(arg).subscribe(
      Response => {
        this.Vendor = Response;
        //console.log(this.Vendor);
      }
    )
  }
  Cess: any = [];
  TINAndPAN: any = [];
  LedgerTypes: any = [];
  getLedgerList(arg) {
    this._acountsService.getLedgerTypeList(arg).subscribe(
      Response => {
        this.LedgerTypes = Response;
        // console.log(this.LedgerTypes);
      }
    )
  }
  ////
  selectedVendor: any = [];
  DropdownChange(index) {
    var slecetedIndex = index - 1;
    this.handleChange1(slecetedIndex);
  }

  handleChange1(slecetedIndex) {
    this.selectedVendor = this.Vendor[slecetedIndex];
    // console.log(this.selectedVendor);
    this.ExpnsDetailsChildModel.SupplierCode = this.selectedVendor.Code;
    this.ExpnsDetailsChildModel.SupplierName = this.selectedVendor.Name;
    this.tdsLedgerDetails(this.selectedVendor.Code);
    this.getVendorSateCode(this.selectedVendor.Code);

  }
  StateCode: any;
  getVendorSateCode(arg) {
    this._acountsService.getVendorSateCode(arg).subscribe(
      response => {
        this.StateCode = response;
        // console.log(JSON.stringify(this.StateCode));
      });

    this._acountsService.getCompanyMaster().subscribe(
      Response => {
        this.CompanyMasterSate = Response;
        if (this.StateCode == this.CompanyMasterSate.StateCode || this.StateCode == 0 || this.StateCode == "undefined") {
          this.StateGSTReadOnly = false;
          this.IGSTReadOnly = true;
        }
        else if (this.StateCode != this.CompanyMasterSate.StateCode) {
          this.StateGSTReadOnly = true;
          this.IGSTReadOnly = false;
        }
      }
    )
  }

  /////
  tdsLedgerDetails(arg) {
    this._acountsService.getTINAndPANList(arg).subscribe(
      response => {
        this.TINAndPAN = response;
        for (let entry of this.TINAndPAN) {
          this.ExpnsHeaderListData.gstin = entry.TIN;
          this.ExpnsDetailsChildModel.GSTNo = entry.TIN;
          this.ExpnsHeaderListData.pan = entry.PAN;

        }
      }
    );
    this._acountsService.getCessList(arg).subscribe(
      response => {
        this.Cess = response;

      });
  }
  ///////
  disAppDate: any;
  getApplicationDate() {
    this._acountsService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.disAppDate = formatDate(appDate["applcationDate"], 'dd/MM/yyyy', 'en_GB');
        this.reprintHeaders.applicationDate = this.applicationDate;
        this.applicationDate = appDate["applcationDate"];
        this.InvoiceDate = appDate["applcationDate"];
        this.ChequeDate = appDate["applcationDate"];
        this.getExpenseByToday();;
      }
    )
  }
  getExpenseByToday() {
    this.date = formatDate(this.applicationDate, 'yyyy-MM-dd', 'en-US');;
    this._acountsService.GetListByDate(this.date).subscribe(
      response => {
        this.ListOfExpenses = response;
      })
  }
  ReCalculate(arg) {
    // alert(arg.target.value);
    this.ExpnsDetailsChildModel.CGSTAmount = null;
    this.ExpnsDetailsChildModel.SGSTAmount = null;
    this.ExpnsDetailsChildModel.IGSTAmount = null;
    // alert(this.ExpnsDetailsChildModel.Amount);

  }

  reprintHeaders = {
    selectedOption: null,
    applicationDate: null
  }
  TDS: any = [{
    "Code": 0,
    "Name": "NA A/C"
  }, {
    "Code": 1,
    "Name": "TDS 1% A/C"
  }, {
    "Code": 2,
    "Name": "TDS 2% A/C"
  }, {
    "Code": 10,
    "Name": "TDS 10% A/C"
  },
  ];

  //captured the respscted object from dropdown option
  selectedValue: any;
  LedgerDropdownChange(index) {
    var slecetIndex = index - 1;
    this.handleChange(slecetIndex);
  }

  handleChange(slecetIndex) {
    this.selectedValue = this.LedgerTypes[slecetIndex];
    this.ExpnsDetailsChildModel.AccName = this.selectedValue.Name;
    this.ExpnsDetailsChildModel.AccCode = this.selectedValue.Code;
  }

  gst: any = [];
  getGST() {
    this._acountsService.getGSTList().subscribe(
      response => {
        this.gst = response;
        // console.log(JSON.stringify(this.gst));
      }
    )
  }

  GstAmount: number;
  CGST: number = 0;
  SGST: number = 0;
  IGST: number = 0;
  FindCgstSgst: number = 0;
  InterstateGST: number = 0;
  IntraSateGST: number = 0;
  GSTsum: number;
  sum: number;
  FinalSum: number;
  EachGSTPercent: number;

  CompanyMasterSate: any;
  CalculateGST(arg) {
    // alert('if satecaode' + this.StateCode
    // )
    if (this.StateCode == this.CompanyMasterSate.StateCode || this.StateCode == 0 || this.StateCode == "undefined") {
      this.StateGSTReadOnly = false;
      this.IGSTReadOnly = true;
      this.CalculateSCGST(arg);
    }
    else if (this.StateCode.StateCode != this.CompanyMasterSate.StateCode) {
      // alert('else if satecaode' + this.StateCode
      // )
      this.CalculateIGST(arg);
      this.StateGSTReadOnly = false;
      this.IGSTReadOnly = true;
    }
  }

  CalculateSCGST(arg) {
    this.GstAmount = arg;
    this.ExpnsDetailsChildModel.Amount = Number(arg);
    this.FindCgstSgst = arg * Number(this.ExpnsDetailsChildModel.GSTGroupCode) / 100;
    this.InterstateGST = Number(this.FindCgstSgst) / 2;
    let CGST = this.InterstateGST.toFixed(2);
    let SGST = this.InterstateGST.toFixed(2);
    this.ExpnsDetailsChildModel.CGSTAmount = Number(CGST);
    this.ExpnsDetailsChildModel.SGSTAmount = Number(SGST);
    this.EachGSTPercent = Number(this.ExpnsDetailsChildModel.GSTGroupCode) / 2;
    this.ExpnsDetailsChildModel.CGSTPercent = Number(this.EachGSTPercent);
    this.ExpnsDetailsChildModel.SGSTPercent = Number(this.EachGSTPercent);
    this.GSTsum = this._acountsService.addBoi(this.ExpnsDetailsChildModel.CGSTAmount, this.ExpnsDetailsChildModel.SGSTAmount);
    this.FinalSum = Number(this.GstAmount) + Number(this.GSTsum);
    this.ExpnsDetailsChildModel.FinalAmount = this.FinalSum;
    this.ExpnsDetailsChildModel.TotalAmount = this.FinalSum;
  }

  FindIgstAmount: number = 0;
  Amount: number;
  IGSTPercent: number = 0;
  sumOfIGST: number;
  IGSTAmount: number;
  IGSTFinalSum: number;

  CalculateIGST(arg) {
    this.StateCode;
    this.Amount = arg;
    this.ExpnsDetailsChildModel.Amount = Number(arg);
    this.FindIgstAmount = arg * Number(this.ExpnsDetailsChildModel.GSTGroupCode) / 100;
    this.ExpnsDetailsChildModel.IGSTAmount = Number(this.FindIgstAmount.toFixed(2));
    this.sumOfIGST = this._acountsService.addBoi(this.Amount, this.FindIgstAmount);
    this.FinalSum = Number(this.Amount) + Number(this.FindIgstAmount);
    this.ExpnsDetailsChildModel.FinalAmount = this.FinalSum;
    this.ExpnsDetailsChildModel.TotalAmount = this.FinalSum;
    this.ExpnsDetailsChildModel.IGSTPercent = Number(this.ExpnsDetailsChildModel.GSTGroupCode);
  }

  CessDeatils(arg) {
    this.ExpnsDetailsChildModel.CessAccountCode = 10;
  }

  tdsPercent: any;
  AddTdsPercent(arg) {
    this.tdsPercent = arg.target.value
    this.ExpnsDetailsChildModel.TDSPercentage = Number(this.tdsPercent);
    this.ExpnsDetailsChildModel.TDSAmount = this.tdsPercent * Number(this.ExpnsDetailsChildModel.Amount) / 100;
  }
  EditTdsAmount(arg) {
    if (arg.target.checked == true) {
      this.ExpnsDetailsChildModel.IsEligible == "true";
      this.ExpnsDetailsChildModel.IsEligible = "true";
      this.isReadTDSAmount = true;
      this.chkchkbxchked = true;
    }
    else if (arg.target.checked == false) {
      this.ExpnsDetailsChildModel.IsEligible == "false";
      this.ExpnsDetailsChildModel.IsEligible = "false";
      this.isReadTDSAmount = false;
      this.chkchkbxchked = false;
    }
  }
  errors: any = [];
  AddToChild(form) {
    if (this.radioBtnValue == 'J' && this.model.option === 'Journal') {
      this.AddJournalData(form);
    } else if (this.radioBtnValue == 'B' && this.model.option === 'Bank') {
      this.AddBankData(form);
    }
    else if (this.radioBtnValue == 'C' && this.model.option === 'Cash') {
      this.AddCashData(form);
    }

  }

  ListOfExpenses: any = [];
  getAllExpenseByDate() {
    this.date = formatDate(this.applicationDate, 'yyyy-MM-dd', 'en-US');;
    this._acountsService.GetListByDate(this.date).subscribe(
      response => {
        this.ListOfExpenses = response;
        if (this.ListOfExpenses.length >= 1) {
          this.ChildListObject = [];
          this.ExpenseVoucherDetailForm.reset();
        }
      }

    )
  }
  respsoneData: any;
  add() {
    // this.ChildListObject = [];
    this.ChildListObject.push(this.ExpnsDetailsChildModel);

    for (let i = 0; i < this.ChildListObject.length; i++) {

    }
    var ans = confirm("Do you want to Add??");
    if (ans) {
      this._acountsService.postExpVoucher(this.ChildListObject).subscribe(
        response => {
          this.respsoneData = response;
          swal("Saved!", "Saved " + this.respsoneData.ExpenseNo + " Saved", "success");
          this.getAllExpenseByDate();
          this.clearExpenseVoucherDetailForm();

        },
        (err) => {
          if (err.status === 400) {
            const validationError = err.error;
            swal("Warning!", validationError, "warning");
          }
          else {
            this.errors.push('something went wrong!');
          }

        }
      )
    }

  }
  AddJournalData(form) {
    if (form.value.frmCtrl_LedgerNames == null || form.value.frmCtrl_LedgerNames == "") {
      swal("Warning!", " Please select Ledger name", "warning");
    }
    else if (form.value.frmCtrl_InvcNo == null || form.value.frmCtrl_InvcNo == "") {
      swal("Warning!", " Please enter  Invoice Number", "warning");
    }
    else if (form.value.frmCtrl_hsn == null || form.value.frmCtrl_hsn == "") {
      swal("Warning!", " Please enter HSN code", "warning");
    }
    else if (form.value.frmCtrl_gst == null || form.value.frmCtrl_gst == "") {
      swal("Warning!", " Please Select GST", "warning");
    }
    else if (form.value.frmCtrl_Amount == null || form.value.frmCtrl_Amount == "") {
      swal("Warning!", " Please enter Amount", "warning");
    }
    else {
      this.ExpnsDetailsChildModel.AccountType = this.radioBtnValue;
      this.ExpnsDetailsChildModel.AccType = this.radioBtnValue;
      this.ExpnsDetailsChildModel.CompanyCode = this.ccode;
      this.ExpnsDetailsChildModel.BranchCode = this.bcode;
      this.ExpnsDetailsChildModel.OperatorCode = localStorage.getItem('Login');
      this.ExpnsDetailsChildModel.InvoiceDate = formatDate(this.InvoiceDate, 'yyyy-MM-dd', 'en-US');
      this.ExpnsDetailsChildModel.ExpenseDate = formatDate(this.applicationDate, 'yyyy-MM-dd', 'en-US');
      this.ExpnsDetailsChildModel.ObjStatus = "O";
      this.ExpnsDetailsChildModel.PANNo = this.ExpnsHeaderListData.pan;
      this.ExpnsDetailsChildModel.AccType = this.radioBtnValue;
      this.ExpnsDetailsChildModel.AccountType = this.radioBtnValue;
      this.ExpnsDetailsChildModel.TotalAmount = Number(this.ExpnsDetailsChildModel.FinalAmount);
      Number(this.ExpnsDetailsChildModel.TDSAmount);
      this.ExpnsDetailsChildModel.TDSID = Number(this.ExpnsDetailsChildModel.TDSID);
      this.add();
    }
  };

  AddCashData(form) {
    if (form.value.frmCtrl_LedgerNames == null || form.value.frmCtrl_LedgerNames == "") {
      swal("Warning!", " Please select Ledger name", "warning");
    }
    else if (form.value.frmCtrl_Description == null || form.value.frmCtrl_Description == "") {
      swal("Warning!", " Please enter  Invoice Number", "warning");
    }
    else if (form.value.frmCtrl_hsn == null || form.value.frmCtrl_hsn == "") {
      swal("Warning!", " Please enter HSN code", "warning");
    }
    else if (form.value.frmCtrl_gst == null || form.value.frmCtrl_gst == "") {
      swal("Warning!", " Please Select GST", "warning");
    }
    else if (form.value.frmCtrl_Amount == null || form.value.frmCtrl_Amount == "") {
      swal("Warning!", " Please enter Amount", "warning");
    }
    // else if (form.value.frmCtrl_TdsAccnt == null || form.value.frmCtrl_TdsAccnt == "") {
    //   swal("Warning!", " Please Select TDS", "warning");
    // }
    else {
      this.ExpnsDetailsChildModel.AccountType = this.radioBtnValue;
      this.ExpnsDetailsChildModel.AccType = this.radioBtnValue;
      this.ExpnsDetailsChildModel.AccountType = this.radioBtnValue;
      this.ExpnsDetailsChildModel.AccType = this.radioBtnValue;
      this.ExpnsDetailsChildModel.CompanyCode = this.ccode;
      this.ExpnsDetailsChildModel.BranchCode = this.bcode;
      this.ExpnsDetailsChildModel.OperatorCode = localStorage.getItem('Login');
      this.ExpnsDetailsChildModel.InvoiceDate = formatDate(this.InvoiceDate, 'yyyy-MM-dd', 'en-US');
      this.ExpnsDetailsChildModel.ExpenseDate = formatDate(this.applicationDate, 'yyyy-MM-dd', 'en-US');
      this.ExpnsDetailsChildModel.ObjStatus = "O";
      this.ExpnsDetailsChildModel.PANNo = this.ExpnsHeaderListData.pan;
      this.ExpnsDetailsChildModel.TotalAmount = Number(this.ExpnsDetailsChildModel.FinalAmount);
      Number(this.ExpnsDetailsChildModel.TDSAmount);
      this.ExpnsDetailsChildModel.TDSID = Number(this.ExpnsDetailsChildModel.TDSID);
      // alert('this ExpnsDetailsChildModel object elemts count' + this._acountsService.countObjectKeys(this.ExpnsDetailsChildModel));
      this.add();
    }

  };
  AddBankData(form) {
    // alert('inside AddBankData 3');
    // alert('AddCashData entry textbox validation');
    // alert(this.radioBtnValue);
    this.ExpnsDetailsChildModel.AccountType = this.radioBtnValue;
    this.ExpnsDetailsChildModel.AccType = this.radioBtnValue;
    if (form.value.frmCtrl_LedgerNames == null || form.value.frmCtrl_LedgerNames == "") {
      swal("Warning!", " Please select Ledger name", "warning");
    }
    else if (form.value.frmCtrl_Description == null || form.value.frmCtrl_Description == "") {
      swal("Warning!", " Please enter  Invoice Number", "warning");
    }
    else if (form.value.frmCtrl_hsn == null || form.value.frmCtrl_hsn == "") {
      swal("Warning!", " Please enter HSN code", "warning");
    }
    else if (form.value.frmCtrl_gst == null || form.value.frmCtrl_gst == "") {
      swal("Warning!", " Please Select GST", "warning");
    }
    else if (form.value.frmCtrl_chqNo == null || form.value.frmCtrl_chqNo == "") {
      swal("Warning!", " Please Enter Cheque Number", "warning");
    }
    else if (form.value.frmCtrl_Amount == null || form.value.frmCtrl_Amount == "") {
      swal("Warning!", " Please enter Amount", "warning");
    }
    else {
      this.ExpnsDetailsChildModel.AccountType = this.radioBtnValue;
      this.ExpnsDetailsChildModel.AccType = this.radioBtnValue;
      this.ExpnsDetailsChildModel.CompanyCode = this.ccode;
      this.ExpnsDetailsChildModel.BranchCode = this.bcode;
      this.ExpnsDetailsChildModel.OperatorCode = localStorage.getItem('Login');
      this.ExpnsDetailsChildModel.InvoiceDate = formatDate(this.InvoiceDate, 'yyyy-MM-dd', 'en-US');
      this.ExpnsDetailsChildModel.ChqDate = formatDate(this.ChequeDate, 'yyyy-MM-dd', 'en-US');
      this.ExpnsDetailsChildModel.ExpenseDate = formatDate(this.applicationDate, 'yyyy-MM-dd', 'en-US');
      this.ExpnsDetailsChildModel.ObjStatus = "O";
      this.ExpnsDetailsChildModel.PANNo = this.ExpnsHeaderListData.pan;
      this.ExpnsDetailsChildModel.TotalAmount = Number(this.ExpnsDetailsChildModel.FinalAmount);
      Number(this.ExpnsDetailsChildModel.TDSAmount);
      this.ExpnsDetailsChildModel.TDSID = Number(this.ExpnsDetailsChildModel.TDSID);
      //alert('this ExpnsDetailsChildModel object elemts count' + this._acountsService.countObjectKeys(this.ExpnsDetailsChildModel));
      this.add();

    }

  };

  LoadBackExpDetails(arg) {
    this.ChildListObject = [];
    //  alert('this.ChildListObject.lenght' + this.ChildListObject.lenght)
    this.ChildListObject.push(arg);
    //alert('row clicking');
    //console.log(JSON.stringify(this.ChildListObject));
  }
  EditField(arg) {
    //  alert('check once again statecode with getVendorSateCode ');
    this.getVendorSateCode(arg.AccCode);
    //  alert('editing the selected row');
    this.EnableSave = true;
    this.EnableAdd = false;
    this.ExpnsDetailsChildModel = arg;
    //alert('editing child array ')
    // console.log(JSON.stringify(this.ExpnsDetailsChildModel));
  }

  SaveModifiedRowData(form) {
    if (this.radioBtnValue == 'J' && this.model.option === 'Journal') {
      //alert('rdb value J and model option journal');

      this.SaveModifieJournalData(form);
    } else if (this.radioBtnValue == 'B' && this.model.option === 'Bank') {
      // alert('rdb value B and model option Bank');
      this.SaveModifiedBankData(form);
    }
    else if (this.radioBtnValue == 'C' && this.model.option === 'Cash') {
      // alert('rdb value C and model option CAsh');
      this.SaveModifiedCashData(form);
    }

  }
  SaveModifieJournalData(form) {
    // alert('came inside SaveModifieJournalData 1');
    // alert(this.radioBtnValue);
    if (form.value.frmCtrl_LedgerNames == null || form.value.frmCtrl_LedgerNames == "") {
      swal("Warning!", " Please select Ledger name", "warning");
    }
    else if (form.value.frmCtrl_InvcNo == null || form.value.frmCtrl_InvcNo == "") {
      swal("Warning!", " Please enter  Invoice Number", "warning");
    }
    else if (form.value.frmCtrl_hsn == null || form.value.frmCtrl_hsn == "") {
      swal("Warning!", " Please enter HSN code", "warning");
    }
    else if (form.value.frmCtrl_gst == null || form.value.frmCtrl_gst == "") {
      swal("Warning!", " Please Select GST", "warning");
    }
    else if (form.value.frmCtrl_Amount == null || form.value.frmCtrl_Amount == "") {
      swal("Warning!", " Please enter Amount", "warning");
    }
    // else if (form.value.frmCtrl_TdsAccnt == null || form.value.frmCtrl_TdsAccnt == "") {
    //   swal("Warning!", " Please Select TDS", "warning");
    // }
    else {
      this.ExpnsDetailsChildModel.AccountType = this.radioBtnValue;
      this.ExpnsDetailsChildModel.AccType = this.radioBtnValue;
      this.ExpnsDetailsChildModel.CompanyCode = this.ccode;
      this.ExpnsDetailsChildModel.BranchCode = this.bcode;
      this.ExpnsDetailsChildModel.OperatorCode = localStorage.getItem('Login');
      this.ExpnsDetailsChildModel.InvoiceDate = formatDate(this.InvoiceDate, 'yyyy-MM-dd', 'en-US');
      this.ExpnsDetailsChildModel.ExpenseDate = formatDate(this.applicationDate, 'yyyy-MM-dd', 'en-US');
      // this.ExpnsDetailsChildModel.InvoiceDate = this.applicationDate;
      // this.ExpnsDetailsChildModel.ExpenseDate = this.applicationDate
      this.ExpnsDetailsChildModel.ObjStatus = "O";
      this.ExpnsDetailsChildModel.PANNo = this.ExpnsHeaderListData.pan;
      this.ExpnsDetailsChildModel.AccType = this.radioBtnValue;
      this.ExpnsDetailsChildModel.AccountType = this.radioBtnValue;
      this.ExpnsDetailsChildModel.TotalAmount = Number(this.ExpnsDetailsChildModel.FinalAmount);
      Number(this.ExpnsDetailsChildModel.TDSAmount);
      this.ExpnsDetailsChildModel.TDSID = Number(this.ExpnsDetailsChildModel.TDSID);
      // alert('this ExpnsDetailsChildModel object elemts count' + this._acountsService.countObjectKeys(this.ExpnsDetailsChildModel));
      // alert('loging the ExpnsDetailsChildModel');
      //alert('journal data loading logiing');
      // console.log(JSON.stringify(this.ExpnsDetailsChildModel));

      this.Save(this.ExpnsDetailsChildModel);
    }


  };
  SaveModifiedBankData(form) {
    // alert('inside SaveModifiedBankData 2');
    // alert(this.radioBtnValue);
    // alert('AddCashData entry textbox validation');
    if (form.value.frmCtrl_LedgerNames == null || form.value.frmCtrl_LedgerNames == "") {
      swal("Warning!", " Please select Ledger name", "warning");
    }
    else if (form.value.frmCtrl_Description == null || form.value.frmCtrl_Description == "") {
      swal("Warning!", " Please enter  Invoice Number", "warning");
    }
    else if (form.value.frmCtrl_hsn == null || form.value.frmCtrl_hsn == "") {
      swal("Warning!", " Please enter HSN code", "warning");
    }
    else if (form.value.frmCtrl_gst == null || form.value.frmCtrl_gst == "") {
      swal("Warning!", " Please Select GST", "warning");
    }
    else if (form.value.frmCtrl_Amount == null || form.value.frmCtrl_Amount == "") {
      swal("Warning!", " Please enter Amount", "warning");
    }
    else if (form.value.frmCtrl_TdsAccnt == null || form.value.frmCtrl_TdsAccnt == "") {
      swal("Warning!", " Please Select TDS", "warning");
    }
    else {
      this.ExpnsDetailsChildModel.AccountType = this.radioBtnValue;
      this.ExpnsDetailsChildModel.AccType = this.radioBtnValue;
      this.ExpnsDetailsChildModel.AccountType = this.radioBtnValue;
      this.ExpnsDetailsChildModel.AccType = this.radioBtnValue;
      this.ExpnsDetailsChildModel.CompanyCode = this.ccode;
      this.ExpnsDetailsChildModel.BranchCode = this.bcode;
      this.ExpnsDetailsChildModel.OperatorCode = localStorage.getItem('Login');
      // this.ExpnsDetailsChildModel.InvoiceDate = this.applicationDate;
      // this.ExpnsDetailsChildModel.ExpenseDate = this.applicationDate
      this.ExpnsDetailsChildModel.InvoiceDate = formatDate(this.InvoiceDate, 'yyyy-MM-dd', 'en-US');
      this.ExpnsDetailsChildModel.ExpenseDate = formatDate(this.applicationDate, 'yyyy-MM-dd', 'en-US');
      this.ExpnsDetailsChildModel.ChqDate = formatDate(this.ChequeDate, 'yyyy-MM-dd', 'en-US');
      this.ExpnsDetailsChildModel.ObjStatus = "O";
      this.ExpnsDetailsChildModel.PANNo = this.ExpnsHeaderListData.pan;
      this.ExpnsDetailsChildModel.TotalAmount = Number(this.ExpnsDetailsChildModel.FinalAmount);
      Number(this.ExpnsDetailsChildModel.TDSAmount);
      this.ExpnsDetailsChildModel.TDSID = Number(this.ExpnsDetailsChildModel.TDSID);
      // alert('this ExpnsDetailsChildModel object elemts count' + this._acountsService.countObjectKeys(this.ExpnsDetailsChildModel));
      // alert('loging the ExpnsDetailsChildModel');
      //console.log(this.ExpnsDetailsChildModel);
      this.Save(this.ExpnsDetailsChildModel);
    }

  };
  SaveModifiedCashData(form) {
    // alert('inside SaveModifiedCashData 3');
    // alert('AddCashData entry textbox validation');
    // alert(this.radioBtnValue);
    this.ExpnsDetailsChildModel.AccountType = this.radioBtnValue;
    this.ExpnsDetailsChildModel.AccType = this.radioBtnValue;
    if (form.value.frmCtrl_LedgerNames == null || form.value.frmCtrl_LedgerNames == "") {
      swal("Warning!", " Please select Ledger name", "warning");
    }
    else if (form.value.frmCtrl_Description == null || form.value.frmCtrl_Description == "") {
      swal("Warning!", " Please enter  Invoice Number", "warning");
    }
    else if (form.value.frmCtrl_hsn == null || form.value.frmCtrl_hsn == "") {
      swal("Warning!", " Please enter HSN code", "warning");
    }
    else if (form.value.frmCtrl_gst == null || form.value.frmCtrl_gst == "") {
      swal("Warning!", " Please Select GST", "warning");
    }
    else if (form.value.frmCtrl_chqNo == null || form.value.frmCtrl_chqNo == "") {
      swal("Warning!", " Please Enter Cheque Number", "warning");
    }
    else if (form.value.frmCtrl_Amount == null || form.value.frmCtrl_Amount == "") {
      swal("Warning!", " Please enter Amount", "warning");
    }
    else {
      this.ExpnsDetailsChildModel.AccountType = this.radioBtnValue;
      this.ExpnsDetailsChildModel.AccType = this.radioBtnValue;
      this.ExpnsDetailsChildModel.CompanyCode = this.ccode;
      this.ExpnsDetailsChildModel.BranchCode = this.bcode;
      this.ExpnsDetailsChildModel.OperatorCode = localStorage.getItem('Login');
      // this.ExpnsDetailsChildModel.InvoiceDate = this.applicationDate;
      // this.ExpnsDetailsChildModel.ExpenseDate = this.applicationDate
      this.ExpnsDetailsChildModel.InvoiceDate = formatDate(this.InvoiceDate, 'yyyy-MM-dd', 'en-US');
      this.ExpnsDetailsChildModel.ExpenseDate = formatDate(this.applicationDate, 'yyyy-MM-dd', 'en-US');
      this.ExpnsDetailsChildModel.ObjStatus = "O";
      this.ExpnsDetailsChildModel.PANNo = this.ExpnsHeaderListData.pan;
      this.ExpnsDetailsChildModel.TotalAmount = Number(this.ExpnsDetailsChildModel.FinalAmount);
      Number(this.ExpnsDetailsChildModel.TDSAmount);
      this.ExpnsDetailsChildModel.TDSID = Number(this.ExpnsDetailsChildModel.TDSID);
      // alert('this ExpnsDetailsChildModel object elemts count' + this._acountsService.countObjectKeys(this.ExpnsDetailsChildModel));
      // alert('loging the ExpnsDetailsChildModel');
      //console.log(this.ExpnsDetailsChildModel);
      this.Save(this.ExpnsDetailsChildModel);
    }
  };

  Save(arg) {
    // alert('inside modify and save');
    this.ChildListObject = [];
    //arg.ObjID = "";
    //arg.TaxName = "";
    //arg.CFlag = "";
    //arg.CancelledBy = "";
    //arg.GSTNo = "";
    // arg.OtherPartyName = "";
    //arg.Remarks = "";
    // arg.CESSPercent = "";

    this.ChildListObject.push(arg);
    for (let i = 0; i < this.ChildListObject.length; i++) {
      // console.log(this.ChildListObject[i]);
      var ans = confirm("Do you want to Save??");
      if (ans) {
        // alert('LENGHT' + this.ChildListObject.length)
        // console.log(this.ChildListObject);
        // console.log(this.ChildListObject.ExpenseNo);
        //  alert(arg.ExpenseNo);
        //  alert(this.ChildListObject[i].ExpenseNo)
        this._acountsService.ModifyOrUpdateCounterByPUT(this.ChildListObject[i].ExpenseNo, this.ChildListObject).subscribe(
          response => {
            //  alert('put api worked came');
            //  console.log(this.errors);
            swal("Saved!", "Saved " + this.ChildListObject[i].ExpenseNo + " Saved", "success");
            this.clearExpenseVoucherDetailForm();
            this.getAllExpenseByDate();
          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error;
              swal("Warning!", validationError, "warning");
            }
            else {
              this.errors.push('something went wrong!');
            }

          }
        )
      }
    }
  }
  clearExpenseVoucherDetailForm() {
    this.ExpnsDetailsChildModel = {
      ObjID: "",
      ExpenseNo: null,
      ExpenseDate: "",
      CompanyCode: "",
      BranchCode: "",
      SupplierCode: "",
      SupplierName: "",
      SNo: null,
      AccCode: null,
      AccName: "",
      InvoiceNo: "",
      InvoiceDate: "",
      Amount: null,
      TaxName: "",
      TaxPercentage: 0,
      TaxAmount: 0,
      TotalAmount: 0,
      TDSPercentage: 0,
      TDSAmount: 0,
      CFlag: "",
      CancelledBy: "",
      OperatorCode: "",
      Description: "",
      ObjStatus: "O",
      SGSTPercent: 0,
      SGSTAmount: 0,
      CGSTPercent: 0,
      CGSTAmount: 0,
      IGSTPercent: 0,
      IGSTAmount: 0,
      HSN: "",
      GSTGroupCode: "",
      AccType: "",
      ChqNo: 0,
      ChqDate: "",
      GSTNo: "",
      OtherPartyName: "",
      PartyCode: "",
      CESSPercent: 0,
      CESSAmount: 0,
      Remarks: "",
      PANNo: "",
      IsEligible: "",
      HSNDescription: "",
      RoundOff: 0,
      FinalAmount: 0,
      TDSID: 0,
      CessAccountCode: 0,
      AccountType: "",
      IsRegistered: ""

    }
    this.ChildListObject = null;
    this.EnableAdd = true;
    this.EnableSave = false;
    //this.getAllExpenseByDate();
  }
}
