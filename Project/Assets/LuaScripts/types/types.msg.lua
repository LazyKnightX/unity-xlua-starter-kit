local utils = utils

---@class msg_util
---@field on fun(msgtype: 'unity_awake', action: fun(), auto_drop: boolean?): msg_info
---@field on fun(msgtype: 'unity_start', action: fun(), auto_drop: boolean?): msg_info
---@field on fun(msgtype: 'unity_update', action: fun(), auto_drop: boolean?): msg_info
---@field on fun(msgtype: 'unity_destroy', action: fun(), auto_drop: boolean?): msg_info
---@field notify fun(msgtype: 'unity_awake', args: ...)
---@field notify fun(msgtype: 'unity_start', args: ...)
---@field notify fun(msgtype: 'unity_update', args: ...)
---@field notify fun(msgtype: 'unity_destroy', args: ...)
local msg = utils.msg
