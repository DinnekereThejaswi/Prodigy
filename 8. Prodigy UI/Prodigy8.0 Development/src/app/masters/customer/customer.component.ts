import { ActivatedRoute } from '@angular/router';
import { Component, OnInit, Input, OnDestroy, OnChanges, ViewChild, ElementRef } from '@angular/core'
import { CustomerService } from '../customer/customer.service';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
import { Router } from '@angular/router';
import { PartialCustomerService } from './partialCustomer.service';
import { FormGroup, FormBuilder, NgForm } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import * as CryptoJS from 'crypto-js';
import { CustomerModel, ModelError, lstOfProofs } from '../customer/customer.model';
import { formatDate } from '@angular/common';
import { ComponentCanDeactivate } from '../../appconfirmation-guard';
import { AppConfigService } from '../../AppConfigService';
import { ToastrService, } from 'ngx-toastr';
import { Http } from '@angular/http';
import { MasterService } from '../../core/common/master.service';
import { DomSanitizer } from '@angular/platform-browser';
import swal from 'sweetalert';
import { Alert } from 'bootstrap';

declare var $: any;

@Component({
  selector: 'app-customer',
  templateUrl: './customer.component.html',
  styleUrls: ['./customer.component.scss'],
  providers: [PartialCustomerService]
})

export class CustomerComponent implements OnInit {
  Searchform: FormGroup;
  CustomerForm: FormGroup;
  UploadDocForm: FormGroup;
  successfulSave: boolean;
  datePickerConfig: Partial<BsDatepickerConfig>;
  errors: string[];
  hasErrors = false;
  id: number = 0;
  EnableDisableCustFields: boolean = false;
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

