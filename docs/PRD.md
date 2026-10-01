> **Imported 30-09-2026 from the supplied `prd.md`.** Requirements below are the product target.
> Decisions on conflicts and the 15-10-2026 cut are in `DECISIONS.md` and `ROADMAP.md`.
> Change 01-10-2026: the **Employee** user type is replaced by **Operator** (`DECISIONS.md` D3, `ACCESS_CONTROL.md`).

# PRD — Revolution HRMS (Offline)

## 1. Product Summary
Offline, LAN-based HRMS for Indian (Haryana) labour law compliance.
Stack: VB.NET WinForms, MS SQL Server, Crystal Reports.
Target: small businesses, factories, compliance consultants.

## 2. Goals
- Run fully offline, no internet dependency
- Multi-PC access over LAN
- Accurate ESI, EPF, gratuity, overtime
- Statutory reports in PDF, XLS, TXT
- Secure sensitive employee data
- Serve multiple companies per installation

## 3. Non-Goals (v1)
- Cloud sync
- Mobile app
- Biometric device integration
- Online payment / bank API
- Multi-language UI

## 4. User Types
| Type | Description | Access |
|---|---|---|
| Associate | Consultant managing many companies | All assigned companies |
| Company | Employer / HR admin | Own company only |
| Operator | Data-entry staff | Operates records (employees, attendance, leave, advances) of the companies assigned to them |

## 5. Functional Requirements

### 5.1 Authentication
- FR-1: Login by username + password
- FR-2: Lockout after 5 failed attempts
- FR-3: Auto-lock after 10 min idle
- FR-4: Password change, forced on first login
- FR-5: Role-based menu

### 5.2 Company & Factory Master
- FR-6: Create/edit company, factory, branch
- FR-7: Store PAN, TAN, ESI code, EPF code, LIN
- FR-8: Financial year setup
- FR-9: Wage period configuration (monthly / weekly)
- FR-10: Haryana minimum wage table by zone/skill

### 5.3 Employee Master
- FR-11: Employee code auto-generation
- FR-12: Personal, address, family, nominee details
- FR-13: Aadhaar, PAN, bank, UAN, ESI IP number (encrypted)
- FR-14: Joining, confirmation, exit dates
- FR-15: Photo and document attachment
- FR-16: Department, designation, skill category

### 5.4 Attendance
- FR-17: Daily attendance: Present, Absent, Half day, Full day
- FR-18: Leave types: CL, SL, EL, weekly off, holiday
- FR-19: Overtime entry in hours
- FR-20: Bulk attendance upload via XLS
- FR-21: Month-close attendance lock
- FR-22: Shift definition

### 5.5 Payroll
- FR-23: Salary structure per employee (Basic, DA, HRA, allowances)
- FR-24: Monthly salary processing
- FR-25: Overtime at double rate (Factories Act)
- FR-26: Deductions: ESI, EPF, PT, advance, loan, fine
- FR-27: Arrears and bonus
- FR-28: Payslip generation
- FR-29: Payroll lock after approval
- FR-30: Rupee format (₹, Indian digit grouping)

### 5.6 Statutory
- FR-31: EPF: employee 12%, employer 12% (8.33% EPS + 3.67% EPF), admin, EDLI
- FR-32: ESI: employee 0.75%, employer 3.25%
- FR-33: ESI wage ceiling check (₹21,000)
- FR-34: EPF wage ceiling handling (₹15,000)
- FR-35: ESI contribution period logic
- FR-36: EPF ECR text file export
- FR-37: ESI monthly contribution file export
- FR-38: Gratuity calculation (15/26 × last drawn × years)
- FR-39: Bonus calculation (Payment of Bonus Act)
- FR-40: Rates configurable, not hard-coded

### 5.7 Career
- FR-41: Promotion entry with effective date
- FR-42: Increment entry
- FR-43: Transfer between factories
- FR-44: Full career history view

### 5.8 Reports
- FR-45: Payslip
- FR-46: Muster roll
- FR-47: Wage register
- FR-48: Overtime register
- FR-49: ESI / EPF challan
- FR-50: Gratuity statement
- FR-51: Employee master listing
- FR-52: Export to PDF, XLS, TXT
- FR-53: Password-protected payslip PDF

### 5.9 Admin
- FR-54: User and role management
- FR-55: Audit log viewer
- FR-56: Backup and restore
- FR-57: License activation

## 6. Non-Functional Requirements
| Area | Requirement |
|---|---|
| Performance | Process 1,000 employees payroll under 60 sec |
| Concurrency | 10 simultaneous LAN users |
| Availability | Works with no internet |
| Security | TDE, field encryption, RLS, audit trail |
| Recovery | RPO 24h, RTO 4h |
| Compatibility | Windows 10/11, SQL Server 2019+ |
| Usability | Keyboard-first data entry |
| Scalability | Up to 50 companies, 10,000 employees |

## 7. Compliance Scope
- Payment of Wages Act
- Minimum Wages Act (Haryana)
- Factories Act, 1948
- EPF & MP Act, 1952
- ESI Act, 1948
- Payment of Gratuity Act, 1972
- Payment of Bonus Act, 1965
- Contract Labour Act, 1970
- DPDP Act 2023 (data handling)

## 8. Success Metrics
- Payroll accuracy: 100% vs manual check
- Payroll run: under 60 sec for 1,000 employees
- Zero cross-company data leaks
- Backup restore success: 100%

## 9. Assumptions
- Dedicated server PC on LAN
- Windows domain not required
- Crystal Reports runtime installable
- Statutory rates change; admin can update

## 10. Risks
| Risk | Mitigation |
|---|---|
| Statutory rate changes | Rate table, effective-dated |
| Data loss | Encrypted backups, restore tests |
| Piracy | Machine-bound license |
| LAN tampering | Force encryption, firewall |
| Crystal runtime issues | Bundle in installer |

## 11. Release Plan
- v0.1: Auth, masters
- v0.2: Attendance
- v0.3: Payroll
- v0.4: ESI/EPF/gratuity
- v0.5: Reports
- v1.0: Installer, licensing, hardening
