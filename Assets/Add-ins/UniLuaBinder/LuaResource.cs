using UnityEngine;
using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace Lua
{
    public static class Resource
    {
        /// <summary>
        /// Lua发布代码文件夹
        /// </summary>
        static readonly string kLuaScriptPath = "/auL";
        /// <summary>
        /// Lua调试代码文件夹
        /// </summary>
        static readonly string kLuaScriptDebugPath = "/Lua";

#if UNITY_EDITOR
        static void CreatePath(string path)
        {
            if(Directory.Exists(path))
                return;
            CreatePath(Path.GetDirectoryName(path));
            Directory.CreateDirectory(path);
        }

        public static void Deploy()
        {
            var debug_path = Application.dataPath + kLuaScriptDebugPath;
            var resources_path = Application.dataPath + "/Resources";
            var deploy_path = resources_path + kLuaScriptPath;

            if(Directory.Exists(deploy_path))
            {
                DirectoryInfo deploy_di = new DirectoryInfo(deploy_path);
                deploy_di.Delete(true);
            }

            FileInfo[] fileinfos = null;
            DirectoryInfo di = new DirectoryInfo(debug_path);
            var debug_path_length = di.FullName.Length;
            if(di != null)
                fileinfos = di.GetFiles("*", SearchOption.AllDirectories);

            foreach(var fi in fileinfos)
            {
                var suffix = Path.GetExtension(fi.FullName);
                if(suffix.ToLower() != ".lua")
                    continue;
                var filepath = fi.FullName.Substring(debug_path_length);
                filepath = Path.GetDirectoryName(filepath);
                var filename = Path.GetFileName(fi.FullName);
                filename = Path.GetFileNameWithoutExtension(filename);
                byte[] bytes = null;
                using(var s = new StreamReader(fi.FullName))
                {
                    var b = s.ReadToEnd();
                    bytes = LZMA.Compress(Encoding.UTF8.GetBytes(b));
                }
                var path = deploy_path + "/" + filepath;
                DeployFile(path, filename, bytes);
            }
        }

        static void DeployFile(string path, string name, byte[] bytes)
        {
            CreatePath(path);
            using(var s = new FileStream(path + "/" + name + ".bytes",
                                            FileMode.Create))
            {
                s.Write(bytes, 0, bytes.Length);
            }
        }
#endif

        public static string Load(string name)
        {
            string script = null;

            var index = name.IndexOf("(Clone)");
            if(index >= 0)
                name = name.Substring(0, index);

#if UNITY_EDITOR
            string filename = Application.dataPath + kLuaScriptDebugPath + "/" + name + ".lua";
            try
            {
                var fs = new StreamReader(filename, Encoding.UTF8);
                script = fs.ReadToEnd();
                fs.Close();
                fs.Dispose();
            }
            catch(System.Exception e)
            { }
#else
        var ta = ResourceUtils.Load<TextAsset>(kLuaScriptPath + "/" + name);
        if(ta == null)
        {
            script_cache_[name] = null;
            return null;
        }
        var bytes = LZMA.Decompress(ta.bytes);
        script = Encoding.UTF8.GetString(bytes);
#endif
            return script;
        }
    }
}