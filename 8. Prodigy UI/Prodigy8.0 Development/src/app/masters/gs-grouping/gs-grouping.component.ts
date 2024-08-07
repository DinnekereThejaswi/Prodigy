import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { GsGrouping } from './gs-grouping.model';
import { GsGroupingService } from './gs-grouping.service';
import { Router, ActivatedRoute } from '@angular/router';
import swal from 'sweetalert';
@Component({
  selector: 'app-gs-grouping',
  templateUrl: './gs-grouping.component.html',
  styleUrls: ['./gs-grouping.component.css']
})
export class GsGroupingComponent implements OnInit {
  GSGroupForm: FormGroup;
  successfulSave: boolean;
  errors: string[];
  hasErrors = false;
  GSGroupListData: GsGrouping = {
    ObjID: null,
    IRCode: null,
    GSCode: null
  }
  constructor(private fb: FormBuilder,
    private _GsGroupingService: GsGroupingService,
    private _router: Router
  ) { }

  ngOnInit() {
    this.getGSList();
    this.getIRType();
    this.GSGroupForm = this.fb.group({
      IRCode: null,
      GSCode: null
    });
  }

  // main table list
  GroupingList: any;
  Mainlist(val) {
    this._GsGroupingService.getGsGroupingList(val).subscribe(
      Response => {
        this.GroupingList = Response;
      }
    );
  }

  // GSGroupList for dropdown
  GSGroupList: any;
  getGSList() {
    this._GsGroupingService.getGSList().subscribe(
      Response => {
        this.GSGroupList = Response;
      }
    )
  }
  // IRType dropdown
  IRTypeList: any;
  getIRType() {
    this._GsGroupingService.getIRType().subscribe(
      Response => {
        this.IRTypeList = Response;
      }
    )
  }
  // Deleting the recode
  deleteGSGroupRecord(ObjID) {
    var ans = confirm('Do you want to delete');
    if (ans) {
      this._GsGroupingService.deleteGSGroup(ObjID)
        .subscribe(data => {
          this.Mainlist(this.GSGroupForm.value.IRCode);
        }
        );
    }
  }
  editGSGroupRecord(ObjID) {
    this._GsGroupingService.editRecord(ObjID)
      .subscribe((data: GsGrouping) => {
        this.GSGroupListData = data;
      });
    // this._GsGroupingService.updateRecord(this.GSGroupForm.value)
    //   .subscribe(
    //     suc => {
    //       this.successfulSave = true;
    //       this.toastr.success('Record Updated Succcessfully', 'Success!');
    //       this.Mainlist(this.GSGroupForm.value.IRCode);
    //     },
    //     (err) => {
    //       this.successfulSave = false;
    //     }
    //   )
  }
  // on submitting the form (validation and post)
  onSubmit(form) {
    if (form.value.IRCode === '' || form.value.IRCode == null) {
      swal("Warning!", "Please select IRType Code", "warning");
    }
    else
      if (form.value.GSCode === '' || form.value.GSCode == null) {
        swal("Warning!", "Please select GS", "warning");
      }
      else {
        this.errors = [];
        this._GsGroupingService.post(form.value)
          .subscribe(
            suc => {
              this.successfulSave = true;

              swal("Success!", "New Record Added Succcessfully");
              // this._router.navigate(['/gs-grouping']);
              this.Mainlist(this.GSGroupForm.value.IRCode);
            },
            (err) => {
              this.successfulSave = false;
            }
          );
      }
  }

}
