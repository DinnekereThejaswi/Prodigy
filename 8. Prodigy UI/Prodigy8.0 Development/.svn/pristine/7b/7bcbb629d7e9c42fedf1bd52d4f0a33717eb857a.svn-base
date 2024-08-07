import { Component, OnInit, Input, ViewChild, ElementRef, OnChanges } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CustomerModel, ModelError, lstOfProofs } from './../customer.model';
import { CustomerService } from './../customer.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { ToastrService, } from 'ngx-toastr';
import { formatDate } from '@angular/common'
import { Http } from '@angular/http';
import { PartialCustomerService } from '../partialCustomer.service';
import * as CryptoJS from 'crypto-js';
import { AppConfigService } from './../../../AppConfigService';
declare var $: any;
import swal from 'sweetalert';
import { DomSanitizer } from '@angular/platform-browser';
import { MasterService } from '../../../core/common/master.service';

@Component({
  selector: 'app-manage-customer',
  templateUrl: './manage-customer.component.html',
  styleUrls: ['./manage-customer.component.css'],
  providers: [PartialCustomerService]
})
export class ManageCustomerComponent implements OnInit, OnChanges {

  CustomerForm: FormGroup;
  UploadDocForm: FormGroup;
  successfulSave: boolean;
  datePickerConfig: Partial<BsDatepickerConfig>;
  errors: string[];
  hasErrors = false;
  id: number = 0;
  ccode: string = "";
  bcode: string = "";
  password: string;
  apiBaseUrl: string;


  @Input() filterRadioBtns: boolean = false;

  //For ServerSide Validations
  modelErrors: ModelError[];
  radioItems: Array<string>;
  @Input() MobileNo: number = 0;
  today = new Date();
  CustomerListData: any;
  CustomerIDProofdets: any;
  model = { option: 'New Customer' };
  EnableJson: boolean = false;
  // Constructor
  constructor(
    private fb: FormBuilder,
    private _router: Router,
    private _avRoute: ActivatedRoute,
    private _customerService: CustomerService,
    private toastr: ToastrService,
    private _partialService: PartialCustomerService, private _httpClient: Http, private sanitizer: DomSanitizer,
    private _masterService: MasterService, private appConfigService: AppConfigService
  ) {
    this.EnableJson = this.appConfigService.EnableJson;
    this.apiBaseUrl = this.appConfigService.apiBaseUrl;
    this.password = this.appConfigService.Pwd;

    this.getCB();

    this.radioItems = ['New Customer', 'Existing Customer'];

    if (this._avRoute.snapshot.params["id"]) {
      this.id = parseInt(this._avRoute.snapshot.params["id"]);
    }
    this.datePickerConfig = Object.assign({},
      {
        containerClass: 'theme-dark-blue',
        showWeekNumbers: false,
        maxDate: this.today,
        dateInputFormat: 'DD/MM/YYYY'
      });
    this.CustomerListData = {
      ObjID: null,
      ID: 0,
      CompanyCode: this.ccode,
      BranchCode: this.bcode,
      CustName: null,
      TIN: null,
      Address1: null,
      Address2: null,
      Address3: null,
      City: null,
      State: null,
      Pincode: null,
      MobileNo: null,
      PhoneNo: null,
      DateOfBirth: null,
      WeddingDate: null,
      CustomerType: null,
      ObjectStatus: null,
      SpouseName: null,
      ChildName1: null,
      ChildName2: null,
      ChildName3: null,
      SpouseDateOfBirth: null,
      Child1DateOfBirth: null,
      Child2DateOfBirth: null,
      Child3DateOfBirth: null,
      PANNo: null,
      IDType: null,
      IDDetails: null,
      UpdateOn: null,
      EmailID: null,
      CreatedDate: null,
      Salutation: null,
      CountryCode: null,
      LoyaltyID: null,
      ICNo: null,
      PassportNo: null,
      PRNo: null,
      PrevilegeID: null,
      Age: null,
      CountryName: null,
      CustCode: null,
      CustCreditLimit: null,
      StateCode: null,
      CorporateID: null,
      CorporateBranchID: null,
      EmployeeID: null,
      RegisteredMN: null,
      ProfessionID: null,
      EmpCorpEmailID: null,
      ImageIDPath: null,
      CorpImageIDPath: null,
      ImageIDPath2: null,
      AccHolderName: null,
      Accsalutation: null,
      RepoCustId: null,
      UpdatedDate: null,
      lstOfProofs: []
    }

    this.CustomerIDProofdets = {
      objID: null,
      CompanyCode: this.ccode,
      BranchCode: this.bcode,
      CustID: null,
      SlNo: null,
      DocCode: null,
      DocName: null,
      DocNo: null,
      DocImage: null,
      RepoCustId: null,
      UpdatedDate: null,
      RepDocID: null,
      DocImagePath: null,
    }
  }

