# 💬 SPA Comments App

[![.NET](https://img.shields.io/badge/.NET_10-5C2D91?style=for-the-badge&logo=.net&logoColor=white)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white)](https://angular.dev/)
[![MSSQL](https://img.shields.io/badge/Microsoft_SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/sql-server/)
[![SignalR](https://img.shields.io/badge/SignalR-0078D4?style=for-the-badge&logo=microsoft&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet/signalr)
[![Docker](https://img.shields.io/badge/Docker-2CA5E0?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)

Full-stack Single Page Application (SPA) for nested comments with file attachments, CAPTCHA, and real-time updates.

## 🛠 Tech Stack
- **Backend**: .NET 10, ASP.NET Core, Entity Framework Core
- **Database**: MSSQL (Microsoft SQL Server)
- **Frontend**: Angular + TypeScript
- **Real-time**: SignalR (WebSocket)
- **Containerization**: Docker + Docker Compose

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

## 🚀 Quick Start

Follow these steps to run the application locally using Docker:

```bash
# 1. Clone the repository
git clone https://github.com/AndriiKhalin/CommentApp.git
cd CommentApp

# 2. Build and start the containers
docker compose up --build
```

**The application will be available at:**
- 🌐 **Frontend**: `http://localhost`
- ⚙️ **API**: `http://localhost:5000`
- 📚 **Swagger UI**: `http://localhost:5000/swagger`

## 📂 Project Structure
```text
📦 CommentApp
 ┣ 📂 backend/             # .NET solution (Domain, Application, Infrastructure, API)
 ┣ 📂 frontend/            # Angular SPA
 ┣ 📜 docker-compose.yml   # Docker orchestration for app & DB
 ┗ 📜 db-schema.sql        # Database initialization script
```
