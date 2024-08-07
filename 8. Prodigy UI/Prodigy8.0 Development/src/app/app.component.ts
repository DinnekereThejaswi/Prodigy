import { KeyboardShortcutsService } from 'ng-keyboard-shortcuts';
import { Router, ActivatedRoute, Event, NavigationStart, NavigationEnd, NavigationCancel, NavigationError } from '@angular/router';
import { ConnectionService } from 'ng-connection-service';
import { Directive, OnInit, ElementRef, Renderer2, HostListener, HostBinding, Input, Component } from '@angular/core';
import { UserIdleService } from 'angular-user-idle';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { SideBarService } from './shared/sidebar/sidebar.service';
import { ToastrService } from 'ngx-toastr';
import { HttpClient } from '@angular/common/http';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  providers: [KeyboardShortcutsService, ConnectionService]
})
export class AppComponent implements OnInit {
  today = new Date();
  title = 'app';
  status = 'ONLINE';
  isConnected = true;
  timeout;
  loader = true;
  datePickerConfig: Partial<BsDatepickerConfig>;
  constructor(
    private _http: HttpClient,
    private connectionService: ConnectionService,
    private router: Router, private _sideBarService: SideBarService
  ) {
    this.connectionService.monitor().subscribe(isConnected => {
      this.isConnected = isConnected;
      if (this.isConnected) {
        this.status = "ONLINE";
        alert('Online');
      }
      else {
        this.status = "OFFLINE";
        alert('OFFLINE');
        window.location.reload();
      }
    })

    router.events.subscribe((event: Event) => {
      this.navigationInterceptor(event);
    });

    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        minDate: this.today,
        dateInputFormat: 'DD/MM/YYYY'
      });

  }


  ngOnInit() {
  }

  navigationInterceptor(event: Event): void {
    if (event instanceof NavigationStart) {
      this.loader = true;
    }
    if (event instanceof NavigationEnd) {
      // Hide loading indicator
      this.timeout = setTimeout(() => {
        clearTimeout(this.timeout);
        // this.getUserMenuAPI();
        this.loader = false;
      }, 500);
    }

    // Set loading state to false in both of the below events to hide the spinner in case a request fails
    if (event instanceof NavigationCancel) {
      this.loader = false;
    }
    if (event instanceof NavigationError) {
      this.loader = false;
    }
  }

  MenuList: any;
  SubMenuList: any = [];
  splitRouter: any = [];
  ExistFlag: boolean = false;

  getUserMenuAPI() {
    this._sideBarService.getMenus().subscribe(
      response => {
        this.MenuList = response;
        this.ExistFlag = false;
        this.splitRouter = this.router.url.split("/");
        this.splitRouter = this.splitRouter[1];
        for (let i = 0; i < this.MenuList.length; i++) {
          if (this.MenuList[i].Tittle.replace(/\s/g, "").toLowerCase() == this.splitRouter.replace(/\s/g, "").toLowerCase()) {
            for (let j = 0; j < this.MenuList[i].lstOfMainMenuWithSubMenu.length; j++) {
              if (this.MenuList[i].lstOfMainMenuWithSubMenu[j].Path == this.router.url + "/") {
                this.ExistFlag = true;
                break;
              }
            }
          }
        }
        //Added below condition since these routes are used for internal purpose
        if (this.router.url == "/dashboard" ||
          this.router.url == "/login" ||
          this.router.url == "/cbwidget" ||
          this.router.url == "/estimation" ||
          this.router.url == "/sales-billing") {
          this.ExistFlag = true;
        }

        if (this.ExistFlag == false) {
          // localStorage.clear();
          // this.router.navigateByUrl('/redirect', { skipLocationChange: true }).then(() =>
          this.router.navigate(['/errorpages']);


        }
      }
    )
  }

  goTo404() {

    // this.route.navigate(['/page']); /
    // this.router.navigateByUrl('/error-page', { skipLocationChange: true }).then(() =>

  }
}