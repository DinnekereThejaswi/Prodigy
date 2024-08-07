import { FormGroup, FormBuilder } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { StockClearService } from './stock-clear.service';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
@Component({
  selector: 'app-stock-clear',
  templateUrl: './stock-clear.component.html',
  styleUrls: ['./stock-clear.component.css']
})
export class StockClearComponent implements OnInit {
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;
  //radioItems: Array<string>;
  Sort = [
    { Code: 'B', Name: 'Batch' },
    { Code: 'C', Name: 'Counter' },
    { Code: 'I', Name: 'Item' },
    { Code: 'C', Name: 'Salesman' },
  ];
  // model = { option: 'Batch' };
  StockClearForm: FormGroup;
  constructor(private fb: FormBuilder, private stockservice: StockClearService, private appConfigService: AppConfigService) {
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.password = this.appConfigService.Pwd;
    this.getCB();
  }

  toggleBatchNo: boolean = true;
  toggleCounter: boolean = false;
  toggleItem: boolean = false;
  toggleSalesman: boolean = false;
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this.StockClearForm = this.fb.group({
      CompanyCode: null,
      BranchCode: null,
      Sort: 'Batch',
    });

    this.getStockTable(this.StockClearForm.value.Sort);
  }

  Changed(arg) {

    if (arg === 'Batch') {
      this.toggleBatchNo = true;
      this.toggleCounter = false;
      this.toggleItem = false;
      this.toggleSalesman = false;
      this.getStockTable(arg);
      // this.model.option = arg;
    }
    else if (arg === 'Counter') {

      this.toggleBatchNo = false;
      this.toggleCounter = true;
      this.toggleItem = false;
      this.toggleSalesman = false;
      this.getStockTable(arg);
      // this.model.option = arg;
    }
    else if (arg === 'Item') {
      this.toggleBatchNo = false;
      this.toggleCounter = false;
      this.toggleItem = true;
      this.toggleSalesman = false;
      this.getStockTable(arg);
      //  this.model.option = arg;
    }
    else if (arg === 'Salesman') {
      this.toggleBatchNo = false;
      this.toggleCounter = false;
      this.toggleItem = false;
      this.toggleSalesman = true;
      this.getStockTable(arg);
      //  this.model.option = arg;
    }

  }
  StockList: any = [];
  getStockTable(arg) {
    this.stockservice.getStock(arg).subscribe(
      Response => {
        this.StockList = Response;
      }
    )
  }
  ClearStock(arg) {
    var ans = confirm('Do you want to delete');
    if (ans) {
      this.stockservice.ClearStock(arg).subscribe(
        data => {
          this.getStockTable(this.StockClearForm.value.Sort);
        }
      )
    }
  }
}