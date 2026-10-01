# Deployment

Topology: **one server PC** running SQL Server Express and **5 to 7 client PCs** on the LAN (`DECISIONS.md` D1). Items marked **Later** are outside the 15-10-2026 release.

## Server PC setup
1. Install SQL Server Express (2019 or later) with TCP/IP enabled.
2. Set a fixed, non-default TCP port. Turn SQL Browser off.
3. Force Encryption (TLS) with a certificate.
4. Disable the `sa` login.
5. Create database `HRMS_Data`, run the numbered scripts, create the app login with `EXECUTE` only (never `db_owner`).
6. Firewall: allow the SQL port only from the client PC addresses.
7. Turn on BitLocker for the data drive (Express has no TDE).
8. Create the backup folder and the Task Scheduler job (see Backup).

Limits to know: Express caps a database at 10 GB and uses about 1 GB RAM. Use Standard beyond that.

## Client PC setup
- Install the app to `C:\Program Files\RevolutionHRMS`.
- Create `C:\ProgramData\RevolutionHRMS` with Modify rights for Users: `Logs\`, `Exports\`, `Temp\`, `License\`.
- Copy `appsettings.example.json` to `appsettings.json` and set the server name, port and app login. DPAPI-encrypted config comes **Later**.
- Test: open the app, log in, and add one employee from this PC.

## Build
```
dotnet publish Revolution.Hrms.UI -c Release -r win-x64 --self-contained true -o publish
```
Self-contained means the client PC does not need the .NET runtime installed.

## Installer (Later)
Inno Setup with server install mode and client install mode. It bundles .NET and the SQL scripts, takes a backup before an upgrade, runs numbered idempotent upgrade scripts, cleans up on uninstall, and ships a user manual and an admin guide (PDF). Crystal runtime is added when reports ship.
For 15-10-2026 use a written install guide for the steps above.

## Runtime folders
```
C:\Program Files\RevolutionHRMS\      app binaries
C:\ProgramData\RevolutionHRMS\
   Logs\  Backups\  Exports\  Temp\  License\
```

## Backup and recovery
- **15-10-2026:** a `sqlcmd` script runs a full backup of `HRMS_Data` to the backup folder, started by Task Scheduler (Express has no SQL Agent). Copy the files to a USB drive or NAS regularly. Test one restore before release.
- **Later (spec plan):** nightly full backup, compressed and encrypted. Differential every 4 hours. Keep 30 daily and 12 monthly copies. Restore wizard in Admin. Monthly restore test.
- Targets: RPO 24 hours, RTO 4 hours.

## Upgrades
- Numbered idempotent scripts, applied in order, recorded in `cfg.SchemaVersion`.
- Take a backup before applying any script.
- Never overwrite the live database during an upgrade.

## Licensing and hardening (Later)
Machine fingerprint, RSA-signed license file, offline activation, company and employee limits. Firewall rule script, BitLocker setup guide, LAN penetration test.

## Clean-PC test
Install on a Windows machine without Visual Studio. Run login, employee add, attendance entry and a payroll run. Do it on at least two client PCs while another user works at the same time.
