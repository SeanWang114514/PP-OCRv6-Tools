# patch_icon.ps1 — 用 UpdateResource 替换 PE 内嵌图标组（不改动其他字节）
param(
    [string]$ExePath,
    [string]$IcoPath
)
$ErrorActionPreference = 'Stop'

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Add-Type -Path (Join-Path $scriptDir 'ResPatch.cs')

$err = $null
$ok = [ResPatch]::Patch($ExePath, $IcoPath, [ref]$err)
if (-not $ok) { throw "Patch failed: $err" }
Write-Output '===== PATCHED VERIFY ====='
[ResPatch]::Verify($ExePath)
