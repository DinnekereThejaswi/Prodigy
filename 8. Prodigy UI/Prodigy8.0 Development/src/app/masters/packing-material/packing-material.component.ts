import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MastersService } from '../masters.service';
import { PackingMaterial } from '../masters.model';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';

@Component({
  selector: 'app-packing-material',
  templateUrl: './packing-material.component.html',
  styleUrls: ['./packing-material.component.css']
})
export class PackingMaterialComponent implements OnInit {
  PackingForm: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  isReadOnly: boolean = false;
  EnableJson: boolean = false;

  constructor(private fb: FormBuilder,
    private _router: Router, private _mastersService: MastersService,
    private _appConfigService: AppConfigService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }
  UOM = [{ "code": "H", "name": "HSN" },
  { "code": "S", "name": "SAC" },
  ]
  PackingMaterialModel: PackingMaterial = {
    ObjID: "",
    CompanyCode: null,
    BranchCode: null,
    PCode: "",
    PName: "",
    MLength: null,
    MLengthUOM: null,
    MHeight: null,
    MHeightUOM: null,
    MWidth: null,
    MWidthUOM: null,
    Color: null,
    MWeight: null,
    MWeightUOM: null,
    Remarks: "",
    ObjStatus: null

  }
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  ngOnInit() {
    this.getPackingListData();
    this.getHLW();
    this.getWeightUOM();
    this.PackingForm = this.fb.group({
      frmctrl_PCode: null,
      frmctrl_PName: null,
      frmCtrl_MLength: null,
      frmCtrl_MLength_UOM: null,
      frmCtrl_Height: null,
      frmCtrl_Height_UOM: null,
      frmCtrl_Weight: null,
      frmCtrl_Weight_UOM: null,
      frmCtrl_Width: null,
      frmCtrl_Width_UOM: null,
      frmctrl_Color: null,
      frmCtrl_Remark: ""
    });

  }
  packingMaterial: any = [];
  getPackingListData() {
    this._mastersService.getPackingData().subscribe(
      response => {
        this.packingMaterial = response;
      })
  }
  HLWuom: any = [];
  getHLW() {
    this._mastersService.getHLWuom().subscribe(
      response => {
        this.HLWuom = response;
      })
  }
  weightUOM: any = [];
  getWeightUOM() {
    this._mastersService.getWeightuom().subscribe(
      response => {
        this.weightUOM = response;
      })
  }
  error: any = [];
  add(form) {
    if (form.value.frmctrl_PCode == null || form.value.frmctrl_PName == "") {
      swal("Warning", "Please enter code", "warning");
    }
    else if (form.value.frmctrl_PName == null || form.value.frmctrl_PName == "") {
      swal("Warning", "Please enter Name", "warning");
    }
    else if (form.value.frmctrl_Color == null || form.value.frmctrl_Color == "") {
      swal("Warning!", "Please enter color", "warning");
    }
    else if (form.value.frmCtrl_Remark == null || form.value.frmCtrl_Remark == "") {
      swal("Warning!", "Please enter remarks", "warning");
    }
    else if (form.value.frmCtrl_MLength == null || form.value.frmCtrl_MLength == "") {
      swal("Warning", "Please enter Length", "warning");
    }
    else if (form.value.frmCtrl_MLength_UOM == null || form.value.frmCtrl_MLength_UOM == "") {
      swal("Warning", "Please enter Length Measure", "warning");
    }
    else if (form.value.frmCtrl_Height == null || form.value.frmCtrl_Height == "") {
      swal("Warning", "Please enter Height", "warning");
    }
    else if (form.value.frmCtrl_Height_UOM == null || form.value.frmCtrl_Height_UOM == "") {
      swal("Warning", "Please enter Height Measure", "warning");
    }

    else if (form.value.frmCtrl_Width == null || form.value.frmCtrl_Width == "") {
      swal("Warning", "Please enter Width", "warning");
    }
    else if (form.value.frmCtrl_Width_UOM == null || form.value.frmCtrl_Width_UOM == "") {
      swal("Warning", "Please enter Width Measure", "warning");
    }
    else if (form.value.frmCtrl_Weight == null || form.value.frmCtrl_Weight == "") {
      swal("Warning", "Please enter Weight", "warning");
    }
    else if (form.value.frmCtrl_Weight_UOM == null || form.value.frmCtrl_Weight_UOM == "") {
      swal("Warning", "Please enter Weight Measure", "warning");
    }

    else {
      this.PackingMaterialModel.CompanyCode = this.ccode;
      this.PackingMaterialModel.BranchCode = this.bcode;
      this.PackingMaterialModel.ObjStatus = "O";
      var ans = confirm("Do you want to Add??" + this.PackingMaterialModel.PName);
      if (ans) {
        this._mastersService.PostPacking(this.PackingMaterialModel).subscribe(
          response => {
            this.error = response;
            swal("Saved!", "Saved " + this.PackingMaterialModel.PName + " Saved", "success");
            this.PackingForm.reset();
            this.getPackingListData();

          },
          (err) => {
            if (err.status === 400) {
              const validationError = err.error.description;
              swal("Warning!", validationError, "warning");
            }
            else {
              this.error.push('something went wrong!');
            }
            this.clear();
          }
        )
      }
    }
  }
  clear() {
    this.isReadOnly = false;
    this.getPackingListData();
    this.PackingForm.reset();
  }
  editField(arg) {
    if (arg.ObjStatus == "C") {
      swal("Warning!", "Packing: " + arg.PName + " is Closed. Open to edit", "warning");
    }
    else {

      this.EnableAdd = false;
      this.EnableSave = true;
      this.PackingMaterialModel = arg;
      this.isReadOnly = true;
    }

  }

