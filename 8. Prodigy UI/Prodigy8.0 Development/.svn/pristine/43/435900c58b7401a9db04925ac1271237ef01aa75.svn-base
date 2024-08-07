import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'activeDeactive'
})
export class ActiveDeactivePipe implements PipeTransform {

  transform(records: Array<any>, searchText?: string): any {
    let statusName: any;
    switch (searchText) {

      case "C": {
        statusName = "Deactive";
        break;
      }
      case "O": {
        statusName = "Active";
        break;
      }
    }
    return statusName;
  }

}
