using System;
using UnityEngine;

/// <summary>
/// ManualBehavior：
/// </summary>
public class ManualBehavior : MonoBehaviour 
{
    public bool created { get; private set; }
    public bool destroyed { get; private set; }
    public bool inited { get; private set; }
    public bool active { get; private set; }
    public object data { get; private set; }

    public string updateProfiler = "";
    public string lateUpdateProfiler = "";

    [NonSerialized] public bool unsafeGameobjectState;

    // 创建对象
    public void _Create()
    {
        if (!created && !destroyed)
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
            created = true;
            try { _OnCreate(); }
            catch (Exception e) { Debug.LogError(e); }
        }
    }

    // 销毁对象
    public void _Destroy()
    {
        if (created)
        {
            _Free();
            created = false;
            destroyed = true;
            try { _OnDestroy(); }
            catch (Exception e) { Debug.LogError(e); }
            GameObject.Destroy(this.gameObject);
        }
    }

    // 初始化对象
    public void _Init(object _data)
    {
        if (created && !inited)
        {
            data = _data;
            bool ok = false;
            try { ok = _OnInit(); }
            catch (Exception e) { ok = false; Debug.LogError(e); }
            if (ok)
            {
                inited = true;
                try { _OnRegEvent(); }
                catch (Exception e) { Debug.LogError(e); }
            }
            else
            {
                inited = false;
                data = null;
            }
        }
    }

    // 释放对象
    public void _Free()
    {
        if (inited)
        {
            _Close();

            try { _OnUnregEvent(); }
            catch (Exception e) { Debug.LogError(e); }

            try { _OnFree(); }
            catch (Exception e) { Debug.LogError(e); }

            inited = false;
            data = null;
        }
    }

    // 开启对象
    public void _Open()
    {
        if (inited && !active)
        {
            active = true;
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            try { _OnOpen(); }
            catch (Exception e) { Debug.LogError(e); }
        }
    }

    // 关闭对象
    public void _Close()
    {
        if (active)
        {
            active = false;

            try { _OnClose(); }
            catch (Exception e) { Debug.LogError(e); }

            if (gameObject.activeSelf)
                gameObject.SetActive(false);
        }
    }

    // 更新对象
    public void _Update()
    {
        if (active)
        {
            if (!unsafeGameobjectState)
            {
                if (gameObject.activeInHierarchy)
                {
#if UNITY_EDITOR
                    Debug.LogWarning("该对象已被关闭，但仍尝试手动更新");
#endif  
                    return;
                }
            }

#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(updateProfiler))
                UnityEngine.Profiling.Profiler.BeginSample(updateProfiler);
#endif

            try { _OnUpdate(); }
            catch (Exception e) { Debug.LogError(e); }

#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(updateProfiler))
                UnityEngine.Profiling.Profiler.EndSample();
#endif
        }
    }

    // 后更新对象
    public void _LateUpdate()
    {
        if (active)
        {
            if (!unsafeGameobjectState)
            {
                if (gameObject.activeInHierarchy)
                {
#if UNITY_EDITOR
                    Debug.LogWarning("该对象已被关闭，但仍尝试手动更新");
#endif  
                    return;
                }
            }

#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(updateProfiler))
                UnityEngine.Profiling.Profiler.BeginSample(updateProfiler);
#endif

            try { _OnLateUpdate(); }
            catch (Exception e) { Debug.LogError(e); }

#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(updateProfiler))
                UnityEngine.Profiling.Profiler.EndSample();
#endif
        }
    }

    protected virtual void _OnCreate() { }
    protected virtual void _OnDestroy() { }
    protected virtual bool _OnInit() { return true; }
    protected virtual void _OnFree() { }
    protected virtual void _OnRegEvent() { }
    protected virtual void _OnUnregEvent() { }
    protected virtual void _OnOpen() { }
    protected virtual void _OnClose() { }
    protected virtual void _OnUpdate() { }
    protected virtual void _OnLateUpdate() { }
}
