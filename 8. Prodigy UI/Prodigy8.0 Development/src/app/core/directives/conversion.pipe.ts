import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'dotToHashFormat'
})
export class DotToHashPipe implements PipeTransform {
  transform(value: string): string {
    return value.toString().replace('.','#');
  }
}