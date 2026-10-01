> **Imported 30-09-2026 from the supplied `design.md`.** Screen, table and report design target.
> The spec names are the adopted names (`DECISIONS.md` D6). Change 01-10-2026: the **Employee** user type is replaced by **Operator** (D3). `DATABASE.md` adds `PayType`, `DailyRate` and a monthly attendance table.

# Design — Revolution HRMS (Offline)

UI, database, and module design.

## 1. UI Design

### 1.1 Principles
- Data-entry speed first
- Keyboard-driven
- Clean, minimal, consistent
- High contrast, readable
- Offline: no web assets

### 1.2 Main Window (MDI)
```
┌──────────────────────────────────────────────┐
│ Menu: Masters | Attendance | Payroll |       │
│       Statutory | Career | Reports | Admin   │
├──────────────────────────────────────────────┤
│ Toolbar: New Save Delete Find Print Export   │
├──────────────────────────────────────────────┤
│                                              │
│           Child form area                    │
│                                              │
├──────────────────────────────────────────────┤
│ User | Company | FY 2026-27 | Server | Role  │
└──────────────────────────────────────────────┘
```

### 1.3 Theme
| Item | Value |
|---|---|
| Font | Segoe UI 9.5 pt |
| Primary | #1F4E79 (navy) |
| Accent | #2E8B57 (green) |
| Danger | #B22222 |
| Background | #F4F6F8 |
| Grid alt row | #EEF3F8 |
| Input focus | #FFF8DC |

### 1.4 Standard Form Layout
- Top: search / filter bar
- Left: list grid
- Right: detail tabs
- Bottom: action buttons (Save, Cancel, Print)
- Mandatory fields: red asterisk

### 1.5 Hotkeys
| Key | Action |
|---|---|
| F2 | New |
| F3 | Find |
| F5 | Refresh |
| Ctrl+S | Save |
| Ctrl+P | Print |
| Ctrl+E | Export |
| Esc | Close / cancel |
| Enter | Next field |

## 2. Screen Inventory

### 2.1 Auth
- `frmLogin`
- `frmChangePassword`
- `frmCompanySelect`

### 2.2 Masters
- `frmCompany`
- `frmFactory`
- `frmDepartment`
- `frmDesignation`
- `frmEmployee` (tabs: Personal, Job, Bank/IDs, Family, Docs)
- `frmMinWage`
- `frmHolidayCalendar`

### 2.3 Attendance
- `frmDailyAttendance`
- `frmBulkAttendance`
- `frmLeave`
- `frmOvertime`
- `frmMonthClose`

### 2.4 Payroll
- `frmSalaryStructure`
- `frmPayrollProcess`
- `frmAdvanceLoan`
- `frmArrearsBonus`
- `frmPayrollApprove`

### 2.5 Statutory
- `frmEsiSetup`
- `frmEpfSetup`
- `frmEcrExport`
- `frmEsiExport`
- `frmGratuity`
- `frmBonus`

### 2.6 Career
- `frmPromotion`
- `frmIncrement`
- `frmTransfer`
- `frmCareerHistory`

### 2.7 Reports
- `frmReportViewer` (Crystal viewer host)
- `frmReportSelect`

### 2.8 Admin
- `frmUsers`
- `frmRoles`
- `frmAuditLog`
- `frmBackup`
- `frmRestore`
- `frmLicense`
- `frmSettings`

## 3. Database Design

### 3.1 Entity Overview
```
Company 1─* Factory 1─* Employee
Employee 1─* Attendance
Employee 1─* SalaryStructure
Employee 1─* PayrollDetail *─1 PayrollRun
Employee 1─* Promotion / Increment / Transfer
Employee 1─1 Gratuity (on exit)
User *─* Company (Associate)
User *─1 Role *─* Permission
```

### 3.2 Core Tables

**sec.User**
`UserID, Username, PasswordHash, Salt, RoleID, UserType, FailedCount, LockedUntil, MustChangePwd, IsActive`

**sec.Role / sec.Permission / sec.RolePermission**
`RoleID, RoleName` | `PermissionID, Code` | mapping

**sec.UserCompany**
`UserID, CompanyID`

**mst.Company**
`CompanyID, Name, Address, PAN, TAN, EsiCode, EpfCode, LIN, IsActive`

**mst.Factory**
`FactoryID, CompanyID, Name, LicenseNo, Address, Zone`

**mst.Employee**
`EmployeeID, CompanyID, FactoryID, EmpCode, Name, FatherName, DOB, Gender, DOJ, DOL, DeptID, DesigID, SkillCategory, AadhaarEnc, PANEnc, BankAccEnc, IFSC, UANEnc, EsiIPEnc, Mobile, Address, IsActive`

