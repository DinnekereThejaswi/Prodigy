import { SigninService } from './signin.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { UserIdleService } from 'angular-user-idle';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import swal from 'sweetalert';

declare var $: any;

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  providers: [SigninService]
})
export class LoginComponent implements OnInit {
  // public form: FormGroup;
  //******************************************** https://www.npmjs.com/package/angular-user-idle//
  tokenDets: any;
  ccode: string = "";
  bcode: string = "";
  role: string;
  password: string;
  
  year = new Date().getFullYear();
  constructor(
    private router: Router,
    public service: SigninService,
    private _toastr: ToastrService,
    private userIdle: UserIdleService,
    private _appConfigService: AppConfigService
  ) {
    this.password = this._appConfigService.Pwd;

  }
  ngOnInit() {

    var showPass = 0;
    $('.btn-show-pass').on('click', function () {
      if (showPass == 0) {
        $(this).next('input').attr('type', 'text');
        $(this).find('i').removeClass(' fa-eye');
        $(this).find('i').addClass(' fa-eye-slash');
        showPass = 1;
      }
      else {
        $(this).next('input').attr('type', 'password');
        $(this).find('i').addClass(' fa-eye');
        $(this).find('i').removeClass(' fa-eye-slash');
        showPass = 0;
      }
    });
    // this.form = this.fb.group ( {
    //   uname: [null , Validators.compose ([ Validators.required ] )] , password: [null , Validators.compose ( [ Validators.required ] )]
    // } );
    this.userIdle.startWatching();

    // Start watching when user idle is starting.
    this.userIdle.onTimerStart().subscribe();

    // Start watch when time is up.
    this.userIdle.onTimeout().subscribe(() => {
      this.router.navigate(['/']);
      swal("Warning!", 'Session Expired', "warning");
    });
  }

  stop() {
    this.userIdle.stopTimer();
  }

  stopWatching() {
    this.userIdle.stopWatching();
  }

  startWatching() {
    this.userIdle.startWatching();
  }

  restart() {
    this.userIdle.resetTimer();
  }

  userName: string = '';
  passwordValue: string = '';
  loginSuccess: any;

  submit = function (f) {
    if (f.value.username == '') {
      swal("Warning!", 'Please Enter UserName', "warning");
      return false;
    }
    else if (f.value.password == '') {
      swal("Warning!", 'Please Enter Password', "warning");
    }
    else {
      this.service.getAccessToken(f.value)
        .subscribe(
          data => {
            this.tokenDets = data;
            localStorage.setItem('Token', this.tokenDets.Credentials.Token);
            localStorage.setItem('refreshToken', this.tokenDets.Credentials.RefreshToken);
            localStorage.setItem('Expiry', this.tokenDets.Credentials.ExpiresIn);
            localStorage.setItem('Login', f.value.username);
            localStorage.setItem('UserID', this.tokenDets.Credentials.UserID);
            this.getCompanyBranch(f.value.username);
          }
        );
      this.loginSuccess = true;
    }
  }

  data: any;
  getCompanyBranch(arg) {
    this.service.getAllCompanybranchCode(arg)
      .subscribe(
        response => {
          this.data = response;
          if (this.data.length === 1) {
            this.router.navigate(['/dashboard']);
            localStorage.setItem('OperatorCode', arg);
            this.ccode = CryptoJS.AES.encrypt(this.data[0].CompanyCode, this.password.trim()).toString();
            this.bcode = CryptoJS.AES.encrypt(this.data[0].BranchCode, this.password.trim()).toString();
            localStorage.setItem('ccode', this.ccode);
            localStorage.setItem('bcode', this.bcode);
            this.service.SendBranchCode(this.bcode);
          }
          else if (this.data.length > 1) {
            localStorage.setItem('OperatorCode', arg);
            this.ccode = CryptoJS.AES.encrypt(this.data[0].CompanyCode, this.password.trim()).toString();
            this.bcode = CryptoJS.AES.encrypt(this.data[0].BranchCode, this.password.trim()).toString();
            localStorage.setItem('ccode', this.ccode);
            localStorage.setItem('bcode', this.bcode);
            this.router.navigate(['/cbwidget']);
          }
          else {
            swal("Warning!", 'Operator Branch mapping not done', "warning");
          }
        }
      )
  }
}