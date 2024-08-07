// export interface CustomerModel {
//     ID: number;
//     Salutation: string;
//     Name: string;
//     Address1: string;
//     Address2: string;
//     Address3:  string;
//     PostalCode: string;
//     City: string;
//     StateCode:  string;
//     LocalityID: string;
//     CountryCode: string;
//     MobileNo:  string;
//     PhoneNo:  string;
//     EmailID:  string;
//     HomePage: string;
//     DateOfBirth: Date;
//     WeddingAnnivarsaryDate: Date;
//     Age: string;
//     Sex: string;
//     GSTTIN: string;
//     PAN: string;
//     PrivilegeID: string;
//     CustCode: string;
//     CreditLimit: null;
//     // CustID: string;
//     ChitCardNo: string;
//     InsertedBy: number;
//    }

export interface CustomerModel {
    ObjID: string;
    CompanyCode: string;
    BranchCode: string;
    ID: number;
    CustName: string;
    Address1: string;
    Address2: string;
    Address3: string;
    City: string;
    State: string;
    Pincode: string;
    MobileNo: string;
    PhoneNo: string;
    DateOfBirth: Date;
    WeddingDate: Date;
    CustomerType: string;
    ObjectStatus: string;
    SpouseName: string;
    ChildName1: string;
    ChildName2: string;
    ChildName3: string;
    SpouseDateOfBirth: Date;
    Child1DateOfBirth: Date;
    Child2DateOfBirth: Date;
    Child3DateOfBirth: Date;
    PANNo: string;
    IDType: string;
    IDDetails: string;
    UpdateOn: Date;
    EmailID: string;
    CreatedDate: Date;
    Salutation: string;
    CountryCode: string;
    LoyaltyID: string;
    ICNo: string;
    PassportNo: string;
    PRNo: string;
    PrevilegeID: string;
    Age: string;
    CountryName: string;
    CustCode: string;
    CustCreditLimit: string;
    TIN: string;
    StateCode: string;
    CorporateID: string;
    CorporateBranchID: string;
    EmployeeID: string;
    RegisteredMN: string;
    ProfessionID: string;
    EmpCorpEmailID: string;
    ImageIDPath: string;
    CorpImageIDPath: string;
    ImageIDPath2: string;
    AccHolderName: string,
    Accsalutation: string,
    RepoCustId: string,
    UpdatedDate: string
}



export class CustomerModel {
    ObjID: string;
    CompanyCode: string;
    BranchCode: string;
    ID: number;
    CustName: string;
    Address1: string;
    Address2: string;
    Address3: string;
    City: string;
    State: string;
    Pincode: string;
    MobileNo: string;
    PhoneNo: string;
    DateOfBirth: Date;
    WeddingDate: Date;
    CustomerType: string;
    ObjectStatus: string;
    SpouseName: string;
    ChildName1: string;
    ChildName2: string;
    ChildName3: string;
    SpouseDateOfBirth: Date;
    Child1DateOfBirth: Date;
    Child2DateOfBirth: Date;
    Child3DateOfBirth: Date;
    PANNo: string;
    IDType: string;
    IDDetails: string;
    UpdateOn: Date;
    EmailID: string;
    CreatedDate: Date;
    Salutation: string;
    CountryCode: string;
    LoyaltyID: string;
    ICNo: string;
    PassportNo: string;
    PRNo: string;
    PrevilegeID: string;
    Age: string;
    CountryName: string;
    CustCode: string;
    CustCreditLimit: string;
    TIN: string;
    StateCode: string;
    CorporateID: string;
    CorporateBranchID: string;
    EmployeeID: string;
    RegisteredMN: string;
    ProfessionID: string;
    EmpCorpEmailID: string;
    ImageIDPath: string;
    CorpImageIDPath: string;
    ImageIDPath2: string;
    AccHolderName: string;
    Accsalutation: string;
    RepoCustId: string;
    UpdatedDate: string;
    lstOfProofs: [];
}

export class lstOfProofs {
    objID: string;
    CompanyCode: string;
    BranchCode: string;
    CustID: number;
    SlNo: number;
    Doccode: string="";
    DocName: string;
    DocNo: string;
    DocImage: Blob;
    RepoCustId: number;
    UpdatedDate: string;
    RepDocID: number;
    DocImagePath: string;
}



export class ModelError {
    public index?: number;
    public field: string;
    public description: string;
    constructor(_index: number, _field: string, _description: string) {
        this.index = _index;
        this.field = _field;
        this.description = _description;
    }
}