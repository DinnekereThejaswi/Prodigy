import { NumbersPipe } from './../../core/directives/numbers.pipe';
import { SearchPipe } from '../../core/directives/search.pipe';
import { ReactiveFormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgxPaginationModule } from 'ngx-pagination';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { EmailValidatorDirective } from '../../core/directives/validators/email-validator.directive';
import { AlphabetValidatorDirective } from '../../core/directives/validators/alphabet-validator.directive';
import { IntegerDecimalValidatorDirective } from '../../core/directives/validators/integer-decimal-validator.directive';
import { MobileValidatorDirective } from '../../core/directives/validators/mobile-validator.directive';
import { NumberFieldvalidatorDirective } from '../../core/directives/validators/number-fieldvalidator.directive';
import { PanValidatorDirective } from '../../core/directives/validators/pan-validator.directive';
import { PincodeValidatorDirective } from '../../core/directives/validators/pincode-validator.directive';
import { NumberToWordsPipe } from './../../core/directives/numberToWord.pipe';
import { PaymentPipe } from './../../core/directives/payment.pipe';
import { ReprintSalesbillingComponent } from '../../sales-billing/reprint-salesbilling/reprint-salesbilling.component';
import { DotToHashPipe } from './../../core/directives/conversion.pipe';
import { NumberDirective } from './../../core/directives/validators/numbers-only.directive';
import { IDTypePipe } from './../../core/directives/IDType.pipe';
import { NumbersOnlyDirective } from './../../core/directives/validators/appNumber.directive';
import { NumbersNoDecimalOnlyDirective } from './../../core/directives/validators/appNumberWithoutDecimals.directive';
import { RoundPipe } from './../../core/directives/round.pipe';
import { LastdocnoComponent } from './../../lastdocno/lastdocno.component';
import { LastdocnoService } from '../../lastdocno/lastdocno.service';
import { PurchasePipe } from './../../core/directives/PurchasePlan.pipe';
import { StatusNamePipe } from './../../core/directives/status-name.pipe';
import { NgxSpinnerModule } from "ngx-spinner";
import { NgxSpinnerService } from "ngx-spinner";
import { OrderByPipe } from './../../core/directives/order-by.pipe';
import { MasterService } from './../../core/common/master.service';
import { YesNoPipe } from './../../core/directives/yes-no.pipe';
import { SafeHtmlPipe } from './../../core/directives/safeHtml.pipe';
import { BlueBorderDirective } from './../../core/directives/blue-border.directive';
import { GreenBorderDirective } from './../../core/directives/green-border.directive';
import { GrdFilterPipePipe } from './../../core/directives/grd-filter-pipe.pipe'
import { ActiveDeactivePipe } from './../../core/directives/active-deactive.pipe'

@NgModule({
  declarations: [EmailValidatorDirective,
    AlphabetValidatorDirective, IntegerDecimalValidatorDirective, MobileValidatorDirective,
    NumberFieldvalidatorDirective, PanValidatorDirective, PincodeValidatorDirective,
    SearchPipe, NumbersPipe, NumberToWordsPipe, PaymentPipe, DotToHashPipe, NumberDirective, IDTypePipe, RoundPipe,
    NumbersOnlyDirective, LastdocnoComponent, PurchasePipe, StatusNamePipe, OrderByPipe, NumbersNoDecimalOnlyDirective,
    YesNoPipe, SafeHtmlPipe, ReprintSalesbillingComponent,
    BlueBorderDirective, GreenBorderDirective, GrdFilterPipePipe, ActiveDeactivePipe
  ],
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    FormsModule,
    NgxPaginationModule,
    BsDatepickerModule.forRoot(),
    NgxSpinnerModule,
  ],
  providers: [LastdocnoService, NgxSpinnerService, MasterService],
  exports: [EmailValidatorDirective,
    AlphabetValidatorDirective, IntegerDecimalValidatorDirective, MobileValidatorDirective, NumbersOnlyDirective, RoundPipe,
    NumberFieldvalidatorDirective, PanValidatorDirective, PincodeValidatorDirective,
    SearchPipe, NumbersPipe, NumberToWordsPipe, PaymentPipe, DotToHashPipe, NumberDirective, IDTypePipe, LastdocnoComponent,
    PurchasePipe, StatusNamePipe, NgxSpinnerModule, OrderByPipe, NumbersNoDecimalOnlyDirective,
    YesNoPipe, SafeHtmlPipe, ReprintSalesbillingComponent, BlueBorderDirective, GreenBorderDirective, GrdFilterPipePipe, ActiveDeactivePipe]

})

export class FullModule {

}