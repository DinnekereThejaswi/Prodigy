import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../AppConfigService';
import * as qz from 'qz-tray';
declare var qz: any;


@Injectable({
	providedIn: 'root'
})
export class OrdersService {

	ccode: string = "";
	bcode: string = "";
	password: string;
	permissionCode: string;
	apiBaseUrl: string;
	constructor(private _http: HttpClient, private appConfigService: AppConfigService) {
		this.apiBaseUrl = this.appConfigService.apiBaseUrl;
		this.password = this.appConfigService.Pwd;
		this.permissionCode = this.appConfigService.permissionCode;
		this.getCB();
	}
	getCB() {
		this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
		this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
	}

	getSalesManData() {
		return this._http.get(this.apiBaseUrl + 'api/Masters/Salesman/' + this.ccode + '/' + this.bcode);
	}
	getKarat() {
		return this._http.get(this.apiBaseUrl + 'api/Masters/Karat/' + this.ccode + '/' + this.bcode);
	}
	getGS() {
		return this._http.get(this.apiBaseUrl + 'api/order/OrderGSType/' + this.ccode + '/' + this.bcode);
	}
	getRateperGram(arg) {
		return this._http.get(this.apiBaseUrl + 'api/order/Rate?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&gsCode=' + arg.GS + '&karat=' + arg.Karat);
	}
	getPaymentMode() {
		return this._http.get(this.apiBaseUrl + 'api/order/PayMode/' + this.ccode + '/' + this.bcode);
	}
	getBank() {
		return this._http.get(this.apiBaseUrl + 'api/order/Bank/' + this.ccode + '/' + this.bcode);
	}
	getBarcodefromAPI(arg) {
		return this._http.get(this.apiBaseUrl + 'api/order/ReservedOrder/' + this.ccode + '/' + this.bcode + '/' + arg);
	}
	getCounter() {
		return this._http.get(this.apiBaseUrl + 'api/order/Counter/' + this.ccode + '/' + this.bcode);
	}
	getMCTypes() {
		return this._http.get(this.apiBaseUrl + 'api/masters/MCTypes/' + this.ccode + '/' + this.bcode);
	}
	getItemName(GS, CounterCode) {
		return this._http.get(this.apiBaseUrl + 'api/order/Items/' + this.ccode + '/' + this.bcode + '/' + CounterCode + '/' + GS);
	}
	get(arg) {
		return this._http.get(this.apiBaseUrl + 'api/order/Get/' + this.ccode + '/' + this.bcode + '/' + arg);
	}

	getOrderReceipt(arg) {
		return this._http.get(this.apiBaseUrl + 'api/order/CancelReceiptGet/' + this.ccode + '/' + this.bcode + '/' + arg);
	}

	getSearchParams() {
		return this._http.get(this.apiBaseUrl + 'api/order/SearchParams/' + this.ccode + '/' + this.bcode);
	}

