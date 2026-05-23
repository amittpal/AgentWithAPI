import { Routes } from '@angular/router';
import { StudentChatComponent } from './student-chat/student-chat.component';

export const routes: Routes = [
  { path: '', component: StudentChatComponent },
  { path: '**', redirectTo: '', pathMatch: 'full' }
];
