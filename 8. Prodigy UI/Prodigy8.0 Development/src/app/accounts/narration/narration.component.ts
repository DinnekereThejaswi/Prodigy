import { Component, OnInit } from '@angular/core';
import { AccountsService } from '../accounts.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NarrationVM } from '../accounts.model';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { AppConfigService } from '../../AppConfigService';

@Component({
  selector: 'app-narration',
  templateUrl: './narration.component.html',
  styleUrls: ['./narration.component.css']
})
export class NarrationComponent implements OnInit {
  NarrationList: any = [];
  NarrationForm: FormGroup;
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;
  EnableJson: boolean = false;
  NarrationModel: NarrationVM = {
    ObjID: "",
    CompanyCode: this.ccode,
    BranchCode: this.bcode,
    NarrID: 0,
    Narration: ""
  }


  constructor(private accountsService: AccountsService, private formBuilder: FormBuilder,
    private appConfigService: AppConfigService) {
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.EnableJson = this.appConfigService.EnableJson;
    this.password = this.appConfigService.Pwd;
    this.getCB();
    this.NarrationModel = {
      ObjID: "",
      CompanyCode: this.ccode,
      BranchCode: this.bcode,
      NarrID: 0,
      Narration: ""
    }
  }

  ngOnInit() {
    this.getNarrationList();
    this.NarrationForm = this.formBuilder.group({
      NarrID: ["", Validators.required],
      Narration: [null, Validators.required]
    });
  }

  getNarrationList() {
    this.accountsService.getNarrationList().subscribe(
      response => {
        this.NarrationList = response;
      }
    )
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  add(form) {
    if (form.value.Narration == null || form.value.Narration == "") {
      swal("Warning!", "Please enter narration", "warning");
    }
    else {
      var ans = confirm("Do you want to save??");
      if (ans) {
        this.accountsService.postNarration(this.NarrationModel).subscribe(
          response => {
            swal("Saved!", "Narration saved successfully.", "success");
            this.getNarrationList();
            this.ClearValues();
          }
        )
      }
    }
  }

  ClearValues() {
    this.NarrationForm.reset();
    this.NarrationModel = {
      ObjID: "",
      CompanyCode: "",
      BranchCode: "",
      NarrID: 0,
      Narration: ""
    }
  }

  deleteField(arg) {
    this.NarrationModel = arg;
    var ans = confirm("Do you want to delete??");
    if (ans) {
      this.accountsService.deleteNarration(this.NarrationModel).subscribe(
        response => {
          swal("Deleted!", "Narration deleted successfully.", "success");
          this.getNarrationList();
          this.ClearValues();
        }
      )
    }
  }

  clear() {
    this.NarrationForm.reset();
  }
}
