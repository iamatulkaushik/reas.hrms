# Revolution HRMS (Offline)

Offline Human Resource Management System for Indian (Haryana) labour law compliance. Windows desktop, no internet needed.

| Item | Value |
|---|---|
| UI | Windows Forms (MDI shell) |
| Language | VB.NET, .NET 10 (`net10.0-windows`) |
| Data | SQL Server Express on one server PC, 5 to 7 client PCs on the LAN. Developers use LocalDB (see `DATABASE.md`) |
| Users | 3 types: Associate, Company, Operator (see `ACCESS_CONTROL.md`) |
| Target release | **15-10-2026**: v0.3 (Auth, Masters, Attendance, Payroll). The rest of the spec follows (see `ROADMAP.md`) |
| Team | 2 developers in parallel (Dev A, Dev B) |
| Status | Fresh start on 01-10-2026. Only `docs/` exists until Phase 0 is done |

## Documents
| File | Purpose |
|---|---|
| `PRD.md` | Product requirements (spec) |
| `DESIGN.md` | UI, screens, table and report design (spec) |
| `BUSINESS_RULES.md` | Labour-law and payroll rules (spec) |
| `TASKS.md` | Full spec task list, 13 phases |
| `DECISIONS.md` | Where the spec and the plan differ, and what was decided |
| `ROADMAP.md` | Day-by-day plan to 15-10-2026 and what is left out |
| `ARCHITECTURE.md` | Solution layout, layers, security, topology |
| `DATABASE.md` | Schemas, tables, procedures, migrations |
| `ACCESS_CONTROL.md` | User types, company access, permissions, audit |
| `MODULES.md` | Feature scope and ownership per module |
| `CODING_STANDARDS.md` | VB.NET style and rules |
| `CONTRIBUTING.md` | Git workflow, branches, PR rules, WinForms conflict rules |
| `TESTING.md` | Test plan and release checklist |
| `DEPLOYMENT.md` | Server and client setup, build, backup, upgrades |
| `CHANGELOG.md` | Version history |
| `CLAUDE.md` (repo root) | Instructions for Claude Code |

## Getting started (after Phase 0)
1. Install Visual Studio 2022 or later with the **.NET desktop development** workload and the **.NET 10 SDK**.
2. Install SQL Server Express LocalDB on your development PC.
3. Clone the repo and open `Revolution.Hrms.sln`.
4. Copy `appsettings.example.json` to `appsettings.json` and check the connection string.
5. Run `Setup-Db.ps1` to create your own local `HRMS_Data` database and apply the numbered scripts.
6. Build (`Ctrl+Shift+B`) and run `Revolution.Hrms.UI`.
7. Log in with the seeded Associate user and change the password when asked.

Server PC and client PC setup is in `DEPLOYMENT.md`.

## Status
Track progress in the phase table at the top of `ROADMAP.md`.