  getCB() {
    this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
  }


  // CountryList: any;
  // getCountryCode() {
  //   this._partialService.getContryCode().subscribe(
  //     Response => {
  //       this.CountryList = Response;
  //     }
  //   );
  // }

  StateList: any;
  getStateCode() {
    this._partialService.getStateCode().subscribe(
      Response => {
        this.StateList = Response;
      }
    );
  }

  ShowHide: boolean = false;

  IDTypeList: any;
  getIDType() {
    // this._partialService.getIDType().subscribe(
    //   Response => {
    //     this.IDTypeList = Response;
    //   }
    // );
    this._customerService.getIDProof().subscribe(
      Response => {
        this.IDTypeList = Response;
      }
    );
  }

  CompanyMaster: any;

  ngOnInit() {
    //this.open();
    this.getStateCode();
    this.getIDType();
    this.CustomerForm = this.fb.group({
      ObjID: null,
      ID: this.id,
      CompanyCode: null,
      BranchCode: null,
      CustName: null,
      Address1: null,
      Address2: null,
      Address3: null,
      TIN: null,
      City: null,
      State: null,
      Pincode: null,
      MobileNo: null,
      PhoneNo: null,
      DateOfBirth: null,
      WeddingDate: null,
      CustomerType: null,
      ObjectStatus: null,
      SpouseName: null,
      ChildName1: null,
      ChildName2: null,
      ChildName3: null,
      SpouseDateOfBirth: null,
      Child1DateOfBirth: null,
      Child2DateOfBirth: null,
      Child3DateOfBirth: null,
      PANNo: null,
      IDType: null,
      IDDetails: null,
      UpdateOn: null,
      EmailID: null,
      CreatedDate: null,
      Salutation: null,
      CountryCode: null,
      LoyaltyID: null,
      ICNo: null,
      PassportNo: null,
      PRNo: null,
      PrevilegeID: null,
      Age: null,
      CountryName: null,
      CustCode: null,
      CustCreditLimit: null,
      StateCode: null,
      CorporateID: null,
      CorporateBranchID: null,
      EmployeeID: null,
      RegisteredMN: null,
      ProfessionID: null,
      EmpCorpEmailID: null,
      ImageIDPath: null,
      CorpImageIDPath: null,
      ImageIDPath2: null,
      AccHolderName: null,
      Accsalutation: null,
      RepoCustId: null,
      UpdatedDate: null,
      lstOfProofs: []
    });


    this.UploadDocForm = this.fb.group({
      objID: null,
      CompanyCode: null,
      BranchCode: null,
      CustID: null,
      SlNo: null,
      DocCode: null,
      DocName: null,
      DocNo: null,
      DocImage: null,
      RepoCustId: null,
      UpdatedDate: null,
      RepDocID: null,
      DocImagePath: null
    });

    this.errors = [];
    this.modelErrors = [];
    this.hasErrors = false;
    // if (this.id > 0) {
    //   this._customerService.getValueFromId(this.id)
    //     .subscribe((data: CustomerModel) => {
    //       this.CustomerListData = data;
    //     });
    // }

    this._masterService.getCompanyMaster().subscribe(
      Response => {
        this.CompanyMaster = Response;
        this.CustomerListData.StateCode = this.CompanyMaster.StateCode;
        this.CustomerListData.City = this.CompanyMaster.city;
      }
    )
  }

  openDatepicker() {
    $('#datepicker').datepicker({
      timepicker: false,
      format: 'yyyy/mm/dd',
      onClose: (dateText, inst) => {
        this.CustomerForm.controls['date'].setValue(dateText);
      }
    });
  }

  isCheckedForm60: boolean = false;
  isChecked(e) {
    this.isCheckedForm60 = e.target.checked;
    this.CustomerListData.PANNo = null;
  }

