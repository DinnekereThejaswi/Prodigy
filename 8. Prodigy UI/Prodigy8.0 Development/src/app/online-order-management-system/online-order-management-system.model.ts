export class ReceiveOrder {
    CompanyCode: string;
    BranchCode: string;
    FromDate: string;
    ToDate: string;
    MarketplaceID: string;
    Type: string;
}

export class CreatePackageModel {
    CompanyCode: string;
    BranchCode: string;
    OrderNo: number;
    OrderReferenceNo: string;
    ItemName: string;
    BarcodeNo: string;
    Qty: number;
    MarketplaceSlNo: number;
    CentralRefNo: number;
    PackageCode: string;
    OTLNo: string;
    Length: number;
    LengthUom: string;
    Width: number;
    WidthUom: string;
    Height: number;
    HeightUom: string;
    Weight: number;
    WeightUom: string;
    PackageID: string;
    OrderSource: string;
}