  constructor(private fb: FormBuilder,
    private _router: Router,
    private _avRoute: ActivatedRoute,
    private _customerService: CustomerService,
    private toastr: ToastrService,
    private _partialService: PartialCustomerService, private _httpClient: Http, private sanitizer: DomSanitizer,
    private _masterService: MasterService, private appConfigService: AppConfigService) {
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
      Salutation: "MR.",
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

    this.Searchform = this.fb.group({
      companyCode: this.ccode,
      branchCode: this.bcode,
      searchParam: null,
      type: null,
    })

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

    this._masterService.getCompanyMaster().subscribe(
      Response => {
        this.CompanyMaster = Response;
        this.CustomerListData.StateCode = this.CompanyMaster.StateCode;
        this.CustomerListData.City = this.CompanyMaster.city;
      }
    )


    this._customerService.cast_arg.subscribe(
      response => {
        this.CustomerListData = response;
        if (this.isEmptyObject(this.CustomerListData) == false && this.CustomerListData.ID != 0) {
          this._customerService.pickCustomer(this.CustomerListData.ID, this.CustomerListData.MobileNo).subscribe(
            response => {
              this.CustomerListData = response;
              if (this.CustomerListData.DateOfBirth != null) {
                this.CustomerListData.DateOfBirth = formatDate(this.CustomerListData.DateOfBirth, 'dd/MM/yyyy', 'en_GB');
              }
              this._customerService.SendCustDataToEstComp(this.CustomerListData);
            }
          )
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
            Salutation: "MR.",
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
          this._customerService.SendCustDataToEstComp(null);
        }
      }
    )

    if (this._router.url == "/repair/repair-delivery" || this._router.url == "/credit-receipt/credit-receipt" || this._router.url == "/salesreturn" || this._router.url == "/salesreturn/ConfirmSalesReturn" || this._router.url == "/orders/OrderReceipt" || this._router.url == "/orders/vieworders" || this._router.url == "/orders/CloseOrder" || this._router.url == "/purchase/purchase-billing") {
      this.EnableDisableCustFields = true;
    }

  }
  ValidateGSTIN(arg) {                        //added validation fro both number or alphabet of gstin last length
    var reggst = new RegExp("^([0-9]){2}([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}([0-9]){1}([a-zA-Z]){1}([a-zA-Z0-9_.-]){1}");
    if (!reggst.test(arg) && arg != '') {
      swal("Warning!", "GST Identification Number is not valid", "warning");
    }
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
  SelecctedId: string;

  IdNumbersOf(arg) {
    this.CustomerIDProofdets.DocNo="";
    this.SelecctedId = arg;
  }

  ValidatePan(str) {
    // var regix = new RegExp("^[A-Z]{5}[0-9]{4}[A-Z]{1}");
    var regix = new RegExp("^[a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}");
    if (regix.test(str) == false) {
      swal("Warning!", "PAN NO foramt should be " + "ABCDEXXXXF" + " this format", "warning");
    }
  }
  ValidateLicense(str){
    var regix = new RegExp("^([a-zA-Z]){2}([0-9]){13}");
    if (regix.test(str) == false) {
      swal("Warning!", "Invalid License Format", "warning");
    }
  }
  ValidatePassport(str) {
    var regix = new RegExp("^[A-PR-WYa-pr-wy][1-9]\\d\\s?\\d{4}[1-9]$");
    if (regix.test(str) == false) {
      swal("Warning!", "Passportforamt should be " + "A2096457" + " this format", "warning");
    }
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

  isEmptyObject(obj) {
    return (obj && (Object.keys(obj).length === 0));
  }


  pickCustDetails: any;

  selectRecord(arg) {
    this._customerService.pickCustomer(arg.ID, arg.MobileNo).subscribe(
      response => {
        this.pickCustDetails = response;
        this.CustomerListData = this.pickCustDetails;
        $("#SearchModal").modal('hide');
        this.Salutation.nativeElement.focus();
      }
    )
  }


  onPageChange(p: number) {
    this.pagenumber = p;
    const skipno = (this.pagenumber - 1) * this.top;
    this.getSearchCustomer(this.top, skipno, this.Searchform);
  }

  onSubmit(form) {
    let formControls = form.controls;
    if (formControls.MobileNo.dirty) {
      this.markFormGroupPristine(form);
      // swal("Warning!", 'Mobile Number Altered', "warning");
    }
    else {
      if (form.value.MobileNo == null || form.value.MobileNo == "") {
        swal("Warning!", 'Please enter Mobile no', "warning");
      }
      else if (this.MobileNoValidations(form.value.MobileNo) == false) {
        swal("Warning!", 'Please enter valid mobile number', "warning");
      }
      else if (form.value.Salutation == "null" || form.value.Salutation == null || form.value.Salutation == "") {
        swal("Warning!", 'Please select Salutation', "warning");
      }
      else if (form.value.CustName == null || form.value.CustName == "") {
        swal("Warning!", 'Please enter Name', "warning");
      }
      else if (form.value.Address1 == null || form.value.Address1 == "") {
        swal("Warning!", 'Please enter the adress1 field', "warning");
      }
      else if (form.value.StateCode == null || form.value.StateCode == "0") {
        swal("Warning!", 'Please select the state field', "warning");
      }
      else if (this.PinCodeValidations(form.value.Pincode) == false) {
        swal("Warning!", 'Please enter valid Pincode', "warning");
      }
      else if (this.EmailIdValidations(form.value.EmailID) == false) {
        swal("Warning!", 'Please enter valid EmailID', "warning");
      }
      else if (this.AlphaNumericValidations(form.value.TIN) == false) {
        swal("Warning!", 'Please enter valid GSTIN', "warning");
      }
      else if (this.AlphaNumericValidations(form.value.IDDetails) == false) {
        swal("Warning!", 'Please enter valid ID No', "warning");
      }
      else if (this.PANValidations(form.value.PANNo) == false) {
        swal("Warning!", 'Please enter valid PAN', "warning");
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
              swal("Sucess!", "Customer Details Added Succcessfully", "success");
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
              swal("Sucess!", "Customer Details Updated Succcessfully", "success");
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


  private markFormGroupPristine(form: FormGroup) {
    Object.values(form.controls).forEach(control => {
      control.markAsPristine();

      if ((control as any).controls) {
        this.markFormGroupPristine(control as FormGroup);
      }
    });
  }


  private markFormGroupDirty(form: FormGroup) {
    Object.values(form.controls).forEach(control => {
      control.markAsDirty();

      if ((control as any).controls) {
        this.markFormGroupDirty(control as FormGroup);
      }
    });
  }

  form60Validations(form) {
    if (form.value.Pincode == null || form.value.Pincode == "") {
      swal("Warning!", 'Please enter PinCode', "warning");
      return false;
    }
    else if (this.PinCodeValidations(form.value.Pincode) == false) {
      swal("Warning!", 'Please enter valid Pincode', "warning");
    }
    else if (form.value.DateOfBirth == null || form.value.DateOfBirth == "") {
      swal("Warning!", 'Please select Date Of Birth', "warning");
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
      swal("Warning!", 'Please select ID type', "warning");
    }
    else if (form.value.DocNo == null || form.value.DocNo == "") {
      swal("Warning!", 'Please enter customer ID document number', "warning");
    }
    else if (form.value.DocCode == "AAD") {
      if (this.AADHARValidations(form.value.DocNo) == false) {
        swal("Warning!", 'Please enter valid aadhar number', "warning");
      }
      else {
        this.pushDatatolstOfProofs(form);
      }
    }
    else if (form.value.DocCode == "PAN") {
      if (this.PANValidations(form.value.DocNo) == false) {
        swal("Warning!", 'Please enter valid PAN', "warning");
      }
      else {
        this.pushDatatolstOfProofs(form);
      }
    }
    else {
      this.pushDatatolstOfProofs(form);
    }
  }

  pagenumber: number = 1;
  top = 10;
  skip = (this.pagenumber - 1) * this.top;

  pushDatatolstOfProofs(form) {
    let data = this.CustomerListData.lstOfProofs.find(x => x.DocCode == form.value.DocCode);
    if (data == null) {
      if (form.value.DocCode == "PAN") {
        this.CustomerListData.PANNo = form.value.DocNo.toUpperCase();
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
      swal("Warning!", 'ID Type already added', "warning");
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


  LoadSearchData() {
    $("#SearchModal").modal('show');
    this.SearchFilterData.type = "MOBILENO";
    this.SearchFilterData.searchParam = "";
    this.NoRecords = false;
  }

  CustDetsByMobNo: any;

  SearchFilterData: any = {
    companyCode: null,
    branchCode: null,
    searchParam: null,
    type: null
  }

  GetDataBySearch(form) {
    if (form.value.type == null) {
      swal("Warning!", 'Please select options from dropdown', "warning");
    }
    else if (form.value.searchParam == null || form.value.searchParam == "") {
      swal("Warning!", 'Please enter the data to search', "warning");
    }
    else {
      this.NoRecords = true;
      this.getSearchCustomer(this.top, this.skip, form);
    }
  }

  getSearchCustomer(top, skip, form) {
    this.SearchFilterData.companyCode = this.ccode;
    this.SearchFilterData.branchCode = this.bcode;
    this._customerService.getSearchCustomer(top, skip, this.SearchFilterData).subscribe(
      response => {
        this.CustomerListData = response;
        if (this.CustomerListData != null) {
          this.getSearchCount(form.value.searchParam, form.value.type);
        }
      }
    );
  }

  NoRecords: boolean = false;

  totalItems: any;

  getSearchCount(UserData, Options) {
    this._customerService.getSearchCount(UserData, Options).subscribe(
      Data => {
        this.totalItems = Data;
        // this.totalItems = this.totalItems.RecordCount;
      }
    )
  }

  clearCustomerListData() {
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
          MobileNo: this.SearchFilterData.searchParam,
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
          Salutation: "MR.",
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
      }
    )
  }

  clear() {
    this.SearchFilterData.type = "MOBILENO";
    this.SearchFilterData.searchParam = "";
    this.NoRecords = false;
    this.CustomerListData = [];
  }

  @ViewChild("Salutation", { static: false }) Salutation: ElementRef;

  getDetsByMobNo() {
    this.Salutation.nativeElement.focus();
    this.SearchFilterData = {
      companyCode: this.ccode,
      branchCode: this.bcode,
      searchParam: this.CustomerListData.MobileNo,
      type: "MOBILENO"
    }

    if (this.MobileNoValidations(this.SearchFilterData.searchParam) == false) {
      swal("Warning!", 'Please enter valid mobile number', "warning");
      this.clearCustomerListData();
    }
    else {
      this._customerService.getSearchCustomer(this.top, this.skip, this.SearchFilterData).subscribe(
        response => {
          this.CustomerListData = response;
          this.markFormGroupPristine(this.CustomerForm);
          if (this.CustomerListData.length > 1) {
            $("#SearchModal").modal('show');
            this._customerService.getSearchCustomer(this.top, this.skip, this.SearchFilterData).subscribe(
              response => {
                this.CustomerListData = response;
                if (this.CustomerListData != null) {
                  this.NoRecords = true;
                  this.getSearchCount(this.SearchFilterData.searchParam, this.SearchFilterData.type);
                }
              },
              (err) => {
                this.clearCustomerListData();
              }
            );
          }
          else if (this.CustomerListData.length == 1) {
            this.CustomerListData = this.CustomerListData[0];
            $("#SearchModal").modal('hide');
            if (this.CustomerListData.DateOfBirth != null) {
              this.CustomerListData.DateOfBirth = formatDate(this.CustomerListData.DateOfBirth, 'dd/MM/yyyy', 'en_GB');
            }
            this._customerService.pickCustomer(this.CustomerListData.ID, this.CustomerListData.MobileNo).subscribe(
              response => {
                this.CustomerListData = response;
                if (this.CustomerListData != null) {
                  this.NoRecords = false;
                  if (this.CustomerListData.DateOfBirth != null) {
                    this.CustomerListData.DateOfBirth = formatDate(this.CustomerListData.DateOfBirth, 'dd/MM/yyyy', 'en_GB');
                  }
                  this.CustomerListData.MobileNo = this.CustomerListData.MobileNo.replace(/\b0+/g, "");
                }
              },
              (err) => {
                this.clearCustomerListData();
              }
            )
          }
          else {
            this.clearCustomerListData();
          }
        },
        (err) => {
          this.clearCustomerListData();
        }
      )
    }
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
        Salutation: "MR.",
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
        Salutation: "MR.",
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