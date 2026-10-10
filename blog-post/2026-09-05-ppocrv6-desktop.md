---
layout: post
title: "PP-OCRv6 Desktop：一个纯 C++ 的离线 OCR + 翻译桌面工具"
date: 2026-09-05 23:00:00 +0800
tags: [AI, 工具, C++, OCR, PaddleOCR]
categories: [工具]
description: 一个原生 Win32 C++ 应用，截图即识别、即翻译，无需联网，无需 Python，单文件直接运行。
---

> 一个原生 Win32 C++ 应用，截图即识别、即翻译，无需联网，无需 Python，单文件直接运行。

<!--more-->

## 为什么做这个

市面上的 OCR 工具大多依赖 Python 运行时，装一堆依赖才能跑起来；在线 API 又要联网、要注册、有调用限制。我只是想快速截图识别一段文字、翻译成英文，但每次都要打开浏览器或者等 Python 环境加载，体验并不流畅。

于是我做了一个 **PP-OCRv6 Desktop**：一个纯 C++ 编写的 Windows 桌面工具，截图 → OCR → 翻译，全程离线，双击即用。

<div style="text-align: center; margin: 2rem 0;">
  <svg width="640" height="360" viewBox="0 0 640 360" xmlns="http://www.w3.org/2000/svg">
    <defs>
      <linearGradient id="bg" x1="0%" y1="0%" x2="100%" y2="100%">
        <stop offset="0%" style="stop-color:#667eea;stop-opacity:1" />
        <stop offset="100%" style="stop-color:#764ba2;stop-opacity:1" />
      </linearGradient>
      <linearGradient id="card" x1="0%" y1="0%" x2="0%" y2="100%">
        <stop offset="0%" style="stop-color:#ffffff;stop-opacity:1" />
        <stop offset="100%" style="stop-color:#f8f9fa;stop-opacity:1" />
      </linearGradient>
      <filter id="shadow" x="-5%" y="-5%" width="110%" height="120%">
        <feDropShadow dx="0" dy="4" stdDeviation="8" flood-color="#000" flood-opacity="0.15"/>
      </filter>
    </defs>
    <!-- Background -->
    <rect width="640" height="360" rx="16" fill="url(#bg)"/>
    <!-- Main card -->
    <rect x="40" y="30" width="560" height="300" rx="12" fill="url(#card)" filter="url(#shadow)"/>
    <!-- Title -->
    <text x="320" y="80" text-anchor="middle" font-family="system-ui, -apple-system, sans-serif" font-size="28" font-weight="bold" fill="#1a1a2e">PP-OCRv6 Desktop</text>
    <text x="320" y="110" text-anchor="middle" font-family="system-ui, -apple-system, sans-serif" font-size="14" fill="#666">离线 OCR + 中英翻译 · 纯 C++ · 单文件运行</text>
    <!-- Three feature boxes -->
    <!-- Screenshot -->
    <rect x="70" y="140" width="150" height="100" rx="10" fill="#f0f4ff" stroke="#667eea" stroke-width="1.5"/>
    <text x="145" y="175" text-anchor="middle" font-size="28">📸</text>
    <text x="145" y="200" text-anchor="middle" font-family="system-ui" font-size="13" font-weight="600" fill="#1a1a2e">截图识别</text>
    <text x="145" y="218" text-anchor="middle" font-family="system-ui" font-size="10" fill="#888">Ctrl+Alt+S 一键框选</text>
    <!-- OCR -->
    <rect x="245" y="140" width="150" height="100" rx="10" fill="#f0fff4" stroke="#38a169" stroke-width="1.5"/>
    <text x="320" y="175" text-anchor="middle" font-size="28">🔍</text>
    <text x="320" y="200" text-anchor="middle" font-family="system-ui" font-size="13" font-weight="600" fill="#1a1a2e">OCR 识别</text>
    <text x="320" y="218" text-anchor="middle" font-family="system-ui" font-size="10" fill="#888">PP-OCRv6 Tiny 引擎</text>
    <!-- Translate -->
    <rect x="420" y="140" width="150" height="100" rx="10" fill="#fff5f5" stroke="#e53e3e" stroke-width="1.5"/>
    <text x="495" y="175" text-anchor="middle" font-size="28">🌐</text>
    <text x="495" y="200" text-anchor="middle" font-family="system-ui" font-size="13" font-weight="600" fill="#1a1a2e">中英翻译</text>
    <text x="495" y="218" text-anchor="middle" font-family="system-ui" font-size="10" fill="#888">MTranServer 本机翻译</text>
    <!-- Arrows -->
    <path d="M 220 190 L 245 190" stroke="#667eea" stroke-width="2" fill="none" marker-end="url(#arrow)"/>
    <path d="M 395 190 L 420 190" stroke="#38a169" stroke-width="2" fill="none" marker-end="url(#arrow2)"/>
    <defs>
      <marker id="arrow" markerWidth="8" markerHeight="6" refX="8" refY="3" orient="auto">
        <polygon points="0 0, 8 3, 0 6" fill="#667eea"/>
      </marker>
      <marker id="arrow2" markerWidth="8" markerHeight="6" refX="8" refY="3" orient="auto">
        <polygon points="0 0, 8 3, 0 6" fill="#38a169"/>
      </marker>
    </defs>
    <!-- Bottom text -->
    <text x="320" y="285" text-anchor="middle" font-family="system-ui" font-size="12" fill="#999">无需 Python · 无需联网 · 单文件 250MB · 双击即用</text>
    <!-- Stars decoration -->
    <text x="70" y="290" font-size="12" fill="#667eea" opacity="0.6">✦</text>
    <text x="560" y="290" font-size="12" fill="#764ba2" opacity="0.6">✦</text>
  </svg>
