import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Comment, PagedResult } from '../models/comment.model';

@Injectable({ providedIn: 'root' })
export class CommentService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5000/api';

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

  getCaptcha(): Observable<{ blob: Blob; sessionId: string }> {
    return new Observable(observer => {
      fetch(`${this.apiUrl}/captcha`)
        .then(async res => {
          const sessionId = res.headers.get('X-Captcha-Session') ?? '';
          const blob = await res.blob();
          observer.next({ blob, sessionId });
          observer.complete();
        })
        .catch(err => observer.error(err));
    });
  }
}
