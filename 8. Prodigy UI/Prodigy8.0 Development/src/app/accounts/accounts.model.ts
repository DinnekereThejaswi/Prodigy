export class MasterGroupVM {
    ObjID: string;
    CompanyCode: string;
    BranchCode: string;
    GroupID: number;
    GroupName: string;
    GroupType: string;
    Under: string;
    GroupDescription: string;
    IsTrading: string;
    ObjStatus: string;
    ParentGroupID: number
    NewGroupCode: string;
    NewSubGroupCode: string;
}

export class SubGroupVM {
    ParentGroupName: string;
    ObjID: string;
    CompanyCode: string;
    BranchCode: string;
    GroupID: number;
    GroupName: string;
    GroupType: string;
    Under: string;
    GroupDescription: string;
    IsTrading: string;
    ObjStatus: string;
    ParentGroupID: number
    NewGroupCode: string;
    NewSubGroupCode: string;
}

export class NarrationVM {
    ObjID: string;
    CompanyCode: string;
    BranchCode: string;
    NarrID: number;
    Narration: string;
}


export class AccVoucherTransactionsVM {
    CompanyCode: string;
    BranchCode: string;
    TxtSeqNo: number;
    VoucherSeqNo: number;
    VoucherNo: number;
    AccountCode: number;
    AccountType: string;
    DebitAmount: number;
    CreditAmount: number;
    ChequeNo: string;
    ChequeDate: Date;
    FinalYear: number;
    FinalPeriod: number;
    AccountCodeMaster: number;
    Narration: string;
    ReceiptNo: string;
    TransType: string;
    ApprovedBy: string;
    CancelledRemarks: string;
    CancelledBy: string;
    Partyname: string;
    VoucherType: string;
    ReconsileBy: string;
    CurrencyType: string;
    NewVoucherNo: number;
    SectionID: string;
    VerifiedBy: string;
    VerifiedRemarks: string;
    AuthorizedBy: string;
    AuthorizedRemarks: string;
    ExchangeRate: number;
    CurrencyValue: number;
    ContraSeqNo: number;
    ReconsileFlag: string;
    Cflag: string;
    IsApproved: string;
    SubledgerAccCode: number;
}

export class Cheque {
    ObjID: string;
    CompanyCode: string;
    BranchCode: string;
    AccCode: number;
    ChqNo: number;
    NoOfChqs: number;
    ChqStartNo: number;
    ChqEndNo: number;
    ChqIssueDate: Date;
    ChqIssued: string;
    ChqClosed: string;
    ChqClosedBy: string;
    ChqClosedRemarks: string;
    MaxAmount: number;
}

export class CashInHand {
    ObjID: string;
    BranchCode: string;
    CompanyCode: string;
    SlNo: number;
    CashBalance: number;
    CashInHand: number;
    BillDate: number;
    FinYear: number;
}


export class LedgerMasterVM {
    ObjID: string;
    CompanyCode: string;
    BranchCode: string;
    AccCode: number;
    AccName: string;
    AccType: string;
    GroupID: number;
    OpeBal: number;
    OpnBalType: string;
    CustID: number;
    PartyCode: string;
    GSCode: string;
    ObjStatus: string;
    GSSeqNo: number;
    SchemeCode: string;
    VATID: string;
    OdLimit: number;
    UpdateOn: Date;
    JFlag: string;
    PANCard: string;
    TIN: string;
    LedgerType: string;
    Address1: string;
    Address2: string;
    Address3: string;
    City: string;
    State: string;
    District: string;
    Country: string;
    PinCode: string;
    Phone: string;
    Mobile: string;
    FaxNo: string;
    WebSite: string;
    CSTNo: string;
    BudgetAmt: number;
    TDSID: number;
    EmailID: string;
    HSN: string;
    GSTGoodsGroupCode: string;
    GSTServicesGroupCode: string;
    StateCode: number;
    VTYPE: string;
    UniqRowID: string;
    TransType: string;
    IsAutomatic: string;
    Schedule_Name: string;
    NewAccCode: string;
    PartyACNo: string;
    PartyACName: string;
    PartyMICR_No: string;
    PartyBankName: string;
    PartyBankBranch: string;
    PartyBankAddress: string;
    PartyRTGScode: string;
    PartyNEFTcode: string;
    PartyIFSCcode: string;
    swiftcode: string;
}
//cash back model class
export class cashbackModal {
    CompanyCode: string;
    BranchCode: string;
    TotalBillAmount: Number;
    TotalEligableCashBackAmount: Number;
    TotalActualCashBackAmount: Number
    OperatorCode: string;
    IsEPayment: false;
    ListCashBackVM: [
        {
            CompanyCode: string;
            BranchCode: string;
            BillNo: Number;
            BillAmt: Number;
            EligableCashBack: Number;
            ActualCashBack: Number;
            Remarks: ""
        }
    ]

}

