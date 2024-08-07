import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { KaratComponent } from './karat/karat.component';
import { FullModule } from './../layouts/full/full.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { MastersRoutingModule } from './masters-routing.module';
import { SalesmanComponent } from './salesman/salesman.component';
import { DesignMasterComponent } from './design-master/design-master.component';
import { CardChargesComponent } from './card-charges/card-charges.component';
import { StoneRateMasterComponent } from './stone-rate-master/stone-rate-master.component';
import { CompanyComponent } from './company/company.component';
import { CounterComponent } from './counter/counter.component';
import { RolMasterComponent } from './rol-master/rol-master.component';
import { RolNewComponent } from './rol-new/rol-new.component';
import { SellingVaMasterComponent } from './selling-va-master/selling-va-master.component';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { DailyRatesComponent } from './daily-rates/daily-rates.component';
import { MainLocationComponent } from './main-location/main-location.component';
import { ItemSizeComponent } from './item-size/item-size.component';
import { Ng2SearchPipeModule } from 'ng2-search-filter';
import { DiscountMasterComponent } from './discount-master/discount-master.component';
import { ProductTreeComponent } from './product-tree/product-tree.component';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { StockGroupComponent } from './stock-group/stock-group.component';
import { GsEntryComponent } from './gs-entry/gs-entry.component';
import { SelectDropDownModule } from 'ngx-select-dropdown';
import { RolePermissionComponent } from './role-permission/role-permission.component';
import { UserPermissionComponent } from './user-permission/user-permission.component';
import { GstMasterComponent } from './gst-master/gst-master.component';
import { HsnMasterComponent } from './hsn-master/hsn-master.component';
import { GstPostingSetupComponent } from './gst-posting-setup/gst-posting-setup.component';
import { IrSetupComponent } from './ir-setup/ir-setup.component';
import { GsGroupingComponent } from './gs-grouping/gs-grouping.component';
import { PackingMaterialComponent } from './packing-material/packing-material.component';
import { FixedOrderComponent } from './fixed-order/fixed-order.component';
import { OrderFixedTypesComponent } from './order-fixed-types/order-fixed-types.component';
import { SkuMasterComponent } from './sku-master/sku-master.component';
import { ProductAttributesComponent } from './product-attributes/product-attributes.component';
import { IpAddressMngmtComponent } from './ip-address-mngmt/ip-address-mngmt.component';
import { FilterPipeModule } from 'ngx-filter-pipe';
import { TdsMasterComponent } from './tds-master/tds-master.component';
import { StoneMasterComponent } from './stone-master/stone-master.component';
import { StoneMasterNewComponent } from './stone-master-new/stone-master-new.component';
import { BranchMasterComponent } from './branch-master/branch-master.component';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { TcsMasterComponent } from './tcs-master/tcs-master.component';
import { CategoryWiseDiscountOfferComponent } from './category-wise-discount-offer/category-wise-discount-offer.component';
import { SchemeCollectionComponent } from './scheme-collection/scheme-collection.component';
import { MetalRateOfferComponent } from './metal-rate-offer/metal-rate-offer.component';
import { PaymentmasterComponent } from './paymentmaster/paymentmaster.component';
import { ApplicationConfigComponent } from './application-config/application-config.component';
@NgModule({
  declarations: [KaratComponent,
    SalesmanComponent, TdsMasterComponent,
    DesignMasterComponent, CardChargesComponent, StoneRateMasterComponent,
    CompanyComponent, CounterComponent, RolMasterComponent, RolNewComponent,
    SellingVaMasterComponent, ItemSizeComponent,
    DailyRatesComponent, MainLocationComponent, DiscountMasterComponent, ProductTreeComponent
    , StockGroupComponent, GsEntryComponent, RolePermissionComponent, UserPermissionComponent, GstMasterComponent
    , HsnMasterComponent, GstPostingSetupComponent, IrSetupComponent, GsGroupingComponent, PackingMaterialComponent, FixedOrderComponent
    , OrderFixedTypesComponent, OrderFixedTypesComponent, SkuMasterComponent, ProductAttributesComponent, IpAddressMngmtComponent, StoneMasterComponent, StoneMasterNewComponent, BranchMasterComponent, TcsMasterComponent, CategoryWiseDiscountOfferComponent, SchemeCollectionComponent, MetalRateOfferComponent, PaymentmasterComponent, ApplicationConfigComponent],
  imports: [
    CommonModule,
    MastersRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    FullModule,
    NgxPaginationModule,
    BsDatepickerModule,
    Ng2SearchPipeModule,
    SelectDropDownModule,
    FilterPipeModule,
    CollapseModule

  ],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ]
})
export class MastersModule { }