</div>

## 功能一览

这个工具麻雀虽小，五脏俱全：

| 功能 | 说明 |
|------|------|
| 📸 **截图识别** | 全局快捷键 `Ctrl + Alt + S` 一键截图，框选区域后自动异步 OCR |
| 🌐 **中英翻译** | 内置 MTranServer，支持中→英 / 英→中翻译，源语言自动检测 |
| 🎨 **Apple HIG 界面** | 遵循 Apple Human Interface Guidelines，Light / Dark 双主题自动跟随系统 |
| 🔲 **托盘常驻** | 关闭主窗口最小化到托盘，双击图标恢复 |
| ⚙️ **自定义快捷键** | 可自由修改截图快捷键 |
| 🚀 **开机自启动** | 可选开机自启动 + 启动后隐藏到托盘 |
| 🧹 **快速清理** | 识别结果支持一键删除空格 / 删除换行 |
| 💾 **窗口记忆** | 记住主窗口 / 设置窗口 / 结果窗口的大小与位置 |
| 🧠 **内存优化** | 隐藏到托盘或空闲 60 秒自动释放工作集 |

其中最值得一提的是 **Apple HIG 界面**。作为一个 Windows 工具，我选择参考 Apple 的 Human Interface Guidelines 来设计 UI——圆角卡片、分组背景、扁平化控件，整体视觉干净利落。Light / Dark 双主题自动跟随系统切换，不需要手动设置。

<div style="text-align: center; margin: 2rem 0;">
  <svg width="600" height="200" viewBox="0 0 600 200" xmlns="http://www.w3.org/2000/svg">
    <defs>
      <linearGradient id="lightBg" x1="0%" y1="0%" x2="0%" y2="100%">
        <stop offset="0%" style="stop-color:#fafafa"/>
        <stop offset="100%" style="stop-color:#f0f0f0"/>
      </linearGradient>
      <linearGradient id="darkBg" x1="0%" y1="0%" x2="0%" y2="100%">
        <stop offset="0%" style="stop-color:#2d2d2d"/>
        <stop offset="100%" style="stop-color:#1a1a1a"/>
      </linearGradient>
    </defs>
    <!-- Light theme card -->
    <rect x="10" y="10" width="280" height="180" rx="12" fill="url(#lightBg)" stroke="#e0e0e0" stroke-width="1"/>
    <rect x="20" y="20" width="260" height="30" rx="6" fill="#e8e8e8"/>
    <text x="30" y="40" font-family="system-ui" font-size="11" fill="#666">Light Theme</text>
    <circle cx="260" cy="35" r="8" fill="#34c759"/>
    <rect x="20" y="60" width="260" height="60" rx="8" fill="#fff" stroke="#e0e0e0"/>
    <text x="35" y="85" font-family="system-ui" font-size="12" fill="#333">识别结果将显示在这里</text>
    <rect x="20" y="130" width="120" height="36" rx="18" fill="#667eea"/>
    <text x="80" y="153" text-anchor="middle" font-family="system-ui" font-size="11" fill="#fff" font-weight="500">复制文本</text>
    <rect x="150" y="130" width="120" height="36" rx="18" fill="#f0f0f0" stroke="#ccc"/>
    <text x="210" y="153" text-anchor="middle" font-family="system-ui" font-size="11" fill="#666">翻译</text>
    <text x="150" y="195" text-anchor="middle" font-family="system-ui" font-size="10" fill="#999">☀️ Light Mode</text>

    <!-- Dark theme card -->
    <rect x="310" y="10" width="280" height="180" rx="12" fill="url(#darkBg)" stroke="#444" stroke-width="1"/>
    <rect x="320" y="20" width="260" height="30" rx="6" fill="#3a3a3a"/>
    <text x="330" y="40" font-family="system-ui" font-size="11" fill="#aaa">Dark Theme</text>
    <circle cx="560" cy="35" r="8" fill="#30d158"/>
    <rect x="320" y="60" width="260" height="60" rx="8" fill="#2a2a2a" stroke="#444"/>
    <text x="335" y="85" font-family="system-ui" font-size="12" fill="#e0e0e0">识别结果将显示在这里</text>
    <rect x="320" y="130" width="120" height="36" rx="18" fill="#667eea"/>
    <text x="380" y="153" text-anchor="middle" font-family="system-ui" font-size="11" fill="#fff" font-weight="500">复制文本</text>
    <rect x="450" y="130" width="120" height="36" rx="18" fill="#3a3a3a" stroke="#555"/>
    <text x="510" y="153" text-anchor="middle" font-family="system-ui" font-size="11" fill="#ccc">翻译</text>
    <text x="450" y="195" text-anchor="middle" font-family="system-ui" font-size="10" fill="#777">🌙 Dark Mode</text>
  </svg>
