import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AccountsService } from '../accounts.service';
import { LedgerMasterVM } from '../accounts.model';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
declare var $: any;

@Component({
  selector: 'app-ledger',
  templateUrl: './ledger.component.html',
  styleUrls: ['./ledger.component.css']
})
export class LedgerComponent implements OnInit {
  public Index: number = -1;
  @ViewChild("Pwd", { static: true }) Pwd: ElementRef;
  EnableValidatePermission: boolean = false;
  PasswordValid: string;
  EnableLedgerName: boolean = true;
  LedgerList: any = [];
  LedgerType: any = [];
  SubGroup: any = [];
  TransactionType: any = [];
  disabled: boolean = false;
  VType: any = [];
  GSTGroup: any = [];
  GSTServiceGroup: any = [];
  EnableDisableODLimit: boolean = false;
  HSNSAC: any = [];
  LedgerEntryForm: FormGroup;
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;
  GroupName: string;
  GroupId: any;
  searchText: string;
  EnableJson: boolean = false;
  LedgerCategory: any = [
    {
      "Name": "CASH",
      "Value": "C"
    },
    {
      "Name": "BANK",
      "Value": "B"
    },
    {
      "Name": "OTHER",
      "Value": "O"
    },
    {
      "Name": "CCARD",
      "Value": "R"
    }
  ]

  BalanceType: any = [
    {
      "Name": "CR",
      "Value": "C"
    },
    {
      "Name": "DR",
      "Value": "D"
    }
  ]


  ledgerModel: LedgerMasterVM = {
    ObjID: null,
    CompanyCode: null,
    BranchCode: null,
    AccCode: 0,
    AccName: null,
    AccType: null,
    GroupID: 0,
    OpeBal: 0,
    OpnBalType: null,
    CustID: 0,
    PartyCode: null,
    GSCode: null,
    ObjStatus: null,
    GSSeqNo: 0,
    SchemeCode: null,
    VATID: null,
    OdLimit: 0,
    UpdateOn: null,
    JFlag: null,
    PANCard: null,
    TIN: null,
    LedgerType: null,
    Address1: null,
    Address2: null,
    Address3: null,
    City: null,
    State: null,
    District: null,
    Country: null,
    PinCode: null,
    Phone: null,
    Mobile: null,
    FaxNo: null,
    WebSite: null,
    CSTNo: null,
    BudgetAmt: 0,
    TDSID: 0,
    EmailID: null,
    HSN: null,
    GSTGoodsGroupCode: null,
    GSTServicesGroupCode: null,
    StateCode: 0,
    VTYPE: null,
    UniqRowID: null,
    TransType: null,
    IsAutomatic: null,
    Schedule_Name: null,
    NewAccCode: null,
    PartyACNo: null,
    PartyACName: null,
    PartyMICR_No: null,
    PartyBankName: null,
    PartyBankBranch: null,
    PartyBankAddress: null,
    PartyRTGScode: null,
    PartyNEFTcode: null,
    PartyIFSCcode: null,
    swiftcode: null,
  }


  constructor(private accountService: AccountsService, private formBuilder: FormBuilder, private appConfigService: AppConfigService) {
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.EnableJson = this.appConfigService.EnableJson;
    this.password = this.appConfigService.Pwd;
    this.PasswordValid = this.appConfigService.RateEditCode.DailyRatesPermission;
    this.getCB();
  }

