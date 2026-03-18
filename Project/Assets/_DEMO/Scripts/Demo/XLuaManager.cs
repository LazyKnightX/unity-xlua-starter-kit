using System;
using System.IO;
using UnityEngine;
using XLua;

public class XLuaManager : MonoBehaviour
{
    private LuaEnv luaEnv;
    private static float lastGCTime = 0;
    private const float GCInterval = 1f;

    [SerializeField]
    private bool dontDestroyOnLoad = true;  // 是否跨场景持久化

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

    private void InitLuaEnv()
    {
        if (luaEnv != null) return;

        try
        {
            luaEnv = new LuaEnv();

            // 注册自定义 Loader（核心部分）
            luaEnv.AddLoader(CustomLuaLoader);

            // 可选：注册一些常用 C# 类型（根据项目需要）
            // luaEnv.AddAction<Action<string>>("PrintLog", msg => Debug.Log(msg));

            // 启动 Lua 主入口
            luaEnv.DoString("require 'main'");

            Debug.Log("Lua 环境初始化成功，当前版本: " + luaEnv.DoString("return _VERSION")[0]);
        }
        catch (Exception ex)
        {
            Debug.LogError("LuaEnv 初始化失败: " + ex.Message + "\n" + ex.StackTrace);
        }
    }

    /// <summary>
    /// 自定义 Lua 文件加载器，支持热更、首包、开发阶段多路径 fallback
    /// </summary>
    private byte[] CustomLuaLoader(ref string filepath)
    {
        try
        {
            // 1. 热更目录（最高优先级）
            string hotfixPath = Path.Combine(Application.persistentDataPath, "lua/" + filepath + ".lua");
            if (File.Exists(hotfixPath))
            {
                filepath = hotfixPath;  // 修改为真实路径，支持调试跳转
                return File.ReadAllBytes(hotfixPath);
            }

            // 2. StreamingAssets（首包内置）
            string streamPath = Path.Combine(Application.streamingAssetsPath, "lua/" + filepath + ".lua");
            if (File.Exists(streamPath))
            {
                filepath = streamPath;
                return File.ReadAllBytes(streamPath);
            }

            // Android StreamingAssets 在 apk 内（jar 协议）
#if UNITY_ANDROID && !UNITY_EDITOR
            string androidUrl = "jar:file://" + Application.dataPath + "!/assets/lua/" + filepath + ".lua";
            using (UnityWebRequest www = UnityWebRequest.Get(androidUrl))
            {
                var asyncOp = www.SendWebRequest();
                // 注意：这里同步等待，生产环境建议改成异步
                while (!asyncOp.isDone) { }

                if (www.result == UnityWebRequest.Result.Success)
                {
                    filepath = androidUrl;
                    return www.downloadHandler.data;
                }
                else
                {
                    Debug.LogWarning($"Android jar 加载失败: {androidUrl} - {www.error}");
                }
            }
#endif

            // 3. 开发阶段 fallback（Editor 下优先读 Assets/LuaScripts）
#if UNITY_EDITOR
            string editorPath = Path.Combine(Application.dataPath, "LuaScripts/" + filepath + ".lua");
            if (File.Exists(editorPath))
            {
                filepath = editorPath;
                return File.ReadAllBytes(editorPath);
            }
#endif

            Debug.LogWarning($"未找到 Lua 文件: {filepath}");
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"LuaLoader 异常: {filepath} - {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 获取 LuaEnv 实例（供其他脚本调用）
    /// </summary>
    public LuaEnv GetLuaEnv()
    {
        return luaEnv;
    }

    /// <summary>
    /// 执行一段 Lua 字符串（安全封装）
    /// </summary>
    public object[] DoString(string chunk, string chunkName = "chunk")
    {
        if (luaEnv == null) return null;
        try
        {
            return luaEnv.DoString(chunk, chunkName);
        }
        catch (Exception ex)
        {
            Debug.LogError($"DoString 失败: {chunkName}\n{ex.Message}\n{ex.StackTrace}");
            return null;
        }
    }

    private void OnDestroy()
    {
        if (luaEnv != null)
        {
            try
            {
                luaEnv.Dispose();
            }
            catch (Exception ex)
            {
                Debug.LogError("LuaEnv Dispose 异常: " + ex.Message);
            }
            luaEnv = null;
        }
    }

    private void Update()
    {
        if (luaEnv != null)
        {
            // 定时GC 移动端性能更优
            if (Time.time - lastGCTime > GCInterval)
            {
                luaEnv.Tick();  // 内部会处理 GC 等
                lastGCTime = Time.time;
            }
        }
    }
}