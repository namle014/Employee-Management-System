# 🏢 HR Management System

---

## 📖 Overview

The backend of the HR Management System is a RESTful API built with **C# .NET Core**, responsible for handling all business logic, data processing, authentication, and communication with the SQL Server database. It serves as the backbone connecting the frontend client to the data layer securely and efficiently.

---

## 🛠️ Tech Stack

| Technology | Description |
|------------|-------------|
| **C# .NET Core** | Primary framework for building the RESTful API |
| **ASP.NET Core Web API** | HTTP request handling and routing |
| **Entity Framework Core** | ORM for database access and migrations |
| **SQL Server** | Relational database for persistent storage |
| **JWT (JSON Web Token)** | Stateless authentication and authorization |
| **ASP.NET Core Identity** | User and role management |
| **BCrypt / SHA-256** | Password hashing |
| **HTTPS** | Encrypted client-server communication |
| **CORS** | Cross-origin request policy |

---

## 📁 Project Structure

```
Employee Management System.sln
├── Employee Management System/    # ASP.NET Core Web API — controllers, middleware, startup
├── OA.Domain/                     # Domain layer — entities, interfaces, enums
├── OA.Infrastructure.EF/          # EF Core — DbContext, migrations, configurations
├── OA.Infrastructure.SQL/         # Raw SQL scripts, stored procedures, seed data
├── OA.Repository/                 # Repository pattern — data access implementations
└── OA.Service/                    # Service layer — business logic
```

The solution follows a **Clean Architecture** / layered architecture pattern:

| Layer | Project | Responsibility |
|-------|---------|---------------|
| **Presentation** | `Employee Management System` | API controllers, routing, middleware, DI setup |
| **Domain** | `OA.Domain` | Core entities, interfaces, business rules |
| **Infrastructure (EF)** | `OA.Infrastructure.EF` | EF Core DbContext, migrations, table configurations |
| **Infrastructure (SQL)** | `OA.Infrastructure.SQL` | Raw SQL scripts, stored procedures, seed data |
| **Repository** | `OA.Repository` | Data access implementations (Repository pattern) |
| **Service** | `OA.Service` | Business logic, orchestration between repositories |

---

## 🗂️ Database Schema

The system includes 27+ tables. Key tables are listed below:

| Table | Purpose |
|-------|---------|
| `AspNetUsers` | Employee account and profile data |
| `AspNetRoles` | Role definitions with salary coefficients |
| `AspNetUserRoles` | User–role mapping |
| `Timekeeping` | Attendance check-in/check-out records |
| `TimeOff` | Leave requests and approval status |
| `Salary` | Monthly payroll records |
| `EmployeeContract` | Labor contract details |
| `Department` | Department structure |
| `Reward` | Reward records per employee |
| `Discipline` | Disciplinary records per employee |
| `Notifications` | Internal notification messages |
| `NotificationFiles` | Files attached to notifications |
| `NotificationRoles` | Notifications targeted by role |
| `NotificationDepartments` | Notifications targeted by department |
| `Events` | Internal events and calendar entries |
| `WorkingRules` | Company working regulations |
| `JobHistory` | Employee work history |
| `PromotionHistory` | Promotion records |
| `TransferHistory` | Department transfer records |
| `SysFunctions` | System function/menu management |
| `SysFiles` | File storage metadata |
| `Holiday` | Public holidays configuration |
| `Messenger` | Chat/messaging records |
| `UserNotifications` | Per-user notification read status |
| `ErrorReport` | Employee-submitted error/issue reports |

---

## 🔐 Authentication & Authorization

The API uses **JWT Bearer Token** authentication combined with **ASP.NET Core Identity** for role management.

**Authentication flow:**
1. Client sends credentials to `POST /api/auth/login`
2. Server validates credentials, generates a signed JWT
3. Client includes the token in the `Authorization: Bearer <token>` header for all subsequent requests
4. Server validates the token and checks role permissions on protected endpoints

