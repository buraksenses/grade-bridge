# GradeBridge Backend

Backend skeleton for the GradeBridge / Not Aktarım Çekirdeği project.

## Projects

```text
backend/
├── GradeBridge.Backend.sln
├── Directory.Build.props
└── src/
    ├── GradeBridge.Api
    ├── GradeBridge.Application
    ├── GradeBridge.Domain
    └── GradeBridge.Infrastructure
```

## Current scope

* Upload optical-result Excel/CSV files
* Parse rows into normalized grade rows
* Validate common grade errors
* Store import jobs and parsed rows
* Review/approve rows
* Export approved rows as CSV
* Simulate transfer through a mock adapter

## Not included yet

* React frontend
* Real UBYS/OBS API adapter
* RPA/browser automation
* SMS/login automation
* Production authentication

## Local DB

Default connection string expects SQL Server on `localhost:1433`:

```text
Server=localhost,1433;Database=GradeBridgeDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;
```

A simple Docker Compose file is included for SQL Server.

## Commands

```bash
cd backend

docker compose up -d

dotnet restore

dotnet ef migrations add InitialCreate --project src/GradeBridge.Infrastructure --startup-project src/GradeBridge.Api

dotnet ef database update \\
  --project src/GradeBridge.Infrastructure \\
  --startup-project src/GradeBridge.Api

dotnet run --project src/GradeBridge.Api
```

Swagger:

```text
https://localhost:7001/swagger
http://localhost:5000/swagger
```

## First demo flow

1. `POST /api/import-jobs/upload`
2. `GET /api/import-jobs/{id}/rows`
3. `POST /api/import-jobs/{id}/approve`
4. `GET /api/import-jobs/{id}/export/csv`
5. `POST /api/import-jobs/{id}/transfer/mock`

