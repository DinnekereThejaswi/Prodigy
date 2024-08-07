export class SalesBilling {
    IsOrder: boolean;
    IsCreditNote: boolean;
    SalesManCode: boolean;
    CompanyCode: string;
    BranchCode: string;
    CustID: number;
    MobileNo: number;
    EstNo: string;
    ReceivableAmt: Number;
    BalanceAmt: Number;
    DifferenceDiscountAmt: Number;
    PayableAmt: Number;
    OperatorCode: string;
    GuaranteeName: string;
    DueDate: string;
    PANNo: string;
    BillType: string;
    lstOfPayment: [];
    lstOfPayToCustomer: PayToCustomer[];
    salesInvoiceAttribute: [];
    OrderOTPAttributes: OTPModel[]
    RowRevision: string;
}

export class BillReceiptModel {
    CompanyCode: string;
    BranchCode: string;
    BillNo: number;
    AddPayments: [];
}


export interface OTPModel {
    OrderNo: number;
    MobileNo: number;
    SMSID: number;
    IsValidated: boolean;
}

export interface PayToCustomer {
    ObjID: null,
    CompanyCode: null,
    BranchCode: null,
    SeriesNo: 0,
    ReceiptNo: 0,
    SNo: 0,
    TransType: null,
    PayMode: null,
    PayDetails: null,
    PayDate: null,
    PayAmount: null,
    RefBillNo: null,
    PartyCode: null,
    BillCounter: null,
    IsPaid: null,
    Bank: null,
    BankName: null,
    ChequeDate: null,
    CardType: null,
    ExpiryDate: null,
    CFlag: null,
    CardAppNo: null,
    SchemeCode: null,
    SalBillType: null,
    OperatorCode: null,
    SessionNo: null,
    UpdateOn: null,
    GroupCode: null,
    AmtReceived: null,
    BonusAmt: null,
    WinAmt: null,
    CTBranch: null,
    FinYear: null,
    CardCharges: null,
    ChequeNo: null,
    NewBillNo: null,
    AddDisc: null,
    IsOrderManual: null,
    CurrencyValue: null,
    ExchangeRate: null,
    CurrencyType: null,
    TaxPercentage: null,
    CancelledBy: null,
    CancelledRemarks: null,
    CancelledDate: null,
    IsExchange: null,
    ExchangeNo: null,
    NewReceiptNo: null,
    GiftAmount: null,
    CardSwipedBy: null,
    Version: null,
    GSTGroupCode: null,
    SGSTPercent: null,
    CGSTPercent: null,
    IGSTPercent: null,
    HSN: null,
    SGSTAmount: null,
    CGSTAmount: null,
    IGSTAmount: null,
    PayAmountBeforeTax: null,
    PayTaxAmount: null,
    IsCashMarking: null,
    ReceivedCash: null,
    AdditionalDiscount: null
}