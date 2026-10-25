$ErrorActionPreference = 'Stop'

$edge = @(
    ${env:ProgramFiles(x86)}
    $env:ProgramFiles
    $env:LOCALAPPDATA
) | Where-Object { $_ } | ForEach-Object {
    Join-Path $_ 'Microsoft\Edge\Application\msedge.exe'
} | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1

if (-not $edge) {
    throw 'Microsoft Edge was not found. Install Edge before debugging the frontend.'
}

$appHost = Join-Path $PSScriptRoot '..\src\backend\ModernWebApp.AppHost\ModernWebApp.AppHost.csproj'
$timer = [System.Diagnostics.Stopwatch]::StartNew()

# Compound debug configurations start in parallel, before Aspire registers its session.
do {
    $remaining = [int][Math]::Ceiling(120 - $timer.Elapsed.TotalSeconds)
    if ($remaining -le 0) {
        throw 'Timed out waiting for Aspire. Check the AppHost debug output for startup errors.'
    }

    & aspire wait frontend --status up --timeout $remaining --apphost $appHost --non-interactive
    $waitExitCode = $LASTEXITCODE
    if ($waitExitCode -eq 0) {
        break
    }

    if ($waitExitCode -ne 7) {
        throw "Aspire could not start the frontend (exit code $waitExitCode). Check the AppHost output."
    }

    Write-Host 'Waiting for the AppHost debug session to register...'
    Start-Sleep -Seconds 2
} while ($true)

$profile = Join-Path ([System.IO.Path]::GetTempPath()) 'modern-web-app-edge-attach'
Start-Process -FilePath $edge -ArgumentList @(
    '--remote-debugging-address=127.0.0.1'
    '--remote-debugging-port=9222'
    ('--user-data-dir="' + $profile + '"')
    '--no-first-run'
    '--no-default-browser-check'
    'http://localhost:5173/'
)
