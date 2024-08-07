import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { OnlineOrderManagementSystemService } from '../online-order-management-system.service';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { MasterService } from '../../core/common/master.service';
import { formatDate } from '@angular/common';
import * as moment from 'moment';
import { NgxSpinnerService } from "ngx-spinner";

@Component({
  selector: 'app-tax-invoice-generation',
  templateUrl: './tax-invoice-generation.component.html',
  styleUrls: ['./tax-invoice-generation.component.css']
})
export class TaxInvoiceGenerationComponent implements OnInit {

  TaxInvoiceForm: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  isReadOnly: boolean = false;
  EnableJson: boolean = false;
  TaxInvoiceJson: any = {
    StartShipDate: "",
    EndShipDate: "",
    AssignmentNo: null
  }

  ////date
  datePickerConfig: Partial<BsDatepickerConfig>;
  Date: string;
  OfferStartDate: string;
  today = new Date();
  shipmentEndDate: number = 0;
  /////////////
  constructor(
    private fb: FormBuilder, private _appConfigService: AppConfigService,
    private _OMSserice: OnlineOrderManagementSystemService,
    private _masterService: MasterService, private SpinnerService: NgxSpinnerService
  ) {
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        dateInputFormat: 'DD/MM/YYYY'
      });
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.shipmentEndDate = this._appConfigService.ShipmentEndDate;
    this.getCB();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this.getPickListNo();
    this.getApplicationDate();
    this.TaxInvoiceForm = this.fb.group({
      StartShipDate: [null, Validators.required],
      EndShipDate: [null, Validators.required],
      AssignmentNo: [null, Validators.required]
    });
  }

  applicationDate: any;
  EnableAssignmentNo: boolean = false;

  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
        this.TaxInvoiceJson.StartShipDate = formatDate(this.applicationDate, 'dd/MM/yyyy', 'en_GB');
        this.addDays();
      }
    )
  }

  futureDate: any;
  addDays() {
    this.futureDate = moment(this.applicationDate).add(this.shipmentEndDate, 'days');
    this.TaxInvoiceJson.EndShipDate = formatDate(this.futureDate, 'dd/MM/yyyy', 'en_GB');
  }

  orderStage: string = "invoice";
  PickListNumber: any = [];

  getPickListNo() {
    this.TaxInvoiceJson.AssignmentNo = null;
    this.orderToBeInvoiced = [];
    this._OMSserice.getPickListNumber(this.orderStage).subscribe(
      response => {
        this.PickListNumber = response;
      }
    )
  }

  refreshPickListNo() {
    this._OMSserice.getPickListNumber(this.orderStage).subscribe(
      response => {
        this.PickListNumber = response;
      });
  }


  orderToBeInvoiced: any = [];

  orderstobeinvoiced() {
    if (this.TaxInvoiceJson.AssignmentNo == null || this.TaxInvoiceJson.AssignmentNo == "null") {
      swal("Warning!", 'Please select the Assignment Number', "warning");
    }
    else {
      this.getorderstobeinvoiced();
    }
  }


  getorderstobeinvoiced() {
    this.TaxInvoiceJson.StartShipDate = moment(this.TaxInvoiceJson.StartShipDate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    this.TaxInvoiceJson.EndShipDate = moment(this.TaxInvoiceJson.EndShipDate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    this._OMSserice.orderstobeinvoiced(this.TaxInvoiceJson).subscribe(
      response => {
        this.orderToBeInvoiced = response;
        this.TaxInvoiceJson.StartShipDate = moment(this.TaxInvoiceJson.StartShipDate, 'YYYY-MM-DD').format('DD/MM/YYYY');
        this.TaxInvoiceJson.EndShipDate = moment(this.TaxInvoiceJson.EndShipDate, 'YYYY-MM-DD').format('DD/MM/YYYY');
      },
      (err) => {
        this.orderToBeInvoiced = [];
        this.TaxInvoiceJson.StartShipDate = moment(this.TaxInvoiceJson.StartShipDate, 'YYYY-MM-DD').format('DD/MM/YYYY');
        this.TaxInvoiceJson.EndShipDate = moment(this.TaxInvoiceJson.EndShipDate, 'YYYY-MM-DD').format('DD/MM/YYYY');
      }
    )
  }

  validateFromDate: string = "";
  validateToDate: string = "";


  getorderstobeinvoicedbasedondates() {
    this.TaxInvoiceJson.AssignmentNo = null;
    this.validateFromDate = moment(this.TaxInvoiceJson.StartShipDate, 'DD/MM/YYYY').format('YYYYMMDD');
    this.validateToDate = moment(this.TaxInvoiceJson.EndShipDate, 'DD/MM/YYYY').format('YYYYMMDD');
    if (this.validateFromDate > this.validateToDate) {
      swal("Warning!", 'End Date should be greater than or equal to Start Date', "warning");
      this.orderToBeInvoiced = [];
    }
    else {
      this.getorderstobeinvoiced();
    }
  }


  outputData: any;
  inputData: any;

  generateInvoice(arg) {
    this.inputData = arg;
    this.SpinnerService.show();
    this._OMSserice.generateInvoice(this.inputData).subscribe(
      response => {
        this.outputData = response;
        //#region Old Logic 
        // if (this.outputData.MarketPlaceInvoiceData != null && this.outputData.MarketPlaceInvoiceData != "") {
        //   this.download(this.outputData.MarketPlaceInvoiceData.FileFormat, this.outputData.MarketPlaceInvoiceData.FileData, this.outputData.MarketPlaceInvoiceData.Name);
        // }
        // if (this.outputData.PoSInvoiceData != null && this.outputData.PoSInvoiceData != "") {
        //   this.download(this.outputData.PoSInvoiceData.FileFormat, this.outputData.PoSInvoiceData.FileData, this.outputData.PoSInvoiceData.Name);
        // }
        //#endregion

        //New Logic
        if (this.outputData != null) {
          for (let i = 0; i < this.outputData.Data.length; i++) {
            this.download(this.outputData.Data[i].FileFormat, this.outputData.Data[i].FileData, this.outputData.Data[i].Name);
          }
        }
        //ends here

        this.SpinnerService.hide();
        if (this.TaxInvoiceJson.AssignmentNo == null || this.TaxInvoiceJson.AssignmentNo == "null") {
          this.getorderstobeinvoicedbasedondates();
        }
        else {
          this.getorderstobeinvoiced();
        }
      },
      (err) => {
        this.SpinnerService.hide();
      }
    )
  }

  generateShipLabel(arg) {
    this.inputData = arg;
    this.SpinnerService.show();
    this._OMSserice.generateshiplabel(this.inputData).subscribe(
      response => {
        this.outputData = response;
        if (this.outputData != null) {
          for (let i = 0; i < this.outputData.Data.length; i++) {
            this.download(this.outputData.Data[i].FileFormat, this.outputData.Data[i].FileData, this.outputData.Data[i].Name);
          }
        }
        // if (this.outputData.MarketPlaceInvoiceData != null && this.outputData.MarketPlaceInvoiceData != "") {
        //   this.download(this.outputData.MarketPlaceInvoiceData.FileFormat, this.outputData.MarketPlaceInvoiceData.FileData, this.outputData.MarketPlaceInvoiceData.Name);
        // }
        // if (this.outputData.PoSInvoiceData != null && this.outputData.PoSInvoiceData != "") {
        //   this.download(this.outputData.PoSInvoiceData.FileFormat, this.outputData.PoSInvoiceData.FileData, this.outputData.PoSInvoiceData.Name);
        // }
        this.SpinnerService.hide();
        if (this.TaxInvoiceJson.AssignmentNo == null || this.TaxInvoiceJson.AssignmentNo == "null") {
          this.getorderstobeinvoicedbasedondates();
        }
        else {
          this.getorderstobeinvoiced();
        }
      },
      (err) => {
        this.SpinnerService.hide();
      }
    )
  }

  FileFormat: string;
  FileData: string;
  FileName: string;
  DisplayData: boolean = false;
  dataSource: string;
  orderDetails: any = [];


  download(FileFormat, FileData, FileName) {
    this.DisplayData = true;
    this.FileFormat = FileFormat;
    this.FileData = FileData;
    this.FileName = FileName + "." + FileFormat;

    //#region Directly Downloading to browser

    //const linkSource = 'data:application/' + this.FileFormat + ';base64,' + this.FileData;
    // const downloadLink = document.createElement("a");
    // const fileName = this.FileName;
    // downloadLink.href = linkSource;
    // downloadLink.download = fileName;
    // downloadLink.click();

    //#endregion

    //#region Directly opening in browser

    let pdfWindow = window.open("");
    if (FileFormat.toLowerCase() == "pdf") {
      pdfWindow.document.write("<html<head><title>" + FileName + "</title><style>body{margin: 0px;}iframe{border-width: 0px;}</style></head>");
      pdfWindow.document.write("<body><embed width='100%' height='100%' src='data:application/pdf;base64, " + FileData + "#toolbar=0&navpanes=0&scrollbar=0'></embed></body></html>");
    }
    else if (FileFormat.toLowerCase() == "png") {
      pdfWindow.document.write("<html<head><title>" + FileName + "</title><style>body{margin: 0px;}iframe{border-width: 0px;}</style></head>");
      pdfWindow.document.write("<body><embed src='data:image/png;base64, " + FileData + "#toolbar=0&navpanes=0&scrollbar=0'></embed></body></html>");
    }
    else {
      pdfWindow.document.write("<html<head><title>" + FileName + "</title><style>body{margin: 0px;}iframe{border-width: 0px;}</style></head>");
      pdfWindow.document.write(atob(FileData));
    }

    //#endregion
  }
}