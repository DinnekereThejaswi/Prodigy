import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { CanDeactivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
export interface ComponentCanDeactivate {
    confirmBeforeLeave(): boolean;
}

@Injectable()
export class appConfirmationGuard implements CanDeactivate<ComponentCanDeactivate> {
    canDeactivate(component: ComponentCanDeactivate): Observable<boolean> | Promise<boolean> | boolean {
        return component.confirmBeforeLeave();
    }
}