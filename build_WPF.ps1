try {
    
    Write-Output "请确保："
    Write-Output "已经安装.NET 8 SDK"

    pause
    Clear-Host

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
   
    

    Write-Output "正在发布WPF"
    dotnet publish FrpGUI.WPF -c Release -o Publish/WPF --self-contained false
    Copy-Item bin/* Publish/WPF -Recurse
    
    Write-Output "正在发布WPF_SingleFile（单文件）"
    dotnet publish FrpGUI.WPF -c Release -o Publish/WPF_SingleFile --self-contained false /p:PublishSingleFile=true
    Copy-Item bin/* Publish/WPF_SingleFile -Recurse

    Write-Output "正在发布WPF_Contained_SingleFile（包含框架，单文件）"
    dotnet publish FrpGUI.WPF -c Release -o Publish/WPF_Contained_SingleFile --self-contained true /p:PublishSingleFile=true
    Copy-Item bin/* Publish/WPF_Contained_SingleFile -Recurse

    Write-Output "正在清理"
    Remove-Item FrpGUI*/bin/Release -Recurse

    Write-Output "操作完成"

    Invoke-Item Publish
    pause
}
catch {
    Write-Error $_
}