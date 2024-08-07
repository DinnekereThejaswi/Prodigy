
import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppConfigService } from '../AppConfigService';
import * as CryptoJS from 'crypto-js';

@Injectable({
  providedIn: 'root'
})

export class MastersService {
  ccode: string;
  bcode: string;
  password: string;
  apiBaseUrl: string;
  permissionCode: string;
  constructor(private _http: HttpClient, private appConfigService: AppConfigService) {
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.password = this.appConfigService.Pwd;
    this.getCB();
    this.permissionCode = this.appConfigService.permissionCode;

  }
  ///get all branches
  getListOFip() {
    return this._http.get(this.apiBaseUrl + 'api/ip-address-mgmt/List');

  }
  getAllCompanybranchCodes(arg) {
    return this._http.get(this.apiBaseUrl + 'api/OperatorBranchMappings/Get?operatorCode=' + arg);
  }
  getBranchCompanys(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Masters/Operator/DefaultBranch/' + arg)
  }
  postNewIPData(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/ip-address-mgmt/Post', body);
  }
  serachApiData(arg1, arg2) {
    return this._http.get(this.apiBaseUrl + 'api/ip-address-mgmt/List?allowDeny=' + arg1 + '&status=' + arg2);

  }
  editIPdetails(arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/ip-address-mgmt/Put', body);
  }
  /////////
  ////////company details
  ///////////////////////////
  getcompany() {
    return this._http.get(this.apiBaseUrl + 'api/Master/CompanyMaster/Get/' + this.ccode + '/' + this.bcode);
  }
  EditCompanyDetails(objid, arg) {
    var body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Master/CompanyMaster/put/' + objid, body);

  }
  getStateCodes() {
    return this._http.get(this.apiBaseUrl + 'api/masters/state/list')
  }

