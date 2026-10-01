import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { TaskApiService } from '../../core/services/task-api.service';
import { DateService } from '../../core/services/date.service';
import { DayTask } from '../../core/models/task.model';

@Component({selector:'app-today',standalone:true,imports:[CommonModule,MatCheckboxModule,MatProgressBarModule,MatButtonModule,RouterLink],template:`
<section class="page-header"><div><h1>Today</h1><div class="muted">{{ dateService.format(today) }}</div></div><a mat-flat-button color="primary" routerLink="/tasks">Manage tasks</a></section>
<div class="card loading" *ngIf="loading()">Loading today…</div>
<ng-container *ngIf="!loading()"><div class="card progress-card"><div class="row"><strong>{{ completedCount() }} of {{ tasks().length }} done</strong><span class="progress-text">{{ percent() }}%</span></div><mat-progress-bar mode="determinate" [value]="percent()" aria-label="Today's completion progress"></mat-progress-bar></div>
<div class="card task-card" *ngIf="tasks().length; else empty"><div class="task-row" *ngFor="let task of tasks(); trackBy: trackById"><mat-checkbox [checked]="task.completed" [attr.aria-label]="'Mark '+task.title+' complete'" (change)="toggle(task,$event.checked)"><span [style.border-left]="'4px solid '+(task.color || '#5c6bc0')" class="task-title">{{ task.title }}</span></mat-checkbox><span class="muted recurrence">{{ task.recurrenceType === 'OneTime' ? 'One-time' : task.recurrenceType === 'Weekly' ? 'Weekly' : 'Daily' }}</span></div></div></ng-container>
<ng-template #empty><div class="card empty"><h3>No tasks due today</h3><p class="muted">Create a task to start tracking your progress.</p><a mat-stroked-button routerLink="/tasks">Create task</a></div></ng-template>`,styles:[`.progress-card{margin-bottom:16px}.progress-card .row{justify-content:space-between;margin-bottom:10px}.task-card{padding:8px 20px}.task-row{display:flex;align-items:center;justify-content:space-between;padding:14px 0;border-bottom:1px solid #eee}.task-row:last-child{border-bottom:0}.task-title{display:inline-block;padding-left:10px}.recurrence{font-size:12px}@media(max-width:600px){.recurrence{display:none}}`]})
export class TodayComponent implements OnInit {
  readonly api=inject(TaskApiService); readonly dateService=inject(DateService); readonly today=this.dateService.today(); readonly tasks=signal<DayTask[]>([]); readonly loading=signal(true);
  readonly completedCount=computed(()=>this.tasks().filter(t=>t.completed).length); readonly percent=computed(()=>this.tasks().length ? Math.round(this.completedCount()/this.tasks().length*100) : 0);
  ngOnInit(){this.load()}
  load(){this.loading.set(true);this.api.day(this.today).subscribe({next:r=>{this.tasks.set(r.tasks);this.loading.set(false)},error:()=>this.loading.set(false)})}
  toggle(task:DayTask,completed:boolean){const before=task.completed;this.tasks.update(xs=>xs.map(x=>x.id===task.id?{...x,completed}:x));this.api.setCompletion(this.today,task.id,completed).subscribe({error:()=>this.tasks.update(xs=>xs.map(x=>x.id===task.id?{...x,completed:before}:x))})}
  trackById(_:number,t:DayTask){return t.id}
}
