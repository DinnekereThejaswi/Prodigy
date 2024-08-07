import { Component, OnInit } from '@angular/core';
import { CustomerService } from '../../customer/customer.service';
@Component({
  selector: 'app-view-customer',
  templateUrl: './view-customer.component.html',
  styleUrls: ['./view-customer.component.css']
})
export class ViewCustomerComponent implements OnInit {

  constructor(private _CustomerService: CustomerService) { }

  ngOnInit() {
    this.getCustomerDetails();
  }
  CustomerListData: any = [];
  getCustomerDetails(){
    this._CustomerService.getCustomerDtls(274154).subscribe(
      Response => {
        this.CustomerListData = Response;
      }
    )
    }
  }

