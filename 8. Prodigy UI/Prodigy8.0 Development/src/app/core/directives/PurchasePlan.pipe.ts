import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'purchasePlan'
})

export class PurchasePipe implements PipeTransform {

    transform(records: Array<any>, searchText?: string): any {
        let payment;
        switch (searchText) {
            case "DM": {
                payment = "DORMANT CUSTOMER ORDER";
                break;
            }
            case "N": {
                payment = "NORMAL";
                break;
            }
            case "SN": {
                payment = "SINDHOORA";
                break;
            }
            case "BN": {
                payment = "BANDHAN";
                break;
            }
            default: {
                payment = "";
                break;
            }
        }
        return payment;
    }
}