  public findErrorByField(modelField: string, index?: number): string {
    const modelErr = this.modelErrors.find(m => (m.field === modelField && m.index === index));
    if (modelErr != null) {
      return modelErr.description;
    } else {
      return '';
    }
  }

  onSubmit(form) {
    if (form.value.MobileNo == null || form.value.MobileNo == "") {
      alert("Please enter Mobile no");
    }
    else if (this.MobileNoValidations(form.value.MobileNo) == false) {
      alert("Please enter valid mobile number");
    }
    else if (form.value.Salutation == "null" || form.value.Salutation == null) {
      alert("Please select Salutation");
    }
    else if (form.value.CustName == null || form.value.CustName == "") {
      alert("Please enter Name");
    }
    else if (form.value.Address1 == null || form.value.Address1 == "") {
      alert("Please enter the adress1 field");
    }
    else if (form.value.StateCode == null || form.value.StateCode == "0") {
      alert("Please select the state field");
    }
    else if (this.PinCodeValidations(form.value.Pincode) == false) {
      alert("Please enter valid Pincode");
    }
    else if (this.EmailIdValidations(form.value.EmailID) == false) {
      alert("Please enter valid EmailID");
    }
    else if (this.AlphaNumericValidations(form.value.TIN) == false) {
      alert("Please enter valid GSTIN");
    }
    else if (this.AlphaNumericValidations(form.value.IDDetails) == false) {
      alert("Please enter valid ID No");
    }
    else if (this.PANValidations(form.value.PANNo) == false) {
      alert("Please enter valid PAN");
    }
    else if (this.isCheckedForm60 == true) {
      if (this.form60Validations(form) == true) {
        this.SubmitCustomerDetails(form);
      }
    }
    else {
      this.SubmitCustomerDetails(form)
    }
  }

  NewCustDets: any;

  SubmitCustomerDetails(form) {
    this.errors = [];
    //POST
    var ans = confirm("Do you want to Save??");
    if (ans) {
      if (form.value.ID === 0) {
        this._customerService.post(this.CustomerListData)
          .subscribe(
            (suc: CustomerModel) => {
              this.successfulSave = true;
              swal("Sucess!", "New Record Added Succcessfully", "success");
              //  this.toastr.success('New Record Added Succcessfully', 'Success!');
              $('#Customer').modal('hide');
              this.model = { option: 'New Customer' };
              this.CustomerListData = suc;
              if (this.CustomerListData.DateOfBirth != null) {
                this.CustomerListData.DateOfBirth = formatDate(this.CustomerListData.DateOfBirth, 'dd/MM/yyyy', 'en_GB');
              }
              this._customerService.SendCustDataToEstComp(this.CustomerListData);
            },
            (err) => {
              this.successfulSave = false;
              this.modelErrors = [];
              if (err.status === 400) {
                this.hasErrors = true;
                // handle validation error
                const validationError = err.error;
                // tslint:disable-next-line:forin
                for (const error in validationError.ModelState) {
                  if (validationError.ModelState[error]) {
                    err = error.toString();
                    this.errors.push(err.substring(err.indexOf('.') + 1) +
                      ' : ' + validationError.ModelState[error]);
                    this.modelErrors.push(
                      new ModelError(null, err.substring(err.indexOf('.') + 1),
                        validationError.ModelState[error].toString()));
                  }
                }
                // this.spinnerService.hide();
              } else {
                this.errors.push('something went wrong!');
              }
            }
          )
      }
      // PUT
      else {
        this.CustomerListData.CompanyCode = this.ccode;
        this.CustomerListData.BranchCode = this.bcode;
        this._customerService.put(this.CustomerListData)
          .subscribe(
            suc => {
              this.NewCustDets = suc;
              this.CustomerListData = this.NewCustDets;
              if (this.CustomerListData.DateOfBirth != null) {
                this.CustomerListData.DateOfBirth = formatDate(this.CustomerListData.DateOfBirth, 'dd/MM/yyyy', 'en_GB');
              }
              this._customerService.SendCustDataToEstComp(this.CustomerListData);
              this.successfulSave = true;
              this.toastr.success('Record Updated Succcessfully', 'Success!');
              $('#Customer').modal('hide');
              this.model = { option: 'New Customer' };

            },
            (err) => {
              this.successfulSave = false;
              this.modelErrors = [];
              if (err.status === 400) {
                this.hasErrors = true;
                // handle validation error
                const validationError = err.error;
                for (const error in validationError.ModelState) {
                  if (validationError.ModelState[error]) {
                    err = error.toString();
                    this.errors.push(err.substring(err.indexOf('.') + 1) +
                      ' : ' + validationError.ModelState[error]);
                    this.modelErrors.push(
                      new ModelError(null, err.substring(err.indexOf('.') + 1),
                        validationError.ModelState[error].toString()));
                  }
                }
              } else {
                this.errors.push('something went wrong!');
              }
            })
      }
    }

  }

