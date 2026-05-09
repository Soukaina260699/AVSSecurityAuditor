# 🛡️ ASVS Security Auditor

> Professional OWASP ASVS compliance assessment platform built with ASP.NET Core 8 MVC

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-blue)
![Entity Framework Core](https://img.shields.io/badge/EF%20Core-8.0-green)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-purple)
![License](https://img.shields.io/badge/License-MIT-yellow)

---

## ✨ Features

| Feature | Description |
|---|---|
| 📥 **CSV Import** | Import official OWASP ASVS v4.0 CSV checklist |
| ✅ **CRUD** | Full management of ASVS requirements & chapters |
| 📋 **Public Checklist** | Browse and search all ASVS requirements |
| 🎯 **Assessment Module** | Mark requirements: Valid / Not Valid / N/A / Pending |
| 📊 **Benchmark Report** | Compliance %, chapter analysis, risk level (Low/Medium/High) |
| 📈 **Dashboard** | Charts, progress bars, security score |
| 🤖 **AI Assistant** | Explain any requirement (DeepSeek / OpenAI / Anthropic) |
| 🖨️ **PDF Export** | Print-to-PDF via browser |
| 🔐 **Identity** | Role-based auth: Admin / User |
| 🌐 **Bilingual** | English & French UI |

---

## 🚀 Quick Start (Local)

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- Git

### Steps

```bash
# 1. Clone the repo
git clone https://github.com/Soukaina260699/AVSSecurityAuditor.git
cd AVSSecurityAuditor

# 2. Restore packages
dotnet restore

# 3. Run the app (SQLite DB auto-created)
dotnet run

# 4. Open in browser
# https://localhost:5001
```

### Default Admin Account
```
Email:    admin@asvs.local
Password: Admin@123
```

---

## 📤 Push to GitHub

```bash
# In your project folder:
git init
git add .
git commit -m "feat: initial ASVS Security Auditor project"
git remote add origin https://github.com/Soukaina260699/AVSSecurityAuditor.git
git branch -M main
git push -u origin main
```

---

## 🌐 Free Deployment Options

### Option 1: Railway (Recommended ⭐)
**Free tier: 500 hours/month, SQLite included**

1. Go to [railway.app](https://railway.app) → Sign up with GitHub
2. New Project → Deploy from GitHub Repo
3. Select `AVSSecurityAuditor`
4. Railway auto-detects ASP.NET Core and builds it
5. Add environment variable: `ASPNETCORE_ENVIRONMENT=Production`
6. Your app is live at `https://xxx.up.railway.app`

### Option 2: Render
**Free tier: auto-sleeps after 15min inactivity**

1. Go to [render.com](https://render.com) → New → Web Service
2. Connect GitHub repo
3. Build Command: `dotnet publish -c Release -o ./publish`
4. Start Command: `dotnet ./publish/AVSSecurityAuditor.dll`
5. Environment: `ASPNETCORE_ENVIRONMENT=Production`

### Option 3: Azure (Free F1 tier)
```bash
# Install Azure CLI, then:
az login
az webapp up --name asvs-auditor --os-type Linux --runtime "DOTNETCORE:8.0"
```

### Option 4: Fly.io (Free 3 shared VMs)

Create `Dockerfile` at project root:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AVSSecurityAuditor.dll"]
```

```bash
fly auth login
fly launch
fly deploy
```

---

## 🤖 AI Configuration

Edit `appsettings.json`:

```json
{
  "AI": {
    "Provider": "deepseek",
    "DeepSeekApiKey": "sk-xxxx",
    "OpenAiApiKey": "sk-xxxx",
    "AnthropicApiKey": "sk-ant-xxxx"
  }
}
```

| Provider | Free Tier | Sign Up |
|---|---|---|
| DeepSeek | ✅ $5 free credits | [platform.deepseek.com](https://platform.deepseek.com) |
| OpenAI | ✅ $5 free credits | [platform.openai.com](https://platform.openai.com) |
| Anthropic | ✅ Limited free | [console.anthropic.com](https://console.anthropic.com) |

---

## 📁 Project Structure

```
AVSSecurityAuditor/
├── Controllers/
│   ├── HomeController.cs
│   ├── AdminController.cs
│   ├── AssessmentController.cs
│   ├── ChecklistAndAccountControllers.cs
├── Data/
│   └── AppDbContext.cs
├── Interfaces/
│   └── IRepositories.cs
├── Models/
│   ├── ApplicationUser.cs
│   ├── AsvsChapter.cs
│   ├── AsvsRequirement.cs
│   └── Assessment.cs
├── Repositories/
│   └── Repositories.cs
├── Services/
│   ├── AiAssistantService.cs
│   ├── CsvImportService.cs
│   └── ReportService.cs
├── ViewModels/
│   └── ViewModels.cs
├── Views/
│   ├── Home/Index.cshtml
│   ├── Admin/
│   ├── Assessment/
│   ├── Checklist/
│   ├── Account/
│   └── Shared/_Layout.cshtml
├── wwwroot/
│   ├── css/site.css
│   └── js/site.js
├── Program.cs
├── appsettings.json
└── AVSSecurityAuditor.csproj
```

---

## 📥 Import ASVS CSV

1. Download the official ASVS 4.0 CSV:
   👉 [OWASP ASVS GitHub](https://github.com/OWASP/ASVS/blob/master/4.0/docs_en/OWASP%20Application%20Security%20Verification%20Standard%204.0.3-en.csv)

2. Login as admin → Admin Panel → Import CSV → Upload file

---

## 🛠️ Tech Stack

- **Backend**: ASP.NET Core 8 MVC, Entity Framework Core, ASP.NET Identity
- **Database**: SQLite (dev) / SQL Server (prod)
- **Frontend**: Bootstrap 5, Chart.js, Font Awesome
- **CSV**: CsvHelper
- **AI**: DeepSeek / OpenAI / Anthropic API

---

## 👤 Default Credentials

| Role | Email | Password |
|---|---|---|
| Admin | admin@asvs.local | Admin@123 |

---

## 📄 License

MIT License — Free to use for academic and personal projects.

---

*Built as a final-year academic project demonstrating OWASP ASVS compliance auditing.*
