import { Injectable } from '@angular/core';
import { CanActivate, Router, CanActivateChild } from '@angular/router';

@Injectable()
export class AuthGuardService implements CanActivate, CanActivateChild {
  constructor(private _Router: Router) {
  }

  canActivateChild(): boolean {
    if (localStorage.getItem('Token') != '') {
      return true;
    }
    else {
      localStorage.removeItem('Token');
      this._Router.navigate(['/']);
      return false;
    }
  }

  CheckToken() {
    return !!localStorage.getItem('Token');
  }

  canActivate(): boolean {
    if (this.CheckToken()) {
      return true;
    }
    else {
      this._Router.navigate(['/']);
      return false;
    }
  }
}