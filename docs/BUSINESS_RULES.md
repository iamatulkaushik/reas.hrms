# Business Rules (Indian labour law)

Source: supplied `rules.md` (sections 4 and 5) and `prd.md` (section 5.6). Engines must follow these rules.
All percentages and ceilings below are **defaults**. They must be read from the rate table, never hard-coded (see section 8).

## 1. Attendance
- Full day = 1.0, half day = 0.5.
- Weekly off and paid holiday are paid.
- Absent is unpaid unless an approved leave covers the day.
- Overtime counts only above shift hours.
- Attendance is locked after month-close. Unlocking needs a Company admin and an audit reason. Until the lock, an Operator can add and edit attendance for assigned companies.

## 2. Overtime (Factories Act, Sec 59)
- Rate = 2 x ordinary rate of wages.
- Ordinary rate = (Basic + DA) / 26 / shift hours.
- Overtime hours are rounded per company setting.
- Show warnings for weekly and quarterly overtime caps.

## 3. EPF
| Rule | Default |
|---|---|
| Wage ceiling | ₹ 15,000 (configurable) |
| Employee share | 12% of PF wage |
| Employer share | 12%, split 8.33% EPS and 3.67% EPF |
| EPS | Capped at the wage ceiling |
| Admin charge, EDLI | From rate table |
| Above-ceiling contribution | Voluntary, per employee |
| ECR file | Per EPFO specification |

## 4. ESI
| Rule | Default |
|---|---|
| Applicable when gross is at most | ₹ 21,000 (configurable) |
| Employee share | 0.75% |
| Employer share | 3.25% |
| Rounding | Up to the next rupee |
| Contribution periods | Apr to Sep, Oct to Mar |
| Coverage | Employee stays covered until the period ends |
| Daily wage exemption | At most ₹ 176 is exempt from the employee share (configurable) |

## 5. Gratuity
- Eligible after 5 years of continuous service. Exception: death or disablement.
- Formula: `last drawn wage x 15 / 26 x completed years`.
- Service of more than 6 months counts as a full year.
- Apply the statutory cap from the rate table.

## 6. Bonus (Payment of Bonus Act)
- Eligible: wage up to ₹ 21,000 and at least 30 working days.
- Minimum 8.33%, maximum 20% (configurable).
- Calculation ceiling comes from the rate table.

## 7. Payroll
- Process only after attendance is locked.
- States: Draft, Processed, Approved, Locked.
- Locked payroll cannot be edited. Use a reversal entry.
- Net pay = Gross - deductions.
- Negative net pay is flagged and blocked from approval.
- Arrears are posted in the month they are paid.

### Calculation flow
1. Verify attendance is locked.
2. Load the employee list and salary structure.
3. Days paid = present + weekly off + holidays + paid leave + 0.5 x half days.
4. Earned = Monthly salary: structure x days paid / days in month. Daily wage: daily rate x days paid.
5. Overtime amount = overtime hours x 2 x ordinary rate.
6. Gross = earned + overtime + arrears + bonus.
7. EPF = 12% of PF wage (ceiling applied).
8. ESI = 0.75% of gross if covered, rounded up.
9. Deduct PT, advance, other.
10. Net = Gross - deductions.
11. Save the payroll detail and set status to Processed.

## 8. Rates
- All rates live in `stat.RateTable` (target design).
- Rates are effective-dated (`FromDate`, `ToDate`).
- Codes: `EPF_EMP_PCT`, `EPS_PCT`, `EPF_ER_PCT`, `EPF_CEILING`, `ESI_EMP_PCT`, `ESI_ER_PCT`, `ESI_CEILING`, `BONUS_MIN`, `BONUS_MAX`, `GRATUITY_CAP`.
- Never hard-code a percent or ceiling.
- A rate change needs Associate rights and is audited. There is no Superadmin level.

## 9. Reporting rules
- Data comes from stored procedures only.
- Every report has company, period and print date in the header.
- Amounts are right-aligned in Indian format (`₹ 1,23,456.00`).
- Exports: PDF, XLS, TXT.
- File name: `<Report>_<Company>_<YYYYMM>.ext`.
- Statutory formats match the latest government forms.
- TXT exports follow the spec exactly, with no extra whitespace.

## 10. Compliance scope
Payment of Wages Act, Minimum Wages Act (Haryana), Factories Act 1948, EPF and MP Act 1952, ESI Act 1948, Payment of Gratuity Act 1972, Payment of Bonus Act 1965, Contract Labour Act 1970, DPDP Act 2023 (data handling).

## 11. Open points
- Legacy "Worked" already includes holidays and leave (Atul, 01-10-2026). Split it on import so nothing is added twice. In the new app "Worked" means days actually present.
- Leave accrual rules need checking against the Factories Act and Haryana rules. (`TASKS.md`, Phase 5b)
- Validate every engine against government calculators before release.
