# Effetto Mandria - build frontend + backend e cartella pronta per il deploy.
# Produzione: UI in wwwroot, API su /api, SignalR su /hubs (stesso schema del reverse proxy Vite in dev).
#
# Uso:
#   .\build.ps1
#   .\build.ps1 -OutputDir .\out
#   .\build.ps1 -SkipNpmInstall
#
# Avvio dopo il build:
#   cd publish
#   $env:ASPNETCORE_URLS = "http://0.0.0.0:5080"
#   dotnet EffettoMandria.Api.dll

param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",

    [string]$OutputDir = "",

    [switch]$SkipNpmInstall
)

$ErrorActionPreference = "Stop"

$Root = $PSScriptRoot
$FrontendDir = Join-Path $Root "frontend"
$BackendProject = Join-Path $Root "backend\EffettoMandria.Api\EffettoMandria.Api.csproj"
$PublishDir = if ([string]::IsNullOrWhiteSpace($OutputDir)) {
    Join-Path $Root "publish"
} else {
    if ([System.IO.Path]::IsPathRooted($OutputDir)) { $OutputDir } else { Join-Path $Root $OutputDir }
}
$WwwRoot = Join-Path $PublishDir "wwwroot"
$FrontendDist = Join-Path $FrontendDir "dist"

function Require-Command($Name) {
    if (-not (Get-Command $Name -ErrorAction SilentlyContinue)) {
        throw "Comando richiesto non trovato: $Name"
    }
}

Write-Host ""
Write-Host "=== Effetto Mandria - build deploy ===" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration"
Write-Host "Output:        $PublishDir"
Write-Host ""

Require-Command "node"
Require-Command "npm"
Require-Command "dotnet"

# --- Frontend (VITE_API_BASE_URL vuoto = same-origin /api e /hubs) ---
Write-Host "[1/3] Frontend (npm run build)..." -ForegroundColor Yellow
Push-Location $FrontendDir
try {
    if (-not $SkipNpmInstall) {
        if (Test-Path "package-lock.json") {
            npm ci
        } else {
            npm install
        }
        if ($LASTEXITCODE -ne 0) { throw "npm install failed" }
    }

    # .env.production + override esplicito per il build di deploy
    $env:VITE_API_BASE_URL = ""
    Remove-Item Env:VITE_DEV_PROXY_TARGET -ErrorAction SilentlyContinue

    npm run build
    if ($LASTEXITCODE -ne 0) { throw "frontend build failed" }

    if (-not (Test-Path (Join-Path $FrontendDist "index.html"))) {
        throw "Manca frontend/dist/index.html - build frontend non riuscita."
    }
    Write-Host "      OK: $FrontendDist" -ForegroundColor Green
}
finally {
    Pop-Location
}

# --- Backend (dotnet publish) ---
Write-Host "[2/3] Backend (dotnet publish)..." -ForegroundColor Yellow
if (Test-Path $PublishDir) {
    Remove-Item $PublishDir -Recurse -Force
}
New-Item -ItemType Directory -Path $PublishDir -Force | Out-Null

dotnet publish $BackendProject -c $Configuration -o $PublishDir
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed" }
Write-Host "      OK: $PublishDir" -ForegroundColor Green

# --- Copia SPA in wwwroot ---
Write-Host "[3/3] Copia UI in wwwroot..." -ForegroundColor Yellow
if (Test-Path $WwwRoot) {
    Remove-Item $WwwRoot -Recurse -Force
}
New-Item -ItemType Directory -Path $WwwRoot -Force | Out-Null
Copy-Item -Path (Join-Path $FrontendDist "*") -Destination $WwwRoot -Recurse -Force
Write-Host "      OK: $WwwRoot" -ForegroundColor Green

# --- Riepilogo ---
$DllName = "EffettoMandria.Api.dll"
$DllPath = Join-Path $PublishDir $DllName

Write-Host ""
Write-Host "=== Deploy pronto ===" -ForegroundColor Cyan
Write-Host "Cartella: $PublishDir"
Write-Host ""
Write-Host "Il browser parla con un solo host (UI + /api + /hubs), come con il proxy Vite in sviluppo."
Write-Host ""
Write-Host "Avvio locale (LAN / telefoni):" -ForegroundColor White
Write-Host "  cd `"$PublishDir`""
Write-Host "  dotnet $DllName"
Write-Host "  (Environment e URL da appsettings.Production.json)"
Write-Host ""
Write-Host "Apri da PC/telefono: http://<IP-del-server>:5080"
Write-Host ""