  form60Validations(form) {
    if (form.value.Pincode == null || form.value.Pincode == "") {
      alert("Please enter PinCode");
      return false;
    }
    else if (this.PinCodeValidations(form.value.Pincode) == false) {
      alert("Please enter valid Pincode");
    }
    else if (form.value.DateOfBirth == null || form.value.DateOfBirth == "") {
      alert("Please select Date Of Birth");
      return false;
    }
    else {
      return true;
    }
  }

  MobileNoValidations(MobileNo) {
    const mobileRegEx = /^[6-9][0-9]{9}$/g;
    const validMobile = mobileRegEx.test(MobileNo);
    return validMobile;
  }

  PANValidations(PAN) {
    if (PAN != null && PAN != "") {
      if (this.isCheckedForm60 == false) {
        const panRegEx = /^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$/;
        const validPan = panRegEx.test(PAN);
        return validPan;
      }
      else {
        return true;
      }
    }
    else {
      return true;
    }
  }

  AADHARValidations(AADNo) {
    if (AADNo != null && AADNo != "") {
      if (this.isCheckedForm60 == false) {
        const panRegEx = /^(\d{12})$/;
        const validPan = panRegEx.test(AADNo);
        return validPan;
      }
      else {
        return true;
      }
    }
    else {
      return true;
    }
  }

  EmailIdValidations(EmailID) {
    if (EmailID != null && EmailID != "") {
      const emailRegEx = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i;
      const validEmail = emailRegEx.test(EmailID);
      return validEmail;
    }
    else {
      return true;
    }
  }

  PinCodeValidations(PinCode) {
    if (PinCode != null && PinCode != "") {
      const isPinRegEx = /^[0-9]{6}$/g;
      const validPin = isPinRegEx.test(PinCode);
      return validPin;
    }
    else {
      return true;
    }
  }

  AlphaNumericValidations(arg) {
    if (arg != null && arg != "") {
      const isPinRegEx = /^[a-zA-Z0-9]+$/g;
      const validPin = isPinRegEx.test(arg);
      return validPin;
    }
    else {
      return true;
    }
  }

  //File Upload

  selectedFile: File = null;
  path: any;
  fileBrowseUrl1: string;
  fileBrowseUrl2: string;
  thumbnail: any;
  fd: FormData = new FormData();
  fileName: string;
  handleBrowse1(event) {
    this.selectedFile = event.target.files[0];
    this.fd.append('uploadFile', this.selectedFile, this.selectedFile.name);
    this._httpClient.post(this.apiBaseUrl + 'api/masters/UploadDoc2', this.fd).subscribe(
      (Response: any) => {
        let bodyBrowse1 = Response.json()
        this.fileBrowseUrl1 = bodyBrowse1.Path;
        this.fileName = this.selectedFile.name;
      }
    )
  }

  //ends here file upload.

  Add(form) {
    if (form.value.DocCode == null || form.value.DocCode == "") {
      alert("Please select ID type");
    }
    else if (form.value.DocNo == null || form.value.DocNo == "") {
      alert("Please enter customer ID document number");
    }
    else if (form.value.DocCode == "AAD") {
      if (this.AADHARValidations(form.value.DocNo) == false) {
        alert("Please enter valid aadhar number");
      }
      else {
        this.pushDatatolstOfProofs(form);
      }
    }
    else if (form.value.DocCode == "PAN") {
      if (this.PANValidations(form.value.DocNo) == false) {
        alert("Please enter valid PAN");
      }
      else {
        this.pushDatatolstOfProofs(form);
      }
    }
    else {
      this.pushDatatolstOfProofs(form);
    }
  }

