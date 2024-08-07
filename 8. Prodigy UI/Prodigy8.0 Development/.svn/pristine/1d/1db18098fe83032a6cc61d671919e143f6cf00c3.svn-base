export class PurchaseModel {
    SalCode: string;
    GSCode: string;
    ItemName: string;
    GrossWt: number;
    StneWt: number;
    NetWt: number;
    PurityPercent: number;
    MeltingPercent: number;
    CompanyCode: string;
    BranchCode: string;
    MeltingLoss: number;
    PurchaseRate: number;
    DiamondAmount: number;
    Dcts: number;
    GoldAmount: number;
    ItemAmount: number;
    lstPurchaseEstStoneDetailsVM: [];
    lstPurchaseEstDiamondDetailsVM: [];
}


export interface PurchaseSummaryModel {
    Qty: Number;
    GrossWt: Number;
    NtWt: Number;
    StoneWt: Number;
    carats: Number;
    Amount: Number;
}

export class PurchaseEstModel {
    CustID: number;
    PurItem: string;
    TodayRate: number;
    OperatorCode: string;
    GrandTotal: string;
    PType: string
    TotalPurchaseAmount: number;
    PurchaseEstDetailsVM: [{
        SalCode: string,
        ItemName: string;
        GrossWt: number;
        StneWt: number;
        NetWt: number;
        MeltingPercent: number;
        MeltingLoss: number;
        PurchaseRate: number;
        DiamondAmount: number;
        GoldAmount: number;
        ItemAmount: number;
        PurchaseEstStoneDetailsVM: [{
            ObjID: string;
            CompanyCode: string;
            BranchCode: string;
            BillNO: number;
            SlNo: number;
            EstNo: number;
            ItemSlNo: number;
            Type: string;
            Name: string;
            Qty: number;
            Carrat: number;
            Rate: number;
            Amount: number;
            PurchaseDealerBillNo: number;
            GSCode: string;
            PurReturnNo: number;
            UpdateOn: number;
            FinYear: number;
        }]
    }]
}


export class DiamondModel {
    estno: number;
    item_sno: string;
    type: string;
    Name: string;
    Qty: number;
    Carrat: number;
    Rate: number;
    OrgRate: number;
    Amount: number;
    CompanyCode: string;
    BranchCode: string;
}

export class PurchaseBilling {
    CompanyCode: string;
    BranchCode: string;
    EstNo: string;
    OperatorCode: string;
    BillCounter: string;
    Type: string;
    BillNo: Number;
    CustName: string;
    PaidAmount: Number;
    CancelRemarks: string;
    lstOfPayment: []
}