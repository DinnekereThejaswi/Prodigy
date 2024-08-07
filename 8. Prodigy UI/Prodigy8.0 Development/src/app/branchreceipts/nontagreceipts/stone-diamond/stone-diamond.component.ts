import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AddBarcodeService } from '../../../sales/add-barcode/add-barcode.service';
import { BranchreceiptsService } from '../../branchreceipts.service';
import { NTReceiptstoneModel } from '../../branchreceipts.model';
import swal from 'sweetalert';


@Component({
  selector: 'app-stone-diamond',
  templateUrl: './stone-diamond.component.html',
  styleUrls: ['./stone-diamond.component.css']
})
export class StoneDiamondComponent implements OnInit {

  ccode: string = "";
  bcode: string = "";
  ShowHide: boolean = true;
  EnableJson: boolean = false;
  password: string;
  fieldArray: any = [];

  constructor(private barcodeService: AddBarcodeService,
    private _branchreceiptsService: BranchreceiptsService) { }

  ngOnInit() {
    this.getStoneType();
    this._branchreceiptsService.castStoneDiamondDetails.subscribe(
      response => {
        this.fieldArray = response;
        for (var field in this.fieldArray) {
          this.EnableEditDelbtn[field] = true;
          this.EnableSaveCnlbtn[field] = false;
          this.readonly[field] = true;
          this.getStoneName(this.fieldArray[field].Type, field);
        }
      }
    )
  }

  public stoneDtls: NTReceiptstoneModel;
  count: number = 0;
  EnableEditDelbtn = {};
  EnableSaveCnlbtn = {};

  readonly = {};

  //To disable the GStype/Item dropdown once row added
  EnableDropdown: boolean = false;

  //To disable and enable Addrow Button
  EnableAddRow: boolean = false;

  //To disable and enable Submit Button
  EnableSubmitButton: boolean = true;

  addrow() {
    this.stoneDtls = {
      BarcodeNo: null,
      SlNo: 0,
      Type: null,
      Name: null,
      Qty: null,
      Carrat: null,
      Weight: null,
      Rate: null,
      Amount: null
    };

    this.fieldArray.push(this.stoneDtls);
    for (let { } of this.fieldArray) {
      this.count++;
    }
    this.EnableSaveCnlbtn[this.count - 1] = true;
    this.EnableEditDelbtn[this.count - 1] = false;
    this.readonly[this.count - 1] = false
    this.count = 0;
    this.EnableAddRow = true;
    this.EnableSubmitButton = true;
    this.EnableDropdown = true;
  }

  @Input() NTReceiptsLinesDetails: any = [];

  saveDataFieldValue(index) {
    if (this.fieldArray[index]["Type"] == null || this.fieldArray[index]["Type"] == 0) {
      swal("Warning!", 'Please Select GS', "warning");
    }
    else if (this.fieldArray[index]["Name"] == null || this.fieldArray[index]["Name"] == 0) {
      swal("Warning!", 'Please Select Item', "warning");
    }
    else if (this.fieldArray[index]["Carrat"] == null || this.fieldArray[index]["Carrat"] == 0) {
      swal("Warning!", 'Please Enter Carrat', "warning");
    }
    else {
      this.NTReceiptsLinesDetails.BarcodeReceiptStoneDetails[index] = this.fieldArray[index];
      this.EnableEditDelbtn[index] = true;
      this.EnableSaveCnlbtn[index] = false;
      this.readonly[index] = true;
      this.EnableAddRow = false;
      this.EnableSubmitButton = false;
      this.SendCaratToNTReceiptsComp();
    }
  }

  @Output() stoneCarat = new EventEmitter<any>();

  stoneValue: any;


  SendCaratToNTReceiptsComp() {
    this.stoneCarat.emit(this.stoneValue);
  }

  cancelDataFieldValue(index) {
    this.EnableAddRow = false;
    this.fieldArray.splice(index, 1);
  }

  deleteFieldValue(index) {
    if (this.EnableAddRow == true) {
      swal("Warning!", 'Please save the enabled item', "warning");
    }
    else {
      var ans = confirm("Do you want to delete??");
      if (ans) {
        this.fieldArray.splice(index, 1);
        this.NTReceiptsLinesDetails.BarcodeReceiptStoneDetails.splice(index, 1);
        this.SendCaratToNTReceiptsComp();
        for (var field in this.fieldArray) {
          this.getStoneName(this.fieldArray[field].Type, field);
        }
      }
    }
  }

  stoneType: any = [];
  getStoneType() {
    this.barcodeService.getStoneType().subscribe(
      response => {
        this.stoneType = response;
      }
    )
  }

  //Stone Calculation
  getStoneWt(index) {
    this.fieldArray[index].Weight = parseFloat((this.fieldArray[index].Carrat * 0.2).toFixed(3));
  }

  getStoneCarat(index) {
    this.fieldArray[index].Carrat = parseFloat((this.fieldArray[index].Weight / 0.2).toFixed(3));
  }

  stoneName: any = [];
  getStoneName(arg, index) {
    this.barcodeService.getStoneName(arg).subscribe(
      response => {
        this.stoneName[index] = response;
      }
    )
  }

  getAmount(index) {
    this.fieldArray[index].Amount = parseFloat((this.fieldArray[index].Carrat * this.fieldArray[index].Rate).toFixed(2));
  }

}
