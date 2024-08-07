import { Directive, ElementRef } from '@angular/core';

@Directive({
  selector: '[BlueBorder]'
})
export class BlueBorderDirective {

  constructor(private elRef: ElementRef) {
    let el = this.elRef.nativeElement;
    el.setAttribute('style', 'border-style: solid; border-color: blue');
  }

}
