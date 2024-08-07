import { Injectable } from '@angular/core';
import { IdleExpiry } from './idleexpiry';
import { LocalStorage } from './localstorage';
/*
 * Represents a localStorage store of expiry values.
 * @extends IdleExpiry
 */
export class LocalStorageExpiry extends IdleExpiry {
    constructor(localStorage) {
        super();
        this.localStorage = localStorage;
        this.idleName = 'main';
    }
    /*
     * Gets or sets the last expiry date in localStorage.
     * If localStorage doesn't work correctly (i.e. Safari in private mode), we store the expiry value in memory.
     * @param value - The expiry value to set; omit to only return the value.
     * @return The current expiry value.
     */
    last(value) {
        if (value !== void 0) {
            this.setExpiry(value);
        }
        return this.getExpiry();
    }
    idling(value) {
        if (value !== void 0) {
            this.setIdling(value);
        }
        return this.getIdling();
    }
    /*
     * Gets the idle name.
     * @return The name of the idle.
     */
    getIdleName() {
        return this.idleName;
    }
    /*
     * Sets the idle name.
     * @param The name of the idle.
     */
    setIdleName(key) {
        if (key) {
            this.idleName = key;
        }
    }
    getExpiry() {
        const expiry = this.localStorage.getItem(this.idleName + '.expiry');
        if (expiry) {
            return new Date(parseInt(expiry, 10));
        }
        else {
            return null;
        }
    }
    setExpiry(value) {
        if (value) {
            this.localStorage.setItem(this.idleName + '.expiry', value.getTime().toString());
        }
        else {
            this.localStorage.removeItem(this.idleName + '.expiry');
        }
    }
    getIdling() {
        const idling = this.localStorage.getItem(this.idleName + '.idling');
        if (idling) {
            return idling === 'true';
        }
        else {
            return false;
        }
    }
    setIdling(value) {
        if (value) {
            this.localStorage.setItem(this.idleName + '.idling', value.toString());
        }
        else {
            this.localStorage.setItem(this.idleName + '.idling', 'false');
        }
    }
}
LocalStorageExpiry.decorators = [
    { type: Injectable }
];
LocalStorageExpiry.ctorParameters = () => [
    { type: LocalStorage }
];
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoibG9jYWxzdG9yYWdlZXhwaXJ5LmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vLi4vLi4vLi4vcHJvamVjdHMvY29yZS9zcmMvbGliL2xvY2Fsc3RvcmFnZWV4cGlyeS50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQSxPQUFPLEVBQUUsVUFBVSxFQUFFLE1BQU0sZUFBZSxDQUFDO0FBQzNDLE9BQU8sRUFBRSxVQUFVLEVBQUUsTUFBTSxjQUFjLENBQUM7QUFDMUMsT0FBTyxFQUFFLFlBQVksRUFBRSxNQUFNLGdCQUFnQixDQUFDO0FBRTlDOzs7R0FHRztBQUVILE1BQU0sT0FBTyxrQkFBbUIsU0FBUSxVQUFVO0lBR2hELFlBQW9CLFlBQTBCO1FBQzVDLEtBQUssRUFBRSxDQUFDO1FBRFUsaUJBQVksR0FBWixZQUFZLENBQWM7UUFGdEMsYUFBUSxHQUFHLE1BQU0sQ0FBQztJQUkxQixDQUFDO0lBRUQ7Ozs7O09BS0c7SUFDSCxJQUFJLENBQUMsS0FBWTtRQUNmLElBQUksS0FBSyxLQUFLLEtBQUssQ0FBQyxFQUFFO1lBQ3BCLElBQUksQ0FBQyxTQUFTLENBQUMsS0FBSyxDQUFDLENBQUM7U0FDdkI7UUFDRCxPQUFPLElBQUksQ0FBQyxTQUFTLEVBQUUsQ0FBQztJQUMxQixDQUFDO0lBRUQsTUFBTSxDQUFDLEtBQWU7UUFDcEIsSUFBSSxLQUFLLEtBQUssS0FBSyxDQUFDLEVBQUU7WUFDcEIsSUFBSSxDQUFDLFNBQVMsQ0FBQyxLQUFLLENBQUMsQ0FBQztTQUN2QjtRQUNELE9BQU8sSUFBSSxDQUFDLFNBQVMsRUFBRSxDQUFDO0lBQzFCLENBQUM7SUFFRDs7O09BR0c7SUFDSCxXQUFXO1FBQ1QsT0FBTyxJQUFJLENBQUMsUUFBUSxDQUFDO0lBQ3ZCLENBQUM7SUFFRDs7O09BR0c7SUFDSCxXQUFXLENBQUMsR0FBVztRQUNyQixJQUFJLEdBQUcsRUFBRTtZQUNQLElBQUksQ0FBQyxRQUFRLEdBQUcsR0FBRyxDQUFDO1NBQ3JCO0lBQ0gsQ0FBQztJQUVPLFNBQVM7UUFDZixNQUFNLE1BQU0sR0FBVyxJQUFJLENBQUMsWUFBWSxDQUFDLE9BQU8sQ0FBQyxJQUFJLENBQUMsUUFBUSxHQUFHLFNBQVMsQ0FBQyxDQUFDO1FBQzVFLElBQUksTUFBTSxFQUFFO1lBQ1YsT0FBTyxJQUFJLElBQUksQ0FBQyxRQUFRLENBQUMsTUFBTSxFQUFFLEVBQUUsQ0FBQyxDQUFDLENBQUM7U0FDdkM7YUFBTTtZQUNMLE9BQU8sSUFBSSxDQUFDO1NBQ2I7SUFDSCxDQUFDO0lBRU8sU0FBUyxDQUFDLEtBQVc7UUFDM0IsSUFBSSxLQUFLLEVBQUU7WUFDVCxJQUFJLENBQUMsWUFBWSxDQUFDLE9BQU8sQ0FDdkIsSUFBSSxDQUFDLFFBQVEsR0FBRyxTQUFTLEVBQ3pCLEtBQUssQ0FBQyxPQUFPLEVBQUUsQ0FBQyxRQUFRLEVBQUUsQ0FDM0IsQ0FBQztTQUNIO2FBQU07WUFDTCxJQUFJLENBQUMsWUFBWSxDQUFDLFVBQVUsQ0FBQyxJQUFJLENBQUMsUUFBUSxHQUFHLFNBQVMsQ0FBQyxDQUFDO1NBQ3pEO0lBQ0gsQ0FBQztJQUVPLFNBQVM7UUFDZixNQUFNLE1BQU0sR0FBVyxJQUFJLENBQUMsWUFBWSxDQUFDLE9BQU8sQ0FBQyxJQUFJLENBQUMsUUFBUSxHQUFHLFNBQVMsQ0FBQyxDQUFDO1FBQzVFLElBQUksTUFBTSxFQUFFO1lBQ1YsT0FBTyxNQUFNLEtBQUssTUFBTSxDQUFDO1NBQzFCO2FBQU07WUFDTCxPQUFPLEtBQUssQ0FBQztTQUNkO0lBQ0gsQ0FBQztJQUVPLFNBQVMsQ0FBQyxLQUFjO1FBQzlCLElBQUksS0FBSyxFQUFFO1lBQ1QsSUFBSSxDQUFDLFlBQVksQ0FBQyxPQUFPLENBQUMsSUFBSSxDQUFDLFFBQVEsR0FBRyxTQUFTLEVBQUUsS0FBSyxDQUFDLFFBQVEsRUFBRSxDQUFDLENBQUM7U0FDeEU7YUFBTTtZQUNMLElBQUksQ0FBQyxZQUFZLENBQUMsT0FBTyxDQUFDLElBQUksQ0FBQyxRQUFRLEdBQUcsU0FBUyxFQUFFLE9BQU8sQ0FBQyxDQUFDO1NBQy9EO0lBQ0gsQ0FBQzs7O1lBakZGLFVBQVU7OztZQU5GLFlBQVkiLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgeyBJbmplY3RhYmxlIH0gZnJvbSAnQGFuZ3VsYXIvY29yZSc7XG5pbXBvcnQgeyBJZGxlRXhwaXJ5IH0gZnJvbSAnLi9pZGxlZXhwaXJ5JztcbmltcG9ydCB7IExvY2FsU3RvcmFnZSB9IGZyb20gJy4vbG9jYWxzdG9yYWdlJztcblxuLypcbiAqIFJlcHJlc2VudHMgYSBsb2NhbFN0b3JhZ2Ugc3RvcmUgb2YgZXhwaXJ5IHZhbHVlcy5cbiAqIEBleHRlbmRzIElkbGVFeHBpcnlcbiAqL1xuQEluamVjdGFibGUoKVxuZXhwb3J0IGNsYXNzIExvY2FsU3RvcmFnZUV4cGlyeSBleHRlbmRzIElkbGVFeHBpcnkge1xuICBwcml2YXRlIGlkbGVOYW1lID0gJ21haW4nO1xuXG4gIGNvbnN0cnVjdG9yKHByaXZhdGUgbG9jYWxTdG9yYWdlOiBMb2NhbFN0b3JhZ2UpIHtcbiAgICBzdXBlcigpO1xuICB9XG5cbiAgLypcbiAgICogR2V0cyBvciBzZXRzIHRoZSBsYXN0IGV4cGlyeSBkYXRlIGluIGxvY2FsU3RvcmFnZS5cbiAgICogSWYgbG9jYWxTdG9yYWdlIGRvZXNuJ3Qgd29yayBjb3JyZWN0bHkgKGkuZS4gU2FmYXJpIGluIHByaXZhdGUgbW9kZSksIHdlIHN0b3JlIHRoZSBleHBpcnkgdmFsdWUgaW4gbWVtb3J5LlxuICAgKiBAcGFyYW0gdmFsdWUgLSBUaGUgZXhwaXJ5IHZhbHVlIHRvIHNldDsgb21pdCB0byBvbmx5IHJldHVybiB0aGUgdmFsdWUuXG4gICAqIEByZXR1cm4gVGhlIGN1cnJlbnQgZXhwaXJ5IHZhbHVlLlxuICAgKi9cbiAgbGFzdCh2YWx1ZT86IERhdGUpOiBEYXRlIHtcbiAgICBpZiAodmFsdWUgIT09IHZvaWQgMCkge1xuICAgICAgdGhpcy5zZXRFeHBpcnkodmFsdWUpO1xuICAgIH1cbiAgICByZXR1cm4gdGhpcy5nZXRFeHBpcnkoKTtcbiAgfVxuXG4gIGlkbGluZyh2YWx1ZT86IGJvb2xlYW4pOiBib29sZWFuIHtcbiAgICBpZiAodmFsdWUgIT09IHZvaWQgMCkge1xuICAgICAgdGhpcy5zZXRJZGxpbmcodmFsdWUpO1xuICAgIH1cbiAgICByZXR1cm4gdGhpcy5nZXRJZGxpbmcoKTtcbiAgfVxuXG4gIC8qXG4gICAqIEdldHMgdGhlIGlkbGUgbmFtZS5cbiAgICogQHJldHVybiBUaGUgbmFtZSBvZiB0aGUgaWRsZS5cbiAgICovXG4gIGdldElkbGVOYW1lKCk6IHN0cmluZyB7XG4gICAgcmV0dXJuIHRoaXMuaWRsZU5hbWU7XG4gIH1cblxuICAvKlxuICAgKiBTZXRzIHRoZSBpZGxlIG5hbWUuXG4gICAqIEBwYXJhbSBUaGUgbmFtZSBvZiB0aGUgaWRsZS5cbiAgICovXG4gIHNldElkbGVOYW1lKGtleTogc3RyaW5nKTogdm9pZCB7XG4gICAgaWYgKGtleSkge1xuICAgICAgdGhpcy5pZGxlTmFtZSA9IGtleTtcbiAgICB9XG4gIH1cblxuICBwcml2YXRlIGdldEV4cGlyeSgpOiBEYXRlIHtcbiAgICBjb25zdCBleHBpcnk6IHN0cmluZyA9IHRoaXMubG9jYWxTdG9yYWdlLmdldEl0ZW0odGhpcy5pZGxlTmFtZSArICcuZXhwaXJ5Jyk7XG4gICAgaWYgKGV4cGlyeSkge1xuICAgICAgcmV0dXJuIG5ldyBEYXRlKHBhcnNlSW50KGV4cGlyeSwgMTApKTtcbiAgICB9IGVsc2Uge1xuICAgICAgcmV0dXJuIG51bGw7XG4gICAgfVxuICB9XG5cbiAgcHJpdmF0ZSBzZXRFeHBpcnkodmFsdWU6IERhdGUpIHtcbiAgICBpZiAodmFsdWUpIHtcbiAgICAgIHRoaXMubG9jYWxTdG9yYWdlLnNldEl0ZW0oXG4gICAgICAgIHRoaXMuaWRsZU5hbWUgKyAnLmV4cGlyeScsXG4gICAgICAgIHZhbHVlLmdldFRpbWUoKS50b1N0cmluZygpXG4gICAgICApO1xuICAgIH0gZWxzZSB7XG4gICAgICB0aGlzLmxvY2FsU3RvcmFnZS5yZW1vdmVJdGVtKHRoaXMuaWRsZU5hbWUgKyAnLmV4cGlyeScpO1xuICAgIH1cbiAgfVxuXG4gIHByaXZhdGUgZ2V0SWRsaW5nKCk6IGJvb2xlYW4ge1xuICAgIGNvbnN0IGlkbGluZzogc3RyaW5nID0gdGhpcy5sb2NhbFN0b3JhZ2UuZ2V0SXRlbSh0aGlzLmlkbGVOYW1lICsgJy5pZGxpbmcnKTtcbiAgICBpZiAoaWRsaW5nKSB7XG4gICAgICByZXR1cm4gaWRsaW5nID09PSAndHJ1ZSc7XG4gICAgfSBlbHNlIHtcbiAgICAgIHJldHVybiBmYWxzZTtcbiAgICB9XG4gIH1cblxuICBwcml2YXRlIHNldElkbGluZyh2YWx1ZTogYm9vbGVhbikge1xuICAgIGlmICh2YWx1ZSkge1xuICAgICAgdGhpcy5sb2NhbFN0b3JhZ2Uuc2V0SXRlbSh0aGlzLmlkbGVOYW1lICsgJy5pZGxpbmcnLCB2YWx1ZS50b1N0cmluZygpKTtcbiAgICB9IGVsc2Uge1xuICAgICAgdGhpcy5sb2NhbFN0b3JhZ2Uuc2V0SXRlbSh0aGlzLmlkbGVOYW1lICsgJy5pZGxpbmcnLCAnZmFsc2UnKTtcbiAgICB9XG4gIH1cbn1cbiJdfQ==