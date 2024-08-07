// import { Component, OnInit } from '@angular/core';
// import { Router } from '@angular/router';

// import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';

// @Component({
//     selector: 'full-layout',
//     templateUrl: './full.component.html',
//     styleUrls: ['./full.component.scss']
// })
// export class FullComponent implements OnInit {

//     color = 'bluedark';
//     showSettings = false;
//     showMinisidebar = false;
//     showDarktheme = false;
//     showHorizontalNav = true;
//     showBoxedtheme = true;

//     isChecked = true;
//     CheckUncheck: Boolean = true;


//     public config: PerfectScrollbarConfigInterface = {};

//     constructor(public router: Router) { }

//     checkValue(event: any) {
//         if (event == true) {
//             this.CheckUncheck = true;
//         }
//         else {
//             this.CheckUncheck = false;
//         }
//     }

//     ngOnInit() {
//         if (this.router.url === '/') {
//             this.router.navigate(['/estimation']);
//         }
//     }
// }


import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';

import { PerfectScrollbarConfigInterface, PerfectScrollbarDirective } from 'ngx-perfect-scrollbar';

@Component({
    selector: 'full-layout',
    templateUrl: './full.component.html',
    styleUrls: ['./full.component.scss']
})
export class FullComponent implements OnInit {
    // color = 'bluedark';
    // showSettings = false;
    // showDarktheme = false;
    // showHorizontalNav = true;
    // showBoxedtheme = true;
    // showMinisidebar = true;
    // isChecked = true;
    // CheckUncheck: Boolean = true;


    // public config: PerfectScrollbarConfigInterface = {};

    // constructor(public router: Router) { }

    // checkValue(event: any) {
    //     if (event == true) {
    //         this.CheckUncheck = true;
    //     }
    //     else {
    //         this.CheckUncheck = false;
    //     }
    // }

    // ngOnInit() {
    //     if (this.router.url === '/') {
    //         this.router.navigate(['/estimation']);
    //     }
    // }
    @ViewChild(PerfectScrollbarDirective, { static: false }) perfectScrollbarDirectiveRef?: PerfectScrollbarDirective;
    color = 'defaultdark';
    showSettings = false;
    showMinisidebar = true;
    showDarktheme = false;

    public config: PerfectScrollbarConfigInterface = {

    };

    constructor(public router: Router) { }

    ngOnInit() {
        // this.perfectScrollbarDirectiveRef.update();
        //this.perfectScrollbarDirectiveRef.scrollToTop(0, 1);

        if (this.router.url === '/') {
            this.router.navigate(['/dashboard/dashboard1']);
        }
    }
}