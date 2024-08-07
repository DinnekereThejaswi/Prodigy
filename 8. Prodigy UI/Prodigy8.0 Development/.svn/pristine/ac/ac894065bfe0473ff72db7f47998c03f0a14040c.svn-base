import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as CryptoJS from 'crypto-js';
import { BehaviorSubject } from 'rxjs';
import { AppConfigService } from '../AppConfigService';

@Injectable({
	providedIn: 'root'
})
export class CreditReceiptService {

	ccode: string = "";
	bcode: string = "";
	password: string;
	apiBaseUrl: string;
	constructor(private _http: HttpClient, private appConfigService: AppConfigService) {
		this.apiBaseUrl = this.appConfigService.apiBaseUrl;
		this.password = this.appConfigService.Pwd;
		this.getCB();
	}

	getCB() {
		this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
		this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
	}
	getValues(BillNo, Year) {
		return this._http.get(this.apiBaseUrl + 'api/Credit/CreditReceipt/CreditBillDetails/' + this.ccode + '/' + this.bcode + '/' + Year + '/' + BillNo);
	}
	getReceiptValues(ReceiptNo) {
		return this._http.get(this.apiBaseUrl + 'api/Credit/CreditReceipt/CreditReceiptDetails/' + this.ccode + '/' + this.bcode + '/' + ReceiptNo);
	}
	getAccFinYear() {
		return this._http.get(this.apiBaseUrl + 'api/Masters/AccFinYear/' + this.ccode + '/' + this.bcode);
	}
	getReceiptValuesForPrint(ReceiptNo) {
		return this._http.get(this.apiBaseUrl + 'api/Credit/CreditReceipt/Print/' + this.ccode + '/' + this.bcode + '/' + ReceiptNo);
	}
	getShowroom() {
		return this._http.get(this.apiBaseUrl + 'api/master/companymaster/get/' + this.ccode + '/' + this.bcode);
	}
	postCancelReceipt(arg) {
		let body = JSON.stringify(arg);
		return this._http.post(this.apiBaseUrl + 'api/Credit/CreditReceipt/CancelCreditReceipt', body);
	}
	postCreditReceipt(year, billNo, arg) {
		let body = JSON.stringify(arg);
		return this._http.post(this.apiBaseUrl + 'api/Credit/CreditReceipt/Post/' + year + '/' + billNo, body);
	}
	public ReceiptNo: BehaviorSubject<string> = new BehaviorSubject<any>({});
	castReceiptNo = this.ReceiptNo.asObservable();

	SendReceiptNoToComp(arg) {
		this.ReceiptNo.next(arg);
		arg = '';
	}
}
