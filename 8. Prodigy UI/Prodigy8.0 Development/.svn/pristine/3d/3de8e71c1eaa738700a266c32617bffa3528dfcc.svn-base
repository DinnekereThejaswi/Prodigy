import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { OrdersService } from '../orders.service';
import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';

@Component({
  selector: 'app-order-issue-to-cpc',
  templateUrl: './order-issue-to-cpc.component.html',
  styleUrls: ['./order-issue-to-cpc.component.css']
})
export class OrderIssueToCpcComponent implements OnInit {

  CollapseItemDetailsTab: boolean = true;
  CollapseOrderDetailsTab: boolean = true;
  OrderCPCForm: FormGroup;
  metalList: any = [];
  counterList: any = [];
  GSList: any = [];
  karatList: any = [];
  SalesManList: any = [];
  EnableOrderDetails: boolean = false;
  EnableStoneDetails: boolean = true;
  password: string;
  EnableJson: boolean = false;
  ccode: string;
  bcode: string;
  ALL: string = "ALL";
  constructor(private fb: FormBuilder,
    private _ordersService: OrdersService,
    private _appConfigService: AppConfigService,) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.postOrderList.CompanyCode = this.ccode;
    this.postOrderList.BranchCode = this.bcode;
  }

  ToggleOrderDetails() {
    this.EnableOrderDetails = !this.EnableOrderDetails;
  }

  ToggleStoneDetails() {
    this.EnableStoneDetails = !this.EnableStoneDetails;
  }
  ngOnInit() {
    this.getMetalList();
    this.getKarat();
    this.getSalesMan();
    this.OrderCPCForm = this.fb.group({
      stock: ["ALL"],
      counterCode: ["ALL"],
      gsCode: ["ALL"],
      karat: ["ALL"],
      orderType: ["O"],
      issuedBy: [null],
    });
  }

  OrderTypeData = [
    {
      'name': 'Customised Order',
      'value': 'O'
    },
    {
      'name': ' Order Advance',
      'value': 'A'
    },
    {
      'name': ' Reserved Order',
      'value': 'R'
    },
  ]

  getMetalList() {
    this._ordersService.getMetalsDetail().subscribe(response => {
      this.metalList = response;
      this._ordersService.getCounter().subscribe(
        response => {
          this.counterList = response;
        }
      )
    })
  }

  getGS(form) {
    this._ordersService.getGSDetail(form.value.stock).subscribe(
      response => {
        this.GSList = response;
      }
    )
  }

  getKarat() {
    this._ordersService.getKarat().subscribe(
      response => {
        this.karatList = response;
      }
    )
  }

  getSalesMan() {
    this._ordersService.getSalesManData().subscribe(response => {
      this.SalesManList = response;
      this._ordersService.getGSDetail("ALL").subscribe(
        response => {
          this.GSList = response;
        }
      )
    })
  }

  OrderList: any = [];

  postOrderList: any = {
    CompanyCode: null,
    BranchCode: null,
    GSCode: "ALL",
    Karat: "ALL",
    Counter: "ALL",
    OrderType: "O",
    IssuedUser: null,
    issueLines: []
  }

  Go(form) {
    if (form.value.stock == null) {
      swal("Warning!", 'Please select the Metal', "warning");
    }
    else if (form.value.counterCode == null) {
      swal("Warning!", 'Please select the Counter', "warning");
    }
    else if (form.value.gsCode == null) {
      swal("Warning!", 'Please select the GS', "warning");
    }
    else if (form.value.karat == null) {
      swal("Warning!", 'Please select the karat', "warning");
    }
    else if (form.value.orderType == null) {
      swal("Warning!", 'Please select the Order Type', "warning");
    }
    else if (form.value.issuedBy == null) {
      swal("Warning!", 'Please select the Issue By', "warning");
    }
    else {
      this.postOrderList.issueLines = [];
      this._ordersService.getOrderIssueToCpcList(form.value).subscribe(response => {
        this.OrderList = response;
      })
    }
  }

  clear() {
    this.OrderList = [];
    this.postOrderList.issueLines = [];
  }

  checkAll(event) {
    if (event.target.checked) {
      this.postOrderList.issueLines = this.OrderList;
      this.postOrderList.issueLines.forEach(x => x.state = event.target.checked)
    }
    else {
      this.postOrderList.issueLines.forEach(x => x.state = event.target.unchecked)
      this.postOrderList.issueLines = [];
    }
  }

  onCheckboxChange(option, event, index) {
    if (event.target.checked) {
      this.postOrderList.issueLines.push(option);
    }
    else {
      for (let i = 0; i < this.postOrderList.issueLines.length; i++) {
        if (this.postOrderList.issueLines[i].OrderNo == option.OrderNo) {
          this.postOrderList.issueLines.splice(i, 1);
          break;
        }
      }
    }
  }

  isAllChecked() {
    return this.OrderList.every(_ => _.state);
  }

  outputData: any = [];
  outputXmlData: any;
  Submit() {
    if (this.postOrderList.issueLines.length == 0) {
      swal("Warning!", 'Please select orders to issue', "warning");
    }
    else {
      this._ordersService.postOrderIssueToCPC(this.postOrderList).subscribe(
        response => {
          this.outputData = response;
          swal("Saved!", this.outputData.Message, "success");
          this.postOrderList = {
            CompanyCode: null,
            BranchCode: null,
            GSCode: "ALL",
            Karat: "ALL",
            Counter: "ALL",
            OrderType: "O",
            IssuedUser: null,
            issueLines: []
          }
          this.OrderList = [];
          this.OrderCPCForm = this.fb.group({
            stock: ["ALL"],
            counterCode: ["ALL"],
            gsCode: ["ALL"],
            karat: ["ALL"],
            orderType: ["O"],
            issuedBy: [null],
          });
          this.getCB();
          this._ordersService.postOrderIssueToCPCXML(this.outputData.DocumentNo).subscribe(
            response => {
              this.outputXmlData = response;
              this._ordersService.getGSDetail("ALL").subscribe(
                response => {
                  this.GSList = response;
                }
              )
            }
          )
        }
      )
    }
  }
}