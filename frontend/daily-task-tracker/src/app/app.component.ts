import { Component, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@Component({
    selector: 'app-root', standalone: true, imports: [RouterOutlet, RouterLink, RouterLinkActive, MatToolbarModule, MatIconModule, MatButtonModule], template: `
<mat-toolbar class="topbar"><button mat-icon-button aria-label="Open navigation" (click)="menuOpen.set(!menuOpen())"><mat-icon>menu</mat-icon></button><span>Daily Task Tracker</span><span class="spacer"></span><a mat-button routerLink="/tasks"><mat-icon>checklist</mat-icon> Tasks</a><a mat-button routerLink="/calendar"><mat-icon>calendar_month</mat-icon> Calendar</a></mat-toolbar>
<div class="shell"><aside [class.open]="menuOpen()"><nav aria-label="Main navigation"><a routerLink="/today" routerLinkActive="active" (click)="menuOpen.set(false)"><mat-icon>today</mat-icon>Today</a><a routerLink="/tasks" routerLinkActive="active" (click)="menuOpen.set(false)"><mat-icon>checklist</mat-icon>Tasks</a><a routerLink="/calendar" routerLinkActive="active" (click)="menuOpen.set(false)"><mat-icon>calendar_month</mat-icon>Calendar</a></nav></aside><main><router-outlet /></main></div>`, styles: [`.topbar{position:sticky;top:0;z-index:10}.spacer{flex:1}.shell{display:flex;min-height:calc(100vh - 64px)}aside{width:230px;padding:18px;border-right:1px solid #ddd;background:#fff}nav{display:flex;flex-direction:column;gap:6px}nav a{display:flex;align-items:center;gap:10px;padding:12px;border-radius:8px;text-decoration:none;color:#333}nav a.active{background:#e8eaf6;color:#283593;font-weight:600}main{flex:1;padding:24px;min-width:0}@media(max-width:800px){aside{display:none;position:fixed;top:64px;bottom:0;left:0;z-index:20;box-shadow:0 5px 20px #0002}aside.open{display:block}.topbar a{display:none}main{padding:16px}}`]
})
export class AppComponent { readonly menuOpen = signal(false); }
