import { Injectable } from '@angular/core';
@Injectable({ providedIn: 'root' })
export class DateService {
  today(): string { return this.toDateOnly(new Date()); }
  toDateOnly(date: Date): string { return `${date.getFullYear()}-${String(date.getMonth()+1).padStart(2,'0')}-${String(date.getDate()).padStart(2,'0')}`; }
  parse(value: string): Date { const [y,m,d] = value.split('-').map(Number); return new Date(y, m - 1, d); }
  format(value: string): string { return this.parse(value).toLocaleDateString(undefined, { weekday:'long', month:'long', day:'numeric', year:'numeric' }); }
}
