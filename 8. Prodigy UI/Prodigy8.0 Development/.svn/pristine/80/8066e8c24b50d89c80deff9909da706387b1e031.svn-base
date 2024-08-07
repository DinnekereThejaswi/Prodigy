import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { UtilitiesService } from '../utilities.service';
import { AppConfigService } from '../../AppConfigService';
import { Router } from '@angular/router';
import { NgxSpinnerService } from "ngx-spinner";

import swal from 'sweetalert';


@Component({
  selector: 'app-role-permission',
  templateUrl: './role-permission.component.html',
  styleUrls: ['./role-permission.component.css']
})
export class RolePermissionComponent implements OnInit {

  password: string;
  EnableJson: boolean = false;
  EnableSave: boolean = false;
  @ViewChild("role", { static: true }) role: ElementRef;

  constructor(private _utilitiesService: UtilitiesService,
    private _appConfigService: AppConfigService,
    private _router: Router, private SpinnerService: NgxSpinnerService) {
    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
  }

  rolesList: any = [];

  ngOnInit() {
    this.getRolesList();
  }

  getRolesList() {
    this._utilitiesService.rolesList().subscribe(
      response => {
        this.rolesList = response;
      }
    )
  }

  RoleAssigmentData: any = [];

  LoadPermission(arg) {
    this._utilitiesService.getRoleAssignment(arg).subscribe(
      response => {
        this.RoleAssigmentData = response;
        this.CheckModulesBasedOnSubModules();
        this.EnableSave = true;
      }
    )
  }

  count: number = 0;

  CheckModulesBasedOnSubModules() {
    for (let i = 0; i < this.RoleAssigmentData.Modules.length; i++) {
      this.count = 0;
      for (let j = 0; j < this.RoleAssigmentData.Modules[i].SubModules.length; j++) {
        if (this.RoleAssigmentData.Modules[i].SubModules[j].Checked == false) {
          ++this.count;
          break;
        }
      }
      if (this.count > 0) {
        this.RoleAssigmentData.Modules[i].Checked = false;
      }
      else {
        this.RoleAssigmentData.Modules[i].Checked = true;
      }
      this.count == 0;
    }
  }

  outputData: any;

  onModuleCheckboxChange(event, moduleIndex) {
    if (event.target.checked) {
      for (let j = 0; j < this.RoleAssigmentData.Modules[moduleIndex].SubModules.length; j++) {
        this.RoleAssigmentData.Modules[moduleIndex].Checked = true;
        this.RoleAssigmentData.Modules[moduleIndex].SubModules[j].Checked = true;
        this.RoleAssigmentData.Modules[moduleIndex].SubModules[j].Functions.AddEnabled = true;
        this.RoleAssigmentData.Modules[moduleIndex].SubModules[j].Functions.EditEnabled = true;
        this.RoleAssigmentData.Modules[moduleIndex].SubModules[j].Functions.DeleteEnabled = true;
        this.RoleAssigmentData.Modules[moduleIndex].SubModules[j].Functions.ViewEnabled = true;
      }
    }
    else {
      for (let j = 0; j < this.RoleAssigmentData.Modules[moduleIndex].SubModules.length; j++) {
        this.RoleAssigmentData.Modules[moduleIndex].Checked = false;
        this.RoleAssigmentData.Modules[moduleIndex].SubModules[j].Checked = false;
        this.RoleAssigmentData.Modules[moduleIndex].SubModules[j].Functions.AddEnabled = false;
        this.RoleAssigmentData.Modules[moduleIndex].SubModules[j].Functions.EditEnabled = false;
        this.RoleAssigmentData.Modules[moduleIndex].SubModules[j].Functions.DeleteEnabled = false;
        this.RoleAssigmentData.Modules[moduleIndex].SubModules[j].Functions.ViewEnabled = false;
      }
    }
  }

  onSubModuleCheckboxChange(event, moduleIndex, submoduleIndex) {
    if (event.target.checked) {
      this.RoleAssigmentData.Modules[moduleIndex].SubModules[submoduleIndex].Checked = true;
      this.RoleAssigmentData.Modules[moduleIndex].SubModules[submoduleIndex].Functions.AddEnabled = true;
      this.RoleAssigmentData.Modules[moduleIndex].SubModules[submoduleIndex].Functions.EditEnabled = true;
      this.RoleAssigmentData.Modules[moduleIndex].SubModules[submoduleIndex].Functions.DeleteEnabled = true;
      this.RoleAssigmentData.Modules[moduleIndex].SubModules[submoduleIndex].Functions.ViewEnabled = true;
      this.CheckModulesBasedOnSubModules();
    }
    else {
      this.RoleAssigmentData.Modules[moduleIndex].SubModules[submoduleIndex].Checked = false;
      this.RoleAssigmentData.Modules[moduleIndex].SubModules[submoduleIndex].Functions.AddEnabled = false;
      this.RoleAssigmentData.Modules[moduleIndex].SubModules[submoduleIndex].Functions.EditEnabled = false;
      this.RoleAssigmentData.Modules[moduleIndex].SubModules[submoduleIndex].Functions.DeleteEnabled = false;
      this.RoleAssigmentData.Modules[moduleIndex].SubModules[submoduleIndex].Functions.ViewEnabled = false;
      this.CheckModulesBasedOnSubModules();
    }
  }

  onAddChange(event, moduleIndex, submoduleIndex) {
    if (event.target.checked) {
      this.RoleAssigmentData.Modules[moduleIndex].SubModules[submoduleIndex].Functions.AddEnabled = true;
    }
    else {
      this.RoleAssigmentData.Modules[moduleIndex].SubModules[submoduleIndex].Functions.AddEnabled = false;
    }
  }

  onEditChange(event, moduleIndex, submoduleIndex) {
    if (event.target.checked) {
      this.RoleAssigmentData.Modules[moduleIndex].SubModules[submoduleIndex].Functions.EditEnabled = true;
    }
    else {
      this.RoleAssigmentData.Modules[moduleIndex].SubModules[submoduleIndex].Functions.EditEnabled = false;
    }
  }

  onDeleteChange(event, moduleIndex, submoduleIndex) {
    if (event.target.checked) {
      this.RoleAssigmentData.Modules[moduleIndex].SubModules[submoduleIndex].Functions.DeleteEnabled = true;
    }
    else {
      this.RoleAssigmentData.Modules[moduleIndex].SubModules[submoduleIndex].Functions.DeleteEnabled = false;
    }
  }

  onViewChange(event, moduleIndex, submoduleIndex) {
    if (event.target.checked) {
      this.RoleAssigmentData.Modules[moduleIndex].SubModules[submoduleIndex].Functions.ViewEnabled = true;
    }
    else {
      this.RoleAssigmentData.Modules[moduleIndex].SubModules[submoduleIndex].Functions.ViewEnabled = false;
    }
  }

  save() {
    var ans = confirm("Do you want to save?");
    if (ans) {
      this.SpinnerService.show();
      this._utilitiesService.postRoleAssignment(this.RoleAssigmentData).subscribe(
        response => {
          this.outputData = response;
          this.SpinnerService.hide();
          swal("Saved!", this.outputData.Message, "success");
          this.EnableSave = false;
          this.getRolesList();
          this.RoleAssigmentData = [];
        },
        (err) => {
          this.SpinnerService.hide();
        }
      )
    }
  }

  cancel() {
    this.EnableSave = false;
    this._router.navigateByUrl('/redirect', { skipLocationChange: true }).then(() =>
      this._router.navigate(['/utilities/role-permission']))
  }
}