  ngOnInit() {
    this.getLedgerList();
    this.LedgerEntryForm = this.formBuilder.group({
      ObjID: ["", Validators.required],
      CompanyCode: ["", Validators.required],
      BranchCode: ["", Validators.required],
      AccCode: ["", Validators.required],
      AccName: ["", Validators.required],
      AccType: ["", Validators.required],
      GroupName: ["", Validators.required],
      GroupID: ["", Validators.required],
      OpeBal: ["", Validators.required],
      OpnBalType: ["", Validators.required],
      CustID: ["", Validators.required],
      PartyCode: ["", Validators.required],
      GSCode: ["", Validators.required],
      ObjStatus: ["", Validators.required],
      GSSeqNo: ["", Validators.required],
      SchemeCode: ["", Validators.required],
      VATID: ["", Validators.required],
      OdLimit: ["", Validators.required],
      UpdateOn: ["", Validators.required],
      JFlag: ["", Validators.required],
      PANCard: ["", Validators.required],
      TIN: ["", Validators.required],
      LedgerType: ["", Validators.required],
      Address1: ["", Validators.required],
      Address2: ["", Validators.required],
      Address3: ["", Validators.required],
      City: ["", Validators.required],
      State: ["", Validators.required],
      District: ["", Validators.required],
      Country: ["", Validators.required],
      PinCode: ["", Validators.required],
      Phone: ["", Validators.required],
      Mobile: ["", Validators.required],
      FaxNo: ["", Validators.required],
      WebSite: ["", Validators.required],
      CSTNo: ["", Validators.required],
      BudgetAmt: ["", Validators.required],
      TDSID: ["", Validators.required],
      EmailID: ["", Validators.required],
      HSN: ["", Validators.required],
      GSTGoodsGroupCode: ["", Validators.required],
      GSTServicesGroupCode: ["", Validators.required],
      StateCode: ["", Validators.required],
      VTYPE: ["", Validators.required],
      UniqRowID: ["", Validators.required],
      TransType: ["", Validators.required],
      IsAutomatic: ["", Validators.required],
      Schedule_Name: ["", Validators.required],
      NewAccCode: ["", Validators.required],
      PartyACNo: ["", Validators.required],
      PartyACName: ["", Validators.required],
      PartyMICR_No: ["", Validators.required],
      PartyBankName: ["", Validators.required],
      PartyBankBranch: ["", Validators.required],
      PartyBankAddress: ["", Validators.required],
      PartyRTGScode: ["", Validators.required],
      PartyNEFTcode: ["", Validators.required],
      PartyIFSCcode: ["", Validators.required],
      swiftcode: ["", Validators.required],
    });
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }



  getLedgerList() {
    this.accountService.getLedgerList().subscribe(
      response => {
        this.LedgerList = response;
      }
    )
  }//added bleow method to refresh ledger 14/04/21
  Refresh() {
    this.LedgerList = [];
    this.accountService.getLedgerList().subscribe(
      response => {
        this.LedgerList = response;
      }
    )
  }


  Add() {
    this.EnableValidatePermission = false;
    this.ledgerModel = {
      ObjID: null,
      CompanyCode: null,
      BranchCode: null,
      AccCode: 0,
      AccName: null,
      AccType: null,
      GroupID: 0,
      OpeBal: 0,
      OpnBalType: null,
      CustID: 0,
      PartyCode: null,
      GSCode: null,
      ObjStatus: null,
      GSSeqNo: 0,
      SchemeCode: null,
      VATID: null,
      OdLimit: 0,
      UpdateOn: null,
      JFlag: null,
      PANCard: null,
      TIN: null,
      LedgerType: null,
      Address1: null,
      Address2: null,
      Address3: null,
      City: null,
      State: null,
      District: null,
      Country: null,
      PinCode: null,
      Phone: null,
      Mobile: null,
      FaxNo: null,
      WebSite: null,
      CSTNo: null,
      BudgetAmt: 0,
      TDSID: 0,
      EmailID: null,
      HSN: null,
      GSTGoodsGroupCode: null,
      GSTServicesGroupCode: null,
      StateCode: 0,
      VTYPE: null,
      UniqRowID: null,
      TransType: null,
      IsAutomatic: null,
      Schedule_Name: null,
      NewAccCode: null,
      PartyACNo: null,
      PartyACName: null,
      PartyMICR_No: null,
      PartyBankName: null,
      PartyBankBranch: null,
      PartyBankAddress: null,
      PartyRTGScode: null,
      PartyNEFTcode: null,
      PartyIFSCcode: null,
      swiftcode: null,
    }
    this.LedgerEntryForm.reset();
    $('#Ledger').modal('show');
    this.getLedgerType();
    this.getSubGroup();
    this.getLedgerTransactionType();
    this.getLedgerVType();
    this.getGSTGroup();
    this.getGSTServiceGroup();
    this.disabled = false;
  }

