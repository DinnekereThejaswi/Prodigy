import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { MastersService } from './../masters.service'

@Component({
  selector: 'app-stone-rate-master',
  templateUrl: './stone-rate-master.component.html',
  styleUrls: ['./stone-rate-master.component.css']
})
export class StoneRateMasterComponent implements OnInit {

  StoneRateForm: FormGroup;
  StoneRateListData = {
    GS: null,
    StoneName: null,
    Caratfrom: null,
    CaratTo: null,
    Amount: null,
    MinAmount: null
  }
  StoneLsit = [{
    Code: "DMD", Name: "DIAMOND"
  },
  {
    Code: "OD", Name: "OLD DIAMOND"
  },
  {
    Code: "OST", Name: "OLD STONE"
  },
  {
    Code: "DL", Name: "LOOSE DIAMOND"
  },
  {
    Code: "STN", Name: "STONES"
  }

  ]
  constructor(private fb: FormBuilder, private _masterservice: MastersService) { }

  ngOnInit() {
    this.getStoneRates();
    this.StoneRateForm = this.fb.group({
      GS: null,
      StoneName: null,
      Caratfrom: null,
      CaratTo: null,
      Amount: null,
      MinAmount: null
    });
  }
  StoneRates: any = [];
  getStoneRates() {
    this._masterservice.getStoneRates().subscribe(
      Response => {
        this.StoneRates = Response;
      }
    )

  }
  onSubmit(form) {
    if (form.value.GS == null || form.value.GS === '') {
      alert('Please select GS');
    }
    else if (form.value.StoneName == null || form.value.StoneName === '') {
      alert('Please select Stone/Diamond Name');
    }
    else if (form.value.Caratfrom == null || form.value.Caratfrom === '') {
      alert('Please enter Carat from');
    }
    else if (form.value.CaratTo == null || form.value.CaratTo === '') {
      alert('Please enter CaratTo');
    }
    else if (form.value.Amount == null || form.value.Amount === '') {
      alert('Please enter Amount');
    }
  }

}
