import { AfterViewInit, Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import '@progress/kendo-ui';
import { TelerikReportViewerComponent } from '@progress/telerik-angular-report-viewer';
import { AppConfigService } from '../AppConfigService';

declare var kendo: any;


@Component({
  encapsulation: ViewEncapsulation.None,
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.css']
})
export class ReportsComponent implements OnInit, AfterViewInit {

  @ViewChild('viewer1', { static: false }) viewer: TelerikReportViewerComponent;

  telerikreportURL: string;

  ngAfterViewInit(): void {
    kendo.culture("en-GB");
    var culture = kendo.culture();
    culture.calendar.patterns.d = "dd-MMM-yyyy";
  }

  EnableViewer: boolean = false;

  dataSource: any = {
    report: "",
  }

  viewerContainerStyle = {
    position: 'relative',
    width: '1400px',
    height: '1200px',
    left: '5px',
    right: '5px',
    overflow: 'hidden',
    clear: 'both',
    ['font-family']: 'ms sans serif',
  };

  constructor(private _appConfigService: AppConfigService) {
    this.telerikreportURL = this._appConfigService.telerikReportUrl;
  }

  ngOnInit() {
  }

  chng() {
    this.EnableViewer = false;
    this.dataSource = {
      report: "",
    }
  }

  Search(arg) {
    this.EnableViewer = true;
    this.dataSource = {
      report: arg + ".trdp",
    }
  }
}