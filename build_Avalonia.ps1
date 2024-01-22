try {
    try {
        dotnet
    }
    catch {
        throw "未安装.NET SDK"
    }
    
    Clear-Host
    try {
        Remove-Item Publish -Recurse
    }
    catch {
    }
   
    Write-Output "正在发布（包含框架，单文件）"
    dotnet publish FrpGUI.Avalonia.Desktop -r win-x64 -c Release -o Publish/Windows --self-contained true /p:PublishSingleFile=true 
    Copy-Item bin/* Publish/Windows -Recurse
    
    Write-Output "正在发布（包含框架，单文件）"
    dotnet publish FrpGUI.Avalonia.Desktop -r linux-x64 -c Release -o Publish/Linux --self-contained true /p:PublishSingleFile=true 
    Copy-Item bin/* Publish/Linux -Recurse

    Write-Output "正在清理"
    Remove-Item FrpGUI*/bin/Release -Recurse

    Write-Output "操作完成"

    Invoke-Item Publish
    pause
}
catch {
    Write-Error $_
}