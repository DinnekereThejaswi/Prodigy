import { Component, Input, OnInit } from '@angular/core';
import { estimationService } from '../estimation.service';

@Component({
  selector: 'app-mergeestimationbarcodes',
  templateUrl: './mergeestimationbarcodes.component.html',
  styleUrls: ['./mergeestimationbarcodes.component.css']
})
export class MergeestimationbarcodesComponent implements OnInit {

  @Input() BarcodeLinesDetails: any = [];
  fieldArray: any = [];
  Index: number;


  constructor(private estimationservice: estimationService) { }

  ngOnInit() {
    this.estimationservice.castBarcodeDetails.subscribe(
      response => {
        this.fieldArray = response;
      }
    )
  }
}