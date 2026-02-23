import { Component, Input, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Comment } from '../../../core/models/comment.model';
import { CommentFormComponent } from '../comment-form/comment-form.component';

@Component({
  selector: 'app-comment-item',
  standalone: true,
  imports: [CommonModule, CommentFormComponent],
  templateUrl: './comment-item.component.html'
})
export class CommentItemComponent {
  @Input() comment!: Comment;
  @Input() depth = 0;

  showReply = false;
  lightboxSrc: string | null = null;

  readonly apiBase = 'http://localhost:5000';

  get marginLeft(): string {
    return `${this.depth * 24}px`;
  }

  onNewComment(): void {
    this.showReply = false;
  }

  openLightbox(path: string): void {
    this.lightboxSrc = `${this.apiBase}${path}`;
  }

  closeLightbox(): void {
    this.lightboxSrc = null;
  }
}
