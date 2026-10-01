# Roadmap to 15-10-2026

**Fresh start.** The old code, scripts and process files are gone. Only `docs/` is kept. Everything below is built from the docs, following the spec (`DECISIONS.md`).

Today: 01-10-2026. 14 days left. Two developers in parallel.

> **Realistic target (my estimate): v0.3.** By 15-10 the team can reach Auth, Masters, Attendance and a basic Payroll run on the server-PC setup. The full spec (reports, statutory exports, licensing, installer, hardening) is **not** reachable in 14 days. Atul confirms the cut or moves the date. See "Not in this release".

## Phase status (update when a phase is finished)
| Phase | Dates | Status | Completed on |
|---|---|---|---|
| 0. Setup | 01-10 to 02-10 | In progress | |
| 1. Database, DAL and Auth | 03-10 to 06-10 | Not started | |
| 2. Masters and Attendance | 07-10 to 09-10 | Not started | |
| 3. Payroll | 10-10 to 12-10 | Not started | |
| 4. Test and release | 13-10 to 15-10 | Not started | |

Status values: Not started, In progress, Done. When a phase is Done: fill the date, tick its boxes, and tag Git (`phase-0-done`, `phase-1-done`, ...).

## Topology (from `DECISIONS.md` D1)
1 server PC (SQL Server Express) and 5 to 7 client PCs on the LAN, TLS on. Developers use LocalDB on their own machines.

## Phase 0: Setup (01-10 to 02-10)
**Both**
- [ ] Atul confirms the remaining "to confirm" rows in `DECISIONS.md` (D5, D9, Operator payroll and Career rights, pay-type reading)
- [ ] `docs/` committed. Default branch is `master`
- [ ] `.gitignore`, `.gitattributes`, `.editorconfig` added

**Dev A**
- [ ] Solution with 8 projects: Common, Models, DAL, BLL, Reports, UI, Installer, Tests (`Revolution.Hrms.*`), VB.NET, .NET 10, `Option Strict On`
- [ ] SQL Server Express installed on the server PC, TCP/IP on a fixed non-default port, `sa` disabled
- [ ] LocalDB on both developer machines
- [ ] Migration runner (numbered `0001_...` scripts, `cfg.SchemaVersion`) and `Setup-Db.ps1`
- [ ] Config (`appsettings.json`, not committed, plus `appsettings.example.json`) and logger in Common

**Dev B**
- [ ] Test project with a test framework chosen and recorded
- [ ] `frmMain` MDI shell: menu, toolbar, status bar (user, company, FY, server, role), theme from `DESIGN.md`
- [ ] Base form with hotkeys (F2, F3, F5, Ctrl+S, Esc, Enter-to-next)

## Phase 1: Database, DAL and Auth (03-10 to 06-10)
**Dev A**
- [ ] Script `0001_init.sql`: schemas `sec mst att pay stat car aud cfg`, `cfg.SchemaVersion`
- [ ] `sec` tables (User, Role, Permission, RolePermission, UserCompany) and `mst` tables (Company, Factory, Department, Designation, Employee, Nominee)
- [ ] Row-level security policy and `SESSION_CONTEXT('CompanyID')`
- [ ] App SQL login with `EXECUTE` only
- [ ] Seed scripts (roles, permissions, first Associate, rates in `stat.RateTable`) that run when the database is created
- [ ] Stored procedures: auth, lockout, change password, company, department, designation, employee
- [ ] DAL: connection manager (TLS, session context set after connect), base procedure-call helper
- [ ] Crypto helper: PBKDF2 (100,000+ iterations, salt), AES-256 for later use
- [ ] `AuthService` (BLL), permission checks, audit log

**Dev B**
- [ ] Models and DTOs for auth, company, employee
- [ ] `frmLogin`, `frmChangePassword` (forced on first login), `frmCompanySelect`
- [ ] Role-based menu builder
- [ ] Idle-timeout lock (10 minutes, Ctrl+L, `frmUnlock`)
- [ ] Tests: lockout after 5 failed logins, hash verify, permissions