  pushDatatolstOfProofs(form) {
    let data = this.CustomerListData.lstOfProofs.find(x => x.DocCode == form.value.DocCode);
    if (data == null) {
      if (form.value.DocCode == "PAN") {
        this.CustomerListData.PANNo = form.value.DocNo;
      }
      form.value.CompanyCode = this.ccode;
      form.value.BranchCode = this.bcode;
      this.CustomerListData.CompanyCode = this.ccode;
      this.CustomerListData.BranchCode = this.bcode;
      form.value.DocImagePath = this.fileBrowseUrl1;
      form.value.DocName = this.fileName;
      form.value.CustID = this.CustomerListData.ID;
      this.CustomerListData.lstOfProofs.push(form.value);
      this.Clear();
    }
    else {
      alert("ID Type already added");
      this.Clear();
    }
  }

  Clear() {
    this.UploadDocForm.reset();
    this.fd = new FormData();
    this.fileBrowseUrl1 = "";
    this.fileName = "";
  }

  Delete(arg, index) {
    var ans = confirm("Do you want to delete?");
    if (ans) {
      if (arg.DocCode == "PAN") {
        this.CustomerListData.PANNo = null;
      }
      this.CustomerListData.lstOfProofs.splice(index, 1);
    }
  }

  CustDetsByMobNo: any;

  getDetsByMobNo() {
    this._customerService.getCustDetsFromMobNo(this.CustomerListData.MobileNo).subscribe(
      response => {
        this.CustomerListData = response;
        if (this.CustomerListData.DateOfBirth != null) {
          this.CustomerListData.DateOfBirth = formatDate(this.CustomerListData.DateOfBirth, 'dd/MM/yyyy', 'en_GB');
          this._customerService.pickCustomer(this.CustomerListData.ID, this.CustomerListData.MobileNo).subscribe(
            response => {
              this.CustomerListData = response;
              this.CustomerListData.DateOfBirth = formatDate(this.CustomerListData.DateOfBirth, 'dd/MM/yyyy', 'en_GB');
            }
          )
        }
      },
      (err) => {
        this._masterService.getCompanyMaster().subscribe(
          Response => {
            this.CompanyMaster = Response;
            this.CustomerListData = {
              ObjID: null,
              ID: 0,
              CompanyCode: this.ccode,
              BranchCode: this.bcode,
              CustName: null,
              TIN: null,
              Address1: null,
              Address2: null,
              Address3: null,
              City: null,
              State: null,
              Pincode: null,
              MobileNo: this.CustomerListData.MobileNo,
              PhoneNo: null,
              DateOfBirth: null,
              WeddingDate: null,
              CustomerType: null,
              ObjectStatus: null,
              SpouseName: null,
              ChildName1: null,
              ChildName2: null,
              ChildName3: null,
              SpouseDateOfBirth: null,
              Child1DateOfBirth: null,
              Child2DateOfBirth: null,
              Child3DateOfBirth: null,
              PANNo: null,
              IDType: null,
              IDDetails: null,
              UpdateOn: null,
              EmailID: null,
              CreatedDate: null,
              Salutation: null,
              CountryCode: null,
              LoyaltyID: null,
              ICNo: null,
              PassportNo: null,
              PRNo: null,
              PrevilegeID: null,
              Age: null,
              CountryName: null,
              CustCode: null,
              CustCreditLimit: null,
              StateCode: null,
              CorporateID: null,
              CorporateBranchID: null,
              EmployeeID: null,
              RegisteredMN: null,
              ProfessionID: null,
              EmpCorpEmailID: null,
              ImageIDPath: null,
              CorpImageIDPath: null,
              ImageIDPath2: null,
              AccHolderName: null,
              Accsalutation: null,
              RepoCustId: null,
              UpdatedDate: null,
              lstOfProofs: []
            }
            this.CustomerListData.StateCode = this.CompanyMaster.StateCode;
            this.CustomerListData.City = this.CompanyMaster.city;
            this._customerService.SendCustDataToEstComp(this.CustomerListData);
          }
        )
      }
    )
  }

