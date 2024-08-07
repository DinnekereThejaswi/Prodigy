// tslint:disable:no-input-rename component-selector
import { Component, Input, ViewChild, Renderer2 } from '@angular/core';
const jsbarcode = require('jsbarcode');
export class NgxBarcode6Component {
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
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoibmd4LWJhcmNvZGU2LmNvbXBvbmVudC5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uLy4uL3Byb2plY3RzL25neC1iYXJjb2RlNi9zcmMvbGliL25neC1iYXJjb2RlNi5jb21wb25lbnQudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUUsb0RBQW9EO0FBQ3BELE9BQU8sRUFBRSxTQUFTLEVBQUUsS0FBSyxFQUFhLFNBQVMsRUFBRSxTQUFTLEVBQWMsTUFBTSxlQUFlLENBQUM7QUFJOUYsTUFBTSxTQUFTLEdBQUcsT0FBTyxDQUFDLFdBQVcsQ0FBQyxDQUFDO0FBT3ZDLE1BQU0sT0FBTyxvQkFBb0I7SUFrRC9CLFlBQW9CLFFBQW1CO1FBQW5CLGFBQVEsR0FBUixRQUFRLENBQVc7UUFoRGIsZ0JBQVcsR0FBNkIsS0FBSyxDQUFDO1FBQ3JELGFBQVEsR0FBRyxTQUFTLENBQUMsQ0FBQyxxQ0FBcUM7UUFFMUQsV0FBTSxHQUNtRixTQUFTLENBQUM7UUFDL0YsY0FBUyxHQUFHLFNBQVMsQ0FBQztRQUMzQixVQUFLLEdBQUcsQ0FBQyxDQUFDO1FBQ1QsV0FBTSxHQUFHLEdBQUcsQ0FBQztRQUNOLGlCQUFZLEdBQUcsS0FBSyxDQUFDO1FBQ3RCLGdCQUFXLEdBQUcsRUFBRSxDQUFDO1FBQ3pCLFNBQUksR0FBRyxXQUFXLENBQUM7UUFDYixjQUFTLEdBQUcsUUFBUSxDQUFDO1FBQ2xCLGlCQUFZLEdBQUcsUUFBUSxDQUFDO1FBQzFCLGVBQVUsR0FBRyxDQUFDLENBQUM7UUFDakIsYUFBUSxHQUFHLEVBQUUsQ0FBQztRQUNiLGVBQVUsR0FBRyxTQUFTLENBQUM7UUFDM0IsV0FBTSxHQUFHLEVBQUUsQ0FBQztRQUNSLGNBQVMsR0FBRyxFQUFFLENBQUM7UUFDWixpQkFBWSxHQUFHLEVBQUUsQ0FBQztRQUNwQixlQUFVLEdBQUcsRUFBRSxDQUFDO1FBQ2YsZ0JBQVcsR0FBRyxFQUFFLENBQUM7UUFDeEIsVUFBSyxHQUFHLEVBQUUsQ0FBQztRQUVYLFVBQUssR0FBa0IsR0FBRyxFQUFFLENBQUMsSUFBSSxDQUFDO0lBeUJWLENBQUM7SUF0QjVDLElBQUksT0FBTztRQUNULE9BQU87WUFDTCxNQUFNLEVBQUUsSUFBSSxDQUFDLE1BQU07WUFDbkIsU0FBUyxFQUFFLElBQUksQ0FBQyxTQUFTO1lBQ3pCLEtBQUssRUFBRSxJQUFJLENBQUMsS0FBSztZQUNqQixNQUFNLEVBQUUsSUFBSSxDQUFDLE1BQU07WUFDbkIsWUFBWSxFQUFFLElBQUksQ0FBQyxZQUFZO1lBQy9CLFdBQVcsRUFBRSxJQUFJLENBQUMsV0FBVztZQUM3QixJQUFJLEVBQUUsSUFBSSxDQUFDLElBQUk7WUFDZixTQUFTLEVBQUUsSUFBSSxDQUFDLFNBQVM7WUFDekIsWUFBWSxFQUFFLElBQUksQ0FBQyxZQUFZO1lBQy9CLFVBQVUsRUFBRSxJQUFJLENBQUMsVUFBVTtZQUMzQixRQUFRLEVBQUUsSUFBSSxDQUFDLFFBQVE7WUFDdkIsVUFBVSxFQUFFLElBQUksQ0FBQyxVQUFVO1lBQzNCLE1BQU0sRUFBRSxJQUFJLENBQUMsTUFBTTtZQUNuQixTQUFTLEVBQUUsSUFBSSxDQUFDLFNBQVM7WUFDekIsWUFBWSxFQUFFLElBQUksQ0FBQyxZQUFZO1lBQy9CLFVBQVUsRUFBRSxJQUFJLENBQUMsVUFBVTtZQUMzQixXQUFXLEVBQUUsSUFBSSxDQUFDLFdBQVc7WUFDN0IsS0FBSyxFQUFFLElBQUksQ0FBQyxLQUFLO1NBQ2xCLENBQUM7SUFDSixDQUFDO0lBR0QsV0FBVztRQUNULElBQUksQ0FBQyxhQUFhLEVBQUUsQ0FBQztJQUN2QixDQUFDO0lBRUQsYUFBYTtRQUNYLElBQUksQ0FBQyxJQUFJLENBQUMsS0FBSyxFQUFFO1lBQUUsT0FBTztTQUFFO1FBQzVCLElBQUksT0FBZ0IsQ0FBQztRQUNyQixRQUFRLElBQUksQ0FBQyxXQUFXLEVBQUU7WUFDeEIsS0FBSyxLQUFLO2dCQUNSLE9BQU8sR0FBRyxJQUFJLENBQUMsUUFBUSxDQUFDLGFBQWEsQ0FBQyxLQUFLLENBQUMsQ0FBQztnQkFDN0MsTUFBTTtZQUNSLEtBQUssUUFBUTtnQkFDWCxPQUFPLEdBQUcsSUFBSSxDQUFDLFFBQVEsQ0FBQyxhQUFhLENBQUMsUUFBUSxDQUFDLENBQUM7Z0JBQ2hELE1BQU07WUFDUixLQUFLLEtBQUssQ0FBQztZQUNYO2dCQUNFLE9BQU8sR0FBRyxJQUFJLENBQUMsUUFBUSxDQUFDLGFBQWEsQ0FBQyxLQUFLLEVBQUUsS0FBSyxDQUFDLENBQUM7U0FDdkQ7UUFFRCxTQUFTLENBQUMsT0FBTyxFQUFFLElBQUksQ0FBQyxLQUFLLEVBQUUsSUFBSSxDQUFDLE9BQU8sQ0FBQyxDQUFDO1FBRTdDLEtBQUssTUFBTSxJQUFJLElBQUksSUFBSSxDQUFDLFNBQVMsQ0FBQyxhQUFhLENBQUMsVUFBVSxFQUFFO1lBQzFELElBQUksQ0FBQyxRQUFRLENBQUMsV0FBVyxDQUFDLElBQUksQ0FBQyxTQUFTLENBQUMsYUFBYSxFQUFFLElBQUksQ0FBQyxDQUFDO1NBQy9EO1FBQ0QsSUFBSSxDQUFDLFFBQVEsQ0FBQyxXQUFXLENBQUMsSUFBSSxDQUFDLFNBQVMsQ0FBQyxhQUFhLEVBQUUsT0FBTyxDQUFDLENBQUM7SUFFbkUsQ0FBQzs7O1lBbkZGLFNBQVMsU0FBQztnQkFDVCxRQUFRLEVBQUUsY0FBYztnQkFDeEIsUUFBUSxFQUFFLDJDQUEyQzthQUV0RDs7O1lBVmdELFNBQVM7OzswQkFhdkQsS0FBSyxTQUFDLGlCQUFpQjt1QkFDdkIsS0FBSyxTQUFDLFVBQVU7cUJBRWhCLEtBQUssU0FBQyxXQUFXO3dCQUVqQixLQUFLLFNBQUMsZUFBZTtvQkFDckIsS0FBSyxTQUFDLFVBQVU7cUJBQ2hCLEtBQUssU0FBQyxXQUFXOzJCQUNqQixLQUFLLFNBQUMsa0JBQWtCOzBCQUN4QixLQUFLLFNBQUMsaUJBQWlCO21CQUN2QixLQUFLLFNBQUMsU0FBUzt3QkFDZixLQUFLLFNBQUMsZUFBZTsyQkFDckIsS0FBSyxTQUFDLGtCQUFrQjt5QkFDeEIsS0FBSyxTQUFDLGdCQUFnQjt1QkFDdEIsS0FBSyxTQUFDLGNBQWM7eUJBQ3BCLEtBQUssU0FBQyxlQUFlO3FCQUNyQixLQUFLLFNBQUMsV0FBVzt3QkFDakIsS0FBSyxTQUFDLGVBQWU7MkJBQ3JCLEtBQUssU0FBQyxrQkFBa0I7eUJBQ3hCLEtBQUssU0FBQyxnQkFBZ0I7MEJBQ3RCLEtBQUssU0FBQyxpQkFBaUI7b0JBQ3ZCLEtBQUssU0FBQyxVQUFVO3dCQUNoQixTQUFTLFNBQUMsV0FBVyxFQUFFLEVBQUUsTUFBTSxFQUFFLElBQUksRUFBRTtvQkFDdkMsS0FBSyxTQUFDLFVBQVUiLCJzb3VyY2VzQ29udGVudCI6WyIgIC8vIHRzbGludDpkaXNhYmxlOm5vLWlucHV0LXJlbmFtZSBjb21wb25lbnQtc2VsZWN0b3JcbiAgaW1wb3J0IHsgQ29tcG9uZW50LCBJbnB1dCwgT25DaGFuZ2VzLCBWaWV3Q2hpbGQsIFJlbmRlcmVyMiwgRWxlbWVudFJlZiB9IGZyb20gJ0Bhbmd1bGFyL2NvcmUnO1xuXG4gIGRlY2xhcmUgdmFyIHJlcXVpcmU6IGFueTtcblxuICBjb25zdCBqc2JhcmNvZGUgPSByZXF1aXJlKCdqc2JhcmNvZGUnKTtcblxuICBAQ29tcG9uZW50KHtcbiAgICBzZWxlY3RvcjogJ25neC1iYXJjb2RlNicsXG4gICAgdGVtcGxhdGU6IGA8ZGl2ICNiY0VsZW1lbnQgW2NsYXNzXT1cImNzc0NsYXNzXCI+PC9kaXY+YCxcbiAgICBzdHlsZXM6IFtdXG4gIH0pXG4gIGV4cG9ydCBjbGFzcyBOZ3hCYXJjb2RlNkNvbXBvbmVudCBpbXBsZW1lbnRzIE9uQ2hhbmdlcyB7XG5cbiAgICBASW5wdXQoJ2JjLWVsZW1lbnQtdHlwZScpIGVsZW1lbnRUeXBlOiAnc3ZnJyB8ICdpbWcnIHwgJ2NhbnZhcycgPSAnc3ZnJztcbiAgICBASW5wdXQoJ2JjLWNsYXNzJykgY3NzQ2xhc3MgPSAnYmFyY29kZSc7IC8vIHRoaXMgc2hvdWxkIGJlIGRvbmUgbW9yZSBlbGVnYW50bHlcblxuICAgIEBJbnB1dCgnYmMtZm9ybWF0JykgZm9ybWF0OiAnJyB8ICdDT0RFMTI4JyB8ICdDT0RFMTI4QScgfCAnQ09ERTEyOEInIHwgJ0NPREUxMjhDJyB8ICdFQU4xMycgfCAnVVBDJyB8ICdFQU44JyB8ICdFQU41JyB8XG4gICAgJ0VBTjInIHwgJ0NPREUzOScgfCAnSVRGMTQnIHwgJ01TSScgfCAnTVNJMTAnIHwgJ01TSTExJyB8ICdNU0kxMDEwJyB8ICdNU0kxMTEwJyB8ICdwaGFybWFjb2RlJyB8ICdjb2RhYmFyJyA9ICdDT0RFMTI4JztcbiAgICBASW5wdXQoJ2JjLWxpbmUtY29sb3InKSBsaW5lQ29sb3IgPSAnIzAwMDAwMCc7XG4gICAgQElucHV0KCdiYy13aWR0aCcpIHdpZHRoID0gMjtcbiAgICBASW5wdXQoJ2JjLWhlaWdodCcpIGhlaWdodCA9IDEwMDtcbiAgICBASW5wdXQoJ2JjLWRpc3BsYXktdmFsdWUnKSBkaXNwbGF5VmFsdWUgPSBmYWxzZTtcbiAgICBASW5wdXQoJ2JjLWZvbnQtb3B0aW9ucycpIGZvbnRPcHRpb25zID0gJyc7XG4gICAgQElucHV0KCdiYy1mb250JykgZm9udCA9ICdtb25vc3BhY2UnO1xuICAgIEBJbnB1dCgnYmMtdGV4dC1hbGlnbicpIHRleHRBbGlnbiA9ICdjZW50ZXInO1xuICAgIEBJbnB1dCgnYmMtdGV4dC1wb3NpdGlvbicpIHRleHRQb3NpdGlvbiA9ICdib3R0b20nO1xuICAgIEBJbnB1dCgnYmMtdGV4dC1tYXJnaW4nKSB0ZXh0TWFyZ2luID0gMjtcbiAgICBASW5wdXQoJ2JjLWZvbnQtc2l6ZScpIGZvbnRTaXplID0gMjA7XG4gICAgQElucHV0KCdiYy1iYWNrZ3JvdW5kJykgYmFja2dyb3VuZCA9ICcjZmZmZmZmJztcbiAgICBASW5wdXQoJ2JjLW1hcmdpbicpIG1hcmdpbiA9IDEwO1xuICAgIEBJbnB1dCgnYmMtbWFyZ2luLXRvcCcpIG1hcmdpblRvcCA9IDEwO1xuICAgIEBJbnB1dCgnYmMtbWFyZ2luLWJvdHRvbScpIG1hcmdpbkJvdHRvbSA9IDEwO1xuICAgIEBJbnB1dCgnYmMtbWFyZ2luLWxlZnQnKSBtYXJnaW5MZWZ0ID0gMTA7XG4gICAgQElucHV0KCdiYy1tYXJnaW4tcmlnaHQnKSBtYXJnaW5SaWdodCA9IDEwO1xuICAgIEBJbnB1dCgnYmMtdmFsdWUnKSB2YWx1ZSA9ICcnO1xuICAgIEBWaWV3Q2hpbGQoJ2JjRWxlbWVudCcsIHsgc3RhdGljOiB0cnVlIH0pIGJjRWxlbWVudDogRWxlbWVudFJlZjtcbiAgICBASW5wdXQoJ2JjLXZhbGlkJykgdmFsaWQ6ICgpID0+IGJvb2xlYW4gPSAoKSA9PiB0cnVlO1xuXG5cbiAgICBnZXQgb3B0aW9ucygpIHtcbiAgICAgIHJldHVybiB7XG4gICAgICAgIGZvcm1hdDogdGhpcy5mb3JtYXQsXG4gICAgICAgIGxpbmVDb2xvcjogdGhpcy5saW5lQ29sb3IsXG4gICAgICAgIHdpZHRoOiB0aGlzLndpZHRoLFxuICAgICAgICBoZWlnaHQ6IHRoaXMuaGVpZ2h0LFxuICAgICAgICBkaXNwbGF5VmFsdWU6IHRoaXMuZGlzcGxheVZhbHVlLFxuICAgICAgICBmb250T3B0aW9uczogdGhpcy5mb250T3B0aW9ucyxcbiAgICAgICAgZm9udDogdGhpcy5mb250LFxuICAgICAgICB0ZXh0QWxpZ246IHRoaXMudGV4dEFsaWduLFxuICAgICAgICB0ZXh0UG9zaXRpb246IHRoaXMudGV4dFBvc2l0aW9uLFxuICAgICAgICB0ZXh0TWFyZ2luOiB0aGlzLnRleHRNYXJnaW4sXG4gICAgICAgIGZvbnRTaXplOiB0aGlzLmZvbnRTaXplLFxuICAgICAgICBiYWNrZ3JvdW5kOiB0aGlzLmJhY2tncm91bmQsXG4gICAgICAgIG1hcmdpbjogdGhpcy5tYXJnaW4sXG4gICAgICAgIG1hcmdpblRvcDogdGhpcy5tYXJnaW5Ub3AsXG4gICAgICAgIG1hcmdpbkJvdHRvbTogdGhpcy5tYXJnaW5Cb3R0b20sXG4gICAgICAgIG1hcmdpbkxlZnQ6IHRoaXMubWFyZ2luTGVmdCxcbiAgICAgICAgbWFyZ2luUmlnaHQ6IHRoaXMubWFyZ2luUmlnaHQsXG4gICAgICAgIHZhbGlkOiB0aGlzLnZhbGlkLFxuICAgICAgfTtcbiAgICB9XG4gICAgY29uc3RydWN0b3IocHJpdmF0ZSByZW5kZXJlcjogUmVuZGVyZXIyKSB7IH1cblxuICAgIG5nT25DaGFuZ2VzKCkge1xuICAgICAgdGhpcy5jcmVhdGVCYXJjb2RlKCk7XG4gICAgfVxuXG4gICAgY3JlYXRlQmFyY29kZSgpIHtcbiAgICAgIGlmICghdGhpcy52YWx1ZSkgeyByZXR1cm47IH1cbiAgICAgIGxldCBlbGVtZW50OiBFbGVtZW50O1xuICAgICAgc3dpdGNoICh0aGlzLmVsZW1lbnRUeXBlKSB7XG4gICAgICAgIGNhc2UgJ2ltZyc6XG4gICAgICAgICAgZWxlbWVudCA9IHRoaXMucmVuZGVyZXIuY3JlYXRlRWxlbWVudCgnaW1nJyk7XG4gICAgICAgICAgYnJlYWs7XG4gICAgICAgIGNhc2UgJ2NhbnZhcyc6XG4gICAgICAgICAgZWxlbWVudCA9IHRoaXMucmVuZGVyZXIuY3JlYXRlRWxlbWVudCgnY2FudmFzJyk7XG4gICAgICAgICAgYnJlYWs7XG4gICAgICAgIGNhc2UgJ3N2Zyc6XG4gICAgICAgIGRlZmF1bHQ6XG4gICAgICAgICAgZWxlbWVudCA9IHRoaXMucmVuZGVyZXIuY3JlYXRlRWxlbWVudCgnc3ZnJywgJ3N2ZycpO1xuICAgICAgfVxuXG4gICAgICBqc2JhcmNvZGUoZWxlbWVudCwgdGhpcy52YWx1ZSwgdGhpcy5vcHRpb25zKTtcblxuICAgICAgZm9yIChjb25zdCBub2RlIG9mIHRoaXMuYmNFbGVtZW50Lm5hdGl2ZUVsZW1lbnQuY2hpbGROb2Rlcykge1xuICAgICAgICB0aGlzLnJlbmRlcmVyLnJlbW92ZUNoaWxkKHRoaXMuYmNFbGVtZW50Lm5hdGl2ZUVsZW1lbnQsIG5vZGUpO1xuICAgICAgfVxuICAgICAgdGhpcy5yZW5kZXJlci5hcHBlbmRDaGlsZCh0aGlzLmJjRWxlbWVudC5uYXRpdmVFbGVtZW50LCBlbGVtZW50KTtcblxuICAgIH1cblxuICB9XG4iXX0=