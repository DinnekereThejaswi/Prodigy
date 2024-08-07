// import { SideBarService } from './sidebar.service';
// import { Component, OnInit, Input } from '@angular/core';
// import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
// //import { ROUTES } from './menu-items';
// import { Router } from "@angular/router";
// import { ToastrService } from 'ngx-toastr';
// import { Keepalive } from '@ng-idle/keepalive';
// import { Idle, DEFAULT_INTERRUPTSOURCES } from '@ng-idle/core';
// import swal from 'sweetalert';


// declare var $: any;
// @Component({
//     selector: 'ap-sidebar',
//     templateUrl: './sidebar.component.html',
//     providers: [SideBarService]
// })
// export class SidebarComponent implements OnInit {

//     showMenu: string = '';
//     showSubMenu: string = '';
//     public sidebarnavItems: any;

//     //this is for the open close
//     addExpandClass(element: any) {
//         if (element === this.showMenu) {
//             this.showMenu = '0';

//         } else {
//             this.showMenu = element;
//         }
//     }

//     addActiveClass(element: any) {
//         if (element === this.showSubMenu) {
//             this.showSubMenu = '0';

//         } else {
//             this.showSubMenu = element;
//         }
//     }

//     constructor(private modalService: NgbModal, private router: Router,
//         private _toastr: ToastrService,
//         private idle: Idle,
//         // private keepalive: Keepalive,
//         private _sidebarService: SideBarService
//     ) {
//         this.getMenuList();
//     }
//     // End open close

//     ROUTES: any = [];

//     ngOnInit() {
//         //this.sidebarnavItems = ROUTES.filter(sidebarnavItem => sidebarnavItem);
//         $(function () {
//             $(".sidebartoggler").on('click', function () {
//                 if ($("#main-wrapper").hasClass("mini-sidebar")) {
//                     $("body").trigger("resize");
//                     $("#main-wrapper").removeClass("mini-sidebar");

//                 } else {
//                     $("body").trigger("resize");
//                     $("#main-wrapper").addClass("mini-sidebar");
//                 }
//             });
//         });
//     }

//     reset() {
//         this.idle.watch();
//     }

//     sideMenuBar: any;
//     getMenuList() {
//         this._sidebarService.getMenus().subscribe(
//             response => {
//                 this.ROUTES = response;
//                 this.sidebarnavItems = this.ROUTES.filter(sidebarnavItem => sidebarnavItem);
//                 //localStorage.setItem('Menus', JSON.stringify(this.sidebarnavItems));                
//                 // console.log(this.sidebarnavItems);
//             },
//             error => {
//                 swal("Error", "Unable to load menus", "error");
//             }
//         )
//     }

//     @Input() Checked: Boolean;
// }


import { SideBarService } from './sidebar.service';
import { Component, OnInit, Input } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
//import { ROUTES } from './menu-items';
import { Router } from "@angular/router";
import { ToastrService } from 'ngx-toastr';
import { Keepalive } from '@ng-idle/keepalive';
import { Idle, DEFAULT_INTERRUPTSOURCES } from '@ng-idle/core';
import swal from 'sweetalert';
import { Observable } from 'rxjs';
import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';

declare var $: any;
@Component({
    selector: 'ap-sidebar',
    templateUrl: './sidebar.component.html',
    providers: [SideBarService]
})
export class SidebarComponent implements OnInit {
    showMenu: string = '';
    showSubMenu: string = '';
    public sidebarnavItems: any;
    public config: PerfectScrollbarConfigInterface = {};
    //this is for the open close
    addExpandClass(element: any) {
        if (element === this.showMenu) {
            this.showMenu = '0';

        } else {
            this.showMenu = element;
        }
    }

    addActiveClass(element: any) {
        if (element === this.showSubMenu) {
            this.showSubMenu = '0';

        } else {
            this.showSubMenu = element;
        }
    }

    constructor(private modalService: NgbModal, private router: Router,
        private _toastr: ToastrService,
        private idle: Idle,
        // private keepalive: Keepalive,
        private _sidebarService: SideBarService,
        private _Route: Router,
    ) {
        this.getMenuList();
    }
    // End open close

    ROUTES: any = [];

    ngOnInit() {

        //this.sidebarnavItems = ROUTES.filter(sidebarnavItem => sidebarnavItem);
        $(function () {
            $(".sidebartoggler").on('click', function () {
                if ($("#main-wrapper").hasClass("mini-sidebar")) {
                    $("body").trigger("resize");
                    $("#main-wrapper").removeClass("mini-sidebar");


                } else {

                    $("body").trigger("resize");
                    $("#main-wrapper").addClass("mini-sidebar");
                }
            });
        });
    }

    reset() {
        this.idle.watch();
    }

    sideMenuBar: any;
    getMenuList() {
        this._sidebarService.getMenus().subscribe(
            response => {
                this.ROUTES = response;
                this.sidebarnavItems = this.ROUTES.filter(sidebarnavItem => sidebarnavItem);
                //localStorage.setItem('Menus', JSON.stringify(this.sidebarnavItems));                
                // console.log(JSON.stringify(this.sidebarnavItems));
            },
            error => {
                swal("Error", "Unable to load menus", "error");
            }
        )
    }
    state: any = [];
    temp(arg) {
        this._Route.navigate(['/' + arg.Path]);
        if (this.state.find(c => c.Path === arg.Path)) {
            this._Route.navigate(['/' + arg.Path])

        }
        else {
            alert("Invalid Search")
        }
    }
    @Input() Checked: Boolean;
    ngAfterViewInit() {

        $("body").trigger("resize");
        $("#main-wrapper").addClass("mini-sidebar");
        $(".search-box a, .search-box .app-search .srh-btn").on('click', function () {
            $(".app-search").toggle(200);
        });
        $("body").trigger("resize");
    }
}
