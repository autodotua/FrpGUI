# FRP GUI

一个使用Avalonia开发的FRP（内网端口转发）的Windows/Linux/MacOS GUI程序

![客户端界面](./img/main_window_client.png)

![服务器界面](./img/main_window_server.png)

## 自动构建

1. 准备依赖库
   1. 从[FzLib](https://github.com/autodotua/FzLib)（master_v2分支）编译`FzLib.dll`和`FzLib.Avalonia.dll`，也可下载Release中发布的dll。当前版本对应的`FzLib`的Commit版本为2024年6月29日版本。
2. 下载frp二进制文件
   1. 在解决方案根目录创建`bin`目录
   1. 下载[frp](https://github.com/fatedier/frp/releases)的二进制文件（目前测试通过的是[v0.55.1](https://github.com/fatedier/frp/releases/tag/v0.55.1)，更新版本也可使用），解压后放置到`bin`目录。此时，`bin`目录的结构如下（目录名中版本号任意）：

```
bin
│
├─frp_0.55.1_darwin_amd64
│      frpc
│      frpc.toml
│      frps
│      frps.toml
│      LICENSE
│
├─frp_0.55.1_linux_amd64
│      frpc
│      frpc.toml
│      frps
│      frps.toml
│      LICENSE
│
└─frp_0.55.1_windows_amd64
        frpc.exe
        frpc.toml
        frps.exe
        frps.toml
        LICENSE
```

3. 编译生成
   1. 确保已安装.NET 8 SDK
   2. 执行`build_Avalonia.ps1`PowerShell脚本。该脚本会自动生成win-x64，linux-x64和macos-x64版本。