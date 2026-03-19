----------------------------------------
-- msg.lua
----------------------------------------
-- 0.1
-- basic & simple message module.
----------------------------------------

-- msg | 事件消息
---@class msg_util
local msg = {}

--msg_list

    local _list = {} ---@type { [string]: msg_info[] }

    ---获取事件消息容器 (按需初始化)
    ---@param msgtype msg_type
    ---@return msg_info[]
    local function _get_list(msgtype)
        if not _list[msgtype] then
            ---@type msg_info[]
            _list[msgtype] = {}
        end

        return _list[msgtype]
    end

--msg_info

    ---构造事件消息实例
    ---@param action function
    ---@param msgtype msg_type
    ---@param auto_drop boolean?
    ---@return msg_info
    local function _alloc_msg_info(action, msgtype, auto_drop)
        ---@type msg_info
        local info = {
            action = action,
            msgtype = msgtype,
            auto_drop = auto_drop,
        }

        return info
    end

--on/off/notify

    ---发送事件消息
    ---@param msgtype msg_type
    ---@param ... any 事件参数
    function msg.notify(msgtype, ...)
        local list = _get_list(msgtype)

        local want_drop = {}

        local n = #list
        for i = 1, n do --先注册的事件先运行，后注册的事件后运行
            local info = list[i] or E
            if info.action then
                info.action(...)

                if info.auto_drop == true then
                    want_drop[info] = true
                end
            end
        end

        --所有事件运行完成后，移除标记了auto_drop的事件
        for i = n, 1, -1 do
            local info = list[i] or E
            if want_drop[info] == true then
                table.remove(list, i)
            end
        end
    end

    ---注册事件消息动作
    ---@param msgtype msg_type
    ---@param action fun(...: any) 响应该事件消息的动作
    ---@param auto_drop? boolean default: nil | true: 运行一次后删除
    ---@return msg_info info 监听器ID，可用于 off
    function msg.on(msgtype, action, auto_drop)
        local info = _alloc_msg_info(action, msgtype, auto_drop)

        local list = _get_list(msgtype)
        table.insert(list, info)

        return info
    end

    ---移除事件消息动作
    ---@param info msg_info
    function msg.off(info)
        local msgtype = info.msgtype

        local list = _get_list(msgtype)
        table.remove_value(list, info)
    end

--

return msg



---@class msg_info
---@field msgtype msg_type
---@field action function
---@field auto_drop boolean?

---@alias msg_type string
