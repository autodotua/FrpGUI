try {
    try {
        dotnet
    }
    catch {
        throw "未安装.NET SDK"
    }
    
    Clear-Host
   
    Write-Output "正在发布win-x64"
    dotnet publish FrpGUI.Avalonia.Desktop -r win-x64 -c Release -o Publish/win-x64 --self-contained true /p:PublishSingleFile=true 
    Copy-Item bin/* Publish/win-x64 -Recurse
    
    Write-Output "正在发布linux-x64"
    dotnet publish FrpGUI.Avalonia.Desktop -r linux-x64 -c Release -o Publish/linux-x64 --self-contained true /p:PublishSingleFile=true 
    Copy-Item bin/* Publish/linux-x64 -Recurse

    Write-Output "正在清理"
    Remove-Item FrpGUI*/bin/Release -Recurse

    Write-Output "操作完成"

    Invoke-Item Publish
    pause
}
catch {
    Write-Error $_
}