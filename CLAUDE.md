# CLAUDE.md: Revolution HRMS (Offline)

Offline HRMS for Indian (Haryana) labour law compliance. Windows desktop, no internet needed.
**Fresh start (01-10-2026):** there is no legacy code in this repo. Everything is built from `docs/`. The docs are the source of truth. Read the relevant one before writing code.
Delivery target **15-10-2026**, two developers (Dev A, Dev B). Plan: `docs/ROADMAP.md`. Decisions: `docs/DECISIONS.md`.

## Stack and topology
- VB.NET, Windows Forms (MDI shell), .NET 10 (`net10.0-windows`)
- SQL Server **Express on one server PC**, 5 to 7 client PCs on the LAN, TLS forced. Developers use LocalDB locally.
- ADO.NET (`Microsoft.Data.SqlClient`) calling **stored procedures only**. No ORM, no inline SQL.
- Crystal Reports comes later (v0.5), not yet.

## Solution layout
`Revolution.Hrms.sln` with 8 projects: `Common` (config, logger, crypto, formatters), `Models`, `DAL`, `BLL`, `Reports`, `UI`, `Installer`, `Tests`.
- Flow: UI calls BLL, BLL calls DAL, DAL calls stored procedures.
- UI: display and input validation only. BLL: rules, calculations, permission checks, no SQL, no forms. DAL: executes procedures, no business decisions. DB: stores and enforces integrity.
- No `MessageBox` in BLL or DAL. Each module is its own form or UserControl so each developer owns separate files.

## Database (`docs/DATABASE.md`)
- Single database `HRMS_Data`. Schemas: `sec mst att pay stat car aud cfg`.
- Keys `<Table>ID` (INT identity). Every table has `CompanyID`, `CreatedBy/On`, `ModifiedBy/On`, `IsActive` (soft delete). Money `DECIMAL(12,2)`.
- Procedures named `schema.usp_<Entity>_<Action>`. No `SELECT *`. No dynamic SQL unless parameterized with `sp_executesql`. `SET XACT_ABORT ON`, short transactions.
- Tenant isolation: set `SESSION_CONTEXT('CompanyID')` right after connect. A row-level security policy filters every tenant table.
- The app SQL login has `EXECUTE` only, never `db_owner`. `sa` is disabled.
- Migrations: numbered `0001_init.sql`, idempotent (`IF NOT EXISTS`), version in `cfg.SchemaVersion`. Never edit a merged script: add a new one. Back up before an upgrade.
- Roles, permissions, the first Associate user and the statutory rates are seeded by seed scripts when the database is created. Rate values must be verified before release.
- Never share a `.mdf` through Git. `appsettings.json` is not committed: copy `appsettings.example.json`.

## Users and access (`docs/ACCESS_CONTROL.md`, `DECISIONS.md` D3)
- **Associate:** assigned companies only. **Company:** own company only. **Operator:** data entry for companies assigned to them (employees, attendance, leave, advances). No Employee login, no Developer level, no Database Console.
- Operators add attendance and edit it until the month-close lock. After the lock only an Associate or Company admin can reopen it, with a reason.
- Operators cannot process or approve payroll, change rates or statutory setup, manage users or take backups.
- Permission checks go through the BLL `AuthService`. The UI hides or disables, the BLL enforces, the database restricts.
- Lock after 5 failed logins and after 10 minutes idle. Force a password change on first login.

## Coding standards (`docs/CODING_STANDARDS.md`)
- `Option Strict On`, `Option Explicit On`, `Option Infer On` everywhere.
- `Decimal` for all money, never `Double`. Round with `Math.Round(x, 2, MidpointRounding.AwayFromZero)`.
- Display dates `dd-MM-yyyy`, money as `₹ 1,23,456.00`. Financial year is 1 April to 31 March. Set culture explicitly.
- No magic numbers. Statutory percentages and ceilings (EPF, ESI, bonus, gratuity) come from `stat.RateTable`, effective-dated, never inline. Other constants are `UPPER_SNAKE`.
- Naming: `PascalCase` types and methods, `camelCase` locals, `_camelCase` private fields, `I` prefix for interfaces. Prefixes `frm`, `uc`, `txt`, `btn`, `dgv`, `cbo`.
- One class per file. Methods up to 50 lines, files up to 500 lines.
- `Using` blocks for every `IDisposable`. Catch specific exceptions, log, show a friendly message. Global handler in `Program.vb`. Never swallow errors.
- Validate input in the UI and again in the BLL. Confirm before delete. Soft delete only. Keyboard first (hotkeys in `docs/DESIGN.md`).
- Never log or hard-code passwords, Aadhaar, PAN, bank numbers, keys or connection strings. Mask sensitive fields as `XXXX-XXXX-1234`.
- No commented-out code. Comments explain why, not what.

## Business rules (`docs/BUSINESS_RULES.md`)
- Employees have a pay type. **Daily wage:** earned = daily rate x days paid. **Monthly salary:** earned = structure x days paid / days in month.
- Days paid = present + weekly off + holidays + paid leave + 0.5 x half days. In legacy data, "Worked" already includes holidays and leave: do not add them twice.
- Overtime = 2 x ordinary rate, ordinary rate = (Basic + DA) / 26 / shift hours.
- Payroll runs only after attendance is locked. States: Draft, Processed, Approved, Locked. Locked payroll changes only by reversal. Negative net pay is flagged and blocked.
- Calculation engines are pure functions with unit tests. Required cases: ceiling boundary, half day, zero overtime, mid-month join and exit.
- Do not guess open points: leave accrual rules and statutory rate values need a person to verify.

## Git workflow (`docs/CONTRIBUTING.md`)
- Never push to the default branch. Use `feature/<module>-<short-name>` or `fix/<short-name>` and a pull request reviewed by the other developer. The default branch is `master`.
- Commit messages: `<module>: <what>`, for example `payroll: add salary engine`.
- Only the module owner edits that module's forms and `*.Designer.vb` / `*.resx`. On a Designer conflict take one side and redo the UI change in Visual Studio. Never hand-merge Designer files.
- Tell the other developer before editing shared files: schema and migrations, BLL interfaces, `frmMain`, `Program`, shared helpers.
- Do not commit `bin/`, `obj/`, `.vs/`, `appsettings.json` or database files.

## Ownership
- **Dev A:** Common, DAL, database scripts, auth BLL, company, employee, users, salary and statutory engines, payroll process.
- **Dev B:** UI shell and theme, login forms, role menu, attendance, leave, overtime, advances, payroll screens, later reports.

## Before you finish a task
- It builds with no errors and follows the standards above.
- Add or update tests. Test manually with the sample data in `docs/TESTING.md`.
- Check company filtering and role permissions on every new screen.
- Update the affected doc in the same PR, tick the phase box in `docs/ROADMAP.md`, add a line to `docs/CHANGELOG.md`.
- Anything not in the current roadmap goes on the "Not in this release" list, not into this release.
