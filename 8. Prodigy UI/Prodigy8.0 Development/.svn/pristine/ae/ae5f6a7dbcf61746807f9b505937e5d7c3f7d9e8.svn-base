import { EventEmitter } from '@angular/core';
/*
 * A base for classes that act as a source for interrupts.
 */
export class InterruptSource {
    constructor(attachFn, detachFn) {
        this.attachFn = attachFn;
        this.detachFn = detachFn;
        this.isAttached = false;
        this.onInterrupt = new EventEmitter();
    }
    /*
     * Attaches to the specified events on the specified source.
     */
    attach() {
        // If the current zone is the 'angular' zone (a.k.a. NgZone) then re-enter this method in its parent zone
        // The parent zone is usually the '<root>' zone but it can also be 'long-stack-trace-zone' in debug mode
        // In tests, the current zone is typically a 'ProxyZone' created by async/fakeAsync (from @angular/core/testing)
        if (Zone.current.get('isAngularZone') === true) {
            Zone.current.parent.run(() => this.attach());
            return;
        }
        if (!this.isAttached && this.attachFn) {
            this.attachFn(this);
        }
        this.isAttached = true;
    }
    /*
     * Detaches from the specified events on the specified source.
     */
    detach() {
        if (this.isAttached && this.detachFn) {
            this.detachFn(this);
        }
        this.isAttached = false;
    }
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiaW50ZXJydXB0c291cmNlLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vLi4vLi4vLi4vcHJvamVjdHMvY29yZS9zcmMvbGliL2ludGVycnVwdHNvdXJjZS50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQSxPQUFPLEVBQUUsWUFBWSxFQUFFLE1BQU0sZUFBZSxDQUFDO0FBTTdDOztHQUVHO0FBQ0gsTUFBTSxPQUFnQixlQUFlO0lBT25DLFlBQ1ksUUFBNEMsRUFDNUMsUUFBNEM7UUFENUMsYUFBUSxHQUFSLFFBQVEsQ0FBb0M7UUFDNUMsYUFBUSxHQUFSLFFBQVEsQ0FBb0M7UUFSeEQsZUFBVSxHQUFHLEtBQUssQ0FBQztRQUVaLGdCQUFXLEdBQWdDLElBQUksWUFBWSxFQUUvRCxDQUFDO0lBS0QsQ0FBQztJQUVKOztPQUVHO0lBQ0gsTUFBTTtRQUNKLHlHQUF5RztRQUN6Ryx3R0FBd0c7UUFDeEcsZ0hBQWdIO1FBQ2hILElBQUksSUFBSSxDQUFDLE9BQU8sQ0FBQyxHQUFHLENBQUMsZUFBZSxDQUFDLEtBQUssSUFBSSxFQUFFO1lBQzlDLElBQUksQ0FBQyxPQUFPLENBQUMsTUFBTSxDQUFDLEdBQUcsQ0FBQyxHQUFHLEVBQUUsQ0FBQyxJQUFJLENBQUMsTUFBTSxFQUFFLENBQUMsQ0FBQztZQUM3QyxPQUFPO1NBQ1I7UUFFRCxJQUFJLENBQUMsSUFBSSxDQUFDLFVBQVUsSUFBSSxJQUFJLENBQUMsUUFBUSxFQUFFO1lBQ3JDLElBQUksQ0FBQyxRQUFRLENBQUMsSUFBSSxDQUFDLENBQUM7U0FDckI7UUFFRCxJQUFJLENBQUMsVUFBVSxHQUFHLElBQUksQ0FBQztJQUN6QixDQUFDO0lBRUQ7O09BRUc7SUFDSCxNQUFNO1FBQ0osSUFBSSxJQUFJLENBQUMsVUFBVSxJQUFJLElBQUksQ0FBQyxRQUFRLEVBQUU7WUFDcEMsSUFBSSxDQUFDLFFBQVEsQ0FBQyxJQUFJLENBQUMsQ0FBQztTQUNyQjtRQUVELElBQUksQ0FBQyxVQUFVLEdBQUcsS0FBSyxDQUFDO0lBQzFCLENBQUM7Q0FDRiIsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCB7IEV2ZW50RW1pdHRlciB9IGZyb20gJ0Bhbmd1bGFyL2NvcmUnO1xuXG5pbXBvcnQgeyBJbnRlcnJ1cHRBcmdzIH0gZnJvbSAnLi9pbnRlcnJ1cHRhcmdzJztcblxuZGVjbGFyZSBjb25zdCBab25lOiBhbnk7XG5cbi8qXG4gKiBBIGJhc2UgZm9yIGNsYXNzZXMgdGhhdCBhY3QgYXMgYSBzb3VyY2UgZm9yIGludGVycnVwdHMuXG4gKi9cbmV4cG9ydCBhYnN0cmFjdCBjbGFzcyBJbnRlcnJ1cHRTb3VyY2Uge1xuICBpc0F0dGFjaGVkID0gZmFsc2U7XG5cbiAgcHVibGljIG9uSW50ZXJydXB0OiBFdmVudEVtaXR0ZXI8SW50ZXJydXB0QXJncz4gPSBuZXcgRXZlbnRFbWl0dGVyPFxuICAgIEludGVycnVwdEFyZ3NcbiAgPigpO1xuXG4gIGNvbnN0cnVjdG9yKFxuICAgIHByb3RlY3RlZCBhdHRhY2hGbj86IChzb3VyY2U6IEludGVycnVwdFNvdXJjZSkgPT4gdm9pZCxcbiAgICBwcm90ZWN0ZWQgZGV0YWNoRm4/OiAoc291cmNlOiBJbnRlcnJ1cHRTb3VyY2UpID0+IHZvaWRcbiAgKSB7fVxuXG4gIC8qXG4gICAqIEF0dGFjaGVzIHRvIHRoZSBzcGVjaWZpZWQgZXZlbnRzIG9uIHRoZSBzcGVjaWZpZWQgc291cmNlLlxuICAgKi9cbiAgYXR0YWNoKCk6IHZvaWQge1xuICAgIC8vIElmIHRoZSBjdXJyZW50IHpvbmUgaXMgdGhlICdhbmd1bGFyJyB6b25lIChhLmsuYS4gTmdab25lKSB0aGVuIHJlLWVudGVyIHRoaXMgbWV0aG9kIGluIGl0cyBwYXJlbnQgem9uZVxuICAgIC8vIFRoZSBwYXJlbnQgem9uZSBpcyB1c3VhbGx5IHRoZSAnPHJvb3Q+JyB6b25lIGJ1dCBpdCBjYW4gYWxzbyBiZSAnbG9uZy1zdGFjay10cmFjZS16b25lJyBpbiBkZWJ1ZyBtb2RlXG4gICAgLy8gSW4gdGVzdHMsIHRoZSBjdXJyZW50IHpvbmUgaXMgdHlwaWNhbGx5IGEgJ1Byb3h5Wm9uZScgY3JlYXRlZCBieSBhc3luYy9mYWtlQXN5bmMgKGZyb20gQGFuZ3VsYXIvY29yZS90ZXN0aW5nKVxuICAgIGlmIChab25lLmN1cnJlbnQuZ2V0KCdpc0FuZ3VsYXJab25lJykgPT09IHRydWUpIHtcbiAgICAgIFpvbmUuY3VycmVudC5wYXJlbnQucnVuKCgpID0+IHRoaXMuYXR0YWNoKCkpO1xuICAgICAgcmV0dXJuO1xuICAgIH1cblxuICAgIGlmICghdGhpcy5pc0F0dGFjaGVkICYmIHRoaXMuYXR0YWNoRm4pIHtcbiAgICAgIHRoaXMuYXR0YWNoRm4odGhpcyk7XG4gICAgfVxuXG4gICAgdGhpcy5pc0F0dGFjaGVkID0gdHJ1ZTtcbiAgfVxuXG4gIC8qXG4gICAqIERldGFjaGVzIGZyb20gdGhlIHNwZWNpZmllZCBldmVudHMgb24gdGhlIHNwZWNpZmllZCBzb3VyY2UuXG4gICAqL1xuICBkZXRhY2goKTogdm9pZCB7XG4gICAgaWYgKHRoaXMuaXNBdHRhY2hlZCAmJiB0aGlzLmRldGFjaEZuKSB7XG4gICAgICB0aGlzLmRldGFjaEZuKHRoaXMpO1xuICAgIH1cblxuICAgIHRoaXMuaXNBdHRhY2hlZCA9IGZhbHNlO1xuICB9XG59XG4iXX0=