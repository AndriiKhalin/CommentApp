import {
  Component, OnInit, Input, Output, EventEmitter, inject
} from '@angular/core';
import {
  ReactiveFormsModule, FormBuilder, Validators, AbstractControl
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CommentService } from '../../../core/services/comment.service';
import { Comment } from '../../../core/models/comment.model';

@Component({
  selector: 'app-comment-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './comment-form.component.html'
})
export class CommentFormComponent implements OnInit {
  @Input() parentId?: number;
  @Output() commentPosted = new EventEmitter<Comment>();

  private fb = inject(FormBuilder);
  private commentService = inject(CommentService);

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

  loadCaptcha(): void {
    this.commentService.getCaptcha().subscribe(({ blob, sessionId }) => {
      this.captchaUrl = URL.createObjectURL(blob);
      this.captchaSessionId = sessionId;
    });
  }

  insertTag(open: string, close: string): void {
    const textarea = document.querySelector<HTMLTextAreaElement>('textarea[formControlName="text"]');
    if (!textarea) return;

    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const current = this.form.get('text')!.value ?? '';
    const newVal = current.slice(0, start) + open + current.slice(start, end) + close + current.slice(end);
    this.form.get('text')!.setValue(newVal);
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
