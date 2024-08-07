import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { estimationService } from './../estimation.service';
import { ToastrService } from 'ngx-toastr';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import { MasterService } from '../../core/common/master.service';
import swal from 'sweetalert';


declare var $: any;

@Component({
  selector: 'app-mergeestimation',
  templateUrl: './mergeestimation.component.html',
  styleUrls: ['./mergeestimation.component.css']
})
export class MergeestimationComponent implements OnInit {

  ccode: string = "";
  bcode: string = "";
  MergeEstPerCode: string;
  password: string;
  EnableJson: boolean = false;
  // @ViewChild("PwdMergeEst") PwdMergeEst: ElementRef;

  @ViewChild("PwdMergeEst", { static: true }) PwdMergeEst: ElementRef;

  constructor(private _estmationService: estimationService, private toastr: ToastrService,
    private _appConfigService: AppConfigService, private _masterService: MasterService) {
    this.EnableJson = this._appConfigService.EnableJson;
    this.password = this._appConfigService.Pwd;
    this.getCB();
    this.MergeEstPerCode = this._appConfigService.RateEditCode.MergeEstimationPermission;
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }


  MergeEstPost: any;

  ngOnInit() {
    this.MergeEstPost = {
      CompanyCode: this.ccode,
      BranchCode: this.bcode,
      EstDet: []
    }
  }

  ngAfterContentChecked() {
    this.PwdMergeEst.nativeElement.focus();
  }

  AllEstDetails: any = [];

  getAllEstimation() {
    this._estmationService.getAllEstimation().subscribe(
      response => {
        this.AllEstDetails = response;
        this._estmationService.SendMergeEstNoToEstComp(null);
      }
    )
  }

  checkedList: any = [];

  public Index: number = -1;
  public EstIndex: number;
  public index1: number;

  onCheckboxChange(option, event, index) {
    this.Index = index === this.Index ? -1 : index;
    if (event.target.checked) {
      this.MergeEstPost.EstDet.push(option);
      this.index1 = this.MergeEstPost.EstDet.findIndex(x => x.EstNo === option.EstNo);
    }
    else {
      this.index1 = this.MergeEstPost.EstDet.findIndex(x => x.EstNo === option.EstNo);
      var ans = confirm("Do you want to remove an estimation");
      if (ans) {
        event.target.checked = false;
        this.MergeEstPost.EstDet.splice(this.index1, 1);
      }
      else {
        event.target.checked = true;
      }
    }
  }

  onCheckboxSubItemsChange(option, event1, SubItemindex) {
    if (event1.target.checked) {
      this.MergeEstPost.EstDet[this.index1].salesEstimatonVM.push(option);
    }
    else {
      event1.target.checked = false;
      this.MergeEstPost.EstDet[this.index1].salesEstimatonVM.splice(SubItemindex, 1);
    }
  }

  MergeEst() {
    $("#PermissonModal").modal('show');
    $("#MergeEstModal").modal('hide');
    this.PwdMergeEst.nativeElement.value = "";
  }

  MergeEstNo: any;

  permissonModel: any = {
    CompanyCode: null,
    BranchCode: null,
    PermissionID: null,
    PermissionData: null
  }

  passWord(arg) {
    if (arg == "") {
      swal("Warning!", 'Please Enter the Password', "warning");
      $('#PermissonModal').modal('show');
      $("#MergeEstModal").modal('hide');
    }
    else {
      this.permissonModel.CompanyCode = this.ccode;
      this.permissonModel.BranchCode = this.bcode;
      this.permissonModel.PermissionID = this.MergeEstPerCode;
      this.permissonModel.PermissionData = btoa(arg);
      this._masterService.postelevatedpermission(this.permissonModel).subscribe(
        response => {
          $('#PermissonModal').modal('hide');
          this.checkedList = [];
          $("#MergeEstModal").modal('show');
          this.getAllEstimation();
          this.Index = -1;
          this.MergeEstPost = {
            CompanyCode: this.ccode,
            BranchCode: this.bcode,
            EstDet: []
          }
        },
        (err) => {
          if (err.status === 401) {
            $('#PermissonModal').modal('show');
            $("#MergeEstModal").modal('hide');
          }
        }
      )
    }
  }

  Save() {
    // if (this.checkedList.length == 0) {
    //   this.toastr.warning('Please select the estimation', 'Alert!');
    // }
    // else {
    this._estmationService.PostMergeEstimation(this.MergeEstPost).subscribe(
      response => {
        this.MergeEstNo = response;
        this._estmationService.SendMergeEstNoToEstComp(this.MergeEstNo);
        $("#MergeEstModal").modal('hide');
        swal("Saved!", "Estimation number " + this.MergeEstNo.NewEstimationNo + " Created", "success");
      },
      (err) => {
        this._estmationService.SendMergeEstNoToEstComp(null);
        $("#MergeEstModal").modal('show');
        //this.getAllEstimation();
      }
    )
    // }
  }
}