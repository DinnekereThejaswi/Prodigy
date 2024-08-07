// import { HeaderService } from './header.service';
// import { Component, AfterViewInit, OnInit } from '@angular/core';
// import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
// import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
// import { Observable } from 'rxjs';
// import "rxjs/add/operator/debounceTime";
// import "rxjs/add/operator/map";
// import "rxjs/add/operator/switchMap";
// import "rxjs/add/operator/distinctUntilChanged";
// import 'rxjs/add/observable/of';
// import 'rxjs/add/operator/do';
// import { Router } from '@angular/router';
// import { ToastrService } from 'ngx-toastr';
// import { Idle, DEFAULT_INTERRUPTSOURCES } from '@ng-idle/core';
// import { SideBarService } from '../sidebar/sidebar.service';
// import * as CryptoJS from 'crypto-js';
// import { AppConfigService } from '../../AppConfigService';
// import { SigninService } from '../../authentication/login/signin.service';
// import state from 'sweetalert/typings/modules/state';
// import { Keepalive } from '@ng-idle/keepalive';
// import { MasterService } from './../../core/common/master.service';
// declare var $: any;

// @Component({
//   selector: 'ap-navigation',
//   templateUrl: './navigation.component.html',
//   providers: [HeaderService]
// })

// export class NavigationComponent implements OnInit {
//   name: string;
//   state: any = [];
//   DailyRate: any = [];
//   Gold22k: string = "";
//   Silver: string = "";
//   idleState = '';
//   timedOut = false;
//   ccode: string = "";
//   LoggedIn: string = "";
//   bcode: string = "";
//   password: string;
//   apiBaseUrl: string;



//   public config: PerfectScrollbarConfigInterface = {};
//   constructor(private modalService: NgbModal,
//     private _headerService: HeaderService,
//     private _toastr: ToastrService,
//     private _Route: Router,
//     private idle: Idle,
//     // private keepalive: Keepalive,
//     private _sidebarService: SideBarService,
//     private appConfigService: AppConfigService,
//     private service: SigninService, private _masterService: MasterService) {

//     this.apiBaseUrl = this.appConfigService.apiBaseUrl;
//     this.password = this.appConfigService.Pwd;

//     //this.getBranch();


//     idle.setIdle(5);
//     idle.setTimeout(6000);
//     idle.setInterrupts(DEFAULT_INTERRUPTSOURCES);

//     idle.onIdleEnd.subscribe(() => {
//       this.idleState = '';
//       this.timedOut = false;
//     });
//     idle.onTimeout.subscribe(() => {
//       this.timedOut = false;
//       this._toastr.error("Session Timed Out", "Oops");
//       this._Route.navigate(['/']);
//     });

//     idle.onIdleStart.subscribe(() => {
//       this.idleState = 'You\'ve gone idle!';
//       this.timedOut = true;
//     });
//     idle.onTimeoutWarning.subscribe(countdown => this.idleState = countdown);

//     // keepalive.interval(15);
//     this.reset();
//   }

//   ngOnInit() {

//     this.idleState = '';
//     this.LoggedIn = localStorage.getItem("UserID");
//     this.service.castBranchCode.subscribe(
//       response => {
//         this.bcode = response;
//         this.bcode = CryptoJS.AES.decrypt(this.bcode, this.password.trim()).toString(CryptoJS.enc.Utf8);
//       }
//     );


//     this.getBranch();
//     this.getCompany();
//     // this.GetDailyRate();
//   }

//   reset() {
//     this.idle.watch();
//     this.timedOut = false;
//   }

//   getBranch() {
//     if (localStorage.getItem('bcode') != null) {
//       this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
//     }
//   }

//   logOut() {
//     localStorage.clear();
//     this._toastr.success("You have logged out successfully", "Great");
//     this._Route.routeReuseStrategy.shouldReuseRoute = () => false;
//     this._Route.navigate(['/login']);
//   }

//   Company: any;

//   getCompany() {
//     this._masterService.getCompanyMaster().subscribe(
//       response => {
//         this.Company = response;
//       }
//     )
//   }

//   // GetDailyRate() {
//   //   this._masterService.GetDailyRate().subscribe(
//   //     Response => {
//   //       this.DailyRate = Response;
//   //       for (let i = 0; i < this.DailyRate.SellingRates.length; i++) {
//   //         if (this.DailyRate.SellingRates[i].Karat == "22K" && this.DailyRate.SellingRates[i].GsCode == "NGO") {
//   //           this.Gold22k = this.DailyRate.SellingRates[i].SellingBoardRate;
//   //         }
//   //         else if (this.DailyRate.SellingRates[i].GsCode == "SL") {
//   //           this.Silver = this.DailyRate.SellingRates[i].SellingBoardRate;
//   //         }
//   //       }
//   //     }
//   //   )
//   // }


//   MenuList: any = [];

