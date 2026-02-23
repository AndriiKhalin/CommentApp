export interface Comment {
  id: number;
  userName: string;
  email: string;
  homePage?: string;
  text: string;
  attachmentPath?: string;
  attachmentType?: 'Image' | 'Text';
  createdAt: string;
  parentId?: number;
  replies: Comment[];
}

export interface PagedResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

export interface CreateCommentRequest {
  userName: string;
  email: string;
  homePage?: string;
  text: string;
  captcha: string;
  captchaSessionId: string;
  parentId?: number;
}
