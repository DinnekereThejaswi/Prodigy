import { EventEmitter, Injectable, NgZone, Optional } from '@angular/core';
import { IdleExpiry } from './idleexpiry';
import { Interrupt } from './interrupt';
import { KeepaliveSvc } from './keepalivesvc';
import { LocalStorageExpiry } from './localstorageexpiry';
/*
 * Indicates the desired auto resume behavior.
 */
export var AutoResume;
(function (AutoResume) {
    /*
     * Auto resume functionality will be disabled.
     */
    AutoResume[AutoResume["disabled"] = 0] = "disabled";
    /*
     * Can resume automatically even if they are idle.
     */
    AutoResume[AutoResume["idle"] = 1] = "idle";
    /*
     * Can only resume automatically if they are not yet idle.
     */
    AutoResume[AutoResume["notIdle"] = 2] = "notIdle";
})(AutoResume || (AutoResume = {}));
/**
 * A service for detecting and responding to user idleness.
 */
export class Idle {
    constructor(expiry, zone, keepaliveSvc) {
        this.expiry = expiry;
        this.zone = zone;
        this.idle = 20 * 60; // in seconds
        this.timeoutVal = 30; // in seconds
        this.autoResume = AutoResume.idle;
        this.interrupts = new Array();
        this.running = false;
        this.keepaliveEnabled = false;
        this.onIdleStart = new EventEmitter();
        this.onIdleEnd = new EventEmitter();
        this.onTimeoutWarning = new EventEmitter();
        this.onTimeout = new EventEmitter();
        this.onInterrupt = new EventEmitter();
        if (keepaliveSvc) {
            this.keepaliveSvc = keepaliveSvc;
            this.keepaliveEnabled = true;
        }
        this.setIdling(false);
    }
    /*
     * Sets the idle name for localStorage.
     * Important to set if multiple instances of Idle with LocalStorageExpiry
     * @param The name of the idle.
     */
    setIdleName(key) {
        if (this.expiry instanceof LocalStorageExpiry) {
            this.expiry.setIdleName(key);
        }
        else {
            throw new Error('Cannot set expiry key name because no LocalStorageExpiry has been provided.');
        }
    }
    /*
     * Returns whether or not keepalive integration is enabled.
     * @return True if integration is enabled; otherwise, false.
     */
    getKeepaliveEnabled() {
        return this.keepaliveEnabled;
    }
    /*
     * Sets and returns whether or not keepalive integration is enabled.
     * @param True if the integration is enabled; otherwise, false.
     * @return The current value.
     */
    setKeepaliveEnabled(value) {
        if (!this.keepaliveSvc) {
            throw new Error('Cannot enable keepalive integration because no KeepaliveSvc has been provided.');
        }
        return (this.keepaliveEnabled = value);
    }
    /*
     * Returns the current timeout value.
     * @return The timeout value in seconds.
     */
    getTimeout() {
        return this.timeoutVal;
    }
    /*
     * Sets the timeout value.
     * @param seconds - The timeout value in seconds. 0 or false to disable timeout feature.
     * @return The current value. If disabled, the value will be 0.
     */
    setTimeout(seconds) {
        if (seconds === false) {
            this.timeoutVal = 0;
        }
        else if (typeof seconds === 'number' && seconds >= 0) {
            this.timeoutVal = seconds;
        }
        else {
            throw new Error("'seconds' can only be 'false' or a positive number.");
        }
        return this.timeoutVal;
    }
    /*
     * Returns the current idle value.
     * @return The idle value in seconds.
     */
    getIdle() {
        return this.idle;
    }
    /*
     * Sets the idle value.
     * @param seconds - The idle value in seconds.
     * @return The idle value in seconds.
     */
    setIdle(seconds) {
        if (seconds <= 0) {
            throw new Error("'seconds' must be greater zero");
        }
        return (this.idle = seconds);
    }
    /*
     * Returns the current autoresume value.
     * @return The current value.
     */
    getAutoResume() {
        return this.autoResume;
    }
    setAutoResume(value) {
        return (this.autoResume = value);
    }
    /*
     * Sets interrupts from the specified sources.
     * @param sources - Interrupt sources.
     * @return The resulting interrupts.
     */
    setInterrupts(sources) {
        this.clearInterrupts();
        const self = this;
        for (const source of sources) {
            const sub = new Interrupt(source);
            sub.subscribe((args) => {
                self.interrupt(args.force, args.innerArgs);
            });
            this.interrupts.push(sub);
        }
        return this.interrupts;
    }
    /*
     * Returns the current interrupts.
     * @return The current interrupts.
     */
    getInterrupts() {
        return this.interrupts;
    }
    /*
     * Pauses, unsubscribes, and clears the current interrupt subscriptions.
     */
    clearInterrupts() {
        for (const sub of this.interrupts) {
            sub.pause();
            sub.unsubscribe();
        }
        this.interrupts.length = 0;
    }
    /*
     * Returns whether or not the service is running i.e. watching for idleness.
     * @return True if service is watching; otherwise, false.
     */
    isRunning() {
        return this.running;
    }
    /*
     * Returns whether or not the user is considered idle.
     * @return True if the user is in the idle state; otherwise, false.
     */
    isIdling() {
        return this.idling;
    }
    /*
     * Starts watching for inactivity.
     */
    watch(skipExpiry) {
        this.safeClearInterval('idleHandle');
        this.safeClearInterval('timeoutHandle');
        const timeout = !this.timeoutVal ? 0 : this.timeoutVal;
        if (!skipExpiry) {
            const value = new Date(this.expiry.now().getTime() + (this.idle + timeout) * 1000);
            this.expiry.last(value);
        }
        if (this.idling) {
            this.toggleState();
        }
        if (!this.running) {
            this.startKeepalive();
            this.toggleInterrupts(true);
        }
        this.running = true;
        const watchFn = () => {
            this.zone.run(() => {
                const diff = this.getExpiryDiff(timeout);
                if (diff > 0) {
                    this.safeClearInterval('idleHandle');
                    this.setIdleIntervalOutsideOfZone(watchFn, diff);
                }
                else {
                    this.toggleState();
                }
            });
        };
        this.setIdleIntervalOutsideOfZone(watchFn, this.idle * 1000);
    }
    /*
     * Allows protractor tests to call waitForAngular without hanging
     */
    setIdleIntervalOutsideOfZone(watchFn, frequency) {
        this.zone.runOutsideAngular(() => {
            this.idleHandle = setInterval(watchFn, frequency);
        });
    }
    /*
     * Stops watching for inactivity.
     */
    stop() {
        this.stopKeepalive();
        this.toggleInterrupts(false);
        this.safeClearInterval('idleHandle');
        this.safeClearInterval('timeoutHandle');
        this.setIdling(false);
        this.running = false;
        this.expiry.last(null);
    }
    /*
     * Forces a timeout event and state.
     */
    timeout() {
        this.stopKeepalive();
        this.toggleInterrupts(false);
        this.safeClearInterval('idleHandle');
        this.safeClearInterval('timeoutHandle');
        this.setIdling(true);
        this.running = false;
        this.countdown = 0;
        this.onTimeout.emit(null);
    }
    /*
     * Signals that user activity has occurred.
     * @param force - Forces watch to be called, unless they are timed out.
     * @param eventArgs - Optional source event arguments.
     */
    interrupt(force, eventArgs) {
        if (!this.running) {
            return;
        }
        if (this.timeoutVal && this.expiry.isExpired()) {
            this.timeout();
            return;
        }
        this.onInterrupt.emit(eventArgs);
        if (force === true ||
            this.autoResume === AutoResume.idle ||
            (this.autoResume === AutoResume.notIdle && !this.expiry.idling())) {
            this.watch(force);
        }
    }
    setIdling(value) {
        this.idling = value;
        this.expiry.idling(value);
    }
    toggleState() {
        this.setIdling(!this.idling);
        if (this.idling) {
            this.onIdleStart.emit(null);
            this.stopKeepalive();
            if (this.timeoutVal > 0) {
                this.countdown = this.timeoutVal;
                this.doCountdown();
                this.setTimoutIntervalOutsideZone(() => {
                    this.doCountdownInZone();
                }, 1000);
            }
        }
        else {
            this.toggleInterrupts(true);
            this.onIdleEnd.emit(null);
            this.startKeepalive();
        }
        this.safeClearInterval('idleHandle');
    }
    setTimoutIntervalOutsideZone(intervalFn, frequency) {
        this.zone.runOutsideAngular(() => {
            this.timeoutHandle = setInterval(() => {
                intervalFn();
            }, frequency);
        });
    }
    toggleInterrupts(resume) {
        for (const interrupt of this.interrupts) {
            if (resume) {
                interrupt.resume();
            }
            else {
                interrupt.pause();
            }
        }
    }
    getExpiryDiff(timeout) {
        const now = this.expiry.now();
        const last = this.expiry.last() || now;
        return last.getTime() - now.getTime() - timeout * 1000;
    }
    doCountdownInZone() {
        this.zone.run(() => {
            this.doCountdown();
        });
    }
    doCountdown() {
        const diff = this.getExpiryDiff(this.timeoutVal);
        if (diff > 0) {
            this.safeClearInterval('timeoutHandle');
            this.interrupt(true);
            return;
        }
        if (!this.idling) {
            return;
        }
        if (this.countdown <= 0) {
            this.timeout();
            return;
        }
        this.onTimeoutWarning.emit(this.countdown);
        this.countdown--;
    }
    safeClearInterval(handleName) {
        const handle = this[handleName];
        if (handle !== null && typeof handle !== 'undefined') {
            clearInterval(this[handleName]);
            this[handleName] = null;
        }
    }
    startKeepalive() {
        if (!this.keepaliveSvc || !this.keepaliveEnabled) {
            return;
        }
        if (this.running) {
            this.keepaliveSvc.ping();
        }
        this.keepaliveSvc.start();
    }
    stopKeepalive() {
        if (!this.keepaliveSvc || !this.keepaliveEnabled) {
            return;
        }
        this.keepaliveSvc.stop();
    }
    /*
     * Called by Angular when destroying the instance.
     */
    ngOnDestroy() {
        this.stop();
        this.clearInterrupts();
    }
}
Idle.decorators = [
    { type: Injectable }
];
Idle.ctorParameters = () => [
    { type: IdleExpiry },
    { type: NgZone },
    { type: KeepaliveSvc, decorators: [{ type: Optional }] }
];
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiaWRsZS5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uLy4uL3Byb2plY3RzL2NvcmUvc3JjL2xpYi9pZGxlLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLE9BQU8sRUFDTCxZQUFZLEVBQ1osVUFBVSxFQUNWLE1BQU0sRUFFTixRQUFRLEVBQ1QsTUFBTSxlQUFlLENBQUM7QUFFdkIsT0FBTyxFQUFFLFVBQVUsRUFBRSxNQUFNLGNBQWMsQ0FBQztBQUMxQyxPQUFPLEVBQUUsU0FBUyxFQUFFLE1BQU0sYUFBYSxDQUFDO0FBR3hDLE9BQU8sRUFBRSxZQUFZLEVBQUUsTUFBTSxnQkFBZ0IsQ0FBQztBQUM5QyxPQUFPLEVBQUUsa0JBQWtCLEVBQUUsTUFBTSxzQkFBc0IsQ0FBQztBQUUxRDs7R0FFRztBQUNILE1BQU0sQ0FBTixJQUFZLFVBYVg7QUFiRCxXQUFZLFVBQVU7SUFDcEI7O09BRUc7SUFDSCxtREFBUSxDQUFBO0lBQ1I7O09BRUc7SUFDSCwyQ0FBSSxDQUFBO0lBQ0o7O09BRUc7SUFDSCxpREFBTyxDQUFBO0FBQ1QsQ0FBQyxFQWJXLFVBQVUsS0FBVixVQUFVLFFBYXJCO0FBRUQ7O0dBRUc7QUFFSCxNQUFNLE9BQU8sSUFBSTtJQXFCZixZQUNVLE1BQWtCLEVBQ2xCLElBQVksRUFDUixZQUEyQjtRQUYvQixXQUFNLEdBQU4sTUFBTSxDQUFZO1FBQ2xCLFNBQUksR0FBSixJQUFJLENBQVE7UUF0QmQsU0FBSSxHQUFXLEVBQUUsR0FBRyxFQUFFLENBQUMsQ0FBQyxhQUFhO1FBQ3JDLGVBQVUsR0FBRyxFQUFFLENBQUMsQ0FBQyxhQUFhO1FBQzlCLGVBQVUsR0FBZSxVQUFVLENBQUMsSUFBSSxDQUFDO1FBQ3pDLGVBQVUsR0FBcUIsSUFBSSxLQUFLLEVBQUUsQ0FBQztRQUMzQyxZQUFPLEdBQUcsS0FBSyxDQUFDO1FBS2hCLHFCQUFnQixHQUFHLEtBQUssQ0FBQztRQUcxQixnQkFBVyxHQUFzQixJQUFJLFlBQVksRUFBRSxD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RSxDQUFDO0lBQzNCLENBQUM7SUFFRDs7T0FFRztJQUNILFdBQVc7UUFDVCxJQUFJLENBQUMsSUFBSSxFQUFFLENBQUM7UUFDWixJQUFJLENBQUMsZUFBZSxFQUFFLENBQUM7SUFDekIsQ0FBQzs7O1lBN1pGLFVBQVU7OztZQTVCRixVQUFVO1lBTGpCLE1BQU07WUFTQyxZQUFZLHVCQWlEaEIsUUFBUSIsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCB7XG4gIEV2ZW50RW1pdHRlcixcbiAgSW5qZWN0YWJsZSxcbiAgTmdab25lLFxuICBPbkRlc3Ryb3ksXG4gIE9wdGlvbmFsXG59IGZyb20gJ0Bhbmd1bGFyL2NvcmUnO1xuXG5pbXBvcnQgeyBJZGxlRXhwaXJ5IH0gZnJvbSAnLi9pZGxlZXhwaXJ5JztcbmltcG9ydCB7IEludGVycnVwdCB9IGZyb20gJy4vaW50ZXJydXB0JztcbmltcG9ydCB7IEludGVycnVwdEFyZ3MgfSBmcm9tICcuL2ludGVycnVwdGFyZ3MnO1xuaW1wb3J0IHsgSW50ZXJydXB0U291cmNlIH0gZnJvbSAnLi9pbnRlcnJ1cHRzb3VyY2UnO1xuaW1wb3J0IHsgS2VlcGFsaXZlU3ZjIH0gZnJvbSAnLi9rZWVwYWxpdmVzdmMnO1xuaW1wb3J0IHsgTG9jYWxTdG9yYWdlRXhwaXJ5IH0gZnJvbSAnLi9sb2NhbHN0b3JhZ2VleHBpcnknO1xuXG4vKlxuICogSW5kaWNhdGVzIHRoZSBkZXNpcmVkIGF1dG8gcmVzdW1lIGJlaGF2aW9yLlxuICovXG5leHBvcnQgZW51bSBBdXRvUmVzdW1lIHtcbiAgLypcbiAgICogQXV0byByZXN1bWUgZnVuY3Rpb25hbGl0eSB3aWxsIGJlIGRpc2FibGVkLlxuICAgKi9cbiAgZGlzYWJsZWQsXG4gIC8qXG4gICAqIENhbiByZXN1bWUgYXV0b21hdGljYWxseSBldmVuIGlmIHRoZXkgYXJlIGlkbGUuXG4gICAqL1xuICBpZGxlLFxuICAvKlxuICAgKiBDYW4gb25seSByZXN1bWUgYXV0b21hdGljYWxseSBpZiB0aGV5IGFyZSBub3QgeWV0IGlkbGUuXG4gICAqL1xuICBub3RJZGxlXG59XG5cbi8qKlxuICogQSBzZXJ2aWNlIGZvciBkZXRlY3RpbmcgYW5kIHJlc3BvbmRpbmcgdG8gdXNlciBpZGxlbmVzcy5cbiAqL1xuQEluamVjdGFibGUoKVxuZXhwb3J0IGNsYXNzIElkbGUgaW1wbGVtZW50cyBPbkRlc3Ryb3kge1xuICBwcml2YXRlIGlkbGU6IG51bWJlciA9IDIwICogNjA7IC8vIGluIHNlY29uZHNcbiAgcHJpdmF0ZSB0aW1lb3V0VmFsID0gMzA7IC8vIGluIHNlY29uZHNcbiAgcHJpdmF0ZSBhdXRvUmVzdW1lOiBBdXRvUmVzdW1lID0gQXV0b1Jlc3VtZS5pZGxlO1xuICBwcml2YXRlIGludGVycnVwdHM6IEFycmF5PEludGVycnVwdD4gPSBuZXcgQXJyYXkoKTtcbiAgcHJpdmF0ZSBydW5uaW5nID0gZmFsc2U7XG4gIHByaXZhdGUgaWRsaW5nOiBib29sZWFuO1xuICBwcml2YXRlIGlkbGVIYW5kbGU6IGFueTtcbiAgcHJpdmF0ZSB0aW1lb3V0SGFuZGxlOiBhbnk7XG4gIHByaXZhdGUgY291bnRkb3duOiBudW1iZXI7XG4gIHByaXZhdGUga2VlcGFsaXZlRW5hYmxlZCA9IGZhbHNlO1xuICBwcml2YXRlIGtlZXBhbGl2ZVN2YzogS2VlcGFsaXZlU3ZjO1xuXG4gIHB1YmxpYyBvbklkbGVTdGFydDogRXZlbnRFbWl0dGVyPGFueT4gPSBuZXcgRXZlbnRFbWl0dGVyKCk7XG4gIHB1YmxpYyBvbklkbGVFbmQ6IEV2ZW50RW1pdHRlcjxhbnk+ID0gbmV3IEV2ZW50RW1pdHRlcigpO1xuICBwdWJsaWMgb25UaW1lb3V0V2FybmluZzogRXZlbnRFbWl0dGVyPG51bWJlcj4gPSBuZXcgRXZlbnRFbWl0dGVyPG51bWJlcj4oKTtcbiAgcHVibGljIG9uVGltZW91dDogRXZlbnRFbWl0dGVyPG51bWJlcj4gPSBuZXcgRXZlbnRFbWl0dGVyPG51bWJlcj4oKTtcbiAgcHVibGljIG9uSW50ZXJydXB0OiBFdmVudEVtaXR0ZXI8YW55PiA9IG5ldyBFdmVudEVtaXR0ZXIoKTtcblxuICBba2V5OiBzdHJpbmddOiBhbnk7XG5cbiAgY29uc3RydWN0b3IoXG4gICAgcHJpdmF0ZSBleHBpcnk6IElkbGVFeHBpcnksXG4gICAgcHJpdmF0ZSB6b25lOiBOZ1pvbmUsXG4gICAgQE9wdGlvbmFsKCkga2VlcGFsaXZlU3ZjPzogS2VlcGFsaXZlU3ZjXG4gICkge1xuICAgIGlmIChrZWVwYWxpdmVTdmMpIHtcbiAgICAgIHRoaXMua2VlcGFsaXZlU3ZjID0ga2VlcGFsaXZlU3ZjO1xuICAgICAgdGhpcy5rZWVwYWxpdmVFbmFibGVkID0gdHJ1ZTtcbiAgICB9XG4gICAgdGhpcy5zZXRJZGxpbmcoZmFsc2UpO1xuICB9XG5cbiAgLypcbiAgICogU2V0cyB0aGUgaWRsZSBuYW1lIGZvciBsb2NhbFN0b3JhZ2UuXG4gICAqIEltcG9ydGFudCB0byBzZXQgaWYgbXVsdGlwbGUgaW5zdGFuY2VzIG9mIElkbGUgd2l0aCBMb2NhbFN0b3JhZ2VFeHBpcnlcbiAgICogQHBhcmFtIFRoZSBuYW1lIG9mIHRoZSBpZGxlLlxuICAgKi9cbiAgc2V0SWRsZU5hbWUoa2V5OiBzdHJpbmcpOiB2b2lkIHtcbiAgICBpZiAodGhpcy5leHBpcnkgaW5zdGFuY2VvZiBMb2NhbFN0b3JhZ2VFeHBpcnkpIHtcbiAgICAgIHRoaXMuZXhwaXJ5LnNldElkbGVOYW1lKGtleSk7XG4gICAgfSBlbHNlIHtcbiAgICAgIHRocm93IG5ldyBFcnJvcihcbiAgICAgICAgJ0Nhbm5vdCBzZXQgZXhwaXJ5IGtleSBuYW1lIGJlY2F1c2Ugbm8gTG9jYWxTdG9yYWdlRXhwaXJ5IGhhcyBiZWVuIHByb3ZpZGVkLidcbiAgICAgICk7XG4gICAgfVxuICB9XG5cbiAgLypcbiAgICogUmV0dXJucyB3aGV0aGVyIG9yIG5vdCBrZWVwYWxpdmUgaW50ZWdyYXRpb24gaXMgZW5hYmxlZC5cbiAgICogQHJldHVybiBUcnVlIGlmIGludGVncmF0aW9uIGlzIGVuYWJsZWQ7IG90aGVyd2lzZSwgZmFsc2UuXG4gICAqL1xuICBnZXRLZWVwYWxpdmVFbmFibGVkKCk6IGJvb2xlYW4ge1xuICAgIHJldHVybiB0aGlzLmtlZXBhbGl2ZUVuYWJsZWQ7XG4gIH1cblxuICAvKlxuICAgKiBTZXRzIGFuZCByZXR1cm5zIHdoZXRoZXIgb3Igbm90IGtlZXBhbGl2ZSBpbnRlZ3JhdGlvbiBpcyBlbmFibGVkLlxuICAgKiBAcGFyYW0gVHJ1ZSBpZiB0aGUgaW50ZWdyYXRpb24gaXMgZW5hYmxlZDsgb3RoZXJ3aXNlLCBmYWxzZS5cbiAgICogQHJldHVybiBUaGUgY3VycmVudCB2YWx1ZS5cbiAgICovXG4gIHNldEtlZXBhbGl2ZUVuYWJsZWQodmFsdWU6IGJvb2xlYW4pOiBib29sZWFuIHtcbiAgICBpZiAoIXRoaXMua2VlcGFsaXZlU3ZjKSB7XG4gICAgICB0aHJvdyBuZXcgRXJyb3IoXG4gICAgICAgICdDYW5ub3QgZW5hYmxlIGtlZXBhbGl2ZSBpbnRlZ3JhdGlvbiBiZWNhdXNlIG5vIEtlZXBhbGl2ZVN2YyBoYXMgYmVlbiBwcm92aWRlZC4nXG4gICAgICApO1xuICAgIH1cblxuICAgIHJldHVybiAodGhpcy5rZWVwYWxpdmVFbmFibGVkID0gdmFsdWUpO1xuICB9XG5cbiAgLypcbiAgICogUmV0dXJucyB0aGUgY3VycmVudCB0aW1lb3V0IHZhbHVlLlxuICAgKiBAcmV0dXJuIFRoZSB0aW1lb3V0IHZhbHVlIGluIHNlY29uZHMuXG4gICAqL1xuICBnZXRUaW1lb3V0KCk6IG51bWJlciB7XG4gICAgcmV0dXJuIHRoaXMudGltZW91dFZhbDtcbiAgfVxuXG4gIC8qXG4gICAqIFNldHMgdGhlIHRpbWVvdXQgdmFsdWUuXG4gICAqIEBwYXJhbSBzZWNvbmRzIC0gVGhlIHRpbWVvdXQgdmFsdWUgaW4gc2Vjb25kcy4gMCBvciBmYWxzZSB0byBkaXNhYmxlIHRpbWVvdXQgZmVhdHVyZS5cbiAgICogQHJldHVybiBUaGUgY3VycmVudCB2YWx1ZS4gSWYgZGlzYWJsZWQsIHRoZSB2YWx1ZSB3aWxsIGJlIDAuXG4gICAqL1xuICBzZXRUaW1lb3V0KHNlY29uZHM6IG51bWJlciB8IGJvb2xlYW4pOiBudW1iZXIge1xuICAgIGlmIChzZWNvbmRzID09PSBmYWxzZSkge1xuICAgICAgdGhpcy50aW1lb3V0VmFsID0gMDtcbiAgICB9IGVsc2UgaWYgKHR5cGVvZiBzZWNvbmRzID09PSAnbnVtYmVyJyAmJiBzZWNvbmRzID49IDApIHtcbiAgICAgIHRoaXMudGltZW91dFZhbCA9IHNlY29uZHM7XG4gICAgfSBlbHNlIHtcbiAgICAgIHRocm93IG5ldyBFcnJvcihcIidzZWNvbmRzJyBjYW4gb25seSBiZSAnZmFsc2UnIG9yIGEgcG9zaXRpdmUgbnVtYmVyLlwiKTtcbiAgICB9XG5cbiAgICByZXR1cm4gdGhpcy50aW1lb3V0VmFsO1xuICB9XG5cbiAgLypcbiAgICogUmV0dXJucyB0aGUgY3VycmVudCBpZGxlIHZhbHVlLlxuICAgKiBAcmV0dXJuIFRoZSBpZGxlIHZhbHVlIGluIHNlY29uZHMuXG4gICAqL1xuICBnZXRJZGxlKCk6IG51bWJlciB7XG4gICAgcmV0dXJuIHRoaXMuaWRsZTtcbiAgfVxuXG4gIC8qXG4gICAqIFNldHMgdGhlIGlkbGUgdmFsdWUuXG4gICAqIEBwYXJhbSBzZWNvbmRzIC0gVGhlIGlkbGUgdmFsdWUgaW4gc2Vjb25kcy5cbiAgICogQHJldHVybiBUaGUgaWRsZSB2YWx1ZSBpbiBzZWNvbmRzLlxuICAgKi9cbiAgc2V0SWRsZShzZWNvbmRzOiBudW1iZXIpOiBudW1iZXIge1xuICAgIGlmIChzZWNvbmRzIDw9IDApIHtcbiAgICAgIHRocm93IG5ldyBFcnJvcihcIidzZWNvbmRzJyBtdXN0IGJlIGdyZWF0ZXIgemVyb1wiKTtcbiAgICB9XG5cbiAgICByZXR1cm4gKHRoaXMuaWRsZSA9IHNlY29uZHMpO1xuICB9XG5cbiAgLypcbiAgICogUmV0dXJucyB0aGUgY3VycmVudCBhdXRvcmVzdW1lIHZhbHVlLlxuICAgKiBAcmV0dXJuIFRoZSBjdXJyZW50IHZhbHVlLlxuICAgKi9cbiAgZ2V0QXV0b1Jlc3VtZSgpOiBBdXRvUmVzdW1lIHtcbiAgICByZXR1cm4gdGhpcy5hdXRvUmVzdW1lO1xuICB9XG5cbiAgc2V0QXV0b1Jlc3VtZSh2YWx1ZTogQXV0b1Jlc3VtZSk6IEF1dG9SZXN1bWUge1xuICAgIHJldHVybiAodGhpcy5hdXRvUmVzdW1lID0gdmFsdWUpO1xuICB9XG5cbiAgLypcbiAgICogU2V0cyBpbnRlcnJ1cHRzIGZyb20gdGhlIHNwZWNpZmllZCBzb3VyY2VzLlxuICAgKiBAcGFyYW0gc291cmNlcyAtIEludGVycnVwdCBzb3VyY2VzLlxuICAgKiBAcmV0dXJuIFRoZSByZXN1bHRpbmcgaW50ZXJydXB0cy5cbiAgICovXG4gIHNldEludGVycnVwdHMoc291cmNlczogQXJyYXk8SW50ZXJydXB0U291cmNlPik6IEFycmF5PEludGVycnVwdD4ge1xuICAgIHRoaXMuY2xlYXJJbnRlcnJ1cHRzKCk7XG5cbiAgICBjb25zdCBzZWxmID0gdGhpcztcblxuICAgIGZvciAoY29uc3Qgc291cmNlIG9mIHNvdXJjZXMpIHtcbiAgICAgIGNvbnN0IHN1YiA9IG5ldyBJbnRlcnJ1cHQoc291cmNlKTtcbiAgICAgIHN1Yi5zdWJzY3JpYmUoKGFyZ3M6IEludGVycnVwdEFyZ3MpID0+IHtcbiAgICAgICAgc2VsZi5pbnRlcnJ1cHQoYXJncy5mb3JjZSwgYXJncy5pbm5lckFyZ3MpO1xuICAgICAgfSk7XG5cbiAgICAgIHRoaXMuaW50ZXJydXB0cy5wdXNoKHN1Yik7XG4gICAgfVxuXG4gICAgcmV0dXJuIHRoaXMuaW50ZXJydXB0cztcbiAgfVxuXG4gIC8qXG4gICAqIFJldHVybnMgdGhlIGN1cnJlbnQgaW50ZXJydXB0cy5cbiAgICogQHJldHVybiBUaGUgY3VycmVudCBpbnRlcnJ1cHRzLlxuICAgKi9cbiAgZ2V0SW50ZXJydXB0cygpOiBBcnJheTxJbnRlcnJ1cHQ+IHtcbiAgICByZXR1cm4gdGhpcy5pbnRlcnJ1cHRzO1xuICB9XG5cbiAgLypcbiAgICogUGF1c2VzLCB1bnN1YnNjcmliZXMsIGFuZCBjbGVhcnMgdGhlIGN1cnJlbnQgaW50ZXJydXB0IHN1YnNjcmlwdGlvbnMuXG4gICAqL1xuICBjbGVhckludGVycnVwdHMoKTogdm9pZCB7XG4gICAgZm9yIChjb25zdCBzdWIgb2YgdGhpcy5pbnRlcnJ1cHRzKSB7XG4gICAgICBzdWIucGF1c2UoKTtcbiAgICAgIHN1Yi51bnN1YnNjcmliZSgpO1xuICAgIH1cblxuICAgIHRoaXMuaW50ZXJydXB0cy5sZW5ndGggPSAwO1xuICB9XG5cbiAgLypcbiAgICogUmV0dXJucyB3aGV0aGVyIG9yIG5vdCB0aGUgc2VydmljZSBpcyBydW5uaW5nIGkuZS4gd2F0Y2hpbmcgZm9yIGlkbGVuZXNzLlxuICAgKiBAcmV0dXJuIFRydWUgaWYgc2VydmljZSBpcyB3YXRjaGluZzsgb3RoZXJ3aXNlLCBmYWxzZS5cbiAgICovXG4gIGlzUnVubmluZygpOiBib29sZWFuIHtcbiAgICByZXR1cm4gdGhpcy5ydW5uaW5nO1xuICB9XG5cbiAgLypcbiAgICogUmV0dXJucyB3aGV0aGVyIG9yIG5vdCB0aGUgdXNlciBpcyBjb25zaWRlcmVkIGlkbGUuXG4gICAqIEByZXR1cm4gVHJ1ZSBpZiB0aGUgdXNlciBpcyBpbiB0aGUgaWRsZSBzdGF0ZTsgb3RoZXJ3aXNlLCBmYWxzZS5cbiAgICovXG4gIGlzSWRsaW5nKCk6IGJvb2xlYW4ge1xuICAgIHJldHVybiB0aGlzLmlkbGluZztcbiAgfVxuXG4gIC8qXG4gICAqIFN0YXJ0cyB3YXRjaGluZyBmb3IgaW5hY3Rpdml0eS5cbiAgICovXG4gIHdhdGNoKHNraXBFeHBpcnk/OiBib29sZWFuKTogdm9pZCB7XG4gICAgdGhpcy5zYWZlQ2xlYXJJbnRlcnZhbCgnaWRsZUhhbmRsZScpO1xuICAgIHRoaXMuc2FmZUNsZWFySW50ZXJ2YWwoJ3RpbWVvdXRIYW5kbGUnKTtcblxuICAgIGNvbnN0IHRpbWVvdXQgPSAhdGhpcy50aW1lb3V0VmFsID8gMCA6IHRoaXMudGltZW91dFZhbDtcbiAgICBpZiAoIXNraXBFeHBpcnkpIHtcbiAgICAgIGNvbnN0IHZhbHVlID0gbmV3IERhdGUoXG4gICAgICAgIHRoaXMuZXhwaXJ5Lm5vdygpLmdldFRpbWUoKSArICh0aGlzLmlkbGUgKyB0aW1lb3V0KSAqIDEwMDBcbiAgICAgICk7XG4gICAgICB0aGlzLmV4cGlyeS5sYXN0KHZhbHVlKTtcbiAgICB9XG5cbiAgICBpZiAodGhpcy5pZGxpbmcpIHtcbiAgICAgIHRoaXMudG9nZ2xlU3RhdGUoKTtcbiAgICB9XG4gICAgaWYgKCF0aGlzLnJ1bm5pbmcpIHtcbiAgICAgIHRoaXMuc3RhcnRLZWVwYWxpdmUoKTtcbiAgICAgIHRoaXMudG9nZ2xlSW50ZXJydXB0cyh0cnVlKTtcbiAgICB9XG5cbiAgICB0aGlzLnJ1bm5pbmcgPSB0cnVlO1xuXG4gICAgY29uc3Qgd2F0Y2hGbiA9ICgpID0+IHtcbiAgICAgIHRoaXMuem9uZS5ydW4oKCkgPT4ge1xuICAgICAgICBjb25zdCBkaWZmID0gdGhpcy5nZXRFeHBpcnlEaWZmKHRpbWVvdXQpO1xuICAgICAgICBpZiAoZGlmZiA+IDApIHtcbiAgICAgICAgICB0aGlzLnNhZmVDbGVhckludGVydmFsKCdpZGxlSGFuZGxlJyk7XG4gICAgICAgICAgdGhpcy5zZXRJZGxlSW50ZXJ2YWxPdXRzaWRlT2Zab25lKHdhdGNoRm4sIGRpZmYpO1xuICAgICAgICB9IGVsc2Uge1xuICAgICAgICAgIHRoaXMudG9nZ2xlU3RhdGUoKTtcbiAgICAgICAgfVxuICAgICAgfSk7XG4gICAgfTtcblxuICAgIHRoaXMuc2V0SWRsZUludGVydmFsT3V0c2lkZU9mWm9uZSh3YXRjaEZuLCB0aGlzLmlkbGUgKiAxMDAwKTtcbiAgfVxuXG4gIC8qXG4gICAqIEFsbG93cyBwcm90cmFjdG9yIHRlc3RzIHRvIGNhbGwgd2FpdEZvckFuZ3VsYXIgd2l0aG91dCBoYW5naW5nXG4gICAqL1xuICBzZXRJZGxlSW50ZXJ2YWxPdXRzaWRlT2Zab25lKHdhdGNoRm46ICgpID0+IHZvaWQsIGZyZXF1ZW5jeTogbnVtYmVyKTogdm9pZCB7XG4gICAgdGhpcy56b25lLnJ1bk91dHNpZGVBbmd1bGFyKCgpID0+IHtcbiAgICAgIHRoaXMuaWRsZUhhbmRsZSA9IHNldEludGVydmFsKHdhdGNoRm4sIGZyZXF1ZW5jeSk7XG4gICAgfSk7XG4gIH1cblxuICAvKlxuICAgKiBTdG9wcyB3YXRjaGluZyBmb3IgaW5hY3Rpdml0eS5cbiAgICovXG4gIHN0b3AoKTogdm9pZCB7XG4gICAgdGhpcy5zdG9wS2VlcGFsaXZlKCk7XG5cbiAgICB0aGlzLnRvZ2dsZUludGVycnVwdHMoZmFsc2UpO1xuXG4gICAgdGhpcy5zYWZlQ2xlYXJJbnRlcnZhbCgnaWRsZUhhbmRsZScpO1xuICAgIHRoaXMuc2FmZUNsZWFySW50ZXJ2YWwoJ3RpbWVvdXRIYW5kbGUnKTtcblxuICAgIHRoaXMuc2V0SWRsaW5nKGZhbHNlKTtcbiAgICB0aGlzLnJ1bm5pbmcgPSBmYWxzZTtcblxuICAgIHRoaXMuZXhwaXJ5Lmxhc3QobnVsbCk7XG4gIH1cblxuICAvKlxuICAgKiBGb3JjZXMgYSB0aW1lb3V0IGV2ZW50IGFuZCBzdGF0ZS5cbiAgICovXG4gIHRpbWVvdXQoKTogdm9pZCB7XG4gICAgdGhpcy5zdG9wS2VlcGFsaXZlKCk7XG5cbiAgICB0aGlzLnRvZ2dsZUludGVycnVwdHMoZmFsc2UpO1xuXG4gICAgdGhpcy5zYWZlQ2xlYXJJbnRlcnZhbCgnaWRsZUhhbmRsZScpO1xuICAgIHRoaXMuc2FmZUNsZWFySW50ZXJ2YWwoJ3RpbWVvdXRIYW5kbGUnKTtcblxuICAgIHRoaXMuc2V0SWRsaW5nKHRydWUpO1xuICAgIHRoaXMucnVubmluZyA9IGZhbHNlO1xuICAgIHRoaXMuY291bnRkb3duID0gMDtcblxuICAgIHRoaXMub25UaW1lb3V0LmVtaXQobnVsbCk7XG4gIH1cblxuICAvKlxuICAgKiBTaWduYWxzIHRoYXQgdXNlciBhY3Rpdml0eSBoYXMgb2NjdXJyZWQuXG4gICAqIEBwYXJhbSBmb3JjZSAtIEZvcmNlcyB3YXRjaCB0byBiZSBjYWxsZWQsIHVubGVzcyB0aGV5IGFyZSB0aW1lZCBvdXQuXG4gICAqIEBwYXJhbSBldmVudEFyZ3MgLSBPcHRpb25hbCBzb3VyY2UgZXZlbnQgYXJndW1lbnRzLlxuICAgKi9cbiAgaW50ZXJydXB0KGZvcmNlPzogYm9vbGVhbiwgZXZlbnRBcmdzPzogYW55KTogdm9pZCB7XG4gICAgaWYgKCF0aGlzLnJ1bm5pbmcpIHtcbiAgICAgIHJldHVybjtcbiAgICB9XG5cbiAgICBpZiAodGhpcy50aW1lb3V0VmFsICYmIHRoaXMuZXhwaXJ5LmlzRXhwaXJlZCgpKSB7XG4gICAgICB0aGlzLnRpbWVvdXQoKTtcbiAgICAgIHJldHVybjtcbiAgICB9XG4gICAgdGhpcy5vbkludGVycnVwdC5lbWl0KGV2ZW50QXJncyk7XG5cbiAgICBpZiAoXG4gICAgICBmb3JjZSA9PT0gdHJ1ZSB8fFxuICAgICAgdGhpcy5hdXRvUmVzdW1lID09PSBBdXRvUmVzdW1lLmlkbGUgfHxcbiAgICAgICh0aGlzLmF1dG9SZXN1bWUgPT09IEF1dG9SZXN1bWUubm90SWRsZSAmJiAhdGhpcy5leHBpcnkuaWRsaW5nKCkpXG4gICAgKSB7XG4gICAgICB0aGlzLndhdGNoKGZvcmNlKTtcbiAgICB9XG4gIH1cblxuICBwcml2YXRlIHNldElkbGluZyh2YWx1ZTogYm9vbGVhbik6IHZvaWQge1xuICAgIHRoaXMuaWRsaW5nID0gdmFsdWU7XG4gICAgdGhpcy5leHBpcnkuaWRsaW5nKHZhbHVlKTtcbiAgfVxuXG4gIHByaXZhdGUgdG9nZ2xlU3RhdGUoKTogdm9pZCB7XG4gICAgdGhpcy5zZXRJZGxpbmcoIXRoaXMuaWRsaW5nKTtcblxuICAgIGlmICh0aGlzLmlkbGluZykge1xuICAgICAgdGhpcy5vbklkbGVTdGFydC5lbWl0KG51bGwpO1xuICAgICAgdGhpcy5zdG9wS2VlcGFsaXZlKCk7XG5cbiAgICAgIGlmICh0aGlzLnRpbWVvdXRWYWwgPiAwKSB7XG4gICAgICAgIHRoaXMuY291bnRkb3duID0gdGhpcy50aW1lb3V0VmFsO1xuICAgICAgICB0aGlzLmRvQ291bnRkb3duKCk7XG4gICAgICAgIHRoaXMuc2V0VGltb3V0SW50ZXJ2YWxPdXRzaWRlWm9uZSgoKSA9PiB7XG4gICAgICAgICAgdGhpcy5kb0NvdW50ZG93bkluWm9uZSgpO1xuICAgICAgICB9LCAxMDAwKTtcbiAgICAgIH1cbiAgICB9IGVsc2Uge1xuICAgICAgdGhpcy50b2dnbGVJbnRlcnJ1cHRzKHRydWUpO1xuICAgICAgdGhpcy5vbklkbGVFbmQuZW1pdChudWxsKTtcbiAgICAgIHRoaXMuc3RhcnRLZWVwYWxpdmUoKTtcbiAgICB9XG5cbiAgICB0aGlzLnNhZmVDbGVhckludGVydmFsKCdpZGxlSGFuZGxlJyk7XG4gIH1cblxuICBwcml2YXRlIHNldFRpbW91dEludGVydmFsT3V0c2lkZVpvbmUoXG4gICAgaW50ZXJ2YWxGbjogKCkgPT4gdm9pZCxcbiAgICBmcmVxdWVuY3k6IG51bWJlclxuICApIHtcbiAgICB0aGlzLnpvbmUucnVuT3V0c2lkZUFuZ3VsYXIoKCkgPT4ge1xuICAgICAgdGhpcy50aW1lb3V0SGFuZGxlID0gc2V0SW50ZXJ2YWwoKCkgPT4ge1xuICAgICAgICBpbnRlcnZhbEZuKCk7XG4gICAgICB9LCBmcmVxdWVuY3kpO1xuICAgIH0pO1xuICB9XG5cbiAgcHJpdmF0ZSB0b2dnbGVJbnRlcnJ1cHRzKHJlc3VtZTogYm9vbGVhbik6IHZvaWQge1xuICAgIGZvciAoY29uc3QgaW50ZXJydXB0IG9mIHRoaXMuaW50ZXJydXB0cykge1xuICAgICAgaWYgKHJlc3VtZSkge1xuICAgICAgICBpbnRlcnJ1cHQucmVzdW1lKCk7XG4gICAgICB9IGVsc2Uge1xuICAgICAgICBpbnRlcnJ1cHQucGF1c2UoKTtcbiAgICAgIH1cbiAgICB9XG4gIH1cblxuICBwcml2YXRlIGdldEV4cGlyeURpZmYodGltZW91dDogbnVtYmVyKTogbnVtYmVyIHtcbiAgICBjb25zdCBub3c6IERhdGUgPSB0aGlzLmV4cGlyeS5ub3coKTtcbiAgICBjb25zdCBsYXN0OiBEYXRlID0gdGhpcy5leHBpcnkubGFzdCgpIHx8IG5vdztcbiAgICByZXR1cm4gbGFzdC5nZXRUaW1lKCkgLSBub3cuZ2V0VGltZSgpIC0gdGltZW91dCAqIDEwMDA7XG4gIH1cblxuICBwcml2YXRlIGRvQ291bnRkb3duSW5ab25lKCk6IHZvaWQge1xuICAgIHRoaXMuem9uZS5ydW4oKCkgPT4ge1xuICAgICAgdGhpcy5kb0NvdW50ZG93bigpO1xuICAgIH0pO1xuICB9XG5cbiAgcHJpdmF0ZSBkb0NvdW50ZG93bigpOiB2b2lkIHtcbiAgICBjb25zdCBkaWZmID0gdGhpcy5nZXRFeHBpcnlEaWZmKHRoaXMudGltZW91dFZhbCk7XG4gICAgaWYgKGRpZmYgPiAwKSB7XG4gICAgICB0aGlzLnNhZmVDbGVhckludGVydmFsKCd0aW1lb3V0SGFuZGxlJyk7XG4gICAgICB0aGlzLmludGVycnVwdCh0cnVlKTtcbiAgICAgIHJldHVybjtcbiAgICB9XG5cbiAgICBpZiAoIXRoaXMuaWRsaW5nKSB7XG4gICAgICByZXR1cm47XG4gICAgfVxuXG4gICAgaWYgKHRoaXMuY291bnRkb3duIDw9IDApIHtcbiAgICAgIHRoaXMudGltZW91dCgpO1xuICAgICAgcmV0dXJuO1xuICAgIH1cblxuICAgIHRoaXMub25UaW1lb3V0V2FybmluZy5lbWl0KHRoaXMuY291bnRkb3duKTtcbiAgICB0aGlzLmNvdW50ZG93bi0tO1xuICB9XG5cbiAgcHJpdmF0ZSBzYWZlQ2xlYXJJbnRlcnZhbChoYW5kbGVOYW1lOiBzdHJpbmcpOiB2b2lkIHtcbiAgICBjb25zdCBoYW5kbGUgPSB0aGlzW2hhbmRsZU5hbWVdO1xuICAgIGlmIChoYW5kbGUgIT09IG51bGwgJiYgdHlwZW9mIGhhbmRsZSAhPT0gJ3VuZGVmaW5lZCcpIHtcbiAgICAgIGNsZWFySW50ZXJ2YWwodGhpc1toYW5kbGVOYW1lXSk7XG4gICAgICB0aGlzW2hhbmRsZU5hbWVdID0gbnVsbDtcbiAgICB9XG4gIH1cblxuICBwcml2YXRlIHN0YXJ0S2VlcGFsaXZlKCk6IHZvaWQge1xuICAgIGlmICghdGhpcy5rZWVwYWxpdmVTdmMgfHwgIXRoaXMua2VlcGFsaXZlRW5hYmxlZCkge1xuICAgICAgcmV0dXJuO1xuICAgIH1cblxuICAgIGlmICh0aGlzLnJ1bm5pbmcpIHtcbiAgICAgIHRoaXMua2VlcGFsaXZlU3ZjLnBpbmcoKTtcbiAgICB9XG5cbiAgICB0aGlzLmtlZXBhbGl2ZVN2Yy5zdGFydCgpO1xuICB9XG5cbiAgcHJpdmF0ZSBzdG9wS2VlcGFsaXZlKCk6IHZvaWQge1xuICAgIGlmICghdGhpcy5rZWVwYWxpdmVTdmMgfHwgIXRoaXMua2VlcGFsaXZlRW5hYmxlZCkge1xuICAgICAgcmV0dXJuO1xuICAgIH1cblxuICAgIHRoaXMua2VlcGFsaXZlU3ZjLnN0b3AoKTtcbiAgfVxuXG4gIC8qXG4gICAqIENhbGxlZCBieSBBbmd1bGFyIHdoZW4gZGVzdHJveWluZyB0aGUgaW5zdGFuY2UuXG4gICAqL1xuICBuZ09uRGVzdHJveSgpOiB2b2lkIHtcbiAgICB0aGlzLnN0b3AoKTtcbiAgICB0aGlzLmNsZWFySW50ZXJydXB0cygpO1xuICB9XG59XG4iXX0=