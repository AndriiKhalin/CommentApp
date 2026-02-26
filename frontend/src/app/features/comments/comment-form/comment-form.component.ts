import {
  Component, OnInit, Input, Output, EventEmitter, inject, ViewChild, ElementRef
} from '@angular/core';
import {
  ReactiveFormsModule, FormBuilder, Validators, AbstractControl
} from '@angular/forms';
import { CommonModule } from '@angular/common';
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
export class CommentFormComponent implements OnInit {
  @Input() parentId?: number;
  @Output() commentPosted = new EventEmitter<Comment>();
  @ViewChild('messageInput') messageInput?: ElementRef<HTMLTextAreaElement>;

  private fb = inject(FormBuilder);
  private commentService = inject(CommentService);
  private captchaService = inject(CaptchaService);

  captchaUrl = '';
  captchaSessionId = '';
  selectedFile: File | null = null;
  fileError = '';
  preview = '';

  form = this.fb.group({
    userName: ['', [
      Validators.required,
      Validators.pattern(/^[a-zA-Z0-9]+$/)
    ]],
    email: ['', [Validators.required, Validators.email]],
    homePage: ['', [Validators.pattern(/^https?:\/\/.+/)]],
    captcha: ['', Validators.required],
    text: ['', Validators.required]
  });

  ngOnInit(): void {
    this.loadCaptcha();

    // Live preview
    this.form.get('text')!.valueChanges.subscribe(val => {
      this.preview = val ?? '';
    });
  }

  ngOnDestroy(): void {
    if (this.captchaUrl) {
      URL.revokeObjectURL(this.captchaUrl);
    }
  }

  loadCaptcha(): void {
    this.captchaService.getCaptcha().subscribe({
      next: ({ blob, sessionId }) => {
        if (this.captchaUrl) {
          URL.revokeObjectURL(this.captchaUrl);
        }
        this.captchaUrl = URL.createObjectURL(blob);
        this.captchaSessionId = sessionId;
      },
      error: () => {
        this.captchaUrl = '';
        this.captchaSessionId = '';
      }
    });
  }

  insertTag(open: string, close: string): void {
  const textarea = this.messageInput?.nativeElement;
  if (!textarea) return;

  const current = this.form.controls.text.value ?? '';
  const start = textarea.selectionStart ?? current.length;
  const end = textarea.selectionEnd ?? current.length;

  const selected = current.slice(start, end) || 'text';

  const next =
    current.slice(0, start) +
    open +
    selected +
    close +
    current.slice(end);

  this.form.controls.text.setValue(next);
  this.form.controls.text.markAsDirty();
  this.form.controls.text.markAsTouched();

  queueMicrotask(() => {
    const pos = start + open.length + selected.length + close.length;
    textarea.focus();
    textarea.setSelectionRange(pos, pos);
  });
}

  onFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
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
        this.form.reset();
        this.loadCaptcha();
      },
      error: (err) => {
        alert(err.error?.error ?? 'Submission error');
        this.loadCaptcha();
      }
    });
  }

  get f() { return this.form.controls; }
}
