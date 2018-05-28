using System;
using System.Collections;
using System.Collections.Generic;

public static partial class LuaWrap
{
    public static void OpenLib(Lua.State state)
    {
        var type = typeof(LuaWrap);
        var method = type.GetMethod("InitMetatable", 
            System.Reflection.BindingFlags.Public|
            System.Reflection.BindingFlags.Static);
        if(method == null)
            return;
        method.Invoke(null, new object[] { state });
    }
    private static void Index(Lua.Param param)
    {
        var userdata = param.GetUserdata(1);
        var index = param.GetString(2);
        var type = userdata.GetType();

        Lua.Metatable m = null;
        while(metatables.TryGetValue(type, out m))
        {
            var r = m.Get(index);
            if(r != null)
            {
                param.Return(r);
                return;
            }
            type = m.parent;
        }
        param.Return();
    }
    private static Dictionary<System.Type, Lua.Metatable> metatables;
}
