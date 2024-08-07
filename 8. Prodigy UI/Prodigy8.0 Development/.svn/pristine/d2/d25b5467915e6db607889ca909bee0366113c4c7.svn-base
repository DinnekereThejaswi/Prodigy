import { Component, OnInit } from '@angular/core';
import {FormGroup,FormBuilder} from '@angular/forms';
@Component({
  selector: 'app-rol-new',
  templateUrl: './rol-new.component.html',
  styleUrls: ['./rol-new.component.css']
})
export class RolNewComponent implements OnInit {

  ROLNewForm: FormGroup;
  ROLNewListData = {
    GS: null,
    CounterCode: null,
    Item: null,
    Design: null,
    ItemSize: null,
    FromWt: null,
    ToWt: null,
    MinQty: null,
    MediumQty: null,
    MaxQty: null
  }
  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.ROLNewForm = this.fb.group({
      GS: null,
      CounterCode: null,
      Item: null,
      Design: null,
      ItemSize: null,
      FromWt: null,
      ToWt: null,
      MinQty: null,
      MediumQty: null,
      MaxQty: null
    });
  }
  onSubmit(form) {
    if (form.value.GS == null || form.value.GS === '') {
      alert('Please select GS');
    }
    else if(form.value.CounterCode == null || form.value.CounterCode === '') {
      alert('Please select Counter Code');
    }
    else if(form.value.Item == null || form.value.Item === '') {
      alert('Please select Item');
    }
    else if(form.value.Design == null || form.value.Design === '') {
      alert('Please select Design');
    }
    else if(form.value.ItemSize == null || form.value.ItemSize === '') {
      alert('Please select Item Size');
    }
  }

}
