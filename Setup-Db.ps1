<#
.SYNOPSIS
    Creates the HRMS_Data database if missing, then applies pending numbered scripts.

.DESCRIPTION
    1. Creates the database if it does not exist.
    2. Runs every Database\Scripts\NNNN_*.sql that is not yet in cfg.SchemaVersion, in order,
       and records it. Scripts must be idempotent. Never edit a merged script: add a new one.
    3. Runs the reference seed scripts (Database\Seed\Reference). These must be idempotent too,
       so they can run every time the database is created or upgraded.
    4. With -Sample, also runs Database\Seed\Sample (development data only).

    Uses Windows authentication. Needs the sqlcmd command line tool.

.EXAMPLE
    .\Setup-Db.ps1
    .\Setup-Db.ps1 -Sample
    .\Setup-Db.ps1 -Server 'SERVERPC,14330' -TrustServerCertificate
#>
[CmdletBinding()]
param(
    [string]$Server = '(localdb)\MSSQLLocalDB',
    [string]$Database = 'HRMS_Data',
    [switch]$Sample,
    [switch]$TrustServerCertificate
)

$ErrorActionPreference = 'Stop'

if ($Database -notmatch '^[A-Za-z0-9_]+$') { throw "Invalid database name: $Database" }
if (-not (Get-Command sqlcmd -ErrorAction SilentlyContinue)) {
    throw 'sqlcmd was not found. Install the SQL Server command line tools and run again.'
}

$scriptsDir = Join-Path $PSScriptRoot 'Database\Scripts'
$referenceDir = Join-Path $PSScriptRoot 'Database\Seed\Reference'
$sampleDir = Join-Path $PSScriptRoot 'Database\Seed\Sample'

$common = @('-S', $Server, '-E', '-b')
if ($TrustServerCertificate) { $common += '-C' }

function Invoke-Query {
    param([string]$Db, [string]$Query)
    $output = & sqlcmd @common -d $Db -h -1 -W -Q $Query
    if ($LASTEXITCODE -ne 0) { throw "sqlcmd failed: $Query`n$output" }
    return $output
}

function Invoke-ScriptFile {
    param([string]$Db, [string]$Path)
    & sqlcmd @common -d $Db -i $Path
    if ($LASTEXITCODE -ne 0) { throw "Script failed: $Path" }
}

if ($Server -like '(localdb)*') {
    $instance = ($Server -replace '^\(localdb\)\\', '')
    & sqllocaldb start $instance 2>$null | Out-Null
}

Write-Host "Server: $Server  Database: $Database"
Invoke-Query -Db 'master' -Query "IF DB_ID(N'$Database') IS NULL CREATE DATABASE [$Database];" | Out-Null

$appliedQuery = "SET NOCOUNT ON; IF OBJECT_ID(N'cfg.SchemaVersion') IS NOT NULL SELECT Version FROM cfg.SchemaVersion;"
$applied = @(Invoke-Query -Db $Database -Query $appliedQuery | ForEach-Object { "$_".Trim() } | Where-Object { $_ })

$scripts = Get-ChildItem -Path $scriptsDir -Filter '*.sql' | Sort-Object Name
foreach ($script in $scripts) {
    if ($script.Name -notmatch '^\d{4}_.+\.sql$') {
        Write-Warning "Skipped (name must look like 0001_name.sql): $($script.Name)"
        continue
    }
    $version = $script.BaseName
    if ($applied -contains $version) {
        Write-Host "  already applied  $version"
        continue
    }
    Write-Host "  applying         $version"
    Invoke-ScriptFile -Db $Database -Path $script.FullName
    Invoke-Query -Db $Database -Query "INSERT INTO cfg.SchemaVersion (Version) VALUES (N'$version');" | Out-Null
}

$seedDirs = @($referenceDir)
if ($Sample) { $seedDirs += $sampleDir }
foreach ($dir in $seedDirs) {
    foreach ($seed in (Get-ChildItem -Path $dir -Filter '*.sql' -ErrorAction SilentlyContinue | Sort-Object Name)) {
        Write-Host "  seeding          $($seed.Name)"
        Invoke-ScriptFile -Db $Database -Path $seed.FullName
    }
}

Write-Host 'Done.'
