# Modules and Ownership

Scope follows the spec (`PRD.md`) and the cut in `ROADMAP.md`. "15-10" means in the 15-10-2026 release (v0.3). "Later" follows the spec release plan.

| Module | Owner | Release | Main features (PRD FR numbers) |
|---|---|---|---|
| Database scripts, DAL, Common | Dev A | 15-10 | Schemas, procedures, row-level security, connection manager, migrations runner, config, logger, crypto helper |
| Authentication | Dev A (BLL, procedures), Dev B (forms) | 15-10 | Login, lockout, idle lock, forced password change, role-based menu (FR-1 to FR-5) |
| Users and company assignment | Dev A | 15-10 | Associate creates Company and Operator users and assigns companies (FR-54) |
| Company master | Dev A | 15-10 | Company, PAN, TAN, ESI and EPF codes, LIN (FR-6, FR-7) |
| Department, designation, holidays, financial year | Dev B | 15-10 | Masters and calendar (FR-8, FR-16) |
| Employee master | Dev A | 15-10 | Auto code, personal and job details, nominee, **pay type**, search (FR-11, FR-12, FR-14, FR-16) |
| Attendance | Dev B | 15-10 | Daily status, monthly summary, half day, overtime hours, month-close lock. Operators add and edit until the lock (FR-17, FR-19, FR-21) |
| Payroll | Dev A (engines, run), Dev B (screens) | 15-10 | Salary structure, processing by pay type, overtime at 2x, advance, arrears, state machine, lock after approval (FR-23 to FR-27, FR-29) |
| EPF and ESI calculation | Dev A | 15-10 | Calculation only, rates from `stat.RateTable` (FR-31 to FR-35, FR-40) |
| Backup | Dev A | 15-10 | `sqlcmd` backup script on the server PC. UI later |
| Factory, Division, minimum wage masters | Unassigned | Later | FR-6, FR-9, FR-10 |
| Employee extras | Unassigned | Later | Encrypted IDs, photo, documents (FR-13, FR-15) |
| Attendance extras | Unassigned | Later | Bulk XLS upload, shifts, leave register and balance (FR-18, FR-20, FR-22) |
| Statutory files | Unassigned | Later | ECR file, ESI file, challan, gratuity, bonus (FR-36 to FR-39) |
| Career | Unassigned | Later | Promotion, increment, transfer, history (FR-41 to FR-44) |
| Reports | Unassigned | Later | Payslip, muster roll, registers, exports, password-protected PDF (FR-45 to FR-53) |
| Admin extras | Unassigned | Later | Audit viewer, backup and restore UI, license activation (FR-55 to FR-57) |

## Leave in this release
Leave types are attendance codes (CL, EL, SL, comp off, weekly off, holiday) in the daily and monthly attendance entry. The leave register, balances and accrual rules come later, after the rules are checked against the Factories Act and Haryana rules.

## Dependencies
- Everything depends on the database scripts, DAL and authentication, so those are built first (Phase 1).
- Payroll needs employees, attendance and the rate table.
- Reports need data from all modules.

## Non-goals for v1 (spec)
Cloud sync, mobile app, biometric integration, online payment or bank API, multi-language UI. Employee self-service was removed when Operator replaced Employee.

## Definition of done (per module)
- [ ] Add, edit, deactivate and search all work
- [ ] Company filter and role permissions applied (UI, BLL and database)
- [ ] Validation and error messages present
- [ ] Unit tests for any calculation, manual test with sample data (`TESTING.md`)
- [ ] Merged to the default branch through a reviewed PR
- [ ] Phase checkbox ticked in `ROADMAP.md` in the same PR
