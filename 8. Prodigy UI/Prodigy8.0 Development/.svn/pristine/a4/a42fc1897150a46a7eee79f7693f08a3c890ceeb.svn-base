/*
 * Represents an alternative storage for browser that doesn't support localstorage. (i.e. Safari in
 * private mode)
 * @implements Storage
 */
export class AlternativeStorage {
    constructor() {
        this.storageMap = {};
    }
    /*
     * Returns an integer representing the number of data items stored in the storageMap object.
     */
    get length() {
        return Object.keys(this.storageMap).length;
    }
    /*
     * Remove all keys out of the storage.
     */
    clear() {
        this.storageMap = {};
    }
    /*
     * Return the key's value
     *
     * @param key - name of the key to retrieve the value of.
     * @return The key's value
     */
    getItem(key) {
        if (typeof this.storageMap[key] !== 'undefined') {
            return this.storageMap[key];
        }
        return null;
    }
    /*
     * Return the nth key in the storage
     *
     * @param index - the number of the key you want to get the name of.
     * @return The name of the key.
     */
    key(index) {
        return Object.keys(this.storageMap)[index] || null;
    }
    /*
     * Remove a key from the storage.
     *
     * @param key - the name of the key you want to remove.
     */
    removeItem(key) {
        this.storageMap[key] = undefined;
    }
    /*
     * Add a key to the storage, or update a key's value if it already exists.
     *
     * @param key - the name of the key.
     * @param value - the value you want to give to the key.
     */
    setItem(key, value) {
        this.storageMap[key] = value;
    }
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiYWx0ZXJuYXRpdmVzdG9yYWdlLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vLi4vLi4vLi4vcHJvamVjdHMvY29yZS9zcmMvbGliL2FsdGVybmF0aXZlc3RvcmFnZS50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTs7OztHQUlHO0FBQ0gsTUFBTSxPQUFPLGtCQUFrQjtJQUEvQjtRQUNVLGVBQVUsR0FBUSxFQUFFLENBQUM7SUE0RC9CLENBQUM7SUExREM7O09BRUc7SUFDSCxJQUFJLE1BQU07UUFDUixPQUFPLE1BQU0sQ0FBQyxJQUFJLENBQUMsSUFBSSxDQUFDLFVBQVUsQ0FBQyxDQUFDLE1BQU0sQ0FBQztJQUM3QyxDQUFDO0lBRUQ7O09BRUc7SUFDSCxLQUFLO1FBQ0gsSUFBSSxDQUFDLFVBQVUsR0FBRyxFQUFFLENBQUM7SUFDdkIsQ0FBQztJQUVEOzs7OztPQUtHO0lBQ0gsT0FBTyxDQUFDLEdBQVc7UUFDakIsSUFBSSxPQUFPLElBQUksQ0FBQyxVQUFVLENBQUMsR0FBRyxDQUFDLEtBQUssV0FBVyxFQUFFO1lBQy9DLE9BQU8sSUFBSSxDQUFDLFVBQVUsQ0FBQyxHQUFHLENBQUMsQ0FBQztTQUM3QjtRQUNELE9BQU8sSUFBSSxDQUFDO0lBQ2QsQ0FBQztJQUVEOzs7OztPQUtHO0lBQ0gsR0FBRyxDQUFDLEtBQWE7UUFDZixPQUFPLE1BQU0sQ0FBQyxJQUFJLENBQUMsSUFBSSxDQUFDLFVBQVUsQ0FBQyxDQUFDLEtBQUssQ0FBQyxJQUFJLElBQUksQ0FBQztJQUNyRCxDQUFDO0lBRUQ7Ozs7T0FJRztJQUNILFVBQVUsQ0FBQyxHQUFXO1FBQ3BCLElBQUksQ0FBQyxVQUFVLENBQUMsR0FBRyxDQUFDLEdBQUcsU0FBUyxDQUFDO0lBQ25DLENBQUM7SUFFRDs7Ozs7T0FLRztJQUNILE9BQU8sQ0FBQyxHQUFXLEVBQUUsS0FBYTtRQUNoQyxJQUFJLENBQUMsVUFBVSxDQUFDLEdBQUcsQ0FBQyxHQUFHLEtBQUssQ0FBQztJQUMvQixDQUFDO0NBSUYiLCJzb3VyY2VzQ29udGVudCI6WyIvKlxuICogUmVwcmVzZW50cyBhbiBhbHRlcm5hdGl2ZSBzdG9yYWdlIGZvciBicm93c2VyIHRoYXQgZG9lc24ndCBzdXBwb3J0IGxvY2Fsc3RvcmFnZS4gKGkuZS4gU2FmYXJpIGluXG4gKiBwcml2YXRlIG1vZGUpXG4gKiBAaW1wbGVtZW50cyBTdG9yYWdlXG4gKi9cbmV4cG9ydCBjbGFzcyBBbHRlcm5hdGl2ZVN0b3JhZ2UgaW1wbGVtZW50cyBTdG9yYWdlIHtcbiAgcHJpdmF0ZSBzdG9yYWdlTWFwOiBhbnkgPSB7fTtcblxuICAvKlxuICAgKiBSZXR1cm5zIGFuIGludGVnZXIgcmVwcmVzZW50aW5nIHRoZSBudW1iZXIgb2YgZGF0YSBpdGVtcyBzdG9yZWQgaW4gdGhlIHN0b3JhZ2VNYXAgb2JqZWN0LlxuICAgKi9cbiAgZ2V0IGxlbmd0aCgpIHtcbiAgICByZXR1cm4gT2JqZWN0LmtleXModGhpcy5zdG9yYWdlTWFwKS5sZW5ndGg7XG4gIH1cblxuICAvKlxuICAgKiBSZW1vdmUgYWxsIGtleXMgb3V0IG9mIHRoZSBzdG9yYWdlLlxuICAgKi9cbiAgY2xlYXIoKTogdm9pZCB7XG4gICAgdGhpcy5zdG9yYWdlTWFwID0ge307XG4gIH1cblxuICAvKlxuICAgKiBSZXR1cm4gdGhlIGtleSdzIHZhbHVlXG4gICAqXG4gICAqIEBwYXJhbSBrZXkgLSBuYW1lIG9mIHRoZSBrZXkgdG8gcmV0cmlldmUgdGhlIHZhbHVlIG9mLlxuICAgKiBAcmV0dXJuIFRoZSBrZXkncyB2YWx1ZVxuICAgKi9cbiAgZ2V0SXRlbShrZXk6IHN0cmluZyk6IHN0cmluZyB8IG51bGwge1xuICAgIGlmICh0eXBlb2YgdGhpcy5zdG9yYWdlTWFwW2tleV0gIT09ICd1bmRlZmluZWQnKSB7XG4gICAgICByZXR1cm4gdGhpcy5zdG9yYWdlTWFwW2tleV07XG4gICAgfVxuICAgIHJldHVybiBudWxsO1xuICB9XG5cbiAgLypcbiAgICogUmV0dXJuIHRoZSBudGgga2V5IGluIHRoZSBzdG9yYWdlXG4gICAqXG4gICAqIEBwYXJhbSBpbmRleCAtIHRoZSBudW1iZXIgb2YgdGhlIGtleSB5b3Ugd2FudCB0byBnZXQgdGhlIG5hbWUgb2YuXG4gICAqIEByZXR1cm4gVGhlIG5hbWUgb2YgdGhlIGtleS5cbiAgICovXG4gIGtleShpbmRleDogbnVtYmVyKTogc3RyaW5nIHwgbnVsbCB7XG4gICAgcmV0dXJuIE9iamVjdC5rZXlzKHRoaXMuc3RvcmFnZU1hcClbaW5kZXhdIHx8IG51bGw7XG4gIH1cblxuICAvKlxuICAgKiBSZW1vdmUgYSBrZXkgZnJvbSB0aGUgc3RvcmFnZS5cbiAgICpcbiAgICogQHBhcmFtIGtleSAtIHRoZSBuYW1lIG9mIHRoZSBrZXkgeW91IHdhbnQgdG8gcmVtb3ZlLlxuICAgKi9cbiAgcmVtb3ZlSXRlbShrZXk6IHN0cmluZyk6IHZvaWQge1xuICAgIHRoaXMuc3RvcmFnZU1hcFtrZXldID0gdW5kZWZpbmVkO1xuICB9XG5cbiAgLypcbiAgICogQWRkIGEga2V5IHRvIHRoZSBzdG9yYWdlLCBvciB1cGRhdGUgYSBrZXkncyB2YWx1ZSBpZiBpdCBhbHJlYWR5IGV4aXN0cy5cbiAgICpcbiAgICogQHBhcmFtIGtleSAtIHRoZSBuYW1lIG9mIHRoZSBrZXkuXG4gICAqIEBwYXJhbSB2YWx1ZSAtIHRoZSB2YWx1ZSB5b3Ugd2FudCB0byBnaXZlIHRvIHRoZSBrZXkuXG4gICAqL1xuICBzZXRJdGVtKGtleTogc3RyaW5nLCB2YWx1ZTogc3RyaW5nKTogdm9pZCB7XG4gICAgdGhpcy5zdG9yYWdlTWFwW2tleV0gPSB2YWx1ZTtcbiAgfVxuXG4gIFtrZXk6IHN0cmluZ106IGFueTtcbiAgW2luZGV4OiBudW1iZXJdOiBzdHJpbmc7XG59XG4iXX0=