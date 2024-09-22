<#
.SYNOPSIS
    发布 FrpGUI 应用程序到不同平台。

.DESCRIPTION
    此脚本用于发布 FrpGUI 应用程序到 Windows、Linux、macOS 和浏览器平台。

.PARAMETER w
    发布到 Windows 平台。

.PARAMETER l
    发布到 Linux 平台。

.PARAMETER m
    发布到 macOS 平台。

.PARAMETER c
    设置此标志以创建客户端的发布版本。

.PARAMETER s
    设置此标志以创建服务器的发布版本。

.PARAMETER b
    设置此标志以发布浏览器版本。

.EXAMPLE
    .\YourScript.ps1 -w -c -s
    发布到 Windows 平台，创建客户端和服务器的发布版本。

.EXAMPLE
    .\YourScript.ps1 -l
    发布到 Linux 平台。

.EXAMPLE
    .\YourScript.ps1 -m -c
    发布到 macOS 平台，仅创建客户端的发布版本。

.EXAMPLE
    .\YourScript.ps1 -b
    发布到浏览器平台。

#>

param(
    [Parameter()]
    [switch]$w, #Windows
    [switch]$l, #Linux
    [switch]$m, #MacOS

    [switch]$c, #Clients
    [switch]$s, #Servers
    [switch]$b #Browser
)

# 如果 $w, $l, $m 都为 false，则全部设为 true
if (-not ($w -or $l -or $m)) {
    $w = $true
    $l = $true
    $m = $true
}

# 如果 $c, $s, $b 都为 false，则全部设为 true
if (-not ($c -or $s -or $b)) {
    $c = $true
    $s = $true
    $b = $true
}

if ($w) { Write-Host "发布Windows" }
if ($l) { Write-Host "发布Linux" }
if ($m) { Write-Host "发布MacOS" }
if ($c) { Write-Host "发布客户端" }
if ($s) { Write-Host "发布服务器端" }
if ($b) { Write-Host "发布浏览器端" }
pause


$ErrorActionPreference = 'Stop'

try {
    # 检查是否安装了.NET SDK
    try {
        dotnet
    }
    catch {
        throw "未安装.NET SDK"
    }
    
    function Publish-UI {
        param (
            [string]$runtime,
            [string]$outputDirectory
        )

        Write-Output "正在发布客户端：$runtime"

        dotnet publish FrpGUI.Avalonia.Desktop -r $runtime -c Release -o $outputDirectory --self-contained true #/p:PublishSingleFile=true 

        $platform = switch ($runtime) {
            "win-x64" { "windows_amd64" }
            "linux-x64" { "linux_amd64" }
            "osx-x64" { "darwin_amd64" }
        }
        mkdir $outputDirectory/frp -ErrorAction SilentlyContinue
        Copy-Item "bin/frp_*_$platform/*" $outputDirectory/frp -Recurse

        if (Test-Path $outputDirectory/FrpGUI.Avalonia.Desktop.exe) {
            Move-Item $outputDirectory/FrpGUI.Avalonia.Desktop.exe $outputDirectory/FrpGUI.exe
        }

        if (Test-Path $outputDirectory/FrpGUI.Avalonia.Desktop) {
            Move-Item $outputDirectory/FrpGUI.Avalonia.Desktop $outputDirectory/FrpGUI
        }
    }
    
    function Publish-Service {
        param (
            [string]$runtime,
            [string]$outputDirectory
        )

        Write-Output "正在发布服务：$runtime"

        dotnet publish FrpGUI.WebAPI -r $runtime -c Release -o $outputDirectory --self-contained true

        $platform = switch ($runtime) {
            "win-x64" { "windows_amd64" }
            "linux-x64" { "linux_amd64" }
            "osx-x64" { "darwin_amd64" }
        }
        mkdir $outputDirectory/frp -ErrorAction SilentlyContinue
        Copy-Item "bin/frp_*_$platform/*" $outputDirectory/frp -Recurse
    }

    Clear-Host

    # 如果Publish目录存在，则删除
    if (Test-Path "Publish") {
        Remove-Item "Publish" -Recurse -Force
    }

    if ($c) {
        if ($w) { Publish-UI -runtime "win-x64" -outputDirectory "Publish/client-win-x64" }
        if ($l) { Publish-UI -runtime "linux-x64" -outputDirectory "Publish/client-linux-x64" }
        if ($m) { Publish-UI -runtime "osx-x64" -outputDirectory "Publish/client-macos-x64" }
    }
    
    if ($s) {
        if ($w) { Publish-Service -runtime "win-x64" -outputDirectory "Publish/server-win-x64" }
        if ($l) { Publish-Service -runtime "linux-x64" -outputDirectory "Publish/server-linux-x64" }
        if ($m) { Publish-Service -runtime "osx-x64" -outputDirectory "Publish/server-macos-x64" }
    }
    
    if ($b) {
        Write-Output "正在发布：Browser"
        dotnet publish FrpGUI.Avalonia.Browser -r browser-wasm -c Release -o "Publish/browser" --self-contained true
        Move-Item "Publish/browser/wwwroot/*" "Publish/browser"
        Copy-Item "FrpGUI.Avalonia.Browser/web.config" "Publish/browser"
        Copy-Item "FrpGUI.Avalonia.Browser/uiconfig.json" "Publish/browser"
        Remove-Item "Publish/browser/obj" -r
        Remove-Item "Publish/browser/wwwroot" -r
    }

    Write-Output "正在清理"
    Remove-Item FrpGUI*/bin/Release -Recurse

    Write-Output "操作完成"

    Invoke-Item Publish
    pause
}
catch {
    Write-Error $_
}