</div>

## 技术架构

整个工具分为两个核心引擎：**OCR 引擎**和**翻译引擎**，它们通过简洁的架构协同工作。

<div style="text-align: center; margin: 2rem 0;">
  <svg width="640" height="320" viewBox="0 0 640 320" xmlns="http://www.w3.org/2000/svg">
    <defs>
      <linearGradient id="headerGrad" x1="0%" y1="0%" x2="100%" y2="0%">
        <stop offset="0%" style="stop-color:#667eea"/>
        <stop offset="100%" style="stop-color:#764ba2"/>
      </linearGradient>
    </defs>
    <!-- Main container -->
    <rect x="20" y="10" width="600" height="300" rx="12" fill="#fff" stroke="#e0e0e0" stroke-width="1.5"/>
    <!-- Header -->
    <rect x="20" y="10" width="600" height="44" rx="12" fill="url(#headerGrad)"/>
    <rect x="20" y="42" width="600" height="12" fill="url(#headerGrad)"/>
    <text x="320" y="38" text-anchor="middle" font-family="system-ui" font-size="15" fill="#fff" font-weight="600">PP-OCRv6 Desktop · 技术架构</text>

    <!-- OCR Engine box -->
    <rect x="40" y="70" width="260" height="220" rx="10" fill="#f8f9ff" stroke="#667eea" stroke-width="1.5"/>
    <text x="170" y="95" text-anchor="middle" font-family="system-ui" font-size="13" font-weight="600" fill="#667eea">🔍 OCR 引擎</text>

    <!-- Step 1 -->
    <rect x="55" y="108" width="230" height="36" rx="6" fill="#fff" stroke="#e0e0e0"/>
    <circle cx="72" cy="126" r="10" fill="#667eea"/>
    <text x="72" y="130" text-anchor="middle" font-family="system-ui" font-size="10" fill="#fff" font-weight="bold">1</text>
    <text x="90" y="130" font-family="system-ui" font-size="11" fill="#333">文字检测 (DBNet)</text>
    <text x="275" y="130" text-anchor="end" font-family="system-ui" font-size="9" fill="#999">det</text>

    <!-- Arrow -->
    <line x1="170" y1="148" x2="170" y2="158" stroke="#667eea" stroke-width="1.5" marker-end="url(#arrowBlue)"/>

    <!-- Step 2 -->
    <rect x="55" y="155" width="230" height="36" rx="6" fill="#fff" stroke="#e0e0e0"/>
    <circle cx="72" cy="173" r="10" fill="#667eea"/>
    <text x="72" y="177" text-anchor="middle" font-family="system-ui" font-size="10" fill="#fff" font-weight="bold">2</text>
    <text x="90" y="177" font-family="system-ui" font-size="11" fill="#333">方向分类 (LCNet)</text>
    <text x="275" y="177" text-anchor="end" font-family="system-ui" font-size="9" fill="#999">cls</text>

    <!-- Arrow -->
    <line x1="170" y1="195" x2="170" y2="205" stroke="#667eea" stroke-width="1.5" marker-end="url(#arrowBlue)"/>

    <!-- Step 3 -->
    <rect x="55" y="202" width="230" height="36" rx="6" fill="#fff" stroke="#e0e0e0"/>
    <circle cx="72" cy="220" r="10" fill="#667eea"/>
    <text x="72" y="224" text-anchor="middle" font-family="system-ui" font-size="10" fill="#fff" font-weight="bold">3</text>
    <text x="90" y="224" font-family="system-ui" font-size="11" fill="#333">文字识别 (CRNN)</text>
    <text x="275" y="224" text-anchor="end" font-family="system-ui" font-size="9" fill="#999">rec</text>

    <!-- Result -->
    <rect x="55" y="250" width="230" height="28" rx="6" fill="#667eea" opacity="0.1"/>
    <text x="170" y="269" text-anchor="middle" font-family="system-ui" font-size="11" fill="#667eea" font-weight="500">📝 识别结果文本</text>

    <!-- Arrow between engines -->
    <defs>
      <marker id="arrowBlue" markerWidth="7" markerHeight="5" refX="7" refY="2.5" orient="auto">
        <polygon points="0 0, 7 2.5, 0 5" fill="#667eea"/>
      </marker>
      <marker id="arrowGreen" markerWidth="7" markerHeight="5" refX="7" refY="2.5" orient="auto">
        <polygon points="0 0, 7 2.5, 0 5" fill="#38a169"/>
      </marker>
    </defs>
    <path d="M 300 264 L 340 264 L 340 180 L 340 150" stroke="#38a169" stroke-width="2" fill="none" stroke-dasharray="5,3" marker-end="url(#arrowGreen)"/>
    <text x="320" y="250" text-anchor="middle" font-family="system-ui" font-size="9" fill="#38a169">HTTP</text>

    <!-- Translation Engine box -->
    <rect x="340" y="70" width="260" height="220" rx="10" fill="#f0fff4" stroke="#38a169" stroke-width="1.5"/>
    <text x="470" y="95" text-anchor="middle" font-family="system-ui" font-size="13" font-weight="600" fill="#38a169">🌐 翻译引擎</text>

    <rect x="355" y="108" width="230" height="36" rx="6" fill="#fff" stroke="#e0e0e0"/>
    <text x="470" y="130" text-anchor="middle" font-family="system-ui" font-size="11" fill="#333">MTranServer</text>

    <rect x="355" y="155" width="230" height="36" rx="6" fill="#fff" stroke="#e0e0e0"/>
    <text x="470" y="177" text-anchor="middle" font-family="system-ui" font-size="11" fill="#333">localhost:8989/translate</text>

    <rect x="355" y="202" width="230" height="36" rx="6" fill="#fff" stroke="#e0e0e0"/>
    <text x="470" y="224" text-anchor="middle" font-family="system-ui" font-size="11" fill="#333">中→英 / 英→中 自动检测</text>

    <rect x="355" y="250" width="230" height="28" rx="6" fill="#38a169" opacity="0.1"/>
    <text x="470" y="269" text-anchor="middle" font-family="system-ui" font-size="11" fill="#38a169" font-weight="500">🌍 翻译结果</text>

    <!-- Bottom labels -->
    <text x="170" y="305" text-anchor="middle" font-family="system-ui" font-size="10" fill="#999">ncnn 20241226 · OpenCV 4.11 · Clipper2</text>
    <text x="470" y="305" text-anchor="middle" font-family="system-ui" font-size="10" fill="#999">MTranServer (内嵌 HTTP 服务)</text>
  </svg>
