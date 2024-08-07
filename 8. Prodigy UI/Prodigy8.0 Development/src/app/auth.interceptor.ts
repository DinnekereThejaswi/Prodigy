import { ToastrService } from 'ngx-toastr';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { Injectable, Compiler } from "@angular/core";
import { Router } from "@angular/router";
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/catch';
import swal from 'sweetalert';
import { SigninService } from './authentication/login/signin.service';
import { AppConfigService } from './AppConfigService';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    RefreshTokenDets: any;
    token: any;
    constructor(
        private _router: Router,
        private _toastr: ToastrService,
        private _compiler: Compiler,
        private http: HttpClient,
        public service: SigninService,
        private appConfigService: AppConfigService
    ) {

    }

    refreshTokenbody: any;

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        this._compiler.clearCache();
        if (req.url.indexOf('auth/token') > -1 || req.url.indexOf('auth/refresh-token') > -1) {
            req = req.clone({
                setHeaders: {
                    //Authorization: "Bearer " + localStorage.getItem('Token'),
                    'Content-Type': 'application/json'
                }
            })
        }
        else {
            req = req.clone({
                setHeaders: {
                    Authorization: "Bearer " + localStorage.getItem('Token'),
                    'Content-Type': 'application/json'
                }
            });
        }

        //#region Old Code for Refresh Token and Error Handling Logic

        // return next.handle(req)
        //     .do(
        //         succ => {
        //         },
        //         err => {
        //             if (err.status === 401) {
        //                 if (err.error != null && err.error.Description == "Invalid login Credentials") {
        //                     swal("Warning!", err.error.Description, "warning");
        //                 }
        //                 else {
        //                     if (localStorage.getItem("Token") != null && localStorage.getItem("refreshToken") != null) {
        //                         this.service.getRefreshToken().subscribe(
        //                             data => {
        //                                 //If reload successful update tokens
        //                                 this.RefreshTokenDets = data;
        //                                 localStorage.setItem('Token', this.RefreshTokenDets.Credentials.Token);
        //                                 localStorage.setItem('refreshToken', this.RefreshTokenDets.Credentials.RefreshToken);
        //                                 this.token = localStorage.getItem('Token');
        //                                 req = req.clone({
        //                                     setHeaders: {
        //                                         Authorization: "Bearer " + this.token,
        //                                         'Content-Type': 'application/json',
        //                                     }
        //                                 });
        //                                 return next.handle(req).subscribe(
        //                                 );
        //                             },
        //                             error => {
        //                                 this.clearData();
        //                             });
        //                     }
        //                 }
        //             }
        //             else if (err.status === 403) {
        //                 const validationError = err.error;
        //                 swal("Warning!", validationError.description, "warning");
        //             }
        //             else if (err.status === 404) {
        //                 const validationError = err.error;
        //                 if (validationError.description != "Estimation No Does Not Exist" && validationError.description != "No payment details for the selected order.") {
        //                     swal("Warning!", validationError.description, "warning");
        //                 }
        //                 //this.clearData();                        
        //             }
        //             else if (err.status === 400) {
        //                 if (err.url.indexOf('DailyRates/Post') == -1 && err.url.indexOf('marketplace-ops/assign-barcode') == -1) {
        //                     const validationError = err.error;
        //                     if (validationError.description != "Somebody altered the Estimation, Do want to overwrite?" && validationError.description.indexOf("Invalid Tag No") < 0) {
        //                         swal("Warning!", validationError.description, "warning");
        //                     }
        //                 }
        //             }
        //             else if (err.status === 500) {
        //                 const validationError = err.error;
        //                 swal("Warning!", validationError.description, "warning");
        //             }
        //         }
        //     )
        //#endregion


        //#region New Code for Refresh Token with retry failed request and common error handling logic
        return next.handle(req).catch(err => {
            if (err.status === 401) {
                if (err.error != null && err.error.Description == "Invalid login Credentials") {
                    swal("Warning!", err.error.Description, "warning");
                }
                else if (err.error != null && err.error.Description == "Invalid/expired refresh token.") {
                    this.clearData();
                }
                else {
                    return this.service.getRefreshToken().flatMap(
                        (data: any) => {
                            localStorage.setItem('Token', data.Credentials.Token);
                            localStorage.setItem('refreshToken', data.Credentials.RefreshToken);
                            req = req.clone({
                                setHeaders: {
                                    Authorization: "Bearer " + data.Credentials.Token,
                                }
                            });
                            return next.handle(req)
                        }
                    )
                }
            }
            else if (err.status === 403) {
                const validationError = err.error;
                swal("Warning!", validationError.description, "warning");
            }
            else if (err.status === 404) {
                const validationError = err.error;
                if (validationError.description != "Estimation No Does Not Exist" && validationError.description != "No payment details for the selected order.") {
                    swal("Warning!", validationError.description, "warning");
                }
            }
            else if (err.status === 400) {
                if (err.url.indexOf('DailyRates/Post') == -1 && err.url.indexOf('marketplace-ops/assign-barcode') == -1) {
                    const validationError = err.error;
                    if (validationError.description != "Somebody altered the Estimation, Do want to overwrite?" && validationError.description.indexOf("Invalid Tag No") < 0) {
                        swal("Warning!", validationError.description, "warning");
                    }
                }
            }
            else if (err.status === 500) {
                const validationError = err.error;
                swal("Warning!", validationError.description, "warning");
            }
            return Observable.throw(err);
        });
        //#endregion

    }

    clearData() {
        localStorage.clear();
        this._toastr.error("Session Timed Out", "");
        this._router.navigate(['/login']);
    }
}