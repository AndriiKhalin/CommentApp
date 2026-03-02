import {
  Component, OnInit, OnDestroy, Input, Output, EventEmitter,
  inject, ViewChild, ElementRef
} from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { CommentService } from '../../../core/services/comment.service';
import { CaptchaService } from '../../../core/services/captcha.service';
import { Comment } from '../../../core/models/comment.model';

@Component({
  selector: 'app-comment-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './comment-form.component.html',
  styleUrl: './comment-form.component.scss'
})
export class CommentFormComponent implements OnInit, OnDestroy {
  @Input() parentId?: number;
  @Output() commentPosted = new EventEmitter<Comment>();
  @ViewChild('messageInput') messageInput?: ElementRef<HTMLTextAreaElement>;
  @ViewChild('fileInput') fileInput?: ElementRef<HTMLInputElement>;

  private fb = inject(FormBuilder);
  private commentService = inject(CommentService);
  private captchaService = inject(CaptchaService);
  private sub?: Subscription;

  captchaUrl = '';
  captchaSessionId = '';
  selectedFile: File | null = null;
  fileError = '';
  showCaptcha = false;
  showPreview = false;
  textareaFocused = false;
  private captchaLoaded = false;

  form = this.fb.group({
    userName: ['', [Validators.required, Validators.pattern(/^[a-zA-Z0-9]+$/)]],
    email: ['', [Validators.required, Validators.email]],
    homePage: ['', [Validators.pattern(/^https?:\/\/.+/)]],
    text: ['', Validators.required],
    captcha: ['', Validators.required],
  });

  ngOnInit(): void {
    this.sub = this.form.valueChanges.subscribe(() => this.checkCaptchaReady());
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
    if (this.captchaUrl) URL.revokeObjectURL(this.captchaUrl);
  }

  // ── Captcha ──

  private checkCaptchaReady(): void {
    const { userName, email, text } = this.form.controls;
    if (userName.valid && email.valid && text.valid && !this.showCaptcha) {
      this.showCaptcha = true;
      if (!this.captchaLoaded) this.loadCaptcha();
    }
  }

  loadCaptcha(): void {
    this.captchaService.getCaptcha().subscribe({
      next: ({ blob, sessionId }) => {
        if (this.captchaUrl) URL.revokeObjectURL(this.captchaUrl);
        this.captchaUrl = URL.createObjectURL(blob);
        this.captchaSessionId = sessionId;
        this.captchaLoaded = true;
      },
      error: () => {
        this.captchaUrl = '';
        this.captchaSessionId = '';
      }
    });
  }

  // ── Tag insertion ──

  wrapSelection(tag: string): void {
    const ta = this.messageInput?.nativeElement;
    if (!ta) return;
    ta.focus();

    const val = this.form.controls.text.value ?? '';
    const start = ta.selectionStart;
    const end = ta.selectionEnd;
    const selected = val.slice(start, end) || 'text';
    const wrapped = `<${tag}>${selected}</${tag}>`;

    this.updateTextValue(val.slice(0, start) + wrapped + val.slice(end), start + wrapped.length);
  }

  insertLink(): void {
    const ta = this.messageInput?.nativeElement;
    if (!ta) return;
    ta.focus();

    const url = prompt('Enter URL:', 'https://');
    if (!url) return;
    const title = prompt('Enter title (optional):', '') ?? '';

    const val = this.form.controls.text.value ?? '';
    const start = ta.selectionStart;
    const end = ta.selectionEnd;
    const selected = val.slice(start, end) || 'link text';
    const titleAttr = title ? ` title="${title}"` : '';
    const tag = `<a href="${url}"${titleAttr}>${selected}</a>`;

    this.updateTextValue(val.slice(0, start) + tag + val.slice(end), start + tag.length);
  }

  private updateTextValue(newValue: string, cursorPos: number): void {
    const ta = this.messageInput?.nativeElement;
    this.form.controls.text.setValue(newValue);
    this.form.controls.text.markAsDirty();
    this.form.controls.text.markAsTouched();

    queueMicrotask(() => {
      ta?.focus();
      ta?.setSelectionRange(cursorPos, cursorPos);
    });
  }

  // ── File ──

  onFileChange(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;

    this.fileError = '';
    const ext = file.name.split('.').pop()?.toLowerCase();

    if (ext === 'txt' && file.size > 100 * 1024) {
      this.fileError = 'Text file must be less than 100KB';
      return;
    }
    if (!['jpg', 'jpeg', 'gif', 'png', 'txt'].includes(ext ?? '')) {
      this.fileError = 'Allowed formats: JPG, GIF, PNG, TXT';
      return;
    }
    this.selectedFile = file;
  }

  removeFile(): void {
    this.selectedFile = null;
    this.fileError = '';
    if (this.fileInput?.nativeElement) this.fileInput.nativeElement.value = '';
  }

  // ── Submit ──

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const fd = new FormData();
    const v = this.form.value;
    fd.append('userName', v.userName!);
    fd.append('email', v.email!);
    if (v.homePage) fd.append('homePage', v.homePage);
    fd.append('text', v.text!);
    fd.append('captcha', v.captcha!);
    fd.append('captchaSessionId', this.captchaSessionId);
    if (this.parentId) fd.append('parentId', String(this.parentId));
    if (this.selectedFile) fd.append('attachment', this.selectedFile);

    this.commentService.createComment(fd).subscribe({
      next: (comment) => {
        this.commentPosted.emit(comment);
        this.resetForm();
      },
      error: (err) => {
        alert(err.error?.error ?? 'Submission error');
        this.loadCaptcha();
      }
    });
  }

  private resetForm(): void {
    this.form.reset();
    this.selectedFile = null;
    this.showPreview = false;
    this.showCaptcha = false;
    this.captchaLoaded = false;
    if (this.fileInput?.nativeElement) this.fileInput.nativeElement.value = '';
  }

  get f() { return this.form.controls; }
}
