import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'payment'
})
export class PaymentPipe implements PipeTransform {

    transform(records: Array<any>, searchText?: string): any {
        let payment;
        switch (searchText) {
            case "Q": {
                payment = "Cheque";
                break;
            }
            case "D": {
                payment = "DD";
                break;
            }
            case "C": {
                payment = "Cash";
                break;
            }
            case "R": {
                payment = "Card";
                break;
            }
            case "CT": {
                payment = "Scheme Adj.";
                break;
            }
            case "BC": {
                payment = "OGS";
                break;
            }
            case "EP": {
                payment = "E-Payment";
                break;
            }
            case "GV": {
                payment = "Gift Vou";
                break;
            }
            case "SR": {
                payment = "SR Bill";
                break;
            }
            case "OP": {
                payment = "Order Adj.";
                break;
            }
            case "PE": {
                payment = "PUR. Est.";
                break;
            }
            case "PB": {
                payment = "PUR. Bill";
                break;
            }
            case "SE": {
                payment = "SR EST";
                break;
            }
            case "SP": {
                payment = "Sales Promotion";
                break;
            }
            case "UPI": {
                payment = "UPI Payment";
                break;
            }
            case "CN": {
                payment = "CREDIT NOTE";
                break;
            }
            case "BD": {
                payment = "DKN";
                break;
            }
            case "BU": {
                payment = "UDP";
                break;
            }
            case "BK": {
                payment = "KRM";
                break;
            }
            case "BJ": {
                payment = "JNR";
                break;
            }
            case "BH": {
                payment = "HBR";
                break;
            }
            case "BM": {
                payment = "MNG";
                break;
            }
            case "BN": {
                payment = "HSN";
                break;
            }
            case "BP": {
                payment = "PNX";
                break;
            }
            case "BS": {
                payment = "DKNBS";
                break;
            }
            case "BMT": {
                payment = "MRT";
                break;
            }
            case "BSM": {
                payment = "SMG";
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