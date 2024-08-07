import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'statusName'
})
export class StatusNamePipe implements PipeTransform {

  transform(records: Array<any>, searchText?: string): any {
    let statusName:any;
    switch (searchText) {
      
      case "C": {
        statusName = "Close";
        break;
      }
      case "O": {
        statusName = "Open";
        break;
      }      
    }
    return statusName;
  }

}