  getLedgerType() {
    this.accountService.getLedgerType().subscribe(
      response => {
        this.LedgerType = response;
      }
    )
  }

  getSubGroup() {
    this.accountService.getLedgerSubGroup().subscribe(
      response => {
        this.SubGroup = response;
      }
    )
  }

  getLedgerTransactionType() {
    this.accountService.getLedgerTransactionType().subscribe(
      response => {
        this.TransactionType = response;
      }
    )
  }

  getGSTGroup() {
    this.accountService.getGSTGroup().subscribe(
      response => {
        this.GSTGroup = response;
      }
    )
  }

  getLedgerVType() {
    this.accountService.getLedgerVType().subscribe(
      response => {
        this.VType = response;
      }
    )
  }

  getGSTServiceGroup() {
    this.accountService.getGSTServiceGroup().subscribe(
      response => {
        this.GSTServiceGroup = response;
      }
    )
  }

  getHsnSac(arg) {
    this.accountService.getHSNbyGST(arg).subscribe(
      response => {
        this.HSNSAC = response;
      }
    )
  }


  openField(arg) {
    this.ledgerModel.CompanyCode = this.ccode;
    this.ledgerModel.BranchCode = this.bcode;
    this.ledgerModel.JFlag = "";
    this.accountService.getLedgerDetailsByID(arg.AccountCode).subscribe(
      response => {
        this.LedgerDets = response;
        this.ledgerModel = this.LedgerDets;
        if (this.ledgerModel.ObjStatus == "O") {
          swal("Warning!", "Ledger: " + this.ledgerModel.AccName + " is already opened", "warning");
        }
        else {
          var ans = confirm("Do you want to open the Ledger: " + this.ledgerModel.AccName + "?");
          if (ans) {
            this.ledgerModel.ObjStatus = "O";
            this.accountService.PutLedger(arg.obj_id, this.ledgerModel).subscribe(
              response => {
                swal("Saved!", "Ledger: " + this.ledgerModel.AccName + " Opened", "success");
                this.getLedgerList();
              }
            )
          }
        }
      }
    )
  }

  closeField(arg) {
    this.ledgerModel.CompanyCode = this.ccode;
    this.ledgerModel.BranchCode = this.bcode;
    this.ledgerModel.JFlag = "";
    this.accountService.getLedgerDetailsByID(arg.AccountCode).subscribe(
      response => {
        this.LedgerDets = response;
        this.ledgerModel = this.LedgerDets;
        if (this.ledgerModel.ObjStatus == "C") {
          swal("Warning!", "Ledger: " + this.ledgerModel.AccName + " is already closed", "warning");
        }
        else {
          var ans = confirm("Do you want to close the Ledger: " + this.ledgerModel.AccName + "?");
          if (ans) {
            this.ledgerModel.ObjStatus = "C";
            this.accountService.PutLedger(arg.obj_id, this.ledgerModel).subscribe(
              response => {
                swal("Saved!", "Ledger: " + this.ledgerModel.AccName + " Closed", "success");
                this.getLedgerList();
              }
            )
          }
        }
      }
    )
  }

  LedgerDets: any;