</div>

### OCR 流程

OCR 引擎基于 PP-OCRv6 Tiny 模型，通过 ncnn 框架在 C++ 中直接推理，三步完成文字识别：

1. **文字检测** — `PP_OCRv6_tiny_det`（DBNet）定位图片中的文字区域
2. **方向分类** — `PP_LCNet_x0_25_textline_ori` 判断文字是正向还是需要旋转
3. **文字识别** — `PP_OCRv6_tiny_rec`（CRNN）将文字区域转换为文本字符串

### 翻译流程

识别完成后，文本通过 HTTP POST 发送到本机的 MTranServer（`localhost:8989/translate`），由翻译引擎完成中英互译。整个过程无需联网，所有数据都在本机处理。

## 从源码构建

### 前置条件

- Windows x64 系统
- Visual Studio 2022（需要 MSVC + CMake 支持）

### 一键构建

```bat
build_native.bat
```

构建脚本会自动完成以下步骤：

1. 调用 VS2022 开发者命令行
2. 运行 CMake 配置（Release 模式）
3. 编译生成 `ChineseOCRLiteDesktop.exe`

CMake 构建后会自动将 `models/` 和 `ppocrv6_config.json` 复制到产物目录，无需手动操作。

### 依赖说明

好消息是，所有核心依赖都已经包含在仓库中：

