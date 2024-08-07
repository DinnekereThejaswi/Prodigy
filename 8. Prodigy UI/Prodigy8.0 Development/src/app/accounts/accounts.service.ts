import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../AppConfigService';

@Injectable({
  providedIn: 'root'
})
export class AccountsService {
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

  // cashVoucher Entry //

  GetMasterLedger() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/CashVoucherEntry/MasterLedger/' + this.ccode + '/' + this.bcode);
  }

  gettype() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/CashVoucherEntry/TransactionType');
  }

  updateAccounts(appDate) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/AccountsUpdate/Update?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&startDate=' + appDate + '&endDate=' + appDate);
  }

  GetAccountNames(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/CashVoucherEntry/AccountName/' + this.ccode + '/' + this.bcode + '/' + arg);
  }

  GetContraAccountNames() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/ContraEntry/AccountName/' + this.ccode + '/' + this.bcode );
  }
  GetNarration() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/CashVoucherEntry/Narration/' + this.ccode + '/' + this.bcode);
  }

  getApplicationDate() {
    return this._http.get(this.apiBaseUrl + 'api/Masters/GetApplicationDate/' + this.ccode + '/' + this.bcode);
  }

  getCashVoucherTable(TransactionType, MasterLedger, date) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/CashVoucherEntry/VoucherDetails/' + this.ccode + '/' + this.bcode + '/' + TransactionType + '/' + MasterLedger + '/' + date);
  }

  editGet(voucherNo, accCode, accCodeMaster) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/CashVoucherEntry/Voucher/' + this.ccode + '/' + this.bcode + '/' + voucherNo + '/' + accCode + '/' + accCodeMaster);
  }

  getCashVoucherForCancel(voucherNo, accCodeMaster, TransType) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/CashVoucherEntry/Get/' + this.ccode + '/' + this.bcode + '/' + voucherNo + '/' + accCodeMaster + '/' + TransType);
  }

  PostCashVoucher(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Accounts/CashVoucherEntry/Post', body);
  }

  PutCashVoucher(arg, voucherNo) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Accounts/CashVoucherEntry/Put?voucherNo=' + voucherNo, body);
  }
  deleteCashVoucherDet(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Accounts/CashVoucherEntry/Delete', body);
  }
  print(voucherNo, accCodeMaster, transType, accType, tranType) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/CashVoucherEntry/Print/' + this.ccode + '/' + this.bcode + '/' + voucherNo + '/' + accCodeMaster + '/' + transType + '/' + accType + '?tranType=' + tranType);
  }

  //  end of cashvoucherEntry //

  getMasterGroupType() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/MasterGroup/GroupType/' + this.ccode + '/' + this.bcode);
  }

  getMasterGroupList() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/MasterGroup/List/' + this.ccode + '/' + this.bcode);
  }

  postMasterGroup(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Accounts/MasterGroup/Post', body);
  }

  putMasterGroup(arg, objID) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Accounts/MasterGroup/Put?objID=' + objID, body);
  }

  getSubGroup() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/SubGroup/ParentGroup?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }

  getSubGroupList() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/SubGroup/List?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }

  postSubGroup(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Accounts/SubGroup/Post', body);
  }

  putSubGroup(arg, objID) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Accounts/SubGroup/Put?objID=' + objID, body);
  }

  getNarrationList() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/Narration/List?companyCode=' + this.ccode + '&branchCode=' + this.bcode);
  }

  postNarration(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Accounts/Narration/Post', body);
  }

  deleteNarration(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Accounts/Narration/Delete', body);
  }

  getMasterLedgerList() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/ContraEntry/MasterLedger/' + this.ccode + '/' + this.bcode + '/B');
  }

  getContraPrintList(appdate) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/ContraEntry/PrintList/' + this.ccode + '/' + this.bcode + '/' + appdate);
  }

  getPaymentType() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/BankVoucherEntry/Type');
  }

  getBankVoucherEntryList(accCode, type, appdate) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/BankVoucherEntry/List/' + this.ccode + '/' + this.bcode + '/' + accCode + '/' + type + '/' + appdate);
  }

  getBankVoucherDetails(arg, transType) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/BankVoucherEntry/Voucher?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&voucherNo=' + arg.VoucherNo + '&accCodeMaster=' + arg.AccCodeMaster + '&tranType=' + transType);
  }

  getBankVoucherDetailsForCancel(voucherNo, accCodeMaster, transType) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/BankVoucherEntry/Voucher?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&voucherNo=' + voucherNo + '&accCodeMaster=' + accCodeMaster + '&tranType=' + transType);
  }

  getBankVoucherPrintList(transtype, appdate) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/BankVoucherEntry/PrintList/' + this.ccode + '/' + this.bcode + '/' + transtype + '/' + appdate);
  }

  postBankVoucherEntry(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Accounts/BankVoucherEntry/Post', body);
  }

  putBankVoucherEntry(arg) {5
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Accounts/BankVoucherEntry/Put?voucherNo=' + arg[0].VoucherNo, body);
  }

  deleteBankVoucherEntry(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Accounts/BankVoucherEntry/Delete', body);
  }

  printBankVoucherEntry(voucherNo, accCode, transType, accType) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/BankVoucherEntry/Print?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&voucherNo=' + voucherNo + '&accCodeMaster=' + accCode + '&tranType=' + transType + '&accType=' + accType);
  }

  getAccountNameforJournalEntry() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/JournalEntry/AccountName/' + this.ccode + '/' + this.bcode);
  }

  getJournalEntryDetails(voucherNo) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/JournalEntry/Get/' + this.ccode + '/' + this.bcode + '/' + voucherNo);
  }

  postJournalEntry(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Accounts/JournalEntry/Post', body);
  }

  cancelJournalEntry(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Accounts/JournalEntry/Delete', body);
  }
  printJournalEntrys(VoucherNo,
    AccouuntCodeMaster, TranTypes,
    AccType) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/JournalEntry/Print?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&voucherNo=' + VoucherNo + '&accCodeMaster=' + AccouuntCodeMaster + '&tranType=' + TranTypes + '&accType=' + AccType);
  }
  printJournalEntry(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/JournalEntry/Print?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&voucherNo=' + arg.VoucherNo + '&accCodeMaster=' + arg.AccountCodeMaster + '&tranType=' + arg.TransType + '&accType=' + arg.AccountType);
  }

  getContraDetails(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/ContraEntry/Voucher/' + this.ccode + '/' + this.bcode + '/' + arg + '/0/0');
  }

  getCashType() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/ContraEntry/MasterLedger/' + this.ccode + '/' + this.bcode + '/C');
  }

  cancelContraEntry(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Accounts/ContraEntry/Delete', body);
  }

  //Cheque Entry

  getChequelist() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/Cheque/List/' + this.ccode + '/' + this.bcode);
  }

  postCheque(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Accounts/Cheque/Post', body);
  }

  DeleteCheque(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Accounts/Cheque/Delete', body);
  }

  getChequeClosinglist(accCode) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/Cheque/ChequeList/' + this.ccode + '/' + this.bcode + '/' + accCode);
  }

  postOpenCloseCheque(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Accounts/Cheque/OpenClose', body);
  }

  getCashInHand(appDate) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/CashInHand/Get?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&date=' + appDate);
  }

  postCashInHand(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Accounts/CashInHand/Post', body);
  }

  // printContraEntry(voucherNo, accCodeMaster, transType, accType, tranType){
  //   return this._http.get(this.apiBaseUrl + 'api/Accounts/ContraEntry/Print/'+this.ccode+ '/'+ this.bcode + '/' +voucherNo+ '/'+accCodeMaster+ '/'+ transType + '/'  +accType+ '?tranType='+ tranType);
  // }
  getPrintListJournalEntry(appDate) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/JournalEntry/PrintList/' + this.ccode + '/' + this.bcode + '/' + appDate);
  }

  reprintJournalEntry(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/JournalEntry/Print?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&voucherNo=' + arg[0].VoucherNo + '&accCodeMaster=' + arg[0].AccountCodeMaster + '&tranType=' + arg[0].TransType + '&accType=' + arg[0].AccountType);
  }

  //Contra API's Start

  getContraType() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/ContraEntry/Type/' + this.ccode + '/' + this.bcode);
  }

  getContraLedger(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/ContraEntry/MasterLedger/' + this.ccode + '/' + this.bcode + '/' + arg);
  }

  getContraEntryList(MasterLedger, date) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/ContraEntry/list/' + this.ccode + '/' + this.bcode + '/' + MasterLedger + '/' + date);
  }

  editGetContraEntry(voucherNo, accCode, accCodeMaster) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/ContraEntry/Voucher/' + this.ccode + '/' + this.bcode + '/' + voucherNo + '/' + accCode + '/' + accCodeMaster);
  }

  PutContraVoucher(arg, voucherNo) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Accounts/ContraEntry/Put?voucherNo=' + voucherNo, body);
  }

  PostContraVoucher(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Accounts/ContraEntry/Post', body);
  }

  printContraEntry(voucherNo, accCodeMaster, transType, accType, tranType) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/ContraEntry/Print/' + this.ccode + '/' + this.bcode + '/' + voucherNo + '/' + accCodeMaster + '/' + transType + '/' + accType + '?tranType=' + tranType);
  }

  //Contra API's End


  //Ledger

  getLedgerList() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/Ledger/List/' + this.ccode + '/' + this.bcode);
  }

  getLedgerType() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/Ledger/LedgerType/' + this.ccode + '/' + this.bcode);
  }

  getLedgerSubGroup() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/Ledger/SubGroup/' + this.ccode + '/' + this.bcode);
  }

  getLedgerTransactionType() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/Ledger/TransactionType/' + this.ccode + '/' + this.bcode);
  }

  getLedgerVType() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/Ledger/VType/' + this.ccode + '/' + this.bcode);
  }

  getGSTGroup() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/Ledger/GSTGroupCode/' + this.ccode + '/' + this.bcode);
  }

  getGSTServiceGroup() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/Ledger/GSTServiceGroupCode/' + this.ccode + '/' + this.bcode);
  }

  getHSNbyGST(gstGroupCode) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/Ledger/HSNByGSTGroupCode/' + this.ccode + '/' + this.bcode + '/' + gstGroupCode);
  }

  getGroup(subGroupID) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/Ledger/Group/' + this.ccode + '/' + this.bcode + '/' + subGroupID);
  }

  getLedgerDetailsByID(AccCode) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/Ledger/LedgerDetail/' + this.ccode + '/' + this.bcode + '/' + AccCode);
  }

  PostLedger(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Accounts/Ledger/Post', body);
  }

  PutLedger(objID, arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/Accounts/Ledger/Put?objID=' + objID, body);
  }

  PrintLedger() {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/Ledger/LedgerDetPrint/' + this.ccode + '/' + this.bcode);
  }

  ////cashabck
  cashbackList(billno) {
    return this._http.get(this.apiBaseUrl + 'api/Accounts/CashBack/Get?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&billNo=' + billno);
  }
  PostCashbackDetails(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/Accounts/CashBack/Post', body);

  }

  // account-posting-setup api's

  getAll() {
    return this._http.get(this.apiBaseUrl + 'api/AccountPostingSetup/AllInOne/' + this.ccode + '/' + this.bcode);
  }

  getGS() {
    return this._http.get(this.apiBaseUrl + 'api/AccountPostingSetup/GSCodes/' + this.ccode + '/' + this.bcode);
  }

  getPayMode() {
    return this._http.get(this.apiBaseUrl + 'api/AccountPostingSetup/PayMode/' + this.ccode + '/' + this.bcode);
  }

  getTransactionTypes() {
    return this._http.get(this.apiBaseUrl + 'api/AccountPostingSetup/TransactionTypes/' + this.ccode + '/' + this.bcode);
  }

  getLedger(Type) {
    return this._http.get(this.apiBaseUrl + 'api/AccountPostingSetup/Ledger/' + this.ccode + '/' + this.bcode + '/' + Type);
  }

  postTransactionPosting(object) {
    var body = JSON.stringify(object)
    return this._http.post(this.apiBaseUrl + 'api/AccountPostingSetup/post', body);
  }

  //Expense voucher cancel

  getExpenseVoucherCancel(voucherNo) {
    return this._http.get(this.apiBaseUrl + 'api/ExpenseVoucherCancel/Voucher?companyCode=' + this.ccode + '&branchCode=' + this.bcode + '&voucherNo=' + voucherNo);
  }

  getVendorList(ledgerType) {
    return this._http.get(this.apiBaseUrl + 'api/ExpanseVoucherEntry/Vendor/' + this.ccode + '/' + this.bcode + '/' + ledgerType);
  }

  postExpenseVoucherCancel(object) {
    var body = JSON.stringify(object)
    return this._http.post(this.apiBaseUrl + 'api/ExpenseVoucherCancel/Post', body);
  }
  //////////////////////////////////////////////////////////////////////////////////////
  ///acc code settings
  //////////////////////////////////////////////////////////////////////////////////////
  getAccSettingData() {
    return this._http.get(this.apiBaseUrl + '/api/AccountCodeSettings/List/' + this.ccode + '/' + this.bcode);
  }
  getAccNamesList() {
    return this._http.get(this.apiBaseUrl + '/api/AccountCodeSettings/AccountLedger/' + this.ccode + '/' + this.bcode);
  }
  PostDetails(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/AccountCodeSettings/Post', body);
  }
  editAccSettingDetails(objID, arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/AccountCodeSettings/Put?ObjID=' + objID, body);
  }
  GetTopAndSkipRecords(top, skip) {
    return this._http.get(this.apiBaseUrl + '/api/AccountCodeSettings/List/' + this.ccode + '/' + this.bcode + '?$top=' + top + '&$skip=' + skip + '&$orderby=ObjID asc');
  }
  GetTotalRecordCount() {
    return this._http.get(this.apiBaseUrl + '/api/AccountCodeSettings/Count/' + this.ccode + '/' + this.bcode);
  }
  getStateCode() {
    return this._http.get(this.apiBaseUrl + 'api/masters/state/list')
  }
  //////////////////////////////////////////////////////////////////////////////////////
  /// ///EXPENSE VOUCEHR ENTRY
  //////////////////////////////////////////////////////////////////////////////////////

  getVendorNameList(arg) {
    return this._http.get(this.apiBaseUrl + 'api/ExpanseVoucherEntry/Vendor/' + this.ccode + '/' + this.bcode + '/' + arg);

  }
  getLedgerTypeList(arg) {
    return this._http.get(this.apiBaseUrl + 'api/ExpanseVoucherEntry/Ledger/' + this.ccode + '/' + this.bcode + '/' + arg);

  }
  getVendorSateCode(arg) {
    return this._http.get(this.apiBaseUrl + 'api/ExpanseVoucherEntry/LedgerDet/' + this.ccode + '/' + this.bcode + '/' + arg);

  }
  getGSTList() {
    return this._http.get(this.apiBaseUrl + 'api/ExpanseVoucherEntry/GSTGroupCode/' + this.ccode + '/' + this.bcode);
  }

  getTINAndPANList(arg) {
    return this._http.get(this.apiBaseUrl + 'api/ExpanseVoucherEntry/TINAndPAN/' + this.ccode + '/' + this.bcode + '/' + arg);

  }
  getCessList(arg) {
    return this._http.get(this.apiBaseUrl + 'api/ExpanseVoucherEntry/CessLedger/' + this.ccode + '/' + this.bcode + '/' + arg);

  }
  getTDSLedgerList(arg) {
    return this._http.get(this.apiBaseUrl + 'api/ExpanseVoucherEntry/TDSLedger/' + this.ccode + '/' + this.bcode + '/' + arg);

  }

  getCompanyMaster() {
    return this._http.get(this.apiBaseUrl + 'api/Master/CompanyMaster/Get/' + this.ccode + '/' + this.bcode);
  }

  addBoi(n1: number, n2: number) {
    return (n1 + n2);
  }
  countObjectKeys(obj) {
    return Object.keys(obj).length;
  }
  postExpVoucher(object) {
    var body = JSON.stringify(object)
    return this._http.post(this.apiBaseUrl + 'api/ExpanseVoucherEntry/Post', body);
  }
  GetListByDate(date) {
    return this._http.get(this.apiBaseUrl + 'api/ExpanseVoucherEntry/ExpanseEntry/' + this.ccode + '/' + this.bcode + '/' + date);
  }
  ModifyOrUpdateCounterByPUT(obj, arg) {
    const body = JSON.stringify(arg);
    return this._http.put(this.apiBaseUrl + 'api/ExpanseVoucherEntry/Put/' + obj, body);
  }
  //////////////////////////////////////////////////////////////////////////////////////
  /// ///VENDOR MASTER
  //////////////////////////////////////////////////////////////////////////////////////
  GetVendorGroups() {
    return this._http.get(this.apiBaseUrl + 'api/Vendor/GroupTo/' + this.ccode + '/' + this.bcode);
  }
  getVendorMetalList() {
    return this._http.get(this.apiBaseUrl + 'api/Vendor/MetalType/' + this.ccode + '/' + this.bcode);
  }
  getVendorGSTypeList() {
    return this._http.get(this.apiBaseUrl + 'api/Vendor/GSType/' + this.ccode + '/' + this.bcode);
  }
  GetVendorTDS() {
    return this._http.get(this.apiBaseUrl + 'api/Vendor/TDS/' + this.ccode + '/' + this.bcode);
  }
  GetVendorOpenTypeList() {
    return this._http.get(this.apiBaseUrl + 'api/Vendor/OpenType/' + this.ccode + '/' + this.bcode);
  }
  postVendorDetails(object) {
    var body = JSON.stringify(object)
    return this._http.post(this.apiBaseUrl + 'api/Vendor/Post', body);
  }
  getAllVendor() {
    return this._http.get(this.apiBaseUrl + 'api/Vendor/VendorList/' + this.ccode + '/' + this.bcode);
  }
  printVendorDetails(arg) {
    return this._http.get(this.apiBaseUrl + 'api/Vendor/PrintList/' + arg + '/' + this.ccode + '/' + this.bcode);
  }


  postelevatedpermission(arg) {
    const body = JSON.stringify(arg);
    return this._http.post(this.apiBaseUrl + 'api/elevatedpermission/post', body);
  }
}