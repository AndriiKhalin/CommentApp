import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CaptchaService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  getCaptcha() {
    return this.http.get(`${this.apiUrl}/captcha`, {
       observe: 'response',
        responseType: 'blob'
      })
      .pipe(map(res => ({
        blob: res.body!,
        sessionId: res.headers.get('X-Captcha-Session') || ''
      })));
  }
}
