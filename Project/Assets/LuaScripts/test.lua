local CS            = CS
local GameObject    = CS.UnityEngine.GameObject
local Vector3       = CS.UnityEngine.Vector3
local Time          = CS.UnityEngine.Time
local Transform     = CS.UnityEngine.Transform

-- 存储所有创建的物体
local cubes = {}

utils.msg.on('unity_start', function()
    -- 创建 10 个 Cube
    for i = 1, 10 do
        local go = GameObject.CreatePrimitive(CS.UnityEngine.PrimitiveType.Cube)
        go.name = 'Cube_' .. i

        -- 随机初始位置（x,z 随机，y 从 0 开始）
        local x = math.random(-8, 8)
        local z = math.random(-8, 8)
        go.transform.position = Vector3(x, 0, z)

        -- 随机颜色（可选）
        local renderer = go:GetComponent('Renderer')
        if renderer then
            renderer.material.color = CS.UnityEngine.Color(
                math.random(), math.random(), math.random(), 1
            )
        end

        table.insert(cubes, go.transform)
    end
end)

utils.msg.on('unity_update', function()
    local t = Time.time

    local amplitude = 2.0                        -- 上下振幅（单位：米）
    local frequency = 1.0                        -- 频率（1秒完成一次完整上下）

    for i, trans in ipairs(cubes) do
        local baseY = 0.0 + (i - 1) * 0.5        -- 每个 Cube 初始高度错开一点
        local offsetY = math.sin(t * frequency * 2 * math.pi + i) * amplitude

        local pos = trans.position
        pos.y = baseY + offsetY                  -- 直接设置 y，不累加
        trans.position = pos
    end
end)
