# 💬 SPA Comments App

[![.NET](https://img.shields.io/badge/.NET_10-5C2D91?style=for-the-badge&logo=.net&logoColor=white)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white)](https://angular.dev/)
[![MSSQL](https://img.shields.io/badge/Microsoft_SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/sql-server/)
[![SignalR](https://img.shields.io/badge/SignalR-0078D4?style=for-the-badge&logo=microsoft&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet/signalr)
[![Docker](https://img.shields.io/badge/Docker-2CA5E0?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)

Full-stack Single Page Application (SPA) for nested comments with file attachments, CAPTCHA, and real-time updates.

## 🛠 Tech Stack

### Backend
- **ASP.NET Core 10** (Web API)
- **Entity Framework Core** (Code First, MSSQL)
- **MediatR** (CQRS pattern)
- **FluentValidation** (request validation)
- **SignalR** (real-time WebSocket notifications)
- **Redis** (CAPTCHA session caching)
- **ImageSharp** (image resize, CAPTCHA generation)
- **HtmlSanitizer** (XSS protection)

### Frontend
- **Angular 21** (standalone components)
- **RxJS** (reactive data flow)
- **SignalR client** (real-time updates)

### Infrastructure
- **Docker & Docker Compose** (full containerization)
- **Nginx** (reverse proxy for frontend)
- **MSSQL 2022** (database)
- **Redis 7** (cache)

## ✨ Features
- 🧵 **Nested threaded comments** (cascade layout)
- 🗂 **Sortable table** (UserName, Email, Date) — LIFO by default
- 📄 **Pagination** (25 comments per page)
- 🛡️ **CAPTCHA protection** against spam
- 🔒 **Security**: 
  - XSS protection (HTML sanitizer — allowed tags: `<a>`, `<code>`, `<i>`, `<strong>`)
  - SQL injection protection via EF Core parameterized queries
- 🖼️ **Image upload** with auto-resize to max 320×240 pixels
- 📁 **TXT file upload** (max size: 100KB)
- 🔍 **Lightbox** for image preview
- ⚡ **Real-time updates** via WebSocket (SignalR)

## 📐 Architecture

```
┌─────────┐     ┌──────────┐     ┌──────────┐     ┌───────┐
│ Angular │────▶│  Nginx   │────▶│ ASP.NET  │────▶│ MSSQL │
│   SPA   │     │ (proxy)  │     │ Core API │     └───────┘
└─────────┘     └──────────┘     │          │────▶┌───────┐
                                 │ SignalR  │     │ Redis │
                                 └──────────┘     └───────┘
```

## 🚀 Quick Start (Docker)

```bash
git clone https://github.com/AndriiKhalin/CommentApp.git
cd CommentApp
docker compose up -d
```

Open **http://localhost** in your browser.

### Services

| Service  | URL                    | Description        |
|----------|------------------------|--------------------|
| Frontend | http://localhost       | Angular SPA        |
| API      | http://localhost/api   | REST API via Nginx |
| Backend  | http://localhost:5000  | Direct API access  |
| MSSQL    | localhost:1434         | Database           |
| Redis    | localhost:6379         | Cache              |

### Stop

```bash
docker compose down        # stop containers
docker compose down -v     # stop + remove data volumes
```
## 🗄 Database Schema

See [db-schema.sql](./db-schema.sql) for the full schema.

```
Comments
├── Id (PK, INT, IDENTITY)
├── UserName (NVARCHAR 100, NOT NULL)
├── Email (NVARCHAR 200, NOT NULL)
├── HomePage (NVARCHAR 500, NULL)
├── Text (NVARCHAR MAX, NOT NULL)
├── AttachmentPath (NVARCHAR 500, NULL)
├── AttachmentType (NVARCHAR 10, NULL) — 'Image' | 'Text'
├── CreatedAt (DATETIME2, DEFAULT GETUTCDATE())
└── ParentId (INT, NULL, FK → Comments.Id) — self-referencing
```

## 📁 Project Structure

```
SpaComments/
├── backend/
│   ├── CommentsApp.API/          # Controllers, Hubs, Middleware
│   ├── CommentsApp.Application/  # CQRS, DTOs, Services, Validators
│   ├── CommentsApp.Domain/       # Entities, Interfaces
│   ├── CommentsApp.Infrastructure/ # EF Core, Redis, Repositories
│   └── Dockerfile
├── frontend/
│   ├── src/app/
│   │   ├── core/                 # Models, Services
│   │   └── features/comments/    # Form, List, Item components
│   ├── nginx.conf
│   └── Dockerfile
├── docker-compose.yml
├── db-schema.sql
└── README.md
```

## 🔧 Local Development (without Docker)

### Backend
```bash
cd backend
dotnet restore
dotnet run --project CommentsApp.API
```
Requires: .NET 10 SDK, MSSQL, Redis running locally.

### Frontend
```bash
cd frontend
npm install
npm start
```
Opens at http://localhost:4200 with proxy to backend.