	getAttachOrderList(searchType, searchValue) {
		if (searchType == "Customer") {
			searchValue = "'" + searchValue + "'";
			searchValue = searchValue.toString().replace(/"/g, "");
			return this._http.get(this.apiBaseUrl + 'api/order/AllOrders/' + this.ccode + '/' + this.bcode + '?$top=100&$skip=0&$orderby=OrderNo%20desc&$filter=startswith(' + searchType + ',' + searchValue + ')');
		}
		else {
			return this._http.get(this.apiBaseUrl + 'api/order/AllOrders/' + this.ccode + '/' + this.bcode + '?$top=100&$skip=0&$filter=' + searchType + ' eq ' + searchValue);
		}
	}

	getAllOrdersCount() {
		return this._http.get(this.apiBaseUrl + 'api/order/AllOrdersCount/' + this.ccode + '/' + this.bcode);
	}

	getOrderAttachedList(EstNo) {
		return this._http.get(this.apiBaseUrl + 'api/order/GetAttachedOrder/' + this.ccode + '/' + this.bcode + '/' + EstNo);
	}

	getAttachedList(EstNo) {
		return this._http.get(this.apiBaseUrl + 'api/order/GetAttachment/' + this.ccode + '/' + this.bcode + '/' + EstNo);
	}

	PostAttachementOrder(object) {
		var body = JSON.stringify(object);
		return this._http.post(this.apiBaseUrl + 'api/order/PostAttachment', body);
	}

	getShowroom() {
		return this._http.get(this.apiBaseUrl + 'api/Master/CompanyMaster/get/' + this.ccode + '/' + this.bcode);
	}

	getReceipt(arg) {
		return this._http.get(this.apiBaseUrl + 'api/order/ReceiptDet/' + this.ccode + '/' + this.bcode + '/' + arg);
	}

	postOrder(object) {
		var body = JSON.stringify(object);
		return this._http.post(this.apiBaseUrl + 'api/order/Post', body);
	}

	putOrder(object, OrderNo) {
		var body = JSON.stringify(object);
		return this._http.put(this.apiBaseUrl + 'api/order/Put?orderNo=' + OrderNo, body);
	}

	//GET Order Receipt Details
	getOrderReceiptDetails(orderno) {
		return this._http.get(this.apiBaseUrl + 'api/order/OrderReceiptGet/' + this.ccode + '/' + this.bcode + '/' + orderno);
	}

	deleteOrderAttachment(arg) {
		return this._http.get(this.apiBaseUrl + 'api/order/RemoveAttachment?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&estNo=' + arg);
	}

	postorderReceipt(object) {
		var body = JSON.stringify(object)
		return this._http.post(this.apiBaseUrl + 'api/order/OrderReceipt', body);
	}

	//Cancel Orders
	getCancelOrderView(arg) {
		return this._http.get(this.apiBaseUrl + 'api/order/GetCancelOrderView/' + this.ccode + '/' + this.bcode + '/' + arg);
	}

	cancelOrder(object) {
		var body = JSON.stringify(object);
		return this._http.post(this.apiBaseUrl + 'api/order/CancelOrder', body);
	}

	cancelReceipt(object) {
		var body = JSON.stringify(object);
		return this._http.post(this.apiBaseUrl + 'api/order/cancelReceipt', body);
	}

	viewOrder(object) {
		return this._http.get(this.apiBaseUrl + 'api/order/GetViewOrder/' + this.ccode + '/' + this.bcode + '/' + object);
	}

	// Reprint Orders
	// getPrintOrder(arg) {
	// 	return this._http.get(this.apiBaseUrl + 'api/order/GetOrderPrint/' + this.ccode + '/' + this.bcode + '/' + arg);
	// }

	getOrderPrint(orderNo, orderType) {
		return this._http.get(this.apiBaseUrl + 'api/order/OrderPrint/' + this.ccode + '/' + this.bcode + '/' + orderNo + '/' + orderType);
	}

	getOrderReceiptPrint(arg) {
		return this._http.get(this.apiBaseUrl + 'api/order/OrderReceiptPrint/' + this.ccode + '/' + this.bcode + '/' + arg);
	}

	getClosedOrderPrint(arg) {
		return this._http.get(this.apiBaseUrl + 'api/order/ClosedOrderPrint/' + this.ccode + '/' + this.bcode + '/' + arg);
	}


	// getPrintReceipt(arg) {
	// 	return this._http.get(this.apiBaseUrl + 'api/order/ReceiptDetPrint/' + this.ccode + '/' + this.bcode + '/' + arg);
	// }

	getApplicationDate() {
		return this._http.get(this.apiBaseUrl + 'api/Masters/GetApplicationDate/' + this.ccode + '/' + this.bcode);
	}

	getOrderNo(arg, isChecked) {
		return this._http.get(this.apiBaseUrl + 'api/order/OrderPrintOrders/' + this.ccode + '/' + this.bcode + '/' + arg + '/' + isChecked)
	}

	getReceiptNo(arg, isChecked) {
		return this._http.get(this.apiBaseUrl + 'api/order/OrderPrintReceiptDetails/' + this.ccode + '/' + this.bcode + '/' + arg + '/' + isChecked)
	}

	getClosedOrder(arg) {
		return this._http.get(this.apiBaseUrl + 'api/order/OrderPrintClosedOrders/' + this.ccode + '/' + this.bcode + '/' + arg);
	}

	getTotal(arg) {
		return this._http.get(this.apiBaseUrl + 'api/order/GetOrderReceiptTotalPrint/' + this.ccode + '/' + this.bcode + '/' + arg);
	}

	getClosedTotal(arg) {
		return this._http.get(this.apiBaseUrl + 'api/order/GetClosedOrderReceiptTotalPrint/' + this.ccode + '/' + this.bcode + '/' + arg);
	}

	getWeightTotal(arg) {
		return this._http.get(this.apiBaseUrl + 'api/order/GetOrderFormTotalPrint/' + this.ccode + '/' + this.bcode + '/' + arg);
	}
	getReceiptTotal(arg) {
		return this._http.get(this.apiBaseUrl + 'api/order/GetReceiptDetTotalPrint/' + this.ccode + '/' + this.bcode + '/' + arg);
	}

	getReceptDetByOrder(arg) {
		return this._http.get(this.apiBaseUrl + 'api/order/ReceptDetByOrder/' + this.ccode + '/' + this.bcode + '/' + arg);
	}

	//Close Order
	closeOrder(object) {
		var body = JSON.stringify(object);
		return this._http.post(this.apiBaseUrl + 'api/order/CloseOrder', body);
	}

	//attach Order List(Estimation);
	getOrderList(top, skip) {
		return this._http.get(this.apiBaseUrl + 'api/order/AllOrders/' + this.ccode + '/' + this.bcode + '?$top=' + top + '&$skip=' + skip + '&$orderby=OrderNo desc');
	}

	getGlobalOrderAmount(Estno) {
		return this._http.get(this.apiBaseUrl + 'api/order/GlobalOrderAmount/' + this.ccode + '/' + this.bcode + '/' + Estno);
	}

	getOrderItemType() {
		return this._http.get(this.apiBaseUrl + 'api/order/OrderItemType/' + this.ccode + '/' + this.bcode);
	}

	getPermission() {
		return this._http.get(this.apiBaseUrl + 'api/elevatedpermission/get?permissionCode=' + this.permissionCode);
	}

	uploadCustOrderImg(salCode, managerCode, file) {
		return this._http.put(this.apiBaseUrl + 'api/order/UploadOrderImage/' + this.ccode + '/' + this.bcode + '/' + salCode + '/' + managerCode, file);
	}

	downloadCustOrderImg(orderNo, slNo) {
		return this._http.get(this.apiBaseUrl + 'api/order/DownloadOrderImage/' + this.ccode + '/' + this.bcode + '/' + orderNo + '/' + slNo);
	}

	postelevatedpermission(arg) {
		const body = JSON.stringify(arg);
		return this._http.post(this.apiBaseUrl + 'api/elevatedpermission/post', body);
	}

	getOrderPrintHTML(orderNo) {
		return this._http.get(this.apiBaseUrl + 'api/order/GetOrderPrintDotMatrix/' + this.ccode + '/' + this.bcode + '/' + orderNo);
	}

	getOrderReceiptPrintHTML(ReceiptNo) {
		return this._http.get(this.apiBaseUrl + 'api/order/ReceiptDetPrintDotMatrix/' + this.ccode + '/' + this.bcode + '/' + ReceiptNo);
	}

	getClosedOrderPrintHTML(ReceiptNo) {
		return this._http.get(this.apiBaseUrl + 'api/order/ClosedOrderPrintDotMatrix/' + this.ccode + '/' + this.bcode + '/' + ReceiptNo);
	}


	getModifyOrder() {
		return this._http.get(this.apiBaseUrl + 'api/order/GetAllOrders/' + this.ccode + '/' + this.bcode);
	}

	getTopModifyOrder(top, skip) {
		return this._http.get(this.apiBaseUrl + 'api/order/GetAllOrders/' + this.ccode + '/' + this.bcode + '?$top=' + top + '&$skip=' + skip);
	}

	ValidateAttachOrder(estNo, orderNo) {
		return this._http.get(this.apiBaseUrl + 'api/order/ValAttachOrderVal/' + this.ccode + '/' + this.bcode + '/' + estNo + '/' + orderNo);
	}


	//Purchase Plan API

	getorderRateType(orderRateType) {
		return this._http.get(this.apiBaseUrl + 'api/order/PurchasePlan/' + this.ccode + '/' + this.bcode + '/' + orderRateType);
	}

	getAdvancePercentDetail(purchasePlanCode) {
		return this._http.get(this.apiBaseUrl + 'api/order/PurchasePlanDetail/' + this.ccode + '/' + this.bcode + '/' + purchasePlanCode);
	}

	//End of Purchase Plan


	public existingOrderDetails: BehaviorSubject<any> = new BehaviorSubject<any>({});
	castexistingOrderDetails = this.existingOrderDetails.asObservable();

	SendExistingOrderDetailsToSubComp(arg) {
		this.existingOrderDetails.next(arg);
		arg = '';
	}

	public OrderNo: BehaviorSubject<string> = new BehaviorSubject<string>(null);
	castOrderNoFromViewOrderComp = this.OrderNo.asObservable();

	SendOrderNoToComp(arg) {
		this.OrderNo.next(arg);
		arg = '';
	}

	public OrderNoToReprintComp: BehaviorSubject<any> = new BehaviorSubject<any>({});
	castOrderNoToReprintComp = this.OrderNoToReprintComp.asObservable();

	SendOrderNoToReprintComp(arg) {
		this.OrderNoToReprintComp.next(arg);
		arg = '';
	}


	public ReceiptNoToReprintComp: BehaviorSubject<any> = new BehaviorSubject<any>({});
	castReceiptNoToReprintComp = this.ReceiptNoToReprintComp.asObservable();

	SendReceiptNoToReprintComp(arg) {
		this.ReceiptNoToReprintComp.next(arg);
		arg = '';
	}


	public OrderDetsToOrderComp: BehaviorSubject<any> = new BehaviorSubject<any>({});
	castOrderDetsToOrderComp = this.OrderDetsToOrderComp.asObservable();

	SendOrderDetsToOrderComp(arg) {
		this.OrderDetsToOrderComp.next(arg);
		arg = '';
	}


	public ModifyOrderDetsToOrderComp: BehaviorSubject<string> = new BehaviorSubject<string>(null);
	castModifyOrderDetsToOrderComp = this.ModifyOrderDetsToOrderComp.asObservable();

	SendModifyOrderNoToItemComp(arg) {
		this.ModifyOrderDetsToOrderComp.next(arg);
		arg = '';
	}

	public ReservedOrderDetsToSalesComp: BehaviorSubject<any> = new BehaviorSubject<any>({});
	castReservedOrderDetsToSalesComp = this.ReservedOrderDetsToSalesComp.asObservable();

	SendReservedOrderDetsToSalesComp(arg) {
		this.ReservedOrderDetsToSalesComp.next(arg);
		arg = '';
	}


	//Dormant Order

	getDormantOrderList(date) {
		return this._http.get(this.apiBaseUrl + 'api/Dormant/list?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&date=' + date);
	}

	getDormantOrderbyOrderNo(orderNo) {
		return this._http.get(this.apiBaseUrl + 'api/Dormant/get?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&orderNo=' + orderNo);
	}

	getLockedOrderDetails(orderNo) {
		return this._http.get(this.apiBaseUrl + 'api/Dormant/get-locked-order?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&orderNo=' + orderNo);
	}

	lockDormantOrder(object) {
		var body = JSON.stringify(object);
		return this._http.post(this.apiBaseUrl + 'api/Dormant/Lock', body);
	}

	unlockDormantOrder(object) {
		var body = JSON.stringify(object);
		return this._http.post(this.apiBaseUrl + 'api/Dormant/UnLock', body);
	}

	//Ends here


	//Orders Issue to CPC

	getMetalsDetail() {
		return this._http.get(this.apiBaseUrl + 'api/order/cpc-issue/get-metals/' + this.ccode + '/' + this.bcode);
	}

	getGSDetail(metalCode) {
		return this._http.get(this.apiBaseUrl + 'api/order/cpc-issue/get-gs/' + this.ccode + '/' + this.bcode + '/' + metalCode);
	}

	getOrderIssueToCpcList(arg) {
		return this._http.get(this.apiBaseUrl + 'api/order/cpc-issue/get-order-list?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&orderType=' + arg.orderType + '&gsCode=' + arg.gsCode + '&counterCode=' + arg.counterCode + '&karat=' + arg.karat);
	}

	postOrderIssueToCPC(object) {
		var body = JSON.stringify(object);
		return this._http.post(this.apiBaseUrl + 'api/order/cpc-issue/post', body);
	}

	postOrderIssueToCPCXML(issueNo) {
		return this._http.post(this.apiBaseUrl + 'api/order/cpc-issue/generate-xml/' + this.ccode + '/' + this.bcode + '/' + issueNo, null);
	}

	//ends here
	public ReserevedOrderNoToSalesComp: BehaviorSubject<any> = new BehaviorSubject<any>({});
	castReservedOrderDetsAtSalesComp = this.ReserevedOrderNoToSalesComp.asObservable();

	SendReservedOrderNoToSalesComp(arg) {
		this.ReserevedOrderNoToSalesComp.next(arg);
		arg = '';
	}

}