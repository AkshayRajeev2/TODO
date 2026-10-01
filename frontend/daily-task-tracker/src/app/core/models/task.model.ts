export type RecurrenceType = 'Daily' | 'Weekly' | 'OneTime';
export interface Task { id: string; title: string; description?: string | null; recurrenceType: RecurrenceType; daysOfWeek: number; startDate: string; endDate?: string | null; color?: string | null; isArchived: boolean; createdAt: string; }
export interface TaskInput { title: string; description?: string | null; recurrenceType: RecurrenceType; daysOfWeek: number; startDate: string; endDate?: string | null; color?: string | null; }
export interface DayTask { id: string; title: string; description?: string | null; recurrenceType: RecurrenceType; color?: string | null; completed: boolean; }
export interface DayResponse { date: string; tasks: DayTask[]; }
export interface CalendarDaySummary { date: string; dueCount: number; completedCount: number; }
export interface TaskStats { taskId: string; currentStreak: number; last30DaysDue: number; last30DaysCompleted: number; last30DaysCompletionRate: number; }
