import { PaymentService } from './../../payment/payment.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { CustomerService } from './../../masters/customer/customer.service';
import { SalesreturnService } from './../salesreturn.service';
import { formatDate } from '@angular/common';
import { MasterService } from './../../core/common/master.service';
import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import swal from 'sweetalert';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { OrdersService } from './../../orders/orders.service';
import { CreditReceiptService } from '../../credit-receipt/credit-receipt.service';
import { ComponentCanDeactivate } from '../../appconfirmation-guard';
import { AppConfigService } from '../../AppConfigService';

declare var $: any;


@Component({
  selector: 'app-confirm-salesreturn',
  templateUrl: './confirm-salesreturn.component.html',
  styleUrls: ['./confirm-salesreturn.component.css']
})
export class ConfirmSalesreturnComponent implements OnInit, OnDestroy, ComponentCanDeactivate {
  @ViewChild('salesReturnEstNo', { static: true }) salesReturnEstNo: ElementRef;
  salesReturnDate = '';
  today = new Date();
  leavePage: boolean = false;
  routerUrl: string = "";
  ConfirmSRHeaderForm: FormGroup;
  BillNo: number;
  fieldArray: any = [];
  public CollapseCustomerDetailsTab: boolean = true;
  EnableSRSummary: boolean = false;
  public pageName = "SR"
  autoFetchAmount: number = 0;
  EnableConfirmSR: boolean = false;
  radioItems: Array<string>;
  EnableJson: boolean = false;
  width = 1;
  password: string;
  height = 25;
  model = { option: 'Adjust' };
  paymentDetails: any;
  constructor(private _SalesreturnService: SalesreturnService, private fb: FormBuilder,
    private _CustomerService: CustomerService, private _paymentservice: PaymentService, private _router: Router,
    private _masterService: MasterService, private toastr: ToastrService, private _OrdersService: OrdersService,
    private Service: CreditReceiptService, private _appConfigService: AppConfigService) {
    this.salesReturnDate = formatDate(this.today, 'yyyy-MM-dd', 'en-US', '+0530');
    this.radioItems = ['Payment', 'Adjust', 'Order'];
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
  }

  paymentPaidAmt: any;

