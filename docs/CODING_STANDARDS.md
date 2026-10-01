# Coding Standards (VB.NET)

One set of rules, following the spec (`DECISIONS.md` D2, D13).

## Project settings
- `Option Strict On`, `Option Explicit On`, `Option Infer On` in every project.
- Nullable reference types enabled where supported.

## Naming
| Item | Style | Example |
|---|---|---|
| Class, Module, Property, Method | PascalCase | `EmployeeService`, `GetById` |
| Local variable, parameter | camelCase | `employeeId` |
| Private field | `_camelCase` | `_repository` |
| Interface | `I` prefix | `IEmployeeService` |
| Constant | `UPPER_SNAKE` | `ESI_WAGE_CEILING` |
| Form | `frm` + name | `frmEmployeeEdit` |
| UserControl | `uc` + name | `ucEmployeeList` |
| Controls | prefix + name | `txtName`, `btnSave`, `dgvEmployees`, `cboDepartment` |
| Stored procedure | `schema.usp_<Entity>_<Action>` | `mst.usp_Employee_Insert` |

## Layers
- UI calls BLL only. BLL calls DAL only. DAL calls stored procedures only.
- Forms: UI code only (events, binding, showing validation). Rules and calculations in BLL. SQL only in database scripts.
- No `MessageBox` in BLL or DAL.

## General rules
- One class per file. File name equals class name.
- Methods up to 50 lines, files up to 500 lines.
- No magic numbers. Use constants or the rate table.
- No commented-out code. Comments explain why, not what.
- Use `Using` blocks for connections, readers and other `IDisposable` objects.
- Catch specific exceptions. Log them and show a friendly message. Global handler in `Program.vb`. Never swallow errors.
- Validate input in the UI and again in the BLL.
- No hard-coded paths, settings, keys or connection strings.

## Database access
- Call stored procedures with parameters only. Never build SQL by string concatenation.
- No `SELECT *`.
- Set the company session context right after opening a connection.

## Money and numbers
- `Decimal` for all money. Never `Double` or `Single`.
- Round with `Math.Round(x, 2, MidpointRounding.AwayFromZero)`.
- Display Indian format `₹ 1,23,456.00`. Store as `DECIMAL`.
- Statutory percentages and ceilings come from `stat.RateTable`, never inline.

## Dates
- Display `dd-MM-yyyy`. Store as `DATE` or `DATETIME2`.
- Financial year runs 1 April to 31 March.
- Set culture explicitly. Never rely on the machine culture.

## Security
- Never store plain-text passwords. Never log passwords, Aadhaar, PAN or bank numbers.
- Mask sensitive fields in the UI as `XXXX-XXXX-1234`.
- Lock the session after 10 minutes idle. Lock the account after 5 failed logins.
- Sanitize file names on export. Delete temp report files after use.

## UI
- Keyboard first: tab order, Enter to move, hotkeys (list in `DESIGN.md`).
- Mandatory fields marked. Confirm before delete. Soft delete only.
- Status bar shows user, company, financial year, server. Font Segoe UI 9 to 10 pt.
- No modal chains deeper than 2.

## Format
- 4-space indent. `.editorconfig` in the repo root is the source of truth.