  openField(arg) {
    if (arg.ObjStatus == "O") {
      swal("Warning!", "Packing: " + arg.PName + " is already Open", "warning");
    }
    else {
      var ans = confirm("Do you want to Open Packing: " + arg.PName + "?");
      if (ans) {
        arg.ObjStatus = "O";
        this._mastersService.putPacking(arg.ObjID, arg).subscribe(
          response => {
            swal("Updated!", "Packing: " + arg.PName + " Open", "success");

            this.clear();
          }
        )
      }
    }
  }


  closeField(arg) {
    if (arg.ObjStatus == "C") {
      swal("Warning!", "Packing: " + arg.PName + " is already closed", "warning");
    }
    else {
      var ans = confirm("Do you want to close the Packing: " + arg.PName + "?");
      if (ans) {
        arg.ObjStatus = "C";
        this._mastersService.putPacking(arg.ObjID, arg).subscribe(
          response => {
            swal("Updated!", "Packing: " + arg.PName + " close", "success");
            this.clear();
          }
        )
      }
    }
  }
  save(form) {
    if (form.value.frmctrl_PCode == null || form.value.frmctrl_PName == "") {
      swal("Warning!", "Please enter code", "warning");
    }
    else if (form.value.frmctrl_PName == null || form.value.frmctrl_PName == "") {
      swal("Warning!", "Please enter Name", "warning");
    }
    else if (form.value.frmctrl_Color == null || form.value.frmctrl_Color == "") {
      swal("Warning!", "Please enter color", "warning");
    }
    else if (form.value.frmCtrl_Remark == null || form.value.frmCtrl_Remark == "") {
      swal("Warning!", "Please enter remarks", "warning");
    }
    else if (form.value.frmCtrl_MLength == null || form.value.frmCtrl_MLength == "") {
      swal("Warning!", "Please enter Length", "warning");
    }
    else if (form.value.frmCtrl_MLength_UOM == null || form.value.frmCtrl_MLength_UOM == "") {
      swal("Warning!", "Please enter Length Measure", "warning");
    }
    else if (form.value.frmCtrl_Height == null || form.value.frmCtrl_Height == "") {
      swal("Warning!", "Please enter Height", "warning");
    }
    else if (form.value.frmCtrl_Height_UOM == null || form.value.frmCtrl_Height_UOM == "") {
      swal("Warning!", "Please enter Height Measure", "warning");
    }

    else if (form.value.frmCtrl_Width == null || form.value.frmCtrl_Width == "") {
      swal("Warning!", "Please enter Width", "warning");
    }
    else if (form.value.frmCtrl_Width_UOM == null || form.value.frmCtrl_Width_UOM == "") {
      swal("Warning!", "Please enter Width Measure", "warning");
    }

    else if (form.value.frmCtrl_Weight == null || form.value.frmCtrl_Weight == "") {
      swal("Warning!", "Please enter Weight", "warning");
    } else if (form.value.frmCtrl_Weight_UOM == null || form.value.frmCtrl_Weight_UOM == "") {
      swal("Warning!", "Please enter Weight Measure", "warning");
    }
    else {
      var ans = confirm("Do you want to save??");
      if (ans) {
        this._mastersService.putPacking(this.PackingMaterialModel.ObjID, this.PackingMaterialModel).subscribe(
          response => {
            swal("Updated!", "Saved " + this.PackingMaterialModel.PName + " Saved", "success");
            this.clear();

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