//   getSearchMenuData() {
//     this._sidebarService.getMenus().subscribe(
//       response => {
//         this.MenuList = response;
//         for (let i = 0; i < this.MenuList.length; i++) {
//           for (var j = 0; j < this.MenuList[i].lstOfMainMenuWithSubMenu.length; j++) {
//             this.state.push(this.MenuList[i].lstOfMainMenuWithSubMenu[j]);
//           }
//         }
//       }
//     )
//   }

//   public model: any;
//   search = (text$: Observable<string>) =>
//     text$
//       .debounceTime(200)
//       .map(
//         term => term === '' ? []
//           : this.state.filter(
//             v => v.Tittle.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 50));

//   formatter2 = (x: { Tittle: string }) => x.Tittle;

//   temp(arg) {
//     this._Route.navigate(['/' + arg.Path]);
//     if (this.state.find(c => c.Path === arg.Path)) {
//       this._Route.navigate(['/' + arg.Path])

//     }
//     else {
//       alert("Invalid Search")
//     }
//   }

//   sideMenuBar: any;
//   ROUTES: any = [];
//   public sidebarnavItems: any;

//   // This is for Notifications
//   notifications: Object[] = [{
//     round: 'round-danger',
//     icon: 'ti-link',
//     title: 'Luanch Admin',
//     subject: 'Just see the my new admin!',
//     time: '9:30 AM'
//   }, {
//     round: 'round-success',
//     icon: 'ti-calendar',
//     title: 'Event today',
//     subject: 'Just a reminder that you have event',
//     time: '9:10 AM'
//   }, {
//     round: 'round-info',
//     icon: 'ti-settings',
//     title: 'Settings',
//     subject: 'You can customize this template as you want',
//     time: '9:08 AM'
//   }, {
//     round: 'round-primary',
//     icon: 'ti-user',
//     title: 'Pavan kumar',
//     subject: 'Just see the my admin!',
//     time: '9:00 AM'
//   }];

//   // This is for Mymessages
//   mymessages: Object[] = [{
//     useravatar: 'assets/images/users/1.jpg',
//     status: 'online',
//     from: 'Pavan kumar',
//     subject: 'Just see the my admin!',
//     time: '9:30 AM'
//   }, {
//     useravatar: 'assets/images/users/2.jpg',
//     status: 'busy',
//     from: 'Sonu Nigam',
//     subject: 'I have sung a song! See you at',
//     time: '9:10 AM'
//   }, {
//     useravatar: 'assets/images/users/2.jpg',
//     status: 'away',
//     from: 'Arijit Sinh',
//     subject: 'I am a singer!',
//     time: '9:08 AM'
//   }, {
//     useravatar: 'assets/images/users/4.jpg',
//     status: 'offline',
//     from: 'Pavan kumar',
//     subject: 'Just see the my admin!',
//     time: '9:00 AM'
//   }];



//   ngAfterViewInit() {
//     this.getSearchMenuData();

//   }
// }


