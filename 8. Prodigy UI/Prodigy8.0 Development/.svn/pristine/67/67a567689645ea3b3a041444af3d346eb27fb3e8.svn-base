import { CustomerService } from '../../masters/customer/customer.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { OrdersService } from '../orders.service';
import { PaymentService } from '../../payment/payment.service';
import { Router } from '@angular/router'
import swal from 'sweetalert';

@Component({
  selector: 'app-vieworders',
  templateUrl: './vieworders.component.html',
  styleUrls: ['./vieworders.component.css']
})
export class ViewordersComponent implements OnInit, OnDestroy {
  radioItems: Array<string>;
  OrderNo: string = "";
  model = { option: 'Existing Order' };
  //to customer Details to Customer Component
  Customer: any = [];
  PaymentDetails: any = [];
  PaymentSummary: any = [];
  EnableCustomerDetails: boolean = false;
  EnablePaymentDetails: boolean = false;
  //EnableItemDetails:boolean=false;
  EnableItemsPaymentsTab: boolean = true;
  NoRecordsPayments: boolean = false;
  EnablePaymentsTab: boolean = true;
  NoRecordsItemsPayments: boolean = false;
  NoRecordsPaymentSummary: boolean = false;
  NoRecordsItemSummary: boolean = false;
  EnableCustomerTab: boolean = true;
  filterRadioBtns: boolean = true;

  toggleData: boolean = false;

  StatusColor: boolean;

  ngOnInit() {

  }
  constructor(private _CustomerService: CustomerService, private _OrdersService: OrdersService,
    private _router: Router, private _paymentService: PaymentService) {
  }

  //Check object is empty
  isEmptyObject(obj) {
    return (obj && (Object.keys(obj).length === 0));
  }

  ToggleCustomer: boolean = false;

  ToggleCustomerData() {
    // if (this.ToggleCustomer == true) {
    //   this.EnableCustomerTab = false;
    // }
    // else {
    //   //swal("Warning", "Data not available", "warning");
    // }
    this.EnableCustomerTab = !this.EnableCustomerTab;
  }

  ToggleItemsPaymentData() {
    // this._OrdersService.castexistingOrderDetails.subscribe(
    //   response => {
    //     this.itemDetails = response;
    //     if (this.isEmptyObject(this.itemDetails) == false && this.isEmptyObject(this.itemDetails) != null) {
    //       this.NoRecordsItemSummary = true;
    //       this.EnableItemsPaymentsTab = false;
    //       this.NoRecordsItemsPayments = true;
    //     }
    //     else {
    //       this.NoRecordsItemsPayments = false;
    //       this.NoRecordsItemSummary = false;
    //       this.EnableItemsPaymentsTab = true;
    //       //swal("Warning", "Data not available", "warning");
    //     }
    //   }
    // );
    this.EnableItemsPaymentsTab = !this.EnableItemsPaymentsTab;
  }

  TogglePaymentData() {
    // this._paymentService.CastPaymentSummaryData.subscribe(
    //   response => {
    //     this.PaymentSummary = response;
    //     if (this.isEmptyObject(this.PaymentSummary) == false && this.isEmptyObject(this.PaymentSummary) != null) {
    //       this.NoRecordsPaymentSummary = true;
    //       this.EnablePaymentsTab = false;
    //       this.NoRecordsPayments = true;
    //     }
    //     else {
    //       this.NoRecordsPaymentSummary = false;
    //       this.NoRecordsPayments = false;
    //       this.EnablePaymentsTab = true;
    //       //swal("Warning", "Data not available", "warning");          
    //     }
    //   }
    // );
    this.EnablePaymentsTab = !this.EnablePaymentsTab;
  }

  status: string;
  itemDetails: any = []
  getOrderDetails(arg) {
    if (arg == '') {
      swal("Warning!", 'Please Enter Order Number', "warning");
    }
    else {
      //  this._OrdersService.viewOrder(arg).subscribe(
      this._OrdersService.viewOrder(arg).subscribe(
        response => {
          this.itemDetails = response;
          // console.log(this.itemDetails)
          if (this.itemDetails.CFlag == 'Y') {
            this.status = "Cancelled";
            this.StatusColor = false;
          }
          else if (this.itemDetails.CFlag == 'N' && this.itemDetails.ClosedFlag == 'N') {
            this.status = "Open";
            this.StatusColor = true;
          }
          else if (this.itemDetails.ClosedFlag == 'Y') {
            this.status = "Closed";
            this.StatusColor = false;
          }


          this.toggleData = true;
          if (this.itemDetails != null) {
            this.EnableCustomerDetails = true;
            this._CustomerService.getCustomerDtls(this.itemDetails.CustID).subscribe(
              response => {
                this.Customer = response;
                this._paymentService.inputData(null);
                this._OrdersService.SendExistingOrderDetailsToSubComp(this.itemDetails);
                this._OrdersService.SendOrderNoToComp(arg);
                this._paymentService.inputData(this.itemDetails);
                this._CustomerService.sendCustomerDtls_To_Customer_Component(this.Customer);
                this._CustomerService.SendCustDataToEstComp(this.Customer);
                this.PaymentDetails = this.itemDetails;
                this.EnablePaymentDetails = true;
                this.NoRecordsPaymentSummary = true;
                this.EnableCustomerTab = true;
                this.ToggleCustomer = true;
                this.NoRecordsItemSummary = true;
                this.NoRecordsItemsPayments = true;
                this.NoRecordsPayments = true;
                this.EnableCustomerTab = true;
                this.EnableItemsPaymentsTab = true;
                this.EnablePaymentsTab = true;
                this._paymentService.CastPaymentSummaryData.subscribe(
                  response => {
                    const validatePayment = response;
                    if (validatePayment != null) {
                      this.PaymentSummary = validatePayment;
                    }
                  }
                );
              }

            )
          }
        },
        (err) => {
          if (err.status === 404) {
            this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
              () => {
                this._router.navigate(['orders/vieworders']);
              })
          }
          if (err.status === 400) {
            this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
              () => {
                this._router.navigate(['orders/vieworders']);
              })
          }
        }

      )
    }
  }
  ngOnDestroy() {
    this._OrdersService.SendOrderNoToComp(null);
    this._paymentService.OutputParentJSONFunction(null);
    this._paymentService.inputData(null);
    this._paymentService.SendPaymentSummaryData(null);
    this._OrdersService.SendExistingOrderDetailsToSubComp(null);
    this._CustomerService.sendCustomerDtls_To_Customer_Component(null);
    this._CustomerService.SendCustDataToEstComp(null);
  }

  cancel() {
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(
      () => {
        this._router.navigate(['/orders/vieworders']);
      }
    )
  }
}