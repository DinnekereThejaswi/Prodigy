import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MastersService } from '../masters.service';
import * as CryptoJS from 'crypto-js';
import swal from 'sweetalert';
import { ProductTreeModel } from '../masters.model';
import { AppConfigService } from '../../AppConfigService';
import { EditGSentrymodel } from '../masters.model';

import { Router } from '@angular/router';
declare var $: any;

@Component({
    selector: 'app-product-tree',
    templateUrl: './product-tree.component.html',
    styleUrls: ['./product-tree.component.css']
})
export class ProductTreeComponent implements OnInit {
    uom = [{ "code": "W", "name": "weight" },
    { "code": "P", "name": "piece" },
    { "code": "C", "name": "Carat" }]

    ccode: string;
    bcode: string;
    password: string;
    EnableJson: boolean = false;
    chkbxchked: boolean = false;
    ////formfroup
    CategoryForm: FormGroup;
    ItemForm: FormGroup;
    EditGsEntryFrom: FormGroup;
    /////////

    //////For Model/
    ProductTReeListData: ProductTreeModel = {
        ObjID: '',
        CompanyCode: '',
        BranchCode: '',
        ItemLevel1ID: 0,
        ItemLevel1Name: null,
        GSCode: null,
        ItemLevel2Id: 0,
        ItemLevel2Name: null,
        ItemLevel3Id: 0,
        ItemLevel3Name: null,
        ItemLevel4Id: 0,
        ItemLevel4Name: null,
        ItemLevel5Id: 0,
        ItemLevel5Name: null,
        ItemLevel6Id: null,
        ItemLevel6Name: null,
        ShortDescription: null,
        IsChild: null,
        MinUnits: null,
        MinStockLevel: null,
        CatalogId: null,
        Grade: null,
        Tagged: null,
        Stone: null,
        Diamond: null,
        CounterCode: null,
        karat: null,
        PieceItem: null,
        ObjStatus: null,
        AliasName: null,
        MinProfitPercent: null,
        QtyLock: null,
        Hallmark: null,
        Certification: null,
        TcsPerc: null,
        GSTGoodsGroupCode: null,
        GSTServicesGroupCode: null,
        HSN: null,
        CounterList: [{
            ObjID: '',
            CompanyCode: '',
            BranchCode: '',
            GSCode: '',
            ItemName: '',
            CounterCode: '',
        }],
    }
    ///gs  editinng
    GSObjId: string = null;
    //////////////// lockInput: boolean = true;
    isReadOnly: boolean = false;
    EnableAdd: boolean = true;
    EnableSave: boolean = false;
    // EnableAdd: boolean = true;
    // EnableSave: boolean = true; //f
    // isReadOnly: boolean = false; //f
    /////////////////gs modificaion\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

