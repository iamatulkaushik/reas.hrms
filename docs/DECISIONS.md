# Decisions and Conflicts

Created 30-09-2026 when the supplied spec (`PRD.md`, `DESIGN.md`, `BUSINESS_RULES.md`, `TASKS.md`) was merged into these docs.
Owner of each decision: Atul with Dev A and Dev B.

**Rule adopted 01-10-2026: follow the spec.** Where the spec and the repo disagree, the spec wins and the repo is changed. Two changes to the spec: the **Employee** user type is replaced by **Operator** (D3), and deployment is **1 server PC with 5 to 7 connected PCs** (D1).

**Status:** D1 to D14 filled in below. D3 and D1 are confirmed by Atul. Rows marked *to confirm* are my reading of the spec or of Atul's answers.

## 1. Decisions
| # | Topic | Decision |
|---|---|---|
| D1 | Database host | **SQL Server Express on one server PC. 5 to 7 client PCs connect over the LAN with TLS.** LocalDB is used only on developer machines. See 2.1 |
| D2 | Data access | ADO.NET with **stored procedures only**. The app login has `EXECUTE` only, no table access. No `SELECT *`. No ORM: a small helper in the DAL calls the procedures. Decided for the fresh start |
| D3 | User types | **Associate, Company, Operator.** Operator replaces Employee. See 2.2 |
| D4 | Tenant isolation | `SESSION_CONTEXT('CompanyID')` plus row-level security on every tenant table, set right after connect |
| D5 | Solution layout | Spec's 8 projects: Common, Models, DAL, BLL, Reports, UI, Installer, Tests. The `Revolution.Hrms.` name prefix can stay. *To confirm* |
| D6 | Schemas and names | Spec: schemas `sec mst att pay stat car aud cfg`, keys `<Table>ID`, audit columns `CreatedBy/On`, `ModifiedBy/On`, money `DECIMAL(12,2)` |
| D7 | Migration naming | Spec: `0001_init.sql`, 4 digits, idempotent, version in `cfg.SchemaVersion`. Fresh start: all scripts are written from scratch under D6 |
| D8 | Reports | Crystal Reports, password-protected payslip PDF. Pin the runtime version and check .NET 10 support before relying on it |
| D9 | Encryption | SQL Express has no TDE, so use **BitLocker plus field encryption**. AES-256 in the business layer for Aadhaar, PAN, bank, UAN, ESI IP, with key export and import across the client PCs (spec Phase 2). Encrypted backups. *To confirm* |
| D10 | Attendance and wages | Spec daily status (P/A/H/L/WO/HD) plus monthly summary entry. Employees have a pay type. See 2.3 |
| D11 | Leave data | Spec `att.Leave`. The repo's `LeaveBalances` is kept only if balances are needed |
| D12 | Backup plan | Spec: nightly full, differential every 4 hours, keep 30 daily and 12 monthly. Task Scheduler and `sqlcmd` (Express has no SQL Agent) |
| D13 | Code limits | Spec: methods up to 50 lines, files up to 500 lines |
| D14 | Code reuse | **Resolved 01-10-2026: fresh start.** The old code, scripts and process files are erased. Only the docs are kept. Legacy behaviour is known only from the docs |

## 2. Reasoning

### 2.1 D1: Database host
- Atul's answer: 1 server PC, 5 to 7 connected PCs. LocalDB cannot serve other PCs, so SQL Express on the server is required. This reverses my earlier "keep LocalDB" recommendation.
- Needed for it: SQL Server TCP/IP on a fixed non-default port, Force Encryption (TLS), firewall allow-list, `sa` disabled, SQL Browser off, server and client install modes.
- SQL Express limits: 10 GB database, about 1 GB RAM. Fine for 5 to 7 users. The PRD asks for 10 simultaneous users.
- Standard edition is only needed for TDE, which is not planned (see D9).

### 2.2 D3: User types
| Type | Who | Access |
|---|---|---|
| Associate | Consultant managing many companies | Assigned companies only (`sec.UserCompany`) |
| Company | Employer or HR admin | Own company only |
| Operator | Data-entry staff. **Replaces Employee** | Operates records for the companies assigned to them: employees, attendance, leave, advances and similar entry work |

