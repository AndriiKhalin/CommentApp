import { Component } from '@angular/core';
import { CommentListComponent } from './features/comments/comment-list/comment-list.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommentListComponent],
  template: `<app-comment-list />`
})
export class AppComponent {}
