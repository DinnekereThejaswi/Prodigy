/*
 * A class for managing an interrupt from an interrupt source.
 */
export class Interrupt {
    constructor(source) {
        this.source = source;
    }
    /*
     * Subscribes to the interrupt using the specified function.
     * @param fn - The subscription function.
     */
    subscribe(fn) {
        this.sub = this.source.onInterrupt.subscribe(fn);
    }
    /*
     * Unsubscribes the interrupt.
     */
    unsubscribe() {
        this.sub.unsubscribe();
        this.sub = null;
    }
    /*
     * Keeps the subscription but resumes interrupt events.
     */
    resume() {
        this.source.attach();
    }
    /*
     * Keeps the subscription but pauses interrupt events.
     */
    pause() {
        this.source.detach();
    }
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiaW50ZXJydXB0LmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vLi4vLi4vLi4vcHJvamVjdHMvY29yZS9zcmMvbGliL2ludGVycnVwdC50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFLQTs7R0FFRztBQUNILE1BQU0sT0FBTyxTQUFTO0lBR3BCLFlBQW1CLE1BQXVCO1FBQXZCLFdBQU0sR0FBTixNQUFNLENBQWlCO0lBQUcsQ0FBQztJQUU5Qzs7O09BR0c7SUFDSCxTQUFTLENBQUMsRUFBaUM7UUFDekMsSUFBSSxDQUFDLEdBQUcsR0FBRyxJQUFJLENBQUMsTUFBTSxDQUFDLFdBQVcsQ0FBQyxTQUFTLENBQUMsRUFBRSxDQUFDLENBQUM7SUFDbkQsQ0FBQztJQUVEOztPQUVHO0lBQ0gsV0FBVztRQUNULElBQUksQ0FBQyxHQUFHLENBQUMsV0FBVyxFQUFFLENBQUM7UUFDdkIsSUFBSSxDQUFDLEdBQUcsR0FBRyxJQUFJLENBQUM7SUFDbEIsQ0FBQztJQUVEOztPQUVHO0lBQ0gsTUFBTTtRQUNKLElBQUksQ0FBQyxNQUFNLENBQUMsTUFBTSxFQUFFLENBQUM7SUFDdkIsQ0FBQztJQUVEOztPQUVHO0lBQ0gsS0FBSztRQUNILElBQUksQ0FBQyxNQUFNLENBQUMsTUFBTSxFQUFFLENBQUM7SUFDdkIsQ0FBQztDQUNGIiwic291cmNlc0NvbnRlbnQiOlsiaW1wb3J0IHsgU3Vic2NyaXB0aW9uIH0gZnJvbSAncnhqcyc7XG5cbmltcG9ydCB7IEludGVycnVwdEFyZ3MgfSBmcm9tICcuL2ludGVycnVwdGFyZ3MnO1xuaW1wb3J0IHsgSW50ZXJydXB0U291cmNlIH0gZnJvbSAnLi9pbnRlcnJ1cHRzb3VyY2UnO1xuXG4vKlxuICogQSBjbGFzcyBmb3IgbWFuYWdpbmcgYW4gaW50ZXJydXB0IGZyb20gYW4gaW50ZXJydXB0IHNvdXJjZS5cbiAqL1xuZXhwb3J0IGNsYXNzIEludGVycnVwdCB7XG4gIHByaXZhdGUgc3ViOiBTdWJzY3JpcHRpb247XG5cbiAgY29uc3RydWN0b3IocHVibGljIHNvdXJjZTogSW50ZXJydXB0U291cmNlKSB7fVxuXG4gIC8qXG4gICAqIFN1YnNjcmliZXMgdG8gdGhlIGludGVycnVwdCB1c2luZyB0aGUgc3BlY2lmaWVkIGZ1bmN0aW9uLlxuICAgKiBAcGFyYW0gZm4gLSBUaGUgc3Vic2NyaXB0aW9uIGZ1bmN0aW9uLlxuICAgKi9cbiAgc3Vic2NyaWJlKGZuOiAoYXJnczogSW50ZXJydXB0QXJncykgPT4gdm9pZCk6IHZvaWQge1xuICAgIHRoaXMuc3ViID0gdGhpcy5zb3VyY2Uub25JbnRlcnJ1cHQuc3Vic2NyaWJlKGZuKTtcbiAgfVxuXG4gIC8qXG4gICAqIFVuc3Vic2NyaWJlcyB0aGUgaW50ZXJydXB0LlxuICAgKi9cbiAgdW5zdWJzY3JpYmUoKTogdm9pZCB7XG4gICAgdGhpcy5zdWIudW5zdWJzY3JpYmUoKTtcbiAgICB0aGlzLnN1YiA9IG51bGw7XG4gIH1cblxuICAvKlxuICAgKiBLZWVwcyB0aGUgc3Vic2NyaXB0aW9uIGJ1dCByZXN1bWVzIGludGVycnVwdCBldmVudHMuXG4gICAqL1xuICByZXN1bWUoKTogdm9pZCB7XG4gICAgdGhpcy5zb3VyY2UuYXR0YWNoKCk7XG4gIH1cblxuICAvKlxuICAgKiBLZWVwcyB0aGUgc3Vic2NyaXB0aW9uIGJ1dCBwYXVzZXMgaW50ZXJydXB0IGV2ZW50cy5cbiAgICovXG4gIHBhdXNlKCk6IHZvaWQge1xuICAgIHRoaXMuc291cmNlLmRldGFjaCgpO1xuICB9XG59XG4iXX0=