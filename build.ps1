# PEAK-AIO Build Script
[CmdletBinding()]
param (
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

Write-Host "======================================================" -ForegroundColor Cyan
Write-Host "             PEAK-AIO Project Build Script" -ForegroundColor Cyan
Write-Host "======================================================" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow

$rootDir = $PSScriptRoot
$projFile = Join-Path $rootDir "PEAK-AIO\PEAK-AIO.csproj"

if (-not (Test-Path $projFile)) {
    Write-Error "Project file not found: $projFile"
    exit 1
}

# 1. Locate MSBuild.exe
$msBuildCandidates = @(
    "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe",
    "$env:WINDIR\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
)

$msBuildPath = $null

foreach ($cand in $msBuildCandidates) {
    if (Test-Path $cand) {
        $msBuildPath = $cand
        break
    }
}

# Fallback: check vswhere
if (-not $msBuildPath) {
    $vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
    if (Test-Path $vswhere) {
        $vsMsbuild = & $vswhere -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe 2>$null
        if ($vsMsbuild -and (Test-Path $vsMsbuild[0])) {
            $msBuildPath = $vsMsbuild[0]
        }
    }
}

# Fallback: check PATH
if (-not $msBuildPath) {
    $cmd = Get-Command "msbuild" -ErrorAction SilentlyContinue
    if ($cmd) {
        $msBuildPath = $cmd.Source
    }
}

if (-not $msBuildPath) {
    Write-Error "MSBuild.exe was not found. Please install .NET Framework 4.7.2 Developer Pack or Visual Studio."
    exit 1
}

Write-Host "Using MSBuild: $msBuildPath" -ForegroundColor Gray
Write-Host "Building project..." -ForegroundColor Green

$buildProcess = Start-Process -FilePath $msBuildPath -ArgumentList "`"$projFile`"", "/p:Configuration=$Configuration", "/nologo", "/v:m" -NoNewWindow -PassThru -Wait

if ($buildProcess.ExitCode -ne 0) {
    Write-Host ""
    Write-Host "Build failed with exit code $($buildProcess.ExitCode)." -ForegroundColor Red
    exit $buildProcess.ExitCode
}

$outDir = Join-Path $rootDir "PEAK-AIO\bin\$Configuration"
$outDll = Join-Path $outDir "PEAK-AIO.dll"
$outPdb = Join-Path $outDir "PEAK-AIO.pdb"

if (-not (Test-Path $outDll)) {
    Write-Error "Build finished but output dll was not found: $outDll"
    exit 1
}

# Copy output to root directory for easy access
$targetDll = Join-Path $rootDir "PEAK-AIO.dll"
$targetPdb = Join-Path $rootDir "PEAK-AIO.pdb"

Copy-Item -Path $outDll -Destination $targetDll -Force
if (Test-Path $outPdb) {
    Copy-Item -Path $outPdb -Destination $targetPdb -Force
}

$dllInfo = Get-Item $targetDll
$sizeKb = [Math]::Round($dllInfo.Length / 1KB, 2)

Write-Host ""
Write-Host "======================================================" -ForegroundColor Green
Write-Host "  BUILD SUCCEEDED!" -ForegroundColor Green
Write-Host "  Output:   $outDll" -ForegroundColor Gray
Write-Host "  Deployed: $targetDll ($sizeKb KB)" -ForegroundColor Cyan
Write-Host "======================================================" -ForegroundColor Green
