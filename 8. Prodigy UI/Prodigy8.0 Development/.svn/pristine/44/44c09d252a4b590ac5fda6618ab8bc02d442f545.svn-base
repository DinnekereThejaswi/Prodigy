import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-online-order-management-system',
  templateUrl: './online-order-management-system.component.html',
  styleUrls: ['./online-order-management-system.component.css']
})
export class OnlineOrderManagementSystemComponent implements OnInit {

  EnableReceivedOrders: boolean = true;
  EnableItemPicking: boolean = true;
  EnableItemPacking: boolean = true;
  EnableTaxInvoiceGeneration: boolean = true;
  EnableItemDispatch: boolean = true;

  constructor() { }

  ngOnInit() {

  }

  ToggleReceivedOrders() {
    this.EnableReceivedOrders = !this.EnableReceivedOrders;
  }

  ToggleItemPicking() {
    this.EnableItemPicking = !this.EnableItemPicking;
  }

  ToggleItemPacking() {
    this.EnableItemPacking = !this.EnableItemPacking;
  }

  ToggleTaxInvoiceGeneration() {
    this.EnableTaxInvoiceGeneration = !this.EnableTaxInvoiceGeneration;
  }

  ToggleItemDispatch() {
    this.EnableItemDispatch = !this.EnableItemDispatch;
  }
}