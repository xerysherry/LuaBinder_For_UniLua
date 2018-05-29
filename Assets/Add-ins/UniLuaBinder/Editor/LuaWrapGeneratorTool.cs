using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LuaTool : EditorWindow
{
    [MenuItem("Tools/LuaWrapGenerator", false, 100)]
    static void OpenWindow()
    {
        // Get existing open window or if none, make a new one:
        LuaTool.GetWindow(typeof(LuaTool));
    }

    void OnGUI()
    {
        if(GUILayout.Button("Generate"))
        {
            Generate();
        }
        if(GUILayout.Button("Deploy"))
        {
            Lua.Resource.Deploy();
            AssetDatabase.Refresh();
        }
    }

    void Generate()
    {
        if(EditorApplication.isCompiling)
        {
            EditorUtility.DisplayDialog("Please Wait Compile Finish",
                                        "Please Wait Compile Finish", "OK");
            return;
        }

        LuaWrapGenerator.ClearClass();
        LuaWrapGenerator.SetSkipClass(new List<System.Type> 
        {
            typeof(Motion),
        });
        LuaWrapGenerator.SetSkipMethod(new List<string>
        {
            "UnityEngine.Texture2D.alphaIsTransparency",
            "UnityEngine.UI.Graphic.OnRebuildRequested",
            "UnityEngine.UI.Text.OnRebuildRequested",
            "UnityEngine.Camera.scene",
        });
        LuaWrapGenerator.Create(new List<System.Type>
        {
            typeof(LuaUtils),
            typeof(LuaMono),

            // Unity Class
            typeof(UnityEngine.Animation),
            typeof(UnityEngine.AnimationBlendMode),
            typeof(UnityEngine.AnimationClip),
            typeof(UnityEngine.AnimationCullingType),
            typeof(UnityEngine.Animator),
            typeof(UnityEngine.AssetBundle),
            typeof(UnityEngine.Camera),
            typeof(UnityEngine.Canvas),
            typeof(UnityEngine.Color),
            typeof(UnityEngine.Color32),
            typeof(UnityEngine.Debug),
            typeof(UnityEngine.GameObject),
            typeof(UnityEngine.Gizmos),
            typeof(UnityEngine.GUI),
            typeof(UnityEngine.GUILayout),
            typeof(UnityEngine.Input),
            typeof(UnityEngine.Material),
            typeof(UnityEngine.Mathf),
            typeof(UnityEngine.Matrix4x4),
            typeof(UnityEngine.Mesh),
            typeof(UnityEngine.MeshFilter),
            typeof(UnityEngine.MonoBehaviour),
            typeof(UnityEngine.ParticleSystem),
            typeof(UnityEngine.PlayerPrefs),
            typeof(UnityEngine.PlayMode),
            typeof(UnityEngine.QualitySettings),
            typeof(UnityEngine.Quaternion),
            typeof(UnityEngine.QueueMode),
            typeof(UnityEngine.Random),
            typeof(UnityEngine.Ray),
            typeof(UnityEngine.Ray2D),
            typeof(UnityEngine.Rect),
            typeof(UnityEngine.RectTransform),
            typeof(UnityEngine.RectTransformUtility),
            typeof(UnityEngine.Renderer),
            typeof(UnityEngine.RenderMode),
            typeof(UnityEngine.RenderTexture),
            typeof(UnityEngine.RenderTextureFormat),
            typeof(UnityEngine.RenderTextureReadWrite),
            typeof(UnityEngine.Screen),
            typeof(UnityEngine.Shader),
            typeof(UnityEngine.Sprite),
            typeof(UnityEngine.SpriteMeshType),
            typeof(UnityEngine.SpriteRenderer),
            typeof(UnityEngine.SystemInfo),
            typeof(UnityEngine.Texture),
            typeof(UnityEngine.Texture2D),
            typeof(UnityEngine.Texture3D),
            typeof(UnityEngine.TextureFormat),
            typeof(UnityEngine.TextureWrapMode),
            typeof(UnityEngine.Time),
            typeof(UnityEngine.Touch),
            typeof(UnityEngine.Transform),
            typeof(UnityEngine.Vector2),
            typeof(UnityEngine.Vector3),
            typeof(UnityEngine.Vector4),

            typeof(UnityEngine.EventSystems.EventSystem),
            typeof(UnityEngine.EventSystems.PointerEventData),
            typeof(UnityEngine.EventSystems.PointerInputModule),
            typeof(UnityEngine.EventSystems.UIBehaviour),
            typeof(UnityEngine.UI.Button),
            typeof(UnityEngine.UI.Graphic), 
            typeof(UnityEngine.UI.Image),
            typeof(UnityEngine.UI.InputField),
            typeof(UnityEngine.UI.Mask),
            typeof(UnityEngine.UI.RawImage),
            typeof(UnityEngine.UI.Scrollbar),
            typeof(UnityEngine.UI.ScrollRect),
            typeof(UnityEngine.UI.Slider),
            typeof(UnityEngine.UI.Text),
        });

        LuaWrapGenerator.CreateWrapClassScript();
        AssetDatabase.Refresh();
    }
}
