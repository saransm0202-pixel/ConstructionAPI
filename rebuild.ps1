# One-command backend workflow: stop API -> publish -> (optional) run from .run folder.
# Run from the solution root:  .\rebuild.ps1
#   .\rebuild.ps1          -> publish only (port 7179 stays FREE, use VS F5 to run)
#   .\rebuild.ps1 -Start   -> publish AND start the API
#
# WHY A SEPARATE RUN FOLDER:
# The server loads its DLLs from .run\, so a normal `dotnet build` (or a build in
# Visual Studio) writes to bin\Debug\ WITHOUT touching the running app.
# The old "file is locked by SSConstructions" error is gone for good.
#
# NOTE: By default the script does NOT start the server, so you can run it yourself
# (Visual Studio) without "address already in use" conflicts.

param([switch]$Start)

$ErrorActionPreference = 'Stop'
$Port = 7179
$Exe  = 'SSConstructions'
$RunDir  = Join-Path $PSScriptRoot '.run'
$Uploads = Join-Path $RunDir 'wwwroot\uploads'
$Backup  = Join-Path $PSScriptRoot '.backup-uploads'
$Proj = Join-Path $PSScriptRoot 'SSConstructions\SSConstructions.csproj'
$RunExe = Join-Path $RunDir "$Exe.exe"

Write-Host '1/4 Stopping running API...' -ForegroundColor Yellow
$listening = Get-NetTCPConnection -State Listen -ErrorAction SilentlyContinue |
    Where-Object { $_.LocalPort -eq $Port }
foreach ($c in $listening) {
    Stop-Process -Id $c.OwningProcess -Force -ErrorAction SilentlyContinue
}
Get-Process -Name $Exe -ErrorAction SilentlyContinue |
    ForEach-Object { Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue }
Start-Sleep -Seconds 2

Write-Host '2/4 Stashing uploaded images, publishing to .run\ ...' -ForegroundColor Yellow
if (Test-Path $Uploads) {
    Remove-Item $Backup -Recurse -Force -ErrorAction SilentlyContinue
    Copy-Item $Uploads $Backup -Recurse -Force
    Write-Host "   stashed $($Uploads) -> $($Backup)"
}
dotnet publish $Proj -c Debug -o $RunDir
if ($LASTEXITCODE -ne 0) {
    Write-Host 'PUBLISH FAILED - see errors above. API was not started.' -ForegroundColor Red
    exit 1
}
if (Test-Path $Backup) {
    Remove-Item $Uploads -Recurse -Force -ErrorAction SilentlyContinue
    Copy-Item $Backup $Uploads -Recurse -Force
    Write-Host "   restored uploaded images to $($Uploads)"
}
# Also merge any uploads kept in the SOURCE project's wwwroot (legacy dotnet run era),
# so previously uploaded images are never orphaned by the .run deploy.
$SourceUploads = Join-Path $PSScriptRoot 'SSConstructions\wwwroot\uploads'
if (Test-Path (Join-Path $SourceUploads 'projects')) {
    Copy-Item (Join-Path $SourceUploads '*') $Uploads -Recurse -Force
    Write-Host "   merged source-wwwroot uploads into $($Uploads)"
}

if (-not $Start) {
    Write-Host 'Publish OK.' -ForegroundColor Green
    Write-Host 'Server NOT started (port 7179 is free) - run it yourself in Visual Studio (F5).' -ForegroundColor Cyan
    Write-Host 'Or start it from here:  .\rebuild.ps1 -Start' -ForegroundColor Cyan
    exit 0
}

Write-Host '3/4 Starting API from .run\ ...' -ForegroundColor Yellow
Start-Process -FilePath $RunExe -ArgumentList @(
    '--urls', "https://localhost:$Port",
    '--contentRoot', $RunDir
) -WorkingDirectory $RunDir -WindowStyle Hidden

Write-Host '4/4 Waiting for API to come up...' -ForegroundColor Yellow
$cameUp = $false
for ($i = 0; $i -lt 60; $i++) {
    Start-Sleep -Seconds 1
    if (Get-NetTCPConnection -State Listen -ErrorAction SilentlyContinue |
        Where-Object { $_.LocalPort -eq $Port }) {
        $cameUp = $true
        break
    }
}
if ($cameUp) {
    Write-Host "API is UP on https://localhost:$Port" -ForegroundColor Green
    Write-Host "Use .\stop-api.ps1 (or .\rebuild.ps1) to free the port before running it yourself." -ForegroundColor Cyan
} else {
    Write-Host 'API did not come up - this window may contain the reason:' -ForegroundColor Red
    exit 1
}