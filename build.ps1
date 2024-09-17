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

        Write-Output "正在发布UI：$runtime"

        dotnet publish FrpGUI.Avalonia.Desktop -r $runtime -c Release -o $outputDirectory --self-contained true /p:PublishSingleFile=true 

        $platform = switch ($runtime) {
            "win-x64" { "windows_amd64" }
            "linux-x64" { "linux_amd64" }
            "osx-x64" { "darwin_amd64" }
        }
        mkdir $outputDirectory/frp -ErrorAction SilentlyContinue
        Copy-Item "bin/frp_*_$platform/*" $outputDirectory/frp -Recurse
    }
    
    function Publish-Service {
        param (
            [string]$runtime,
            [string]$outputDirectory
        )

        Write-Output "正在发布Service：$runtime"

        dotnet publish FrpGUI.Service -r $runtime -c Release -o $outputDirectory --self-contained true

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

    Publish-UI -runtime "win-x64" -outputDirectory "Publish/ui/win-x64"
    Publish-UI -runtime "linux-x64" -outputDirectory "Publish/ui/linux-x64"
    Publish-UI -runtime "osx-x64" -outputDirectory "Publish/ui/macos-x64"

    Publish-Service -runtime "win-x64" -outputDirectory "Publish/service/win-x64"
    Publish-Service -runtime "linux-x64" -outputDirectory "Publish/service/linux-x64"
    Publish-Service -runtime "osx-x64" -outputDirectory "Publish/service/macos-x64"

    
    Write-Output "正在发布UI：Browser"
    dotnet publish FrpGUI.Avalonia.Browser -r browser-wasm -c Release -o "Publish/ui/browser" --self-contained true

    Write-Output "正在清理"
    Remove-Item FrpGUI*/bin/Release -Recurse

    Write-Output "操作完成"

    Invoke-Item Publish
    pause
}
catch {
    Write-Error $_
}
