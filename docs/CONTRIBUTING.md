# Contributing / Git Workflow

## Branches
- **`master`** (default branch): always builds and runs. Nobody pushes directly.
- `feature/<module>-<short-name>`, for example `feature/attendance-entry`
- `fix/<short-name>` for bug fixes

## Daily flow
```
git checkout master && git pull
git checkout -b feature/attendance-entry
# work, commit often
git fetch origin && git rebase origin/master
git push -u origin feature/attendance-entry
# open a Pull Request into master
```

## Pull request rules
- Small PRs (one feature or fix). Merge at least daily.
- The other developer reviews before merge. Keep review turnaround under a few hours.
- The PR must build with no errors and follow `CODING_STANDARDS.md`.
- PR description: what changed, how to test, any script, schema or shared file change.
- Update the affected doc, the `ROADMAP.md` checkbox and `CHANGELOG.md` in the same PR.

## Commit messages
`<module>: <what>`, for example `payroll: add salary engine`.

## Database scripts
- Numbered `0001_init.sql`, `0002_...`. Take the next free number and announce it to the other developer before you merge.
- Idempotent (`IF NOT EXISTS`). Never edit a merged script: add a new one.

## WinForms and VB.NET conflict rules
- Only the module owner edits that module's forms and `*.Designer.vb` / `*.resx` files.
- If a Designer conflict appears, do **not** merge by hand. Take one side, then redo the small UI change in Visual Studio.
- Keep logic out of code-behind: put it in services in BLL.
- Shared files (`frmMain`, `Program`, `Shared/*`, database scripts, BLL interfaces, Models) need a heads-up to the other developer before editing.
- Do not commit `bin/`, `obj/`, `.vs/`, `appsettings.json` or database files.

## Recommended `.gitattributes`
```
*.vb        text eol=crlf
*.sln       text eol=crlf
*.vbproj    text eol=crlf
*.resx      text eol=crlf
*.sql       text eol=crlf
```
