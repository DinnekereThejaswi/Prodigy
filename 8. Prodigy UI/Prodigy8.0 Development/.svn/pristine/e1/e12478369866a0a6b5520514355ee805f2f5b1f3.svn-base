import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from '../../AppConfigService';
import { MastersService } from '../masters.service';
import swal from 'sweetalert';
declare var $: any;


@Component({
  selector: 'app-application-config',
  templateUrl: './application-config.component.html',
  styleUrls: ['./application-config.component.css']
})
export class ApplicationConfigComponent implements OnInit {
  EnableAdd: boolean = true;
  EnableSave: boolean = false;
  isReadOnly: boolean = false;
  EnableJson: boolean = false;
  ConfigForm: FormGroup;
  ccode: string;
  bcode: string;
  password: string;
  ConfigModel = {
    "ObjId": "",
    "CompanyCode": "",
    "BranchCode": "",
    "Description": "",
    "value": "",
  }
  ConfigData = [
    {
      "ObjId": "1042018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-do not post new accounts,0-new account posting",
      "value": "0"
    },
    {
      "ObjId": "1042019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "0- Normal, 1- thoalsi changes",
      "value": "0"
    },
    {
      "ObjId": "1042021",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1- MeraldaGSPosting Reqired, 0- Not reqired",
      "value": "0"
    },
    {
      "ObjId": "1052018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "order",
      "value": "0"
    },
    {
      "ObjId": "1062018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "CR",
      "value": "0"
    },
    {
      "ObjId": "1072018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Bhima Dealer Sales,0-New Account Posting,1-Old Account Posting",
      "value": "0"
    },
    {
      "ObjId": "1072019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Journal Voucher Appenidng year month 1- apply , 0 - normal sequence",
      "value": "1"
    },
    {
      "ObjId": "1102019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "0-operator txt box visble false, 1- operator txt visble true",
      "value": "1"
    },
    {
      "ObjId": "1122017",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Barcode print Format 0 - EKM, 1- BNG",
      "value": "1"
    },
    {
      "ObjId": "2042018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-validate double entry",
      "value": "0"
    },
    {
      "ObjId": "2062018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "SR",
      "value": "0"
    },
    {
      "ObjId": "3042018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Sales Bill Edit,0-Account Posting,1-Not Posting ",
      "value": "0"
    },
    {
      "ObjId": "3052021",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1 for TCS Calculation for Purchase Return, 0-Not Required",
      "value": "1"
    },
    {
      "ObjId": "3062018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Repair",
      "value": "0"
    },
    {
      "ObjId": "3072019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "0- wasatge per disable in VA master for type 1 , 1- wasatge per enable in VA master for type 1 ",
      "value": "0"
    },
    {
      "ObjId": "3082020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-Enable Authorize Discount  , 0 - Disable Authorize Discount",
      "value": "0"
    },
    {
      "ObjId": "3092020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1- Smith HM and Repair Auto Receipts, 0- Manual",
      "value": "0"
    },
    {
      "ObjId": "3102019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1- GS Validation",
      "value": "1"
    },
    {
      "ObjId": "4032020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "VA Calcuation at Branch :1-Enabled,0-Disabled",
      "value": "0"
    },
    {
      "ObjId": "4042018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Purchase Account Posting,0-Account Posting,1-Not Posting ",
      "value": "0"
    },
    {
      "ObjId": "4052020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Option to provide flexi order :Flexi Order Generation :1,No Flexi Order :0",
      "value": "1"
    },
    {
      "ObjId": "4062018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-Stone Sales  0- acc posting",
      "value": "0"
    },
    {
      "ObjId": "4122017",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Bulk Receipt and bulk purchase receipt for bhima 1,others 0",
      "value": "1"
    },
    {
      "ObjId": "5022021",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1- In Credit Purchase Posted to Customer Ledger, 0- Credit Purchase Ledger",
      "value": "0"
    },
    {
      "ObjId": "5032018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "VA Master Supplier All Option -0 not show,1-Show All Option",
      "value": "0"
    },
    {
      "ObjId": "5042018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Purchase Double Entry Validation,0-Validate,1-Not Validate ",
      "value": "0"
    },
    {
      "ObjId": "5042019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Wholesales  Supplier Touch Conf. 0 - Supplier wise, 1 - Product wise",
      "value": "0"
    },
    {
      "ObjId": "5062018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-Stone Sales Double Entry  0- acc posting",
      "value": "1"
    },
    {
      "ObjId": "5112019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1 - Automatic Stone Weight in Barcode Entry, 0 - Default",
      "value": "0"
    },
    {
      "ObjId": "5122020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1- Smith Receipts Account Posting Not Required, 0- Required",
      "value": "1"
    },
    {
      "ObjId": "6022020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "0 - Do not validate PIN code, 1 - Validate PIN code",
      "value": "0"
    },
    {
      "ObjId": "6042019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Wholesales  customer Touch Conf. 0 - customer wise, 1 - Product wise",
      "value": "0"
    },
    {
      "ObjId": "6062018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-stone purchase  0- acc posting",
      "value": "0"
    },
    {
      "ObjId": "6112020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "0- for Other Customers, 1- Accounted Branch Kavitha ",
      "value": "0"
    },
    {
      "ObjId": "7012020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Journalprint",
      "value": "1"
    },
    {
      "ObjId": "7042021",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "0- Old Bill number, 1-New Bill number",
      "value": "0"
    },
    {
      "ObjId": "7062018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1- Stone  Purchase Double Entry  0- acc posting",
      "value": "1"
    },
    {
      "ObjId": "7082020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1- Accounts Posts for Diamond Excel Import, 0 - No acc Posting",
      "value": "0"
    },
    {
      "ObjId": "7112019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-enable validation for duplicate item name 0-disable validation",
      "value": "1"
    },
    {
      "ObjId": "8012018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "OFFER AMOUNT VALIDATION FOR SCHEME ADJUSTMENT 0:VALIDATION IS APPLICABLE,1:NOT APPLICABLE",
      "value": "0"
    },
    {
      "ObjId": "8062018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Enable report based on booking type",
      "value": "1"
    },
    {
      "ObjId": "9062020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "OTP valid time(minutes)",
      "value": "2"
    },
    {
      "ObjId": "1",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "DESIGN MASTER LINK FOR SELLING VA LOGIC.  1. VA details considering design -value(1).2. VA details without considering design - value (2)",
      "value": "1"
    },
    {
      "ObjId": "10",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Barcode Print Type : Kavitha :1, Bhima Cochin :2,Bhima Bangalore : 3",
      "value": "3"
    },
    {
      "ObjId": "1001",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Purchase Estimation Print 1 For Bhima and kavitha,0 For  Ernakulam",
      "value": "1"
    },
    {
      "ObjId": "10082020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-Validate for offer discount attached for ordres   , 0 - No validateion for  offer discount attached for ordres ",
      "value": "1"
    },
    {
      "ObjId": "10092019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-POS STATECODE,0-STATECODE",
      "value": "0"
    },
    {
      "ObjId": "1072019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Journal Voucher Appenidng year month 1- apply , 0 - normal sequence",
      "value": "1"
    },
    {
      "ObjId": "11",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-Load issue details 0- hide Issue details",
      "value": "1"
    },
    {
      "ObjId": "11052020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "ORDER PAYMENT VALIDATION NOT REQUIRED-1 REQUIRED-0",
      "value": "0"
    },
    {
      "ObjId": "1132021",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "CESS field enable 1 else 0",
      "value": "0"
    },
    {
      "ObjId": "1200",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": " 1.Bhima PURCHASE ",
      "value": "1"
    },
    {
      "ObjId": "1201",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Stone Validation Remove in Non Tag Receipts",
      "value": "2"
    },
    {
      "ObjId": "12022021",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1- Last data Loading, 0- Not Required",
      "value": "0"
    },
    {
      "ObjId": "12052020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "PARTIAL ORDER CLOSED AT SALES ESTIMATION VALIDATION  :VALIDATE FOR PARTIAL ORDER CLOSE:1 ,NO VALIDATION :0",
      "value": "1"
    },
    {
      "ObjId": "12092019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Kavitha-not to display the gift counter details in stock    details",
      "value": "0"
    },
    {
      "ObjId": "12122017",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Item Bifurcation should to take diamond carrat as calculation of nwt, 0 -others,1-Bhima",
      "value": "1"
    },
    {
      "ObjId": "1222",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Old purchase Adjustment with 24K Sales Item - True:1,False : 0",
      "value": "0"
    },
    {
      "ObjId": "1300",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "ShortExcess inCounterStock 1.Bhima and Eranakumal,0 Kavitha",
      "value": "1"
    },
    {
      "ObjId": "15032018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "For  linked server 0 and for web service 1",
      "value": "0"
    },
    {
      "ObjId": "15092020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1- Smith Issue/receipts Counter and GS Stock Upadte, 0- Only GS Stock",
      "value": "0"
    },
    {
      "ObjId": "16042021",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1- Cash In Hand Ledger list in Bank Vou Entry, 0- Not reqired",
      "value": "0"
    },
    {
      "ObjId": "16092019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "0-Normal Offer Discount ,1-Category Wise Discount",
      "value": "0"
    },
    {
      "ObjId": "16102020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Validate for Addtional Discount 0,No Validation 1",
      "value": "1"
    },
    {
      "ObjId": "16122020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-Stone details not to print in OPG report, 0-print stone details",
      "value": "0"
    },
    {
      "ObjId": "17062019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1 for Visible Billtype Combobox, 0 for Hide Billtype Combobox",
      "value": "0"
    },
    {
      "ObjId": "17092020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "For kavitha -1, others -0",
      "value": "0"
    },
    {
      "ObjId": "18022021",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1- Is Eligible, 0- Not Required",
      "value": "0"
    },
    {
      "ObjId": "18062020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "In purchase estimation selecting the category combo is mandatory for value 1 else 0",
      "value": "0"
    },
    {
      "ObjId": "19042018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Barcode Issue Form Bin No loaded throgh GS -1 -Load GS wise,0-All",
      "value": "1"
    },
    {
      "ObjId": "190620",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Advance percent combobox at order UI visible =1 , 0= invisibe",
      "value": "0"
    },
    {
      "ObjId": "19062020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Advance percent combobox at order UI visible =1 , 0= invisibe",
      "value": "0"
    },
    {
      "ObjId": "19092019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Selling MC 1-Filter by Supplier, esle - 0",
      "value": "0"
    },
    {
      "ObjId": "19102020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "E-invoicing enabling 1 else 0",
      "value": "0"
    },
    {
      "ObjId": "2",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Single Side print - 1,Double side print - 2",
      "value": "2"
    },
    {
      "ObjId": "2001",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Journal Voucher Bhima 1 else 0",
      "value": "1"
    },
    {
      "ObjId": "2002",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Sales local interstate posting in to Accounts Ledger , Single ledger :0 , Split Ledger : 1",
      "value": "0"
    },
    {
      "ObjId": "2003",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Accounts Stock transfer issue / receipt 1 for posting ",
      "value": "0"
    },
    {
      "ObjId": "2004",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "CTC GS Validation 1-Kavitha,0-others",
      "value": "0"
    },
    {
      "ObjId": "20042018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Purchase Return Item Enabled-1 -Enabled ,0-Disabled",
      "value": "0"
    },
    {
      "ObjId": "2005",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Idols and Frame GST posting",
      "value": "1"
    },
    {
      "ObjId": "20052021",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "kovawt is added in seprate gold gust gs stock if value is 1 - else 0 ",
      "value": "0"
    },
    {
      "ObjId": "2006",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1- Other Entity/Dealer sales Pure Wt Calculation, 0 - Net wt calculation",
      "value": "0"
    },
    {
      "ObjId": "20062019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "0- Purchase total amount disable, 1-  Purchase total amount enable",
      "value": "0"
    },
    {
      "ObjId": "20072019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "0-Other Customer, 1- LAS",
      "value": "0"
    },
    {
      "ObjId": "20082019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Purchase return stone posting split 0 else 1",
      "value": "0"
    },
    {
      "ObjId": "2018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Select and insert in  Chit download , if  0 for Closded mode = B  and  1 for Not Compare with Closed mode, in CHTU_CHIT_CLOSURE table",
      "value": "1"
    },
    {
      "ObjId": "20180227",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-Bhima blr,0-OTHERS ",
      "value": "1"
    },
    {
      "ObjId": "20181009",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Purchase Rate disable 1-disable,0-Enable",
      "value": "1"
    },
    {
      "ObjId": "20200915",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Bhima BLR value posting 1 else other clients 0",
      "value": "1"
    },
    {
      "ObjId": "20200920",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Scheme Dormant flag update code 1 else 0",
      "value": "1"
    },
    {
      "ObjId": "21012020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Is Stone Weight Automatic calculation based on stone details in Barcode Entry 0 - false, 1- true",
      "value": "0"
    },
    {
      "ObjId": "21072018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Hallmark Account Posting 0-Post,1-   not post",
      "value": "1"
    },
    {
      "ObjId": "21072019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "0-Other Customer, 1- LAS",
      "value": "0"
    },
    {
      "ObjId": "21082020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Order EMAIL ENABLE 1,DISBLE 0",
      "value": "0"
    },
    {
      "ObjId": "22112018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "SR barcode no updation - 1-barcode sold flag update to N,0-No updation of barcode",
      "value": "0"
    },
    {
      "ObjId": "2252018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Amount calculation in Branch Issue through purchase rate or Selling rate,1-Purchase Rate,0-Selling Rate",
      "value": "2"
    },
    {
      "ObjId": "23042021",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Validate For Single GS billing - 0 false, 1- true",
      "value": "0"
    },
    {
      "ObjId": "230421",
      "CompanyCode": "BH",
      "BranchCode": "CPC",
      "Description": "Image Processing Y for true-1 else 0",
      "value": "1"
    },
    {
      "ObjId": "23062020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Rate Change Validation 1: Validate ,0 :No Validation",
      "value": "1"
    },
    {
      "ObjId": "23072019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "0- Smith Ledger Separate GWT , NWT Report, 1 -  Smith Ledger Both weights in   Single Report",
      "value": "0"
    },
    {
      "ObjId": "24022021",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Load orders depend upon bookinhg type 1:Load only normal order,0:Load all orders",
      "value": "1"
    },
    {
      "ObjId": "24032018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "SR should update to counter stock 1: for update ,0: for no update",
      "value": "0"
    },
    {
      "ObjId": "24032020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "SALES BILLING REFERED BY COMBO LOAD FROM  :  REFERED BY MASTER:1 ,CUSTOMER MASTER :0",
      "value": "1"
    },
    {
      "ObjId": "24072018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1- Purchase Invoice entry, the posting date should be the same as bulk receipt date ",
      "value": "0"
    },
    {
      "ObjId": "24072020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1- Validate Scheme for Cash Back Offer, 0 - No Validation for Cash Back Offer",
      "value": "1"
    },
    {
      "ObjId": "24082020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Invoices Sent through Email :0 Disable Email Option,1:Enable Email Option",
      "value": "0"
    },
    {
      "ObjId": "24092018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "INTERNEL RECEIPT SHOWS IN BRANCH RECEIPT REPORT -0 SHOWS ,1-HIDE",
      "value": "1"
    },
    {
      "ObjId": "25062019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "0- for normal Stock Ledger, 1- Weight wise Stock Ledger",
      "value": "0"
    },
    {
      "ObjId": "25092019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "GS wise stock issue/receipts value posting 1 else 0",
      "value": "1"
    },
    {
      "ObjId": "25122017",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Stonesmithissue effect counter stock  0:Effect counter stock,1 Not consider counter stock",
      "value": "0"
    },
    {
      "ObjId": "25122018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Deduct Weight stock transfer to selected counter and item,1- Transfer to another counter and item,0-Same counter and item",
      "value": "0"
    },
    {
      "ObjId": "26",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Supplier Master Value Opening update in to Accounts Ledger , True :0 , False : 1",
      "value": "1"
    },
    {
      "ObjId": "261120202",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Flexi Rate Validity Validation",
      "value": "0"
    },
    {
      "ObjId": "26122017",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Wastage Amount added to MC Amount  0:Not added,1 Added",
      "value": "1"
    },
    {
      "ObjId": "27072018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Bulk Receipt Report",
      "value": "0"
    },
    {
      "ObjId": "27072020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-  Cash Back Offer discount in diamond , 0 - Normal Discount",
      "value": "0"
    },
    {
      "ObjId": "270819891",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "0 - No Market Place Order, 1 - Market Place order present",
      "value": "1"
    },
    {
      "ObjId": "270820",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "SupplierLoading in CRDR Note",
      "value": "1"
    },
    {
      "ObjId": "27112020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1- Round off Not Posting to Party Ledger, 0- Round off Posting to Party",
      "value": "0"
    },
    {
      "ObjId": "27122017",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Nonbage Receipts need to show iaaue details  0:Not show,1 Show",
      "value": "1"
    },
    {
      "ObjId": "28012020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-ORDER GENERATION ORDER RECEIPT PRINT IS REMOVED ,0-ORDER RECEIPT PRINT INCLUDED",
      "value": "1"
    },
    {
      "ObjId": "28022019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-Customer Consoldiation Required,0-Not required",
      "value": "0"
    },
    {
      "ObjId": "28022020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "ORDER IS ATTACHED IN SALES BILL NEED TO SEND OTP :  ENABLE OTP VALIDATION:1 ,DISABLE OTP VALIDATION :0",
      "value": "0"
    },
    {
      "ObjId": "28082018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Allow Order receipts from Fixed rate order - 0 - Yes, 1 - No",
      "value": "0"
    },
    {
      "ObjId": "28082019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "0- Branch Issue GS Stock Validaton, 1 - No GS stock Validation",
      "value": "0"
    },
    {
      "ObjId": "28122017",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Purchase Receipt Posted to Accounts 0:Posted,1 Not posted",
      "value": "0"
    },
    {
      "ObjId": "2892019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "0 - Purchase Detail Report, 1 - Report with Calculated Pure wt",
      "value": "0"
    },
    {
      "ObjId": "29062019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "0- direct barcode mc disable, 1- direct barcode mc enable",
      "value": "0"
    },
    {
      "ObjId": "30",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Barcode Issue,Barcode Amt Calcuation-0 :NetWt*Rate+VA+st+Dm,1-NetWt*Rate+st+Dm",
      "value": "0"
    },
    {
      "ObjId": "3000",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Wholesales  Default Metal amount calculation 1 - Pwt based, 0- nwt based",
      "value": "0"
    },
    {
      "ObjId": "30012019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Scheme Posting single ledger 1- apply , 0 - scheme wise",
      "value": "1"
    },
    {
      "ObjId": "300620",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "AccountsCreditNotDebitNote",
      "value": "1"
    },
    {
      "ObjId": "30062019",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "0- new order no text box disable in estiamtion, 1-  new order no text box disable in estiamtion",
      "value": "0"
    },
    {
      "ObjId": "30112018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Display Cheque Nos",
      "value": "0"
    },
    {
      "ObjId": "31032021",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1- OGS Pay mode list, 0- Not reqired",
      "value": "0"
    },
    {
      "ObjId": "31052018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-Only for Approval reecipt need to issue combo 0- hide the issue combo",
      "value": "1"
    },
    {
      "ObjId": "4",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "BILLING LOGIG.1.Cut weight issued to particular item and particular counter",
      "value": "0"
    },
    {
      "ObjId": "4052020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Flexi Order Option :  Visible :1 ,Hide :0",
      "value": "1"
    },
    {
      "ObjId": "4250",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Customer Details are disabled at sales billing:1,False : 0",
      "value": "1"
    },
    {
      "ObjId": "5",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "BILLING,1.Bill Type,0.EMpty",
      "value": "1"
    },
    {
      "ObjId": "600",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-Bhima BLR Print",
      "value": "1"
    },
    {
      "ObjId": "605",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-Order textbox visible in sales est, 0 -not visible  ",
      "value": "1"
    },
    {
      "ObjId": "610",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-Bhima BLR CPC Partial Wt receipt in OPG segregation , 0-Others ",
      "value": "1"
    },
    {
      "ObjId": "6112020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "E-Com integration Enable 1/ disable 0",
      "value": "1"
    },
    {
      "ObjId": "650",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-Bhima Cochin Dealer Sales Report Supplier Load ",
      "value": "1"
    },
    {
      "ObjId": "660",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-Bhima Cochin Ledger Edit Password,0-Others ",
      "value": "0"
    },
    {
      "ObjId": "7",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Customer Data Mandatory",
      "value": "0"
    },
    {
      "ObjId": "700",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "1-BJ Cochin Supplier code autoGenerate,0 for others ",
      "value": "0"
    },
    {
      "ObjId": "7001",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "GST Applicablility on Orders: 0 GST not applicable, 1 GST applicable",
      "value": "0"
    },
    {
      "ObjId": "7002",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Order Receipt: 0 for Show ,1 -Hide",
      "value": "1"
    },
    {
      "ObjId": "7008",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "SGST,CGST and IGST Calculation for State and Other territory 0:Calculation in Normal,1:Other territory only IGST,other tan all state SGST &CGST",
      "value": "1"
    },
    {
      "ObjId": "72020",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Amazon API Call for Inventory Update",
      "value": "1"
    },
    {
      "ObjId": "750",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Enable Rate Only",
      "value": "1"
    },
    {
      "ObjId": "8",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Journal Voucher Bhima 1 else 0 for others",
      "value": "0"
    },
    {
      "ObjId": "9",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Stone Validation Remove in Non Tag Receipts",
      "value": "0"
    },
    {
      "ObjId": "9000",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Mapping the payment modes of Scheme Collections Posting",
      "value": "0"
    },
    {
      "ObjId": "90219",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "Order generate through credit receipt",
      "value": "0"
    },
    {
      "ObjId": "982018",
      "CompanyCode": "BH",
      "BranchCode": "KRM",
      "Description": "CorpEmpDetailsTabEnableinSalesandEst",
      "value": "1"
    }
  ]
  constructor(private _appConfigService: AppConfigService, private _masterservice: MastersService, private fb: FormBuilder) {

    this.password = this._appConfigService.Pwd;
    this.EnableJson = this._appConfigService.EnableJson;
    this.getCB();
  }
  getCB() {

    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);

  }

  ngOnInit() {
    this.ConfigData;
    this.ConfigModel.CompanyCode = this.ccode;
    this.ConfigModel.BranchCode = this.bcode;
    this.ConfigForm = this.fb.group({
      frmCtrl_ObjId: null,
      frmCtrl_Description: null,
      frmCtrl_Value: null
    });
  }
  edit(arg) {
    this.EnableAdd = false;
    this.EnableSave = true;
    this.ConfigModel = arg;
  }
  Clear() {
    this.EnableSave = false;
    this.EnableAdd = true;
    this.ConfigForm.reset();
    this.ConfigData;
  }
}