import { HeaderService } from './header.service';
import { Component, AfterViewInit, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
import { Observable } from 'rxjs';
import "rxjs/add/operator/debounceTime";
import "rxjs/add/operator/map";
import "rxjs/add/operator/switchMap";
import "rxjs/add/operator/distinctUntilChanged";
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/do';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Idle, DEFAULT_INTERRUPTSOURCES } from '@ng-idle/core';
import { SideBarService } from '../sidebar/sidebar.service';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import { SigninService } from '../../authentication/login/signin.service';
import state from 'sweetalert/typings/modules/state';
import { Keepalive } from '@ng-idle/keepalive';
import { MasterService } from './../../core/common/master.service';
declare var $: any;

@Component({
  selector: 'ap-navigation',
  templateUrl: './navigation.component.html',
  providers: [HeaderService]
})

export class NavigationComponent implements OnInit {
  name: string;
  state: any = [];
  DailyRate: any = [];
  Gold22k: string = "";
  Silver: string = "";
 // idleState = '';
  timedOut = false;
  ccode: string = "";
  LoggedIn: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;

  idleState = 'NOT_STARTED';
  countdown?: string = null;



  public config: PerfectScrollbarConfigInterface = {};
  constructor(private modalService: NgbModal,
    private _headerService: HeaderService,
    private _toastr: ToastrService,
    private _Route: Router,
    private idle: Idle,
    // private keepalive: Keepalive,
    private _sidebarService: SideBarService,
    private appConfigService: AppConfigService,
    private service: SigninService, private _masterService: MasterService) {


    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.password = this.appConfigService.Pwd;

    this.getBranch();

    // set idle parameters

    idle.setIdle(5);// how long can they be inactive before considered idle, in seconds
    //idle.setTimeout();// how long can they be idle before considered timed out, in seconds
   // idle.setInterrupts(DEFAULT_INTERRUPTSOURCES);// provide sources that will "interrupt" aka provide events indicating the user is active

      // do something when the user becomes idle
      idle.onIdleStart.subscribe(() => {
        this.idleState = 'You\'ve gone idle!';
        this.timedOut = true;
      });


 // do something when the user is no longer idle
    idle.onIdleEnd.subscribe(() => {
      this.idleState = '';
      this.timedOut = false;
    });

       // do something when the user has timed out
    idle.onTimeout.subscribe(() => {
      this.timedOut = false;
      this._toastr.error("Session Timed Out", "Oops");
      this._Route.navigate(['/']);
    });

    // do something as the timeout countdown does its thing
    idle.onTimeoutWarning.subscribe(countdown => this.idleState = countdown);

    // keepalive.interval(15);
    this.reset();
  }

  ngOnInit() {
    this.idleState = '';
    this.LoggedIn = localStorage.getItem("UserID");
    this.service.castBranchCode.subscribe(
      response => {
        this.bcode = response;
        this.bcode = CryptoJS.AES.decrypt(this.bcode, this.password.trim()).toString(CryptoJS.enc.Utf8);
      }
    );


    this.getBranch();
    this.getCompany();
    // this.GetDailyRate();
  }

  reset() {
    this.idle.watch();
    this.timedOut = false;
  }

  getBranch() {
    if (localStorage.getItem('bcode') != null) {
      this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    }
  }

  logOut() {
    localStorage.clear();
    this._toastr.success("You have logged out successfully", "Great");
    this._Route.routeReuseStrategy.shouldReuseRoute = () => false;
    this._Route.navigate(['/login']);
  }

  Company: any;

  getCompany() {
    this._masterService.getCompanyMaster().subscribe(
      response => {
        this.Company = response;
      }
    )
  }

  // GetDailyRate() {
  //   this._masterService.GetDailyRate().subscribe(
  //     Response => {
  //       this.DailyRate = Response;
  //       for (let i = 0; i < this.DailyRate.SellingRates.length; i++) {
  //         if (this.DailyRate.SellingRates[i].Karat == "22K" && this.DailyRate.SellingRates[i].GsCode == "NGO") {
  //           this.Gold22k = this.DailyRate.SellingRates[i].SellingBoardRate;
  //         }
  //         else if (this.DailyRate.SellingRates[i].GsCode == "SL") {
  //           this.Silver = this.DailyRate.SellingRates[i].SellingBoardRate;
  //         }
  //       }
  //     }
  //   )
  // }


  MenuList: any = [];

  getSearchMenuData() {
    this._sidebarService.getMenus().subscribe(
      response => {
        this.MenuList = response;
        for (let i = 0; i < this.MenuList.length; i++) {
          for (var j = 0; j < this.MenuList[i].lstOfMainMenuWithSubMenu.length; j++) {
            this.state.push(this.MenuList[i].lstOfMainMenuWithSubMenu[j]);
          }
        }
      }
    )
  }

  public model: any;
  search = (text$: Observable<string>) =>
    text$
      .debounceTime(200)
      .map(
        term => term === '' ? []
          : this.state.filter(
            v => v.Tittle.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 50));

  formatter2 = (x: { Tittle: string }) => x.Tittle;

  temp(arg) {
    this._Route.navigate(['/' + arg.Path]);
    if (this.state.find(c => c.Path === arg.Path)) {
      this._Route.navigate(['/' + arg.Path])

    }
    else {
      alert("Invalid Search")
    }
  }

  sideMenuBar: any;
  ROUTES: any = [];
  public sidebarnavItems: any;

  // This is for Notifications
  notifications: Object[] = [{
    round: 'round-danger',
    icon: 'ti-link',
    title: 'Luanch Admin',
    subject: 'Just see the my new admin!',
    time: '9:30 AM'
  }, {
    round: 'round-success',
    icon: 'ti-calendar',
    title: 'Event today',
    subject: 'Just a reminder that you have event',
    time: '9:10 AM'
  }, {
    round: 'round-info',
    icon: 'ti-settings',
    title: 'Settings',
    subject: 'You can customize this template as you want',
    time: '9:08 AM'
  }, {
    round: 'round-primary',
    icon: 'ti-user',
    title: 'Pavan kumar',
    subject: 'Just see the my admin!',
    time: '9:00 AM'
  }];

  // This is for Mymessages
  mymessages: Object[] = [{
    useravatar: 'assets/images/users/1.jpg',
    status: 'online',
    from: 'Pavan kumar',
    subject: 'Just see the my admin!',
    time: '9:30 AM'
  }, {
    useravatar: 'assets/images/users/2.jpg',
    status: 'busy',
    from: 'Sonu Nigam',
    subject: 'I have sung a song! See you at',
    time: '9:10 AM'
  }, {
    useravatar: 'assets/images/users/2.jpg',
    status: 'away',
    from: 'Arijit Sinh',
    subject: 'I am a singer!',
    time: '9:08 AM'
  }, {
    useravatar: 'assets/images/users/4.jpg',
    status: 'offline',
    from: 'Pavan kumar',
    subject: 'Just see the my admin!',
    time: '9:00 AM'
  }];



  ngAfterViewInit() {
    this.getSearchMenuData();
    let ExpiredBy = Number(localStorage.getItem("Expiry"));
    this.idle.setTimeout(ExpiredBy)

  }
}