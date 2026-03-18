using System;
using System.IO;
using UnityEngine;
using XLua;

// https://github.com/Tencent/xLua/blob/master/Assets/XLua/Examples/02_U3DScripting/LuaBehaviour.cs

[LuaCallCSharp]
public class XLuaManager : MonoBehaviour
{
    private static LuaEnv luaEnv = new LuaEnv();

    private LuaTable scriptScopeTable;
    public Injection[] injections;

    private Action luaStart;
    private Action luaUpdate;
    private Action luaOnDestroy;

    private static float lastGCTime = 0;
    private const float GCInterval = 1f;

    [SerializeField] private bool dontDestroyOnLoad = true; // 是否跨场景持久化

    private void InitLuaEnv()
    {
        scriptScopeTable = luaEnv.NewTable();

        using (LuaTable meta = luaEnv.NewTable())
        {
            meta.Set("__index", luaEnv.Global);
            scriptScopeTable.SetMetaTable(meta);
        }

        scriptScopeTable.Set("self", this);
        foreach (var injection in injections)
        {
            scriptScopeTable.Set(injection.name, injection.value);
        }

        luaEnv.AddLoader(CustomLuaLoader);
        luaEnv.DoString("require 'main'", "XLuaManager", scriptScopeTable);
        Debug.Log("Lua 环境初始化成功，当前版本: " + luaEnv.DoString("return _VERSION")[0]);

        Action luaAwake = scriptScopeTable.Get<Action>("awake");
        scriptScopeTable.Get("start", out luaStart);
        scriptScopeTable.Get("update", out luaUpdate);
        scriptScopeTable.Get("ondestroy", out luaOnDestroy);

        luaAwake?.Invoke();
    }

    /// <summary>
    /// 尝试加载指定路径的 Lua 文件，并设置 filepath 为真实路径（用于调试跳转）
    /// </summary>
    /// <param name="filepath">ref 参数，会被修改为实际找到的文件路径</param>
    /// <param name="fullPath">要尝试的完整文件路径</param>
    /// <param name="buffer">输出：加载到的字节数组，失败为 null</param>
    /// <returns>是否成功加载</returns>
    private bool TryLoadLua(ref string filepath, string fullPath, out byte[] buffer)
    {
        buffer = null;

        if (!File.Exists(fullPath))
            return false;

        try
        {
            filepath = fullPath;
            buffer = File.ReadAllBytes(fullPath);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"读取 Lua 文件失败: {fullPath} → {ex.Message}");
            return false;
        }
    }
    
#if UNITY_ANDROID && !UNITY_EDITOR
    /// <summary>
    /// Android jar 协议加载辅助函数（同步方式）
    /// </summary>
    private bool TryLoadFromAndroidUrl(string url, ref string filepath, out byte[] buffer)
    {
        buffer = null;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            var asyncOp = www.SendWebRequest();

            // 同步等待（开发阶段可接受，生产建议改异步）
            while (!asyncOp.isDone) { }

            if (www.result == UnityWebRequest.Result.Success)
            {
                filepath = url;  // 设置为 jar url，便于日志追踪
                buffer = www.downloadHandler.data;
                return true;
            }
            else
            {
                Debug.LogWarning($"Android jar 加载失败: {url} - {www.error}");
                return false;
            }
        }
    }
#endif
    
    /// <summary>
    /// 自定义 Lua 文件加载器，支持热更、首包、开发阶段多路径 fallback
    /// 支持模块目录下的 init.lua
    /// </summary>
    private byte[] CustomLuaLoader(ref string filepath)
    {
        try
        {
            string originalPath = filepath; // 原始的 'a.b.c'
            
            string scriptPath;
            byte[] buffer;
            
            // 修复路径
            filepath = filepath.Replace('.', Path.DirectorySeparatorChar);

            // 1. 热更目录（最高优先级）
            scriptPath = Path.Combine(Application.persistentDataPath, "lua/" + filepath + ".lua");
            if (TryLoadLua(ref filepath, scriptPath, out buffer)) return buffer;

            scriptPath = Path.Combine(Application.persistentDataPath, "lua/" + filepath + "/init.lua");
            if (TryLoadLua(ref filepath, scriptPath, out buffer)) return buffer;

            // 2. StreamingAssets（首包内置）
            scriptPath = Path.Combine(Application.streamingAssetsPath, "lua/" + filepath + ".lua");
            if (TryLoadLua(ref filepath, scriptPath, out buffer)) return buffer;

            scriptPath = Path.Combine(Application.streamingAssetsPath, "lua/" + filepath + "/init.lua");
            if (TryLoadLua(ref filepath, scriptPath, out buffer)) return buffer;

            // 3. Android 特殊处理（apk 内 StreamingAssets 使用 jar 协议）
#if UNITY_ANDROID && !UNITY_EDITOR
            string androidUrl = "jar:file://" + Application.dataPath + "!/assets/lua/" + filepath + ".lua";
            if (TryLoadFromAndroidUrl(androidUrl, ref filepath, out buffer)) return buffer;

            androidUrl = "jar:file://" + Application.dataPath + "!/assets/lua/" + filepath + "/init.lua";
            if (TryLoadFromAndroidUrl(androidUrl, ref filepath, out buffer)) return buffer;
#endif

            // 4. 开发阶段 fallback（Editor 下优先读 Assets/LuaScripts）
#if UNITY_EDITOR
            scriptPath = Path.Combine(Application.dataPath, "LuaScripts/" + filepath + ".lua");
            if (TryLoadLua(ref filepath, scriptPath, out buffer)) return buffer;

            scriptPath = Path.Combine(Application.dataPath, "LuaScripts/" + filepath + "/init.lua");
            if (TryLoadLua(ref filepath, scriptPath, out buffer)) return buffer;
#endif

            Debug.LogWarning($"未找到 Lua 文件: {filepath}");
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"LuaLoader 异常: {filepath} - {ex.Message}\n{ex.StackTrace}");
            return null;
        }
    }

    private void Awake()
    {
        // 避免多个实例
        if (FindObjectsOfType<XLuaManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        InitLuaEnv();
    }

    private void Start()
    {
        luaStart?.Invoke();
    }

    private void Update()
    {
        // Debug.Log("XLuaManager Update");

        luaUpdate?.Invoke();

        // 定时GC 移动端性能更优
        if (Time.time - lastGCTime > GCInterval)
        {
            luaEnv.Tick(); // 内部会处理 GC 等
            lastGCTime = Time.time;
        }
    }

    private void OnDestroy()
    {
        luaOnDestroy?.Invoke();

        scriptScopeTable.Dispose();

        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        luaEnv = null;
        injections = null;
    }
}

[Serializable]
public class Injection
{
    public string name;
    public GameObject value;
}