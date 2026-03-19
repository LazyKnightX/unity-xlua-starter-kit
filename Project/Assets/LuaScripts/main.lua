-- main.lua
print('xLua 演示启动！当前版本：' .. _VERSION)

-- 安全访问标记符
-- eg. alloc_unit(((data or E).config or E).gid or 'u000', pid)
---@type table
E = setmetatable({}, {
    __newindex = function(_, v)
        utils.log.log_2error('禁止修改 `E` ！')
    end
})

-- 空函数占位符
---@type fun(...): ...
F = function(...) return ... end

---@class utils
utils = {}

require 'libs.extensions.extend_table'
utils.log = require 'libs.utils.log'
utils.msg = require 'libs.utils.msg'

require 'unity.player-loop'
require 'test'