**mst.EmployeeNominee**
`NomineeID, EmployeeID, Name, Relation, DOB, SharePct`

**att.Attendance**
`AttID, CompanyID, EmployeeID, AttDate, Status (P/A/H/L/WO/HD), ShiftID, InTime, OutTime, OTHours, IsLocked`

**att.Leave**
`LeaveID, EmployeeID, LeaveType, FromDate, ToDate, Days, Status`

**pay.SalaryStructure**
`StructID, EmployeeID, Basic, DA, HRA, Other, EffectiveFrom, EffectiveTo`

**pay.PayrollRun**
`RunID, CompanyID, Month, Year, Status (Draft/Processed/Approved/Locked), ProcessedBy, ApprovedBy`

**pay.PayrollDetail**
`DetailID, RunID, EmployeeID, DaysPaid, OTHours, Basic, DA, HRA, OTAmount, Gross, EsiEmp, EsiEr, EpfEmp, EpsEr, EpfEr, PT, Advance, OtherDed, NetPay`

**stat.RateTable**
`RateID, Code, Value, FromDate, ToDate`
Codes: `EPF_EMP_PCT, EPS_PCT, EPF_ER_PCT, EPF_CEILING, ESI_EMP_PCT, ESI_ER_PCT, ESI_CEILING, BONUS_MIN, BONUS_MAX, GRATUITY_CAP`

**stat.Gratuity**
`GratuityID, EmployeeID, Years, LastWage, Amount, PaidOn`

**car.Promotion / car.Increment / car.Transfer**
`ID, EmployeeID, EffectiveDate, Old*, New*, OrderNo, Remarks`

**aud.AuditLog**
`AuditID, TableName, KeyValue, Action, OldValue, NewValue, UserID, Machine, LoggedOn`

**cfg.SchemaVersion**
`Version, AppliedOn`

### 3.3 Key Constraints
- Unique: `(CompanyID, EmpCode)`
- Unique: `(EmployeeID, AttDate)`
- Unique: `(CompanyID, Month, Year)` on PayrollRun
- FK on all relations
- Check: `Status` values

## 4. Calculation Flow

### 4.1 Payroll Process
```
1. Verify attendance locked
2. Load employee list + salary structure
3. Days paid = present + WO + holidays + paid leave + 0.5 × half
4. Earned = structure × days paid / month days
5. OT amount = OT hours × 2 × ordinary rate
6. Gross = earned + OT + arrears + bonus
7. EPF = 12% of PF wage (ceiling applied)
8. ESI = 0.75% of gross (if covered), round up
9. Deduct PT, advance, other
10. Net = Gross − deductions
11. Save PayrollDetail, status = Processed
```

### 4.2 Gratuity
```
Years = completed years (>6 months = +1)
Gratuity = LastWage × 15 / 26 × Years
Apply cap from RateTable
```

## 5. Report Design

### 5.1 Report List
| Report | Format |
|---|---|
| Payslip | PDF (password) |
| Muster Roll | PDF, XLS |
| Wage Register | PDF, XLS |
| Overtime Register | PDF, XLS |
| EPF ECR | TXT |
| ESI Contribution | XLS, TXT |
| Challan Summary | PDF |
| Gratuity Statement | PDF |
| Employee Master | XLS |
| Audit Log | PDF, XLS |

### 5.2 Common Layout
- Header: company name, address, code
- Sub-header: report title, period
- Body: grid, right-aligned amounts
- Footer: page X of Y, printed by, date

### 5.3 Amount Format
`₹ 1,23,456.00` Indian grouping

## 6. Role Permission Matrix

| Module | Associate | Company | Operator |
|---|---|---|---|
| Company master | Full | View own | — |
| Employee master | Full | Full | Operate (assigned companies) |
| Attendance | Full | Full | Add and modify until month-close lock |
| Payroll process | Full | Full | — |
| Payslip | All | All | View, print (assigned companies) |
| Statutory | Full | Full | — |
| Career | Full | Full | — |
| Reports | All | Own | Operational reports (assigned companies) |
| Users | Full | Own company | — |
| Backup | Full | — | — |
| Audit | Full | Own | — |

## 7. Error & Message Design
- Info: blue icon
- Warning: amber, confirm action
- Error: red, short text, log ID
- Success: status bar text, 3 sec
- No stack traces shown

## 8. Folder Layout (Runtime)
```
C:\Program Files\RevolutionHRMS\      app binaries
C:\ProgramData\RevolutionHRMS\
   ├── Logs\
   ├── Backups\
   ├── Exports\
   ├── Temp\
   └── License\
```
