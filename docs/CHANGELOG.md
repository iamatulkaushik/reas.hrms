# Changelog

Format: [Keep a Changelog](https://keepachangelog.com). Versioning: `MAJOR.MINOR.PATCH`. Release numbers follow the spec plan in `PRD.md` s11.

## [Unreleased]
### Added
- Project documentation set, rebuilt on 01-10-2026 to follow the spec
- Spec documents: `PRD.md`, `DESIGN.md`, `BUSINESS_RULES.md`, `TASKS.md`
- `DECISIONS.md` with D1 to D14 filled in
- `ROADMAP.md` for the 15-10-2026 release, `CLAUDE.md` for Claude Code
### Changed
- Fresh start: all old code, scripts and process files erased, only `docs/` kept
- Database: SQL Server Express on a server PC with 5 to 7 client PCs (was LocalDB per PC)
- Data access: stored procedures only, row-level security, schemas `sec mst att pay stat car aud cfg`
- Users: Associate, Company, Operator. Operator replaces the spec's Employee. The Developer level and Database Console are dropped

## [0.3.0] - 2026-10-15 (planned)
### Added
- Login, lockout, idle lock, role-based menu
- Companies, users and company assignment
- Employees with pay type, departments, designations, holidays, financial year
- Daily and monthly attendance with month-close lock
- Salary structure, payroll run, overtime, EPF and ESI calculation
- Backup script for the server PC

## Later releases (spec plan)
- 0.4: statutory files, gratuity, bonus, career
- 0.5: Crystal reports, password-protected payslip
- 1.0: installer, licensing, hardening
