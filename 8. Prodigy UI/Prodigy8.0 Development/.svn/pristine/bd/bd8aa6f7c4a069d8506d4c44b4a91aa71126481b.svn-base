import { Injectable } from '@angular/core';
import { AlternativeStorage } from './alternativestorage';
/*
 * Represents a localStorage store.
 */
export class LocalStorage {
    constructor() {
        this.storage = this.getStorage();
    }
    /*
     * Safari, in Private Browsing Mode, looks like it supports localStorage but all calls to setItem
     * throw QuotaExceededError. We're going to detect this and just silently drop any calls to
     * setItem
     * to avoid the entire page breaking, without having to do a check at each usage of Storage.
     */
    getStorage() {
        try {
            const storage = localStorage;
            storage.setItem('ng2IdleStorage', '');
            storage.removeItem('ng2IdleStorage');
            return storage;
        }
        catch (err) {
            return new AlternativeStorage();
        }
    }
    /*
     * Gets an item in the storage.
     *
     * @param value - The value to get.
     * @return The current value.
     */
    getItem(key) {
        return this.storage.getItem('ng2Idle.' + key);
    }
    /*
     * Removes an item in the storage.
     *
     * @param value - The value to remove.
     */
    removeItem(key) {
        this.storage.removeItem('ng2Idle.' + key);
    }
    /*
     * Sets an item in the storage.
     *
     * @param key - The key to set the value.
     * @param value - The value to set to the key.
     */
    setItem(key, data) {
        this.storage.setItem('ng2Idle.' + key, data);
    }
    /*
     * Represents the storage, commonly use for testing purposes.
     *
     * @param key - The key to set the value.
     * @param value - The value to set to the key.
     */
    _wrapped() {
        return this.storage;
    }
}
LocalStorage.decorators = [
    { type: Injectable }
];
LocalStorage.ctorParameters = () => [];
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoibG9jYWxzdG9yYWdlLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vLi4vLi4vLi4vcHJvamVjdHMvY29yZS9zcmMvbGliL2xvY2Fsc3RvcmFnZS50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQSxPQUFPLEVBQUUsVUFBVSxFQUFFLE1BQU0sZUFBZSxDQUFDO0FBQzNDLE9BQU8sRUFBRSxrQkFBa0IsRUFBRSxNQUFNLHNCQUFzQixDQUFDO0FBRTFEOztHQUVHO0FBRUgsTUFBTSxPQUFPLFlBQVk7SUFHdkI7UUFDRSxJQUFJLENBQUMsT0FBTyxHQUFHLElBQUksQ0FBQyxVQUFVLEVBQUUsQ0FBQztJQUNuQyxDQUFDO0lBRUQ7Ozs7O09BS0c7SUFDSyxVQUFVO1FBQ2hCLElBQUk7WUFDRixNQUFNLE9BQU8sR0FBRyxZQUFZLENBQUM7WUFDN0IsT0FBTyxDQUFDLE9BQU8sQ0FBQyxnQkFBZ0IsRUFBRSxFQUFFLENBQUMsQ0FBQztZQUN0QyxPQUFPLENBQUMsVUFBVSxDQUFDLGdCQUFnQixDQUFDLENBQUM7WUFDckMsT0FBTyxPQUFPLENBQUM7U0FDaEI7UUFBQyxPQUFPLEdBQUcsRUFBRTtZQUNaLE9BQU8sSUFBSSxrQkFBa0IsRUFBRSxDQUFDO1NBQ2pDO0lBQ0gsQ0FBQztJQUVEOzs7OztPQUtHO0lBQ0gsT0FBTyxDQUFDLEdBQVc7UUFDakIsT0FBTyxJQUFJLENBQUMsT0FBTyxDQUFDLE9BQU8sQ0FBQyxVQUFVLEdBQUcsR0FBRyxDQUFDLENBQUM7SUFDaEQsQ0FBQztJQUVEOzs7O09BSUc7SUFDSCxVQUFVLENBQUMsR0FBVztRQUNwQixJQUFJLENBQUMsT0FBTyxDQUFDLFVBQVUsQ0FBQyxVQUFVLEdBQUcsR0FBRyxDQUFDLENBQUM7SUFDNUMsQ0FBQztJQUVEOzs7OztPQUtHO0lBQ0gsT0FBTyxDQUFDLEdBQVcsRUFBRSxJQUFZO1FBQy9CLElBQUksQ0FBQyxPQUFPLENBQUMsT0FBTyxDQUFDLFVBQVUsR0FBRyxHQUFHLEVBQUUsSUFBSSxDQUFDLENBQUM7SUFDL0MsQ0FBQztJQUVEOzs7OztPQUtHO0lBQ0gsUUFBUTtRQUNOLE9BQU8sSUFBSSxDQUFDLE9BQU8sQ0FBQztJQUN0QixDQUFDOzs7WUE5REYsVUFBVSIsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCB7IEluamVjdGFibGUgfSBmcm9tICdAYW5ndWxhci9jb3JlJztcbmltcG9ydCB7IEFsdGVybmF0aXZlU3RvcmFnZSB9IGZyb20gJy4vYWx0ZXJuYXRpdmVzdG9yYWdlJztcblxuLypcbiAqIFJlcHJlc2VudHMgYSBsb2NhbFN0b3JhZ2Ugc3RvcmUuXG4gKi9cbkBJbmplY3RhYmxlKClcbmV4cG9ydCBjbGFzcyBMb2NhbFN0b3JhZ2Uge1xuICBwcml2YXRlIHN0b3JhZ2U6IFN0b3JhZ2U7XG5cbiAgY29uc3RydWN0b3IoKSB7XG4gICAgdGhpcy5zdG9yYWdlID0gdGhpcy5nZXRTdG9yYWdlKCk7XG4gIH1cblxuICAvKlxuICAgKiBTYWZhcmksIGluIFByaXZhdGUgQnJvd3NpbmcgTW9kZSwgbG9va3MgbGlrZSBpdCBzdXBwb3J0cyBsb2NhbFN0b3JhZ2UgYnV0IGFsbCBjYWxscyB0byBzZXRJdGVtXG4gICAqIHRocm93IFF1b3RhRXhjZWVkZWRFcnJvci4gV2UncmUgZ29pbmcgdG8gZGV0ZWN0IHRoaXMgYW5kIGp1c3Qgc2lsZW50bHkgZHJvcCBhbnkgY2FsbHMgdG9cbiAgICogc2V0SXRlbVxuICAgKiB0byBhdm9pZCB0aGUgZW50aXJlIHBhZ2UgYnJlYWtpbmcsIHdpdGhvdXQgaGF2aW5nIHRvIGRvIGEgY2hlY2sgYXQgZWFjaCB1c2FnZSBvZiBTdG9yYWdlLlxuICAgKi9cbiAgcHJpdmF0ZSBnZXRTdG9yYWdlKCk6IFN0b3JhZ2Uge1xuICAgIHRyeSB7XG4gICAgICBjb25zdCBzdG9yYWdlID0gbG9jYWxTdG9yYWdlO1xuICAgICAgc3RvcmFnZS5zZXRJdGVtKCduZzJJZGxlU3RvcmFnZScsICcnKTtcbiAgICAgIHN0b3JhZ2UucmVtb3ZlSXRlbSgnbmcySWRsZVN0b3JhZ2UnKTtcbiAgICAgIHJldHVybiBzdG9yYWdlO1xuICAgIH0gY2F0Y2ggKGVycikge1xuICAgICAgcmV0dXJuIG5ldyBBbHRlcm5hdGl2ZVN0b3JhZ2UoKTtcbiAgICB9XG4gIH1cblxuICAvKlxuICAgKiBHZXRzIGFuIGl0ZW0gaW4gdGhlIHN0b3JhZ2UuXG4gICAqXG4gICAqIEBwYXJhbSB2YWx1ZSAtIFRoZSB2YWx1ZSB0byBnZXQuXG4gICAqIEByZXR1cm4gVGhlIGN1cnJlbnQgdmFsdWUuXG4gICAqL1xuICBnZXRJdGVtKGtleTogc3RyaW5nKTogc3RyaW5nIHwgbnVsbCB7XG4gICAgcmV0dXJuIHRoaXMuc3RvcmFnZS5nZXRJdGVtKCduZzJJZGxlLicgKyBrZXkpO1xuICB9XG5cbiAgLypcbiAgICogUmVtb3ZlcyBhbiBpdGVtIGluIHRoZSBzdG9yYWdlLlxuICAgKlxuICAgKiBAcGFyYW0gdmFsdWUgLSBUaGUgdmFsdWUgdG8gcmVtb3ZlLlxuICAgKi9cbiAgcmVtb3ZlSXRlbShrZXk6IHN0cmluZyk6IHZvaWQge1xuICAgIHRoaXMuc3RvcmFnZS5yZW1vdmVJdGVtKCduZzJJZGxlLicgKyBrZXkpO1xuICB9XG5cbiAgLypcbiAgICogU2V0cyBhbiBpdGVtIGluIHRoZSBzdG9yYWdlLlxuICAgKlxuICAgKiBAcGFyYW0ga2V5IC0gVGhlIGtleSB0byBzZXQgdGhlIHZhbHVlLlxuICAgKiBAcGFyYW0gdmFsdWUgLSBUaGUgdmFsdWUgdG8gc2V0IHRvIHRoZSBrZXkuXG4gICAqL1xuICBzZXRJdGVtKGtleTogc3RyaW5nLCBkYXRhOiBzdHJpbmcpOiB2b2lkIHtcbiAgICB0aGlzLnN0b3JhZ2Uuc2V0SXRlbSgnbmcySWRsZS4nICsga2V5LCBkYXRhKTtcbiAgfVxuXG4gIC8qXG4gICAqIFJlcHJlc2VudHMgdGhlIHN0b3JhZ2UsIGNvbW1vbmx5IHVzZSBmb3IgdGVzdGluZyBwdXJwb3Nlcy5cbiAgICpcbiAgICogQHBhcmFtIGtleSAtIFRoZSBrZXkgdG8gc2V0IHRoZSB2YWx1ZS5cbiAgICogQHBhcmFtIHZhbHVlIC0gVGhlIHZhbHVlIHRvIHNldCB0byB0aGUga2V5LlxuICAgKi9cbiAgX3dyYXBwZWQoKTogU3RvcmFnZSB7XG4gICAgcmV0dXJuIHRoaXMuc3RvcmFnZTtcbiAgfVxufVxuIl19