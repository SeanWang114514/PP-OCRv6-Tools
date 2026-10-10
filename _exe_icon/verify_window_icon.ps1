# verify_window_icon.ps1 — 启动 exe，验证窗口标题栏图标（左上角）已设置
param(
    [string]$ExePath = 'D:\VibeCoding\ocr工具\native-dist\ChineseOCRLiteDesktop.exe',
    [string]$ShotDir = 'D:\VibeCoding\ocr工具\_exe_icon'
)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
Add-Type @"
using System;
using System.Runtime.InteropServices;
using System.Text;
public class WinIconProbe {
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] public static extern IntPtr FindWindowW(string cls, string title);
    [DllImport("user32.dll")] public static extern IntPtr SendMessage(IntPtr h, uint m, IntPtr w, IntPtr l);
    [DllImport("user32.dll", EntryPoint = "GetClassLongPtrW")] public static extern IntPtr GetClassLongPtr(IntPtr h, int idx);
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr h, out RECT r);
    [DllImport("user32.dll")] public static extern bool PrintWindow(IntPtr h, IntPtr hdc, uint flags);
    [DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr h, IntPtr a, int x, int y, int cx, int cy, uint f);
    [StructLayout(LayoutKind.Sequential)] public struct RECT { public int L, T, R, B; }
}
"@
$p = Start-Process -FilePath $ExePath -PassThru
Start-Sleep -Seconds 5
$p.Refresh()
if ($p.HasExited) { Write-Output "EXITED code=$($p.ExitCode)"; exit 1 }
$h = [WinIconProbe]::FindWindowW('ChineseOCRLiteDesktop', $null)
if ($h -eq [IntPtr]::Zero) { Write-Output 'NO MAIN WINDOW'; Stop-Process -Id $p.Id -Force; exit 1 }
Write-Output "main window: $h"
# WM_GETICON: ICON_SMALL=0, ICON_BIG=1, ICON_SMALL2=2
$small = [WinIconProbe]::SendMessage($h, 0x007F, [IntPtr]0, [IntPtr]0)
$big   = [WinIconProbe]::SendMessage($h, 0x007F, [IntPtr]1, [IntPtr]0)
$clsIcon  = [WinIconProbe]::GetClassLongPtr($h, -14)   # GCLP_HICON
$clsIconS = [WinIconProbe]::GetClassLongPtr($h, -34)   # GCLP_HICONSM
Write-Output ("WM_GETICON small=0x{0:X} big=0x{1:X} | class HICON=0x{2:X} HICONSM=0x{3:X}" -f $small.ToInt64(), $big.ToInt64(), $clsIcon.ToInt64(), $clsIconS.ToInt64())
if ($clsIcon -eq [IntPtr]::Zero -and $big -eq [IntPtr]::Zero) { Write-Output 'ICON NOT SET'; Stop-Process -Id $p.Id -Force; exit 1 }
# 窗口移到固定位置，PrintWindow 截图（含标题栏）
[WinIconProbe]::SetWindowPos($h, [IntPtr]::Zero, 60, 60, 0, 0, 0x0001) | Out-Null
Start-Sleep -Milliseconds 500
$r = New-Object WinIconProbe+RECT
[WinIconProbe]::GetWindowRect($h, [ref]$r) | Out-Null
$w = $r.R - $r.L; $hh = $r.B - $r.T
$bmp = New-Object System.Drawing.Bitmap($w, $hh)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$hdc = $g.GetHdc()
$ok = [WinIconProbe]::PrintWindow($h, $hdc, 2)
$g.ReleaseHdc($hdc)
if ($ok) {
    $png = Join-Path $ShotDir 'window_title.png'
    $bmp.Save($png, [System.Drawing.Imaging.ImageFormat]::Png)
    Write-Output "window shot: $png ($w x $hh)"
}
$g.Dispose(); $bmp.Dispose()
Stop-Process -Id $p.Id -Force
Write-Output 'DONE'
