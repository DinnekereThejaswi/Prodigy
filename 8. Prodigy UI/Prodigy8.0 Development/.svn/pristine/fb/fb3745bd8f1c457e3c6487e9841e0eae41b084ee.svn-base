import { Component, OnInit } from '@angular/core';
import { HsnMaster } from '../hsn-master.model';
import { FormGroup, FormBuilder } from '@angular/forms';
import { HsnMasterService } from '../hsn-master.service';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-manage-hsn-master',
  templateUrl: './manage-hsn-master.component.html',
  styleUrls: ['./manage-hsn-master.component.css']
})
export class ManageHsnMasterComponent implements OnInit {
  HSNMasterForm: FormGroup;
  code: number;
  successfulSave: boolean;
  errors: string[];
  hasErrors = false;
  constructor(private _router: Router, private fb: FormBuilder,
    private _hsnmaster: HsnMasterService, private toastr: ToastrService, private _avRoute: ActivatedRoute)
     { 
      if (this._avRoute.snapshot.params['code']) {
        this.code = parseInt(this._avRoute.snapshot.params['code']);
        alert(this.code);
      }
     }
  HSNMasterListData: HsnMaster = {
    Code: null,
    GSTGroupCode: null,
    Description: null,
    Type: null,
    // LastModifiedOn: null,
    IsActive: null,
    // LastModifiedBy: null
  };

  ngOnInit() {
 
    

  

}


}

