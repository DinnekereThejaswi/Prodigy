export class OPGBatchLines {
    GSCode: string;
    BatchId: string;
    GrossWt: number;
    StoneWt: number;
    NetWt: number;
    Rate: number;
    Amount: number;
}

export class OPGMeltingLines {
    IssueNo: number;
    BatchId: string;
    GSCode: string;
    ItemCode: string;
    Qty: number;
    GrossWt: number;
    StoneWt: number;
    NetWt: number;
    WastageWt: number;
    PurityPercent: number;
    AlloyWt: number;
    Amount: number;
    Rate: number;
}

export class OPGSeggregationModel {
    GSCode: string;
    GrossWeight: number;
    StoneGSCode: string;
    StoneWeight: number;
    DiamondGSCode: string;
    DiamondCaretWeight: number;
    NetWt: number;
    PurityPercent: number;
    PureWeight: number;
}


export class OPGStoneModel {
    SlNo: number;
    BillNo: number;
    GS: string;
    Name: string;
    Qty: number;
    Carrat: number;
    Rate: number;
    Amount: number;
}