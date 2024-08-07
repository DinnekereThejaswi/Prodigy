import { Injectable, NgModule, Pipe } from '@angular/core';

import * as ɵngcc0 from '@angular/core';
class Ng2SearchPipe {
    /**
     * @param {?} items object from array
     * @param {?} term term's search
     * @return {?}
     */
    transform(items, term) {
        if (!term || !items)
            return items;
        return Ng2SearchPipe.filter(items, term);
    }
    /**
     *
     * @param {?} items List of items to filter
     * @param {?} term  a string term to compare with every property of the list
     *
     * @return {?}
     */
    static filter(items, term) {
        const /** @type {?} */ toCompare = term.toLowerCase();
        /**
         * @param {?} item
         * @param {?} term
         * @return {?}
         */
        function checkInside(item, term) {
            for (let /** @type {?} */ property in item) {
                if (item[property] === null || item[property] == undefined) {
                    continue;
                }
                if (typeof item[property] === 'object') {
                    if (checkInside(item[property], term)) {
                        return true;
                    }
                }
                if (item[property].toString().toLowerCase().includes(toCompare)) {
                    return true;
                }
            }
            return false;
        }
        return items.filter(function (item) {
            return checkInside(item, term);
        });
    }
}
Ng2SearchPipe.ngInjectableDef = ɵngcc0.ɵɵdefineInjectable({ token: Ng2SearchPipe, factory: function Ng2SearchPipe_Factory(t) { return new (t || Ng2SearchPipe)(); }, providedIn: null });
/*@__PURE__*/ ɵngcc0.ɵsetClassMetadata(Ng2SearchPipe, [{
        type: Pipe,
        args: [{
                name: 'filter',
                pure: false
            }]
    }, {
        type: Injectable
    }], null, { transform: [] });
Ng2SearchPipe.ngPipeDef = ɵngcc0.ɵɵdefinePipe({ name: "filter", type: Ng2SearchPipe, factory: function Ng2SearchPipe_Factory(t) { return new (t || Ng2SearchPipe)(); }, pure: false });
/*@__PURE__*/ ɵngcc0.ɵsetClassMetadata(Ng2SearchPipe, [{
        type: Pipe,
        args: [{
                name: 'filter',
                pure: false
            }]
    }, {
        type: Injectable
    }], null, { transform: [] });
/**
 * @nocollapse
 */
Ng2SearchPipe.ctorParameters = () => [];

class Ng2SearchPipeModule {
}
Ng2SearchPipeModule.ngModuleDef = ɵngcc0.ɵɵdefineNgModule({ type: Ng2SearchPipeModule });
/*@__PURE__*/ ɵngcc0.ɵɵsetNgModuleScope(Ng2SearchPipeModule, { declarations: [Ng2SearchPipe], exports: [Ng2SearchPipe] });
/*@__PURE__*/ ɵngcc0.ɵsetClassMetadata(Ng2SearchPipeModule, [{
        type: NgModule,
        args: [{
                declarations: [Ng2SearchPipe],
                exports: [Ng2SearchPipe]
            }]
    }], null, null);
Ng2SearchPipeModule.ngInjectorDef = ɵngcc0.ɵɵdefineInjector({ factory: function Ng2SearchPipeModule_Factory(t) { return new (t || Ng2SearchPipeModule)(); } });
/**
 * @nocollapse
 */
Ng2SearchPipeModule.ctorParameters = () => [];

/**
 * Generated bundle index. Do not edit.
 */

export { Ng2SearchPipeModule, Ng2SearchPipe };

//# sourceMappingURL=ng2-search-filter.js.map