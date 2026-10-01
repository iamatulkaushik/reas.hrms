# Database

**Engine: SQL Server Express on the server PC.** One database `HRMS_Data` holds all companies, separated by a `CompanyID` column. Developers use LocalDB (`(localdb)\MSSQLLocalDB`) on their own PC. See `DECISIONS.md` D1, D2, D4, D6, D7.

## Connection (in `appsettings.json`, not committed)
```
Server=<server-pc>,<port>;Database=HRMS_Data;User Id=<app login>;Password=<from config>;Encrypt=True;
```
Developer machine:
```
Server=(localdb)\MSSQLLocalDB;Database=HRMS_Data;Trusted_Connection=True;TrustServerCertificate=True;
```
NuGet: `Microsoft.Data.SqlClient`. No ORM. The DAL calls stored procedures only.

## Schemas
| Schema | Content |
|---|---|
| `sec` | Users, roles, permissions |
| `mst` | Company, factory, employee, department, designation |
| `att` | Attendance, leave, shift, overtime |
| `pay` | Salary structure, payroll, payslip |
| `stat` | ESI, EPF, gratuity, bonus, rate tables |
| `car` | Promotion, increment, transfer |
| `aud` | Audit logs |
| `cfg` | Settings, financial year, holidays, schema version |

## Conventions
- Primary key `<Table>ID`, INT identity.
- Every table has `CompanyID`, `CreatedBy`, `CreatedOn`, `ModifiedBy`, `ModifiedOn`, `IsActive` (soft delete).
- Money `DECIMAL(12,2)`. Foreign keys enforced. Check constraints on `Status` values.
- Rates are effective-dated (`FromDate`, `ToDate`).
- Table names singular PascalCase. Procedures `schema.usp_<Entity>_<Action>`.
- No `SELECT *`. No dynamic SQL unless parameterized through `sp_executesql`.
- Index `CompanyID`, foreign keys and date columns. Paginate lists over 500 rows. Avoid cursors. Short transactions with `SET XACT_ABORT ON`.

## Tenant isolation and access
- The DAL sets `SESSION_CONTEXT('CompanyID')` right after connect, from the user's companies.
- A row-level security policy filters every tenant table on `CompanyID`.
- Associate and Operator users get their companies from `sec.UserCompany`. A Company user has one.
- The app login has `EXECUTE` on the schemas only, never `db_owner`. `sa` is disabled.

## Tables (key columns)
Columns marked *(added)* are not in the original spec. They support decisions in `DECISIONS.md`.

| Table | Columns |
|---|---|
| `sec.User` | UserID, Username, PasswordHash, Salt, RoleID, UserType (Associate, Company, Operator), FailedCount, LockedUntil, MustChangePwd, IsActive |
| `sec.Role`, `sec.Permission`, `sec.RolePermission` | Role, permission code, mapping |
| `sec.UserCompany` | UserID, CompanyID, AssignedBy *(added)*, AssignedOn *(added)* |
| `mst.Company` | CompanyID, Name, Address, PAN, TAN, EsiCode, EpfCode, LIN, IsActive |
| `mst.Factory` | FactoryID, CompanyID, Name, LicenseNo, Address, Zone |
| `mst.Department`, `mst.Designation` | ID, CompanyID, Name |
| `mst.Employee` | EmployeeID, CompanyID, FactoryID, EmpCode, Name, FatherName, DOB, Gender, DOJ, DOL, DeptID, DesigID, SkillCategory, **PayType** *(added: Daily wage or Monthly salary)*, AadhaarEnc, PANEnc, BankAccEnc, IFSC, UANEnc, EsiIPEnc, Mobile, Address, IsActive |
| `mst.EmployeeNominee` | NomineeID, EmployeeID, Name, Relation, DOB, SharePct |
| `att.Attendance` | AttID, CompanyID, EmployeeID, AttDate, Status (P/A/H/L/WO/HD), ShiftID, InTime, OutTime, OTHours, IsLocked |
| `att.AttendanceMonth` *(added)* | AttMonthID, CompanyID, EmployeeID, Month, Year, Worked, Holidays, CL, EL, SL, CompOff, OTHours, IsLocked |
| `att.Leave` | LeaveID, EmployeeID, LeaveType, FromDate, ToDate, Days, Status |
| `pay.SalaryStructure` | StructID, EmployeeID, Basic, DA, HRA, Other, **DailyRate** *(added, daily-wage employees)*, EffectiveFrom, EffectiveTo |
| `pay.PayrollRun` | RunID, CompanyID, Month, Year, Status (Draft/Processed/Approved/Locked), ProcessedBy, ApprovedBy |
| `pay.PayrollDetail` | DetailID, RunID, EmployeeID, DaysPaid, OTHours, Basic, DA, HRA, OTAmount, Gross, EsiEmp, EsiEr, EpfEmp, EpsEr, EpfEr, PT, Advance, OtherDed, NetPay |
| `stat.RateTable` | RateID, Code, Value, FromDate, ToDate (codes in `BUSINESS_RULES.md` s8) |
| `stat.Gratuity` | GratuityID, EmployeeID, Years, LastWage, Amount, PaidOn |
| `car.Promotion`, `car.Increment`, `car.Transfer` | ID, EmployeeID, EffectiveDate, Old*, New*, OrderNo, Remarks |
| `aud.AuditLog` | AuditID, TableName, KeyValue, Action, OldValue, NewValue, UserID, Machine, LoggedOn |
| `cfg.SchemaVersion` | Version, AppliedOn |

Entities: Company 1-* Factory 1-* Employee. Employee 1-* Attendance, SalaryStructure, PayrollDetail, Promotion, Increment, Transfer. Employee 1-1 Gratuity on exit. User *-* Company through `sec.UserCompany`.

`Worked` in `att.AttendanceMonth` means days actually present. The legacy "Worked" already included holidays and leave, so legacy data must be split on import. Days paid = Worked + Holidays + CL + EL + SL + CompOff.

## Constraints
- Unique `(CompanyID, EmpCode)`, `(EmployeeID, AttDate)`, `(EmployeeID, Month, Year)` on `att.AttendanceMonth`, and `(CompanyID, Month, Year)` on payroll runs.
- Foreign keys on all relations.

## Working with the database as two developers
- **Each developer has their own LocalDB database.** Never share a `.mdf` through Git or a shared folder.
- The schema is shared only through **numbered scripts** in `Database/Scripts/` (`0001_init.sql`, `0002_...`). `Setup-Db.ps1` or the app applies pending scripts and records them in `cfg.SchemaVersion`.
- Scripts are idempotent (`IF NOT EXISTS`). Never edit a merged script: add a new one.
- Procedures live in numbered scripts too, so a procedure change is also a new script.
- Keep `Database/Seed/` with sample data scripts so both developers can rebuild the same test database.
- Back up the database automatically before an upgrade.

## Seed data
Reference data is seeded when the database is created (seed scripts in `Database/Seed/`, run by `Setup-Db.ps1` and the server setup). Sample data is for development only.
- Roles and permissions.
- One Associate user with `MustChangePwd` set (the first admin).
- Rates in `stat.RateTable` (EPF, ESI, bonus, gratuity cap) with effective dates. Values to be verified (`BUSINESS_RULES.md` s11).
- One sample company, departments and designations for development only.

## Protected data (Later)
- Aadhaar, PAN, bank account, UAN and ESI IP are encrypted (AES-256 in BLL). The columns are created with the employee table but stay empty until the feature ships.
- UI shows `XXXX-XXXX-1234`. Full value only with permission, with an audit entry.
- Never log passwords, Aadhaar, PAN or bank numbers.