export class TransactionPosting {
    CompanyCode: string;
    BranchCode: string;
    GSCode: string;
    TransType: string;
    AccountCode: number;
    OldAccountCode: number;
}

export class TDSExpenseCancelVM {
    ExpenseNo: number;
    CompanyCode: string;
    BranchCode: string;
    Remarks: string;
}



export class ExpenseVocherModel {
    ObjID: string;
    ExpenseNo: number;
    ExpenseDate: string;
    CompanyCode: string;
    BranchCode: string;
    SupplierCode: string;
    SupplierName: string;
    SNo: number;
    AccCode: number;
    AccName: string;
    InvoiceNo: string;
    InvoiceDate: string;
    Amount: number;
    TaxName: string;
    TaxPercentage: number;
    TaxAmount: number;
    TotalAmount: number;
    TDSPercentage: number;
    TDSAmount: number;
    CFlag: string;
    CancelledBy: string;
    OperatorCode: string;
    Description: string;
    ObjStatus: string;
    SGSTPercent: number;
    SGSTAmount: number;
    CGSTPercent: number;
    CGSTAmount: number;
    IGSTPercent: number;
    IGSTAmount: number;
    HSN: string;
    GSTGroupCode: string;
    AccType: string;
    ChqNo: number;
    ChqDate: string;
    GSTNo: string;
    OtherPartyName: string;
    PartyCode: string;
    CESSPercent: number;
    CESSAmount: number;
    Remarks: string;
    PANNo: string;
    IsEligible: string;
    HSNDescription: string;
    RoundOff: number;
    FinalAmount: number;
    TDSID: number;
    CessAccountCode: number;
    AccountType: string;
    IsRegistered: string
}

export class VendorModel {
    ObjID: string;
    CompanyCode: string;
    BranchCode: string;
    PartyCode: string;
    PartyName: string;
    VoucherCode: string;
    Address1: string;
    Address2: string;
    Address3: string;
    District: string;
    State: string;
    StateStatus: string;
    City: string;
    Country: string;
    PinCode: string;
    Phone: string;
    Mobile: string;
    FaxNo: string;
    PAN: string;
    Website: string;
    TIN: string;
    TDS: string;
    VAT: string;
    CSTNo: string;
    ObjectStatus: string;
    UpdateOn: string;
    PartyRTGScode: string;
    PartyNEFTcode: string;
    PartyIFSCcode: string;
    PartyAccountNo: string;
    PartyMICRNo: string;
    PartyBankName: string;
    PartyBankBranch: string;
    PartyBankAddress: string;
    ContactPerson: string;
    ContactEmail: string;
    Email: string;
    ContactMobileNo: string;
    PartyACName: string;
    OpnBal: Number;
    OpnBalType: string;
    OpnWeight: Number;
    OpnWeightType: string;
    CreditPeriod: Number;
    LeadTime: Number;
    MaxPayment: Number;
    ConvRate: Number;
    SwiftCode: string;
    BranchType: string;
    TDSPercent: Number;
    CreditWeight: Number;
    TDSId: Number;
    IsSameEntity: string;
    StateCode: Number;
    SchemeType: string;
    SupplierMetal: string;
    UniqRowID: string;
    ListGroupTo: [];
    ListOpenDet: [];
}

export class ListGroupTo {
    ObjID: string;
    CompanyCode: string;
    BranchCode: string;
    PartyCode: string;
    VoucherCode: string;
    IRCode: string;
    GroupName: string;
    UpdateOn: string;
    UniqRowID: string;
}
export class ListOpenDet {
    ObjID: string;
    CompanyCode: string;
    BranchCode: string;
    PartyCode: string;
    VoucherCode: string;
    OpnBal: Number;
    BalType: string;
    OpnWeight: Number;
    WeightType: string;
    ObjectStatus: string;
    UpdateOn: string;
    MetalCode: string;
    OpnPureWeight: Number;
    OpnNetWeight: Number;
    FinYear: Number;
    GSCode: string;
    RefNo: Number;
    UniqRowID: string
}
