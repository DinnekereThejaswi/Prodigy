import { Component, OnInit } from '@angular/core';
import { MasterService } from './../core/common/master.service';
@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  ordDate = '';
  ApplicationDate: string = "";
  DailyRate: any = [];
  Platinum: string = "";
  Gold22k: string = "";
  Gold18k: string = "";
  Gold24k: string = "";
  Silver: string = "";


  constructor(private _masterService: MasterService) { }

  ngOnInit() {
    this.getApplicationdate();
    this.GetDailyRate();
  }

  GetDailyRate() {
    this._masterService.GetDailyRate().subscribe(
      Response => {
        this.DailyRate = Response;
        for (let i = 0; i < this.DailyRate.SellingRates.length; i++) {
          if (this.DailyRate.SellingRates[i].Karat == "22K" && this.DailyRate.SellingRates[i].GsCode == "NGO") {
            this.Gold22k = this.DailyRate.SellingRates[i].SellingBoardRate;
          }
          else if (this.DailyRate.SellingRates[i].Karat == "24K" && this.DailyRate.SellingRates[i].GsCode == "SGO") {
            this.Gold24k = this.DailyRate.SellingRates[i].SellingBoardRate;
          }
          else if (this.DailyRate.SellingRates[i].Karat == "18K" && this.DailyRate.SellingRates[i].GsCode == "NGO") {
            this.Gold18k = this.DailyRate.SellingRates[i].SellingBoardRate;
          }
          else if (this.DailyRate.SellingRates[i].GsCode == "SL") {
            this.Silver = this.DailyRate.SellingRates[i].SellingBoardRate;
          }
        }
      }
    )
  }

  applicationDate: any;


  getApplicationdate() {
    this._masterService.getApplicationDate().subscribe(
      response => {
        let appDate = response;
        this.applicationDate = appDate["applcationDate"];
      }
    )
  }
}