| Module | Associate | Company | Operator |
|---|---|---|---|
| Company master | Full | View own | - |
| Employee master | Full | Full | Operate (assigned companies) |
| Attendance | Full | Full | Add and modify until month-close lock (assigned companies) |
| Leave, overtime, advance, arrears entry | Full | Full | Operate (assigned companies) |
| Payroll process and approve | Full | Full | - *to confirm* |
| Payslip | All | All | View, print (assigned companies) |
| Statutory setup and rates | Full | Full | - |
| Career | Full | Full | - *to confirm* |
| Reports | All | Own | Operational reports for assigned companies |
| Users | Full (creates Company and Operator users) | Own company (creates its Operators) | - |
| Backup, restore, audit | Full | Audit own | - |

- Who assigns companies to an Operator: Associate for any of their companies, Company admin for their own company.
- Every query is still filtered by company (D4), so an Operator never sees an unassigned company.
- **Consequences of replacing Employee:** no employee login and no self-service payslip or attendance view.
- **Consequences of following the spec:** the repo's Developer level and Database Console are not in the spec. They are dropped from the user types. Use SQL tools directly for support. Rate changes need Associate rights and are audited. *To confirm*
- Other spec rules still apply: lock after 5 failed logins, 10-minute idle lock, forced password change on first login, unlocking attendance needs a Company admin and a reason.

### 2.3 D10: Attendance and wages
Atul's answer: daily-wage employees are day based, salaried employees use day count.
- Each employee has a **pay type**: Daily wage or Monthly salary.
- **Daily wage:** earned = daily rate x days paid.
- **Monthly salary:** earned = monthly structure x days paid / days in month (`BUSINESS_RULES.md` s7, step 4).
- Days paid = present + weekly off + holidays + paid leave + 0.5 x half days.
- **Please confirm:** I read this as the pay calculation. If you meant that attendance is entered per day for daily-wage staff and as a monthly day count for salaried staff, the entry screens change, the payroll maths does not.
- Month-close lock applies to both, with a reason to reopen.

## 3. Open items and answers
| Item | Status |
|---|---|
| SQL Express or Standard | **SQL Express** (D1). LocalDB on developer PCs only |
| Does "Worked" in the legacy app include holidays and leave? | **Yes** (Atul). So days paid must not add holidays and leave on top of "Worked" when migrating legacy data. *Please confirm that reading* |
| Leave accrual rules against the Factories Act and Haryana rules | Answered "Yes". Still needs a person to check the rules before the engine is built. *Please say if you meant something else* |
| Statutory rate values and effective dates | Seeded by the seed scripts when the database is created (Atul, 01-10-2026). The values still need verifying against a government calculator before release |
| Always Encrypted or AES-256 | AES-256 in the business layer (D9) |
| Crystal runtime version | Not chosen. Pin it and test on .NET 10 |
| Reuse legacy code or rewrite | Open (D14) |

## 4. Schedule risk
The spec has 13 phases. `ROADMAP.md` plans 4 phases in 14 days for two developers. Following the spec for architecture (server database, stored procedures, row-level security, 8 projects, Crystal, encryption) is much more work than the roadmap assumed. Regenerating the migrations and the Data layer alone takes days.
Spec release order (`PRD.md` s11): v0.1 Auth and masters, v0.2 Attendance, v0.3 Payroll, v0.4 ESI/EPF/gratuity, v0.5 Reports, v1.0 Installer, licensing and hardening.
**My estimate:** by 15-10-2026 the team can realistically reach the v0.1 to v0.3 range, not v1.0. Atul must decide the cut: deliver fewer modules on 15-10, or move the date. Record it in `ROADMAP.md`.

## 5. Docs to update once confirmed
`README.md`, `ARCHITECTURE.md`, `DATABASE.md`, `ACCESS_CONTROL.md`, `MODULES.md`, `ROADMAP.md`, `DEPLOYMENT.md`, `TESTING.md`, `CODING_STANDARDS.md`, `CONTRIBUTING.md` (migration naming), `CHANGELOG.md`, and `CLAUDE.md` (its decisions section is now out of date).
