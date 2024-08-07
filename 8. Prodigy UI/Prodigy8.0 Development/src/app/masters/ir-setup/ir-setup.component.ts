import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MastersService } from '../masters.service';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { ToastrService } from 'ngx-toastr';
import { IRmodel } from '../masters.model'

@Component({
  selector: 'app-ir-setup',
  templateUrl: './ir-setup.component.html',
  styleUrls: ['./ir-setup.component.css']
})
export class IrSetupComponent implements OnInit {
  irMasterForm: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  isReadOnly: boolean = false;
  EnableJson: boolean = false;
  irListDatamodel: IRmodel = {
    ObjID: "",
    CompanyCode: "",
    BranchCode: "",
    VoucherCode: null,
    IRCode: null,
    IRName: null,
    ACCode: 0,
    ObjStatus: "O",
    UpdateOn: "",
    UniqRowID: ""
  }


  constructor(private fb: FormBuilder, private _appConfigService: AppConfigService,
    private _mastersService: MastersService,
    private toastr: ToastrService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this.irListDatamodel.CompanyCode = this.ccode;
    this.irListDatamodel.BranchCode = this.bcode;
    this.getIRTYPES();
    // this.getIRlist();
    this.irMasterForm = this.fb.group({
      frmCtrl_Type: null,
      frmCtrl_Code: null,
      frmCtrl_Name: null,
    });

  }
  IRTYPES: any = []
  getIRTYPES() {
    this._mastersService.loadIRCodes().subscribe(
      Response => {
        this.IRTYPES = Response;
      }
    )
  }
  IRListData: any = []
  getIRlist(arg) {
    this._mastersService.loadIRListData(arg.target.value).subscribe(
      Response => {
        this.IRListData = Response;
      }
    )
  }
  editField(arg) {
    this.EnableAdd = false;
    this.EnableSave = true;
    this.irListDatamodel = arg;
    this.isReadOnly = true;

  }
  errors: any = [];
  addIRdetails(form) {
    if (form.value.frmCtrl_Type == null || form.value.frmCtrl_Type == "") {
      swal("Warning!", "Please select Type", "warning");
    }
    else if (form.value.frmCtrl_Code == null || form.value.frmCtrl_Code == "") {
      swal("Warning!", "Please enter code", "warning");
    }
    else if (form.value.frmCtrl_Name == null || form.value.frmCtrl_Name == "") {
      swal("Warning!", "Please enter name", "warning");
    }
    else {
      var ans = confirm("Do you want to Add ??" + form.value.frmCtrl_Name);
      if (ans) {
        this._mastersService.PostIRListData(this.irListDatamodel).subscribe(
          response => {
            this.errors = response;
            swal("Saved!", "Saved " + this.irListDatamodel.IRName + " Saved", "success");
            this.getIRlist(this.irListDatamodel.VoucherCode);
          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error;
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
  save(form) {
    if (form.value.frmCtrl_Type == null || form.value.frmCtrl_Type == "") {
      swal("Warning!", "Please enter Type", "warning");
    }
    else if (form.value.frmCtrl_Name == null || form.value.frmCtrl_Name == "") {
      swal("Warning!", "Please enter Name", "warning");
    }
    else {
      this.irListDatamodel.CompanyCode = this.ccode;
      this.irListDatamodel.BranchCode = this.bcode;
      var ans = confirm("Do you want to save ??" + this.irListDatamodel.IRName);
      if (ans) {
        this._mastersService.putIRListData(this.irListDatamodel.ObjID, this.irListDatamodel).subscribe(
          response => {
            swal("Updated!", "Saved " + this.irListDatamodel.IRName + " Saved", "success");
            this.irMasterForm.reset();
          }
        )
      }
    }
  }

  Delete(arg) {
    this.irListDatamodel = arg;

    var ans = confirm("Do you want to Delete ??" + arg.IRName);
    if (ans) {
      this.irListDatamodel.CompanyCode = this.ccode;
      this.irListDatamodel.BranchCode = this.bcode;
      this.irListDatamodel.ObjStatus = "C";
      this._mastersService.putIRListData(this.irListDatamodel.ObjID, this.irListDatamodel).subscribe(
        response => {
          swal("Success!", this.irListDatamodel.IRName + "Deleted Succesfully", "success");
          this.getIRlist(this.irListDatamodel.VoucherCode);
        }
      )
    }

  }
  clear(arg) {
    arg.reset();
    this.irMasterForm.reset();
    this.isReadOnly = false;
    this.EnableAdd = true;
    this.EnableSave = false;
    this.getIRlist(this.irListDatamodel.VoucherCode);

  }
}
