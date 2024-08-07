import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { MachineRegistrationService } from '../machine-registration.service';
import { MachineRegistration } from '../machine-registration.model';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
@Component({
  selector: 'app-manage-machine-registration',
  templateUrl: './manage-machine-registration.component.html',
  styleUrls: ['./manage-machine-registration.component.css']
})
export class ManageMachineRegistrationComponent implements OnInit {
  MachineForm: FormGroup;
  successfulSave: boolean;
  errors: string[];
  hasErrors = false;
  MachineListData: MachineRegistration = {
    MachineName: null,
    BillCounter: null
  }
  constructor(private fb: FormBuilder,
    private _MachineRegistrationService: MachineRegistrationService,
    private _router: Router,
    private toastr: ToastrService) { }

  ngOnInit() {
    this.getMachine();
    this.getBillCounter();
    this.MachineForm = this.fb.group({
      MachineName: null,
      BillCounter: null
    });

  }
  MachineList: any;
  getMachine() {
    this._MachineRegistrationService.getMachineDropdown().subscribe(
      Response => {
        this.MachineList = Response;
      }
    );
  }
  BillCounterList: any;
  getBillCounter() {
    this._MachineRegistrationService.getBillCounter().subscribe(
      Response => {
        this.BillCounterList = Response;

      }
    )
  }
  onSubmit(form) {
    if (form.value.MachineName == null || form.value.MachineName === '') {
      alert("Please select Machine Name");
    }
    else if (form.value.BillCounter == null || form.value.BillCounter === '') {
      alert("Please select Bill Counter");
    }
    else {
      this.errors = [];
      this._MachineRegistrationService.post(form.value)
        .subscribe(
          suc => {
            this.successfulSave = true;
            this.toastr.success('New Record Added Succcessfully', 'Success!');
            this._router.navigate(['/machine-registration']);
          },
          (err) => {
            this.successfulSave = false;
          }
        )
    }
  }
}
