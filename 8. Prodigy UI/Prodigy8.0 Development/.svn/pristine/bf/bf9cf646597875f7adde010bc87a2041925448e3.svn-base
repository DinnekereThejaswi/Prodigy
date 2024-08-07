import { NgModule } from '@angular/core';
import { Idle } from './idle';
import { IdleExpiry } from './idleexpiry';
import { LocalStorageExpiry } from './localstorageexpiry';
import { LocalStorage } from './localstorage';
export class NgIdleModule {
    static forRoot() {
        return {
            ngModule: NgIdleModule,
            providers: [
                LocalStorageExpiry,
                { provide: IdleExpiry, useExisting: LocalStorageExpiry },
                Idle
            ]
        };
    }
}
NgIdleModule.decorators = [
    { type: NgModule, args: [{
                providers: [LocalStorage]
            },] }
];
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoibW9kdWxlLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vLi4vLi4vLi4vcHJvamVjdHMvY29yZS9zcmMvbGliL21vZHVsZS50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQSxPQUFPLEVBQXVCLFFBQVEsRUFBRSxNQUFNLGVBQWUsQ0FBQztBQUU5RCxPQUFPLEVBQUUsSUFBSSxFQUFFLE1BQU0sUUFBUSxDQUFDO0FBQzlCLE9BQU8sRUFBRSxVQUFVLEVBQUUsTUFBTSxjQUFjLENBQUM7QUFDMUMsT0FBTyxFQUFFLGtCQUFrQixFQUFFLE1BQU0sc0JBQXNCLENBQUM7QUFDMUQsT0FBTyxFQUFFLFlBQVksRUFBRSxNQUFNLGdCQUFnQixDQUFDO0FBSzlDLE1BQU0sT0FBTyxZQUFZO0lBQ3ZCLE1BQU0sQ0FBQyxPQUFPO1FBQ1osT0FBTztZQUNMLFFBQVEsRUFBRSxZQUFZO1lBQ3RCLFNBQVMsRUFBRTtnQkFDVCxrQkFBa0I7Z0JBQ2xCLEVBQUUsT0FBTyxFQUFFLFVBQVUsRUFBRSxXQUFXLEVBQUUsa0JBQWtCLEVBQUU7Z0JBQ3hELElBQUk7YUFDTDtTQUNGLENBQUM7SUFDSixDQUFDOzs7WUFiRixRQUFRLFNBQUM7Z0JBQ1IsU0FBUyxFQUFFLENBQUMsWUFBWSxDQUFDO2FBQzFCIiwic291cmNlc0NvbnRlbnQiOlsiaW1wb3J0IHsgTW9kdWxlV2l0aFByb3ZpZGVycywgTmdNb2R1bGUgfSBmcm9tICdAYW5ndWxhci9jb3JlJztcblxuaW1wb3J0IHsgSWRsZSB9IGZyb20gJy4vaWRsZSc7XG5pbXBvcnQgeyBJZGxlRXhwaXJ5IH0gZnJvbSAnLi9pZGxlZXhwaXJ5JztcbmltcG9ydCB7IExvY2FsU3RvcmFnZUV4cGlyeSB9IGZyb20gJy4vbG9jYWxzdG9yYWdlZXhwaXJ5JztcbmltcG9ydCB7IExvY2FsU3RvcmFnZSB9IGZyb20gJy4vbG9jYWxzdG9yYWdlJztcblxuQE5nTW9kdWxlKHtcbiAgcHJvdmlkZXJzOiBbTG9jYWxTdG9yYWdlXVxufSlcbmV4cG9ydCBjbGFzcyBOZ0lkbGVNb2R1bGUge1xuICBzdGF0aWMgZm9yUm9vdCgpOiBNb2R1bGVXaXRoUHJvdmlkZXJzPE5nSWRsZU1vZHVsZT4ge1xuICAgIHJldHVybiB7XG4gICAgICBuZ01vZHVsZTogTmdJZGxlTW9kdWxlLFxuICAgICAgcHJvdmlkZXJzOiBbXG4gICAgICAgIExvY2FsU3RvcmFnZUV4cGlyeSxcbiAgICAgICAgeyBwcm92aWRlOiBJZGxlRXhwaXJ5LCB1c2VFeGlzdGluZzogTG9jYWxTdG9yYWdlRXhwaXJ5IH0sXG4gICAgICAgIElkbGVcbiAgICAgIF1cbiAgICB9O1xuICB9XG59XG4iXX0=