# Builds the mod and patches it into TowerFall.exe. The game has to be closed.
#
#     .\install.ps1
#
# Needs the .NET SDK, and a game folder that was patched once with a released version of the mod
# (that leaves Patcher.exe and the Patcher folder there). Set TOWERFALL_PATH if the game is not
# in Steam's default folder.
param(
  [string]$GameDir = $(if ($env:TOWERFALL_PATH) { $env:TOWERFALL_PATH } else { 'C:\Program Files (x86)\Steam\steamapps\common\TowerFall' })
)
$ErrorActionPreference = 'Stop'

if (Get-Process TowerFall -ErrorAction SilentlyContinue) {
  throw 'TowerFall is running. Close it first.'
}
if (-not (Test-Path (Join-Path $GameDir 'Patcher.exe'))) {
  throw "Patcher.exe not found in $GameDir. Install a released version of the mod there first."
}

$dotnet = if (Get-Command dotnet -ErrorAction SilentlyContinue) { 'dotnet' } else { Join-Path $env:ProgramFiles 'dotnet\dotnet.exe' }
$project = Join-Path $PSScriptRoot 'TowerFallAi\TowerfallAiMod'
& $dotnet build (Join-Path $project 'TowerfallAiMod.csproj') -c Release -nologo -v q
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }

$output = Join-Path $project 'bin\Release\net40'
Copy-Item (Join-Path $output 'TowerfallAiMod.dll'), (Join-Path $output 'TowerfallAiMod.pdb') $GameDir -Force

# Patcher.exe always starts from the vanilla exe it keeps in Patcher\Original.
Push-Location $GameDir
try {
  $log = & .\Patcher.exe
  if ($LASTEXITCODE -ne 0) { $log | Write-Host; throw 'Patcher failed.' }
} finally {
  Pop-Location
}
Write-Host "Installed the mod into $GameDir"
