# Google Clash of Clans Launcher

这是一个基于Windows Forms的Google Clash of Clans启动器，具有图形化界面和键盘输入模拟功能。

## 功能特点

- 图形化用户界面，方便操作
- 支持自定义文本输入模拟
- 提供固定输入"123"的快捷功能
- 自动检测游戏进程运行状态
- 可激活游戏窗口并将输入发送到游戏中

## 项目结构

```
GoogleClashofClansLauncher/
├── Core/
│   ├── ProcessManager.cs  # 进程管理相关功能
│   └── WindowManager.cs   # 窗口管理相关功能
├── Input/
│   ├── InputSimulator.cs  # 输入模拟基础功能
│   ├── KeyboardSimulator.cs  # 键盘输入模拟
│   └── MouseSimulator.cs  # 鼠标输入模拟
├── Form1.cs              # 主窗口实现
├── Form1.Designer.cs     # 窗口设计器代码
├── Program.cs            # 应用程序入口
└── GoogleClashofClansLauncher.csproj  # 项目配置文件
```

## 使用方法

1. 确保已安装Google Play Games并已安装Clash of Clans游戏
2. 运行本程序
3. 程序会自动检测`crosvm.exe`进程是否运行
4. 在文本框中输入您想要模拟的文本，或直接点击"固定输入123"按钮
5. 点击"模拟输入"按钮，程序会激活游戏窗口并模拟键盘输入

## 注意事项

- 程序需要以管理员权限运行，以确保能够正确地检测进程和模拟输入
- 如果输入模拟没有响应，请检查游戏窗口是否正常打开并且处于焦点状态
- 程序目前仅支持模拟基本的键盘输入功能

## 开发环境

- Visual Studio 2022
- .NET 8.0
- Windows Forms

## 历史更新

- 优化了键盘输入模拟逻辑，减少了输入延迟
- 修复了固定输入功能仅输入"1"的问题
- 添加了进程检测和窗口激活功能
- 将控制台应用转换为图形化窗口应用