import { Component, Renderer2, Input, ViewChild, NgModule } from '@angular/core';

// tslint:disable:no-input-rename component-selector
const jsbarcode = require('jsbarcode');
class NgxBarcode6Component {
    constructor(renderer) {
        this.renderer = renderer;
        this.elementType = 'svg';
        this.cssClass = 'barcode'; // this should be done more elegantly
        this.format = 'CODE128';
        this.lineColor = '#000000';
        this.width = 2;
        this.height = 100;
        this.displayValue = false;
        this.fontOptions = '';
        this.font = 'monospace';
        this.textAlign = 'center';
        this.textPosition = 'bottom';
        this.textMargin = 2;
        this.fontSize = 20;
        this.background = '#ffffff';
        this.margin = 10;
        this.marginTop = 10;
        this.marginBottom = 10;
        this.marginLeft = 10;
        this.marginRight = 10;
        this.value = '';
        this.valid = () => true;
    }
    get options() {
        return {
            format: this.format,
            lineColor: this.lineColor,
            width: this.width,
            height: this.height,
            displayValue: this.displayValue,
            fontOptions: this.fontOptions,
            font: this.font,
            textAlign: this.textAlign,
            textPosition: this.textPosition,
            textMargin: this.textMargin,
            fontSize: this.fontSize,
            background: this.background,
            margin: this.margin,
            marginTop: this.marginTop,
            marginBottom: this.marginBottom,
            marginLeft: this.marginLeft,
            marginRight: this.marginRight,
            valid: this.valid,
        };
    }
    ngOnChanges() {
        this.createBarcode();
    }
    createBarcode() {
        if (!this.value) {
            return;
        }
        let element;
        switch (this.elementType) {
            case 'img':
                element = this.renderer.createElement('img');
                break;
            case 'canvas':
                element = this.renderer.createElement('canvas');
                break;
            case 'svg':
            default:
                element = this.renderer.createElement('svg', 'svg');
        }
        jsbarcode(element, this.value, this.options);
        for (const node of this.bcElement.nativeElement.childNodes) {
            this.renderer.removeChild(this.bcElement.nativeElement, node);
        }
        this.renderer.appendChild(this.bcElement.nativeElement, element);
    }
}
NgxBarcode6Component.decorators = [
    { type: Component, args: [{
                selector: 'ngx-barcode6',
                template: `<div #bcElement [class]="cssClass"></div>`
            },] }
];
NgxBarcode6Component.ctorParameters = () => [
    { type: Renderer2 }
];
NgxBarcode6Component.propDecorators = {
    elementType: [{ type: Input, args: ['bc-element-type',] }],
    cssClass: [{ type: Input, args: ['bc-class',] }],
    format: [{ type: Input, args: ['bc-format',] }],
    lineColor: [{ type: Input, args: ['bc-line-color',] }],
    width: [{ type: Input, args: ['bc-width',] }],
    height: [{ type: Input, args: ['bc-height',] }],
    displayValue: [{ type: Input, args: ['bc-display-value',] }],
    fontOptions: [{ type: Input, args: ['bc-font-options',] }],
    font: [{ type: Input, args: ['bc-font',] }],
    textAlign: [{ type: Input, args: ['bc-text-align',] }],
    textPosition: [{ type: Input, args: ['bc-text-position',] }],
    textMargin: [{ type: Input, args: ['bc-text-margin',] }],
    fontSize: [{ type: Input, args: ['bc-font-size',] }],
    background: [{ type: Input, args: ['bc-background',] }],
    margin: [{ type: Input, args: ['bc-margin',] }],
    marginTop: [{ type: Input, args: ['bc-margin-top',] }],
    marginBottom: [{ type: Input, args: ['bc-margin-bottom',] }],
    marginLeft: [{ type: Input, args: ['bc-margin-left',] }],
    marginRight: [{ type: Input, args: ['bc-margin-right',] }],
    value: [{ type: Input, args: ['bc-value',] }],
    bcElement: [{ type: ViewChild, args: ['bcElement', { static: true },] }],
    valid: [{ type: Input, args: ['bc-valid',] }]
};

class NgxBarcode6Module {
}
NgxBarcode6Module.decorators = [
    { type: NgModule, args: [{
                declarations: [NgxBarcode6Component],
                imports: [],
                exports: [NgxBarcode6Component]
            },] }
];

/*
 * Public API Surface of ngx-barcode6
 */

/**
 * Generated bundle index. Do not edit.
 */

export { NgxBarcode6Component, NgxBarcode6Module };
//# sourceMappingURL=ngx-barcode6.js.map
