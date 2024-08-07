import { Component, OnInit } from '@angular/core';
import { SalesreturnService } from '../salesreturn.service';
import { Router } from '@angular/router';
import swal from 'sweetalert';
import { ComponentCanDeactivate } from '../../appconfirmation-guard';

@Component({
  selector: 'app-delete-salesreturn',
  templateUrl: './delete-salesreturn.component.html',
  styleUrls: ['./delete-salesreturn.component.css']
})
export class DeleteSalesreturnComponent implements OnInit, ComponentCanDeactivate {

  SREstList: any = [];
  leavePage: boolean = false;
  EnableReprintSR: boolean = false;

  constructor(private _salesreturnService: SalesreturnService, private _router: Router) { }

  ngOnInit() {
    this._salesreturnService.getSREstNoList().subscribe(
      response => {
        this.SREstList = response;
        this.leavePage = false;
      }
    )
  }

  DeactivateGuard() {
    this.leavePage = true;
  }

  onPreview(arg) {
    if (arg == 0) {
      swal("Warning!", "Please select SR Est No", "warning");
    }
    else {
      this.EnableReprintSR = true;
      this._salesreturnService.SendSRNoToReprintComp(arg);
    }
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

  deleteSR(arg) {
    if (arg == 0) {
      swal("Warning!", "Please select SR Est No", "warning");
    }
    else {
      var ans = confirm("Do you want to Cancel SR Est: " + arg);
      if (ans) {
        this.EnableReprintSR = false;
        this._salesreturnService.deleteSREstNo(arg).subscribe(
          response => {
            swal("Cancelled!", "SR estimation: " + arg + " deleted.", "success");
            this.leavePage = false;
            this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(() =>
              this._router.navigate(['/salesreturn/delete-salesreturn']),
            );
          }
        )
      }
    }
  }
}