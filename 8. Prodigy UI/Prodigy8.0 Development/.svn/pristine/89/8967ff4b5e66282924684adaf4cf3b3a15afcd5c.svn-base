import { NgModule } from '@angular/core';
import { AuthGuardService } from './../auth-guard.service';
import { Routes, RouterModule } from '@angular/router';
import {TagSplitComponent} from './tag-split.component';

export const TagSplitRoutes: Routes =[
  {
    path: '',
    canActivate: [AuthGuardService],
    canActivateChild: [AuthGuardService],
     component:   TagSplitComponent, data: { 
      title: 'Tag Split', 
      urls: [{title: 'Dashboard',url: '/dashboard'},{title: 'Tag Split'}]
    },
  }
];

@NgModule({
  imports: [RouterModule.forChild(TagSplitRoutes)],
  exports: [RouterModule]
})
export class TagSplitRoutingModule { }
