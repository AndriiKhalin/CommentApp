import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Comment, PagedResult } from '../models/comment.model';

@Injectable({ providedIn: 'root' })
export class CommentService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl;

  getComments(
    page = 1,
    pageSize = 25,
    sortBy = 'createdAt',
    descending = true
  ): Observable<PagedResult<Comment>> {
    const params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize)
      .set('sortBy', sortBy)
      .set('descending', descending);

    return this.http.get<PagedResult<Comment>>(`${this.apiUrl}/comments`, { params });
  }

  createComment(formData: FormData): Observable<Comment> {
    return this.http.post<Comment>(`${this.apiUrl}/comments`, formData);
  }
}
