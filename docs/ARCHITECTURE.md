# Architecture

Follows the spec (`DECISIONS.md` D1 to D9). Items marked **Later** are not in the 15-10-2026 release (`ROADMAP.md`).

## Topology
Three-tier LAN application: WinForms clients, business class libraries, SQL Server Express on a server PC.
```
Client PC 1 .. 7 (WinForms) --LAN, TLS, fixed port--> Server PC
                                                      SQL Server Express: HRMS_Data
                                                      Backup folder -> encrypted backup (USB / NAS)
```
Developers use LocalDB on their own PC. Only the connection string differs.

## Solution layout
```
Revolution.Hrms.sln
├── Revolution.Hrms.Common     config, logger, crypto, formatters (Indian currency, dates, financial year)
├── Revolution.Hrms.Models     DTOs and enums
├── Revolution.Hrms.DAL        connection manager, procedure-call helper, one class per entity
├── Revolution.Hrms.BLL        services, calculation engines, permission checks
├── Revolution.Hrms.Reports    Crystal reports (Later)
├── Revolution.Hrms.UI         WinForms: forms, UserControls, Program
├── Revolution.Hrms.Installer  Inno Setup script (Later)
└── Revolution.Hrms.Tests      unit tests for BLL and engines
```

## Layers
| Layer | Does | Never does |
|---|---|---|
| UI | Display, input validation | Touch the database, calculate salary |
| BLL | Rules, permission checks, EPF, ESI and overtime math | Build SQL, show forms |
| DAL | Execute stored procedures | Business decisions |
| DB | Store, enforce integrity, row-level security | UI logic |

Flow: user action, UI, BLL (validate, calculate), DAL (stored procedure), SQL Server, back up the same path.
- References: UI to BLL, Models, Common. BLL to DAL, Models, Common. DAL to Models, Common. Models and Common reference nothing in the solution.
- No SQL outside the database scripts. No `MessageBox` in BLL or DAL.

## UI structure
```
Revolution.Hrms.UI
├── Forms/      frmMain (MDI shell), frmLogin, frmChangePassword, frmCompanySelect, frmUnlock
├── Modules/    Masters, Attendance, Payroll, Statutory, Career, Reports, Admin (forms per DESIGN.md s2)
├── Shared/     base form (hotkeys), base controls, theme, helpers
└── My Project/ VB application framework files
```
Each module is a separate form or UserControl so each developer owns separate files.

## Cross-cutting
| Concern | Approach |
|---|---|
| Configuration | `appsettings.json` next to the exe. Connection string DPAPI-encrypted (Later) |
| Logging | Rolling file in `C:\ProgramData\RevolutionHRMS\Logs`. No sensitive data in logs |
| Error handling | Global handler in `Program.vb` plus try/catch at BLL boundaries. The user sees a friendly message with a log ID |
| Auth | Associate, Company, Operator through `AuthService` in BLL (`ACCESS_CONTROL.md`) |
| Dates and money | `Date` and `Decimal`. Never `Double` for money |

## Security architecture
| Area | Spec | Release |
|---|---|---|
| Passwords | PBKDF2, 100k+ iterations, salt | 15-10 |
| Lockout | 5 failed attempts | 15-10 |
| Idle timeout | 10 minutes, Ctrl+L, `frmUnlock` | 15-10 |
| First login | Force password change (`MustChangePwd`) | 15-10 |
| Authorization | Role, permission and menu mapping in the database. UI hides, BLL enforces, DB restricts | 15-10 |
| SQL login | One app role, `EXECUTE` on procedures only, no table access | 15-10 |
| Tenant isolation | `SESSION_CONTEXT('CompanyID')` plus row-level security | 15-10 |
| Connection | Force Encryption (TLS) | 15-10 |
| Host | Non-default port, LAN only, firewall allow-list, SQL Browser off, `sa` disabled, BitLocker on the server | 15-10 |
| Database files | TDE is not available on Express. BitLocker plus field encryption instead | Later |
| Aadhaar, PAN, bank, UAN, ESI IP | AES-256 in BLL, key export and import across client PCs | Later |
| Connection string | DPAPI-encrypted config | Later |
| Backups | Encrypted `.bak` | Later |
| Payslip PDF | Password protected, employee-specific | Later |
| Audit | `aud.AuditLog` for sensitive actions. Triggers on all tenant tables write old and new values. App role can only insert | Log table 15-10, triggers Later |
| Keys | Certificates in the Windows store, DPAPI machine scope, backup key stored apart from the server, no keys in source or config | Later |

## Calculation engines (pure functions, unit-tested, in BLL)
| Engine | Responsibility | Rules in | Release |
|---|---|---|---|
| `AttendanceEngine` | Days paid, half day, overtime hours | `BUSINESS_RULES.md` s1, s2 | 15-10 |
| `SalaryEngine` | Earnings by pay type (Daily wage or Monthly salary), prorata, arrears | s7 | 15-10 |
| `EPFEngine` | Ceiling, EPS/EPF split, EDLI, admin | s3 | 15-10 (calculation only) |
| `ESIEngine` | Ceiling check, contribution period, round up | s4 | 15-10 (calculation only) |
| `GratuityEngine` | 15/26 formula, cap | s5 | Later |
| `BonusEngine` | Minimum and maximum bonus | s6 | Later |

All rates come from `stat.RateTable`. Never hard-code a percent or ceiling.

## Reporting (Later)
- Crystal `.rpt` files in the Reports project. Data from stored procedures only. Parameters: company, month, year, employee.
- Exports: PDF, XLS, TXT (custom writer for ECR and ESI files). Temp files deleted after export.
- Pin the Crystal runtime version and check it runs on .NET 10 before starting.

## Licensing (Later)
Machine fingerprint (CPU, disk, MAC hash). RSA-signed license file. Limits: companies, employees, expiry. Offline activation code flow.

## Technology choices
| Decision | Choice | Reason |
|---|---|---|
| UI | WinForms | Desktop, keyboard-first data entry |
| Database | SQL Server Express (server), LocalDB (developers) | Row-level security, LAN access, free |
| Access | ADO.NET and stored procedures | Speed, security, EXECUTE-only login |
| Reports | Crystal (Later) | Pixel-perfect statutory forms |
| Installer | Inno Setup (Later) | Simple, free |

## Limits
- SQL Express: 10 GB database cap, about 1 GB RAM. Use Standard beyond that.
- Express has no SQL Agent. Use Task Scheduler and `sqlcmd` for backups.
- TDE needs Standard or Enterprise.
- Scale target: 10 simultaneous LAN users, up to 50 companies and 10,000 employees.

## Shared files (change only through a PR reviewed by the other developer)
- Database scripts and procedures
- BLL interfaces and Models
- `frmMain`, `Program`, `Shared/*`
