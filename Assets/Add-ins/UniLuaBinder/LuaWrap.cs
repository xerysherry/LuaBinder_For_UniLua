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
}