  ngOnInit() {
    this._paymentservice.castData.subscribe(
      response => {
        this.paymentDetails = response;
        if (this.isEmptyObject(this.paymentDetails) == false && this.isEmptyObject(this.paymentDetails) != null) {
          this.leavePage = true;
          this.paymentPaidAmt = 0;
          this.getSalesReturnData.lstOfPayment.forEach((d) => {
            this.paymentPaidAmt += parseInt(d.PayAmount);
          });
          if (this.getSalesReturnData.lstOfPayment.length == 0) {
            this.leavePage = false;
          }
          this.getSalesReturnData.TotalSRAmount = (this.TotalSRAmount - this.paymentPaidAmt);
          this.PayableAmount = Math.round(this.grandTotal + this.TaxAmount) - <number>this.VPAmount - <number>this.paymentPaidAmt;
          return this.paymentPaidAmt;
        }
      }
    )
    this.ConfirmSRHeaderForm = this.fb.group({
      salesReturnDate: null,
      BilledBranch: null,
      SalCode: null,
      VotureBillNo: null,
      Remarks: null,
      BillDate: null,
      BillNo: null,
    });
    this.getSalesMan();
    this.getBilledBranch();
    this.GetCustomerDetsFromCustComp();
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

  confirmSRHeader: any = {
    EstNo: null,
    TotalSRAmount: null,
    Remarks: null,
    BillNo: null,
    lstOfSalesReturnDetails: [],
    lstOfPayment: []
  }

  ItemAmount: number = 0.00;

  enablePayment: boolean = false;
  OnRadioBtnChnge(arg) {
    if (arg == "Payment") {
      this.paymentPaidAmt = 0;
      this.enablePayment = true;
      this.getSalesReturnData.IsOrder = false;
      this._SalesreturnService.getSalesReturnDetails(this.SrEstNo).subscribe(
        response => {
          this.getSalesReturnData = response;
          if (this.getSalesReturnData != null) {
            this.EnableConfirmSR = true;
            this.EnableSRSummary = true;
            this.confirmSRHeader.EstNo = this.getSalesReturnData.EstNo;
            this.confirmSRHeader.TotalSRAmount = Math.round(this.getSalesReturnData.TotalSRAmount);

            this.confirmSRHeader.BilledBranch = this.getSalesReturnData.BilledBranch;
            this.confirmSRHeader.SalCode = this.getSalesReturnData.SalCode;
            this.confirmSRHeader.BillDate = this.getSalesReturnData.BillDate;
            this.confirmSRHeader.BillNo = this.getSalesReturnData.VotureBillNo;
            this.Billdate = this.confirmSRHeader.BillDate;
            this.BillNo = this.getSalesReturnData.VotureBillNo;

            ///Summary Calculation
            this.TotalSRAmount = Math.round(this.getSalesReturnData.TotalSRAmount);
            this.VPAmount = this.getSalesReturnData.VPAmount;
            this.TaxAmount = this.getSalesReturnData.TaxAmount;
            this.DiscountAmount = this.getSalesReturnData.DiscountAmount;

            const ItemDetails = this.TotalSRAmount - this.TaxAmount;

            this.ItemAmount = this.TotalSRAmount - this.TaxAmount;

            this.grandTotal = <number>ItemDetails - <number>this.DiscountAmount;

            this.TotSRAmt = Math.round(this.grandTotal + this.TaxAmount);
            this.BalanceAmount();

            //this._paymentservice.inputData()

            if (this.getSalesReturnData.IsCreditBill == true) {
              this.model.option = "Payment";
              this.enablePayment = true;
            }


            this.fieldArray = this.getSalesReturnData.lstOfSalesReturnDetails;
            if (this.getSalesReturnData != null) {
              this._CustomerService.getCustomerDtls(this.getSalesReturnData.CustomerID).subscribe(
                data => {
                  const customerDtls = data;
                  this._CustomerService.sendCustomerDtls_To_Customer_Component(customerDtls);
                  this._SalesreturnService.sendItemDatatoItemComp(this.getSalesReturnData);
                  this._paymentservice.inputData(this.getSalesReturnData);
                }
              )
            }
          }
        }
      )
    }
    else if (arg == "Adjust") {
      this.enablePayment = false;
      this.getSalesReturnData.IsOrder = false;
    }
    else {
      this.enablePayment = false;
      this.getSalesReturnData.IsOrder = true;
    }
    this.model.option = arg;
  }

  SalesManList: any;
  getSalesMan() {
    this._masterService.getSalesMan().subscribe(
      response => {
        this.SalesManList = response;
      })
  }

  BilledBranch: any;
  getBilledBranch() {
    this._SalesreturnService.getBilledBranch().subscribe(
      response => {
        this.BilledBranch = response;
      }
    )
  }

  getSalesReturnData: any;
  Billdate: any;

  TotalSRAmount: number;
  VPAmount: 0.00;
  TaxAmount: 0.00;
  DiscountAmount: 0.00;
  grandTotal: any;
  SrEstNo: number = 0;

  GetsalesRetEstNo(arg) {
    if (arg == '') {
      swal("Warning!", 'Please enter sales est number', "warning");
    }
    else {
      this.SrEstNo = arg;
      this.ClearValues();
      this._SalesreturnService.getSalesReturnDetails(arg).subscribe(
        response => {
          this.getSalesReturnData = response;
          if (this.getSalesReturnData != null) {
            this.EnableConfirmSR = true;
            this.EnableSRSummary = true;
            this.confirmSRHeader.EstNo = this.getSalesReturnData.EstNo;
            this.confirmSRHeader.TotalSRAmount = Math.round(this.getSalesReturnData.TotalSRAmount);

            this.confirmSRHeader.BilledBranch = this.getSalesReturnData.BilledBranch;
            this.confirmSRHeader.SalCode = this.getSalesReturnData.SalCode;
            this.confirmSRHeader.BillDate = this.getSalesReturnData.BillDate;
            this.confirmSRHeader.BillNo = this.getSalesReturnData.VotureBillNo;
            this.Billdate = this.confirmSRHeader.BillDate;
            this.BillNo = this.getSalesReturnData.VotureBillNo;

            ///Summary Calculation
            this.TotalSRAmount = Math.round(this.getSalesReturnData.TotalSRAmount);
            this.VPAmount = this.getSalesReturnData.VPAmount;
            this.TaxAmount = this.getSalesReturnData.TaxAmount;
            this.DiscountAmount = this.getSalesReturnData.DiscountAmount;

            const ItemDetails = this.TotalSRAmount - this.TaxAmount;

            this.ItemAmount = this.TotalSRAmount - this.TaxAmount;


            this.grandTotal = <number>ItemDetails - <number>this.DiscountAmount;

            this.TotSRAmt = Math.round(this.grandTotal + this.TaxAmount);
            this.BalanceAmount();


            // this.PayableAmount = <number>this.TotalSRAmount - <number>this.VPAmount;

            this.PayableAmount = Math.round(this.grandTotal + this.TaxAmount) - <number>this.VPAmount;

            if (this.getSalesReturnData.IsCreditBill == true && this.PayableAmount < 0) {
              this.model.option = "Adjust";
              this.enablePayment = false;
              this.getSalesReturnData.IsPayable = true;
              this.getSalesReturnData.IsAdjust = true;
            }

            //this._paymentservice.inputData()


            if (this.getSalesReturnData.IsCreditBill == true && this.PayableAmount > 0) {
              this.model.option = "Payment";
              this.enablePayment = true;
            }


            this.fieldArray = this.getSalesReturnData.lstOfSalesReturnDetails;
            if (this.getSalesReturnData != null) {
              this._CustomerService.getCustomerDtls(this.getSalesReturnData.CustomerID).subscribe(
                data => {
                  const customerDtls = data;
                  this._CustomerService.sendCustomerDtls_To_Customer_Component(customerDtls);
                  this._SalesreturnService.sendItemDatatoItemComp(this.getSalesReturnData);
                  this._paymentservice.inputData(this.getSalesReturnData);
                }
              )
            }
          }
        }
      )
    }
  }

  CustomerName: any;
  //Hide Show data when accordian collapsed(Header)
  Customer: any;
  EnableCustomerTab: boolean = true;
  ToggleCustomer: boolean = false;

  EnableItemsTab: boolean = true;
  NoRecordsItems: boolean = true;
  ToggleItem: boolean = false;

  TogglePayments: boolean = false;
  EnablePaymentsTab: boolean = false;

  public CollapseCustomerTab: boolean = true;

  NoRecordsCustomer: boolean = false;
  filterRadioBtns: boolean = false;

  EnableReprintSR: boolean = false;


  isEmptyObject(obj) {
    return (obj && (Object.keys(obj).length === 0));
  }

  //Hide Show data when accordian collapsed(Customer)
  //Data visible when collapse
  ToggleCustomerData() {
    this.CollapseCustomerTab = !this.CollapseCustomerTab;
    this.CollapseCustomerDetailsTab = !this.CollapseCustomerDetailsTab;
  }


  GetCustomerDetsFromCustComp() {
    this._CustomerService.cast.subscribe(
      response => {
        this.CustomerName = response;
        if (this.isEmptyObject(this.CustomerName) == false && this.isEmptyObject(this.CustomerName) != null) {
          this.CollapseCustomerTab = true;
          this.NoRecordsCustomer = true;
          this.CollapseCustomerDetailsTab = true;
        }
        else {
          this.CollapseCustomerTab = false;
          this.NoRecordsCustomer = false;
        }
      });
  }



  ToggleItemData() {
    this.EnableItemsTab = !this.EnableItemsTab;
  }

  TogglePaymentData() {
    this.EnablePaymentsTab = !this.EnablePaymentsTab;
  }

  ngOnDestroy() {
    this._CustomerService.SendCustDataToEstComp(null);
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
    this._paymentservice.outputData(null);
    this._paymentservice.inputData(null);
    this._paymentservice.OutputParentJSONFunction(null);
    this._paymentservice.SendPaymentSummaryData(null);
    this._SalesreturnService.SendSRBillNoToReprintComp(null);
    this._SalesreturnService.SendSRNoToReprintComp(null);
    this.ClearValues();
  }

  ConfirmSRData: any = {
    "SalesBillNo": 0,
    "OrderNo": 0,
    "ReceiptNo": 0,
    "CreditReceiptNo": 0
  }


  ClearValues() {
    this.PaidAmount = 0.00;
    this.BalAmount = 0.00;
    this.TotSRAmt = 0.00;
    this.Grosswt = 0.000;
    this.AddedWt = 0.000;
    this.Ntwt = 0.000;
    this.TotAmount = 0.000;
    this.Quantity = 0;
    this.DeductedWt = 0.000;
    this.TotalSRAmount = 0.00;
    this.VPAmount = 0.00;
    this.TaxAmount = 0.00;
    this.DiscountAmount = 0.00;
    this.grandTotal = 0.00;
    this.fieldArray = [];
  }

  EnableReprintOrder: boolean = false;
  OrderNo: string = "";
  OrderReceiptheading: string = "Order Receipt";
  ReprintType: string = "Original";
  Orderheading: string = "Order Form";
  orderNo: any = [];


  submitPost() {
    let estNo;

    if ((this.model.option == "Payment") && (this.BalAmount > 0)) {
      swal("Warning!", 'Please enter full amount', "warning");
    }
    else if ((this.model.option == "Payment") && (this.BalAmount < 0)) {
      swal("Warning!", 'Amount is greater than balance', "warning");
    }
    else {
      var ans = confirm("Do you want to save??");
      if (ans) {
        if (this.model.option == "Order") {
          this._SalesreturnService.ConfirmSR(this.getSalesReturnData).subscribe(
            response => {
              this.orderNo = response;
              this.routerUrl = this._router.url;
              if (this.orderNo.ReceiptNo != 0) {
                swal("Saved!", "New Order No: " + this.orderNo.OrderNo + " and Receipt No: " + this.orderNo.ReceiptNo + " generated successfully!", "success");
              }
              else {
                swal("Saved!", "New Order No: " + this.orderNo.OrderNo + " generated successfully!", "success");
              }
              this.salesReturnEstNo.nativeElement.value = "";
              var ans = confirm("Do You want to take order Print Out??");
              if (ans) {
                this.EnableReprintOrder = true,
                  this.EnableConfirmSR = false;
                this.EnableReprintSR = false,
                  this.OrderNo = this.orderNo.OrderNo
                //this._OrdersService.SendOrderNoToReprintComp(this.OrderNo);
                //this._OrdersService.SendOrderNoToReprintComp(this.OrderNo);
                this.orderPrint(this.orderNo);
              }
              else {
                this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(() =>
                  this._router.navigate(['/salesreturn/ConfirmSalesReturn'])
                );
              }
            }
          )
        }
        else {
          this.confirmSRHeader.Remarks = this.getSalesReturnData.Remarks;
          this._SalesreturnService.ConfirmSR(this.getSalesReturnData).subscribe(
            response => {
              estNo = response;
              this.routerUrl = this._router.url;
              swal("Saved!", "New Sales Return number " + estNo.SalesBillNo + " Confirmed Successfully", "success");
              this.salesReturnEstNo.nativeElement.value = "";
              var confirmPrint = confirm("Do you want to take Sales Return Bill No Print Out??");
              if (confirmPrint) {
                this.EnableReprintSR = true,
                  this.ConfirmSRData = {
                    "SalesBillNo": estNo.SalesBillNo,
                    "OrderNo": 0,
                    "ReceiptNo": 0,
                    "CreditReceiptNo": estNo.CreditReceiptNo
                  }
                this._SalesreturnService.SendSRBillNoToReprintComp(this.ConfirmSRData);
                this.EnableConfirmSR = false;
                this.EnableReprintOrder = false;
              }
              else {
                this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(() =>
                  this._router.navigate(['/salesreturn/ConfirmSalesReturn'])
                );
              }
            }
          )
        }
        this.leavePage = false;
      }
    }
  }

  clearComponents() {
    this._CustomerService.SendCustDataToEstComp(null);
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
    this._paymentservice.outputData(null);
    this._paymentservice.inputData(null);
    this._paymentservice.OutputParentJSONFunction(null);
    this._paymentservice.SendPaymentSummaryData(null);
    this.ClearValues();
    this.fieldArray = [];
  }


  orderPrint(arg) {
    this.model.option == 'Order No';
    this.getOrderDetails(arg.OrderNo);
    // if (arg.ReceiptNo != 0 && arg.ReceiptNo != null) {
    //   this.model.option == 'Receipt No';
    //   this.getReceiptDetails(arg.ReceiptNo);
    // }
  }


  orderDetails: any = [];
  Linesarray: any = [];
  Paymentarray: any = [];
  ShowroomList: any = [];
  OrderNum: number;
  PrintTypeBasedOnConfig: any;

  // To display the Order Form Details
  getOrderDetails(arg) {
    // this._OrdersService.getPrintOrder(arg).subscribe(
    //   response => {
    //     this.orderDetails = response;
    //     this.OrderNum = arg;
    //     this.Linesarray = this.orderDetails.lstOfOrderItemDetailsVM;
    //     this.Paymentarray = this.orderDetails.lstOfPayment;
    //     this.getShowroomDetails();
    //     this.getWeightTotal(this.orderDetails.OrderNo);
    //     this.getTotal(this.orderDetails.OrderNo);
    //     $('#ReprintTypeModal').modal('hide');
    //     $('#OrderTab').modal('show');
    //   }
    // );

    this._OrdersService.getOrderPrint(arg, this.ReprintType.toUpperCase()).subscribe(
      response => {
        this.PrintTypeBasedOnConfig = response;
        if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
          $('#OrderTab').modal('show');
          this.orderDetails = atob(this.PrintTypeBasedOnConfig.Data);
          $('#DisplayOrderData').html(this.orderDetails);
        }
        else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
          $('#OrderTab').modal('show');
          this.orderDetails = this.PrintTypeBasedOnConfig.Data;
        }
      }
    )
  }

  ReceiptList: any = [];
  ReceiptListdisplay: any = [];
  CustomerID: any;
  ReceiptNum: number;

  // To display the Order Receipt Details
  // getReceiptDetails(arg) {
  //   if (!arg) {
  //     swal("Warning!", 'Please select Receipt number', "warning");
  //     $('#ReceiptTab').modal('hide');
  //   }
  //   this._OrdersService.getPrintReceipt(arg).subscribe(
  //     response => {
  //       this.ReceiptList = response;
  //       this.ReceiptListdisplay = this.ReceiptList[0];
  //       this.CustomerID = this.ReceiptList[0].SeriesNo;
  //       this.ReceiptNum = arg;
  //       //this.getOrderDetails(this.CustomerID);
  //       this._OrdersService.getPrintOrder(this.CustomerID).subscribe(
  //         response => {
  //           this.orderDetails = response;
  //           this.Linesarray = this.orderDetails.lstOfOrderItemDetailsVM;
  //           this.Paymentarray = this.orderDetails.lstOfPayment;
  //           this.getShowroomDetails();
  //           this.getWeightTotal(this.orderDetails.OrderNo);
  //           this.getTotal(this.orderDetails.OrderNo);
  //         }
  //       );
  //       this.getReceiptTotal(arg);
  //       $('#ReceiptTab').modal('show');
  //     }
  //   );
  // }

  // To display the Showroom Details
  getShowroomDetails() {
    this._OrdersService.getShowroom().subscribe(
      response => {
        this.ShowroomList = response;
      }
    );
  }

  WeightTotalList: any = [];
  getWeightTotal(arg) {
    this._OrdersService.getWeightTotal(arg).subscribe(
      response => {
        this.WeightTotalList = response;
      }
    )
  }

  TotalList: any = [];
  getTotal(arg) {
    this._OrdersService.getTotal(arg).subscribe(
      response => {
        this.TotalList = response;
      },
      (err) => {
        if (err.status === 404) {
          this.TotalList = null;
          // const validationError = err.error;
          // if (validationError.description != "No payment details for the selected order.") {

          // }
        }
      }
    )
  }

  ReceiptTotal: any = []
  getReceiptTotal(arg) {
    this._OrdersService.getReceiptTotal(arg).subscribe(
      response => {
        this.ReceiptTotal = response;
      }
    )
  }

  PaidAmount: number = 0.00;
  BalAmount: number = 0.00;
  TotSRAmt: number = 0.00;

  PaidTotalAmountCalculation(Amount) {
    this.PaidAmount = Amount;
    this.TotSRAmt = Math.round(Number(this.grandTotal) + Number(this.TaxAmount));
    this.BalanceAmount();
  }

  BalanceAmount() {
    this.BalAmount = Math.round(Number(this.grandTotal) + Number(this.TaxAmount)) - Number(this.PaidAmount);
    this.autoFetchAmount = this.BalAmount;
    //this.BalAmount = Number(this.TotSRAmt) - Number(this.VPAmount) - Number(this.PaidAmount);
    return this.BalAmount;
  }

  Grosswt: number = 0.000;
  AddedWt: number = 0.000;
  Ntwt: number = 0.000;
  TotAmount: number = 0.000;
  Quantity: number = 0;
  DeductedWt: number = 0.000;
  PayableAmount: number = 0.00;

  GrossWtTotal(arg) { let total: any = 0.00; arg.forEach((d) => { total += parseFloat(d.GrossWt) }); this.Grosswt = total; return total; }

  AddWt(arg) { let total: any = 0.00; arg.forEach((d) => { total += parseFloat(d.AddWt); }); this.AddedWt = total; return total; }

  DedWt(arg) { let total: any = 0.00; arg.forEach((d) => { total += parseFloat(d.DeductWt); }); this.DeductedWt = total; return total; }

  NetWt(arg) { let total: any = 0.00; arg.forEach((d) => { total += parseFloat(d.NetWt); }); this.Ntwt = total; return total; }

  Qty(arg) {
    let total: any = 0;
    arg.forEach((d) => { total += parseFloat(d.Quantity); }); this.Quantity = total; return total;
  }

  Amount(arg) {
    let total: any = 0.00;
    arg.forEach((d) => { total += parseFloat(d.ItemFinalAmount); }); this.TotAmount = total; return total;
  }

  // for printing the form

  printOrder() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.orderDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printOrderContents, popupWin;
      printOrderContents = document.getElementById('DisplayOrderData').innerHTML;
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
    ${printOrderContents}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }

  printOrderReceipt() {
    let printContents, popupWin;
    printContents = document.getElementById('print-receipt-section').innerHTML;
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

  printClosedOrder() {
    let printContents, popupWin;
    printContents = document.getElementById('print-closed-section').innerHTML;
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


  // Added for Plain Text printing related to Order

  closeOrderTab() {
    this.getOrderPlainText(this.OrderNum);
  }

  orderPlainTextDetails: any = [];

  getOrderPlainText(arg) {
    this._OrdersService.getOrderPrintHTML(arg).subscribe(
      response => {
        this.orderPlainTextDetails = response;
        $('#OrderPlainTextTab').modal('show');
      }
    )
  }


  printOrderPlainText() {
    this._masterService.printPlainText(this.orderPlainTextDetails);
  }

  // Ends Here


  // Added for Plain Text printing related to Order Receipt

  closeOrderReceiptTab() {
    this.getOrderReceiptPlainText(this.ReceiptNum);
  }

  orderReceiptPlainTextDetails: any = [];

  getOrderReceiptPlainText(arg) {
    this._OrdersService.getOrderReceiptPrintHTML(arg).subscribe(
      response => {
        this.orderReceiptPlainTextDetails = response;
        $('#OrderReceiptPlainTextTab').modal('show');
      }
    )
  }

  printOrderReceiptPlainText() {
    this._masterService.printPlainText(this.orderReceiptPlainTextDetails);
  }

  // Ends Here


  //Added for SR and Credit Receipt Print


  closeOrderPlainTextTab() {
    this._SalesreturnService.getSRPrintbyBillNo(this.orderNo.SalesBillNo).subscribe(
      response => {
        this.PrintDetails = response;
        $('#ReprintSRBillModal').modal('show');
        $('#DisplayData').html(this.PrintDetails);
        $('#ReprintSREstModal').modal('hide');
      }
    )
  }

  print() {
    if (this.PrintTypeBasedOnConfig.PrintType == "RAW") {
      this._masterService.printPlainText(this.ReceiptDetails);
    }
    else if (this.PrintTypeBasedOnConfig.PrintType == "HTML") {
      let printCreditReceiptContents, popupWin;
      printCreditReceiptContents = document.getElementById('DisplayCreditReceiptData').innerHTML;
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
        // thead > tr
        // {
        //     border-style: solid;
        //     border: 3px solid rgb(0, 0, 0);
        // }
        table tr td.classname{
          border-right: 3px solid rgb(0, 0, 0) !important;
        }
        table tr td.bottom{
          border-bottom: 3px solid rgb(0, 0, 0) !important;
        }
        table tr td.left{
          border-left: 3px solid rgb(0, 0, 0) !important;
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
    ${printCreditReceiptContents}</body>
      </html>`
      );
      popupWin.document.close();
    }
  }

  ReceiptDetails: any = [];
  PrintDetails: any = [];
  SRBillNo: number;
  SummaryHeader: any = {
    ReceiptNo: null,
    BillNo: null,
    BalanceAmount: null,
    BilledDate: null
  }

  PrintCreditReceipt() {
    if (this._router.url == "/salesreturn/ConfirmSalesReturn") {
      if (this.orderNo.CreditReceiptNo != 0 && this.orderNo.CreditReceiptNo != null) {
        // this.Service.getReceiptValuesForPrint(this.orderNo.CreditReceiptNo).subscribe(
        //   Response => {
        //     this.ReceiptDetails = Response;
        //     this.Linesarray = this.ReceiptDetails.lstOfPayment;
        //     this.getShowroomDetails();
        //     this.getbillDetails(this.ReceiptDetails.BillNo, this.ReceiptDetails.FinYear);
        //     $('#exampleModal').modal('show');
        //     $('#ReprintSRBillModal').modal('hide');
        //     $('#ReprintSREstModal').modal('hide');
        //   }
        // )
        this.Service.getReceiptValuesForPrint(this.orderNo.CreditReceiptNo).subscribe(
          response => {
            this.PrintTypeBasedOnConfig = response;
            if (this.PrintTypeBasedOnConfig.PrintType == "HTML" && this.PrintTypeBasedOnConfig.Data != "") {
              this.ReceiptDetails = atob(this.PrintTypeBasedOnConfig.Data);
              $('#DisplayCreditReceiptData').html(this.ReceiptDetails);
              $('#CreditReceiptModal').modal('show');
            }
            else if (this.PrintTypeBasedOnConfig.PrintType == "RAW" && this.PrintTypeBasedOnConfig.Data != "") {
              this.ReceiptDetails = this.PrintTypeBasedOnConfig.Data;
              $('#CreditReceiptModal').modal('show');
            }
          }
        );
      }
      else {
        $('#OrderTab').modal('hide');
      }
    }
  }

  BillDetails: any;
  getbillDetails(arg1, arg2) {
    this.Service.getValues(arg1, arg2).subscribe(
      Response => {
        this.BillDetails = Response;
        this.SummaryHeader.BillNo = this.BillDetails.BillNo;
        this.SummaryHeader.BalanceAmount = this.BillDetails.BalanceAmount;

      }
    )
  }


  closeSRBill() {
    if (this.orderNo.SalesBillNo != 0 && this.orderNo.SalesBillNo != null) {
      this._SalesreturnService.getSRPrintDotMatrixbyBillNo(this.orderNo.SalesBillNo).subscribe(
        response => {
          this.PrintDetails = response;
          $('#ReprintSRBillModal').modal('hide');
          $('#ReprintSREstModal').modal('show');
        }
      )
    }
  }


  printSREst() {
    this._masterService.printPlainText(this.PrintDetails);
  }

  printSRBill() {
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
          table tr td.borderLeft{
            border-left: 3px solid rgb(0, 0, 0) !important;
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
}