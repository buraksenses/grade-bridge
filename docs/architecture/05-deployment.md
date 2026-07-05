# GradeBridge Architecture - 05 Deployment

## Local Development

```text
React Web App      → localhost:5173
.NET API           → localhost:5000 / Rider port
SQL Server Docker  → localhost:1433
```

## Recommended MVP Deployment

```text
Frontend: Azure Static Web Apps or Vercel
Backend: Azure App Service
Database: Azure SQL
File Storage: Azure Blob Storage
DNS: Cloudflare
CI/CD: GitHub Actions
```

## Environment Variables

Secrets must not be stored in source code.

```text
ConnectionStrings\_\_DefaultConnection
Jwt\_\_Secret
Storage\_\_ConnectionString
ObsApi\_\_BaseUrl
ObsApi\_\_ClientId
ObsApi\_\_ClientSecret
```

## File Storage

Do not store uploaded files directly in SQL Server.

Correct approach:

```text
Original file → Blob Storage
File metadata → SQL Server
```

## Whitelist / Static IP

If an institution requires IP whitelist, the GradeBridge backend outbound IP must be shared. External API calls must originate from the backend, not the frontend.

