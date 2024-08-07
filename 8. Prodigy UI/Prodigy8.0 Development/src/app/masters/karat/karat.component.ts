import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import { KaratModel } from '../masters.model';
import { MastersService } from '../masters.service';
import swal from 'sweetalert';

@Component({
  selector: 'app-karat',
  templateUrl: './karat.component.html',
  styleUrls: ['./karat.component.css']
})
export class KaratComponent implements OnInit {
  KaratForm: FormGroup;
  ccode: string;
  bcode: string;
  KaratList: any = [];
  password: string;
  EnableJson: boolean = false;
  karatListData: KaratModel = {
    BranchCode: "",
    CompanyCode: "",
    Karat: "",
    ObjID: "",
    ObjStatus: "",
  }
  constructor(private _appConfigService: AppConfigService, private fb: FormBuilder, private _masterService: MastersService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8
    );
  }

  ngOnInit() {
    this.getKaratData();
    this.KaratForm = this.fb.group({
      frmCtrl_Karat: ["", Validators.required],
    });
  }
  getKaratData() {
    this._masterService.GETKaratsToTable().subscribe(
      Response => {
        this.KaratList = Response;
      })
  }
  errors: any = [];
  onSubmit(form) {
    var name: string;
    name = form.value.frmCtrl_Karat;

    if (form.value.frmCtrl_Karat == null || form.value.frmCtrl_Karat === '') {
      swal("Warning!", "Please enter Karat", "warning");
    }
    else {
      var ans = confirm("Do you want to save??");
      if (ans) {
        this.karatListData.CompanyCode = this.ccode;
        this.karatListData.BranchCode = this.bcode;
        this.karatListData.ObjStatus = "O";
        this._masterService.PostKarat(this.karatListData).subscribe(
          response => {
            swal("!Success", "Karat: " + name + "Saved", "success");
            this.getKaratData();
            this.KaratForm.reset();
          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error.description;
              swal("Warning!", validationError, "warning");
            }
            else {
              this.errors.push('something went wrong!');
            }

          }
        )
      
      }
    }
  }
  Clear() {
    this.KaratForm.reset();
    this.getKaratData();
  }
  openField(arg) {
    if (arg.ObjStatus == "O") {
      swal("Warning!", "Karat: " + arg.Karat + " is already opened", "warning");
    }
    else {
      var ans = confirm("Do you want to open Karat: " + arg.Karat + "?");
      if (ans) {
        arg.ObjStatus = "O";
        this._masterService.KaratStatus(arg, arg.ObjID).subscribe(
          response => {
            swal("Updated!", "Updated", "success");
            this.getKaratData();
          }
        )
      }
    }
  }
  closeField(arg) {
    if (arg.ObjStatus == "C") {
      swal("Warning!", "Karat: " + arg.Karat + " is already closed", "warning");
    }
    else {
      var ans = confirm("Do you want to close the Karat: " + arg.Karat + "?");
      if (ans) {
        arg.ObjStatus = "C";
        this._masterService.KaratStatus(arg, arg.ObjID).subscribe(
          response => {
            swal("Closed!", "Karat: " + arg.Karat + " closed", "success");
            this.getKaratData();
          }
        )
      }
    }
  }
  getStatusColor(ObjStatus) {
    switch (ObjStatus) {
      case 'O':
        return 'green';
      case 'C':
        return 'red';

    }
  }

}
