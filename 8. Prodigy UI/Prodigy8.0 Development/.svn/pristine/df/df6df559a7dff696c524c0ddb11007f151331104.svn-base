import { ReceiptModel } from './../orders.model';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { OrdersService } from './../orders.service';
import { Component, OnInit } from '@angular/core';
import { AppConfigService } from '../../AppConfigService';
@Component({
  selector: 'app-receipt',
  templateUrl: './receipt.component.html',
  styleUrls: ['./receipt.component.css']
})
export class ReceiptComponent implements OnInit {

  constructor(private _ordersService: OrdersService, private formBuilder: FormBuilder,
    private _appConfigService: AppConfigService) {
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
  }

  ReceiptForm: FormGroup;
  EnableJson: boolean = false;
  password: string;
  ngOnInit() {
    this.getPaymentMode();
    this.getBank();

    this.ReceiptForm = this.formBuilder.group({
      PayMode: null,
      RefBillNo: null,
      CardNo: null,
      CardAppNo: null,
      CardType: null,
      BankName: null,
      Bank: null,
      ExpiryDate: null,
      CardCharges: null,
      CardSwipedBy: null,
      ChequeNo: null,
      ChequeDate: null,
      Details: null,
      AmtReceived: null,
      SchemeCode: null,
      GroupCode: null,
      CTBranch: null,
    });
  }

  PaymentModes: any;
  getPaymentMode() {
    this._ordersService.getPaymentMode().subscribe(
      response => {
        this.PaymentModes = response;
      }
    )
  }

  BankList: any;
  getBank() {
    this._ordersService.getBank().subscribe(
      response => {
        this.BankList = response;
      }
    )
  }

  ReceiptHeaders: ReceiptModel = {
    PayMode: null,
    RefBillNo: null,
    CardNo: null,
    CardAppNo: null,
    CardType: null,
    BankName: null,
    Bank: null,
    ExpiryDate: null,
    CardCharges: null,
    CardSwipedBy: null,
    ChequeNo: null,
    ChequeDate: null,
    Details: null,
    AmtReceived: null,
    SchemeCode: null,
    GroupCode: null,
    CTBranch: null,
    PayAmount: null
  }

  toggle: string;
  onchangePaymode(paymodeArg) {

    switch (paymodeArg) {
      case "Q": {
        this.toggle = "Cheque";
        break;
      }
      case "D": {
        this.toggle = "DD";
        break;
      }
      case "C": {
        this.toggle = "Cash";
        break;
      }
      case "R": {
        this.toggle = "Card";
        break;
      }
      case "BC": {
        this.toggle = "BC";
        break;
      }
      default: {
        this.toggle = "Invalid";
        break;
      }
    }
  }

  ArrayList: any = [];
  PaymentList: any = [];

  addRow(arrayLines) {
    this.ArrayList.push(arrayLines.value);
    this.ReceiptForm.reset();
    arrayLines = null;
    this.toggle = "Invalid"
  }

  DeleteRow(index: number, arg) {
    var ans = confirm("Do you want to delete");
    if (ans) {
      this.ArrayList.splice(index, 1)
    }
  }
}