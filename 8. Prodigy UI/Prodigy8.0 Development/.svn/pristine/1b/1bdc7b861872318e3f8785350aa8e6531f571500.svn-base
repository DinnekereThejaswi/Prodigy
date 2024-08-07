import { Component, OnInit } from '@angular/core';
import { MasterService } from '../../core/common/master.service';
import { Form, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
declare var $: any;

@Component({
  selector: 'app-role-permission',
  templateUrl: './role-permission.component.html',
  styleUrls: ['./role-permission.component.css']
})

export class RolePermissionComponent implements OnInit {

  constructor(private _masterService: MasterService, private formBuilder: FormBuilder, private appConfigService: AppConfigService) {
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.password = this.appConfigService.Pwd;
    
    this.getCB();
  }

  PermissionList: any;
  RolesList: any;
  RolesPermissionList: any;
  RolePermissionForm: FormGroup;
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;

  rolePermissionModel: any = {
    CompanyCode: null,
    BranchCode: null,
    PermissionID: null,
    PermissionName: null,
    RoleID: null,
    RoleName: null,
    Active: null
  }




  ngOnInit() {

    this.RolePermissionForm = this.formBuilder.group({
      CompanyCode: ["", Validators.required],
      BranchCode: ["", Validators.required],
      PermissionID: ["", Validators.required],
      PermissionName: ["", Validators.required],
      RoleID: ["", Validators.required],
      RoleName: ["", Validators.required],
      Active: ["", Validators.required]
    });

    this.getPermission();
    this.getRoles();
    this.getRolesPermissionList();

  }

  OpenModal() {
    $('#Modal').modal('show');
  }

  getPermission() {
    this._masterService.getPermissions().subscribe(
      response => {
        this.PermissionList = response;
        console.log( this.PermissionList);
        
      }
    )
  }


  getRoles() {
    this._masterService.getRoles().subscribe(
      response => {
        this.RolesList = response;
      }
    )
  }

  getRolesPermissionList() {
    this._masterService.getRolesPermissionList().subscribe(
      response => {
        this.RolesPermissionList = response;
        //  console.log(this.RolesPermissionList);
      }
    )
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }


  Activate(index, arg) {
    if (arg.Active == true) {
      swal("Warning!", "Already Activate", "warning");
    }
    else {
      this.rolePermissionModel = arg;
      this.rolePermissionModel.CompanyCode = this.ccode;
      this.rolePermissionModel.BranchCode = this.bcode;
      this.rolePermissionModel.Active = true;
      this._masterService.putActivatePermissions(this.rolePermissionModel).subscribe(
        response => {
          swal("Saved!", "Updated Successfully", "success");
          this.getRolesPermissionList();
          this.RolePermissionForm.reset();
        }
      )
    }
  }

  Deactivate(index, arg) {
    if (arg.Active == false) {
      swal("Warning!", "Already De-Activate", "warning");
    }
    else {
      this.rolePermissionModel = arg;
      this.rolePermissionModel.CompanyCode = this.ccode;
      this.rolePermissionModel.BranchCode = this.bcode;
      this.rolePermissionModel.Active = false;
      this._masterService.putDeactivatePermissions(this.rolePermissionModel).subscribe(
        response => {
          swal("Saved!", "Updated Successfully", "success");
          this.getRolesPermissionList();
          this.RolePermissionForm.reset();
        }
      )
    }
  }

  Add(form) {
    if (form.value.PermissionID == null) {
      alert("Please select the permission");
    }
    else if (form.value.RoleID == null) {
      alert("Please select the role");
    }
    else {
      this.rolePermissionModel.CompanyCode = this.ccode;
      this.rolePermissionModel.BranchCode = this.bcode;
      this.rolePermissionModel.Active = true;
      this._masterService.postPermissions(this.rolePermissionModel).subscribe(
        response => {
          swal("Saved!", "Created Successfully", "success");
          $('#Modal').modal('hide');
          this.getRolesPermissionList();
          this.RolePermissionForm.reset();
        },
        (err) => {

        }
      )
    }
  }
}