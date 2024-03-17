try {
    # 检查是否安装了.NET SDK
    try {
        dotnet
    }
    catch {
        throw "未安装.NET SDK"
    }
    
    function Publish-Application {
        param (
            [string]$runtime,
            [string]$outputDirectory
        )

        Write-Output "正在发布 $runtime"

        dotnet publish FrpGUI.Avalonia.Desktop -r $runtime -c Release -o $outputDirectory --self-contained true /p:PublishSingleFile=true 

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
    if (Test-Path "Generation/Publish") {
        Remove-Item "Generation/Publish" -Recurse -Force
    }

    Publish-Application -runtime "win-x64" -outputDirectory "Generation/Publish/win-x64"
    Publish-Application -runtime "linux-x64" -outputDirectory "Generation/Publish/linux-x64"
    Publish-Application -runtime "osx-x64" -outputDirectory "Generation/Publish/macos-x64"

    Write-Output "正在清理"
    Remove-Item FrpGUI*/bin/Release -Recurse

    Write-Output "操作完成"

    Invoke-Item Generation/Publish
    pause
}
catch {
    Write-Error $_
}
