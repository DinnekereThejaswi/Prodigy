
export interface GstPostingSetup {
    ID: number;
    GSTGroupCode: string;
    GSTComponentCode: string;
    EffectiveDate: Date;
    GSTPercent: number;
    CalculationOrder: number;
    ReceivableAccount: number;
    PayableAccount: number;
    ExpenseAccount: number;
    RefundAccount: number;
    IsRegistered: boolean;
}
