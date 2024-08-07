export class NTReceiptModel {
    GSCode: string;
    ItemCode: string;
    BarcodeNo: string;
    CounterCode: string;
    SlNo: number;
    Qty: number;
    GrossWt: number;
    StoneWt: number;
    NetWt: number;
    Dcts: number;
    PurityPercent: number;
    PureWeight: number;
    Rate: number;
    Amount: number;
    BarcodeReceiptStoneDetails: []
}

export class NTReceiptstoneModel {
    BarcodeNo: number;
    SlNo: number;
    Type: string;
    Name: string;
    Qty: number;
    Carrat: number;
    Weight: number;
    Rate: number;
    Amount: number
}