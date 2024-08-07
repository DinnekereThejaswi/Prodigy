import { Component, OnInit } from '@angular/core';
import { MasterService } from '../../core/common/master.service';
import { Form, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AppConfigService } from '../../AppConfigService';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
declare var $: any;

@Component({
  selector: 'app-user-permission',
  templateUrl: './user-permission.component.html',
  styleUrls: ['./user-permission.component.css']
})
export class UserPermissionComponent implements OnInit {

  constructor(private _masterService: MasterService, private formBuilder: FormBuilder, private appConfigService: AppConfigService) {
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.password = this.appConfigService.Pwd;
    this.getCB();
  }

  PermissionList: any;
  UsersList: any;
  UsersPermissionList: any;
  UsersPermissionForm: FormGroup;
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;

  usersPermissionModel: any = {
    CompanyCode: null,
    BranchCode: null,
    PermissionID: null,
    PermissionName: null,
    UserCode: null,
    UserName: null,
    Active: null
  }




  ngOnInit() {

    this.UsersPermissionForm = this.formBuilder.group({
      CompanyCode: ["", Validators.required],
      BranchCode: ["", Validators.required],
      PermissionID: ["", Validators.required],
      PermissionName: ["", Validators.required],
      UserCode: ["", Validators.required],
      UserName: ["", Validators.required],
      Active: ["", Validators.required]
    });

    this.getPermission();
    this.getUsers();
    this.getUsersPermissionList();
  }

  OpenModal() {
    $('#Modal').modal('show');
  }

  getPermission() {
    this._masterService.getUserPermissions().subscribe(
      response => {
        this.PermissionList = response;
      }
    )
  }


  getUsers() {
    this._masterService.getusers().subscribe(
      response => {
        this.UsersList = response;
      }
    )
  }

  getUsersPermissionList() {
    this._masterService.getUsersPermissionList().subscribe(
      response => {
        this.UsersPermissionList = response;
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
      this.usersPermissionModel = arg;
      this.usersPermissionModel.CompanyCode = this.ccode;
      this.usersPermissionModel.BranchCode = this.bcode;
      this.usersPermissionModel.Active = true;
      this._masterService.putUsersActivatePermissions(this.usersPermissionModel).subscribe(
        response => {
          swal("Saved!", "Updated Successfully", "success");
          this.getUsersPermissionList();
          this.UsersPermissionForm.reset();
        }
      )
    }
  }

  Deactivate(index, arg) {
    if (arg.Active == false) {
      swal("Warning!", "Already De-Activate", "warning");
    }
    else {
      this.usersPermissionModel = arg;
      this.usersPermissionModel.CompanyCode = this.ccode;
      this.usersPermissionModel.BranchCode = this.bcode;
      this.usersPermissionModel.Active = false;
      this._masterService.putUsersDeactivatePermissions(this.usersPermissionModel).subscribe(
        response => {
          swal("Saved!", "Updated Successfully", "success");
          this.getUsersPermissionList();
          this.UsersPermissionForm.reset();
        }
      )
    }
  }

  Add(form) {
    if (form.value.PermissionID == null) {
      swal("Warning", "Please select the permissione", "warning");
    }
    else if (form.value.UserCode == null) {
      alert("Please select the user");
      swal("Warning", "Please select the permission", "warning");
    }
    else {
      this.usersPermissionModel.CompanyCode = this.ccode;
      this.usersPermissionModel.BranchCode = this.bcode;
      this.usersPermissionModel.Active = true;
      this._masterService.postUsersPermissions(this.usersPermissionModel).subscribe(
        response => {
          swal("Saved!", "Created Successfully", "success");
          $('#Modal').modal('hide');
          this.getUsersPermissionList();
          this.UsersPermissionForm.reset();
        },
        (err) => {

        }
      )
    }
  }
}
