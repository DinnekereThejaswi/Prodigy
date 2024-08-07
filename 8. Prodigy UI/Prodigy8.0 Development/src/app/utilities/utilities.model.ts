export class Utilities {
}

export class Operator {
    OperatorCode: string;
    OperatorName: string;
    OperatorType: string;
    MobileNo: string;
    BranchCode: string;
    CompanyCode: string;
    EmployeeID: Number;
    Password: string;
    RoleID: Number;
    Status: string;
    MaxDiscountPercentAllowed: Number;
    CounterCode: string;
    DefaultStore: string;
    MappedStores: [];
}


export class MainModule {
    ID: number;
    CompanyCode: string;
    BranchCode: string;
    Name: string;
    SortOrder: number;
    Status: string;
    UIRoute: string;
    Icon: string;
    Class: string;
    Label: string;
    LabelClass: string;
}

export class SubModule {
    CompanyCode: string;
    BranchCode: string;
    ID: number;
    Name: string;
    ModuleID: number;
    SortOrder: number;
    Status: string;
    AutoApprove: boolean;
    Flag: string;
    UIRoute: string;
    Icon: string;
    Class: string;
    Label: string;
    LabelClass: string;
    FormType: string;
    ReportServerType: string;
    ReportApiRoute: string;
}
export class Tolerance {
    ID: number;
    CompanyCode: string;
    BranchCode: string;
    Description: string;
    MinValue: number;
    MaxValue: number;
}
