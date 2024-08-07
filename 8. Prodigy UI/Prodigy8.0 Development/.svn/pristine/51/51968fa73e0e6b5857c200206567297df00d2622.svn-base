import { Component, OnInit } from '@angular/core';
import {Router} from '@angular/router';
import { ItemSizeService} from './item-size.service';

@Component({
  selector: 'app-item-size',
  templateUrl: './item-size.component.html',
  styleUrls: ['./item-size.component.css']
})
export class ItemSizeComponent implements OnInit {

  constructor(private _router: Router, private _ItemSizeService: ItemSizeService) { }

  ngOnInit() {
    // this.getItemList();
  }
  addItemSizeRecord() {
    this._router.navigate(['item-size/add']);
  }
  ItemSizeList: any;
  // getItemList(){
  //   this._ItemSizeService.getItemList().subscribe(
  //     Response => {
  //       this.ItemSizeList = Response;
  //     }
  //   )

  // }

}
