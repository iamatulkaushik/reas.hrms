# Access Control

Three user types from the spec, with **Operator replacing Employee** (`DECISIONS.md` D3). There is no Developer level and no Database Console. Support uses SQL tools on the server PC.

| Type | Who | Access |
|---|---|---|
| **Associate** | Consultant managing many companies | The companies assigned to them in `sec.UserCompany` |
| **Company** | Employer or HR admin | Own company only |
| **Operator** | Data-entry staff | Records of the companies assigned to them: employees, attendance, leave, advances, arrears and similar entry work |

## Rules
1. Every query on a tenant table is filtered by `CompanyID`. Row-level security enforces it from `SESSION_CONTEXT('CompanyID')`, set right after connect.
2. Associate and Operator users see only companies in their `sec.UserCompany` rows. A Company user has exactly one.
3. Associate creates Company and Operator users and assigns or removes company access. A Company admin creates Operators for their own company only.
4. An Operator cannot see user management, rates and statutory setup, backup and restore, the audit log, or process or approve payroll.
5. Permission checks live in the BLL `AuthService`. Forms ask `CanAccess(module, action)` to hide or disable UI. The BLL **also** enforces it, and the database restricts with row-level security and an `EXECUTE`-only login, so the UI is never the only protection.
6. Removing a company from a user takes effect on their next action.

## Module access matrix
| Module | Associate | Company | Operator |
|---|---|---|---|
| Company master | Full | View own | - |
| Employee master | Full | Full | Operate (assigned companies) |
| Attendance | Full | Full | Add and modify until month-close lock (assigned companies) |
| Leave, overtime, advance, arrears entry | Full | Full | Operate (assigned companies) |
| Payroll process and approve | Full | Full | - |
| Payslip | All | All | View and print (assigned companies) |
| Statutory setup and rates | Full | Full | - |
| Career | Full | Full | - |
| Reports | All | Own | Operational reports (assigned companies) |
| Users | Full (Company and Operator users) | Own company (Operators) | - |
| Backup and restore | Full | - | - |
| Audit log | Full | Own | - |

"Operate" means add, edit and deactivate records, never hard delete.
**Confirmed 01-10-2026:** an Operator adds attendance and modifies it until the month-close lock. After the lock only an Associate or Company admin can reopen it, with a written reason.
**Still assumed, not stated:** Operators cannot process or approve payroll and cannot enter Career records.

## Authentication rules
| Rule | Release |
|---|---|
| Login by username and password | 15-10 (Phase 1) |
| Lock after 5 failed attempts (setting `Security.MaxFailedLogins`) | 15-10 |
| Password hash PBKDF2, 100,000+ iterations, salt | 15-10 |
| Forced password change on first login (`MustChangePwd`) | 15-10 |
| Auto-lock after 10 minutes idle (Ctrl+L, `frmUnlock`) | 15-10 |
| Role-based menu (`MenuCatalog`) | 15-10 |
| Unlocking locked attendance needs a Company admin or Associate and a written reason | 15-10 |
| Rate changes need Associate rights and are audited | 15-10 |
| Mask sensitive fields in the UI. Reveal needs permission and an audit entry | Later |

## Audit log
Log logins, failed logins, user and company-assignment changes, deletes, payroll runs and approvals, rate changes, attendance unlocks, backup and restore, and reveals of sensitive fields.
Each entry stores table, key, action, old value, new value, user, machine and time. The app login can only insert into `aud.AuditLog`. Never write password hashes into it.
