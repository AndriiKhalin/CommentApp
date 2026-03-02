import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { CommentService } from '../../../core/services/comment.service';
import { RealtimeService } from '../../../core/services/realtime.service';
import { Comment, PagedResult } from '../../../core/models/comment.model';
import { CommentFormComponent } from '../comment-form/comment-form.component';
import { CommentItemComponent } from '../comment-item/comment-item.component';

type SortField = 'createdAt' | 'userName' | 'email';

@Component({
  selector: 'app-comment-list',
  standalone: true,
  imports: [CommonModule, CommentFormComponent, CommentItemComponent],
  templateUrl: './comment-list.component.html',
  styleUrl: './comment-list.component.scss'
})
export class CommentListComponent implements OnInit, OnDestroy {
  private commentService = inject(CommentService);
  private realtimeService = inject(RealtimeService);
  private sub?: Subscription;

  result: PagedResult<Comment> | null = null;
  page = 1;
  sortBy: SortField = 'createdAt';
  descending = true;
  expandedId: number | null = null;

  ngOnInit(): void {
    this.loadComments();
    this.realtimeService.connect();
    this.sub = this.realtimeService.newComment$.subscribe(() => this.loadComments());
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
    this.realtimeService.disconnect();
  }

  loadComments(): void {
    this.commentService
      .getComments(this.page, 25, this.sortBy, this.descending)
      .subscribe(data => this.result = data);
  }

  toggleExpand(id: number): void {
    this.expandedId = this.expandedId === id ? null : id;
  }

  stripHtml(html: string): string {
    return html?.replace(/<[^>]*>/g, '') ?? '';
  }

  handleSort(field: SortField): void {
    if (this.sortBy === field) {
      this.descending = !this.descending;
    } else {
      this.sortBy = field;
      this.descending = true;
    }
    this.page = 1;
    this.expandedId = null;
    this.loadComments();
  }

  sortIcon(field: SortField): string {
    if (this.sortBy !== field) return '';
    return this.descending ? '▼' : '▲';
  }

  get totalPages(): number {
    if (!this.result) return 1;
    return Math.max(1, Math.ceil(this.result.total / 25));
  }

  get pages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  goToPage(p: number): void {
    if (p < 1 || p > this.totalPages) return;
    this.page = p;
    this.expandedId = null;
    this.loadComments();
  }
}