  // handleBrowse2(event) {
  //   this.selectedFile = event.target.files[0];
  //   let fd:FormData = new FormData();
  //   fd.append('uploadFile',this.selectedFile,this.selectedFile.name);
  //   this._httpClient.post(AppSettings.API_ENDPOINT +'api/masters/UploadDoc2',fd).subscribe(
  //     (Response:any)=>{
  //       let bodyBrowse2 = Response.json()
  //       this.fileBrowseUrl2 = bodyBrowse2.Path;
  //     }
  //   )   
  // }

  // cancel() {
  //   if (!!this.CustomerForm && this.CustomerForm.dirty) {
  //     var ans = confirm('Do you want to exit transaction');
  //     if (ans) {
  //       if (this.filterRadioBtns) {
  //         $('#Customer').modal('hide');
  //       }
  //       else {
  //         this._router.navigate(["/customer"]);
  //       }
  //     }
  //   }
  //   else {
  //     if (this.filterRadioBtns) {
  //       $('#Customer').modal('hide');
  //     }
  //     else {
  //       this._router.navigate(["/customer"]);
  //     }
  //   }
  // }


  ngOnChanges() {
    if (this.MobileNo == 0 || this.MobileNo == null) {
      this.CustomerForm.reset();
      this.CustomerListData = {
        ObjID: null,
        ID: 0,
        CompanyCode: this.ccode,
        BranchCode: this.bcode,
        CustName: null,
        TIN: null,
        Address1: null,
        Address2: null,
        Address3: null,
        City: null,
        State: null,
        Pincode: null,
        MobileNo: null,
        PhoneNo: null,
        DateOfBirth: null,
        WeddingDate: null,
        CustomerType: null,
        ObjectStatus: null,
        SpouseName: null,
        ChildName1: null,
        ChildName2: null,
        ChildName3: null,
        SpouseDateOfBirth: null,
        Child1DateOfBirth: null,
        Child2DateOfBirth: null,
        Child3DateOfBirth: null,
        PANNo: null,
        IDType: null,
        IDDetails: null,
        UpdateOn: null,
        EmailID: null,
        CreatedDate: null,
        Salutation: null,
        CountryCode: null,
        LoyaltyID: null,
        ICNo: null,
        PassportNo: null,
        PRNo: null,
        PrevilegeID: null,
        Age: null,
        CountryName: null,
        CustCode: null,
        CustCreditLimit: null,
        StateCode: null,
        CorporateID: null,
        CorporateBranchID: null,
        EmployeeID: null,
        RegisteredMN: null,
        ProfessionID: null,
        EmpCorpEmailID: null,
        ImageIDPath: null,
        CorpImageIDPath: null,
        ImageIDPath2: null,
        AccHolderName: null,
        Accsalutation: null,
        RepoCustId: null,
        UpdatedDate: null,
        lstOfProofs: []
      }
    }
    else {
      this.CustomerListData = {
        ObjID: null,
        ID: 0,
        CompanyCode: this.ccode,
        BranchCode: this.bcode,
        CustName: null,
        TIN: null,
        Address1: null,
        Address2: null,
        Address3: null,
        City: null,
        State: null,
        Pincode: null,
        MobileNo: this.MobileNo,
        PhoneNo: null,
        DateOfBirth: null,
        WeddingDate: null,
        CustomerType: null,
        ObjectStatus: null,
        SpouseName: null,
        ChildName1: null,
        ChildName2: null,
        ChildName3: null,
        SpouseDateOfBirth: null,
        Child1DateOfBirth: null,
        Child2DateOfBirth: null,
        Child3DateOfBirth: null,
        PANNo: null,
        IDType: null,
        IDDetails: null,
        UpdateOn: null,
        EmailID: null,
        CreatedDate: null,
        Salutation: null,
        CountryCode: null,
        LoyaltyID: null,
        ICNo: null,
        PassportNo: null,
        PRNo: null,
        PrevilegeID: null,
        Age: null,
        CountryName: null,
        CustCode: null,
        CustCreditLimit: null,
        StateCode: null,
        CorporateID: null,
        CorporateBranchID: null,
        EmployeeID: null,
        RegisteredMN: null,
        ProfessionID: null,
        EmpCorpEmailID: null,
        ImageIDPath: null,
        CorpImageIDPath: null,
        ImageIDPath2: null,
        AccHolderName: null,
        Accsalutation: null,
        RepoCustId: null,
        UpdatedDate: null,
        lstOfProofs: []
      }
      this.getDetsByMobNo();
    }
  }
}