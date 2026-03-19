----------------------------------------
-- log.lua
----------------------------------------
-- 0.1
-- basic & simple log module.
----------------------------------------

-- log | 日志
---@class log_util
local log = {}

C_LOG_LEVEL_ALL     = 99
C_LOG_LEVEL_TRACE   = 6     --trace debug info warn error fatal
C_LOG_LEVEL_DEBUG   = 5     --debug info warn error fatal
C_LOG_LEVEL_INFO    = 4     --info warn error fatal
C_LOG_LEVEL_WARN    = 3     --warn error fatal
C_LOG_LEVEL_ERROR   = 2     --error fatal
C_LOG_LEVEL_FATAL   = 1     --fatal
C_LOG_LEVEL_OFF     = 0

local m_log_level = C_LOG_LEVEL_DEBUG

local function _validate_level(level)
    return m_log_level >= level
end

local function _log(...)
    print(...)
end

local function _log_fatal(...)
    if not _validate_level(C_LOG_LEVEL_FATAL) then return end
    _log('[FATAL]', ...)
end

local function _log_error(...)
    if not _validate_level(C_LOG_LEVEL_ERROR) then return end
    _log('[ERROR]', ...)
end

local function _log_warn(...)
    if not _validate_level(C_LOG_LEVEL_WARN) then return end
    _log('[WARN]', ...)
end

local function _log_info(...)
    if not _validate_level(C_LOG_LEVEL_INFO) then return end
    _log('[INFO]', ...)
end

local function _log_debug(...)
    if not _validate_level(C_LOG_LEVEL_DEBUG) then return end
    _log('[DEBUG]', ...)
end

local function _log_trace(...)
    if not _validate_level(C_LOG_LEVEL_TRACE) then return end
    _log('[TRACE]', ...)
end

log.log_1fatal = _log_fatal
log.log_2error = _log_error
log.log_3warn = _log_warn
log.log_4info = _log_info
log.log_5debug = _log_debug
log.log_6trace = _log_trace

return log