Dev B codes against agreed BLL interfaces until Dev A's procedures are merged.

## Phase 2: Masters and Attendance (07-10 to 09-10)
**Dev A**
- [ ] Company CRUD (Associate and Company admin rules)
- [ ] Employee CRUD with pay type (Daily wage or Monthly salary), search and filter grid, auto employee code
- [ ] User management: Associate creates Company and Operator users and assigns companies

**Dev B**
- [ ] Department and designation screens, holiday calendar, financial year setup
- [ ] Script for attendance tables and procedures
- [ ] Daily attendance and monthly summary entry, half day logic in 0.5 steps. Operators add and edit until the month-close lock
- [ ] `AttendanceEngine` with tests (half day, zero overtime)
- [ ] Month-close lock and reopen with a reason (audited)

## Phase 3: Payroll (10-10 to 12-10)
**Dev A**
- [ ] Salary structure per employee
- [ ] `SalaryEngine`: Daily wage = daily rate x days paid, Monthly salary = structure x days paid / days in month
- [ ] `EPFEngine` and `ESIEngine` (calculation only, rates read from `stat.RateTable`, ceilings and rounding per `BUSINESS_RULES.md`)
- [ ] Payroll run: Draft, Processed, Approved, Locked. Negative pay blocked. Locked runs change only by reversal

**Dev B**
- [ ] `frmPayrollProcess`, `frmPayrollApprove`
- [ ] Overtime hours at 2x ordinary rate
- [ ] Advance and arrears entry
- [ ] Payroll view and export of the run (full reports come later)
- [ ] Engine tests: ceiling boundary, half day, zero overtime, mid-month join and exit

**Feature freeze at end of 12-10.**

## Phase 4: Test and release (13-10 to 15-10)
- [ ] Install on the server PC and at least two client PCs. Both developers log in at the same time
- [ ] Cross-company leak test: an Operator sees zero rows of an unassigned company on every screen
- [ ] Payroll checked against a manual calculation
- [ ] Backup and restore tested once on the server PC
- [ ] Blocker and Major bugs fixed. No new features
- [ ] Short install guide for server and client mode
- [ ] Tag `v0.3.0`, update `CHANGELOG.md`
- [ ] **15-10-2026: delivery**

## Not in this release (spec items, later)
Crystal reports and password-protected payslip, ECR and ESI export files, challan, gratuity, bonus, career (promotion, increment, transfer), Factory and Division masters, minimum wage master, bulk attendance upload, shifts, encrypted sensitive IDs (Aadhaar, PAN, bank, UAN, ESI IP), encrypted and scheduled backups, installer (Inno Setup), licensing, penetration test, Operator and Company self-service extras.
These follow the spec release plan: v0.4 statutory, v0.5 reports, v1.0 installer, licensing and hardening.

## Daily routine
1. Pull the default branch at the start of the day, rebase your feature branch.
2. Small commits, push at the end of the day.
3. 10-minute sync: done, blocked, which shared files each person touches.

## Risks
| Risk | Mitigation |
|---|---|
| Spec architecture is much bigger than 14 days | Cut recorded above. Confirm with Atul on day 1 |
| Dev B waits on Dev A's database and procedures | Agree BLL interfaces and models in Phase 0. Dev B builds the shell and login against stubs |
| Both edit the same form or Designer file | One owner per module (`MODULES.md`) |
| Schema drift across developers and the server | Numbered scripts only, announced before merge |
| A tenant table without row-level security or company filter | One policy script per table and the leak test |
| Statutory rates wrong or unverified | Rates in `stat.RateTable`, values checked against a government calculator before release |
| Server PC network setup takes longer than planned | Do it on day 1 to 2, not day 13 |
| Scope creep | New ideas go to the "Not in this release" list |
