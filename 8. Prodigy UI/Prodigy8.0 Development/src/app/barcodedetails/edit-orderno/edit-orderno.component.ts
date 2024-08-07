import { Component, OnInit } from '@angular/core';
import { BarcodedetailsService } from '../barcodedetails.service';
import { BarcodeDetails } from '../barcodedetails.model';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';

@Component({
  selector: 'app-edit-orderno',
  templateUrl: './edit-orderno.component.html',
  styleUrls: ['./edit-orderno.component.css']
})
export class EditOrdernoComponent implements OnInit {

  ccode: string;
  bcode: string;
  password: string;
  EnableJson: boolean = false;

  constructor(private _barcodedetailsService: BarcodedetailsService,
    private _appConfigService: AppConfigService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }

  ngOnInit() {
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }


  barcodeDetails: BarcodeDetails = {
    BarcodeNo: null,
    CounterCode: null,
    GSCode: null,
    GrossWt: null,
    ItemCode: null,
    NetWt: null,
    OrderNo: null,
    Qty: null,
    StoneWt: null,
  }

  BarcodeDetails() {
    this._barcodedetailsService.getBarcodeDetails(this.barcodeDetails.BarcodeNo).subscribe((response: BarcodeDetails) => {
      this.barcodeDetails = response;
    },
      (err) => {
        this.cancel();
      }
    )
  }

  outputData: any;

  save() {
    if (this.barcodeDetails.BarcodeNo == null || this.barcodeDetails.BarcodeNo == "") {
      swal("Warning!", 'Please enter the Barcode No', "warning");
    }
    else if (this.barcodeDetails.OrderNo == null) {
      swal("Warning!", 'Please enter the Order No', "warning");
    }
    else {
      var ans = confirm("Do you want to save??");
      if (ans) {
        this._barcodedetailsService.updateOrderNoToBarcodeDetail(this.barcodeDetails).subscribe(
          response => {
            this.outputData = response;
            swal("Saved!", this.outputData.Message, "success");
            this.cancel();
          }
        )
      }
    }
  }

  cancel() {
    this.barcodeDetails = {
      BarcodeNo: null,
      CounterCode: null,
      GSCode: null,
      GrossWt: null,
      ItemCode: null,
      NetWt: null,
      OrderNo: null,
      Qty: null,
      StoneWt: null,
    }
  }
}