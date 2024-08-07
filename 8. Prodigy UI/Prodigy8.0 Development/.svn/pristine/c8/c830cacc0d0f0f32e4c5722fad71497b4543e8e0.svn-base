import { WindowInterruptSource } from './windowinterruptsource';
/*
 * An interrupt source on the storage event of Window.
 */
export class StorageInterruptSource extends WindowInterruptSource {
    constructor(throttleDelay = 500) {
        super('storage', throttleDelay);
    }
    /*
     * Checks to see if the event should be filtered.
     * @param event - The original event object.
     * @return True if the event should be filtered (don't cause an interrupt); otherwise, false.
     */
    filterEvent(event) {
        if (event.key &&
            event.key.indexOf('ng2Idle.') >= 0 &&
            event.key.indexOf('.expiry') >= 0) {
            return false;
        }
        return true;
    }
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoic3RvcmFnZWludGVycnVwdHNvdXJjZS5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uLy4uL3Byb2plY3RzL2NvcmUvc3JjL2xpYi9zdG9yYWdlaW50ZXJydXB0c291cmNlLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLE9BQU8sRUFBRSxxQkFBcUIsRUFBRSxNQUFNLHlCQUF5QixDQUFDO0FBRWhFOztHQUVHO0FBQ0gsTUFBTSxPQUFPLHNCQUF1QixTQUFRLHFCQUFxQjtJQUMvRCxZQUFZLGFBQWEsR0FBRyxHQUFHO1FBQzdCLEtBQUssQ0FBQyxTQUFTLEVBQUUsYUFBYSxDQUFDLENBQUM7SUFDbEMsQ0FBQztJQUVEOzs7O09BSUc7SUFDSCxXQUFXLENBQUMsS0FBbUI7UUFDN0IsSUFDRSxLQUFLLENBQUMsR0FBRztZQUNULEtBQUssQ0FBQyxHQUFHLENBQUMsT0FBTyxDQUFDLFVBQVUsQ0FBQyxJQUFJLENBQUM7WUFDbEMsS0FBSyxDQUFDLEdBQUcsQ0FBQyxPQUFPLENBQUMsU0FBUyxDQUFDLElBQUksQ0FBQyxFQUNqQztZQUNBLE9BQU8sS0FBSyxDQUFDO1NBQ2Q7UUFDRCxPQUFPLElBQUksQ0FBQztJQUNkLENBQUM7Q0FDRiIsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCB7IFdpbmRvd0ludGVycnVwdFNvdXJjZSB9IGZyb20gJy4vd2luZG93aW50ZXJydXB0c291cmNlJztcblxuLypcbiAqIEFuIGludGVycnVwdCBzb3VyY2Ugb24gdGhlIHN0b3JhZ2UgZXZlbnQgb2YgV2luZG93LlxuICovXG5leHBvcnQgY2xhc3MgU3RvcmFnZUludGVycnVwdFNvdXJjZSBleHRlbmRzIFdpbmRvd0ludGVycnVwdFNvdXJjZSB7XG4gIGNvbnN0cnVjdG9yKHRocm90dGxlRGVsYXkgPSA1MDApIHtcbiAgICBzdXBlcignc3RvcmFnZScsIHRocm90dGxlRGVsYXkpO1xuICB9XG5cbiAgLypcbiAgICogQ2hlY2tzIHRvIHNlZSBpZiB0aGUgZXZlbnQgc2hvdWxkIGJlIGZpbHRlcmVkLlxuICAgKiBAcGFyYW0gZXZlbnQgLSBUaGUgb3JpZ2luYWwgZXZlbnQgb2JqZWN0LlxuICAgKiBAcmV0dXJuIFRydWUgaWYgdGhlIGV2ZW50IHNob3VsZCBiZSBmaWx0ZXJlZCAoZG9uJ3QgY2F1c2UgYW4gaW50ZXJydXB0KTsgb3RoZXJ3aXNlLCBmYWxzZS5cbiAgICovXG4gIGZpbHRlckV2ZW50KGV2ZW50OiBTdG9yYWdlRXZlbnQpOiBib29sZWFuIHtcbiAgICBpZiAoXG4gICAgICBldmVudC5rZXkgJiZcbiAgICAgIGV2ZW50LmtleS5pbmRleE9mKCduZzJJZGxlLicpID49IDAgJiZcbiAgICAgIGV2ZW50LmtleS5pbmRleE9mKCcuZXhwaXJ5JykgPj0gMFxuICAgICkge1xuICAgICAgcmV0dXJuIGZhbHNlO1xuICAgIH1cbiAgICByZXR1cm4gdHJ1ZTtcbiAgfVxufVxuIl19