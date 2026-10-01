import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Task, TaskInput, RecurrenceType } from '../../core/models/task.model';
import { TaskApiService } from '../../core/services/task-api.service';

@Component({
  selector: 'app-task-editor', standalone: true, imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, MatIconModule], template: `
<div class="editor card"><div class="row head"><h2>{{ task ? 'Edit task' : 'New task' }}</h2><button mat-icon-button type="button" aria-label="Close editor" (click)="closed.emit()"><mat-icon>close</mat-icon></button></div>
<form [formGroup]="form" (ngSubmit)="save()"><mat-form-field appearance="outline" class="full"><mat-label>Title</mat-label><input matInput formControlName="title" maxlength="200" required><mat-error>Title is required.</mat-error></mat-form-field>
<mat-form-field appearance="outline" class="full"><mat-label>Description</mat-label><textarea matInput rows="3" formControlName="description"></textarea></mat-form-field>
<div class="two"><mat-form-field appearance="outline"><mat-label>Recurrence</mat-label><mat-select formControlName="recurrenceType"><mat-option value="Daily">Daily</mat-option><mat-option value="Weekly">Weekly</mat-option><mat-option value="OneTime">One-time</mat-option></mat-select></mat-form-field><mat-form-field appearance="outline"><mat-label>Color</mat-label><input matInput type="color" formControlName="color"></mat-form-field></div>
<div *ngIf="form.controls.recurrenceType.value==='Weekly'" class="weekdays"><label *ngFor="let d of days"><input type="checkbox" [checked]="selectedDay(d.bit)" (change)="toggleDay(d.bit,$any($event.target).checked)"> {{d.name}}</label></div><div *ngIf="form.controls.recurrenceType.value==='Weekly' && mask===0" class="week-error">Select at least one weekday.</div>
<div class="two"><mat-form-field appearance="outline"><mat-label>Start date</mat-label><input matInput type="date" formControlName="startDate"></mat-form-field><mat-form-field appearance="outline"><mat-label>End date (optional)</mat-label><input matInput type="date" formControlName="endDate"></mat-form-field></div>
<div class="actions"><button mat-button type="button" (click)="closed.emit()">Cancel</button><button mat-flat-button color="primary" type="submit" [disabled]="form.invalid || saving || (form.controls.recurrenceType.value==='Weekly' && mask===0)">{{saving?'Saving…':'Save'}}</button></div></form></div>`, styles: [`.editor{max-width:620px;margin:auto}.head{justify-content:space-between;margin-bottom:8px}.head h2{margin:0}.two{display:grid;grid-template-columns:1fr 1fr;gap:12px}.weekdays{display:flex;flex-wrap:wrap;gap:10px;padding:4px 0 16px}.weekdays label{font-size:14px}.week-error{color:#b3261e;font-size:12px;margin-top:-8px;margin-bottom:12px}.actions{display:flex;justify-content:flex-end;gap:8px}@media(max-width:600px){.two{grid-template-columns:1fr}}`]
})
export class TaskEditorComponent {
  @Input() task: Task | null = null; @Output() closed = new EventEmitter<void>(); @Output() saved = new EventEmitter<Task>();
  private fb = inject(FormBuilder); private api = inject(TaskApiService); saving = false; days = [{ name: 'Mon', bit: 1 }, { name: 'Tue', bit: 2 }, { name: 'Wed', bit: 4 }, { name: 'Thu', bit: 8 }, { name: 'Fri', bit: 16 }, { name: 'Sat', bit: 32 }, { name: 'Sun', bit: 64 }]; mask = 0;
  form = this.fb.nonNullable.group({ title: ['', [Validators.required, Validators.maxLength(200)]], description: [''], recurrenceType: ['Daily' as RecurrenceType], startDate: [this.localToday(), Validators.required], endDate: [''], color: ['#5c6bc0'] });
  ngOnChanges() { if (this.task) { this.mask = this.task.daysOfWeek; this.form.patchValue({ title: this.task.title, description: this.task.description ?? '', recurrenceType: this.task.recurrenceType, startDate: this.task.startDate, endDate: this.task.endDate ?? '', color: this.task.color ?? '#5c6bc0' }); } }
  selectedDay(bit: number) { return (this.mask & bit) !== 0 } toggleDay(bit: number, checked: boolean) { this.mask = checked ? this.mask | bit : this.mask & ~bit }
  save() { if (this.form.invalid) return; const v = this.form.getRawValue(); const input: TaskInput = { title: v.title, description: v.description || null, recurrenceType: v.recurrenceType, daysOfWeek: v.recurrenceType === 'Weekly' ? this.mask : 0, startDate: v.startDate, endDate: v.endDate || null, color: v.color || null }; this.saving = true; const request = this.task ? this.api.update(this.task.id, input) : this.api.create(input); request.subscribe({ next: t => { this.saving = false; this.saved.emit(t) }, error: () => this.saving = false }); }
  private localToday() { const d = new Date(); return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}` }
}
