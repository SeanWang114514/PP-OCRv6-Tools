# PP-OCRv6 Desktop

**离线 OCR + 中英翻译的 Windows 桌面工具**

一个原生 Win32 C++ x64 应用，无需 Python、PySide 或任何运行时依赖。OCR 引擎基于 PP-OCRv6 Tiny 的 ncnn C++ 推理实现，翻译由内嵌的 MTranServer 在本机完成。

> 🔽 最新版可执行文件发布在 **[GitHub Releases](https://github.com/SeanWang114514/PP-OCRv6-Desktop/releases)**，下载 `ChineseOCRLiteDesktop.exe` 即可直接使用（单文件、自包含、无需安装，约 250MB）。

---

## ✨ 功能

| 功能 | 说明 |
|------|------|
| 📸 **截图识别** | 全局快捷键（默认 `Ctrl + Alt + S`）一键截图，框选区域后自动异步 OCR |
| 🌐 **中英翻译** | 内置 MTranServer，支持中→英 / 英→中翻译，源语言自动检测 |
| 🎨 **Apple HIG 界面** | 遵循 Apple Human Interface Guidelines，Light / Dark 双主题自动跟随系统 |
| 🔲 **托盘常驻** | 关闭主窗口最小化到托盘，双击图标恢复 |
| ⚙️ **自定义快捷键** | 可自由修改截图快捷键 |
| 🚀 **开机自启动** | 可选开机自启动 + 启动后隐藏到托盘 |
| 🧹 **快速清理** | 识别结果支持一键删除空格 / 删除换行 |
| 💾 **窗口记忆** | 记住主窗口 / 设置窗口 / 结果窗口的大小与位置 |
| 🧠 **内存优化** | 隐藏到托盘或空闲 60 秒自动释放工作集 |

---

## 🏗️ 技术架构

```
┌─────────────────────────────────────────────────┐
│                PP-OCRv6 Desktop                  │
│                 (Win32 GUI)                      │
├──────────────┬──────────────────────────────────┤
│  OCR 引擎    │         翻译引擎                  │
│  PP-OCRv6    │       MTranServer                │
│  ncnn C++    │     (内嵌 HTTP 服务)              │
│  OpenCV 4.11 │    localhost:8989                 │
├──────────────┴──────────────────────────────────┤
│           第三方静态库                            │
│  ncnn 20241226 · OpenCV 4.11 · Clipper2          │
└─────────────────────────────────────────────────┘
```

### OCR 流程

1. **文字检测** — `PP_OCRv6_tiny_det` (DBNet) 定位文字区域
2. **方向分类** — `PP_LCNet_x0_25_textline_ori` 判断文字方向
3. **文字识别** — `PP_OCRv6_tiny_rec` (CRNN) 识别文字内容

### 翻译流程

截图识别 → 获取文字 → HTTP POST `localhost:8989/translate` → MTranServer 翻译 → 返回结果

---

## 📦 内置模型

| 模型 | 用途 |
|------|------|
| `PP_OCRv6_tiny_det` | 文字检测 |
| `PP_OCRv6_tiny_rec` | 文字识别 |
| `PP_LCNet_x0_25_textline_ori` | 文字方向分类 |
| `ppocr_keys_v6.txt` | 识别字典 |

模型文件首次运行时会从 exe 内释放到 `%LOCALAPPDATA%\PP-OCRv6 Desktop\models\`。

---

## 🔨 从源码构建

### 前置条件

- Windows x64
- Visual Studio 2022（MSVC + CMake 支持）

### 构建步骤

```bat
build_native.bat
```

该脚本会：
1. 调用 VS2022 开发者命令行
2. 运行 CMake 配置（Release 模式）
3. 编译生成 `ChineseOCRLiteDesktop.exe`

CMake 构建后会自动将 `models/` 和 `ppocrv6_config.json` 复制到产物目录。

### 依赖说明

- **ncnn 20241226** 和 **OpenCV 4.11** 静态库已包含在仓库的 `third_party/` 目录中
- **Clipper2** 多边形裁剪库在 `ppocrv6_engine/3rdparty/clipper2/` 中
- 翻译资源（`mtranserver.exe`、`mtran_models/`、`mtran_config/`）体积较大未纳入 git，
  需从 [Release 附件](https://github.com/SeanWang114514/PP-OCRv6-Tools/releases/latest)
  下载 `PP-OCRv6-build-deps-mtran.zip` 解压回 `translation_assets/`

> 完整的依赖还原步骤见 **[BUILD-DEPENDENCIES.md](BUILD-DEPENDENCIES.md)**。

---

## 🧪 自检

```bat
ChineseOCRLiteDesktop.exe --selftest
```

退出码：
- `0` — OCR 模型加载 + 中英翻译均正常
- `3` — OCR 模型文件缺失
- `4` — OCR 引擎初始化失败
- `5` — 翻译引擎启动失败
- `6` — 翻译结果为空

---

## 📁 项目结构

```
.
├── src/                          # 主程序源码
│   └── main.cpp                  # Win32 GUI + OCR 调用 + 翻译 + 托盘
├── ppocrv6_engine/               # PP-OCRv6 ncnn C++ 推理引擎
│   ├── ocr_engine.cpp/h          # OCR 引擎入口
│   ├── db_net.cpp/h              # 文字检测网络
│   ├── angle_net.cpp/h           # 方向分类网络
│   ├── crnn_net.cpp/h            # 文字识别网络
│   ├── utils.cpp/h               # 工具函数
│   ├── config.h                  # 配置结构
│   ├── common.h                  # 公共数据结构
│   └── 3rdparty/                 # Clipper2 + plog
├── models/                       # OCR 模型文件
│   └── ppocrv6_tiny/             # PP-OCRv6 Tiny 模型
├── third_party/                  # 预编译静态库
│   ├── ncnn/                     # ncnn x64
│   ├── ncnn-arm64/               # ncnn ARM64
│   └── opencv/                   # OpenCV x64
├── translation_assets/           # 翻译引擎资源（未纳入 git）
├── CMakeLists.txt                # CMake 构建配置
├── ppocrv6_config.json           # OCR 引擎配置
├── build_native.bat              # 一键构建脚本
└── deploy_verify.ps1             # 自动化验证脚本
```

---

## 📜 已完成验证

```
CMake + MSVC x64 Release 构建：PASS
--selftest（OCR 模型加载 + 英→中 / 中→英翻译）：PASS（exit 0）
GUI 保活测试：PASS
结果弹窗自动化验证（deploy_verify.ps1）：PASS
```

---

## 📄 来源与许可

本项目基于以下开源项目：

- [PaddleOCR](https://github.com/PaddlePaddle/PaddleOCR) — PP-OCR 系列模型
- [Tencent/ncnn](https://github.com/Tencent/ncnn) — 高性能神经网络推理框架
- [Avafly/PaddleOCR-ncnn-CPP](https://github.com/Avafly/PaddleOCR-ncnn-CPP) — PP-OCRv6 ncnn C++ 推理集成参考
- [OpenCV](https://opencv.org/) — 计算机视觉库
- [Clipper2](http://www.angusj.com/clipper2/) — 多边形裁剪库

再分发时请保留相应上游许可证。

---

## 📝 许可证

本项目源码遵循上游项目的开源协议。PP-OCR 模型遵循 [PaddlePaddle 的许可条款](https://github.com/PaddlePaddle/PaddleOCR/blob/main/LICENSE)。