- **ncnn 20241226** 和 **OpenCV 4.11** 静态库在 `third_party/` 目录
- **Clipper2** 多边形裁剪库在 `ppocrv6_engine/3rdparty/clipper2/`
- 翻译资源（`mtranserver.exe`、模型、配置）已内置于发布版 exe

你只需要一个 VS2022，就能从源码构建出完整的工具。

## 自检功能

工具内置了自检命令，方便验证安装是否正常：

```bat
ChineseOCRLiteDesktop.exe --selftest
```

退出码含义：

| 退出码 | 含义 |
|--------|------|
| `0` | ✅ OCR 模型加载 + 中英翻译均正常 |
| `3` | ❌ OCR 模型文件缺失 |
| `4` | ❌ OCR 引擎初始化失败 |
| `5` | ❌ 翻译引擎启动失败 |
| `6` | ❌ 翻译结果为空 |

## 使用方式

### 下载

**下载地址：[GitHub Releases](https://github.com/SeanWang114514/PP-OCRv6-Desktop/releases)**

下载 `ChineseOCRLiteDesktop.exe` 即可直接使用——单文件、自包含、无需安装，约 250MB。

### 快速上手

1. 双击运行 `ChineseOCRLiteDesktop.exe`
2. 按 `Ctrl + Alt + S` 截图（可在设置中修改快捷键）
3. 框选需要识别的区域
4. 等待 OCR 识别完成
5. 查看结果，支持一键翻译

### 使用场景

- 📖 阅读外文文档时快速翻译
- 📋 从图片中提取文字，告别手打
- 🔬 查看论文截图中的公式和数据
- 🌏 翻译软件界面、菜单、报错信息
- 📝 快速记录白板、PPT 上的内容

## 项目结构

```
PP-OCRv6-Desktop/
├── src/                          # 主程序源码
│   └── main.cpp                  # Win32 GUI + OCR + 翻译 + 托盘
├── ppocrv6_engine/               # PP-OCRv6 ncnn C++ 推理引擎
│   ├── ocr_engine.cpp/h          # OCR 引擎入口
│   ├── db_net.cpp/h              # 文字检测网络
│   ├── angle_net.cpp/h           # 方向分类网络
│   ├── crnn_net.cpp/h            # 文字识别网络
│   └── 3rdparty/                 # Clipper2 + plog
├── models/                       # OCR 模型文件
├── third_party/                  # 预编译静态库 (ncnn, OpenCV)
├── CMakeLists.txt                # CMake 构建配置
└── build_native.bat              # 一键构建脚本
```

## 开源与致谢

本项目站在这些优秀开源项目的肩膀上：

- [PaddleOCR](https://github.com/PaddlePaddle/PaddleOCR) — PP-OCR 系列模型
- [Tencent/ncnn](https://github.com/Tencent/ncnn) — 高性能神经网络推理框架
- [Avafly/PaddleOCR-ncnn-CPP](https://github.com/Avafly/PaddleOCR-ncnn-CPP) — PP-OCRv6 ncnn C++ 推理集成参考
- [OpenCV](https://opencv.org/) — 计算机视觉库
- [Clipper2](http://www.angusj.com/clipper2/) — 多边形裁剪库

<div style="text-align: center; margin: 2rem 0;">
  <svg width="500" height="120" viewBox="0 0 500 120" xmlns="http://www.w3.org/2000/svg">
    <rect x="0" y="0" width="500" height="120" rx="12" fill="#f8f9ff" stroke="#e0e0e0"/>
    <text x="250" y="30" text-anchor="middle" font-family="system-ui" font-size="13" font-weight="600" fill="#333">🔗 相关链接</text>
    <text x="250" y="55" text-anchor="middle" font-family="system-ui" font-size="11" fill="#667eea">项目地址：github.com/SeanWang114514/PP-OCRv6-Desktop</text>
    <text x="250" y="75" text-anchor="middle" font-family="system-ui" font-size="11" fill="#667eea">下载地址：github.com/SeanWang114514/PP-OCRv6-Desktop/releases</text>
    <text x="250" y="100" text-anchor="middle" font-family="system-ui" font-size="10" fill="#999">源码遵循上游项目的开源协议 · 模型遵循 PaddlePaddle 许可条款</text>
  </svg>
</div>

## 写在最后

做这个工具的初衷很简单：我想要一个**打开就能用**的 OCR + 翻译工具。不需要装 Python，不需要配置环境，不需要联网，不需要注册账号。双击 exe，截图，识别，翻译，完成。

如果你也有类似的需求，欢迎去 [GitHub](https://github.com/SeanWang114514/PP-OCRv6-Desktop) 下载体验，也欢迎提 issue 或 star 支持一下。
