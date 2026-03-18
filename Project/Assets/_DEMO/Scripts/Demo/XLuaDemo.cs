using UnityEngine;
using XLua;

public class XLuaDemo : MonoBehaviour
{
    void Start()
    {
        LuaEnv env = new LuaEnv();

        env.DoString("print('xLua ready!')");

        env.DoString(@"
print(_VERSION)          -- 输出类似：Lua 5.4
print(type(_VERSION))    -- 输出：string

-- 如果需要判断版本号（推荐做比较）
local ver = _VERSION     -- 如 'Lua 5.4'
local major, minor = ver:match('Lua (%d+)%.(%d+)')
major = tonumber(major)
minor = tonumber(minor)

if major == 5 and minor >= 4 then
    print('支持 Lua 5.4+ 的新特性（如 utf8、<const> 等）')
elseif major == 5 and minor == 3 then
    print('Lua 5.3，整数支持')
else
    print('旧版本 Lua')
end
");
    }
}
