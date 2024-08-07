import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
declare var $: any;
@Component({
  selector: 'app-errorpages',
  templateUrl: './errorpages.component.html',
  styleUrls: ['./errorpages.component.css']
})
export class ErrorpagesComponent implements OnInit {
  ImagePath: string;
  constructor(private _router: Router) {
    this.ImagePath = '/assets/images/HTTP404.jpg'
  }

  ngOnInit() {
    $("#404Model").modal('show');
  }
  BackTodashBoard() {
    this._router.navigate(['/dashboard']);
  }

}
