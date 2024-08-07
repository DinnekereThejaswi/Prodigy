import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import * as CryptoJS from 'crypto-js';
import { FormGroup, FormBuilder } from '@angular/forms';
import { GsStockService } from './gs-stock.service';
import { AppConfigService } from '../../AppConfigService';
import { MasterService } from '../../core/common/master.service';
import * as XLSX from 'xlsx';

@Component({
  selector: 'app-gs-stock',
  templateUrl: './gs-stock.component.html',
  styleUrls: ['./gs-stock.component.css']
})
export class GsStockComponent implements OnInit {
  GSStockForm: FormGroup;
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;
  @ViewChild('TABLE', { static: false }) TABLE: ElementRef;

  UOM = [
    { UOMCode: 'W', UOMName: 'Weight' },
    { UOMCode: 'P', UOMName: 'Piece' },
    { UOMCode: 'C', UOMName: 'Carets' }
  ]
  p: boolean = true;
  w: boolean = false;
  c: boolean = false;
  constructor(private fb: FormBuilder, private service: GsStockService, private appConfigService: AppConfigService,
    private _masterService: MasterService) {
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.password = this.appConfigService.Pwd;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }
  ngOnInit() {
    this.GSStockForm = this.fb.group({
      UOM: 'P',
    });
    this.onChange(this.GSStockForm.value.UOM);
    this.getApplicationDate();
  }

  applicationDate: any;
  disAppDate: any;
  getApplicationDate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
      }
    )
  }


  GSStockList: any = [];
  onChange(arg) {
    if (arg === 'P') {
      this.p = true;
      this.w = false;
      this.c = false;

    }
    else if (arg == 'W') {
      this.p = false;
      this.w = true;
      this.c = false;
    }
    else if (arg == 'C') {
      this.p = false;
      this.w = false;
      this.c = true;
    }
    this.service.getGSStock(arg).subscribe(
      Response => {
        this.GSStockList = Response;
      }
    )
  }

  ExportTOExcel() {
    const ws: XLSX.WorkSheet = XLSX.utils.table_to_sheet(this.TABLE.nativeElement);
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Sheet1');
    XLSX.writeFile(wb, 'GsStock.xlsx');
  }
}