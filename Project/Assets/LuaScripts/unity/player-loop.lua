local utils = utils
local msg = utils.msg

function awake()
    msg.notify('unity_awake')
end

function start()
    msg.notify('unity_start')
end

function update()
    msg.notify('unity_update')
end

function ondestroy()
    msg.notify('unity_destroy')
end
