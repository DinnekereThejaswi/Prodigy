import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ReligionMasterService } from './religion-master.service';
@Component({
  selector: 'app-religion-master',
  templateUrl: './religion-master.component.html',
  styleUrls: ['./religion-master.component.css']
})
export class ReligionMasterComponent implements OnInit {

  constructor(private _router: Router, private _religionService: ReligionMasterService) { }

  ngOnInit() {
    this.getList();
  }
  ReligionList: any;
  getList() {
    this._religionService.getReligionMaster().subscribe(

      Response => {
        this.ReligionList = Response;
      }

    );
  }
  addReligionRecord() {
    this._router.navigate(['religion-master/add']);
  }
  editReligionRecord(id){
    this._router.navigate(['religion-master/edit', id]);
  }
}
