import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CalendarDaySummary, DayResponse, Task, TaskInput, TaskStats } from '../models/task.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class TaskApiService {
  private readonly http = inject(HttpClient); private readonly base = environment.apiBaseUrl;
  tasks(): Observable<Task[]> { return this.http.get<Task[]>(`${this.base}/tasks`); }
  create(input: TaskInput): Observable<Task> { return this.http.post<Task>(`${this.base}/tasks`, input); }
  update(id: string, input: TaskInput): Observable<Task> { return this.http.put<Task>(`${this.base}/tasks/${id}`, input); }
  archive(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/tasks/${id}`); }
  day(date: string): Observable<DayResponse> { return this.http.get<DayResponse>(`${this.base}/days/${date}`); }
  setCompletion(date: string, taskId: string, completed: boolean): Observable<void> { return this.http.put<void>(`${this.base}/days/${date}/tasks/${taskId}/completion`, { completed }); }
  calendar(year: number, month: number): Observable<CalendarDaySummary[]> { return this.http.get<CalendarDaySummary[]>(`${this.base}/calendar?year=${year}&month=${month}`); }
  stats(id: string): Observable<TaskStats> { return this.http.get<TaskStats>(`${this.base}/tasks/${id}/stats`); }
}
