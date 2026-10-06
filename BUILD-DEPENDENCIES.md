# 构建依赖说明

本仓库包含 x64 构建所需的全部源码、静态库与 OCR 模型。但有两类**大体积二进制依赖未纳入 git**，需要从 Release 附件下载后放回原位，才能完成完整构建。

## 快速还原

从 [最新 Release](https://github.com/SeanWang114514/PP-OCRv6-Tools/releases/latest) 下载对应附件：

| 附件 | 大小 | 用途 |
|------|------|------|
| `PP-OCRv6-build-deps-mtran.zip` | 约 117MB | 翻译引擎（内嵌进 exe，构建必需） |
| `PP-OCRv6-build-deps-arm64.zip` | 约 23MB | ARM64 构建依赖（仅构建 ARM64 版时需要） |

### 还原翻译引擎依赖（x64 构建必需）

解压 `PP-OCRv6-build-deps-mtran.zip` 到 `translation_assets/`，最终结构应为：

```
translation_assets/
├── mtranserver.exe
├── mtran_models/
│   ├── en_zh-Hans/
│   └── zh-Hans_en/
├── mtran_config/
│   └── records.json
└── translator.rc
```

> `translator.rc` 会把上述文件以 RCDATA 资源形式内嵌进 exe（资源 ID 100~108），
> 程序首次运行时释放到 `%LOCALAPPDATA%\PP-OCRv6 Desktop\`。因此缺少这些文件时链接阶段会失败。

### 还原 ARM64 依赖（仅 ARM64 构建需要）

解压 `PP-OCRv6-build-deps-arm64.zip` 到 `third_party/`，最终结构应为：

```
third_party/
├── ncnn-arm64/
└── opencv-arm64/
```

## 已纳入 git 的依赖（无需额外下载）

| 路径 | 说明 |
|------|------|
| `third_party/ncnn/` | ncnn x64 静态库（含 CMake config） |
| `third_party/opencv/` | OpenCV 4.11 x64 静态库 |
| `models/ppocrv6_tiny/` | PP-OCRv6 Tiny 检测 / 识别 / 方向模型 + 字典 |
| `ppocrv6_engine/3rdparty/clipper2/` | 多边形裁剪库源码 |

## 构建

前置条件：Windows x64 + Visual Studio（需「使用 C++ 的桌面开发」工作负载）+ CMake 3.21+。

```bat
build_native.bat
```

脚本会通过 `vswhere` 自动定位 Visual Studio，配置 CMake 并编译出
`out/Release/ChineseOCRLiteDesktop.exe`。构建后会自动复制 `models/` 与
`ppocrv6_config.json` 到产物目录。

### 验证

```bat
out\Release\ChineseOCRLiteDesktop.exe --selftest
```

返回 `0` 表示 OCR 模型加载与中英互译均正常。

## 未被引用的目录

`translation_assets/m2m100/` 与 `translation_assets/CrispASR/` 是早期实验遗留，
当前代码与构建流程均未引用，可安全删除。
