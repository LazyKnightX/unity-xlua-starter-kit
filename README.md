# Unity xLua Starter Kit

基于 xLua 2.1.16 构建的 Starter Kit 。

*注：这是一个学习仓库，如有任何问题，欢迎提交issue！*

* xLua: 2.1.16 (标准版, Lua 5.4)
* Unity: 2022.3.63f3

## 特点

* 支持 *.lua */init.lua 自动匹配
* 支持从 `Assets/LuaScripts` / `StreamingAssets` 加载代码
* 针对 Android 平台自动处理 apk 内 StreamingAssets 路径转换
* 支持从热更目录（`Application.persistentDataPath`）加载代码
* `awake/start/update/ondestroy` 对应 XLuaManager 事件
* 加载顺序：
    1. 热更目录
    2. StreamingAssets
    3. StreamingAssets (Android)
    4. Editor 下 Assets/LuaScripts

## 基础模块 (C#)

* Assets/_DEMO/Scripts/Demo
    * XLuaDemo.cs - 基础演示
    * XLuaManager.cs - 带有CustomLoader的演示
* Assets/_DEMO/Scripts/Demo
    * Editor/DemoGenConfig.cs - 基础的 Generate Code 配置

## 基础模块 (Lua)

* types/
    * types.msg.lua
* libs/extensions/
    * extend_table.lua
* libs/utils/
    * log.lua
    * msg.lua
* unity/
    * player-loop.lua
* main.lua
* test.lua

## 开始使用

1. `git clone https://github.com/LazyKnightX/unity-xlua-starter-kit.git`
2. 打开项目
3. 打开 GameScene.unity
4. 运行并测试

## LICENSE

```
The MIT License (MIT)

Copyright (C) 2026 Lazy Knight (i@lazyknight.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
```
