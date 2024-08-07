import { Routes } from '@angular/router';

//import { NgbdDatepickerBasic } from './datepicker/datepicker.component';

import { NgbdtypeheadBasic } from './typehead/typehead.component';


export const ComponentsRoutes: Routes = [
  {
    path: '',
    children: [
      {
        path: 'typehead',
        component: NgbdtypeheadBasic,
        data: {
          title: 'Typehead',
          urls: [{ title: 'Dashboard', url: '/dashboard' }, { title: 'ngComponent' }, { title: 'Typehead' }]
        }
      }
    ]
  }
];
