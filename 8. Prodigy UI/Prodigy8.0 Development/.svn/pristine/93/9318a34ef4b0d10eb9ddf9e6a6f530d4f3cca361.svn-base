import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'IDType'
})
export class IDTypePipe implements PipeTransform {

    transform(records: Array<any>, searchText?: string): any {
        let payment;
        switch (searchText) {
            case "AAD": {
                payment = "ADHAAR";
                break;
            }
            case "ARM": {
                payment = "Arms License";
                break;
            }
            case "DRI": {
                payment = "Driving License";
                break;
            }
            case "OTH": {
                payment = "Others";
                break;
            }
            case "PAN": {
                payment = "Income Tax PAN";
                break;
            }
            case "PAS": {
                payment = "Passport";
                break;
            }
            case "RAT": {
                payment = "Ration Card";
                break;
            }
            case "VOT": {
                payment = "Voter ID";
                break;
            }
            default: {
                payment = "Invalid";
                break;
            }
        }
        return payment;
    }
}