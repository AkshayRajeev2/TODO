import { Routes } from '@angular/router';
export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'today' },
  { path: 'today', loadComponent: () => import('./features/today/today.component').then(m => m.TodayComponent) },
  { path: 'tasks', loadComponent: () => import('./features/tasks/task-list.component').then(m => m.TaskListComponent) },
  { path: 'calendar', loadComponent: () => import('./features/calendar/calendar.component').then(m => m.CalendarComponent) },
  { path: '**', redirectTo: 'today' }
];
