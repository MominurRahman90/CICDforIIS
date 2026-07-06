# CI/CD Setup Guide for .NET Framework 4.8 CRUD API with Oracle

## Project Structure

```
CrudBuroApi.sln
├── CrudBuroApi/
│   ├── App_Start/WebApiConfig.cs
│   ├── Controllers/
│   │   ├── ProductsController.cs
│   │   └── HealthController.cs
│   ├── Data/
│   │   └── ProductRepository.cs
│   ├── Models/
│   │   └── Product.cs
│   ├── Properties/AssemblyInfo.cs
│   ├── CrudBuroApi.csproj
│   ├── packages.config
│   ├── Web.config
│   └── Global.asax(.cs)
└── .github/workflows/
    ├── ci.yml
    └── cd.yml
```

## Prerequisites

### 1. Configure GitHub Secrets

Go to your GitHub repository → Settings → Secrets and variables → Actions

| Secret Name | Description | Example Value |
|-------------|-------------|---------------|
| `IIS_SERVER` | IIS server hostname | `your-server.company.com` |
| `IIS_USERNAME` | IIS deployment username | `deploy_user` |
| `IIS_PASSWORD` | IIS deployment password | `secure_password` |

### 2. Configure GitHub Environments

1. Go to Settings → Environments
2. Create three environments: `dev`, `staging`, `production`
3. For `production`:
   - Enable "Required reviewers"
   - Add at least one reviewer

### 3. Oracle Database Setup

Run the schema script on your Oracle server:

```sql
-- Connect as SYSTEM/DBA
@database/schema.sql
```

### 4. Update Web.config

Update the connection string in `CrudBuroApi\Web.config`:

```xml
<add name="OracleConnection"
     connectionString="User Id=CRUD_USER;Password=your_password;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=YOUR_ORACLE_SERVER)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL)))"
     providerName="Oracle.ManagedDataAccess.Client" />
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | Get all products |
| GET | `/api/products/{id}` | Get product by ID |
| POST | `/api/products` | Create new product |
| PUT | `/api/products/{id}` | Update product |
| DELETE | `/api/products/{id}` | Delete product |
| GET | `/api/health` | Health check |

## CI/CD Pipeline

### CI (ci.yml)
- Triggers on push/PR to `main` and `develop`
- Restores NuGet packages
- Builds solution with MSBuild
- Uploads build artifact

### CD (cd.yml)
- Triggers after successful CI build
- Auto-deploys based on branch:
  - `develop` → Dev environment
  - `main` → Staging environment
  - Manual dispatch → Any environment
- Deploys to IIS via Web Deploy
- Runs health check after deployment

## Local Development

### Build locally
```bash
nuget restore CrudBuroApi.sln
msbuild CrudBuroApi.sln /p:Configuration=Debug
```

### Run locally
1. Open in Visual Studio
2. Set `CrudBuroApi` as startup project
3. Press F5

## Deployment

The CI/CD pipeline handles deployment automatically. For manual deployment:

1. Build in Release mode
2. Use Web Deploy or copy files to IIS
3. Update connection string in Web.config
4. Ensure Oracle client connectivity