    GsEntryListData: EditGSentrymodel = {
        ObjID: "",
        CompanyCode: '',
        BranchCode: '',
        ItemLevel1ID: 0,
        ItemLevel1Name: null,
        GsCode: '',
        MeasureType: '',
        MetalType: '',
        Karat: '',
        BillType: '',
        Tax: 0,
        OpeningUnits: 0,
        OpeningGwt: 0,
        OpeningNwt: 0,
        OpeningGwtValue: 0,
        OpeningNwtValue: 0,
        ObjectStatus: '',
        DisplayOrder: 0,
        CommodityCode: '',
        ExciseDuty: 0,
        EduCess: 0,
        HighEle: 0,
        Tcs: 0,
        IsStone: '',
        Purity: 0,
        TcsPerc: 0,
        STax: 0,
        CTax: 0,
        ITax: 0,
        GSTGoodsGroupCode: '',
        GSTServicesGroupCode: '',
        HSN: ''
    }
    ////////////////////////////////////
    constructor(private fb: FormBuilder, private _mastersService: MastersService,
        private _appConfigService: AppConfigService,
        private _router: Router) {
        this.password = this._appConfigService.Pwd;
        this.EnableJson = this._appConfigService.EnableJson;
        this.getCB();
    }
    getCB() {
        this.ccode = CryptoJS.AES.decrypt(localStorage.getItem('ccode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
        this.bcode = CryptoJS.AES.decrypt(localStorage.getItem('bcode'), this.password.trim()).toString(CryptoJS.enc.Utf8);
    }

    config = {
        displayKey: "CounterName", //if objects array passed which key to be displayed defaults to description
        search: false,//true/false for the search functionlity defaults to false,
        height: 'auto', //height of the list so that if there are more no of items it can show a scroll defaults to auto. With auto height scroll will never appear
        placeholder: 'Select', // text to be displayed when no item is selected defaults to Select,
        customComparator: () => { }, // a custom function using which user wants to sort the items. default is undefined and Array.sort() will be used in that case,
        limitTo: 0, // number thats limits the no of options displayed in the UI (if zero, options will not be limited)
        moreText: 'more', // text to be displayed whenmore than one items are selected like Option 1 + 5 more
        noResultsFound: 'No results found!',// text to be displayed when no items are found while searching
        searchPlaceholder: 'Search', // label thats displayed in search input,
        searchOnKey: 'name', // key on which search should be performed this will be selective search. if undefined this will be extensive search on all keys
        clearOnSelection: false, // clears search criteria when an option is selected if set to true, default is false
        inputDirection: 'ltr', // the direction of the search input can be rtl or ltr(default)
    }
    SelectedCounters: {};
    ngOnInit() {
        this.getItemListGroup();
        this.GetGSListItem();
        this.getKaratData();
        this.GoodsType();
        this.ServiceTypeList();
        this.hsnList();
        this.GetAllCounter();
        this.getStockList();
        this.SelectedCounters;
        this.counterList;
        this.GetAllDefaultcounterList();
        this.CategoryForm = this.fb.group({
            frmCtrl_ItemCode: null,
            frmCtrl_ItemName: null,
            frmCtrl_Karat: "NA",
            frmCtrl_minQty: null,
            frmCtrl_LockQty: null,
            frmCtrl_TcsPercent: null
        });
        this.ItemForm = this.fb.group({
            frmCtrl_ItemCode: null,
            frmCtrl_ItemName: null,
            frmCtrl_Karat: "NA",
            frmCtrl_minQty: null,
            frmCtrl_LockQty: null,
            frmCtrl_TcsPercent: null,
            ffrmCtrl_MinMc: null,
            ffrmCtrl_ImgId: null,
            frmCtrl_goodsType: null,
            frmCtrl_serviceTpe: null,
            frmCtrl_Hsn: null,
            frmCtrl_Counter: "M",
            frmCtrl_DefltCounter: null,
            frmCtrl_iSClosed: null,
            frmCtrl_iSTagged: null,
            frmCtrl_iSStone: null,
            frmCtrl_iSDiamond: null,
            frmCtrl_isPieceItem: null,
            frmCtrl_isHallMarked: null,
            frmCtrl_isCertification: null
        });
        this.EditGsEntryFrom = this.fb.group({
            FormGSCode: ["", Validators.required],
            FormGSName: ["", Validators.required],
            Formkarat: ["", Validators.required],
            FormMeasureType: ["", Validators.required],
            FormStockName: ["", Validators.required],
            FormPurtiy: ["", Validators.required],
            FormIsStoneAttached: ["", Validators.required],
            FormIsClosed: ["", Validators.required],
            FormGoodsTye: ["", Validators.required],
            FormServiceType: ["", Validators.required],
            FormHsnCode: ["", Validators.required],
            FormOpnPiece: ["", Validators.required],
            FormOpnValue: ["", Validators.required],
            FormOpnGWT: ["", Validators.required],
            FormOpnNWT: ["", Validators.required],
            FormSEQNo: ["", Validators.required],
            FormCommodityCode: ["", Validators.required],
            FormIsSales: ["", Validators.required],
            FormIsPurchase: ["", Validators.required],
            FormIsRepair: ["", Validators.required],
            FormIsPacking: ["", Validators.required],
            FormIsGift: ["", Validators.required],

        });

    }
    ItemList: any = []
    getItemListGroup() {
        // alert('loading all product tree details');
        this._mastersService.GetAllItemsList().subscribe(
            Response => {
                this.ItemList = Response;
                // console.log(this.ItemList);
            }
        )
    }
    sorting() {
        this.ItemList.sort(function (a, b) {
            return a.ItemLevelID - b.ItemLevelID;
        }
        )

    }
    GSList: any = [];
    GetGSListItem() {
        this._mastersService.GetGSListItems().subscribe(
            Response => {
                this.GSList = Response;
            }
        )
    }

    goodsType: any = [];
    GoodsType() {
        this._mastersService.GetGoodsType().subscribe(
            Response => {
                this.goodsType = Response;

            })
    }
    serviceType: any = [];
    ServiceTypeList() {
        this._mastersService.getServiceType().subscribe(
            Response => {
                this.serviceType = Response;
            })
    }
    HSN: any = [];
    hsnList() {
        this._mastersService.getHSN().subscribe(
            Response => {
                this.HSN = Response;
            })
    }
    KaratList: any = [];
    getKaratData() {
        this._mastersService.GETKaratsToTable().subscribe(
            Response => {
                this.KaratList = Response;
                // console.log( this.KaratList );
            })
    }
    counterList: any = [];
    GetAllCounter() {
        this._mastersService.getCounterList().subscribe(
            Response => {
                this.counterList = Response;
                // console.log( this.counterList);
            })
    }
    DefaultcounterList: any = [];
    GetAllDefaultcounterList() {
        this._mastersService.getCounterList().subscribe(
            Response => {
                this.DefaultcounterList = Response;
                // console.log( this.counterList);
            })
    }
    CatagoryId: number; //to log cat id
    Itemlevel1Id: number; //to log id
    Itemlevel1Name: string; //to log name
    ItemLevel1GsCode: string // to log gs code
    ItemIndexId: number; //to log index
    AddNewCategory(levelId: number, LevelName: string, gsCode: string, index: number, arg) {
        this.ProductTReeListData = {
            ObjID: arg.ObjID,
            CompanyCode: this.ccode,
            BranchCode: this.bcode,
            ItemLevel1ID: arg.ItemLevel1ID,
            ItemLevel1Name: arg.ItemLevel1Name,
            GSCode: arg.GSCode,
            ItemLevel2Id: 0,
            ItemLevel2Name: null,
            ItemLevel3Id: 0,
            ItemLevel3Name: null,
            ItemLevel4Id: 0,
            ItemLevel4Name: null,
            ItemLevel5Id: 0,
            ItemLevel5Name: null,
            ItemLevel6Id: null,
            ItemLevel6Name: null,
            ShortDescription: null,
            IsChild: null,
            MinUnits: null,
            MinStockLevel: null,
            CatalogId: null,
            Grade: null,
            Tagged: null,
            Stone: null,
            Diamond: null,
            CounterCode: null,
            karat: null,
            PieceItem: null,
            ObjStatus: null,
            AliasName: null,
            MinProfitPercent: null,
            QtyLock: null,
            Hallmark: null,
            Certification: null,
            TcsPerc: null,
            GSTGoodsGroupCode: null,
            GSTServicesGroupCode: null,
            HSN: null,
            CounterList: [{
                ObjID: '',
                CompanyCode: '',
                BranchCode: '',
                GSCode: '',
                ItemName: '',
                CounterCode: '',

            }],
        }
        this.AddCategoryModel();
    }
    AddCategoryModel() {
        this.ProductTReeListData.karat = "NA";
        this.isReadOnly = false;
        $('#CategoryModel').modal('show');
    }

    SaveLevel2Item(form) {
        var name: string;
        name = form.value.frmCtrl_ItemName
        if (form.value.frmCtrl_ItemCode == null || form.value.frmCtrl_ItemCode == "") {
            swal("Warning!", "Please enter Itemcode", "warning");
        }
        else if (form.value.frmCtrl_ItemName == null || form.value.frmCtrl_ItemName == "") {
            swal("Warning!", "Please select the Item Name", "warning");
        }
        else if (form.value.frmCtrl_Karat == null || form.value.frmCtrl_Karat == "") {
            swal("Warning!", "Please select the Karat", "warning");
        }
        else if (form.value.frmCtrl_minQty == null || form.value.frmCtrl_minQty == "") {
            swal("Warning!", "Please select the  Minimum Quantity", "warning");
        }
        else {
            this.ProductTReeListData.CounterList = [{
                ObjID: '',
                CompanyCode: '',
                BranchCode: '',
                GSCode: '',
                ItemName: '',
                CounterCode: '',

            }];
            var ans = confirm("Do you want to Add ??");
            if (ans) {
                // var Print =JSON.stringify(this.ProductTReeListData);
                // console.log(Print);
                this._mastersService.PostCategoryItem(this.ProductTReeListData).subscribe(
                    response => {

                        swal("Saved!", "Saved " + name + " Saved", "success");
                        this.isReadOnly = false;
                        this.CategoryForm.reset();
                        this.ProductTReeListData = {
                            ObjID: '',
                            CompanyCode: '',
                            BranchCode: '',
                            ItemLevel1ID: 0,
                            ItemLevel1Name: null,
                            GSCode: null,
                            ItemLevel2Id: 0,
                            ItemLevel2Name: null,
                            ItemLevel3Id: 0,
                            ItemLevel3Name: null,
                            ItemLevel4Id: 0,
                            ItemLevel4Name: null,
                            ItemLevel5Id: 0,
                            ItemLevel5Name: null,
                            ItemLevel6Id: null,
                            ItemLevel6Name: null,
                            ShortDescription: null,
                            IsChild: null,
                            MinUnits: null,
                            MinStockLevel: null,
                            CatalogId: null,
                            Grade: null,
                            Tagged: null,
                            Stone: null,
                            Diamond: null,
                            CounterCode: null,
                            karat: null,
                            PieceItem: null,
                            ObjStatus: null,
                            AliasName: null,
                            MinProfitPercent: null,
                            QtyLock: null,
                            Hallmark: null,
                            Certification: null,
                            TcsPerc: null,
                            GSTGoodsGroupCode: null,
                            GSTServicesGroupCode: null,
                            HSN: null,
                            CounterList: [{
                                ObjID: '',
                                CompanyCode: '',
                                BranchCode: '',
                                GSCode: '',
                                ItemName: '',
                                CounterCode: ''
                            }]
                        }
                        // console.log( this.ProductTReeListData);
                        this.getItemListGroup();
                    },
                    (err) => {
                        if (err.status === 400) {
                            const validationError = err.error.description;
                            swal("Warning!", validationError, "warning");
                        }
                        else {
                            this.errors.push('something went wrong!');
                        }
                    }
                )
            }
        }
    }
    refreshPage() {
        window.location.reload();
    }
    Itemlevel2Id: number;
    Itemlevel2Name: string;
    ItemLevel2GsCode: string
    ItemLevel2IndexId: number;
    AddNewItem(Itemlevel2Id: number, Itemlevel2Name: string, ItemLevel2IndexId: number, ItemLevel2GsCode: string, arg) {
        // alert('adding new item to the category');
        ///to check is ProductTReeListData model is empty or not
        // var Print =JSON.stringify(this.ProductTReeListData);
        // console.log(Print);
        ////
        this.ProductTReeListData = {
            ObjID: arg.ObjID,
            CompanyCode: this.ccode,
            BranchCode: this.bcode,
            ItemLevel1ID: arg.ItemLevel1ID,
            ItemLevel1Name: arg.ItemLevel1Name,
            GSCode: arg.GSCode,
            ItemLevel2Id: arg.ItemLevel2Id,
            ItemLevel2Name: arg.ItemLevel2Name,
            ItemLevel3Id: 0,
            ItemLevel3Name: null,
            ItemLevel4Id: 0,
            ItemLevel4Name: null,
            ItemLevel5Id: 0,
            ItemLevel5Name: null,
            ItemLevel6Id: null,
            ItemLevel6Name: null,
            ShortDescription: null,
            IsChild: null,
            MinUnits: null,
            MinStockLevel: null,
            CatalogId: null,
            Grade: null,
            Tagged: null,
            Stone: null,
            Diamond: null,
            CounterCode: null,
            karat: null,
            PieceItem: null,
            ObjStatus: null,
            AliasName: null,
            MinProfitPercent: null,
            QtyLock: null,
            Hallmark: null,
            Certification: null,
            TcsPerc: null,
            GSTGoodsGroupCode: null,
            GSTServicesGroupCode: null,
            HSN: null,
            CounterList: [{
                ObjID: '',
                CompanyCode: '',
                BranchCode: '',
                GSCode: '',
                ItemName: '',
                CounterCode: '',
            }],
        }
        this.AddNewItemModel();
    }
    AddNewItemModel() {
        this.ItemForm.reset();
        this.ProductTReeListData.karat = "NA";
        this.ProductTReeListData.CounterCode = "NA";
        //to check is ProductTReeListData model is after assgning level 1 and 2 details to it
        // var Print =JSON.stringify(this.ProductTReeListData);
        // console.log(Print);
        $('#AddItemModel').modal('show');
    }
    IsQuantityLock(e) {
        if (e.target.checked == true) {
            this.ProductTReeListData.QtyLock = "Y";
        }
        if (e.target.checked == false) {

            this.ProductTReeListData.QtyLock = "N";
        }
    }
    errors = [];
    SaveLevel3Item(form) {
        var name: string;
        name = form.value.frmCtrl_ItemName;
        if (form.value.frmCtrl_ItemCode == null || form.value.frmCtrl_ItemCode == "") {
            swal("Warning!", "Please Entert the ItemCode", "warning");
        }
        else if (form.value.frmCtrl_ItemName == null || form.value.frmCtrl_ItemName == "") {
            swal("Warning!", "Please Entert the ItemName", "warning");
        }
        else if (form.value.frmCtrl_Karat == null || form.value.frmCtrl_Karat == "") {
            swal("Warning!", "Please Entert the Karat", "warning");
        }
        else if (form.value.frmCtrl_minQty == null || form.value.frmCtrl_minQty == "") {
            swal("Warning!", "Please Entert the Qty", "warning");
        }

        else if (form.value.frmCtrl_goodsType == null || form.value.frmCtrl_goodsType == "") {
            swal("Warning!", "Please select the GoodsType", "warning");
        }
        else if (form.value.frmCtrl_serviceTpe == null || form.value.frmCtrl_serviceTpe == "") {
            swal("Warning!", "Please select the ServiceTpe", "warning");
        }
        else if (form.value.frmCtrl_Hsn == null || form.value.frmCtrl_Hsn == "") {
            swal("Warning!", "Please select the HSN", "warning");
        }
        else if (form.value.frmCtrl_DefltCounter == null || form.value.frmCtrl_DefltCounter == "") {
            swal("Warning!", "Please select the Default Counter", "warning");
        }
        else if (form.value.frmCtrl_Counter == null || form.value.frmCtrl_Counter == "") {
            swal("Warning!", "Please select the Counter", "warning");
        }
        else {
            var ans = confirm("Do you want to save??");
            // var obj= JSON.stringify(this.ProductTReeListData);
            // console.log(obj);
            this._mastersService.PostCategoryItem(this.ProductTReeListData).subscribe(
                response => {
                    swal("Saved!", "Saved " + name + " Saved", "success");
                    this.isReadOnly = false;
                    this.ItemForm.reset();
                    this.ProductTReeListData = {
                        ObjID: '',
                        CompanyCode: '',
                        BranchCode: '',
                        ItemLevel1ID: 0,
                        ItemLevel1Name: null,
                        GSCode: null,
                        ItemLevel2Id: 0,
                        ItemLevel2Name: null,
                        ItemLevel3Id: 0,
                        ItemLevel3Name: null,
                        ItemLevel4Id: 0,
                        ItemLevel4Name: null,
                        ItemLevel5Id: 0,
                        ItemLevel5Name: null,
                        ItemLevel6Id: null,
                        ItemLevel6Name: null,
                        ShortDescription: null,
                        IsChild: null,
                        MinUnits: null,
                        MinStockLevel: null,
                        CatalogId: null,
                        Grade: null,
                        Tagged: null,
                        Stone: null,
                        Diamond: null,
                        CounterCode: null,
                        karat: null,
                        PieceItem: null,
                        ObjStatus: null,
                        AliasName: null,
                        MinProfitPercent: null,
                        QtyLock: null,
                        Hallmark: null,
                        Certification: null,
                        TcsPerc: null,
                        GSTGoodsGroupCode: null,
                        GSTServicesGroupCode: null,
                        HSN: null,
                        CounterList: [{
                            ObjID: '',
                            CompanyCode: '',
                            BranchCode: '',
                            GSCode: '',
                            ItemName: '',
                            CounterCode: ''
                        }]
                    }
                    // console.log( this.ProductTReeListData);
                    this.getItemListGroup();
                },
                (err) => {
                    if (err.status === 400) {
                        const validationError = err.error.description;
                        swal("Warning!", validationError, "warning");
                    }
                    else {
                        this.errors.push('something went wrong!');
                    }
                }
            )
        }
    }
    level2ObjID: number;
    categoryName: string;
    DeleteCategory(level2ObjID: number, categoryName: string, arg) {
        var myObj = arg.innerLevelItems;
        if (myObj.length != 0) {
            swal("Warning!", "Cant Delete This node", "warning");
            return false;
        }
        else {
            this.ProductTReeListData = arg;
        }
        var ans = confirm("Do you want to Delete ??" + arg.AliasName);
        if (ans) {
            this._mastersService.DeleteCategoryByPostBody(this.ProductTReeListData).subscribe(
                response => {
                    swal("Deleted!", "Deleted " + this.ProductTReeListData.AliasName + " Deleted", "success");
                    this.getItemListGroup();
                },
                (err) => {
                    if (err.status === 400) {
                        const validationError = err.error.description;
                        swal("Warning!", validationError, "warning");
                    }
                    else {
                        this.errors.push('something went wrong!');
                    }
                    this.CategoryForm.reset();
                }
            )
        }
    }
    level3ObjID: number;
    level3Name: string;
    DeleteItem(level3ObjID: number, level3Name: string, arg) {
        this.ProductTReeListData = arg;
        var ans = confirm("Do you want to Delete ??" + arg.AliasName);
        if (ans) {
            this._mastersService.DeleteCategoryByPostBody(this.ProductTReeListData).subscribe(
                response => {
                    swal("Deleted!", "Deleted " + this.ProductTReeListData.AliasName + " Deleted", "success");
                    this.getItemListGroup();
                },
                (err) => {
                    if (err.status === 400) {
                        const validationError = err.error.description;
                        swal("Warning!", validationError, "warning");
                    }
                    else {
                        this.errors.push('something went wrong!');
                    }
                    this.getItemListGroup();
                }

            )
        }
        this.getItemListGroup();

    }

    AddItemModelModalClose() {
        // alert('AddItemModelModalClose');
        if (this.ItemForm.pristine == true) {
            // alert(this.ItemForm.pristine);
            $('#AddItemModel').modal('hide');
            this.getItemListGroup();
            this.ItemForm.reset();
        }
        else if (this.ItemForm.pristine == false) {
            // alert(this.ItemForm.pristine);
            $('#AddItemModel').modal('hide');
            this.ItemForm.reset();
            this.isReadOnly = false;
            this.ProductTReeListData = {
                ObjID: '',
                CompanyCode: '',
                BranchCode: '',
                ItemLevel1ID: 0,
                ItemLevel1Name: null,
                GSCode: null,
                ItemLevel2Id: 0,
                ItemLevel2Name: null,
                ItemLevel3Id: 0,
                ItemLevel3Name: null,
                ItemLevel4Id: 0,
                ItemLevel4Name: null,
                ItemLevel5Id: 0,
                ItemLevel5Name: null,
                ItemLevel6Id: null,
                ItemLevel6Name: null,
                ShortDescription: null,
                IsChild: null,
                MinUnits: null,
                MinStockLevel: null,
                CatalogId: null,
                Grade: null,
                Tagged: null,
                Stone: null,
                Diamond: null,
                CounterCode: null,
                karat: null,
                PieceItem: null,
                ObjStatus: null,
                AliasName: null,
                MinProfitPercent: null,
                QtyLock: null,
                Hallmark: null,
                Certification: null,
                TcsPerc: null,
                GSTGoodsGroupCode: null,
                GSTServicesGroupCode: null,
                HSN: null,
                CounterList: [{
                    ObjID: '',
                    CompanyCode: '',
                    BranchCode: '',
                    GSCode: '',
                    ItemName: '',
                    CounterCode: ''
                }]
            },
                // alert('item reload ');
                this._mastersService.GetAllItemsList().subscribe(
                    Response => {
                        this.ItemList = Response;
                        // console.log(this.ItemList);
                    }
                )
        }

    }

    CategoryModelModalClose() {
        // alert('CategoryModelModalClose');
        if (this.CategoryForm.pristine == true) {
            // alert(this.CategoryForm.pristine);
            $('#CategoryModel').modal('hide');
            this.getItemListGroup();
            this.CategoryForm.reset();
        }
        else if (this.CategoryForm.pristine == false) {
            // alert(this.CategoryForm.pristine);
            $('#CategoryModel').modal('hide');
            this.CategoryForm.reset();
            this.isReadOnly = false;
            this.ProductTReeListData = {
                ObjID: '',
                CompanyCode: '',
                BranchCode: '',
                ItemLevel1ID: 0,
                ItemLevel1Name: null,
                GSCode: null,
                ItemLevel2Id: 0,
                ItemLevel2Name: null,
                ItemLevel3Id: 0,
                ItemLevel3Name: null,
                ItemLevel4Id: 0,
                ItemLevel4Name: null,
                ItemLevel5Id: 0,
                ItemLevel5Name: null,
                ItemLevel6Id: null,
                ItemLevel6Name: null,
                ShortDescription: null,
                IsChild: null,
                MinUnits: null,
                MinStockLevel: null,
                CatalogId: null,
                Grade: null,
                Tagged: null,
                Stone: null,
                Diamond: null,
                CounterCode: null,
                karat: null,
                PieceItem: null,
                ObjStatus: null,
                AliasName: null,
                MinProfitPercent: null,
                QtyLock: null,
                Hallmark: null,
                Certification: null,
                TcsPerc: null,
                GSTGoodsGroupCode: null,
                GSTServicesGroupCode: null,
                HSN: null,
                CounterList: [{
                    ObjID: '',
                    CompanyCode: '',
                    BranchCode: '',
                    GSCode: '',
                    ItemName: '',
                    CounterCode: ''
                }]
            },
                // alert('cat ');
                this._mastersService.GetAllItemsList().subscribe(
                    Response => {
                        this.ItemList = Response;
                        // console.log(this.ItemList);
                    }
                )

        }

    }

    IsTagged(e) {
        if (e.target.checked == true) {
            this.ProductTReeListData.Tagged = "Y";
        }
        if (e.target.checked == false) {
            this.ProductTReeListData.Tagged = "N";
        }
    }
    IsStone(e) {
        if (e.target.checked == true) {
            this.ProductTReeListData.Stone = "Y";
        }
        if (e.target.checked == false) {
            this.ProductTReeListData.Stone = "N";
        }
    }
    IsDiamond(e) {
        if (e.target.checked == true) {
            this.ProductTReeListData.Diamond = "Y";
        }
        if (e.target.checked == false) {
            this.ProductTReeListData.Diamond = "N";
        }
    }
    IsPieceItem(e) {
        if (e.target.checked == true) {
            this.ProductTReeListData.PieceItem = "Y";
        }
        if (e.target.checked == false) {
            this.ProductTReeListData.PieceItem = "N";
        }
    }
    IsHallmark(e) {
        if (e.target.checked == true) {
            this.ProductTReeListData.Hallmark = "Y";
        }
        if (e.target.checked == false) {
            this.ProductTReeListData.Hallmark = "N";
        }
    }
    IsCertification(e) {
        if (e.target.checked == true) {
            this.ProductTReeListData.Certification = "Y";
        }
        if (e.target.checked == false) {
            this.ProductTReeListData.Certification = "N";
        }
    }
    EditCategory(arg, index) {
        this.EnableAdd = false;
        this.EnableSave = true;
        this.isReadOnly = true;
        //to confrim anything aare binded to model
        // var print=JSON.stringify(this.ProductTReeListData);
        // console.log(print);
        //////
        ////loging all the arguemnts
        // var print=JSON.stringify(arg[index]);
        // console.log(print);
        this.ProductTReeListData = {
            ObjID: arg[index].ObjID,
            CompanyCode: this.ccode,
            BranchCode: this.bcode,
            ItemLevel1ID: arg[index].ItemLevel1ID,
            ItemLevel1Name: arg[index].ItemLevel1Name,
            GSCode: arg[index].GSCode,
            ItemLevel2Id: arg[index].ItemLevel2Id,
            ItemLevel2Name: arg[index].ItemLevel2Name,
            ItemLevel3Id: 0,
            ItemLevel3Name: null,
            ItemLevel4Id: 0,
            ItemLevel4Name: null,
            ItemLevel5Id: 0,
            ItemLevel5Name: null,
            ItemLevel6Id: null,
            ItemLevel6Name: null,
            ShortDescription: null,
            IsChild: null,
            MinUnits: arg[index].MinUnits,
            MinStockLevel: null,
            CatalogId: null,
            Grade: null,
            Tagged: null,
            Stone: null,
            Diamond: null,
            CounterCode: null,
            karat: arg[index].karat,
            PieceItem: null,
            ObjStatus: null,
            AliasName: arg[index].AliasName,
            MinProfitPercent: null,
            QtyLock: arg[index].QtyLock,
            Hallmark: null,
            Certification: null,
            TcsPerc: arg[index].TcsPerc,
            GSTGoodsGroupCode: null,
            GSTServicesGroupCode: null,
            HSN: null,
            CounterList: [{
                ObjID: '',
                CompanyCode: '',
                BranchCode: '',
                GSCode: '',
                ItemName: '',
                CounterCode: '',

            }],
        };
        //  var print=JSON.stringify(this.ProductTReeListData);
        // console.log(print);
        this.AddCategoryModel();

    }
    ModifiedCategory(form) {
        if (form.value.frmCtrl_ItemCode == null || form.value.frmCtrl_ItemCode == "") {
            swal("Warning!", "Please enter Itemcode", "warning");
        }
        else if (form.value.frmCtrl_ItemName == null || form.value.frmCtrl_ItemName == "") {
            swal("Warning!", "Please enter Item Name", "warning");
        }
        else if (form.value.frmCtrl_Karat == null || form.value.frmCtrl_Karat == "") {
            swal("Warning!", "Please select the Karat", "warning");
        }
        else if (form.value.frmCtrl_minQty == null || form.value.frmCtrl_minQty == "") {
            swal("Warning!", "Please enter Minimum Quantity", "warning");
        }
        else {
            // var obj= JSON.stringify(this.ProductTReeListData);
            // console.log(obj);
            var ans = confirm("Do you want to save ??");
            if (ans) {

                this._mastersService.ModifyCategoryByPut(this.ProductTReeListData.ObjID, this.ProductTReeListData).subscribe(
                    response => {
                        swal("Saved!", "Saved " + this.ProductTReeListData.AliasName + " Saved", "success");

                        this.CategoryForm.reset();
                        this.isReadOnly = false;
                        this.ProductTReeListData = {
                            ObjID: '',
                            CompanyCode: '',
                            BranchCode: '',
                            ItemLevel1ID: 0,
                            ItemLevel1Name: null,
                            GSCode: null,
                            ItemLevel2Id: 0,
                            ItemLevel2Name: null,
                            ItemLevel3Id: 0,
                            ItemLevel3Name: null,
                            ItemLevel4Id: 0,
                            ItemLevel4Name: null,
                            ItemLevel5Id: 0,
                            ItemLevel5Name: null,
                            ItemLevel6Id: null,
                            ItemLevel6Name: null,
                            ShortDescription: null,
                            IsChild: null,
                            MinUnits: null,
                            MinStockLevel: null,
                            CatalogId: null,
                            Grade: null,
                            Tagged: null,
                            Stone: null,
                            Diamond: null,
                            CounterCode: null,
                            karat: null,
                            PieceItem: null,
                            ObjStatus: null,
                            AliasName: null,
                            MinProfitPercent: null,
                            QtyLock: null,
                            Hallmark: null,
                            Certification: null,
                            TcsPerc: null,
                            GSTGoodsGroupCode: null,
                            GSTServicesGroupCode: null,
                            HSN: null,
                            CounterList: [{
                                ObjID: '',
                                CompanyCode: '',
                                BranchCode: '',
                                GSCode: '',
                                ItemName: '',
                                CounterCode: ''
                            }]
                        }
                        // console.log( this.ProductTReeListData);
                        this.getItemListGroup();

                    },
                    (err) => {
                        if (err.status === 400) {
                            const validationError = err.error.description;
                            swal("Warning!", validationError, "warning");
                        }
                        else {
                            this.errors.push('something went wrong!');
                        }
                    }
                )
            }
        }

    }

    EditItem(arg, index) {
        // var Print =JSON.stringify(arg[index]);
        // console.log(Print); 
        this.SelectedCounters = arg[index].CounterList;
        //  var Print =JSON.stringify(this.SelectedCounters);
        // console.log(Print); 

        this.isReadOnly = true;
        this.EnableAdd = false;
        this.EnableSave = true;
        this.ProductTReeListData = {
            ObjID: arg[index].ObjID,
            CompanyCode: this.ccode,
            BranchCode: this.bcode,
            ItemLevel1ID: arg[index].ItemLevel1ID,
            ItemLevel1Name: arg[index].ItemLevel1Name,
            GSCode: arg[index].GSCode,
            ItemLevel2Id: arg[index].ItemLevel2Id,
            ItemLevel2Name: arg[index].ItemLevel2Name,
            ItemLevel3Id: arg[index].ItemLevel3Id,
            ItemLevel3Name: arg[index].ItemLevel3Name,
            ItemLevel4Id: 0,
            ItemLevel4Name: null,
            ItemLevel5Id: 0,
            ItemLevel5Name: null,
            ItemLevel6Id: null,
            ItemLevel6Name: null,
            ShortDescription: null,
            IsChild: arg[index].IsChild,
            MinUnits: arg[index].MinUnits,
            MinStockLevel: null,
            CatalogId: null,
            Grade: null,
            Tagged: arg[index].Tagged,
            Stone: arg[index].Stone,
            Diamond: arg[index].Diamond,
            CounterCode: arg[index].CounterCode,
            karat: arg[index].karat,
            PieceItem: arg[index].PieceItem,
            ObjStatus: arg[index].ObjStatus,
            AliasName: arg[index].AliasName,
            MinProfitPercent: arg[index].MinProfitPercent,
            QtyLock: arg[index].QtyLock,
            Hallmark: arg[index].Hallmark,
            Certification: arg[index].Certification,
            TcsPerc: null,
            GSTGoodsGroupCode: arg[index].GSTGoodsGroupCode,
            GSTServicesGroupCode: arg[index].GSTServicesGroupCode,
            HSN: arg[index].HSN,
            CounterList: arg[index].CounterList,
        };

        // this.SelectedCounters = arg[index].CounterList;

        $('#AddItemModel').modal('show');
        ///check is that all arguments are binded properlt to the model
        //  var print=JSON.stringify(this.ProductTReeListData);
        // console.log(print);
        ///////

    }
    ModifiedItem(form) {
        if (form.value.frmCtrl_ItemCode == null || form.value.frmCtrl_ItemCode == "") {
            swal("Warning!", "Please enter Itemcode", "warning");
        }
        else if (form.value.frmCtrl_ItemName == null || form.value.frmCtrl_ItemName == "") {
            swal("Warning!", "Please enter Item Name", "warning");
        }
        else if (form.value.frmCtrl_Karat == null || form.value.frmCtrl_Karat == "") {
            swal("Warning!", "Please Select Karat", "warning");
        }
        else if (form.value.frmCtrl_minQty == null || form.value.frmCtrl_minQty == "") {
            swal("Warning", "Please enter Minimum Quantity", "warning");
        }
        else if (form.value.frmCtrl_minQty == null || form.value.frmCtrl_minQty == "") {
            swal("Warning!", "Please enter Minimum Quantity", "warning");
        }

        else if (form.value.frmCtrl_goodsType == null || form.value.frmCtrl_goodsType == "") {
            swal("Warning!", "Please Select GoodsType", "warning");
        }
        else if (form.value.frmCtrl_serviceTpe == null || form.value.frmCtrl_serviceTpe == "") {
            swal("Warning!", "Please Select ServiceType", "warning");
        }
        else if (form.value.frmCtrl_Hsn == null || form.value.frmCtrl_Hsn == "") {
            swal("Warning!", "Please Select HSN", "warning");
        }
        else if (form.value.frmCtrl_DefltCounter == null || form.value.frmCtrl_DefltCounter == "") {
            swal("Warning!", "Please Select Default Counter", "warning");
        }
        else if (form.value.frmCtrl_Counter == null || form.value.frmCtrl_Counter == "") {
            swal("Warning!", "Please Select Counter", "warning");
        }
        else {
            var ans = confirm("Do you want to save??");
            // var obj=JSON.stringify( this.ProductTReeListData);
            // console.log(obj);
            this._mastersService.ModifyItemyByPut(this.ProductTReeListData.ObjID, this.ProductTReeListData).subscribe(
                response => {
                    swal("Saved!", "Saved " + this.ProductTReeListData.AliasName + " Saved", "success");
                    this.isReadOnly = false;
                    this.ItemForm.reset();
                    this.ProductTReeListData = {
                        ObjID: '',
                        CompanyCode: '',
                        BranchCode: '',
                        ItemLevel1ID: 0,
                        ItemLevel1Name: null,
                        GSCode: null,
                        ItemLevel2Id: 0,
                        ItemLevel2Name: null,
                        ItemLevel3Id: 0,
                        ItemLevel3Name: null,
                        ItemLevel4Id: 0,
                        ItemLevel4Name: null,
                        ItemLevel5Id: 0,
                        ItemLevel5Name: null,
                        ItemLevel6Id: null,
                        ItemLevel6Name: null,
                        ShortDescription: null,
                        IsChild: null,
                        MinUnits: null,
                        MinStockLevel: null,
                        CatalogId: null,
                        Grade: null,
                        Tagged: null,
                        Stone: null,
                        Diamond: null,
                        CounterCode: null,
                        karat: null,
                        PieceItem: null,
                        ObjStatus: null,
                        AliasName: null,
                        MinProfitPercent: null,
                        QtyLock: null,
                        Hallmark: null,
                        Certification: null,
                        TcsPerc: null,
                        GSTGoodsGroupCode: null,
                        GSTServicesGroupCode: null,
                        HSN: null,
                        CounterList: [{
                            ObjID: '',
                            CompanyCode: '',
                            BranchCode: '',
                            GSCode: '',
                            ItemName: '',
                            CounterCode: ''
                        }]
                    }
                    // console.log( this.ProductTReeListData);
                    this.getItemListGroup();
                },
                (err) => {
                    if (err.status === 400) {
                        const validationError = err.error.description;
                        swal("Warning!", validationError, "warning");
                    }
                    else {
                        this.errors.push('something went wrong!');
                    }

                }

            )

        }

    }
    ReadLoaItemDetails: any = [];
    SelectedCountersCode = [];
    ListOfCountersSelected(arg) {
        this.ProductTReeListData.CounterList = arg.value;
    }
    ///before modiying need to call stock group karat 
    StockGroupList: any = [];
    getStockList() {
        this._mastersService.getStockGroup().subscribe(
            Response => {
                this.StockGroupList = Response;
            }
        )
    }
    SelectedStockGroupCode: string = "";
    StockGroupChanged(arg) {
        this.SelectedStockGroupCode = arg.target.value;
    }

    //////////////Modifying GS Detials

    EditGsModel: {};
    GsData: any = [];
    checkkboxchng(e) {
        if (e.target.checked == true) {
            this.GsEntryListData.IsStone = "Y";
        }
        if (e.target.checked == false) {
            this.GsEntryListData.IsStone = "N";
        }
    }
    checkkboxchng1(e) {
        if (e.target.checked == true) {
            this.GsEntryListData.BillType = "S";
        }

    }
    checkkboxchng2(e) {
        if (e.target.checked == true) {
            this.GsEntryListData.BillType = "P";
        }
    }
    checkkboxchng3(e) {
        if (e.target.checked) {
            this.GsEntryListData.BillType = "R";
        }
    }
    checkkboxchng4(e) {
        if (e.target.checked == true) {
            this.GsEntryListData.BillType = "Z";
        }
    }
    checkkboxchng5(e) {
        if (e.target.checked == true) {
            this.GsEntryListData.BillType = "G";
        }
    }
    EditGS(arg) {
        // alert(arg.ItemLevel1ID);
        this._mastersService.editGsAtProductTree(arg.ItemLevel1ID).subscribe(
            Response => {
                this.GsData = Response;
                this.isReadOnly = true;
                // console.log(this.GsData);
                this.GsEntryListData.ObjID = this.GsData.ObjID;
                this.GsEntryListData.CompanyCode = this.GsData.CompanyCode;
                this.GsEntryListData.BranchCode = this.GsData.BranchCode;
                this.GsEntryListData.ItemLevel1ID = this.GsData.ItemLevel1ID;
                this.GsEntryListData.ItemLevel1Name = this.GsData.ItemLevel1Name;
                this.GsEntryListData.GsCode = this.GsData.GsCode;
                this.GsEntryListData.MeasureType = this.GsData.MeasureType;
                this.GsEntryListData.MetalType = this.GsData.MetalType;
                this.GsEntryListData.Karat = this.GsData.Karat;
                this.GsEntryListData.BillType = this.GsData.BillType;
                this.GsEntryListData.Tax = this.GsData.Tax;
                this.GsEntryListData.OpeningUnits = this.GsData.OpeningUnits;
                this.GsEntryListData.OpeningGwt = this.GsData.OpeningGwt;
                this.GsEntryListData.OpeningNwt = this.GsData.OpeningNwt;
                this.GsEntryListData.OpeningGwtValue = this.GsData.OpeningGwtValue;
                this.GsEntryListData.OpeningNwtValue = this.GsData.OpeningNwtValue;
                this.GsEntryListData.ObjectStatus = this.GsData.ObjectStatus;
                this.GsEntryListData.DisplayOrder = this.GsData.DisplayOrder;
                this.GsEntryListData.CommodityCode = this.GsData.CommodityCode;
                this.GsEntryListData.ExciseDuty = this.GsData.ExciseDuty;
                this.GsEntryListData.EduCess = this.GsData.EduCess;
                this.GsEntryListData.HighEle = this.GsData.HighEle;
                this.GsEntryListData.Tcs = this.GsData.Tcs;
                this.GsEntryListData.IsStone = this.GsData.IsStone;
                this.GsEntryListData.Purity = this.GsData.Purity;
                this.GsEntryListData.TcsPerc = this.GsData.TcsPerc;
                this.GsEntryListData.STax = this.GsData.STax;
                this.GsEntryListData.CTax = this.GsData.CTax;
                this.GsEntryListData.ITax = this.GsData.ITax;
                this.GsEntryListData.GSTGoodsGroupCode = this.GsData.GSTGoodsGroupCode;
                this.GsEntryListData.GSTServicesGroupCode = this.GsData.GSTServicesGroupCode;
                this.GsEntryListData.HSN = this.GsData.HSN;
                // console.log(this.GsEntryListData);
            }
        )
        $('#EditGSModel').modal('show');
        $('#CategoryModel').modal('hide');
        $('#AddItemModel').modal('hide');
        // alert(arg.ObjID);
        // by adding this json chunk i am passing the captured object into the gs entry component
        // this._router.navigate(['/masters/gs-entry'], { state: { ObjectId: arg.ObjID } });
    }
    UpdateGSList() {
        this._mastersService.modifyGSList(this.GsEntryListData.ItemLevel1ID, this.GsEntryListData).subscribe(
            response => {
                swal("Updated!", "Updated " + this.GsEntryListData.ItemLevel1Name + "Updated", "success");
                this.isReadOnly = false;
                this.EditGsEntryFrom.reset();
            },
            (err) => {
                if (err.status === 400) {
                    const validationError = err.error.description;
                    swal("Warning!", validationError, "warning");
                }
                else {
                    this.errors.push('something went wrong!');
                }
                this.EditGsEntryFrom.reset();
                this.isReadOnly = false;
                this.ProductTReeListData = {
                    ObjID: '',
                    CompanyCode: '',
                    BranchCode: '',
                    ItemLevel1ID: 0,
                    ItemLevel1Name: null,
                    GSCode: null,
                    ItemLevel2Id: 0,
                    ItemLevel2Name: null,
                    ItemLevel3Id: 0,
                    ItemLevel3Name: null,
                    ItemLevel4Id: 0,
                    ItemLevel4Name: null,
                    ItemLevel5Id: 0,
                    ItemLevel5Name: null,
                    ItemLevel6Id: null,
                    ItemLevel6Name: null,
                    ShortDescription: null,
                    IsChild: null,
                    MinUnits: null,
                    MinStockLevel: null,
                    CatalogId: null,
                    Grade: null,
                    Tagged: null,
                    Stone: null,
                    Diamond: null,
                    CounterCode: null,
                    karat: null,
                    PieceItem: null,
                    ObjStatus: null,
                    AliasName: null,
                    MinProfitPercent: null,
                    QtyLock: null,
                    Hallmark: null,
                    Certification: null,
                    TcsPerc: null,
                    GSTGoodsGroupCode: null,
                    GSTServicesGroupCode: null,
                    HSN: null,
                    CounterList: [{
                        ObjID: '',
                        CompanyCode: '',
                        BranchCode: '',
                        GSCode: '',
                        ItemName: '',
                        CounterCode: ''
                    }]
                }
                this.GsEntryListData = {
                    ObjID: "",
                    CompanyCode: '',
                    BranchCode: '',
                    ItemLevel1ID: 0,
                    ItemLevel1Name: null,
                    GsCode: '',
                    MeasureType: '',
                    MetalType: '',
                    Karat: '',
                    BillType: '',
                    Tax: 0,
                    OpeningUnits: 0,
                    OpeningGwt: 0,
                    OpeningNwt: 0,
                    OpeningGwtValue: 0,
                    OpeningNwtValue: 0,
                    ObjectStatus: '',
                    DisplayOrder: 0,
                    CommodityCode: '',
                    ExciseDuty: 0,
                    EduCess: 0,
                    HighEle: 0,
                    Tcs: 0,
                    IsStone: '',
                    Purity: 0,
                    TcsPerc: 0,
                    STax: 0,
                    CTax: 0,
                    ITax: 0,
                    GSTGoodsGroupCode: '',
                    GSTServicesGroupCode: '',
                    HSN: ''
                }
                this.GsData.length = 0;
            }
        )
    }
    EditGSModelClose() {
        if (this.EditGsEntryFrom.pristine == true) {
            // alert(this.EditGsEntryFrom.pristine);
            $('#EditGSModel').modal('hide');
            this.getItemListGroup();
        }
        else if (this.EditGsEntryFrom.pristine == false) {
            // alert(this.EditGsEntryFrom.pristine);

            $('#EditGSModel').modal('hide');
            this.EditGsEntryFrom.reset();
            this.isReadOnly = false;
            this.ProductTReeListData = {
                ObjID: '',
                CompanyCode: '',
                BranchCode: '',
                ItemLevel1ID: 0,
                ItemLevel1Name: null,
                GSCode: null,
                ItemLevel2Id: 0,
                ItemLevel2Name: null,
                ItemLevel3Id: 0,
                ItemLevel3Name: null,
                ItemLevel4Id: 0,
                ItemLevel4Name: null,
                ItemLevel5Id: 0,
                ItemLevel5Name: null,
                ItemLevel6Id: null,
                ItemLevel6Name: null,
                ShortDescription: null,
                IsChild: null,
                MinUnits: null,
                MinStockLevel: null,
                CatalogId: null,
                Grade: null,
                Tagged: null,
                Stone: null,
                Diamond: null,
                CounterCode: null,
                karat: null,
                PieceItem: null,
                ObjStatus: null,
                AliasName: null,
                MinProfitPercent: null,
                QtyLock: null,
                Hallmark: null,
                Certification: null,
                TcsPerc: null,
                GSTGoodsGroupCode: null,
                GSTServicesGroupCode: null,
                HSN: null,
                CounterList: [{
                    ObjID: '',
                    CompanyCode: '',
                    BranchCode: '',
                    GSCode: '',
                    ItemName: '',
                    CounterCode: ''
                }]
            }
            this.GsEntryListData = {
                ObjID: "",
                CompanyCode: '',
                BranchCode: '',
                ItemLevel1ID: 0,
                ItemLevel1Name: null,
                GsCode: '',
                MeasureType: '',
                MetalType: '',
                Karat: '',
                BillType: '',
                Tax: 0,
                OpeningUnits: 0,
                OpeningGwt: 0,
                OpeningNwt: 0,
                OpeningGwtValue: 0,
                OpeningNwtValue: 0,
                ObjectStatus: '',
                DisplayOrder: 0,
                CommodityCode: '',
                ExciseDuty: 0,
                EduCess: 0,
                HighEle: 0,
                Tcs: 0,
                IsStone: '',
                Purity: 0,
                TcsPerc: 0,
                STax: 0,
                CTax: 0,
                ITax: 0,
                GSTGoodsGroupCode: '',
                GSTServicesGroupCode: '',
                HSN: ''
            }
            this.GsData.length = 0;
            this.getItemListGroup();

        }

    }
}