  editField(arg) {
    this.ledgerModel = {
      ObjID: null,
      CompanyCode: null,
      BranchCode: null,
      AccCode: 0,
      AccName: null,
      AccType: null,
      GroupID: 0,
      OpeBal: 0,
      OpnBalType: null,
      CustID: 0,
      PartyCode: null,
      GSCode: null,
      ObjStatus: null,
      GSSeqNo: 0,
      SchemeCode: null,
      VATID: null,
      OdLimit: 0,
      UpdateOn: null,
      JFlag: null,
      PANCard: null,
      TIN: null,
      LedgerType: null,
      Address1: null,
      Address2: null,
      Address3: null,
      City: null,
      State: null,
      District: null,
      Country: null,
      PinCode: null,
      Phone: null,
      Mobile: null,
      FaxNo: null,
      WebSite: null,
      CSTNo: null,
      BudgetAmt: 0,
      TDSID: 0,
      EmailID: null,
      HSN: null,
      GSTGoodsGroupCode: null,
      GSTServicesGroupCode: null,
      StateCode: 0,
      VTYPE: null,
      UniqRowID: null,
      TransType: null,
      IsAutomatic: null,
      Schedule_Name: null,
      NewAccCode: null,
      PartyACNo: null,
      PartyACName: null,
      PartyMICR_No: null,
      PartyBankName: null,
      PartyBankBranch: null,
      PartyBankAddress: null,
      PartyRTGScode: null,
      PartyNEFTcode: null,
      PartyIFSCcode: null,
      swiftcode: null,
    }
    this.EnableValidatePermission = true;
    this.disabled = true;
    this.getLedgerType();
    this.getSubGroup();
    this.getLedgerTransactionType();
    this.getLedgerVType();
    this.getGSTGroup();
    this.getGSTServiceGroup();
    this.accountService.getLedgerDetailsByID(arg).subscribe(
      response => {
        this.LedgerDets = response;
        // console.log(JSON.stringify(this.LedgerDets));
        this.ledgerModel = this.LedgerDets;
        if (this.ledgerModel.ObjStatus == "C") {
          swal("Warning!", this.ledgerModel.AccName + " ( " + this.ledgerModel.AccCode + " ) is closed.", "warning");
          $('#Ledger').modal('hide');
        }
        else {
          $('#Ledger').modal('show');
          if (this.ledgerModel.GroupID != null) {
            this.GetGroup(this.ledgerModel.GroupID);
          }
          if (this.ledgerModel.GSTGoodsGroupCode != null) {
            this.getHsnSac(this.ledgerModel.GSTGoodsGroupCode);
          }
        }
      }
    )
  }


  GetGroup(arg) {
    this.accountService.getGroup(arg).subscribe(
      response => {
        this.GroupId = response;
        this.GroupName = this.GroupId.Group;
      }
    )
  }

  EnableOD() {
    if (this.ledgerModel.AccType == "B") {
      this.EnableDisableODLimit = true;
    }
    else {
      this.EnableDisableODLimit = false;
    }
  }

  Save() {
    if (this.ledgerModel.LedgerType == null) {
      swal("Warning!", "Please select the Ledger Type.", "warning");
    }
    else if (this.ledgerModel.AccName == null) {
      swal("Warning!", "Please enter the Ledger Name.", "warning");
    }
    else if (this.validateAccName(this.ledgerModel.AccName) == true && this.EnableValidatePermission == false) {
      swal("Warning!", "Ledger Name already exists", "warning");
    }
    else if (this.ledgerModel.AccType == null) {
      swal("Warning!", "Please select the Ledger Category.", "warning");
    }
    else if (this.ledgerModel.OpnBalType == null) {
      swal("Warning!", "Please select the Balance Type.", "warning");
    }
    else {
      this.ledgerModel.CompanyCode = this.ccode;
      this.ledgerModel.BranchCode = this.bcode;
      this.ledgerModel.JFlag = "";
      this.ledgerModel.ObjStatus = "O";
      var ans = confirm("Do you want to save details?");
      if (ans) {
        if (this.ledgerModel.ObjID == null) {
          this.accountService.PostLedger(this.ledgerModel).subscribe(
            response => {
              //swal("Saved!", "Ledger Created", "success");
              $('#Ledger').modal('hide');
              this.LedgerEntryForm.reset();
              this.getLedgerList();
            }
          )
        }
        else {
          this.accountService.PutLedger(this.ledgerModel.ObjID, this.ledgerModel).subscribe(
            response => {
              swal("Saved!", "Ledger Updated", "success");
              $('#Ledger').modal('hide');
              this.LedgerEntryForm.reset();
              this.getLedgerList();
            }
          )
        }
      }
    }
  }

