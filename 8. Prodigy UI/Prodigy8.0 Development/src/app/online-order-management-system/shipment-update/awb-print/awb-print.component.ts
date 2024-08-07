import { Component, OnInit } from '@angular/core';
import { OnlineOrderManagementSystemService } from '../../online-order-management-system.service';
import swal from 'sweetalert';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AppConfigService } from '../../../AppConfigService';
import { MasterService } from '../../../core/common/master.service';


@Component({
  selector: 'app-awb-print',
  templateUrl: './awb-print.component.html',
  styleUrls: ['./awb-print.component.css']
})
export class AwbPrintComponent implements OnInit {

  AWBPrintForm: FormGroup;
  AwbNo: number = null;


  constructor(private _onlineOrderManagementSystemService: OnlineOrderManagementSystemService,
    private fb: FormBuilder, private _appConfigService: AppConfigService,
    private _masterService: MasterService) {

  }

  ngOnInit() {
    this.AWBPrintForm = this.fb.group({
      OrderNo: [null],
      AWBNo: [null]
    });
  }

  AwbOutput: any;

  getAwb(form) {
    this._onlineOrderManagementSystemService.getAwb(form.value.OrderNo).subscribe(
      response => {
        this.AwbOutput = response;
        this.AwbNo = this.AwbOutput.DocumentNo;
        this.AWBPrintForm.controls.AWBNo.setValue(this.AwbNo);
      },
      (err) => {
        this.AWBPrintForm.controls.AWBNo.setValue(null);
      }
    )
  }


  outputData: any;

  printShipLabelContent(form) {
    if (form.value.OrderNo == null || form.value.OrderNo == "") {
      swal("Warning!", 'Please enter the Order No', "warning");
    }
    else if (form.value.AWBNo == null || form.value.OrderNo == "") {
      swal("Warning!", 'Please enter the AWB No', "warning");
    }
    else {
      this._onlineOrderManagementSystemService.getShipLabelContent(form.value.OrderNo).subscribe(
        response => {
          this.outputData = response;
          if (this.outputData.MarketPlaceInvoiceData != null && this.outputData.MarketPlaceInvoiceData != "") {
            this.download(this.outputData.MarketPlaceInvoiceData.FileFormat, this.outputData.MarketPlaceInvoiceData.FileData, this.outputData.MarketPlaceInvoiceData.Name);
          }
          if (this.outputData.PoSInvoiceData != null && this.outputData.PoSInvoiceData != "") {
            this.download(this.outputData.PoSInvoiceData.FileFormat, this.outputData.PoSInvoiceData.FileData, this.outputData.PoSInvoiceData.Name);
          }
        }
      )
    }
  }

  FileFormat: string;
  FileData: string;
  FileName: string;
  DisplayData: boolean = false;


  download(FileFormat, FileData, FileName) {
    this.DisplayData = true;
    this.FileFormat = FileFormat;
    this.FileData = FileData;
    this.FileName = FileName + "." + FileFormat;
    let pdfWindow = window.open("");
    pdfWindow.document.write("<html<head><title>" + FileName + "</title><style>body{margin: 0px;}iframe{border-width: 0px;}</style></head>");

    if (FileFormat != "html") {
      pdfWindow.document.write("<body><embed width='100%' height='100%' src='data:application/pdf;base64, " + FileData + "#toolbar=0&navpanes=0&scrollbar=0'></embed></body></html>");
    }
    else {
      pdfWindow.document.write(atob(FileData));
    }
  }

  clearAWB() {
    this.AWBPrintForm.reset();
  }

}
