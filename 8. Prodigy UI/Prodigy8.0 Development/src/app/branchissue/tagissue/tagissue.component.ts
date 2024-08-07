import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BranchissueService } from './../branchissue.service';
import swal from 'sweetalert';
import { MasterService } from '../../core/common/master.service';
import { ToastrService } from 'ngx-toastr';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import { analyzeAndValidateNgModules } from '@angular/compiler';
import { formatDate } from '@angular/common';
import { Router } from '@angular/router';
declare var $: any;

@Component({
  selector: 'app-tagissue',
  templateUrl: './tagissue.component.html',
  styleUrls: ['./tagissue.component.css']
})
export class TagissueComponent implements OnInit {
  ApplicationDate: any = [];
  date: any;
  password: string;
  routerUrl: string = "";
  TagIssueForm: FormGroup;
  CollapseItemDetailsTab: boolean = false;
  CollapseStoneDetailsTab: boolean = true;
  EnableJson: boolean = false;

  @ViewChild("barcodeNo", { static: false }) barcodeNo: ElementRef;

  constructor(private _branchissueService: BranchissueService,
    private formBuilder: FormBuilder, private _appConfigService: AppConfigService,
    private _masterService: MasterService, private toastr: ToastrService, private _router: Router) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this.routerUrl = this._router.url;
    this.getDateofTheApplication();
    this.GetIssueTo();
    this.getGSList();
    this.TagIssueForm = this.formBuilder.group({
      issueTo: [null, Validators.required],
      issueType: [null, Validators.required],
      gscode: [null, Validators.required],
      barcode: [null, Validators.required],
      remarks: [null, Validators.required],
      IssuedBy: "Admin",
      Date: null,
    });
  }
  getDateofTheApplication() {
    this._branchissueService.getApplicationDate().subscribe(
      response => {
        this.ApplicationDate = response;
        this.date = formatDate(this.ApplicationDate.appViewDate, 'dd/MM/yyyy', 'en_GB');
      }
    )
  }
  IssueTo: any = [];

  barcodeIssuePost: any = {
    CompanyCode: null,
    BranchCode: null,
    IssueTo: null,
    IssueType: null,
    GSCode: null,
    Remarks: null,
    IssueLines: []
  }

  GetIssueTo() {
    this._branchissueService.getIssuesTo().subscribe(
      response => {
        this.IssueTo = response;
      }
    )
  }

  ToggleissueDetails() {
    this.CollapseItemDetailsTab = !this.CollapseItemDetailsTab;
  }

  TogglestoneDetails() {
    this.CollapseStoneDetailsTab = !this.CollapseStoneDetailsTab;
  }


  barcodeDetails: any;
  EnableBarcodeDetails: boolean = false;
  finalBarcodeDetails: any = [];

  getDetails(form) {
    if (form.value.issueTo == null) {
      swal("Warning!", 'Please select IssueTo', "warning");
    }
    else if (form.value.gscode == null || form.value.gsType == "") {
      swal("Warning!", 'Please select GSType', "warning");
    }
    else if (form.value.barcode == null || form.value.barcode == "") {
      swal("Warning!", 'Please enter the barcode', "warning");
    }
    else {
      this._branchissueService.getBarcodeDetail(form.value.issueTo, form.value.gscode, form.value.barcode).subscribe(
        response => {
          this.barcodeDetails = response;
          let data = this.finalBarcodeDetails.find(x => x.BarcodeNo == form.value.barcode);
          if (data == null) {
            this.finalBarcodeDetails.push(this.barcodeDetails);
            this.getStoneDetails();
            this.barcodeNo.nativeElement.value = "";
          }
          else {
            if (this.finalBarcodeDetails.length >= 1) {
              swal("Warning!", "Barcode number already added", "warning");
            }
          }
        }
      )
    }
  }

  StoneDetails: any = [];

  getStoneDetails() {
    if (this.finalBarcodeDetails.length == 0) {
      this.StoneDetails = [];
    }
    else {
      for (let i = 0; i < this.finalBarcodeDetails.length; i++) {
        if (this.finalBarcodeDetails[i].StoneDetails.length > 0) {
          for (let j = 0; j < this.finalBarcodeDetails[i].StoneDetails.length; j++) {
            this.StoneDetails.push(this.finalBarcodeDetails[i].StoneDetails[j]);
          }
        }
      }
    }
  }

  DeleteTagRow(index: number, arg) {
    var ans = confirm("Do you want to delete");
    if (ans) {
      this.finalBarcodeDetails.splice(index, 1);
      this.getStoneDetails();
    }
  }

  total_items(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      total += d.Qty;
    });
    return total;
  }

  grossWT(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      total += d.GrossWt;
    });
    return total;
  }

  stoneWt(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      total += d.StoneWt;
    });
    return total;
  }

  netWT(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      total += d.NetWt;
    });
    return total;
  }

  dcts(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      total += d.Dcts;
    });
    return total;
  }

  amount(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      total += d.AmountAfterTax;
    });
    return total;
  }
  tagWeight(arg: any[] = []) {
    let total = 0;
    arg.forEach((d) => {
      total += d.TagWeight;
    });
    return total;

  }


  ccode: string = "";
  bcode: string = "";
  outputBarcodeIssue: any;
  SL: string = "SL";
  DMD: string = "DMD";
  B: string = "B";

  PrintTypeBasedOnConfig: any;
  tagIssuePrint: any = [];
  outputXml: any;

  Submit() {
    this.barcodeIssuePost.CompanyCode = this.ccode;
    this.barcodeIssuePost.BranchCode = this.bcode;
    this.barcodeIssuePost.IssueLines = this.finalBarcodeDetails;
    var ans = confirm("Do you want to Issue Barcodes to " + this.barcodeIssuePost.IssueTo + "??");
    if (ans) {
      this._branchissueService.post(this.barcodeIssuePost).subscribe(
        response => {
          this.outputBarcodeIssue = response;
          swal("Saved!", this.outputBarcodeIssue.Message, "success");
          this.finalBarcodeDetails = [];
          this.StoneDetails = [];
          this.TagIssueForm.reset();
          this._branchissueService.generateBarcodeXML(this.outputBarcodeIssue.DocumentNo).subscribe(
            response => {
              this.outputXml = response;
              var ans = confirm("Do you want to take print??");
              if (ans) {
                this._branchissueService.getBarcodePrint(this.outputBarcodeIssue.DocumentNo).subscribe(
                  response => {
                    this.PrintTypeBasedOnConfig = response;
                    if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
                      $('#TAGIssueTab').modal('show');
                      this.tagIssuePrint = atob(this.PrintTypeBasedOnConfig.Data);
                      $('#DisplaytagIssueData').html(this.tagIssuePrint);
                    }
                    else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
                      $('#TAGIssueTab').modal('show');
                      this.tagIssuePrint = this.PrintTypeBasedOnConfig.Data;
                    }
                  }
                )
              }
            }
          )
        }
      )
    }
  }

  GSList: any = [];

  getGSList() {
    this._masterService.getGsList('S').subscribe(
      Response => {
        this.GSList = Response;
      }
    )
  }

  printTagIssue() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.tagIssuePrint);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printtagIssueData, popupWin;
      printtagIssueData = document.getElementById('DisplaytagIssueData').innerHTML;
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
    ${printtagIssueData}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }

}
