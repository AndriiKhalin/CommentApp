import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Comment } from '../../../core/models/comment.model';
import { CommentFormComponent } from '../comment-form/comment-form.component';

@Component({
  selector: 'app-comment-item',
  standalone: true,
  imports: [CommonModule, CommentFormComponent],
  templateUrl: './comment-item.component.html',
  styleUrl: './comment-item.component.scss'
})
export class CommentItemComponent {
  @Input() comment!: Comment;
  @Input() depth = 0;
  @Output() replyPosted = new EventEmitter<void>();

  showReply = false;
  lightboxSrc: string | null = null;

  get marginLeft(): string {
    return `${this.depth * 24}px`;
  }

  onNewComment(): void {
    this.showReply = false;
    this.replyPosted.emit();
  }

  openLightbox(path: string): void {
    this.lightboxSrc = path;
  }

  closeLightbox(): void {
    this.lightboxSrc = null;
  }
}