  /////////////////////////////////////
  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }

  // salesman component
  getSearchSalesMan1(top, skip, object) {
    return this._http.get(this.apiBaseUrl + 'api/Master/SalesmanMaster/List/' + this.ccode + '/' + this.bcode + '?$top=' + top + '&$skip=' + skip + '&$filter=SalesManName eq' + "'" + object + "'");
  }

  getSearchSalesMan(searchValue) {
    return this._http.get(this.apiBaseUrl + 'api/Master/SalesmanMaster/List/' + this.ccode + '/' + this.bcode + '?$filter=substringof(' + "'" + searchValue + "',SalesManName ) eq true");
  }


  getSearchCount(Type) {
    return this._http.get(this.apiBaseUrl + 'searchByParamCount/' + Type + '/' + this.ccode + '/' + this.bcode);
  }


  GetSalesMen() {
    return this._http.get(this.apiBaseUrl + 'api/Master/SalesmanMaster/List/' + this.ccode + '/' + this.bcode);
  }
  PostSalesMen(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Master/SalesmanMaster/Post', body);
  }
  putSalesMen(code, arg,) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Master/SalesmanMaster/Put?code=' + code, body);
  }

  SalesManStatus(arg, SalCode) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Master/SalesmanMaster/OpenOrClose/' + this.ccode + '/' + this.bcode + '/' + SalCode, body);
  }
  GetTotalSalesManRecordCount() {
    return this._http.get(this.apiBaseUrl + 'api/Master/SalesmanMaster/RecordCount?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }
  GetTopValueSalesMen(top, skip) {
    return this._http.get(this.apiBaseUrl + 'api/Master/SalesmanMaster/List/' + this.ccode + '/' + this.bcode + '?$top=' + top + '&$skip=' + skip + '&$orderby=SalesManCode asc');
  }

  // end salesman component
  ///-------------for selling mc/ VA master----------------
  GSList() {

    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/GSNames/' + this.ccode + '/' + this.bcode);
  }
  Supplier() {

    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/SupplierNames/' + this.ccode + '/' + this.bcode);
  }
  GetItemList(gs) {

    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/itemnames/' + this.ccode + '/' + this.bcode + '/' + gs);
  }
  DesignName() {

    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/DesignNames/' + this.ccode + '/' + this.bcode);
  }
  getMCTypes() {

    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/MCTypes/' + this.ccode + '/' + this.bcode);
  }

  getFromWT(supplier, gs, item, design) {

    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/Fromwt/' + this.ccode + '/' + this.bcode + '/' + supplier + '/' + gs + '/' + item + '/' + design);
  }

  getTable(supplier, gs, item, design) {

    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/SellingMCMasterDetails/' + this.ccode + '/' + this.bcode + '/' + supplier + '/' + gs + '/' + item + '/' + design);
  }
  VAMasterPost(arg) {
    let body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Master/SellingMCMaster/Post', body);
  }

  GetVaRecordsCount(supplier, gs, item, design) {
    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/RecordsCount?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&supplierCode=' + supplier + '&gsCode=' + gs + '&itemCode=' + item + '&designCode=' + design);
  }

  GetVaRecordsCountBYSupplier(supplier) {
    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/RecordsCount?' + this.ccode + '/' + this.bcode + '/' + supplier);
  }
  GetVaRecordsCountBYGsItems(gs, item) {

    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/RecordsCount?' + this.ccode + '/' + this.bcode + '/' + gs + '/' + item);
  }
  GetTopVArecords(supplier, gs, item, top, skip) {
    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/SellingMCMasterDetails/' + this.ccode + '/' + this.bcode + '/' + supplier + '/' + gs + '/' + item + '?$top=' + top + '&$skip=' + skip);
  }
  GetAllRecordToPage(supplier, gs, item, design, top, skip) {
    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/SellingMCMasterDetails/' + this.ccode + '/' + this.bcode + '/' + supplier + '/' + gs + '/' + item + '/' + design + '?$top=' + top + '&$skip=' + skip);
  }
  GetVAREcordCountBySGID(supplier, gs, item, design) {
    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/RecordsCount/' + this.ccode + '/' + this.bcode + '/' + supplier + '/' + gs + '/' + item + '/' + design);
  }

  GetVARecordToModelByGSAndItem(gs, item, top, skip) {
    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/SellingMCMasterDetailsByGSAndItem/' + this.ccode + '/' + this.bcode + '/' + gs + '/' + item + '?$top=' + top + '&$skip=' + skip);
  }
  GetVAREcordCountByGI(gs, item) {
    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/RecordsCount/' + this.ccode + '/' + this.bcode + '/' + gs + '/' + item);
  }

  printVADetail(arg) {
    let body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/VAMaster/Print', body);

  }
  //model gs and items
  getVaByGsAndItems(gs, item) {
    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/SellingMCMasterDetailsByGSAndItem/' + this.ccode + '/' + this.bcode + '/' + gs + '/' + item);
  }

  GetVARecordOfGsAndItemToModel(gs, item, top, skip) {

    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/SellingMCMasterDetailsByGSAndItem/' + this.ccode + '/' + this.bcode + '/' + gs + '/' + item + '?$top=' + top + '&$skip=' + skip);
  }

  GetVATotalRecordOfGsAndItemToModel(gs, item) {
    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/RecordsCountByGs/' + this.ccode + '/' + this.bcode + '/' + gs + '/' + item);
  }
  ///BY SUPPLIER

  getVaBySupplier(supplier) {
    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/SellingMCMasterDetails/' + this.ccode + '/' + this.bcode + '/' + supplier);
  }

  GetVATotalRecordOfSupplier(supplier, top, skip) {
    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/SellingMCMasterDetails/' + this.ccode + '/' + this.bcode + '/' + supplier + '?$top=' + top + '&$skip=' + skip);
  }
  GetVATotalRecordOfSupplierToModel(supplier) {
    return this._http.get(this.apiBaseUrl + 'api/Master/SellingMCMaster/RecordsCount/' + this.ccode + '/' + this.bcode + '/' + supplier);
  }
  copySupplierToSupplier(arg) {
    let body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/VAMaster/Copy', body);
  }

  ///daily rate master

  getPermission() {
    return this._http.get(this.apiBaseUrl + 'api/elevatedpermission/get?permissionCode=' + this.permissionCode);
  }

  GetDailyRate() {
    return this._http.get(this.apiBaseUrl + 'api/Master/DailyRates/Get?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }


  postDailyRate(arg, performDayend) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Master/DailyRates/Post/' + performDayend, body);
  }



  //////////////////////////////////////////////////////////////////////////////////////
  ///KARAT MASTER
  /////////////////


  GETKaratsToTable() {
    return this._http.get(this.apiBaseUrl + 'api/Master/KaratMaster/List');
  }
  PostKarat(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Master/KaratMaster/Post', body);
  }

  KaratStatus(arg, ObjId) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Master/KaratMaster/OpenOrClose/' + ObjId, body);
  }


  ////////////////////////////////////////////////////////////////////////////////
  ///////COUNTER MASTER and MAIN LOCATION
  //////////////////
  getMaincounters() {
    return this._http.get(this.apiBaseUrl + 'api/Masters/MainLocation/List/' + this.ccode + '/' + this.bcode);
  }


  PostLocation(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Masters/MainLocation/Post', body);
  }
  GETCounterDetailsByCode(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Master/CounterMaster/ListByMainCounter/' + arg + '/' + this.ccode + '/' + this.bcode);
  }
  getCounterList() {
    return this._http.get(this.apiBaseUrl + 'api/Master/CounterMaster/List/' + this.ccode + '/' + this.bcode);
  }
  PostDetails(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Master/CounterMaster/Post', body);
  }
  ModifyOrUpdateLocationByPUT(obj, arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Masters/MainLocation/Put/' + obj, body);
  }
  ModifyOrUpdateCounterByPUT(obj, arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Master/CounterMaster/Put/' + obj, body);
  }


  //////////////////////////////////////////////////////////////
  ////////////////Item size
  ///////////////////////

  getItemList() {
    return this._http.get(this.apiBaseUrl + 'api/Master/ItemSize/List/BG');
  }


  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ////////////////Offer Discount
  /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  getOfferOptionList() {
    return this._http.get(this.apiBaseUrl + 'api/Masters/Discount/Get?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }

  PostOfferOptions(arg) {
    const body = JSON.stringify(arg);

    return this._http.post(this.apiBaseUrl + 'api/Masters/Discount/Post', body);
  }

  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ////////////////Stock Group
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  getStockGroup() {
    return this._http.get(this.apiBaseUrl + 'api/Master/StockGroup/List/' + this.ccode + '/' + this.bcode)

  }
  StockStatus(obj, arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Master/StockGroup/Put/' + obj, body);
  }

  PostStock(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Master/StockGroup/Post', body);
  }
  ModifyStock(obj, arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Master/StockGroup/Put/' + obj, body);
  }
  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ////////////////GS ENtry
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////





  GetGSListItems() {
    return this._http.get(this.apiBaseUrl + 'api/Master/GSItemEntryMaster/List/' + this.ccode + '/' + this.bcode)
  }

  GetGoodsType() {
    return this._http.get(this.apiBaseUrl + 'api/Master/GSItemEntryMaster/GoodsType?companyCode=' + this.ccode + '&branchCode=' + this.bcode);

  }

  getServiceType() {
    return this._http.get(this.apiBaseUrl + 'api/Master/GSItemEntryMaster/ServiceType?companyCode=' + this.ccode + '&branchCode=' + this.bcode);

  }
  getHSN() {
    return this._http.get(this.apiBaseUrl + 'api/Masters/HSN/List/' + this.ccode + '/' + this.bcode);

  }
  PostGsEntry(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Master/GSItemEntryMaster/Post', body);
  }
  modifyGSList(levelid, arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Master/GSItemEntryMaster/Put/' + levelid, body);
  }
  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ////////////////Product Tree
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  GetAllItemsList() {
    return this._http.get(this.apiBaseUrl + 'api/Masters/ProductTree/Get/' + this.ccode + '/' + this.bcode);
  }
  PostCategoryItem(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Masters/ProductTree/Post', body);
  }
  ModifyCategoryByPut(objid, arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Masters/ProductTree/Put/' + objid, body);
  }
  ModifyItemyByPut(objid, arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Masters/ProductTree/Put/' + objid, body);
  }
  DeleteCategoryByPostBody(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Masters/ProductTree/Delete', body);
  }

  editGsAtProductTree(Level1Id) {


    return this._http.get(this.apiBaseUrl + 'api/Master/GSItemEntryMaster/GSDet?id=' + Level1Id + '&companyCode=' + this.ccode + '&branchCode=' + this.bcode);

  }
  /////sending data from peoduct tree to gs entry ui to modify the contenet



  getGSObjId(arg) {
    return this._http.post(0 + '0', arg);
  }

  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  //////////////// GST MASter
  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  getGSTList() {
    return this._http.get(this.apiBaseUrl + 'api/Master/GSTMaster/List?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }
  PostGSTList(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Master/GSTMaster/Post', body);
  }
  putGSTListData(code, arg,) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Master/GSTMaster/Put/' + code, body);
  }
  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  //////////////// HSN MASter
  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  getHSNMaster() {
    return this._http.get(this.apiBaseUrl + 'api/Masters/HSN/List/' + this.ccode + '/' + this.bcode);
  }
  PostHSNList(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Masters/HSN/Post', body);
  }
  putHSNListData(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Masters/HSN/Put', body);
  }

  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  //////////////// GST POsting
  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  getApplicationDate() {
    return this._http.get(this.apiBaseUrl + 'api/Masters/GetApplicationDate/' + this.ccode + '/' + this.bcode);
  }
  getGSTPostingSetup() {
    return this._http.get(this.apiBaseUrl + 'api/Master/GSTPostingSetUp/List/' + this.ccode + '/' + this.bcode);
  }

  getAcTypes() {
    return this._http.get(this.apiBaseUrl + 'api/Master/GSTPostingSetUp/InputOutputAccount/' + this.ccode + '/' + this.bcode);
  }
  deleteGSTpostingSetup(ID) {
    return this._http.delete(this.apiBaseUrl + 'api/Master/GSTPostingSetUp/Delete/' + ID);
  }
  getGstGroupCode() {
    return this._http.get(this.apiBaseUrl + 'api/Master/GSTPostingSetUp/GroupCode/' + this.ccode + '/' + this.bcode);
  }
  getComponentCodeAndCalculationOrder() {
    return this._http.get(this.apiBaseUrl + 'api/Views/ListofGSTComponents');
  }
  getInputAndOutputRecords() {
    return this._http.get(this.apiBaseUrl + 'api/Views/ListOfAccLedgerDetails');
  }
  getDetailsFromID(ID) {
    return this._http.get(this.apiBaseUrl + 'api/Master/GSTPostingSetUp/Get/' + ID);

  }
  putGstPosting(ID, arg) {
    var body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Master/GSTPostingSetUp/Put/' + ID, body);
  }
  postGSTPosting(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Master/GSTPostingSetUp/Post', body);
  }
  deleteGstPost(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/GSTPostingSetUp/Delete', body);
  }


  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ////////////////Packing material
  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  getPackingData() {
    return this._http.get(this.apiBaseUrl + 'api/Masters/PackingMaterial/List/' + this.ccode + '/' + this.bcode);
  }
  getHLWuom() {
    return this._http.get(this.apiBaseUrl + 'api/Masters/PackingMaterial/HLW');
  }
  getWeightuom() {
    return this._http.get(this.apiBaseUrl + 'api/Masters/PackingMaterial/Weight');
  }
  PostPacking(arg) {
    return this._http.post(this.apiBaseUrl + 'api/Masters/PackingMaterial/Post', arg);
  }
  putPacking(objID, arg) {
    return this._http.put(this.apiBaseUrl + 'api/Masters/PackingMaterial/Put/' + objID, arg);
  }

  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ////////////////  Order fixed type
  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  getfixedOrders(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Masters/OrderFixedType/OrderTypesDetails/' + this.ccode + '/' + this.bcode + '/' + arg);
  }
  planNames() {
    return this._http.get(this.apiBaseUrl + 'api/Masters/OrderFixedType/PlanNames/' + this.ccode + '/' + this.bcode);
  }
  PostFixedOrder(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Masters/OrderFixedType/Post', body);
  }
  putFixedOrder(ID, arg) {
    return this._http.put(this.apiBaseUrl + 'api/Masters/OrderFixedType/Put/' + ID, arg);
  }
  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ////////////////  Order fixed
  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  getTypesRate() {
    return this._http.get(this.apiBaseUrl + 'api/Masters/FixedOrder/RateType/' + this.ccode + '/' + this.bcode);
  }
  getLedgerTypes() {
    return this._http.get(this.apiBaseUrl + 'api/Masters/FixedOrder/AccountLedger/' + this.ccode + '/' + this.bcode);
  }
  getFixedOrderList() {
    return this._http.get(this.apiBaseUrl + 'api/Masters/FixedOrder/List/' + this.ccode + '/' + this.bcode);
  }
  PostFixedOrders(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Masters/FixedOrder/Post', body);
  }
  putOrderFixedDetails(objID, arg) {
    return this._http.put(this.apiBaseUrl + 'api/Masters/FixedOrder/Put?objID=' + objID, arg);
  }

  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ////////////////  sku master
  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  getDesginBygsCode(arg) {
    return this._http.get(this.apiBaseUrl + 'api/SKU/Item/' + this.ccode + '/' + this.bcode + '/' + arg);
  }
  getDesginByItemCode(gscode, itemcode) {
    return this._http.get(this.apiBaseUrl + 'api/SKU/Design/' + this.ccode + '/' + this.bcode + '/' + gscode + '/' + itemcode);
  }
  getSKUList() {
    return this._http.get(this.apiBaseUrl + 'api/SKU/SKUDet/' + this.ccode + '/' + this.bcode);
  }
  PostSKU(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/SKU/Post', body);
  }

  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ////////////////  ROL master
  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  getROLGSitems() {
    return this._http.get(this.apiBaseUrl + '/api/ROL/GS/' + this.ccode + '/' + this.bcode);
  }

  getItemsByGs(arg) {
    return this._http.get(this.apiBaseUrl + 'api/ROL/Item/' + this.ccode + '/' + this.bcode + '/' + arg);
  }
  RolDesignName() {
    return this._http.get(this.apiBaseUrl + 'api/ROL/Design/' + this.ccode + '/' + this.bcode);
  }
  getRolCounter(GS, item) {
    return this._http.get(this.apiBaseUrl + 'api/ROL/Counter/' + this.ccode + '/' + this.bcode + '/' + GS + '/' + item);

  }
  getRolStock(GS, item) {
    return this._http.get(this.apiBaseUrl + 'api/ROL/Stock/' + this.ccode + '/' + this.bcode + '/' + GS + '/' + item);
  }
  PostRolrders(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/ROL/Stock', body);
  }
  putRolStock(arg) {
    return this._http.put(this.apiBaseUrl + 'api/ROL/Put', arg);
  }
  DeleteRolStock(objid) {
    return this._http.delete(this.apiBaseUrl + 'api/ROL/Delete/' + this.ccode + '/' + this.bcode + '/' + objid);
  }
  printRolStock(GS, item) {
    return this._http.get(this.apiBaseUrl + 'api/ROL/Print/' + this.ccode + '/' + this.bcode + '/' + GS + '/' + item);
  }

  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  //////////////// IR master
  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  loadIRCodes() {
    return this._http.get(this.apiBaseUrl + 'api/IRSetup/IRTypes/' + this.ccode + '/' + this.bcode);
  }
  loadIRListData(arg) {
    return this._http.get(this.apiBaseUrl + 'api/IRSetup/IRSetup/' + this.ccode + '/' + this.bcode + '/' + arg);
  }
  PostIRListData(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/IRSetup/Post', body);
  }
  putIRListData(objID, arg) {
    return this._http.put(this.apiBaseUrl + 'api/IRSetup/Put?objID=' + objID, arg);
  }


  //Branch master
  getBrnachGSTypeList() {
    return this._http.get(this.apiBaseUrl + 'api/Vendor/GSType/' + this.ccode + '/' + this.bcode);
  }
  GetBrnachOpenTypeList() {
    return this._http.get(this.apiBaseUrl + 'api/Vendor/OpenType/' + this.ccode + '/' + this.bcode);
  }
  getStateCode() {
    return this._http.get(this.apiBaseUrl + 'api/masters/state/list')
  }
  GetBranchTDS() {
    return this._http.get(this.apiBaseUrl + 'api/Vendor/TDS/' + this.ccode + '/' + this.bcode);
  }
  //////////////////////////////card charges
  getAccName() {
    return this._http.get(this.apiBaseUrl + 'api/CardCharges/AccountName/' + this.ccode + '/' + this.bcode);
  }

  getBankName() {
    return this._http.get(this.apiBaseUrl + 'api/CardCharges/BankName/' + this.ccode + '/' + this.bcode);
  }
  getCardChargesList() {
    return this._http.get(this.apiBaseUrl + 'api/CardCharges/CardCharges/' + this.ccode + '/' + this.bcode);
  }
  PostCardCharges(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/CardCharges/Post', body);
  }
  putCardCharges(objId, arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/CardCharges/Put/' + objId, body);
  }
  deletCardCharges(objId) {
    return this._http.delete(this.apiBaseUrl + 'api/CardCharges/Delete/' + objId + '/' + this.ccode + '/' + this.bcode);
  }
  //////////////////stone master
  getListOfStonesByType(stoneGs, type) {
    return this._http.get(this.apiBaseUrl + 'api/Master/StoneMaster/List/' + stoneGs + '/' + type);
  }
  PostStoneDetails(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/CardCharges/Post', body);
  }


  ////////////////////stone master and stone master new
  getStoneMasterNewLIst(stoneGs, type) {
    return this._http.get(this.apiBaseUrl + 'api/Master/StoneMasterNew/List/' + this.ccode + '/' + this.bcode + '/' + stoneGs + '/' + type);
  }
  PostStoneDetailsNew(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Master/StoneMasterNew/Post', body);
  }

  EditStoneDetailsNew(arg, type, stonename) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Master/StoneMasterNew/put/' + this.ccode + '/' + this.bcode + '/' + type + '/' + stonename, body);
  }

  ActivateStone(type, stonename) {

    return this._http.put(this.apiBaseUrl + 'api/Master/StoneMasterNew/Activate/' + this.ccode + '/' + this.bcode + '/' + type + '/' + stonename, null);
  }
  DeactivateStone(type, stonename) {

    return this._http.put(this.apiBaseUrl + 'api/Master/StoneMasterNew/Deactivate/' + this.ccode + '/' + this.bcode + '/' + type + '/' + stonename, null);
  }
  ////TDS
  getTDSListData() {
    return this._http.get(this.apiBaseUrl + 'api/Master/TDSMaster/List');
  }
  PostTDS(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Master/TDSMaster/Post', body);
  }
  putTDS(objId, arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Master/TDSMaster/Put/' + objId, body);
  }
  DeactivateTDS(arg) {
    return this._http.post(this.apiBaseUrl + 'api/Master/TDSMaster/OpenOrClose/' + arg, null);
  }
  ActivateTDS(arg) {
    return this._http.post(this.apiBaseUrl + 'api/Master/TDSMaster/OpenOrClose/' + arg, null);
  }

  ////////////////////TCS Master
  GetAccName() {
    return this._http.get(this.apiBaseUrl + 'api/TCS/AccountName/' + this.ccode + '/' + this.bcode);
  }

  getKYCTypes() {
    return this._http.get(this.apiBaseUrl + 'api/TCS/KYC/' + this.ccode + '/' + this.bcode);
  }

  getTranType() {
    return this._http.get(this.apiBaseUrl + 'api/TCS/TransType/' + this.ccode + '/' + this.bcode);
  }
  calculatedOn() {
    return this._http.get(this.apiBaseUrl + 'api/TCS/CalOn/' + this.ccode + '/' + this.bcode);
  }

  ////////////////////Stone Rate Master
  getStoneRates() {
    return this._http.get(this.apiBaseUrl + 'api/Master/StoneRateMaster/List/' + this.ccode + '/' + this.bcode);
  }

  ////////////////////design  Master
  getdesigns(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Master/DesignMaster/List/' + arg + '/' + this.ccode + '/' + this.bcode);
  }
  PostNewDesign(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Master/DesignMaster/Post', body);
  }
  EditDesignDetails(objID, arg) {
    return this._http.put(this.apiBaseUrl + 'api/Master/DesignMaster/Put/' + objID, arg);
  }
  OpenDesign(objID) {
    return this._http.post(this.apiBaseUrl + 'api/Master/DesignMaster/OpenOrClose/' + objID + '/' + this.ccode + '/' + this.bcode, null);
  }
  CloseDeign(objID) {
    return this._http.post(this.apiBaseUrl + 'api/Master/DesignMaster/OpenOrClose/' + objID + '/' + this.ccode + '/' + this.bcode, null);
  }
  //branch master
  GetTDS() {
    return this._http.get(this.apiBaseUrl + 'api/Vendor/TDS/' + this.ccode + '/' + this.bcode);
  }

  ///Paymenyt Types master
  getPaymentTypes() {
    return this._http.get(this.apiBaseUrl + 'api/Masters/PaymentType/list?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }
  AddPaymentType(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Masters/PaymentType/post?companyCode=' + this.ccode + '&branchCode=' + this.bcode, body);
  }
  editPaymentType(code, arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Masters/PaymentType/put?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&paymentCode=' + code, body);
  }
  deletePaymentType(code) {
    return this._http.post(this.apiBaseUrl + 'api/Masters/PaymentType/delete?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&paymentCode=' + code, null);
  }
}

