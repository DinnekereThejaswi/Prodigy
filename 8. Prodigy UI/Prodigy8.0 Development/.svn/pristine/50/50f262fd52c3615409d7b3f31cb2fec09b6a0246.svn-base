import { Component, OnInit } from '@angular/core';
import { FormGroup,FormBuilder } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ReligionMaster } from '../religion-master.model';
import { ReligionMasterService } from '../religion-master.service';
import { ToastrService } from 'ngx-toastr';
@Component({
  selector: 'app-manage-religion-master',
  templateUrl: './manage-religion-master.component.html', 
  styleUrls: ['./manage-religion-master.component.css']
})
export class ManageReligionMasterComponent implements OnInit {
  ReligionMasterForm: FormGroup;
  id: number = 0;
  successfulSave: boolean;
  errors: string[];
  hasErrors = false;
  ReligionMasterListData: ReligionMaster = {
    ID:0,
    Religion: null,
    DisplaySequence: null
  }
  constructor(private fb: FormBuilder, private _router: Router, 
    private _ReligionMasterService: ReligionMasterService,
    private toastr: ToastrService) { }

  ngOnInit() {
    this.ReligionMasterForm = this.fb.group({
      ID:this.id,
      Religion: null,
      DisplaySequence: null
    });
  }
  onSubmit(form) {
    if (form.value.Religion == null || form.value.Religion === '') {
      alert('Please enter Religion Name');
    }
    else if (form.value.DisplaySequence == null || form.value.DisplaySequence === '') {
      alert('Please enter Display Sequence');
    }
    {
      this.errors = [];
    this._ReligionMasterService.post(form.value)
        .subscribe(
          suc => {
            this.successfulSave = true;
            this.toastr.success('New Record Added Succcessfully', 'Success!');
            this._router.navigate(['/religion-master']);
          },
          (err) => {
            this.successfulSave = false;
          }
        )
    }
  }

}
