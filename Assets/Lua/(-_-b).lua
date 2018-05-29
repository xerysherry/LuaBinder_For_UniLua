-----------------------------------------------------------
-- print
local UnityDebug = UnityEngine.Debug
local LogError = UnityDebug.LogError
local LogWarning = UnityDebug.LogWarning
local _print = print
local tconcat = table.concat
local tinsert = table.insert
local tremove = table.remove

ToLuaObj = LuaUtils.ToLuaObj
ToObj = LuaUtils.ToObj
ToString = LuaUtils.ToString
ToType = LuaUtils.ToType
GetType = LuaUtils.GetType

function print(...)
    local text = ""
    for _, v in ipairs({...}) do
        text = text .. ToString(v) .. "\t"
    end
	--local text = tconcat({...})
    _print(text)
end

function printError(...)
    local text = ""
    for _, v in ipairs({...}) do
        text = text .. ToString(v) .. "\t"
    end
	--local text = tconcat({...})
    LogError(text)
end

function printWarning(...)
    local text = ""
    for _, v in ipairs({...}) do
        text = text .. ToString(v) .. "\t"
    end
	--local text = tconcat({...})
    LogWarning(text)
end

-----------------------------------------------------------
-- coroutine
function WaitUntil(pred)
	while not pred()  do
		coroutine.yield();
	end
end

function WaitFrame(count)
    if not count or count==1 then
        coroutine.yield()
    else
	    for i = 1,count do
		    coroutine.yield()
	    end
    end
end

local UnityTime = UnityEngine.Time
local GetUnscaledTime = UnityTime.GetUnscaledTime
local GetTime = UnityTime.GetTime

function WaitSecond(t)
    local r = GetTime() + t
    while r > GetTime() do
        coroutine.yield()
    end
end

function WaitUnscaledSecond(t)
    local r = GetUnscaledTime() + t
    while r > GetUnscaledTime() do
        coroutine.yield()
    end
end

-----------------------------------------------------------
-- signal
local signal = 
{
    send = function(this, func, delay)
        tinsert(this.list, {func, delay or 0})
    end,
    emit = function(this)
        local next_time = GetTime()
        local delta_time = next_time - this.time
        local list = this.list

        local length = #list
        if length > 0 then

            for i=1, length do
                if i > length then
                    break;
                end

                local v = list[i]
                local r = false
                if not v[1] then
                    r = true
                else
                    v[2] = v[2]-delta_time
                    if v[2] <= 0 then
                        r = true
                    end
                end

                if r then
                    if v[1] then
                        v[1]()
                    end
                    list[i] = list[length]
                    list[length] = nil
                    i = i - 1
                    length = length - 1
                end
            end

            --for i=length-remove+1, length do
            --    local v = list[i]
            --    if v[1] then
            --        v[1]()
            --    end
            --    list[i] = nil
            --end
        end

        this.time = next_time
    end,
    time = GetTime(),
    list = {},
}

-----------------------------------------------------------
--[[ 延迟函数
	func：函数
	delay: 延迟时间（单位秒）
]]
function Delay(func, delay)
	signal:send(func, delay)
end

-----------------------------------------------------------
-- Mono
local tinsert = table.insert
local tremove = table.remove
local _MonoBase = 
{
    StartCoroutine = function(self, func)
        if self.coroutines == nil then
            self.coroutines = {}
        end
        local co = coroutine.create(func)
        table.insert(self.coroutines, co)
        return co
    end,
    StopCoroutine = function(self, co)
        if self.coroutines == nil then
            return
        end
        local f
        for i, v in ipairs(self.coroutines) do
            if v == co then
                f = i
                break
            end
        end
        if f then
           
        end
    end,
    StopAllCoroutine = function(self)
        self.coroutines = nil
    end,
    UpdateCoroutine = function(self)
        if self.coroutines == nil then
            return
        end
        for _, v in pairs(self.coroutines) do
            local cr, ur = coroutine.resume(v)
            if (not cr) or ur then
                if ur ~= "cannot resume dead coroutine" then
                    -- coroutine error text
                    printError("Coroutine Error : \x0d\x0a" .. ur)
                    self.coroutines[v] = nil
                end
            end
        end
    end,
    -- coroutine list
    coroutines = nil,
}

MonoBase = setmetatable(_MonoBase, {
    __call = function(self, o)
        local newinstance = o or {}
        newinstance = setmetatable(newinstance, {
            __index = _MonoBase
        })
        newinstance.StartCoroutine = function(func)
            return _MonoBase.StartCoroutine(newinstance, func)
        end
        newinstance.StopCoroutine = function(co)
            _MonoBase.StopCoroutine(newinstance, co)
        end
        newinstance.StopAllCoroutine = function()
            _MonoBase.StopAllCoroutine(newinstance)
        end
        newinstance.UpdateCoroutine = function()
            _MonoBase.UpdateCoroutine(newinstance)
            signal:emit()
        end
        newinstance.SetGameObject = function(gameObject)
            _MonoBase.gameObject = gameObject
        end
        newinstance.SetMonoBehaviour = function(mono)
            _MonoBase.luaMono = mono
        end
        return newinstance
    end,
})
