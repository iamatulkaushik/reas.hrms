> **Fresh start 01-10-2026.** All old code, scripts and tests are erased. Any note below that says work exists is void: tick nothing until it is rebuilt.
> The schedule for 15-10-2026 is in `ROADMAP.md`; this file is the wider spec scope.
> `legacy-gap.md` is referenced below but was never supplied and is not in the repo.

# Tasks — Revolution HRMS (Offline)

Legend: `[ ]` todo, `[~]` in progress, `[x]` done

## Phase 0: Setup
- [ ] Create GitHub repo, branches
- [ ] Create solution `HRMS.sln` with 8 projects
- [ ] Install SQL Server Express/Standard on dev
- [ ] Install Crystal Reports runtime + VS add-in
- [ ] Configure `.gitignore`, `.editorconfig`
- [ ] Set `Option Strict On` project-wide
- [ ] Setup logging library
- [ ] Setup unit test project

## Phase 1: Database Foundation
- [ ] Stored procs: auth, company, factory, dept, desig, employee, nominee (0008-0011)
- [ ] Run scripts 0001-0007 on local SQL Server
- [ ] Run `HRMS.SmokeTest --db` (tests scripts 0008-0012 end to end)
- [ ] Script 0012: DAL support (session with no company, ServerNow, bootstrap check, sensitive-on-request)
- [ ] Make sure password hashes never reach the audit log
- [ ] Create `HRMS_Data` database (single DB; master merged)
- [ ] Create schemas: sec, mst, att, pay, stat, car, aud, cfg
- [ ] Script `0001_init.sql`
- [ ] Create `cfg.SchemaVersion`
- [ ] Create sec tables (User, Role, Permission, UserCompany)
- [ ] Create mst tables (Company, Factory, Employee, Nominee, Dept, Desig)
- [ ] Create att tables
- [ ] Create pay tables
- [ ] Create stat tables + seed `RateTable`
- [ ] Create car tables
- [ ] Create `aud.AuditLog`
- [ ] Add FKs, unique constraints, indexes
- [ ] Implement RLS policy + `SESSION_CONTEXT`
- [ ] Create app SQL login, EXECUTE-only role
- [ ] Audit triggers on all tenant tables
- [ ] Seed roles, permissions (default admin via installer)

## Phase 2: Common & DAL
- [ ] `HRMS.Common`: config, DPAPI, logger
- [ ] Crypto helper: PBKDF2, AES-256 (+ key store, LAN key export/import)
- [ ] Connection manager (TLS via connection string, session context)
- [ ] Base DAL class (proc execute helpers)
- [ ] Models / DTOs (auth, company, factory, employee, nominee done; rest with their phases)
- [ ] Indian currency formatter
- [ ] Date helpers, FY helper

## Phase 3: Authentication (v0.1)
- [ ] Stored procs: login, lockout, change pwd
- [ ] BLL: `AuthService`
- [ ] `frmLogin`
- [ ] `frmChangePassword`
- [ ] `frmCompanySelect` (Associate)
- [ ] Idle-timeout lock (`frmUnlock`, 10 min, Ctrl+L)
- [ ] Role-based menu builder (`MenuCatalog`)
- [ ] Tests: lockout, hash verify, permissions (`SmokeTest --db`)
- [ ] First-run setup wizard (`frmSetup`: DB connection, key create/import/export, first admin)
- [ ] Main MDI shell (`frmMain`: menu, status bar, switch company, log out)
- [ ] Visual check of all Phase 3 screens on a real PC

## Phase 4: Masters (v0.1)
- [ ] Company CRUD (4 tabs: basic, contact, registrations, bank)
- [ ] Factory CRUD (site; optional Division)
- [ ] Department, Designation
- [ ] Division master (legacy Division)
- [ ] Reference masters: State, District, Bank (seeded; lists in dropdowns)
- [ ] Employee detail: family, address, education, EPF/ESIC/labour, payment and employment type
- [ ] Employee CRUD with 6 tabs
- [ ] Encrypt/mask Aadhaar, PAN, bank, UAN, ESI IP (BLL)
- [ ] Nominee grid
- [ ] Photo / document attach
- [ ] Employee code generator (proc: E00001...)
- [ ] Holiday calendar
- [ ] Haryana minimum wage master
- [ ] Financial year setup
- [ ] Search / filter grid (employees: text, factory, department, active, paging)

- [ ] Visual check of all Phase 4 screens on a real PC
- [ ] Legacy review: see `legacy-gap.md` (defects, code values, migration)

