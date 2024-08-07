import { Directive, ElementRef } from '@angular/core';

@Directive({
  selector: '[GreenBorder]'
})
export class GreenBorderDirective {

  constructor(private elRef: ElementRef) {
    let el = this.elRef.nativeElement;
    el.setAttribute('style', 'border-style: solid; border-color: rgb(32, 128, 0)');
  }

}
