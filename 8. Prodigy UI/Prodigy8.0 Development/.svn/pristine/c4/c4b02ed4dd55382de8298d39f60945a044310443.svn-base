import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { GstMaster } from '../gst-master.model';
import { GstMasterService } from '../gst-master.service';

@Component({
  selector: 'app-manage-gst-master',
  templateUrl: './manage-gst-master.component.html',
  styleUrls: ['./manage-gst-master.component.css']
})
export class ManageGstMasterComponent implements OnInit {
  GSTMasterForm: FormGroup;
  constructor(private fb: FormBuilder,
    private _router: Router,
    private _avRoute: ActivatedRoute,
    private _GstMasterService: GstMasterService) { }
  GSTMasterListData: GstMaster = {
    Code: null,
    GSTGroupType: null,
    Description: null,
    LastModifiedOn: null,
    LastModifiedBy: null,
    IsActive: null,
    SortOrder: null
  };
  ngOnInit() {
    this.getGroupType();
    this.GSTMasterForm = this.fb.group({
      Code: null,
      GSTGroupType: null,
      Description: null,
      LastModifiedOn: null,
      LastModifiedBy: null,
      IsActive: null,
      SortOrder: null
    });
  }
  GroupTypeList: any;
  getGroupType() {
    this._GstMasterService.getGroupTypes().subscribe(
      Response => {
        this.GroupTypeList = Response;
      }
    );
  }
  onSubmit(form) {
    if (form.value.Code == null || form.value.Code === '') {
      alert("Please enter GST Code");
    }
    else if (form.value.GSTGroupType == null || form.value.GSTGroupType === '') {
      alert("Please select Group Type");
    }
    else if (form.value.Description == null || form.value.Description === '') {
      alert("Please enter Description");
    }
  }

}