  validateAccName(arg) {
    let dupCheck = this.LedgerList.find(x => x.AccountName == arg.toUpperCase())
    if (dupCheck != null) {
      return true;
    }
    else {
      return false;
    }
  }

  PrintDetails: any = [];

  getHtmlPrint() {
    this.accountService.PrintLedger().subscribe(
      response => {
        this.PrintDetails = response;
        $('#ReprintModal').modal('show');
        $('#DisplayData').html(this.PrintDetails);
      }
    )
  }

  print() {
    let printContents, popupWin;
    printContents = document.getElementById('DisplayData').innerHTML;
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    popupWin.document.open();
    popupWin.document.write(`
      <html>
        <head>
          <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css" integrity="sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm" crossorigin="anonymous">
          <title>Print tab</title>
          <style>
         .htmlPrint{display:none;}

          @media print {
            .table-bordered
        {
            border-style: solid;
            border: 3px solid rgb(0, 0, 0);
        }
        .margin{
          margin-top:-24px;
        }
        .mb{
          vertical-align:middle;position:absolute;
        }
        tr.spaceUnder>td {
          padding-bottom: 40px !important;
        }
        .top {
          margin-top:10px;
          border-bottom: 3px solid rgb(0, 0, 0) !important;
          line-height: 1.6;
        }
        .modal-content{
          font-family: "Times New Roman", Times, serif;

        }
       .padding-top {
         padding-top:20px;
       }
        .watermark{
          -webkit-transform: rotate(331deg);
          -moz-transform: rotate(331deg);
          -o-transform: rotate(331deg);
          transform: rotate(331deg);
          font-size: 15em;
          color: rgba(255, 5, 5, 0.37);
          position: absolute;
          text-transform:uppercase;
          padding-left: 10%;
          margin-top:-10px;
        }
        .right{
          text-align:left;
        }
        .px-2 {
          padding-left: 10px !important;
        }
        thead > tr
        {
            border-style: solid;
            border: 3px solid rgb(0, 0, 0);
        }
        table tr td.classname{
          border-right: 3px solid rgb(0, 0, 0) !important;
        }
        table tr td.bottom{
          border-bottom: 3px solid rgb(0, 0, 0) !important;
        }
        table tr th.classname{
          border-right: 3px solid rgb(0, 0, 0) !important;
        }
        .divborder {
          border-right: 3px solid rgb(0, 0, 0) !important;
          line-height: 1.6;
        }
        .lastdivborder{
          line-height: 1.6;
        }
    .border{
      border-right: 3px solid rgb(0, 0, 0) !important;
      border-bottom: 3px solid rgb(0, 0, 0) !important;
    }

    .tdborder{
      border-right: 3px solid rgb(0, 0, 0) !important;
    }
        .invoice {
          margin-top: 250px;
      }
        .card{
          border-style: solid;
          border-width: 5px;
          border: 3px solid rgb(0, 0, 0);
      }
        .printMe {
          display: none !important;
        }
      }
      body{
            font-size: 15px;
            line-height: 18px;
      }
 </style>
        </head>
    <body onload="window.print();window.close()">
    ${printContents}</body>
      </html>`
    );
    popupWin.document.close();
  }



  ValidatePermission() {
    $("#PermissonModal").modal('show');
    this.Pwd.nativeElement.value = "";
  }

  passWord(arg) {
    if (arg == "") {
      swal("Warning!", "Please Enter the Password", "warning");
      $('#PermissonModal').modal('show');
    }
    else {
      this.permissonModel.CompanyCode = this.ccode;
      this.permissonModel.BranchCode = this.bcode;
      this.permissonModel.PermissionID = this.PasswordValid;
      this.permissonModel.PermissionData = btoa(arg);
      this.accountService.postelevatedpermission(this.permissonModel).subscribe(
        response => {
          $('#PermissonModal').modal('hide');
          this.Index = -1;
          this.disabled = false;
        }
      )
    }
  }

  permissonModel: any = {
    CompanyCode: null,
    BranchCode: null,
    PermissionID: null,
    PermissionData: null
  }
}