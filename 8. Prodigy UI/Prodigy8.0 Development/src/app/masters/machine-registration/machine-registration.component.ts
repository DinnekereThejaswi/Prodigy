import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MachineRegistrationService } from './machine-registration.service'


@Component({
  selector: 'app-machine-registration',
  templateUrl: './machine-registration.component.html',
  styleUrls: ['./machine-registration.component.css']
})
export class MachineRegistrationComponent implements OnInit {

  constructor(private _router: Router, private _MachineRegistrationService: MachineRegistrationService) { }

  ngOnInit() {
    this.getMachineLists();
  }
  addMachineRecord() {
    this._router.navigate(['machine-registration/add']);
  }
  MachineList: any;
  getMachineLists() {
    this._MachineRegistrationService.getMachine().subscribe(
      Response => {
        this.MachineList = Response;
      }
    )
  }
  deleteMachineRecord(ObjID) {
    var ans = confirm('Do you want to delete');
    if (ans) {
      this._MachineRegistrationService.deleteMachine(ObjID)
        .subscribe(data => {
          this.getMachineLists();
        }
        );
    }
  }
}
