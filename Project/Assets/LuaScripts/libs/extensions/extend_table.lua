----------------------------------------
-- extend_table.lua
----------------------------------------
-- 0.1
-- add table.remove_value
----------------------------------------

---删除表内指定数值 (有多个时只删除第一个)
---@return boolean, integer - 删除成功, 表内序列
function table.remove_value(list, value)
	for index = #list, 1, -1 do
		if list[index] == value then
			table.remove(list, index)
			return true, index
		end
	end
	return false, -1
end
