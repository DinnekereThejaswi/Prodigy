import { Component, OnInit } from '@angular/core';
import swal from 'sweetalert';
import { StocksService } from '../stocks.service';
import { MasterService } from '../../core/common/master.service';
declare var $: any;

@Component({
  selector: 'app-reprint-stock-taking',
  templateUrl: './reprint-stock-taking.component.html',
  styleUrls: ['./reprint-stock-taking.component.css']
})
export class ReprintStockTakingComponent implements OnInit {

  constructor(private _stocksService: StocksService,
    private _masterService: MasterService) {
    this.radioItems = ['ST. Summary', 'ST. Details'];
  }

  model = { option: 'ST. Summary' };
  radioItems: Array<string>;
  BatchNo: any = [];
  isChecked: boolean = false;

  ngOnInit() {
    this.getBatchNo();
  }

  getBatchNo() {
    this._stocksService.getBatchNo().subscribe(
      response => {
        this.BatchNo = response;
      }
    )
  }

  Changed(arg) {
    if (arg === 'ST. Summary') {
      this.model.option = arg;
    }
    else if (arg === 'ST. Details') {
      this.model.option = arg;
    }
  }

  printData: any;

  getStockTakingReprint(arg) {
    if (arg === "null") {
      swal("Warning!", 'Please select Batch No', "warning");
      $('#BatchSummary').modal('hide');
    }
    else {
      if (this.model.option == "ST. Summary") {
        this._stocksService.getStockTakingDetailBySummary(arg).subscribe(
          response => {
            this.printData = response;
            $('#BatchSummary').modal('show');
          }
        )
      }
      else {
        this._stocksService.getStockTakingDetail(arg).subscribe(
          response => {
            this.printData = response;
            $('#BatchSummary').modal('show');
          }
        )
      }
    }
  }

  printBatchSummary() {
    this._masterService.printPlainText(this.printData);
  }
}