**Roles:**
- `Admin / Manager` — full system access
- `HR` — contracts, payroll, rewards, discipline
- `Employee` — personal data, attendance, leave, payslip

**Security measures:**
- Passwords hashed with **BCrypt**
- Token expiry enforced server-side
- Account lockout after repeated failed login attempts
- Two-Factor Authentication **(2FA)** supported
- **CORS** configured to whitelist allowed origins only

---

## 🚀 Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) >= 7.0
- SQL Server 2019 or later
- Git

### Installation

```bash
# Clone the repository
git clone <repository-url>
cd backend
```

### Configuration

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=HRManagement;Trusted_Connection=True;"
  },
  "JwtSettings": {
    "SecretKey": "YOUR_SECRET_KEY",
    "Issuer": "HRManagementAPI",
    "Audience": "HRManagementClient",
    "ExpiryMinutes": 60
  },
  "AllowedOrigins": ["http://localhost:3000"]
}
```

### Run Migrations

```bash
# Apply all pending migrations to the database
dotnet ef database update
```

### Start the API

```bash
dotnet restore
dotnet run
```

API available at: `https://localhost:5001`  
Swagger UI: `https://localhost:5001/swagger`

---

## 📡 API Endpoints Overview

| Module | Base Route | Methods |
|--------|-----------|---------|
| Authentication | `/api/auth` | Login, Logout, Refresh Token, Forgot Password |
| Employees | `/api/employees` | CRUD, lock/unlock, change department/position |
| Attendance | `/api/timekeeping` | Check-in/out, view, edit, delete |
| Leave | `/api/timeoff` | Submit, approve, reject, cancel |
| Payroll | `/api/salary` | Create, view, update, delete payslips |
| Contracts | `/api/contracts` | CRUD labor contracts |
| Departments | `/api/departments` | CRUD departments |
| Positions | `/api/roles` | CRUD positions/roles |
| Rewards | `/api/rewards` | CRUD reward records |
| Discipline | `/api/discipline` | CRUD disciplinary records |
| Notifications | `/api/notifications` | Send, list, mark as read |
| Events | `/api/events` | Create and manage internal events |
| Working Rules | `/api/workingrules` | CRUD working regulations |
| Permissions | `/api/permissions` | Manage role-based access |
| Reports | `/api/reports` | Aggregated statistics and dashboards |

---

## 🔄 Development Workflow

The project follows an **Agile** approach with short sprints. Source control is managed via Git with the following branching strategy:

```
main           ← stable production branch
└── feature/   ← one branch per feature
└── fix/       ← bug fix branches
    └── Pull Request → reviewed → merged into main
```

Testing includes:
- **Functional Testing** — each endpoint verified against requirements
- **Security Testing** — auth bypass, injection, and token validation checks
- **UI Compatibility Testing** — API responses validated against frontend expectations

---

## 📈 Strengths

- ✅ Clean architecture with separation of concerns (Controller → Service → Repository)
- ✅ Secure authentication using JWT + Identity
- ✅ Scalable database design with 27+ normalized tables
- ✅ Full CRUD coverage for all HR modules
- ✅ Role-based access enforced at the API level
- ✅ Supports file attachments via `SysFiles` table

---

## 🔭 Future Development

- 🤖 **AI / OCR Integration**: Auto-extract employee data from uploaded documents
- ⛓️ **Blockchain**: Smart contracts for tamper-proof labor agreements
- 📧 **Email / SMS Notifications**: Real-time alerts via external messaging services
- 🔗 **ERP Integration**: Connect with third-party payroll or accounting systems
- 📊 **Advanced Reporting**: Custom report builder and data export (Excel, PDF)

---

## 📚 References

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [SQL Server Documentation](https://docs.microsoft.com/en-us/sql/sql-server/)
- [JWT Authentication in .NET](https://jwt.io/)
- [ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [GitHub Documentation](https://docs.github.com/en/github)

---
