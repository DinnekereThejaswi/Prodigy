import { Component, Input, OnChanges, OnInit } from '@angular/core';
import { LastdocnoService } from './lastdocno.service';
import { Router } from '@angular/router';
import * as moment from 'moment'

declare var $: any;

@Component({
  selector: 'app-lastdocno',
  templateUrl: './lastdocno.component.html',
  styleUrls: ['./lastdocno.component.css']
})
export class LastdocnoComponent implements OnInit, OnChanges {

  EnableLastDocNo: boolean = false;

  @Input()
  routerUrl: string = "";

  constructor(private _router: Router, private lastDocNoService: LastdocnoService) {
    this.EnableLastDocNo = false;
  }

  ngOnInit() {
    this.CurrentFinancialYear = moment(new Date()).format('YY');
    this.LastDocNo();
  }

  lastDocNo: any;
  CurrentFinancialYear: string;
  lastNo: any;
  routeUrl: string;
  DocType: string;

  ngOnChanges() {
    this.routerUrl = this.routerUrl;
    this.LastDocNo();
  }

  LastDocNo() {
    this.routeUrl = this._router.url;
    switch (this.routeUrl) {
      case "/orders":
        this.DocType = "ORDNO";
        this.getLastDocNo("Last Order No", this.DocType);
        break;
      case "/estimation":
        this.DocType = "SALEST";
        this.getLastDocNo("Last Est No", this.DocType);
        break;
      case "/purchase":
        this.DocType = "PUREST";
        this.getLastDocNo("Last Est No", this.DocType);
        break;
      case "/salesreturn":
        this.DocType = "SREST";
        this.getLastDocNo("Last SR Est.No", this.DocType);
        break;
      case "/sales-billing":
        this.DocType = "SALINV";
        this.getLastDocNo("Last Bill No", this.DocType);
        break;
      case "/new-sales-billing":
        this.DocType = "SALINV";
        this.getLastDocNo("Last Bill No", this.DocType);
        break;
      case "/repair/repair-delivery":
        this.DocType = "REPISS";
        this.getLastDocNo("Last Delivery No", this.DocType);
        break;
      case "/repair/repair-receipts":
        this.DocType = "REPREC";
        this.getLastDocNo("Last Repair No", this.DocType);
        break;
      case "/purchase/purchase-billing":
        this.DocType = "PURINV";
        this.getLastDocNo("Last Bill No", this.DocType);
        break;
      case "/orders/OrderReceipt":
        this.DocType = "ORDREC";
        this.getLastDocNo("Last Order Receipt No", this.DocType);
        break;
      case "/salesreturn/ConfirmSalesReturn":
        this.DocType = "SRINV";
        this.getLastDocNo("Last SR No", this.DocType);
        break;
      case "/credit-receipt/credit-receipt":
        this.DocType = "CREINV";
        this.getLastDocNo("Last Receipt No", this.DocType);
        break;
      case "/stocks/stock-taking":
        this.DocType = "STKBAT";
        this.getLastDocNo("Last Batch No", this.DocType);
        break;
      ///Issue And Receipts , OPG
      //branch issue
      case "/branchissue/nt-issue":
        this.DocType = "NTGISS";
        this.getLastDocNo("Last Issue No", this.DocType);
        break;
      case "/branchissue/sr-issue":
        this.DocType = "SRISS";
        this.getLastDocNo("Last Issue No", this.DocType);
        break;
      case "/branchissue/opgissue":
        this.DocType = "OPGISS";
        this.getLastDocNo("Last Issue No", this.DocType);
        break;
      case "/branchissue/tagissue":
        this.DocType = "TAGISS";
        this.getLastDocNo("Last Issue No", this.DocType);
        break;
      case "/branchreceipts/barcodereceipts":
        this.DocType = "BARREC";
        this.getLastDocNo("Last Receipt No", this.DocType);
        break;
      case "/branchreceipts/nontagreceipts":
        this.DocType = "NTGREC";
        this.getLastDocNo("Last Receipt No", this.DocType);
        break;
      case "/branchreceipts/autobarcode-receipts":
        this.DocType = "AUTREC";
        this.getLastDocNo("Last Receipt No", this.DocType);
        break;
      case "/opg-process/opg-seggregation":
        this.DocType = "OPGSEG";
        this.getLastDocNo("Last  No", this.DocType);
        break;
      case "/opg-process/opg-melting-issue":
        this.DocType = "MLTISS";
        this.getLastDocNo("Last  No", this.DocType);
        break;
      case "/opg-process/opg-melting-receipt":
        this.DocType = "MLTREC";
        this.getLastDocNo("Last  No", this.DocType);
        break;
      case "/opg-process/opg-mg-issue-cpc":
        this.DocType = "CPCISS";
        this.getLastDocNo("Last Issue No", this.DocType);
        break;
    }
  }

  getLastDocNo(docText, docType) {
    this.lastDocNoService.getLastDocNum(docType).subscribe(
      response => {
        this.lastNo = response;
        if (this.lastNo != null && this.lastNo != 0) {
          this.lastDocNo = docText + " : " + this.lastNo.DocumentNo;
          // if (docType == "SALEST" || docType == "PUREST" || docType == "SREST") {
          //   this.lastDocNo = docText + " : " + this.lastNo.DocumentNo;
          // }
          // else {
          //   this.lastDocNo = docText + " : " + this.CurrentFinancialYear + this.lastNo.DocumentNo;
          // }
          //this.lastDocNo = this.CurrentFinancialYear+this.lastNo.DocumentNo;            
          //$("#LastDocumentNo").modal('show');
        }
      }
    )
  }
}