## Phase 5: Attendance (v0.2) - monthly mode first, optional parts in 5b
- [ ] Monthly attendance summary entry (legacy mode: worked, holidays, CL, EL, SL, comp leave, OT hours) - primary
- [ ] Full day / half day logic (0.5 steps enforced in engine, screen and database)
- [ ] Overtime entry (OT hours column; pay is calculated in Phase 6)
- [ ] Bulk import from .xlsx or .csv (no Excel needed) + CSV export that doubles as a template
- [ ] Month-close lock / reopen with a written reason (audited; reopen blocked once payroll is approved)
- [ ] `AttendanceEngine` + importer + file readers + tests
- [ ] Site (factory) can differ per month per employee
- [ ] Visual check of the monthly attendance screen on a real PC
- [x] Decided 01-10-2026: yes, legacy "Worked" already includes holidays and leave. In the new app "Worked" means days actually present and paid days = sum of all columns
- [ ] 5b: Daily attendance entry (optional mode)
- [ ] 5b: Leave register + balance (accrual rules need checking against the Factories Act / Haryana rules)
- [ ] 5b: Shift master (only needed with daily attendance)
- [ ] Muster roll report (Phase 9, Crystal)

## Phase 6: Payroll (v0.3)
- [ ] Designation pay template (legacy: pay lives on the designation) + per-employee override
- [ ] Pay components as legacy: Basic, DA, HRA, Conveyance, Special, Medical, Lunch, CCA, Travel, Other1-2, Arrears1-4, Daily wage
- [ ] Deductions as legacy: EPF, ESIC, Labour welfare (employee + employer), VPF, TDS, advances, loan
- [ ] Salary structure form
- [ ] Advance / loan module
- [ ] Arrears and bonus entry
- [ ] `SalaryEngine` (prorata, earnings)
- [ ] Overtime calc (2× rate)
- [ ] `frmPayrollProcess`
- [ ] Payroll state machine
- [ ] `frmPayrollApprove`
- [ ] Payroll lock
- [ ] Negative pay check
- [ ] Wage register report
- [ ] Payslip report
- [ ] Tests: boundary, mid-month join/exit

## Phase 7: Statutory (v0.4)
- [ ] Rate table admin UI
- [ ] `EPFEngine` + tests
- [ ] `ESIEngine` + tests (rounding, period)
- [ ] EPF ECR TXT export
- [ ] ESI contribution export
- [ ] Challan summary report
- [ ] `GratuityEngine` + tests
- [ ] Gratuity form + statement
- [ ] `BonusEngine` + bonus report
- [ ] Validate against govt calculators

## Phase 8: Career (v0.4)
- [ ] Promotion entry
- [ ] Increment entry (auto salary update)
- [ ] Transfer entry
- [ ] Career history view
- [ ] Order letter print

## Phase 9: Reports (v0.5)
- [ ] Crystal viewer host form
- [ ] Report selector
- [ ] Common header/footer template
- [ ] Payslip PDF with password
- [ ] Overtime register
- [ ] Employee master listing
- [ ] Audit log report
- [ ] Export engine: PDF, XLS, TXT
- [ ] Temp file cleanup
- [ ] Verify all against statutory forms

## Phase 10: Admin (v1.0)
- [ ] User management
- [ ] Role / permission editor
- [ ] Audit log viewer
- [ ] Backup UI (encrypted `.bak`)
- [ ] Scheduled backup (Task Scheduler + sqlcmd)
- [ ] Restore wizard
- [ ] Settings form

## Phase 11: Licensing & Hardening (v1.0)
- [ ] Machine fingerprint
- [ ] RSA-signed license file
- [ ] Offline activation flow
- [ ] Company/employee limit checks
- [ ] Force TLS on SQL connection
- [ ] Disable `sa`, SQL Browser
- [ ] Firewall rule script
- [ ] BitLocker setup guide
- [ ] Security checklist run
- [ ] Penetration test (LAN)

## Phase 12: Installer & Release
- [ ] Inno Setup script (create `C:\ProgramData\RevolutionHRMS` with Modify rights for Users; app writes logs/keys/config there)
- [ ] Bundle .NET + Crystal runtime
- [ ] DB create/upgrade scripts
- [ ] Pre-upgrade backup
- [ ] Server install mode
- [ ] Client install mode
- [ ] Uninstall clean-up
- [ ] User manual (PDF)
- [ ] Admin guide (PDF)
- [ ] Changelog, tag `v1.0.0`

## Phase 13: QA
- [ ] Unit test pass (all engines)
- [ ] Payroll parallel run vs manual
- [ ] 10-user LAN concurrency test
- [ ] 1,000-employee payroll timing
- [ ] Backup / restore drill
- [ ] Cross-company leak test
- [ ] Upgrade test (v0.x → v1.0)

## Backlog (Post v1)
- [ ] Biometric device import
- [ ] Bank advice file (NEFT)
- [ ] Form 16 / TDS
- [ ] Contract labour module
- [ ] Multi-language labels
- [ ] Optional cloud sync

## Open Decisions
- [x] SQL Express (decided 01-10-2026)
- [ ] Always Encrypted vs BLL AES
- [x] Fresh start, no reuse (decided 01-10-2026)
- [ ] Crystal runtime version to pin
